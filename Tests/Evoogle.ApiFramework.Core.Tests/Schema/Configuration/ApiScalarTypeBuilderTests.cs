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

public class ApiScalarTypeBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    public class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public string ApiName { get; init; } = null!;
        public Type ClrType { get; init; } = null!;
        public ApiType ApiTypeExpected { get; init; } = null!;
        public Type? ApiExtensionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiType? ApiTypeActual { get; set; }
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
            var builder = new ApiScalarTypeBuilder(this.ClrType, context)
                .WithName(this.ApiName);

            if (this.ApiExtensionType != null)
            {
                var extension = Activator.CreateInstance(this.ApiExtensionType);
                builder.AddExtension(this.ApiExtensionType, extension!);
            }

            this.ApiTypeActual = builder.Build();
            this.WriteLine($"Actual:   {this.ApiTypeActual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ApiTypeActual.Should().NotBeNull();

            this.ApiTypeActual.Should().BeOfType<ApiScalarType>();
            this.ApiTypeExpected.Should().BeOfType<ApiScalarType>();

            this.ApiTypeActual.As<ApiScalarType>().Should().BeEquivalentTo(this.ApiTypeExpected.As<ApiScalarType>());
        }
        #endregion
    }
    #endregion

    #region Theory Data
    private static ApiScalarType Int64Type { get; } = new ApiScalarType("number", typeof(long));
    private static ApiScalarType Int64TypeWithExtension { get; } = new ApiScalarType("number", typeof(long))
    {
        Extensions = new OrderedDictionary<Type, object>
        {
            [typeof(TestExtension)] = new TestExtension()
        }
    };

    private static ApiScalarType StringType { get; } = new ApiScalarType("text", typeof(string));
    private static ApiScalarType StringTypeWithExtension { get; } = new ApiScalarType("text", typeof(string))
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
            Name = $"Build {Int64Type}",
            ApiName = Int64Type.ApiName,
            ClrType = Int64Type.ClrType,
            ApiTypeExpected = Int64Type,
        },
        new BuildTest
        {
            Name = $"Build {StringType}",
            ApiName = StringType.ApiName,
            ClrType = StringType.ClrType,
            ApiTypeExpected = StringType,
        },
        new BuildTest
        {
            Name = $"Build {Int64TypeWithExtension} with extension",
            ApiName = Int64TypeWithExtension.ApiName,
            ClrType = Int64TypeWithExtension.ClrType,
            ApiTypeExpected = Int64TypeWithExtension,
            ApiExtensionType = typeof(TestExtension),
        },
        new BuildTest
        {
            Name = $"Build {StringTypeWithExtension} with extension",
            ApiName = StringTypeWithExtension.ApiName,
            ClrType = StringTypeWithExtension.ClrType,
            ApiTypeExpected = StringTypeWithExtension,
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

