// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Types.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiObjectTypeBuilderExtensions
{
    #region Methods
    public static void ConfigureKeys(this ApiObjectTypeBuilder builder, ApiObjectType apiObjectType)
    {
        foreach (var apiKeyDefinition in apiObjectType.ApiKeys)
        {
            builder.AddKey(apiKeyDefinition.ApiName!, k =>
            {
                foreach (var keyPath in apiKeyDefinition.ApiKeyPaths)
                {
                    var apiPropertyReferences = keyPath.ApiSegments.Select
                        (s => s.ApiPropertyReference);
                    if (keyPath.ApiRootObjectTypeReference is null)
                    {
                        k.AddPath(apiPropertyReferences);
                    }
                    else
                    {
                        k.AddPath(keyPath.ApiRootObjectTypeReference, apiPropertyReferences);
                    }
                }
            });
        }
    }

    public static void ConfigureOptions(this ApiObjectTypeBuilder builder, ApiObjectType apiObjectType)
    {
        var apiOptions = apiObjectType.ApiOptions;
        if (apiOptions is not null)
        {
            builder.WithOptions(optionsBuilder =>
            {
                if (apiOptions.ApiKeyNullHandling.HasValue)
                {
                    var apiKeyNullHandling = apiOptions.ApiKeyNullHandling.Value;

                    switch (apiKeyNullHandling)
                    {
                        case ApiKeyNullHandling.ThrowOnNull:
                            optionsBuilder.ThrowOnNullKeyPart();
                            break;

                        default:
                            optionsBuilder.UseDefaultOnNullKeyPart();
                            break;
                    }
                }
            });
        }
    }

    public static void ConfigureVersion
    (
        this ApiObjectTypeBuilder builder,
        ApiObjectType apiObjectType
    )
    {
        var apiVersionDefinition = apiObjectType.ApiVersion;
        if (apiVersionDefinition is null)
        {
            return;
        }

        var apiPropertyReference = apiVersionDefinition.ApiPropertyReference;
        if (apiPropertyReference is null)
        {
            builder.WithRepositoryVersion
            (
                apiVersionDefinition.ClrType,
                x => x.ConfigureExtensions(apiVersionDefinition)
            );
        }
        else
        {
            builder.WithVersion
            (
                apiPropertyReference,
                x => x.ConfigureExtensions(apiVersionDefinition)
            );
        }
    }
    #endregion
}
