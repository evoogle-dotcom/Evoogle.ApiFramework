// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using FluentAssertions;
using Evoogle.XUnit;
using Xunit.Abstractions;

namespace Evoogle.ApiFramework.Schema;

public class ApiSchemaTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private class Person
    {
        public string Name { get; set; } = string.Empty;
    }
    #endregion

    #region Test Classes
    private class TryGetByApiNameTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiSchema? Schema { get; set; }
        public string? ApiName { get; set; }
        public ApiType? ExpectedApiType { get; set; }
        #endregion

        #region Calculated Properties
        private bool ActualFound { get; set; }
        private ApiType? ActualApiType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            this.ActualFound = this.Schema!.TryGetByApiName(this.ApiName!, out var apiType);
            this.ActualApiType = apiType;
        }

        protected override void Assert()
        {
            this.ActualFound.Should().BeTrue();
            this.ActualApiType.Should().BeSameAs(this.ExpectedApiType);
        }
        #endregion
    }

    private class TryGetByClrLookupTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiSchema? Schema { get; set; }
        public string? ClrName { get; set; }
        public Type? ClrType { get; set; }
        public ApiType? ExpectedApiType { get; set; }
        #endregion

        #region Calculated Properties
        private ApiType? ByName { get; set; }
        private ApiType? ByType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            this.Schema!.TryGetByClrName(this.ClrName!, out var byName);
            this.Schema!.TryGetByClrType(this.ClrType!, out var byType);
            this.ByName = byName;
            this.ByType = byType;
        }

        protected override void Assert()
        {
            this.ByName.Should().BeSameAs(this.ExpectedApiType);
            this.ByType.Should().BeSameAs(this.ExpectedApiType);
        }
        #endregion
    }

    private class ConstructorDuplicateTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiType[] ApiTypes { get; set; } = [];
        #endregion

        #region Calculated Properties
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            try
            {
                _ = new ApiSchema(this.ApiTypes);
            }
            catch (Exception ex)
            {
                this.ActualException = ex;
            }
        }

        protected override void Assert()
        {
            this.ActualException.Should().BeOfType<ApiSchemaException>();
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TryGetByApiNameTheoryData =>
    [
        CreateTryGetByApiNameTest(),
    ];

    private static TryGetByApiNameTest CreateTryGetByApiNameTest()
    {
        var booleanType = new ApiScalarType("Boolean", typeof(bool));
        var stringType = new ApiScalarType("String", typeof(string));
        var nameProperty = new ApiProperty("Name", stringType, ApiTypeModifiers.Required, nameof(Person.Name));
        var personType = new ApiObjectType(nameof(Person), [nameProperty], typeof(Person));
        var schema = new ApiSchema([booleanType, stringType, personType]);

        return new TryGetByApiNameTest
        {
            Name = "Find Boolean by ApiName",
            Schema = schema,
            ApiName = "Boolean",
            ExpectedApiType = booleanType,
        };
    }

    public static TheoryDataRow<IXUnitTest>[] TryGetByClrLookupTheoryData =>
    [
        CreateTryGetByClrLookupTest(),
    ];

    private static TryGetByClrLookupTest CreateTryGetByClrLookupTest()
    {
        var booleanType = new ApiScalarType("Boolean", typeof(bool));
        var schema = new ApiSchema([booleanType]);

        return new TryGetByClrLookupTest
        {
            Name = "Boolean by CLR name and type",
            Schema = schema,
            ClrName = "Boolean",
            ClrType = typeof(bool),
            ExpectedApiType = booleanType,
        };
    }

    public static TheoryDataRow<IXUnitTest>[] ConstructorDuplicateTheoryData =>
    [
        CreateConstructorDuplicateTest(),
    ];

    private static ConstructorDuplicateTest CreateConstructorDuplicateTest()
    {
        var t1 = new ApiScalarType("Boolean", typeof(bool));
        var t2 = new ApiScalarType("Boolean", typeof(bool));

        return new ConstructorDuplicateTest
        {
            Name = "Duplicate detection",
            ApiTypes = [t1, t2],
        };
    }
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(TryGetByApiNameTheoryData))]
    public void TryGetByApiName(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetByClrLookupTheoryData))]
    public void TryGetByClrNameAndClrType(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ConstructorDuplicateTheoryData))]
    public void Constructor_ThrowsWhenDuplicatesDetected(IXUnitTest test) => test.Execute(this);
    #endregion
}
