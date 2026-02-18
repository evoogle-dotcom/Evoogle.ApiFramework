// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Identity;

public partial class ApiIdentitySnapshotTests
{
    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        new JsonDeserializeTest
        {
            Name = "Scalar snapshot with empty value",
            SourceJson = @"{""Kind"":""Scalar"",""ScalarValue"":null}",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Scalar,
                ScalarValue = ApiId.Empty
            }
        },

        new JsonDeserializeTest
        {
            Name = "Scalar snapshot with integer value",
            SourceJson = @"{""Kind"":""Scalar"",""ScalarValue"":{""Kind"":""Int32"",""Value"":42}}",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Scalar,
                ScalarValue = ApiId.FromInt32(42)
            }
        },

        new JsonDeserializeTest
        {
            Name = "Composite snapshot with empty parts",
            SourceJson = @"{""Kind"":""Composite"",""NestedParts"":[]}",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts = []
            }
        },

        new JsonDeserializeTest
        {
            Name = "Composite snapshot with scalar part",
            SourceJson = @"{""Kind"":""Composite"",""NestedParts"":[{""Name"":""Id"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Id"",""ScalarValue"":{""Kind"":""Int32"",""Value"":42}}}]}",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts =
                [
                    new ApiIdentityPartConfig
                    (
                        Name: "Id",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Scalar,
                            ScalarValue = ApiId.FromInt32(42)
                        },
                        Structure: null
                    )
                ]
            }
        },

        new JsonDeserializeTest
        {
            Name = "Composite snapshot with resolved nested identity parts",
            SourceJson = @"{""Kind"":""Composite"",""NestedParts"":[{""Name"":""Customer"",""Snapshot"":{""Kind"":""Composite"",""Path"":""Customer"",""NestedParts"":[{""Name"":""Country"",""Snapshot"":{""Kind"":""Composite"",""Path"":""Customer.Country"",""NestedParts"":[{""Name"":""Id"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Customer.Country.Id"",""ScalarValue"":{""Kind"":""Int32"",""Value"":1}}}]}},{""Name"":""CustomerId"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Customer.CustomerId"",""ScalarValue"":{""Kind"":""Int32"",""Value"":42}}}]}},{""Name"":""OrderNumber"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""OrderNumber"",""ScalarValue"":{""Kind"":""Int64"",""Value"":1001}}}]}",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts =
                [
                    new ApiIdentityPartConfig
                    (
                        Name: "Customer",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Composite,
                            NestedParts =
                            [
                                new ApiIdentityPartConfig
                                (
                                    Name: "Country",
                                    Snapshot: new ApiIdentitySnapshotConfig
                                    {
                                        Kind = ApiIdentitySnapshotKind.Composite,
                                        NestedParts =
                                        [
                                            new ApiIdentityPartConfig
                                            (
                                                Name: "Id",
                                                Snapshot: new ApiIdentitySnapshotConfig
                                                {
                                                    Kind = ApiIdentitySnapshotKind.Scalar,
                                                    ScalarValue = ApiId.FromInt32(1)
                                                },
                                                Structure: null
                                            )
                                        ]
                                    },
                                    Structure: null
                                ),
                                new ApiIdentityPartConfig
                                (
                                    Name: "CustomerId",
                                    Snapshot: new ApiIdentitySnapshotConfig
                                    {
                                        Kind = ApiIdentitySnapshotKind.Scalar,
                                        ScalarValue = ApiId.FromInt32(42)
                                    },
                                    Structure: null
                                )
                            ]
                        },
                        Structure: null
                    ),
                    new ApiIdentityPartConfig
                    (
                        Name: "OrderNumber",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Scalar,
                            ScalarValue = ApiId.FromInt64(1001)
                        },
                        Structure: null
                    )
                ]
            }
        },

        new JsonDeserializeTest
        {
            Name = "Composite snapshot with unresolved nested identity parts",
            SourceJson = @"{""Kind"":""Composite"",""NestedParts"":[{""Name"":""Customer"",""Structure"":[{""Name"":""Country"",""Structure"":[{""Name"":""Id""}]},{""Name"":""CustomerId""}]},{""Name"":""OrderNumber"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""OrderNumber"",""ScalarValue"":{""Kind"":""Int64"",""Value"":1001}}}]}",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts =
                [
                    new ApiIdentityPartConfig
                    (
                        Name: "Customer",
                        Snapshot: null,
                        Structure:
                        [
                            new ApiIdentityPartConfig
                            (
                                Name: "Country",
                                Snapshot: null,
                                Structure:
                                [
                                    new ApiIdentityPartConfig
                                    (
                                        Name: "Id",
                                        Snapshot: null,
                                        Structure: null
                                    )
                                ]
                            ),
                            new ApiIdentityPartConfig
                            (
                                Name: "CustomerId",
                                Snapshot: null,
                                Structure: null
                            )
                        ]
                    ),
                    new ApiIdentityPartConfig
                    (
                        Name: "OrderNumber",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Scalar,
                            ScalarValue = ApiId.FromInt64(1001)
                        },
                        Structure: null
                    )
                ]
            }
        },

        new JsonDeserializeTest
        {
            Name = "Composite snapshot with deeply nested parts",
            SourceJson = @"{""Kind"":""Composite"",""NestedParts"":[{""Name"":""Level1"",""Snapshot"":{""Kind"":""Composite"",""Path"":""Level1"",""NestedParts"":[{""Name"":""Level2"",""Snapshot"":{""Kind"":""Composite"",""Path"":""Level1.Level2"",""NestedParts"":[{""Name"":""Level3"",""Snapshot"":{""Kind"":""Composite"",""Path"":""Level1.Level2.Level3"",""NestedParts"":[{""Name"":""Level4"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Level1.Level2.Level3.Level4"",""ScalarValue"":{""Kind"":""String"",""Value"":""deep""}}},{""Name"":""SequenceNumber"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Level1.Level2.Level3.SequenceNumber"",""ScalarValue"":{""Kind"":""Int32"",""Value"":42}}}]}},{""Name"":""Id"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Level1.Level2.Id"",""ScalarValue"":{""Kind"":""Int32"",""Value"":100}}}]}},{""Name"":""Name"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Level1.Name"",""ScalarValue"":{""Kind"":""String"",""Value"":""test""}}}]}},{""Name"":""RootId"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""RootId"",""ScalarValue"":{""Kind"":""Guid"",""Value"":""12345678-1234-1234-1234-123456789abc""}}}]}",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts =
                [
                    new ApiIdentityPartConfig
                    (
                        Name: "Level1",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Composite,
                            NestedParts =
                            [
                                new ApiIdentityPartConfig
                                (
                                    Name: "Level2",
                                    Snapshot: new ApiIdentitySnapshotConfig
                                    {
                                        Kind = ApiIdentitySnapshotKind.Composite,
                                        NestedParts =
                                        [
                                            new ApiIdentityPartConfig
                                            (
                                                Name: "Level3",
                                                Snapshot: new ApiIdentitySnapshotConfig
                                                {
                                                    Kind = ApiIdentitySnapshotKind.Composite,
                                                    NestedParts =
                                                    [
                                                        new ApiIdentityPartConfig
                                                        (
                                                            Name: "Level4",
                                                            Snapshot: new ApiIdentitySnapshotConfig
                                                            {
                                                                Kind = ApiIdentitySnapshotKind.Scalar,
                                                                ScalarValue = ApiId.FromString("deep")
                                                            },
                                                            Structure: null
                                                        ),
                                                        new ApiIdentityPartConfig
                                                        (
                                                            Name: "SequenceNumber",
                                                            Snapshot: new ApiIdentitySnapshotConfig
                                                            {
                                                                Kind = ApiIdentitySnapshotKind.Scalar,
                                                                ScalarValue = ApiId.FromInt32(42)
                                                            },
                                                            Structure: null
                                                        )
                                                    ]
                                                },
                                                Structure: null
                                            ),
                                            new ApiIdentityPartConfig
                                            (
                                                Name: "Id",
                                                Snapshot: new ApiIdentitySnapshotConfig
                                                {
                                                    Kind = ApiIdentitySnapshotKind.Scalar,
                                                    ScalarValue = ApiId.FromInt32(100)
                                                },
                                                Structure: null
                                            )
                                        ]
                                    },
                                    Structure: null
                                ),
                                new ApiIdentityPartConfig
                                (
                                    Name: "Name",
                                    Snapshot: new ApiIdentitySnapshotConfig
                                    {
                                        Kind = ApiIdentitySnapshotKind.Scalar,
                                        ScalarValue = ApiId.FromString("test")
                                    },
                                    Structure: null
                                )
                            ]
                        },
                        Structure: null
                    ),
                    new ApiIdentityPartConfig
                    (
                        Name: "RootId",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Scalar,
                            ScalarValue = ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc"))
                        },
                        Structure: null
                    )
                ]
            },
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        new JsonRoundtripTest
        {
            Name = "Scalar snapshot with empty value",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Scalar,
                ScalarValue = ApiId.Empty
            }
        },

        new JsonRoundtripTest
        {
            Name = "Scalar snapshot with integer value",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Scalar,
                ScalarValue = ApiId.FromInt32(42)
            }
        },

        new JsonRoundtripTest
        {
            Name = "Composite snapshot with empty parts",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts = []
            }
        },

        new JsonRoundtripTest
        {
            Name = "Composite snapshot with scalar part",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts =
                [
                    new ApiIdentityPartConfig
                    (
                        Name: "Id",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Scalar,
                            ScalarValue = ApiId.FromInt32(42)
                        },
                        Structure: null
                    )
                ]
            },
        },

        new JsonRoundtripTest
        {
            Name = "Composite snapshot with resolved nested identity parts",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts =
                [
                    new ApiIdentityPartConfig
                    (
                        Name: "Customer",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Composite,
                            NestedParts =
                            [
                                new ApiIdentityPartConfig
                                (
                                    Name: "Country",
                                    Snapshot: new ApiIdentitySnapshotConfig
                                    {
                                        Kind = ApiIdentitySnapshotKind.Composite,
                                        NestedParts =
                                        [
                                            new ApiIdentityPartConfig
                                            (
                                                Name: "Id",
                                                Snapshot: new ApiIdentitySnapshotConfig
                                                {
                                                    Kind = ApiIdentitySnapshotKind.Scalar,
                                                    ScalarValue = ApiId.FromInt32(1)
                                                },
                                                Structure: null
                                            )
                                        ]
                                    },
                                    Structure: null
                                ),
                                new ApiIdentityPartConfig
                                (
                                    Name: "CustomerId",
                                    Snapshot: new ApiIdentitySnapshotConfig
                                    {
                                        Kind = ApiIdentitySnapshotKind.Scalar,
                                        ScalarValue = ApiId.FromInt32(42)
                                    },
                                    Structure: null
                                )
                            ]
                        },
                        Structure: null
                    ),
                    new ApiIdentityPartConfig
                    (
                        Name: "OrderNumber",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Scalar,
                            ScalarValue = ApiId.FromInt64(1001)
                        },
                        Structure: null
                    )
                ]
            },
        },

        new JsonRoundtripTest
        {
            Name = "Composite snapshot with unresolved nested identity parts",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts =
                [
                    new ApiIdentityPartConfig
                    (
                        Name: "Customer",
                        Snapshot: null,
                        Structure:
                        [
                            new ApiIdentityPartConfig
                            (
                                Name: "Country",
                                Snapshot: null,
                                Structure:
                                [
                                    new ApiIdentityPartConfig
                                    (
                                        Name: "Id",
                                        Snapshot: null,
                                        Structure: null
                                    )
                                ]
                            ),
                            new ApiIdentityPartConfig
                            (
                                Name: "CustomerId",
                                Snapshot: null,
                                Structure: null
                            )
                        ]
                    ),
                    new ApiIdentityPartConfig
                    (
                        Name: "OrderNumber",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Scalar,
                            ScalarValue = ApiId.FromInt64(1001)
                        },
                        Structure: null
                    )
                ]
            },
        },

        new JsonRoundtripTest
        {
            Name = "Composite snapshot with deeply nested parts",
            ExpectedFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts =
                [
                    new ApiIdentityPartConfig
                    (
                        Name: "Level1",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Composite,
                            NestedParts =
                            [
                                new ApiIdentityPartConfig
                                (
                                    Name: "Level2",
                                    Snapshot: new ApiIdentitySnapshotConfig
                                    {
                                        Kind = ApiIdentitySnapshotKind.Composite,
                                        NestedParts =
                                        [
                                            new ApiIdentityPartConfig
                                            (
                                                Name: "Level3",
                                                Snapshot: new ApiIdentitySnapshotConfig
                                                {
                                                    Kind = ApiIdentitySnapshotKind.Composite,
                                                    NestedParts =
                                                    [
                                                        new ApiIdentityPartConfig
                                                        (
                                                            Name: "Level4",
                                                            Snapshot: new ApiIdentitySnapshotConfig
                                                            {
                                                                Kind = ApiIdentitySnapshotKind.Scalar,
                                                                ScalarValue = ApiId.FromString("deep")
                                                            },
                                                            Structure: null
                                                        ),
                                                        new ApiIdentityPartConfig
                                                        (
                                                            Name: "SequenceNumber",
                                                            Snapshot: new ApiIdentitySnapshotConfig
                                                            {
                                                                Kind = ApiIdentitySnapshotKind.Scalar,
                                                                ScalarValue = ApiId.FromInt32(42)
                                                            },
                                                            Structure: null
                                                        )
                                                    ]
                                                },
                                                Structure: null
                                            ),
                                            new ApiIdentityPartConfig
                                            (
                                                Name: "Id",
                                                Snapshot: new ApiIdentitySnapshotConfig
                                                {
                                                    Kind = ApiIdentitySnapshotKind.Scalar,
                                                    ScalarValue = ApiId.FromInt32(100)
                                                },
                                                Structure: null
                                            )
                                        ]
                                    },
                                    Structure: null
                                ),
                                new ApiIdentityPartConfig
                                (
                                    Name: "Name",
                                    Snapshot: new ApiIdentitySnapshotConfig
                                    {
                                        Kind = ApiIdentitySnapshotKind.Scalar,
                                        ScalarValue = ApiId.FromString("test")
                                    },
                                    Structure: null
                                )
                            ]
                        },
                        Structure: null
                    ),
                    new ApiIdentityPartConfig
                    (
                        Name: "RootId",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Scalar,
                            ScalarValue = ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc"))
                        },
                        Structure: null
                    )
                ]
            },
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        new JsonSerializeTest
        {
            Name = "Scalar snapshot with empty value",
            SourceFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Scalar,
                ScalarValue = ApiId.Empty
            },
            ExpectedJson = @"{""Kind"":""Scalar"",""ScalarValue"":null}"
        },

        new JsonSerializeTest
        {
            Name = "Scalar snapshot with integer value",
            SourceFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Scalar,
                ScalarValue = ApiId.FromInt32(42)
            },
            ExpectedJson = @"{""Kind"":""Scalar"",""ScalarValue"":{""Kind"":""Int32"",""Value"":42}}"
        },

        new JsonSerializeTest
        {
            Name = "Composite snapshot with empty parts",
            SourceFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts = []
            },
            ExpectedJson = @"{""Kind"":""Composite"",""NestedParts"":[]}"
        },

        new JsonSerializeTest
        {
            Name = "Composite snapshot with scalar part",
            SourceFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts =
                [
                    new ApiIdentityPartConfig
                    (
                        Name: "Id",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Scalar,
                            ScalarValue = ApiId.FromInt32(42)
                        },
                        Structure: null
                    )
                ]
            },
            ExpectedJson = @"{""Kind"":""Composite"",""NestedParts"":[{""Name"":""Id"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Id"",""ScalarValue"":{""Kind"":""Int32"",""Value"":42}}}]}"
        },

        new JsonSerializeTest
        {
            Name = "Composite snapshot with resolved nested identity parts",
            SourceFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts =
                [
                    new ApiIdentityPartConfig
                    (
                        Name: "Customer",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Composite,
                            NestedParts =
                            [
                                new ApiIdentityPartConfig
                                (
                                    Name: "Country",
                                    Snapshot: new ApiIdentitySnapshotConfig
                                    {
                                        Kind = ApiIdentitySnapshotKind.Composite,
                                        NestedParts =
                                        [
                                            new ApiIdentityPartConfig
                                            (
                                                Name: "Id",
                                                Snapshot: new ApiIdentitySnapshotConfig
                                                {
                                                    Kind = ApiIdentitySnapshotKind.Scalar,
                                                    ScalarValue = ApiId.FromInt32(1)
                                                },
                                                Structure: null
                                            )
                                        ]
                                    },
                                    Structure: null
                                ),
                                new ApiIdentityPartConfig
                                (
                                    Name: "CustomerId",
                                    Snapshot: new ApiIdentitySnapshotConfig
                                    {
                                        Kind = ApiIdentitySnapshotKind.Scalar,
                                        ScalarValue = ApiId.FromInt32(42)
                                    },
                                    Structure: null
                                )
                            ]
                        },
                        Structure: null
                    ),
                    new ApiIdentityPartConfig
                    (
                        Name: "OrderNumber",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Scalar,
                            ScalarValue = ApiId.FromInt64(1001)
                        },
                        Structure: null
                    )
                ]
            },
            ExpectedJson = @"{""Kind"":""Composite"",""NestedParts"":[{""Name"":""Customer"",""Snapshot"":{""Kind"":""Composite"",""Path"":""Customer"",""NestedParts"":[{""Name"":""Country"",""Snapshot"":{""Kind"":""Composite"",""Path"":""Customer.Country"",""NestedParts"":[{""Name"":""Id"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Customer.Country.Id"",""ScalarValue"":{""Kind"":""Int32"",""Value"":1}}}]}},{""Name"":""CustomerId"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Customer.CustomerId"",""ScalarValue"":{""Kind"":""Int32"",""Value"":42}}}]}},{""Name"":""OrderNumber"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""OrderNumber"",""ScalarValue"":{""Kind"":""Int64"",""Value"":1001}}}]}"
        },

        new JsonSerializeTest
        {
            Name = "Composite snapshot with unresolved nested identity parts",
            SourceFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts =
                [
                    new ApiIdentityPartConfig
                    (
                        Name: "Customer",
                        Snapshot: null,
                        Structure:
                        [
                            new ApiIdentityPartConfig
                            (
                                Name: "Country",
                                Snapshot: null,
                                Structure:
                                [
                                    new ApiIdentityPartConfig
                                    (
                                        Name: "Id",
                                        Snapshot: null,
                                        Structure: null
                                    )
                                ]
                            ),
                            new ApiIdentityPartConfig
                            (
                                Name: "CustomerId",
                                Snapshot: null,
                                Structure: null
                            )
                        ]
                    ),
                    new ApiIdentityPartConfig
                    (
                        Name: "OrderNumber",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Scalar,
                            ScalarValue = ApiId.FromInt64(1001)
                        },
                        Structure: null
                    )
                ]
            },
            ExpectedJson = @"{""Kind"":""Composite"",""NestedParts"":[{""Name"":""Customer"",""Structure"":[{""Name"":""Country"",""Structure"":[{""Name"":""Id""}]},{""Name"":""CustomerId""}]},{""Name"":""OrderNumber"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""OrderNumber"",""ScalarValue"":{""Kind"":""Int64"",""Value"":1001}}}]}"
        },

        new JsonSerializeTest
        {
            Name = "Composite snapshot with deeply nested parts",
            SourceFactoryArgument = new ApiIdentitySnapshotConfig
            {
                Kind = ApiIdentitySnapshotKind.Composite,
                NestedParts =
                [
                    new ApiIdentityPartConfig
                    (
                        Name: "Level1",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Composite,
                            NestedParts =
                            [
                                new ApiIdentityPartConfig
                                (
                                    Name: "Level2",
                                    Snapshot: new ApiIdentitySnapshotConfig
                                    {
                                        Kind = ApiIdentitySnapshotKind.Composite,
                                        NestedParts =
                                        [
                                            new ApiIdentityPartConfig
                                            (
                                                Name: "Level3",
                                                Snapshot: new ApiIdentitySnapshotConfig
                                                {
                                                    Kind = ApiIdentitySnapshotKind.Composite,
                                                    NestedParts =
                                                    [
                                                        new ApiIdentityPartConfig
                                                        (
                                                            Name: "Level4",
                                                            Snapshot: new ApiIdentitySnapshotConfig
                                                            {
                                                                Kind = ApiIdentitySnapshotKind.Scalar,
                                                                ScalarValue = ApiId.FromString("deep")
                                                            },
                                                            Structure: null
                                                        ),
                                                        new ApiIdentityPartConfig
                                                        (
                                                            Name: "SequenceNumber",
                                                            Snapshot: new ApiIdentitySnapshotConfig
                                                            {
                                                                Kind = ApiIdentitySnapshotKind.Scalar,
                                                                ScalarValue = ApiId.FromInt32(42)
                                                            },
                                                            Structure: null
                                                        )
                                                    ]
                                                },
                                                Structure: null
                                            ),
                                            new ApiIdentityPartConfig
                                            (
                                                Name: "Id",
                                                Snapshot: new ApiIdentitySnapshotConfig
                                                {
                                                    Kind = ApiIdentitySnapshotKind.Scalar,
                                                    ScalarValue = ApiId.FromInt32(100)
                                                },
                                                Structure: null
                                            )
                                        ]
                                    },
                                    Structure: null
                                ),
                                new ApiIdentityPartConfig
                                (
                                    Name: "Name",
                                    Snapshot: new ApiIdentitySnapshotConfig
                                    {
                                        Kind = ApiIdentitySnapshotKind.Scalar,
                                        ScalarValue = ApiId.FromString("test")
                                    },
                                    Structure: null
                                )
                            ]
                        },
                        Structure: null
                    ),
                    new ApiIdentityPartConfig
                    (
                        Name: "RootId",
                        Snapshot: new ApiIdentitySnapshotConfig
                        {
                            Kind = ApiIdentitySnapshotKind.Scalar,
                            ScalarValue = ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc"))
                        },
                        Structure: null
                    )
                ]
            },
            ExpectedJson = @"{""Kind"":""Composite"",""NestedParts"":[{""Name"":""Level1"",""Snapshot"":{""Kind"":""Composite"",""Path"":""Level1"",""NestedParts"":[{""Name"":""Level2"",""Snapshot"":{""Kind"":""Composite"",""Path"":""Level1.Level2"",""NestedParts"":[{""Name"":""Level3"",""Snapshot"":{""Kind"":""Composite"",""Path"":""Level1.Level2.Level3"",""NestedParts"":[{""Name"":""Level4"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Level1.Level2.Level3.Level4"",""ScalarValue"":{""Kind"":""String"",""Value"":""deep""}}},{""Name"":""SequenceNumber"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Level1.Level2.Level3.SequenceNumber"",""ScalarValue"":{""Kind"":""Int32"",""Value"":42}}}]}},{""Name"":""Id"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Level1.Level2.Id"",""ScalarValue"":{""Kind"":""Int32"",""Value"":100}}}]}},{""Name"":""Name"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""Level1.Name"",""ScalarValue"":{""Kind"":""String"",""Value"":""test""}}}]}},{""Name"":""RootId"",""Snapshot"":{""Kind"":""Scalar"",""Path"":""RootId"",""ScalarValue"":{""Kind"":""Guid"",""Value"":""12345678-1234-1234-1234-123456789abc""}}}]}"
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
