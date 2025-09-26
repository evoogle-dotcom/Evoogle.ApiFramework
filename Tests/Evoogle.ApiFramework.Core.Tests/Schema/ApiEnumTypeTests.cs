// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public class ApiEnumTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    public class InitializeThrowsTest : XUnitTest
    {
        #region User Supplied Properties
        public List<ApiEnumValue>? ApiEnumValueCollection { get; init; }
        public Type? ClrEnumType { get; init; }
        public string? ExpectedApiSchemaExceptionMessage { get; init; }
        public List<string>? ExpectedValidationResults { get; init; }
        #endregion

        #region Calculated Properties
        private bool? ActualApiSchemaExceptionThrown { get; set; }
        private string? ActualApiSchemaExceptionMessage { get; set; }
        private List<string>? ActualValidationResults { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiEnumValuesString = this.ApiEnumValueCollection?
                .Select(x => $"{{{nameof(ApiEnumValue.ApiName)}={x.ApiName},{nameof(ApiEnumValue.ClrName)}={x.ClrName},{nameof(ApiEnumValue.ClrOrdinal)}={x.ClrOrdinal}}}")
                .SafeToDelimitedString(',') ?? "None";

            this.WriteLine($"API Enum Values: [{apiEnumValuesString}]");
            this.WriteLine();

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
                var apiEnumType = new ApiEnumType(nameof(Gender), // Using 'Gender' as a placeholder API name
                                                  this.ApiEnumValueCollection ?? throw new ArgumentNullException(nameof(this.ApiEnumValueCollection)),
                                                  this.ClrEnumType ?? throw new ArgumentNullException(nameof(this.ClrEnumType)));

                var apiSchema = new ApiSchema
                (
                    apiName: nameof(ApiSchema),
                    apiScalarTypes: null,
                    apiEnumTypes: [apiEnumType],
                    apiObjectTypes: null
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
        TryGetValueByApiName,
        TryGetValueByClrName,
        TryGetValueByClrOrdinal
    }

    public class TryGetTest : XUnitTest
    {
        #region User Supplied Properties
        public List<ApiEnumValue>? ApiEnumValueCollection { get; init; }
        public Type? ClrEnumType { get; init; }
        public TryGetMethod? TryGetMethod { get; init; }
        public object? Input { get; init; }
        public bool? ExpectedFound { get; init; }
        #endregion

        #region Calculated Properties
        private ApiEnumType? ApiEnumType { get; set; }
        private bool? ActualFound { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiEnumValuesString = this.ApiEnumValueCollection?
                .Select(x => $"{{{nameof(ApiEnumValue.ApiName)}={x.ApiName},{nameof(ApiEnumValue.ClrName)}={x.ClrName},{nameof(ApiEnumValue.ClrOrdinal)}={x.ClrOrdinal}}}")
                .SafeToDelimitedString(',') ?? "None";

            this.WriteLine($"API Enum Values: [{apiEnumValuesString}]");
            this.WriteLine();

            var apiEnumType = new ApiEnumType(nameof(Gender), // Using 'Gender' as a placeholder API name
                                              this.ApiEnumValueCollection ?? throw new ArgumentNullException(nameof(this.ApiEnumValueCollection)),
                                              this.ClrEnumType ?? throw new ArgumentNullException(nameof(this.ClrEnumType)));
            this.ApiEnumType = apiEnumType;

            var apiSchema = new ApiSchema
            (
                apiName: nameof(ApiSchema),
                apiScalarTypes: null,
                apiEnumTypes: [apiEnumType],
                apiObjectTypes: null
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

            var inputValue = this.Input;
            if (inputValue is JsonElement jsonElement)
            {
                switch (this.TryGetMethod.Value)
                {
                    case ApiEnumTypeTests.TryGetMethod.TryGetValueByApiName:
                    case ApiEnumTypeTests.TryGetMethod.TryGetValueByClrName:
                        inputValue = jsonElement.GetString();
                        break;
                    case ApiEnumTypeTests.TryGetMethod.TryGetValueByClrOrdinal:
                        inputValue = jsonElement.GetInt32();
                        break;
                }
            }

            this.ActualFound = this.TryGetMethod.Value switch
            {
                ApiEnumTypeTests.TryGetMethod.TryGetValueByApiName => this.ApiEnumType!.TryGetValueByApiName((string)inputValue!, out _),
                ApiEnumTypeTests.TryGetMethod.TryGetValueByClrName => this.ApiEnumType!.TryGetValueByClrName((string)inputValue!, out _),
                ApiEnumTypeTests.TryGetMethod.TryGetValueByClrOrdinal => (bool?)this.ApiEnumType!.TryGetValueByClrOrdinal((int)inputValue!, out _),
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
        // Duplicate API enum value names
        new InitializeThrowsTest
        {
            Name = "Throws on duplicate API enum value names",
            ApiEnumValueCollection =
            [
                new("unspecified", "Unspecified", 0),
                new("male", "Male", 1),
                new("female", "Female1", 2),
                new("female", "Female2", 3), // Duplicate API Name
            ],
            ClrEnumType = typeof(Gender),
            ExpectedApiSchemaExceptionMessage = $"{nameof(ApiSchema)} initialization failed.",
            ExpectedValidationResults =
            [
                $"{nameof(ApiEnumType)}[\"Evoogle.ApiFramework.Schema.TestData.Gender\"][\"Gender\"].{nameof(ApiEnumValue)} unable to initialize because duplicate {nameof(ApiEnumValue.ApiName)} values detected: female"
            ]
        },

        // Duplicate CLR enum value names
        new InitializeThrowsTest
        {
            Name = "Throws on duplicate CLR enum value names",
            ApiEnumValueCollection =
            [
                new("unspecified", "Unspecified", 0),
                new("male", "Male", 1),
                new("female1", "Female", 2),
                new("female2", "Female", 3), // Duplicate CLR Name
            ],
            ClrEnumType = typeof(Gender),
            ExpectedApiSchemaExceptionMessage = $"{nameof(ApiSchema)} initialization failed.",
            ExpectedValidationResults =
            [
                $"{nameof(ApiEnumType)}[\"Evoogle.ApiFramework.Schema.TestData.Gender\"][\"Gender\"].{nameof(ApiEnumValue)} unable to initialize because duplicate {nameof(ApiEnumValue.ClrName)} values detected: Female"
            ]
        },

        // Duplicate CLR enum value ordinals
        new InitializeThrowsTest
        {
            Name = "Throws on duplicate CLR enum value ordinals",
            ApiEnumValueCollection =
            [
                new("unspecified", "Unspecified", 0),
                new("male", "Male", 1),
                new("female", "Female", 2),
                new("alien", "Alien", 2), // Duplicate CLR Ordinal
            ],
            ClrEnumType = typeof(Gender),
            ExpectedApiSchemaExceptionMessage = $"{nameof(ApiSchema)} initialization failed.",
            ExpectedValidationResults =
            [
                $"{nameof(ApiEnumType)}[\"Evoogle.ApiFramework.Schema.TestData.Gender\"][\"Gender\"].{nameof(ApiEnumValue)} unable to initialize because duplicate {nameof(ApiEnumValue.ClrOrdinal)} values detected: 2"
            ]
        },

        // CLR type is not an enum
        new InitializeThrowsTest
        {
            Name = "Throws if CLR type is not an enum",
            ApiEnumValueCollection =
            [
                new("unspecified", "Unspecified", 0),
                new("male", "Male", 1),
                new("female", "Female", 2)
            ],
            ClrEnumType = typeof(string), // Using a non-enum type
            ExpectedApiSchemaExceptionMessage = $"{nameof(ApiSchema)} initialization failed.",
            ExpectedValidationResults =
            [
                $"{nameof(ApiEnumType)}[\"System.String\"][\"Gender\"].{nameof(ApiEnumType.ClrType)} must be a CLR enum type."
            ]
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetTheoryData =>
    [
        // TryGetValueByApiName
        new TryGetTest
        {
            Name = "TryGetValueByApiName works for known API enum value name and exact case",
            ApiEnumValueCollection =
            [
                new("unspecified", "Unspecified", 0),
                new("male", "Male", 1),
                new("female", "Female", 2),
            ],
            ClrEnumType = typeof(Gender),
            TryGetMethod = TryGetMethod.TryGetValueByApiName,
            Input = "male",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetValueByApiName works for known API enum value name and case insensitivity",
            ApiEnumValueCollection =
            [
                new("unspecified", "Unspecified", 0),
                new("male", "Male", 1),
                new("female", "Female", 2),
            ],
            ClrEnumType = typeof(Gender),
            TryGetMethod = TryGetMethod.TryGetValueByApiName,
            Input = "MALE",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetValueByApiName works for unknown API enum value name",
            ApiEnumValueCollection =
            [
                new("unspecified", "Unspecified", 0),
                new("male", "Male", 1),
                new("female", "Female", 2),
            ],
            ClrEnumType = typeof(Gender),
            TryGetMethod = TryGetMethod.TryGetValueByApiName,
            Input = "alien",
            ExpectedFound = false
        },

        // TryGetValueByClrName
        new TryGetTest
        {
            Name = "TryGetValueByClrName works for known CLR enum value name and exact case",
            ApiEnumValueCollection =
            [
                new("unspecified", "Unspecified", 0),
                new("male", "Male", 1),
                new("female", "Female", 2),
            ],
            ClrEnumType = typeof(Gender),
            TryGetMethod = TryGetMethod.TryGetValueByClrName,
            Input = "Male",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetValueByClrName works for known CLR enum value name and case insensitivity",
            ApiEnumValueCollection =
            [
                new("unspecified", "Unspecified", 0),
                new("male", "Male", 1),
                new("female", "Female", 2),
            ],
            ClrEnumType = typeof(Gender),
            TryGetMethod = TryGetMethod.TryGetValueByClrName,
            Input = "MALE",
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetValueByClrName works for unknown CLR enum value name",
            ApiEnumValueCollection =
            [
                new("unspecified", "Unspecified", 0),
                new("male", "Male", 1),
                new("female", "Female", 2),
            ],
            ClrEnumType = typeof(Gender),
            TryGetMethod = TryGetMethod.TryGetValueByClrName,
            Input = "Alien",
            ExpectedFound = false
        },

        // TryGetValueByClrOrdinal
        new TryGetTest
        {
            Name = "TryGetValueByClrOrdinal works for known CLR enum value ordinal",
            ApiEnumValueCollection =
            [
                new("unspecified", "Unspecified", 0),
                new("male", "Male", 1),
                new("female", "Female", 2),
            ],
            ClrEnumType = typeof(Gender),
            TryGetMethod = TryGetMethod.TryGetValueByClrOrdinal,
            Input = 1,
            ExpectedFound = true
        },

        new TryGetTest
        {
            Name = "TryGetValueByClrOrdinal works for unknown CLR enum value ordinal",
            ApiEnumValueCollection =
            [
                new("unspecified", "Unspecified", 0),
                new("male", "Male", 1),
                new("female", "Female", 2),
            ],
            ClrEnumType = typeof(Gender),
            TryGetMethod = TryGetMethod.TryGetValueByClrOrdinal,
            Input = 3,
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
