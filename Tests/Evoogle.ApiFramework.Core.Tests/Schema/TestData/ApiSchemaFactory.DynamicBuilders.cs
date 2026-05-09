// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;
using Evoogle.Extension;

namespace Evoogle.ApiFramework.Schema.TestData;

public static partial class ApiSchemaFactory
{
    #region Dynamic Builder Definitions
    // ApiSchema
    public record ApiSchemaDef
    (
        string ApiName,
        string? ApiVersion = null,
        ApiIdentityPartNullHandling? ApiIdentityPartNullHandling = null,
        List<ApiTypeDef>? ApiNamedTypes = null,
        List<ApiRelationshipDef>? ApiRelationships = null,
        List<Type>? ExtensionTypes = null
    );

    // ApiSchemaElement
    public abstract record ApiSchemaElementDef(List<Type>? ExtensionTypes = null);

    // ApiType
    public abstract record ApiTypeDef(Type ClrType, List<Type>? ExtensionTypes = null) : ApiSchemaElementDef(ExtensionTypes);

    public abstract record ApiNamedTypeDef(string ApiName, Type ClrType, List<Type>? ExtensionTypes = null) : ApiTypeDef(ClrType, ExtensionTypes);

    public record ApiScalarTypeDef(string ApiName, Type ClrType, List<Type>? ExtensionTypes = null)
        : ApiNamedTypeDef(ApiName, ClrType, ExtensionTypes);

    public record ApiEnumTypeDef(string ApiName, Type ClrType, List<Type>? ExtensionTypes = null)
        : ApiNamedTypeDef(ApiName, ClrType, ExtensionTypes);

    public record ApiObjectTypeDef
    (
        string ApiName,
        Type ClrType,
        ApiIdentityPartNullHandling? ApiIdentityPartNullHandling = null,
        List<ApiIdentityDef>? ApiIdentities = null,
        List<ApiPropertyDef>? ApiProperties = null,
        List<Type>? ExtensionTypes = null
    ) : ApiNamedTypeDef(ApiName, ClrType, ExtensionTypes);

    public record ApiCollectionTypeDef
    (
        Type ClrType,
        ApiTypeExpression ApiItemTypeExpression,
        ApiTypeModifiers ApiItemTypeModifiers,
        List<Type>? ExtensionTypes = null
    ) : ApiTypeDef(ClrType, ExtensionTypes);

    // ApiIdentity
    public record ApiIdentityDef(string ApiName, List<ApiIdentityPartDef> Parts);

    // ApiIdentityPart
    public abstract record ApiIdentityPartDef;

    public record ApiIdentityScalarPartDef(string ApiPropertyName, Type? ClrScalarTypeHint = null) : ApiIdentityPartDef;

    public record ApiIdentityNestedPartDef(string ApiPropertyName, string? ApiIdentityName = null) : ApiIdentityPartDef;

    public record ApiIdentityOwnerPartDef(string? ApiIdentityName = null) : ApiIdentityPartDef;

    // ApiProperty
    public record ApiPropertyDef
    (
        string ApiName,
        ApiTypeExpression ApiTypeExpression,
        ApiTypeModifiers ApiTypeModifiers,
        string ClrName,
        ClrMemberKind ClrMemberKind
    );

    // ApiRelationship
    public abstract record ApiRelationshipDef(string ApiName, List<Type>? ExtensionTypes = null) : ApiSchemaElementDef(ExtensionTypes);

    public abstract record ApiRelationshipElementDef(Type ClrObjectType, List<Type>? ExtensionTypes = null) : ApiSchemaElementDef(ExtensionTypes);

