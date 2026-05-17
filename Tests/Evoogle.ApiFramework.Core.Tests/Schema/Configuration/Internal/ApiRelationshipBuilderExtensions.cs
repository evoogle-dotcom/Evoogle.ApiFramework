// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiRelationshipBuilderExtensions
{
    #region Methods
    public static void ConfigureDependentEnd(this ApiRelationshipOneToOneBuilder builder, ApiRelationshipDependentEnd apiDependentEnd)
    {
        builder.WithDependentEnd(apiDependentEnd.ClrObjectType, y =>
        {
            if (apiDependentEnd.HasKeyBinding)
            {
                ConfigureDependentEndKeyPaths(y, apiDependentEnd.ApiKeyPaths);
            }
            y.ConfigureExtensions(apiDependentEnd);
        });
    }

    public static void ConfigureDependentEnd(this ApiRelationshipOneToManyBuilder builder, ApiRelationshipDependentEnd apiDependentEnd)
    {
        builder.WithDependentEnd(apiDependentEnd.ClrObjectType, y =>
        {
            if (apiDependentEnd.HasKeyBinding)
            {
                ConfigureDependentEndKeyPaths(y, apiDependentEnd.ApiKeyPaths);
            }
            y.ConfigureExtensions(apiDependentEnd);
        });
    }

    public static void ConfigureAssociation(this ApiRelationshipManyToManyBuilder builder, ApiRelationshipAssociation apiAssociation)
    {
        builder.WithAssociation(apiAssociation.ClrObjectType, y =>
        {
            if (apiAssociation.HasKeyBinding)
            {
                ConfigureAssociationKeyPaths(y, apiAssociation.ApiKeyPathsA, apiAssociation.ApiKeyPathsB);
            }
            y.ConfigureExtensions(apiAssociation);
        });
    }
    #endregion

    #region Implementation Methods
    private static void ConfigureDependentEndKeyPaths(ApiRelationshipDependentEndBuilder builder, IEnumerable<ApiRelationshipKeyPath> keyPaths)
    {
        foreach (var keyPath in keyPaths)
        {
            switch (keyPath.ApiKind)
            {
                case ApiRelationshipKeyPathKind.Scalar:
                    {
                        var scalarPath = (ApiRelationshipScalarKeyPath)keyPath;
                        builder.AddScalarPath(scalarPath.ClrPropertyName, p => p.ConfigureExtensions(scalarPath));
                        break;
                    }
                case ApiRelationshipKeyPathKind.Nested:
                    {
                        var nestedPath = (ApiRelationshipNestedKeyPath)keyPath;
                        builder.AddNestedPath(nestedPath.ClrPropertyName, z =>
                        {
                            ConfigureChildKeyPaths(z, nestedPath.ApiKeyPaths);
                            z.ConfigureExtensions(nestedPath);
                        });
                        break;
                    }
                case ApiRelationshipKeyPathKind.Owner:
                    {
                        var ownerPath = (ApiRelationshipOwnerKeyPath)keyPath;
                        if (ownerPath.HasKeyBinding)
                        {
                            builder.AddOwnerPath(z =>
                            {
                                ConfigureChildKeyPaths(z, ownerPath.ApiKeyPaths);
                                z.ConfigureExtensions(ownerPath);
                            });
                        }
                        else
                        {
                            builder.AddOwnerPath(z => z.ConfigureExtensions(ownerPath));
                        }
                        break;
                    }
                default:
                    throw new InvalidOperationException($"Unsupported {nameof(ApiRelationshipKeyPathKind)}: {keyPath.ApiKind}");
            }
        }
    }

    private static void ConfigureAssociationKeyPaths(ApiRelationshipAssociationBuilder builder, IEnumerable<ApiRelationshipKeyPath> keyPathsA, IEnumerable<ApiRelationshipKeyPath> keyPathsB)
    {
        foreach (var keyPath in keyPathsA)
        {
            switch (keyPath.ApiKind)
            {
                case ApiRelationshipKeyPathKind.Scalar:
                    {
                        var scalarPath = (ApiRelationshipScalarKeyPath)keyPath;
                        builder.AddScalarPathA(scalarPath.ClrPropertyName, p => p.ConfigureExtensions(scalarPath));
                        break;
                    }
                case ApiRelationshipKeyPathKind.Nested:
                    {
                        var nestedPath = (ApiRelationshipNestedKeyPath)keyPath;
                        builder.AddNestedPathA(nestedPath.ClrPropertyName, z =>
                        {
                            ConfigureChildKeyPaths(z, nestedPath.ApiKeyPaths);
                            z.ConfigureExtensions(nestedPath);
                        });
                        break;
                    }
                case ApiRelationshipKeyPathKind.Owner:
                    {
                        var ownerPath = (ApiRelationshipOwnerKeyPath)keyPath;
                        if (ownerPath.HasKeyBinding)
                        {
                            builder.AddOwnerPathA(z =>
                            {
                                ConfigureChildKeyPaths(z, ownerPath.ApiKeyPaths);
                                z.ConfigureExtensions(ownerPath);
                            });
                        }
                        else
                        {
                            builder.AddOwnerPathA(z => z.ConfigureExtensions(ownerPath));
                        }
                        break;
                    }
                default:
                    throw new InvalidOperationException($"Unsupported {nameof(ApiRelationshipKeyPathKind)}: {keyPath.ApiKind}");
            }
        }

        foreach (var keyPath in keyPathsB)
        {
            switch (keyPath.ApiKind)
            {
                case ApiRelationshipKeyPathKind.Scalar:
                    {
                        var scalarPath = (ApiRelationshipScalarKeyPath)keyPath;
                        builder.AddScalarPathB(scalarPath.ClrPropertyName, p => p.ConfigureExtensions(scalarPath));
                        break;
                    }
                case ApiRelationshipKeyPathKind.Nested:
                    {
                        var nestedPath = (ApiRelationshipNestedKeyPath)keyPath;
                        builder.AddNestedPathB(nestedPath.ClrPropertyName, z =>
                        {
                            ConfigureChildKeyPaths(z, nestedPath.ApiKeyPaths);
                            z.ConfigureExtensions(nestedPath);
                        });
                        break;
                    }
                case ApiRelationshipKeyPathKind.Owner:
                    {
                        var ownerPath = (ApiRelationshipOwnerKeyPath)keyPath;
                        if (ownerPath.HasKeyBinding)
                        {
                            builder.AddOwnerPathB(z =>
                            {
                                ConfigureChildKeyPaths(z, ownerPath.ApiKeyPaths);
                                z.ConfigureExtensions(ownerPath);
                            });
                        }
                        else
                        {
                            builder.AddOwnerPathB(z => z.ConfigureExtensions(ownerPath));
                        }
                        break;
                    }
                default:
                    throw new InvalidOperationException($"Unsupported {nameof(ApiRelationshipKeyPathKind)}: {keyPath.ApiKind}");
            }
        }
    }

    private static void ConfigureChildKeyPaths(ApiRelationshipKeyPathBuilder builder, IEnumerable<ApiRelationshipKeyPath> keyPaths)
    {
        foreach (var keyPath in keyPaths)
        {
            switch (keyPath.ApiKind)
            {
                case ApiRelationshipKeyPathKind.Scalar:
                    {
                        var scalarPath = (ApiRelationshipScalarKeyPath)keyPath;
                        builder.AddScalarPath(scalarPath.ClrPropertyName, p => p.ConfigureExtensions(scalarPath));
                        break;
                    }
                case ApiRelationshipKeyPathKind.Nested:
                    {
                        var nestedPath = (ApiRelationshipNestedKeyPath)keyPath;
                        builder.AddNestedPath(nestedPath.ClrPropertyName, z =>
                        {
                            ConfigureChildKeyPaths(z, nestedPath.ApiKeyPaths);
                            z.ConfigureExtensions(nestedPath);
                        });
                        break;
                    }
                case ApiRelationshipKeyPathKind.Owner:
                    {
                        var ownerPath = (ApiRelationshipOwnerKeyPath)keyPath;
                        if (ownerPath.HasKeyBinding)
                        {
                            builder.AddOwnerPath(z =>
                            {
                                ConfigureChildKeyPaths(z, ownerPath.ApiKeyPaths);
                                z.ConfigureExtensions(ownerPath);
                            });
                        }
                        else
                        {
                            builder.AddOwnerPath(z => z.ConfigureExtensions(ownerPath));
                        }
                        break;
                    }
                default:
                    throw new InvalidOperationException($"Unsupported {nameof(ApiRelationshipKeyPathKind)}: {keyPath.ApiKind}");
            }
        }
    }
    #endregion
}
