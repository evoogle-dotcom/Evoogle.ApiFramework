// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Query;
using Evoogle.XUnit;
using Evoogle.XUnit.Json;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Query;

public class ApiQuerySourceBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class BuildInputParameterTest : XUnitTest
    {
        #region User Supplied Properties
        [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiSchema>))]
        public required Expression<Func<ApiSchema>> BuildExpression { get; init; }

        public required ApiPresence ExpectedPresence { get; init; }

        public required ApiNullability ExpectedNullability { get; init; }
        #endregion

        #region Calculated Properties
        private ApiQuerySourceParameter ActualParameter { get; set; } = null!;
        private BuildInputParameterTest RestoredTestCase { get; set; } = null!;
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario:      {this.Name}");
            this.WriteLine("API Parameter: Echo.text (String)");
            this.WriteLine();

            this.WriteLine($"Build Expression: {this.BuildExpression}");
            this.WriteLine();

            this.WriteLine($"Expected Presence:    {this.ExpectedPresence}");
            this.WriteLine($"Expected Nullability: {this.ExpectedNullability}");
            this.WriteLine("Expected Exception:   None");

            var serializer = new XUnitTestSerializer();
            this.RestoredTestCase = (BuildInputParameterTest)serializer.Deserialize(
                this.GetType(), serializer.Serialize(this));
        }

        protected override void Act()
        {
            var schema = this.RestoredTestCase.BuildExpression.Compile()();
            this.ActualParameter = schema.ApiQuerySources.Single().ApiInputParameters.Single();

            this.WriteLine($"Actual Presence:              {this.ActualParameter.ApiPresence}");
            this.WriteLine($"Actual Nullability:           {this.ActualParameter.ApiNullability}");
            this.WriteLine($"Actual CLR Type:              {this.ActualParameter.ApiType.ClrType}");
            this.WriteLine("Schema Compilation:           Succeeded");
        }

        protected override void Assert()
        {
            this.ActualParameter.ApiName.Should().Be("text");
            this.ActualParameter.ApiType.ClrType.Should().Be<string>();
            this.ActualParameter.ApiPresence.Should().Be(this.ExpectedPresence);
            this.ActualParameter.IsRequired.Should().Be(this.ExpectedPresence == ApiPresence.Required);
            this.ActualParameter.IsOptional.Should().Be(this.ExpectedPresence == ApiPresence.Optional);
            this.ActualParameter.ApiNullability.Should().Be(this.ExpectedNullability);
        }
        #endregion
    }

    private sealed class BuildCollectionOutputTest : XUnitTest
    {
        #region User Supplied Properties
        public required bool UsesGenericOutput { get; init; }

        public required bool UsesDefaultComposition { get; init; }

        public required ApiNullability ExpectedCollectionNullability { get; init; }

        public required ApiNullability ExpectedItemNullability { get; init; }

        public required ApiQueryComposition ExpectedComposition { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaBuilder Builder { get; set; } = null!;

        private ApiQuerySource ActualSource { get; set; } = null!;
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario:                        {this.Name}");
            this.WriteLine("API Query Source:                Texts (String collection)");
            this.WriteLine($"Uses Generic Output:             {this.UsesGenericOutput}");
            this.WriteLine($"Expected Collection Nullability: {this.ExpectedCollectionNullability}");
            this.WriteLine($"Expected Item Nullability:       {this.ExpectedItemNullability}");
            this.WriteLine($"Uses Default Composition:        {this.UsesDefaultComposition}");
            this.WriteLine($"Expected Composition:            {this.ExpectedComposition}");
            this.WriteLine("Expected Exception:              None");

            this.Builder = new ApiSchemaBuilder()
                .WithName("CollectionOutputTests")
                .AddScalar<string>()
                .AddQuerySource("Texts", source =>
                {
                    if (this.UsesGenericOutput)
                    {
                        source.WithCollectionOutput<string>
                        (
                            apiCollectionNullability: this.ExpectedCollectionNullability,
                            apiItemNullability: this.ExpectedItemNullability
                        );
                    }
                    else
                    {
                        source.WithCollectionOutput
                        (
                            typeof(string),
                            apiCollectionNullability: this.ExpectedCollectionNullability,
                            apiItemNullability: this.ExpectedItemNullability
                        );
                    }

                    if (!this.UsesDefaultComposition)
                    {
                        source
                            .WithOutputComposition(ApiQueryComposition.Projection | ApiQueryComposition.Filtering)
                            .WithOutputComposition(this.ExpectedComposition);
                    }
                });
        }

        protected override void Act()
        {
            var schema = this.Builder.Build();
            this.ActualSource = schema.ApiQuerySources.Single();
            var collectionType = (ApiCollectionType)this.ActualSource.ApiOutputType;

            this.WriteLine($"Actual Collection Nullability: {this.ActualSource.ApiOutputNullability}");
            this.WriteLine($"Actual Item Nullability:       {collectionType.ApiItemNullability}");
            this.WriteLine($"Actual Composition:            {this.ActualSource.ApiOutputComposition}");
            this.WriteLine($"Actual Item CLR Type:          {collectionType.ApiItemType.ClrType}");
            this.WriteLine("Schema Compilation:            Succeeded");
        }

        protected override void Assert()
        {
            this.ActualSource.ApiName.Should().Be("Texts");
            this.ActualSource.ApiOutputNullability.Should().Be(this.ExpectedCollectionNullability);
            this.ActualSource.ApiOutputComposition.Should().Be(this.ExpectedComposition);
            var collectionType = this.ActualSource.ApiOutputType.Should().BeOfType<ApiCollectionType>().Subject;
            collectionType.ApiItemNullability.Should().Be(this.ExpectedItemNullability);
            collectionType.ApiItemType.ClrType.Should().Be<string>();
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildInputParameterTheoryData =>
    [
        new BuildInputParameterTest
        {
            Name = "Later equal-precedence presence replaces earlier presence without changing nullability",
            BuildExpression = static () => ApiQuerySourceBuilderTestsFactory.BuildWithLaterOptionalPresence(),
            ExpectedPresence = ApiPresence.Optional,
            ExpectedNullability = ApiNullability.NonNullable
        },
        new BuildInputParameterTest
        {
            Name = "Annotation presence takes precedence over a later convention",
            BuildExpression = static () => ApiQuerySourceBuilderTestsFactory.BuildWithAnnotationPresence(),
            ExpectedPresence = ApiPresence.Optional,
            ExpectedNullability = ApiNullability.NonNullable
        },
        new BuildInputParameterTest
        {
            Name = "Explicit presence takes precedence over a later annotation",
            BuildExpression = static () => ApiQuerySourceBuilderTestsFactory.BuildWithExplicitPresenceOverAnnotation(),
            ExpectedPresence = ApiPresence.Optional,
            ExpectedNullability = ApiNullability.NonNullable
        },
        new BuildInputParameterTest
        {
            Name = "Default argument settings require a non-null value",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildWithDefaultArgumentSettings(),
            ExpectedPresence = ApiPresence.Required,
            ExpectedNullability = ApiNullability.NonNullable
        },
        new BuildInputParameterTest
        {
            Name = "Required argument with non-null value",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildRequiredArgumentWithNonNullableValue(),
            ExpectedPresence = ApiPresence.Required,
            ExpectedNullability = ApiNullability.NonNullable
        },
        new BuildInputParameterTest
        {
            Name = "Required argument with nullable value",
            BuildExpression = static () => ApiQuerySourceBuilderTestsFactory.BuildRequiredArgumentWithNullableValue(),
            ExpectedPresence = ApiPresence.Required,
            ExpectedNullability = ApiNullability.Nullable
        },
        new BuildInputParameterTest
        {
            Name = "Optional argument with non-null value",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildOptionalArgumentWithNonNullableValue(),
            ExpectedPresence = ApiPresence.Optional,
            ExpectedNullability = ApiNullability.NonNullable
        },
        new BuildInputParameterTest
        {
            Name = "Optional argument with nullable value",
            BuildExpression = static () => ApiQuerySourceBuilderTestsFactory.BuildOptionalArgumentWithNullableValue(),
            ExpectedPresence = ApiPresence.Optional,
            ExpectedNullability = ApiNullability.Nullable
        },
        new BuildInputParameterTest
        {
            Name = "Later required non-null configuration replaces optional nullable configuration",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildWithLaterRequiredNonNullableConfiguration(),
            ExpectedPresence = ApiPresence.Required,
            ExpectedNullability = ApiNullability.NonNullable
        },
        new BuildInputParameterTest
        {
            Name = "Later nullable configuration replaces explicit non-nullability",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildWithLaterNullableConfiguration(),
            ExpectedPresence = ApiPresence.Required,
            ExpectedNullability = ApiNullability.Nullable
        },
        new BuildInputParameterTest
        {
            Name = "Lower precedence cannot replace explicit optional nullable configuration",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildWithExplicitOptionalNullableConfiguration(),
            ExpectedPresence = ApiPresence.Optional,
            ExpectedNullability = ApiNullability.Nullable
        },
        new BuildInputParameterTest
        {
            Name = "Lower precedence cannot replace explicit required non-null configuration",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildWithExplicitRequiredNonNullableConfiguration(),
            ExpectedPresence = ApiPresence.Required,
            ExpectedNullability = ApiNullability.NonNullable
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] BuildCollectionOutputTheoryData =>
    [
        new BuildCollectionOutputTest
        {
            Name = "Default composition allows projection",
            UsesGenericOutput = true,
            UsesDefaultComposition = true,
            ExpectedCollectionNullability = ApiNullability.NonNullable,
            ExpectedItemNullability = ApiNullability.NonNullable,
            ExpectedComposition = ApiQueryComposition.Projection
        },
        new BuildCollectionOutputTest
        {
            Name = "Generic nullable collection with non-null items and ordering only",
            UsesGenericOutput = true,
            UsesDefaultComposition = false,
            ExpectedCollectionNullability = ApiNullability.Nullable,
            ExpectedItemNullability = ApiNullability.NonNullable,
            ExpectedComposition = ApiQueryComposition.Ordering
        },
        new BuildCollectionOutputTest
        {
            Name = "Generic non-null collection with nullable items and no composition",
            UsesGenericOutput = true,
            UsesDefaultComposition = false,
            ExpectedCollectionNullability = ApiNullability.NonNullable,
            ExpectedItemNullability = ApiNullability.Nullable,
            ExpectedComposition = ApiQueryComposition.None
        },
        new BuildCollectionOutputTest
        {
            Name = "CLR nullable collection with non-null items and combined composition",
            UsesGenericOutput = false,
            UsesDefaultComposition = false,
            ExpectedCollectionNullability = ApiNullability.Nullable,
            ExpectedItemNullability = ApiNullability.NonNullable,
            ExpectedComposition = ApiQueryComposition.Projection | ApiQueryComposition.Ordering
                | ApiQueryComposition.OffsetPagination
        },
        new BuildCollectionOutputTest
        {
            Name = "CLR non-null collection with nullable items and no composition",
            UsesGenericOutput = false,
            UsesDefaultComposition = false,
            ExpectedCollectionNullability = ApiNullability.NonNullable,
            ExpectedItemNullability = ApiNullability.Nullable,
            ExpectedComposition = ApiQueryComposition.None
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildInputParameterTheoryData))]
    public void BuildKeepsArgumentPresenceIndependentOfValueNullability(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(BuildCollectionOutputTheoryData))]
    public void BuildPreservesCollectionNullabilityAndReplacesOutputComposition(IXUnitTest test)
        => test.Execute(this);
    #endregion
}
