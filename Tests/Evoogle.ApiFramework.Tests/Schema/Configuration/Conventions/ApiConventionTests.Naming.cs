// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

public partial class ApiConventionTests
{
    #region Test Types
    private enum BuiltInNamingStyle
    {
        CamelCase,
        KebabCase,
        LowerCase,
        PascalCase,
        UpperCase,
        CamelThenKebabCase,
    }

    private sealed record NamingExpectation
    (
        string ScalarApiName,
        string EnumApiName,
        string InferredEnumValueApiName,
        string ObjectApiName,
        string InferredPropertyApiName
    );

    private sealed class BuiltInNamingTest : XUnitTest
    {
        #region User Supplied Properties
        public required BuiltInNamingStyle Style { get; init; }
        public ApiNamingConventionTargets Targets { get; init; } = ApiNamingConventionTargets.All;
        #endregion

        #region Calculated Properties
        private ApiSchemaBuilder? ApiSchemaBuilder { get; set; }
        private ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region Constructors
        public BuiltInNamingTest() => this.Name = nameof(BuiltInNamingTest);
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ApiSchemaBuilder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddScalar<Guid>()
                .AddScalar<string>()
                .AddScalar<CustomScalar>()
                .AddEnum<PipelineStatus>(x => x
                    .AddValue(PipelineStatus.Active, "ExplicitEnumValue")
                    .AddValue(PipelineStatus.InProgress))
                .AddObject<PersonWithId>(x => x
                    .AddProperty(p => p.Id)
                    .AddProperty(p => p.Name)
                    .AddProperty(p => p.Email, "ExplicitProperty"))
                .AddObject<OrderWithPersonId>(x => x
                    .WithName("ExplicitObject")
                    .AddProperty(p => p.OrderId));

            this.WriteLine($"Style: {this.Style}; Targets: {this.Targets}");
        }

