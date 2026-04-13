// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Abstract base class for the two ends of an <see cref="ApiRelationship"/>.
///     Each end describes one participating <see cref="ApiObjectType"/> and the delete behavior
///     applied to objects at the opposite end when an object on this end is deleted.
/// </summary>
/// <param name="clrObjectType">The CLR type of the participating <see cref="ApiObjectType"/> on this end of the relationship.</param>
/// <param name="apiDeleteBehavior">The delete behavior applied to objects at the opposite end when an object on this end is deleted.</param>
[JsonConverter(typeof(ApiRelationshipEndJsonConverter))]
public abstract class ApiRelationshipEnd
(
    Type clrObjectType,
    ApiRelationshipDeleteBehavior apiDeleteBehavior = ApiRelationshipDeleteBehavior.None
) : ApiSchemaElement
{
    #region ApiRelationshipEnd Fields
    private ApiObjectType? _apiResolvedObjectType = null;
    private ApiRelationship? _apiResolvedRelationship = null;
    private ApiRelationshipEnd? _apiResolvedOppositeEnd = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => this.GetType().Name;
    #endregion

    #region ApiRelationshipEnd Properties
    /// <summary>Gets the kind of this relationship end, either <see cref="ApiRelationshipEndKind.Principal"/> or <see cref="ApiRelationshipEndKind.Dependent"/>.</summary>
    public abstract ApiRelationshipEndKind ApiKind { get; }

    /// <summary>Gets the CLR type of the participating <see cref="ApiObjectType"/> on this end of the relationship.</summary>
    public Type ClrObjectType { get; } = clrObjectType;

    /// <summary>
    ///     Gets the delete behavior applied to objects at the opposite end when an object on this end is deleted.
    /// </summary>
    public ApiRelationshipDeleteBehavior ApiDeleteBehavior { get; } = apiDeleteBehavior;

    /// <summary>Gets the resolved <see cref="ApiObjectType"/> for this end. Available after initialization.</summary>
    public ApiObjectType ApiObjectType => this.ThrowIfNotInitialized(_apiResolvedObjectType);

    /// <summary>Gets the parent <see cref="ApiRelationship"/> that owns this end. Available after initialization.</summary>
    public ApiRelationship ApiRelationship => this.ThrowIfNotInitialized(_apiResolvedRelationship);

    /// <summary>Gets the opposite end of the relationship. Available after initialization.</summary>
    public ApiRelationshipEnd ApiOppositeEnd => this.ThrowIfNotInitialized(_apiResolvedOppositeEnd);

    protected ApiObjectType? ResolvedObjectType => _apiResolvedObjectType;
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: null);

    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeClrObjectType(context);
        this.InitializeApiObjectType(context);
    }
    #endregion

    #region ApiRelationshipEnd Internal Methods
    internal void WireBackReferences(ApiRelationship relationship, ApiRelationshipEnd oppositeEnd)
    {
        ArgumentNullException.ThrowIfNull(relationship);
        ArgumentNullException.ThrowIfNull(oppositeEnd);

        _apiResolvedRelationship = relationship;
        _apiResolvedOppositeEnd = oppositeEnd;
    }
    #endregion

    #region Implementation Methods
    private void InitializeClrObjectType(ApiInitializationContext context)
    {
        if (this.ClrObjectType is not null)
        {
            return;
        }

        context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
            ApiInitializationCode.API_RELATIONSHIP_END_NULL_CLR_OBJECT_TYPE,
            $"{nameof(this.ClrObjectType)} must not be null",
            $"Specify a valid {nameof(this.ClrObjectType)} value");
    }

    private void InitializeApiObjectType(ApiInitializationContext context)
    {
        _apiResolvedObjectType = null;

        if (this.ClrObjectType is null)
        {
            return;
        }

        if (context.ApiSchema.TryGetObjectTypeByClrType(this.ClrObjectType, out var apiObjectType))
        {
            _apiResolvedObjectType = apiObjectType;
            return;
        }

        var availableTypes = string.Join(", ", context.ApiSchema.ApiObjectTypes.Select(t => $"'{t.ApiName}' ({t.ClrType.Name})"));
        var remediation = !string.IsNullOrEmpty(availableTypes)
            ? $"Use one of the available object types: {availableTypes}"
            : $"Define an {nameof(Schema.ApiObjectType)} for CLR type '{this.ClrObjectType.FullName}' in the schema";

        context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
            ApiInitializationCode.API_RELATIONSHIP_END_UNRESOLVED_OBJECT_TYPE,
            $"No {nameof(Schema.ApiObjectType)} is registered for CLR type '{this.ClrObjectType.FullName}'",
            remediation);
    }
    #endregion
}
