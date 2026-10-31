// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>Specifies whether a value permits null in a particular API context.</summary>
public enum ApiNullability
{
    #region Values
    /// <summary>Specifies that an API value may be null.</summary>
    Nullable = 0,

    /// <summary>Specifies that an API value must be non-null; does not require its presence in an input.</summary>
    NonNullable = 1,
    #endregion
}
