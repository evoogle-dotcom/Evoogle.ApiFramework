// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiRelationshipBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    private class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public string ApiName { get; init; } = null!;
        public string? ApiPropertyName { get; init; }
        public ApiRelationship ApiRelationshipExpected { get; init; } = null!;
        public Type? ApiExtensionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiRelationship? ApiRelationshipActual { get; set; }
        #endregion

        protected override void Arrange()
        {
            this.WriteLine($"ApiName: {this.ApiName}");
            this.WriteLine($"ApiPropertyName: {this.ApiPropertyName}");
            this.WriteLine($"ApiExtensionType: {this.ApiExtensionType.SafeToName()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.ApiRelationshipExpected.SafeToString()}");
        }

        protected override void Act()
        {
            var builder = new ApiRelationshipBuilder(this.ApiName, this.ApiPropertyName);
            if (this.ApiExtensionType != null)
            {
                var extension = Activator.CreateInstance(this.ApiExtensionType);
                builder.AddExtension(this.ApiExtensionType, extension!);
            }

            this.ApiRelationshipActual = builder.Build();
            this.WriteLine($"Actual:   {this.ApiRelationshipActual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ApiRelationshipActual.Should().NotBeNull();

            this.ApiRelationshipActual.Should().BeEquivalentTo
            (
                this.ApiRelationshipExpected,
                opt => opt
                    .Excluding(info => info.Path.Contains(nameof(ApiSchemaElement.ApiPath)))
                    .Excluding(r => r.ApiProperty)
                    .Excluding(r => r.ApiCardinality)
                    .WithStrictOrdering()
            );
        }
    }

    #region Theory Data
    private static ApiRelationship CustomerRelationship { get; } = new ApiRelationship("customer");
    private static ApiRelationship CustomerRelationshipWithExtension { get; } = new ApiRelationship("customer")
    {
        Extensions = new OrderedDictionary<Type, object>
        {
            [typeof(TestExtension)] = new TestExtension()
        }
    };

    private static ApiRelationship OrdersRelationship { get; } = new ApiRelationship("orders", "top-ten-orders");
    private static ApiRelationship OrdersRelationshipWithExtension { get; } = new ApiRelationship("orders", "top-ten-orders")
    {
        Extensions = new OrderedDictionary<Type, object>
        {
            [typeof(TestExtension)] = new TestExtension()
        }
    };

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = $"Builds {CustomerRelationship}",
            ApiName = CustomerRelationship.ApiName,
            ApiPropertyName = CustomerRelationship.ApiPropertyName,
            ApiRelationshipExpected = CustomerRelationship,
        },
        new BuildTest
        {
            Name = $"Builds {CustomerRelationshipWithExtension} with extension",
            ApiName = CustomerRelationshipWithExtension.ApiName,
            ApiPropertyName = CustomerRelationshipWithExtension.ApiPropertyName,
            ApiRelationshipExpected = CustomerRelationshipWithExtension,
            ApiExtensionType = typeof(TestExtension),
        },
        new BuildTest
        {
            Name = $"Builds {OrdersRelationship}",
            ApiName = OrdersRelationship.ApiName,
            ApiPropertyName = OrdersRelationship.ApiPropertyName,
            ApiRelationshipExpected = OrdersRelationship,
        },
        new BuildTest
        {
            Name = $"Builds {OrdersRelationshipWithExtension} with extension",
            ApiName = OrdersRelationshipWithExtension.ApiName,
            ApiPropertyName = OrdersRelationshipWithExtension.ApiPropertyName,
            ApiRelationshipExpected = OrdersRelationshipWithExtension,
            ApiExtensionType = typeof(TestExtension),
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}