        protected override void Act()
        {
            this.ApiSchemaActual = this.Style switch
            {
                BuiltInNamingStyle.CamelCase => this.ApiSchemaBuilder!
                    .UseCamelCaseNaming(this.Targets)
                    .Build(),
                BuiltInNamingStyle.KebabCase => this.ApiSchemaBuilder!
                    .UseKebabCaseNaming(this.Targets)
                    .Build(),
                BuiltInNamingStyle.LowerCase => this.ApiSchemaBuilder!
                    .UseLowerCaseNaming(this.Targets)
                    .Build(),
                BuiltInNamingStyle.PascalCase => this.ApiSchemaBuilder!
                    .UsePascalCaseNaming(this.Targets)
                    .Build(),
                BuiltInNamingStyle.UpperCase => this.ApiSchemaBuilder!
                    .UseUpperCaseNaming(this.Targets)
                    .Build(),
                BuiltInNamingStyle.CamelThenKebabCase => this.ApiSchemaBuilder!
                    .UseCamelCaseNaming(this.Targets)
                    .UseKebabCaseNaming(this.Targets)
                    .Build(),
                _ => throw new InvalidOperationException($"Unknown naming style: {this.Style}"),
            };
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();

            var apiSchema = this.ApiSchemaActual!;
            var expected = this.Style switch
            {
                BuiltInNamingStyle.CamelCase => new NamingExpectation
                (
                    "customScalar",
                    "pipelineStatus",
                    "inProgress",
                    "personWithId",
                    "id"
                ),
                BuiltInNamingStyle.KebabCase or BuiltInNamingStyle.CamelThenKebabCase =>
                    new NamingExpectation
                    (
                        "custom-scalar",
                        "pipeline-status",
                        "in-progress",
                        "person-with-id",
                        "id"
                    ),
                BuiltInNamingStyle.LowerCase => new NamingExpectation
                (
                    "customscalar",
                    "pipelinestatus",
                    "inprogress",
                    "personwithid",
                    "id"
                ),
                BuiltInNamingStyle.PascalCase => new NamingExpectation
                (
                    "CustomScalar",
                    "PipelineStatus",
                    "InProgress",
                    "PersonWithId",
                    "Id"
                ),
                BuiltInNamingStyle.UpperCase => new NamingExpectation
                (
                    "CUSTOMSCALAR",
                    "PIPELINESTATUS",
                    "INPROGRESS",
                    "PERSONWITHID",
                    "ID"
                ),
                _ => throw new InvalidOperationException($"Unknown naming style: {this.Style}"),
            };

            var scalarType = apiSchema.ApiScalarTypes
                .Single(x => x.ClrType == typeof(CustomScalar));
            scalarType.ApiName.Should().Be
            (
                this.GetExpectedApiName
                (
                    expected.ScalarApiName,
                    nameof(CustomScalar),
                    ApiNamingConventionTargets.ScalarType
                )
            );

            var enumType = apiSchema.ApiEnumTypes.Single(x => x.ClrType == typeof(PipelineStatus));
            enumType.ApiName.Should().Be
            (
                this.GetExpectedApiName
                (
                    expected.EnumApiName,
                    nameof(PipelineStatus),
                    ApiNamingConventionTargets.EnumType
                )
            );

            var inferredEnumValue = enumType.ApiEnumValues
                .Single(x => x.ClrName == nameof(PipelineStatus.InProgress));
            inferredEnumValue.ApiName.Should().Be
            (
                this.GetExpectedApiName
                (
                    expected.InferredEnumValueApiName,
                    nameof(PipelineStatus.InProgress),
                    ApiNamingConventionTargets.EnumValue
                )
            );

            var explicitEnumValue = enumType.ApiEnumValues
                .Single(x => x.ClrName == nameof(PipelineStatus.Active));
            explicitEnumValue.ApiName.Should().Be("ExplicitEnumValue");

            var objectType = apiSchema.ApiObjectTypes
                .Single(x => x.ClrType == typeof(PersonWithId));
            objectType.ApiName.Should().Be
            (
                this.GetExpectedApiName
                (
                    expected.ObjectApiName,
                    nameof(PersonWithId),
                    ApiNamingConventionTargets.ObjectType
                )
            );

            var inferredProperty = objectType.ApiProperties
                .Single(x => x.ClrName == nameof(PersonWithId.Id));
            inferredProperty.ApiName.Should().Be
            (
                this.GetExpectedApiName
                (
                    expected.InferredPropertyApiName,
                    nameof(PersonWithId.Id),
                    ApiNamingConventionTargets.Property
                )
            );

            var inferredNameProperty = objectType.ApiProperties
                .Single(x => x.ClrName == nameof(PersonWithId.Name));
            inferredNameProperty.ApiName.Should().Be
            (
                this.GetExpectedApiName
                (
                    this.Style switch
                    {
                        BuiltInNamingStyle.CamelCase or
                        BuiltInNamingStyle.KebabCase or
                        BuiltInNamingStyle.LowerCase or
                        BuiltInNamingStyle.CamelThenKebabCase => "name",
                        BuiltInNamingStyle.PascalCase => "Name",
                        BuiltInNamingStyle.UpperCase => "NAME",
                        _ => throw new InvalidOperationException
                        (
                            $"Unknown naming style: {this.Style}"
                        ),
                    },
                    nameof(PersonWithId.Name),
                    ApiNamingConventionTargets.Property
                )
            );

            var explicitProperty = objectType.ApiProperties
                .Single(x => x.ClrName == nameof(PersonWithId.Email));
            explicitProperty.ApiName.Should().Be("ExplicitProperty");

            var explicitObject = apiSchema.ApiObjectTypes
                .Single(x => x.ClrType == typeof(OrderWithPersonId));
            explicitObject.ApiName.Should().Be("ExplicitObject");
        }

