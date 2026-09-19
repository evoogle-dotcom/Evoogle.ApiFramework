// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
/// <remarks>
///     Provides the framework's exact, ordinal comparison policy for API names.
/// </remarks>
internal static class ApiNameComparer
{
    #region Properties
    public static StringComparer Instance { get; } = StringComparer.Ordinal;
    #endregion
}
