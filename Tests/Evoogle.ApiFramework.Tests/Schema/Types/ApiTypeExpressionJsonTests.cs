// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Types;

public class ApiTypeExpressionJsonTests(ITestOutputHelper output) : XUnitTests(output)
{
    private sealed class ShapeReadTest : XUnitTest
    {
        public required string SourceJson { get; init; }
        public JsonSerializerOptions? Options { get; init; }
        public bool ExpectsReference { get; init; }
        public bool ExpectsInline { get; init; }

        private ApiTypeExpression? ActualExpression { get; set; }

        protected override void Arrange()
        {
        }

        protected override void Act()
        {
            this.ActualExpression = JsonSerializer.Deserialize<ApiTypeExpression>
            (
                this.SourceJson,
                this.Options
            );
        }

        protected override void Assert()
        {
            this.ActualExpression.Should().NotBeNull();
            this.ActualExpression!.IsReference.Should().Be(this.ExpectsReference);
            this.ActualExpression.IsInline.Should().Be(this.ExpectsInline);
        }
    }

    public static TheoryDataRow<IXUnitTest>[] ShapeReadTheoryData =>
    [
        new ShapeReadTest
        {
            Name = "API reference accepts reversed property order",
            SourceJson = """{"ApiName":"Boolean","ApiKind":"Scalar"}""",
            ExpectsReference = true
        },
        new ShapeReadTest
        {
            Name = "API reference ignores inactive null CLR type",
            SourceJson = """{"ApiKind":"Scalar","ClrType":null,"ApiName":"Boolean"}""",
            ExpectsReference = true
        },
        new ShapeReadTest
        {
            Name = "Escaped property name still identifies an API reference",
            SourceJson = """{"ApiK\u0069nd":"Scalar","ApiName":"Boolean"}""",
            ExpectsReference = true
        },
        new ShapeReadTest
        {
            Name = "Camel-case reference names follow serializer options",
            SourceJson = """{"apiKind":"Scalar","apiName":"Boolean","clrType":null}""",
            Options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            },
            ExpectsReference = true
        },
        new ShapeReadTest
        {
            Name = "Duplicate CLR type names preserve reference classification",
            SourceJson = $$"""
                {
                    "ClrType":"{{typeof(bool).AssemblyQualifiedName}}",
                    "ClrType":"{{typeof(bool).AssemblyQualifiedName}}"
                }
                """,
            ExpectsReference = true
        },
        new ShapeReadTest
        {
            Name = "Three non-null properties identify an inline type",
            SourceJson = $$"""
                {
                    "ApiKind":"Scalar",
                    "ApiName":"Boolean",
                    "ClrType":"{{typeof(bool).AssemblyQualifiedName}}"
                }
                """,
            ExpectsInline = true
        },
        new ShapeReadTest
        {
            Name = "Unknown nested value leaves an invalid expression",
            SourceJson = """{"Other":{"Nested":[1,2]}}"""
        },
        new ShapeReadTest
        {
            Name = "Empty object leaves an invalid expression",
            SourceJson = "{}"
        }
    ];

    [Theory]
    [MemberData(nameof(ShapeReadTheoryData))]
    public void ReadShape(IXUnitTest test) => test.Execute(this);
}
