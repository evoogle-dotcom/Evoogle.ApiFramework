// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Evoogle.ApiFramework.Schema;

public abstract class ApiNamedType(string apiName, Type clrType) : ApiType(clrType)
{
    #region ApiNamedType Properties
    public string ApiName { get; } = apiName;
    #endregion
}
