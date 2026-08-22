// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores common relationship configuration and source precedence metadata.
/// </summary>
internal sealed class ApiRelationshipState(ApiRelationshipDeleteBehavior defaultDeleteBehavior)
{
    #region Properties
    internal ApiRelationshipDeleteBehavior DeleteBehavior { get; set; } = defaultDeleteBehavior;

    internal ApiConfigurationSource? DeleteBehaviorSource { get; set; }

    internal ApiConfigurationSource? RegistrationSource { get; set; }
    #endregion
}
