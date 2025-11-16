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
    #region Local Test Types
    // Public struct required so it can be used as a generic type argument in public test classes
    public struct TestPoint
    {
        public int X; // field
        public int Y { get; set; } // property
    }
    #endregion

    #region TryGet Tests
    public class TryGetGenericTest<TObject, TValue> : XUnitTest
    {
        #region User Supplied Properties
        public TObject ClrObject { get; init; } = default!;
        public string ClrName { get; init; } = default!;
        public bool ExpectedSuccess { get; init; }
        public TValue? ExpectedClrValue { get; init; } = default!;
        #endregion

        #region Calculated Properties
        private ApiProperty ApiProperty { get; set; } = default!;
        private bool ActualSuccess { get; set; }
        private TValue? ActualClrValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiName = this.ClrName;
            var apiTypeExpression = ApiTypeExpression.ClrRef<TValue>();
            var clrName = this.ClrName;

            this.ApiProperty = new ApiProperty
            (
                apiName,
                apiTypeExpression,
                ApiTypeModifiers.None, // Need to address modifiers in near future
                clrName
            );
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

    public class TryGetNonGenericTest : XUnitTest
    {
        #region User Supplied Properties
        public object ClrObject { get; init; } = default!;
        public string ClrName { get; init; } = default!;
        public Type ClrValueType { get; init; } = default!;
        public bool ExpectedSuccess { get; init; }
        public object? ExpectedClrValue { get; init; } = default!;
        #endregion

        #region Calculated Properties
        private ApiProperty ApiProperty { get; set; } = default!;
        private bool ActualSuccess { get; set; }
        private object? ActualClrValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiName = this.ClrName;
            var apiTypeExpression = new ApiTypeExpression(this.ClrValueType);
            var clrName = this.ClrName;

            this.ApiProperty = new ApiProperty
            (
                apiName,
                apiTypeExpression,
                ApiTypeModifiers.None, // Need to address modifiers in near future
                clrName
            );
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




    public class TryGetGenericTestOld<TObject, TValue> : XUnitTest
    {
        #region User Supplied Properties
        public string? ClrName { get; init; }
        public TObject Target { get; init; } = default!;
        public bool? ExpectedFound { get; init; }
        public TValue? ExpectedValue { get; init; } = default!;
        #endregion

        #region Calculated Properties
        private ApiProperty? ApiProperty { get; set; }
        private bool? ActualFound { get; set; }
        private TValue? ActualValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ClrName.Should().NotBeNullOrWhiteSpace();
            this.Target.Should().NotBeNull();

            var targetType = this.Target!.GetType();
            var clrType = targetType.GetProperty(this!.ClrName!)?.PropertyType
                        ?? targetType.GetField(this!.ClrName!)?.FieldType
                        ?? typeof(object);

            this.ApiProperty = new ApiProperty(this!.ClrName!, new ApiTypeExpression(clrType), ApiTypeModifiers.None, this.ClrName!);
        }

        protected override void Act()
        {
            this.ActualFound = this.ApiProperty!.TryGetValue<TObject, TValue>(this.Target!, out var value);
            this.ActualValue = value;
        }

        protected override void Assert()
        {
            this.ActualFound.Should().Be(this!.ExpectedFound);
            if (this.ExpectedFound == true)
            {
                this.ActualValue.Should().Be(this!.ExpectedValue);
            }
        }
        #endregion
    }
    #endregion

    #region TrySet Tests
    public class TrySetObjectTest : XUnitTest
    {
        #region User Supplied Properties
        public string? ClrName { get; init; }
        public object? Target { get; init; }
        public object? InputValue { get; init; }
        public bool? ExpectedSuccess { get; init; }
        public object? ExpectedValue { get; init; }
        #endregion

        #region Calculated Properties
        private ApiProperty? ApiProperty { get; set; }
        private bool? ActualSuccess { get; set; }
        private object? ActualValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ClrName.Should().NotBeNullOrWhiteSpace();
            this.Target.Should().NotBeNull();

            var targetType = this.Target!.GetType();
            var clrType = targetType.GetProperty(this!.ClrName!)?.PropertyType
                        ?? targetType.GetField(this!.ClrName!)?.FieldType
                        ?? typeof(object);

            this.ApiProperty = new ApiProperty(this!.ClrName!, new ApiTypeExpression(clrType), ApiTypeModifiers.None, this.ClrName!);
        }

        protected override void Act()
        {
            this.ActualSuccess = this.ApiProperty!.TrySetValue(this!.Target!, this!.InputValue);
            // Read back via reflection to verify mutation
            this.ActualValue = this.Target!.GetType().GetProperty(this!.ClrName!)?.GetValue(this.Target!)
                             ?? this.Target!.GetType().GetField(this!.ClrName!)?.GetValue(this.Target!);
        }

        protected override void Assert()
        {
            this.ActualSuccess.Should().Be(this!.ExpectedSuccess);
            if (this.ExpectedSuccess == true)
            {
                this.ActualValue.Should().Be(this!.ExpectedValue);
            }
        }
        #endregion
    }

    public class TrySetGenericTest<TObject, TValue> : XUnitTest
    {
        #region User Supplied Properties
        public string? ClrName { get; init; }
        public TObject Target { get; init; } = default!;
        public TValue InputValue { get; init; } = default!;
        public bool? ExpectedSuccess { get; init; }
        public object? ExpectedValue { get; init; }
        #endregion

        #region Calculated Properties
        private ApiProperty? ApiProperty { get; set; }
        private bool? ActualSuccess { get; set; }
        private object? ActualValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ClrName.Should().NotBeNullOrWhiteSpace();
            this.Target.Should().NotBeNull();

            var targetType = this.Target!.GetType();
            var clrType = targetType.GetProperty(this!.ClrName!)?.PropertyType
                        ?? targetType.GetField(this!.ClrName!)?.FieldType
                        ?? typeof(object);

            this.ApiProperty = new ApiProperty(this!.ClrName!, new ApiTypeExpression(clrType), ApiTypeModifiers.None, this.ClrName!);
        }

        protected override void Act()
        {
            this.ActualSuccess = this.ApiProperty!.TrySetValue<TObject, TValue>(this.Target!, this.InputValue);
            // Read back via reflection
            var boxed = (object)this.Target!;
            this.ActualValue = boxed.GetType().GetProperty(this!.ClrName!)?.GetValue(boxed)
                             ?? boxed.GetType().GetField(this!.ClrName!)?.GetValue(boxed);
        }

        protected override void Assert()
        {
            this.ActualSuccess.Should().Be(this!.ExpectedSuccess);
            if (this.ExpectedSuccess == true)
            {
                this.ActualValue.Should().Be(this!.ExpectedValue);
            }
        }
        #endregion
    }

    public class TrySetByRefStructTest<TValue> : XUnitTest
    {
        #region User Supplied Properties
        public string? ClrName { get; init; }
        public TestPoint Target { get; init; }
        public TValue InputValue { get; init; } = default!;
        public int ExpectedX { get; init; }
        public int ExpectedY { get; init; }
        #endregion

        #region Calculated Properties
        private ApiProperty? ApiProperty { get; set; }
        private bool? ActualSuccess { get; set; }
        private TestPoint ActualPoint { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ClrName.Should().NotBeNullOrWhiteSpace();
            var memberType = typeof(TestPoint).GetProperty(this!.ClrName!)?.PropertyType
                           ?? typeof(TestPoint).GetField(this!.ClrName!)?.FieldType
                           ?? typeof(object);

            this.ApiProperty = new ApiProperty(this!.ClrName!, new ApiTypeExpression(memberType), ApiTypeModifiers.None, this.ClrName!);
        }

        protected override void Act()
        {
            var p = this.Target; // local copy
            this.ActualSuccess = this.ApiProperty!.TrySetValueRef<TestPoint, TValue>(ref p, this.InputValue);
            this.ActualPoint = p;
        }

        protected override void Assert()
        {
            this.ActualSuccess.Should().BeTrue();
            this.ActualPoint.X.Should().Be(this.ExpectedX);
            this.ActualPoint.Y.Should().Be(this.ExpectedY);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TryGetGenericTheoryData =>
    [
        new TryGetGenericTest<ScalarsOnly, string>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredName)} TryGetValue<TObject,TValue> returns string value for known non-null reference property",
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrName = nameof(ScalarsOnly.RequiredName),
            ExpectedSuccess = true,
            ExpectedClrValue = "Alice"
        },

        new TryGetGenericTest<ScalarsOnly, long>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredNumber)} TryGetValue<TObject,TValue> returns long value for known value property",
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrName = nameof(ScalarsOnly.RequiredNumber),
            ExpectedSuccess = true,
            ExpectedClrValue = 123
        },

        new TryGetGenericTest<ScalarsOnly, bool>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.RequiredPredicate)} TryGetValue<TObject,TValue> returns bool value for known value property",
            ClrObject = new ScalarsOnly("Alice", 123, true),
            ClrName = nameof(ScalarsOnly.RequiredPredicate),
            ExpectedSuccess = true,
            ExpectedClrValue = true
        },

        new TryGetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} TryGetValue<TObject,TValue> returns string value for known nullable reference property",
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = "Bob" },
            ClrName = nameof(ScalarsOnly.OptionalName),
            ExpectedSuccess = true,
            ExpectedClrValue = "Bob"
        },

        new TryGetGenericTest<ScalarsOnly, string?>
        {
            Name = $"{nameof(ScalarsOnly)}:{nameof(ScalarsOnly.OptionalName)} TryGetValue<TObject,TValue> returns null for known nullable reference property",
            ClrObject = new ScalarsOnly("Alice", 123, true) { OptionalName = null },
            ClrName = nameof(ScalarsOnly.OptionalName),
            ExpectedSuccess = true,
            ExpectedClrValue = null
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetNonGenericTheoryData =>
    [
        new TryGetNonGenericTest
        {
            Name = $"{nameof(Person)}:{nameof(Person.Name)} TryGetValue(object, out object) returns value for known property",
            ClrObject = new Person { Name = "Bob" },
            ClrName = nameof(Person.Name),
            ClrValueType = typeof(string),
            ExpectedSuccess = true,
            ExpectedClrValue = "Bob"
        },

        // object-based getter: unknown property
        // new TryGetObjectTestOld
        // {
        //     Name = "TryGetValue(object, out object) returns false for unknown property",
        //     ClrName = "DoesNotExist",
        //     Target = new Person { Name = "Bob" },
        //     ExpectedFound = false,
        //     ExpectedValue = null
        // },
    ];

    public static TheoryDataRow<IXUnitTest>[] TrySetTheoryData =>
    [
        // object-based setter on reference type
        new TrySetObjectTest
        {
            Name = "TrySetValue(object, object?) sets reference type property",
            ClrName = nameof(Person.Name),
            Target = new Person { Name = "Old" },
            InputValue = "New",
            ExpectedSuccess = true,
            ExpectedValue = "New"
        },

        // generic setter on reference type
        new TrySetGenericTest<Person, string>
        {
            Name = "TrySetValue<TObject,TValue> sets reference type property",
            ClrName = nameof(Person.Name),
            Target = new Person { Name = "Old" },
            InputValue = "New",
            ExpectedSuccess = true,
            ExpectedValue = "New"
        },

        // generic setter on reference type, numeric
        new TrySetGenericTest<ScalarsOnly, long>
        {
            Name = "TrySetValue<TObject,TValue> sets numeric property",
            ClrName = nameof(ScalarsOnly.RequiredNumber),
            Target = new ScalarsOnly("n", 1, true),
            InputValue = 42L,
            ExpectedSuccess = true,
            ExpectedValue = 42L
        },

        // non-ref setter on struct target does not mutate caller (field)
        new TrySetGenericTest<TestPoint, int>
        {
            Name = "TrySetValue<TObject,TValue> on struct target assigns copy (field)",
            ClrName = nameof(TestPoint.X),
            Target = new TestPoint { X = 1, Y = 2 },
            InputValue = 10,
            ExpectedSuccess = true,
            // Reading back from boxed Target reflects original value (no mutation)
            ExpectedValue = 1
        },

        // non-ref setter on struct target does not mutate caller (property)
        new TrySetGenericTest<TestPoint, int>
        {
            Name = "TrySetValue<TObject,TValue> on struct target assigns copy (property)",
            ClrName = nameof(TestPoint.Y),
            Target = new TestPoint { X = 1, Y = 2 },
            InputValue = 20,
            ExpectedSuccess = true,
            ExpectedValue = 2
        },

        // ref setter on struct target mutates caller (field)
        new TrySetByRefStructTest<int>
        {
            Name = "TrySetValueRef mutates struct target (field)",
            ClrName = nameof(TestPoint.X),
            Target = new TestPoint { X = 1, Y = 2 },
            InputValue = 10,
            ExpectedX = 10,
            ExpectedY = 2
        },

        // ref setter on struct target mutates caller (property)
        new TrySetByRefStructTest<int>
        {
            Name = "TrySetValueRef mutates struct target (property)",
            ClrName = nameof(TestPoint.Y),
            Target = new TestPoint { X = 1, Y = 2 },
            InputValue = 20,
            ExpectedX = 1,
            ExpectedY = 20
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

    // [Theory]
    // [MemberData(nameof(TrySetTheoryData))]
    // public void TrySet(IXUnitTest test) => test.Execute(this);
    #endregion
}
