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
    private enum NamingInflectionStyle
    {
        Pluralize,
        Singularize,
    }

    private enum NamingInflectionRegistration
    {
        Fluent,
        DirectConvention,
    }
    #endregion

    #region Test Classes
    private sealed class NamingInflectionConversionTest : XUnitTest
    {
        #region User Supplied Properties
        public required NamingInflectionStyle Style { get; init; }
        public required string ApiName { get; init; }
        public required string ExpectedApiName { get; init; }
        #endregion

        #region Calculated Properties
        private ApiNamingConvention? NamingConvention { get; set; }
        private string? ApiNameActual { get; set; }
        #endregion

        #region Constructors
        public NamingInflectionConversionTest()
            => this.Name = nameof(NamingInflectionConversionTest);
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.NamingConvention = this.Style switch
            {
                NamingInflectionStyle.Pluralize => new ApiNamingPluralizeConvention(),
                NamingInflectionStyle.Singularize => new ApiNamingSingularizeConvention(),
                _ => throw new InvalidOperationException($"Unknown naming style: {this.Style}"),
            };
        }

        protected override void Act()
        {
            this.ApiNameActual = this.NamingConvention!.ConvertName
            (
                this.ApiName,
                new ApiNamingConventionContext
                (
                    ApiNamingConventionTarget.ObjectType,
                    typeof(object)
                )
            );
        }

        protected override void Assert()
        {
            this.ApiNameActual.Should().Be(this.ExpectedApiName);
        }
        #endregion
    }

    private sealed class NamingInflectionDefaultTargetTest : XUnitTest
    {
        #region User Supplied Properties
        public required NamingInflectionStyle Style { get; init; }
        public required NamingInflectionRegistration Registration { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaBuilder? ApiSchemaBuilder { get; set; }
        private ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region Constructors
        public NamingInflectionDefaultTargetTest()
            => this.Name = nameof(NamingInflectionDefaultTargetTest);
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ApiSchemaBuilder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddScalar<Guid>()
                .AddScalar<string>()
                .AddScalar<CustomScalar>()
                .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.InProgress))
                .AddObject<People>(x => x.AddProperty(p => p.Id))
                .AddObject<PersonWithId>(x => x
                    .AddProperty(p => p.Id)
                    .AddProperty(p => p.Name)
                    .AddProperty(p => p.Email, "ExplicitProperty"))
                .AddObject<OrderWithPersonId>(x => x
                    .WithName("ExplicitObject")
                    .AddProperty(p => p.OrderId));
        }

        protected override void Act()
        {
            this.ApiSchemaActual = (this.Registration, this.Style) switch
            {
                (NamingInflectionRegistration.Fluent, NamingInflectionStyle.Pluralize) =>
                    this.ApiSchemaBuilder!.UsePluralizeNaming().Build(),
                (NamingInflectionRegistration.Fluent, NamingInflectionStyle.Singularize) =>
                    this.ApiSchemaBuilder!.UseSingularizeNaming().Build(),
                (NamingInflectionRegistration.DirectConvention, NamingInflectionStyle.Pluralize) =>
                    this.ApiSchemaBuilder!
                        .UseConventions(c => c.AddConvention(new ApiNamingPluralizeConvention()))
                        .Build(),
                (NamingInflectionRegistration.DirectConvention, NamingInflectionStyle.Singularize) =>
                    this.ApiSchemaBuilder!
                        .UseConventions(c => c.AddConvention(new ApiNamingSingularizeConvention()))
                        .Build(),
                _ => throw new InvalidOperationException
                (
                    $"Unknown naming registration: {this.Registration}; style: {this.Style}"
                ),
            };
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();

            var apiSchema = this.ApiSchemaActual!;
            var people = apiSchema.ApiObjectTypes.Single(x => x.ClrType == typeof(People));
            var personWithId = apiSchema.ApiObjectTypes
                .Single(x => x.ClrType == typeof(PersonWithId));
            var explicitObject = apiSchema.ApiObjectTypes
                .Single(x => x.ClrType == typeof(OrderWithPersonId));

            people.ApiName.Should().Be
            (
                this.Style == NamingInflectionStyle.Singularize
                    ? "Person"
                    : "People"
            );
            people.ApiProperties.Single(x => x.ClrName == nameof(People.Id)).ApiName
                .Should().Be("Id");

            personWithId.ApiName.Should().Be
            (
                this.Style == NamingInflectionStyle.Pluralize
                    ? "PersonWithIds"
                    : "PersonWithId"
            );
            personWithId.ApiProperties.Single(x => x.ClrName == nameof(PersonWithId.Id)).ApiName
                .Should().Be("Id");
            personWithId.ApiProperties.Single(x => x.ClrName == nameof(PersonWithId.Name)).ApiName
                .Should().Be("Name");
            personWithId.ApiProperties.Single(x => x.ClrName == nameof(PersonWithId.Email)).ApiName
                .Should().Be("ExplicitProperty");

            explicitObject.ApiName.Should().Be("ExplicitObject");
        }
        #endregion
    }

    private sealed class NamingInflectionTargetSelectionTest : XUnitTest
    {
        #region User Supplied Properties
        public required NamingInflectionStyle Style { get; init; }
        public required ApiNamingConventionTargets Targets { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaBuilder? ApiSchemaBuilder { get; set; }
        private ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region Constructors
        public NamingInflectionTargetSelectionTest()
            => this.Name = nameof(NamingInflectionTargetSelectionTest);
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ApiSchemaBuilder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddScalar<Guid>()
                .AddScalar<string>()
                .AddObject<People>(x => x.AddProperty(p => p.Names))
                .AddObject<PersonWithId>(x => x.AddProperty(p => p.Name));
        }

        protected override void Act()
        {
            this.ApiSchemaActual = this.Style switch
            {
                NamingInflectionStyle.Pluralize => this.ApiSchemaBuilder!
                    .UsePluralizeNaming(this.Targets)
                    .Build(),
                NamingInflectionStyle.Singularize => this.ApiSchemaBuilder!
                    .UseSingularizeNaming(this.Targets)
                    .Build(),
                _ => throw new InvalidOperationException($"Unknown naming style: {this.Style}"),
            };
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();

            var apiSchema = this.ApiSchemaActual!;
            var people = apiSchema.ApiObjectTypes.Single(x => x.ClrType == typeof(People));
            var personWithId = apiSchema.ApiObjectTypes
                .Single(x => x.ClrType == typeof(PersonWithId));

            var isObjectTypeTargeted =
                (this.Targets & ApiNamingConventionTargets.ObjectType) !=
                ApiNamingConventionTargets.None;
            var isPropertyTargeted =
                (this.Targets & ApiNamingConventionTargets.Property) !=
                ApiNamingConventionTargets.None;

            people.ApiName.Should().Be
            (
                this.Style == NamingInflectionStyle.Singularize && isObjectTypeTargeted
                    ? "Person"
                    : "People"
            );
            people.ApiProperties.Single(x => x.ClrName == nameof(People.Names)).ApiName
                .Should().Be
                (
                    this.Style == NamingInflectionStyle.Singularize && isPropertyTargeted
                        ? "Name"
                        : "Names"
                );

            personWithId.ApiName.Should().Be
            (
                this.Style == NamingInflectionStyle.Pluralize && isObjectTypeTargeted
                    ? "PersonWithIds"
                    : "PersonWithId"
            );
            personWithId.ApiProperties.Single(x => x.ClrName == nameof(PersonWithId.Name)).ApiName
                .Should().Be
                (
                    this.Style == NamingInflectionStyle.Pluralize && isPropertyTargeted
                        ? "Names"
                        : "Name"
                );
        }
        #endregion
    }

    private sealed class NamingInflectionValidationTest : XUnitTest
    {
        #region User Supplied Properties
        public required NamingInflectionStyle Style { get; init; }
        public required string? ApiName { get; init; }
        public required Type ExpectedExceptionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiNamingConvention? NamingConvention { get; set; }
        private Exception? ExceptionActual { get; set; }
        #endregion

        #region Constructors
        public NamingInflectionValidationTest()
            => this.Name = nameof(NamingInflectionValidationTest);
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.NamingConvention = this.Style switch
            {
                NamingInflectionStyle.Pluralize => new ApiNamingPluralizeConvention(),
                NamingInflectionStyle.Singularize => new ApiNamingSingularizeConvention(),
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
            this.ExceptionActual.Should().BeOfType(this.ExpectedExceptionType);
        }
        #endregion
    }

    private sealed class NamingInflectionTargetValidationTest : XUnitTest
    {
        #region User Supplied Properties
        public required NamingInflectionStyle Style { get; init; }
        #endregion

        #region Calculated Properties
        private ApiNamingConvention? NamingConventionActual { get; set; }
        private Exception? ExceptionActual { get; set; }
        #endregion

        #region Constructors
        public NamingInflectionTargetValidationTest()
            => this.Name = nameof(NamingInflectionTargetValidationTest);
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
                    NamingInflectionStyle.Pluralize => new ApiNamingPluralizeConvention
                    (
                        (ApiNamingConventionTargets)int.MaxValue
                    ),
                    NamingInflectionStyle.Singularize => new ApiNamingSingularizeConvention
                    (
                        (ApiNamingConventionTargets)int.MaxValue
                    ),
                    _ => throw new InvalidOperationException
                    (
                        $"Unknown naming style: {this.Style}"
                    ),
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
    public static TheoryDataRow<IXUnitTest>[] NamingInflectionConversionTheoryData =>
    [
        new NamingInflectionConversionTest
        {
            Name = "Singularize converts an irregular plural API name",
            Style = NamingInflectionStyle.Singularize,
            ApiName = "People",
            ExpectedApiName = "Person",
        },
        new NamingInflectionConversionTest
        {
            Name = "Singularize preserves an already singular API name",
            Style = NamingInflectionStyle.Singularize,
            ApiName = "Person",
            ExpectedApiName = "Person",
        },
        new NamingInflectionConversionTest
        {
            Name = "Pluralize converts an irregular singular API name",
            Style = NamingInflectionStyle.Pluralize,
            ApiName = "Person",
            ExpectedApiName = "People",
        },
        new NamingInflectionConversionTest
        {
            Name = "Pluralize preserves an already plural API name",
            Style = NamingInflectionStyle.Pluralize,
            ApiName = "People",
            ExpectedApiName = "People",
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] NamingInflectionDefaultTargetTheoryData =>
    [
        new NamingInflectionDefaultTargetTest
        {
            Name = "Pluralize fluent naming defaults to object types",
            Style = NamingInflectionStyle.Pluralize,
            Registration = NamingInflectionRegistration.Fluent,
        },
        new NamingInflectionDefaultTargetTest
        {
            Name = "Pluralize convention defaults to object types",
            Style = NamingInflectionStyle.Pluralize,
            Registration = NamingInflectionRegistration.DirectConvention,
        },
        new NamingInflectionDefaultTargetTest
        {
            Name = "Singularize fluent naming defaults to object types",
            Style = NamingInflectionStyle.Singularize,
            Registration = NamingInflectionRegistration.Fluent,
        },
        new NamingInflectionDefaultTargetTest
        {
            Name = "Singularize convention defaults to object types",
            Style = NamingInflectionStyle.Singularize,
            Registration = NamingInflectionRegistration.DirectConvention,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] NamingInflectionTargetSelectionTheoryData =>
    [
        new NamingInflectionTargetSelectionTest
        {
            Name = "Pluralize naming selects all targets explicitly",
            Style = NamingInflectionStyle.Pluralize,
            Targets = ApiNamingConventionTargets.All,
        },
        new NamingInflectionTargetSelectionTest
        {
            Name = "Singularize naming disables all targets with None",
            Style = NamingInflectionStyle.Singularize,
            Targets = ApiNamingConventionTargets.None,
        },
        new NamingInflectionTargetSelectionTest
        {
            Name = "Singularize naming selects a property target",
            Style = NamingInflectionStyle.Singularize,
            Targets = ApiNamingConventionTargets.Property,
        },
        new NamingInflectionTargetSelectionTest
        {
            Name = "Pluralize naming selects a property target",
            Style = NamingInflectionStyle.Pluralize,
            Targets = ApiNamingConventionTargets.Property,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] NamingInflectionValidationTheoryData =>
    [
        new NamingInflectionValidationTest
        {
            Name = "Pluralize naming rejects null API names",
            Style = NamingInflectionStyle.Pluralize,
            ApiName = null,
            ExpectedExceptionType = typeof(ArgumentNullException),
        },
        new NamingInflectionValidationTest
        {
            Name = "Pluralize naming rejects whitespace API names",
            Style = NamingInflectionStyle.Pluralize,
            ApiName = " ",
            ExpectedExceptionType = typeof(ArgumentException),
        },
        new NamingInflectionValidationTest
        {
            Name = "Singularize naming rejects null API names",
            Style = NamingInflectionStyle.Singularize,
            ApiName = null,
            ExpectedExceptionType = typeof(ArgumentNullException),
        },
        new NamingInflectionValidationTest
        {
            Name = "Singularize naming rejects whitespace API names",
            Style = NamingInflectionStyle.Singularize,
            ApiName = " ",
            ExpectedExceptionType = typeof(ArgumentException),
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] NamingInflectionTargetValidationTheoryData =>
    [
        new NamingInflectionTargetValidationTest
        {
            Name = "Pluralize naming rejects unknown targets",
            Style = NamingInflectionStyle.Pluralize,
        },
        new NamingInflectionTargetValidationTest
        {
            Name = "Singularize naming rejects unknown targets",
            Style = NamingInflectionStyle.Singularize,
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(NamingInflectionConversionTheoryData))]
    public void NamingInflectionConversion(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(NamingInflectionDefaultTargetTheoryData))]
    public void NamingInflectionDefaultTarget(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(NamingInflectionTargetSelectionTheoryData))]
    public void NamingInflectionTargetSelection(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(NamingInflectionValidationTheoryData))]
    public void NamingInflectionValidation(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(NamingInflectionTargetValidationTheoryData))]
    public void NamingInflectionTargetValidation(IXUnitTest test) => test.Execute(this);
    #endregion
}
