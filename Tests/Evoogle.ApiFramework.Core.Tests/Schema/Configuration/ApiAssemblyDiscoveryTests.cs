// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Configuration.Conventions;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed class ApiAssemblyDiscoveryTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private enum PipelineDiscoveryKind
    {
        TypeInference,
        Annotation,
        Configuration
    }

    private sealed class PipelineFilterFailureTest : XUnitTest
    {
        #region User Supplied Properties
        public required PipelineDiscoveryKind DiscoveryKind { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaInitializationException? ExceptionActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"DiscoveryKind: {this.DiscoveryKind}");
            this.WriteLine();
        }

        protected override void Act()
        {
            var assembly = typeof(AssemblyFilterFailureCandidate).Assembly;
            var builder = new ApiSchemaBuilder().WithName("Test");

            try
            {
                switch (this.DiscoveryKind)
                {
                    case PipelineDiscoveryKind.TypeInference:
                        builder.UseAssemblyTypeInference(assembly, ThrowForCandidate);
                        break;
                    case PipelineDiscoveryKind.Annotation:
                        builder
                            .UseDefaultAnnotations()
                            .UseAssemblyAnnotationScanning(assembly, ThrowForCandidate);
                        break;
                    case PipelineDiscoveryKind.Configuration:
                        builder.UseConfigurationsFromAssembly(assembly, ThrowForCandidate);
                        break;
                    default:
                        throw new InvalidOperationException
                        (
                            $"Unknown discovery kind: {this.DiscoveryKind}"
                        );
                }

                _ = builder.Build();
            }
            catch (ApiSchemaInitializationException exception)
            {
                this.ExceptionActual = exception;
            }
        }

        protected override void Assert()
        {
            this.ExceptionActual.Should().NotBeNull();
            this.ExceptionActual!.Errors.Should().Contain
            (
                issue => issue.Code == ApiInitializationCode.ApiAssemblyDiscoveryFailed &&
                    issue.Severity == ApiInitializationSeverity.Error &&
                    issue.ApiPath == typeof(AssemblyFilterFailureCandidate).FullName &&
                    issue.Exception is InvalidOperationException
            );
        }
        #endregion

        #region Private Methods
        private static bool ThrowForCandidate(Type type)
        {
            if (type == typeof(AssemblyFilterFailureCandidate))
            {
                throw new InvalidOperationException("Assembly filter failure.");
            }

            return false;
        }
        #endregion
    }

    private sealed class ReflectionTypeLoadFailureTest : XUnitTest
    {
        #region Calculated Properties
        private ApiAssemblyTypeScanResult? ScanActual { get; set; }
        private ReflectionTypeLoadException? ExceptionExpected { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            this.ExceptionExpected = new ReflectionTypeLoadException
            (
                [typeof(AssemblyLoadableCandidate), null!],
                [new TypeLoadException("Assembly type could not be loaded.")]
            );

            this.ScanActual = ApiAssemblyTypeScanner.Scan
            (
                typeof(AssemblyLoadableCandidate).Assembly,
                filter: null,
                getExportedTypes: _ => throw this.ExceptionExpected
            );
        }

        protected override void Assert()
        {
            this.ScanActual.Should().NotBeNull();
            this.ScanActual!.Types.Should().Equal(typeof(AssemblyLoadableCandidate));

            this.ScanActual.Issues.Should().ContainSingle();
            var issue = this.ScanActual.Issues.Single();
            issue.Code.Should().Be(ApiInitializationCode.ApiAssemblyDiscoveryFailed);
            issue.Severity.Should().Be(ApiInitializationSeverity.Error);
            issue.Exception.Should().BeSameAs(this.ExceptionExpected);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] PipelineFilterFailureTheoryData =>
    [
        new PipelineFilterFailureTest
        {
            Name = "Type inference filter failures become initialization issues",
            DiscoveryKind = PipelineDiscoveryKind.TypeInference
        },
        new PipelineFilterFailureTest
        {
            Name = "Annotation discovery filter failures become initialization issues",
            DiscoveryKind = PipelineDiscoveryKind.Annotation
        },
        new PipelineFilterFailureTest
        {
            Name = "Configuration discovery filter failures become initialization issues",
            DiscoveryKind = PipelineDiscoveryKind.Configuration
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] ReflectionTypeLoadFailureTheoryData =>
    [
        new ReflectionTypeLoadFailureTest
        {
            Name = "Reflection type-load failures preserve loadable types and become issues"
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(PipelineFilterFailureTheoryData))]
    public void PipelineFilterFailure(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ReflectionTypeLoadFailureTheoryData))]
    public void ReflectionTypeLoadFailure(IXUnitTest test) => test.Execute(this);
    #endregion
}

#region Test Types
public sealed class AssemblyFilterFailureCandidate
{
}

public sealed class AssemblyLoadableCandidate
{
}
#endregion
