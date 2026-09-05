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

public partial class ApiPropertyTests(ITestOutputHelper output) : XUnitTests(output)
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

}
