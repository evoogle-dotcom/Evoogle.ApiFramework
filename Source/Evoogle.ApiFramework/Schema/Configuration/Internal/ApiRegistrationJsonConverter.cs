// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

internal sealed class ApiRegistrationJsonConverter<TRegistration> : JsonConverter<TRegistration>
{
    #region Constructors
    public ApiRegistrationJsonConverter()
    {
    }
    #endregion

    #region JsonConverter<TRegistration> Methods
    public override TRegistration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException("Schema authoring registrations cannot be deserialized.");

    public override void Write(Utf8JsonWriter writer, TRegistration registration, JsonSerializerOptions options)
        => throw new NotSupportedException("Schema authoring registrations cannot be serialized.");
    #endregion
}
