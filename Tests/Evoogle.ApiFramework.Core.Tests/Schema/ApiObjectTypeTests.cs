// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public class ApiObjectTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    public record struct ApiPropertyStub(string ApiName, ApiTypeKind ApiTypeExpressionKind, string ApiTypeExpressionApiName, ApiTypeModifiers ApiTypeModifiers, string ClrName);
    public record struct ApiRelationshipStub(string ApiName);

    public class ConstructorThrowsTest : XUnitTest
    {
        #region User Supplied Properties
        public List<ApiPropertyStub>? ApiPropertyStubCollection { get; init; }
        public List<ApiRelationshipStub>? ApiRelationshipStubCollection { get; init; }
        public string? ExpectedApiSchemaExceptionMessage { get; init; }
        #endregion

        #region Calculated Properties
        private List<ApiProperty>? ApiPropertyCollection { get; set; }
        private List<ApiRelationship>? ApiRelationshipCollection { get; set; }
        private bool? ActualApiSchemaExceptionThrown { get; set; }
        private string? ActualApiSchemaExceptionMessage { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiPropertiesString = this.ApiPropertyStubCollection?
                .Select(x => $"{{ApiName={x.ApiName},ClrName={x.ClrName}}}")
                .SafeToDelimitedString(',') ?? "None";

            this.WriteLine($"API Properties: [{apiPropertiesString}]");
            this.WriteLine();

            this.ApiPropertyCollection = this.ApiPropertyStubCollection?.Select
            (
                stub => new ApiProperty
                (
                    stub.ApiName,
                    new ApiTypeExpression(stub.ApiTypeExpressionKind, stub.ApiTypeExpressionApiName),
                    ApiTypeModifiers.None,
                    stub.ClrName
                )
            ).ToList() ?? [];

            var apiRelationshipsString = this.ApiRelationshipStubCollection?
                .Select(x => $"{{ApiName={x.ApiName}}}")
                .SafeToDelimitedString(',') ?? "None";

            this.WriteLine($"API Relationships: [{apiRelationshipsString}]");
            this.WriteLine();

            this.ApiRelationshipCollection = this.ApiRelationshipStubCollection?.Select
            (
                stub => new ApiRelationship(stub.ApiName)
            ).ToList() ?? [];

            this.WriteLine($"Expected Exception Message: {this.ExpectedApiSchemaExceptionMessage.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            try
            {
                var apiObjectType = new ApiObjectType(nameof(Object), // Using 'Object' as a placeholder API name
                                                      this.ApiPropertyCollection ?? throw new ArgumentNullException(nameof(this.ApiPropertyCollection)),
                                                      this.ApiRelationshipCollection ?? throw new ArgumentNullException(nameof(this.ApiRelationshipCollection)),
                                                      typeof(object)); // Using 'object' as a placeholder CLR type
            }
            catch (ApiSchemaException ex)
            {
                this.ActualApiSchemaExceptionThrown = true;
                this.ActualApiSchemaExceptionMessage = ex.Message;
            }

            this.WriteLine($"Actual Exception Thrown:  {this.ActualApiSchemaExceptionThrown.SafeToString()}");
            this.WriteLine($"Actual Exception Message: {this.ActualApiSchemaExceptionMessage.SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualApiSchemaExceptionThrown.Should().BeTrue();
            this.ActualApiSchemaExceptionMessage.Should().Be(this.ExpectedApiSchemaExceptionMessage);
        }
        #endregion
    }

    public enum TryGetMethod
    {
        TryGetPropertyByApiName,
        TryGetPropertyByClrName,
        TryGetRelationshipByApiName
    }

    public class TryGetTest : XUnitTest
    {
        #region User Supplied Properties
        public List<ApiPropertyStub>? ApiPropertyStubCollection { get; init; }
        public List<ApiRelationshipStub>? ApiRelationshipStubCollection { get; init; }
        public TryGetMethod? TryGetMethod { get; init; }
        public string? Input { get; init; }
        public bool? ExpectedFound { get; init; }
        #endregion

        #region Calculated Properties
        private List<ApiProperty>? ApiPropertyCollection { get; set; }
        private List<ApiRelationship>? ApiRelationshipCollection { get; set; }
        private ApiObjectType? ApiObjectType { get; set; }
        private bool? ActualFound { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiPropertiesString = this.ApiPropertyStubCollection?
                .Select(x => $"{{ApiName={x.ApiName},ClrName={x.ClrName}}}")
                .SafeToDelimitedString(',') ?? "None";

            this.WriteLine($"API Properties: [{apiPropertiesString}]");
            this.WriteLine();

            this.ApiPropertyCollection = this.ApiPropertyStubCollection?.Select
            (
                stub => new ApiProperty
                (
                    stub.ApiName,
                    new ApiTypeExpression(stub.ApiTypeExpressionKind, stub.ApiTypeExpressionApiName),
                    ApiTypeModifiers.None,
                    stub.ClrName
                )
            ).ToList() ?? [];

            var apiRelationshipsString = this.ApiRelationshipStubCollection?
                .Select(x => $"{{ApiName={x.ApiName}}}")
                .SafeToDelimitedString(',') ?? "None";

            this.WriteLine($"API Relationships: [{apiRelationshipsString}]");
            this.WriteLine();

            this.ApiRelationshipCollection = this.ApiRelationshipStubCollection?.Select
            (
                stub => new ApiRelationship(stub.ApiName)
            ).ToList() ?? [];

            this.ApiObjectType = new ApiObjectType(nameof(Object), // Using 'Object' as a placeholder API name
                                                   this.ApiPropertyCollection ?? throw new ArgumentNullException(nameof(this.ApiPropertyCollection)),
                                                   this.ApiRelationshipCollection ?? throw new ArgumentNullException(nameof(this.ApiRelationshipCollection)),
                                                   typeof(object)); // Using object as a placeholder CLR type

            this.WriteLine($"TryGetMethod:  {this.TryGetMethod.SafeToString()}");
            this.WriteLine($"Input:         {this.Input.SafeToString()}");
            this.WriteLine($"ExpectedFound: {this.ExpectedFound.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            if (this.TryGetMethod is null)
                throw new InvalidOperationException($"{nameof(this.TryGetMethod)} is null.");

            this.ActualFound = this.TryGetMethod.Value switch
            {
                ApiObjectTypeTests.TryGetMethod.TryGetPropertyByApiName => this.ApiObjectType!.TryGetPropertyByApiName(this.Input!, out _),
                ApiObjectTypeTests.TryGetMethod.TryGetPropertyByClrName => this.ApiObjectType!.TryGetPropertyByClrName(this.Input!, out _),
                ApiObjectTypeTests.TryGetMethod.TryGetRelationshipByApiName => (bool?)this.ApiObjectType!.TryGetRelationshipByApiName(this.Input!, out _),
                _ => throw new InvalidOperationException($"Unknown {nameof(this.TryGetMethod)}: {this.TryGetMethod}"),
            };

            this.WriteLine($"ActualFound: {this.ActualFound.SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualFound.Should().NotBeNull();
            this.ActualFound.Should().Be(this.ExpectedFound);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] ConstructorThrowsTheoryData =>
    [
        // Duplicate API property names
        new ConstructorThrowsTest
        {
            Name = "Throws on duplicate API property names",
            ApiPropertyStubCollection =
            [
                new("Id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id"),
                new("Name", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Name1"),
                new("Name", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Name2") // Duplicate API Name
            ],
            ApiRelationshipStubCollection = [],
            ExpectedApiSchemaExceptionMessage = $"Unable to create {nameof(ApiObjectType)} because duplicate {nameof(ApiProperty)}.{nameof(ApiProperty.ApiName)} values detected: Name",
        },

        // Duplicate CLR property names
        new ConstructorThrowsTest
        {
            Name = "Throws on duplicate CLR property names",
            ApiPropertyStubCollection =
            [
                new("Id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id"),
                new("Name1", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Name"),
                new("Name2", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Name") // Duplicate CLR Name
            ],
            ApiRelationshipStubCollection = [],
            ExpectedApiSchemaExceptionMessage = $"Unable to create {nameof(ApiObjectType)} because duplicate {nameof(ApiProperty)}.{nameof(ApiProperty.ClrName)} values detected: Name",
        },

        // Duplicate API relationship names
        new ConstructorThrowsTest
        {
            Name = "Throws on duplicate API relationship names",
            ApiPropertyStubCollection =
            [
                new("Id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id"),
                new("Name1", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Name1"),
                new("Name2", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Name2")
            ],
            ApiRelationshipStubCollection =
            [
                new("RelatedObject"),
                new("RelatedObject") // Duplicate API Name
            ],
            ExpectedApiSchemaExceptionMessage = $"Unable to create {nameof(ApiObjectType)} because duplicate {nameof(ApiRelationship)}.{nameof(ApiRelationship.ApiName)} values detected: RelatedObject",
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetTheoryData =>
    [
        // TryGetPropertyByApiName
        new TryGetTest
        {
            Name = "TryGetPropertyByApiName works for known API property name and exact case",
            ApiPropertyStubCollection =
            [
                new("id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id"),
            ],
            ApiRelationshipStubCollection = [],
            TryGetMethod = TryGetMethod.TryGetPropertyByApiName,
            Input = "id",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetPropertyByApiName works for known API property name and case insensitivity",
            ApiPropertyStubCollection =
            [
                new("id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id"),
            ],
            ApiRelationshipStubCollection = [],
            TryGetMethod = TryGetMethod.TryGetPropertyByApiName,
            Input = "ID",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetPropertyByApiName works for unknown API property name",
            ApiPropertyStubCollection =
            [
                new("id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id"),
            ],
            ApiRelationshipStubCollection = [],
            TryGetMethod = TryGetMethod.TryGetPropertyByApiName,
            Input = "Name",
            ExpectedFound = false
        },

        // TryGetPropertyByClrName
        new TryGetTest
        {
            Name = "TryGetPropertyByClrName works for known API property name and exact case",
            ApiPropertyStubCollection =
            [
                new("id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id"),
            ],
            ApiRelationshipStubCollection = [],
            TryGetMethod = TryGetMethod.TryGetPropertyByClrName,
            Input = "Id",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetPropertyByClrName works for known API property name and case insensitivity",
            ApiPropertyStubCollection =
            [
                new("id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id"),
            ],
            ApiRelationshipStubCollection = [],
            TryGetMethod = TryGetMethod.TryGetPropertyByClrName,
            Input = "ID",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetPropertyByClrName works for unknown API property name",
            ApiPropertyStubCollection =
            [
                new("id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id"),
            ],
            ApiRelationshipStubCollection = [],
            TryGetMethod = TryGetMethod.TryGetPropertyByClrName,
            Input = "Name",
            ExpectedFound = false
        },

        // TryGetRelationshipByApiName
        new TryGetTest
        {
            Name = "TryGetRelationshipByApiName works for known API property name and exact case",
            ApiPropertyStubCollection = [],
            ApiRelationshipStubCollection =
            [
                new("rel")
            ],
            TryGetMethod = TryGetMethod.TryGetRelationshipByApiName,
            Input = "rel",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetRelationshipByApiName works for known API property name and case insensitivity",
            ApiPropertyStubCollection = [],
            ApiRelationshipStubCollection =
            [
                new("rel")
            ],
            TryGetMethod = TryGetMethod.TryGetRelationshipByApiName,
            Input = "REL",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetRelationshipByApiName works for unknown API property name",
            ApiPropertyStubCollection = [],
            ApiRelationshipStubCollection =
            [
                new("rel")
            ],
            TryGetMethod = TryGetMethod.TryGetRelationshipByApiName,
            Input = "Name",
            ExpectedFound = false
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(ConstructorThrowsTheoryData))]
    public void ConstructorThrows(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetTheoryData))]
    public void TryGet(IXUnitTest test) => test.Execute(this);
    #endregion
}
