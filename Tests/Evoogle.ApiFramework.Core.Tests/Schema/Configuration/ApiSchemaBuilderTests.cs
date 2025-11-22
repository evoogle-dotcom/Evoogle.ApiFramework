// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiSchemaBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    public class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public string ApiName { get; init; } = null!;
        public string? ApiVersion { get; init; }
        public ApiType[]? ApiScalarTypes { get; init; }
        public ApiType[]? ApiEnumTypes { get; init; }
        public ApiType[]? ApiObjectTypes { get; init; }
        public Type? ApiExtensionType { get; init; }

        public ApiSchema? ApiSchemaExpected { get; init; } = null!;
        public string? ApiSchemaExceptionMessageExpected { get; init; }
        public List<string>? ApiSchemaValidationResultsExpected { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchemaActual { get; set; }
        private bool? ApiSchemaExceptionThrown { get; set; }
        private string? ApiSchemaExceptionMessageActual { get; set; }
        private List<string>? ApiSchemaValidationResultsActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"ApiName: {this.ApiName.SafeToString()}");
            this.WriteLine($"ApiVersion: {this.ApiVersion.SafeToString()}");
            this.WriteLine($"ApiExtensionType: {this.ApiExtensionType.SafeToName()}");
            this.WriteLine();

            if (this.ApiSchemaExpected is not null)
            {
                this.WriteLine($"Expected: {this.ApiSchemaExpected.SafeToString()}");
            }

            if (this.ApiSchemaExceptionMessageExpected is not null)
            {
                this.WriteLine($"Expected Exception Message: {this.ApiSchemaExceptionMessageExpected.SafeToString()}");
            }

            if (this.ApiSchemaValidationResultsExpected is not null)
            {
                foreach (var apiSchemaValidationResult in this.ApiSchemaValidationResultsExpected)
                {
                    this.WriteLine($"Expected Validation Result: {apiSchemaValidationResult.SafeToString()}");
                }
            }
            this.WriteLine();
        }

        protected override void Act()
        {
            var builder = new ApiSchemaBuilder()
                .WithName(this.ApiName);

            if (this.ApiVersion != null)
            {
                builder = builder.WithVersion(this.ApiVersion);
            }

            foreach (var apiScalarType in this.ApiScalarTypes.SafeCast<ApiScalarType>())
            {
                var apiName = apiScalarType.ApiName;
                var clrType = apiScalarType.ClrType;
                builder = builder.AddScalar(clrType, x => x.WithName(apiName));
            }

            foreach (var apiEnumType in this.ApiEnumTypes.SafeCast<ApiEnumType>())
            {
                var apiName = apiEnumType.ApiName;
                var clrType = apiEnumType.ClrType;
                builder = builder.AddEnum(clrType, x =>
                {
                    x.WithName(apiName);
                    foreach (var apiEnumValue in apiEnumType.ApiEnumValues)
                    {
                        x.AddValue(apiEnumValue.ApiName, apiEnumValue.ClrName, apiEnumValue.ClrOrdinal);
                    }
                });
            }

            foreach (var apiObjectType in this.ApiObjectTypes.SafeCast<ApiObjectType>())
            {
                var apiName = apiObjectType.ApiName;
                var clrType = apiObjectType.ClrType;
                builder = builder.AddObject(clrType, x =>
                {
                    x.WithName(apiName);
                    foreach (var apiProperty in apiObjectType.ApiProperties)
                    {
                        x.AddProperty(apiProperty.ApiName, apiProperty.ClrName, y =>
                        {
                            if (apiProperty.IsRequired())
                            {
                                y.Required();
                            }
                            else
                            {
                                y.Optional();
                            }
                        });
                    }
                    foreach (var apiRelationship in apiObjectType.ApiRelationships)
                    {
                        x.AddRelationship(apiRelationship.ApiName, apiRelationship.ApiPropertyName);
                    }
                });
            }

            if (this.ApiExtensionType != null)
            {
                var extension = Activator.CreateInstance(this.ApiExtensionType);
                builder.AddExtension(this.ApiExtensionType, extension!);
            }

            try
            {
                this.ApiSchemaActual = builder.Build();
                this.WriteLine($"Actual:   {this.ApiSchemaActual.SafeToString()}");
            }
            catch (ApiSchemaValidationException ex)
            {
                this.ApiSchemaExceptionThrown = true;
                this.ApiSchemaExceptionMessageActual = ex.Message;
                this.ApiSchemaValidationResultsActual = [.. ex.ValidationResults.Where(x => x.ErrorMessage is not null).Select(x => x.ErrorMessage!)];

                this.WriteLine($"Actual Exception Message: {this.ApiSchemaExceptionMessageActual.SafeToString()}");

                foreach (var apiSchemaValidationResult in this.ApiSchemaValidationResultsActual)
                {
                    this.WriteLine($"Actual Validation Result: {apiSchemaValidationResult.SafeToString()}");
                }

            }
            catch (ApiSchemaException ex)
            {
                this.ApiSchemaExceptionThrown = true;
                this.ApiSchemaExceptionMessageActual = ex.Message;
                this.ApiSchemaValidationResultsActual = null;

                this.WriteLine($"Actual Exception Message: {this.ApiSchemaExceptionMessageActual.SafeToString()}");
            }
        }

        protected override void Assert()
        {
            if (!this.ApiSchemaExceptionThrown.GetValueOrDefault())
            {
                this.ApiSchemaExceptionMessageActual.Should().BeNull();
                this.ApiSchemaValidationResultsActual.Should().BeNull();

                this.ApiSchemaActual.Should().NotBeNull();
                this.ApiSchemaActual.Should().BeEquivalentTo
                (
                    this.ApiSchemaExpected,
                    opt => opt
                        .WithStrictOrdering()
                );
            }
            else
            {
                this.ApiSchemaActual.Should().BeNull();

                this.ApiSchemaExceptionMessageActual.Should().NotBeNull();
                this.ApiSchemaExceptionMessageActual.Should().Be(this.ApiSchemaExceptionMessageExpected);

                this.ApiSchemaValidationResultsActual.Should().NotBeNull();
                this.ApiSchemaValidationResultsActual.Should().BeEquivalentTo(this.ApiSchemaValidationResultsExpected);
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    private static ApiType ApiBooleanScalarType { get; } = new ApiScalarType(nameof(Boolean), typeof(bool));
    private static ApiType ApiInt32ScalarType { get; } = new ApiScalarType(nameof(Int32), typeof(int));
    private static ApiType ApiInt64ScalarType { get; } = new ApiScalarType(nameof(Int64), typeof(long));
    private static ApiType ApiStringScalarType { get; } = new ApiScalarType(nameof(String), typeof(string));

    private static ApiType ApiGenderEnumType { get; } = new ApiEnumType
    (
        nameof(Gender),
        [
            new (nameof(Gender.Unspecified), nameof(Gender.Unspecified), (int)Gender.Unspecified),
            new (nameof(Gender.Male), nameof(Gender.Male), (int)Gender.Male),
            new (nameof(Gender.Female), nameof(Gender.Female), (int)Gender.Female),
        ],
        typeof(Gender)
    );

    private static ApiType ApiScalarsOnlyObjectType { get; } = new ApiObjectType
    (
        nameof(ScalarsOnly),
        [
            new (nameof(ScalarsOnly.RequiredName), ApiTypeExpression.ClrRef<string>(), ApiTypeModifiers.Required, nameof(ScalarsOnly.RequiredName)),
            new (nameof(ScalarsOnly.RequiredNumber), ApiTypeExpression.ClrRef<long>(), ApiTypeModifiers.Required, nameof(ScalarsOnly.RequiredNumber)),
            new (nameof(ScalarsOnly.RequiredPredicate), ApiTypeExpression.ClrRef<bool>(), ApiTypeModifiers.Required, nameof(ScalarsOnly.RequiredPredicate)),
            new (nameof(ScalarsOnly.OptionalName), ApiTypeExpression.ClrRef<string>(), ApiTypeModifiers.None, nameof(ScalarsOnly.OptionalName)),
            new (nameof(ScalarsOnly.OptionalNumber), ApiTypeExpression.ClrRef<long>(), ApiTypeModifiers.None, nameof(ScalarsOnly.OptionalNumber)),
            new (nameof(ScalarsOnly.OptionalPredicate), ApiTypeExpression.ClrRef<bool>(), ApiTypeModifiers.None, nameof(ScalarsOnly.OptionalPredicate)),
        ],
        [],
        typeof(ScalarsOnly)
    );

    private static ApiType ApiPersonObjectType { get; } = new ApiObjectType
    (
        nameof(Person),
        [
            new (nameof(Person.Name), ApiTypeExpression.ClrRef<string>(), ApiTypeModifiers.Required, nameof(Person.Name)),
            new (nameof(Person.Age), ApiTypeExpression.ClrRef<int>(), ApiTypeModifiers.None, nameof(Person.Age)),
            new (nameof(Person.Gender), ApiTypeExpression.ClrRef<Gender>(), ApiTypeModifiers.None, nameof(Person.Gender)),
            new (nameof(Person.Hobbies), ApiTypeExpression.ListOf<string>(ApiTypeModifiers.Required), ApiTypeModifiers.None, nameof(Person.Hobbies)),
        ],
        [],
        typeof(Person)
    );

    private static ApiType ApiCompanyObjectType { get; } = new ApiObjectType
    (
        nameof(Company),
        [
            new (nameof(Company.Name), ApiTypeExpression.ClrRef<string>(), ApiTypeModifiers.Required, nameof(Company.Name)),
            new (nameof(Company.Owner), ApiTypeExpression.ClrRef<Person>(), ApiTypeModifiers.None, nameof(Company.Owner)),
            new (nameof(Company.Employees), ApiTypeExpression.ListOf<Person>(ApiTypeModifiers.Required), ApiTypeModifiers.None, nameof(Company.Employees)),
        ],
        [
            new (nameof(Company.Owner)),
            new (nameof(Company.Employees)),
        ],
        typeof(Company)
    );

    private static ApiType ApiCompanyObjectTypeWithMissingRelatedProperties { get; } = new ApiObjectType
    (
        nameof(Company),
        [
            new (nameof(Company.Name), ApiTypeExpression.ClrRef<string>(), ApiTypeModifiers.Required, nameof(Company.Name)),
            new (nameof(Company.Owner), ApiTypeExpression.ClrRef<Person>(), ApiTypeModifiers.None, nameof(Company.Owner)),
            new (nameof(Company.Employees), ApiTypeExpression.ListOf<Person>(ApiTypeModifiers.Required), ApiTypeModifiers.None, nameof(Company.Employees)),
        ],
        [
            new (nameof(Company.Owner), nameof(Company.Owner) + "Missing"),
            new (nameof(Company.Employees), nameof(Company.Employees) + "Missing"),
        ],
        typeof(Company)
    );

    private static ApiSchema ApiEmptySchema { get; } = ApiSchema.Create
    (
        apiName: nameof(ApiEmptySchema),
        apiScalarTypes: [],
        apiEnumTypes: [],
        apiObjectTypes: [],
        apiVersion: "1.0"
    );

    private static ApiSchema ApiEmptySchemaWithExtension { get; } = ApiSchema.Create
    (
        apiName: nameof(ApiEmptySchemaWithExtension),
        apiScalarTypes: [],
        apiEnumTypes: [],
        apiObjectTypes: [],
        apiVersion: "1.0",
        extensions: [new TestExtension()]
    );

    private static ApiSchema ApiScalarsOnlySchema { get; } = ApiSchema.Create
    (
        apiName: nameof(ApiScalarsOnlySchema),
        apiScalarTypes:
        [
            (ApiScalarType)ApiStringScalarType.DeepCopy()!,
            (ApiScalarType)ApiInt64ScalarType.DeepCopy()!,
            (ApiScalarType)ApiBooleanScalarType.DeepCopy()!,
        ],
        apiEnumTypes: [],
        apiObjectTypes: [],
        apiVersion: "1.0"
    );

    private static ApiSchema ApiScalarsOnlySchemaWithExtension { get; } = ApiSchema.Create
    (
        apiName: nameof(ApiScalarsOnlySchemaWithExtension),
        apiScalarTypes:
        [
            (ApiScalarType)ApiStringScalarType.DeepCopy()!,
            (ApiScalarType)ApiInt64ScalarType.DeepCopy()!,
            (ApiScalarType)ApiBooleanScalarType.DeepCopy()!,
        ],
        apiEnumTypes: [],
        apiObjectTypes: [],
        apiVersion: "1.0",
        extensions: [new TestExtension()]
    );

    private static ApiSchema ApiScalarsOnlyObjectTypeSchema { get; } = ApiSchema.Create
    (
        apiName: nameof(ApiScalarsOnlyObjectTypeSchema),
        apiScalarTypes:
        [
            (ApiScalarType)ApiStringScalarType.DeepCopy()!,
            (ApiScalarType)ApiInt64ScalarType.DeepCopy()!,
            (ApiScalarType)ApiBooleanScalarType.DeepCopy()!,
        ],
        apiEnumTypes: [],
        apiObjectTypes: [
            (ApiObjectType)ApiScalarsOnlyObjectType.DeepCopy()!
        ],
        apiVersion: "1.0"
    );

    private static ApiSchema ApiPersonObjectTypeSchema { get; } = ApiSchema.Create
    (
        apiName: nameof(ApiPersonObjectTypeSchema),
        apiScalarTypes:
        [
            (ApiScalarType)ApiStringScalarType.DeepCopy()!,
            (ApiScalarType)ApiInt32ScalarType.DeepCopy()!,
        ],
        apiEnumTypes:
        [
            (ApiEnumType)ApiGenderEnumType.DeepCopy()!
        ],
        apiObjectTypes:
        [
            (ApiObjectType)ApiPersonObjectType.DeepCopy()!
        ],
        apiVersion: "1.0"
    );

    private static ApiSchema ApiCompanyObjectTypeSchema { get; } = ApiSchema.Create
    (
        apiName: nameof(ApiCompanyObjectTypeSchema),
        apiScalarTypes:
        [
            (ApiScalarType)ApiStringScalarType.DeepCopy()!,
            (ApiScalarType)ApiInt32ScalarType.DeepCopy()!,
        ],
        apiEnumTypes:
        [
            (ApiEnumType)ApiGenderEnumType.DeepCopy()!
        ],
        apiObjectTypes:
        [
            (ApiObjectType)ApiCompanyObjectType.DeepCopy()!,
            (ApiObjectType)ApiPersonObjectType.DeepCopy()!,
        ],
        apiVersion: "1.0"
    );

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = $"Build {ApiEmptySchema}",
            ApiName = nameof(ApiEmptySchema),
            ApiVersion = "1.0",
            ApiSchemaExpected = ApiEmptySchema,
        },
        new BuildTest
        {
            Name = $"Build {ApiEmptySchemaWithExtension}",
            ApiName = nameof(ApiEmptySchemaWithExtension),
            ApiVersion = "1.0",
            ApiSchemaExpected = ApiEmptySchemaWithExtension,
            ApiExtensionType = typeof(TestExtension),
        },
        new BuildTest
        {
            Name = $"Build {ApiScalarsOnlySchema}",
            ApiName = nameof(ApiScalarsOnlySchema),
            ApiVersion = "1.0",
            ApiScalarTypes =
            [
                ApiStringScalarType,
                ApiInt64ScalarType,
                ApiBooleanScalarType,
            ],
            ApiEnumTypes = [],
            ApiObjectTypes = [],
            ApiSchemaExpected = ApiScalarsOnlySchema,
        },
        new BuildTest
        {
            Name = $"Build {ApiScalarsOnlySchemaWithExtension}",
            ApiName = nameof(ApiScalarsOnlySchemaWithExtension),
            ApiVersion = "1.0",
            ApiScalarTypes =
            [
                ApiStringScalarType,
                ApiInt64ScalarType,
                ApiBooleanScalarType,
            ],
            ApiEnumTypes = [],
            ApiObjectTypes = [],
            ApiSchemaExpected = ApiScalarsOnlySchemaWithExtension,
            ApiExtensionType = typeof(TestExtension),
        },
        new BuildTest
        {
            Name = $"Build {ApiScalarsOnlyObjectTypeSchema}",
            ApiName = nameof(ApiScalarsOnlyObjectTypeSchema),
            ApiVersion = "1.0",
            ApiScalarTypes =
            [
                ApiStringScalarType,
                ApiInt64ScalarType,
                ApiBooleanScalarType,
            ],
            ApiEnumTypes = [],
            ApiObjectTypes =
            [
                ApiScalarsOnlyObjectType,
            ],
            ApiSchemaExpected = ApiScalarsOnlyObjectTypeSchema,
        },
        new BuildTest
        {
            Name = $"Build {ApiScalarsOnlyObjectTypeSchema} fails with missing scalar types",
            ApiName = nameof(ApiScalarsOnlyObjectTypeSchema),
            ApiVersion = "1.0",
            ApiScalarTypes =
            [
                ApiInt64ScalarType,
                ApiBooleanScalarType,
            ],
            ApiEnumTypes = [],
            ApiObjectTypes =
            [
                ApiScalarsOnlyObjectType,
            ],
            ApiSchemaExceptionMessageExpected = "ApiSchema initialization failed.",
            ApiSchemaValidationResultsExpected =
            [
                @"ApiObjectType[""ScalarsOnly""].ApiProperty[""RequiredName""].ApiTypeExpression.ApiType is unresolved for ClrType=String.",
                @"ApiObjectType[""ScalarsOnly""].ApiProperty[""OptionalName""].ApiTypeExpression.ApiType is unresolved for ClrType=String.",
            ],
        },
        new BuildTest
        {
            Name = $"Build {ApiPersonObjectTypeSchema}",
            ApiName = nameof(ApiPersonObjectTypeSchema),
            ApiVersion = "1.0",
            ApiScalarTypes =
            [
                ApiStringScalarType,
                ApiInt32ScalarType,
            ],
            ApiEnumTypes =
            [
                ApiGenderEnumType,
            ],
            ApiObjectTypes =
            [
                ApiPersonObjectType,
            ],
            ApiSchemaExpected = ApiPersonObjectTypeSchema,
        },
        new BuildTest
        {
            Name = $"Build {ApiPersonObjectTypeSchema} fails with missing scalar and enum types",
            ApiName = nameof(ApiPersonObjectTypeSchema),
            ApiVersion = "1.0",
            ApiScalarTypes =
            [
                ApiInt32ScalarType,
            ],
            ApiEnumTypes = [],
            ApiObjectTypes =
            [
                ApiPersonObjectType,
            ],
            ApiSchemaExceptionMessageExpected = "ApiSchema initialization failed.",
            ApiSchemaValidationResultsExpected =
            [
                @"ApiObjectType[""Person""].ApiProperty[""Name""].ApiTypeExpression.ApiType is unresolved for ClrType=String.",
                @"ApiObjectType[""Person""].ApiProperty[""Gender""].ApiTypeExpression.ApiType is unresolved for ClrType=Gender.",
                @"ApiCollectionType.ApiItemTypeExpression.ApiType is unresolved for ClrType=String.",
            ],
        },
        new BuildTest
        {
            Name = $"Build {ApiCompanyObjectTypeSchema}",
            ApiName = nameof(ApiCompanyObjectTypeSchema),
            ApiVersion = "1.0",
            ApiScalarTypes =
            [
                ApiStringScalarType,
                ApiInt32ScalarType,
            ],
            ApiEnumTypes =
            [
                ApiGenderEnumType,
            ],
            ApiObjectTypes =
            [
                ApiCompanyObjectType,
                ApiPersonObjectType,
            ],
            ApiSchemaExpected = ApiCompanyObjectTypeSchema,
        },
        new BuildTest
        {
            Name = $"Build {ApiCompanyObjectTypeSchema} fails with missing object types",
            ApiName = nameof(ApiCompanyObjectTypeSchema),
            ApiVersion = "1.0",
            ApiScalarTypes =
            [
                ApiStringScalarType,
            ],
            ApiEnumTypes = [],
            ApiObjectTypes =
            [
                ApiCompanyObjectType
            ],
            ApiSchemaExceptionMessageExpected = "ApiSchema initialization failed.",
            ApiSchemaValidationResultsExpected =
            [
                @"ApiObjectType[""Company""].ApiProperty[""Owner""].ApiTypeExpression.ApiType is unresolved for ClrType=Person.",
                @"ApiCollectionType.ApiItemTypeExpression.ApiType is unresolved for ClrType=Person.",
            ],
        },
        new BuildTest
        {
            Name = $"Build {ApiCompanyObjectTypeSchema} fails with missing relationship properties",
            ApiName = nameof(ApiCompanyObjectTypeSchema),
            ApiVersion = "1.0",
            ApiScalarTypes =
            [
                ApiStringScalarType,
                ApiInt32ScalarType,
            ],
            ApiEnumTypes =
            [
                ApiGenderEnumType,
            ],
            ApiObjectTypes =
            [
                ApiCompanyObjectTypeWithMissingRelatedProperties,
                ApiPersonObjectType,
            ],
            ApiSchemaExceptionMessageExpected = "ApiSchema initialization failed.",
            ApiSchemaValidationResultsExpected =
            [
                @"ApiObjectType[""Company""].ApiRelationship[""Owner""].ApiProperty unable to resolve ApiProperty[""OwnerMissing""].",
                @"ApiObjectType[""Company""].ApiRelationship[""Employees""].ApiProperty unable to resolve ApiProperty[""EmployeesMissing""].",
            ],
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}

