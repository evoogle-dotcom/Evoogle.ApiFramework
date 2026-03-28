// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Identity;

public partial class ApiIdentityValueTests
{
    #region Test Types
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        new JsonDeserializeTest
        {
            Name = $"Scalar identity with one scalar part (empty)",
            SourceJson = @"
            {
                ""ApiParts"": [
                    {
                        ""ApiName"": ""Id"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": null
                    }
                ]
            }",
            ExpectedFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Id",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.Empty,
                    },
                ],
            },
        },

        new JsonDeserializeTest
        {
            Name = $"Scalar identity with one scalar part (integer)",
            SourceJson = @"
            {
                ""ApiParts"": [
                    {
                        ""ApiName"": ""Id"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": {
                            ""ApiKind"": ""Int32"",
                            ""ClrValue"": 42
                        }
                    }
                ]
            }",
            ExpectedFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Id",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromInt32(42),
                    },
                ],
            },
        },

        new JsonDeserializeTest
        {
            Name = $"Composite identity with two scalar parts (integer, integer)",
            SourceJson = @"
            {
                ""ApiParts"": [
                    {
                        ""ApiName"": ""Id"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": {
                            ""ApiKind"": ""Int32"",
                            ""ClrValue"": 42
                        }
                    },
                    {
                        ""ApiName"": ""OrderNumber"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": {
                            ""ApiKind"": ""Int32"",
                            ""ClrValue"": 1001
                        }
                    }
                ]
            }",
            ExpectedFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Id",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromInt32(42),
                    },
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "OrderNumber",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromInt32(1001),
                    },
                ],
            },
        },

        new JsonDeserializeTest
        {
            Name = $"Composite identity with object and scalar parts",
            SourceJson = @"
            {
                ""ApiParts"": [
                    {
                        ""ApiName"": ""Customer"",
                        ""ApiKind"": ""Object"",
                        ""ApiObjectValue"": {
                            ""ApiParts"": [
                                {
                                    ""ApiName"": ""Country"",
                                    ""ApiKind"": ""Object"",
                                    ""ApiObjectValue"": {
                                        ""ApiParts"": [
                                            {
                                                ""ApiName"": ""Id"",
                                                ""ApiKind"": ""Scalar"",
                                                ""ApiScalarValue"": {
                                                    ""ApiKind"": ""Int32"",
                                                    ""ClrValue"": 1
                                                }
                                            }
                                        ]
                                    }
                                },
                                {
                                    ""ApiName"": ""CustomerId"",
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiScalarValue"": {
                                        ""ApiKind"": ""Int32"",
                                        ""ClrValue"": 42
                                    }
                                }
                            ]
                        }
                    },
                    {
                        ""ApiName"": ""OrderNumber"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": {
                            ""ApiKind"": ""Int32"",
                            ""ClrValue"": 1001
                        }
                    }
                ]
            }",
            ExpectedFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Customer",
                        ApiKind = ApiIdentityPartValueKind.Object,
                        ApiObjectValue = new ApiIdentityValueConfig
                        {
                            ApiParts =
                            [
                                new ApiIdentityPartValueConfig
                                {
                                    ApiName = "Country",
                                    ApiKind = ApiIdentityPartValueKind.Object,
                                    ApiObjectValue = new ApiIdentityValueConfig
                                    {
                                        ApiParts =
                                        [
                                            new ApiIdentityPartValueConfig
                                            {
                                                ApiName = "Id",
                                                ApiKind = ApiIdentityPartValueKind.Scalar,
                                                ApiScalarValue = ApiId.FromInt32(1),
                                            }
                                        ]
                                    }
                                },
                                new ApiIdentityPartValueConfig
                                {
                                    ApiName = "CustomerId",
                                    ApiKind = ApiIdentityPartValueKind.Scalar,
                                    ApiScalarValue = ApiId.FromInt32(42),
                                }
                            ]
                        }
                    },
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "OrderNumber",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromInt32(1001),
                    },
                ],
            },
        },

        new JsonDeserializeTest
        {
            Name = $"Composite identity with unresolved object and scalar parts",
            SourceJson = @"
            {
                ""ApiParts"": [
                    {
                        ""ApiName"": ""Customer"",
                        ""ApiKind"": ""Object"",
                        ""ApiStructure"": [
                            {
                                ""ApiName"": ""Country"",
                                ""ApiKind"": ""Object"",
                                ""ApiStructure"": [
                                    {
                                        ""ApiName"": ""Id"",
                                        ""ApiKind"": ""Scalar"",
                                        ""ApiScalarValue"": null
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""CustomerId"",
                                ""ApiKind"": ""Scalar"",
                                ""ApiScalarValue"": null
                            }
                        ]
                    },
                    {
                        ""ApiName"": ""OrderNumber"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": null
                    }
                ]
            }",
            ExpectedFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Customer",
                        ApiKind = ApiIdentityPartValueKind.Object,
                        ApiStructure =
                        [
                            new ApiIdentityPartValueConfig
                            {
                                ApiName = "Country",
                                ApiKind = ApiIdentityPartValueKind.Object,
                                ApiStructure =
                                [
                                    new ApiIdentityPartValueConfig
                                    {
                                        ApiName = "Id",
                                        ApiKind = ApiIdentityPartValueKind.Scalar,
                                        ApiScalarValue = ApiId.Empty,
                                    }
                                ]
                            },
                            new ApiIdentityPartValueConfig
                            {
                                ApiName = "CustomerId",
                                ApiKind = ApiIdentityPartValueKind.Scalar,
                                ApiScalarValue = ApiId.Empty,
                            }
                        ]
                    },
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "OrderNumber",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.Empty,
                    },
                ],
            },
        },

        new JsonDeserializeTest
        {
            Name = $"Composite identity with deep object and scalar parts",
            SourceJson = @"
            {
                ""ApiParts"": [
                    {
                        ""ApiName"": ""Level1"",
                        ""ApiKind"": ""Object"",
                        ""ApiObjectValue"": {
                            ""ApiParts"": [
                                {
                                    ""ApiName"": ""Level2"",
                                    ""ApiKind"": ""Object"",
                                    ""ApiObjectValue"": {
                                        ""ApiParts"": [
                                            {
                                                ""ApiName"": ""Level3"",
                                                ""ApiKind"": ""Object"",
                                                ""ApiObjectValue"": {
                                                    ""ApiParts"": [
                                                        {
                                                            ""ApiName"": ""Level4"",
                                                            ""ApiKind"": ""Scalar"",
                                                            ""ApiScalarValue"": {
                                                                ""ApiKind"": ""String"",
                                                                ""ClrValue"": ""DeepValue""
                                                            }
                                                        },
                                                        {
                                                            ""ApiName"": ""SequenceNumber"",
                                                            ""ApiKind"": ""Scalar"",
                                                            ""ApiScalarValue"": {
                                                                ""ApiKind"": ""Int32"",
                                                                ""ClrValue"": 42
                                                            }
                                                        }
                                                    ]
                                                }
                                            }
                                        ]
                                    }
                                }
                            ]
                        }
                    },
                    {
                        ""ApiName"": ""RootId"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": {
                            ""ApiKind"": ""Guid"",
                            ""ClrValue"": ""12345678-1234-1234-1234-123456789abc""
                        }
                    }
                ]
            }",
            ExpectedFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Level1",
                        ApiKind = ApiIdentityPartValueKind.Object,
                        ApiObjectValue = new ApiIdentityValueConfig
                        {
                            ApiParts =
                            [
                                new ApiIdentityPartValueConfig
                                {
                                    ApiName = "Level2",
                                    ApiKind = ApiIdentityPartValueKind.Object,
                                    ApiObjectValue = new ApiIdentityValueConfig
                                    {
                                        ApiParts =
                                        [
                                            new ApiIdentityPartValueConfig
                                            {
                                                ApiName = "Level3",
                                                ApiKind = ApiIdentityPartValueKind.Object,
                                                ApiObjectValue = new ApiIdentityValueConfig
                                                {
                                                    ApiParts =
                                                    [
                                                        new ApiIdentityPartValueConfig
                                                        {
                                                            ApiName = "Level4",
                                                            ApiKind = ApiIdentityPartValueKind.Scalar,
                                                            ApiScalarValue = ApiId.FromString("DeepValue"),
                                                        },
                                                        new ApiIdentityPartValueConfig
                                                        {
                                                            ApiName = "SequenceNumber",
                                                            ApiKind = ApiIdentityPartValueKind.Scalar,
                                                            ApiScalarValue = ApiId.FromInt32(42),
                                                        },
                                                    ]
                                                }
                                            }
                                        ]
                                    }
                                }
                            ]
                        }
                    },
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "RootId",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc"))
                    },
                ],
            },
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        new JsonRoundtripTest
        {
            Name = $"Scalar identity with one scalar part (empty)",
            ExpectedFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Id",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.Empty,
                    },
                ],
            },
        },

        new JsonRoundtripTest
        {
            Name = $"Scalar identity with one scalar part (integer)",
            ExpectedFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Id",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromInt32(42),
                    },
                ],
            },
        },

        new JsonRoundtripTest
        {
            Name = $"Composite identity with two scalar parts (integer, integer)",
            ExpectedFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Id",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromInt32(42),
                    },
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "OrderNumber",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromInt32(1001),
                    },
                ],
            },
        },

        new JsonRoundtripTest
        {
            Name = $"Composite identity with object and scalar parts",
            ExpectedFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Customer",
                        ApiKind = ApiIdentityPartValueKind.Object,
                        ApiObjectValue = new ApiIdentityValueConfig
                        {
                            ApiParts =
                            [
                                new ApiIdentityPartValueConfig
                                {
                                    ApiName = "Country",
                                    ApiKind = ApiIdentityPartValueKind.Object,
                                    ApiObjectValue = new ApiIdentityValueConfig
                                    {
                                        ApiParts =
                                        [
                                            new ApiIdentityPartValueConfig
                                            {
                                                ApiName = "Id",
                                                ApiKind = ApiIdentityPartValueKind.Scalar,
                                                ApiScalarValue = ApiId.FromInt32(1),
                                            }
                                        ]
                                    }
                                },
                                new ApiIdentityPartValueConfig
                                {
                                    ApiName = "CustomerId",
                                    ApiKind = ApiIdentityPartValueKind.Scalar,
                                    ApiScalarValue = ApiId.FromInt32(42),
                                }
                            ]
                        }
                    },
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "OrderNumber",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromInt32(1001),
                    },
                ],
            },
        },

        new JsonRoundtripTest
        {
            Name = $"Composite identity with unresolved object and scalar parts",
            ExpectedFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Customer",
                        ApiKind = ApiIdentityPartValueKind.Object,
                        ApiStructure =
                        [
                            new ApiIdentityPartValueConfig
                            {
                                ApiName = "Country",
                                ApiKind = ApiIdentityPartValueKind.Object,
                                ApiStructure =
                                [
                                    new ApiIdentityPartValueConfig
                                    {
                                        ApiName = "Id",
                                        ApiKind = ApiIdentityPartValueKind.Scalar,
                                        ApiScalarValue = ApiId.Empty,
                                    }
                                ]
                            },
                            new ApiIdentityPartValueConfig
                            {
                                ApiName = "CustomerId",
                                ApiKind = ApiIdentityPartValueKind.Scalar,
                                ApiScalarValue = ApiId.Empty,
                            }
                        ]
                    },
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "OrderNumber",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.Empty,
                    },
                ],
            },
        },

        new JsonRoundtripTest
        {
            Name = $"Composite identity with deep object and scalar parts",
            ExpectedFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Level1",
                        ApiKind = ApiIdentityPartValueKind.Object,
                        ApiObjectValue = new ApiIdentityValueConfig
                        {
                            ApiParts =
                            [
                                new ApiIdentityPartValueConfig
                                {
                                    ApiName = "Level2",
                                    ApiKind = ApiIdentityPartValueKind.Object,
                                    ApiObjectValue = new ApiIdentityValueConfig
                                    {
                                        ApiParts =
                                        [
                                            new ApiIdentityPartValueConfig
                                            {
                                                ApiName = "Level3",
                                                ApiKind = ApiIdentityPartValueKind.Object,
                                                ApiObjectValue = new ApiIdentityValueConfig
                                                {
                                                    ApiParts =
                                                    [
                                                        new ApiIdentityPartValueConfig
                                                        {
                                                            ApiName = "Level4",
                                                            ApiKind = ApiIdentityPartValueKind.Scalar,
                                                            ApiScalarValue = ApiId.FromString("DeepValue"),
                                                        },
                                                        new ApiIdentityPartValueConfig
                                                        {
                                                            ApiName = "SequenceNumber",
                                                            ApiKind = ApiIdentityPartValueKind.Scalar,
                                                            ApiScalarValue = ApiId.FromInt32(42),
                                                        },
                                                    ]
                                                }
                                            }
                                        ]
                                    }
                                }
                            ]
                        }
                    },
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "RootId",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc"))
                    },
                ],
            },
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        new JsonSerializeTest
        {
            Name = $"Scalar identity with one scalar part (empty)",
            SourceFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Id",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.Empty,
                    },
                ],
            },
            ExpectedJson = @"
            {
                ""ApiParts"": [
                    {
                        ""ApiName"": ""Id"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": null
                    }
                ]
            }",
        },

        new JsonSerializeTest
        {
            Name = $"Scalar identity with one scalar part (integer)",
            SourceFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Id",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromInt32(42),
                    },
                ],
            },
            ExpectedJson = @"
            {
                ""ApiParts"": [
                    {
                        ""ApiName"": ""Id"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": {
                            ""ApiKind"": ""Int32"",
                            ""ClrValue"": 42
                        }
                    }
                ]
            }",
        },

        new JsonSerializeTest
        {
            Name = $"Composite identity with two scalar parts (integer, integer)",
            SourceFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Id",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromInt32(42),
                    },
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "OrderNumber",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromInt32(1001),
                    },
                ],
            },
            ExpectedJson = @"
            {
                ""ApiParts"": [
                    {
                        ""ApiName"": ""Id"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": {
                            ""ApiKind"": ""Int32"",
                            ""ClrValue"": 42
                        }
                    },
                    {
                        ""ApiName"": ""OrderNumber"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": {
                            ""ApiKind"": ""Int32"",
                            ""ClrValue"": 1001
                        }
                    }
                ]
            }",
        },

        new JsonSerializeTest
        {
            Name = $"Composite identity with object and scalar parts",
            SourceFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Customer",
                        ApiKind = ApiIdentityPartValueKind.Object,
                        ApiObjectValue = new ApiIdentityValueConfig
                        {
                            ApiParts =
                            [
                                new ApiIdentityPartValueConfig
                                {
                                    ApiName = "Country",
                                    ApiKind = ApiIdentityPartValueKind.Object,
                                    ApiObjectValue = new ApiIdentityValueConfig
                                    {
                                        ApiParts =
                                        [
                                            new ApiIdentityPartValueConfig
                                            {
                                                ApiName = "Id",
                                                ApiKind = ApiIdentityPartValueKind.Scalar,
                                                ApiScalarValue = ApiId.FromInt32(1),
                                            }
                                        ]
                                    }
                                },
                                new ApiIdentityPartValueConfig
                                {
                                    ApiName = "CustomerId",
                                    ApiKind = ApiIdentityPartValueKind.Scalar,
                                    ApiScalarValue = ApiId.FromInt32(42),
                                }
                            ]
                        }
                    },
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "OrderNumber",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromInt32(1001),
                    },
                ],
            },
            ExpectedJson = @"
            {
                ""ApiParts"": [
                    {
                        ""ApiName"": ""Customer"",
                        ""ApiKind"": ""Object"",
                        ""ApiObjectValue"": {
                            ""ApiParts"": [
                                {
                                    ""ApiName"": ""Country"",
                                    ""ApiKind"": ""Object"",
                                    ""ApiObjectValue"": {
                                        ""ApiParts"": [
                                            {
                                                ""ApiName"": ""Id"",
                                                ""ApiKind"": ""Scalar"",
                                                ""ApiScalarValue"": {
                                                    ""ApiKind"": ""Int32"",
                                                    ""ClrValue"": 1
                                                }
                                            }
                                        ]
                                    }
                                },
                                {
                                    ""ApiName"": ""CustomerId"",
                                    ""ApiKind"": ""Scalar"",
                                    ""ApiScalarValue"": {
                                        ""ApiKind"": ""Int32"",
                                        ""ClrValue"": 42
                                    }
                                }
                            ]
                        }
                    },
                    {
                        ""ApiName"": ""OrderNumber"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": {
                            ""ApiKind"": ""Int32"",
                            ""ClrValue"": 1001
                        }
                    }
                ]
            }",
        },

        new JsonSerializeTest
        {
            Name = $"Composite identity with unresolved object and scalar parts",
            SourceFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Customer",
                        ApiKind = ApiIdentityPartValueKind.Object,
                        ApiStructure =
                        [
                            new ApiIdentityPartValueConfig
                            {
                                ApiName = "Country",
                                ApiKind = ApiIdentityPartValueKind.Object,
                                ApiStructure =
                                [
                                    new ApiIdentityPartValueConfig
                                    {
                                        ApiName = "Id",
                                        ApiKind = ApiIdentityPartValueKind.Scalar,
                                        ApiScalarValue = ApiId.Empty,
                                    }
                                ]
                            },
                            new ApiIdentityPartValueConfig
                            {
                                ApiName = "CustomerId",
                                ApiKind = ApiIdentityPartValueKind.Scalar,
                                ApiScalarValue = ApiId.Empty,
                            }
                        ]
                    },
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "OrderNumber",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.Empty,
                    },
                ],
            },
            ExpectedJson = @"
            {
                ""ApiParts"": [
                    {
                        ""ApiName"": ""Customer"",
                        ""ApiKind"": ""Object"",
                        ""ApiStructure"": [
                            {
                                ""ApiName"": ""Country"",
                                ""ApiKind"": ""Object"",
                                ""ApiStructure"": [
                                    {
                                        ""ApiName"": ""Id"",
                                        ""ApiKind"": ""Scalar"",
                                        ""ApiScalarValue"": null
                                    }
                                ]
                            },
                            {
                                ""ApiName"": ""CustomerId"",
                                ""ApiKind"": ""Scalar"",
                                ""ApiScalarValue"": null
                            }
                        ]
                    },
                    {
                        ""ApiName"": ""OrderNumber"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": null
                    }
                ]
            }",
        },

        new JsonSerializeTest
        {
            Name = $"Composite identity with deep object and scalar parts",
            SourceFactoryArgument = new ApiIdentityValueConfig
            {
                ApiParts =
                [
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "Level1",
                        ApiKind = ApiIdentityPartValueKind.Object,
                        ApiObjectValue = new ApiIdentityValueConfig
                        {
                            ApiParts =
                            [
                                new ApiIdentityPartValueConfig
                                {
                                    ApiName = "Level2",
                                    ApiKind = ApiIdentityPartValueKind.Object,
                                    ApiObjectValue = new ApiIdentityValueConfig
                                    {
                                        ApiParts =
                                        [
                                            new ApiIdentityPartValueConfig
                                            {
                                                ApiName = "Level3",
                                                ApiKind = ApiIdentityPartValueKind.Object,
                                                ApiObjectValue = new ApiIdentityValueConfig
                                                {
                                                    ApiParts =
                                                    [
                                                        new ApiIdentityPartValueConfig
                                                        {
                                                            ApiName = "Level4",
                                                            ApiKind = ApiIdentityPartValueKind.Scalar,
                                                            ApiScalarValue = ApiId.FromString("DeepValue"),
                                                        },
                                                        new ApiIdentityPartValueConfig
                                                        {
                                                            ApiName = "SequenceNumber",
                                                            ApiKind = ApiIdentityPartValueKind.Scalar,
                                                            ApiScalarValue = ApiId.FromInt32(42),
                                                        },
                                                    ]
                                                }
                                            }
                                        ]
                                    }
                                }
                            ]
                        }
                    },
                    new ApiIdentityPartValueConfig
                    {
                        ApiName = "RootId",
                        ApiKind = ApiIdentityPartValueKind.Scalar,
                        ApiScalarValue = ApiId.FromGuid(Guid.Parse("12345678-1234-1234-1234-123456789abc"))
                    },
                ],
            },
            ExpectedJson = @"
            {
                ""ApiParts"": [
                    {
                        ""ApiName"": ""Level1"",
                        ""ApiKind"": ""Object"",
                        ""ApiObjectValue"": {
                            ""ApiParts"": [
                                {
                                    ""ApiName"": ""Level2"",
                                    ""ApiKind"": ""Object"",
                                    ""ApiObjectValue"": {
                                        ""ApiParts"": [
                                            {
                                                ""ApiName"": ""Level3"",
                                                ""ApiKind"": ""Object"",
                                                ""ApiObjectValue"": {
                                                    ""ApiParts"": [
                                                        {
                                                            ""ApiName"": ""Level4"",
                                                            ""ApiKind"": ""Scalar"",
                                                            ""ApiScalarValue"": {
                                                                ""ApiKind"": ""String"",
                                                                ""ClrValue"": ""DeepValue""
                                                            }
                                                        },
                                                        {
                                                            ""ApiName"": ""SequenceNumber"",
                                                            ""ApiKind"": ""Scalar"",
                                                            ""ApiScalarValue"": {
                                                                ""ApiKind"": ""Int32"",
                                                                ""ClrValue"": 42
                                                            }
                                                        }
                                                    ]
                                                }
                                            }
                                        ]
                                    }
                                }
                            ]
                        }
                    },
                    {
                        ""ApiName"": ""RootId"",
                        ""ApiKind"": ""Scalar"",
                        ""ApiScalarValue"": {
                            ""ApiKind"": ""Guid"",
                            ""ClrValue"": ""12345678-1234-1234-1234-123456789abc""
                        }
                    }
                ]
            }",
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
