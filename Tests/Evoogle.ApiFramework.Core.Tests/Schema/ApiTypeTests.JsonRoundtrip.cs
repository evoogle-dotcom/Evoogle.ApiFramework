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
            ExpectedFactoryArgument = new ApiTypeDescriptor
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
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Boolean)}] With {nameof(GraphQlExtension)}",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(bool),
                    ExtensionTypes: [ typeof(GraphQlExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Boolean)
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Int32)}]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
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
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(Single)}]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Scalar,
                    ClrType: typeof(float)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(Single)
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiScalarType)} [{nameof(String)}]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
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
            )
        },

        // ApiEnumType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Enum,
                    ClrType: typeof(StopLight)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(StopLight)
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiEnumType)} [{nameof(StopLight)}] With {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Enum,
                    ClrType: typeof(StopLight),
                    ExtensionTypes: [ typeof(JsonApiExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(StopLight)
                )
            )
        },

        // ApiCollectionType
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(String)}>]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<string>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: "String"),
                    ApiItemTypeModifiers: ApiTypeModifiers.Required
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(String)}?>]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<string?>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: "String"),
                    ApiItemTypeModifiers: ApiTypeModifiers.None
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}>]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<StopLight>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                    ApiItemTypeModifiers: ApiTypeModifiers.Required
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiCollectionType)} [List<{nameof(StopLight)}?>]",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Collection,
                    ClrType: typeof(List<StopLight?>)
                ),
                ApiCollectionType: new ApiCollectionTypeConfig
                (
                    ApiItemTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Enum, apiName: nameof(StopLight)),
                    ApiItemTypeModifiers: ApiTypeModifiers.None
                )
            )
        },

        // ApiObjectType With API Named Typed Expressions
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And API Options",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiOptions: new ApiObjectTypeOptionsConfig
                    (
                        ApiIdentityPartNullHandling: ApiIdentityPartNullHandling.ThrowOnNull
                    ),
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With API Named Typed Expressions And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly),
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(String)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Int64)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(apiKind: ApiTypeKind.Scalar, apiName: nameof(Boolean)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With API Named Typed Expressions",
            ExpectedFactoryArgument = new ApiTypeDescriptor
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
            )
        },

        // ApiObjectType With CLR Typed Expressions
        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions And API Options",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly)
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiOptions: new ApiObjectTypeOptionsConfig
                    (
                        ApiIdentityPartNullHandling: ApiIdentityPartNullHandling.ThrowOnNull
                    ),
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(ScalarsOnly)}] With CLR Typed Expressions And {nameof(GraphQlExtension)} And {nameof(JsonApiExtension)}",
            ExpectedFactoryArgument = new ApiTypeDescriptor
            (
                ApiType: new ApiTypeConfig
                (
                    ApiKind: ApiTypeKind.Object,
                    ClrType: typeof(ScalarsOnly),
                    ExtensionTypes: [ typeof(GraphQlExtension), typeof(JsonApiExtension) ]
                ),
                ApiNamedType: new ApiNamedTypeConfig
                (
                    ApiName: nameof(ScalarsOnly)
                ),
                ApiObjectType: new ApiObjectTypeConfig
                (
                    ApiProperties:
                    [
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredName),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredNumber),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.RequiredPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(ScalarsOnly.RequiredPredicate),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalName),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalName),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalNumber),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(long)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalNumber),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(ScalarsOnly.OptionalPredicate),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(bool)),
                            ApiTypeModifiers: ApiTypeModifiers.None,
                            ClrName: nameof(ScalarsOnly.OptionalPredicate),
                            ClrMemberKind: ClrMemberKind.Field
                        ),
                    ]
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"{nameof(ApiObjectType)} [{nameof(Company)}] With CLR Typed Expressions",
            ExpectedFactoryArgument = new ApiTypeDescriptor
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
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(Ulid)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(Company.Id),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(Company.Name),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(string)),
                            ApiTypeModifiers: ApiTypeModifiers.Required,
                            ClrName: nameof(Company.Name),
                            ClrMemberKind: ClrMemberKind.Property
                        ),
                        new ApiPropertyConfig
                        (
                            ApiName: nameof(Company.Owner),
                            ApiTypeExpression: new ApiTypeExpression(clrType: typeof(Person)),
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
