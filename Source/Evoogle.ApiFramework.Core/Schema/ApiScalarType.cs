// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API scalar (singular, individual, primitive) value.
/// </summary>
public class ApiScalarType(string apiName, Type clrScalarType)
    : ApiNamedType(apiName, clrScalarType)
{
    #region ApiType Properties
    public override ApiTypeKind Kind => ApiTypeKind.Scalar;
    #endregion

    #region Object Methods
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrType = this.ClrType.SafeToString();

        return $"{nameof(ApiScalarType)} {{{nameof(ApiName)}={apiName}}} [{clrType}]";
    }
    #endregion
}
