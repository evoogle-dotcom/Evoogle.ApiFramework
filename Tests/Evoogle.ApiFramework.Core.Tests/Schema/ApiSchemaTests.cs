// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

using Evoogle.Json;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

[DynamicLinqType]
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
        [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiSchema>))]
        public Expression<Func<ApiSchema>>? ApiSchemaFactoryExpression { get; set; }

        public string? ApiName { get; set; }
        public ApiType? ExpectedApiType { get; set; }
        #endregion

        #region Calculated Properties
        public ApiSchema? ApiSchema { get; set; }
        private bool ExpectedFound { get; set; }
        private bool ActualFound { get; set; }
        private ApiType? ActualApiType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchemaFactoryFunc = this.ApiSchemaFactoryExpression!.Compile();
            var apiSchema = apiSchemaFactoryFunc();
            this.ApiSchema = apiSchema!;

            this.ExpectedFound = this.ExpectedApiType != null;

            this.WriteLine($"Expected Found:   {this.ExpectedFound.SafeToString()}");
            this.WriteLine($"Expected ApiType: {this.ExpectedApiType.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualFound = this.ApiSchema!.TryGetByApiName(this.ApiName!, out var apiType);
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

    // private class TryGetByClrLookupTest : XUnitTest
    // {
    //     #region User Supplied Properties
    //     public ApiSchema? Schema { get; set; }
    //     public string? ClrName { get; set; }
    //     public Type? ClrType { get; set; }
    //     public ApiType? ExpectedApiType { get; set; }
    //     #endregion

    //     #region Calculated Properties
    //     private ApiType? ByName { get; set; }
    //     private ApiType? ByType { get; set; }
    //     #endregion

    //     #region XUnitTest Methods
    //     protected override void Act()
    //     {
    //         this.Schema!.TryGetByClrName(this.ClrName!, out var byName);
    //         this.Schema!.TryGetByClrType(this.ClrType!, out var byType);
    //         this.ByName = byName;
    //         this.ByType = byType;
    //     }

    //     protected override void Assert()
    //     {
    //         this.ByName.Should().BeSameAs(this.ExpectedApiType);
    //         this.ByType.Should().BeSameAs(this.ExpectedApiType);
    //     }
    //     #endregion
    // }

    // private class ConstructorDuplicateTest : XUnitTest
    // {
    //     #region User Supplied Properties
    //     public ApiType[] ApiTypes { get; set; } = [];
    //     #endregion

    //     #region Calculated Properties
    //     private Exception? ActualException { get; set; }
    //     #endregion

    //     #region XUnitTest Methods
    //     protected override void Act()
    //     {
    //         try
    //         {
    //             _ = new ApiSchema(this.ApiTypes);
    //         }
    //         catch (Exception ex)
    //         {
    //             this.ActualException = ex;
    //         }
    //     }

    //     protected override void Assert()
    //     {
    //         this.ActualException.Should().BeOfType<ApiSchemaException>();
    //     }
    //     #endregion
    // }
    #endregion

    #region Theory Data
    public static ApiSchema CreateTestApiSchema()
    {
        var schema = new ApiSchema(
                [
                    new ApiScalarType("Boolean", typeof(bool)),
                    new ApiScalarType("String", typeof(string)),
                    new ApiObjectType
                    (
                        nameof(Person),
                        [new ApiProperty("Name", new ApiScalarType("String", typeof(string)), ApiTypeModifiers.Required, nameof(Person.Name))],
                        typeof(Person)
                    )
                ]);
        return schema;
    }

    public static TheoryDataRow<IXUnitTest>[] TryGetByApiNameTheoryData =>
    [
        new TryGetByApiNameTest()
        {
            Name = "Find Boolean by ApiName",
            ApiSchemaFactoryExpression = () => CreateTestApiSchema(),
            ApiName = "Boolean",
            ExpectedApiType = new ApiScalarType("Boolean", typeof(bool)),
        },
    ];

    // public static TheoryDataRow<IXUnitTest>[] TryGetByClrLookupTheoryData =>
    // [
    //     CreateTryGetByClrLookupTest(),
    // ];

    // private static TryGetByClrLookupTest CreateTryGetByClrLookupTest()
    // {
    //     var booleanType = new ApiScalarType("Boolean", typeof(bool));
    //     var schema = new ApiSchema([booleanType]);

    //     return new TryGetByClrLookupTest
    //     {
    //         Name = "Boolean by CLR name and type",
    //         Schema = schema,
    //         ClrName = "Boolean",
    //         ClrType = typeof(bool),
    //         ExpectedApiType = booleanType,
    //     };
    // }

    // public static TheoryDataRow<IXUnitTest>[] ConstructorDuplicateTheoryData =>
    // [
    //     CreateConstructorDuplicateTest(),
    // ];

    // private static ConstructorDuplicateTest CreateConstructorDuplicateTest()
    // {
    //     var t1 = new ApiScalarType("Boolean", typeof(bool));
    //     var t2 = new ApiScalarType("Boolean", typeof(bool));

    //     return new ConstructorDuplicateTest
    //     {
    //         Name = "Duplicate detection",
    //         ApiTypes = [t1, t2],
    //     };
    // }
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(TryGetByApiNameTheoryData))]
    public void TryGetByApiName(IXUnitTest test) => test.Execute(this);

    // [Theory]
    // [MemberData(nameof(TryGetByClrLookupTheoryData))]
    // public void TryGetByClrNameAndClrType(IXUnitTest test) => test.Execute(this);

    // [Theory]
    // [MemberData(nameof(ConstructorDuplicateTheoryData))]
    // public void Constructor_ThrowsWhenDuplicatesDetected(IXUnitTest test) => test.Execute(this);
    #endregion
}
