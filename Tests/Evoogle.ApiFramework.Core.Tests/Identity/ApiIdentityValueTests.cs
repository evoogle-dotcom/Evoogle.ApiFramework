// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using static Evoogle.XUnit.Tests.JsonUnitTests;

namespace Evoogle.ApiFramework.Identity;

public partial class ApiIdentityValueTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private record ApiIdentityValueConfig
    {
        // ApiIdentityValue
        public ApiIdentityPartValueConfig[]? ApiParts { get; init; }
    };

    private record ApiIdentityPartValueConfig
    {
        // ApiIdentityPartValue
        public required string ApiName { get; init; }
        public required ApiIdentityPartValueKind ApiKind { get; init; }

        // ApiObjectIdentityPartValue
        public ApiIdentityValueConfig? ApiObjectValue { get; init; }
        public ApiIdentityPartValueConfig[]? ApiStructure { get; init; }

        // ApiScalarIdentityPartValue
        public ApiId? ApiScalarValue { get; init; }
    };

    private class JsonDeserializeTest : JsonDeserializeTest<ApiIdentityValue, ApiIdentityValueConfig>
    {
        #region Constructors
        public JsonDeserializeTest()
        {
            this.ExcludeMembers =
            [
                new ExcludeMember(typeof(ApiIdentityValue), nameof(ApiIdentityValue.ApiScalarValue)),
                new ExcludeMember(typeof(ApiIdentityValue), nameof(ApiIdentityValue.ApiObjectValue)),
            ];
        }
        #endregion

        #region JsonDeserializeTest<T, TFactoryArg> Methods
        protected override ApiIdentityValue? CreateExpected(ApiIdentityValueConfig? config)
        {
            return CreateApiIdentityValue(config);
        }
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiIdentityValue, ApiIdentityValueConfig>
    {
        #region Constructors
        public JsonRoundtripTest()
        {
            this.ExcludeMembers =
            [
                new ExcludeMember(typeof(ApiIdentityValue), nameof(ApiIdentityValue.ApiScalarValue)),
                new ExcludeMember(typeof(ApiIdentityValue), nameof(ApiIdentityValue.ApiObjectValue)),
            ];
        }
        #endregion

        #region JsonRoundtripTest<T, TFactoryArg> Methods
        protected override ApiIdentityValue? CreateExpected(ApiIdentityValueConfig? config)
        {
            return CreateApiIdentityValue(config);
        }
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiIdentityValue, ApiIdentityValueConfig>
    {
        #region JsonSerializeTest<T, TFactoryArg> Methods
        protected override ApiIdentityValue? CreateSource(ApiIdentityValueConfig? config)
        {
            return CreateApiIdentityValue(config);
        }
        #endregion
    }
    #endregion

    #region Test Factories
    private static ApiIdentityValue? CreateApiIdentityValue(ApiIdentityValueConfig? config)
    {
        if (config is null)
        {
            return null;
        }

        var apiParts = config.ApiParts?.Select(CreateApiIdentityPartValue);

        var apiIdentityValue = new ApiIdentityValue(apiParts);
        return apiIdentityValue;
    }

    private static ApiIdentityPartValue CreateApiIdentityPartValue(ApiIdentityPartValueConfig config)
    {
        ArgumentNullException.ThrowIfNull(config, nameof(config));

        var apiName = config.ApiName;
        var apiKind = config.ApiKind;

        switch (apiKind)
        {
            case ApiIdentityPartValueKind.Scalar:
                return new ApiScalarIdentityPartValue(apiName, config.ApiScalarValue.GetValueOrDefault());

            case ApiIdentityPartValueKind.Object:
                var apiObjectValue = config.ApiObjectValue is not null
                    ? CreateApiIdentityValue(config.ApiObjectValue)
                    : null;

                var apiStructure = config.ApiStructure?.Select(CreateApiIdentityPartValue);

                return new ApiObjectIdentityPartValue(apiName, apiObjectValue, apiStructure);

            default:
                throw new ArgumentException($"Unsupported ApiIdentityPartValueKind: {apiKind}. ApiName: {apiName}");
        }
    }
    #endregion

    #region Test Data
    private static ApiIdentityValue ScalarEmptyPart { get; } =
        ApiIdentityValue.Scalar("Id", ApiId.Empty);

    private static ApiIdentityValue ScalarIntegerPart { get; } =
        ApiIdentityValue.Scalar("Id", ApiId.FromInt32(42));

    private static ApiIdentityValue ObjectSinglePart { get; } =
        ApiIdentityValue.Object("User", ApiIdentityValue.Scalar("Id", ApiId.FromInt32(42)));

    private static ApiIdentityValue ObjectSinglePartUnresolved { get; } =
        new ApiIdentityValue([new ApiObjectIdentityPartValue("User", apiObjectValue: null)]);

    private static ApiIdentityValue CompositeWithScalarParts { get; } =
        ApiIdentityValue.Composite(
        [
            new ApiScalarIdentityPartValue("CustomerId", ApiId.FromInt32(42)),
            new ApiScalarIdentityPartValue("OrderNumber", ApiId.FromInt32(1001))
        ]);

    private static ApiIdentityValue CompositeWithObjectParts { get; } =
        ApiIdentityValue.Composite(
        [
            new ApiObjectIdentityPartValue("Customer",
                ApiIdentityValue.Composite(
                [
                    new ApiObjectIdentityPartValue("Country",
                        ApiIdentityValue.Composite(
                        [
                            new ApiScalarIdentityPartValue("Id", ApiId.FromInt32(1))
                        ])),
                    new ApiScalarIdentityPartValue("CustomerId", ApiId.FromInt32(42))
                ])),
            new ApiScalarIdentityPartValue("OrderNumber", ApiId.FromInt32(1001))
        ]);

    private static ApiIdentityValue CompositeWithUnresolvedObjectParts { get; } =
        ApiIdentityValue.Composite(
        [
            new ApiObjectIdentityPartValue("Customer", apiObjectValue: null, apiStructure:
            [
                new ApiObjectIdentityPartValue("Country", apiObjectValue: null, apiStructure:
                [
                    new ApiScalarIdentityPartValue("Id", ApiId.Empty)
                ]),
                new ApiScalarIdentityPartValue("CustomerId", ApiId.Empty)
            ]),
            new ApiScalarIdentityPartValue("OrderNumber", ApiId.Empty)
        ]);

    private static ApiIdentityValue CompositeWithPartiallyUnresolvedObjectParts { get; } =
        ApiIdentityValue.Composite(
        [
            new ApiObjectIdentityPartValue("Customer", apiObjectValue: null, apiStructure:
            [
                new ApiObjectIdentityPartValue("Country", apiObjectValue: null, apiStructure:
                [
                    new ApiScalarIdentityPartValue("Id", ApiId.Empty)
                ]),
                new ApiScalarIdentityPartValue("CustomerId", ApiId.Empty)
            ]),
            new ApiScalarIdentityPartValue("OrderNumber", ApiId.FromInt32(1001))
        ]);

    private static ApiIdentityValue CompositeWithDeepObjectParts { get; } =
        ApiIdentityValue.Composite(
        [
            new ApiObjectIdentityPartValue("Level1",
                ApiIdentityValue.Composite(
                [
                    new ApiObjectIdentityPartValue("Level2",
                        ApiIdentityValue.Composite(
                        [
                            new ApiObjectIdentityPartValue("Level3",
                                ApiIdentityValue.Composite(
                                [
                                    new ApiScalarIdentityPartValue("Level4", ApiId.FromString("DeepValue")),
                                    new ApiScalarIdentityPartValue("SequenceNumber", ApiId.FromInt32(42))
                                ]))
                        ]))
                ])),
            new ApiScalarIdentityPartValue("RootId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")))
        ]);
    #endregion
}
