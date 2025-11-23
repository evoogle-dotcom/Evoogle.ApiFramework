// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extension;
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
public sealed class ApiRelationship(string apiName, string? apiPropertyName = null) : ExtensibleBase
{
    #region Fields
    private ApiSchemaContext? _apiSchemaContext = null;

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

    /// <summary>Gets the schema context for this relationship.</summary>
    internal ApiSchemaContext Context => this.ThrowIfNotInitialized(_apiSchemaContext);
    #endregion

    #region ApiRelationship Methods
    internal string GetValidationPath(string parentPath) => $"{parentPath.SafeToString()}.{nameof(ApiRelationship)}[\"{this.ApiName.SafeToString()}\"]";

    internal void Initialize(ApiSchema apiSchema, ApiSchemaContext apiSchemaContext, ApiObjectType apiObjectType, string apiValidationPath, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(apiSchemaContext);
        ArgumentNullException.ThrowIfNull(apiObjectType);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        _apiSchemaContext = apiSchemaContext;

        this.InitializeApiName(apiValidationPath, ref results);
        this.InitializeApiProperty(apiObjectType, apiValidationPath, ref results);
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
    private void InitializeApiName(string apiValidationPath, ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiName)} cannot be null or whitespace.", [nameof(this.ApiName)]));
        }
    }

    private void InitializeApiProperty(ApiObjectType apiObjectType, string apiValidationPath, ref List<ValidationResult>? results)
    {
        _apiResolvedProperty = null;

        if (string.IsNullOrWhiteSpace(this.ApiPropertyName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiPropertyName)} cannot be null or whitespace.", [nameof(this.ApiPropertyName)]));
        }
        else
        {
            // Resolve the related API property for the parent API object type.
            if (apiObjectType.TryGetPropertyByApiName(this.ApiPropertyName, out var apiResolvedProperty))
            {
                _apiResolvedProperty = apiResolvedProperty;
            }
        }

        if (_apiResolvedProperty is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiProperty)} unable to resolve {nameof(this.ApiProperty)}[\"{this.ApiPropertyName.SafeToString()}\"].", [nameof(this.ApiProperty)]));
        }
    }
    #endregion
}
