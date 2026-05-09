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
///     Abstract base class for all first-class relationships declared at the <see cref="ApiSchema"/> level.
/// </summary>
/// <remarks>
///     Concrete subclasses express specific structural kinds:
///     <see cref="ApiRelationshipOneToOne"/>, <see cref="ApiRelationshipOneToMany"/>,
///     and <see cref="ApiRelationshipManyToMany"/>.
///     The <see cref="ApiKind"/> property serves as the JSON polymorphic discriminator.
/// </remarks>
/// <param name="apiName">The API name that uniquely identifies this relationship within the schema.</param>
/// <param name="apiDeleteBehavior">The delete behavior that governs what happens to related objects when either end is affected.</param>
[JsonConverter(typeof(ApiRelationshipJsonConverter))]
public abstract class ApiRelationship(string apiName, ApiRelationshipDeleteBehavior apiDeleteBehavior) : ApiSchemaElement
{
    #region ApiRelationship Properties
    /// <summary>
    ///     Gets the structural kind of this relationship. Used as the JSON polymorphic discriminator.
    /// </summary>
    public abstract ApiRelationshipKind ApiKind { get; }

    /// <summary>Gets the API name that uniquely identifies this relationship within the schema.</summary>
    public string ApiName { get; } = apiName;

    /// <summary>
    ///     Gets the delete behavior that governs what happens to related objects when either end is affected.
    ///     <list type="bullet">
    ///         <item><description><strong>Principal deleted:</strong> what happens to dependent objects when the principal is deleted.</description></item>
    ///         <item><description><strong>Dependent orphaned:</strong> what happens to a dependent when it is removed from the relationship.</description></item>
    ///     </list>
    /// </summary>
    public ApiRelationshipDeleteBehavior ApiDeleteBehavior { get; } = apiDeleteBehavior;
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: this.ApiName);

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
        var isApiNameInvalid = ApiSchemaHelpers.IsNameInvalid(this.ApiName);
        if (isApiNameInvalid)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_INVALID_API_NAME;
            var description = $"{nameof(this.ApiName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion
}
