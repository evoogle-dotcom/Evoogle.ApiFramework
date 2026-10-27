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
using Evoogle.Extensions;
using Evoogle.XUnit;
using Evoogle.XUnit.Json;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
    #region Test Classes
    private sealed record EnumDiagnostic
    (
        ApiSchemaCompilationCode Code,
        ApiSchemaCompilationSeverity Severity,
        string ApiPath
    );

    private sealed class CompilationIssueTest : XUnitTest
    {
        #region User Supplied Properties
        public required EnumDiagnostic[] ExpectedIssues { get; init; }

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
            var updateJson = this.UpdateJson.Compile()
                ?? throw new InvalidOperationException($"Unable to compile {nameof(this.UpdateJson)} into an action.");
            updateJson(json);
            this.SourceJson = json.ToJsonString();
            this.WriteLine($"Schema Fixture: {this.SchemaKind}; Mutation: {this.UpdateJson}");
            this.WriteLine($"Expected Boundary: {nameof(ApiSchemaCompilationException)}; Published Schema: null");
            foreach (var issue in this.ExpectedIssues)
            {
                this.WriteLine($"Expected: {issue.Severity}; {issue.Code}; {issue.ApiPath}");
            }
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

            this.WriteLine($"Captured Exception: {this.ActualException.SafeToString()}");
            if (this.ActualException is ApiSchemaCompilationException exception)
            {
                foreach (var issue in exception.Issues)
                {
                    this.WriteLine($"Actual: {issue.Severity}; {issue.Code}; {issue.ApiPath}");
                }
            }
        }

        protected override void Assert()
        {
            var exception = this.ActualException.Should().BeOfType<ApiSchemaCompilationException>().Which;

            exception.Issues.Select(issue => new EnumDiagnostic(issue.Code, issue.Severity, issue.ApiPath)).Should().BeEquivalentTo(this.ExpectedIssues);
            exception.IsValid.Should().BeFalse();
            exception.Result.Schema.Should().BeNull();

            exception.Errors.Should().HaveCount(this.ExpectedIssues.Count(issue => issue.Severity == ApiSchemaCompilationSeverity.Error));
            exception.Errors.Should().BeEquivalentTo(exception.Issues.Where(issue => issue.Severity == ApiSchemaCompilationSeverity.Error));

            exception.Warnings.Should().HaveCount(this.ExpectedIssues.Count(issue => issue.Severity == ApiSchemaCompilationSeverity.Warning));
            exception.Warnings.Should().BeEquivalentTo(exception.Issues.Where(issue => issue.Severity == ApiSchemaCompilationSeverity.Warning));
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

        protected override void Act() => this.ActualSchema = JsonSerializer.Deserialize<ApiSchema>(this.SourceJson!);

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

        protected override void Assert() => this.ActualException.Should().BeOfType<JsonException>();
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] EnumJsonCompilationIssueTheoryData =>
    [
        new CompilationIssueTest
        {
            Name = "Property nullability rejects an unknown JSON enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedIssues = SimpleEnumIssues(ApiSchemaCompilationCode.ApiPropertyInvalidApiNullability, "ApiSchema[\"Simple\"].ApiObjectType[\"Company\"].ApiProperty[\"Id\"]"),
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetFirstPropertyApiNullabilityToUnknownString(a),
        },
        new CompilationIssueTest
        {
            Name = "Property nullability rejects a JSON null enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedIssues = SimpleEnumIssues(ApiSchemaCompilationCode.ApiPropertyInvalidApiNullability, "ApiSchema[\"Simple\"].ApiObjectType[\"Company\"].ApiProperty[\"Id\"]"),
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetFirstPropertyApiNullabilityToNull(a),
        },
        new CompilationIssueTest
        {
            Name = "Collection item nullability rejects an incompatible JSON enum token",
            SchemaKind = ApiSchemaKind.Commerce,
            ExpectedIssues =
            [
                new EnumDiagnostic
                (
                    ApiSchemaCompilationCode.ApiCollectionTypeInvalidApiItemNullability,
                    ApiSchemaCompilationSeverity.Error,
                    "ApiSchema[\"Commerce\"].ApiObjectType[\"Category\"].ApiProperty[\"Children\"].ApiCollectionType"
                ),
                new EnumDiagnostic
                (
                    ApiSchemaCompilationCode.ApiCollectionItemNullableNonNullableMismatch,
                    ApiSchemaCompilationSeverity.Warning,
                    "ApiSchema[\"Commerce\"].ApiObjectType[\"Category\"].ApiProperty[\"Children\"]"
                )
            ],
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetFirstCollectionTypeApiItemNullabilityToIncompatibleToken(a),
        },
        new CompilationIssueTest
        {
            Name = "Schema key null handling rejects a JSON null enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedIssues = SimpleEnumIssues(ApiSchemaCompilationCode.ApiSchemaInvalidApiKeyNullHandling, "ApiSchema[\"Simple\"]"),
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetSchemaApiKeyNullHandlingToNull(a),
        },
        new CompilationIssueTest
        {
            Name = "Object key null handling rejects an unknown JSON enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedIssues = SimpleEnumIssues(ApiSchemaCompilationCode.ApiObjectTypeInvalidApiKeyNullHandling, "ApiSchema[\"Simple\"].ApiObjectType[\"Company\"]"),
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetFirstObjectTypeApiKeyNullHandlingToUnknownString(a),
        },
        new CompilationIssueTest
        {
            Name = "Relationship delete behavior rejects an incompatible JSON enum token",
            SchemaKind = ApiSchemaKind.Relationship,
            ExpectedIssues =
            [
                new EnumDiagnostic
                (
                    ApiSchemaCompilationCode.ApiRelationshipInvalidApiDeleteBehavior,
                    ApiSchemaCompilationSeverity.Error,
                    "ApiSchema[\"Relationship\"].ApiRelationshipOneToMany[" +
                    "\"REL_CatalogItem_OrderLine_1toN_ViaNestedComposite\"]"
                )
            ],
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetFirstRelationshipApiDeleteBehaviorToIncompatibleToken(a),
        },
        new CompilationIssueTest
        {
            Name = "Type expression API kind rejects an unknown JSON enum value",
            SchemaKind = ApiSchemaKind.Simple,
            ExpectedIssues = SimpleEnumIssues(ApiSchemaCompilationCode.ApiTypeReferenceInvalidApiKind, "ApiSchema[\"Simple\"].ApiObjectType[\"Company\"].ApiProperty[\"Id\"]"),
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.SetFirstPropertyApiTypeApiKindToUnknownString(a),
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] EnumJsonDefaultAndInheritanceTheoryData =>
    [
        new DefaultAndInheritanceTest
        {
            Name = "Omitted schema key null handling uses the default",
            ExpectedSchemaNullHandling = ApiKeyNullHandling.UseEmptyOnNull,
            UpdateJson = a => EnumJsonDeserializationPolicyTestsFactory.RemoveSchemaApiKeyNullHandling(a),
        },
        new DefaultAndInheritanceTest
        {
            Name = "Null object key null handling preserves inheritance",
            ExpectedSchemaNullHandling = ApiKeyNullHandling.UseEmptyOnNull,
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
    [MemberData(nameof(EnumJsonCompilationIssueTheoryData))]
    public void DeserializeInvalidEnumReportsErrorsAtOwningElementPaths(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(EnumJsonDefaultAndInheritanceTheoryData))]
    public void EnumJsonDefaultsAndInheritance(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(StructuralEnumJsonDiscriminatorTheoryData))]
    public void StructuralEnumJsonDiscriminators(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Helper Methods
    private static EnumDiagnostic[] SimpleEnumIssues(ApiSchemaCompilationCode code, string apiPath) =>
    [
        new EnumDiagnostic(code, ApiSchemaCompilationSeverity.Error, apiPath),
        new EnumDiagnostic(ApiSchemaCompilationCode.ApiObjectTypeNullOrEmptyProperties, ApiSchemaCompilationSeverity.Warning, "ApiSchema[\"Simple\"].ApiObjectType[\"Empty\"]")
    ];

    private static JsonObject CreateSchemaJson(ApiSchemaKind schemaKind)
    {
        var schema = BuildTestApiSchema(schemaKind);
        var json = JsonNode.Parse(JsonSerializer.Serialize(schema));

        return json?.AsObject() ?? throw new InvalidOperationException($"{nameof(ApiSchema)} JSON was not an object.");
    }
    #endregion
}
