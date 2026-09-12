// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Key;

namespace Evoogle.ApiFramework.Schema.Configuration.Relationships.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiRelationshipAssociationState
{
    #region Properties
    internal ApiKeyTypeBuilder? ForeignKeyTypeBuilderA { get; set; }

    internal ApiKeyTypeBuilder? ForeignKeyTypeBuilderB { get; set; }

    internal ApiConfigurationSource? ForeignKeyTypeBuilderASource { get; set; }

    internal ApiConfigurationSource? ForeignKeyTypeBuilderBSource { get; set; }
    #endregion
}
