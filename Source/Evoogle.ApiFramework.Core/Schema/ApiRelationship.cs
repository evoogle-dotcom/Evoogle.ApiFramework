// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Abstract base class for all first-class relationships declared at the <see cref="ApiSchema"/> level.
/// </summary>
/// <remarks>
///     Concrete subclasses express specific structural kinds:
///     <see cref="ApiRelationshipOneToOne"/>, <see cref="ApiRelationshipOneToMany"/>,
///     and <see cref="ApiRelationshipManyToMany"/>.
///     The <see cref="ApiKind"/> property serves as the JSON polymorphic discriminator.
/// </remarks>
[JsonConverter(typeof(ApiRelationshipJsonConverter))]
public abstract class ApiRelationship
(
    string apiName,
    string? apiDisplayName = null,
    string? apiDescription = null
) : ApiSchemaElement
{
    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationship);
    #endregion

    #region ApiRelationship Properties
    /// <summary>
    ///     Gets the structural kind of this relationship. Used as the JSON polymorphic discriminator.
    /// </summary>
    public abstract ApiRelationshipKind ApiKind { get; }

    /// <summary>Gets the API name that uniquely identifies this relationship within the schema.</summary>
    public string ApiName { get; } = apiName;

    /// <summary>Gets the optional human-readable display name for this relationship.</summary>
    public string? ApiDisplayName { get; } = apiDisplayName;

    /// <summary>Gets the optional description for this relationship.</summary>
    public string? ApiDescription { get; } = apiDescription;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiKind = this.ApiKind.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationship)} {{{nameof(this.ApiKind)}={apiKind}, {nameof(this.ApiName)}={apiName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: nameof(ApiRelationship), segmentName: this.ApiName);

    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiName(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiName(ApiInitializationContext context)
    {
        if (!ApiSchemaHelpers.IsNameInvalid(this.ApiName))
        {
            return;
        }

        context.AddIssue(this.ApiPath, ApiInitializationSeverity.Error,
            ApiInitializationCode.API_RELATIONSHIP_INVALID_API_NAME,
            $"{nameof(this.ApiName)} must not be null, empty, or whitespace",
            $"Specify a valid {nameof(this.ApiName)} value");
    }
    #endregion
}
