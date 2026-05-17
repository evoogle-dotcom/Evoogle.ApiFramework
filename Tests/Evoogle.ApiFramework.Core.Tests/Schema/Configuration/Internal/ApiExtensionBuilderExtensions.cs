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
internal static class ApiExtensionBuilderExtensions
{
    #region Methods
    public static void ConfigureExtensions(this ApiEnumTypeBuilder builder, ApiEnumType apiEnumType)
    {
        var extensions = apiEnumType.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddEnumExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureExtensions(this ApiIdentityBuilder builder, ApiIdentity apiIdentity)
    {
        var extensions = apiIdentity.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddIdentityExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureExtensions(this ApiIdentityPartBuilder builder, ApiIdentityPart apiIdentityPart)
    {
        var extensions = apiIdentityPart.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddIdentityPartExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureExtensions(this ApiObjectTypeBuilder builder, ApiObjectType apiObjectType)
    {
        var extensions = apiObjectType.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddObjectExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureExtensions(this ApiPropertyBuilder builder, ApiProperty apiProperty)
    {
        var extensions = apiProperty.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddPropertyExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureExtensions(this ApiRelationshipAssociationBuilder builder, ApiRelationshipAssociation apiRelationshipAssociation)
    {
        var extensions = apiRelationshipAssociation.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddAssociationExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureExtensions(this ApiRelationshipDependentEndBuilder builder, ApiRelationshipDependentEnd apiRelationshipDependentEnd)
    {
        var extensions = apiRelationshipDependentEnd.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddDependentEndExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureExtensions(this ApiRelationshipKeyPathBuilder builder, ApiRelationshipKeyPath apiRelationshipKeyPath)
    {
        var extensions = apiRelationshipKeyPath.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddKeyPathExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureExtensions(this ApiRelationshipManyToManyBuilder builder, ApiRelationship apiRelationship)
    {
        var extensions = apiRelationship.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddRelationshipExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureExtensions(this ApiRelationshipOneToManyBuilder builder, ApiRelationship apiRelationship)
    {
        var extensions = apiRelationship.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddRelationshipExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureExtensions(this ApiRelationshipOneToOneBuilder builder, ApiRelationship apiRelationship)
    {
        var extensions = apiRelationship.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddRelationshipExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureExtensions(this ApiRelationshipPrincipalEndBuilder builder, ApiRelationshipPrincipalEnd apiRelationshipPrincipalEnd)
    {
        var extensions = apiRelationshipPrincipalEnd.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddPrincipalEndExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureExtensions(this ApiScalarTypeBuilder builder, ApiScalarType apiScalarType)
    {
        var extensions = apiScalarType.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddScalarExtension(extension.Key, extension.Value);
        }
    }

    public static void ConfigureExtensions(this ApiSchemaBuilder builder, ApiSchema apiSchema)
    {
        var extensions = apiSchema.Extensions;
        foreach (var extension in extensions ?? [])
        {
            builder.AddSchemaExtension(extension.Key, extension.Value);
        }
    }
    #endregion
}
