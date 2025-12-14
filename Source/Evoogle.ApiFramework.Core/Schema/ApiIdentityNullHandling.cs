// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Specifies how null property values should be handled when building an <see cref="ApiIdentity"/>.
/// </summary>
public enum ApiIdentityNullHandling
{
    /// <summary>
    ///     When a property value is null, return <see cref="Identity.ApiId.Empty"/> for that part.
    ///     This is the default behavior and allows partial identities.
    /// </summary>
    ReturnEmpty = 0,

    /// <summary>
    ///     When a property value is null, throw an <see cref="Exceptions.ApiIdentityException"/>
    ///     with details about the identity, property, and object type.
    /// </summary>
    ThrowException = 1
}
