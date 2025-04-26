// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents basic guidance about an API type.
/// </summary>
public enum ApiTypeKind
{
    #region Values
    /// <summary>Represents the API collection type.</summary>
    Collection,

    /// <summary>Represents the API enumeration type.</summary>
    Enum,

    /// <summary>Represents the API object type.</summary>
    Object,

    /// <summary>Represents the API scalar type.</summary>
    Scalar
    #endregion
}
