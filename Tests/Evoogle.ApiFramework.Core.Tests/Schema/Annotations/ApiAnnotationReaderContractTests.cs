// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Configuration;
using Evoogle.ApiFramework.Schema.Configuration.Annotations;
using Evoogle.ApiFramework.Schema.Configuration.Annotations.Internal;
using Evoogle.Reflection;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Annotations;

public sealed class ApiAnnotationReaderContractTests(ITestOutputHelper output)
    : XUnitTests(output)
{
    #region Test Classes
    private enum ConfigurationTestCase
    {
        AdditiveCallsPreserveReaders,
        CallbackFailurePreservesExistingReaders,
        CustomThenDefault,
        DefaultThenCustom
    }

    private sealed class ConfigurationTest : XUnitTest
    {
        #region User Supplied Properties
        public required ConfigurationTestCase TestCase { get; init; }
        public required string ApiNameExpected { get; init; }
        public required int ReaderCallsExpected { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaBuilder? ApiSchemaBuilder { get; set; }
        private ApiSchema? ApiSchemaActual { get; set; }
        private CountingTypeNameReader? Reader { get; set; }
        private Exception? ConfigurationExceptionActual { get; set; }
        private Exception? BuildExceptionActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var reader = new CountingTypeNameReader("CustomName");
            this.Reader = reader;

            var apiSchemaBuilder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddObject<AnnotationConfigurationObject>(builder => builder
                    .AddProperty("Id", nameof(AnnotationConfigurationObject.Id)))
                .AddScalar<int>();

            switch (this.TestCase)
            {
                case ConfigurationTestCase.AdditiveCallsPreserveReaders:
                    apiSchemaBuilder
                        .UseAnnotations(annotations => annotations.AddReader(reader))
                        .UseAnnotations(annotations => annotations.AddReader(reader));
                    break;
                case ConfigurationTestCase.CallbackFailurePreservesExistingReaders:
                    apiSchemaBuilder.UseAnnotations(annotations => annotations.AddReader(reader));
                    break;
                case ConfigurationTestCase.CustomThenDefault:
                    apiSchemaBuilder
                        .UseAnnotations(annotations => annotations.AddReader(reader))
                        .UseDefaultAnnotations();
                    break;
                case ConfigurationTestCase.DefaultThenCustom:
                    apiSchemaBuilder
                        .UseDefaultAnnotations()
                        .UseAnnotations(annotations => annotations.AddReader(reader));
                    break;
                default:
                    throw new InvalidOperationException($"Unknown test case: {this.TestCase}");
            }

            this.ApiSchemaBuilder = apiSchemaBuilder;

            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine($"ApiNameExpected: {this.ApiNameExpected}");
            this.WriteLine($"ReaderCallsExpected: {this.ReaderCallsExpected}");
            this.WriteLine();
        }

        protected override void Act()
        {
            if (this.TestCase == ConfigurationTestCase.CallbackFailurePreservesExistingReaders)
            {
                try
                {
                    this.ApiSchemaBuilder!.UseAnnotations
                    (
                        annotations =>
                        {
                            annotations.AddReader(new CountingTypeNameReader("DiscardedName"));
                            throw new InvalidOperationException("configuration failure");
                        }
                    );
                }
                catch (Exception exception)
                {
                    this.ConfigurationExceptionActual = exception;
                }
            }

            try
            {
                this.ApiSchemaActual = this.ApiSchemaBuilder!.Build();
            }
            catch (Exception exception)
            {
                this.BuildExceptionActual = exception;
            }
        }

        protected override void Assert()
        {
            this.BuildExceptionActual.Should().BeNull();
            if (this.TestCase == ConfigurationTestCase.CallbackFailurePreservesExistingReaders)
            {
                this.ConfigurationExceptionActual.Should().BeOfType<InvalidOperationException>();
            }
            else
            {
                this.ConfigurationExceptionActual.Should().BeNull();
            }
            this.ApiSchemaActual.Should().NotBeNull();
            this.ApiSchemaActual!.ApiObjectTypes.Single().ApiName.Should().Be(this.ApiNameExpected);
            this.Reader!.CallCount.Should().Be(this.ReaderCallsExpected);

            if (this.TestCase == ConfigurationTestCase.CallbackFailurePreservesExistingReaders)
            {
                this.ApiSchemaBuilder.Should().NotBeNull();
            }
        }
        #endregion
    }

    private sealed class BoundaryTest : XUnitTest
    {
        #region Calculated Properties
        private Type? ReaderSetTypeActual { get; set; }
        private MethodInfo? BuildMethodActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ReaderSetTypeActual = typeof(ApiAnnotationReaderSet);
            this.BuildMethodActual = typeof(ApiAnnotationReaderSetBuilder).GetMethod
            (
                nameof(ApiAnnotationReaderSetBuilder.Build),
                BindingFlags.Instance | BindingFlags.NonPublic
            );
        }

        protected override void Act()
        {
        }

        protected override void Assert()
        {
            this.ReaderSetTypeActual!.IsPublic.Should().BeFalse();
            this.BuildMethodActual.Should().NotBeNull();
            this.BuildMethodActual!.IsAssembly.Should().BeTrue();
        }
        #endregion
    }

    private enum DiscoveryValidationCase
    {
        FilterExcluded,
        IneligibleType,
        InvalidApiKind,
        InvalidApiKindThenValid,
        NullClrType,
        NullContributions,
        NullDiagnostics,
        NullResultRow,
        NullWrapper,
        Valid
    }

    private sealed class DiscoveryValidationTest : XUnitTest
    {
        #region User Supplied Properties
        public required DiscoveryValidationCase TestCase { get; init; }
        #endregion

        #region Calculated Properties
        private ApiAnnotationReaderSet? ReaderSet { get; set; }
        private IReadOnlyList<ApiTypeDiscoveryAnnotationResult>? ResultsActual { get; set; }
        private IReadOnlyList<ApiInitializationIssue>? IssuesActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ReaderSet = new ApiAnnotationReaderSetBuilder()
                .AddReader(new DiscoveryValidationReader(this.TestCase))
                .Build();

            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine();
        }

        protected override void Act()
        {
            Func<Type, bool> filter = this.TestCase == DiscoveryValidationCase.FilterExcluded
                ? static (Type _) => false
                : static type => type == typeof(DiscoveryValidType) ||
                    type == typeof(DiscoverySecondValidType);

            var readerSet = this.ReaderSet!;
            this.ResultsActual = readerSet.ReadTypeDiscoveryAnnotations
            (
                typeof(ApiAnnotationReaderContractTests).Assembly,
                filter
            );
            this.IssuesActual = readerSet.Issues;
        }

        protected override void Assert()
        {
            this.ResultsActual.Should().NotBeNull();
            this.IssuesActual.Should().NotBeNull();

            var expectedIssue = this.TestCase != DiscoveryValidationCase.Valid;
            this.IssuesActual!.Any
            (
                issue => issue.Code == ApiInitializationCode.ApiAnnotationInvalidContribution
            )
                .Should().Be(expectedIssue);

            if (this.TestCase is DiscoveryValidationCase.NullDiagnostics or
                DiscoveryValidationCase.NullResultRow or
                DiscoveryValidationCase.NullClrType)
            {
                this.ResultsActual!.Select(result => result.ClrType)
                    .Should().Contain(typeof(DiscoveryValidType));
            }

            if (this.TestCase == DiscoveryValidationCase.InvalidApiKindThenValid)
            {
                this.ResultsActual!.Select(result => result.ClrType)
                    .Should().Contain(typeof(DiscoverySecondValidType));
            }

            if (this.TestCase == DiscoveryValidationCase.Valid)
            {
                this.ResultsActual!.Select(result => result.ClrType)
                    .Should().Contain(typeof(DiscoveryValidType));
            }
        }
        #endregion
    }

    private sealed class MarkerConflictTest : XUnitTest
    {
        #region Calculated Properties
        private IReadOnlyList<ApiTypeDiscoveryAnnotationResult>? ResultsActual { get; set; }
        private IReadOnlyList<ApiInitializationIssue>? IssuesActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
        }

        protected override void Act()
        {
            var readerSet = new ApiAnnotationReaderSetBuilder()
                .AddReader(new ApiAttributeAnnotationReader())
                .Build();

            this.ResultsActual = readerSet.ReadTypeDiscoveryAnnotations
            (
                typeof(ApiAnnotationReaderContractTests).Assembly,
                static type => type == typeof(ConflictingMarkerType) ||
                    type == typeof(ValidMarkerType)
            );
            this.IssuesActual = readerSet.Issues;
        }

        protected override void Assert()
        {
            this.ResultsActual!.Select(result => result.ClrType)
                .Should().Contain(typeof(ValidMarkerType));
            this.ResultsActual!.Select(result => result.ClrType)
                .Should().NotContain(typeof(ConflictingMarkerType));

            var issue = this.IssuesActual!.Single
            (
                issue => issue.Code == ApiInitializationCode.ApiAnnotationTypeMarkerConflict
            );
            issue.ReaderType.Should().Be(typeof(ApiAttributeAnnotationReader));
            issue.Description.Should().Contain("both");
        }
        #endregion
    }

    private enum IssueMetadataTestCase
    {
        DiagnosticMetadata,
        ExecutionMetadata,
        PersistentIssueHistory
    }

    private sealed class IssueMetadataTest : XUnitTest
    {
        #region User Supplied Properties
        public required IssueMetadataTestCase TestCase { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaInitializationException? ExceptionActual { get; set; }
        private IReadOnlyList<ApiInitializationIssue>? IssuesActual { get; set; }
        private Exception? ReaderExceptionExpected { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ReaderExceptionExpected = new InvalidOperationException("reader failure");
            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine();
        }

        protected override void Act()
        {
            switch (this.TestCase)
            {
                case IssueMetadataTestCase.DiagnosticMetadata:
                    {
                        var readerSet = new ApiAnnotationReaderSetBuilder()
                            .AddReader(new DiagnosticReader(this.ReaderExceptionExpected!))
                            .Build();
                        _ = readerSet.ReadTypeDiscoveryAnnotations
                        (
                            typeof(ApiAnnotationReaderContractTests).Assembly,
                            static type => type == typeof(DiscoveryValidType)
                        );
                        this.IssuesActual = readerSet.Issues;
                        break;
                    }
                case IssueMetadataTestCase.ExecutionMetadata:
                    {
                        var builder = BuildMetadataSchema
                        (
                            new ThrowingTypeReader(this.ReaderExceptionExpected!)
                        );
                        try
                        {
                            _ = builder.Build();
                        }
                        catch (ApiSchemaInitializationException exception)
                        {
                            this.ExceptionActual = exception;
                        }

                        break;
                    }
                case IssueMetadataTestCase.PersistentIssueHistory:
                    {
                        var builder = BuildMetadataSchema
                        (
                            new ThrowingTypeReader(this.ReaderExceptionExpected!)
                        );
                        try
                        {
                            _ = builder.Build();
                        }
                        catch (ApiSchemaInitializationException exception)
                        {
                            this.ExceptionActual = exception;
                        }

                        try
                        {
                            _ = builder.Build();
                        }
                        catch (ApiSchemaInitializationException exception)
                        {
                            this.ExceptionActual = exception;
                        }

                        break;
                    }
                default:
                    throw new InvalidOperationException($"Unknown test case: {this.TestCase}");
            }
        }

        protected override void Assert()
        {
            if (this.TestCase == IssueMetadataTestCase.DiagnosticMetadata)
            {
                var issue = this.IssuesActual!.Single();
                issue.ReaderType.Should().Be(typeof(DiagnosticReader));
                issue.Exception.Should().BeSameAs(this.ReaderExceptionExpected);
                return;
            }

            this.ExceptionActual.Should().NotBeNull();
            var errors = this.ExceptionActual!.Errors;
            errors.Should().NotBeEmpty();
            errors.Select(issue => issue.ReaderType)
                .Should().Contain(typeof(ThrowingTypeReader));
            errors.Select(issue => issue.Exception)
                .Should().Contain(this.ReaderExceptionExpected);

            if (this.TestCase == IssueMetadataTestCase.PersistentIssueHistory)
            {
                errors.Count(issue => issue.ReaderType == typeof(ThrowingTypeReader))
                    .Should().Be(2);
            }

            var issueWithException = errors.First(issue => issue.Exception is not null);
            issueWithException.ToMessage().Should().NotContain(nameof(ThrowingTypeReader));
            issueWithException.ToString().Should().NotContain(nameof(ThrowingTypeReader));
        }
        #endregion
    }

    private enum CapabilityTestCase
    {
        EnumValue,
        Key,
        ObjectProperty,
        Relationship,
        Type
    }

    private sealed class CapabilityTest : XUnitTest
    {
        #region User Supplied Properties
        public required CapabilityTestCase TestCase { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaBuilder? ApiSchemaBuilder { get; set; }
        private ApiSchema? ApiSchemaActual { get; set; }
        private Exception? ExceptionActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var builder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddScalar<int>()
                .AddScalar<string>()
                .UsePropertyDiscovery()
                .UseAnnotations(annotations => annotations.AddReader(new CapabilityReader()));

            this.ApiSchemaBuilder = this.TestCase switch
            {
                CapabilityTestCase.EnumValue => builder
                    .UseEnumValueDiscovery()
                    .AddEnum<CapabilityEnum>(enumBuilder => enumBuilder.AddAllValues()),
                CapabilityTestCase.Key => builder.AddObject<CapabilityObject>
                (
                    objectBuilder => objectBuilder
                        .AddProperty("Id", nameof(CapabilityObject.Id))
                ),
                CapabilityTestCase.ObjectProperty => builder.AddObject<CapabilityObject>(),
                CapabilityTestCase.Relationship => builder
                    .AddObject<CapabilityPrincipal>(objectBuilder => objectBuilder
                        .AddProperty("Id", nameof(CapabilityPrincipal.Id))
                        .AddKey("PrincipalKey", keyBuilder => keyBuilder.AddPath
                        (
                            typeof(CapabilityPrincipal),
                            nameof(CapabilityPrincipal.Id)
                        )))
                    .AddObject<CapabilityDependent>(objectBuilder => objectBuilder
                        .AddProperty("Id", nameof(CapabilityDependent.Id))
                        .AddProperty("PrincipalId", nameof(CapabilityDependent.PrincipalId))
                        .AddKey("DependentKey", keyBuilder => keyBuilder.AddPath
                        (
                            typeof(CapabilityDependent),
                            nameof(CapabilityDependent.Id)
                        ))),
                CapabilityTestCase.Type => builder.AddObject<CapabilityObject>
                (
                    objectBuilder => objectBuilder
                        .AddProperty("Id", nameof(CapabilityObject.Id))
                ),
                _ => throw new InvalidOperationException($"Unknown test case: {this.TestCase}")
            };

            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine();
        }

        protected override void Act()
        {
            try
            {
                this.ApiSchemaActual = this.ApiSchemaBuilder!.Build();
            }
            catch (Exception exception)
            {
                this.ExceptionActual = exception;
            }
        }

        protected override void Assert()
        {
            this.ExceptionActual.Should().BeNull();
            this.ApiSchemaActual.Should().NotBeNull();

            switch (this.TestCase)
            {
                case CapabilityTestCase.EnumValue:
                    this.ApiSchemaActual!.ApiEnumTypes.Single().TryGetValueByClrName
                    (
                        nameof(CapabilityEnum.First),
                        out var enumValue
                    ).Should().BeTrue();
                    enumValue!.ApiName.Should().Be("FirstApi");
                    break;
                case CapabilityTestCase.Key:
                    this.ApiSchemaActual!.ApiObjectTypes.Single().ApiKeyTypes.Single().ApiName
                        .Should().Be("CapabilityKey");
                    break;
                case CapabilityTestCase.ObjectProperty:
                    this.ApiSchemaActual!.ApiObjectTypes.Single().ApiProperties
                        .Single
                        (
                            property => property.ClrName == nameof(CapabilityObject.Value)
                        ).ApiName
                        .Should().Be("ValueApi");
                    break;
                case CapabilityTestCase.Relationship:
                    this.ApiSchemaActual!.ApiRelationships.Single().ApiName
                        .Should().Be("CapabilityRelationship");
                    break;
                case CapabilityTestCase.Type:
                    this.ApiSchemaActual!.ApiObjectTypes.Single().ApiName
                        .Should().Be("CapabilityType");
                    break;
                default:
                    throw new InvalidOperationException($"Unknown test case: {this.TestCase}");
            }
        }
        #endregion
    }

    private enum InvalidCapabilityCase
    {
        EnumValueNullList,
        EnumValueNullRow,
        KeyNullList,
        KeyNullRow,
        PropertyNullList,
        PropertyNullRow,
        RelationshipNullList,
        RelationshipNullRow,
        TypeNullList,
        TypeNullRow
    }

    private sealed class InvalidContributionTest : XUnitTest
    {
        #region User Supplied Properties
        public required InvalidCapabilityCase TestCase { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaBuilder? ApiSchemaBuilder { get; set; }
        private ApiSchemaInitializationException? ExceptionActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var builder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddScalar<int>()
                .UseAnnotations
                (
                    annotations => annotations.AddReader
                    (
                        new InvalidContributionReader(this.TestCase)
                    )
                );

            this.ApiSchemaBuilder = this.TestCase switch
            {
                InvalidCapabilityCase.EnumValueNullList or InvalidCapabilityCase.EnumValueNullRow =>
                    builder
                        .UseEnumValueDiscovery()
                        .AddEnum<CapabilityEnum>(enumBuilder => enumBuilder.AddAllValues()),
                InvalidCapabilityCase.RelationshipNullList or
                    InvalidCapabilityCase.RelationshipNullRow => builder
                        .AddObject<CapabilityPrincipal>(objectBuilder => objectBuilder
                            .AddProperty("Id", nameof(CapabilityPrincipal.Id)))
                        .AddObject<CapabilityDependent>(objectBuilder => objectBuilder
                            .AddProperty("Id", nameof(CapabilityDependent.Id))),
                _ => builder.AddObject<CapabilityObject>(objectBuilder => objectBuilder
                    .AddProperty("Id", nameof(CapabilityObject.Id))
                    .AddProperty("Value", nameof(CapabilityObject.Value)))
            };

            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine();
        }

        protected override void Act()
        {
            try
            {
                _ = this.ApiSchemaBuilder!.Build();
            }
            catch (ApiSchemaInitializationException exception)
            {
                this.ExceptionActual = exception;
            }
        }

        protected override void Assert()
        {
            this.ExceptionActual.Should().NotBeNull();
            this.ExceptionActual!.Errors
                .Should().Contain(issue =>
                    issue.Code == ApiInitializationCode.ApiAnnotationInvalidContribution &&
                    issue.ReaderType == typeof(InvalidContributionReader));
        }
        #endregion
    }
    #endregion

    #region Test Readers
    private sealed class CountingTypeNameReader(string apiName) : IApiTypeAnnotationReader
    {
        public int CallCount { get; private set; }

        public IReadOnlyList<ApiTypeAnnotationResult> ReadObjectTypeAnnotations(Type clrType)
        {
            if (clrType == typeof(AnnotationConfigurationObject) ||
                clrType == typeof(DefaultAnnotationConfigurationObject))
            {
                this.CallCount++;
                return [new(apiName)];
            }

            return [];
        }

        public IReadOnlyList<ApiTypeAnnotationResult> ReadScalarTypeAnnotations(Type clrType) => [];

        public IReadOnlyList<ApiTypeAnnotationResult> ReadEnumTypeAnnotations(Type clrType) => [];
    }

    private sealed class DiscoveryValidationReader(DiscoveryValidationCase testCase)
        : IApiTypeDiscoveryAnnotationReader
    {
        public ApiAnnotationReaderResult<ApiTypeDiscoveryAnnotationResult>
            ReadTypeDiscoveryAnnotations
        (
            Assembly assembly,
            Func<Type, bool>? filter
        )
        {
            return testCase switch
            {
                DiscoveryValidationCase.FilterExcluded or DiscoveryValidationCase.Valid =>
                    new([new(typeof(DiscoveryValidType), ApiTypeKind.Object)], []),
                DiscoveryValidationCase.IneligibleType =>
                    new([new(typeof(string), ApiTypeKind.Object)], []),
                DiscoveryValidationCase.InvalidApiKind =>
                    new([new(typeof(DiscoveryValidType), (ApiTypeKind)999)], []),
                DiscoveryValidationCase.InvalidApiKindThenValid => new
                (
                    [
                        new(typeof(DiscoveryValidType), (ApiTypeKind)999),
                        new(typeof(DiscoverySecondValidType), ApiTypeKind.Object)
                    ],
                    []
                ),
                DiscoveryValidationCase.NullClrType => new
                (
                    [
                        new(null!, ApiTypeKind.Object),
                        new(typeof(DiscoveryValidType), ApiTypeKind.Object)
                    ],
                    []
                ),
                DiscoveryValidationCase.NullContributions => new(null!, []),
                DiscoveryValidationCase.NullDiagnostics => new
                (
                    [new(typeof(DiscoveryValidType), ApiTypeKind.Object)],
                    null!
                ),
                DiscoveryValidationCase.NullResultRow => new
                (
                    [null!, new(typeof(DiscoveryValidType), ApiTypeKind.Object)],
                    []
                ),
                DiscoveryValidationCase.NullWrapper => null!,
                _ => throw new InvalidOperationException($"Unknown test case: {testCase}")
            };
        }
    }

    private sealed class DiagnosticReader(Exception exception) : IApiTypeDiscoveryAnnotationReader
    {
        public ApiAnnotationReaderResult<ApiTypeDiscoveryAnnotationResult>
            ReadTypeDiscoveryAnnotations
        (
            Assembly assembly,
            Func<Type, bool>? filter
        )
            => new
            (
                [],
                [new
                (
                    ApiInitializationCode.ApiAnnotationInvalidContribution,
                    "DiagnosticPath",
                    "A diagnostic reader reported an invalid contribution.",
                    "Correct the contribution.",
                    exception
                )]
            );
    }

    private sealed class ThrowingTypeReader(Exception exception) : IApiTypeAnnotationReader
    {
        public IReadOnlyList<ApiTypeAnnotationResult> ReadObjectTypeAnnotations(Type clrType)
        {
            if (clrType == typeof(MetadataObject))
            {
                throw exception;
            }

            return [];
        }

        public IReadOnlyList<ApiTypeAnnotationResult> ReadScalarTypeAnnotations(Type clrType) => [];

        public IReadOnlyList<ApiTypeAnnotationResult> ReadEnumTypeAnnotations(Type clrType) => [];
    }

    private sealed class CapabilityReader :
        IApiTypeAnnotationReader,
        IApiPropertyAnnotationReader,
        IApiEnumValueAnnotationReader,
        IApiKeyAnnotationReader,
        IApiRelationshipAnnotationReader
    {
        public IReadOnlyList<ApiTypeAnnotationResult> ReadObjectTypeAnnotations(Type clrType)
            => clrType == typeof(CapabilityObject) ? [new("CapabilityType")] : [];

        public IReadOnlyList<ApiTypeAnnotationResult> ReadScalarTypeAnnotations(Type clrType) => [];

        public IReadOnlyList<ApiTypeAnnotationResult> ReadEnumTypeAnnotations(Type clrType) => [];

        public IReadOnlyList<ApiPropertyAnnotationResult> ReadPropertyAnnotations
        (
            MemberInfo clrMember,
            ClrMemberKind clrMemberKind,
            MemberNullableInfo clrNullabilityInfo
        )
            => clrMember.DeclaringType == typeof(CapabilityObject) &&
                clrMember.Name == nameof(CapabilityObject.Value)
                ? [new("ValueApi", null)]
                : [];

        public IReadOnlyList<ApiEnumValueAnnotationResult>
            ReadEnumValueAnnotations(FieldInfo clrField)
            => clrField.DeclaringType == typeof(CapabilityEnum) &&
                clrField.Name == nameof(CapabilityEnum.First)
                ? [new("FirstApi")]
                : [];

        public IReadOnlyList<ApiKeyAnnotationResult> ReadKeyAnnotations(Type clrType)
            => clrType == typeof(CapabilityObject)
                ? [new("CapabilityKey", 0, clrType, [nameof(CapabilityObject.Id)])]
                : [];

        public IReadOnlyList<ApiOneToManyRelationshipAnnotationResult> ReadOneToManyRelationships
        (
            Type clrType
        )
            => clrType == typeof(CapabilityPrincipal)
                ? [new
                (
                    "CapabilityRelationship",
                    typeof(CapabilityPrincipal),
                    typeof(CapabilityDependent),
                    nameof(CapabilityDependent.PrincipalId),
                    ApiRelationshipDeleteBehavior.None
                )]
                : [];

        public IReadOnlyList<ApiOneToOneRelationshipAnnotationResult> ReadOneToOneRelationships
        (
            Type clrType
        ) => [];

        public IReadOnlyList<ApiManyToManyRelationshipAnnotationResult> ReadManyToManyRelationships
        (
            Type clrType
        ) => [];
    }

    private sealed class InvalidContributionReader(InvalidCapabilityCase testCase) :
        IApiTypeAnnotationReader,
        IApiPropertyAnnotationReader,
        IApiEnumValueAnnotationReader,
        IApiKeyAnnotationReader,
        IApiRelationshipAnnotationReader
    {
        public IReadOnlyList<ApiTypeAnnotationResult> ReadObjectTypeAnnotations(Type clrType)
            => testCase switch
            {
                InvalidCapabilityCase.TypeNullList => null!,
                InvalidCapabilityCase.TypeNullRow => [null!],
                _ => []
            };

        public IReadOnlyList<ApiTypeAnnotationResult> ReadScalarTypeAnnotations(Type clrType) => [];

        public IReadOnlyList<ApiTypeAnnotationResult> ReadEnumTypeAnnotations(Type clrType) => [];

        public IReadOnlyList<ApiPropertyAnnotationResult> ReadPropertyAnnotations
        (
            MemberInfo clrMember,
            ClrMemberKind clrMemberKind,
            MemberNullableInfo clrNullabilityInfo
        )
            => testCase switch
            {
                InvalidCapabilityCase.PropertyNullList => null!,
                InvalidCapabilityCase.PropertyNullRow => [null!],
                _ => []
            };

        public IReadOnlyList<ApiEnumValueAnnotationResult>
            ReadEnumValueAnnotations(FieldInfo clrField)
            => testCase switch
            {
                InvalidCapabilityCase.EnumValueNullList => null!,
                InvalidCapabilityCase.EnumValueNullRow => [null!],
                _ => []
            };

        public IReadOnlyList<ApiKeyAnnotationResult> ReadKeyAnnotations(Type clrType)
            => testCase switch
            {
                InvalidCapabilityCase.KeyNullList => null!,
                InvalidCapabilityCase.KeyNullRow => [null!],
                _ => []
            };

        public IReadOnlyList<ApiOneToManyRelationshipAnnotationResult> ReadOneToManyRelationships
        (
            Type clrType
        )
            => testCase switch
            {
                InvalidCapabilityCase.RelationshipNullList => null!,
                InvalidCapabilityCase.RelationshipNullRow => [null!],
                _ => []
            };

        public IReadOnlyList<ApiOneToOneRelationshipAnnotationResult> ReadOneToOneRelationships
        (
            Type clrType
        ) => [];

        public IReadOnlyList<ApiManyToManyRelationshipAnnotationResult> ReadManyToManyRelationships
        (
            Type clrType
        ) => [];
    }
    #endregion

    #region Helper Methods
    private static ApiSchemaBuilder BuildMetadataSchema(IApiTypeAnnotationReader reader)
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddObject<MetadataObject>(builder => builder
                .AddProperty("Id", nameof(MetadataObject.Id)))
            .AddScalar<int>()
            .UseAnnotations(annotations => annotations.AddReader(reader));
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BoundaryTheoryData =>
    [
        new BoundaryTest { Name = "Annotation reader set implementation remains internal" }
    ];

    public static TheoryDataRow<IXUnitTest>[] CapabilityTheoryData =>
    [
        new CapabilityTest
        {
            Name = "Custom type reader configures an object type",
            TestCase = CapabilityTestCase.Type
        },
        new CapabilityTest
        {
            Name = "Custom property reader configures a property",
            TestCase = CapabilityTestCase.ObjectProperty
        },
        new CapabilityTest
        {
            Name = "Custom enum-value reader configures an enum value",
            TestCase = CapabilityTestCase.EnumValue
        },
        new CapabilityTest
        {
            Name = "Custom key reader configures a key",
            TestCase = CapabilityTestCase.Key
        },
        new CapabilityTest
        {
            Name = "Custom relationship reader configures a relationship",
            TestCase = CapabilityTestCase.Relationship
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] ConfigurationTheoryData =>
    [
        new ConfigurationTest
        {
            Name = "Repeated annotation configuration calls preserve duplicate reader registration",
            TestCase = ConfigurationTestCase.AdditiveCallsPreserveReaders,
            ApiNameExpected = "CustomName",
            ReaderCallsExpected = 2
        },
        new ConfigurationTest
        {
            Name = "A failed annotation configuration callback preserves the existing readers",
            TestCase = ConfigurationTestCase.CallbackFailurePreservesExistingReaders,
            ApiNameExpected = "CustomName",
            ReaderCallsExpected = 1
        },
        new ConfigurationTest
        {
            Name =
                "A custom reader registered before the default reader is " +
                "overridden by the default reader",
            TestCase = ConfigurationTestCase.CustomThenDefault,
            ApiNameExpected = "BuiltInName",
            ReaderCallsExpected = 1
        },
        new ConfigurationTest
        {
            Name = "A custom reader registered after the default reader takes precedence",
            TestCase = ConfigurationTestCase.DefaultThenCustom,
            ApiNameExpected = "CustomName",
            ReaderCallsExpected = 1
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] DiscoveryValidationTheoryData =>
    [
        new DiscoveryValidationTest
        {
            Name = "Valid discovery contributions are accepted",
            TestCase = DiscoveryValidationCase.Valid
        },
        new DiscoveryValidationTest
        {
            Name = "A null discovery wrapper becomes an invalid contribution issue",
            TestCase = DiscoveryValidationCase.NullWrapper
        },
        new DiscoveryValidationTest
        {
            Name = "Null discovery contributions become an invalid contribution issue",
            TestCase = DiscoveryValidationCase.NullContributions
        },
        new DiscoveryValidationTest
        {
            Name = "Null discovery diagnostics become an invalid contribution issue",
            TestCase = DiscoveryValidationCase.NullDiagnostics
        },
        new DiscoveryValidationTest
        {
            Name = "Null discovery rows do not prevent valid rows from being processed",
            TestCase = DiscoveryValidationCase.NullResultRow
        },
        new DiscoveryValidationTest
        {
            Name = "Null discovery CLR types do not prevent valid rows from being processed",
            TestCase = DiscoveryValidationCase.NullClrType
        },
        new DiscoveryValidationTest
        {
            Name = "Ineligible discovery CLR types become invalid contribution issues",
            TestCase = DiscoveryValidationCase.IneligibleType
        },
        new DiscoveryValidationTest
        {
            Name = "Invalid discovery API kinds become invalid contribution issues",
            TestCase = DiscoveryValidationCase.InvalidApiKind
        },
        new DiscoveryValidationTest
        {
            Name = "Invalid discovery API kinds do not prevent valid rows from being processed",
            TestCase = DiscoveryValidationCase.InvalidApiKindThenValid
        },
        new DiscoveryValidationTest
        {
            Name = "The discovery filter participates in contribution eligibility",
            TestCase = DiscoveryValidationCase.FilterExcluded
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] InvalidContributionTheoryData =>
    [
        new InvalidContributionTest
        {
            Name = "A null type annotation result list becomes an invalid contribution issue",
            TestCase = InvalidCapabilityCase.TypeNullList
        },
        new InvalidContributionTest
        {
            Name = "A null type annotation result row becomes an invalid contribution issue",
            TestCase = InvalidCapabilityCase.TypeNullRow
        },
        new InvalidContributionTest
        {
            Name = "A null property annotation result list becomes an invalid contribution issue",
            TestCase = InvalidCapabilityCase.PropertyNullList
        },
        new InvalidContributionTest
        {
            Name = "A null property annotation result row becomes an invalid contribution issue",
            TestCase = InvalidCapabilityCase.PropertyNullRow
        },
        new InvalidContributionTest
        {
            Name = "A null enum-value annotation result list becomes an invalid contribution issue",
            TestCase = InvalidCapabilityCase.EnumValueNullList
        },
        new InvalidContributionTest
        {
            Name = "A null enum-value annotation result row becomes an invalid contribution issue",
            TestCase = InvalidCapabilityCase.EnumValueNullRow
        },
        new InvalidContributionTest
        {
            Name = "A null key annotation result list becomes an invalid contribution issue",
            TestCase = InvalidCapabilityCase.KeyNullList
        },
        new InvalidContributionTest
        {
            Name = "A null key annotation result row becomes an invalid contribution issue",
            TestCase = InvalidCapabilityCase.KeyNullRow
        },
        new InvalidContributionTest
        {
            Name =
                "A null relationship annotation result list becomes an invalid contribution issue",
            TestCase = InvalidCapabilityCase.RelationshipNullList
        },
        new InvalidContributionTest
        {
            Name =
                "A null relationship annotation result row becomes an invalid contribution issue",
            TestCase = InvalidCapabilityCase.RelationshipNullRow
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] IssueMetadataTheoryData =>
    [
        new IssueMetadataTest
        {
            Name = "Reader execution issues retain reader type and exception metadata",
            TestCase = IssueMetadataTestCase.ExecutionMetadata
        },
        new IssueMetadataTest
        {
            Name = "Reader diagnostics retain reader type and exception metadata",
            TestCase = IssueMetadataTestCase.DiagnosticMetadata
        },
        new IssueMetadataTest
        {
            Name = "Reader issues accumulate across repeated builds",
            TestCase = IssueMetadataTestCase.PersistentIssueHistory
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] MarkerConflictTheoryData =>
    [
        new MarkerConflictTest
        {
            Name = "Conflicting built-in type markers are diagnosed while valid types continue"
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BoundaryTheoryData))]
    public void Boundary(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(CapabilityTheoryData))]
    public void Capability(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ConfigurationTheoryData))]
    public void Configuration(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(DiscoveryValidationTheoryData))]
    public void DiscoveryValidation(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(InvalidContributionTheoryData))]
    public void InvalidContribution(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(IssueMetadataTheoryData))]
    public void IssueMetadata(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(MarkerConflictTheoryData))]
    public void MarkerConflict(IXUnitTest test) => test.Execute(this);
    #endregion
}

#region Test Types
[ApiObject(ApiName = "BuiltInName")]
public sealed class AnnotationConfigurationObject
{
    public int Id { get; set; }
}

[ApiObject(ApiName = "BuiltInName")]
public sealed class DefaultAnnotationConfigurationObject
{
    public int Id { get; set; }
}

public sealed class DiscoveryValidType
{
    public int Id { get; set; }
}

public sealed class DiscoverySecondValidType
{
}

[ApiObject]
[ApiScalar]
public sealed class ConflictingMarkerType
{
}

[ApiObject]
public sealed class ValidMarkerType
{
}

public sealed class MetadataObject
{
    public int Id { get; set; }
}

public sealed class CapabilityObject
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
}

public enum CapabilityEnum
{
    First,
    Second
}

public sealed class CapabilityPrincipal
{
    public int Id { get; set; }
}

public sealed class CapabilityDependent
{
    public int Id { get; set; }
    public int PrincipalId { get; set; }
}
#endregion
