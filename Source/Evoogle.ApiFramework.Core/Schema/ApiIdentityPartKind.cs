// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Specifies how an <see cref="ApiIdentityPart"/> sources its value within an <see cref="ApiIdentity"/> definition.
/// </summary>
public enum ApiIdentityPartKind
{
    Scalar,

    Nested,

    Parent
}
