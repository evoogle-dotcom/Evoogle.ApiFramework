// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Specifies the kinds of identifiers that can be used in the API framework.
/// </summary>
public enum ApiIdKind : byte
{
    #region Values
    None,
    Composite,
    Culture,
    Guid,
    Int32,
    Int64,
    String,
    Ulid,
    #endregion
}
