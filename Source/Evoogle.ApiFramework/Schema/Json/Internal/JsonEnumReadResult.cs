// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Json.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
/// <remarks>
///     The default value represents a member that was not present in the JSON.
/// </remarks>
internal readonly record struct JsonEnumReadResult<TEnum>(JsonEnumReadStatus Status, TEnum? Value, string? RawText)
    where TEnum : struct, Enum
{
    #region Methods
    public TEnum ValueOr(TEnum fallbackValue) => this.Value ?? fallbackValue;
    #endregion
}
