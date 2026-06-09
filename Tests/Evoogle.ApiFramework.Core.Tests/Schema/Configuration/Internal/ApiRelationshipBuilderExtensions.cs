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
        builder.To(apiDependentEnd.ClrObjectType, y =>
        {
            if (apiDependentEnd.HasKeyBinding)
            {
                ConfigureForeignKeyType(y, apiDependentEnd.ApiForeignKeyType);
            }
            y.ConfigureExtensions(apiDependentEnd);
        });
    }

    public static void ConfigureDependentEnd(this ApiRelationshipOneToManyBuilder builder, ApiRelationshipDependentEnd apiDependentEnd)
    {
        builder.To(apiDependentEnd.ClrObjectType, y =>
        {
            if (apiDependentEnd.HasKeyBinding)
            {
                ConfigureForeignKeyType(y, apiDependentEnd.ApiForeignKeyType);
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
                ConfigureForeignKeyType(y, apiAssociation.ApiForeignKeyTypeA, isA: true);
                ConfigureForeignKeyType(y, apiAssociation.ApiForeignKeyTypeB, isA: false);
            }
            y.ConfigureExtensions(apiAssociation);
        });
    }
    #endregion

    #region Implementation Methods
    private static void ConfigureForeignKeyType(ApiRelationshipDependentEndBuilder builder, ApiKeyType apiForeignKeyType)
    {
        builder.WithForeignKey(fk =>
        {
            foreach (var keyPath in apiForeignKeyType.ApiKeyPaths)
            {
                var clrPropertyNames = keyPath.ApiSegments.Select(s => s.ClrPropertyName);
                fk.AddPath(keyPath.ClrRootType, clrPropertyNames, p => p.ConfigureExtensions(keyPath));
            }
            fk.ConfigureExtensions(apiForeignKeyType);
        });
    }

    private static void ConfigureForeignKeyType(ApiRelationshipAssociationBuilder builder, ApiKeyType apiForeignKeyType, bool isA)
    {
        if (isA)
        {
            builder.WithForeignKeyA(fk =>
            {
                foreach (var keyPath in apiForeignKeyType.ApiKeyPaths)
                {
                    var clrPropertyNames = keyPath.ApiSegments.Select(s => s.ClrPropertyName);
                    fk.AddPath(keyPath.ClrRootType, clrPropertyNames, p => p.ConfigureExtensions(keyPath));
                }
                fk.ConfigureExtensions(apiForeignKeyType);
            });
        }
        else
        {
            builder.WithForeignKeyB(fk =>
            {
                foreach (var keyPath in apiForeignKeyType.ApiKeyPaths)
                {
                    var clrPropertyNames = keyPath.ApiSegments.Select(s => s.ClrPropertyName);
                    fk.AddPath(keyPath.ClrRootType, clrPropertyNames, p => p.ConfigureExtensions(keyPath));
                }
                fk.ConfigureExtensions(apiForeignKeyType);
            });
        }
    }
    #endregion
}
