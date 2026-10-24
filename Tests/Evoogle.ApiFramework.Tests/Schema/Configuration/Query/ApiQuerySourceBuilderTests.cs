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

        public required bool ExpectedApiRequiresArgument { get; init; }

        public required ApiTypeModifiers ExpectedTypeModifiers { get; init; }
        #endregion

        #region Calculated Properties
        private ApiQuerySourceParameter ActualParameter { get; set; } = null!;
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario:        {this.Name}");
            this.WriteLine("API Parameter:   Echo.text (String)");
            this.WriteLine();

            this.WriteLine($"Build Expression: {this.BuildExpression}");
            this.WriteLine();

            this.WriteLine($"Expected API Requires Argument: {this.ExpectedApiRequiresArgument}");
            this.WriteLine($"Expected Type Modifiers:        {this.ExpectedTypeModifiers}");
            this.WriteLine("Expected Exception:             None");
        }

        protected override void Act()
        {
            var schema = this.BuildExpression.Compile()();
            this.ActualParameter = schema.ApiQuerySources.Single().ApiInputParameters.Single();

            this.WriteLine($"Actual API Requires Argument:   {this.ActualParameter.ApiRequiresArgument}");
            this.WriteLine($"Actual Type Modifiers:          {this.ActualParameter.ApiTypeModifiers}");
            this.WriteLine($"Actual CLR Type:                {this.ActualParameter.ApiType.ClrType}");
            this.WriteLine("Schema Compilation:             Succeeded");
        }

        protected override void Assert()
        {
            this.ActualParameter.ApiName.Should().Be("text");
            this.ActualParameter.ApiType.ClrType.Should().Be<string>();
            this.ActualParameter.ApiRequiresArgument.Should().Be(this.ExpectedApiRequiresArgument);
            this.ActualParameter.ApiTypeModifiers.Should().Be(this.ExpectedTypeModifiers);
        }
        #endregion
    }

    private sealed class BuildCollectionOutputTest : XUnitTest
    {
        #region User Supplied Properties
        public required bool UsesGenericOutput { get; init; }

        public required bool UsesDefaultComposition { get; init; }

        public required ApiTypeModifiers ExpectedCollectionTypeModifiers { get; init; }

        public required ApiTypeModifiers ExpectedItemTypeModifiers { get; init; }

        public required ApiQueryComposition ExpectedComposition { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaBuilder Builder { get; set; } = null!;

        private ApiQuerySource ActualSource { get; set; } = null!;
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario:                      {this.Name}");
            this.WriteLine("API Query Source:              Texts (String collection)");
            this.WriteLine($"Uses Generic Output:           {this.UsesGenericOutput}");
            this.WriteLine($"Expected Collection Modifiers: {this.ExpectedCollectionTypeModifiers}");
            this.WriteLine($"Expected Item Modifiers:       {this.ExpectedItemTypeModifiers}");
            this.WriteLine($"Uses Default Composition:      {this.UsesDefaultComposition}");
            this.WriteLine($"Expected Composition:          {this.ExpectedComposition}");
            this.WriteLine("Expected Exception:            None");

            this.Builder = new ApiSchemaBuilder()
                .WithName("CollectionOutputTests")
                .AddScalar<string>()
                .AddQuerySource("Texts", source =>
                {
                    if (this.UsesGenericOutput)
                    {
                        source.WithCollectionOutput<string>
                        (
                            apiCollectionTypeModifiers: this.ExpectedCollectionTypeModifiers,
                            apiItemTypeModifiers: this.ExpectedItemTypeModifiers
                        );
                    }
                    else
                    {
                        source.WithCollectionOutput<string>(apiCollectionTypeModifiers: this.ExpectedCollectionTypeModifiers, apiItemTypeModifiers: this.ExpectedItemTypeModifiers);
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

            this.WriteLine($"Actual Collection Modifiers:   {this.ActualSource.ApiOutputTypeModifiers}");
            this.WriteLine($"Actual Item Modifiers:         {collectionType.ApiItemTypeModifiers}");
            this.WriteLine($"Actual Composition:            {this.ActualSource.ApiOutputComposition}");
            this.WriteLine($"Actual Item CLR Type:          {collectionType.ApiItemType.ClrType}");
            this.WriteLine("Schema Compilation:            Succeeded");
        }

        protected override void Assert()
        {
            this.ActualSource.ApiName.Should().Be("Texts");
            this.ActualSource.ApiOutputTypeModifiers.Should().Be(this.ExpectedCollectionTypeModifiers);
            this.ActualSource.ApiOutputComposition.Should().Be(this.ExpectedComposition);
            var collectionType = this.ActualSource.ApiOutputType.Should().BeOfType<ApiCollectionType>().Subject;
            collectionType.ApiItemTypeModifiers.Should().Be(this.ExpectedItemTypeModifiers);
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
            Name = "Default argument settings require a non-null value",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildWithDefaultArgumentSettings(),
            ExpectedApiRequiresArgument = true,
            ExpectedTypeModifiers = ApiTypeModifiers.Required
        },
        new BuildInputParameterTest
        {
            Name = "Required argument with non-null value",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildRequiredArgumentWithNonNullableValue(),
            ExpectedApiRequiresArgument = true,
            ExpectedTypeModifiers = ApiTypeModifiers.Required
        },
        new BuildInputParameterTest
        {
            Name = "Required argument with nullable value",
            BuildExpression = static () => ApiQuerySourceBuilderTestsFactory.BuildRequiredArgumentWithNullableValue(),
            ExpectedApiRequiresArgument = true,
            ExpectedTypeModifiers = ApiTypeModifiers.None
        },
        new BuildInputParameterTest
        {
            Name = "Optional argument with non-null value",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildOptionalArgumentWithNonNullableValue(),
            ExpectedApiRequiresArgument = false,
            ExpectedTypeModifiers = ApiTypeModifiers.Required
        },
        new BuildInputParameterTest
        {
            Name = "Optional argument with nullable value",
            BuildExpression = static () => ApiQuerySourceBuilderTestsFactory.BuildOptionalArgumentWithNullableValue(),
            ExpectedApiRequiresArgument = false,
            ExpectedTypeModifiers = ApiTypeModifiers.None
        },
        new BuildInputParameterTest
        {
            Name = "Later required non-null configuration replaces optional nullable configuration",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildWithLaterRequiredNonNullableConfiguration(),
            ExpectedApiRequiresArgument = true,
            ExpectedTypeModifiers = ApiTypeModifiers.Required
        },
        new BuildInputParameterTest
        {
            Name = "Later nullable configuration replaces explicit non-null modifiers",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildWithLaterNullableConfiguration(),
            ExpectedApiRequiresArgument = true,
            ExpectedTypeModifiers = ApiTypeModifiers.None
        },
        new BuildInputParameterTest
        {
            Name = "Lower precedence cannot replace explicit optional nullable configuration",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildWithExplicitOptionalNullableConfiguration(),
            ExpectedApiRequiresArgument = false,
            ExpectedTypeModifiers = ApiTypeModifiers.None
        },
        new BuildInputParameterTest
        {
            Name = "Lower precedence cannot replace explicit required non-null configuration",
            BuildExpression = static () =>
                ApiQuerySourceBuilderTestsFactory.BuildWithExplicitRequiredNonNullableConfiguration(),
            ExpectedApiRequiresArgument = true,
            ExpectedTypeModifiers = ApiTypeModifiers.Required
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] BuildCollectionOutputTheoryData =>
    [
        new BuildCollectionOutputTest
        {
            Name = "Default composition allows projection",
            UsesGenericOutput = true,
            UsesDefaultComposition = true,
            ExpectedCollectionTypeModifiers = ApiTypeModifiers.Required,
            ExpectedItemTypeModifiers = ApiTypeModifiers.Required,
            ExpectedComposition = ApiQueryComposition.Projection
        },
        new BuildCollectionOutputTest
        {
            Name = "Generic nullable collection with non-null items and ordering only",
            UsesGenericOutput = true,
            UsesDefaultComposition = false,
            ExpectedCollectionTypeModifiers = ApiTypeModifiers.None,
            ExpectedItemTypeModifiers = ApiTypeModifiers.Required,
            ExpectedComposition = ApiQueryComposition.Ordering
        },
        new BuildCollectionOutputTest
        {
            Name = "Generic non-null collection with nullable items and no composition",
            UsesGenericOutput = true,
            UsesDefaultComposition = false,
            ExpectedCollectionTypeModifiers = ApiTypeModifiers.Required,
            ExpectedItemTypeModifiers = ApiTypeModifiers.None,
            ExpectedComposition = ApiQueryComposition.None
        },
        new BuildCollectionOutputTest
        {
            Name = "CLR nullable collection with non-null items and combined composition",
            UsesGenericOutput = false,
            UsesDefaultComposition = false,
            ExpectedCollectionTypeModifiers = ApiTypeModifiers.None,
            ExpectedItemTypeModifiers = ApiTypeModifiers.Required,
            ExpectedComposition = ApiQueryComposition.Projection | ApiQueryComposition.Ordering
                | ApiQueryComposition.OffsetPagination
        },
        new BuildCollectionOutputTest
        {
            Name = "CLR non-null collection with nullable items and no composition",
            UsesGenericOutput = false,
            UsesDefaultComposition = false,
            ExpectedCollectionTypeModifiers = ApiTypeModifiers.Required,
            ExpectedItemTypeModifiers = ApiTypeModifiers.None,
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
