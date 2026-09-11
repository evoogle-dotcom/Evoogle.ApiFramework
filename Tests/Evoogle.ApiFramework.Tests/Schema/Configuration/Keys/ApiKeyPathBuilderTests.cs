// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Keys;

public class ApiKeyPathBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class BuildPathTest : XUnitTest
    {
        #region User Supplied Properties
        public required string[] ClrMemberNames { get; init; }

        public required string[] ExpectedClrMemberNames { get; init; }

        public bool UsesKeyTypeBuilder { get; init; }
        #endregion

        #region Calculated Properties
        private ApiKeyPath? ActualPath { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            if (this.UsesKeyTypeBuilder)
            {
                var keyType = new ApiKeyTypeBuilder().AddPath(typeof(object), this.ClrMemberNames).Build();
                this.ActualPath = keyType.ApiKeyPaths.Single();
            }
            else
            {
                var builder = new ApiKeyPathBuilder(typeof(object), this.ClrMemberNames);
                this.ActualPath = builder.Build();
            }
        }

        protected override void Assert()
        {
            this.ActualPath.Should().NotBeNull();
            this.ActualPath!.ApiSegments.Select(static segment => segment.ClrMemberName)
                .Should().Equal(this.ExpectedClrMemberNames);
        }
        #endregion
    }

    private sealed class RejectDottedSegmentTest : XUnitTest
    {
        #region User Supplied Properties
        public bool UsesSegmentBuilder { get; init; }
        #endregion

        #region Calculated Properties
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            try
            {
                if (this.UsesSegmentBuilder)
                {
                    _ = new ApiKeyPathSegmentBuilder("NestedPart.Id");
                }
                else
                {
                    _ = new ApiKeyPathBuilder(typeof(object), ["Id"]).AddSegment("NestedPart.Id");
                }
            }
            catch (ArgumentException exception)
            {
                this.ActualException = exception;
            }
        }

        protected override void Assert()
        {
            this.ActualException.Should().BeOfType<ArgumentException>();
            this.ActualException!.Message.Should().Contain("cannot contain a dot");
        }
        #endregion
    }

    private sealed class RejectInvalidPathTest : XUnitTest
    {
        #region Calculated Properties
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            try
            {
                _ = new ApiKeyPathBuilder(typeof(object), ["NestedPart..Id"]);
            }
            catch (ArgumentException exception)
            {
                this.ActualException = exception;
            }
        }

        protected override void Assert()
        {
            this.ActualException.Should().BeOfType<ArgumentException>();
            this.ActualException!.Message.Should().Contain("non-empty dot-delimited member names");
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BuildPathTheoryData =>
    [
        new BuildPathTest
        {
            Name = "Builds key path from dot-delimited CLR path",
            ClrMemberNames = ["NestedPart.Id"],
            ExpectedClrMemberNames = ["NestedPart", "Id"]
        },
        new BuildPathTest
        {
            Name = "Builds key path from mixed CLR path fragments",
            ClrMemberNames = [" NestedPart . Id ", "Name"],
            ExpectedClrMemberNames = ["NestedPart", "Id", "Name"],
            UsesKeyTypeBuilder = true
        },
        new RejectDottedSegmentTest
        {
            Name = "Rejects dot-delimited name for one key path segment"
        },
        new RejectDottedSegmentTest
        {
            Name = "Rejects dot-delimited name for one key path segment builder",
            UsesSegmentBuilder = true
        },
        new RejectInvalidPathTest
        {
            Name = "Rejects invalid dot-delimited key path"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildPathTheoryData))]
    public void BuildPath(IXUnitTest test) => test.Execute(this);
    #endregion
}
