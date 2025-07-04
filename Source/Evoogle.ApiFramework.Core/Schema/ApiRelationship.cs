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

public sealed class ApiRelationship(string apiName, string? apiPropertyName = null) : ExtensibleBase
{
    #region Fields
    private ApiProperty? _apiResolvedProperty = null;

    private readonly string? _apiBackingPropertyName = apiPropertyName;
    #endregion

    #region Properties
    public string ApiName { get; } = apiName ?? throw new ArgumentNullException(nameof(apiName), $"{nameof(apiName)} cannot be null.");
    #endregion

    #region Computed Properties
    public string ApiPropertyName => _apiBackingPropertyName ?? this.ApiName;

    public ApiProperty ApiProperty => _apiResolvedProperty ?? throw new ApiSchemaException($"{nameof(ApiRelationship)} has not been resolved yet.");

    public ApiRelationshipCardinality ApiCardinality => this.ApiProperty.ApiType.Kind switch
    {
        ApiTypeKind.Object => ApiRelationshipCardinality.ToOne,
        ApiTypeKind.Collection => ApiRelationshipCardinality.ToMany,
        _ => throw new ApiSchemaException($"Unsupported API type kind: {this.ApiProperty.ApiType.Kind.SafeToString()} for {this}. Only Object and Collection types are supported.")
    };
    #endregion

    #region ApiRelationship Methods
    public void Resolve(ApiObjectType apiObjectType, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiObjectType);

        // Lookup the API property by API name.
        if (!apiObjectType.TryGetPropertyByApiName(this.ApiPropertyName, out var apiProperty))
        {
            var message = $"Failed to lookup API property '{this.ApiPropertyName.SafeToString()}' from {apiObjectType.SafeToString()}.";

            results ??= [];
            results.Add(new ValidationResult(message, [nameof(ApiProperty)]));
            return;
        }

        // If we found the property, we have resolved it.
        _apiResolvedProperty = apiProperty;
    }
    #endregion

    #region Object Methods
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