        private string GetExpectedApiName
        (
            string transformedApiName,
            string originalApiName,
            ApiNamingConventionTargets target
        )
        {
            return (this.Targets & target) != ApiNamingConventionTargets.None
                ? transformedApiName
                : originalApiName;
        }
        #endregion
    }

    private sealed class NamingConventionValidationTest : XUnitTest
    {
        #region User Supplied Properties
        public required BuiltInNamingStyle Style { get; init; }
        public required string? ApiName { get; init; }
        public required Type ExpectedExceptionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiNamingConvention? NamingConvention { get; set; }
        private Exception? ExceptionActual { get; set; }
        #endregion

        #region Constructors
        public NamingConventionValidationTest()
            => this.Name = nameof(NamingConventionValidationTest);
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.NamingConvention = this.Style switch
            {
                BuiltInNamingStyle.CamelCase => new ApiNamingCamelCaseConvention(),
                BuiltInNamingStyle.KebabCase => new ApiNamingKebabCaseConvention(),
                BuiltInNamingStyle.LowerCase => new ApiNamingLowerCaseConvention(),
                BuiltInNamingStyle.PascalCase => new ApiNamingPascalCaseConvention(),
                BuiltInNamingStyle.UpperCase => new ApiNamingUpperCaseConvention(),
                _ => throw new InvalidOperationException($"Unknown naming style: {this.Style}"),
            };
        }

        protected override void Act()
        {
            try
            {
                this.NamingConvention!.ConvertName
                (
                    this.ApiName!,
                    new ApiNamingConventionContext
                    (
                        ApiNamingConventionTarget.ObjectType,
                        typeof(object)
                    )
                );
            }
            catch (Exception exception)
            {
                this.ExceptionActual = exception;
            }
        }

        protected override void Assert()
        {
            this.ExceptionActual.Should().NotBeNull();
            this.ExceptionActual.Should().BeOfType(this.ExpectedExceptionType);
        }
        #endregion
    }

    private sealed class NamingConventionTargetValidationTest : XUnitTest
    {
        #region User Supplied Properties
        public required BuiltInNamingStyle Style { get; init; }
        #endregion

        #region Calculated Properties
        private ApiNamingConvention? NamingConventionActual { get; set; }
        private Exception? ExceptionActual { get; set; }
        #endregion

        #region Constructors
        public NamingConventionTargetValidationTest()
            => this.Name = nameof(NamingConventionTargetValidationTest);
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
        }

        protected override void Act()
        {
            try
            {
                this.NamingConventionActual = this.Style switch
                {
                    BuiltInNamingStyle.CamelCase => new ApiNamingCamelCaseConvention
                    (
                        (ApiNamingConventionTargets)int.MaxValue
                    ),
                    BuiltInNamingStyle.KebabCase => new ApiNamingKebabCaseConvention
                    (
                        (ApiNamingConventionTargets)int.MaxValue
                    ),
                    BuiltInNamingStyle.LowerCase => new ApiNamingLowerCaseConvention
                    (
                        (ApiNamingConventionTargets)int.MaxValue
                    ),
                    BuiltInNamingStyle.PascalCase => new ApiNamingPascalCaseConvention
                    (
                        (ApiNamingConventionTargets)int.MaxValue
                    ),
                    BuiltInNamingStyle.UpperCase => new ApiNamingUpperCaseConvention
                    (
                        (ApiNamingConventionTargets)int.MaxValue
                    ),
                    _ => throw new InvalidOperationException($"Unknown naming style: {this.Style}"),
                };
            }
            catch (Exception exception)
            {
                this.ExceptionActual = exception;
            }
        }

        protected override void Assert()
        {
            this.ExceptionActual.Should().BeOfType<ArgumentOutOfRangeException>();
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuiltInNamingTheoryData =>
    [
        new BuiltInNamingTest
        {
            Name = "Camel-case naming applies to all supported naming targets",
            Style = BuiltInNamingStyle.CamelCase,
        },
        new BuiltInNamingTest
        {
            Name = "Kebab-case naming applies to all supported naming targets",
            Style = BuiltInNamingStyle.KebabCase,
        },
        new BuiltInNamingTest
        {
            Name = "Lower-case naming applies to all supported naming targets",
            Style = BuiltInNamingStyle.LowerCase,
        },
        new BuiltInNamingTest
        {
            Name = "Pascal-case naming applies to all supported naming targets",
            Style = BuiltInNamingStyle.PascalCase,
        },
        new BuiltInNamingTest
        {
            Name = "Upper-case naming applies to all supported naming targets",
            Style = BuiltInNamingStyle.UpperCase,
        },
        new BuiltInNamingTest
        {
            Name = "Naming conventions compose in registration order",
            Style = BuiltInNamingStyle.CamelThenKebabCase,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] NamingConventionValidationTheoryData =>
    [
        new NamingConventionValidationTest
        {
            Name = "Camel-case naming rejects null API names",
            Style = BuiltInNamingStyle.CamelCase,
            ApiName = null,
            ExpectedExceptionType = typeof(ArgumentNullException),
        },
        new NamingConventionValidationTest
        {
            Name = "Camel-case naming rejects whitespace API names",
            Style = BuiltInNamingStyle.CamelCase,
            ApiName = " ",
            ExpectedExceptionType = typeof(ArgumentException),
        },
        new NamingConventionValidationTest
        {
            Name = "Kebab-case naming rejects null API names",
            Style = BuiltInNamingStyle.KebabCase,
            ApiName = null,
            ExpectedExceptionType = typeof(ArgumentNullException),
        },
        new NamingConventionValidationTest
        {
            Name = "Kebab-case naming rejects whitespace API names",
            Style = BuiltInNamingStyle.KebabCase,
            ApiName = " ",
            ExpectedExceptionType = typeof(ArgumentException),
        },
        new NamingConventionValidationTest
        {
            Name = "Lower-case naming rejects null API names",
            Style = BuiltInNamingStyle.LowerCase,
            ApiName = null,
            ExpectedExceptionType = typeof(ArgumentNullException),
        },
        new NamingConventionValidationTest
        {
            Name = "Lower-case naming rejects whitespace API names",
            Style = BuiltInNamingStyle.LowerCase,
            ApiName = " ",
            ExpectedExceptionType = typeof(ArgumentException),
        },
        new NamingConventionValidationTest
        {
            Name = "Pascal-case naming rejects null API names",
            Style = BuiltInNamingStyle.PascalCase,
            ApiName = null,
            ExpectedExceptionType = typeof(ArgumentNullException),
        },
        new NamingConventionValidationTest
        {
            Name = "Pascal-case naming rejects whitespace API names",
            Style = BuiltInNamingStyle.PascalCase,
            ApiName = " ",
            ExpectedExceptionType = typeof(ArgumentException),
        },
        new NamingConventionValidationTest
        {
            Name = "Upper-case naming rejects null API names",
            Style = BuiltInNamingStyle.UpperCase,
            ApiName = null,
            ExpectedExceptionType = typeof(ArgumentNullException),
        },
        new NamingConventionValidationTest
        {
            Name = "Upper-case naming rejects whitespace API names",
            Style = BuiltInNamingStyle.UpperCase,
            ApiName = " ",
            ExpectedExceptionType = typeof(ArgumentException),
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] BuiltInNamingTargetTheoryData =>
    [
        new BuiltInNamingTest
        {
            Name = "Camel-case naming selects object types",
            Style = BuiltInNamingStyle.CamelCase,
            Targets = ApiNamingConventionTargets.ObjectType,
        },
        new BuiltInNamingTest
        {
            Name = "Kebab-case naming selects scalar types",
            Style = BuiltInNamingStyle.KebabCase,
            Targets = ApiNamingConventionTargets.ScalarType,
        },
        new BuiltInNamingTest
        {
            Name = "Lower-case naming selects enum types",
            Style = BuiltInNamingStyle.LowerCase,
            Targets = ApiNamingConventionTargets.EnumType,
        },
        new BuiltInNamingTest
        {
            Name = "Pascal-case naming selects properties",
            Style = BuiltInNamingStyle.PascalCase,
            Targets = ApiNamingConventionTargets.Property,
        },
        new BuiltInNamingTest
        {
            Name = "Upper-case naming selects enum values",
            Style = BuiltInNamingStyle.UpperCase,
            Targets = ApiNamingConventionTargets.EnumValue,
        },
        new BuiltInNamingTest
        {
            Name = "None disables naming convention target selection",
            Style = BuiltInNamingStyle.PascalCase,
            Targets = ApiNamingConventionTargets.None,
        },
        new BuiltInNamingTest
        {
            Name = "Naming convention target selection composes",
            Style = BuiltInNamingStyle.CamelThenKebabCase,
            Targets = ApiNamingConventionTargets.ObjectType |
                ApiNamingConventionTargets.Property,
        },
        new BuiltInNamingTest
        {
            Name = "All naming convention targets can be selected explicitly",
            Style = BuiltInNamingStyle.CamelCase,
            Targets = ApiNamingConventionTargets.All,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] NamingConventionTargetValidationTheoryData =>
    [
        new NamingConventionTargetValidationTest
        {
            Name = "Camel-case naming rejects unknown targets",
            Style = BuiltInNamingStyle.CamelCase,
        },
        new NamingConventionTargetValidationTest
        {
            Name = "Kebab-case naming rejects unknown targets",
            Style = BuiltInNamingStyle.KebabCase,
        },
        new NamingConventionTargetValidationTest
        {
            Name = "Lower-case naming rejects unknown targets",
            Style = BuiltInNamingStyle.LowerCase,
        },
        new NamingConventionTargetValidationTest
        {
            Name = "Pascal-case naming rejects unknown targets",
            Style = BuiltInNamingStyle.PascalCase,
        },
        new NamingConventionTargetValidationTest
        {
            Name = "Upper-case naming rejects unknown targets",
            Style = BuiltInNamingStyle.UpperCase,
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuiltInNamingTheoryData))]
    public void BuiltInNaming(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(NamingConventionValidationTheoryData))]
    public void NamingConventionValidation(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(BuiltInNamingTargetTheoryData))]
    public void BuiltInNamingTargetSelection(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(NamingConventionTargetValidationTheoryData))]
    public void NamingConventionTargetValidation(IXUnitTest test) => test.Execute(this);
    #endregion
}
