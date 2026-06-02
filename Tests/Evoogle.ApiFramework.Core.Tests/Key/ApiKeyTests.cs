// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Globalization;

using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.XUnit.Tests.JsonUnitTests;

namespace Evoogle.ApiFramework.Key;

public partial class ApiKeyTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private enum ApiKeyFromFactory
    {
        FromString,
        FromInt32,
        FromInt64,
        FromGuid,
        FromUlid,
        FromCulture
    }

    private enum ApiKeyCompositeFactory
    {
        Named,
        Ordered
    }

    private record ApiKeyDescriptor
    (
        ApiKeyScalarConfig? ScalarConfig = null,
        ApiKeyCompositePartConfig[]? CompositePartsConfig = null
    );

    private record ApiKeyScalarConfig
    (
        ApiKeyKind Kind,

        string? CultureValue = null,
        Guid? GuidValue = null,
        int? Int32Value = null,
        long? Int64Value = null,
        string? StringValue = null,
        Ulid? UlidValue = null
    );

    private record ApiKeyCompositePartConfig
    (
        ApiKeyScalarConfig ScalarConfig,
        string? Name = null
    );

    private class ComparisonTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiKey Left { get; init; }
        public ApiKey Right { get; init; }
        public int ExpectedSign { get; init; }
        #endregion

        #region Calculated Properties
        private int ActualLeftToRightCompareToMethod { get; set; }
        private int ActualRightToLeftCompareToMethod { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Left:  {this.Left.SafeToString()}");
            this.WriteLine($"Right: {this.Right.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected Sign: {this.ExpectedSign}");
        }

        protected override void Act()
        {
            this.ActualLeftToRightCompareToMethod = this.Left.CompareTo(this.Right);
            this.ActualRightToLeftCompareToMethod = this.Right.CompareTo(this.Left);

            this.WriteLine($"Actual LeftToRight CompareTo Method: {this.ActualLeftToRightCompareToMethod}");
            this.WriteLine($"Actual RightToLeft CompareTo Method: {this.ActualRightToLeftCompareToMethod}");
        }

        protected override void Assert()
        {
            this.ActualLeftToRightCompareToMethod.Should().Be(this.ExpectedSign, "CompareTo should return the expected ordering");
            this.ActualRightToLeftCompareToMethod.Should().Be(-this.ExpectedSign, "CompareTo should be antisymmetric");
        }
        #endregion
    }

    private class CompositeTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiKeyCompositeFactory Factory { get; init; }
        public ApiKey[]? KeyCollection { get; init; } = null;
        public ApiKeyPart[]? PartCollection { get; init; } = null;
        #endregion

        #region Calculated Properties
        private ApiKey ExpectedApiKey { get; set; }
        private ApiKey ActualApiKey { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Factory:        {this.Factory}");
            this.WriteLine($"KeyCollection:  [{this.KeyCollection.SafeToDelimitedString(',')}]");
            this.WriteLine($"PartCollection: [{this.PartCollection.SafeToDelimitedString(',')}]");
            this.WriteLine();

            this.ExpectedApiKey = this.CreateExpectedApiKey();

            this.WriteLine($"Expected ApiKey: {this.ExpectedApiKey.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualApiKey = this.CreateActualApiKey();

            this.WriteLine($"Actual ApiKey:   {this.ActualApiKey.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualApiKey.Should().BeEquivalentTo(this.ExpectedApiKey);
        }
        #endregion

        #region Factory Methods
        private ApiKey CreateActualApiKey()
            => this.Factory switch
            {
                ApiKeyCompositeFactory.Named => ApiKey.Composite(this.PartCollection!),
                ApiKeyCompositeFactory.Ordered => ApiKey.Composite(this.KeyCollection!),
                _ => throw new InvalidOperationException($"Unsupported factory {this.Factory}.")
            };

        private ApiKey CreateExpectedApiKey()
        {
            var partCollection = default(ApiKeyPart[]?);
            switch (this.Factory)
            {
                case ApiKeyCompositeFactory.Named:
                    {
                        if (this.PartCollection is null || this.PartCollection.Length == 0)
                        {
                            return ApiKey.Empty;
                        }

                        partCollection = this.PartCollection!;
                        break;
                    }

                case ApiKeyCompositeFactory.Ordered:
                    {
                        if (this.KeyCollection is null || this.KeyCollection.Length == 0)
                        {
                            return ApiKey.Empty;
                        }

                        partCollection = [.. this.KeyCollection.Select(id => new ApiKeyPart(null, id))];
                        break;
                    }
            }

            if (partCollection is not null)
            {
                var apiId = new ApiKey
                (
                    ApiKeyKind.Composite,
                    default,
                    partCollection,
                    ApiKey.ToCompositeString(partCollection)
                );
                return apiId;
            }

            throw new InvalidOperationException($"Unsupported factory {this.Factory}.");
        }
        #endregion
    }

    private class EqualityTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiKey Left { get; init; }
        public ApiKey Right { get; init; }
        public bool ExpectedEqual { get; init; }
        #endregion

        #region Calculated Properties
        private bool ActualLeftToRightEqualsMethod { get; set; }
        private bool ActualRightToLeftEqualsMethod { get; set; }
        private bool ActualLeftToRightEqualsOperator { get; set; }
        private bool ActualLeftToRightNotEqualsOperator { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Left:  {this.Left.SafeToString()}");
            this.WriteLine($"Right: {this.Right.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected Equal: {this.ExpectedEqual}");
        }

        protected override void Act()
        {
            this.ActualLeftToRightEqualsMethod = this.Left.Equals(this.Right);
            this.ActualRightToLeftEqualsMethod = this.Right.Equals(this.Left);
            this.ActualLeftToRightEqualsOperator = this.Left == this.Right;
            this.ActualLeftToRightNotEqualsOperator = this.Left != this.Right;

            this.WriteLine($"Actual LeftToRight Equals Method:      {this.ActualLeftToRightEqualsMethod}");
            this.WriteLine($"Actual RightToLeft Equals Method:      {this.ActualRightToLeftEqualsMethod}");
            this.WriteLine($"Actual LeftToRight Equals Operator:    {this.ActualLeftToRightEqualsOperator}");
            this.WriteLine($"Actual LeftToRight NotEquals Operator: {this.ActualLeftToRightNotEqualsOperator}");
        }

        protected override void Assert()
        {
            this.ActualLeftToRightEqualsMethod.Should().Be(this.ExpectedEqual, "Equals should match expectation");
            this.ActualRightToLeftEqualsMethod.Should().Be(this.ExpectedEqual, "Equals should be symmetric");
            this.ActualLeftToRightEqualsOperator.Should().Be(this.ExpectedEqual, "operator == should match expectation");
            this.ActualLeftToRightNotEqualsOperator.Should().Be(!this.ExpectedEqual, "operator != should match expectation");

            if (this.ExpectedEqual)
            {
                this.Left.GetHashCode().Should().Be(this.Right.GetHashCode(), "equal values must produce identical hashes");
            }
        }
        #endregion
    }

    private class FromScalarTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiKeyFromFactory Factory { get; init; }
        public string Value { get; init; } = null!;
        public ApiKey ExpectedApiKey { get; init; }
        #endregion

        #region Calculated Properties
        private ApiKey ActualApiKey { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Factory: {this.Factory}");
            this.WriteLine($"Value:   {this.Value.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected ApiKey: {this.ExpectedApiKey.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualApiKey = this.CreateApiKey();

            this.WriteLine($"Actual ApiKey:   {this.ActualApiKey.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualApiKey.Should().BeEquivalentTo(this.ExpectedApiKey);
        }
        #endregion

        #region Factory Methods
        private ApiKey CreateApiKey()
            => this.Factory switch
            {
                ApiKeyFromFactory.FromString => ApiKey.FromString(this.Value),
                ApiKeyFromFactory.FromInt32 => ApiKey.FromInt32(int.Parse(this.Value, CultureInfo.InvariantCulture)),
                ApiKeyFromFactory.FromInt64 => ApiKey.FromInt64(long.Parse(this.Value, CultureInfo.InvariantCulture)),
                ApiKeyFromFactory.FromGuid => ApiKey.FromGuid(Guid.Parse(this.Value)),
                ApiKeyFromFactory.FromUlid => ApiKey.FromUlid(Ulid.Parse(this.Value)),
                ApiKeyFromFactory.FromCulture => ApiKey.FromCulture(this.Value),
                _ => throw new InvalidOperationException($"Unsupported factory {this.Factory}.")
            };
        #endregion
    }

    private class JsonDeserializeTest : JsonDeserializeTest<ApiKey, ApiKeyDescriptor>
    {
        #region JsonDeserializeTest<T, TFactoryArg> Methods
        protected override ApiKey CreateExpected(ApiKeyDescriptor? descriptor)
        {
            return BuildApiKey(descriptor);
        }
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiKey, ApiKeyDescriptor>
    {
        #region JsonRoundtripTest<T, TFactoryArg> Methods
        protected override ApiKey CreateExpected(ApiKeyDescriptor? descriptor)
        {
            return BuildApiKey(descriptor);
        }
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiKey, ApiKeyDescriptor>
    {
        #region JsonSerializeTest<T, TFactoryArg> Methods
        protected override ApiKey CreateSource(ApiKeyDescriptor? descriptor)
        {
            return BuildApiKey(descriptor);
        }
        #endregion
    }

    private class TryParseTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiKeyKind Kind { get; init; }
        public string? Text { get; init; }
        public bool ExpectedResult { get; init; }
        public ApiKey? ExpectedApiKey { get; init; }
        #endregion

        #region Calculated Properties
        private bool? ActualResult { get; set; }
        private ApiKey? ActualApiKey { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Kind: {this.Kind}");
            this.WriteLine($"Text: {this.Text.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"Expected Result: {this.ExpectedResult}");
            this.WriteLine($"Expected ApiKey: {this.ExpectedApiKey.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = ApiKey.TryParse(this.Kind, this.Text, out var value);
            this.ActualApiKey = value;
            this.WriteLine($"Actual Result: {this.ActualResult}");
            this.WriteLine($"Actual ApiKey: {this.ActualApiKey.SafeToString()}");
        }

        protected override void Assert()
        {
            if (this.ExpectedResult)
            {
                this.ActualResult.Should().BeTrue();
                this.ActualApiKey.Should().NotBeNull();
                this.ActualApiKey.Should().BeEquivalentTo(this.ExpectedApiKey);
            }
            else
            {
                this.ActualResult.Should().BeFalse();
                this.ActualApiKey.Should().Be(ApiKey.Empty);
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    private static ApiKey TestStringAlphaApiKey { get; } = ApiKey.FromString("alpha");
    private static ApiKey TestStringALPHAApiKey { get; } = ApiKey.FromString("ALPHA");

    private static ApiKey TestStringBetaApiKey { get; } = ApiKey.FromString("beta");

    private static ApiKey TestInt24ApiKey { get; } = ApiKey.FromInt32(24);
    private static ApiKey TestInt42ApiKey { get; } = ApiKey.FromInt32(42);

    private static ApiKey TestLong24ApiKey { get; } = ApiKey.FromInt64(24);
    private static ApiKey TestLong42ApiKey { get; } = ApiKey.FromInt64(42);

    private static ApiKey TestCultureEnUsApiKey { get; } = ApiKey.FromCulture("en-us");
    private static ApiKey TestCultureFrFrApiKey { get; } = ApiKey.FromCulture("fr-fr");

    private static ApiKey TestCompositeInt24AndInt24ApiKey { get; } = ApiKey.Composite
    (
        ApiKeyPart.Create(ApiKey.FromInt32(24)),
        ApiKeyPart.Create(ApiKey.FromInt32(24))
    );

    private static ApiKey TestCompositeInt24AndInt42ApiKey { get; } = ApiKey.Composite
    (
        ApiKeyPart.Create(ApiKey.FromInt32(24)),
        ApiKeyPart.Create(ApiKey.FromInt32(42))
    );

    private static ApiKey TestCompositeInt24AndInt42AndInt48ApiKey { get; } = ApiKey.Composite
    (
        ApiKeyPart.Create(ApiKey.FromInt32(24)),
        ApiKeyPart.Create(ApiKey.FromInt32(42)),
        ApiKeyPart.Create(ApiKey.FromInt32(48))
    );

    private static ApiKey TestCompositeAlphaInt24AndBetaInt24ApiKey { get; } = ApiKey.Composite
    (
        ApiKeyPart.Create("alpha", ApiKey.FromInt32(24)),
        ApiKeyPart.Create("beta", ApiKey.FromInt32(24))
    );

    private static ApiKey TestCompositeAlphaInt24AndBetaInt42ApiKey { get; } = ApiKey.Composite
    (
        ApiKeyPart.Create("alpha", ApiKey.FromInt32(24)),
        ApiKeyPart.Create("beta", ApiKey.FromInt32(42))
    );

    private static ApiKey TestCompositeAlphaInt24AndZetaInt42ApiKey { get; } = ApiKey.Composite
    (
        ApiKeyPart.Create("alpha", ApiKey.FromInt32(24)),
        ApiKeyPart.Create("zeta", ApiKey.FromInt32(42))
    );

    public const string TestGuidString = "86d5d1a9-ec14-4730-8d9a-41812e5a117a";
    public static Guid TestGuid { get; } = Guid.Parse(TestGuidString);
    private static ApiKey TestGuidApiKey { get; } = ApiKey.FromGuid(TestGuid);

    public const string TestUlidString = "46TQ8TKV0M8WR8V6J1G4Q5M4BT";
    public static Ulid TestUlid { get; } = Ulid.Parse(TestUlidString);
    private static ApiKey TestUlidApiKey { get; } = ApiKey.FromUlid(TestUlid);
    #endregion

    #region Helper Methods
    private static ApiKey BuildApiKey(ApiKeyDescriptor? descriptor)
    {
        if (descriptor != null && descriptor.ScalarConfig != null)
        {
            var scalarConfig = descriptor.ScalarConfig;
            return scalarConfig.Kind switch
            {
                ApiKeyKind.Empty => ApiKey.Empty,
                ApiKeyKind.Culture when scalarConfig.CultureValue != null => ApiKey.FromCulture(scalarConfig.CultureValue),
                ApiKeyKind.Guid when scalarConfig.GuidValue != null => ApiKey.FromGuid(scalarConfig.GuidValue.Value),
                ApiKeyKind.Int32 when scalarConfig.Int32Value != null => ApiKey.FromInt32(scalarConfig.Int32Value.Value),
                ApiKeyKind.Int64 when scalarConfig.Int64Value != null => ApiKey.FromInt64(scalarConfig.Int64Value.Value),
                ApiKeyKind.String when scalarConfig.StringValue != null => ApiKey.FromString(scalarConfig.StringValue),
                ApiKeyKind.Ulid when scalarConfig.UlidValue != null => ApiKey.FromUlid(scalarConfig.UlidValue.Value),
                _ => throw new InvalidOperationException($"Invalid {nameof(ApiKeyDescriptor)}: {nameof(ApiKeyDescriptor.ScalarConfig)} is missing required value.")
            };
        }
        else if (descriptor != null && descriptor.CompositePartsConfig != null && descriptor.CompositePartsConfig.Length > 0)
        {
            var parts = descriptor.CompositePartsConfig.Select(partConfig =>
            {
                var partApiKey = BuildApiKey(new ApiKeyDescriptor(partConfig.ScalarConfig, null));
                return ApiKeyPart.Create(partConfig.Name, partApiKey);
            }).ToArray();

            return ApiKey.Composite(parts);
        }

        return ApiKey.Empty;
    }
    #endregion
}
