// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
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
///         objects, collections, and complex types.
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
/// <param name="clrMemberKind">The kind of CLR member (property or field) this API property represents.</param>
[JsonConverter(typeof(ApiPropertyJsonConverter))]
public sealed partial class ApiProperty
(
    string apiName,
    ApiTypeExpression apiTypeExpression,
    ApiTypeModifiers apiTypeModifiers,
    string clrName,
    ClrMemberKind clrMemberKind
) : ApiSchemaElement
{
    #region ApiProperty Types
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

    #region ApiProperty Fields
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

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiProperty);
    #endregion

    #region ApiProperty Properties
    /// <summary>Gets the API name of the property (used in API requests/responses).</summary>
    public string ApiName { get; } = apiName;

    /// <summary>Gets the API type of the property.</summary>
    public ApiType ApiType => this.ApiTypeExpression.ApiType;

    /// <summary>Gets the modifiers applied to this property (e.g., Required).</summary>
    public ApiTypeModifiers ApiTypeModifiers { get; } = apiTypeModifiers;

    /// <summary>Gets the CLR name of the property (matching the C# property name).</summary>
    public string ClrName { get; } = clrName;

    /// <summary>Gets the kind of CLR member (property or field) this API property represents.</summary>
    public ClrMemberKind ClrMemberKind { get; } = clrMemberKind;

    internal ApiTypeExpression ApiTypeExpression { get; } = apiTypeExpression;

    private static MethodInfo GenericCoerceMethodDefinition => _genericCoerceMethodDefinition
        ?? throw new ApiSchemaException($"Failed to locate generic method definition for {nameof(TypeCoercion)}.{nameof(TypeCoercion.Coerce)}.");

    private static MethodInfo NonGenericCoerceMethod => _nonGenericCoerceMethod
        ?? throw new ApiSchemaException($"Failed to locate non-generic method for {nameof(TypeCoercion)}.{nameof(TypeCoercion.Coerce)}.");
    #endregion

    #region ApiProperty Computed Properties
    /// <summary>Gets a value indicating whether this property is optional (not required).</summary>
    public bool IsOptional => !this.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required);

    /// <summary>Gets a value indicating whether this property is required.</summary>
    public bool IsRequired => this.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required);

    internal bool IsResolved => this.ApiTypeExpression?.IsResolved == true;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiTypeExpression = this.ApiTypeExpression.SafeToString();
        var apiTypeModifiers = this.ApiTypeModifiers.SafeToString();
        var clrName = this.ClrName.SafeToString();
        var clrMemberKind = this.ClrMemberKind.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiProperty)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiTypeExpression)}={apiTypeExpression}, {nameof(this.ApiTypeModifiers)}={apiTypeModifiers}, {nameof(this.ClrName)}={clrName}, {nameof(this.ClrMemberKind)}={clrMemberKind}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaPathFormatting.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: this.ApiName);

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
        var isApiNameInvalid = ApiSchemaNameValidation.IsNameInvalid(this.ApiName);
        if (isApiNameInvalid)
        {
            var path = this.ApiPath;
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
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_NULL_TYPE;
            var description = $"{nameof(this.ApiType)} must not be null";
            var remediation = $"Specify a valid {nameof(this.ApiType)}";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        var childContext = context.WithDeclaringSchemaElement(this);
        this.ApiTypeExpression.InitializeForProperty(childContext);
    }

    private void InitializeClrFieldGetterAndSetter(ApiInitializationContext context, FieldInfo clrFieldInfo)
    {
        var apiObjectType = context.ApiDeclaringObjectType;
        var clrObjectType = apiObjectType.ClrType;
        var clrMemberName = this.ClrName;

        // Build compiled field getter and setter
        try
        {
            _clrGetter = BuildNonGenericClrFieldGetter(clrObjectType, clrFieldInfo);
        }
        catch (Exception ex)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_FIELD_GETTER;
            var description = $"Failed to compile field getter for '{clrMemberName}': {ex.Message.TrimEnd('.')}";
            var remediation = $"Verify that field '{clrMemberName}' is readable and can be used in expression trees";

            context.AddIssue(path, severity, code, description, remediation);
        }

        try
        {
            _clrSetter = BuildNonGenericClrFieldSetter(clrObjectType, clrFieldInfo);
        }
        catch (Exception ex)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_FIELD_SETTER;
            var description = $"Failed to compile field setter for '{clrMemberName}': {ex.Message.TrimEnd('.')}";
            var remediation = $"Verify that field '{clrMemberName}' is writable and can be used in expression trees";
            context.AddIssue(path, severity, code, description, remediation);
        }

        // Validate nullability alignment between API declaration and CLR member
        var clrFieldNullableInfo = FieldReflection.GetNullabilityInfo(clrFieldInfo);
        this.ValidateNullabilityMismatch(context, clrFieldNullableInfo, clrMemberName);
    }

    private void InitializeClrPropertyGetterAndSetter(ApiInitializationContext context, PropertyInfo clrPropertyInfo)
    {
        var apiObjectType = context.ApiDeclaringObjectType;
        var clrObjectType = apiObjectType.ClrType;
        var clrMemberName = this.ClrName;

        // Exclude indexers
        if (clrPropertyInfo.GetIndexParameters().Length > 0)
        {
            var path = this.ApiPath;
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
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_PROPERTY_GETTER;
            var description = $"Failed to compile property getter for '{clrMemberName}': {ex.Message.TrimEnd('.')}";
            var remediation = $"Verify that property '{clrMemberName}' is readable and can be used in expression trees";

            context.AddIssue(path, severity, code, description, remediation);
        }

        try
        {
            _clrSetter = BuildNonGenericClrPropertySetter(clrObjectType, clrPropertyInfo);
        }
        catch (Exception ex)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_PROPERTY_SETTER;
            var description = $"Failed to compile property setter for '{clrMemberName}': {ex.Message.TrimEnd('.')}";
            var remediation = $"Verify that property '{clrMemberName}' is writable and can be used in expression trees";

            context.AddIssue(path, severity, code, description, remediation);
        }

        // Validate nullability alignment between API declaration and CLR member
        var clrPropertyNullableInfo = PropertyReflection.GetNullabilityInfo(clrPropertyInfo);
        this.ValidateNullabilityMismatch(context, clrPropertyNullableInfo, clrMemberName);
    }

    private void InitializeClrGetterAndSetter(ApiInitializationContext context)
    {
        var apiObjectType = context.ApiDeclaringObjectType;
        var clrObjectType = apiObjectType.ClrType;
        var clrMemberName = this.ClrName;

        var isClrObjectTypeNull = clrObjectType is null;
        if (isClrObjectTypeNull)
        {
            // If the parent CLR object type is null, skip further processing
            return;
        }

        var isClrMemberNameInvalid = ApiSchemaNameValidation.IsNameInvalid(clrMemberName);
        if (isClrMemberNameInvalid)
        {
            // If the CLR member name is invalid, skip further processing
            return;
        }

        try
        {
            // Prefer property, then field
            var clrPropertyInfo = TypeReflection.GetProperty(clrObjectType!, clrMemberName, BindingFlags.Public | BindingFlags.Instance);
            if (clrPropertyInfo is not null)
            {
                if (!this.ValidateClrMemberType(context, clrPropertyInfo.PropertyType, clrMemberName))
                {
                    // Fail fast on invalid member type
                    return;
                }

                this.InitializeClrPropertyGetterAndSetter(context, clrPropertyInfo);
                return; // Found valid property
            }

            var clrFieldInfo = TypeReflection.GetField(clrObjectType!, clrMemberName, BindingFlags.Public | BindingFlags.Instance);
            if (clrFieldInfo is not null)
            {
                if (!this.ValidateClrMemberType(context, clrFieldInfo.FieldType, clrMemberName))
                {
                    // Fail fast on invalid member type
                    return;
                }

                this.InitializeClrFieldGetterAndSetter(context, clrFieldInfo);
                return; // Found valid field
            }

            // Member not found
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_MISSING_CLR_MEMBER;
            var description = $"CLR member '{clrMemberName}' was not found on CLR type '{clrObjectType.SafeToName()}'";
            var remediation = $"Add a public CLR property or field named '{clrMemberName}' to CLR type '{clrObjectType.SafeToName()}'";

            context.AddIssue(path, severity, code, description, remediation);
        }
        catch (Exception ex)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_CLR_MEMBER;
            var description = $"Failed to compile getter or setter accessor for '{clrMemberName}': {ex.Message.TrimEnd('.')}";
            var remediation = $"Verify that '{clrMemberName}' exists as a public property or field on {nameof(ApiObjectType)}.{nameof(ApiObjectType.ClrType)} '{clrObjectType.SafeToName()}'";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeClrName(ApiInitializationContext context)
    {
        var isClrNameInvalid = ApiSchemaNameValidation.IsNameInvalid(this.ClrName);
        if (isClrNameInvalid)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_CLR_NAME;
            var description = $"{nameof(this.ClrName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ClrName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion

    #region Validation Methods
    private void ValidateNullabilityMismatch(ApiInitializationContext context, MemberNullableInfo clrNullableInfo, string clrMemberName)
    {
        // Skip if nullability cannot be determined (defensive guard for types from assemblies without NRT)
        if (clrNullableInfo.Nullability == MemberNullability.Unknown)
        {
            return;
        }

        // Required + CLR Nullable: API contract demands a value but CLR type permits null
        if (this.IsRequired && clrNullableInfo.Nullability == MemberNullability.Nullable)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Warning;
            var code = ApiInitializationCode.API_PROPERTY_REQUIRED_NULLABLE_MISMATCH;
            var description = $"CLR member '{clrMemberName}' is nullable but property '{this.ApiName}' is declared Required";
            var remediation = $"Change CLR member '{clrMemberName}' to a non-nullable type, or change property '{this.ApiName}' to Optional";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        // Optional + CLR NonNullable (reference types only): absent optional value may assign null
        // to a CLR member that cannot hold it. Value types are excluded: absent value → default, never null.
        if (this.IsOptional && clrNullableInfo.Nullability == MemberNullability.NonNullable && !clrNullableInfo.MemberType.IsValueType)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Warning;
            var code = ApiInitializationCode.API_PROPERTY_OPTIONAL_NON_NULLABLE_MISMATCH;
            var description = $"CLR member '{clrMemberName}' is non-nullable but property '{this.ApiName}' is declared Optional";
            var remediation = $"Change CLR member '{clrMemberName}' to a nullable reference type, or change property '{this.ApiName}' to Required";

            context.AddIssue(path, severity, code, description, remediation);
        }

        // Check collection item nullability against ApiCollectionType.ApiItemTypeModifiers
        if (clrNullableInfo.CollectionChain.Count > 0 && this.ApiType is ApiCollectionType apiCollectionType)
        {
            var itemNullability = clrNullableInfo.CollectionChain[0].ElementNullability;
            var itemElementType = clrNullableInfo.CollectionChain[0].ElementType;

            // Skip if item nullability cannot be determined
            if (itemNullability == MemberNullability.Unknown)
            {
                return;
            }

            // Item Required + CLR element Nullable: API contract demands a value but CLR element permits null
            if (apiCollectionType.IsItemRequired && itemNullability == MemberNullability.Nullable)
            {
                var path = this.ApiPath;
                var severity = ApiInitializationSeverity.Warning;
                var code = ApiInitializationCode.API_COLLECTION_ITEM_REQUIRED_NULLABLE_MISMATCH;
                var description = $"CLR collection element in '{clrMemberName}' is nullable but item is declared Required";
                var remediation = $"Change the CLR element type in '{clrMemberName}' to non-nullable, or change the item modifier to Optional";

                context.AddIssue(path, severity, code, description, remediation);
                return;
            }

            // Item Optional + CLR element NonNullable (reference types only): absent item may assign null
            // to a CLR element that cannot hold it. Value types are excluded: absent item → default, never null.
            if (apiCollectionType.IsItemOptional && itemNullability == MemberNullability.NonNullable && !itemElementType.IsValueType)
            {
                var path = this.ApiPath;
                var severity = ApiInitializationSeverity.Warning;
                var code = ApiInitializationCode.API_COLLECTION_ITEM_OPTIONAL_NON_NULLABLE_MISMATCH;
                var description = $"CLR collection element in '{clrMemberName}' is non-nullable but item is declared Optional";
                var remediation = $"Change the CLR element type in '{clrMemberName}' to a nullable reference type, or change the item modifier to Required";

                context.AddIssue(path, severity, code, description, remediation);
            }
        }
    }

    private bool ValidateClrMemberType(ApiInitializationContext context, Type memberType, string memberName)
    {
        // Check if the type is a ref struct (cannot be boxed/unboxed)
        if (memberType.IsByRefLike)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_PROPERTY_INVALID_CLR_MEMBER;
            var description = $"CLR member '{memberName}' has type '{memberType.SafeToName()}' which is a ref struct. Ref structs cannot be boxed to object and are not supported for API properties.";
            var remediation = $"Change the type of CLR member '{memberName}' to a non-ref struct type.";

            context.AddIssue(path, severity, code, description, remediation);
            return false;
        }

        return true;
    }
    #endregion
}
