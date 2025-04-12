// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
using Evoogle.Extension;

namespace Evoogle.ApiFramework.Schema;

public class ApiProperty(string apiName, ApiType apiType, ApiTypeModifiers apiTypeModifiers, string clrName)
    : ExtensibleBase
{
    #region ApiProperty Properties
    public string ApiName { get; } = apiName;
    public ApiType ApiType { get; } = apiType;
    public ApiTypeModifiers ApiTypeModifiers { get; } = apiTypeModifiers;

    public string ClrName { get; } = clrName;
    #endregion

    #region Object Methods
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiType = this.ApiType.SafeToString();
        var apiTypeModifiers = this.ApiTypeModifiers.SafeToString();
        var clrName = this.ClrName.SafeToString();

        return $"{nameof(ApiProperty)} {{{nameof(ApiName)}={apiName}, {nameof(ApiType)}={apiType}, {nameof(ApiTypeModifiers)}={apiTypeModifiers}, {nameof(ClrName)}={clrName}}}";
    }
    #endregion
}