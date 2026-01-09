// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema;

public class ApiPropertyTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Base Tests
    private abstract class TryTestBase : XUnitTest
    {
        #region User Supplied Properties
        public required ApiSchemaKind ApiSchemaKind { get; init; }
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
            var apiSchema = BuildTestApiSchema(this.ApiSchemaKind);
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

    private abstract class TryGetTestBase : TryTestBase
    {
        #region User Supplied Properties
        public required bool ExpectedSuccess { get; init; }
        #endregion

        #region Calculated Properties
        protected bool? ActualSuccess { get; set; }
        #endregion
    }

    private abstract class TrySetTestBase : TryTestBase
    {
        #region User Supplied Properties
        public required bool ExpectedTrySetSuccess { get; set; }
        #endregion

        #region Calculated Properties
        protected bool? ActualTrySetSuccess { get; set; }
        #endregion
    }
    #endregion

    #region TryGet Tests (Generic and Non-Generic)
    private class TryGetGenericTest<TObject, TValue> : TryGetTestBase
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

    private class TryGetNonGenericTest : TryGetTestBase
    {
        #region User Supplied Properties
        public object? ClrObject { get; init; }
        public Type? ClrValueType { get; init; }
        public object? ExpectedClrValue { get; init; }
        #endregion

        #region Calculated Properties
        private object? ActualClrValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            base.Arrange();

            this.WriteLine($"ClrObject:    {this.ClrObject.SafeToString()}");
            this.WriteLine($"ClrValueType: {this.ClrValueType.SafeToName()}");
            this.WriteLine();
            this.WriteLine($"ExpectedSuccess:  {this.ExpectedSuccess.SafeToString()}");
            this.WriteLine($"ExpectedClrValue: {this.ExpectedClrValue.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualSuccess = this.ApiProperty!.TryGetValue(this.ClrObject, out var clrValue, this.ClrValueType);
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
    private class TrySetGenericTest<TObject, TValue> : TrySetTestBase
    {
        #region User Supplied Properties
        public TObject? ClrObject { get; init; }
        public TValue? ClrValue { get; init; }
        public TObject? ExpectedTrySetClrObject { get; init; }
        #endregion

        #region Calculated Properties
        private TObject? ActualTrySetClrObject { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            base.Arrange();

            this.WriteLine($"ClrObject: {this.ClrObject.SafeToString()}");
            this.WriteLine($"ClrValue:  {this.ClrValue.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"ExpectedTrySetSuccess:   {this.ExpectedTrySetSuccess.SafeToString()}");
            this.WriteLine($"ExpectedTrySetClrObject: {this.ExpectedTrySetClrObject.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualTrySetSuccess = this.ApiProperty!.TrySetValue(this.ClrObject, this.ClrValue);
            this.ActualTrySetClrObject = this.ClrObject;

            this.WriteLine();
            this.WriteLine($"ActualTrySetSuccess:   {this.ActualTrySetSuccess.SafeToString()}");
            this.WriteLine($"ActualTrySetClrObject: {this.ActualTrySetClrObject.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualTrySetSuccess.Should().Be(this.ExpectedTrySetSuccess);
            if (this.ExpectedTrySetSuccess)
            {
                this.ActualTrySetClrObject.Should().BeEquivalentTo(this.ExpectedTrySetClrObject);
            }
            else
            {
                this.ActualTrySetClrObject.Should().BeEquivalentTo(this.ClrObject);
            }
        }
        #endregion
    }

    private class TrySetByRefGenericTest<TObject, TValue> : TrySetTestBase
        where TObject : struct
    {
        #region User Supplied Properties
        public TObject ClrObject { get; init; }
        public TValue? ClrValue { get; init; }
        public TObject? ExpectedTrySetClrObject { get; init; }
        #endregion

        #region Calculated Properties
        private TObject? ActualTrySetClrObject { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            base.Arrange();

            this.WriteLine($"ClrObject: {this.ClrObject.SafeToString()}");
            this.WriteLine($"ClrValue:  {this.ClrValue.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"ExpectedTrySetSuccess:   {this.ExpectedTrySetSuccess.SafeToString()}");
            this.WriteLine($"ExpectedTrySetClrObject: {this.ExpectedTrySetClrObject.SafeToString()}");
        }

        protected override void Act()
        {
            var local = this.ClrObject;
            this.ActualTrySetSuccess = this.ApiProperty!.TrySetValueByRef(ref local, this.ClrValue);
            this.ActualTrySetClrObject = local;

            this.WriteLine();
            this.WriteLine($"ActualTrySetSuccess:   {this.ActualTrySetSuccess.SafeToString()}");
            this.WriteLine($"ActualTrySetClrObject: {this.ActualTrySetClrObject.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualTrySetSuccess.Should().Be(this.ExpectedTrySetSuccess);
            if (this.ExpectedTrySetSuccess)
            {
                this.ActualTrySetClrObject.Should().BeEquivalentTo(this.ExpectedTrySetClrObject);
            }
            else
            {
                this.ActualTrySetClrObject.Should().BeEquivalentTo(this.ClrObject);
            }
        }
        #endregion
    }

    private class TrySetNonGenericTest : TrySetTestBase
    {
        #region User Supplied Properties
        public object? ClrObject { get; init; }
        public object? ClrValue { get; init; }
        public object? ExpectedTrySetClrObject { get; init; }
        #endregion

        #region Calculated Properties
        private object? ActualTrySetClrObject { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            base.Arrange();

            this.WriteLine($"ClrObject: {this.ClrObject.SafeToString()}");
            this.WriteLine($"ClrValue:  {this.ClrValue.SafeToString()}");
            this.WriteLine();
            this.WriteLine($"ExpectedTrySetSuccess:   {this.ExpectedTrySetSuccess.SafeToString()}");
            this.WriteLine($"ExpectedTrySetClrObject: {this.ExpectedTrySetClrObject.SafeToString()}");
        }

        protected override void Act()
        {
            this.ActualTrySetSuccess = this.ApiProperty!.TrySetValue(this.ClrObject, this.ClrValue);
            this.ActualTrySetClrObject = this.ClrObject;

            this.WriteLine();
            this.WriteLine($"ActualTrySetSuccess:   {this.ActualTrySetSuccess.SafeToString()}");
            this.WriteLine($"ActualTrySetClrObject: {this.ActualTrySetClrObject.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualTrySetSuccess.Should().Be(this.ExpectedTrySetSuccess);
            if (this.ExpectedTrySetSuccess)
            {
                this.ActualTrySetClrObject.Should().BeEquivalentTo(this.ExpectedTrySetClrObject);
            }
            else
            {
                this.ActualTrySetClrObject.Should().BeEquivalentTo(this.ClrObject);
            }
        }
        #endregion
    }
    #endregion

    #region Test Data
    public const string TestGuidString = "86d5d1a9-ec14-4730-8d9a-41812e5a117a";
    public static Guid TestGuid { get; } = Guid.Parse(TestGuidString);

    public const string TestUlidString = "46TQ8TKV0M8WR8V6J1G4Q5M4BT";
    public static Ulid TestUlid { get; } = Ulid.Parse(TestUlidString);
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TryGetGenericTheoryData =>
    [
        // Required Properties
        new TryGetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns failure for null object",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = null,
            ExpectedSuccess = false
        },

        new TryGetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for required string property",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = "Alice"
        },

        new TryGetGenericTest<ScalarsOnly, long>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredNumber)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for required long property",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = 123
        },

        new TryGetGenericTest<ScalarsOnly, bool>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredPredicate)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for required bool property",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = true
        },

        // Optional Fields
        new TryGetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for optional string field for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ExpectedSuccess = true,
            ExpectedClrValue = "Bob"
        },

        new TryGetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for optional string field for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },

        new TryGetGenericTest<ScalarsOnly, long?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for optional long field for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ExpectedSuccess = true,
            ExpectedClrValue = 42
        },

        new TryGetGenericTest<ScalarsOnly, long?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for optional long field for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },

        new TryGetGenericTest<ScalarsOnly, bool?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for optional bool field for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ExpectedSuccess = true,
            ExpectedClrValue = false
        },

        new TryGetGenericTest<ScalarsOnly, bool?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for optional bool field for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },

        // Required Properties With Coercion
        new TryGetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredNumber)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for required long property with coercion to string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = "123"
        },

        new TryGetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredPredicate)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for required bool property with coercion to string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = "true"
        },

        // Optional Fields With Coercion
        new TryGetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for optional long field for non-null value with coercion to string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ExpectedSuccess = true,
            ExpectedClrValue = "42"
        },

        new TryGetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for optional long field for null value with coercion to string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },

        new TryGetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for optional bool field for non-null value with coercion to string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ExpectedSuccess = true,
            ExpectedClrValue = "false"
        },

        new TryGetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TryGetValue)}<TObject,TValue> returns success for optional bool field for null value with coercion to string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetNonGenericTheoryData =>
    [
        // Required Properties
        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} {nameof(ApiProperty.TryGetValue)} returns failure for null object",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = null,
            ExpectedSuccess = false
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} {nameof(ApiProperty.TryGetValue)} returns success for required string property",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = "Alice"
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredNumber)} {nameof(ApiProperty.TryGetValue)} returns success for required long property",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = 123
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredPredicate)} {nameof(ApiProperty.TryGetValue)} returns success for required bool property",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ExpectedSuccess = true,
            ExpectedClrValue = true
        },

        // Optional Fields
        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} {nameof(ApiProperty.TryGetValue)} returns success for optional string field for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ExpectedSuccess = true,
            ExpectedClrValue = "Bob"
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} {nameof(ApiProperty.TryGetValue)} returns success for optional string field for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TryGetValue)} returns success for optional long field for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ExpectedSuccess = true,
            ExpectedClrValue = 42
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TryGetValue)} returns success for optional long field for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TryGetValue)} returns success for optional bool field for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ExpectedSuccess = true,
            ExpectedClrValue = false
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TryGetValue)} returns success for optional bool field for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = null },
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },

        // Required Properties With Coercion
        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredNumber)} {nameof(ApiProperty.TryGetValue)} returns success for required long property with coercion to string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValueType = typeof(string),
            ExpectedSuccess = true,
            ExpectedClrValue = "123"
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredPredicate)} {nameof(ApiProperty.TryGetValue)} returns success for required bool property with coercion to string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValueType = typeof(string),
            ExpectedSuccess = true,
            ExpectedClrValue = "true"
        },

        // Optional Fields With Coercion
        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TryGetValue)} returns success for optional long field for non-null value with coercion to string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ClrValueType = typeof(string),
            ExpectedSuccess = true,
            ExpectedClrValue = "42"
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TryGetValue)} returns success for optional long field for null value with coercion to string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = null },
            ClrValueType = typeof(string),
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TryGetValue)} returns success for optional bool field for non-null value with coercion to string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ClrValueType = typeof(string),
            ExpectedSuccess = true,
            ExpectedClrValue = "false"
        },

        new TryGetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TryGetValue)} returns success for optional bool field for null value with coercion to string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = null },
            ClrValueType = typeof(string),
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TrySetGenericTheoryData =>
    [
        // Required Properties
        new TrySetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for required string property for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = "Bob",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Bob", 123, true),
        },

        new TrySetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for required string property for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly(null!, 123, true),
        },

        new TrySetGenericTest<ScalarsOnly, long>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredNumber)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for required long property",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = 42,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 42, true),
        },

        new TrySetGenericTest<ScalarsOnly, bool>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredPredicate)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for required bool property",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = false,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, false),
        },

        // Optional Fields
        new TrySetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for optional string field for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ClrValue = "Charlie",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Charlie" },
        },

        new TrySetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for optional string field for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = null },
        },

        new TrySetGenericTest<ScalarsOnly, long?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for optional long field for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ClrValue = 100,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 100 },
        },

        new TrySetGenericTest<ScalarsOnly, long?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for optional long field for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = null },
        },

        new TrySetGenericTest<ScalarsOnly, bool?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for optional bool field for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ClrValue = true,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = true },
        },

        new TrySetGenericTest<ScalarsOnly, bool?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for optional bool field for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = null },
        },

        // Required Properties With Coercion
        new TrySetGenericTest<ScalarsOnly, long>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for required string property for non-null value with coercion from long",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = 42,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("42", 123, true),
        },

        new TrySetGenericTest<ScalarsOnly, long?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for required string property for null value with coercion from nullable long",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly(null!, 123, true),
        },

        new TrySetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredNumber)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for required long property with coercion from string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = "42",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 42, true),
        },

        new TrySetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredPredicate)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for required bool property with coercion from string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = "false",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, false),
        },

        // Optional Fields With Coercion
        new TrySetGenericTest<ScalarsOnly, Ulid?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for optional string field for non-null value with coercion from nullable Ulid",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ClrValue = TestUlid,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = TestUlidString },
        },

        new TrySetGenericTest<ScalarsOnly, Ulid?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for optional string field for null value with coercion from nullable Ulid",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = null },
        },

        new TrySetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for optional long field for non-null value with coercion from string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ClrValue = "100",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 100 },
        },

        new TrySetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for optional long field for null value with coercion from string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = null },
        },

        new TrySetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for optional bool field for non-null value with coercion from string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ClrValue = "true",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = true },
        },

        new TrySetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TrySetValue)}<TObject,TValue> returns success for optional bool field for null value with coercion from string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = null },
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TrySetByRefGenericTheoryData =>
    [
        // Required Fields
        new TrySetByRefGenericTest<Point, long>
        {
            Name = $"{nameof(Point)}:{nameof(Point.X)} {nameof(ApiProperty.TrySetValueByRef)}<TObject,TValue> returns success for required long field",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Point),
            ApiPropertyName = nameof(Point.X),
            ClrObject = new Point { X = 1, Y = 2 },
            ClrValue = 10,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new Point { X = 10, Y = 2 },
        },

        // Required Fields With Coercion
        new TrySetByRefGenericTest<Point, string>
        {
            Name = $"{nameof(Point)}:{nameof(Point.X)} {nameof(ApiProperty.TrySetValueByRef)}<TObject,TValue> returns success for required long field with coercion from string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Point),
            ApiPropertyName = nameof(Point.X),
            ClrObject = new Point { X = 1, Y = 2 },
            ClrValue = "10",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new Point { X = 10, Y = 2 },
        },

        // Required Properties
        new TrySetByRefGenericTest<Point, long>
        {
            Name = $"{nameof(Point)}:{nameof(Point.Y)} {nameof(ApiProperty.TrySetValueByRef)}<TObject,TValue> returns success for required long property",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Point),
            ApiPropertyName = nameof(Point.Y),
            ClrObject = new Point { X = 1, Y = 2 },
            ClrValue = 20,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new Point { X = 1, Y = 20 },
        },

        // Required Properties With Coercion
        new TrySetByRefGenericTest<Point, string>
        {
            Name = $"{nameof(Point)}:{nameof(Point.Y)} {nameof(ApiProperty.TrySetValueByRef)}<TObject,TValue> returns success for required long property with coercion from string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Point),
            ApiPropertyName = nameof(Point.Y),
            ClrObject = new Point { X = 1, Y = 2 },
            ClrValue = "20",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new Point { X = 1, Y = 20 },
        },

        // Optional Properties
        new TrySetByRefGenericTest<Point, string?>
        {
            Name = $"{nameof(Point)}:{nameof(Point.Note)} {nameof(ApiProperty.TrySetValueByRef)}<TObject,TValue> returns success for optional string field for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Point),
            ApiPropertyName = nameof(Point.Note),
            ClrObject = new Point { X = 1, Y = 2, Note = "Alice" },
            ClrValue = "Bob",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new Point { X = 1, Y = 2, Note = "Bob" },
        },

        new TrySetByRefGenericTest<Point, string?>
        {
            Name = $"{nameof(Point)}:{nameof(Point.Note)} {nameof(ApiProperty.TrySetValueByRef)}<TObject,TValue> returns success for optional string field for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Point),
            ApiPropertyName = nameof(Point.Note),
            ClrObject = new Point { X = 1, Y = 2, Note = "Alice" },
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new Point { X = 1, Y = 2, Note = null },
        },

        // Optional Properties With Coercion
        new TrySetByRefGenericTest<Point, Guid?>
        {
            Name = $"{nameof(Point)}:{nameof(Point.Note)} {nameof(ApiProperty.TrySetValueByRef)}<TObject,TValue> returns success for optional string field for non-null value with coercion from nullable Guid",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Point),
            ApiPropertyName = nameof(Point.Note),
            ClrObject = new Point { X = 1, Y = 2, Note = "Alice" },
            ClrValue = TestGuid,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new Point { X = 1, Y = 2, Note = TestGuidString },
        },

        new TrySetByRefGenericTest<Point, Guid?>
        {
            Name = $"{nameof(Point)}:{nameof(Point.Note)} {nameof(ApiProperty.TrySetValueByRef)}<TObject,TValue> returns success for optional string field for null value with coercion from nullable Guid",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(Point),
            ApiPropertyName = nameof(Point.Note),
            ClrObject = new Point { X = 1, Y = 2, Note = "Alice" },
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new Point { X = 1, Y = 2, Note = null },
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TrySetNonGenericTheoryData =>
    [
        // Required Properties
        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} {nameof(ApiProperty.TrySetValue)} returns success for required string property for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = "Bob",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Bob", 123, true),
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} {nameof(ApiProperty.TrySetValue)} returns success for required string property for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredName),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly(null!, 123, true),
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredNumber)} {nameof(ApiProperty.TrySetValue)} returns success for required long property",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = 42,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 42, true),
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredPredicate)} {nameof(ApiProperty.TrySetValue)} returns success for required bool property",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = false,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, false),
        },

        // Optional Fields
        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} {nameof(ApiProperty.TrySetValue)} returns success for optional string field for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ClrValue = "Charlie",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Charlie" },
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} {nameof(ApiProperty.TrySetValue)} returns success for optional string field for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalName),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = null },
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TrySetValue)} returns success for optional long field for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ClrValue = 100,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 100 },
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TrySetValue)} returns success for optional long field for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = null },
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TrySetValue)} returns success for optional bool field for non-null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ClrValue = true,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = true }
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TrySetValue)} returns success for optional bool field for null value",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ClrValue = null,
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = null }
        },

        // Required Properties With Coercion
        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredNumber)} {nameof(ApiProperty.TrySetValue)} returns success for required long property with coercion from string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = "42",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 42, true),
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredPredicate)} {nameof(ApiProperty.TrySetValue)} returns success for required bool property with coercion from string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrValue = "false",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, false),
        },

        // Optional Fields With Coercion
        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalNumber)} {nameof(ApiProperty.TrySetValue)} returns success for optional long field for non-null value with coercion from string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 42 },
            ClrValue = "100",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 100 },
        },

        new TrySetNonGenericTest
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalPredicate)} {nameof(ApiProperty.TrySetValue)} returns success for optional bool field for non-null value with coercion from string",
            ApiSchemaKind = ApiSchemaKind.Simple,
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalPredicate),
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = false },
            ClrValue = "true",
            ExpectedTrySetSuccess = true,
            ExpectedTrySetClrObject = new ScalarsOnly("Alice", 123, true) { OptionalPredicate = true }
        },
    ];
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
    [MemberData(nameof(TrySetByRefGenericTheoryData))]
    public void TrySetByRefGeneric(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TrySetNonGenericTheoryData))]
    public void TrySetNonGeneric(IXUnitTest test) => test.Execute(this);
    #endregion
}
