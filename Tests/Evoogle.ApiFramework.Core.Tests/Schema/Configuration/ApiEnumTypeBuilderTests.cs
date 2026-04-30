// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiEnumTypeBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public string? ApiName { get; init; }
        public required Type ClrType { get; init; } = null!;
        public required ApiEnumValue[] ApiEnumValues { get; init; } = null!;
        public required ApiType ApiTypeExpected { get; init; } = null!;
        public Type? ApiExtensionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiType? ApiTypeActual { get; set; }
        #endregion

        #region Constructors
        public BuildTest()
        {
            this.ExcludeMembers = ApiSchemaExcludeMembers.Standard;
        }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"ApiName: {this.ApiName.SafeToString()}");
            this.WriteLine($"ClrType: {this.ClrType.SafeToName()}");
            this.WriteLine($"ApiExtensionType: {this.ApiExtensionType.SafeToName()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.ApiTypeExpected.SafeToString()}");
        }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext();
            var builder = new ApiEnumTypeBuilder(this.ClrType, context);
            if (this.ApiName != null)
            {
                builder = builder.WithName(this.ApiName);
            }

            foreach (var apiEnumValue in this.ApiEnumValues)
            {
                builder.AddValue(apiEnumValue.ApiName, apiEnumValue.ClrName, apiEnumValue.ClrOrdinal);
            }

            if (this.ApiExtensionType != null)
            {
                var extension = Activator.CreateInstance(this.ApiExtensionType);
                builder.AddEnumExtension(this.ApiExtensionType, extension!);
            }

            this.ApiTypeActual = builder.Build();
            this.WriteLine($"Actual:   {this.ApiTypeActual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ApiTypeActual.Should().NotBeNull();

            this.ApiTypeActual.Should().BeOfType<ApiEnumType>();
            this.ApiTypeExpected.Should().BeOfType<ApiEnumType>();

            this.AssertBeEquivalentTo(this.ApiTypeActual, this.ApiTypeExpected);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    private static ApiEnumType EnumType { get; } = new ApiEnumType
    (
        "enum",
        [
            new ApiEnumValue($"{nameof(OrderStatus.Pending)}", $"{nameof(OrderStatus.Pending)}", (int)OrderStatus.Pending),
            new ApiEnumValue($"{nameof(OrderStatus.Paid)}", $"{nameof(OrderStatus.Paid)}", (int)OrderStatus.Paid),
            new ApiEnumValue($"{nameof(OrderStatus.Shipped)}", $"{nameof(OrderStatus.Shipped)}", (int)OrderStatus.Shipped),
            new ApiEnumValue($"{nameof(OrderStatus.Cancelled)}", $"{nameof(OrderStatus.Cancelled)}", (int)OrderStatus.Cancelled),
            new ApiEnumValue($"{nameof(OrderStatus.Returned)}", $"{nameof(OrderStatus.Returned)}", (int)OrderStatus.Returned)
        ],
        typeof(OrderStatus)
    );

    private static ApiEnumType EnumTypeWithExtension { get; } = new ApiEnumType
    (
        "enum",
        [
            new ApiEnumValue($"{nameof(OrderStatus.Pending)}", $"{nameof(OrderStatus.Pending)}", (int)OrderStatus.Pending),
            new ApiEnumValue($"{nameof(OrderStatus.Paid)}", $"{nameof(OrderStatus.Paid)}", (int)OrderStatus.Paid),
            new ApiEnumValue($"{nameof(OrderStatus.Shipped)}", $"{nameof(OrderStatus.Shipped)}", (int)OrderStatus.Shipped),
            new ApiEnumValue($"{nameof(OrderStatus.Cancelled)}", $"{nameof(OrderStatus.Cancelled)}", (int)OrderStatus.Cancelled),
            new ApiEnumValue($"{nameof(OrderStatus.Returned)}", $"{nameof(OrderStatus.Returned)}", (int)OrderStatus.Returned)
        ],
        typeof(OrderStatus)
    )
    {
        Extensions = new OrderedDictionary<Type, object>
        {
            [typeof(TestExtension)] = new TestExtension()
        }
    };

    private static ApiEnumType EnumTypeWithDefaultName { get; } = new ApiEnumType
    (
        nameof(OrderStatus),
        [
            new ApiEnumValue($"{nameof(OrderStatus.Pending)}", $"{nameof(OrderStatus.Pending)}", (int)OrderStatus.Pending),
            new ApiEnumValue($"{nameof(OrderStatus.Paid)}", $"{nameof(OrderStatus.Paid)}", (int)OrderStatus.Paid),
            new ApiEnumValue($"{nameof(OrderStatus.Shipped)}", $"{nameof(OrderStatus.Shipped)}", (int)OrderStatus.Shipped),
            new ApiEnumValue($"{nameof(OrderStatus.Cancelled)}", $"{nameof(OrderStatus.Cancelled)}", (int)OrderStatus.Cancelled),
            new ApiEnumValue($"{nameof(OrderStatus.Returned)}", $"{nameof(OrderStatus.Returned)}", (int)OrderStatus.Returned)
        ],
        typeof(OrderStatus)
    );

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = $"Build {EnumType}",
            ApiName = EnumType.ApiName,
            ClrType = EnumType.ClrType,
            ApiEnumValues = [.. EnumType.ApiEnumValues],
            ApiTypeExpected = EnumType,
        },
        new BuildTest
        {
            Name = $"Build {EnumTypeWithExtension} with extension",
            ApiName = EnumTypeWithExtension.ApiName,
            ClrType = EnumTypeWithExtension.ClrType,
            ApiEnumValues = [.. EnumTypeWithExtension.ApiEnumValues],
            ApiTypeExpected = EnumTypeWithExtension,
            ApiExtensionType = typeof(TestExtension),
        },
        new BuildTest
        {
            Name = $"Build {EnumTypeWithDefaultName} with default name",
            ClrType = EnumTypeWithDefaultName.ClrType,
            ApiEnumValues = [.. EnumTypeWithDefaultName.ApiEnumValues],
            ApiTypeExpected = EnumTypeWithDefaultName,
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}
