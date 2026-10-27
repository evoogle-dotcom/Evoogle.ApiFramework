// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Types;

public class ApiPropertyBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class NullabilityModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Alias { get; set; }
        public List<string> Labels { get; set; } = [];
        public List<string?>? NullableLabels { get; set; }

#nullable disable
        public string ObliviousText { get; set; } = string.Empty;
        public List<string> ObliviousLabels { get; set; } = [];
#nullable restore
    }

    private sealed class BuildPropertyTest : XUnitTest
    {
        #region User Supplied Properties
        public required string ClrMemberName { get; init; }
        public bool? InitialIsNullable { get; init; }
        public bool? OverrideIsNullable { get; init; }
        public required bool ExpectedIsNullable { get; init; }
        public bool? ExpectedItemIsNullable { get; init; }
        public ApiNullability? ConventionNullability { get; init; }
        public ApiNullability? AnnotationNullability { get; init; }
        public ApiNullability? ExplicitNullability { get; init; }
        public ApiNullability? LateConventionNullability { get; init; }
        public ApiNullability? LateAnnotationNullability { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaBuilder Builder { get; set; } = null!;
        private ApiProperty ActualProperty { get; set; } = null!;
        private string RestoredTestJson { get; set; } = null!;
        private string OriginalTestJson { get; set; } = null!;
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario:   {this.Name}");
            this.WriteLine($"CLR Member: {nameof(NullabilityModel)}.{this.ClrMemberName}");
            this.WriteLine();
            this.WriteLine($"Initial Nullable Override: {this.InitialIsNullable}");
            this.WriteLine($"Final Nullable Override:   {this.OverrideIsNullable}");
            this.WriteLine($"Convention Nullability:    {this.ConventionNullability}");
            this.WriteLine($"Annotation Nullability:    {this.AnnotationNullability}");
            this.WriteLine($"Explicit Nullability:      {this.ExplicitNullability}");
            this.WriteLine($"Late Convention Override:  {this.LateConventionNullability}");
            this.WriteLine($"Late Annotation Override:  {this.LateAnnotationNullability}");
            this.WriteLine();
            this.WriteLine($"Expected Nullable:      {this.ExpectedIsNullable}");
            this.WriteLine($"Expected Item Nullable: {this.ExpectedItemIsNullable}");
            this.WriteLine("Expected Exception:     None");

            var serializer = new XUnitTestSerializer();
            this.OriginalTestJson = serializer.Serialize(this);
            this.RestoredTestJson = serializer.Serialize(serializer.Deserialize(this.GetType(), this.OriginalTestJson));

            this.Builder = new ApiSchemaBuilder()
                .WithName("PropertyNullabilityTests")
                .AddScalar<string>()
                .AddObject<NullabilityModel>(model => model
                    .AddProperty(this.ClrMemberName, property =>
                    {
                        if (this.ConventionNullability is ApiNullability conventionNullability)
                        {
                            property.SetNullabilityConvention(conventionNullability);
                        }

                        if (this.AnnotationNullability is ApiNullability annotationNullability)
                        {
                            property.SetNullabilityDataAnnotation(annotationNullability);
                        }

                        if (this.ExplicitNullability is ApiNullability explicitNullability)
                        {
                            property.WithNullability(explicitNullability);
                        }

                        if (this.InitialIsNullable is bool initialIsNullable)
                        {
                            if (initialIsNullable)
                            {
                                property.AsNullable();
                            }
                            else
                            {
                                property.AsNonNullable();
                            }
                        }

                        if (this.OverrideIsNullable is bool overrideIsNullable)
                        {
                            if (overrideIsNullable)
                            {
                                property.AsNullable();
                            }
                            else
                            {
                                property.AsNonNullable();
                            }
                        }

                        if (this.LateConventionNullability is ApiNullability lateConventionNullability)
                        {
                            property.SetNullabilityConvention(lateConventionNullability);
                        }

                        if (this.LateAnnotationNullability is ApiNullability lateAnnotationNullability)
                        {
                            property.SetNullabilityDataAnnotation(lateAnnotationNullability);
                        }
                    }));
        }

        protected override void Act()
        {
            var schema = this.Builder.Build();
            this.ActualProperty = schema.ApiObjectTypes.Single().ApiProperties.Single();

            this.WriteLine();
            this.WriteLine($"Actual Nullable:     {this.ActualProperty.IsNullable}");
            this.WriteLine($"Actual Non-Nullable: {this.ActualProperty.IsNonNullable}");
            this.WriteLine($"Actual Nullability:  {this.ActualProperty.ApiNullability}");
            if (this.ActualProperty.ApiType is ApiCollectionType collectionType)
            {
                this.WriteLine();
                this.WriteLine($"Actual Item Nullable:     {collectionType.IsItemNullable}");
                this.WriteLine($"Actual Item Non-Nullable: {collectionType.IsItemNonNullable}");
            }
        }

        protected override void Assert()
        {
            this.RestoredTestJson.Should().Be(this.OriginalTestJson);
            this.ActualProperty.ApiName.Should().Be(this.ClrMemberName);
            this.ActualProperty.IsNullable.Should().Be(this.ExpectedIsNullable);
            this.ActualProperty.IsNonNullable.Should().Be(!this.ExpectedIsNullable);
            ApiPropertyExtensions.IsNullable(this.ActualProperty).Should().Be(this.ExpectedIsNullable);
            ApiPropertyExtensions.IsNonNullable(this.ActualProperty).Should().Be(!this.ExpectedIsNullable);

            using var json = JsonDocument.Parse(JsonSerializer.Serialize(this.ActualProperty));
            json.RootElement.GetProperty(nameof(ApiProperty.ApiNullability)).GetString().Should().Be
            (
                this.ExpectedIsNullable ? nameof(ApiNullability.Nullable) : nameof(ApiNullability.NonNullable)
            );

            if (this.ExpectedItemIsNullable is bool expectedItemIsNullable)
            {
                var collectionType = this.ActualProperty.ApiType.Should().BeOfType<ApiCollectionType>().Subject;
                collectionType.IsItemNullable.Should().Be(expectedItemIsNullable);
                collectionType.IsItemNonNullable.Should().Be(!expectedItemIsNullable);
                ApiCollectionTypeExtensions.IsItemNullable(collectionType).Should().Be(expectedItemIsNullable);
                ApiCollectionTypeExtensions.IsItemNonNullable(collectionType).Should().Be(!expectedItemIsNullable);
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildPropertyTheoryData =>
    [
        new BuildPropertyTest
        {
            Name = "Unknown CLR member nullability falls back to nullable",
            ClrMemberName = nameof(NullabilityModel.ObliviousText),
            ExpectedIsNullable = true
        },
        new BuildPropertyTest
        {
            Name = "Unknown CLR collection and item nullability fall back to nullable",
            ClrMemberName = nameof(NullabilityModel.ObliviousLabels),
            ExpectedIsNullable = true,
            ExpectedItemIsNullable = true
        },
        new BuildPropertyTest
        {
            Name = "Explicit nullability overrides unknown CLR nullability",
            ClrMemberName = nameof(NullabilityModel.ObliviousText),
            ExplicitNullability = ApiNullability.NonNullable,
            ExpectedIsNullable = false
        },
        new BuildPropertyTest
        {
            Name = "Convention nullability overrides CLR inference",
            ClrMemberName = nameof(NullabilityModel.Name),
            ConventionNullability = ApiNullability.Nullable,
            ExpectedIsNullable = true
        },
        new BuildPropertyTest
        {
            Name = "Annotation nullability overrides convention and ignores later convention",
            ClrMemberName = nameof(NullabilityModel.Name),
            ConventionNullability = ApiNullability.NonNullable,
            AnnotationNullability = ApiNullability.Nullable,
            LateConventionNullability = ApiNullability.NonNullable,
            ExpectedIsNullable = true
        },
        new BuildPropertyTest
        {
            Name = "Explicit nullability ignores later convention and annotation",
            ClrMemberName = nameof(NullabilityModel.Name),
            ExplicitNullability = ApiNullability.NonNullable,
            LateConventionNullability = ApiNullability.Nullable,
            LateAnnotationNullability = ApiNullability.Nullable,
            ExpectedIsNullable = false
        },
        new BuildPropertyTest
        {
            Name = "Later annotation replaces earlier annotation nullability",
            ClrMemberName = nameof(NullabilityModel.Name),
            AnnotationNullability = ApiNullability.NonNullable,
            LateAnnotationNullability = ApiNullability.Nullable,
            ExpectedIsNullable = true
        },
        new BuildPropertyTest
        {
            Name = "Later convention replaces earlier convention nullability",
            ClrMemberName = nameof(NullabilityModel.Name),
            ConventionNullability = ApiNullability.NonNullable,
            LateConventionNullability = ApiNullability.Nullable,
            ExpectedIsNullable = true
        },
        new BuildPropertyTest
        {
            Name = "AddProperty infers a non-nullable CLR reference member",
            ClrMemberName = nameof(NullabilityModel.Name),
            ExpectedIsNullable = false
        },
        new BuildPropertyTest
        {
            Name = "AddProperty infers a nullable CLR reference member",
            ClrMemberName = nameof(NullabilityModel.Alias),
            ExpectedIsNullable = true
        },
        new BuildPropertyTest
        {
            Name = "AddProperty infers non-nullable collection and items",
            ClrMemberName = nameof(NullabilityModel.Labels),
            ExpectedIsNullable = false,
            ExpectedItemIsNullable = false
        },
        new BuildPropertyTest
        {
            Name = "AddProperty infers nullable collection and items",
            ClrMemberName = nameof(NullabilityModel.NullableLabels),
            ExpectedIsNullable = true,
            ExpectedItemIsNullable = true
        },
        new BuildPropertyTest
        {
            Name = "AsNullable overrides non-nullable CLR inference",
            ClrMemberName = nameof(NullabilityModel.Name),
            OverrideIsNullable = true,
            ExpectedIsNullable = true
        },
        new BuildPropertyTest
        {
            Name = "AsNonNullable overrides nullable CLR inference",
            ClrMemberName = nameof(NullabilityModel.Alias),
            OverrideIsNullable = false,
            ExpectedIsNullable = false
        },
        new BuildPropertyTest
        {
            Name = "Later AsNullable replaces AsNonNullable",
            ClrMemberName = nameof(NullabilityModel.Alias),
            InitialIsNullable = false,
            OverrideIsNullable = true,
            ExpectedIsNullable = true
        },
        new BuildPropertyTest
        {
            Name = "Later AsNonNullable replaces AsNullable",
            ClrMemberName = nameof(NullabilityModel.Name),
            InitialIsNullable = true,
            OverrideIsNullable = false,
            ExpectedIsNullable = false
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildPropertyTheoryData))]
    public void BuildInfersNullabilityAndAppliesExplicitOverrides(IXUnitTest test) => test.Execute(this);
    #endregion
}
