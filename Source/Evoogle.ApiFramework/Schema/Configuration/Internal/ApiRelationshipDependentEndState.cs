// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores foreign-key configuration for a relationship dependent end.
/// </summary>
internal sealed class ApiRelationshipDependentEndState
{
    #region Properties
    internal ApiKeyTypeBuilder? ForeignKeyTypeBuilder { get; set; }

    internal ApiConfigurationSource? ForeignKeyTypeBuilderSource { get; set; }
    #endregion
}
