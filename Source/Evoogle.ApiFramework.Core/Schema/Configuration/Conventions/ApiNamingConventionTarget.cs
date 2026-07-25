// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Identifies the schema element kind whose API name is being produced.
/// </summary>
public enum ApiNamingConventionTarget
{
    /// <summary>An <see cref="ApiObjectType"/> API name.</summary>
    ObjectType,

    /// <summary>An <see cref="ApiScalarType"/> API name.</summary>
    ScalarType,

    /// <summary>An <see cref="ApiEnumType"/> API name.</summary>
    EnumType,

    /// <summary>An <see cref="ApiProperty"/> API name.</summary>
    Property,

    /// <summary>An <see cref="ApiEnumValue"/> API name.</summary>
    EnumValue,
}
