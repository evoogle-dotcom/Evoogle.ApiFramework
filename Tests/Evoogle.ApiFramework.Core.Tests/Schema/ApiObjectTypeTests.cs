// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public class ApiObjectTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    // private static JsonSerializerOptions DefaultJsonSerializerOptions { get; } = new()
    // {
    //     WriteIndented = false,
    //     DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    // };

    #region Test Types
    // public class InitializeThrowsTest : XUnitTest
    // {
    //     #region User Supplied Properties
    //     public JsonSerializerOptions JsonSerializerOptions { get; init; } = DefaultJsonSerializerOptions;
    //     public string? Source { get; init; }
    //     public string? ExpectedApiSchemaExceptionMessage { get; init; }
    //     public List<string>? ExpectedValidationResults { get; init; }
    //     #endregion

    //     #region Calculated Properties
    //     private List<ApiProperty>? ApiPropertyCollection { get; set; }
    //     private List<ApiRelationship>? ApiRelationshipCollection { get; set; }
    //     private bool? ActualApiSchemaExceptionThrown { get; set; }
    //     private string? ActualApiSchemaExceptionMessage { get; set; }
    //     private List<string>? ActualValidationResults { get; set; }
    //     #endregion

    //     #region XUnitTest Methods
    //     protected override void Arrange()
    //     {
    //         var apiPropertiesString = this.ApiPropertyStubCollection?
    //             .Select(x => $"{{ApiName={x.ApiName},ClrName={x.ClrName}}}")
    //             .SafeToDelimitedString(',') ?? "None";

    //         this.WriteLine($"API Properties: [{apiPropertiesString}]");
    //         this.WriteLine();

    //         this.ApiPropertyCollection = this.ApiPropertyStubCollection?.Select
    //         (
    //             stub => new ApiProperty
    //             (
    //                 stub.ApiName,
    //                 new ApiTypeExpression(stub.ApiTypeExpressionKind, stub.ApiTypeExpressionApiName),
    //                 ApiTypeModifiers.None,
    //                 stub.ClrName
    //             )
    //         ).ToList() ?? [];

    //         var apiRelationshipsString = this.ApiRelationshipStubCollection?
    //             .Select(x => $"{{ApiName={x.ApiName}}}")
    //             .SafeToDelimitedString(',') ?? "None";

    //         this.WriteLine($"API Relationships: [{apiRelationshipsString}]");
    //         this.WriteLine();

    //         this.ApiRelationshipCollection = this.ApiRelationshipStubCollection?.Select
    //         (
    //             stub => new ApiRelationship(stub.ApiName)
    //         ).ToList() ?? [];

    //         this.WriteLine($"Expected Exception Message: {this.ExpectedApiSchemaExceptionMessage.SafeToString()}");
    //         if (this.ExpectedValidationResults is not null)
    //         {
    //             foreach (var expectedValidationResult in this.ExpectedValidationResults)
    //             {
    //                 this.WriteLine($"Expected Validation Result: {expectedValidationResult.SafeToString()}");
    //             }
    //         }
    //         this.WriteLine();
    //     }

    //     protected override void Act()
    //     {
    //         try
    //         {
    //             var apiScalarTypes = this.ApiPropertyStubCollection
    //                 ?.Where(x => x.ApiTypeExpressionKind == ApiTypeKind.Scalar)
    //                 ?.GroupBy(x => x.ApiTypeExpressionClrType)
    //                 ?.Select(x => x.First())
    //                 ?.Select(x => new ApiScalarType(x.ApiTypeExpressionApiName, x.ApiTypeExpressionClrType)).ToList() ?? [];

    //             var apiObjectType = new ApiObjectType(nameof(Object), // Using 'Object' as a placeholder API name
    //                                                   this.ApiPropertyCollection ?? throw new ArgumentNullException(nameof(this.ApiPropertyCollection)),
    //                                                   this.ApiRelationshipCollection ?? throw new ArgumentNullException(nameof(this.ApiRelationshipCollection)),
    //                                                   typeof(object)); // Using 'object' as a placeholder CLR type

    //             var apiSchema = ApiSchema.Create
    //             (
    //                 apiName: nameof(ApiSchema),
    //                 apiScalarTypes: apiScalarTypes,
    //                 apiEnumTypes: null,
    //                 apiObjectTypes: [apiObjectType]
    //             );
    //             var result = apiSchema.Initialize();
    //             result.ThrowIfInvalid();

    //         }
    //         catch (ApiSchemaValidationException ex)
    //         {
    //             this.ActualApiSchemaExceptionThrown = true;
    //             this.ActualApiSchemaExceptionMessage = ex.Message;
    //             this.ActualValidationResults = [.. ex.ValidationResults.Where(x => x.ErrorMessage is not null).Select(x => x.ErrorMessage!)];
    //         }
    //         catch (ApiSchemaException ex)
    //         {
    //             this.ActualApiSchemaExceptionThrown = true;
    //             this.ActualApiSchemaExceptionMessage = ex.Message;
    //             this.ActualValidationResults = null;
    //         }

    //         this.WriteLine($"Actual Exception Thrown:  {this.ActualApiSchemaExceptionThrown.SafeToString()}");
    //         this.WriteLine($"Actual Exception Message: {this.ActualApiSchemaExceptionMessage.SafeToString()}");
    //         if (this.ActualValidationResults is not null)
    //         {
    //             foreach (var actualValidationResult in this.ActualValidationResults)
    //             {
    //                 this.WriteLine($"Actual Validation Result: {actualValidationResult.SafeToString()}");
    //             }
    //         }
    //         this.WriteLine();
    //     }

    //     protected override void Assert()
    //     {
    //         this.ActualApiSchemaExceptionThrown.Should().BeTrue();
    //         this.ActualApiSchemaExceptionMessage.Should().Be(this.ExpectedApiSchemaExceptionMessage);
    //         this.ActualValidationResults.Should().NotBeNull();
    //         this.ActualValidationResults.Should().BeEquivalentTo(this.ExpectedValidationResults);
    //     }
    //     #endregion
    // }

    public enum TryGetMethod
    {
        TryGetPropertyByApiName,
        TryGetPropertyByClrName,
        TryGetRelationshipByApiName
    }

    public class TryGetTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiTestSchemaKind ApiSchemaKind { get; init; }
        public required string ApiObjectTypeName { get; init; }
        public required TryGetMethod TryGetMethod { get; init; }
        public required string Input { get; init; }
        public required bool ExpectedFound { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchema { get; set; }
        private ApiObjectType? ApiObjectType { get; set; }
        private bool? ActualFound { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = ApiTestSchemaFactory.BuildTestSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            var apiObjectType = this.ApiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName);
            this.ApiObjectType = apiObjectType ?? throw new InvalidOperationException($"{nameof(Schema.ApiObjectType)} '{this.ApiObjectTypeName}' not found in ApiSchema.");

            this.WriteLine($"ApiSchema:     {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType: {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"TryGetMethod:  {this.TryGetMethod.SafeToString()}");
            this.WriteLine($"Input:         {this.Input.SafeToString()}");
            this.WriteLine($"ExpectedFound: {this.ExpectedFound.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualFound = this.TryGetMethod switch
            {
                TryGetMethod.TryGetPropertyByApiName => this.ApiObjectType!.TryGetPropertyByApiName(this.Input!, out _),
                TryGetMethod.TryGetPropertyByClrName => this.ApiObjectType!.TryGetPropertyByClrName(this.Input!, out _),
                TryGetMethod.TryGetRelationshipByApiName => this.ApiObjectType!.TryGetRelationshipByApiName(this.Input!, out _),
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
    // public static TheoryDataRow<IXUnitTest>[] InitializeThrowsTheoryData =>
    // [
    //     // Duplicate API property names
    //     new InitializeThrowsTest
    //     {
    //         Name = "Throws on duplicate API property names",
    //         ApiPropertyStubCollection =
    //         [
    //             new("Id", ApiTypeKind.Scalar, nameof(Int32), typeof(int), ApiTypeModifiers.None, "Id"),
    //             new("Name", ApiTypeKind.Scalar, nameof(String), typeof(string), ApiTypeModifiers.None, "Name1"),
    //             new("Name", ApiTypeKind.Scalar, nameof(String), typeof(string), ApiTypeModifiers.None, "Name2") // Duplicate API Name
    //         ],
    //         ApiRelationshipStubCollection = [],
    //         ExpectedApiSchemaExceptionMessage = $"{nameof(ApiSchema)} initialization failed.",
    //         ExpectedValidationResults =
    //         [
    //             $"{nameof(ApiObjectType)}[\"System.Object\"][\"Object\"].{nameof(ApiProperty)} unable to initialize because duplicate {nameof(ApiProperty.ApiName)} values detected: Name"
    //         ]
    //     },

    //     // Duplicate CLR property names
    //     new InitializeThrowsTest
    //     {
    //         Name = "Throws on duplicate CLR property names",
    //         ApiPropertyStubCollection =
    //         [
    //             new("Id", ApiTypeKind.Scalar, nameof(Int32), typeof(int), ApiTypeModifiers.None, "Id"),
    //             new("Name1", ApiTypeKind.Scalar, nameof(String), typeof(string), ApiTypeModifiers.None, "Name"),
    //             new("Name2", ApiTypeKind.Scalar, nameof(String), typeof(string), ApiTypeModifiers.None, "Name") // Duplicate CLR Name
    //         ],
    //         ApiRelationshipStubCollection = [],
    //         ExpectedApiSchemaExceptionMessage = $"{nameof(ApiSchema)} initialization failed.",
    //         ExpectedValidationResults =
    //         [
    //             $"{nameof(ApiObjectType)}[\"System.Object\"][\"Object\"].{nameof(ApiProperty)} unable to initialize because duplicate {nameof(ApiProperty.ClrName)} values detected: Name"
    //         ]
    //     },

    //     // Duplicate API relationship names
    //     new InitializeThrowsTest
    //     {
    //         Name = "Throws on duplicate API relationship names",
    //         ApiPropertyStubCollection =
    //         [
    //             new("Id", ApiTypeKind.Scalar, nameof(Int32), typeof(int), ApiTypeModifiers.None, "Id"),
    //             new("Name1", ApiTypeKind.Scalar, nameof(String), typeof(string), ApiTypeModifiers.None, "Name1"),
    //             new("Name2", ApiTypeKind.Scalar, nameof(String), typeof(string), ApiTypeModifiers.None, "Name2")
    //         ],
    //         ApiRelationshipStubCollection =
    //         [
    //             new("Name2"),
    //             new("Name2") // Duplicate API Name
    //         ],
    //         ExpectedApiSchemaExceptionMessage = $"{nameof(ApiSchema)} initialization failed.",
    //         ExpectedValidationResults =
    //         [
    //             $"{nameof(ApiObjectType)}[\"System.Object\"][\"Object\"].{nameof(ApiRelationship)} unable to initialize because duplicate {nameof(ApiRelationship.ApiName)} values detected: Name2"
    //         ]
    //     },
    // ];

    // public class TryGetTest : XUnitTest
    // {
    //     #region User Supplied Properties
    //     public required ApiTestSchemaKind ApiSchemaKind { get; init; }
    //     public required string ApiObjectTypeName { get; init; }
    //     public required string Input { get; init; }
    //     public required bool ExpectedSuccess { get; init; }
    //     public required TryGetMethod TryGetMethod { get; init; }
    //     public required bool ExpectedFound { get; init; }
    //     #endregion
    // }

    public static TheoryDataRow<IXUnitTest>[] TryGetTheoryData =>
    [
        // TryGetPropertyByApiName
        new TryGetTest
        {
            Name = "TryGetPropertyByApiName works for known API property name and exact case",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByApiName,
            Input = "RequiredName",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetPropertyByApiName works for known API property name and case insensitivity",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByApiName,
            Input = "requiredname",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetPropertyByApiName works for unknown API property name",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByApiName,
            Input = "Unknown_Property",
            ExpectedFound = false
        },

        // TryGetPropertyByClrName
        new TryGetTest
        {
            Name = "TryGetPropertyByClrName works for known API property name and exact case",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByClrName,
            Input = "OptionalName",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetPropertyByClrName works for known API property name and case insensitivity",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByClrName,
            Input = "OPTIONALNAME",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetPropertyByClrName works for unknown API property name",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            TryGetMethod = TryGetMethod.TryGetPropertyByClrName,
            Input = "Unknown_Property",
            ExpectedFound = false
        },

        // TryGetRelationshipByApiName
        new TryGetTest
        {
            Name = "TryGetRelationshipByApiName works for known API property name and exact case",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            TryGetMethod = TryGetMethod.TryGetRelationshipByApiName,
            Input = "Company_Owner",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetRelationshipByApiName works for known API property name and case insensitivity",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            TryGetMethod = TryGetMethod.TryGetRelationshipByApiName,
            Input = "COMPANY_OWNER",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetRelationshipByApiName works for unknown API property name",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(Company),
            TryGetMethod = TryGetMethod.TryGetRelationshipByApiName,
            Input = "Unknown_Relationship",
            ExpectedFound = false
        },
    ];
    #endregion

    #region Test Methods
    // [Theory]
    // [MemberData(nameof(InitializeThrowsTheoryData))]
    // public void InitializeThrows(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetTheoryData))]
    public void TryGet(IXUnitTest test) => test.Execute(this);
    #endregion
}
