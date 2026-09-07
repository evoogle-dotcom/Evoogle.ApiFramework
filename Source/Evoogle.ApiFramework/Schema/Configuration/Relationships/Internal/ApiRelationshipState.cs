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
internal sealed class ApiRelationshipState(ApiRelationshipDeleteBehavior defaultDeleteBehavior)
{
    #region Properties
    internal ApiRelationshipDeleteBehavior DeleteBehavior { get; set; } = defaultDeleteBehavior;

    internal ApiConfigurationSource? DeleteBehaviorSource { get; set; }

    internal ApiConfigurationSource? RegistrationSource { get; set; }
    #endregion
}
