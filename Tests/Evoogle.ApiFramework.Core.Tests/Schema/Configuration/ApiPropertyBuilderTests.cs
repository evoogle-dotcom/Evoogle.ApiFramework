// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiPropertyBuilderTests(ITestOutputHelper output) : ApiBuilderTests(output)
{
    #region Test Classes
    public class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public string ApiName { get; init; } = null!;
        public string ClrName { get; init; } = null!;
        public ApiProperty ApiPropertyExpected { get; init; } = null!;
        public bool AddExtension { get; init; }
        #endregion

        #region Calculated Properties
        private ApiProperty? ApiPropertyActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"ApiName: {this.ApiName.SafeToString()}");
            this.WriteLine($"ClrName: {this.ClrName.SafeToString()}");
            this.WriteLine($"AddExtension: {this.AddExtension.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.ApiPropertyExpected.SafeToString()}");
        }

        protected override void Act()
        {
            var builder = new ApiPropertyBuilder(this.ApiName, this.ClrName);
            if (this.AddExtension == true)
            {
                builder.AddExtension(TestExtension.Instance());
            }

            this.ApiPropertyActual = builder.Build(typeof(TestClass));
            this.WriteLine($"Actual:   {this.ApiPropertyActual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ApiPropertyActual.Should().NotBeNull();

            this.ApiPropertyActual.Should().BeEquivalentTo
            (
                this.ApiPropertyExpected,
                opt => opt
                    .Excluding(p => p.ApiType)
            );
        }
        #endregion
    }
    #endregion

    #region Theory Data
    private static ApiProperty RequiredNameProperty { get; } = new ApiProperty("name", new ApiTypeExpression(typeof(string)), ApiTypeModifiers.Required, nameof(TestClass.RequiredName));
    private static ApiProperty RequiredNamePropertyWithExtension { get; } = new ApiProperty("name", new ApiTypeExpression(typeof(string)), ApiTypeModifiers.Required, nameof(TestClass.RequiredName))
    {
        Extensions = new OrderedDictionary<Type, object>
        {
            [typeof(TestExtension)] = TestExtension.Instance()
        }
    };

    private static ApiProperty OptionalAgeProperty { get; } = new ApiProperty("age", new ApiTypeExpression(typeof(int)), ApiTypeModifiers.None, nameof(TestClass.OptionalAge));
    private static ApiProperty OptionalAgePropertyWithExtension { get; } = new ApiProperty("age", new ApiTypeExpression(typeof(int)), ApiTypeModifiers.None, nameof(TestClass.OptionalAge))
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
            Name = $"Builds {RequiredNameProperty} without extension",
            ApiName = RequiredNameProperty.ApiName,
            ClrName = RequiredNameProperty.ClrName,
            ApiPropertyExpected = RequiredNameProperty,
            AddExtension = false,
        },
        new BuildTest
        {
            Name = $"Builds {RequiredNameProperty} with extension",
            ApiName = RequiredNamePropertyWithExtension.ApiName,
            ClrName = RequiredNamePropertyWithExtension.ClrName,
            ApiPropertyExpected = RequiredNamePropertyWithExtension,
            AddExtension = true,
        },
        new BuildTest
        {
            Name = $"Builds {OptionalAgeProperty} without extension",
            ApiName = OptionalAgeProperty.ApiName,
            ClrName = OptionalAgeProperty.ClrName,
            ApiPropertyExpected = OptionalAgeProperty,
            AddExtension = false,
        },
        new BuildTest
        {
            Name = $"Builds {OptionalAgeProperty} with extension",
            ApiName = OptionalAgePropertyWithExtension.ApiName,
            ClrName = OptionalAgePropertyWithExtension.ClrName,
            ApiPropertyExpected = OptionalAgePropertyWithExtension,
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

