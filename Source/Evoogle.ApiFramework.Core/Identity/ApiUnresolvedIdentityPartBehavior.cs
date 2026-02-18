// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Specifies how to handle unresolved identity parts when flattening an identity snapshot.
/// </summary>
public enum ApiUnresolvedIdentityPartBehavior
{
    /// <summary>
    ///     Throw an exception when encountering an unresolved part.
    /// </summary>
    Throw,

    /// <summary>
    ///     Allow unresolved parts by using ApiId.Empty.
    ///     The structure will be preserved by emitting ApiId.Empty for all scalar leaves according to the part's Structure definition.
    /// </summary>
    AllowUnresolved
}
