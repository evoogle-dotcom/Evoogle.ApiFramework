// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Extension;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public class ApiTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    public class JsonDeserializeTest : XUnitTest
    {
        #region User Supplied Properties
        public string? SourceJson { get; set; }
        public ApiType? ExpectedApiType { get; set; }
        public bool? AddTestExtension1 { get; set; } = false;
        public bool? AddTestExtension2 { get; set; } = false;
        #endregion

        #region Calculated Properties
        private ApiType? ActualApiType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            if (this?.AddTestExtension1 == true)
            {
                // Attach test extension 1 to the ApiType if requested.
                this.ExpectedApiType?.AttachExtension(new TestExtension1());
            }

            if (this?.AddTestExtension2 == true)
            {
                // Attach test extension 2 to the ApiType if requested.
                this.ExpectedApiType?.AttachExtension(new TestExtension2());
            }

            this.WriteLine($"Source      JSON: {this.SourceJson.SafeToString().RemoveWhitespace()}");
            this.WriteLine($"Expected ApiType: {this.ExpectedApiType.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualApiType = JsonSerializer.Deserialize<ApiType>(this.SourceJson!);
            this.WriteLine($"Actual   ApiType: {this.ActualApiType.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualApiType.Should().BeEquivalentTo(ExpectedApiType);
        }
        #endregion
    }

    public class JsonRoundtripTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiType? ExpectedApiType { get; set; }
        public bool? AddTestExtension1 { get; set; } = false;
        public bool? AddTestExtension2 { get; set; } = false;
        #endregion

        #region Calculated Properties
        private ApiType? ActualApiType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            if (this?.AddTestExtension1 == true)
            {
                // Attach test extension 1 to the ApiType if requested.
                this.ExpectedApiType?.AttachExtension(new TestExtension1());
            }

            if (this?.AddTestExtension2 == true)
            {
                // Attach test extension 2 to the ApiType if requested.
                this.ExpectedApiType?.AttachExtension(new TestExtension2());
            }

            this.WriteLine($"Expected ApiType: {this.ExpectedApiType.SafeToString()}");
        }

        protected override void Act()
        {
            var json = JsonSerializer.Serialize(this.ExpectedApiType);
            this.ActualApiType = JsonSerializer.Deserialize<ApiType>(json);
            this.WriteLine($"Actual   ApiType: {this.ActualApiType.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualApiType.Should().BeEquivalentTo(ExpectedApiType);
        }
        #endregion
    }

    public class JsonSerializeTest : XUnitTest
    {
        #region User Supplied Properties
        public ApiType? SourceApiType { get; set; }
        public string? ExpectedJson { get; set; }
        public bool? AddTestExtension1 { get; set; } = false;
        public bool? AddTestExtension2 { get; set; } = false;
        #endregion

        #region Calculated Properties
        private string? ActualJson { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            if (this?.AddTestExtension1 == true)
            {
                // Attach test extension 1 to the ApiType if requested.
                this.SourceApiType?.AttachExtension(new TestExtension1());
            }

            if (this?.AddTestExtension2 == true)
            {
                // Attach test extension 2 to the ApiType if requested.
                this.SourceApiType?.AttachExtension(new TestExtension2());
            }

            this.WriteLine($"Source ApiType: {this.SourceApiType.SafeToString()}");
            this.WriteLine($"Expected  JSON: {this.ExpectedJson.SafeToString().RemoveWhitespace()}");
        }

        protected override void Act()
        {
            this.ActualJson = JsonSerializer.Serialize(this.SourceApiType);
            this.WriteLine($"Actual    JSON: {this.ActualJson.SafeToString().RemoveWhitespace()}");
        }

        protected override void Assert()
        {
            var actualJsonMinusWhitespace = this.ActualJson.RemoveWhitespace();
            var expectedJsonMinusWhitespace = this.ExpectedJson.RemoveWhitespace();

            actualJsonMinusWhitespace.Should().Be(expectedJsonMinusWhitespace);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public class ClassWithScalars(string requiredName, long requiredNumber, bool requiredPredicate)
    {
        public string RequiredName { get; set; } = requiredName;
        public long RequiredNumber { get; set; } = requiredNumber;
        public bool RequiredPredicate { get; set; } = requiredPredicate;

        public string? OptionalName { get; set; }
        public long? OptionalNumber { get; set; }
        public bool? OptionalPredicate { get; set; }
    }

    public enum StopLight
    {
        None,
        Green,
        Yellow,
        Red
    }

    public class TestExtension1
    {
        public string Description { get; set; } = "This is test extension 1.";
    }

    public class TestExtension2
    {
        public string Id { get; set; } = "2";
        public string Name { get; set; } = "Test Extension 2.";
    }

    public static TheoryDataRow<IXUnitTest>[] JsonDeserializeTheoryData =>
    [
        // Null
        new JsonDeserializeTest
        {
            Name = "Null",
            SourceJson = "null",
            ExpectedApiType = null
        },

        // ApiScalarType
        new JsonDeserializeTest
        {
            Name = "ApiScalarType [Boolean]",
            SourceJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiScalarType("Boolean", typeof(bool))
        },

        new JsonDeserializeTest
        {
            Name = "ApiScalarType [Boolean] With Extension 1",
            SourceJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""This is test extension 1.""
                    }
                }
            }",
            ExpectedApiType = new ApiScalarType("Boolean", typeof(bool)),
            AddTestExtension1 = true
        },

        new JsonDeserializeTest
        {
            Name = "ApiScalarType [Boolean] With Extra Property",
            SourceJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib"",
                ""Unexpected"": ""IgnoreMe""
            }",
            ExpectedApiType = new ApiScalarType("Boolean", typeof(bool))
        },

        new JsonDeserializeTest
        {
            Name = "ApiScalarType [ID]",
            SourceJson =  @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""ID"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiScalarType("ID", typeof(string))
        },

        new JsonDeserializeTest
        {
            Name = "ApiScalarType [Int]",
            SourceJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Int"",
                ""ClrType"": ""System.Int32, System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiScalarType("Int", typeof(int))
        },

        new JsonDeserializeTest
        {
            Name = "ApiScalarType [Float]",
            SourceJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Float"",
                ""ClrType"": ""System.Single, System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiScalarType("Float", typeof(float))
        },

        new JsonDeserializeTest
        {
            Name = "ApiScalarType [String]",
            SourceJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""String"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiScalarType("String", typeof(string))
        },

        // ApiEnumType
        new JsonDeserializeTest
        {
            Name = $"ApiEnumType [{nameof(StopLight)}]",
            SourceJson = @"
            {
                ""Kind"": ""Enum"",
                ""ApiName"": ""StopLight"",
                ""ApiEnumValues"": [
                    {
                        ""ApiName"": ""None"",
                        ""ClrName"": ""None"",
                        ""ClrOrdinal"": 0
                    },
                    {
                        ""ApiName"": ""Green"",
                        ""ClrName"": ""Green"",
                        ""ClrOrdinal"": 1
                    },
                    {
                        ""ApiName"": ""Yellow"",
                        ""ClrName"": ""Yellow"",
                        ""ClrOrdinal"": 2
                    },
                    {
                        ""ApiName"": ""Red"",
                        ""ClrName"": ""Red"",
                        ""ClrOrdinal"": 3
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests""
            }",
            ExpectedApiType = new ApiEnumType
            (
                nameof(StopLight),
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
            )
        },

        new JsonDeserializeTest
        {
            Name = $"ApiEnumType [{nameof(StopLight)}] With Extension 2",
            SourceJson = @"
            {
                ""Kind"": ""Enum"",
                ""ApiName"": ""StopLight"",
                ""ApiEnumValues"": [
                    {
                        ""ApiName"": ""None"",
                        ""ClrName"": ""None"",
                        ""ClrOrdinal"": 0
                    },
                    {
                        ""ApiName"": ""Green"",
                        ""ClrName"": ""Green"",
                        ""ClrOrdinal"": 1
                    },
                    {
                        ""ApiName"": ""Yellow"",
                        ""ClrName"": ""Yellow"",
                        ""ClrOrdinal"": 2
                    },
                    {
                        ""ApiName"": ""Red"",
                        ""ClrName"": ""Red"",
                        ""ClrOrdinal"": 3
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BTestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""Test Extension 2.""
                    }
                }
            }",
            ExpectedApiType = new ApiEnumType
            (
                nameof(StopLight),
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
            ),
            AddTestExtension2 = true
        },

        // ApiCollectionType
        new JsonDeserializeTest
        {
            Name = "ApiCollectionType [List<String>]",
            SourceJson = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Scalar"",
                    ""ApiName"": ""String"",
                    ""ClrType"": ""System.String, System.Private.CoreLib""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiCollectionType(new ApiScalarType("String", typeof(string)), ApiTypeModifiers.Required, typeof(List<string>))
        },

        new JsonDeserializeTest
        {
            Name = "ApiCollectionType [List<String?>]",
            SourceJson = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Scalar"",
                    ""ApiName"": ""String"",
                    ""ClrType"": ""System.String, System.Private.CoreLib""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiCollectionType(new ApiScalarType("String", typeof(string)), ApiTypeModifiers.None, typeof(List<string?>))
        },

        new JsonDeserializeTest
        {
            Name = $"ApiCollectionType [List<{nameof(StopLight)}>]",
            SourceJson = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Enum"",
                    ""ApiName"": ""StopLight"",
                    ""ApiEnumValues"": [
                        {
                            ""ApiName"": ""None"",
                            ""ClrName"": ""None"",
                            ""ClrOrdinal"": 0
                        },
                        {
                            ""ApiName"": ""Green"",
                            ""ClrName"": ""Green"",
                            ""ClrOrdinal"": 1
                        },
                        {
                            ""ApiName"": ""Yellow"",
                            ""ClrName"": ""Yellow"",
                            ""ClrOrdinal"": 2
                        },
                        {
                            ""ApiName"": ""Red"",
                            ""ClrName"": ""Red"",
                            ""ClrOrdinal"": 3
                        }
                    ],
                    ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiCollectionType
            (
                new ApiEnumType
                (
                    nameof(StopLight),
                    [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                    typeof(StopLight)
                ),
                ApiTypeModifiers.Required,
                typeof(List<StopLight>)
            )
        },

        new JsonDeserializeTest
        {
            Name = $"ApiCollectionType [List<{nameof(StopLight)}?>]",
            SourceJson = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Enum"",
                    ""ApiName"": ""StopLight"",
                    ""ApiEnumValues"": [
                        {
                            ""ApiName"": ""None"",
                            ""ClrName"": ""None"",
                            ""ClrOrdinal"": 0
                        },
                        {
                            ""ApiName"": ""Green"",
                            ""ClrName"": ""Green"",
                            ""ClrOrdinal"": 1
                        },
                        {
                            ""ApiName"": ""Yellow"",
                            ""ClrName"": ""Yellow"",
                            ""ClrOrdinal"": 2
                        },
                        {
                            ""ApiName"": ""Red"",
                            ""ClrName"": ""Red"",
                            ""ClrOrdinal"": 3
                        }
                    ],
                    ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }",
            ExpectedApiType = new ApiCollectionType
            (
                new ApiEnumType
                (
                    nameof(StopLight),
                    [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                    typeof(StopLight)
                ),
                ApiTypeModifiers.None,
                typeof(List<StopLight>)
            )
        },

        // ApiObjectType
        new JsonDeserializeTest
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}]",
            SourceJson = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ClassWithScalars"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String"",
                            ""ClrType"": ""System.String, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long"",
                            ""ClrType"": ""System.Int64, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean"",
                            ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String"",
                            ""ClrType"": ""System.String, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long"",
                            ""ClrType"": ""System.Int64, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean"",
                            ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BClassWithScalars, Evoogle.ApiFramework.Core.Tests""
            }",
            ExpectedApiType = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiScalarType("String", typeof(string)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiScalarType("Long", typeof(long)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiScalarType("Boolean", typeof(bool)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiScalarType("String", typeof(string)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiScalarType("Long", typeof(long)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiScalarType("Boolean", typeof(bool)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                typeof(ClassWithScalars)
            )
        },

        new JsonDeserializeTest
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With Extension 1 and Extension 2",
            SourceJson = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ClassWithScalars"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String"",
                            ""ClrType"": ""System.String, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long"",
                            ""ClrType"": ""System.Int64, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean"",
                            ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String"",
                            ""ClrType"": ""System.String, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long"",
                            ""ClrType"": ""System.Int64, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean"",
                            ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BClassWithScalars, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""This is test extension 1.""
                    },
                    ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BTestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""Test Extension 2.""
                    }
                }
            }",
            ExpectedApiType = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiScalarType("String", typeof(string)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiScalarType("Long", typeof(long)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiScalarType("Boolean", typeof(bool)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiScalarType("String", typeof(string)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiScalarType("Long", typeof(long)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiScalarType("Boolean", typeof(bool)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                typeof(ClassWithScalars)
            ),
            AddTestExtension1 = true,
            AddTestExtension2 = true
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData =>
    [
        // Null
        new JsonRoundtripTest
        {
            Name = "Null",
            ExpectedApiType = null
        },

        // ApiScalarType
        new JsonRoundtripTest
        {
            Name = "ApiScalarType [Boolean]",
            ExpectedApiType = new ApiScalarType("Boolean", typeof(bool))
        },

        new JsonRoundtripTest
        {
            Name = "ApiScalarType [Boolean] With Extension 1",
            ExpectedApiType = new ApiScalarType("Boolean", typeof(bool)),
            AddTestExtension1 = true
        },

        new JsonRoundtripTest
        {
            Name = "ApiScalarType [ID]",
            ExpectedApiType = new ApiScalarType("ID", typeof(string))
        },

        new JsonRoundtripTest
        {
            Name = "ApiScalarType [Int]",
            ExpectedApiType = new ApiScalarType("Int", typeof(int))
        },

        new JsonRoundtripTest
        {
            Name = "ApiScalarType [Float]",
            ExpectedApiType = new ApiScalarType("Float", typeof(float))
        },

        new JsonRoundtripTest
        {
            Name = "ApiScalarType [String]",
            ExpectedApiType = new ApiScalarType("String", typeof(string))
        },

        // ApiEnumType
        new JsonRoundtripTest
        {
            Name = $"ApiEnumType [{nameof(StopLight)}]",
            ExpectedApiType = new ApiEnumType
            (
                nameof(StopLight),
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
            )
        },

        new JsonRoundtripTest
        {
            Name = $"ApiEnumType [{nameof(StopLight)}] With Extension 2",
            ExpectedApiType = new ApiEnumType
            (
                nameof(StopLight),
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
            ),
            AddTestExtension2 = true
        },

        // ApiCollectionType
        new JsonRoundtripTest
        {
            Name = "ApiCollectionType [List<String>]",
            ExpectedApiType = new ApiCollectionType(new ApiScalarType("String", typeof(string)), ApiTypeModifiers.Required, typeof(List<string>))
        },

        new JsonRoundtripTest
        {
            Name = "ApiCollectionType [List<String?>]",
            ExpectedApiType = new ApiCollectionType(new ApiScalarType("String", typeof(string)), ApiTypeModifiers.None, typeof(List<string?>))
        },

        new JsonRoundtripTest
        {
            Name = $"ApiCollectionType [List<{nameof(StopLight)}>]",
            ExpectedApiType = new ApiCollectionType
            (
                new ApiEnumType
                (
                    nameof(StopLight),
                    [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                    typeof(StopLight)
                ),
                ApiTypeModifiers.Required,
                typeof(List<StopLight>)
            )
        },

        new JsonRoundtripTest
        {
            Name = $"ApiCollectionType [List<{nameof(StopLight)}?>]",
            ExpectedApiType = new ApiCollectionType
            (
                new ApiEnumType
                (
                    nameof(StopLight),
                    [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                    typeof(StopLight)
                ),
                ApiTypeModifiers.None,
                typeof(List<StopLight>)
            )
        },

        // ApiObjectType
        new JsonRoundtripTest
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}]",
            ExpectedApiType = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiScalarType("String", typeof(string)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiScalarType("Long", typeof(long)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiScalarType("Boolean", typeof(bool)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiScalarType("String", typeof(string)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiScalarType("Long", typeof(long)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiScalarType("Boolean", typeof(bool)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                typeof(ClassWithScalars)
            )
        },

        new JsonRoundtripTest
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With Extension 1 and Extension 2",
            ExpectedApiType = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiScalarType("String", typeof(string)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiScalarType("Long", typeof(long)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiScalarType("Boolean", typeof(bool)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiScalarType("String", typeof(string)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiScalarType("Long", typeof(long)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiScalarType("Boolean", typeof(bool)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                typeof(ClassWithScalars)
            ),
            AddTestExtension1 = true,
            AddTestExtension2 = true
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] JsonSerializeTheoryData =>
    [
        // Null
        new JsonSerializeTest
        {
            Name = "Null",
            SourceApiType = null,
            ExpectedJson = "null"
        },

        // ApiScalarType
        new JsonSerializeTest
        {
            Name = "ApiScalarType [Boolean]",
            SourceApiType = new ApiScalarType("Boolean", typeof(bool)),
            ExpectedJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [Boolean] With Extension 1",
            SourceApiType = new ApiScalarType("Boolean", typeof(bool)),
            ExpectedJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Boolean"",
                ""ClrType"": ""System.Boolean, System.Private.CoreLib"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""This is test extension 1.""
                    }
                }
            }",
            AddTestExtension1 = true
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [ID]",
            SourceApiType = new ApiScalarType("ID", typeof(string)),
            ExpectedJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""ID"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [Int]",
            SourceApiType = new ApiScalarType("Int", typeof(int)),
            ExpectedJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Int"",
                ""ClrType"": ""System.Int32, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [Float]",
            SourceApiType = new ApiScalarType("Float", typeof(float)),
            ExpectedJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""Float"",
                ""ClrType"": ""System.Single, System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "ApiScalarType [String]",
            SourceApiType = new ApiScalarType("String", typeof(string)),
            ExpectedJson = @"
            {
                ""Kind"": ""Scalar"",
                ""ApiName"": ""String"",
                ""ClrType"": ""System.String, System.Private.CoreLib""
            }"
        },

        // ApiEnumType
        new JsonSerializeTest
        {
            Name = $"ApiEnumType [{nameof(StopLight)}]",
            SourceApiType = new ApiEnumType
            (
                nameof(StopLight),
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
            ),
            ExpectedJson = @"
            {
                ""Kind"": ""Enum"",
                ""ApiName"": ""StopLight"",
                ""ApiEnumValues"": [
                    {
                        ""ApiName"": ""None"",
                        ""ClrName"": ""None"",
                        ""ClrOrdinal"": 0
                    },
                    {
                        ""ApiName"": ""Green"",
                        ""ClrName"": ""Green"",
                        ""ClrOrdinal"": 1
                    },
                    {
                        ""ApiName"": ""Yellow"",
                        ""ClrName"": ""Yellow"",
                        ""ClrOrdinal"": 2
                    },
                    {
                        ""ApiName"": ""Red"",
                        ""ClrName"": ""Red"",
                        ""ClrOrdinal"": 3
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"ApiEnumType [{nameof(StopLight)}] With Extension 2",
            SourceApiType = new ApiEnumType
            (
                nameof(StopLight),
                [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                typeof(StopLight)
            ),
            ExpectedJson = @"
            {
                ""Kind"": ""Enum"",
                ""ApiName"": ""StopLight"",
                ""ApiEnumValues"": [
                    {
                        ""ApiName"": ""None"",
                        ""ClrName"": ""None"",
                        ""ClrOrdinal"": 0
                    },
                    {
                        ""ApiName"": ""Green"",
                        ""ClrName"": ""Green"",
                        ""ClrOrdinal"": 1
                    },
                    {
                        ""ApiName"": ""Yellow"",
                        ""ClrName"": ""Yellow"",
                        ""ClrOrdinal"": 2
                    },
                    {
                        ""ApiName"": ""Red"",
                        ""ClrName"": ""Red"",
                        ""ClrOrdinal"": 3
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BTestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""Test Extension 2.""
                    }
                }
            }",
            AddTestExtension2 = true
        },

        // ApiCollectionType
        new JsonSerializeTest
        {
            Name = "ApiCollectionType [List<String>]",
            SourceApiType = new ApiCollectionType(new ApiScalarType("String", typeof(string)), ApiTypeModifiers.Required, typeof(List<string>)),
            ExpectedJson = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Scalar"",
                    ""ApiName"": ""String"",
                    ""ClrType"": ""System.String, System.Private.CoreLib""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = "ApiCollectionType [List<String?>]",
            SourceApiType = new ApiCollectionType(new ApiScalarType("String", typeof(string)), ApiTypeModifiers.None, typeof(List<string?>)),
            ExpectedJson = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Scalar"",
                    ""ApiName"": ""String"",
                    ""ClrType"": ""System.String, System.Private.CoreLib""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[System.String, System.Private.CoreLib]], System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"ApiCollectionType [List<{nameof(StopLight)}>]",
            SourceApiType = new ApiCollectionType
            (
                new ApiEnumType
                (
                    nameof(StopLight),
                    [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                    typeof(StopLight)
                ),
                ApiTypeModifiers.Required,
                typeof(List<StopLight>)
            ),
            ExpectedJson = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Enum"",
                    ""ApiName"": ""StopLight"",
                    ""ApiEnumValues"": [
                        {
                            ""ApiName"": ""None"",
                            ""ClrName"": ""None"",
                            ""ClrOrdinal"": 0
                        },
                        {
                            ""ApiName"": ""Green"",
                            ""ClrName"": ""Green"",
                            ""ClrOrdinal"": 1
                        },
                        {
                            ""ApiName"": ""Yellow"",
                            ""ClrName"": ""Yellow"",
                            ""ClrOrdinal"": 2
                        },
                        {
                            ""ApiName"": ""Red"",
                            ""ClrName"": ""Red"",
                            ""ClrOrdinal"": 3
                        }
                    ],
                    ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests""
                },
                ""ApiItemTypeModifiers"": ""Required"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"ApiCollectionType [List<{nameof(StopLight)}?>]",
            SourceApiType = new ApiCollectionType
            (
                new ApiEnumType
                (
                    nameof(StopLight),
                    [new ApiEnumValue("None", "None", 0), new ApiEnumValue("Green", "Green", 1), new ApiEnumValue("Yellow", "Yellow", 2), new ApiEnumValue("Red", "Red", 3)],
                    typeof(StopLight)
                ),
                ApiTypeModifiers.None,
                typeof(List<StopLight>)
            ),
            ExpectedJson = @"
            {
                ""Kind"": ""Collection"",
                ""ApiItemType"": {
                    ""Kind"": ""Enum"",
                    ""ApiName"": ""StopLight"",
                    ""ApiEnumValues"": [
                        {
                            ""ApiName"": ""None"",
                            ""ClrName"": ""None"",
                            ""ClrOrdinal"": 0
                        },
                        {
                            ""ApiName"": ""Green"",
                            ""ClrName"": ""Green"",
                            ""ClrOrdinal"": 1
                        },
                        {
                            ""ApiName"": ""Yellow"",
                            ""ClrName"": ""Yellow"",
                            ""ClrOrdinal"": 2
                        },
                        {
                            ""ApiName"": ""Red"",
                            ""ClrName"": ""Red"",
                            ""ClrOrdinal"": 3
                        }
                    ],
                    ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests""
                },
                ""ApiItemTypeModifiers"": ""None"",
                ""ClrType"": ""System.Collections.Generic.List\u00601[[Evoogle.ApiFramework.Schema.ApiTypeTests\u002BStopLight, Evoogle.ApiFramework.Core.Tests]], System.Private.CoreLib""
            }"
        },

        // ApiObjectType
        new JsonSerializeTest
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}]",
            SourceApiType = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiScalarType("String", typeof(string)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiScalarType("Long", typeof(long)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiScalarType("Boolean", typeof(bool)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiScalarType("String", typeof(string)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiScalarType("Long", typeof(long)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiScalarType("Boolean", typeof(bool)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                typeof(ClassWithScalars)
            ),
            ExpectedJson = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ClassWithScalars"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String"",
                            ""ClrType"": ""System.String, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long"",
                            ""ClrType"": ""System.Int64, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean"",
                            ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String"",
                            ""ClrType"": ""System.String, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long"",
                            ""ClrType"": ""System.Int64, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean"",
                            ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BClassWithScalars, Evoogle.ApiFramework.Core.Tests""
            }"
        },

        new JsonSerializeTest
        {
            Name = $"ApiObjectType [{nameof(ClassWithScalars)}] With Extension 1 and Extension 2",
            SourceApiType = new ApiObjectType
            (
                nameof(ClassWithScalars),
                [
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredName),
                        new ApiScalarType("String", typeof(string)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredNumber),
                        new ApiScalarType("Long", typeof(long)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.RequiredPredicate),
                        new ApiScalarType("Boolean", typeof(bool)),
                        ApiTypeModifiers.Required,
                        nameof(ClassWithScalars.RequiredPredicate)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalName),
                        new ApiScalarType("String", typeof(string)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalName)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalNumber),
                        new ApiScalarType("Long", typeof(long)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalNumber)
                    ),
                    new ApiProperty
                    (
                        nameof(ClassWithScalars.OptionalPredicate),
                        new ApiScalarType("Boolean", typeof(bool)),
                        ApiTypeModifiers.None,
                        nameof(ClassWithScalars.OptionalPredicate)
                    ),
                ],
                typeof(ClassWithScalars)
            ),
            ExpectedJson = @"
            {
                ""Kind"": ""Object"",
                ""ApiName"": ""ClassWithScalars"",
                ""ApiProperties"": [
                    {
                        ""ApiName"": ""RequiredName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String"",
                            ""ClrType"": ""System.String, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredName""
                    },
                    {
                        ""ApiName"": ""RequiredNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long"",
                            ""ClrType"": ""System.Int64, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredNumber""
                    },
                    {
                        ""ApiName"": ""RequiredPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean"",
                            ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""Required"",
                        ""ClrName"": ""RequiredPredicate""
                    },
                    {
                        ""ApiName"": ""OptionalName"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""String"",
                            ""ClrType"": ""System.String, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalName""
                    },
                    {
                        ""ApiName"": ""OptionalNumber"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Long"",
                            ""ClrType"": ""System.Int64, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalNumber""
                    },
                    {
                        ""ApiName"": ""OptionalPredicate"",
                        ""ApiType"": {
                            ""Kind"": ""Scalar"",
                            ""ApiName"": ""Boolean"",
                            ""ClrType"": ""System.Boolean, System.Private.CoreLib""
                        },
                        ""ApiTypeModifiers"": ""None"",
                        ""ClrName"": ""OptionalPredicate""
                    }
                ],
                ""ClrType"": ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BClassWithScalars, Evoogle.ApiFramework.Core.Tests"",
                ""Extensions"": {
                    ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BTestExtension1, Evoogle.ApiFramework.Core.Tests"": {
                        ""Description"": ""This is test extension 1.""
                    },
                    ""Evoogle.ApiFramework.Schema.ApiTypeTests\u002BTestExtension2, Evoogle.ApiFramework.Core.Tests"": {
                        ""Id"": ""2"",
                        ""Name"": ""Test Extension 2.""
                    }
                }
            }",
            AddTestExtension1 = true,
            AddTestExtension2 = true
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(JsonDeserializeTheoryData))]
    public void JsonDeserialize(IXUnitTest test)
    {
        test.Execute(this);
    }

    [Theory]
    [MemberData(nameof(JsonRoundtripTheoryData))]
    public void JsonRoundtrip(IXUnitTest test)
    {
        test.Execute(this);
    }

    [Theory]
    [MemberData(nameof(JsonSerializeTheoryData))]
    public void JsonSerialize(IXUnitTest test)
    {
        test.Execute(this);
    }
    #endregion
}
