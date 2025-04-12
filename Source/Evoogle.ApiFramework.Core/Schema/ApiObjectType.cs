// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API object that has API named properties that can be read/written from/to an API service.
/// </summary>
public class ApiObjectType(string apiName, IEnumerable<ApiProperty> apiProperties, Type clrObjectType)
    : ApiNamedType(apiName, clrObjectType)
{
    #region ApiType Properties
    public override ApiTypeKind Kind => ApiTypeKind.Object;
    #endregion

    #region ApiObject Properties
    public IEnumerable<ApiProperty> ApiProperties { get; } = apiProperties;
    #endregion

    #region Object Methods
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrType = this.ClrType.SafeToString();

        return $"{nameof(ApiObjectType)} {{{nameof(ApiName)}={apiName}}} [{clrType}]";
    }
    #endregion
}
