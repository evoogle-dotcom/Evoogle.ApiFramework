// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Configuration;
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Json;

public class SemanticDefaultJsonSerializationTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    public enum DefaultScenario
    {
        SchemaVersion,
        SchemaKeyNullHandling,
        ObjectKeyNullHandling,
        PropertyModifiers,
        CollectionItemModifiers,
        QueryOutputModifiers,
        QueryOutputComposition,
        ParameterModifiers,
        ParameterRequiresArgument,
        OneToOneDeleteBehavior,
        OneToManyDeleteBehavior,
        ManyToManyDeleteBehavior,
    }

    public class RoundtripTest : XUnitTest
    {
        #region User Supplied Properties
        public required DefaultScenario Scenario { get; init; }
        public required string PropertyName { get; init; }
        public string? ExpectedValue { get; init; }
        public required JsonIgnoreCondition IgnoreCondition { get; init; }
        public required bool UsesCamelCase { get; init; }
        public required bool ExpectedIsWritten { get; init; }
        #endregion

        #region Calculated Properties
        private object SourceMetadata { get; set; } = null!;
        private Type MetadataType { get; set; } = null!;
        private JsonSerializerOptions Options { get; set; } = null!;
        private string JsonPropertyName { get; set; } = null!;
        private string? ActualValue { get; set; }
        private bool ActualIsWritten { get; set; }
        private Exception? ActualException { get; set; }
        private RoundtripTest RestoredTestCase { get; set; } = null!;
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = this.IgnoreCondition,
                PropertyNamingPolicy = this.UsesCamelCase ? JsonNamingPolicy.CamelCase : null,
            };
            this.JsonPropertyName = this.Options.PropertyNamingPolicy?.ConvertName(this.PropertyName)
                ?? this.PropertyName;
            this.SourceMetadata = CreateMetadata(this.Scenario, this.ExpectedValue);
            this.MetadataType = this.SourceMetadata switch
            {
                ApiRelationship => typeof(ApiRelationship),
                ApiType => typeof(ApiType),
                _ => this.SourceMetadata.GetType()
            };

            this.WriteLine($"Scenario:      {this.Scenario}");
            this.WriteLine($"JSON Property: {this.JsonPropertyName}");
            this.WriteLine($"Ignore Policy: {this.IgnoreCondition}");
            this.WriteLine();
            this.WriteLine($"Supplied Value: {this.ExpectedValue ?? "<null>"}");
            this.WriteLine();
            this.WriteLine($"Expected Value:     {this.ExpectedValue ?? "<null>"}");
            this.WriteLine($"Expected Written:   {this.ExpectedIsWritten}");
            this.WriteLine("Expected Exception: None");

            var serializer = new XUnitTestSerializer();
            this.RestoredTestCase = (RoundtripTest)serializer.Deserialize(this.GetType(), serializer.Serialize(this));
        }

        protected override void Act()
        {
            try
            {
                var json = JsonSerializer.Serialize(this.SourceMetadata, this.MetadataType, this.Options);
                using var document = JsonDocument.Parse(json);
                this.ActualIsWritten = document.RootElement.TryGetProperty(this.JsonPropertyName, out _);
                var restored = JsonSerializer.Deserialize(json, this.MetadataType, this.Options)
                    ?? throw new JsonException("The schema metadata deserialized to null.");
                this.ActualValue = restored.GetType().GetProperty(this.PropertyName)!.GetValue(restored)?.ToString();
            }
            catch (Exception exception)
            {
                this.ActualException = exception;
            }

            this.WriteLine();
            this.WriteLine($"Actual Value:   {this.ActualValue ?? "<null>"}");
            this.WriteLine($"Actual Written: {this.ActualIsWritten}");
            this.WriteLine();
            this.WriteLine($"Captured Exception: {this.ActualException?.ToString() ?? "None"}");
        }

        protected override void Assert()
        {
            this.RestoredTestCase.Should().BeEquivalentTo(this);
            this.ActualException.Should().BeNull();
            this.ActualIsWritten.Should().Be(this.ExpectedIsWritten);
            this.ActualValue.Should().Be(this.ExpectedValue);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] RoundtripTheoryData =>
    [.. (
        from scenario in Enum.GetValues<DefaultScenario>()
        from usesSemanticDefault in new[] { true, false }
        from usesCamelCase in new[] { false, true }
        from condition in new[]
        {
            JsonIgnoreCondition.Never,
            JsonIgnoreCondition.WhenWritingNull,
            JsonIgnoreCondition.WhenWritingDefault
        }
        let expectation = GetExpectation(scenario, usesSemanticDefault)
        let isInherited = scenario == DefaultScenario.ObjectKeyNullHandling && usesSemanticDefault
        select new TheoryDataRow<IXUnitTest>(new RoundtripTest
        {
            Name = $"{scenario}, SemanticDefault={usesSemanticDefault}, CamelCase={usesCamelCase}, {condition}",
            Scenario = scenario,
            PropertyName = expectation.PropertyName,
            ExpectedValue = expectation.Value,
            IgnoreCondition = condition,
            UsesCamelCase = usesCamelCase,
            ExpectedIsWritten = condition == JsonIgnoreCondition.Never
                || (condition == JsonIgnoreCondition.WhenWritingNull && !isInherited)
                || (condition == JsonIgnoreCondition.WhenWritingDefault && !usesSemanticDefault),
        })
    )];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(RoundtripTheoryData))]
    public void RoundtripPreservesSemanticDefaultsAndExplicitOverrides(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Factory Methods
    private static object CreateMetadata(DefaultScenario scenario, string? suppliedValue)
    {
        var textType = new ApiTypeExpression(new ApiScalarType("Text", typeof(string)));
        var principal = new ApiRelationshipPrincipalEnd(new ApiTypeReference(ApiTypeKind.Object, "Principal"));
        var dependent = new ApiRelationshipDependentEnd(new ApiTypeReference(ApiTypeKind.Object, "Dependent"));
        return scenario switch
        {
            DefaultScenario.SchemaVersion => new ApiSchemaBuilder()
                .WithName("Defaults").WithVersion(suppliedValue!).Build(),
            DefaultScenario.SchemaKeyNullHandling => new ApiSchemaOptions
            {
                ApiKeyNullHandling = Enum.Parse<ApiKeyNullHandling>(suppliedValue!),
            },
            DefaultScenario.ObjectKeyNullHandling => new ApiObjectTypeOptions
            {
                ApiKeyNullHandling = suppliedValue is null ? null : Enum.Parse<ApiKeyNullHandling>(suppliedValue),
            },
            DefaultScenario.PropertyModifiers => new ApiProperty("Text", textType,
                Enum.Parse<ApiTypeModifiers>(suppliedValue!), new ClrMemberReference(ClrMemberKind.Property, "Text")),
            DefaultScenario.CollectionItemModifiers => new ApiCollectionType(textType,
                Enum.Parse<ApiTypeModifiers>(suppliedValue!), typeof(List<string>)),
            DefaultScenario.QueryOutputModifiers => new ApiQuerySource("Text", [], textType,
                Enum.Parse<ApiTypeModifiers>(suppliedValue!), ApiQueryComposition.Projection),
            DefaultScenario.QueryOutputComposition => new ApiQuerySource("Text", [], textType,
                ApiTypeModifiers.None, Enum.Parse<ApiQueryComposition>(suppliedValue!)),
            DefaultScenario.ParameterModifiers => new ApiQuerySourceParameter("text", textType,
                Enum.Parse<ApiTypeModifiers>(suppliedValue!), true),
            DefaultScenario.ParameterRequiresArgument => new ApiQuerySourceParameter("text", textType,
                ApiTypeModifiers.Required, bool.Parse(suppliedValue!)),
            DefaultScenario.OneToOneDeleteBehavior => new ApiRelationshipOneToOne("Profile", principal, dependent,
                Enum.Parse<ApiRelationshipDeleteBehavior>(suppliedValue!)),
            DefaultScenario.OneToManyDeleteBehavior => new ApiRelationshipOneToMany("Profiles", principal, dependent,
                Enum.Parse<ApiRelationshipDeleteBehavior>(suppliedValue!)),
            DefaultScenario.ManyToManyDeleteBehavior => new ApiRelationshipManyToMany("Profiles", principal,
                new ApiRelationshipPrincipalEnd(new ApiTypeReference(ApiTypeKind.Object, "OtherPrincipal")),
                new ApiRelationshipAssociation(new ApiTypeReference(ApiTypeKind.Object, "Association")),
                Enum.Parse<ApiRelationshipDeleteBehavior>(suppliedValue!)),
            _ => throw new ArgumentOutOfRangeException(nameof(scenario))
        };
    }

    private static (string PropertyName, string? Value) GetExpectation
    (
        DefaultScenario scenario,
        bool usesSemanticDefault
    )
        => scenario switch
        {
            DefaultScenario.SchemaVersion => (nameof(ApiSchema.ApiVersion), usesSemanticDefault ? "0.1.0" : "2.0.0"),
            DefaultScenario.SchemaKeyNullHandling => (nameof(ApiSchemaOptions.ApiKeyNullHandling),
                usesSemanticDefault ? "UseEmptyOnNull" : "ThrowOnNull"),
            DefaultScenario.ObjectKeyNullHandling => (nameof(ApiObjectTypeOptions.ApiKeyNullHandling),
                usesSemanticDefault ? null : "UseEmptyOnNull"),
            DefaultScenario.PropertyModifiers => (nameof(ApiProperty.ApiTypeModifiers),
                usesSemanticDefault ? "None" : "Required"),
            DefaultScenario.CollectionItemModifiers => (nameof(ApiCollectionType.ApiItemTypeModifiers),
                usesSemanticDefault ? "None" : "Required"),
            DefaultScenario.QueryOutputModifiers => (nameof(ApiQuerySource.ApiOutputTypeModifiers),
                usesSemanticDefault ? "None" : "Required"),
            DefaultScenario.QueryOutputComposition => (nameof(ApiQuerySource.ApiOutputComposition),
                usesSemanticDefault ? "Projection" : "None"),
            DefaultScenario.ParameterModifiers => (nameof(ApiQuerySourceParameter.ApiTypeModifiers),
                usesSemanticDefault ? "Required" : "None"),
            DefaultScenario.ParameterRequiresArgument => (nameof(ApiQuerySourceParameter.ApiRequiresArgument),
                usesSemanticDefault ? "True" : "False"),
            DefaultScenario.OneToOneDeleteBehavior or DefaultScenario.OneToManyDeleteBehavior =>
                (nameof(ApiRelationship.ApiDeleteBehavior), usesSemanticDefault ? "None" : "Delete"),
            DefaultScenario.ManyToManyDeleteBehavior => (nameof(ApiRelationship.ApiDeleteBehavior),
                usesSemanticDefault ? "Delete" : "None"),
            _ => throw new ArgumentOutOfRangeException(nameof(scenario))
        };
    #endregion
}
