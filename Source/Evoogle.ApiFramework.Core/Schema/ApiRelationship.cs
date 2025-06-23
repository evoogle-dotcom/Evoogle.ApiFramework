// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extension;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

public sealed class ApiRelationship(ApiRelationshipCardinality apiCardinality, ApiProperty apiProperty)
    : ExtensibleBase
{
    #region ApiRelationship Properties
    public ApiRelationshipCardinality ApiCardinality { get; } = apiCardinality;
    public ApiProperty ApiProperty { get; } = apiProperty ?? throw new ArgumentNullException(nameof(apiProperty));
    #endregion

    #region Object Methods
    public override string ToString()
    {
        var apiCardinality = this.ApiCardinality.SafeToString();
        var apiPropertyName = this.ApiProperty.ApiName.SafeToString();

        return $"{nameof(ApiRelationship)} {{{nameof(ApiCardinality)}={apiCardinality}, {nameof(ApiProperty)}={apiPropertyName}}}";
    }
    #endregion
}
