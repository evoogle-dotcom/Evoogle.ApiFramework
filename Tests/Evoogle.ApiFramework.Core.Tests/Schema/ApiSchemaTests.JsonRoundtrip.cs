// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;
using static Evoogle.XUnit.Tests.JsonUnitTests;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{

    #region Test Types
    private class JsonRoundtripTest : JsonRoundtripTest<ApiSchema, ApiSchemaDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonRoundtripTest()
        {
            this.ExpectedFactoryExpression = (arg) => BuildTestApiSchema(arg);
            this.ExcludeMembers = _excludeMembers;
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        new JsonRoundtripTest
        {
            Name = "Null",
            ExpectedFactoryArgument = null
        },

        // ApiSchema With 0 ApiType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}"
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
                    ApiOptions: new ApiSchemaOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    )
                )
            )
        },

        // ApiSchema With 1 ApiScalarType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 1 ApiScalarType And ApiSchemaOptions
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
                    ApiOptions: new ApiSchemaOptionsConfig
                    (
                        ApiIdentityNullHandling: ApiIdentityNullHandling.ThrowException
                    )
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 1 ApiScalarType And GraphQlExtension
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 2 ApiScalarType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType And GraphQlExtension And JsonApiExtension
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(bool)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Boolean)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 1 ApiEnumType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 1 ApiEnumType And ProtobufExtension
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
                    ExtensionTypes: [ typeof(ProtobufExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    )
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(Ulid)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Ulid)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_Person_Id",
                                    ApiIdentitySources:
                                    [
                                        new ApiIdentitySourceConfig
                                        (
                                            ApiPropertyName: nameof(Person.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_Person_Name",
                                    ApiIdentitySources:
                                    [
                                        new ApiIdentitySourceConfig
                                        (
                                            ApiPropertyName: nameof(Person.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Id),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.CompanyId),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.CompanyId),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                            ]
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person) And GraphQlExtension And JsonApiExtension
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(Ulid)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Ulid)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_Person_Id",
                                    ApiIdentitySources:
                                    [
                                        new ApiIdentitySourceConfig
                                        (
                                            ApiPropertyName: nameof(Person.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_Person_Name",
                                    ApiIdentitySources:
                                    [
                                        new ApiIdentitySourceConfig
                                        (
                                            ApiPropertyName: nameof(Person.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Id),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.CompanyId),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.CompanyId),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                            ]
                        )
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarTypes And 1 ApiEnumType And 2 ApiObjectTypes (Company And Person)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(int)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Int32)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(string)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(String)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(Ulid)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Ulid)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Enum,
                            ClrType: typeof(Gender)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Gender)
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(Company)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Company)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_Company_Id",
                                    ApiIdentitySources:
                                    [
                                        new ApiIdentitySourceConfig
                                        (
                                            ApiPropertyName: nameof(Company.Id),
                                            ClrScalarType: typeof(string)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_Company_Name",
                                    ApiIdentitySources:
                                    [
                                        new ApiIdentitySourceConfig
                                        (
                                            ApiPropertyName: nameof(Company.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Id),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Company.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Company.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Owner),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Company.Owner),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Employees),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Object, apiName: nameof(Person)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<Person>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Company.Employees),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                            ],
                            ApiRelationships:
                            [
                                new ApiRelationshipConfig(ApiName: "Company_Owner", ApiPropertyName: nameof(Company.Owner)),
                                new ApiRelationshipConfig(ApiName: "Company_Employees", ApiPropertyName: nameof(Company.Employees))
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(Person)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Person)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_Person_Id",
                                    ApiIdentitySources:
                                    [
                                        new ApiIdentitySourceConfig
                                        (
                                            ApiPropertyName: nameof(Person.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_Person_Name",
                                    ApiIdentitySources:
                                    [
                                        new ApiIdentitySourceConfig
                                        (
                                            ApiPropertyName: nameof(Person.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Id),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Gender),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Hobbies),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<string>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Hobbies),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.CompanyId),
                                    ApiTypeExpression: new ApiTypeExpression(kind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.CompanyId),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                            ]
                        )
                    ),
                ]
            )
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonRoundtripTheoryData))]
    public void JsonRoundtrip(IXUnitTest test) => test.Execute(this);
    #endregion
}
