// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration.Relationships.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiRelationshipEndsState
{
    #region Properties
    internal ApiRelationshipPrincipalEndBuilder? PrincipalEndA { get; set; }

    internal ApiRelationshipPrincipalEndBuilder? PrincipalEndB { get; set; }

    internal ApiRelationshipDependentEndBuilder? DependentEnd { get; set; }

    internal ApiRelationshipAssociationBuilder? Association { get; set; }

    internal ApiConfigurationSource AssociationSource { get; set; } = ApiConfigurationSource.Convention;

    internal ApiConfigurationSource PrincipalEndASource { get; set; } = ApiConfigurationSource.Convention;

    internal ApiConfigurationSource PrincipalEndBSource { get; set; } = ApiConfigurationSource.Convention;

    internal ApiConfigurationSource DependentEndSource { get; set; } = ApiConfigurationSource.Convention;
    #endregion
}
