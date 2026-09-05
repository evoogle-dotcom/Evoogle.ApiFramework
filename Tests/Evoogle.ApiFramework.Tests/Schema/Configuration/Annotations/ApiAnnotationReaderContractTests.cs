// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema;
using Evoogle.ApiFramework.Schema.Configuration;
using Evoogle.ApiFramework.Schema.Configuration.Annotations.Internal;
using Evoogle.Reflection;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

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
        private IReadOnlyList<ApiSchemaCompilationIssue>? IssuesActual { get; set; }
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
                issue => issue.Code == ApiSchemaCompilationCode.ApiAnnotationInvalidContribution
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

    private enum MarkerConflictCase
    {
        Discovery,
        ExplicitObject,
        ExplicitScalar
    }

    private sealed class MarkerConflictTest : XUnitTest
    {
        #region User Supplied Properties
        public required MarkerConflictCase TestCase { get; init; }
        #endregion

        #region Calculated Properties
        private IReadOnlyList<ApiTypeDiscoveryAnnotationResult>? ResultsActual { get; set; }
        private IReadOnlyList<ApiSchemaCompilationIssue>? IssuesActual { get; set; }
        private ApiSchemaCompilationException? ExceptionActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine();
        }

        protected override void Act()
        {
            if (this.TestCase == MarkerConflictCase.Discovery)
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
                return;
            }

            var builder = new ApiSchemaBuilder()
                .WithName("Test")
                .UseDefaultAnnotations();
            if (this.TestCase == MarkerConflictCase.ExplicitObject)
            {
                builder.AddObject<ConflictingMarkerType>();
            }
            else
            {
                builder.AddScalar<ConflictingMarkerType>();
            }

            try
            {
                _ = builder.Build();
            }
            catch (ApiSchemaCompilationException exception)
            {
                this.ExceptionActual = exception;
            }
        }

        protected override void Assert()
        {
            if (this.TestCase == MarkerConflictCase.Discovery)
            {
                this.ResultsActual!.Select(result => result.ClrType)
                    .Should().Contain(typeof(ValidMarkerType));
                this.ResultsActual!.Select(result => result.ClrType)
                    .Should().NotContain(typeof(ConflictingMarkerType));

                var issue = this.IssuesActual!.Single
                (
                    issue => issue.Code == ApiSchemaCompilationCode.ApiAnnotationTypeMarkerConflict
                );
                issue.ReaderType.Should().Be(typeof(ApiAttributeAnnotationReader));
                issue.Description.Should().Contain("both");
                return;
            }

            this.ExceptionActual.Should().NotBeNull();
            var explicitIssue = this.ExceptionActual!.Errors.Single
            (
                issue => issue.Code == ApiSchemaCompilationCode.ApiAnnotationTypeMarkerConflict
            );
            explicitIssue.ReaderType.Should().Be(typeof(ApiAttributeAnnotationReader));
            explicitIssue.Description.Should().Contain("both");
        }
        #endregion
    }

    private enum DiscoveryConflictCase
    {
        ObjectThenScalar,
        ScalarThenObject
    }

    private sealed class DiscoveryConflictTest : XUnitTest
    {
        #region User Supplied Properties
        public required DiscoveryConflictCase TestCase { get; init; }
        public required ApiTypeKind ApiKindExpected { get; init; }
        #endregion

        #region Calculated Properties
        private IReadOnlyList<ApiTypeDiscoveryAnnotationResult>? ResultsActual { get; set; }
        private IReadOnlyList<ApiSchemaCompilationIssue>? IssuesActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine($"ApiKindExpected: {this.ApiKindExpected}");
            this.WriteLine();
        }

        protected override void Act()
        {
            var objectReader = new DiscoveryTypeReader(ApiTypeKind.Object);
            var scalarReader = new DiscoveryTypeReader(ApiTypeKind.Scalar);
            var readerSet = this.TestCase == DiscoveryConflictCase.ObjectThenScalar
                ? new ApiAnnotationReaderSetBuilder()
                    .AddReader(objectReader)
                    .AddReader(scalarReader)
                    .Build()
                : new ApiAnnotationReaderSetBuilder()
                    .AddReader(scalarReader)
                    .AddReader(objectReader)
                    .Build();

            this.ResultsActual = readerSet.ReadTypeDiscoveryAnnotations
            (
                typeof(ApiAnnotationReaderContractTests).Assembly,
                static type => type == typeof(CrossReaderDiscoveryType)
            );
            this.IssuesActual = readerSet.Issues;
        }

        protected override void Assert()
        {
            this.ResultsActual.Should().ContainSingle();
            var result = this.ResultsActual!.Single();
            result.ClrType.Should().Be(typeof(CrossReaderDiscoveryType));
            result.ApiKind.Should().Be(this.ApiKindExpected);

            var issue = this.IssuesActual!.Single
            (
                issue => issue.Code == ApiSchemaCompilationCode.ApiAnnotationTypeDiscoveryConflict
            );
            issue.ReaderType.Should().Be(typeof(DiscoveryTypeReader));
            issue.Description.Should().Contain
            (
                this.ApiKindExpected == ApiTypeKind.Object ? "Object" : "Scalar"
            );
        }
        #endregion
    }

    private sealed class DiscoveryRegistrationTest : XUnitTest
    {
        #region Calculated Properties
        private ApiSchemaBuilder? ApiSchemaBuilder { get; set; }
        private IReadOnlyList<string>? ReaderOrderActual { get; set; }
        private ApiSchema? ApiSchemaActual { get; set; }
        private Exception? ExceptionActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var readerOrder = new List<string>();
            var firstReader = new OrderedDiscoveryReader("First", readerOrder);
            var secondReader = new OrderedDiscoveryReader("Second", readerOrder);
            this.ReaderOrderActual = readerOrder;
            this.ApiSchemaBuilder = new ApiSchemaBuilder()
                .WithName("Test")
                .UseAnnotations(annotations => annotations
                    .AddReader(firstReader)
                    .AddReader(secondReader)
                    .AddReader(firstReader))
                .UseAssemblyAnnotationScanning
                (
                    typeof(ApiAnnotationReaderContractTests).Assembly,
                    static type => type == typeof(DiscoveryRegistrationType)
                );
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
            this.ReaderOrderActual.Should().Equal("First", "Second", "First");
            this.ApiSchemaActual!.ApiObjectTypes
                .Count(type => type.ClrType == typeof(DiscoveryRegistrationType))
                .Should().Be(1);
        }
        #endregion

    }

    private enum IssueMetadataTestCase
    {
        DiagnosticMetadata,
        InvalidDiagnosticCode,
        DiagnosticSeverity,
        ExecutionMetadata,
        PersistentIssueHistory
    }

    private sealed class IssueMetadataTest : XUnitTest
    {
        #region User Supplied Properties
        public required IssueMetadataTestCase TestCase { get; init; }
        public ApiSchemaCompilationSeverity SeverityExpected { get; init; } =
            ApiSchemaCompilationSeverity.Error;
        #endregion

        #region Calculated Properties
        private ApiSchemaCompilationException? ExceptionActual { get; set; }
        private ApiSchemaCompilationException? FirstExceptionActual { get; set; }
        private ApiSchemaCompilationException? SecondExceptionActual { get; set; }
        private IReadOnlyList<ApiSchemaCompilationIssue>? IssuesActual { get; set; }
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
                case IssueMetadataTestCase.DiagnosticSeverity:
                case IssueMetadataTestCase.InvalidDiagnosticCode:
                    {
                        var readerSet = new ApiAnnotationReaderSetBuilder()
                            .AddReader
                            (
                                new DiagnosticReader
                                (
                                    this.ReaderExceptionExpected!,
                                    this.SeverityExpected,
                                    this.TestCase == IssueMetadataTestCase.InvalidDiagnosticCode
                                        ? (ApiSchemaCompilationCode)999
                                        : ApiSchemaCompilationCode.ApiAnnotationInvalidContribution
                                )
                            )
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
                        catch (ApiSchemaCompilationException exception)
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
                        catch (ApiSchemaCompilationException exception)
                        {
                            this.FirstExceptionActual = exception;
                        }

                        try
                        {
                            _ = builder.Build();
                        }
                        catch (ApiSchemaCompilationException exception)
                        {
                            this.SecondExceptionActual = exception;
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
            if (this.TestCase == IssueMetadataTestCase.InvalidDiagnosticCode)
            {
                this.IssuesActual!.Should().ContainSingle
                (
                    issue => issue.Code == ApiSchemaCompilationCode.ApiAnnotationInvalidContribution &&
                        issue.ReaderType == typeof(DiagnosticReader)
                );
                return;
            }

            if (this.TestCase is IssueMetadataTestCase.DiagnosticMetadata or
                IssueMetadataTestCase.DiagnosticSeverity)
            {
                var issue = this.IssuesActual!.Single();
                issue.ReaderType.Should().Be(typeof(DiagnosticReader));
                issue.Exception.Should().BeSameAs(this.ReaderExceptionExpected);
                issue.Severity.Should().Be(this.SeverityExpected);
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
                this.FirstExceptionActual.Should().NotBeNull();
                this.SecondExceptionActual.Should().NotBeNull();

                var firstErrors = this.FirstExceptionActual!.Errors;
                firstErrors.Count(issue => issue.ReaderType == typeof(ThrowingTypeReader))
                    .Should().Be(1);
                errors.Count(issue => issue.ReaderType == typeof(ThrowingTypeReader))
                    .Should().Be(1);
                errors.Select(issue =>
                    (
                        issue.ApiPath,
                        issue.Severity,
                        issue.Code,
                        issue.Description,
                        issue.Remediation,
                        issue.ReaderType,
                        issue.Exception
                    )).Should().Equal(firstErrors.Select(issue =>
                    (
                        issue.ApiPath,
                        issue.Severity,
                        issue.Code,
                        issue.Description,
                        issue.Remediation,
                        issue.ReaderType,
                        issue.Exception
                    )));
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
        RelationshipManyToManyNullList,
        RelationshipManyToManyNullRow,
        RelationshipNullList,
        RelationshipNullRow,
        RelationshipOneToOneNullList,
        RelationshipOneToOneNullRow,
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
        private ApiSchemaCompilationException? ExceptionActual { get; set; }
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
                InvalidCapabilityCase.RelationshipManyToManyNullList or
                InvalidCapabilityCase.RelationshipManyToManyNullRow or
                InvalidCapabilityCase.RelationshipNullList or
                InvalidCapabilityCase.RelationshipNullRow or
                InvalidCapabilityCase.RelationshipOneToOneNullList or
                InvalidCapabilityCase.RelationshipOneToOneNullRow => builder
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
            catch (ApiSchemaCompilationException exception)
            {
                this.ExceptionActual = exception;
            }
        }

        protected override void Assert()
        {
            this.ExceptionActual.Should().NotBeNull();
            this.ExceptionActual!.Errors
                .Should().Contain(issue =>
                    issue.Code == ApiSchemaCompilationCode.ApiAnnotationInvalidContribution &&
                    issue.ReaderType == typeof(InvalidContributionReader));
        }
        #endregion
    }

    private enum ReaderExecutionCase
    {
        EnumValue,
        Key,
        ManyToMany,
        ObjectProperty,
        OneToMany,
        OneToOne,
        Type
    }

    private sealed class ReaderExecutionTest : XUnitTest
    {
        #region User Supplied Properties
        public required ReaderExecutionCase TestCase { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaBuilder? ApiSchemaBuilder { get; set; }
        private ApiSchemaCompilationException? ExceptionActual { get; set; }
        private Exception? ReaderExceptionExpected { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ReaderExceptionExpected = new InvalidOperationException("reader failure");
            var builder = new ApiSchemaBuilder()
                .WithName("Test")
                .UsePropertyDiscovery()
                .UseEnumValueDiscovery()
                .AddScalar<int>()
                .UseAnnotations(annotations => annotations.AddReader
                (
                    new ThrowingCapabilityReader(this.TestCase, this.ReaderExceptionExpected)
                ));

            this.ApiSchemaBuilder = this.TestCase switch
            {
                ReaderExecutionCase.EnumValue => builder
                    .AddEnum<CapabilityEnum>(enumBuilder => enumBuilder.AddAllValues()),
                ReaderExecutionCase.ManyToMany or
                ReaderExecutionCase.OneToMany or
                ReaderExecutionCase.OneToOne => builder
                    .AddObject<CapabilityPrincipal>()
                    .AddObject<CapabilityDependent>(),
                _ => builder.AddObject<CapabilityObject>()
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
            catch (ApiSchemaCompilationException exception)
            {
                this.ExceptionActual = exception;
            }
        }

        protected override void Assert()
        {
            this.ExceptionActual.Should().NotBeNull();
            this.ExceptionActual!.Errors.Should().Contain
            (
                issue => issue.Code == ApiSchemaCompilationCode.ApiAnnotationReaderExecutionFailed &&
                    issue.ReaderType == typeof(ThrowingCapabilityReader) &&
                    issue.Exception == this.ReaderExceptionExpected
            );
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

    private sealed class DiscoveryTypeReader(ApiTypeKind apiKind) :
        IApiTypeDiscoveryAnnotationReader
    {
        public ApiAnnotationReaderResult<ApiTypeDiscoveryAnnotationResult>
            ReadTypeDiscoveryAnnotations
        (
            Assembly assembly,
            Func<Type, bool>? filter
        )
            => new
            (
                [new(typeof(CrossReaderDiscoveryType), apiKind)],
                []
            );
    }

    private sealed class OrderedDiscoveryReader(string name, ICollection<string> readerOrder) :
        IApiTypeDiscoveryAnnotationReader
    {
        public ApiAnnotationReaderResult<ApiTypeDiscoveryAnnotationResult>
            ReadTypeDiscoveryAnnotations
        (
            Assembly assembly,
            Func<Type, bool>? filter
        )
        {
            readerOrder.Add(name);
            return new
            (
                [new(typeof(DiscoveryRegistrationType), ApiTypeKind.Object)],
                []
            );
        }
    }

    private sealed class DiagnosticReader
    (
        Exception exception,
        ApiSchemaCompilationSeverity severity,
        ApiSchemaCompilationCode code
    ) : IApiTypeDiscoveryAnnotationReader
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
                    code,
                    "DiagnosticPath",
                    "A diagnostic reader reported an invalid contribution.",
                    "Correct the contribution.",
                    exception,
                    severity
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
        )
            => testCase switch
            {
                InvalidCapabilityCase.RelationshipOneToOneNullList => null!,
                InvalidCapabilityCase.RelationshipOneToOneNullRow => [null!],
                _ => []
            };

        public IReadOnlyList<ApiManyToManyRelationshipAnnotationResult> ReadManyToManyRelationships
        (
            Type clrType
        )
            => testCase switch
            {
                InvalidCapabilityCase.RelationshipManyToManyNullList => null!,
                InvalidCapabilityCase.RelationshipManyToManyNullRow => [null!],
                _ => []
            };
    }

    private sealed class ThrowingCapabilityReader
    (
        ReaderExecutionCase testCase,
        Exception exception
    ) :
        IApiTypeAnnotationReader,
        IApiPropertyAnnotationReader,
        IApiEnumValueAnnotationReader,
        IApiKeyAnnotationReader,
        IApiRelationshipAnnotationReader
    {
        public IReadOnlyList<ApiTypeAnnotationResult> ReadObjectTypeAnnotations(Type clrType)
        {
            this.ThrowIfSelected(ReaderExecutionCase.Type);
            return [];
        }

        public IReadOnlyList<ApiTypeAnnotationResult> ReadScalarTypeAnnotations(Type clrType)
        {
            this.ThrowIfSelected(ReaderExecutionCase.Type);
            return [];
        }

        public IReadOnlyList<ApiTypeAnnotationResult> ReadEnumTypeAnnotations(Type clrType)
        {
            this.ThrowIfSelected(ReaderExecutionCase.Type);
            return [];
        }

        public IReadOnlyList<ApiPropertyAnnotationResult> ReadPropertyAnnotations
        (
            MemberInfo clrMember,
            ClrMemberKind clrMemberKind,
            MemberNullableInfo clrNullabilityInfo
        )
        {
            this.ThrowIfSelected(ReaderExecutionCase.ObjectProperty);
            return [];
        }

        public IReadOnlyList<ApiEnumValueAnnotationResult> ReadEnumValueAnnotations(FieldInfo clrField)
        {
            this.ThrowIfSelected(ReaderExecutionCase.EnumValue);
            return [];
        }

        public IReadOnlyList<ApiKeyAnnotationResult> ReadKeyAnnotations(Type clrType)
        {
            this.ThrowIfSelected(ReaderExecutionCase.Key);
            return [];
        }

        public IReadOnlyList<ApiOneToManyRelationshipAnnotationResult> ReadOneToManyRelationships
        (
            Type clrType
        )
        {
            this.ThrowIfSelected(ReaderExecutionCase.OneToMany);
            return [];
        }

        public IReadOnlyList<ApiOneToOneRelationshipAnnotationResult> ReadOneToOneRelationships
        (
            Type clrType
        )
        {
            this.ThrowIfSelected(ReaderExecutionCase.OneToOne);
            return [];
        }

        public IReadOnlyList<ApiManyToManyRelationshipAnnotationResult> ReadManyToManyRelationships
        (
            Type clrType
        )
        {
            this.ThrowIfSelected(ReaderExecutionCase.ManyToMany);
            return [];
        }

        private void ThrowIfSelected(ReaderExecutionCase capability)
        {
            if (testCase == capability)
            {
                throw exception;
            }
        }
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
        },
        new InvalidContributionTest
        {
            Name =
                "A null one-to-one relationship result list becomes an invalid contribution issue",
            TestCase = InvalidCapabilityCase.RelationshipOneToOneNullList
        },
        new InvalidContributionTest
        {
            Name =
                "A null one-to-one relationship result row becomes an invalid contribution issue",
            TestCase = InvalidCapabilityCase.RelationshipOneToOneNullRow
        },
        new InvalidContributionTest
        {
            Name =
                "A null many-to-many relationship result list becomes an invalid " +
                "contribution issue",
            TestCase = InvalidCapabilityCase.RelationshipManyToManyNullList
        },
        new InvalidContributionTest
        {
            Name =
                "A null many-to-many relationship result row becomes an invalid contribution issue",
            TestCase = InvalidCapabilityCase.RelationshipManyToManyNullRow
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
            Name = "Undefined reader diagnostic codes become invalid contribution issues",
            TestCase = IssueMetadataTestCase.InvalidDiagnosticCode
        },
        new IssueMetadataTest
        {
            Name = "Reader diagnostics preserve their configured severity",
            TestCase = IssueMetadataTestCase.DiagnosticSeverity,
            SeverityExpected = ApiSchemaCompilationSeverity.Warning
        },
        new IssueMetadataTest
        {
            Name = "Reader issues reset across repeated builds",
            TestCase = IssueMetadataTestCase.PersistentIssueHistory
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] MarkerConflictTheoryData =>
    [
        new MarkerConflictTest
        {
            Name = "Conflicting built-in type markers are diagnosed during assembly discovery",
            TestCase = MarkerConflictCase.Discovery
        },
        new MarkerConflictTest
        {
            Name = "Conflicting built-in type markers are diagnosed for explicit object types",
            TestCase = MarkerConflictCase.ExplicitObject
        },
        new MarkerConflictTest
        {
            Name = "Conflicting built-in type markers are diagnosed for explicit scalar types",
            TestCase = MarkerConflictCase.ExplicitScalar
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] DiscoveryConflictTheoryData =>
    [
        new DiscoveryConflictTest
        {
            Name = "The first object discovery kind wins over a later scalar conflict",
            TestCase = DiscoveryConflictCase.ObjectThenScalar,
            ApiKindExpected = ApiTypeKind.Object
        },
        new DiscoveryConflictTest
        {
            Name = "The first scalar discovery kind wins over a later object conflict",
            TestCase = DiscoveryConflictCase.ScalarThenObject,
            ApiKindExpected = ApiTypeKind.Scalar
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] DiscoveryRegistrationTheoryData =>
    [
        new DiscoveryRegistrationTest
        {
            Name = "Discovery readers execute in registration order, including duplicates"
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] ReaderExecutionTheoryData =>
    [
        new ReaderExecutionTest
        {
            Name = "Type reader exceptions become reader-attributed compilation issues",
            TestCase = ReaderExecutionCase.Type
        },
        new ReaderExecutionTest
        {
            Name = "Property reader exceptions become reader-attributed compilation issues",
            TestCase = ReaderExecutionCase.ObjectProperty
        },
        new ReaderExecutionTest
        {
            Name = "Enum-value reader exceptions become reader-attributed compilation issues",
            TestCase = ReaderExecutionCase.EnumValue
        },
        new ReaderExecutionTest
        {
            Name = "Key reader exceptions become reader-attributed compilation issues",
            TestCase = ReaderExecutionCase.Key
        },
        new ReaderExecutionTest
        {
            Name = "One-to-many reader exceptions become reader-attributed compilation issues",
            TestCase = ReaderExecutionCase.OneToMany
        },
        new ReaderExecutionTest
        {
            Name = "One-to-one reader exceptions become reader-attributed compilation issues",
            TestCase = ReaderExecutionCase.OneToOne
        },
        new ReaderExecutionTest
        {
            Name = "Many-to-many reader exceptions become reader-attributed compilation issues",
            TestCase = ReaderExecutionCase.ManyToMany
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

    [Theory]
    [MemberData(nameof(DiscoveryConflictTheoryData))]
    public void DiscoveryConflict(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(DiscoveryRegistrationTheoryData))]
    public void DiscoveryRegistration(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ReaderExecutionTheoryData))]
    public void ReaderExecution(IXUnitTest test) => test.Execute(this);
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

public sealed class CrossReaderDiscoveryType
{
    public int Id { get; set; }
}

public sealed class DiscoveryRegistrationType
{
    public int Id { get; set; }
}

[ApiObject]
[ApiScalar]
public sealed class ConflictingMarkerType
{
    public int Id { get; set; }
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
