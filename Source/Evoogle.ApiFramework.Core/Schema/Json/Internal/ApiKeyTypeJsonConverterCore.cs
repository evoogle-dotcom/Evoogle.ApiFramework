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
internal static class ApiKeyTypeJsonConverterCore
{
    #region Types
    internal delegate void ApiKeyPathArrayWriter
    (
        Utf8JsonWriter writer,
        IEnumerable<ApiKeyPath> apiKeyPaths,
        Action<ApiKeyPath> writeApiKeyPath
    );

    internal readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required string ApiKeyPaths { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
        {
            return new PropertyNames
            {
                ApiKeyPaths = policy.ConvertName(nameof(ApiKeyType.ApiKeyPaths))
            };
        }
        #endregion
    }

    internal sealed class ReadData
    {
        #region Properties
        public List<ApiKeyPath>? ApiKeyPaths { get; set; }
        #endregion
    }
    #endregion

    #region Read Methods
    internal static void ReadApiKeyPath
    (
        ref Utf8JsonReader reader,
        JsonSerializerOptions options,
        ICollection<ApiKeyPath> apiKeyPaths
    )
    {
        var apiKeyPath = JsonSerializer.Deserialize<ApiKeyPath>(ref reader, options);
        if (apiKeyPath is not null)
        {
            apiKeyPaths.Add(apiKeyPath);
        }
    }
    #endregion

    #region Write Methods
    internal static void WriteApiKeyPaths
    (
        Utf8JsonWriter writer,
        ApiKeyType apiKeyType,
        string propertyName,
        JsonSerializerOptions options,
        ApiKeyPathArrayWriter writeApiKeyPathArray
    )
    {
        var apiKeyPaths = apiKeyType.ApiKeyPaths;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            apiKeyPaths,
            options,
            collection => writeApiKeyPathArray
            (
                writer,
                collection,
                item => writer.TryWriteWithSerializer(item, options)
            )
        );
    }
    #endregion
}
