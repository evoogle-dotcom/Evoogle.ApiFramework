// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores the two foreign-key configurations for a relationship association.
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
