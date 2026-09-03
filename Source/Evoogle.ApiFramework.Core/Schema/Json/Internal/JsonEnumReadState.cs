// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Json;

namespace Evoogle.ApiFramework.Schema.Json.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal sealed class JsonEnumReadState<TEnum>
    where TEnum : struct, Enum
{
    #region Properties
    public bool IsInvalid => this.WasPresent && !this.IsNull && this.Value is null;

    public bool IsNull { get; private set; }

    public TEnum? Value { get; private set; }

    public bool WasPresent { get; private set; }
    #endregion

    #region Methods
    public void Read
    (
        ref Utf8JsonReader reader,
        JsonSerializerOptions options,
        NullableEnumJsonConverter<TEnum> converter
    )
    {
        this.WasPresent = true;
        this.IsNull = reader.TokenType == JsonTokenType.Null;
        this.Value = this.IsNull ? null : converter.Read(ref reader, typeof(TEnum?), options);
    }
    #endregion
}
