// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;

namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiObjectTypeBuilderExtensions
{
    #region Methods
    public static void ConfigureIdentities(this ApiObjectTypeBuilder builder, ApiObjectType apiObjectType)
    {
        foreach (var apiKeyType in apiObjectType.ApiKeyTypes)
        {
            builder.AddKeyType(apiKeyType.ApiName, k =>
            {
                foreach (var keyPath in apiKeyType.ApiKeyPaths)
                {
                    k.AddKeyPath(keyPath.ClrRootType, keyPath.ApiSegments.Select(s => s.ClrPropertyName));
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
                if (apiOptions.ApiIdentityPartNullHandling.HasValue)
                {
                    var apiIdentityPartNullHandling = apiOptions.ApiIdentityPartNullHandling.Value;

                    switch (apiIdentityPartNullHandling)
                    {
                        case ApiIdentityPartNullHandling.ThrowOnNull:
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
    #endregion
}
