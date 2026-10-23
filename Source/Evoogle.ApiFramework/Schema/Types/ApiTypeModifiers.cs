// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>
///     Represents modifiers that affect the behavior or constraints of an API type (e.g., nullability).
/// </summary>
[Flags]
public enum ApiTypeModifiers
{
    #region Values
    /// <summary>Represents that the API type has no modifiers.</summary>
    None = 0,

    /// <summary>Specifies that an API value must be non-null; does not require its presence in an input.</summary>
    Required = 1,
    #endregion
}
