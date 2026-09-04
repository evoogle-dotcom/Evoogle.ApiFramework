// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Text.Json.Nodes;

namespace Evoogle.ApiFramework.Schema;

// Mutations referenced from Expression<Action<JsonObject>> theory data must be single static-method
// calls resolvable by System.Linq.Dynamic.Core, so all JSON mutation logic lives here.
[DynamicLinqType]
internal static class EnumJsonDeserializationPolicyTestsFactory
{
    #region Initialization Issue Factory Methods
    public static void SetFirstPropertyApiTypeModifiersToUnknownString(JsonObject schema)
        => GetFirstProperty(schema)[nameof(ApiProperty.ApiTypeModifiers)] = "Unknown";

    public static void SetFirstPropertyApiTypeModifiersToNull(JsonObject schema)
        => GetFirstProperty(schema)[nameof(ApiProperty.ApiTypeModifiers)] = null;

    public static void SetFirstCollectionTypeApiItemTypeModifiersToIncompatibleToken(JsonObject schema)
        => GetFirstCollectionType(schema)[nameof(ApiCollectionType.ApiItemTypeModifiers)] = 1;

    public static void SetSchemaApiKeyNullHandlingToNull(JsonObject schema)
        => schema[nameof(ApiSchema.ApiOptions)]!.AsObject()[nameof(ApiSchemaOptions.ApiKeyNullHandling)] = null;

    public static void SetFirstObjectTypeApiKeyNullHandlingToUnknownString(JsonObject schema)
        => GetFirstObjectType(schema)[nameof(ApiObjectType.ApiOptions)] = new JsonObject
        {
            [nameof(ApiObjectTypeOptions.ApiKeyNullHandling)] = "Unknown",
        };

    public static void SetFirstRelationshipApiDeleteBehaviorToIncompatibleToken(JsonObject schema)
        => schema[nameof(ApiSchema.ApiRelationships)]!.AsArray()[0]!.AsObject()[nameof(ApiRelationship.ApiDeleteBehavior)] = new JsonObject();

    public static void SetFirstPropertyApiTypeApiKindToUnknownString(JsonObject schema)
        => GetFirstProperty(schema)[nameof(ApiProperty.ApiType)]!.AsObject()[nameof(ApiTypeExpression.ApiKind)] = "Unknown";
    #endregion

    #region Default And Inheritance Factory Methods
    public static void RemoveSchemaApiKeyNullHandling(JsonObject schema)
        => schema[nameof(ApiSchema.ApiOptions)]!.AsObject().Remove(nameof(ApiSchemaOptions.ApiKeyNullHandling));

    public static void SetFirstObjectTypeApiKeyNullHandlingToNull(JsonObject schema)
        => GetFirstObjectType(schema)[nameof(ApiObjectType.ApiOptions)] = new JsonObject
        {
            [nameof(ApiObjectTypeOptions.ApiKeyNullHandling)] = null,
        };
    #endregion

    #region Structural Discriminator Factory Methods
    // Shared by ApiType and ApiRelationship theory rows; both discriminators are named "ApiKind".
    public static void RemoveApiKind(JsonObject value)
        => value.Remove(nameof(ApiType.ApiKind));

    public static void SetApiKindToNull(JsonObject value)
        => value[nameof(ApiType.ApiKind)] = null;

    public static void SetApiKindToWhitespace(JsonObject value)
        => value[nameof(ApiType.ApiKind)] = " ";

    public static void SetApiKindToUnknownString(JsonObject value)
        => value[nameof(ApiType.ApiKind)] = "Unknown";

    public static void SetApiKindToIncompatibleToken(JsonObject value)
        => value[nameof(ApiType.ApiKind)] = new JsonArray();
    #endregion

    #region Navigation Helper Methods
    private static JsonObject GetFirstCollectionType(JsonObject json)
    {
        foreach (var apiObjectTypeNode in json[nameof(ApiSchema.ApiObjectTypes)]!.AsArray())
        {
            var apiObjectType = apiObjectTypeNode!.AsObject();
            foreach (var apiPropertyNode in apiObjectType[nameof(ApiObjectType.ApiProperties)]!.AsArray())
            {
                var apiTypeExpression = apiPropertyNode!.AsObject()[nameof(ApiProperty.ApiType)]!.AsObject();
                var apiInlineType = apiTypeExpression[nameof(ApiTypeExpression.ApiInlineType)]?.AsObject();
                if (apiInlineType?[nameof(ApiType.ApiKind)]?.GetValue<string>() == nameof(ApiTypeKind.Collection))
                {
                    return apiInlineType;
                }
            }
        }

        throw new InvalidOperationException("The schema contains no inline collection type.");
    }

    private static JsonObject GetFirstObjectType(JsonObject json)
        => json[nameof(ApiSchema.ApiObjectTypes)]!.AsArray()[0]!.AsObject();

    private static JsonObject GetFirstProperty(JsonObject json)
        => GetFirstObjectType(json)[nameof(ApiObjectType.ApiProperties)]!.AsArray()[0]!.AsObject();
    #endregion
}
