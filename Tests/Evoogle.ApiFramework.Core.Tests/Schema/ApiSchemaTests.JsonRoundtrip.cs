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
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)}"
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 0 {nameof(ApiType)} And {nameof(ApiSchemaOptions)}",
                ApiIdentityPartNullHandling: ApiIdentityPartNullHandling.ThrowOnNull
            )
        },

        // ApiSchema With 1 ApiScalarType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)}",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Boolean),
                        ClrType: typeof(bool)
                    )
                ]
            )
        },

        // ApiSchema With 1 ApiScalarType And ApiSchemaOptions
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(ApiSchemaOptions)}",
                ApiIdentityPartNullHandling: ApiIdentityPartNullHandling.ThrowOnNull,
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Boolean),
                        ClrType: typeof(bool)
                    )
                ]
            )
        },

        // ApiSchema With 1 ApiScalarType And GraphQlExtension
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)}",
                ExtensionTypes: [ typeof(GraphQlExtension) ],
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Boolean),
                        ClrType: typeof(bool)
                    )
                ]
            )
        },

        // ApiSchema With 2 ApiScalarType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 2 {nameof(ApiScalarType)}",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Boolean),
                        ClrType: typeof(bool)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)}",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Boolean),
                        ClrType: typeof(bool)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType And GraphQlExtension And JsonApiExtension
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ],
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Boolean),
                        ClrType: typeof(bool)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                ]
            )
        },

        // ApiSchema With 1 ApiEnumType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)}",
                ApiNamedTypes:
                [
                    new ApiEnumTypeDef
                    (
                        ApiName: nameof(Gender),
                        ClrType: typeof(Gender)
                    )
                ]
            )
        },

        // ApiSchema With 1 ApiEnumType And ProtobufExtension
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 1 {nameof(ApiEnumType)} And {nameof(ProtobufExtension)}",
                ExtensionTypes: [ typeof(ProtobufExtension) ],
                ApiNamedTypes:
                [
                    new ApiEnumTypeDef
                    (
                        ApiName: nameof(Gender),
                        ClrType: typeof(Gender)
                    )
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person)",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Ulid),
                        ClrType: typeof(Ulid)
                    ),
                    new ApiEnumTypeDef
                    (
                        ApiName: nameof(Gender),
                        ClrType: typeof(Gender)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(Person),
                        ClrType: typeof(Person),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_Person_Id",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(Person.Id)
                                    )
                                ]
                            ),
                            new ApiIdentityDef
                            (
                                ApiName: "AK_Person_Name",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(Person.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Person.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Person.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Age),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Age),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Gender),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Gender),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
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
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.CompanyId),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.CompanyId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ]
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarType And 1 ApiEnumType And 1 ApiObjectType (Person) And GraphQlExtension And JsonApiExtension
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 1 {nameof(ApiObjectType)} (Person) And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
                ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ],
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Ulid),
                        ClrType: typeof(Ulid)
                    ),
                    new ApiEnumTypeDef
                    (
                        ApiName: nameof(Gender),
                        ClrType: typeof(Gender)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(Person),
                        ClrType: typeof(Person),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_Person_Id",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(Person.Id)
                                    )
                                ]
                            ),
                            new ApiIdentityDef
                            (
                                ApiName: "AK_Person_Name",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(Person.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Person.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Person.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Age),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Age),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Gender),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Gender),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
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
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.CompanyId),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.CompanyId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ]
                    ),
                ]
            )
        },

        // ApiSchema With 3 ApiScalarTypes And 1 ApiEnumType And 2 ApiObjectTypes (Company And Person)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With 3 {nameof(ApiScalarType)} And 1 {nameof(ApiEnumType)} And 2 {nameof(ApiObjectType)} (Company And Person)",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Ulid),
                        ClrType: typeof(Ulid)
                    ),
                    new ApiEnumTypeDef
                    (
                        ApiName: nameof(Gender),
                        ClrType: typeof(Gender)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(Company),
                        ClrType: typeof(Company),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_Company_Id",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(Company.Id),
                                        ClrScalarTypeHint: typeof(string)
                                    )
                                ]
                            ),
                            new ApiIdentityDef
                            (
                                ApiName: "AK_Company_Name",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(Company.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Company.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Company.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Company.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Company.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Company.Owner),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(Person)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Company.Owner),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
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
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(Person),
                        ClrType: typeof(Person),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_Person_Id",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(Person.Id)
                                    )
                                ]
                            ),
                            new ApiIdentityDef
                            (
                                ApiName: "AK_Person_Name",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(Person.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Person.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(Person.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Age),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Age),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.Gender),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Enum, apiName: nameof(Gender)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.Gender),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
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
                            new ApiPropertyDef
                            (
                                ApiName: nameof(Person.CompanyId),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Ulid)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(Person.CompanyId),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                        ]
                    ),
                ]
            )
        },
        // TC1: ApiSchema With Identity Schema (IdentityScalar)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityScalar)})",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityScalar)})",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityScalar),
                        ClrType: typeof(IdentityScalar),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityScalar",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityScalar.Id)
                                    )
                                ]
                            ),
                            new ApiIdentityDef
                            (
                                ApiName: "AK_IdentityScalar",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityScalar.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityScalar.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityScalar.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityScalar.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityScalar.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            )
        },

        // TC2: ApiSchema With Identity Schema (IdentityTwoScalarPartComposite)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityTwoScalarPartComposite)})",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityTwoScalarPartComposite)})",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityTwoScalarPartComposite),
                        ClrType: typeof(IdentityTwoScalarPartComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityTwoScalarPartComposite",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id1)
                                    ),
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id2)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityTwoScalarPartComposite.Id1),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityTwoScalarPartComposite.Id1),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityTwoScalarPartComposite.Id2),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityTwoScalarPartComposite.Id2),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityTwoScalarPartComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityTwoScalarPartComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            )
        },

        // TC3: ApiSchema With Identity Schema (IdentityThreeScalarPartComposite)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityThreeScalarPartComposite)})",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityThreeScalarPartComposite)})",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Guid),
                        ClrType: typeof(Guid)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityThreeScalarPartComposite),
                        ClrType: typeof(IdentityThreeScalarPartComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityThreeScalarPartComposite",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id1)
                                    ),
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id2)
                                    ),
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id3)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Id1),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Id1),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Id2),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Id2),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Id3),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Guid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Id3),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            )
        },

        // TC4: ApiSchema With Identity Schema (IdentityNested And IdentityNestedComposite Without Explicit ApiIdentityName)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} Without Explicit ApiIdentityName)",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} Without Explicit ApiIdentityName)",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityNested),
                        ClrType: typeof(IdentityNested),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityNestedPart",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNested.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNested.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNested.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNested.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityNested.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityNestedComposite),
                        ClrType: typeof(IdentityNestedComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityNestedComposite",
                                Parts:
                                [
                                    new ApiNestedPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNestedComposite.NestedPart)
                                    ),
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNestedComposite.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNestedComposite.NestedPart),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityNested)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNestedComposite.NestedPart),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNestedComposite.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNestedComposite.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            )
        },

        // TC5: ApiSchema With Identity Schema (IdentityOwner, IdentityOwnedComposite, And IdentityOwnedDependent Without Explicit ApiIdentityName)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} Without Explicit ApiIdentityName)",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} Without Explicit ApiIdentityName)",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwner),
                        ClrType: typeof(IdentityOwner),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwner",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityOwner.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwner.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityOwner.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
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
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Dependent),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedDependent)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwner.Dependent),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwnedComposite),
                        ClrType: typeof(IdentityOwnedComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwnedComposite",
                                Parts:
                                [
                                    new ApiOwnerPartDef(),
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityOwnedComposite.LineNumber)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedComposite.LineNumber),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedComposite.LineNumber),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwnedDependent),
                        ClrType: typeof(IdentityOwnedDependent),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwnedDependent",
                                Parts:
                                [
                                    new ApiOwnerPartDef()
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedDependent.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedDependent.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            )
        },

        // TC6: ApiSchema With Full Identity Schema
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Full Identity Schema",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Full Identity Schema",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Guid),
                        ClrType: typeof(Guid)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityScalar),
                        ClrType: typeof(IdentityScalar),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityScalar",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityScalar.Id)
                                    )
                                ]
                            ),
                            new ApiIdentityDef
                            (
                                ApiName: "AK_IdentityScalar",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityScalar.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityScalar.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityScalar.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityScalar.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityScalar.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityTwoScalarPartComposite),
                        ClrType: typeof(IdentityTwoScalarPartComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityTwoScalarPartComposite",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id1)
                                    ),
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityTwoScalarPartComposite.Id2)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityTwoScalarPartComposite.Id1),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityTwoScalarPartComposite.Id1),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityTwoScalarPartComposite.Id2),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityTwoScalarPartComposite.Id2),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityTwoScalarPartComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityTwoScalarPartComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityThreeScalarPartComposite),
                        ClrType: typeof(IdentityThreeScalarPartComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityThreeScalarPartComposite",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id1)
                                    ),
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id2)
                                    ),
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityThreeScalarPartComposite.Id3)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Id1),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Id1),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Id2),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Id2),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Id3),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Guid)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Id3),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityThreeScalarPartComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityThreeScalarPartComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityNested),
                        ClrType: typeof(IdentityNested),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityNestedPart",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNested.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNested.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNested.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNested.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityNested.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityNestedComposite),
                        ClrType: typeof(IdentityNestedComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityNestedComposite",
                                Parts:
                                [
                                    new ApiNestedPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNestedComposite.NestedPart)
                                    ),
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNestedComposite.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNestedComposite.NestedPart),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityNested)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNestedComposite.NestedPart),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNestedComposite.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNestedComposite.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwner),
                        ClrType: typeof(IdentityOwner),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwner",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityOwner.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwner.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityOwner.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
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
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Dependent),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedDependent)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwner.Dependent),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwnedComposite),
                        ClrType: typeof(IdentityOwnedComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwnedComposite",
                                Parts:
                                [
                                    new ApiOwnerPartDef(),
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityOwnedComposite.LineNumber)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedComposite.LineNumber),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedComposite.LineNumber),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwnedDependent),
                        ClrType: typeof(IdentityOwnedDependent),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwnedDependent",
                                Parts:
                                [
                                    new ApiOwnerPartDef()
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedDependent.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedDependent.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            )
        },

        // TC7: ApiSchema With Identity Schema (IdentityNested And IdentityNestedComposite With Explicit ApiIdentityName)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} With Explicit ApiIdentityName)",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityNested)} And {nameof(IdentityNestedComposite)} With Explicit ApiIdentityName)",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityNested),
                        ClrType: typeof(IdentityNested),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityNestedPart",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNested.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNested.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNested.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNested.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityNested.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityNestedComposite),
                        ClrType: typeof(IdentityNestedComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityNestedComposite",
                                Parts:
                                [
                                    new ApiNestedPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNestedComposite.NestedPart),
                                        ApiIdentityName: "PK_IdentityNestedPart"
                                    ),
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityNestedComposite.Name)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNestedComposite.NestedPart),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityNested)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNestedComposite.NestedPart),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityNestedComposite.Name),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityNestedComposite.Name),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    )
                ]
            )
        },

        // TC8: ApiSchema With Identity Schema (IdentityOwner, IdentityOwnedComposite, And IdentityOwnedDependent With Explicit ApiIdentityName)
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} With Explicit ApiIdentityName)",
            ExpectedFactoryArgument = new ApiSchemaDef
            (
                ApiName: $"{nameof(ApiSchema)} With Identity Schema ({nameof(IdentityOwner)}, {nameof(IdentityOwnedComposite)}, And {nameof(IdentityOwnedDependent)} With Explicit ApiIdentityName)",
                ApiNamedTypes:
                [
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(Int32),
                        ClrType: typeof(int)
                    ),
                    new ApiScalarTypeDef
                    (
                        ApiName: nameof(String),
                        ClrType: typeof(string)
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwner),
                        ClrType: typeof(IdentityOwner),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwner",
                                Parts:
                                [
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityOwner.Id)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Id),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwner.Id),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.None,
                                ClrName: nameof(IdentityOwner.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
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
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwner.Dependent),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Object, apiName: nameof(IdentityOwnedDependent)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwner.Dependent),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwnedComposite),
                        ClrType: typeof(IdentityOwnedComposite),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwnedComposite",
                                Parts:
                                [
                                    new ApiOwnerPartDef(ApiIdentityName: "PK_IdentityOwner"),
                                    new ApiScalarPartDef
                                    (
                                        ApiPropertyName: nameof(IdentityOwnedComposite.LineNumber)
                                    )
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedComposite.LineNumber),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int32)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedComposite.LineNumber),
                                ClrMemberKind: ClrMemberKind.Property
                            ),
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedComposite.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedComposite.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
                    ),
                    new ApiObjectTypeDef
                    (
                        ApiName: nameof(IdentityOwnedDependent),
                        ClrType: typeof(IdentityOwnedDependent),
                        ApiIdentities:
                        [
                            new ApiIdentityDef
                            (
                                ApiName: "PK_IdentityOwnedDependent",
                                Parts:
                                [
                                    new ApiOwnerPartDef(ApiIdentityName: "PK_IdentityOwner")
                                ]
                            )
                        ],
                        ApiProperties:
                        [
                            new ApiPropertyDef
                            (
                                ApiName: nameof(IdentityOwnedDependent.Description),
                                ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                                ApiTypeModifiers: ApiTypeModifiers.Required,
                                ClrName: nameof(IdentityOwnedDependent.Description),
                                ClrMemberKind: ClrMemberKind.Property
                            )
                        ]
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
