// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public class ApiObjectTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    public record struct ApiPropertyStub(string ApiName, ApiTypeKind ApiTypeExpressionKind, string ApiTypeExpressionApiName, ApiTypeModifiers ApiTypeModifiers, string ClrName, Type ApiTypeExpressionClrType);
    public record struct ApiRelationshipStub(string ApiName);

    public class InitializeThrowsTest : XUnitTest
    {
        #region User Supplied Properties
        public List<ApiPropertyStub>? ApiPropertyStubCollection { get; init; }
        public List<ApiRelationshipStub>? ApiRelationshipStubCollection { get; init; }
        public string? ExpectedApiSchemaExceptionMessage { get; init; }
        public List<string>? ExpectedValidationResults { get; init; }
        #endregion

        #region Calculated Properties
        private List<ApiProperty>? ApiPropertyCollection { get; set; }
        private List<ApiRelationship>? ApiRelationshipCollection { get; set; }
        private bool? ActualApiSchemaExceptionThrown { get; set; }
        private string? ActualApiSchemaExceptionMessage { get; set; }
        private List<string>? ActualValidationResults { get; set; }
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
            if (this.ExpectedValidationResults is not null)
            {
                foreach (var expectedValidationResult in this.ExpectedValidationResults)
                {
                    this.WriteLine($"Expected Validation Result: {expectedValidationResult.SafeToString()}");
                }
            }
            this.WriteLine();
        }

        protected override void Act()
        {
            try
            {
                var apiScalarTypes = this.ApiPropertyStubCollection
                    ?.Where(x => x.ApiTypeExpressionKind == ApiTypeKind.Scalar)
                    ?.GroupBy(x => x.ApiTypeExpressionClrType)
                    ?.Select(x => x.First())
                    ?.Select(x => new ApiScalarType(x.ApiTypeExpressionApiName, x.ApiTypeExpressionClrType)).ToList() ?? [];

                var apiObjectType = new ApiObjectType(nameof(Object), // Using 'Object' as a placeholder API name
                                                      this.ApiPropertyCollection ?? throw new ArgumentNullException(nameof(this.ApiPropertyCollection)),
                                                      this.ApiRelationshipCollection ?? throw new ArgumentNullException(nameof(this.ApiRelationshipCollection)),
                                                      typeof(object)); // Using 'object' as a placeholder CLR type

                var apiSchema = new ApiSchema
                (
                    apiName: nameof(ApiSchema),
                    apiScalarTypes: apiScalarTypes,
                    apiEnumTypes: null,
                    apiObjectTypes: [apiObjectType]
                );
                var result = apiSchema.Initialize();
                result.ThrowIfInvalid();

            }
            catch (ApiSchemaValidationException ex)
            {
                this.ActualApiSchemaExceptionThrown = true;
                this.ActualApiSchemaExceptionMessage = ex.Message;
                this.ActualValidationResults = [.. ex.ValidationResults.Where(x => x.ErrorMessage is not null).Select(x => x.ErrorMessage!)];
            }
            catch (ApiSchemaException ex)
            {
                this.ActualApiSchemaExceptionThrown = true;
                this.ActualApiSchemaExceptionMessage = ex.Message;
                this.ActualValidationResults = null;
            }

            this.WriteLine($"Actual Exception Thrown:  {this.ActualApiSchemaExceptionThrown.SafeToString()}");
            this.WriteLine($"Actual Exception Message: {this.ActualApiSchemaExceptionMessage.SafeToString()}");
            if (this.ActualValidationResults is not null)
            {
                foreach (var actualValidationResult in this.ActualValidationResults)
                {
                    this.WriteLine($"Actual Validation Result: {actualValidationResult.SafeToString()}");
                }
            }
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualApiSchemaExceptionThrown.Should().BeTrue();
            this.ActualApiSchemaExceptionMessage.Should().Be(this.ExpectedApiSchemaExceptionMessage);
            this.ActualValidationResults.Should().NotBeNull();
            this.ActualValidationResults.Should().BeEquivalentTo(this.ExpectedValidationResults);
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

            var apiScalarTypes = this.ApiPropertyStubCollection
                ?.Where(x => x.ApiTypeExpressionKind == ApiTypeKind.Scalar)
                ?.GroupBy(x => x.ApiTypeExpressionClrType)
                ?.FirstOrDefault()
                ?.Select(x => new ApiScalarType(x.ApiTypeExpressionApiName, x.ApiTypeExpressionClrType)).ToList() ?? [];

            var apiObjectType = new ApiObjectType(nameof(Object), // Using 'Object' as a placeholder API name
                                                  this.ApiPropertyCollection ?? throw new ArgumentNullException(nameof(this.ApiPropertyCollection)),
                                                  this.ApiRelationshipCollection ?? throw new ArgumentNullException(nameof(this.ApiRelationshipCollection)),
                                                  typeof(object)); // Using object as a placeholder CLR type
            this.ApiObjectType = apiObjectType;

            var apiSchema = new ApiSchema
            (
                apiName: nameof(ApiSchema),
                apiScalarTypes: apiScalarTypes,
                apiEnumTypes: null,
                apiObjectTypes: [apiObjectType]
            );
            var result = apiSchema.Initialize();
            result.ThrowIfInvalid();

            this.WriteLine($"TryGetMethod:  {this.TryGetMethod.SafeToString()}");
            this.WriteLine($"Input:         {this.Input.SafeToString()}");
            this.WriteLine($"ExpectedFound: {this.ExpectedFound.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            if (this.TryGetMethod is null)
            {
                throw new InvalidOperationException($"{nameof(this.TryGetMethod)} is null.");
            }

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
    public static TheoryDataRow<IXUnitTest>[] InitializeThrowsTheoryData =>
    [
        // Duplicate API property names
        new InitializeThrowsTest
        {
            Name = "Throws on duplicate API property names",
            ApiPropertyStubCollection =
            [
                new("Id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id", typeof(int)),
                new("Name", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Name1", typeof(string)),
                new("Name", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Name2", typeof(string)) // Duplicate API Name
            ],
            ApiRelationshipStubCollection = [],
            ExpectedApiSchemaExceptionMessage = $"{nameof(ApiSchema)} initialization failed.",
            ExpectedValidationResults =
            [
                $"{nameof(ApiObjectType)}[\"System.Object\"][\"Object\"].{nameof(ApiProperty)} unable to initialize because duplicate {nameof(ApiProperty.ApiName)} values detected: Name"
            ]
        },

        // Duplicate CLR property names
        new InitializeThrowsTest
        {
            Name = "Throws on duplicate CLR property names",
            ApiPropertyStubCollection =
            [
                new("Id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id", typeof(int)),
                new("Name1", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Name", typeof(string)),
                new("Name2", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Name", typeof(string)) // Duplicate CLR Name
            ],
            ApiRelationshipStubCollection = [],
            ExpectedApiSchemaExceptionMessage = $"{nameof(ApiSchema)} initialization failed.",
            ExpectedValidationResults =
            [
                $"{nameof(ApiObjectType)}[\"System.Object\"][\"Object\"].{nameof(ApiProperty)} unable to initialize because duplicate {nameof(ApiProperty.ClrName)} values detected: Name"
            ]
        },

        // Duplicate API relationship names
        new InitializeThrowsTest
        {
            Name = "Throws on duplicate API relationship names",
            ApiPropertyStubCollection =
            [
                new("Id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id", typeof(int)),
                new("Name1", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Name1", typeof(string)),
                new("Name2", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Name2", typeof(string))
            ],
            ApiRelationshipStubCollection =
            [
                new("Name2"),
                new("Name2") // Duplicate API Name
            ],
            ExpectedApiSchemaExceptionMessage = $"{nameof(ApiSchema)} initialization failed.",
            ExpectedValidationResults =
            [
                $"{nameof(ApiObjectType)}[\"System.Object\"][\"Object\"].{nameof(ApiRelationship)} unable to initialize because duplicate {nameof(ApiRelationship.ApiName)} values detected: Name2"
            ]
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
                new("id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id", typeof(int)),
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
                new("id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id", typeof(int)),
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
                new("id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id", typeof(int)),
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
                new("id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id", typeof(int)),
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
                new("id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id", typeof(int)),
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
                new("id", ApiTypeKind.Scalar, nameof(Int32), ApiTypeModifiers.None, "Id", typeof(int)),
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
            ApiPropertyStubCollection =
            [
                new("rel", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Rel", typeof(string)),
            ],
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
            ApiPropertyStubCollection =
            [
                new("rel", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Rel", typeof(string)),
            ],
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
            ApiPropertyStubCollection =
            [
                new("rel", ApiTypeKind.Scalar, nameof(String), ApiTypeModifiers.None, "Rel", typeof(string)),
            ],
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
    [MemberData(nameof(InitializeThrowsTheoryData))]
    public void InitializeThrows(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetTheoryData))]
    public void TryGet(IXUnitTest test) => test.Execute(this);
    #endregion
}
