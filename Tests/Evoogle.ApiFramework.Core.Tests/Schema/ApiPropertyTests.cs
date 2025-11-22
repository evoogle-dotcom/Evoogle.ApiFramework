// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public class ApiPropertyTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Base Tests
    public abstract class TryTestBase : XUnitTest
    {
        #region User Supplied Properties
        public required ApiTestSchemaKind ApiSchemaKind { get; init; }
        public required string ApiObjectTypeName { get; init; }
        public required string ApiPropertyName { get; init; }
        public required bool ExpectedSuccess { get; init; }
        #endregion

        #region Calculated Properties
        protected ApiSchema ApiSchema { get; set; } = default!;
        protected ApiObjectType ApiObjectType { get; set; } = default!;
        protected ApiProperty ApiProperty { get; set; } = default!;
        protected bool ActualSuccess { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = ApiTestSchemaFactory.BuildTestSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema;

            var apiObjectType = this.ApiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName);
            this.ApiObjectType = apiObjectType;

            var apiProperty = apiObjectType.GetPropertyByApiName(this.ApiPropertyName);
            this.ApiProperty = apiProperty;
        }
        #endregion
    }
    #endregion

    #region TryGet Tests (Generic and Non-Generic)
    public class TryGetGenericTest<TObject, TValue> : TryTestBase
    {
        #region User Supplied Properties
        public TObject? ClrObject { get; init; }
        public TValue? ExpectedClrValue { get; init; }
        #endregion

        #region Calculated Properties
        private TValue? ActualClrValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            base.Arrange();

            this.WriteLine($"ApiSchema:     {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType: {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiProperty:   {this.ApiPropertyName.SafeToString()}");
            this.WriteLine($"ClrObject:     {this.ClrObject.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"ExpectedSuccess:  {this.ExpectedSuccess.SafeToString()}");
            this.WriteLine($"ExpectedClrValue: {this.ExpectedClrValue.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualSuccess = this.ApiProperty.TryGetValue<TObject, TValue>(this.ClrObject, out var clrValue);
            this.ActualClrValue = clrValue;
        }

        protected override void Assert()
        {
            this.ActualSuccess.Should().Be(this.ExpectedSuccess);
            if (this.ExpectedSuccess)
            {
                this.ActualClrValue.Should().BeEquivalentTo(this.ExpectedClrValue);
            }
            else
            {
                this.ActualClrValue.Should().Be(default(TValue));
            }
        }
        #endregion
    }

    public class TryGetNonGenericTest : TryTestBase
    {
        #region User Supplied Properties
        public object? ClrObject { get; init; }
        public object? ExpectedClrValue { get; init; }
        #endregion

        #region Calculated Properties
        private object? ActualClrValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            base.Arrange();

            this.WriteLine($"ApiSchema:     {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType: {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiProperty:   {this.ApiPropertyName.SafeToString()}");
            this.WriteLine($"ClrObject:     {this.ClrObject.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"ExpectedSuccess:  {this.ExpectedSuccess.SafeToString()}");
            this.WriteLine($"ExpectedClrValue: {this.ExpectedClrValue.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualSuccess = this.ApiProperty.TryGetValue(this.ClrObject, out var clrValue);
            this.ActualClrValue = clrValue;
        }

        protected override void Assert()
        {
            this.ActualSuccess.Should().Be(this.ExpectedSuccess);
            if (this.ExpectedSuccess)
            {
                this.ActualClrValue.Should().BeEquivalentTo(this.ExpectedClrValue);
            }
            else
            {
                this.ActualClrValue.Should().BeNull();
            }
        }
        #endregion
    }
    #endregion

    #region TrySet Tests
    public class TrySetGenericTest<TObject, TValue> : TryTestBase
    {
        #region User Supplied Properties
        public TObject? ClrObject { get; init; }
        public TValue? ClrValue { get; init; }
        #endregion

        #region Calculated Properties
        private bool ActualTrySetSuccess { get; set; }
        private bool ActualTryGetSuccess { get; set; }
        private TValue? ActualTryGetClrValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            base.Arrange();

            this.WriteLine($"ApiSchema:     {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType: {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiProperty:   {this.ApiPropertyName.SafeToString()}");
            this.WriteLine($"ClrObject:     {this.ClrObject.SafeToString()}");
            this.WriteLine($"ClrValue:      {this.ClrValue.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"ExpectedSuccess:  {this.ExpectedSuccess.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualTrySetSuccess = this.ApiProperty!.TrySetValue(this.ClrObject, this.ClrValue);
            if (this.ActualTrySetSuccess)
            {
                // Read back via TryGet to verify mutation
                this.ActualTryGetSuccess = this.ApiProperty!.TryGetValue<TObject, TValue>(this.ClrObject, out var clrValue);
                this.ActualTryGetClrValue = clrValue;
            }
        }

        protected override void Assert()
        {
            this.ActualTrySetSuccess.Should().Be(this.ExpectedSuccess);
            if (this.ExpectedSuccess)
            {
                this.ActualTryGetSuccess.Should().BeTrue("TryGetValue should succeed after TrySetValue");
                this.ActualTryGetClrValue.Should().BeEquivalentTo(this.ClrValue);
            }
        }
        #endregion
    }

    public class TrySetNonGenericTest : TryTestBase
    {
        #region User Supplied Properties
        public object? ClrObject { get; init; }
        public object? ClrValue { get; init; }
        #endregion

        #region Calculated Properties
        private bool ActualTrySetSuccess { get; set; }
        private bool ActualTryGetSuccess { get; set; }
        private object? ActualTryGetClrValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            base.Arrange();

            this.WriteLine($"ApiSchema:     {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType: {this.ApiObjectTypeName.SafeToString()}");
            this.WriteLine($"ApiProperty:   {this.ApiPropertyName.SafeToString()}");
            this.WriteLine($"ClrObject:     {this.ClrObject.SafeToString()}");
            this.WriteLine($"ClrValue:      {this.ClrValue.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"ExpectedSuccess:  {this.ExpectedSuccess.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualTrySetSuccess = this.ApiProperty!.TrySetValue(this.ClrObject, this.ClrValue);
            if (this.ActualTrySetSuccess)
            {
                // Read back via TryGet to verify mutation
                this.ActualTryGetSuccess = this.ApiProperty!.TryGetValue(this.ClrObject, out var clrValue);
                this.ActualTryGetClrValue = clrValue;
            }
        }

        protected override void Assert()
        {
            this.ActualTrySetSuccess.Should().Be(this.ExpectedSuccess);
            if (this.ExpectedSuccess)
            {
                this.ActualTryGetSuccess.Should().BeTrue("TryGetValue should succeed after TrySetValue");
                this.ActualTryGetClrValue.Should().BeEquivalentTo(this.ClrValue);
            }
        }
        #endregion
    }

    // public class TrySetByRefStructTest<TValue> : XUnitTest
    // {
    //     #region User Supplied Properties
    //     public string? ClrName { get; init; }
    //     public TestPoint Target { get; init; }
    //     public TValue InputValue { get; init; } = default!;
    //     public int ExpectedX { get; init; }
    //     public int ExpectedY { get; init; }
    //     #endregion

    //     #region Calculated Properties
    //     private ApiProperty? ApiProperty { get; set; }
    //     private bool? ActualSuccess { get; set; }
    //     private TestPoint ActualPoint { get; set; }
    //     #endregion

    //     #region XUnitTest Methods
    //     protected override void Arrange()
    //     {
    //         this.ClrName.Should().NotBeNullOrWhiteSpace();
    //         var memberType = typeof(TestPoint).GetProperty(this!.ClrName!)?.PropertyType
    //                        ?? typeof(TestPoint).GetField(this!.ClrName!)?.FieldType
    //                        ?? typeof(object);

    //         this.ApiProperty = new ApiProperty(this!.ClrName!, new ApiTypeExpression(memberType), ApiTypeModifiers.None, this.ClrName!);
    //     }

    //     protected override void Act()
    //     {
    //         var p = this.Target; // local copy
    //         this.ActualSuccess = this.ApiProperty!.TrySetValueRef<TestPoint, TValue>(ref p, this.InputValue);
    //         this.ActualPoint = p;
    //     }

    //     protected override void Assert()
    //     {
    //         this.ActualSuccess.Should().BeTrue();
    //         this.ActualPoint.X.Should().Be(this.ExpectedX);
    //         this.ActualPoint.Y.Should().Be(this.ExpectedY);
    //     }
    //     #endregion
    // }
    #endregion

    #region Theory Data
    public static ApiSchema TestSchema { get; } = ApiTestSchemaFactory.BuildSimpleSchema();

    public static TheoryDataRow<IXUnitTest>[] TryGetGenericTheoryData =>
    [
        new TryGetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} TryGetValue<TObject,TValue> returns failure for null object",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = null,
            ExpectedSuccess = false
        },

        new TryGetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} TryGetValue<TObject,TValue> returns success for required string property",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = "Alice"
        },

        new TryGetGenericTest<ScalarsOnly, long>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredNumber)} TryGetValue<TObject,TValue> returns success for required long property",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = 123
        },

        new TryGetGenericTest<ScalarsOnly, bool>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredPredicate)} TryGetValue<TObject,TValue> returns success for required bool property",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = true
        },

        new TryGetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} TryGetValue<TObject,TValue> returns success for optional string property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ExpectedSuccess = true,
            ExpectedClrValue = "Bob"
        },

        new TryGetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} TryGetValue<TObject,TValue> returns success for optional string property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },

        new TryGetGenericTest<ScalarsOnly, long?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} TryGetValue<TObject,TValue> returns success for optional long property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ExpectedSuccess = true,
            ExpectedClrValue = 42
        },

        new TryGetGenericTest<ScalarsOnly, long?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} TryGetValue<TObject,TValue> returns success for optional long property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },

        new TryGetGenericTest<ScalarsOnly, bool?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} TryGetValue<TObject,TValue> returns success for optional bool property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ExpectedSuccess = true,
            ExpectedClrValue = false
        },

        new TryGetGenericTest<ScalarsOnly, bool?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} TryGetValue<TObject,TValue> returns success for optional bool property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetNonGenericTheoryData =>
    [
        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} TryGetValue returns failure for null object",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = null,
            ExpectedSuccess = false
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} TryGetValue returns success for required string property",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = "Alice"
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredNumber)} TryGetValue returns success for required long property",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = 123
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredPredicate)} TryGetValue returns success for required bool property",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = true
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} TryGetValue returns success for optional string property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ExpectedSuccess = true,
            ExpectedClrValue = "Bob"
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} TryGetValue returns success for optional string property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} TryGetValue returns success for optional long property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ExpectedSuccess = true,
            ExpectedClrValue = 42
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} TryGetValue returns success for optional long property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} TryGetValue returns success for optional bool property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ExpectedSuccess = true,
            ExpectedClrValue = false
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} TryGetValue returns success for optional bool property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TrySetGenericTheoryData =>
    [
        new TrySetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} TrySetValue<TObject,TValue> returns success for required string property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = "Bob",
            ExpectedSuccess = true,
        },

        new TrySetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} TrySetValue<TObject,TValue> returns success for required string property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = null,
            ExpectedSuccess = true,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TrySetNonGenericTheoryData =>
    [
        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} TrySetValue<TObject,TValue> returns success for required string property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = "Bob",
            ExpectedSuccess = true,
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} TrySetValue<TObject,TValue> returns success for required string property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = null,
            ExpectedSuccess = true,
        },
    ];

    // public static TheoryDataRow<IXUnitTest>[] TrySetTheoryData =>
    // [
    //     // object-based setter on reference type
    //     new TrySetObjectTest
    //     {
    //         Name = "TrySetValue(object, object?) sets reference type property",
    //         ClrName = nameof(Person.Name),
    //         Target = new Person { Name = "Old" },
    //         InputValue = "New",
    //         ExpectedSuccess = true,
    //         ExpectedValue = "New"
    //     },

    //     // generic setter on reference type
    //     new TrySetGenericTest<Person, string>
    //     {
    //         Name = "TrySetValue<TObject,TValue> sets reference type property",
    //         ClrName = nameof(Person.Name),
    //         Target = new Person { Name = "Old" },
    //         InputValue = "New",
    //         ExpectedSuccess = true,
    //         ExpectedValue = "New"
    //     },

    //     // generic setter on reference type, numeric
    //     new TrySetGenericTest<ScalarsOnly, long>
    //     {
    //         Name = "TrySetValue<TObject,TValue> sets numeric property",
    //         ClrName = nameof(ScalarsOnly.RequiredNumber),
    //         Target = new ScalarsOnly("n", 1, true),
    //         InputValue = 42L,
    //         ExpectedSuccess = true,
    //         ExpectedValue = 42L
    //     },

    //     // non-ref setter on struct target does not mutate caller (field)
    //     new TrySetGenericTest<TestPoint, int>
    //     {
    //         Name = "TrySetValue<TObject,TValue> on struct target assigns copy (field)",
    //         ClrName = nameof(TestPoint.X),
    //         Target = new TestPoint { X = 1, Y = 2 },
    //         InputValue = 10,
    //         ExpectedSuccess = true,
    //         // Reading back from boxed Target reflects original value (no mutation)
    //         ExpectedValue = 1
    //     },

    //     // non-ref setter on struct target does not mutate caller (property)
    //     new TrySetGenericTest<TestPoint, int>
    //     {
    //         Name = "TrySetValue<TObject,TValue> on struct target assigns copy (property)",
    //         ClrName = nameof(TestPoint.Y),
    //         Target = new TestPoint { X = 1, Y = 2 },
    //         InputValue = 20,
    //         ExpectedSuccess = true,
    //         ExpectedValue = 2
    //     },

    //     // ref setter on struct target mutates caller (field)
    //     new TrySetByRefStructTest<int>
    //     {
    //         Name = "TrySetValueRef mutates struct target (field)",
    //         ClrName = nameof(TestPoint.X),
    //         Target = new TestPoint { X = 1, Y = 2 },
    //         InputValue = 10,
    //         ExpectedX = 10,
    //         ExpectedY = 2
    //     },

    //     // ref setter on struct target mutates caller (property)
    //     new TrySetByRefStructTest<int>
    //     {
    //         Name = "TrySetValueRef mutates struct target (property)",
    //         ClrName = nameof(TestPoint.Y),
    //         Target = new TestPoint { X = 1, Y = 2 },
    //         InputValue = 20,
    //         ExpectedX = 1,
    //         ExpectedY = 20
    //     },
    // ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(TryGetGenericTheoryData))]
    public void TryGetGeneric(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetNonGenericTheoryData))]
    public void TryGetNonGeneric(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TrySetGenericTheoryData))]
    public void TrySetGeneric(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TrySetNonGenericTheoryData))]
    public void TrySetNonGeneric(IXUnitTest test) => test.Execute(this);
    #endregion
}
