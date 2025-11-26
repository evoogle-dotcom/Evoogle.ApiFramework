// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Coercion;
using Evoogle.Extension;
using Evoogle.Extensions;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents structural metadata of an API property belonging to an <see cref="ApiObjectType"/>.
///     Each property corresponds to a named data element in an API contract.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiProperty"/> captures type-level structure for fields such as primitives,
///         objects, collections, and complex types. Properties may be referenced by one or more
///         <see cref="ApiRelationship"/> instances to convey semantic meaning (e.g., parent-child, references).
///     </para>
///     <para>
///         For optimal performance, getter and setter accessors are compiled once during initialization
///         as lambda expressions using the owning <see cref="ApiObjectType"/>'s CLR type. The compiled
///         delegates are stored directly on each <see cref="ApiProperty"/> instance and provide near-native
///         performance (~10-20x faster than reflection) for property/field access operations.
///     </para>
///     <para>
///         The non-generic Try* methods accept <see cref="object"/> and will box value types by design. Prefer
///         the fully generic overloads to avoid boxing both the target instance and the returned value.
///     </para>
/// </remarks>
/// <param name="apiName">The API name of the property.</param>
/// <param name="apiTypeExpression">The API type expression of the property.</param>
/// <param name="apiTypeModifiers">Modifiers applied to the property (e.g., Required).</param>
/// <param name="clrName">The CLR property name corresponding to this API property.</param>
[JsonConverter(typeof(ApiPropertyJsonConverter))]
public sealed partial class ApiProperty(string apiName, ApiTypeExpression apiTypeExpression, ApiTypeModifiers apiTypeModifiers, string clrName) : ExtensibleBase
{
    #region Types
    private delegate void ByRefAction<TObject, in TValue>(ref TObject clrObject, ApiSchemaContext apiSchemaContext, TValue? clrValue);

    private readonly record struct ClrCacheKey(Type ClrObjectType, string ClrMemberName);

    private readonly record struct ClrGetterCacheValue<TObject, TValue>(Func<TObject, ApiSchemaContext, TValue?>? ClrGetter);

    private readonly record struct ClrSetterCacheValue<TObject, TValue>(Action<TObject, ApiSchemaContext, TValue?>? ClrSetter);

    private readonly record struct ClrSetterByRefCacheValue<TObject, TValue>(ByRefAction<TObject, TValue?>? ClrSetterByRef)
        where TObject : struct;

    private static class ClrGetterCache<TObject, TValue>
    {
        public static readonly ConcurrentDictionary<ClrCacheKey, ClrGetterCacheValue<TObject, TValue>> Cache = new();
    }

    private static class ClrSetterCache<TObject, TValue>
    {
        public static readonly ConcurrentDictionary<ClrCacheKey, ClrSetterCacheValue<TObject, TValue>> Cache = new();
    }

    private static class ClrSetterByRefCache<TObject, TValue>
        where TObject : struct
    {
        public static readonly ConcurrentDictionary<ClrCacheKey, ClrSetterByRefCacheValue<TObject, TValue>> Cache = new();
    }

    private readonly record struct CoerceMethodCacheKey(Type ClrInputType, Type ClrOutputType);
    #endregion

    #region Fields
    private ApiSchemaContext? _apiSchemaContext = null;

    private Func<object, ApiSchemaContext, object?>? _clrGetter;

    private Action<object, ApiSchemaContext, object?>? _clrSetter;

    private static readonly ConcurrentDictionary<CoerceMethodCacheKey, MethodInfo> _coerceMethodCache = new();

    private static readonly MethodInfo? _genericCoerceMethodDefinition = TypeReflection.GetGenericMethodDefinition
    (
        type: typeof(TypeCoercion),
        methodName: nameof(TypeCoercion.Coerce),
        bindingFlags: BindingFlags.Public | BindingFlags.Instance,
        parameterCount: 2
    );

    private static readonly MethodInfo? _nonGenericCoerceMethod = TypeReflection.GetMethod
    (
        type: typeof(TypeCoercion),
        methodName: nameof(TypeCoercion.Coerce),
        bindingFlags: BindingFlags.Public | BindingFlags.Instance,
        parameterTypes: [typeof(object), typeof(Type), typeof(TypeCoercionContext)]
    );
    #endregion

    #region Properties
    /// <summary>Gets the API name of the property (used in API requests/responses).</summary>
    public string ApiName { get; } = apiName;

    /// <summary>Gets the API type of the property.</summary>
    public ApiType ApiType => this.ApiTypeExpression.ApiType;

    /// <summary>Gets the modifiers applied to this property (e.g., Required).</summary>
    public ApiTypeModifiers ApiTypeModifiers { get; } = apiTypeModifiers;

    /// <summary>Gets the CLR name of the property (matching the C# property name).</summary>
    public string ClrName { get; } = clrName;

    /// <summary>
    ///     Gets the API type expression to the API type of this property.
    ///     May point to a named type or inline type (e.g., collection).
    /// </summary>
    internal ApiTypeExpression ApiTypeExpression { get; } = apiTypeExpression;

    /// <summary>Gets the schema context for this property.</summary>
    internal ApiSchemaContext ApiSchemaContext => this.ThrowIfNotInitialized(_apiSchemaContext);

    /// <summary>Gets the cached generic method definition for TypeCoercion.Coerce&lt;TInput, TOutput&gt;.</summary>
    private static MethodInfo GenericCoerceMethodDefinition => _genericCoerceMethodDefinition
        ?? throw new InvalidOperationException($"Failed to locate generic method definition for {nameof(TypeCoercion)}.{nameof(TypeCoercion.Coerce)}.");

    /// <summary>Gets the cached non-generic method for TypeCoercion.Coerce(object, Type, TypeCoercionContext).</summary>
    private static MethodInfo NonGenericCoerceMethod => _nonGenericCoerceMethod
        ?? throw new InvalidOperationException($"Failed to locate non-generic method for {nameof(TypeCoercion)}.{nameof(TypeCoercion.Coerce)}.");
    #endregion

    #region Computed Properties
    /// <summary>Gets a value indicating whether this property is optional (not required).</summary>
    public bool IsOptional => !this.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required);

    /// <summary>Gets a value indicating whether this property is required.</summary>
    public bool IsRequired => this.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiTypeExpression = this.ApiTypeExpression.SafeToString();
        var apiTypeModifiers = this.ApiTypeModifiers.SafeToString();
        var clrName = this.ClrName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiProperty)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiTypeExpression)}={apiTypeExpression}, {nameof(this.ApiTypeModifiers)}={apiTypeModifiers}, {nameof(this.ClrName)}={clrName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion
}
