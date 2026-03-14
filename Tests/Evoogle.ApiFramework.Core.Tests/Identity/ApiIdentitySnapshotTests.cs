// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

#if false
public partial class ApiIdentitySnapshotTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private record ApiIdentitySnapshotConfig
    {
        public required ApiIdentitySnapshotKind Kind { get; init; }
        public ApiId? ScalarValue { get; init; }
        public List<ApiIdentityPartConfig>? NestedParts { get; init; }
    }

    private record ApiIdentityPartConfig
    (
        string Name,
        ApiIdentitySnapshotConfig? Snapshot,
        List<ApiIdentityPartConfig>? Structure
    );

    private class JsonDeserializeTest : JsonDeserializeTest<ApiIdentitySnapshot, ApiIdentitySnapshotConfig>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonDeserializeTest()
        {
            this.ExpectedFactoryExpression = (arg) => CreateSnapshot(arg!);
        }
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiIdentitySnapshot, ApiIdentitySnapshotConfig>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonRoundtripTest()
        {
            this.ExpectedFactoryExpression = (arg) => CreateSnapshot(arg!);
        }
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiIdentitySnapshot, ApiIdentitySnapshotConfig>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonSerializeTest()
        {
            this.SourceFactoryExpression = (arg) => CreateSnapshot(arg!);
        }
        #endregion
    }
    #endregion

    #region Test Factories
    private static ApiIdentitySnapshot CreateSnapshot(ApiIdentitySnapshotConfig config)
    {
        switch (config.Kind)
        {
            case ApiIdentitySnapshotKind.Scalar:
                if (config.ScalarValue is null)
                {
                    throw new ArgumentException("ScalarValue must be provided for scalar snapshot.");
                }

                var scalarValue = config.ScalarValue.Value;
                return ApiIdentitySnapshot.Scalar(scalarValue);

            case ApiIdentitySnapshotKind.Composite:
                var parts = config.NestedParts?.ConvertAll(CreatePartFromConfig).ToArray() ?? [];
                return ApiIdentitySnapshot.Composite(parts);

            default:
                throw new ArgumentOutOfRangeException(nameof(config), config.Kind, $"Invalid {config.Kind} value.");
        }
    }

    private static ApiIdentityPart CreatePartFromConfig(ApiIdentityPartConfig config)
    {
        var snapshot = config.Snapshot is not null
            ? CreateSnapshot(config.Snapshot)
            : null;

        var structure = config.Structure is not null && config.Structure.Count > 0
            ? config.Structure.ConvertAll(CreatePartFromConfig)
            : null;

        return new ApiIdentityPart(config.Name, snapshot, structure);
    }
    #endregion

    #region Test Data
    // Scalar snapshots
    protected static ApiIdentitySnapshot ScalarSnapshot { get; } = ApiIdentitySnapshot.Scalar(ApiId.FromInt32(42));

    // Composite snapshots with various configurations
    protected static ApiIdentitySnapshot CompositeSnapshotEmpty { get; } = ApiIdentitySnapshot.Composite();

    protected static ApiIdentitySnapshot CompositeSnapshotWithResolvedScalarPart { get; } = ApiIdentitySnapshot.Composite
    (
        new ApiIdentityPart
        (
            Name: "Id",
            Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromInt32(42))
        )
    );

    protected static ApiId CompositeSnapshotWithResolvedScalarPartToNamedCompositeApiId { get; } = ApiId.Composite(ApiIdPart.Create("Id", ApiId.FromInt32(42)));
    protected static ApiId CompositeSnapshotWithResolvedScalarPartToUnnamedCompositeApiId { get; } = ApiId.Composite(ApiIdPart.Create(ApiId.FromInt32(42)));

    protected static ApiIdentitySnapshot CompositeSnapshotWithUnresolvedScalarPart { get; } = ApiIdentitySnapshot.Composite
    (
        new ApiIdentityPart
        (
            Name: "Id",
            Snapshot: null
        )
    );

    protected static ApiId CompositeSnapshotWithUnresolvedScalarPartToNamedCompositeApiId { get; } = ApiId.Composite(ApiIdPart.Create("Id", ApiId.Empty));
    protected static ApiId CompositeSnapshotWithUnresolvedScalarPartToUnnamedCompositeApiId { get; } = ApiId.Composite(ApiIdPart.Create(ApiId.Empty));

    protected static ApiIdentitySnapshot CompositeSnapshotWithResolvedNestedIdentityParts { get; } = ApiIdentitySnapshot.Composite
    (
        new ApiIdentityPart
        (
            Name: "Customer",
            Snapshot: ApiIdentitySnapshot.Composite
            (
                new ApiIdentityPart
                (
                    Name: "Country",
                    Snapshot: ApiIdentitySnapshot.Composite
                    (
                        new ApiIdentityPart
                        (
                            Name: "Id",
                            Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromInt32(1))
                        )
                    )
                ),
                new ApiIdentityPart
                (
                    Name: "CustomerId",
                    Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromInt32(42))
                )
            )
        ),
        new ApiIdentityPart
        (
            Name: "OrderNumber",
            Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromInt32(1001))
        )
    );

    protected static ApiId CompositeSnapshotWithResolvedNestedIdentityPartsToNamedCompositeApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create("Customer.Country.Id", ApiId.FromInt32(1)),
        ApiIdPart.Create("Customer.CustomerId", ApiId.FromInt32(42)),
        ApiIdPart.Create("OrderNumber", ApiId.FromInt32(1001))
    );

    protected static ApiId CompositeSnapshotWithResolvedNestedIdentityPartsToUnnamedCompositeApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create(ApiId.FromInt32(1)),
        ApiIdPart.Create(ApiId.FromInt32(42)),
        ApiIdPart.Create(ApiId.FromInt32(1001))
    );

    protected static ApiIdentitySnapshot CompositeSnapshotWithUnresolvedNestedIdentityParts { get; } = ApiIdentitySnapshot.Composite
    (
        new ApiIdentityPart
        (
            Name: "Customer",
            Snapshot: ApiIdentitySnapshot.Composite
            (
                new ApiIdentityPart
                (
                    Name: "Country",
                    Snapshot: null,
                    Structure:
                    [
                        new ApiIdentityPart
                        (
                            Name: "Id",
                            Snapshot: null
                        )
                    ]
                ),
                new ApiIdentityPart
                (
                    Name: "CustomerId",
                    Snapshot: null
                )
            )
        ),
        new ApiIdentityPart
        (
            Name: "OrderNumber",
            Snapshot: null
        )
    );

    protected static ApiId CompositeSnapshotWithUnresolvedNestedIdentityPartsToNamedCompositeApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create("Customer.Country.Id", ApiId.Empty),
        ApiIdPart.Create("Customer.CustomerId", ApiId.Empty),
        ApiIdPart.Create("OrderNumber", ApiId.Empty)
    );

    protected static ApiId CompositeSnapshotWithUnresolvedNestedIdentityPartsToUnnamedCompositeApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create(ApiId.Empty),
        ApiIdPart.Create(ApiId.Empty),
        ApiIdPart.Create(ApiId.Empty)
    );

    protected static ApiIdentitySnapshot CompositeSnapshotWithPartialUnresolvedNestedIdentityParts { get; } = ApiIdentitySnapshot.Composite
    (
        new ApiIdentityPart
        (
            Name: "Customer",
            Snapshot: ApiIdentitySnapshot.Composite
            (
                new ApiIdentityPart
                (
                    Name: "Country",
                    Snapshot: null,
                    Structure:
                    [
                        new ApiIdentityPart
                        (
                            Name: "Id",
                            Snapshot: null
                        )
                    ]
                ),
                new ApiIdentityPart
                (
                    Name: "CustomerId",
                    Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromInt32(42))
                )
            )
        ),
        new ApiIdentityPart
        (
            Name: "OrderNumber",
            Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromInt32(1001))
        )
    );

    protected static ApiId CompositeSnapshotWithPartialUnresolvedNestedIdentityPartsToNamedCompositeApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create("Customer.Country.Id", ApiId.Empty),
        ApiIdPart.Create("Customer.CustomerId", ApiId.FromInt32(42)),
        ApiIdPart.Create("OrderNumber", ApiId.FromInt32(1001))
    );

    protected static ApiId CompositeSnapshotWithPartialUnresolvedNestedIdentityPartsToUnnamedCompositeApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create(ApiId.Empty),
        ApiIdPart.Create(ApiId.FromInt32(42)),
        ApiIdPart.Create(ApiId.FromInt32(1001))
    );

    protected static ApiIdentitySnapshot CompositeSnapshotWithResolvedDeeplyNestedIdentityParts { get; } = ApiIdentitySnapshot.Composite
    (
        new ApiIdentityPart
        (
            Name: "Level1",
            Snapshot: ApiIdentitySnapshot.Composite
            (
                new ApiIdentityPart
                (
                    Name: "Level2",
                    Snapshot: ApiIdentitySnapshot.Composite
                    (
                        new ApiIdentityPart
                        (
                            Name: "Level3",
                            Snapshot: ApiIdentitySnapshot.Composite
                            (
                                new ApiIdentityPart
                                (
                                    Name: "Level4",
                                    Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromString("DeepValue"))
                                ),
                                new ApiIdentityPart
                                (
                                    Name: "SequenceNumber",
                                    Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromInt32(42))
                                )
                            )
                        )
                    )
                )
            )
        ),
        new ApiIdentityPart
        (
            Name: "RootId",
            Snapshot: ApiIdentitySnapshot.Scalar(ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")))
        )
    );

    protected static ApiId CompositeSnapshotWithResolvedDeeplyNestedIdentityPartsToNamedCompositeApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create("Level1.Level2.Level3.Level4", ApiId.FromString("DeepValue")),
        ApiIdPart.Create("Level1.Level2.Level3.SequenceNumber", ApiId.FromInt32(42)),
        ApiIdPart.Create("RootId", ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")))
    );

    protected static ApiId CompositeSnapshotWithResolvedDeeplyNestedIdentityPartsToUnnamedCompositeApiId { get; } = ApiId.Composite
    (
        ApiIdPart.Create(ApiId.FromString("DeepValue")),
        ApiIdPart.Create(ApiId.FromInt32(42)),
        ApiIdPart.Create(ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc")))
    );
    #endregion
}
#endif
