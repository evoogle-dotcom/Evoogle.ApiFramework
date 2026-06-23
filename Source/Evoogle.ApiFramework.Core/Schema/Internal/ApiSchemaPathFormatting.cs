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
    public static string BuildPath(string? basePath, string segment, string? segmentName)
    {
        var hasBasePath = !string.IsNullOrWhiteSpace(basePath);
        var hasSegmentName = !string.IsNullOrWhiteSpace(segmentName);

        if (!hasBasePath)
        {
            return !hasSegmentName
                ? segment
                : $"{segment}[\"{segmentName}\"]";
        }

        if (!hasSegmentName)
        {
            return $"{basePath}.{segment}";
        }

        var capacity = (hasBasePath ? basePath!.Length + 1 : 0)
            + segment.Length
            + (hasSegmentName ? segmentName!.Length + 4 : 0);

        var stringBuilder = new StringBuilder(capacity);

        if (hasBasePath)
        {
            stringBuilder.Append(basePath);
            stringBuilder.Append('.');
        }

        stringBuilder.Append(segment);

        if (hasSegmentName)
        {
            stringBuilder.Append("[\"");
            stringBuilder.Append(segmentName);
            stringBuilder.Append("\"]");
        }

        return stringBuilder.ToString();
    }
    #endregion
}
