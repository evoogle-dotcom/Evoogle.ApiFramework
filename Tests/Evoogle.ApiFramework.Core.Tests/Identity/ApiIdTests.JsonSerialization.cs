// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Identity;

public partial class ApiIdTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        // Empty
        new JsonDeserializeTest
        {
            Name = "Empty",
            SourceJson = "null",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Empty
                )
            )
        },

        // Scalars

        // .. String
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.String)}:alpha",
            SourceJson = @"{""ApiKind"":""String"",""ApiValue"":""alpha""}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.String,
                    StringValue: "alpha"
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.String)}:ALPHA",
            SourceJson = @"{""ApiKind"":""String"",""ApiValue"":""ALPHA""}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.String,
                    StringValue: "ALPHA"
                )
            )
        },

        // .. Int32
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Int32)}:42",
            SourceJson = @"{""ApiKind"":""Int32"",""ApiValue"":42}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Int32,
                    Int32Value: 42
                )
            )
        },

        // .. Int64
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Int64)}:24",
            SourceJson = @"{""ApiKind"":""Int64"",""ApiValue"":24}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Int64,
                    Int64Value: 24
                )
            )
        },

        // .. Guid
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Guid)}:{TestGuid}",
            SourceJson = @"{""ApiKind"":""Guid"",""ApiValue"":""" + TestGuid + @"""}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Guid,
                    GuidValue: TestGuid
                )
            )
        },

        // .. Ulid
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Ulid)}:{TestUlid}",
            SourceJson = @"{""ApiKind"":""Ulid"",""ApiValue"":""" + TestUlid + @"""}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Ulid,
                    UlidValue: TestUlid
                )
            )
        },

        // .. Culture
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Culture)}:en-US",
            SourceJson = @"{""ApiKind"":""Culture"",""ApiValue"":""en-US""}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Culture,
                    CultureValue: "en-US"
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Culture)}:fr-FR",
            SourceJson = @"{""ApiKind"":""Culture"",""ApiValue"":""fr-FR""}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Culture,
                    CultureValue: "fr-FR"
                )
            )
        },

        // Composites

        // .. Ordered (unnamed parts)
        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:24|24",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiValue"":[{""ApiKind"":""Int32"",""ApiValue"":24},{""ApiKind"":""Int32"",""ApiValue"":24}]}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            )
        },

        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:24|42",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiValue"":[{""ApiKind"":""Int32"",""ApiValue"":24},{""ApiKind"":""Int32"",""ApiValue"":42}]}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },

        // .. Named (named parts)
        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:alpha=24|beta=24",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiValue"":[{""ApiName"":""alpha"",""ApiKind"":""Int32"",""ApiValue"":24},{""ApiName"":""beta"",""ApiKind"":""Int32"",""ApiValue"":24}]}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            )
        },
        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:alpha=24|beta=42",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiValue"":[{""ApiName"":""alpha"",""ApiKind"":""Int32"",""ApiValue"":24},{""ApiName"":""beta"",""ApiKind"":""Int32"",""ApiValue"":42}]}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },
        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:alpha=24|zeta=42",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiValue"":[{""ApiName"":""alpha"",""ApiKind"":""Int32"",""ApiValue"":24},{""ApiName"":""zeta"",""ApiKind"":""Int32"",""ApiValue"":42}]}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "zeta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        // Empty
        new JsonRoundtripTest
        {
            Name = "Empty",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Empty
                )
            )
        },

        // Scalars

        // .. String
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.String)}:alpha",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.String,
                    StringValue: "alpha"
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.String)}:ALPHA",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.String,
                    StringValue: "ALPHA"
                )
            )
        },

        // .. Int32
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Int32)}:42",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Int32,
                    Int32Value: 42
                )
            )
        },

        // .. Int64
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Int64)}:24",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Int64,
                    Int64Value: 24
                )
            )
        },

        // .. Guid
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Guid)}:{TestGuid}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Guid,
                    GuidValue: TestGuid
                )
            )
        },

        // .. Ulid
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Ulid)}:{TestUlid}",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Ulid,
                    UlidValue: TestUlid
                )
            )
        },

        // .. Culture
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Culture)}:en-US",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Culture,
                    CultureValue: "en-US"
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Culture)}:fr-FR",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Culture,
                    CultureValue: "fr-FR"
                )
            )
        },

        // Composites

        // .. Ordered (unnamed parts)
        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:24|24",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            )
        },

        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:24|42",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },

        // .. Named (named parts)
        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:alpha=24|beta=24",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            )
        },
        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:alpha=24|beta=42",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },
        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:alpha=24|zeta=42",
            ExpectedFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "zeta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        // Empty
        new JsonSerializeTest
        {
            Name = "Empty",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Empty
                )
            ),
            ExpectedJson = "null"
        },

        // Scalars

        // .. String
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.String)}:alpha",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.String,
                    StringValue: "alpha"
                )
            ),
            ExpectedJson = @"{""ApiKind"":""String"",""ApiValue"":""alpha""}"
        },

        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.String)}:ALPHA",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.String,
                    StringValue: "ALPHA"
                )
            ),
            ExpectedJson = @"{""ApiKind"":""String"",""ApiValue"":""ALPHA""}"
        },

        // .. Int32
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Int32)}:42",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Int32,
                    Int32Value: 42
                )
            ),
            ExpectedJson = @"{""ApiKind"":""Int32"",""ApiValue"":42}"
        },

        // .. Int64
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Int64)}:24",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Int64,
                    Int64Value: 24
                )
            ),
            ExpectedJson = @"{""ApiKind"":""Int64"",""ApiValue"":24}"
        },

        // .. Guid
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Guid)}:{TestGuid}",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Guid,
                    GuidValue: TestGuid
                )
            ),
            ExpectedJson = @"{""ApiKind"":""Guid"",""ApiValue"":""" + TestGuid + @"""}"
        },

        // .. Ulid
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Ulid)}:{TestUlid}",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Ulid,
                    UlidValue: TestUlid
                )
            ),
            ExpectedJson = @"{""ApiKind"":""Ulid"",""ApiValue"":""" + TestUlid + @"""}"
        },

        // .. Culture
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Culture)}:en-US",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Culture,
                    CultureValue: "en-US"
                )
            ),
            ExpectedJson = @"{""ApiKind"":""Culture"",""ApiValue"":""en-US""}"
        },

        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiIdKind.Culture)}:fr-FR",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                ScalarConfig: new ApiIdScalarConfig
                (
                    Kind: ApiIdKind.Culture,
                    CultureValue: "fr-FR"
                )
            ),
            ExpectedJson = @"{""ApiKind"":""Culture"",""ApiValue"":""fr-FR""}"
        },

        // Composites

        // .. Ordered (unnamed parts)
        new JsonSerializeTest
        {
            Name = $"Composite: Composite:24|24",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""ApiKind"":""Composite"",""ApiValue"":[{""ApiKind"":""Int32"",""ApiValue"":24},{""ApiKind"":""Int32"",""ApiValue"":24}]}"
        },

        new JsonSerializeTest
        {
            Name = $"Composite: Composite:24|42",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""ApiKind"":""Composite"",""ApiValue"":[{""ApiKind"":""Int32"",""ApiValue"":24},{""ApiKind"":""Int32"",""ApiValue"":42}]}"
        },

        // .. Named (named parts)
        new JsonSerializeTest
        {
            Name = $"Composite: Composite:alpha=24|beta=24",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""ApiKind"":""Composite"",""ApiValue"":[{""ApiName"":""alpha"",""ApiKind"":""Int32"",""ApiValue"":24},{""ApiName"":""beta"",""ApiKind"":""Int32"",""ApiValue"":24}]}"
        },
        new JsonSerializeTest
        {
            Name = $"Composite: Composite:alpha=24|beta=42",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""ApiKind"":""Composite"",""ApiValue"":[{""ApiName"":""alpha"",""ApiKind"":""Int32"",""ApiValue"":24},{""ApiName"":""beta"",""ApiKind"":""Int32"",""ApiValue"":42}]}"
        },
        new JsonSerializeTest
        {
            Name = $"Composite: Composite:alpha=24|zeta=42",
            SourceFactoryArgument = new ApiIdDescriptor
            (
                CompositePartsConfig: [
                    new ApiIdCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiIdCompositePartConfig
                    (
                        Name: "zeta",
                        ScalarConfig: new ApiIdScalarConfig
                        (
                            Kind: ApiIdKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""ApiKind"":""Composite"",""ApiValue"":[{""ApiName"":""alpha"",""ApiKind"":""Int32"",""ApiValue"":24},{""ApiName"":""zeta"",""ApiKind"":""Int32"",""ApiValue"":42}]}"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonDeserializeTheoryData))]
    public void JsonDeserialize(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonRoundtripTheoryData))]
    public void JsonRoundtrip(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonSerializeTheoryData))]
    public void JsonSerialize(IXUnitTest test) => test.Execute(this);
    #endregion
}
