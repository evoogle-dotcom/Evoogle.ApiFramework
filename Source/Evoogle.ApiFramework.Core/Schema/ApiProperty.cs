// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Coercion;
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
public sealed partial class ApiProperty(string apiName, ApiTypeExpression apiTypeExpression, ApiTypeModifiers apiTypeModifiers, string clrName) : ApiSchemaElement
{
    #region Types
    private delegate void ClrByRefAction<TObject, in TValue>(ref TObject clrObject, ApiSchemaContext apiSchemaContext, TValue? clrValue);

    private readonly record struct ClrCacheKey(Type ClrObjectType, string ClrMemberName);

    private readonly record struct ClrGetterCacheValue<TObject, TValue>(Func<TObject, ApiSchemaContext, TValue?>? ClrGetter);

    private readonly record struct ClrSetterCacheValue<TObject, TValue>(Action<TObject, ApiSchemaContext, TValue?>? ClrSetter);

    private readonly record struct ClrSetterByRefCacheValue<TObject, TValue>(ClrByRefAction<TObject, TValue?>? ClrSetterByRef)
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

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiParentPath)
        => ApiSchemaHelpers.BuildPath(apiParentPath, apiChildPath: nameof(ApiProperty), apiApiName: this.ApiName);

    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiName(context);
        this.InitializeApiTypeExpression(context);
        this.InitializeClrName(context);
        this.InitializeClrGetterAndSetter(context);
    }

    private void InitializeApiName(ApiInitializationContext context)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiName)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_API_NAME;
            var description = $"{nameof(this.ApiName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeApiTypeExpression(ApiInitializationContext context)
    {
        if (this.ApiTypeExpression is null)
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiType)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_NULL_TYPE;
            var description = $"{nameof(this.ApiType)} must not be null";
            var remediation = $"Specify a valid {nameof(this.ApiType)}";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        var childContext = context.WithParentSchemaElement(this);
        this.ApiTypeExpression.InitializeForProperty(childContext);
    }

    private void InitializeClrFieldGetterAndSetter(ApiInitializationContext context, FieldInfo clrFieldInfo)
    {
        var apiObjectType = context.ApiParentObjectType;
        var clrObjectType = apiObjectType.ClrType;
        var clrMemberName = this.ClrName;

        // Build compiled field getter and setter
        try
        {
            _clrGetter = BuildNonGenericClrFieldGetter(clrObjectType, clrFieldInfo);
        }
        catch (Exception ex)
        {
            var path = $"{this.ApiPath}.{nameof(this.ClrName)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_FIELD_GETTER;
            var description = $"Failed to compile field getter for '{clrMemberName}': {ex.Message}";
            var remediation = $"Verify that field '{clrMemberName}' on {nameof(ApiObjectType)}.{nameof(ApiObjectType.ClrType)} '{clrObjectType.SafeToName()}' is accessible";

            context.AddIssue(path, severity, code, description, remediation);
        }

        try
        {
            _clrSetter = BuildNonGenericClrFieldSetter(clrObjectType, clrFieldInfo);
        }
        catch (Exception ex)
        {
            var path = $"{this.ApiPath}.{nameof(this.ClrName)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_FIELD_SETTER;
            var description = $"Failed to compile field setter for '{clrMemberName}': {ex.Message}";
            var remediation = $"Verify that field '{clrMemberName}' on {nameof(ApiObjectType)}.{nameof(ApiObjectType.ClrType)} '{clrObjectType.SafeToName()}' is accessible and writable";

            context.AddIssue(path, severity, code, description, remediation);
        }

        return; // Found valid field
    }

    private void InitializeClrPropertyGetterAndSetter(ApiInitializationContext context, PropertyInfo clrPropertyInfo)
    {
        var apiObjectType = context.ApiParentObjectType;
        var clrObjectType = apiObjectType.ClrType;
        var clrMemberName = this.ClrName;

        // Exclude indexers
        if (clrPropertyInfo.GetIndexParameters().Length > 0)
        {
            var path = $"{this.ApiPath}.{nameof(this.ClrName)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_PROPERTY_GETTER;
            var description = $"Property '{clrMemberName}' is an indexer, which is not supported";

            context.AddIssue(path, severity, code, description, remediation: null);
            return;
        }

        // Build compiled property getter and setter
        try
        {
            _clrGetter = BuildNonGenericClrPropertyGetter(clrObjectType, clrPropertyInfo);
        }
        catch (Exception ex)
        {
            var path = $"{this.ApiPath}.{nameof(this.ClrName)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_PROPERTY_GETTER;
            var description = $"Failed to compile property getter for '{clrMemberName}': {ex.Message}";
            var remediation = $"Verify that property '{clrMemberName}' on {nameof(ApiObjectType)}.{nameof(ApiObjectType.ClrType)} '{clrObjectType.SafeToName()}' is accessible";

            context.AddIssue(path, severity, code, description, remediation);
        }

        try
        {
            _clrSetter = BuildNonGenericClrPropertySetter(clrObjectType, clrPropertyInfo);
        }
        catch (Exception ex)
        {
            var path = $"{this.ApiPath}.{nameof(this.ClrName)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_PROPERTY_SETTER;
            var description = $"Failed to compile property setter for '{clrMemberName}': {ex.Message}";
            var remediation = $"Verify that property '{clrMemberName}' on {nameof(ApiObjectType)}.{nameof(ApiObjectType.ClrType)} '{clrObjectType.SafeToName()}' is accessible and writable";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeClrGetterAndSetter(ApiInitializationContext context)
    {
        var apiObjectType = context.ApiParentObjectType;
        var clrObjectType = apiObjectType.ClrType;
        var clrMemberName = this.ClrName;

        try
        {
            // Prefer property, then field
            var clrPropertyInfo = TypeReflection.GetProperty(clrObjectType, clrMemberName, BindingFlags.Public | BindingFlags.Instance);
            if (clrPropertyInfo is not null)
            {
                this.InitializeClrPropertyGetterAndSetter(context, clrPropertyInfo);
                return; // Found valid property
            }

            var clrFieldInfo = TypeReflection.GetField(clrObjectType, clrMemberName, BindingFlags.Public | BindingFlags.Instance);
            if (clrFieldInfo is not null)
            {
                this.InitializeClrFieldGetterAndSetter(context, clrFieldInfo);
                return; // Found valid field
            }

            // Member not found
            var path = $"{this.ApiPath}.{nameof(this.ClrName)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_MISSING_CLR_MEMBER;
            var description = $"Member '{clrMemberName}' was not found on {nameof(ApiObjectType)}.{nameof(ApiObjectType.ClrType)} '{clrObjectType.SafeToName()}'";
            var remediation = $"Add a public property or field named '{clrMemberName}' to {nameof(ApiObjectType)}.{nameof(ApiObjectType.ClrType)} '{clrObjectType.SafeToName()}'";

            context.AddIssue(path, severity, code, description, remediation);
        }
        catch (Exception ex)
        {
            var path = $"{this.ApiPath}.{nameof(this.ClrName)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_CLR_MEMBER;
            var description = $"Failed to compile getter or setter accessor for '{clrMemberName}': {ex.Message}";
            var remediation = $"Verify that '{clrMemberName}' exists as a public property or field on {nameof(ApiObjectType)}.{nameof(ApiObjectType.ClrType)} '{clrObjectType.SafeToName()}'";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeClrName(ApiInitializationContext context)
    {
        if (string.IsNullOrWhiteSpace(this.ClrName))
        {
            var path = $"{this.ApiPath}.{nameof(this.ClrName)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_CLR_NAME;
            var description = $"{nameof(this.ClrName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ClrName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion

    #region Validation Methods
    /// <summary>
    ///     Validates the specified CLR value against this property's constraints with type safety.
    /// </summary>
    /// <param name="apiValidationPath">The validation path used for error reporting.</param>
    /// <param name="clrValue">The CLR value to validate.</param>
    /// <returns>A <see cref="ValidationResult"/> if validation fails; otherwise, <c>null</c>.</returns>
    /// <remarks>
    ///     This method validates whether a value can be assigned to this property based on the <see cref="ApiTypeModifiers"/>.
    ///     Currently validates that required properties receive non-null values.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when <paramref name="apiValidationPath"/> is null or whitespace.</exception>
    public ValidationResult? ValidateValue(string apiValidationPath, object? clrValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        if (clrValue is null && this.IsRequired)
        {
            return new ValidationResult($"'{apiValidationPath}' is required and cannot be set to null.", [this.ApiName]);
        }
        return null;
    }

    /// <summary>
    ///     Validates the specified CLR value against this property's constraints with type safety.
    /// </summary>
    /// <typeparam name="TValue">The type of the CLR value to validate.</typeparam>
    /// <param name="apiValidationPath">The validation path used for error reporting.</param>
    /// <param name="clrValue">The CLR value to validate.</param>
    /// <returns>A <see cref="ValidationResult"/> if validation fails; otherwise, <c>null</c>.</returns>
    /// <remarks>
    ///     This method validates whether a value can be assigned to this property based on the <see cref="ApiTypeModifiers"/>.
    ///     Currently validates that required properties receive non-null values.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when <paramref name="apiValidationPath"/> is null or whitespace.</exception>
    public ValidationResult? ValidateValue<TValue>(string apiValidationPath, TValue? clrValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        if (clrValue is null && this.IsRequired)
        {
            return new ValidationResult($"'{apiValidationPath}' is required and cannot be set to null.", [this.ApiName]);
        }
        return null;
    }
    #endregion
}
