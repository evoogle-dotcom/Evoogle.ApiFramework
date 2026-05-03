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

public partial class ApiTypeTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        // Null
        new JsonRoundtripTest
        {
            Name = "Null",
            ExpectedFactoryArgument = null
        },

        // ApiScalarType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [Boolean]",
            ExpectedFactoryArgument = new ApiScalarTypeDef
            (
                ApiName: nameof(Boolean),
                ClrType: typeof(bool)
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Boolean)}] With {nameof(GraphQlExtension)}",
            ExpectedFactoryArgument = new ApiScalarTypeDef
            (
                ApiName: nameof(Boolean),
                ClrType: typeof(bool),
                ExtensionTypes: [ typeof(GraphQlExtension) ]
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Int32)}]",
            ExpectedFactoryArgument = new ApiScalarTypeDef
            (
                ApiName: nameof(Int32),
                ClrType: typeof(int)
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Single)}]",
            ExpectedFactoryArgument = new ApiScalarTypeDef
            (
                ApiName: nameof(Single),
                ClrType: typeof(float)
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(String)}]",
            ExpectedFactoryArgument = new ApiScalarTypeDef
            (
                ApiName: nameof(String),
                ClrType: typeof(string)
            )
        },

        // ApiEnumType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}]",
            ExpectedFactoryArgument = new ApiEnumTypeDef
            (
                ApiName: nameof(StopLight),
                ClrType: typeof(StopLight)
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}] With {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiEnumTypeDef
            (
                ApiName: nameof(StopLight),
                ClrType: typeof(StopLight),
                ExtensionTypes: [ typeof(JsonApiExtension) ]
            )
        },

        // ApiCollectionType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(String)}>]",
            ExpectedFactoryArgument = new ApiCollectionTypeDef
            (
                ClrType: typeof(List<string>),
                ApiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: "String"),
                ApiItemTypeModifiers: ApiTypeModifiers.Required
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(String)}?>]",
            ExpectedFactoryArgument = new ApiCollectionTypeDef
            (
                ClrType: typeof(List<string?>),
                ApiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: "String"),
                ApiItemTypeModifiers: ApiTypeModifiers.None
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}>]",
            ExpectedFactoryArgument = new ApiCollectionTypeDef
            (
                ClrType: typeof(List<StopLight>),
                ApiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                ApiItemTypeModifiers: ApiTypeModifiers.Required
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}?>]",
            ExpectedFactoryArgument = new ApiCollectionTypeDef
            (
                ClrType: typeof(List<StopLight?>),
                ApiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                ApiItemTypeModifiers: ApiTypeModifiers.None
            )
        },

        // ApiObjectType With API Named Typed Expressions
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions",
            ExpectedFactoryArgument = new ApiObjectTypeDef
            (
                ApiName: nameof(ScalarsOnly),
                ClrType: typeof(ScalarsOnly),
                ApiProperties:
                [
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredName),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredName),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredNumber),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredNumber),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredPredicate),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredPredicate),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalName),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalName),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalNumber),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalNumber),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalPredicate),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalPredicate),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                ]
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And API Options",
            ExpectedFactoryArgument = new ApiObjectTypeDef
            (
                ApiName: nameof(ScalarsOnly),
                ClrType: typeof(ScalarsOnly),
                ApiIdentityPartNullHandling: ApiIdentityPartNullHandling.ThrowOnNull,
                ApiProperties:
                [
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredName),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredName),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredNumber),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredNumber),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredPredicate),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredPredicate),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalName),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalName),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalNumber),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalNumber),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalPredicate),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalPredicate),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                ]
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiObjectTypeDef
            (
                ApiName: nameof(ScalarsOnly),
                ClrType: typeof(ScalarsOnly),
                ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ],
                ApiProperties:
                [
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredName),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredName),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredNumber),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredNumber),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredPredicate),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredPredicate),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalName),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalName),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalNumber),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalNumber),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalPredicate),
                        ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalPredicate),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                ]
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With API Named Typed Expressions",
            ExpectedFactoryArgument = new ApiObjectTypeDef
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
            )
        },

        // ApiObjectType With CLR Typed Expressions
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions",
            ExpectedFactoryArgument = new ApiObjectTypeDef
            (
                ApiName: nameof(ScalarsOnly),
                ClrType: typeof(ScalarsOnly),
                ApiProperties:
                [
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredName),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredName),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredNumber),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredNumber),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredPredicate),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredPredicate),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalName),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalName),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalNumber),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalNumber),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalPredicate),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalPredicate),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                ]
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions And API Options",
            ExpectedFactoryArgument = new ApiObjectTypeDef
            (
                ApiName: nameof(ScalarsOnly),
                ClrType: typeof(ScalarsOnly),
                ApiIdentityPartNullHandling: ApiIdentityPartNullHandling.ThrowOnNull,
                ApiProperties:
                [
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredName),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredName),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredNumber),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredNumber),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredPredicate),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredPredicate),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalName),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalName),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalNumber),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalNumber),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalPredicate),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalPredicate),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                ]
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiObjectTypeDef
            (
                ApiName: nameof(ScalarsOnly),
                ClrType: typeof(ScalarsOnly),
                ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ],
                ApiProperties:
                [
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredName),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredName),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredNumber),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredNumber),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.RequiredPredicate),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(ScalarsOnly.RequiredPredicate),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalName),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalName),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalNumber),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalNumber),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(ScalarsOnly.OptionalPredicate),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                        ApiTypeModifiers: ApiTypeModifiers.None,
                        ClrName: nameof(ScalarsOnly.OptionalPredicate),
                        ClrMemberKind: ClrMemberKind.Field
                    ),
                ]
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With CLR Typed Expressions",
            ExpectedFactoryArgument = new ApiObjectTypeDef
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
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(Ulid)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(Company.Id),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(Company.Name),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                        ApiTypeModifiers: ApiTypeModifiers.Required,
                        ClrName: nameof(Company.Name),
                        ClrMemberKind: ClrMemberKind.Property
                    ),
                    new ApiPropertyDef
                    (
                        ApiName: nameof(Company.Owner),
                        ApiTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
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
                                apiItemTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
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
        },
    ];

    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonRoundtripTheoryData))]
    public void JsonRoundtrip(IXUnitTest test) => test.Execute(this);
    #endregion
}
