// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Query;

/// <summary>Declares the composition permitted over a query source's output.</summary>
[Flags]
public enum ApiQueryComposition
{
    #region Values
    /// <summary>No further composition is permitted.</summary>
    None = 0,

    /// <summary>Output may be projected into a selected result shape.</summary>
    Projection = 1,

    /// <summary>Collection output may be filtered.</summary>
    Filtering = 2,

    /// <summary>Collection output may be ordered.</summary>
    Ordering = 4,

    /// <summary>Collection output may use offset and limit pagination.</summary>
    OffsetPagination = 8
    #endregion
}
