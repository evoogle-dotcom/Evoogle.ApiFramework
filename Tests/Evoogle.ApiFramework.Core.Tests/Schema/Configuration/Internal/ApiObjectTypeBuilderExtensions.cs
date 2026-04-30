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
    public static void ConfigureExtensions(this ApiObjectTypeBuilder builder, ApiObjectType apiObjectType)
    {
        var extensions = apiObjectType.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddObjectExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureIdentities(this ApiObjectTypeBuilder builder, ApiObjectType apiObjectType)
    {
        var apiIdentities = apiObjectType.ApiIdentities;
        foreach (var apiIdentity in apiIdentities ?? [])
        {
            builder.AddIdentity(apiIdentity.ApiName, identityBuilder =>
            {
                foreach (var apiIdentityPart in apiIdentity.ApiIdentityParts)
                {
                    var apiKind = apiIdentityPart.ApiKind;
                    switch (apiKind)
                    {
                        case ApiIdentityPartKind.Scalar:
                            {
                                var scalarPart = (ApiIdentityScalarPart)apiIdentityPart;
                                var clrScalarTypeHint = scalarPart.ClrScalarTypeHint;
                                if (clrScalarTypeHint is not null)
                                {
                                    identityBuilder.AddScalarPart(scalarPart.ClrPropertyName, clrScalarTypeHint);
                                }
                                else
                                {
                                    identityBuilder.AddScalarPart(scalarPart.ClrPropertyName);
                                }
                                break;
                            }

                        case ApiIdentityPartKind.Nested:
                            {
                                var nestedPart = (ApiIdentityNestedPart)apiIdentityPart;
                                var apiIdentityName = nestedPart.ApiIdentityName;
                                if (apiIdentityName is not null)
                                {
                                    identityBuilder.AddNestedPart(nestedPart.ClrPropertyName, apiIdentityName);
                                }
                                else
                                {
                                    identityBuilder.AddNestedPart(nestedPart.ClrPropertyName);
                                }
                                break;
                            }

                        case ApiIdentityPartKind.Owner:
                            {
                                var ownerPart = (ApiIdentityOwnerPart)apiIdentityPart;
                                var apiIdentityName = ownerPart.ApiIdentityName;
                                if (apiIdentityName is not null)
                                {
                                    identityBuilder.AddOwnerPart(apiIdentityName);
                                }
                                else
                                {
                                    identityBuilder.AddOwnerPart();
                                }
                                break;
                            }

                        default:
                            throw new InvalidOperationException($"Unsupported API identity part kind: {apiKind}");
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
