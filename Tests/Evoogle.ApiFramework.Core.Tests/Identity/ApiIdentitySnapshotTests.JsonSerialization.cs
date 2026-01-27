// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.XUnit;

using FluentAssertions;
using Xunit.Sdk;

namespace Evoogle.ApiFramework.Identity;

public partial class ApiIdentitySnapshotTests
{
    #region Test Types
    private class JsonRoundTripTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiIdentitySnapshot Snapshot { get; init; }
        public bool ExpectResolvedRoundTrip { get; init; } = true;
        #endregion

        #region Calculated Properties
        private string? ActualJson { get; set; }
        private ApiIdentitySnapshot? ActualSnapshot { get; set; }
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            try
            {
                this.ActualJson = JsonSerializer.Serialize(this.Snapshot);
                this.ActualSnapshot = JsonSerializer.Deserialize<ApiIdentitySnapshot>(this.ActualJson);
            }
            catch (Exception ex)
            {
                this.ActualException = ex;
            }
        }

        protected override void Assert()
        {
            if (this.ActualException is not null)
            {
                var json = this.ActualJson ?? "<null>";
                throw new XunitException($"JSON round-trip failed with: {this.ActualException.GetType().Name}: {this.ActualException.Message}\nJSON:\n{json}");
            }

            this.ActualSnapshot.Should().NotBeNull();

            var roundTripped = this.ActualSnapshot!;

            roundTripped.Name.Should().Be(this.Snapshot.Name);
            roundTripped.Path.Should().Be(this.Snapshot.Path);
            roundTripped.IsScalar.Should().Be(this.Snapshot.IsScalar);
            roundTripped.IsComposite.Should().Be(this.Snapshot.IsComposite);
            roundTripped.PartCount.Should().Be(this.Snapshot.PartCount);
            roundTripped.IsFullyResolved.Should().Be(this.Snapshot.IsFullyResolved);
            roundTripped.PartNames.Should().Equal(this.Snapshot.PartNames);

            // Structural equivalence: compare deterministic flattening and unresolved parts.
            roundTripped.GetUnresolvedParts().Should().BeEquivalentTo(this.Snapshot.GetUnresolvedParts());

            var expectedNamed = this.Snapshot.ToApiId(useNamedParts: true, unresolvedBehavior: UnresolvedPartBehavior.UseEmpty);
            var expectedUnnamed = this.Snapshot.ToApiId(useNamedParts: false, unresolvedBehavior: UnresolvedPartBehavior.UseEmpty);

            var actualNamed = roundTripped.ToApiId(useNamedParts: true, unresolvedBehavior: UnresolvedPartBehavior.UseEmpty);
            var actualUnnamed = roundTripped.ToApiId(useNamedParts: false, unresolvedBehavior: UnresolvedPartBehavior.UseEmpty);

            actualNamed.Should().Be(expectedNamed);
            actualUnnamed.Should().Be(expectedUnnamed);

            if (this.ExpectResolvedRoundTrip)
            {
                // When fully resolved, Throw-mode should round-trip too.
                var expectedThrowNamed = this.Snapshot.ToApiId(useNamedParts: true, unresolvedBehavior: UnresolvedPartBehavior.Throw);
                var expectedThrowUnnamed = this.Snapshot.ToApiId(useNamedParts: false, unresolvedBehavior: UnresolvedPartBehavior.Throw);

                roundTripped.ToApiId(useNamedParts: true, unresolvedBehavior: UnresolvedPartBehavior.Throw).Should().Be(expectedThrowNamed);
                roundTripped.ToApiId(useNamedParts: false, unresolvedBehavior: UnresolvedPartBehavior.Throw).Should().Be(expectedThrowUnnamed);
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonRoundTripTheoryData =>
    [
        new JsonRoundTripTest
        {
            Name = "Scalar snapshot round-trips",
            Snapshot = ApiIdentitySnapshot.Scalar("ProductId", 42),
        },
        new JsonRoundTripTest
        {
            Name = "Empty snapshot round-trips",
            Snapshot = ApiIdentitySnapshot.Empty("Empty"),
        },
        new JsonRoundTripTest
        {
            Name = "Composite resolved snapshot round-trips",
            Snapshot = CreateOrderSnapshot(),
        },
        new JsonRoundTripTest
        {
            Name = "Composite unresolved snapshot round-trips",
            Snapshot = CreateUnresolvedSnapshot(),
            ExpectResolvedRoundTrip = false
        },
        new JsonRoundTripTest
        {
            Name = "Deeply nested snapshot round-trips",
            Snapshot = CreateDeeplyNestedSnapshot(),
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonRoundTripTheoryData))]
    public void JsonRoundTrip(IXUnitTest test) => test.Execute(this);
    #endregion
}
