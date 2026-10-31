// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>Specifies whether a member must be supplied in its containing contract.</summary>
public enum ApiPresence
{
    #region Values
    /// <summary>The member may be omitted.</summary>
    Optional = 0,

    /// <summary>The member must be supplied, independently of whether its value permits null.</summary>
    Required = 1,
    #endregion
}
