// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Globalization;

using Evoogle.ApiFramework.Identity;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Identity.Tests;

public class ApiIdTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    public enum ApiIdFactory
    {
        FromString,
        FromInt32,
        FromInt64,
        FromGuid,
        FromUlid,
        FromCulture
    }

    public class ScalarApiIdTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdFactory Factory { get; init; }
        public string Value { get; init; } = null!;
        public ApiIdKind ExpectedKind { get; init; }
        public string? ExpectedString { get; init; }
        public string? ExpectedOriginalString { get; init; }
        #endregion

        #region Calculated Properties
        private ApiId Source { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Source = this.Factory switch
            {
                ApiIdFactory.FromString => ApiId.FromString(this.Value),
                ApiIdFactory.FromInt32 => ApiId.FromInt32(int.Parse(this.Value, CultureInfo.InvariantCulture)),
                ApiIdFactory.FromInt64 => ApiId.FromInt64(long.Parse(this.Value, CultureInfo.InvariantCulture)),
                ApiIdFactory.FromGuid => ApiId.FromGuid(Guid.Parse(this.Value)),
                ApiIdFactory.FromUlid => ApiId.FromUlid(Ulid.Parse(this.Value)),
                ApiIdFactory.FromCulture => ApiId.FromCulture(CultureInfo.GetCultureInfo(this.Value)),
                _ => throw new InvalidOperationException($"Unsupported factory {this.Factory}.")
            };

            this.WriteLine($"Source Kind: {this.Source.Kind}");
            this.WriteLine($"Expected Kind: {this.ExpectedKind}");
            this.WriteLine($"Factory: {this.Factory}");
            this.WriteLine($"Value: {this.Value.SafeToString()}");
            this.WriteLine($"Expected String: {this.ExpectedString.SafeToString()}");
            this.WriteLine($"Expected Original: {this.ExpectedOriginalString.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.WriteLine($"Actual Kind: {this.Source.Kind}");
            this.WriteLine($"Actual HasValue: {this.Source.HasValue}");
            this.WriteLine($"Actual ToString(): {this.Source.ToString().SafeToString()}");
            this.WriteLine($"Actual Original: {this.Source.OriginalString.SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.Source.HasValue.Should().BeTrue();
            this.Source.Kind.Should().Be(this.ExpectedKind);
            this.Source.IsString.Should().Be(this.ExpectedKind == ApiIdKind.String);
            this.Source.IsInt32.Should().Be(this.ExpectedKind == ApiIdKind.Int32);
            this.Source.IsInt64.Should().Be(this.ExpectedKind == ApiIdKind.Int64);
            this.Source.IsGuid.Should().Be(this.ExpectedKind == ApiIdKind.Guid);
            this.Source.IsUlid.Should().Be(this.ExpectedKind == ApiIdKind.Ulid);
            this.Source.IsCulture.Should().Be(this.ExpectedKind == ApiIdKind.Culture);
            this.Source.IsComposite.Should().BeFalse();
            this.Source.PartCount.Should().Be(1);
            this.Source.Parts.IsEmpty.Should().BeTrue();

            if (this.ExpectedString is not null)
            {
                this.Source.ToString().Should().Be(this.ExpectedString);
            }

            if (this.ExpectedOriginalString is not null)
            {
                this.Source.OriginalString.Should().Be(this.ExpectedOriginalString);
            }

            switch (this.ExpectedKind)
            {
                case ApiIdKind.String:
                    this.Source.AsStringOrThrow().Should().Be(this.Value);
                    this.Source.TryGet(out string? s).Should().BeTrue();
                    s.Should().Be(this.Value);
                    break;
                case ApiIdKind.Int32:
                    var expectedInt32 = int.Parse(this.Value, CultureInfo.InvariantCulture);
                    this.Source.AsInt32OrThrow().Should().Be(expectedInt32);
                    this.Source.TryGet(out int i32).Should().BeTrue();
                    i32.Should().Be(expectedInt32);
                    break;
                case ApiIdKind.Int64:
                    var expectedInt64 = long.Parse(this.Value, CultureInfo.InvariantCulture);
                    this.Source.AsInt64OrThrow().Should().Be(expectedInt64);
                    this.Source.TryGet(out long i64).Should().BeTrue();
                    i64.Should().Be(expectedInt64);
                    break;
                case ApiIdKind.Guid:
                    var expectedGuid = Guid.Parse(this.Value);
                    this.Source.AsGuidOrThrow().Should().Be(expectedGuid);
                    this.Source.TryGet(out Guid guid).Should().BeTrue();
                    guid.Should().Be(expectedGuid);
                    break;
                case ApiIdKind.Ulid:
                    var expectedUlid = Ulid.Parse(this.Value);
                    this.Source.AsUlidOrThrow().Should().Be(expectedUlid);
                    this.Source.TryGet(out Ulid ulid).Should().BeTrue();
                    ulid.Should().Be(expectedUlid);
                    break;
                case ApiIdKind.Culture:
                    var expectedCulture = CultureInfo.GetCultureInfo(this.Value);
                    this.Source.AsCultureOrThrow().Should().BeEquivalentTo(expectedCulture);
                    this.Source.TryGet(out CultureInfo? culture).Should().BeTrue();
                    culture.Should().NotBeNull();
                    culture!.Name.Should().Be(expectedCulture.Name);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported scalar kind: {this.ExpectedKind}");
            }
        }
        #endregion
    }

    public class TryParseTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiIdKind Kind { get; init; }
        public string? Text { get; init; }
        public bool ExpectedResult { get; init; }
        public string? ExpectedValue { get; init; }
        #endregion

        #region Calculated Properties
        private bool ActualResult { get; set; }
        private ApiId ActualValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Kind: {this.Kind}");
            this.WriteLine($"Text: {this.Text.SafeToString()}");
            this.WriteLine($"Expected Result: {this.ExpectedResult}");
            this.WriteLine($"Expected Value: {this.ExpectedValue.SafeToString()}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualResult = ApiId.TryParse(this.Kind, this.Text, out var value);
            this.ActualValue = value;
            this.WriteLine($"Actual Result: {this.ActualResult}");
            this.WriteLine($"Actual Value: {this.ActualValue.SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualResult.Should().Be(this.ExpectedResult);
            if (this.ExpectedResult)
            {
                this.ActualValue.Kind.Should().Be(this.Kind);
                this.ActualValue.ToString().Should().Be(this.ExpectedValue);

                switch (this.Kind)
                {
                    case ApiIdKind.String:
                        this.ActualValue.AsStringOrThrow().Should().Be(this.ExpectedValue);
                        break;
                    case ApiIdKind.Int32:
                        this.ActualValue.AsInt32OrThrow().Should().Be(int.Parse(this.ExpectedValue!, CultureInfo.InvariantCulture));
                        break;
                    case ApiIdKind.Int64:
                        this.ActualValue.AsInt64OrThrow().Should().Be(long.Parse(this.ExpectedValue!, CultureInfo.InvariantCulture));
                        break;
                    case ApiIdKind.Guid:
                        this.ActualValue.AsGuidOrThrow().Should().Be(Guid.Parse(this.ExpectedValue!));
                        break;
                    case ApiIdKind.Ulid:
                        this.ActualValue.AsUlidOrThrow().Should().Be(Ulid.Parse(this.ExpectedValue!));
                        break;
                    case ApiIdKind.Culture:
                        this.ActualValue.AsCultureOrThrow().Name.Should().Be(this.ExpectedValue);
                        break;
                }
            }
            else
            {
                this.ActualValue.Should().Be(ApiId.Empty);
            }
        }
        #endregion
    }

    public readonly record struct CompositePartData(ApiIdKind Kind, string Value, string? Name = null);

    public class CompositeApiIdTest : XUnitTest
    {
        #region User Supplied Properties
        public CompositePartData[] PartsData { get; init; } = Array.Empty<CompositePartData>();
        public bool ExpectedIsNamed { get; init; }
        public bool ExpectedIsOrdered { get; init; }
        public string? LookupName { get; init; }
        public bool ExpectedLookupResult { get; init; }
        public ApiIdKind? ExpectedLookupKind { get; init; }
        public string? ExpectedLookupValue { get; init; }
        #endregion

        #region Calculated Properties
        private ApiId Source { get; set; }
        private ApiIdPart[] ExpectedParts { get; set; } = Array.Empty<ApiIdPart>();
        private bool? ActualLookupResult { get; set; }
        private ApiId ActualLookupValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var parts = new ApiIdPart[this.PartsData.Length];
            for (var i = 0; i < this.PartsData.Length; i++)
            {
                var partData = this.PartsData[i];
                var partValue = CreateApiId(partData.Kind, partData.Value);
                parts[i] = new ApiIdPart(partData.Name, partValue);
                this.WriteLine($"Expected Part: {parts[i].SafeToString()}");
            }

            this.ExpectedParts = parts;
            if (parts.Length > 0 && parts[0].Name is not null)
            {
                this.Source = ApiId.Composite(parts);
            }
            else
            {
                this.Source = ApiId.Composite(parts.Select(p => p.Value).ToArray());
            }

            this.WriteLine($"Expected IsNamed: {this.ExpectedIsNamed}");
            this.WriteLine($"Expected IsOrdered: {this.ExpectedIsOrdered}");
            if (this.LookupName is not null)
            {
                this.WriteLine($"Lookup Name: {this.LookupName}");
                this.WriteLine($"Expected Lookup Result: {this.ExpectedLookupResult}");
                this.WriteLine($"Expected Lookup Value: {this.ExpectedLookupValue.SafeToString()}");
            }
            this.WriteLine();
        }

        protected override void Act()
        {
            if (this.LookupName is not null)
            {
                this.ActualLookupResult = this.Source.TryGetPart(this.LookupName, out var value);
                this.ActualLookupValue = value;
                this.WriteLine($"Actual Lookup Result: {this.ActualLookupResult}");
                this.WriteLine($"Actual Lookup Value: {this.ActualLookupValue.SafeToString()}");
                this.WriteLine();
            }
        }

        protected override void Assert()
        {
            this.Source.Kind.Should().Be(ApiIdKind.Composite);
            this.Source.IsComposite.Should().BeTrue();
            this.Source.IsNamedComposite.Should().Be(this.ExpectedIsNamed);
            this.Source.IsOrderedComposite.Should().Be(this.ExpectedIsOrdered);
            this.Source.PartCount.Should().Be(this.ExpectedParts.Length);

            var parts = this.Source.Parts.ToArray();
            parts.Should().HaveCount(this.ExpectedParts.Length);
            for (var i = 0; i < parts.Length; i++)
            {
                parts[i].Name.Should().Be(this.ExpectedParts[i].Name);
                parts[i].Value.Should().Be(this.ExpectedParts[i].Value);
                this.Source[i].Should().Be(this.ExpectedParts[i].Value);
            }

            if (this.LookupName is not null)
            {
                this.ActualLookupResult.Should().NotBeNull();
                this.ActualLookupResult.Should().Be(this.ExpectedLookupResult);
                if (this.ExpectedLookupResult)
                {
                    this.ExpectedLookupKind.Should().NotBeNull();
                    this.ActualLookupValue.Kind.Should().Be(this.ExpectedLookupKind);
                    this.ActualLookupValue.ToString().Should().Be(this.ExpectedLookupValue);
                }
                else
                {
                    this.ActualLookupValue.Should().Be(ApiId.Empty);
                }
            }
        }
        #endregion

        #region Helpers
        private static ApiId CreateApiId(ApiIdKind kind, string value)
            => kind switch
            {
                ApiIdKind.String => ApiId.FromString(value),
                ApiIdKind.Int32 => ApiId.FromInt32(int.Parse(value, CultureInfo.InvariantCulture)),
                ApiIdKind.Int64 => ApiId.FromInt64(long.Parse(value, CultureInfo.InvariantCulture)),
                ApiIdKind.Guid => ApiId.FromGuid(Guid.Parse(value)),
                ApiIdKind.Ulid => ApiId.FromUlid(Ulid.Parse(value)),
                ApiIdKind.Culture => ApiId.FromCulture(CultureInfo.GetCultureInfo(value)),
                _ => throw new InvalidOperationException($"Unsupported composite part kind: {kind}")
            };
        #endregion
    }

    public class EmptyApiIdTest : XUnitTest
    {
        #region Calculated Properties
        private ApiId Source { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Source = ApiId.Empty;
            this.WriteLine("Testing ApiId.Empty");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.WriteLine($"Kind: {this.Source.Kind}");
            this.WriteLine($"HasValue: {this.Source.HasValue}");
            this.WriteLine($"PartCount: {this.Source.PartCount}");
            this.WriteLine($"ToString(): {this.Source.ToString().SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.Source.Kind.Should().Be(ApiIdKind.None);
            this.Source.HasValue.Should().BeFalse();
            this.Source.IsComposite.Should().BeFalse();
            this.Source.PartCount.Should().Be(0);
            this.Source.Parts.IsEmpty.Should().BeTrue();
            this.Source.ToString().Should().BeNull();
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] ScalarTheoryData =>
    [
        new ScalarApiIdTest
        {
            Name = "String scalar",
            Factory = ApiIdFactory.FromString,
            Value = "alpha",
            ExpectedKind = ApiIdKind.String,
            ExpectedString = "alpha",
            ExpectedOriginalString = "alpha"
        },
        new ScalarApiIdTest
        {
            Name = "Int32 scalar",
            Factory = ApiIdFactory.FromInt32,
            Value = "42",
            ExpectedKind = ApiIdKind.Int32,
            ExpectedString = "42",
            ExpectedOriginalString = "42"
        },
        new ScalarApiIdTest
        {
            Name = "Int64 scalar",
            Factory = ApiIdFactory.FromInt64,
            Value = "8675309",
            ExpectedKind = ApiIdKind.Int64,
            ExpectedString = "8675309",
            ExpectedOriginalString = "8675309"
        },
        new ScalarApiIdTest
        {
            Name = "Guid scalar",
            Factory = ApiIdFactory.FromGuid,
            Value = "5bd5095b-2a77-4a63-8d13-20dce65d6cfe",
            ExpectedKind = ApiIdKind.Guid,
            ExpectedString = "5bd5095b-2a77-4a63-8d13-20dce65d6cfe",
            ExpectedOriginalString = "5bd5095b-2a77-4a63-8d13-20dce65d6cfe"
        },
        new ScalarApiIdTest
        {
            Name = "Ulid scalar",
            Factory = ApiIdFactory.FromUlid,
            Value = "01ARZ3NDEKTSV4RRFFQ69G5FAV",
            ExpectedKind = ApiIdKind.Ulid,
            ExpectedString = "01ARZ3NDEKTSV4RRFFQ69G5FAV",
            ExpectedOriginalString = "01ARZ3NDEKTSV4RRFFQ69G5FAV"
        },
        new ScalarApiIdTest
        {
            Name = "Culture scalar",
            Factory = ApiIdFactory.FromCulture,
            Value = "en-US",
            ExpectedKind = ApiIdKind.Culture,
            ExpectedString = "en-US",
            ExpectedOriginalString = "en-US"
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] TryParseTheoryData =>
    [
        new TryParseTest
        {
            Name = "TryParse string success",
            Kind = ApiIdKind.String,
            Text = "orders/123",
            ExpectedResult = true,
            ExpectedValue = "orders/123"
        },
        new TryParseTest
        {
            Name = "TryParse int32 success",
            Kind = ApiIdKind.Int32,
            Text = "123",
            ExpectedResult = true,
            ExpectedValue = "123"
        },
        new TryParseTest
        {
            Name = "TryParse culture success",
            Kind = ApiIdKind.Culture,
            Text = "fr-FR",
            ExpectedResult = true,
            ExpectedValue = "fr-FR"
        },
        new TryParseTest
        {
            Name = "TryParse composite fails",
            Kind = ApiIdKind.Composite,
            Text = "part",
            ExpectedResult = false,
            ExpectedValue = null
        },
        new TryParseTest
        {
            Name = "TryParse invalid int32 fails",
            Kind = ApiIdKind.Int32,
            Text = "abc",
            ExpectedResult = false,
            ExpectedValue = null
        },
        new TryParseTest
        {
            Name = "TryParse whitespace fails",
            Kind = ApiIdKind.String,
            Text = "  ",
            ExpectedResult = false,
            ExpectedValue = null
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] CompositeTheoryData =>
    [
        new CompositeApiIdTest
        {
            Name = "Named composite",
            PartsData =
            [
                new CompositePartData(ApiIdKind.String, "US", "country"),
                new CompositePartData(ApiIdKind.Culture, "en-US", "language")
            ],
            ExpectedIsNamed = true,
            ExpectedIsOrdered = false,
            LookupName = "language",
            ExpectedLookupResult = true,
            ExpectedLookupKind = ApiIdKind.Culture,
            ExpectedLookupValue = "en-US"
        },
        new CompositeApiIdTest
        {
            Name = "Ordered composite",
            PartsData =
            [
                new CompositePartData(ApiIdKind.Int32, "1"),
                new CompositePartData(ApiIdKind.Int32, "2")
            ],
            ExpectedIsNamed = false,
            ExpectedIsOrdered = true,
            LookupName = "missing",
            ExpectedLookupResult = false,
            ExpectedLookupKind = null,
            ExpectedLookupValue = null
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] EmptyTheoryData =>
    [
        new EmptyApiIdTest
        {
            Name = "Empty instance"
        }
    ];
    #endregion

    #region Theory Methods
    [Theory]
    [MemberData(nameof(ScalarTheoryData))]
    public void Scalar_ids(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryParseTheoryData))]
    public void TryParse_ids(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(CompositeTheoryData))]
    public void Composite_ids(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(EmptyTheoryData))]
    public void Empty_id(IXUnitTest test) => test.Execute(this);
    #endregion
}
