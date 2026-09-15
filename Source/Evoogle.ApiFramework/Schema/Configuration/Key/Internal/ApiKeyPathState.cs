// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Key.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiKeyPathState
(
    ApiTypeReference? apiRootObjectTypeReference,
    IEnumerable<ApiKeyPathSegmentBuilder> segmentBuilders
)
{
    #region Properties
    internal ApiTypeReference? ApiRootObjectTypeReference { get; } =
        apiRootObjectTypeReference;

    internal List<ApiKeyPathSegmentBuilder> SegmentBuilders { get; } = [.. segmentBuilders ?? throw new ArgumentNullException(nameof(segmentBuilders))];
    #endregion
}
