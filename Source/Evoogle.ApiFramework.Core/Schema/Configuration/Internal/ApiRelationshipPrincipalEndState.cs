// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores principal-end key selection configuration.
/// </summary>
internal sealed class ApiRelationshipPrincipalEndState
{
    #region Properties
    internal string? PrincipalKeyTypeName { get; set; }

    internal ApiConfigurationSource? PrincipalKeyTypeNameSource { get; set; }
    #endregion
}
