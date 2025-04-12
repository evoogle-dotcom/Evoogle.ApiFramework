// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API collection.
/// </summary>
public class ApiCollectionType(ApiType apiItemType, ApiTypeModifiers apiItemTypeModifiers, Type clrCollectionType)
    : ApiType(clrCollectionType)
{
    #region ApiType Properties
    public override ApiTypeKind Kind => ApiTypeKind.Collection;
    #endregion

    #region ApiObject Properties
    public ApiType ApiItemType { get; } = apiItemType;
    public ApiTypeModifiers ApiItemTypeModifiers { get; } = apiItemTypeModifiers;
    #endregion

    #region Object Methods
    public override string ToString()
    {
        var apiItemType = this.ApiItemType.SafeToString();
        var apiItemTypeModifiers = this.ApiItemTypeModifiers.SafeToString();
        var clrType = this.ClrType.SafeToString();

        return $"{nameof(ApiCollectionType)} {{{nameof(ApiItemType)}={apiItemType}, {nameof(ApiTypeModifiers)}={apiItemTypeModifiers}}} [{clrType}]";
    }
    #endregion
}
