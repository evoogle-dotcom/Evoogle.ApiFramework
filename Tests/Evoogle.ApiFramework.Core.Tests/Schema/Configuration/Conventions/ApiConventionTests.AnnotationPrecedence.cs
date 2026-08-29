// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Annotations;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

public partial class ApiConventionTests
{
    #region Test Classes
    private enum AnnotationPrecedenceTestCase
    {
        ConventionProperty,
        ConventionEnumValue,
        ExplicitProperty,
        ExplicitEnumValue
    }

    private sealed class AnnotationPrecedenceTest : XUnitTest
    {
        #region User Supplied Properties
        public required AnnotationPrecedenceTestCase TestCase { get; init; }
        public required string ApiNameExpected { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var builder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddScalar<int>();

            this.ApiSchemaBuilder = this.TestCase switch
            {
                AnnotationPrecedenceTestCase.ConventionProperty => builder
                    .AddObject<ConventionPropertyAnnotationTarget>()
                    .UseConventions(c => c.AddConvention(new AddAnnotatedPropertyConvention()))
                    .UseDefaultAnnotations(),
                AnnotationPrecedenceTestCase.ConventionEnumValue => builder
                    .AddEnum<ConventionEnumAnnotationTarget>()
                    .UseConventions(c => c.AddConvention(new AddAnnotatedEnumValueConvention()))
                    .UseDefaultAnnotations(),
                AnnotationPrecedenceTestCase.ExplicitProperty => builder
                    .AddObject<ConventionPropertyAnnotationTarget>
                    (
                        x => x.AddProperty
                        (
                            "ExplicitPropertyName",
                            nameof(ConventionPropertyAnnotationTarget.Annotated)
                        )
                    )
                    .UseDefaultAnnotations(),
                AnnotationPrecedenceTestCase.ExplicitEnumValue => builder
                    .AddEnum<ConventionEnumAnnotationTarget>
                    (
                        x => x.AddValue
                        (
                            "ExplicitEnumValueName",
                            nameof(ConventionEnumAnnotationTarget.Annotated),
                            (int)ConventionEnumAnnotationTarget.Annotated
                        )
                    )
                    .UseDefaultAnnotations(),
                _ => throw new InvalidOperationException($"Unknown test case: {this.TestCase}")
            };

            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine($"ApiNameExpected: {this.ApiNameExpected}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ApiSchemaActual = this.ApiSchemaBuilder!.Build();
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();

            if (this.TestCase is AnnotationPrecedenceTestCase.ConventionProperty or
                AnnotationPrecedenceTestCase.ExplicitProperty)
            {
                this.ApiSchemaActual!.ApiObjectTypes.Single().ApiProperties.Single().ApiName
                    .Should().Be(this.ApiNameExpected);
                return;
            }

            this.ApiSchemaActual!.ApiEnumTypes.Single().ApiEnumValues.Single().ApiName
                .Should().Be(this.ApiNameExpected);
        }
        #endregion

        #region Private Properties
        private ApiSchemaBuilder? ApiSchemaBuilder { get; set; }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] AnnotationPrecedenceTheoryData =>
    [
        new AnnotationPrecedenceTest
        {
            Name = "A convention-added property yields to its annotation name",
            TestCase = AnnotationPrecedenceTestCase.ConventionProperty,
            ApiNameExpected = "AnnotationPropertyName"
        },
        new AnnotationPrecedenceTest
        {
            Name = "A convention-added enum value yields to its annotation name",
            TestCase = AnnotationPrecedenceTestCase.ConventionEnumValue,
            ApiNameExpected = "AnnotationEnumValueName"
        },
        new AnnotationPrecedenceTest
        {
            Name = "An explicit property name remains above its annotation name",
            TestCase = AnnotationPrecedenceTestCase.ExplicitProperty,
            ApiNameExpected = "ExplicitPropertyName"
        },
        new AnnotationPrecedenceTest
        {
            Name = "An explicit enum value name remains above its annotation name",
            TestCase = AnnotationPrecedenceTestCase.ExplicitEnumValue,
            ApiNameExpected = "ExplicitEnumValueName"
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(AnnotationPrecedenceTheoryData))]
    public void AnnotationPrecedence(IXUnitTest test) => test.Execute(this);
    #endregion
}

#region Test Conventions
public partial class ApiConventionTests
{
    internal sealed class AddAnnotatedPropertyConvention : IApiObjectTypeConvention
    {
        public ApiConventionPhase Phase => ApiConventionPhase.Configuration;

        public void Apply(ApiObjectTypeBuilder builder)
        {
            if (builder.ClrType == typeof(ConventionPropertyAnnotationTarget))
            {
                builder.AddProperty
                (
                    "ConventionPropertyName",
                    nameof(ConventionPropertyAnnotationTarget.Annotated)
                );
            }
        }
    }

    internal sealed class AddAnnotatedEnumValueConvention : IApiEnumTypeConvention
    {
        public ApiConventionPhase Phase => ApiConventionPhase.Configuration;

        public void Apply(ApiEnumTypeBuilder builder)
        {
            if (builder.ClrType == typeof(ConventionEnumAnnotationTarget))
            {
                builder.AddValue
                (
                    "ConventionEnumValueName",
                    nameof(ConventionEnumAnnotationTarget.Annotated),
                    (int)ConventionEnumAnnotationTarget.Annotated
                );
            }
        }
    }
}
#endregion

#region Test Types
public sealed class ConventionPropertyAnnotationTarget
{
    [ApiProperty(ApiName = "AnnotationPropertyName")]
    public int Annotated { get; set; }
}

public enum ConventionEnumAnnotationTarget
{
    [ApiEnumValue(ApiName = "AnnotationEnumValueName")]
    Annotated
}
#endregion
