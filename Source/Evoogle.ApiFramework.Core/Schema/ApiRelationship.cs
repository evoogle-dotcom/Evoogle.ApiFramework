// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents semantic metadata for a named relationship within an <see cref="ApiObjectType"/> that
///     expresses object-to-object linkage and cardinality.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiRelationship"/> refers to a specific <see cref="ApiProperty"/> on the parent object
///         and interprets it as a navigational link to another object type. Relationships capture
///         semantic context (e.g., "customer → orders", "user → roles") that goes beyond the property's
///         structural definition.
///     </para>
/// </remarks>
[JsonConverter(typeof(ApiRelationshipJsonConverter))]
public sealed class ApiRelationship(string apiName, string? apiPropertyName = null) : ApiSchemaElement
{
    #region Fields
    private readonly string? _apiPropertyName = apiPropertyName;

    private ApiProperty? _apiResolvedProperty = null;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the API name of the relationship.
    /// </summary>
    public string ApiName { get; } = apiName;

    /// <summary>
    ///     Gets the API property name this relationship refers to.
    ///     If no property name is explicitly provided, defaults to <see cref="ApiName"/>.
    /// </summary>
    public string ApiPropertyName => _apiPropertyName ?? this.ApiName;

    /// <summary>
    ///     Gets the resolved <see cref="ApiProperty"/> backing this relationship.
    ///     This property must be resolved first using <see cref="Initialize"/>.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if the property has not yet been resolved via <see cref="Initialize"/>.
    /// </exception>
    public ApiProperty ApiProperty => this.ThrowIfNotInitialized(_apiResolvedProperty);

    /// <summary>
    ///     Gets the relationship cardinality (<see cref="ApiRelationshipCardinality.ToOne"/> or <see cref="ApiRelationshipCardinality.ToMany"/>) based on the <see cref="ApiProperty.ApiType"/> kind.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if the property type is not an object or collection.
    /// </exception>
    public ApiRelationshipCardinality ApiCardinality => this.ApiProperty.ApiType.Kind switch
    {
        ApiTypeKind.Object => ApiRelationshipCardinality.ToOne,
        ApiTypeKind.Collection => ApiRelationshipCardinality.ToMany,
        _ => throw new ApiSchemaException($"Unsupported {nameof(ApiTypeKind)}: {this.ApiProperty.ApiType.Kind.SafeToString()} for {this}. Only Object and Collection types are supported.")
    };
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiParentPath)
        => ApiSchemaHelpers.BuildPath(apiParentPath, apiChildPath: nameof(ApiRelationship), apiApiName: this.ApiName);

    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiName(context);
        this.InitializeApiProperty(context);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiPropertyName = this.ApiPropertyName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        if (apiName.Equals(apiPropertyName, StringComparison.OrdinalIgnoreCase))
        {
            return $"{nameof(ApiRelationship)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
        }
        else
        {
            return $"{nameof(ApiRelationship)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiPropertyName)}={apiPropertyName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
        }
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiName(ApiInitializationContext context)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiName)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_INVALID_NAME;
            var description = $"{nameof(this.ApiName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeApiProperty(ApiInitializationContext context)
    {
        _apiResolvedProperty = null;

        if (!string.IsNullOrWhiteSpace(this.ApiPropertyName))
        {
            // Resolve the related API property for the parent API object type.
            var apiParentObjectType = context.ApiParentObjectType;
            if (apiParentObjectType.TryGetPropertyByApiName(this.ApiPropertyName, out var apiResolvedProperty))
            {
                _apiResolvedProperty = apiResolvedProperty;
            }
        }

        if (_apiResolvedProperty is null)
        {
            var apiObjectTypeName = context.ApiParentObjectType.ApiName.SafeToString();

            var path = $"{this.ApiPath}.{nameof(this.ApiProperty)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_RELATIONSHIP_UNRESOLVED_PROPERTY;
            var description = $"{nameof(this.ApiProperty)} could not be resolved for {nameof(this.ApiPropertyName)}='{this.ApiPropertyName.SafeToString()}' on parent {nameof(ApiObjectType)}='{apiObjectTypeName}'";
            var remediation = $"Verify that {nameof(this.ApiPropertyName)} refers to a valid property on parent {nameof(ApiObjectType)}='{apiObjectTypeName}'";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion
}
