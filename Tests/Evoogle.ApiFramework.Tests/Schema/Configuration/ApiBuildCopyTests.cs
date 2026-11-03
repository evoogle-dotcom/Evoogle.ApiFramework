// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public sealed class ApiBuildCopyTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class BuildCopyCompletenessTest : XUnitTest
    {
        #region User Supplied Properties
        public required string CopyTypeName { get; init; }
        #endregion

        #region Calculated Properties
        private Type CopyType { get; set; } = null!;
        private string OriginalTestJson { get; set; } = null!;
        private string RestoredTestJson { get; set; } = null!;
        private IReadOnlyList<string> ActualDefects { get; set; } = [];
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.CopyType = typeof(ApiSchemaBuilder).Assembly.GetType(this.CopyTypeName, throwOnError: true)!;

            this.WriteLine($"Scenario:  {this.Name.SafeToString()}");
            this.WriteLine($"Copy Type: {this.CopyType.FullName.SafeToString()}");
            this.WriteLine();
            this.WriteLine("Expected Defects:   None");
            this.WriteLine("Expected Exception: None");

            var serializer = new XUnitTestSerializer();
            this.OriginalTestJson = serializer.Serialize(this);
            this.RestoredTestJson = serializer.Serialize(serializer.Deserialize(this.GetType(), this.OriginalTestJson));
        }

        protected override void Act()
        {
            this.WriteLine();
            try
            {
                this.ActualDefects = ApiBuildCopyProbe.Probe(this.CopyType, this.WriteLine);
            }
            catch (Exception exception)
            {
                this.ActualException = exception;
            }

            this.WriteLine();
            this.WriteLine($"Actual Defect Count: {this.ActualDefects.Count}");
            foreach (var defect in this.ActualDefects)
            {
                this.WriteLine($"Actual Defect:       {defect}");
            }

            this.WriteLine();
            this.WriteLine($"Captured Exception: {this.ActualException?.ToString().SafeToString()}");
        }

        protected override void Assert()
        {
            this.RestoredTestJson.Should().Be(this.OriginalTestJson);
            this.ActualException.Should().BeNull();
            this.ActualDefects.Should().BeEmpty();
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildCopyCompletenessTheoryData =>
    [
        .. ApiBuildCopyProbe.GetCopyableTypes().Select(static type => (TheoryDataRow<IXUnitTest>)new BuildCopyCompletenessTest
        {
            Name = $"{type.Name} build copy preserves every field and isolates mutable authoring state",
            CopyTypeName = type.FullName!,
        }),
    ];
    #endregion

    #region Theory Methods
    [Theory]
    [MemberData(nameof(BuildCopyCompletenessTheoryData))]
    public void BuildCopyPreservesEveryFieldAndIsolatesMutableAuthoringState(IXUnitTest test)
        => test.Execute(this);
    #endregion
}
