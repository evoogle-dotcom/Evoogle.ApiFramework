// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.ApiFramework.Schema.Types.Internal;

namespace Evoogle.ApiFramework.Schema.Relationships;

/// <summary>
///     Exposes a named direction through a relationship from one participating object type to
///     another. It is an API field, separate from the source type's contained properties.
/// </summary>
/// <remarks>
///     A traversal may bind to a CLR navigation member on its source object type. The binding
///     identifies where a loaded relationship is read or a selected client result is assigned;
///     the member's value alone does not establish whether the relationship was loaded.
/// </remarks>
/// <param name="apiName">The API name exposed on the source object type.</param>
/// <param name="clrNavigationMember">The optional CLR navigation member reference.</param>
[JsonConverter(typeof(ApiRelationshipTraversalJsonConverter))]
public sealed partial class ApiRelationshipTraversal(string apiName, ClrMemberReference? clrNavigationMember = null)
    : ApiSchemaElement
{
    #region ApiRelationshipTraversal Fields
    private readonly ClrMemberBinding _clrNavigationMemberBinding = new(clrNavigationMember);
    private ApiRelationshipEnd? _targetEnd;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override ApiSchemaElementKind Kind => ApiSchemaElementKind.RelationshipTraversal;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipTraversal);
    #endregion

    #region ApiRelationshipTraversal Properties
    /// <summary>Gets the API name of this traversal on its source object type.</summary>
    public string ApiName { get; } = apiName;

    /// <summary>Gets the optional CLR navigation member reference.</summary>
    public ClrMemberReference? ClrNavigationMember => _clrNavigationMemberBinding.ClrMemberReference;

    /// <summary>Gets the relationship end from which this traversal begins.</summary>
    public ApiRelationshipEnd SourceEnd => this.Parent as ApiRelationshipEnd
        ?? throw new ApiSchemaException("A traversal must be owned by a relationship end.");

    /// <summary>Gets the opposite relationship end after schema compilation.</summary>
    public ApiRelationshipEnd TargetEnd => this.RequireValue(_targetEnd);

    /// <summary>Gets the source object type after schema compilation.</summary>
    public ApiObjectType SourceObjectType => this.SourceEnd.ApiObjectType;

    /// <summary>Gets the target object type after schema compilation.</summary>
    public ApiObjectType TargetObjectType => this.TargetEnd.ApiObjectType;
    #endregion

    #region ApiRelationshipTraversal Computed Properties
    /// <summary>Gets whether this traversal has a CLR navigation member binding.</summary>
    public bool HasClrNavigationMember => _clrNavigationMemberBinding.HasReference;

    /// <summary>
    ///     Gets whether one source can reach multiple targets through this traversal.
    /// </summary>
    public bool IsToMany => this.SourceEnd.ApiRelationship switch
    {
        ApiRelationshipOneToMany oneToMany => ReferenceEquals(this.SourceEnd, oneToMany.ApiPrincipalEnd),
        ApiRelationshipManyToMany => true,
        _ => false
    };
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath) =>
        ApiSchemaPathFormatting.BuildPath(apiPreviousPath, this.ApiElementName, this.ApiName);

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        base.CompileCore(context);

        if (ApiSchemaNameValidation.IsNameInvalid(this.ApiName))
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiRelationshipTraversalInvalidApiName;
            var description = "A relationship traversal must have a nonempty API name";
            var remediation = "Specify an API name for the traversal";

            context.AddIssue(severity, code, description, remediation);
        }

    }
    #endregion

    #region Binding Methods
    internal void BindTargetEnd(ApiRelationshipEnd targetEnd, ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(targetEnd);
        this.ThrowIfFrozen();
        _targetEnd = targetEnd;

        if (!this.HasClrNavigationMember)
        {
            return;
        }

        if (this.SourceEnd.ApiResolvedObjectType is not { } sourceType ||
            targetEnd.ApiResolvedObjectType is not { } targetType)
        {
            _clrNavigationMemberBinding.ClrMemberReference!.Validate(context, this.ApiPath);
            return;
        }

        if (!_clrNavigationMemberBinding.TryResolveReference
        (
            sourceType.ClrType,
            context,
            "Traversal CLR navigation member reference",
            requiresRead: true,
            requiresWrite: true,
            apiPath: this.ApiPath
        ))
        {
            return;
        }

        var memberType = _clrNavigationMemberBinding.ClrMemberType;
        var isCompatibleType = this.IsToMany
            ?
            (
                memberType.IsGenericType &&
                    memberType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                    memberType.GenericTypeArguments[0] == targetType.ClrType ||
                memberType.GetInterfaces().Any(candidate =>
                    candidate.IsGenericType &&
                    candidate.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                    candidate.GenericTypeArguments[0] == targetType.ClrType)
            )
            : memberType == targetType.ClrType;
        if (isCompatibleType)
        {
            return;
        }

        var apiPath = this.ApiPath;
        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ClrMemberIncompatible;
        var description = $"CLR navigation member '{this.ClrNavigationMember!.ClrName}' is "
            + $"incompatible with '{targetType.ClrType}'";
        var remediation = "Bind a readable and writable member with the traversal's target and cardinality";

        context.AddIssue(apiPath, severity, code, description, remediation);
        _clrNavigationMemberBinding.InvalidateAccessor();
    }
    #endregion
}
