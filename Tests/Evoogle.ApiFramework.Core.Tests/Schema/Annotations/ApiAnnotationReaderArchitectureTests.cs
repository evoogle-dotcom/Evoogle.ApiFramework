// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Configuration;
using Evoogle.ApiFramework.Schema.Configuration.Annotations;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Annotations;

public sealed class ApiAnnotationReaderArchitectureTests(ITestOutputHelper output)
    : XUnitTests(output)
{
    #region Test Classes
    private sealed class TypeNameTest : XUnitTest
    {
        #region User Supplied Properties
        public string? ExplicitApiName { get; init; }
        public required string[] AnnotationApiNames { get; init; }
        public required string ApiNameExpected { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaBuilder? ApiSchemaBuilder { get; set; }
        private ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchemaBuilder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddObject<ExplicitNameAnnotationType>(builder =>
                {
                    if (this.ExplicitApiName is not null)
                    {
                        builder.WithName(this.ExplicitApiName);
                    }

                    builder.AddProperty("Id", nameof(ExplicitNameAnnotationType.Id));
                })
                .AddScalar<int>()
                .UseAnnotations
                (
                    annotations =>
                    {
                        foreach (var annotationApiName in this.AnnotationApiNames)
                        {
                            annotations.AddReader(new TypeNameReader(annotationApiName));
                        }
                    }
                );

            this.ApiSchemaBuilder = apiSchemaBuilder;

            this.WriteLine($"ExplicitApiName: {this.ExplicitApiName ?? "(none)"}");
            this.WriteLine($"AnnotationApiNames: {string.Join(", ", this.AnnotationApiNames)}");
            this.WriteLine($"ApiNameExpected: {this.ApiNameExpected}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ApiSchemaActual = this.ApiSchemaBuilder!.Build();
            this.WriteLine($"ApiSchemaActual: {this.ApiSchemaActual.ApiName}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();
            this.ApiSchemaActual!.ApiObjectTypes.Single().ApiName.Should().Be(this.ApiNameExpected);
        }
        #endregion
    }

    private enum KeyPathTestCase
    {
        AnnotationKeyPathsAreSorted,
        ExplicitKeyIsPreserved
    }

    private sealed class KeyPathTest : XUnitTest
    {
        #region User Supplied Properties
        public required KeyPathTestCase TestCase { get; init; }
        public required string[] ApiKeyPathPropertyNamesExpected { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaBuilder? ApiSchemaBuilder { get; set; }
        private ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchemaBuilder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddScalar<int>();

            this.ApiSchemaBuilder = this.TestCase switch
            {
                KeyPathTestCase.AnnotationKeyPathsAreSorted => apiSchemaBuilder
                    .AddObject<ReverseOrderKeyType>(builder => builder
                        .AddProperty("Second", nameof(ReverseOrderKeyType.Second))
                        .AddProperty("First", nameof(ReverseOrderKeyType.First))),
                KeyPathTestCase.ExplicitKeyIsPreserved => apiSchemaBuilder
                    .AddObject<ExplicitKeyOverrideType>(builder => builder
                        .AddProperty("Explicit", nameof(ExplicitKeyOverrideType.Explicit))
                        .AddProperty("Annotation", nameof(ExplicitKeyOverrideType.Annotation))
                        .AddKey("Shared", key => key.AddPath
                        (
                            typeof(ExplicitKeyOverrideType),
                            nameof(ExplicitKeyOverrideType.Explicit)
                        ))),
                _ => throw new InvalidOperationException($"Unknown test case: {this.TestCase}")
            };

            this.ApiSchemaBuilder.UseDefaultAnnotations();

            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine
            (
                $"ApiKeyPathPropertyNamesExpected: " +
                string.Join(", ", this.ApiKeyPathPropertyNamesExpected)
            );
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ApiSchemaActual = this.ApiSchemaBuilder!.Build();
            this.WriteLine($"ApiSchemaActual: {this.ApiSchemaActual.ApiName}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();

            var apiKeyPaths = this.ApiSchemaActual!.ApiObjectTypes.Single().ApiKeyTypes!
                .Single().ApiKeyPaths;
            apiKeyPaths.Select(path => path.ApiSegments.Single().ClrPropertyName)
                .Should().Equal(this.ApiKeyPathPropertyNamesExpected);
        }
        #endregion
    }

    private enum ExceptionTestCase
    {
        DuplicateAnnotationKeyOrders,
        MarkerOnlyAnnotationReader
    }

    private sealed class ExceptionTest : XUnitTest
    {
        #region User Supplied Properties
        public required ExceptionTestCase TestCase { get; init; }
        public required Type ExceptionTypeExpected { get; init; }
        public ApiInitializationCode? InitializationCodeExpected { get; init; }
        #endregion

        #region Calculated Properties
        private Exception? ExceptionActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine($"ExceptionTypeExpected: {this.ExceptionTypeExpected.Name}");
            this.WriteLine($"InitializationCodeExpected: {this.InitializationCodeExpected}");
            this.WriteLine();
        }

        protected override void Act()
        {
            try
            {
                _ = this.TestCase switch
                {
                    ExceptionTestCase.DuplicateAnnotationKeyOrders => (object)new ApiSchemaBuilder()
                        .WithName("Test")
                        .AddScalar<int>()
                        .AddObject<DuplicateOrderKeyType>(builder => builder
                            .AddProperty("First", nameof(DuplicateOrderKeyType.First))
                            .AddProperty("Second", nameof(DuplicateOrderKeyType.Second)))
                        .UseDefaultAnnotations()
                        .Build(),
                    ExceptionTestCase.MarkerOnlyAnnotationReader => (object)new ApiAnnotationReaderSetBuilder().AddReader(new MarkerOnlyReader()),
                    _ => throw new InvalidOperationException($"Unknown test case: {this.TestCase}")
                };
            }
            catch (Exception exception)
            {
                this.ExceptionActual = exception;
                this.WriteLine
                (
                    $"ExceptionActual: {exception.GetType().Name} - {exception.Message}"
                );
            }
        }

        protected override void Assert()
        {
            this.ExceptionActual.Should().NotBeNull();
            this.ExceptionActual.Should().BeOfType(this.ExceptionTypeExpected);

            if (this.InitializationCodeExpected is not { } initializationCodeExpected)
            {
                return;
            }

            var exception = (ApiSchemaInitializationException)this.ExceptionActual!;
            exception.Errors.Select(issue => issue.Code)
                .Should().Contain(initializationCodeExpected);
        }
        #endregion
    }

    private enum DiscoveryTestCase
    {
        CustomReader,
        DefaultAttributeReader,
        NoDiscoveryReader
    }

    private sealed class DiscoveryTest : XUnitTest
    {
        #region User Supplied Properties
        public required DiscoveryTestCase TestCase { get; init; }
        public required Type ClrTypeExpected { get; init; }
        public required bool IsExpected { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaBuilder? ApiSchemaBuilder { get; set; }
        private ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchemaBuilder = new ApiSchemaBuilder()
                .WithName("Test")
                .AddScalar<int>()
                .UsePropertyDiscovery();

            switch (this.TestCase)
            {
                case DiscoveryTestCase.CustomReader:
                    apiSchemaBuilder.UseAnnotations
                    (
                        annotations => annotations.AddReader(new TypeDiscoveryReader())
                    );
                    break;
                case DiscoveryTestCase.DefaultAttributeReader:
                    apiSchemaBuilder.UseDefaultAnnotations();
                    break;
                case DiscoveryTestCase.NoDiscoveryReader:
                    break;
                default:
                    throw new InvalidOperationException($"Unknown test case: {this.TestCase}");
            }

            this.ApiSchemaBuilder = apiSchemaBuilder.UseAssemblyAnnotationScanning
                (
                    typeof(ApiAnnotationReaderArchitectureTests).Assembly,
                    type => type == this.ClrTypeExpected
                );

            this.WriteLine($"TestCase: {this.TestCase}");
            this.WriteLine($"ClrTypeExpected: {this.ClrTypeExpected.Name}");
            this.WriteLine($"IsExpected: {this.IsExpected}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ApiSchemaActual = this.ApiSchemaBuilder!.Build();
            this.WriteLine($"ApiSchemaActual: {this.ApiSchemaActual.ApiName}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();

            var apiObjectTypes = this.ApiSchemaActual!.ApiObjectTypes.Select(type => type.ClrType);
            if (this.IsExpected)
            {
                apiObjectTypes.Should().Contain(this.ClrTypeExpected);
            }
            else
            {
                apiObjectTypes.Should().NotContain(this.ClrTypeExpected);
            }
        }
        #endregion
    }
    #endregion

    #region Test Readers
    private sealed class TypeNameReader(string apiName) : IApiTypeAnnotationReader
    {
        public IReadOnlyList<ApiTypeAnnotationResult> ReadObjectTypeAnnotations(Type clrType)
            => clrType == typeof(ExplicitNameAnnotationType) ? [new(apiName)] : [];

        public IReadOnlyList<ApiTypeAnnotationResult> ReadScalarTypeAnnotations(Type clrType) => [];

        public IReadOnlyList<ApiTypeAnnotationResult> ReadEnumTypeAnnotations(Type clrType) => [];
    }

    private sealed class TypeDiscoveryReader : IApiTypeDiscoveryAnnotationReader
    {
        public IReadOnlyList<ApiTypeDiscoveryAnnotationResult> ReadTypeDiscoveryAnnotations
        (
            Assembly assembly,
            Func<Type, bool>? filter
        )
            => [new(typeof(DiscoveredAnnotationType), ApiTypeKind.Object)];
    }

    private sealed class MarkerOnlyReader : IApiAnnotationReader
    {
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TypeNameTheoryData =>
    [
        new TypeNameTest
        {
            Name = "Explicit type name takes precedence over annotation type name",
            ExplicitApiName = "ExplicitName",
            AnnotationApiNames = ["AnnotationName"],
            ApiNameExpected = "ExplicitName"
        },
        new TypeNameTest
        {
            Name = "Later type annotation reader takes precedence over earlier reader",
            AnnotationApiNames = ["EarlierName", "LaterName"],
            ApiNameExpected = "LaterName"
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] KeyPathTheoryData =>
    [
        new KeyPathTest
        {
            Name = "Annotation key paths are sorted across members before application",
            TestCase = KeyPathTestCase.AnnotationKeyPathsAreSorted,
            ApiKeyPathPropertyNamesExpected =
            [
                nameof(ReverseOrderKeyType.First),
                nameof(ReverseOrderKeyType.Second)
            ]
        },
        new KeyPathTest
        {
            Name = "Explicit key is not replaced by annotation key",
            TestCase = KeyPathTestCase.ExplicitKeyIsPreserved,
            ApiKeyPathPropertyNamesExpected = [nameof(ExplicitKeyOverrideType.Explicit)]
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] ExceptionTheoryData =>
    [
        new ExceptionTest
        {
            Name = "Duplicate annotation key orders become initialization issues",
            TestCase = ExceptionTestCase.DuplicateAnnotationKeyOrders,
            ExceptionTypeExpected = typeof(ApiSchemaInitializationException),
            InitializationCodeExpected = ApiInitializationCode.ApiAnnotationKeyOrderConflict
        },
        new ExceptionTest
        {
            Name = "Marker-only annotation reader is rejected",
            TestCase = ExceptionTestCase.MarkerOnlyAnnotationReader,
            ExceptionTypeExpected = typeof(ArgumentException)
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] DiscoveryTheoryData =>
    [
        new DiscoveryTest
        {
            Name = "Custom discovery reader participates in assembly scanning",
            TestCase = DiscoveryTestCase.CustomReader,
            ClrTypeExpected = typeof(DiscoveredAnnotationType),
            IsExpected = true
        },
        new DiscoveryTest
        {
            Name = "Default attribute reader participates in assembly scanning",
            TestCase = DiscoveryTestCase.DefaultAttributeReader,
            ClrTypeExpected = typeof(BuiltInDiscoveredAnnotationType),
            IsExpected = true
        },
        new DiscoveryTest
        {
            Name = "Assembly scanning without a discovery reader ignores built-in attributes",
            TestCase = DiscoveryTestCase.NoDiscoveryReader,
            ClrTypeExpected = typeof(BuiltInDiscoveredAnnotationType),
            IsExpected = false
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(TypeNameTheoryData))]
    public void TypeName(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(KeyPathTheoryData))]
    public void KeyPath(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ExceptionTheoryData))]
    public void Exception(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(DiscoveryTheoryData))]
    public void Discovery(IXUnitTest test) => test.Execute(this);
    #endregion
}

#region Test Types
public sealed class ExplicitNameAnnotationType
{
    public int Id { get; set; }
}

public sealed class ReverseOrderKeyType
{
    [ApiKey(ApiName = "Composite", Order = 1)]
    public int Second { get; set; }

    [ApiKey(ApiName = "Composite", Order = 0)]
    public int First { get; set; }
}

public sealed class DuplicateOrderKeyType
{
    [ApiKey(ApiName = "Duplicate", Order = 0)]
    public int First { get; set; }

    [ApiKey(ApiName = "Duplicate", Order = 0)]
    public int Second { get; set; }
}

public sealed class ExplicitKeyOverrideType
{
    [ApiKey(ApiName = "Shared", Order = 0)]
    public int Annotation { get; set; }

    public int Explicit { get; set; }
}

public sealed class DiscoveredAnnotationType
{
    public int Id { get; set; }
}

[ApiObject]
public sealed class BuiltInDiscoveredAnnotationType
{
    public int Id { get; set; }
}
#endregion
