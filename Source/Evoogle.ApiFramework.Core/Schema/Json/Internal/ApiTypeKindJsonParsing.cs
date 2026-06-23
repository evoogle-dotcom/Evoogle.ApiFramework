// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Logging;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiTypeKindJsonParsing
{
    #region Utility Methods
    public static ApiTypeKind? GetApiTypeKind(ILogger logger, string? kindAsString)
    {
        if (kindAsString is null)
        {
            return null;
        }

        if (Enum.TryParse<ApiTypeKind>(kindAsString, out var kind) == false)
        {
            logger.LogError("Unable to parse {Kind} enumeration string: '{KindAsString}'", nameof(ApiTypeKind), kindAsString);
            return null;
        }

        return kind;
    }
    #endregion
}
