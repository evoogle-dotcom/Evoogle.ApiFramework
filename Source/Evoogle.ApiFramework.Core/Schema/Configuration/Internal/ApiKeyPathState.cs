// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores the root type and ordered segments for a key path builder.
/// </summary>
internal sealed class ApiKeyPathState(Type clrRootType, IEnumerable<ApiKeyPathSegmentBuilder> segmentBuilders)
{
    #region Properties
    internal Type ClrRootType { get; } = clrRootType ?? throw new ArgumentNullException(nameof(clrRootType));

    internal List<ApiKeyPathSegmentBuilder> SegmentBuilders { get; } = [.. segmentBuilders ?? throw new ArgumentNullException(nameof(segmentBuilders))];
    #endregion
}
