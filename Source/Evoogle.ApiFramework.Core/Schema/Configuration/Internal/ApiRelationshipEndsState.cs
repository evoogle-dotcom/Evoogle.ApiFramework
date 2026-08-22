// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores relationship principal and dependent end builders with their source metadata.
/// </summary>
internal sealed class ApiRelationshipEndsState
{
    #region Properties
    internal ApiRelationshipPrincipalEndBuilder? PrincipalEndA { get; set; }

    internal ApiRelationshipPrincipalEndBuilder? PrincipalEndB { get; set; }

    internal ApiRelationshipDependentEndBuilder? DependentEnd { get; set; }

    internal ApiRelationshipAssociationBuilder? Association { get; set; }

    internal ApiConfigurationSource? AssociationSource { get; set; }

    internal ApiConfigurationSource? PrincipalEndASource { get; set; }

    internal ApiConfigurationSource? PrincipalEndBSource { get; set; }

    internal ApiConfigurationSource? DependentEndSource { get; set; }
    #endregion
}
