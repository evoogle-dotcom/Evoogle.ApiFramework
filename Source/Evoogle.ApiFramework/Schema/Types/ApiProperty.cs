// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;
using Evoogle.MemberAccess;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Types;

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
///         During schema compilation, the CLR member is resolved to a <see cref="MemberAccessor"/>. Core owns
///         the process-wide accessor and compiled-delegate caches, while this class provides schema-aware
///         convenience methods that use the schema's configured type coercion.
///     </para>
///     <para>
///         The non-generic Try* methods accept <see cref="object"/> and will box value types by design. Prefer
///         the fully generic overloads to avoid boxing both the target instance and the returned value.
///     </para>
/// </remarks>
[JsonConverter(typeof(ApiPropertyJsonConverter))]
public sealed partial class ApiProperty : ApiSchemaElement
{
    #region ApiProperty Fields
    private readonly ClrMemberKind? _clrMemberKind;

    private MemberAccessor? _clrMemberAccessor;

    private bool _hasInvalidApiTypeModifiers;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiProperty"/> class.
    /// </summary>
    /// <param name="apiName">The API name of the property.</param>
    /// <param name="apiTypeExpression">The API type expression of the property.</param>
    /// <param name="apiTypeModifiers">Modifiers applied to the property (e.g., Required).</param>
    /// <param name="clrName">The CLR name of the property or field corresponding to this API property.</param>
    /// <param name="clrMemberKind">The concrete kind of CLR member this API property represents.</param>
    public ApiProperty
    (
        string apiName,
        ApiTypeExpression apiTypeExpression,
        ApiTypeModifiers apiTypeModifiers,
        string clrName,
        ClrMemberKind clrMemberKind
    )
        : this(apiName, apiTypeExpression, apiTypeModifiers, clrName, (ClrMemberKind?)clrMemberKind)
    {
    }

