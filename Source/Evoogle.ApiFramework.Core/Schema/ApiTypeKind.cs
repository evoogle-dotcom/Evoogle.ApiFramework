// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents basic guidance about an API type.
/// </summary>
public enum ApiTypeKind
{
    #region Values
    /// <summary>Represents an unknown API type.</summary>
    Unknown,

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
