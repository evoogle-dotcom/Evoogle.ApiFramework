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
    private class BuildTest : XUnitTest
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
    private static ApiType ApiBooleanScalarType { get; } = new ApiScalarType(apiName: nameof(Boolean), clrScalarType: typeof(bool));
    private static ApiType ApiInt32ScalarType { get; } = new ApiScalarType(apiName: nameof(Int32), clrScalarType: typeof(int));
    private static ApiType ApiInt64ScalarType { get; } = new ApiScalarType(apiName: nameof(Int64), clrScalarType: typeof(long));
    private static ApiType ApiStringScalarType { get; } = new ApiScalarType(apiName: nameof(String), clrScalarType: typeof(string));

    private static ApiType ApiGenderEnumType { get; } = new ApiEnumType
    (
        apiName: nameof(Gender),
        apiEnumValues:
        [
            new (apiName: nameof(Gender.Unspecified), clrName: nameof(Gender.Unspecified), clrOrdinal: (int)Gender.Unspecified),
            new (apiName: nameof(Gender.Male), clrName: nameof(Gender.Male), clrOrdinal: (int)Gender.Male),
            new (apiName: nameof(Gender.Female), clrName: nameof(Gender.Female), clrOrdinal: (int)Gender.Female),
        ],
        clrEnumType: typeof(Gender)
    );

    private static ApiType ApiScalarsOnlyObjectType { get; } = new ApiObjectType
    (
        apiName: nameof(ScalarsOnly),
        apiProperties:
        [
            new (apiName: nameof(ScalarsOnly.RequiredName), apiTypeExpression: ApiTypeExpression.ClrRef<string>(), apiTypeModifiers: ApiTypeModifiers.Required, clrName: nameof(ScalarsOnly.RequiredName), clrMemberKind: ClrMemberKind.Property),
            new (apiName: nameof(ScalarsOnly.RequiredNumber), apiTypeExpression: ApiTypeExpression.ClrRef<long>(), apiTypeModifiers: ApiTypeModifiers.Required, clrName: nameof(ScalarsOnly.RequiredNumber), clrMemberKind: ClrMemberKind.Property),
            new (apiName: nameof(ScalarsOnly.RequiredPredicate), apiTypeExpression: ApiTypeExpression.ClrRef<bool>(), apiTypeModifiers: ApiTypeModifiers.Required, clrName: nameof(ScalarsOnly.RequiredPredicate), clrMemberKind: ClrMemberKind.Property),
            new (apiName: nameof(ScalarsOnly.OptionalName), apiTypeExpression: ApiTypeExpression.ClrRef<string>(), apiTypeModifiers: ApiTypeModifiers.None, clrName: nameof(ScalarsOnly.OptionalName), clrMemberKind: ClrMemberKind.Field),
            new (apiName: nameof(ScalarsOnly.OptionalNumber), apiTypeExpression: ApiTypeExpression.ClrRef<long>(), apiTypeModifiers: ApiTypeModifiers.None, clrName: nameof(ScalarsOnly.OptionalNumber), clrMemberKind: ClrMemberKind.Field),
            new (apiName: nameof(ScalarsOnly.OptionalPredicate), apiTypeExpression: ApiTypeExpression.ClrRef<bool>(), apiTypeModifiers: ApiTypeModifiers.None, clrName: nameof(ScalarsOnly.OptionalPredicate), clrMemberKind: ClrMemberKind.Field),
        ],
        apiRelationships: null,
        apiIdentities: null,
        apiOptions: null,
        clrObjectType: typeof(ScalarsOnly)
    );

    private static ApiType ApiPersonObjectType { get; } = new ApiObjectType
    (
        apiName: nameof(Person),
        apiProperties:
        [
            new (apiName: nameof(Person.Name), apiTypeExpression: ApiTypeExpression.ClrRef<string>(), apiTypeModifiers: ApiTypeModifiers.Required, clrName: nameof(Person.Name), clrMemberKind: ClrMemberKind.Property),
            new (apiName: nameof(Person.Age), apiTypeExpression: ApiTypeExpression.ClrRef<int>(), apiTypeModifiers: ApiTypeModifiers.None, clrName: nameof(Person.Age), clrMemberKind: ClrMemberKind.Property),
            new (apiName: nameof(Person.Gender), apiTypeExpression: ApiTypeExpression.ClrRef<Gender>(), apiTypeModifiers: ApiTypeModifiers.None, clrName: nameof(Person.Gender), clrMemberKind: ClrMemberKind.Property),
            new (apiName: nameof(Person.Hobbies), apiTypeExpression: ApiTypeExpression.ListOf<string>(apiItemTypeModifiers: ApiTypeModifiers.Required), apiTypeModifiers: ApiTypeModifiers.None, clrName: nameof(Person.Hobbies), clrMemberKind: ClrMemberKind.Property),
        ],
        apiRelationships: null,
        apiIdentities: null,
        apiOptions: null,
        clrObjectType: typeof(Person)
    );

    private static ApiType ApiCompanyObjectType { get; } = new ApiObjectType
    (
        apiName: nameof(Company),
        apiProperties:
        [
            new (apiName: nameof(Company.Name), apiTypeExpression: ApiTypeExpression.ClrRef<string>(), apiTypeModifiers: ApiTypeModifiers.Required, clrName: nameof(Company.Name), clrMemberKind: ClrMemberKind.Property),
            new (apiName: nameof(Company.Owner), apiTypeExpression: ApiTypeExpression.ClrRef<Person>(), apiTypeModifiers: ApiTypeModifiers.None, clrName: nameof(Company.Owner), clrMemberKind: ClrMemberKind.Property),
            new (apiName: nameof(Company.Employees), apiTypeExpression: ApiTypeExpression.ListOf<Person>(apiItemTypeModifiers: ApiTypeModifiers.Required), apiTypeModifiers: ApiTypeModifiers.None, clrName: nameof(Company.Employees), clrMemberKind: ClrMemberKind.Property),
        ],
        apiRelationships:
        [
            new (apiName: nameof(Company.Owner)),
            new (apiName: nameof(Company.Employees)),
        ],
        apiIdentities: null,
        apiOptions: null,
        clrObjectType: typeof(Company)
    );

    private static ApiType ApiCompanyObjectTypeWithMissingRelatedProperties { get; } = new ApiObjectType
    (
        apiName: nameof(Company),
        apiProperties:
        [
            new (apiName: nameof(Company.Name), apiTypeExpression: ApiTypeExpression.ClrRef<string>(), apiTypeModifiers: ApiTypeModifiers.Required, clrName: nameof(Company.Name), clrMemberKind: ClrMemberKind.Property),
            new (apiName: nameof(Company.Owner), apiTypeExpression: ApiTypeExpression.ClrRef<Person>(), apiTypeModifiers: ApiTypeModifiers.None, clrName: nameof(Company.Owner), clrMemberKind: ClrMemberKind.Property),
            new (apiName: nameof(Company.Employees), apiTypeExpression: ApiTypeExpression.ListOf<Person>(apiItemTypeModifiers: ApiTypeModifiers.Required), apiTypeModifiers: ApiTypeModifiers.None, clrName: nameof(Company.Employees), clrMemberKind: ClrMemberKind.Property),
        ],
        apiRelationships:
        [
            new (apiName: nameof(Company.Owner), apiPropertyName: nameof(Company.Owner) + "Missing"),
            new (apiName: nameof(Company.Employees), apiPropertyName: nameof(Company.Employees) + "Missing"),
        ],
        apiIdentities: null,
        apiOptions: null,
        clrObjectType: typeof(Company)
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
        extensionTypeAndInstances: [(typeof(TestExtension), new TestExtension())]
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
        extensionTypeAndInstances: [(typeof(TestExtension), new TestExtension())]
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
