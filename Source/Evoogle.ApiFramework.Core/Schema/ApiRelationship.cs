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

public sealed class ApiRelationship(ApiPropertyExpression apiPropertyExpression) : ExtensibleBase
{
    #region ApiRelationship Properties
    public ApiPropertyExpression ApiPropertyExpression { get; } = apiPropertyExpression ?? throw new ArgumentNullException(nameof(apiPropertyExpression));

    public ApiProperty ApiProperty => this.ApiPropertyExpression.ApiResolvedProperty ?? throw new ApiSchemaException($"{nameof(this.ApiPropertyExpression)} has not been resolved yet.");

    public ApiRelationshipCardinality ApiCardinality => this.ApiProperty.ApiType.Kind switch
    {
        ApiTypeKind.Object => ApiRelationshipCardinality.ToOne,
        ApiTypeKind.Collection => ApiRelationshipCardinality.ToMany,
        _ => throw new ApiSchemaException($"Unsupported API type kind: {this.ApiProperty.ApiType.Kind.SafeToString()} for relationship '{this.ApiPropertyExpression.SafeToString()}'. Only Object and Collection types are supported.")
    };
    #endregion

    #region ApiRelationship Methods
    public void Resolve(ApiObjectType apiObjectType, ref List<ValidationResult>? results) => this.ApiPropertyExpression.Resolve(apiObjectType, ref results);
    #endregion

    #region Object Methods
    public override string ToString()
    {
        var apiPropertyExpression = this.ApiPropertyExpression.SafeToString();

        return $"{nameof(ApiRelationship)} {{{nameof(this.ApiPropertyExpression)}={apiPropertyExpression}}}";
    }
    #endregion
}
