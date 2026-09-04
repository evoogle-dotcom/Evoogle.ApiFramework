// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Trace;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed class ApiSchemaBuilderTraceTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private sealed class ValidCompilationTraceTest : XUnitTest
    {
        private ApiSchemaCompilationResult? Result { get; set; }

        private ApiInMemorySchemaBuildTraceSink? TraceSink { get; set; }

        protected override void Arrange()
        {
            this.TraceSink = new ApiInMemorySchemaBuildTraceSink();
        }

        protected override void Act()
        {
            this.Result = new ApiSchemaBuilder()
                .WithName("Trace")
                .BuildResult(this.TraceSink!);
        }

        protected override void Assert()
        {
            this.Result!.IsValid.Should().BeTrue();

            var events = this.TraceSink!.CreateTrace().Events;
            var terminalEventKinds = events
                .Select(GetEventKind)
                .Where(eventKind => eventKind is not null)
                .Cast<string>();

            terminalEventKinds.Should().ContainInOrder
            (
                "Compilation Started",
                "Compilation Completed",
                "Freezing Started",
                "Freezing Completed",
                "Build Completed"
            );
        }

        private static string? GetEventKind(ApiSchemaBuildTraceEvent traceEvent)
        {
            return traceEvent switch
            {
                ApiSchemaBuildPhaseStartedEvent { Phase: ApiSchemaBuildPhase.Compilation } =>
                    "Compilation Started",
                ApiSchemaBuildPhaseCompletedEvent { Phase: ApiSchemaBuildPhase.Compilation } =>
                    "Compilation Completed",
                ApiSchemaBuildPhaseStartedEvent { Phase: ApiSchemaBuildPhase.Freezing } =>
                    "Freezing Started",
                ApiSchemaBuildPhaseCompletedEvent { Phase: ApiSchemaBuildPhase.Freezing } =>
                    "Freezing Completed",
                ApiSchemaBuildCompletedEvent => "Build Completed",
                _ => null,
            };
        }
    }

    private sealed class InvalidCompilationTraceTest : XUnitTest
    {
        private ApiSchemaCompilationResult? Result { get; set; }

        private ApiInMemorySchemaBuildTraceSink? TraceSink { get; set; }

        protected override void Arrange()
        {
            this.TraceSink = new ApiInMemorySchemaBuildTraceSink();
        }

        protected override void Act()
        {
            this.Result = new ApiSchemaBuilder().BuildResult(this.TraceSink!);
        }

        protected override void Assert()
        {
            this.Result!.IsValid.Should().BeFalse();

            var events = this.TraceSink!.CreateTrace().Events;
            events.OfType<ApiSchemaBuildPhaseStartedEvent>().Should().ContainSingle
            (
                traceEvent => traceEvent.Phase == ApiSchemaBuildPhase.Compilation
            );
            events.OfType<ApiSchemaBuildPhaseCompletedEvent>().Should().NotContain
            (
                traceEvent => traceEvent.Phase == ApiSchemaBuildPhase.Compilation
            );
            events.OfType<ApiSchemaBuildPhaseStartedEvent>().Should().NotContain
            (
                traceEvent => traceEvent.Phase == ApiSchemaBuildPhase.Freezing
            );
            events.OfType<ApiSchemaBuildPhaseCompletedEvent>().Should().NotContain
            (
                traceEvent => traceEvent.Phase == ApiSchemaBuildPhase.Freezing
            );
            events.Last().Should().BeOfType<ApiSchemaBuildFailedEvent>();
        }
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TraceTheoryData =>
    [
        new ValidCompilationTraceTest
        {
            Name = "Successful compilation completes before freezing"
        },
        new InvalidCompilationTraceTest
        {
            Name = "Invalid compilation fails without freezing"
        }
    ];
    #endregion

    #region Tests
    [Theory]
    [MemberData(nameof(TraceTheoryData))]
    public void BuildResultEmitsExpectedTrace(IXUnitTest test) => test.Execute(this);
    #endregion
}
