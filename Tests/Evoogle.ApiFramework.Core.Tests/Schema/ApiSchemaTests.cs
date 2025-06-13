// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public class ApiSchemaTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private enum Gender
    {
        Unspecified,
        Male,
        Female
    }

    private class Person
    {
        public string Name { get; set; } = string.Empty;
        public int? Age { get; set; }
        public Gender? Gender { get; set; }
    }
    #endregion

    #region Test Classes
    private class TryGetByApiNameTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiSchema? ApiSchema { get; set; }
        public string? ApiName { get; set; }
        public ApiType? ExpectedApiType { get; set; }
        #endregion

        #region Calculated Properties
        private bool ExpectedFound { get; set; }
        private bool ActualFound { get; set; }
        private ApiType? ActualApiType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"ApiName: {this.ApiName.SafeToString()}");
            this.WriteLine();

            this.ExpectedFound = this.ExpectedApiType != null;

            this.WriteLine($"Expected Found:   {this.ExpectedFound.SafeToString()}");
            this.WriteLine($"Expected ApiType: {this.ExpectedApiType.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualFound = this.ApiSchema!.TryGetApiType(this.ApiName!, out var apiType);
            this.ActualApiType = apiType;

            this.WriteLine($"Actual   Found:   {this.ActualFound.SafeToString()}");
            this.WriteLine($"Actual   ApiType: {this.ActualApiType.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualFound.Should().Be(this.ExpectedFound);
            this.ActualApiType.Should().BeEquivalentTo(this.ExpectedApiType);
        }
        #endregion
    }

    private class TryGetByClrTypeTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiSchema? ApiSchema { get; set; }
        public Type? ClrType { get; set; }
        public ApiType? ExpectedApiType { get; set; }
        #endregion

        #region Calculated Properties
        private bool ExpectedFound { get; set; }
        private bool ActualFound { get; set; }
        private ApiType? ActualApiType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var clrTypeName = this.ClrType!.Name;
            this.WriteLine($"ClrType: {clrTypeName.SafeToString()}");
            this.WriteLine();

            this.ExpectedFound = this.ExpectedApiType != null;

            this.WriteLine($"Expected Found:   {this.ExpectedFound.SafeToString()}");
            this.WriteLine($"Expected ApiType: {this.ExpectedApiType.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualFound = this.ApiSchema!.TryGetApiType(this.ClrType!, out var apiType);
            this.ActualApiType = apiType;

            this.WriteLine($"Actual   Found:   {this.ActualFound.SafeToString()}");
            this.WriteLine($"Actual   ApiType: {this.ActualApiType.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualFound.Should().Be(this.ExpectedFound);
            this.ActualApiType.Should().BeEquivalentTo(this.ExpectedApiType);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static readonly ApiScalarType TestApiScalarTypeBoolean = new(nameof(Boolean), typeof(bool));
    public static readonly ApiScalarType TestApiScalarTypeInt32 = new(nameof(Int32), typeof(int));
    public static readonly ApiScalarType TestApiScalarTypeString = new(nameof(String), typeof(string));

    public static readonly ApiEnumType TestApiEnumTypeGender = new
    (
        nameof(Gender),
        [
            new ApiEnumValue(nameof(Gender.Unspecified), nameof(Gender.Unspecified), (int)Gender.Unspecified),
            new ApiEnumValue(nameof(Gender.Male), nameof(Gender.Male), (int)Gender.Male),
            new ApiEnumValue(nameof(Gender.Female), nameof(Gender.Female), (int)Gender.Female)
        ],
        typeof(Gender)
    );

    public static readonly ApiObjectType TestApiObjectTypePerson = new
    (
        nameof(Person),
        [
            new ApiProperty(nameof(Person.Name), TestApiScalarTypeString, ApiTypeModifiers.Required, nameof(Person.Name)),
            new ApiProperty(nameof(Person.Age), TestApiScalarTypeInt32, ApiTypeModifiers.None, nameof(Person.Age)),
            new ApiProperty(nameof(Person.Gender), TestApiEnumTypeGender, ApiTypeModifiers.None, nameof(Person.Gender))
        ],
        typeof(Person)
    );

    public static readonly ApiSchema TestApiSchema = new
    (
        nameof(TestApiSchema),
        [
            TestApiScalarTypeBoolean,
            TestApiScalarTypeInt32,
            TestApiScalarTypeString,
            TestApiEnumTypeGender,
            TestApiObjectTypePerson
        ]
    );

    public static TheoryDataRow<IXUnitTest>[] TryGetByApiNameTheoryData =>
    [
        new TryGetByApiNameTest()
        {
            Name = $"Find {TestApiScalarTypeBoolean} by ApiName",
            ApiSchema = TestApiSchema,
            ApiName = nameof(Boolean),
            ExpectedApiType = TestApiScalarTypeBoolean,
        },
        new TryGetByApiNameTest()
        {
            Name = $"Find {TestApiScalarTypeInt32} by ApiName",
            ApiSchema = TestApiSchema,
            ApiName = nameof(Int32),
            ExpectedApiType = TestApiScalarTypeInt32,
        },
        new TryGetByApiNameTest()
        {
            Name = $"Find {TestApiScalarTypeString} by ApiName",
            ApiSchema = TestApiSchema,
            ApiName = nameof(String),
            ExpectedApiType = TestApiScalarTypeString,
        },
        new TryGetByApiNameTest()
        {
            Name = $"Find {TestApiEnumTypeGender} by ApiName",
            ApiSchema = TestApiSchema,
            ApiName = nameof(Gender),
            ExpectedApiType = TestApiEnumTypeGender,
        },
        new TryGetByApiNameTest()
        {
            Name = $"Find {TestApiObjectTypePerson} by ApiName",
            ApiSchema = TestApiSchema,
            ApiName = nameof(Person),
            ExpectedApiType = TestApiObjectTypePerson
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetByClrTypeTheoryData =>
    [
        new TryGetByClrTypeTest()
        {
            Name = $"Find {TestApiScalarTypeBoolean} by ClrType",
            ApiSchema = TestApiSchema,
            ClrType = typeof(bool),
            ExpectedApiType = TestApiScalarTypeBoolean,
        },
        new TryGetByClrTypeTest()
        {
            Name = $"Find {TestApiScalarTypeInt32} by ClrType",
            ApiSchema = TestApiSchema,
            ClrType = typeof(int),
            ExpectedApiType = TestApiScalarTypeInt32,
        },
        new TryGetByClrTypeTest()
        {
            Name = $"Find {TestApiScalarTypeString} by ClrType",
            ApiSchema = TestApiSchema,
            ClrType = typeof(string),
            ExpectedApiType = TestApiScalarTypeString,
        },
        new TryGetByClrTypeTest()
        {
            Name = $"Find {TestApiEnumTypeGender} by ClrType",
            ApiSchema = TestApiSchema,
            ClrType = typeof(Gender),
            ExpectedApiType = TestApiEnumTypeGender,
        },
        new TryGetByClrTypeTest()
        {
            Name = $"Find {TestApiObjectTypePerson} by ClrType",
            ApiSchema = TestApiSchema,
            ClrType = typeof(Person),
            ExpectedApiType = TestApiObjectTypePerson
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(TryGetByApiNameTheoryData))]
    public void TryGetByApiName(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetByClrTypeTheoryData))]
    public void TryGetByClrType(IXUnitTest test) => test.Execute(this);
    #endregion
}

