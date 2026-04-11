// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
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
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_Person_Name",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
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
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Enum, apiName: nameof(Gender)),
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
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
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
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
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
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_Person_Name",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
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
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Enum, apiName: nameof(Gender)),
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
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
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
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
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
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Company.Id),
                                            ClrScalarTypeHint: typeof(string)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_Company_Name",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
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
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Company.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Company.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Company.Owner),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(Person)),
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
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(Person)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<Person>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Company.Employees),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
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
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(Person.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_Person_Name",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
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
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(Person.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Age),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(Person.Age),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(Person.Gender),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Enum, apiName: nameof(Gender)),
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
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
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
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
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
        // TC1: ApiSchema With Identity Schema (IdentityScalar)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityScalar)})",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityScalar)})"
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
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityScalar)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityScalar)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityScalar",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityScalar.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_IdentityScalar",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityScalar.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityScalar.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityScalar.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityScalar.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityScalar.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            )
        },

        // TC2: ApiSchema With Identity Schema (IdentityTwoScalarPartComposite)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityTwoScalarPartComposite)})",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityTwoScalarPartComposite)})"
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
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityTwoScalarPartComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityTwoScalarPartComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityTwoScalarPartComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id1)
                                        ),
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id2)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityTwoScalarPartComposite.Id1),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityTwoScalarPartComposite.Id1),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityTwoScalarPartComposite.Id2),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityTwoScalarPartComposite.Id2),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityTwoScalarPartComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityTwoScalarPartComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            )
        },

        // TC3: ApiSchema With Identity Schema (IdentityThreeScalarPartComposite)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityThreeScalarPartComposite)})",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityThreeScalarPartComposite)})"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(Guid)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Guid)
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
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityThreeScalarPartComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityThreeScalarPartComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityThreeScalarPartComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id1)
                                        ),
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id2)
                                        ),
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id3)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Id1),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Id1),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Id2),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Id2),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Id3),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Guid)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Id3),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            )
        },

        // TC4: ApiSchema With Identity Schema (IdentityNested And IdentityNestedComposite Without Explicit ApiIdentityName)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} Without Explicit ApiIdentityName)",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} Without Explicit ApiIdentityName)"
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
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityNested)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityNested)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityNestedPart",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityNested.Id)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityNested.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNested.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityNested.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityNested.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityNestedComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityNestedComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityNestedComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Nested,
                                            ApiPropertyName: nameof(IdentityNestedComposite.NestedPart)
                                        ),
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityNestedComposite.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityNestedComposite.NestedPart),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityNested)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNestedComposite.NestedPart),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityNestedComposite.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNestedComposite.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            )
        },

        // TC5: ApiSchema With Identity Schema (IdentityOwner, IdentityOwnedComposite, And IdentityOwnedDependent Without Explicit ApiIdentityName)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} Without Explicit ApiIdentityName)",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} Without Explicit ApiIdentityName)"
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
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwner)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityOwner)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityOwner",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityOwner.Id)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwner.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwner.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityOwner.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwner.Dependents),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedComposite)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<IdentityOwnedComposite>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Dependents),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwner.Dependent),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedDependent)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Dependent),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwnedComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityOwnedComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityOwnedComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Owner
                                        ),
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityOwnedComposite.LineNumber)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwnedComposite.LineNumber),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedComposite.LineNumber),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwnedComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwnedDependent)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityOwnedDependent)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityOwnedDependent",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Owner
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwnedDependent.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedDependent.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            )
        },

        // TC6: ApiSchema With Full Identity Schema
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Full Identity Schema",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With Full Identity Schema"
                ),
                ApiNamedTypes:
                [
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Scalar,
                            ClrType: typeof(Guid)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(Guid)
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
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityScalar)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityScalar)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityScalar",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityScalar.Id)
                                        )
                                    ]
                                ),
                                new ApiIdentityConfig
                                (
                                    ApiName: "AK_IdentityScalar",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityScalar.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityScalar.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityScalar.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityScalar.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityScalar.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityTwoScalarPartComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityTwoScalarPartComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityTwoScalarPartComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id1)
                                        ),
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id2)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityTwoScalarPartComposite.Id1),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityTwoScalarPartComposite.Id1),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityTwoScalarPartComposite.Id2),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityTwoScalarPartComposite.Id2),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityTwoScalarPartComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityTwoScalarPartComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityThreeScalarPartComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityThreeScalarPartComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityThreeScalarPartComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id1)
                                        ),
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id2)
                                        ),
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id3)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Id1),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Id1),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Id2),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Id2),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Id3),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Guid)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Id3),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityThreeScalarPartComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityThreeScalarPartComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityNested)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityNested)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityNestedPart",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityNested.Id)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityNested.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNested.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityNested.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityNested.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityNestedComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityNestedComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityNestedComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Nested,
                                            ApiPropertyName: nameof(IdentityNestedComposite.NestedPart)
                                        ),
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityNestedComposite.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityNestedComposite.NestedPart),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityNested)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNestedComposite.NestedPart),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityNestedComposite.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNestedComposite.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwner)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityOwner)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityOwner",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityOwner.Id)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwner.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwner.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityOwner.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwner.Dependents),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedComposite)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<IdentityOwnedComposite>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Dependents),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwner.Dependent),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedDependent)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Dependent),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwnedComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityOwnedComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityOwnedComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Owner
                                        ),
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityOwnedComposite.LineNumber)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwnedComposite.LineNumber),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedComposite.LineNumber),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwnedComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwnedDependent)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityOwnedDependent)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityOwnedDependent",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Owner
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwnedDependent.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedDependent.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            )
        },

        // TC7: ApiSchema With Identity Schema (IdentityNested And IdentityNestedComposite With Explicit ApiIdentityName)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} With Explicit ApiIdentityName)",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} With Explicit ApiIdentityName)"
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
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityNested)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityNested)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityNestedPart",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityNested.Id)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityNested.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNested.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityNested.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityNested.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityNestedComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityNestedComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityNestedComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Nested,
                                            ApiPropertyName: nameof(IdentityNestedComposite.NestedPart),
                                            ApiIdentityName: "PK_IdentityNestedPart"
                                        ),
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityNestedComposite.Name)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityNestedComposite.NestedPart),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityNested)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNestedComposite.NestedPart),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityNestedComposite.Name),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityNestedComposite.Name),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
                ]
            )
        },

        // TC8: ApiSchema With Identity Schema (IdentityOwner, IdentityOwnedComposite, And IdentityOwnedDependent With Explicit ApiIdentityName)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} With Explicit ApiIdentityName)",
            ExpectedFactoryArgument = new ApiSchemaDescriptor
            (
                ApiSchema: new ApiSchemaConfig
                (
                    ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} With Explicit ApiIdentityName)"
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
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwner)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityOwner)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityOwner",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityOwner.Id)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwner.Id),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Id),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwner.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.None,
                                    ClrName: nameof(IdentityOwner.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwner.Dependents),
                                    ApiTypeExpression: new ApiTypeExpression
                                    (
                                        apiInlineType: new ApiCollectionType
                                        (
                                            apiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedComposite)),
                                            apiItemTypeModifiers: ApiTypeModifiers.Required,
                                            clrCollectionType: typeof(List<IdentityOwnedComposite>)
                                        )
                                    ),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Dependents),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwner.Dependent),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedDependent)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwner.Dependent),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwnedComposite)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityOwnedComposite)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityOwnedComposite",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Owner,
                                            ApiIdentityName: "PK_IdentityOwner"
                                        ),
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Scalar,
                                            ApiPropertyName: nameof(IdentityOwnedComposite.LineNumber)
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwnedComposite.LineNumber),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedComposite.LineNumber),
                                    ClrMemberKind: ClrMemberKind.Property
                                ),
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwnedComposite.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedComposite.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    ),
                    new ApiTypeDescriptor
                    (
                        ApiType: new ApiTypeConfig
                        (
                            ApiKind: ApiTypeKind.Object,
                            ClrType: typeof(IdentityOwnedDependent)
                        ),
                        ApiNamedType: new ApiNamedTypeConfig
                        (
                            ApiName: nameof(IdentityOwnedDependent)
                        ),
                        ApiObjectType: new ApiObjectTypeConfig
                        (
                            ApiIdentities:
                            [
                                new ApiIdentityConfig
                                (
                                    ApiName: "PK_IdentityOwnedDependent",
                                    ApiIdentityParts:
                                    [
                                        new ApiIdentityPartConfig
                                        (
                                            ApiKind: ApiIdentityPartKind.Owner,
                                            ApiIdentityName: "PK_IdentityOwner"
                                        )
                                    ]
                                )
                            ],
                            ApiProperties:
                            [
                                new ApiPropertyConfig
                                (
                                    ApiName: nameof(IdentityOwnedDependent.Description),
                                    ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                    ApiTypeModifiers: ApiTypeModifiers.Required,
                                    ClrName: nameof(IdentityOwnedDependent.Description),
                                    ClrMemberKind: ClrMemberKind.Property
                                )
                            ]
                        )
                    )
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
