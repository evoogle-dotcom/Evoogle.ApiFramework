// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Version;
using Evoogle.Extension;

namespace Evoogle.ApiFramework.Schema.TestData;

public static partial class ApiSchemaFactory
{
    #region Dynamic Builder Definitions
    // Schema
    public abstract record ApiSchemaElementDef(List<Type>? ExtensionTypes = null);

    public record ApiSchemaDef
    (
        string ApiName,
        string? ApiVersion = null,
        ApiKeyNullHandling? ApiKeyNullHandling = null,
        List<ApiTypeDef>? ApiNamedTypes = null,
        List<ApiRelationshipDef>? ApiRelationships = null,
        List<Type>? ExtensionTypes = null
    ) : ApiSchemaElementDef(ExtensionTypes);

    // Types
    public abstract record ApiTypeDef
    (
        Type ClrType,
        List<Type>? ExtensionTypes = null
    ) : ApiSchemaElementDef(ExtensionTypes);

    public abstract record ApiNamedTypeDef
    (
        string ApiName,
        Type ClrType,
        List<Type>? ExtensionTypes = null
    ) : ApiTypeDef(ClrType, ExtensionTypes);

    public record ApiScalarTypeDef
    (
        string ApiName,
        Type ClrType,
        List<Type>? ExtensionTypes = null
    ) : ApiNamedTypeDef(ApiName, ClrType, ExtensionTypes);

    public record ApiEnumTypeDef
    (
        string ApiName,
        Type ClrType,
        List<Type>? ExtensionTypes = null
    ) : ApiNamedTypeDef(ApiName, ClrType, ExtensionTypes);

    public record ApiObjectTypeDef
    (
        string ApiName,
        Type ClrType,
        ApiKeyNullHandling? ApiKeyNullHandling = null,
        List<ApiPropertyDef>? ApiProperties = null,
        List<ApiKeyDef>? ApiKeys = null,
        ApiVersionDef? ApiVersion = null,
        List<Type>? ExtensionTypes = null
    ) : ApiNamedTypeDef(ApiName, ClrType, ExtensionTypes);

    public record ApiCollectionTypeDef
    (
        Type ClrType,
        ApiTypeExpressionDef ApiItemTypeExpression,
        ApiTypeModifiers ApiItemTypeModifiers,
        List<Type>? ExtensionTypes = null
    ) : ApiTypeDef(ClrType, ExtensionTypes);

    public record ApiPropertyDef
    (
        string ApiName,
        ApiTypeExpressionDef ApiTypeExpression,
        ApiTypeModifiers ApiTypeModifiers,
        string ClrName,
        ClrMemberKind ClrMemberKind,
        List<Type>? ExtensionTypes = null
    ) : ApiSchemaElementDef(ExtensionTypes);

    public record ApiTypeReferenceDef
    (
        ApiTypeKind? ApiKind,
        string? ApiName,
        Type? ClrType
    );

    public record ApiTypeExpressionDef
    (
        ApiTypeDef? ApiTypeDef,
        ApiTypeReferenceDef? ApiTypeReferenceDef
    );

    // Key
    public record ApiKeyDef
    (
        string ApiName,
        List<ApiKeyPathDef> ApiKeyPaths,
        List<Type>? ExtensionTypes = null
    ) : ApiSchemaElementDef(ExtensionTypes);

    public record ApiKeyPathDef
    (
        ApiTypeReferenceDef? ApiRootObjectTypeReference,
        List<ApiKeyPathSegmentDef> ApiKeyPathSegments,
        List<Type>? ExtensionTypes = null
    ) : ApiSchemaElementDef(ExtensionTypes);

    public record ApiKeyPathSegmentDef
    (
        string ClrMemberName,
        List<Type>? ExtensionTypes = null
    ) : ApiSchemaElementDef(ExtensionTypes);

    // Relationships
    public abstract record ApiRelationshipDef
    (
        string ApiName,
        List<Type>? ExtensionTypes = null
    ) : ApiSchemaElementDef(ExtensionTypes);

