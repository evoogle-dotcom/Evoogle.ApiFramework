// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
using Evoogle.Extension;

namespace Evoogle.ApiFramework.Schema;

public class ApiRelationship(ApiProperty apiProperty, ApiRelationshipCardinality apiCardinality)
    : ExtensibleBase
{
    #region ApiRelationship Properties
    public ApiProperty ApiProperty { get; } = apiProperty;
    public ApiRelationshipCardinality ApiCardinality { get; } = apiCardinality;
    #endregion

    #region Object Methods
    public override string ToString()
    {
        var apiPropertyName = this.ApiProperty.ApiName.SafeToString();
        var apiCardinality = this.ApiCardinality.SafeToString();

        return $"{nameof(ApiRelationship)} {{{nameof(ApiProperty)}={apiPropertyName}, {nameof(ApiCardinality)}={apiCardinality}}}";
    }
    #endregion
}