// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
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
        ApiKeyNullHandling? ApiKeyNullHandling = null,
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
        ApiKeyNullHandling? ApiKeyNullHandling = null,
        List<ApiPropertyDef>? ApiProperties = null,
        List<ApiKeyTypeDef>? ApiKeyTypes = null,
        List<Type>? ExtensionTypes = null
    ) : ApiNamedTypeDef(ApiName, ClrType, ExtensionTypes);

    public record ApiCollectionTypeDef
    (
        Type ClrType,
        ApiTypeExpression ApiItemTypeExpression,
        ApiTypeModifiers ApiItemTypeModifiers,
        List<Type>? ExtensionTypes = null
    ) : ApiTypeDef(ClrType, ExtensionTypes);

    // ApiKeyType
    public record ApiKeyTypeDef(string ApiName, List<ApiKeyPathDef> ApiKeyPaths, List<Type>? ExtensionTypes = null) : ApiSchemaElementDef(ExtensionTypes);

    public record ApiKeyPathDef(Type ClrRootType, List<ApiKeyPathSegmentDef> ApiKeyPathSegments, List<Type>? ExtensionTypes = null) : ApiSchemaElementDef(ExtensionTypes);

    public record ApiKeyPathSegmentDef(string ClrPropertyName, List<Type>? ExtensionTypes = null) : ApiSchemaElementDef(ExtensionTypes);

    // ApiProperty
    public record ApiPropertyDef
    (
        string ApiName,
        ApiTypeExpression ApiTypeExpression,
        ApiTypeModifiers ApiTypeModifiers,
        string ClrName,
        ClrMemberKind ClrMemberKind,
        List<Type>? ExtensionTypes = null
    ) : ApiSchemaElementDef(ExtensionTypes);

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
        ApiKeyTypeDef? ApiForeignKeyTypeA = null,
        ApiKeyTypeDef? ApiForeignKeyTypeB = null,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipElementDef(ClrObjectType, ExtensionTypes);

    // ApiRelationshipEnd
    public record ApiRelationshipPrincipalEndDef
    (
        Type ClrObjectType,
        string? ApiPrimaryKeyTypeName = null,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipElementDef(ClrObjectType, ExtensionTypes);

    public record ApiRelationshipDependentEndDef
    (
        Type ClrObjectType,
        ApiKeyTypeDef? ApiForeignKeyType = null,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipElementDef(ClrObjectType, ExtensionTypes);
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

    #region Dynamic Key Builders
    private static ApiKeyType BuildApiKeyType(ApiKeyTypeDef def)
    {
        var apiName = def.ApiName;
        var apiKeyPaths = def.ApiKeyPaths.Select(BuildApiKeyPath);

        var apiKeyType = new ApiKeyType(apiName, apiKeyPaths);

        AttachExtensions(apiKeyType, def);

        return apiKeyType;
    }

    private static ApiKeyPath BuildApiKeyPath(ApiKeyPathDef def)
    {
        var clrRootType = def.ClrRootType;
        var apiKeyPathSegments = def.ApiKeyPathSegments.Select(BuildApiKeyPathSegment);

        var apiKeyPath = new ApiKeyPath(clrRootType, apiKeyPathSegments);

        AttachExtensions(apiKeyPath, def);

        return apiKeyPath;
    }

    private static ApiKeyPathSegment BuildApiKeyPathSegment(ApiKeyPathSegmentDef def)
    {
        var clrPropertyName = def.ClrPropertyName;

        var apiKeyPathSegment = new ApiKeyPathSegment(clrPropertyName);

        AttachExtensions(apiKeyPathSegment, def);

        return apiKeyPathSegment;
    }
    #endregion

    #region Dynamic Object Options Builders
    private static ApiObjectTypeOptions? BuildApiObjectTypeOptions(ApiObjectTypeDef def)
    {
        if (!def.ApiKeyNullHandling.HasValue)
        {
            return null;
        }

        return new ApiObjectTypeOptions
        {
            ApiKeyNullHandling = def.ApiKeyNullHandling.Value
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

        var apiProperty = new ApiProperty(apiName, apiTypeExpression, apiTypeModifiers, clrName, clrMemberKind);

        AttachExtensions(apiProperty, def);

        return apiProperty;
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

        var apiRelationshipOneToOne = new ApiRelationshipOneToOne(apiName, apiPrincipalEnd, apiDependentEnd, apiDeleteBehavior);

        AttachExtensions(apiRelationshipOneToOne, def);

        return apiRelationshipOneToOne;
    }

    private static ApiRelationshipOneToMany BuildApiRelationshipOneToMany(ApiRelationshipOneToManyDef def)
    {
        var apiName = def.ApiName;
        var apiPrincipalEnd = BuildApiRelationshipPrincipalEnd(def.PrincipalEnd);
        var apiDependentEnd = BuildApiRelationshipDependentEnd(def.DependentEnd);
        var apiDeleteBehavior = def.ApiDeleteBehavior;

        var apiRelationshipOneToMany = new ApiRelationshipOneToMany(apiName, apiPrincipalEnd, apiDependentEnd, apiDeleteBehavior);

        AttachExtensions(apiRelationshipOneToMany, def);

        return apiRelationshipOneToMany;
    }

    private static ApiRelationshipManyToMany BuildApiRelationshipManyToMany(ApiRelationshipManyToManyDef def)
    {
        var apiName = def.ApiName;
        var apiPrincipalEndA = BuildApiRelationshipPrincipalEnd(def.PrincipalEndA);
        var apiPrincipalEndB = BuildApiRelationshipPrincipalEnd(def.PrincipalEndB);
        var apiAssociation = BuildApiRelationshipAssociation(def.Association);
        var apiDeleteBehavior = def.ApiDeleteBehavior;

        var apiRelationshipManyToMany = new ApiRelationshipManyToMany(apiName, apiPrincipalEndA, apiPrincipalEndB, apiAssociation, apiDeleteBehavior);

        AttachExtensions(apiRelationshipManyToMany, def);

        return apiRelationshipManyToMany;
    }

    private static ApiRelationshipAssociation BuildApiRelationshipAssociation(ApiRelationshipAssociationDef def)
    {
        var clrObjectType = def.ClrObjectType;
        var apiForeignKeyTypeA = def.ApiForeignKeyTypeA != null ? BuildApiKeyType(def.ApiForeignKeyTypeA) : null;
        var apiForeignKeyTypeB = def.ApiForeignKeyTypeB != null ? BuildApiKeyType(def.ApiForeignKeyTypeB) : null;

        var apiRelationshipAssociation = apiForeignKeyTypeA != null && apiForeignKeyTypeB != null
            ? new ApiRelationshipAssociation(clrObjectType, apiForeignKeyTypeA, apiForeignKeyTypeB)
            : new ApiRelationshipAssociation(clrObjectType);

        AttachExtensions(apiRelationshipAssociation, def);

        return apiRelationshipAssociation;
    }

    private static ApiRelationshipPrincipalEnd BuildApiRelationshipPrincipalEnd(ApiRelationshipPrincipalEndDef def)
    {
        var clrObjectType = def.ClrObjectType;
        var apiPrimaryKeyTypeName = def.ApiPrimaryKeyTypeName;

        var apiRelationshipPrincipalEnd = new ApiRelationshipPrincipalEnd
        (
            clrObjectType,
            apiPrimaryKeyTypeName
        );

        AttachExtensions(apiRelationshipPrincipalEnd, def);

        return apiRelationshipPrincipalEnd;
    }

    private static ApiRelationshipDependentEnd BuildApiRelationshipDependentEnd(ApiRelationshipDependentEndDef def)
    {
        var clrObjectType = def.ClrObjectType;
        var apiForeignKeyType = def.ApiForeignKeyType != null ? BuildApiKeyType(def.ApiForeignKeyType) : null;

        var apiRelationshipDependentEnd = apiForeignKeyType != null
            ? new ApiRelationshipDependentEnd(clrObjectType, apiForeignKeyType)
            : new ApiRelationshipDependentEnd(clrObjectType);

        AttachExtensions(apiRelationshipDependentEnd, def);

        return apiRelationshipDependentEnd;
    }
    #endregion

    #region Dynamic Schema Options Builders
    private static ApiSchemaOptions? BuildApiSchemaOptions(ApiSchemaDef def)
    {
        if (!def.ApiKeyNullHandling.HasValue)
        {
            return null;
        }

        return new ApiSchemaOptions
        {
            ApiKeyNullHandling = def.ApiKeyNullHandling.Value
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
        var apiProperties = def.ApiProperties?.Select(BuildApiProperty);
        var apiKeyTypes = def.ApiKeyTypes?.Select(BuildApiKeyType);
        var clrType = def.ClrType;

        return new ApiObjectType(apiName, apiOptions, apiProperties, apiKeyTypes, clrType);
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
