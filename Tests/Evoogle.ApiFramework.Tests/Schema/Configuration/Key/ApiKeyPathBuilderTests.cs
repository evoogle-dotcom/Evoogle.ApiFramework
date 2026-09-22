// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Key;

public class ApiKeyPathBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class RootObject
    {
        public NestedObject Nested { get; } = new();
    }

    private sealed class NestedObject
    {
        public int Id { get; set; }
    }

    private sealed class BuildPathTest : XUnitTest
    {
        #region User Supplied Properties
        public required string[] ClrMemberNames { get; init; }

        public required string[] ExpectedClrMemberNames { get; init; }

        public bool UsesKeyDefinitionBuilder { get; init; }
        #endregion

        #region Calculated Properties
        private ApiKeyPath? ActualPath { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            if (this.UsesKeyDefinitionBuilder)
            {
                var keyDefinition = new ApiKeyDefinitionBuilder()
                    .AddPath(typeof(object), this.ClrMemberNames)
                    .Build();
                this.ActualPath = keyDefinition.ApiKeyPaths.Single();
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
            this.ActualPath!.ApiSegments.Select(static segment =>
                segment.ApiPropertyReference.ClrName)
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

    private sealed class BuildReferencePathTest : XUnitTest
    {
        private ApiKeyPath? ActualPath { get; set; }

        private string? ActualJson { get; set; }

        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.ActualPath = new ApiKeyPathBuilder
            (
                [
                    new ApiKeyPathSegmentBuilder(ApiPropertyReference.ApiRef("nested")),
                    new ApiKeyPathSegmentBuilder(ApiPropertyReference.ClrRef("Id"))
                ]
            ).Build();
            this.ActualJson = JsonSerializer.Serialize(this.ActualPath);
        }

        protected override void Assert()
        {
            this.ActualPath!.ApiSegments[0].ApiPropertyReference.Should().Be
            (
                ApiPropertyReference.ApiRef("nested")
            );
            this.ActualPath.ApiSegments[1].ApiPropertyReference.Should().Be
            (
                ApiPropertyReference.ClrRef("Id")
            );
            this.ActualJson.Should().Contain("\"ApiSegments\"");
            this.ActualJson.Should().Contain("\"ApiName\":\"nested\"");
            this.ActualJson.Should().Contain("\"ClrName\":\"Id\"");
            this.ActualJson.Should().NotContain("\"ClrPath\"");
        }
    }

    private sealed class CompileReferencePathTest : XUnitTest
    {
        private ApiKeyPath? ActualPath { get; set; }

        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.ActualPath = new ApiKeyPathBuilder
            (
                [
                    new ApiKeyPathSegmentBuilder(ApiPropertyReference.ApiRef("nested")),
                    new ApiKeyPathSegmentBuilder(ApiPropertyReference.ClrRef(nameof(NestedObject.Id)))
                ]
            ).Build();

            var nestedProperty = new ApiProperty
            (
                "nested",
                new ApiTypeExpression(new ApiTypeReference(typeof(NestedObject))),
                ApiTypeModifiers.Required,
                nameof(RootObject.Nested),
                ClrMemberKind.Property
            );
            var identifierProperty = new ApiProperty
            (
                "identifier",
                new ApiTypeExpression(new ApiTypeReference(typeof(int))),
                ApiTypeModifiers.Required,
                nameof(NestedObject.Id),
                ClrMemberKind.Property
            );
            var rootType = new ApiObjectType
            (
                nameof(RootObject),
                null,
                [nestedProperty],
                [new ApiNamedKeyDefinition("Primary", [this.ActualPath])],
                null,
                typeof(RootObject)
            );
            var nestedType = new ApiObjectType
            (
                nameof(NestedObject),
                null,
                [identifierProperty],
                null,
                null,
                typeof(NestedObject)
            );
            var schema = new ApiSchema
            (
                "PropertyReferencePath",
                null,
                null,
                [new ApiScalarType("Int32", typeof(int))],
                null,
                [rootType, nestedType],
                null
            );

            ApiSchemaCompiler.Compile(schema).ThrowIfInvalid();
        }

        protected override void Assert()
        {
            this.ActualPath!.ClrPath.Should().Be("Nested.Id");
            this.ActualPath.ApiSegments.Select(static segment => segment.ApiProperty.ApiName)
                .Should().Equal("nested", "identifier");
        }
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
            UsesKeyDefinitionBuilder = true
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
        new BuildReferencePathTest
        {
            Name = "Builds and serializes mixed property-reference path"
        },
        new CompileReferencePathTest
        {
            Name = "Compiles mixed property-reference path to CLR metadata"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildPathTheoryData))]
    public void BuildPath(IXUnitTest test) => test.Execute(this);
    #endregion
}
