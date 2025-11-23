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
        #endregion

        #region Calculated Properties
        protected ApiSchema? ApiSchema { get; set; }
        protected ApiObjectType? ApiObjectType { get; set; }
        protected ApiProperty? ApiProperty { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = ApiTestSchemaFactory.BuildTestSchema(this.ApiSchemaKind);
            this.ApiSchema = apiSchema ?? throw new InvalidOperationException($"{nameof(Schema.ApiSchema)} creation failed.");

            var apiObjectType = this.ApiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName);
            this.ApiObjectType = apiObjectType ?? throw new InvalidOperationException($"{nameof(Schema.ApiObjectType)} '{this.ApiObjectTypeName}' not found in ApiSchema.");

            var apiProperty = apiObjectType.GetPropertyByApiName(this.ApiPropertyName);
            this.ApiProperty = apiProperty ?? throw new InvalidOperationException($"{nameof(Schema.ApiProperty)} '{this.ApiPropertyName}' not found in ApiObjectType '{this.ApiObjectTypeName}'.");

            this.WriteLine($"ApiSchema:     {this.ApiSchema.ApiName.SafeToString()}");
            this.WriteLine($"ApiObjectType: {this.ApiObjectType.ApiName.SafeToString()}");
            this.WriteLine($"ApiProperty:   {this.ApiProperty.ApiName.SafeToString()}");
            this.WriteLine();
        }
        #endregion
    }


    public abstract class TryGetTestBase : TryTestBase
    {
        #region User Supplied Properties
        public required bool ExpectedSuccess { get; init; }
        #endregion

        #region Calculated Properties
        protected bool? ActualSuccess { get; set; }
        #endregion
    }

    public abstract class TrySetTestBase : TryTestBase
    {
        #region User Supplied Properties
        public required bool ExpectedTrySetSuccess { get; set; }
        #endregion

        #region Calculated Properties
        protected bool? ActualTrySetSuccess { get; set; }
        protected bool? ActualTryGetSuccess { get; set; }
        #endregion
    }
    #endregion

    #region TryGet Tests (Generic and Non-Generic)
    public class TryGetGenericTest<TObject, TValue> : TryGetTestBase
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

            this.WriteLine($"ClrObject: {this.ClrObject.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"ExpectedSuccess:  {this.ExpectedSuccess.SafeToString()}");
            this.WriteLine($"ExpectedClrValue: {this.ExpectedClrValue.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualSuccess = this.ApiProperty!.TryGetValue<TObject, TValue>(this.ClrObject, out var clrValue);
            this.ActualClrValue = clrValue;

            this.WriteLine();
            this.WriteLine($"ActualSuccess:  {this.ActualSuccess.SafeToString()}");
            this.WriteLine($"ActualClrValue: {this.ActualClrValue.SafeToString()}");
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

    public class TryGetNonGenericTest : TryGetTestBase
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

            this.WriteLine($"ClrObject: {this.ClrObject.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"ExpectedSuccess:  {this.ExpectedSuccess.SafeToString()}");
            this.WriteLine($"ExpectedClrValue: {this.ExpectedClrValue.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualSuccess = this.ApiProperty!.TryGetValue(this.ClrObject, out var clrValue);
            this.ActualClrValue = clrValue;

            this.WriteLine();
            this.WriteLine($"ActualSuccess:  {this.ActualSuccess.SafeToString()}");
            this.WriteLine($"ActualClrValue: {this.ActualClrValue.SafeToString()}");
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
    public class TrySetGenericTest<TObject, TValue> : TrySetTestBase
    {
        #region User Supplied Properties
        public TObject? ClrObject { get; init; }
        public TValue? ClrValue { get; init; }
        #endregion

        #region Calculated Properties
        private TValue? ActualTryGetClrValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            base.Arrange();

            this.WriteLine($"ClrObject: {this.ClrObject.SafeToString()}");
            this.WriteLine($"ClrValue:  {this.ClrValue.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"ExpectedTrySetSuccess: {this.ExpectedTrySetSuccess.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualTrySetSuccess = this.ApiProperty!.TrySetValue(this.ClrObject, this.ClrValue);
            if (this.ActualTrySetSuccess.Value)
            {
                // Read back via TryGet to verify mutation
                this.ActualTryGetSuccess = this.ApiProperty!.TryGetValue<TObject, TValue>(this.ClrObject, out var clrValue);
                this.ActualTryGetClrValue = clrValue;
            }

            this.WriteLine();
            this.WriteLine($"ActualTrySetSuccess:  {this.ActualTrySetSuccess.SafeToString()}");
            this.WriteLine($"ActualTryGetSuccess:  {this.ActualTryGetSuccess.SafeToString()}");
            this.WriteLine($"ActualTryGetClrValue: {this.ActualTryGetClrValue.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualTrySetSuccess.Should().Be(this.ExpectedTrySetSuccess);
            if (this.ExpectedTrySetSuccess)
            {
                this.ActualTryGetSuccess.Should().BeTrue("TryGetValue should succeed after TrySetValue");
                this.ActualTryGetClrValue.Should().BeEquivalentTo(this.ClrValue);
            }
        }
        #endregion
    }

    public class TrySetNonGenericTest : TrySetTestBase
    {
        #region User Supplied Properties
        public object? ClrObject { get; init; }
        public object? ClrValue { get; init; }
        #endregion

        #region Calculated Properties
        private object? ActualTryGetClrValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            base.Arrange();

            this.WriteLine($"ClrObject: {this.ClrObject.SafeToString()}");
            this.WriteLine($"ClrValue:  {this.ClrValue.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"ExpectedTrySetSuccess: {this.ExpectedTrySetSuccess.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualTrySetSuccess = this.ApiProperty!.TrySetValue(this.ClrObject, this.ClrValue);
            if (this.ActualTrySetSuccess.Value)
            {
                // Read back via TryGet to verify mutation
                this.ActualTryGetSuccess = this.ApiProperty!.TryGetValue(this.ClrObject, out var clrValue);
                this.ActualTryGetClrValue = clrValue;
            }

            this.WriteLine();
            this.WriteLine($"ActualTrySetSuccess:  {this.ActualTrySetSuccess.SafeToString()}");
            this.WriteLine($"ActualTryGetSuccess:  {this.ActualTryGetSuccess.SafeToString()}");
            this.WriteLine($"ActualTryGetClrValue: {this.ActualTryGetClrValue.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualTrySetSuccess.Should().Be(this.ExpectedTrySetSuccess);
            if (this.ExpectedTrySetSuccess)
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
            ExpectedTrySetSuccess = true,
        },

        new TrySetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} TrySetValue<TObject,TValue> returns success for required string property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = null,
            ExpectedTrySetSuccess = true,
        },

        new TrySetGenericTest<ScalarsOnly, long>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredNumber)} TrySetValue<TObject,TValue> returns success for required long property",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = 42,
            ExpectedTrySetSuccess = true
        },

        new TrySetGenericTest<ScalarsOnly, bool>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredPredicate)} TrySetValue<TObject,TValue> returns success for required bool property",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = false,
            ExpectedTrySetSuccess = true
        },

        new TrySetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} TrySetValue<TObject,TValue> returns success for optional string property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ClrValue = "Charlie",
            ExpectedTrySetSuccess = true
        },

        new TrySetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} TrySetValue<TObject,TValue> returns success for optional string property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ClrValue = null,
            ExpectedTrySetSuccess = true
        },

        new TrySetGenericTest<ScalarsOnly, long?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} TrySetValue<TObject,TValue> returns success for optional long property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ClrValue = 100,
            ExpectedTrySetSuccess = true
        },

        new TrySetGenericTest<ScalarsOnly, long?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} TrySetValue<TObject,TValue> returns success for optional long property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ClrValue = null,
            ExpectedTrySetSuccess = true
        },

        new TrySetGenericTest<ScalarsOnly, bool?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} TrySetValue<TObject,TValue> returns success for optional bool property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ClrValue = true,
            ExpectedTrySetSuccess = true
        },

        new TrySetGenericTest<ScalarsOnly, bool?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} TrySetValue<TObject,TValue> returns success for optional bool property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ClrValue = null,
            ExpectedTrySetSuccess = true
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
            ExpectedTrySetSuccess = true,
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} TrySetValue<TObject,TValue> returns success for required string property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = null,
            ExpectedTrySetSuccess = true,
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredNumber)} TrySetValue returns success for required long property",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = 42,
            ExpectedTrySetSuccess = true
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredPredicate)} TrySetValue returns success for required bool property",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = false,
            ExpectedTrySetSuccess = true
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} TrySetValue returns success for optional string property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ClrValue = "Charlie",
            ExpectedTrySetSuccess = true
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} TrySetValue returns success for optional string property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ClrValue = null,
            ExpectedTrySetSuccess = true
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} TrySetValue returns success for optional long property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ClrValue = 100,
            ExpectedTrySetSuccess = true
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} TrySetValue returns success for optional long property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ClrValue = null,
            ExpectedTrySetSuccess = true
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} TrySetValue returns success for optional bool property for non-null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ClrValue = true,
            ExpectedTrySetSuccess = true
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} TrySetValue returns success for optional bool property for null value",
            ApiSchemaKind = ApiTestSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ClrValue = null,
            ExpectedTrySetSuccess = true
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
