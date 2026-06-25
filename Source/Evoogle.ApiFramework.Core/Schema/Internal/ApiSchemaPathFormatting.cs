// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text;

namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiSchemaPathFormatting
{
    #region Utility Methods
    public static string BuildPath(string? apiBasePath, string apiPathSegment, string? apiPathSegmentName)
    {
        var hasBasePath = !string.IsNullOrWhiteSpace(apiBasePath);
        var hasSegmentName = !string.IsNullOrWhiteSpace(apiPathSegmentName);

        if (!hasBasePath)
        {
            return !hasSegmentName
                ? apiPathSegment
                : $"{apiPathSegment}[\"{apiPathSegmentName}\"]";
        }

        if (!hasSegmentName)
        {
            return $"{apiBasePath}.{apiPathSegment}";
        }

        var capacity = (hasBasePath ? apiBasePath!.Length + 1 : 0)
            + apiPathSegment.Length
            + (hasSegmentName ? apiPathSegmentName!.Length + 4 : 0);

        var stringBuilder = new StringBuilder(capacity);

        if (hasBasePath)
        {
            stringBuilder.Append(apiBasePath);
            stringBuilder.Append('.');
        }

        stringBuilder.Append(apiPathSegment);

        if (hasSegmentName)
        {
            stringBuilder.Append("[\"");
            stringBuilder.Append(apiPathSegmentName);
            stringBuilder.Append("\"]");
        }

        return stringBuilder.ToString();
    }
    #endregion
}
