// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiEnumTypeBuilderTests(ITestOutputHelper output) : ApiBuilderTests(output)
{
    #region Test Classes
    public class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public string ApiName { get; init; } = null!;
        public Type ClrType { get; init; } = null!;
        public ApiType ApiTypeExpected { get; init; } = null!;
        public ApiEnumValue[] ApiEnumValues { get; init; } = null!;
        public bool AddExtension { get; init; }
        #endregion

        #region Calculated Properties
        private ApiType? ApiTypeActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"ApiName: {this.ApiName.SafeToString()}");
            this.WriteLine($"ClrType: {this.ClrType.SafeToName()}");
            this.WriteLine($"AddExtension: {this.AddExtension.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.ApiTypeExpected.SafeToString()}");
        }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext(null);
            var builder = new ApiEnumTypeBuilder(this.ClrType, context)
                .WithName(this.ApiName);
            foreach (var apiEnumValue in this.ApiEnumValues)
            {
                builder.AddValue(apiEnumValue.ApiName, apiEnumValue.ClrName, apiEnumValue.ClrOrdinal);
            }

            if (this.AddExtension == true)
            {
                builder.AddExtension(TestExtension.Instance());
            }

            this.ApiTypeActual = builder.Build();
            this.WriteLine($"Actual:   {this.ApiTypeActual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ApiTypeActual.Should().NotBeNull();
            this.ApiTypeActual.Should().BeEquivalentTo(this.ApiTypeExpected);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    private static ApiEnumType EnumType { get; } = new ApiEnumType
    (
        "enum",
        [
            new ApiEnumValue("first", $"{nameof(TestEnum.First)}", (int)TestEnum.First),
            new ApiEnumValue("second", $"{nameof(TestEnum.Second)}", (int)TestEnum.Second)
        ],
        typeof(TestEnum)
    );

    private static ApiEnumType EnumTypeWithExtension { get; } = new ApiEnumType
    (
        "enum",
        [
            new ApiEnumValue("first", $"{nameof(TestEnum.First)}", (int)TestEnum.First),
            new ApiEnumValue("second", $"{nameof(TestEnum.Second)}", (int)TestEnum.Second)
        ],
        typeof(TestEnum)
    )
    {
        Extensions = new OrderedDictionary<Type, object>
        {
            [typeof(TestExtension)] = TestExtension.Instance()
        }
    };

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = $"Builds {EnumType} without extension",
            ApiName = EnumType.ApiName,
            ClrType = EnumType.ClrType,
            ApiEnumValues = [.. EnumType.ApiEnumValues],
            ApiTypeExpected = EnumType,
            AddExtension = false,
        },
        new BuildTest
        {
            Name = $"Builds {EnumType} with extension",
            ApiName = EnumTypeWithExtension.ApiName,
            ClrType = EnumTypeWithExtension.ClrType,
            ApiEnumValues = [.. EnumTypeWithExtension.ApiEnumValues],
            ApiTypeExpected = EnumTypeWithExtension,
            AddExtension = true,
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}

