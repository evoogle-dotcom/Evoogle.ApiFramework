// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a single enumeration value within an API enumeration type.
/// </summary>
/// <param name="ApiName">The API name of the enumeration value (typically used in API requests/responses).</param>
/// <param name="ClrName">The CLR name of the enumeration value (corresponding to the C# enum name).</param>
/// <param name="ClrOrdinal">The CLR ordinal (integer value) of the enumeration value.</param>
public sealed record ApiEnumValue(string ApiName, string ClrName, int ClrOrdinal);