    public record ApiRelationshipOneToOneDef
    (
        string ApiName,
        ApiRelationshipPrincipalEndDef PrincipalEnd,
        ApiRelationshipDependentEndDef DependentEnd,
        ApiRelationshipDeleteBehavior ApiDeleteBehavior = ApiRelationshipOneToOne.DefaultDeleteBehavior,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipDef(ApiName, ExtensionTypes);

    public record ApiRelationshipOneToManyDef
    (
        string ApiName,
        ApiRelationshipPrincipalEndDef PrincipalEnd,
        ApiRelationshipDependentEndDef DependentEnd,
        ApiRelationshipDeleteBehavior ApiDeleteBehavior = ApiRelationshipOneToMany.DefaultDeleteBehavior,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipDef(ApiName, ExtensionTypes);

    public record ApiRelationshipManyToManyDef
    (
        string ApiName,
        ApiRelationshipPrincipalEndDef PrincipalEndA,
        ApiRelationshipPrincipalEndDef PrincipalEndB,
        ApiRelationshipAssociationDef Association,
        ApiRelationshipDeleteBehavior ApiDeleteBehavior = ApiRelationshipManyToMany.DefaultDeleteBehavior,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipDef(ApiName, ExtensionTypes);

    // ApiRelationshipAssociation
    public record ApiRelationshipAssociationDef
    (
        Type ClrObjectType,
        IEnumerable<ApiRelationshipKeyPathDef>? ApiKeyPathsA = null,
        IEnumerable<ApiRelationshipKeyPathDef>? ApiKeyPathsB = null,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipElementDef(ClrObjectType, ExtensionTypes);

    // ApiRelationshipEnd
    public record ApiRelationshipPrincipalEndDef
    (
        Type ClrObjectType,
        string? ApiIdentityName = null,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipElementDef(ClrObjectType, ExtensionTypes);

    public record ApiRelationshipDependentEndDef
    (
        Type ClrObjectType,
        IEnumerable<ApiRelationshipKeyPathDef>? ApiKeyPaths = null,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipElementDef(ClrObjectType, ExtensionTypes);

    // ApiRelationshipKeyPath
    public abstract record ApiRelationshipKeyPathDef(List<Type>? ExtensionTypes = null) : ApiSchemaElementDef(ExtensionTypes);

    public record ApiRelationshipScalarKeyPathDef
    (
        string ClrPropertyName,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipKeyPathDef(ExtensionTypes);

    public record ApiRelationshipNestedKeyPathDef
    (
        string ClrPropertyName,
        List<ApiRelationshipKeyPathDef> ApiKeyPaths,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipKeyPathDef(ExtensionTypes);

    public record ApiRelationshipOwnerKeyPathDef
    (
        List<ApiRelationshipKeyPathDef>? ApiKeyPaths = null,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipKeyPathDef(ExtensionTypes);
    #endregion

    #region Dynamic Builder Methods
    public static ApiSchema? BuildTestApiSchema(ApiSchemaDef? apiSchemaDef)
    {
        if (apiSchemaDef == null)
        {
            return default;
        }

        var apiName = apiSchemaDef.ApiName;
        var apiVersion = apiSchemaDef.ApiVersion;

        var apiOptions = BuildApiSchemaOptions(apiSchemaDef);

        var apiNamedTypes = (apiSchemaDef.ApiNamedTypes ?? [])
            .Select(BuildTestApiType)
            .Where(t => t != null)
            .Cast<ApiNamedType>()
            .ToList();

        var apiRelationships = (apiSchemaDef.ApiRelationships ?? [])
            .Select(BuildApiRelationship)
            .Where(r => r != null)
            .ToList();

        var extensionTypeAndInstances = BuildExtensionInstances(apiSchemaDef.ExtensionTypes);

        return ApiSchema.Create
        (
            apiName,
            apiNamedTypes,
            apiVersion,
            apiOptions,
            apiRelationships,
            extensionTypeAndInstances
        );
    }

    public static ApiType? BuildTestApiType(ApiTypeDef? apiTypeDef)
    {
        if (apiTypeDef == null)
        {
            return default;
        }

        var apiType = (ApiType)((object?)apiTypeDef switch
        {
            ApiScalarTypeDef d => BuildApiScalarType(d),
            ApiEnumTypeDef d => BuildApiEnumType(d),
            ApiObjectTypeDef d => BuildApiObjectType(d),
            ApiCollectionTypeDef d => BuildApiCollectionType(d),
            _ => throw new InvalidOperationException($"Unsupported {nameof(ApiTypeDef)}: {apiTypeDef.GetType().Name}")
        });

        AttachExtensions(apiType, apiTypeDef);

        return apiType;
    }
    #endregion

    #region Dynamic Identity Builders
    private static ApiIdentity BuildApiIdentity(ApiIdentityDef def)
    {
        return new ApiIdentity(def.ApiName, def.Parts.Select(BuildApiIdentityPart));
    }

    private static ApiIdentityPart BuildApiIdentityPart(ApiIdentityPartDef def) => def switch
    {
        ApiIdentityScalarPartDef d => new ApiIdentityScalarPart(d.ApiPropertyName, d.ClrScalarTypeHint),
        ApiIdentityNestedPartDef d => new ApiIdentityNestedPart(d.ApiPropertyName, d.ApiIdentityName),
        ApiIdentityOwnerPartDef d => new ApiIdentityOwnerPart(d.ApiIdentityName),
        _ => throw new InvalidOperationException($"Unsupported {nameof(ApiIdentityPartDef)}: {def.GetType().Name}")
    };
    #endregion

    #region Dynamic Object Options Builders
    private static ApiObjectTypeOptions? BuildApiObjectTypeOptions(ApiObjectTypeDef def)
    {
        if (!def.ApiIdentityPartNullHandling.HasValue)
        {
            return null;
        }

        return new ApiObjectTypeOptions
        {
            ApiIdentityPartNullHandling = def.ApiIdentityPartNullHandling.Value
        };
    }
    #endregion

    #region Dynamic Property Builders
    private static ApiProperty BuildApiProperty(ApiPropertyDef def)
    {
        var apiName = def.ApiName;
        var apiTypeExpression = def.ApiTypeExpression;
        var apiTypeModifiers = def.ApiTypeModifiers;
        var clrName = def.ClrName;
        var clrMemberKind = def.ClrMemberKind;

        return new ApiProperty(apiName, apiTypeExpression, apiTypeModifiers, clrName, clrMemberKind);
    }
    #endregion

    #region Dynamic Relationship Builders
    private static ApiRelationship BuildApiRelationship(ApiRelationshipDef def)
    {
        var apiRelationship = (ApiRelationship)((object?)def switch
        {
            ApiRelationshipOneToOneDef d => BuildApiRelationshipOneToOne(d),
            ApiRelationshipOneToManyDef d => BuildApiRelationshipOneToMany(d),
            ApiRelationshipManyToManyDef d => BuildApiRelationshipManyToMany(d),
            _ => throw new InvalidOperationException($"Unsupported {nameof(ApiRelationshipDef)}: {def.GetType().Name}")
        });

        AttachExtensions(apiRelationship, def);

        return apiRelationship;
    }

    private static ApiRelationshipOneToOne BuildApiRelationshipOneToOne(ApiRelationshipOneToOneDef def)
    {
        var apiName = def.ApiName;
        var apiPrincipalEnd = BuildApiRelationshipPrincipalEnd(def.PrincipalEnd);
        var apiDependentEnd = BuildApiRelationshipDependentEnd(def.DependentEnd);
        var apiDeleteBehavior = def.ApiDeleteBehavior;

        return new ApiRelationshipOneToOne(apiName, apiPrincipalEnd, apiDependentEnd, apiDeleteBehavior);
    }

    private static ApiRelationshipOneToMany BuildApiRelationshipOneToMany(ApiRelationshipOneToManyDef def)
    {
        var apiName = def.ApiName;
        var apiPrincipalEnd = BuildApiRelationshipPrincipalEnd(def.PrincipalEnd);
        var apiDependentEnd = BuildApiRelationshipDependentEnd(def.DependentEnd);
        var apiDeleteBehavior = def.ApiDeleteBehavior;

        return new ApiRelationshipOneToMany(apiName, apiPrincipalEnd, apiDependentEnd, apiDeleteBehavior);
    }

    private static ApiRelationshipManyToMany BuildApiRelationshipManyToMany(ApiRelationshipManyToManyDef def)
    {
        var apiName = def.ApiName;
        var apiPrincipalEndA = BuildApiRelationshipPrincipalEnd(def.PrincipalEndA);
        var apiPrincipalEndB = BuildApiRelationshipPrincipalEnd(def.PrincipalEndB);
        var apiAssociation = BuildApiRelationshipAssociation(def.Association);
        var apiDeleteBehavior = def.ApiDeleteBehavior;

        return new ApiRelationshipManyToMany(apiName, apiPrincipalEndA, apiPrincipalEndB, apiAssociation, apiDeleteBehavior);
    }

    private static ApiRelationshipAssociation BuildApiRelationshipAssociation(ApiRelationshipAssociationDef def)
    {
        var clrObjectType = def.ClrObjectType;
        var apiKeyPathsA = def.ApiKeyPathsA?.Select(BuildApiRelationshipKeyPath);
        var apiKeyPathsB = def.ApiKeyPathsB?.Select(BuildApiRelationshipKeyPath);

        var apiRelationshipDependentEnd = new ApiRelationshipAssociation
        (
            clrObjectType,
            apiKeyPathsA,
            apiKeyPathsB
        );

        AttachExtensions(apiRelationshipDependentEnd, def);

        return apiRelationshipDependentEnd;
    }

    private static ApiRelationshipPrincipalEnd BuildApiRelationshipPrincipalEnd(ApiRelationshipPrincipalEndDef def)
    {
        var clrObjectType = def.ClrObjectType;
        var apiIdentityName = def.ApiIdentityName;

        var apiRelationshipPrincipalEnd = new ApiRelationshipPrincipalEnd
        (
            clrObjectType,
            apiIdentityName
        );

        AttachExtensions(apiRelationshipPrincipalEnd, def);

        return apiRelationshipPrincipalEnd;
    }

    private static ApiRelationshipDependentEnd BuildApiRelationshipDependentEnd(ApiRelationshipDependentEndDef def)
    {
        var clrObjectType = def.ClrObjectType;
        var apiKeyPaths = def.ApiKeyPaths?.Select(BuildApiRelationshipKeyPath);

        var apiRelationshipDependentEnd = new ApiRelationshipDependentEnd
        (
            clrObjectType,
            apiKeyPaths
        );

        AttachExtensions(apiRelationshipDependentEnd, def);

        return apiRelationshipDependentEnd;
    }

    private static ApiRelationshipKeyPath BuildApiRelationshipKeyPath(ApiRelationshipKeyPathDef def)
    {
        var apiRelationshipKeyPath = (ApiRelationshipKeyPath)((object?)def switch
        {
            ApiRelationshipScalarKeyPathDef d => BuildApiRelationshipScalarKeyPath(d),
            ApiRelationshipNestedKeyPathDef d => BuildApiRelationshipNestedKeyPath(d),
            ApiRelationshipOwnerKeyPathDef d => BuildApiRelationshipOwnerKeyPath(d),
            _ => throw new InvalidOperationException($"Unsupported {nameof(ApiRelationshipKeyPathDef)}: {def.GetType().Name}")
        });

        AttachExtensions(apiRelationshipKeyPath, def);

        return apiRelationshipKeyPath;
    }

    private static ApiRelationshipScalarKeyPath BuildApiRelationshipScalarKeyPath(ApiRelationshipScalarKeyPathDef def)
    {
        var clrPropertyName = def.ClrPropertyName;

        return new ApiRelationshipScalarKeyPath(clrPropertyName);
    }

    private static ApiRelationshipNestedKeyPath BuildApiRelationshipNestedKeyPath(ApiRelationshipNestedKeyPathDef def)
    {
        var clrPropertyName = def.ClrPropertyName;
        var apiKeyPaths = def.ApiKeyPaths.Select(BuildApiRelationshipKeyPath);

        return new ApiRelationshipNestedKeyPath(clrPropertyName, apiKeyPaths);
    }

    private static ApiRelationshipOwnerKeyPath BuildApiRelationshipOwnerKeyPath(ApiRelationshipOwnerKeyPathDef def)
    {
        var apiKeyPaths = def.ApiKeyPaths?.Select(BuildApiRelationshipKeyPath);

        return new ApiRelationshipOwnerKeyPath(apiKeyPaths);
    }
    #endregion

    #region Dynamic Schema Options Builders
    private static ApiSchemaOptions? BuildApiSchemaOptions(ApiSchemaDef def)
    {
        if (!def.ApiIdentityPartNullHandling.HasValue)
        {
            return null;
        }

        return new ApiSchemaOptions
        {
            ApiIdentityPartNullHandling = def.ApiIdentityPartNullHandling.Value
        };
    }
    #endregion

    #region Dynamic Type Builders
    private static ApiCollectionType BuildApiCollectionType(ApiCollectionTypeDef def)
    {
        var apiItemTypeExpression = def.ApiItemTypeExpression;
        var apiItemTypeModifiers = def.ApiItemTypeModifiers;
        var clrType = def.ClrType;

        return new ApiCollectionType(apiItemTypeExpression, apiItemTypeModifiers, clrType);
    }

    private static ApiEnumType BuildApiEnumType(ApiEnumTypeDef def)
    {
        var apiName = def.ApiName;
        var clrType = def.ClrType;

        var clrEnumValues = Enum.GetValues(def.ClrType);
        var apiEnumValues = clrEnumValues
            .Cast<int>()
            .Select(x =>
            {
                var clrName = Enum.GetName(def.ClrType, x)!;
                return new ApiEnumValue(apiName: clrName, clrName: clrName, clrOrdinal: x);
            })
            .ToList();

        return new ApiEnumType(apiName, apiEnumValues, clrType);
    }

    private static ApiObjectType BuildApiObjectType(ApiObjectTypeDef def)
    {
        var apiName = def.ApiName;
        var apiOptions = BuildApiObjectTypeOptions(def);
        var apiIdentities = def.ApiIdentities?.Select(BuildApiIdentity);
        var apiProperties = def.ApiProperties?.Select(BuildApiProperty);
        var clrType = def.ClrType;

        return new ApiObjectType(apiName, apiOptions, apiIdentities, apiProperties, clrType);
    }

    private static ApiScalarType BuildApiScalarType(ApiScalarTypeDef def)
    {
        var apiName = def.ApiName;
        var clrType = def.ClrType;

        return new ApiScalarType(apiName, clrType);
    }
    #endregion

    #region Dynamic Extension Builders
    private static void AttachExtensions(ExtensibleBase extensibleBase, ApiSchemaElementDef apiSchemaElementDef)
    {
        var extensionTypes = apiSchemaElementDef.ExtensionTypes;
        if (extensionTypes == null)
        {
            return;
        }

        foreach (var extensionType in extensionTypes)
        {
            var extensionInstance = Activator.CreateInstance(extensionType)!;
            extensibleBase.AttachExtension(extensionType, extensionInstance);
        }
    }

    private static List<(Type ExtensionType, object ExtensionInstance)>? BuildExtensionInstances(List<Type>? extensionTypes)
    {
        if (extensionTypes == null)
        {
            return null;
        }

        var result = new List<(Type, object)>(extensionTypes.Count);
        foreach (var extensionType in extensionTypes)
        {
            result.Add((extensionType, Activator.CreateInstance(extensionType)!));
        }
        return result;
    }
    #endregion
}