    public abstract record ApiRelationshipElementDef
    (
        ApiTypeReferenceDef ApiObjectTypeReference,
        List<Type>? ExtensionTypes = null
    ) : ApiSchemaElementDef(ExtensionTypes);

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

    public record ApiRelationshipAssociationDef
    (
        ApiTypeReferenceDef ApiObjectTypeReference,
        ApiKeyDef? ApiForeignKeyA = null,
        ApiKeyDef? ApiForeignKeyB = null,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipElementDef(ApiObjectTypeReference, ExtensionTypes);

    public record ApiRelationshipPrincipalEndDef
    (
        ApiTypeReferenceDef ApiObjectTypeReference,
        string? ApiPrincipalKeyName = null,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipElementDef(ApiObjectTypeReference, ExtensionTypes);

    public record ApiRelationshipDependentEndDef
    (
        ApiTypeReferenceDef ApiObjectTypeReference,
        ApiKeyDef? ApiForeignKey = null,
        List<Type>? ExtensionTypes = null
    ) : ApiRelationshipElementDef(ApiObjectTypeReference, ExtensionTypes);

    // Version
    public record ApiVersionDef : ApiSchemaElementDef
    {
        public ApiVersionDef(Type clrType, List<Type>? extensionTypes = null)
            : base(extensionTypes)
        {
            this.ClrType = clrType;
        }

        public ApiVersionDef(string clrMemberName, List<Type>? extensionTypes = null)
            : base(extensionTypes)
        {
            this.ClrMemberName = clrMemberName;
        }

        public Type? ClrType { get; }
        public string? ClrMemberName { get; }
    }
    #endregion

    #region Dynamic Builder Methods
    // Schema
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
            .Select(BuildTestApiRelationship)
            .Where(r => r != null)
            .Cast<ApiRelationship>()
            .ToList();

        var extensionTypeAndInstances = BuildExtensionInstances(apiSchemaDef.ExtensionTypes);

        return CreateFrozenSchema
        (
            apiName,
            apiNamedTypes,
            apiVersion,
            apiOptions,
            apiRelationships,
            extensionTypeAndInstances
        );
    }

    // Types
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

    public static ApiTypeReference? BuildApiTypeReference(ApiTypeReferenceDef? apiTypeReferenceDef)
    {
        if (apiTypeReferenceDef == null)
        {
            return default;
        }

        var apiKind = apiTypeReferenceDef.ApiKind;
        var apiName = apiTypeReferenceDef.ApiName;
        var clrType = apiTypeReferenceDef.ClrType;
        var hasInvalidApiKind = apiKind is not null && Enum.IsDefined(typeof(ApiTypeKind), apiKind) is false;

        return new ApiTypeReference(apiKind, apiName, clrType, hasInvalidApiKind: hasInvalidApiKind);
    }

    public static ApiTypeExpression? BuildApiTypeExpression(ApiTypeExpressionDef? apiTypeExpressionDef)
    {
        if (apiTypeExpressionDef == null)
        {
            return default;
        }

        var apiType = BuildTestApiType(apiTypeExpressionDef.ApiTypeDef);
        var apiTypeReference = BuildApiTypeReference(apiTypeExpressionDef.ApiTypeReferenceDef);

        return new ApiTypeExpression(apiType, apiTypeReference);
    }

    // Key

    // Relationships
    public static ApiRelationship? BuildTestApiRelationship(ApiRelationshipDef? apiRelationshipDef)
    {
        if (apiRelationshipDef == null)
        {
            return default;
        }

        var apiRelationship = (ApiRelationship)((object?)apiRelationshipDef switch
        {
            ApiRelationshipOneToOneDef d => BuildApiRelationshipOneToOne(d),
            ApiRelationshipOneToManyDef d => BuildApiRelationshipOneToMany(d),
            ApiRelationshipManyToManyDef d => BuildApiRelationshipManyToMany(d),
            _ => throw new InvalidOperationException($"Unsupported {nameof(ApiRelationshipDef)}: {apiRelationshipDef.GetType().Name}")
        });

        AttachExtensions(apiRelationship, apiRelationshipDef);

        return apiRelationship;
    }

    // Version
    public static ApiVersionDefinition BuildApiVersionDefinition(ApiVersionDef def)
    {
        var apiVersionDefinition = def.ClrMemberName is not null
            ? new ApiVersionDefinition(def.ClrMemberName)
            : new ApiVersionDefinition(def.ClrType!);

        AttachExtensions(apiVersionDefinition, def);

        return apiVersionDefinition;
    }
    #endregion

    #region Implementation Methods
    // Schema
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

    private static ApiSchema CreateFrozenSchema
    (
        string apiName,
        IEnumerable<ApiNamedType>? apiNamedTypes,
        string? apiVersion = null,
        ApiSchemaOptions? apiOptions = null,
        IEnumerable<ApiRelationship>? apiRelationships = null,
        IEnumerable<(Type ExtensionType, object ExtensionInstance)>? extensionTypeAndInstances = null
    )
    {
        var apiSchema = new ApiSchema(apiName, apiVersion, apiOptions, apiNamedTypes, apiRelationships);
        if (extensionTypeAndInstances is not null)
        {
            foreach (var (extensionType, extensionInstance) in extensionTypeAndInstances)
            {
                apiSchema.AttachExtension(extensionType, extensionInstance);
            }
        }

        var result = ApiSchemaCompiler.Compile(apiSchema);
        result.ThrowIfInvalid();
        return result.Schema!;
    }

    private static ApiSchema CreateFrozenSchema
    (
        string apiName,
        IEnumerable<ApiScalarType>? apiScalarTypes,
        IEnumerable<ApiEnumType>? apiEnumTypes,
        IEnumerable<ApiObjectType>? apiObjectTypes,
        string? apiVersion = null,
        ApiSchemaOptions? apiOptions = null,
        IEnumerable<ApiRelationship>? apiRelationships = null
    )
    {
        var apiSchema = new ApiSchema
        (
            apiName,
            apiVersion,
            apiOptions,
            apiScalarTypes,
            apiEnumTypes,
            apiObjectTypes,
            apiRelationships
        );
        var result = ApiSchemaCompiler.Compile(apiSchema);
        result.ThrowIfInvalid();
        return result.Schema!;
    }

    // Types
    private static ApiCollectionType BuildApiCollectionType(ApiCollectionTypeDef def)
    {
        var apiItemTypeExpression = BuildApiTypeExpression(def.ApiItemTypeExpression)!;
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
        var apiKeys = def.ApiKeys?.Select(BuildApiNamedKeyDefinition);
        var apiVersionDefinition = def.ApiVersion is not null
            ? BuildApiVersionDefinition(def.ApiVersion)
            : null;
        var clrType = def.ClrType;

        return new ApiObjectType
        (
            apiName,
            apiOptions,
            apiProperties,
            apiKeys,
            apiVersionDefinition,
            clrType
        );
    }

    private static ApiScalarType BuildApiScalarType(ApiScalarTypeDef def)
    {
        var apiName = def.ApiName;
        var clrType = def.ClrType;

        return new ApiScalarType(apiName, clrType);
    }

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

    private static ApiProperty BuildApiProperty(ApiPropertyDef def)
    {
        var apiName = def.ApiName;
        var apiTypeExpression = BuildApiTypeExpression(def.ApiTypeExpression)!;
        var apiTypeModifiers = def.ApiTypeModifiers;
        var clrName = def.ClrName;
        var clrMemberKind = def.ClrMemberKind;

        var apiProperty = new ApiProperty(apiName, apiTypeExpression, apiTypeModifiers, clrName, clrMemberKind);

        AttachExtensions(apiProperty, def);

        return apiProperty;
    }

    // Key
    private static ApiKeyDefinition BuildApiKeyDefinition(ApiKeyDef def)
    {
        var apiKeyPaths = def.ApiKeyPaths.Select(BuildApiKeyPath);

        var apiKeyDefinition = new ApiKeyDefinition(apiKeyPaths);

        AttachExtensions(apiKeyDefinition, def);

        return apiKeyDefinition;
    }

    private static ApiNamedKeyDefinition BuildApiNamedKeyDefinition(ApiKeyDef def)
    {
        var apiName = def.ApiName;
        var apiKeyPaths = def.ApiKeyPaths.Select(BuildApiKeyPath);

        var apiNamedKeyDefinition = new ApiNamedKeyDefinition(apiName, apiKeyPaths);

        AttachExtensions(apiNamedKeyDefinition, def);

        return apiNamedKeyDefinition;
    }

    private static ApiKeyPath BuildApiKeyPath(ApiKeyPathDef def)
    {
        var apiRootObjectTypeReference = BuildApiTypeReference(def.ApiRootObjectTypeReference);
        var apiKeyPathSegments = def.ApiKeyPathSegments.Select(BuildApiKeyPathSegment);

        var apiKeyPath = new ApiKeyPath(apiRootObjectTypeReference, apiKeyPathSegments);

        AttachExtensions(apiKeyPath, def);

        return apiKeyPath;
    }

    private static ApiKeyPathSegment BuildApiKeyPathSegment(ApiKeyPathSegmentDef def)
    {
        var clrMemberName = def.ClrMemberName;

        var apiKeyPathSegment = new ApiKeyPathSegment(clrMemberName);

        AttachExtensions(apiKeyPathSegment, def);

        return apiKeyPathSegment;
    }

    // Relationships
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
        var apiObjectTypeReference = BuildApiTypeReference(def.ApiObjectTypeReference)!;
        var apiForeignKeyA = def.ApiForeignKeyA != null ? BuildApiKeyDefinition(def.ApiForeignKeyA) : null;
        var apiForeignKeyB = def.ApiForeignKeyB != null ? BuildApiKeyDefinition(def.ApiForeignKeyB) : null;

        var apiRelationshipAssociation = apiForeignKeyA != null && apiForeignKeyB != null
            ? new ApiRelationshipAssociation(apiObjectTypeReference, apiForeignKeyA, apiForeignKeyB)
            : new ApiRelationshipAssociation(apiObjectTypeReference);

        AttachExtensions(apiRelationshipAssociation, def);

        return apiRelationshipAssociation;
    }

