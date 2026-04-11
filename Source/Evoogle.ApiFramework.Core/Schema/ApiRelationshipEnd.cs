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
[JsonConverter(typeof(ApiRelationshipEndJsonConverter))]
public abstract class ApiRelationshipEnd
(
    string apiObjectTypeName,
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

    /// <summary>Gets the API name of the participating <see cref="ApiObjectType"/> on this end of the relationship.</summary>
    public string ApiObjectTypeName { get; } = apiObjectTypeName;

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

        this.InitializeApiObjectTypeName(context);
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
    private void InitializeApiObjectTypeName(ApiInitializationContext context)
    {
        if (!ApiSchemaHelpers.IsNameInvalid(this.ApiObjectTypeName))
        {
            return;
        }

        context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
            ApiInitializationCode.API_RELATIONSHIP_END_INVALID_OBJECT_TYPE_NAME,
            $"{nameof(this.ApiObjectTypeName)} must not be null, empty, or whitespace",
            $"Specify a valid {nameof(this.ApiObjectTypeName)} value");
    }

    private void InitializeApiObjectType(ApiInitializationContext context)
    {
        _apiResolvedObjectType = null;

        if (ApiSchemaHelpers.IsNameInvalid(this.ApiObjectTypeName))
        {
            return;
        }

        if (context.ApiSchema.TryGetObjectTypeByApiName(this.ApiObjectTypeName, out var apiObjectType))
        {
            _apiResolvedObjectType = apiObjectType;
            return;
        }

        var availableTypes = string.Join(", ", context.ApiSchema.ApiObjectTypes.Select(t => $"'{t.ApiName}'"));
        var remediation = !string.IsNullOrEmpty(availableTypes)
            ? $"Use one of the available object types: {availableTypes}"
            : $"Define an {nameof(Schema.ApiObjectType)} with name '{this.ApiObjectTypeName}' in the schema";

        context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
            ApiInitializationCode.API_RELATIONSHIP_END_UNRESOLVED_OBJECT_TYPE,
            $"Object type '{this.ApiObjectTypeName}' could not be found in the schema",
            remediation);
    }
    #endregion
}
