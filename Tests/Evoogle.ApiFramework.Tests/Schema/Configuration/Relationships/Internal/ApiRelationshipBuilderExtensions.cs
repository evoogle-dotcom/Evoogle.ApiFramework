// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Relationships.Internal;

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
            if (apiDependentEnd.HasForeignKey)
            {
                ConfigureForeignKey(y, apiDependentEnd.ApiForeignKey);
            }
            y.ConfigureExtensions(apiDependentEnd);
        });
    }

    public static void ConfigureDependentEnd(this ApiRelationshipOneToManyBuilder builder, ApiRelationshipDependentEnd apiDependentEnd)
    {
        builder.To(apiDependentEnd.ClrObjectType, y =>
        {
            if (apiDependentEnd.HasForeignKey)
            {
                ConfigureForeignKey(y, apiDependentEnd.ApiForeignKey);
            }
            y.ConfigureExtensions(apiDependentEnd);
        });
    }

    public static void ConfigureAssociation(this ApiRelationshipManyToManyBuilder builder, ApiRelationshipAssociation apiAssociation)
    {
        builder.WithAssociation(apiAssociation.ClrObjectType, y =>
        {
            if (apiAssociation.HasForeignKeys)
            {
                ConfigureForeignKey(y, apiAssociation.ApiForeignKeyA, isA: true);
                ConfigureForeignKey(y, apiAssociation.ApiForeignKeyB, isA: false);
            }
            y.ConfigureExtensions(apiAssociation);
        });
    }
    #endregion

    #region Implementation Methods
    private static void ConfigureForeignKey(ApiRelationshipDependentEndBuilder builder, ApiKeyDefinition apiForeignKey)
    {
        builder.WithForeignKey(fk =>
        {
            foreach (var keyPath in apiForeignKey.ApiKeyPaths)
            {
                var clrMemberNames = keyPath.ApiSegments.Select(s => s.ClrMemberName);
                fk.AddPath(keyPath.ClrRootType, clrMemberNames, p => p.ConfigureExtensions(keyPath));
            }
            fk.ConfigureExtensions(apiForeignKey);
        });
    }

    private static void ConfigureForeignKey(ApiRelationshipAssociationBuilder builder, ApiKeyDefinition apiForeignKey, bool isA)
    {
        if (isA)
        {
            builder.WithForeignKeyA(fk =>
            {
                foreach (var keyPath in apiForeignKey.ApiKeyPaths)
                {
                    var clrMemberNames = keyPath.ApiSegments.Select(s => s.ClrMemberName);
                    fk.AddPath(keyPath.ClrRootType, clrMemberNames, p => p.ConfigureExtensions(keyPath));
                }
                fk.ConfigureExtensions(apiForeignKey);
            });
        }
        else
        {
            builder.WithForeignKeyB(fk =>
            {
                foreach (var keyPath in apiForeignKey.ApiKeyPaths)
                {
                    var clrMemberNames = keyPath.ApiSegments.Select(s => s.ClrMemberName);
                    fk.AddPath(keyPath.ClrRootType, clrMemberNames, p => p.ConfigureExtensions(keyPath));
                }
                fk.ConfigureExtensions(apiForeignKey);
            });
        }
    }
    #endregion
}
