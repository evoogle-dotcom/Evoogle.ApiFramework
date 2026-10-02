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
using Evoogle.ApiFramework.Schema.Types.Internal;
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
    private readonly ClrMemberBinding _clrMemberBinding;

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
        var clrMemberReference = new ClrMemberReference
        (
            clrMemberKind,
            clrName,
            hasInvalidClrKind: clrMemberKind is null
        );
        _clrMemberBinding = new ClrMemberBinding(clrMemberReference);
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
    public string ClrName => _clrMemberBinding.ClrMemberReference!.ClrName;

    /// <summary>
    ///     Gets the authoritative CLR member kind used with <see cref="ClrName"/> to bind this property.
    ///     <see cref="ClrMemberKind.Property"/> resolves only properties and
    ///     <see cref="ClrMemberKind.Field"/> resolves only fields.
    /// </summary>
    public ClrMemberKind ClrMemberKind => _clrMemberBinding.ClrMemberReference!.ClrKind;

    internal ApiTypeExpression ApiTypeExpression { get; }

    internal void MarkInvalidApiTypeModifiers() => _hasInvalidApiTypeModifiers = true;
    #endregion

    #region ApiProperty Computed Properties
    /// <summary>Gets a value indicating whether this property is optional (not required).</summary>
    public bool IsOptional => !this.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required);

    /// <summary>Gets a value indicating whether this property is required.</summary>
    public bool IsRequired => this.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required);

    internal bool IsResolved => this.ApiTypeExpression?.IsResolved == true;

    internal Type? ClrMemberType => _clrMemberBinding.BoundClrMemberAccessor?.MemberType;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiTypeExpression = this.ApiTypeExpression.SafeToString();
        var apiTypeModifiers = this.ApiTypeModifiers.SafeToString();
        var clrName = this.ClrName.SafeToString();
        var clrMemberKind = _clrMemberBinding.ClrMemberReference!.NullableClrKind.SafeToString();
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
        if (this.ApiTypeExpression?.IsResolved == true)
        {
            this.InitializeClrMemberAccessor(context);
        }
        else
        {
            _clrMemberBinding.ClrMemberReference!.Validate(context);
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

    private void InitializeClrMemberAccessor(ApiSchemaCompilationContext context)
    {
        var apiObjectType = this.GetApiObjectType();
        var clrObjectType = apiObjectType?.ClrType;

        if (clrObjectType is null)
        {
            // The reference remains independently validatable when the parent has no CLR type.
            _clrMemberBinding.ClrMemberReference!.Validate(context);
            return;
        }

        if (!_clrMemberBinding.TryResolveReference
        (
            clrObjectType,
            context,
            "API property CLR member reference",
            requiresRead: false,
            requiresWrite: false
        ))
        {
            return;
        }

        var clrNullableInfo = _clrMemberBinding.ClrMemberInfo switch
        {
            PropertyInfo clrPropertyInfo => PropertyReflection.GetNullabilityInfo(clrPropertyInfo),
            FieldInfo clrFieldInfo => FieldReflection.GetNullabilityInfo(clrFieldInfo),
            _ => throw new ApiSchemaException("A CLR member binding must contain a property or field.")
        };
        this.ValidateNullabilityMismatch(context, clrNullableInfo, this.ClrName);
    }

    private ApiObjectType GetApiObjectType()
        => this.Parent as ApiObjectType ?? throw new ApiSchemaException($"An {nameof(ApiProperty)} must be owned by an {nameof(ApiObjectType)}.");
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
    #endregion
}
