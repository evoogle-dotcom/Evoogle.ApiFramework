// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Key;

namespace Evoogle.ApiFramework.Schema.Json.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal static class ApiKeyDefinitionJsonConverterCore
{
    #region Types
    public readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required string ApiKeyPaths { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
        {
            return new PropertyNames
            {
                ApiKeyPaths = policy.ConvertName(nameof(ApiKeyDefinition.ApiKeyPaths))
            };
        }
        #endregion
    }

    public sealed class ReadData
    {
        #region Properties
        public List<ApiKeyPath>? ApiKeyPaths { get; set; }
        #endregion
    }
    #endregion

    #region Read Methods
    public static void ReadApiKeyPath
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
}
