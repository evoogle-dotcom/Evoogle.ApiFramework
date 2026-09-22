// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Conventions;
using Evoogle.ApiFramework.Schema.Version;
using Evoogle.ApiFramework.Version;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

public sealed class ApiVersionAnnotationTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private enum BuiltInVersionCase
    {
        Property,
        Field,
        Repository,
        InheritedRepository
    }

    private sealed class BuiltInVersionTest : XUnitTest
    {
        #region User Supplied Properties
        public required BuiltInVersionCase TestCase { get; init; }
        public required Type ClrTypeExpected { get; init; }
        public string? ClrMemberNameExpected { get; init; }
        #endregion

        #region Calculated Properties
        private ApiVersion MaterializedVersion { get; set; }
        private ApiVersionDefinition? VersionDefinition { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine($"ClrTypeExpected: {this.ClrTypeExpected}");
            this.WriteLine($"ClrMemberNameExpected: {this.ClrMemberNameExpected ?? "(none)"}");
        }

        protected override void Act()
        {
            var builder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddScalar<int>()
                .AddScalar<long>()
                .UseDefaultConventions()
                .UseDefaultAnnotations();

            builder = this.TestCase switch
            {
                BuiltInVersionCase.Property => builder.AddObject<PropertyVersionedType>(),
                BuiltInVersionCase.Field => builder.AddObject<FieldVersionedType>(),
                BuiltInVersionCase.Repository => builder.AddObject<RepositoryVersionedType>(),
                BuiltInVersionCase.InheritedRepository =>
                    builder.AddObject<InheritedRepositoryVersionedType>(),
                _ => throw new InvalidOperationException($"Unknown test case: {this.TestCase}")
            };

            this.VersionDefinition = builder.Build().ApiObjectTypes.Single().ApiVersion;
            this.MaterializedVersion = this.TestCase switch
            {
                BuiltInVersionCase.Property => this.VersionDefinition!.MaterializeVersion
                (
                    new PropertyVersionedType { Version = 42 }
                ),
                BuiltInVersionCase.Field => this.VersionDefinition!.MaterializeVersion
                (
                    new FieldVersionedType { Version = 42 }
                ),
                BuiltInVersionCase.Repository =>
                    this.VersionDefinition!.MaterializeVersionFromValue(42L),
                BuiltInVersionCase.InheritedRepository =>
                    this.VersionDefinition!.MaterializeVersionFromValue(42L),
                _ => throw new InvalidOperationException($"Unknown test case: {this.TestCase}")
            };
        }

        protected override void Assert()
        {
            this.VersionDefinition.Should().NotBeNull();
            this.VersionDefinition!.ClrType.Should().Be(this.ClrTypeExpected);
            this.VersionDefinition.ApiPropertyReference?.ClrName.Should()
                .Be(this.ClrMemberNameExpected);
            this.MaterializedVersion.HasValue.Should().BeTrue();
            this.MaterializedVersion.ClrType.Should().Be(this.ClrTypeExpected);
        }
        #endregion
    }

    private enum PrecedenceCase
    {
        LaterReader,
        ExplicitOverAnnotation,
        AnnotationOverConvention
    }

    private sealed class PrecedenceTest : XUnitTest
    {
        #region User Supplied Properties
        public required PrecedenceCase TestCase { get; init; }
        public required Type ClrTypeExpected { get; init; }
        public string? ClrMemberNameExpected { get; init; }
        #endregion

        #region Calculated Properties
        private ApiVersionDefinition? VersionDefinition { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine($"ClrTypeExpected: {this.ClrTypeExpected}");
            this.WriteLine($"ClrMemberNameExpected: {this.ClrMemberNameExpected ?? "(none)"}");
        }

        protected override void Act()
        {
            var builder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddScalar<int>()
                .AddScalar<long>()
                .AddScalar<Guid>();

            builder = this.TestCase switch
            {
                PrecedenceCase.LaterReader => builder
                    .AddObject<ReaderVersionedType>(ConfigureReaderVersionedType)
                    .UseAnnotations(annotations => annotations
                        .AddReader
                        (
                            new StaticVersionReader
                            (
                                new ApiVersionAnnotationResult
                                (
                                    nameof(ReaderVersionedType.First)
                                )
                            )
                        )
                        .AddReader
                        (
                            new StaticVersionReader
                            (
                                new ApiVersionAnnotationResult
                                (
                                    nameof(ReaderVersionedType.Second)
                                )
                            )
                        )),
                PrecedenceCase.ExplicitOverAnnotation => builder
                    .AddObject<AnnotatedPrecedenceType>(objectType => objectType
                        .AddRequiredProperty(x => x.Version)
                        .WithRepositoryVersion<Guid>())
                    .UseDefaultAnnotations(),
                PrecedenceCase.AnnotationOverConvention => builder
                    .AddObject<AnnotatedPrecedenceType>(objectType => objectType
                        .AddRequiredProperty(x => x.Version))
                    .UseDefaultAnnotations()
                    .UseConventions(conventions => conventions.AddConvention
                    (
                        new RepositoryVersionConvention()
                    )),
                _ => throw new InvalidOperationException($"Unknown test case: {this.TestCase}")
            };

            this.VersionDefinition = builder.Build().ApiObjectTypes.Single().ApiVersion;
        }

        protected override void Assert()
        {
            this.VersionDefinition.Should().NotBeNull();
            this.VersionDefinition!.ClrType.Should().Be(this.ClrTypeExpected);
            this.VersionDefinition.ApiPropertyReference?.ClrName.Should()
                .Be(this.ClrMemberNameExpected);
        }
        #endregion
    }

    private enum InvalidCase
    {
        MissingClrRepositoryType,
        MultipleMembers,
        TypeAndMember,
        MissingProperty,
        OptionalProperty,
        MemberClrTypeSpecified,
        NullableRepositoryType,
        UnregisteredScalar
    }

    private sealed class InvalidTest : XUnitTest
    {
        #region User Supplied Properties
        public required InvalidCase TestCase { get; init; }
        public required ApiSchemaCompilationCode CompilationCodeExpected { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaCompilationResult? Result { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine($"CompilationCodeExpected: {this.CompilationCodeExpected}");
        }

        protected override void Act()
        {
            var builder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddScalar<int>()
                .AddScalar<long>();

            builder = this.TestCase switch
            {
                InvalidCase.MissingClrRepositoryType => builder
                    .AddObject<MissingRepositoryTypeVersion>()
                    .UseDefaultAnnotations(),
                InvalidCase.MultipleMembers => builder
                    .AddObject<MultipleMemberVersions>(objectType => objectType
                        .AddRequiredProperty(x => x.First)
                        .AddRequiredProperty(x => x.Second))
                    .UseDefaultAnnotations(),
                InvalidCase.TypeAndMember => builder
                    .AddObject<TypeAndMemberVersions>(objectType => objectType
                        .AddRequiredProperty(x => x.Version))
                    .UseDefaultAnnotations(),
                InvalidCase.MissingProperty => builder
                    .AddObject<MissingPropertyVersion>()
                    .UseDefaultAnnotations(),
                InvalidCase.OptionalProperty => builder
                    .AddObject<OptionalPropertyVersion>(objectType => objectType
                        .AddOptionalProperty(x => x.Version))
                    .UseDefaultAnnotations(),
                InvalidCase.MemberClrTypeSpecified => builder
                    .AddObject<MemberClrTypeSpecifiedVersion>(objectType => objectType
                        .AddRequiredProperty(x => x.Version))
                    .UseDefaultAnnotations(),
                InvalidCase.NullableRepositoryType => builder
                    .AddObject<NullableRepositoryVersion>()
                    .UseDefaultAnnotations(),
                InvalidCase.UnregisteredScalar => new ApiSchemaBuilder()
                    .WithName("Test")
                    .AddObject<UnregisteredScalarVersion>(objectType => objectType
                        .AddRequiredProperty(x => x.Version))
                    .UseDefaultAnnotations(),
                _ => throw new InvalidOperationException($"Unknown test case: {this.TestCase}")
            };

            this.Result = builder.BuildResult();
        }

        protected override void Assert()
        {
            this.Result.Should().NotBeNull();
            this.Result!.Schema.Should().BeNull();
            this.Result.Errors.Should().Contain
            (
                issue => issue.Code == this.CompilationCodeExpected
            );
        }
        #endregion
    }

    private enum ReaderContractCase
    {
        NullList,
        NullResult,
        NullClrType,
        EmptyMemberName,
        ReaderThrows
    }

    private sealed class ReaderContractTest : XUnitTest
    {
        #region User Supplied Properties
        public required ReaderContractCase TestCase { get; init; }
        public required ApiSchemaCompilationCode CompilationCodeExpected { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaCompilationResult? Result { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine($"CompilationCodeExpected: {this.CompilationCodeExpected}");
        }

        protected override void Act()
        {
            this.Result = new ApiSchemaBuilder()
                .WithName("Test")
                .AddScalar<int>()
                .AddScalar<long>()
                .AddObject<ReaderVersionedType>(ConfigureReaderVersionedType)
                .UseAnnotations(annotations => annotations.AddReader
                (
                    new InvalidVersionReader(this.TestCase)
                ))
                .BuildResult();
        }

        protected override void Assert()
        {
            this.Result.Should().NotBeNull();
            this.Result!.Schema.Should().BeNull();
            this.Result.Errors.Should().Contain
            (
                issue => issue.Code == this.CompilationCodeExpected
            );
        }
        #endregion
    }

    private sealed class RepeatedBuildTest : XUnitTest
    {
        #region Calculated Properties
        private ApiSchemaCompilationResult? FirstResult { get; set; }
        private ApiSchemaCompilationResult? SecondResult { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            var builder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddScalar<int>()
                .AddScalar<long>()
                .AddObject<ReaderVersionedType>(ConfigureReaderVersionedType)
                .UseAnnotations(annotations => annotations.AddReader
                (
                    new InvalidVersionReader(ReaderContractCase.NullList)
                ));

            this.FirstResult = builder.BuildResult();
            this.SecondResult = builder.BuildResult();
        }

        protected override void Assert()
        {
            this.FirstResult.Should().NotBeNull();
            this.SecondResult.Should().NotBeNull();
            this.SecondResult!.Errors.Select(ToIssueSignature).Should().Equal
            (
                this.FirstResult!.Errors.Select(ToIssueSignature)
            );
        }
        #endregion
    }
    #endregion

    #region Test Readers and Conventions
    private sealed class StaticVersionReader(ApiVersionAnnotationResult version)
        : IApiVersionAnnotationReader
    {
        public IReadOnlyList<ApiVersionAnnotationResult> ReadVersionAnnotations(Type clrObjectType)
            => clrObjectType == typeof(ReaderVersionedType)
                ? [version]
                : [];
    }

    private sealed class InvalidVersionReader(ReaderContractCase testCase)
        : IApiVersionAnnotationReader
    {
        public IReadOnlyList<ApiVersionAnnotationResult> ReadVersionAnnotations(Type clrType)
        {
            if (clrType != typeof(ReaderVersionedType))
            {
                return [];
            }

            return testCase switch
            {
                ReaderContractCase.NullList => null!,
                ReaderContractCase.NullResult => [null!],
                ReaderContractCase.NullClrType => [new((Type)null!)],
                ReaderContractCase.EmptyMemberName => [new(string.Empty)],
                ReaderContractCase.ReaderThrows => throw new InvalidOperationException
                (
                    "Reader failure."
                ),
                _ => throw new InvalidOperationException($"Unknown test case: {testCase}")
            };
        }
    }

    private sealed class RepositoryVersionConvention : IApiObjectTypeConvention
    {
        public ApiConventionPhase Phase => ApiConventionPhase.Configuration;

        public void Apply(ApiObjectTypeBuilder builder)
        {
            if (builder.ClrType == typeof(AnnotatedPrecedenceType))
            {
                builder.WithRepositoryVersion(typeof(long));
            }
        }
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuiltInVersionTheoryData =>
    [
        new BuiltInVersionTest
        {
            Name = "Member annotation creates a property-backed version",
            TestCase = BuiltInVersionCase.Property,
            ClrTypeExpected = typeof(int),
            ClrMemberNameExpected = nameof(PropertyVersionedType.Version)
        },
        new BuiltInVersionTest
        {
            Name = "Field annotation creates a property-backed version",
            TestCase = BuiltInVersionCase.Field,
            ClrTypeExpected = typeof(int),
            ClrMemberNameExpected = nameof(FieldVersionedType.Version)
        },
        new BuiltInVersionTest
        {
            Name = "Type annotation creates a repository-backed version",
            TestCase = BuiltInVersionCase.Repository,
            ClrTypeExpected = typeof(long),
            ClrMemberNameExpected = null
        },
        new BuiltInVersionTest
        {
            Name = "Repository version annotation is inherited",
            TestCase = BuiltInVersionCase.InheritedRepository,
            ClrTypeExpected = typeof(long),
            ClrMemberNameExpected = null
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] PrecedenceTheoryData =>
    [
        new PrecedenceTest
        {
            Name = "Later version annotation reader replaces earlier reader",
            TestCase = PrecedenceCase.LaterReader,
            ClrTypeExpected = typeof(long),
            ClrMemberNameExpected = nameof(ReaderVersionedType.Second)
        },
        new PrecedenceTest
        {
            Name = "Explicit version configuration wins over annotation",
            TestCase = PrecedenceCase.ExplicitOverAnnotation,
            ClrTypeExpected = typeof(Guid),
            ClrMemberNameExpected = null
        },
        new PrecedenceTest
        {
            Name = "Version annotation wins over convention",
            TestCase = PrecedenceCase.AnnotationOverConvention,
            ClrTypeExpected = typeof(int),
            ClrMemberNameExpected = nameof(AnnotatedPrecedenceType.Version)
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] InvalidTheoryData =>
    [
        new InvalidTest
        {
            Name = "Type annotation requires a repository CLR type",
            TestCase = InvalidCase.MissingClrRepositoryType,
            CompilationCodeExpected = ApiSchemaCompilationCode.ApiAnnotationInvalidContribution
        },
        new InvalidTest
        {
            Name = "Multiple member annotations produce a version conflict",
            TestCase = InvalidCase.MultipleMembers,
            CompilationCodeExpected = ApiSchemaCompilationCode.ApiAnnotationVersionConflict
        },
        new InvalidTest
        {
            Name = "Type and member annotations produce a version conflict",
            TestCase = InvalidCase.TypeAndMember,
            CompilationCodeExpected = ApiSchemaCompilationCode.ApiAnnotationVersionConflict
        },
        new InvalidTest
        {
            Name = "Member annotation does not add an API property",
            TestCase = InvalidCase.MissingProperty,
            CompilationCodeExpected = ApiSchemaCompilationCode.ApiVersionDefinitionUnresolvedProperty
        },
        new InvalidTest
        {
            Name = "Member annotation does not make its API property required",
            TestCase = InvalidCase.OptionalProperty,
            CompilationCodeExpected = ApiSchemaCompilationCode.ApiVersionDefinitionOptionalProperty
        },
        new InvalidTest
        {
            Name = "Member annotation rejects a repository CLR type",
            TestCase = InvalidCase.MemberClrTypeSpecified,
            CompilationCodeExpected = ApiSchemaCompilationCode.ApiAnnotationInvalidContribution
        },
        new InvalidTest
        {
            Name = "Repository annotation rejects a nullable CLR value type",
            TestCase = InvalidCase.NullableRepositoryType,
            CompilationCodeExpected = ApiSchemaCompilationCode.ApiVersionDefinitionNullableClrType
        },
        new InvalidTest
        {
            Name = "Member annotation does not register its scalar type",
            TestCase = InvalidCase.UnregisteredScalar,
            CompilationCodeExpected = ApiSchemaCompilationCode.ApiPropertyUnresolvedType
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] ReaderContractTheoryData =>
    [
        .. Enum.GetValues<ReaderContractCase>().Select
        (
            testCase => new TheoryDataRow<IXUnitTest>
            (
                new ReaderContractTest
                {
                    Name = $"Version reader contract handles {testCase}",
                    TestCase = testCase,
                    CompilationCodeExpected = testCase == ReaderContractCase.ReaderThrows
                        ? ApiSchemaCompilationCode.ApiAnnotationReaderExecutionFailed
                        : ApiSchemaCompilationCode.ApiAnnotationInvalidContribution
                }
            )
        )
    ];

    public static TheoryDataRow<IXUnitTest>[] RepeatedBuildTheoryData =>
    [
        new RepeatedBuildTest
        {
            Name = "Repeated builds retain stable version reader diagnostics"
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuiltInVersionTheoryData))]
    public void BuiltInVersion(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(PrecedenceTheoryData))]
    public void Precedence(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(InvalidTheoryData))]
    public void Invalid(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ReaderContractTheoryData))]
    public void ReaderContract(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(RepeatedBuildTheoryData))]
    public void RepeatedBuild(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Private Methods
    private static void ConfigureReaderVersionedType
    (
        ApiObjectTypeBuilder<ReaderVersionedType> builder
    )
    {
        builder
            .AddRequiredProperty(x => x.First)
            .AddRequiredProperty(x => x.Second);
    }

    private static
        (
            string ApiPath,
            ApiSchemaCompilationSeverity Severity,
            ApiSchemaCompilationCode Code,
            string Description
        )
        ToIssueSignature(ApiSchemaCompilationIssue issue)
        => (issue.ApiPath, issue.Severity, issue.Code, issue.Description);
    #endregion

    #region Test Types
    private sealed class PropertyVersionedType
    {
        [ApiVersion]
        public int Version { get; init; }
    }

    private sealed class FieldVersionedType
    {
        [ApiVersion]
        public int Version;
    }

    [ApiVersion(ClrType = typeof(long))]
    private sealed class RepositoryVersionedType;

    [ApiVersion(ClrType = typeof(long))]
    private class RepositoryVersionBase;

    private sealed class InheritedRepositoryVersionedType : RepositoryVersionBase;

    private sealed class ReaderVersionedType
    {
        public int First { get; init; }
        public long Second { get; init; }
    }

    private sealed class AnnotatedPrecedenceType
    {
        [ApiVersion]
        public int Version { get; init; }
    }

    [ApiVersion]
    private sealed class MissingRepositoryTypeVersion;

    private sealed class MultipleMemberVersions
    {
        [ApiVersion]
        public int First { get; init; }

        [ApiVersion]
        public long Second { get; init; }
    }

    [ApiVersion(ClrType = typeof(long))]
    private sealed class TypeAndMemberVersions
    {
        [ApiVersion]
        public int Version { get; init; }
    }

    private sealed class MissingPropertyVersion
    {
        [ApiVersion]
        public int Version { get; init; }
    }

    private sealed class OptionalPropertyVersion
    {
        [ApiVersion]
        public int Version { get; init; }
    }

    private sealed class MemberClrTypeSpecifiedVersion
    {
        [ApiVersion(ClrType = typeof(long))]
        public int Version { get; init; }
    }

    [ApiVersion(ClrType = typeof(int?))]
    private sealed class NullableRepositoryVersion;

    private sealed class UnregisteredScalarVersion
    {
        [ApiVersion]
        public int Version { get; init; }
    }
    #endregion
}
