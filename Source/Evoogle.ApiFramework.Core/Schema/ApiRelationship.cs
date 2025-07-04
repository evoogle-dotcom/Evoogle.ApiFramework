// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extension;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a named API relationship that optionally resolves to a specific <see cref="ApiProperty"/> defined on an <see cref="ApiObjectType"/>.
///     Relationships abstract navigation and cardinality.
/// </summary>
public sealed class ApiRelationship(string apiName, string? apiPropertyName = null) : ExtensibleBase
{
    #region Fields
    private ApiProperty? _apiResolvedProperty = null;

    private readonly string? _apiPropertyName = apiPropertyName;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the API name of the relationship.
    /// </summary>
    public string ApiName { get; } = apiName ?? throw new ArgumentNullException(nameof(apiName), $"{nameof(apiName)} cannot be null.");
    #endregion

    #region Computed Properties
    /// <summary>
    ///     Gets the API property name this relationship refers to.
    ///     If no property name is explicitly provided, defaults to <see cref="ApiName"/>.
    /// </summary>
    public string ApiPropertyName => _apiPropertyName ?? this.ApiName;

    /// <summary>
    ///     Gets the resolved <see cref="ApiProperty"/> backing this relationship.
    ///     This property must be resolved first using <see cref="Resolve"/>.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if the property has not yet been resolved via <see cref="Resolve"/>.
    /// </exception>
    public ApiProperty ApiProperty => _apiResolvedProperty ?? throw new ApiSchemaException($"{nameof(ApiRelationship)} has not been resolved yet.");

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

    #region ApiRelationship Methods
    /// <summary>
    ///     Resolves the relationship by binding it to a property of the given <see cref="ApiObjectType"/>.
    ///     Adds a <see cref="ValidationResult"/> to <paramref name="results"/> if the referenced property cannot be found.
    /// </summary>
    /// <param name="apiObjectType">The API object type containing the property.</param>
    /// <param name="results">An optional list to which validation errors are appended.</param>    
    public void Resolve(ApiObjectType apiObjectType, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiObjectType);

        // Lookup the API property by API name.
        if (!apiObjectType.TryGetPropertyByApiName(this.ApiPropertyName, out var apiProperty))
        {
#pragma warning disable IDE0009 // Member access should be qualified.
            var message = $"Failed to lookup {nameof(ApiProperty)} '{this.ApiPropertyName.SafeToString()}' from {apiObjectType.SafeToString()}.";
#pragma warning restore IDE0009 // Member access should be qualified.

            results ??= [];
            results.Add(new ValidationResult(message, [nameof(this.ApiProperty)]));
            return;
        }

        // If we found the property, we have resolved it.
        _apiResolvedProperty = apiProperty;
    }
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiPropertyName = this.ApiPropertyName.SafeToString();

        if (apiName.Equals(apiPropertyName, StringComparison.OrdinalIgnoreCase))
            return $"{nameof(ApiRelationship)} {{{nameof(this.ApiName)}={apiName}}}";
        else
            return $"{nameof(ApiRelationship)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiPropertyName)}={apiPropertyName}}}";
    }
    #endregion
}
