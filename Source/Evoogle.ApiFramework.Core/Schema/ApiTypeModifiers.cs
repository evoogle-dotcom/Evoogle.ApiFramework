// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents modifiers that affect the behavior or constraints of an API type (e.g., required/optional).
/// </summary>
[Flags]
public enum ApiTypeModifiers
{
    #region Values    
    /// <summary>Represents that the API type has no modifiers.</summary>
    None = 0,

    /// <summary>Represents the API instance (of API type) is required and will always be non-null.</summary>
    Required = 1,
    #endregion
}
