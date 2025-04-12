// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Evoogle.ApiFramework.Schema;

public class ApiEnumValue(string apiName, string clrName, int clrOrdinal)
{
    #region ApiEnumValue Properties
    public string ApiName { get; } = apiName;
    public string ClrName { get; } = clrName;
    public int ClrOrdinal { get; } = clrOrdinal;
    #endregion

    #region Object Methods
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrName = this.ClrName.SafeToString();
        var clrOrdinal = this.ClrOrdinal.SafeToString();

        return $"{nameof(ApiEnumValue)} {{{nameof(ApiName)}={apiName}, {nameof(ClrName)}={clrName}, {nameof(ClrOrdinal)}={clrOrdinal}}}";
    }
    #endregion
}