    internal ApiProperty
    (
        string apiName,
        ApiTypeExpression apiTypeExpression,
        ApiTypeModifiers apiTypeModifiers,
        string clrName,
        ClrMemberKind? clrMemberKind
    )
    {
        this.ApiName = apiName;
        this.ApiTypeExpression = apiTypeExpression;
        this.ApiTypeModifiers = apiTypeModifiers;
        this.ClrName = clrName;
        _clrMemberKind = clrMemberKind;
    }
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override ApiSchemaElementKind Kind => ApiSchemaElementKind.Property;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiProperty);
    #endregion

    #region ApiProperty Properties
    /// <summary>Gets the API name of the property (used in API requests/responses).</summary>
    public string ApiName { get; }

    /// <summary>Gets the API type of the property.</summary>
    public ApiType ApiType => this.ApiTypeExpression.ApiType;

    /// <summary>Gets the modifiers applied to this property (e.g., Required).</summary>
    public ApiTypeModifiers ApiTypeModifiers { get; }

    /// <summary>Gets the CLR name of the member backing this API property.</summary>
    public string ClrName { get; }

    /// <summary>
    ///     Gets the authoritative CLR member kind used with <see cref="ClrName"/> to bind this property.
    ///     <see cref="ClrMemberKind.Property"/> resolves only properties and
    ///     <see cref="ClrMemberKind.Field"/> resolves only fields.
    /// </summary>
    public ClrMemberKind ClrMemberKind => this.RequireValue(_clrMemberKind);

    internal ApiTypeExpression ApiTypeExpression { get; }

    internal void MarkInvalidApiTypeModifiers() => _hasInvalidApiTypeModifiers = true;
    #endregion

    #region ApiProperty Computed Properties
    /// <summary>Gets a value indicating whether this property is optional (not required).</summary>
    public bool IsOptional => !this.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required);

    /// <summary>Gets a value indicating whether this property is required.</summary>
    public bool IsRequired => this.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required);

    internal bool IsResolved => this.ApiTypeExpression?.IsResolved == true;

    internal Type? ClrMemberType => _clrMemberAccessor?.MemberType;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiTypeExpression = this.ApiTypeExpression.SafeToString();
        var apiTypeModifiers = this.ApiTypeModifiers.SafeToString();
        var clrName = this.ClrName.SafeToString();
        var clrMemberKind = _clrMemberKind.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiProperty)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiTypeExpression)}={apiTypeExpression}, {nameof(this.ApiTypeModifiers)}={apiTypeModifiers}, {nameof(this.ClrName)}={clrName}, {nameof(this.ClrMemberKind)}={clrMemberKind}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override IEnumerable<ApiSchemaElement> GetOwnedElements()
    {
        if (this.ApiTypeExpression?.ApiInlineType is ApiType apiInlineType)
        {
            yield return apiInlineType;
        }
    }

    /// <inheritdoc />
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaPathFormatting.BuildPath(apiBasePath: apiPreviousPath, apiPathSegment: this.ApiElementName, apiPathSegmentName: this.ApiName);

    /// <inheritdoc />
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        this.ValidateApiName(context);
        this.ValidateApiTypeModifiers(context);
        this.ResolveApiTypeExpression(context);
        this.ValidateClrName(context);

        if (this.ValidateClrMemberKind(context) && this.ApiTypeExpression?.IsResolved == true)
        {
            this.InitializeClrMemberAccessor(context);
        }
    }

    private void ValidateApiName(ApiSchemaCompilationContext context)
    {
        var isApiNameInvalid = ApiSchemaNameValidation.IsNameInvalid(this.ApiName);
        if (isApiNameInvalid)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiPropertyInvalidApiName;
            var description = $"{nameof(this.ApiName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(severity, code, description, remediation);
        }
    }

    private void ValidateApiTypeModifiers(ApiSchemaCompilationContext context)
    {
        if (!_hasInvalidApiTypeModifiers)
        {
            return;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiPropertyInvalidApiTypeModifiers;
        var description = $"{nameof(this.ApiTypeModifiers)} must be a valid {nameof(this.ApiTypeModifiers)} value";
        var remediation = $"Specify a valid {nameof(this.ApiTypeModifiers)} value";

        context.AddIssue(severity, code, description, remediation);
    }

    private void ResolveApiTypeExpression(ApiSchemaCompilationContext context)
    {
        if (this.ApiTypeExpression is null)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiPropertyNullType;
            var description = $"{nameof(this.ApiType)} must not be null";
            var remediation = $"Specify a valid {nameof(this.ApiType)}";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        this.ApiTypeExpression.ResolveForProperty(context);
    }

    private void InitializeClrFieldAccessor(ApiSchemaCompilationContext context, FieldInfo clrFieldInfo)
    {
        var clrMemberName = this.ClrName;
        var clrMemberAccessor = MemberAccessor.Create(clrFieldInfo);
        _clrMemberAccessor = clrMemberAccessor;

        if (clrMemberAccessor.CanRead)
        {
            try
            {
                MemberAccessorFactory.CreateGetter(clrFieldInfo);
            }
            catch (Exception ex)
            {
                var severity = ApiSchemaCompilationSeverity.Error;
                var code = ApiSchemaCompilationCode.ApiPropertyInvalidFieldGetter;
                var rootCause = GetGetterRootCauseMessage(ex, clrFieldInfo.FieldType);
                var description = $"Failed to compile field getter for '{clrMemberName}': {rootCause}";
                var remediation = $"Verify that field '{clrMemberName}' is readable and can be used in expression trees";

                context.AddIssue(severity, code, description, remediation);
            }
        }

        if (clrMemberAccessor.CanWrite && !clrMemberAccessor.DeclaringType.IsValueType)
        {
            try
            {
                MemberAccessorFactory.CreateCoercingSetter(clrFieldInfo);
            }
            catch (Exception ex)
            {
                var severity = ApiSchemaCompilationSeverity.Error;
                var code = ApiSchemaCompilationCode.ApiPropertyInvalidFieldSetter;
                var description = $"Failed to compile field setter for '{clrMemberName}': {GetRootCauseMessage(ex)}";
                var remediation = $"Verify that field '{clrMemberName}' is writable and can be used in expression trees";

                context.AddIssue(severity, code, description, remediation);
            }
        }

        var clrFieldNullableInfo = FieldReflection.GetNullabilityInfo(clrFieldInfo);
        this.ValidateNullabilityMismatch(context, clrFieldNullableInfo, clrMemberName);
    }

    private void InitializeClrPropertyAccessor(ApiSchemaCompilationContext context, PropertyInfo clrPropertyInfo)
    {
        var clrMemberName = this.ClrName;

        if (clrPropertyInfo.GetIndexParameters().Length > 0)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiPropertyInvalidPropertyGetter;
            var description = $"Property '{clrMemberName}' is an indexer, which is not supported";

            context.AddIssue(severity, code, description, remediation: null);
            return;
        }

        var clrMemberAccessor = MemberAccessor.Create(clrPropertyInfo);
        _clrMemberAccessor = clrMemberAccessor;

        if (clrMemberAccessor.CanRead)
        {
            try
            {
                MemberAccessorFactory.CreateGetter(clrPropertyInfo);
            }
            catch (Exception ex)
            {
                var severity = ApiSchemaCompilationSeverity.Error;
                var code = ApiSchemaCompilationCode.ApiPropertyInvalidPropertyGetter;
                var rootCause = GetGetterRootCauseMessage(ex, clrPropertyInfo.PropertyType);
                var description = $"Failed to compile property getter for '{clrMemberName}': {rootCause}";
                var remediation = $"Verify that property '{clrMemberName}' is readable and can be used in expression trees";

                context.AddIssue(severity, code, description, remediation);
            }
        }

        if (clrMemberAccessor.CanWrite && !clrMemberAccessor.DeclaringType.IsValueType)
        {
            try
            {
                MemberAccessorFactory.CreateCoercingSetter(clrPropertyInfo);
            }
            catch (Exception ex)
            {
                var severity = ApiSchemaCompilationSeverity.Error;
                var code = ApiSchemaCompilationCode.ApiPropertyInvalidPropertySetter;
                var description = $"Failed to compile property setter for '{clrMemberName}': {GetRootCauseMessage(ex)}";
                var remediation = $"Verify that property '{clrMemberName}' is writable and can be used in expression trees";

                context.AddIssue(severity, code, description, remediation);
            }
        }

        var clrPropertyNullableInfo = PropertyReflection.GetNullabilityInfo(clrPropertyInfo);
        this.ValidateNullabilityMismatch(context, clrPropertyNullableInfo, clrMemberName);
    }

    private void InitializeClrMemberAccessor(ApiSchemaCompilationContext context)
    {
        var apiObjectType = this.GetApiObjectType();
        var clrObjectType = apiObjectType?.ClrType;
        var clrMemberName = this.ClrName;

        if (clrObjectType is null)
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
            switch (this.ClrMemberKind)
            {
                case ClrMemberKind.Property:
                    var clrPropertyInfo = TypeReflection.GetProperty
                    (
                        clrObjectType,
                        clrMemberName,
                        BindingFlags.Public | BindingFlags.Instance
                    );

                    if (clrPropertyInfo is null)
                    {
                        this.AddMissingClrMemberIssue
                        (
                            context,
                            clrObjectType,
                            clrMemberName
                        );
                        return;
                    }

                    if (!ValidateClrMemberType(context, clrPropertyInfo.PropertyType, clrMemberName))
                    {
                        return;
                    }

                    this.InitializeClrPropertyAccessor(context, clrPropertyInfo);
                    return;

                case ClrMemberKind.Field:
                    var clrFieldInfo = TypeReflection.GetField
                    (
                        clrObjectType,
                        clrMemberName,
                        BindingFlags.Public | BindingFlags.Instance
                    );

                    if (clrFieldInfo is null)
                    {
                        this.AddMissingClrMemberIssue
                        (
                            context,
                            clrObjectType,
                            clrMemberName
                        );
                        return;
                    }

                    if (!ValidateClrMemberType(context, clrFieldInfo.FieldType, clrMemberName))
                    {
                        return;
                    }

                    this.InitializeClrFieldAccessor(context, clrFieldInfo);
                    return;
            }
        }
        catch (Exception ex)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiPropertyInvalidClrMember;
            var description = $"Failed to compile getter or setter accessor for '{clrMemberName}': {GetRootCauseMessage(ex)}";
            var remediation = $"Verify that '{clrMemberName}' exists as a public {this.ClrMemberKind.ToString().ToLowerInvariant()} on {nameof(ApiObjectType)}.{nameof(ApiObjectType.ClrType)} '{clrObjectType.SafeToName()}'";

            context.AddIssue(severity, code, description, remediation);
        }
    }

    private static string GetRootCauseMessage(Exception exception)
        => exception.GetBaseException().Message.TrimEnd('.');

    private static string GetGetterRootCauseMessage(Exception exception, Type clrMemberType)
    {
        if (clrMemberType.IsPointer)
        {
            return $"No coercion operator is defined between types '{clrMemberType}' and '{typeof(object)}'";
        }

        return GetRootCauseMessage(exception);
    }

    private bool ValidateClrMemberKind(ApiSchemaCompilationContext context)
    {
        var isClrMemberKindValid = _clrMemberKind is ClrMemberKind.Property or ClrMemberKind.Field;
        if (isClrMemberKindValid)
        {
            return true;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiPropertyInvalidClrMember;
        var description = $"{nameof(this.ClrMemberKind)} must be {ClrMemberKind.Property} or {ClrMemberKind.Field}";
        var remediation = $"Specify {nameof(this.ClrMemberKind)} as {ClrMemberKind.Property} or {ClrMemberKind.Field}";

        context.AddIssue(severity, code, description, remediation);
        return false;
    }

    private void AddMissingClrMemberIssue(ApiSchemaCompilationContext context, Type clrObjectType, string clrMemberName)
    {
        var clrMemberKindName = this.ClrMemberKind.ToString().ToLowerInvariant();
        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiPropertyMissingClrMember;
        var description = $"CLR {clrMemberKindName} '{clrMemberName}' was not found on CLR type '{clrObjectType.SafeToName()}'";
        var remediation = $"Add a public CLR {clrMemberKindName} named '{clrMemberName}' to CLR type '{clrObjectType.SafeToName()}'";

        context.AddIssue(severity, code, description, remediation);
    }

    private void ValidateClrName(ApiSchemaCompilationContext context)
    {
        var isClrNameInvalid = ApiSchemaNameValidation.IsNameInvalid(this.ClrName);
        if (isClrNameInvalid)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiPropertyInvalidClrName;
            var description = $"{nameof(this.ClrName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ClrName)} value";

            context.AddIssue(severity, code, description, remediation);
        }
    }

    private ApiObjectType GetApiObjectType()
    {
        return this.Parent as ApiObjectType
            ?? throw new ApiSchemaException($"An {nameof(ApiProperty)} must be owned by an {nameof(ApiObjectType)}.");
    }
    #endregion

    #region Validation Methods
    private void ValidateNullabilityMismatch(ApiSchemaCompilationContext context, MemberNullableInfo clrNullableInfo, string clrMemberName)
    {
        // Skip if nullability cannot be determined (defensive guard for types from assemblies without NRT)
        if (clrNullableInfo.Nullability == MemberNullability.Unknown)
        {
            return;
        }

        // Required + CLR Nullable: API contract demands a value but CLR type permits null
        if (this.IsRequired && clrNullableInfo.Nullability == MemberNullability.Nullable)
        {
            var severity = ApiSchemaCompilationSeverity.Warning;
            var code = ApiSchemaCompilationCode.ApiPropertyRequiredNullableMismatch;
            var description = $"CLR member '{clrMemberName}' is nullable but property '{this.ApiName}' is declared Required";
            var remediation = $"Change CLR member '{clrMemberName}' to a non-nullable type, or change property '{this.ApiName}' to Optional";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        // Optional + CLR NonNullable (reference types only): absent optional value may assign null
        // to a CLR member that cannot hold it. Value types are excluded: absent value → default, never null.
        if (this.IsOptional && clrNullableInfo.Nullability == MemberNullability.NonNullable && !clrNullableInfo.MemberType.IsValueType)
        {
            var severity = ApiSchemaCompilationSeverity.Warning;
            var code = ApiSchemaCompilationCode.ApiPropertyOptionalNonNullableMismatch;
            var description = $"CLR member '{clrMemberName}' is non-nullable but property '{this.ApiName}' is declared Optional";
            var remediation = $"Change CLR member '{clrMemberName}' to a nullable reference type, or change property '{this.ApiName}' to Required";

            context.AddIssue(severity, code, description, remediation);
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
                var severity = ApiSchemaCompilationSeverity.Warning;
                var code = ApiSchemaCompilationCode.ApiCollectionItemRequiredNullableMismatch;
                var description = $"CLR collection element in '{clrMemberName}' is nullable but item is declared Required";
                var remediation = $"Change the CLR element type in '{clrMemberName}' to non-nullable, or change the item modifier to Optional";

                context.AddIssue(severity, code, description, remediation);
                return;
            }

            // Item Optional + CLR element NonNullable (reference types only): absent item may assign null
            // to a CLR element that cannot hold it. Value types are excluded: absent item → default, never null.
            if (apiCollectionType.IsItemOptional && itemNullability == MemberNullability.NonNullable && !itemElementType.IsValueType)
            {
                var severity = ApiSchemaCompilationSeverity.Warning;
                var code = ApiSchemaCompilationCode.ApiCollectionItemOptionalNonNullableMismatch;
                var description = $"CLR collection element in '{clrMemberName}' is non-nullable but item is declared Optional";
                var remediation = $"Change the CLR element type in '{clrMemberName}' to a nullable reference type, or change the item modifier to Required";

                context.AddIssue(severity, code, description, remediation);
            }
        }
    }

    private static bool ValidateClrMemberType(ApiSchemaCompilationContext context, Type memberType, string memberName)
    {
        // Check if the type is a ref struct (cannot be boxed/unboxed)
        if (memberType.IsByRefLike)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiPropertyInvalidClrMember;
            var description = $"CLR member '{memberName}' has type '{memberType.SafeToName()}' which is a ref struct. Ref structs cannot be boxed to object and are not supported for API properties.";
            var remediation = $"Change the type of CLR member '{memberName}' to a non-ref struct type.";

            context.AddIssue(severity, code, description, remediation);
            return false;
        }

        return true;
    }
    #endregion
}
