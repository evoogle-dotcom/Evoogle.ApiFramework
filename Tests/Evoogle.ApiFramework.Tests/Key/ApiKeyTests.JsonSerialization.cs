// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Key;

public partial class ApiKeyTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        // Empty
        new JsonDeserializeTest
        {
            Name = "Empty",
            SourceJson = "null",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Empty
                )
            )
        },

        // Scalars

        // .. String
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.String)}:alpha",
            SourceJson = @"{""ApiKind"":""String"",""ClrValue"":""alpha""}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.String,
                    StringValue: "alpha"
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.String)}:ALPHA",
            SourceJson = @"{""ApiKind"":""String"",""ClrValue"":""ALPHA""}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.String,
                    StringValue: "ALPHA"
                )
            )
        },

        // .. Int32
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Int32)}:42",
            SourceJson = @"{""ApiKind"":""Int32"",""ClrValue"":42}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Int32,
                    Int32Value: 42
                )
            )
        },

        // .. Int64
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Int64)}:24",
            SourceJson = @"{""ApiKind"":""Int64"",""ClrValue"":24}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Int64,
                    Int64Value: 24
                )
            )
        },

        // .. Guid
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Guid)}:{TestGuid}",
            SourceJson = @"{""ApiKind"":""Guid"",""ClrValue"":""" + TestGuid + @"""}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Guid,
                    GuidValue: TestGuid
                )
            )
        },

        // .. Ulid
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Ulid)}:{TestUlid}",
            SourceJson = @"{""ApiKind"":""Ulid"",""ClrValue"":""" + TestUlid + @"""}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Ulid,
                    UlidValue: TestUlid
                )
            )
        },

        // .. Culture
        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Culture)}:en-US",
            SourceJson = @"{""ApiKind"":""Culture"",""ClrValue"":""en-US""}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Culture,
                    CultureValue: "en-US"
                )
            )
        },

        new JsonDeserializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Culture)}:fr-FR",
            SourceJson = @"{""ApiKind"":""Culture"",""ClrValue"":""fr-FR""}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Culture,
                    CultureValue: "fr-FR"
                )
            )
        },

        // Composites

        // .. Ordered (unnamed parts)
        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:24|24",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiParts"":[{""ApiKind"":""Int32"",""ClrValue"":24},{""ApiKind"":""Int32"",""ClrValue"":24}]}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            )
        },

        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:24|42",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiParts"":[{""ApiKind"":""Int32"",""ClrValue"":24},{""ApiKind"":""Int32"",""ClrValue"":42}]}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
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
            SourceJson = @"{""ApiKind"":""Composite"",""ApiParts"":[{""ApiName"":""alpha"",""ApiKind"":""Int32"",""ClrValue"":24},{""ApiName"":""beta"",""ApiKind"":""Int32"",""ClrValue"":24}]}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            )
        },
        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:alpha=24|beta=42",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiParts"":[{""ApiName"":""alpha"",""ApiKind"":""Int32"",""ClrValue"":24},{""ApiName"":""beta"",""ApiKind"":""Int32"",""ClrValue"":42}]}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },
        new JsonDeserializeTest
        {
            Name = $"Composite: Composite:alpha=24|zeta=42",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiParts"":[{""ApiName"":""alpha"",""ApiKind"":""Int32"",""ClrValue"":24},{""ApiName"":""zeta"",""ApiKind"":""Int32"",""ClrValue"":42}]}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        Name: "zeta",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeExceptionTheoryData =>
    [
        // Missing top-level ApiKind property
        new JsonDeserializeTest
        {
            Name = "Error: Missing top-level ApiKind",
            SourceJson = @"{""ClrValue"":""alpha""}",
            ExpectedFactoryArgument = null,
            ExpectedExceptionType = typeof(System.Text.Json.JsonException),
            ExpectedExceptionMessage = "Missing required property"
        },

        // Empty ApiParts array for Composite
        new JsonDeserializeTest
        {
            Name = "Error: Composite with empty ApiParts array",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiParts"":[]}",
            ExpectedFactoryArgument = null,
            ExpectedExceptionType = typeof(System.Text.Json.JsonException),
            ExpectedExceptionMessage = "requires non-empty array property"
        },

        // Null element inside ApiParts array — the base framework skips null array items,
        // so [null] produces an empty ApiParts list, triggering the same guard as [].
        new JsonDeserializeTest
        {
            Name = "Error: Composite with null element in ApiParts",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiParts"":[null]}",
            ExpectedFactoryArgument = null,
            ExpectedExceptionType = typeof(System.Text.Json.JsonException),
            ExpectedExceptionMessage = "requires non-empty array property"
        },

        // Missing ApiKind inside a composite part
        new JsonDeserializeTest
        {
            Name = "Error: Composite part missing ApiKind",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiParts"":[{""ClrValue"":42}]}",
            ExpectedFactoryArgument = null,
            ExpectedExceptionType = typeof(System.Text.Json.JsonException),
            ExpectedExceptionMessage = "Missing required property"
        },

        // Composite kind used as a composite part's ApiKind (exercises Issue 2 fix)
        new JsonDeserializeTest
        {
            Name = "Error: Composite part ApiKind is Composite",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiParts"":[{""ApiKind"":""Composite"",""ClrValue"":42}]}",
            ExpectedFactoryArgument = null,
            ExpectedExceptionType = typeof(System.Text.Json.JsonException),
            ExpectedExceptionMessage = "is not valid as a scalar value"
        },

        // Empty kind used as a composite part's ApiKind (exercises Issue 2 fix)
        new JsonDeserializeTest
        {
            Name = "Error: Composite part ApiKind is Empty",
            SourceJson = @"{""ApiKind"":""Composite"",""ApiParts"":[{""ApiKind"":""Empty"",""ClrValue"":42}]}",
            ExpectedFactoryArgument = null,
            ExpectedExceptionType = typeof(System.Text.Json.JsonException),
            ExpectedExceptionMessage = "is not valid as a scalar value"
        },

        // Int32 ClrValue out of range (exceeds int.MaxValue)
        new JsonDeserializeTest
        {
            Name = "Error: Int32 ClrValue out of range",
            SourceJson = @"{""ApiKind"":""Int32"",""ClrValue"":2147483648}",
            ExpectedFactoryArgument = null,
            ExpectedExceptionType = typeof(System.Text.Json.JsonException),
            ExpectedExceptionMessage = "out of range for Int32"
        },

        // Non-integer numeric ClrValue (floating-point) for Int32
        new JsonDeserializeTest
        {
            Name = "Error: Int32 ClrValue is floating-point",
            SourceJson = @"{""ApiKind"":""Int32"",""ClrValue"":1.5}",
            ExpectedFactoryArgument = null,
            ExpectedExceptionType = typeof(System.Text.Json.JsonException),
            ExpectedExceptionMessage = "could not be read as integer"
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        // Empty
        new JsonRoundtripTest
        {
            Name = "Empty",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Empty
                )
            )
        },

        // Scalars

        // .. String
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.String)}:alpha",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.String,
                    StringValue: "alpha"
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.String)}:ALPHA",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.String,
                    StringValue: "ALPHA"
                )
            )
        },

        // .. Int32
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Int32)}:42",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Int32,
                    Int32Value: 42
                )
            )
        },

        // .. Int64
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Int64)}:24",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Int64,
                    Int64Value: 24
                )
            )
        },

        // .. Guid
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Guid)}:{TestGuid}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Guid,
                    GuidValue: TestGuid
                )
            )
        },

        // .. Ulid
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Ulid)}:{TestUlid}",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Ulid,
                    UlidValue: TestUlid
                )
            )
        },

        // .. Culture
        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Culture)}:en-US",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Culture,
                    CultureValue: "en-US"
                )
            )
        },

        new JsonRoundtripTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Culture)}:fr-FR",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Culture,
                    CultureValue: "fr-FR"
                )
            )
        },

        // Composites

        // .. Ordered (unnamed parts)
        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:24|24",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            )
        },

        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:24|42",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
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
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            )
        },
        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:alpha=24|beta=42",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            )
        },
        new JsonRoundtripTest
        {
            Name = $"Composite: Composite:alpha=24|zeta=42",
            ExpectedFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        Name: "zeta",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
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
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Empty
                )
            ),
            ExpectedJson = "null"
        },

        // Scalars

        // .. String
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.String)}:alpha",
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.String,
                    StringValue: "alpha"
                )
            ),
            ExpectedJson = @"{""ApiKind"":""String"",""ClrValue"":""alpha""}"
        },

        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.String)}:ALPHA",
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.String,
                    StringValue: "ALPHA"
                )
            ),
            ExpectedJson = @"{""ApiKind"":""String"",""ClrValue"":""ALPHA""}"
        },

        // .. Int32
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Int32)}:42",
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Int32,
                    Int32Value: 42
                )
            ),
            ExpectedJson = @"{""ApiKind"":""Int32"",""ClrValue"":42}"
        },

        // .. Int64
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Int64)}:24",
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Int64,
                    Int64Value: 24
                )
            ),
            ExpectedJson = @"{""ApiKind"":""Int64"",""ClrValue"":24}"
        },

        // .. Guid
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Guid)}:{TestGuid}",
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Guid,
                    GuidValue: TestGuid
                )
            ),
            ExpectedJson = @"{""ApiKind"":""Guid"",""ClrValue"":""" + TestGuid + @"""}"
        },

        // .. Ulid
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Ulid)}:{TestUlid}",
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Ulid,
                    UlidValue: TestUlid
                )
            ),
            ExpectedJson = @"{""ApiKind"":""Ulid"",""ClrValue"":""" + TestUlid + @"""}"
        },

        // .. Culture
        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Culture)}:en-US",
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Culture,
                    CultureValue: "en-US"
                )
            ),
            ExpectedJson = @"{""ApiKind"":""Culture"",""ClrValue"":""en-US""}"
        },

        new JsonSerializeTest
        {
            Name = $"Scalar: {nameof(ApiKeyKind.Culture)}:fr-FR",
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                ScalarConfig: new ApiKeyScalarConfig
                (
                    Kind: ApiKeyKind.Culture,
                    CultureValue: "fr-FR"
                )
            ),
            ExpectedJson = @"{""ApiKind"":""Culture"",""ClrValue"":""fr-FR""}"
        },

        // Composites

        // .. Ordered (unnamed parts)
        new JsonSerializeTest
        {
            Name = $"Composite: Composite:24|24",
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""ApiKind"":""Composite"",""ApiParts"":[{""ApiKind"":""Int32"",""ClrValue"":24},{""ApiKind"":""Int32"",""ClrValue"":24}]}"
        },

        new JsonSerializeTest
        {
            Name = $"Composite: Composite:24|42",
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""ApiKind"":""Composite"",""ApiParts"":[{""ApiKind"":""Int32"",""ClrValue"":24},{""ApiKind"":""Int32"",""ClrValue"":42}]}"
        },

        // .. Named (named parts)
        new JsonSerializeTest
        {
            Name = $"Composite: Composite:alpha=24|beta=24",
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""ApiKind"":""Composite"",""ApiParts"":[{""ApiName"":""alpha"",""ApiKind"":""Int32"",""ClrValue"":24},{""ApiName"":""beta"",""ApiKind"":""Int32"",""ClrValue"":24}]}"
        },
        new JsonSerializeTest
        {
            Name = $"Composite: Composite:alpha=24|beta=42",
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        Name: "beta",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""ApiKind"":""Composite"",""ApiParts"":[{""ApiName"":""alpha"",""ApiKind"":""Int32"",""ClrValue"":24},{""ApiName"":""beta"",""ApiKind"":""Int32"",""ClrValue"":42}]}"
        },
        new JsonSerializeTest
        {
            Name = $"Composite: Composite:alpha=24|zeta=42",
            SourceFactoryArgument = new ApiKeyDescriptor
            (
                CompositePartsConfig: [
                    new ApiKeyCompositePartConfig
                    (
                        Name: "alpha",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 24
                        )
                    ),
                    new ApiKeyCompositePartConfig
                    (
                        Name: "zeta",
                        ScalarConfig: new ApiKeyScalarConfig
                        (
                            Kind: ApiKeyKind.Int32,
                            Int32Value: 42
                        )
                    )
                ]
            ),
            ExpectedJson = @"{""ApiKind"":""Composite"",""ApiParts"":[{""ApiName"":""alpha"",""ApiKind"":""Int32"",""ClrValue"":24},{""ApiName"":""zeta"",""ApiKind"":""Int32"",""ClrValue"":42}]}"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonDeserializeTheoryData))]
    public void JsonDeserialize(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonDeserializeExceptionTheoryData))]
    public void JsonDeserializeException(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonRoundtripTheoryData))]
    public void JsonRoundtrip(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonSerializeTheoryData))]
    public void JsonSerialize(IXUnitTest test) => test.Execute(this);
    #endregion
}
