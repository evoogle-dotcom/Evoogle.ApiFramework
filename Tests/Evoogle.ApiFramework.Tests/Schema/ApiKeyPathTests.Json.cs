// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.TestData;
using Evoogle.Extension;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public class ApiKeyPathTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private enum JsonWriteCase
    {
        Compact,
        PathExtension,
        SegmentExtension,
        Empty,
        LiteralDotSegment,
    }

    private sealed class JsonWriteTest : XUnitTest
    {
        #region User Supplied Properties
        public required string[] ExpectedClrPropertyNames { get; init; }

        public required JsonWriteCase JsonWriteCase { get; init; }

        public required bool ExpectsDetailedSegments { get; init; }

        public string? ExpectedClrPath { get; init; }
        #endregion

        #region Calculated Properties
        private ApiKeyPath? ActualPath { get; set; }

        private string? ActualJson { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            var sourcePath = CreatePath(this.JsonWriteCase);
            this.ActualJson = JsonSerializer.Serialize(sourcePath);
            this.ActualPath = JsonSerializer.Deserialize<ApiKeyPath>(this.ActualJson);
        }

        protected override void Assert()
        {
            this.ActualJson.Should().NotBeNull();
            this.ActualPath.Should().NotBeNull();
            this.ActualPath!.ApiSegments.Select(static segment => segment.ClrPropertyName)
                .Should().Equal(this.ExpectedClrPropertyNames);

            using var document = JsonDocument.Parse(this.ActualJson!);
            var properties = document.RootElement;
            properties.TryGetProperty(nameof(ApiKeyPath.ApiSegments), out var apiSegments).Should()
                .Be(this.ExpectsDetailedSegments);
            properties.TryGetProperty(nameof(ApiKeyPath.ClrPath), out var clrPath).Should()
                .Be(!this.ExpectsDetailedSegments);

            if (this.ExpectsDetailedSegments)
            {
                apiSegments.ValueKind.Should().Be(JsonValueKind.Array);
            }
            else
            {
                clrPath.GetString().Should().Be(this.ExpectedClrPath);
            }

            switch (this.JsonWriteCase)
            {
                case JsonWriteCase.PathExtension:
                    this.ActualPath.ExtensionCount.Should().Be(1);
                    break;
                case JsonWriteCase.SegmentExtension:
                    this.ActualPath.ApiSegments.Single().ExtensionCount.Should().Be(1);
                    break;
            }
        }
        #endregion
    }

    private sealed class JsonReadTest : XUnitTest
    {
        #region User Supplied Properties
        public required string[] ExpectedClrPropertyNames { get; init; }

        public required string SourceJson { get; init; }
        #endregion

        #region Calculated Properties
        private ApiKeyPath? ActualPath { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.ActualPath = JsonSerializer.Deserialize<ApiKeyPath>(this.SourceJson);
        }

        protected override void Assert()
        {
            this.ActualPath.Should().NotBeNull();
            this.ActualPath!.ApiSegments.Select(static segment => segment.ClrPropertyName)
                .Should().Equal(this.ExpectedClrPropertyNames);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonWriteTheoryData =>
    [
        new JsonWriteTest
        {
            Name = "Writes an extension-free key path in compact JSON",
            JsonWriteCase = JsonWriteCase.Compact,
            ExpectedClrPropertyNames = [nameof(KeyNestedComposite.NestedPart), nameof(KeyNested.Id)],
            ExpectsDetailedSegments = false,
            ExpectedClrPath = nameof(KeyNestedComposite.NestedPart) + "." + nameof(KeyNested.Id)
        },
        new JsonWriteTest
        {
            Name = "Writes a path extension with compact key path JSON",
            JsonWriteCase = JsonWriteCase.PathExtension,
            ExpectedClrPropertyNames = [nameof(KeyOneScalarPart.Id)],
            ExpectsDetailedSegments = false,
            ExpectedClrPath = nameof(KeyOneScalarPart.Id)
        },
        new JsonWriteTest
        {
            Name = "Writes a segment extension with detailed key path JSON",
            JsonWriteCase = JsonWriteCase.SegmentExtension,
            ExpectedClrPropertyNames = [nameof(KeyOneScalarPart.Id)],
            ExpectsDetailedSegments = true
        },
        new JsonWriteTest
        {
            Name = "Writes an empty key path with detailed JSON",
            JsonWriteCase = JsonWriteCase.Empty,
            ExpectedClrPropertyNames = [],
            ExpectsDetailedSegments = true
        },
        new JsonWriteTest
        {
            Name = "Writes a literal dot segment with detailed JSON",
            JsonWriteCase = JsonWriteCase.LiteralDotSegment,
            ExpectedClrPropertyNames = ["Nested.Part"],
            ExpectsDetailedSegments = true
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonReadTheoryData =>
    [
        new JsonReadTest
        {
            Name = "Reads and normalizes a compact key path",
            SourceJson = @"{ ""ClrPath"": "" NestedPart . Id "" }",
            ExpectedClrPropertyNames = ["NestedPart", "Id"]
        },
        new JsonReadTest
        {
            Name = "Uses detailed key path segments before compact path",
            SourceJson = @"{ ""ApiSegments"": [ { ""ClrPropertyName"": ""Id"" } ], ""ClrPath"": ""NestedPart.Id"" }",
            ExpectedClrPropertyNames = ["Id"]
        },
        new JsonReadTest
        {
            Name = "Treats null compact key path as absent",
            SourceJson = @"{ ""ClrPath"": null }",
            ExpectedClrPropertyNames = []
        },
        new JsonReadTest
        {
            Name = "Preserves malformed compact key path for compilation validation",
            SourceJson = @"{ ""ClrPath"": ""NestedPart..Id"" }",
            ExpectedClrPropertyNames = ["NestedPart", "", "Id"]
        },
        new JsonReadTest
        {
            Name = "Reads a detailed literal dot key path segment",
            SourceJson = @"{ ""ApiSegments"": [ { ""ClrPropertyName"": ""Nested.Part"" } ] }",
            ExpectedClrPropertyNames = ["Nested.Part"]
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonWriteTheoryData))]
    public void JsonWrite(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonReadTheoryData))]
    public void JsonRead(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Helper Methods
    private static ApiKeyPath CreatePath(JsonWriteCase jsonWriteCase)
    {
        return jsonWriteCase switch
        {
            JsonWriteCase.Compact => new
            (
                typeof(KeyNestedComposite),
                [
                    new ApiKeyPathSegment(nameof(KeyNestedComposite.NestedPart)),
                    new ApiKeyPathSegment(nameof(KeyNested.Id))
                ]
            ),
            JsonWriteCase.PathExtension => CreatePathWithExtension(),
            JsonWriteCase.SegmentExtension => CreatePathWithSegmentExtension(),
            JsonWriteCase.Empty => new(typeof(KeyOneScalarPart), []),
            JsonWriteCase.LiteralDotSegment => new(typeof(KeyOneScalarPart), [new ApiKeyPathSegment("Nested.Part")]),
            _ => throw new InvalidOperationException($"Unsupported {nameof(JsonWriteCase)} value '{jsonWriteCase}'."),
        };
    }

    private static ApiKeyPath CreatePathWithExtension()
    {
        var path = new ApiKeyPath(typeof(KeyOneScalarPart), [new ApiKeyPathSegment(nameof(KeyOneScalarPart.Id))]);
        path.AttachExtension(new GraphQlExtension());
        return path;
    }

    private static ApiKeyPath CreatePathWithSegmentExtension()
    {
        var segment = new ApiKeyPathSegment(nameof(KeyOneScalarPart.Id));
        segment.AttachExtension(new GraphQlExtension());
        return new(typeof(KeyOneScalarPart), [segment]);
    }
    #endregion
}