    private static ApiRelationshipPrincipalEnd BuildApiRelationshipPrincipalEnd(ApiRelationshipPrincipalEndDef def)
    {
        var apiObjectTypeReference = BuildApiTypeReference(def.ApiObjectTypeReference)!;
        var apiPrincipalKeyName = def.ApiPrincipalKeyName;

        var apiRelationshipPrincipalEnd = new ApiRelationshipPrincipalEnd
        (
            apiObjectTypeReference,
            apiPrincipalKeyName
        );

        AttachExtensions(apiRelationshipPrincipalEnd, def);

        return apiRelationshipPrincipalEnd;
    }

    private static ApiRelationshipDependentEnd BuildApiRelationshipDependentEnd(ApiRelationshipDependentEndDef def)
    {
        var apiObjectTypeReference = BuildApiTypeReference(def.ApiObjectTypeReference)!;
        var apiForeignKey = def.ApiForeignKey != null ? BuildApiKeyDefinition(def.ApiForeignKey) : null;

        var apiRelationshipDependentEnd = apiForeignKey != null
            ? new ApiRelationshipDependentEnd(apiObjectTypeReference, apiForeignKey)
            : new ApiRelationshipDependentEnd(apiObjectTypeReference);

        AttachExtensions(apiRelationshipDependentEnd, def);

        return apiRelationshipDependentEnd;
    }

    // Extensions
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
