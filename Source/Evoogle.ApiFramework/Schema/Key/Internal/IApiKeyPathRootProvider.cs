// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Key.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
/// <remarks>
///     Supplies the inferred root object type and diagnostic label for key paths in an enclosing
///     schema context.
/// </remarks>
internal interface IApiKeyPathRootProvider
{
    #region Properties
    /// <summary>Gets the inferred root object type, or null when it is unavailable.</summary>
    ApiObjectType? RootObjectType { get; }

    /// <summary>Gets the label that identifies the inferred root in a key path.</summary>
    string? RootLabel { get; }
    #endregion
}
