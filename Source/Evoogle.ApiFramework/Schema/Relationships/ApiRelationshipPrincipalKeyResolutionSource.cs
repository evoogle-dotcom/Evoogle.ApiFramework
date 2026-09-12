// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Relationships;

/// <summary>
///     Describes how a relationship's principal key was selected for a resolved key binding.
/// </summary>
public enum ApiRelationshipPrincipalKeyResolutionSource
{
    /// <summary>The principal key was selected by an explicit principal key name.</summary>
    Explicit,

    /// <summary>The principal key was inferred from the foreign key shape.</summary>
    Inferred,
}
