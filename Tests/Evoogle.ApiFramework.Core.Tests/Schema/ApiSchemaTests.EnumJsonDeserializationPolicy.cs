// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Nodes;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
    #region Test Classes
    private sealed class InitializationIssueTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiInitializationCode ExpectedCode { get; init; }

        public required ApiSchemaKind SchemaKind { get; init; }

        public required Action<JsonObject> UpdateJson { get; init; }
        #endregion

        #region Calculated Properties
        private Exception? ActualException { get; set; }

        private string? SourceJson { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var json = CreateSchemaJson(this.SchemaKind);
            this.UpdateJson(json);
            this.SourceJson = json.ToJsonString();
        }

        protected override void Act()
        {
            try
            {
                JsonSerializer.Deserialize<ApiSchema>(this.SourceJson!);
            }
            catch (Exception ex)
            {
                this.ActualException = ex;
            }
        }

        protected override void Assert()
        {
            var exception = this.ActualException.Should().BeOfType<ApiSchemaInitializationException>().Which;
            exception.Issues.Should().Contain(issue => issue.Code == this.ExpectedCode);
        }
        #endregion
    }

    private sealed class DefaultAndInheritanceTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiKeyNullHandling ExpectedSchemaNullHandling { get; init; }

        public bool ExpectsInheritedObjectNullHandling { get; init; }

        public required Action<JsonObject> UpdateJson { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ActualSchema { get; set; }

        private string? SourceJson { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var json = CreateSchemaJson(ApiSchemaKind.Simple);
            this.UpdateJson(json);
            this.SourceJson = json.ToJsonString();
        }

        protected override void Act()
        {
            this.ActualSchema = JsonSerializer.Deserialize<ApiSchema>(this.SourceJson!);
        }

        protected override void Assert()
        {
            this.ActualSchema.Should().NotBeNull();
            this.ActualSchema!.ApiOptions.ApiKeyNullHandling.Should().Be(this.ExpectedSchemaNullHandling);

            if (this.ExpectsInheritedObjectNullHandling)
            {
                this.ActualSchema.ApiObjectTypes[0].ApiOptions!.ApiKeyNullHandling.Should().BeNull();
            }
        }
        #endregion
    }

    private sealed class StructuralDiscriminatorTest : XUnitTest
    {
        #region User Supplied Properties
        public required bool IsRelationship { get; init; }

        public required Action<JsonObject> UpdateJson { get; init; }
        #endregion

        #region Calculated Properties
        private Exception? ActualException { get; set; }

        private string? SourceJson { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var schema = CreateSchemaJson(this.IsRelationship ? ApiSchemaKind.Relationship : ApiSchemaKind.Simple);
            var value = this.IsRelationship
                ? schema[nameof(ApiSchema.ApiRelationships)]!.AsArray()[0]!.AsObject()
                : schema[nameof(ApiSchema.ApiScalarTypes)]!.AsArray()[0]!.AsObject();

            this.UpdateJson(value);
            this.SourceJson = value.ToJsonString();
        }

        protected override void Act()
        {
            try
            {
                if (this.IsRelationship)
                {
                    JsonSerializer.Deserialize<ApiRelationship>(this.SourceJson!);
                }
                else
                {
                    JsonSerializer.Deserialize<ApiType>(this.SourceJson!);
                }
            }
            catch (Exception ex)
            {
                this.ActualException = ex;
            }
        }

        protected override void Assert()
        {
            this.ActualException.Should().BeOfType<JsonException>();
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] EnumJsonInitializationIssueTheoryData =>
    [
        new InitializationIssueTest
        {
            Name = "Property type modifiers reject an unknown JSON enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedCode = ApiInitializationCode.ApiPropertyInvalidApiTypeModifiers,
            UpdateJson = static json => GetFirstProperty(json)[nameof(ApiProperty.ApiTypeModifiers)] = "Unknown",
        },
        new InitializationIssueTest
        {
            Name = "Property type modifiers reject a JSON null enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedCode = ApiInitializationCode.ApiPropertyInvalidApiTypeModifiers,
            UpdateJson = static json => GetFirstProperty(json)[nameof(ApiProperty.ApiTypeModifiers)] = null,
        },
        new InitializationIssueTest
        {
            Name = "Collection item type modifiers reject an incompatible JSON enum token",
            SchemaKind = ApiSchemaKind.Commerce,
            ExpectedCode = ApiInitializationCode.ApiCollectionTypeInvalidApiItemTypeModifiers,
            UpdateJson = static json => GetFirstCollectionType(json)[nameof(ApiCollectionType.ApiItemTypeModifiers)] = 1,
        },
        new InitializationIssueTest
        {
            Name = "Schema key null handling rejects a JSON null enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedCode = ApiInitializationCode.ApiSchemaInvalidApiKeyNullHandling,
            UpdateJson = static json => json[nameof(ApiSchema.ApiOptions)]!.AsObject()[nameof(ApiSchemaOptions.ApiKeyNullHandling)] = null,
        },
        new InitializationIssueTest
        {
            Name = "Object key null handling rejects an unknown JSON enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedCode = ApiInitializationCode.ApiObjectTypeInvalidApiKeyNullHandling,
            UpdateJson = static json => GetFirstObjectType(json)[nameof(ApiObjectType.ApiOptions)] = new JsonObject
            {
                [nameof(ApiObjectTypeOptions.ApiKeyNullHandling)] = "Unknown",
            },
        },
        new InitializationIssueTest
        {
            Name = "Relationship delete behavior rejects an incompatible JSON enum token",
            SchemaKind = ApiSchemaKind.Relationship,
            ExpectedCode = ApiInitializationCode.ApiRelationshipInvalidApiDeleteBehavior,
            UpdateJson = static json => json[nameof(ApiSchema.ApiRelationships)]!.AsArray()[0]!.AsObject()[nameof(ApiRelationship.ApiDeleteBehavior)] = new JsonObject(),
        },
        new InitializationIssueTest
        {
            Name = "Type expression API kind rejects an unknown JSON enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedCode = ApiInitializationCode.ApiTypeExpressionInvalidApiKind,
            UpdateJson = static json => GetFirstProperty(json)[nameof(ApiProperty.ApiType)]!.AsObject()[nameof(ApiTypeExpression.ApiKind)] = "Unknown",
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] EnumJsonDefaultAndInheritanceTheoryData =>
    [
        new DefaultAndInheritanceTest
        {
            Name = "Omitted schema key null handling uses the default",
            ExpectedSchemaNullHandling = ApiKeyNullHandling.UseDefaultOnNull,
            UpdateJson = static json => json[nameof(ApiSchema.ApiOptions)]!.AsObject().Remove(nameof(ApiSchemaOptions.ApiKeyNullHandling)),
        },
        new DefaultAndInheritanceTest
        {
            Name = "Null object key null handling preserves inheritance",
            ExpectedSchemaNullHandling = ApiKeyNullHandling.UseDefaultOnNull,
            ExpectsInheritedObjectNullHandling = true,
            UpdateJson = static json => GetFirstObjectType(json)[nameof(ApiObjectType.ApiOptions)] = new JsonObject
            {
                [nameof(ApiObjectTypeOptions.ApiKeyNullHandling)] = null,
            },
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] StructuralEnumJsonDiscriminatorTheoryData =>
    [
        new StructuralDiscriminatorTest
        {
            Name = "Type discriminator rejects an omitted JSON enum value",
            IsRelationship = false,
            UpdateJson = static json => json.Remove(nameof(ApiType.ApiKind)),
        },
        new StructuralDiscriminatorTest
        {
            Name = "Type discriminator rejects a JSON null enum value",
            IsRelationship = false,
            UpdateJson = static json => json[nameof(ApiType.ApiKind)] = null,
        },
        new StructuralDiscriminatorTest
        {
            Name = "Type discriminator rejects a whitespace JSON enum value",
            IsRelationship = false,
            UpdateJson = static json => json[nameof(ApiType.ApiKind)] = " ",
        },
        new StructuralDiscriminatorTest
        {
            Name = "Type discriminator rejects an unknown JSON enum value",
            IsRelationship = false,
            UpdateJson = static json => json[nameof(ApiType.ApiKind)] = "Unknown",
        },
        new StructuralDiscriminatorTest
        {
            Name = "Type discriminator rejects an incompatible JSON enum token",
            IsRelationship = false,
            UpdateJson = static json => json[nameof(ApiType.ApiKind)] = new JsonArray(),
        },
        new StructuralDiscriminatorTest
        {
            Name = "Relationship discriminator rejects an omitted JSON enum value",
            IsRelationship = true,
            UpdateJson = static json => json.Remove(nameof(ApiRelationship.ApiKind)),
        },
        new StructuralDiscriminatorTest
        {
            Name = "Relationship discriminator rejects a JSON null enum value",
            IsRelationship = true,
            UpdateJson = static json => json[nameof(ApiRelationship.ApiKind)] = null,
        },
        new StructuralDiscriminatorTest
        {
            Name = "Relationship discriminator rejects a whitespace JSON enum value",
            IsRelationship = true,
            UpdateJson = static json => json[nameof(ApiRelationship.ApiKind)] = " ",
        },
        new StructuralDiscriminatorTest
        {
            Name = "Relationship discriminator rejects an unknown JSON enum value",
            IsRelationship = true,
            UpdateJson = static json => json[nameof(ApiRelationship.ApiKind)] = "Unknown",
        },
        new StructuralDiscriminatorTest
        {
            Name = "Relationship discriminator rejects an incompatible JSON enum token",
            IsRelationship = true,
            UpdateJson = static json => json[nameof(ApiRelationship.ApiKind)] = new JsonArray(),
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(EnumJsonInitializationIssueTheoryData))]
    public void EnumJsonInitializationIssues(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(EnumJsonDefaultAndInheritanceTheoryData))]
    public void EnumJsonDefaultsAndInheritance(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(StructuralEnumJsonDiscriminatorTheoryData))]
    public void StructuralEnumJsonDiscriminators(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Helper Methods
    private static JsonObject CreateSchemaJson(ApiSchemaKind schemaKind)
    {
        var schema = BuildTestApiSchema(schemaKind);
        var json = JsonNode.Parse(JsonSerializer.Serialize(schema));

        return json?.AsObject()
            ?? throw new InvalidOperationException($"{nameof(ApiSchema)} JSON was not an object.");
    }

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
