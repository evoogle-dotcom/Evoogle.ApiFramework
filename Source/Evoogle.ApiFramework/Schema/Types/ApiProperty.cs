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
    private readonly ClrMemberBinding _clrValueMemberBinding;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiProperty"/> class.
    /// </summary>
    /// <param name="apiName">The API name of the property.</param>
    /// <param name="apiTypeExpression">The API type expression of the property.</param>
    /// <param name="apiNullability">Nullability of the property value.</param>
    /// <param name="clrValueMember">The required CLR value member reference.</param>
    public ApiProperty
    (
        string apiName,
        ApiTypeExpression apiTypeExpression,
        ApiNullability apiNullability,
        ClrMemberReference clrValueMember
    )
    {
        ArgumentNullException.ThrowIfNull(clrValueMember);

        this.ApiName = apiName;
        this.ApiTypeExpression = apiTypeExpression;
        this.ApiNullability = apiNullability;
        _clrValueMemberBinding = new ClrMemberBinding(clrValueMember);
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

    /// <summary>Gets the nullability of the property value.</summary>
    public ApiNullability ApiNullability { get; }

    /// <summary>Gets the required CLR value member reference.</summary>
    public ClrMemberReference ClrValueMember => _clrValueMemberBinding.ClrMemberReference!;

    /// <summary>Gets whether the referenced CLR value member is a property or field.</summary>
    public ClrMemberKind ClrKind => this.ClrValueMember.ClrKind;

    /// <summary>Gets the name of the referenced CLR value member.</summary>
    public string ClrName => this.ClrValueMember.ClrName;

    internal ApiTypeExpression ApiTypeExpression { get; }
    #endregion

    #region ApiProperty Computed Properties
    /// <summary>Gets a value indicating whether this property permits a null value.</summary>
    public bool IsNullable => this.ApiNullability == ApiNullability.Nullable;

    /// <summary>Gets a value indicating whether this property prohibits a null value.</summary>
    public bool IsNonNullable => this.ApiNullability == ApiNullability.NonNullable;

    internal bool IsResolved => this.ApiTypeExpression?.IsResolved == true;

    internal Type? ClrMemberType => _clrValueMemberBinding.BoundClrMemberAccessor?.MemberType;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiTypeExpression = this.ApiTypeExpression.SafeToString();
        var apiNullability = this.ApiNullability.SafeToString();
        var clrValueMember = this.ClrValueMember.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiProperty)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiTypeExpression)}={apiTypeExpression}, {nameof(this.ApiNullability)}={apiNullability}, {nameof(this.ClrValueMember)}={clrValueMember}, {nameof(this.ExtensionCount)}={extensionCount}}}";
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
        this.ValidateApiNullability(context);
        this.ResolveApiTypeExpression(context);
        if (this.ApiTypeExpression?.IsResolved == true)
        {
            this.InitializeClrMemberAccessor(context);
        }
        else
        {
            this.ClrValueMember.Validate(context);
        }

        this.InitializeClrValueAccessor(context.Session.ApiSchemaContext);
    }

    private void ValidateApiName(ApiSchemaCompilationContext context)
    {
        var isApiNameInvalid = ApiSchemaNameValidation.IsNameInvalid(this.ApiName);
        if (isApiNameInvalid)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiPropertyInvalidApiName;
            var description = $"{nameof(this.ApiName)} must contain a non-whitespace value";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(severity, code, description, remediation);
        }
    }

    private void ValidateApiNullability(ApiSchemaCompilationContext context)
    {
        var jsonEnumFinding = this.GetJsonEnumFinding(nameof(this.ApiNullability));
        if (jsonEnumFinding is null && Enum.IsDefined(this.ApiNullability))
        {
            return;
        }

        var apiPath = this.ApiPath;
        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiPropertyInvalidApiNullability;
        var description = jsonEnumFinding?.DescribeProblem() ?? $"{nameof(this.ApiNullability)} is invalid";
        var remediation = $"Specify {ApiNullability.Nullable} or {ApiNullability.NonNullable} for {nameof(this.ApiNullability)}";

        context.AddIssue(apiPath, severity, code, description, remediation);
    }

    private void ResolveApiTypeExpression(ApiSchemaCompilationContext context)
    {
        if (this.ApiTypeExpression is null)
        {
            var apiPath = this.ApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiPropertyMissingType;
            var description = $"{nameof(this.ApiType)} is required";
            var remediation = $"Specify a valid {nameof(this.ApiType)}";

            context.AddIssue(apiPath, severity, code, description, remediation);
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
            this.ClrValueMember.Validate(context);
            return;
        }

        if (!_clrValueMemberBinding.TryResolveReference
        (
            clrObjectType,
            context,
            "API property CLR value member reference",
            requiresRead: false,
            requiresWrite: false
        ))
        {
            return;
        }

        var clrNullableInfo = _clrValueMemberBinding.ClrMemberInfo switch
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
        var isClrNullabilityUnknown = clrNullableInfo.Nullability == MemberNullability.Unknown;
        if (isClrNullabilityUnknown)
        {
            return;
        }

        // Non-nullable API property backed by a nullable CLR member.
        var isApiPropertyNonNullable = this.IsNonNullable;
        var isClrMemberNullable = clrNullableInfo.Nullability == MemberNullability.Nullable;
        var hasNonNullableApiPropertyWithNullableClrMember = isApiPropertyNonNullable && isClrMemberNullable;

        if (hasNonNullableApiPropertyWithNullableClrMember)
        {
            var apiPath = context.ApiPath;
            var severity = ApiSchemaCompilationSeverity.Warning;
            var code = ApiSchemaCompilationCode.ApiPropertyNullabilityMismatch;
            var description = $"CLR member '{clrMemberName}' is nullable but property '{this.ApiName}' is declared non-nullable";
            var remediation = $"Change CLR member '{clrMemberName}' to a non-nullable type, or change property '{this.ApiName}' to nullable";

            context.AddIssue(apiPath, severity, code, description, remediation);
            return;
        }

        // Nullable API property backed by a non-nullable CLR reference member may assign null
        // to a member that cannot hold it. Value types retain the existing default-value handling.
        var isApiPropertyNullable = this.IsNullable;
        var isClrMemberNonNullable = clrNullableInfo.Nullability == MemberNullability.NonNullable;
        var isClrMemberReferenceType = !clrNullableInfo.MemberType.IsValueType;
        var hasNullableApiPropertyWithNonNullableClrReferenceMember = isApiPropertyNullable && isClrMemberNonNullable && isClrMemberReferenceType;

        if (hasNullableApiPropertyWithNonNullableClrReferenceMember)
        {
            var apiPath = context.ApiPath;
            var severity = ApiSchemaCompilationSeverity.Warning;
            var code = ApiSchemaCompilationCode.ApiPropertyNullabilityMismatch;
            var description = $"CLR member '{clrMemberName}' is non-nullable but property '{this.ApiName}' is declared nullable";
            var remediation = $"Change CLR member '{clrMemberName}' to a nullable reference type, or change property '{this.ApiName}' to non-nullable";

            context.AddIssue(apiPath, severity, code, description, remediation);
        }

        // Check collection item nullability against ApiCollectionType.ApiItemNullability
        var hasClrCollectionElements = clrNullableInfo.CollectionChain.Count > 0;

        if (hasClrCollectionElements && this.ApiType is ApiCollectionType apiCollectionType)
        {
            var itemNullability = clrNullableInfo.CollectionChain[0].ElementNullability;
            var itemElementType = clrNullableInfo.CollectionChain[0].ElementType;

            // Skip if item nullability cannot be determined
            var isClrItemNullabilityUnknown = itemNullability == MemberNullability.Unknown;
            if (isClrItemNullabilityUnknown)
            {
                return;
            }

            // Non-nullable API items backed by nullable CLR elements.
            var isApiItemNonNullable = apiCollectionType.IsItemNonNullable;
            var isClrItemNullable = itemNullability == MemberNullability.Nullable;
            var hasNonNullableApiItemWithNullableClrElement = isApiItemNonNullable && isClrItemNullable;

            if (hasNonNullableApiItemWithNullableClrElement)
            {
                var apiPath = context.ApiPath;
                var severity = ApiSchemaCompilationSeverity.Warning;
                var code = ApiSchemaCompilationCode.ApiCollectionItemNullabilityMismatch;
                var description = $"CLR collection element in '{clrMemberName}' is nullable but item is declared non-nullable";
                var remediation = $"Change the CLR element type in '{clrMemberName}' to non-nullable, or change the item nullability to nullable";

                context.AddIssue(apiPath, severity, code, description, remediation);
                return;
            }

            // Nullable API items backed by non-nullable CLR reference elements may assign null
            // to elements that cannot hold it. Value types retain the existing default-value handling.
            var isApiItemNullable = apiCollectionType.IsItemNullable;
            var isClrItemNonNullable = itemNullability == MemberNullability.NonNullable;
            var isClrItemReferenceType = !itemElementType.IsValueType;
            var hasNullableApiItemWithNonNullableClrReferenceElement = isApiItemNullable && isClrItemNonNullable && isClrItemReferenceType;

            if (hasNullableApiItemWithNonNullableClrReferenceElement)
            {
                var apiPath = context.ApiPath;
                var severity = ApiSchemaCompilationSeverity.Warning;
                var code = ApiSchemaCompilationCode.ApiCollectionItemNullabilityMismatch;
                var description = $"CLR collection element in '{clrMemberName}' is non-nullable but item is declared nullable";
                var remediation = $"Change the CLR element type in '{clrMemberName}' to a nullable reference type, or change the item nullability to non-nullable";

                context.AddIssue(apiPath, severity, code, description, remediation);
            }
        }
    }
    #endregion
}
