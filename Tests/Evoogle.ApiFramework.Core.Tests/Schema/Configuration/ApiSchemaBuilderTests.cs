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

        public ApiSchema? Expected { get; init; } = null!;
        public string? ExpectedExceptionMessage { get; init; }
        public List<ApiInitializationIssue>? ExpectedIssues { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? Actual { get; set; }
        private bool? ActualExceptionThrown { get; set; }
        private string? ActualExceptionMessage { get; set; }
        private List<ApiInitializationIssue>? ActualIssues { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"ApiName:          {this.ApiName.SafeToString()}");
            this.WriteLine($"ApiVersion:       {this.ApiVersion.SafeToString()}");
            this.WriteLine($"ApiExtensionType: {this.ApiExtensionType.SafeToName()}");
            this.WriteLine();

            if (this.Expected is not null)
            {
                this.WriteLine($"Expected: {this.Expected.SafeToString()}");
            }

            if (this.ExpectedExceptionMessage is not null)
            {
                this.WriteLine($"Expected Exception Message: {this.ExpectedExceptionMessage.SafeToString()}");
            }

            if (this.ExpectedIssues is not null)
            {
                this.WriteLine();
                foreach (var expectedIssue in this.ExpectedIssues)
                {
                    this.WriteLine($"Expected Issue: {expectedIssue.SafeToString()}");
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
                this.Actual = builder.Build();
                this.WriteLine($"Actual:   {this.Actual.SafeToString()}");
            }
            catch (ApiSchemaInitializationException ex)
            {
                this.ActualExceptionThrown = true;
                this.ActualExceptionMessage = ex.Message;
                this.ActualIssues = [.. ex.Issues];

                this.WriteLine($"Actual Exception Thrown:  {this.ActualExceptionThrown.SafeToString()}");
                this.WriteLine($"Actual Exception Message: {this.ActualExceptionMessage.SafeToString()}");
                this.WriteLine();
                foreach (var actualIssue in this.ActualIssues)
                {
                    this.WriteLine($"Actual Issue: {actualIssue.SafeToString()}");
                }
            }
        }

        protected override void Assert()
        {
            if (this.ActualExceptionThrown.GetValueOrDefault() == false)
            {
                this.ActualExceptionMessage.Should().BeNull();
                this.ActualIssues.Should().BeNull();

                this.Actual.Should().NotBeNull();
                this.Actual.Should().BeEquivalentTo
                (
                    this.Expected,
                    opt => opt
                        .WithStrictOrdering()
                        .Excluding(info => info.Path == $"{nameof(ApiSchemaContext)}" || info.Path == $"{nameof(ApiSchemaContext)}.{nameof(ApiSchemaContext.ApiSchema)}")
                );
            }
            else
            {
                this.Actual.Should().BeNull();

                this.ActualExceptionMessage.Should().Be(this.ExpectedExceptionMessage);
                this.ActualIssues.Should().NotBeNull();
                this.ActualIssues.Should().BeEquivalentTo(this.ExpectedIssues);
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
            Expected = ApiEmptySchema,
        },
        new BuildTest
        {
            Name = $"Build {ApiEmptySchemaWithExtension}",
            ApiName = nameof(ApiEmptySchemaWithExtension),
            ApiVersion = "1.0",
            Expected = ApiEmptySchemaWithExtension,
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
            Expected = ApiScalarsOnlySchema,
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
            Expected = ApiScalarsOnlySchemaWithExtension,
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
            Expected = ApiScalarsOnlyObjectTypeSchema,
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
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=2, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"{nameof(ScalarsOnly.RequiredName)}\"].{nameof(ApiProperty.ApiType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_UNRESOLVED_TYPE,
                    description: $"{nameof(ApiProperty.ApiType)} could not be resolved for {nameof(ApiTypeExpression.ClrType)}='{nameof(String)}'",
                    remediation: $"Verify that a type is declared in the schema for {nameof(ApiTypeExpression.ClrType)}='{nameof(String)}'"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(ScalarsOnly)}\"].{nameof(ApiProperty)}[\"{nameof(ScalarsOnly.OptionalName)}\"].{nameof(ApiProperty.ApiType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_UNRESOLVED_TYPE,
                    description: $"{nameof(ApiProperty.ApiType)} could not be resolved for {nameof(ApiTypeExpression.ClrType)}='{nameof(String)}'",
                    remediation: $"Verify that a type is declared in the schema for {nameof(ApiTypeExpression.ClrType)}='{nameof(String)}'"
                ),
            ]
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
            Expected = ApiPersonObjectTypeSchema,
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
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=3, Errors=3, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(Person)}\"].{nameof(ApiProperty)}[\"{nameof(Person.Name)}\"].{nameof(ApiProperty.ApiType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_UNRESOLVED_TYPE,
                    description: $"{nameof(ApiProperty.ApiType)} could not be resolved for {nameof(ApiTypeExpression.ClrType)}='{nameof(String)}'",
                    remediation: $"Verify that a type is declared in the schema for {nameof(ApiTypeExpression.ClrType)}='{nameof(String)}'"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(Person)}\"].{nameof(ApiProperty)}[\"{nameof(Person.Gender)}\"].{nameof(ApiProperty.ApiType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_UNRESOLVED_TYPE,
                    description: $"{nameof(ApiProperty.ApiType)} could not be resolved for {nameof(ApiTypeExpression.ClrType)}='{nameof(Gender)}'",
                    remediation: $"Verify that a type is declared in the schema for {nameof(ApiTypeExpression.ClrType)}='{nameof(Gender)}'"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(Person)}\"].{nameof(ApiProperty)}[\"{nameof(Person.Hobbies)}\"].{nameof(ApiCollectionType)}.{nameof(ApiCollectionType.ApiItemType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_COLLECTION_TYPE_UNRESOLVED_ITEM_TYPE,
                    description: $"{nameof(ApiCollectionType.ApiItemType)} could not be resolved for {nameof(ApiTypeExpression.ClrType)}='{nameof(String)}'",
                    remediation: $"Verify that a type is declared in the schema for {nameof(ApiTypeExpression.ClrType)}='{nameof(String)}'"
                ),
            ]
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
            Expected = ApiCompanyObjectTypeSchema,
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
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=2, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(Company)}\"].{nameof(ApiProperty)}[\"{nameof(Company.Owner)}\"].{nameof(ApiProperty.ApiType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_PROPERTY_UNRESOLVED_TYPE,
                    description: $"{nameof(ApiProperty.ApiType)} could not be resolved for {nameof(ApiTypeExpression.ClrType)}='{nameof(Person)}'",
                    remediation: $"Verify that a type is declared in the schema for {nameof(ApiTypeExpression.ClrType)}='{nameof(Person)}'"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(Company)}\"].{nameof(ApiProperty)}[\"{nameof(Company.Employees)}\"].{nameof(ApiCollectionType)}.{nameof(ApiCollectionType.ApiItemType)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_COLLECTION_TYPE_UNRESOLVED_ITEM_TYPE,
                    description: $"{nameof(ApiCollectionType.ApiItemType)} could not be resolved for {nameof(ApiTypeExpression.ClrType)}='{nameof(Person)}'",
                    remediation: $"Verify that a type is declared in the schema for {nameof(ApiTypeExpression.ClrType)}='{nameof(Person)}'"
                ),
            ]
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
            ExpectedExceptionMessage = $"{nameof(ApiSchema)} initialization failed. Issues=2, Errors=2, Warnings=0.",
            ExpectedIssues =
            [
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(Company)}\"].{nameof(ApiRelationship)}[\"{nameof(Company.Owner)}\"].{nameof(ApiRelationship.ApiProperty)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_UNRESOLVED_PROPERTY,
                    description: $"{nameof(ApiRelationship.ApiProperty)} could not be resolved for {nameof(ApiRelationship.ApiPropertyName)}='OwnerMissing' on parent {nameof(ApiObjectType)}='{nameof(Company)}'",
                    remediation: $"Verify that {nameof(ApiRelationship.ApiPropertyName)} refers to a valid property on parent {nameof(ApiObjectType)}='{nameof(Company)}'"
                ),
                new ApiInitializationIssue
                (
                    path: $"{nameof(ApiObjectType)}[\"{nameof(Company)}\"].{nameof(ApiRelationship)}[\"{nameof(Company.Employees)}\"].{nameof(ApiRelationship.ApiProperty)}",
                    severity: ApiInitializationSeverity.Error,
                    code: ApiInitializationCode.API_RELATIONSHIP_UNRESOLVED_PROPERTY,
                    description: $"{nameof(ApiRelationship.ApiProperty)} could not be resolved for {nameof(ApiRelationship.ApiPropertyName)}='EmployeesMissing' on parent {nameof(ApiObjectType)}='{nameof(Company)}'",
                    remediation: $"Verify that {nameof(ApiRelationship.ApiPropertyName)} refers to a valid property on parent {nameof(ApiObjectType)}='{nameof(Company)}'"
                ),
            ]
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}
