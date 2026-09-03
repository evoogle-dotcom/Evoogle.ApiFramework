// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.XUnit;
using Evoogle.XUnit.Json;

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

        [JsonConverter(typeof(ExpressionActionJsonConverter<JsonObject>))]
        public required Expression<Action<JsonObject>> UpdateJson { get; init; }
        #endregion

        #region Calculated Properties
        private Exception? ActualException { get; set; }

        private string? SourceJson { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var json = CreateSchemaJson(this.SchemaKind);
            var updateJson = this.UpdateJson.Compile() ?? throw new InvalidOperationException($"Unable to compile {nameof(this.UpdateJson)} into an action.");
            updateJson(json);
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

        [JsonConverter(typeof(ExpressionActionJsonConverter<JsonObject>))]
        public required Expression<Action<JsonObject>> UpdateJson { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ActualSchema { get; set; }

        private string? SourceJson { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var json = CreateSchemaJson(ApiSchemaKind.Simple);
            var updateJson = this.UpdateJson.Compile() ?? throw new InvalidOperationException($"Unable to compile {nameof(this.UpdateJson)} into an action.");
            updateJson(json);
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

        [JsonConverter(typeof(ExpressionActionJsonConverter<JsonObject>))]
        public required Expression<Action<JsonObject>> UpdateJson { get; init; }
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

            var updateJson = this.UpdateJson.Compile() ?? throw new InvalidOperationException($"Unable to compile {nameof(this.UpdateJson)} into an action.");
            updateJson(value);
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
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetFirstPropertyApiTypeModifiersToUnknownString(a),
        },
        new InitializationIssueTest
        {
            Name = "Property type modifiers reject a JSON null enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedCode = ApiInitializationCode.ApiPropertyInvalidApiTypeModifiers,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetFirstPropertyApiTypeModifiersToNull(a),
        },
        new InitializationIssueTest
        {
            Name = "Collection item type modifiers reject an incompatible JSON enum token",
            SchemaKind = ApiSchemaKind.Commerce,
            ExpectedCode = ApiInitializationCode.ApiCollectionTypeInvalidApiItemTypeModifiers,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetFirstCollectionTypeApiItemTypeModifiersToIncompatibleToken(a),
        },
        new InitializationIssueTest
        {
            Name = "Schema key null handling rejects a JSON null enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedCode = ApiInitializationCode.ApiSchemaInvalidApiKeyNullHandling,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetSchemaApiKeyNullHandlingToNull(a),
        },
        new InitializationIssueTest
        {
            Name = "Object key null handling rejects an unknown JSON enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedCode = ApiInitializationCode.ApiObjectTypeInvalidApiKeyNullHandling,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetFirstObjectTypeApiKeyNullHandlingToUnknownString(a),
        },
        new InitializationIssueTest
        {
            Name = "Relationship delete behavior rejects an incompatible JSON enum token",
            SchemaKind = ApiSchemaKind.Relationship,
            ExpectedCode = ApiInitializationCode.ApiRelationshipInvalidApiDeleteBehavior,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetFirstRelationshipApiDeleteBehaviorToIncompatibleToken(a),
        },
        new InitializationIssueTest
        {
            Name = "Type expression API kind rejects an unknown JSON enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedCode = ApiInitializationCode.ApiTypeExpressionInvalidApiKind,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetFirstPropertyApiTypeApiKindToUnknownString(a),
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] EnumJsonDefaultAndInheritanceTheoryData =>
    [
        new DefaultAndInheritanceTest
        {
            Name = "Omitted schema key null handling uses the default",
            ExpectedSchemaNullHandling = ApiKeyNullHandling.UseDefaultOnNull,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.RemoveSchemaApiKeyNullHandling(a),
        },
        new DefaultAndInheritanceTest
        {
            Name = "Null object key null handling preserves inheritance",
            ExpectedSchemaNullHandling = ApiKeyNullHandling.UseDefaultOnNull,
            ExpectsInheritedObjectNullHandling = true,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetFirstObjectTypeApiKeyNullHandlingToNull(a),
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] StructuralEnumJsonDiscriminatorTheoryData =>
    [
        new StructuralDiscriminatorTest
        {
            Name = "Type discriminator rejects an omitted JSON enum value",
            IsRelationship = false,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.RemoveApiKind(a),
        },
        new StructuralDiscriminatorTest
        {
            Name = "Type discriminator rejects a JSON null enum value",
            IsRelationship = false,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetApiKindToNull(a),
        },
        new StructuralDiscriminatorTest
        {
            Name = "Type discriminator rejects a whitespace JSON enum value",
            IsRelationship = false,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetApiKindToWhitespace(a),
        },
        new StructuralDiscriminatorTest
        {
            Name = "Type discriminator rejects an unknown JSON enum value",
            IsRelationship = false,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetApiKindToUnknownString(a),
        },
        new StructuralDiscriminatorTest
        {
            Name = "Type discriminator rejects an incompatible JSON enum token",
            IsRelationship = false,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetApiKindToIncompatibleToken(a),
        },
        new StructuralDiscriminatorTest
        {
            Name = "Relationship discriminator rejects an omitted JSON enum value",
            IsRelationship = true,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.RemoveApiKind(a),
        },
        new StructuralDiscriminatorTest
        {
            Name = "Relationship discriminator rejects a JSON null enum value",
            IsRelationship = true,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetApiKindToNull(a),
        },
        new StructuralDiscriminatorTest
        {
            Name = "Relationship discriminator rejects a whitespace JSON enum value",
            IsRelationship = true,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetApiKindToWhitespace(a),
        },
        new StructuralDiscriminatorTest
        {
            Name = "Relationship discriminator rejects an unknown JSON enum value",
            IsRelationship = true,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetApiKindToUnknownString(a),
        },
        new StructuralDiscriminatorTest
        {
            Name = "Relationship discriminator rejects an incompatible JSON enum token",
            IsRelationship = true,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetApiKindToIncompatibleToken(a),
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
    #endregion
}
