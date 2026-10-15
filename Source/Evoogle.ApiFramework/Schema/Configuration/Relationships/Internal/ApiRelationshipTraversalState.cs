// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Relationships;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Relationships.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
/// <remarks>
///     Holds reusable traversal configuration. A builder retains this state rather than an
///     <see cref="ApiRelationshipTraversal"/> because each built schema tree must own a distinct
///     traversal instance.
/// </remarks>
internal sealed class ApiRelationshipTraversalState(string apiName, ClrMemberReference? clrNavigationMember)
{
    #region Properties
    internal string ApiName { get; } = apiName;

    internal ClrMemberReference? ClrNavigationMember { get; } = clrNavigationMember;
    #endregion

    #region Build Methods
    internal ApiRelationshipTraversal Build() => new(this.ApiName, this.ClrNavigationMember);
    #endregion
}
