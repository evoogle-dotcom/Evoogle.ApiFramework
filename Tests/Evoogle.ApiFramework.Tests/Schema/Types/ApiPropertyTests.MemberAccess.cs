// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema.Types;

public partial class ApiPropertyTests
{
    #region Test Types
    public sealed class AccessorCapabilities
    {
        private long _writeOnly;

        public long ReadOnly => 7;

        public long WriteOnly
        {
            set => _writeOnly = value;
        }

        public long WrittenValue => _writeOnly;
    }

    private enum GetOperation
    {
        ObjectDirect,
        ObjectCoercing,
        GenericDirect,
        GenericCoercing,
        TryObjectDirect,
        TryGenericCoercing,
        TryNullTarget
    }

    private enum SetOperation
    {
        ObjectDirect,
        ObjectCoercing,
        GenericDirect,
        GenericCoercing,
        TryObjectCoercing,
        TryFailedCoercion
    }

    private enum CoerceOperation
    {
        Throwing,
        Try,
        TryFailure
    }
    #endregion

    #region Base Test Classes
    private abstract class MemberAccessTestBase : XUnitTest
    {
        #region User Supplied Properties
        public required string ApiObjectTypeName { get; init; }
        public required string ApiPropertyName { get; init; }
        #endregion

        #region Calculated Properties
        protected ApiProperty? ApiProperty { get; private set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiSchema = BuildTestApiSchema(ApiSchemaKind.Simple);
            var apiObjectType = apiSchema.GetObjectTypeByApiName(this.ApiObjectTypeName)
                ?? throw new InvalidOperationException
                (
                    $"{nameof(ApiObjectType)} '{this.ApiObjectTypeName}' was not found."
                );
            this.ApiProperty = apiObjectType.GetPropertyByApiName(this.ApiPropertyName)
                ?? throw new InvalidOperationException
                (
                    $"{nameof(Types.ApiProperty)} '{this.ApiPropertyName}' was not found."
                );
        }
        #endregion
    }
    #endregion

    #region Metadata Tests
    private sealed class BindingMetadataTest : MemberAccessTestBase
    {
        #region User Supplied Properties
        public required ClrMemberKind ExpectedMemberKind { get; init; }
        #endregion

        #region Calculated Properties
        private object? ActualValue { get; set; }

        private long ActualUpdatedValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            var clrObject = new ScalarsOnly("Alice", 123, true) { OptionalNumber = 123 };
            this.ActualValue = this.ApiProperty!.GetValue(clrObject);
            this.ApiProperty.SetValue(clrObject, 42L);
            this.ActualUpdatedValue = this.ExpectedMemberKind == ClrMemberKind.Property
                ? clrObject.RequiredNumber
                : clrObject.OptionalNumber!.Value;
        }

        protected override void Assert()
        {
            this.ApiProperty!.ClrName.Should().Be(this.ApiPropertyName);
            this.ApiProperty.ClrMemberKind.Should().Be(this.ExpectedMemberKind);
            this.ActualValue.Should().Be(123L);
            this.ActualUpdatedValue.Should().Be(42L);
        }
        #endregion
    }
    #endregion

    #region Get Tests
    private sealed class GetForwarderTest : MemberAccessTestBase
    {
        #region User Supplied Properties
        public required GetOperation Operation { get; init; }
        public required object? ExpectedValue { get; init; }
        public required bool ExpectedSuccess { get; init; }
        #endregion

        #region Calculated Properties
        private object? ActualValue { get; set; }
        private bool ActualSuccess { get; set; }
        #endregion

        protected override void Act()
        {
            var clrObject = new ScalarsOnly("Alice", 123, true);
            switch (this.Operation)
            {
                case GetOperation.ObjectDirect:
                    this.ActualValue = this.ApiProperty!.GetValue(clrObject);
                    this.ActualSuccess = true;
                    break;
                case GetOperation.ObjectCoercing:
                    this.ActualValue = this.ApiProperty!.GetValue(clrObject, typeof(string));
                    this.ActualSuccess = true;
                    break;
                case GetOperation.GenericDirect:
                    this.ActualValue = this.ApiProperty!.GetValue<ScalarsOnly, long>(clrObject);
                    this.ActualSuccess = true;
                    break;
                case GetOperation.GenericCoercing:
                    this.ActualValue = this.ApiProperty!.GetValue<ScalarsOnly, string>(clrObject);
                    this.ActualSuccess = true;
                    break;
                case GetOperation.TryObjectDirect:
                    this.ActualSuccess = this.ApiProperty!.TryGetValue(clrObject, out var objectValue);
                    this.ActualValue = objectValue;
                    break;
                case GetOperation.TryGenericCoercing:
                    this.ActualSuccess = this.ApiProperty!
                        .TryGetValue<ScalarsOnly, string>(clrObject, out var stringValue);
                    this.ActualValue = stringValue;
                    break;
                case GetOperation.TryNullTarget:
                    this.ActualSuccess = this.ApiProperty!.TryGetValue(null, out var value);
                    this.ActualValue = value;
                    break;
            }
        }

        protected override void Assert()
        {
            this.ActualSuccess.Should().Be(this.ExpectedSuccess);
            this.ActualValue.Should().Be(this.ExpectedValue);
        }
    }
    #endregion

    #region Set Tests
    private sealed class SetForwarderTest : MemberAccessTestBase
    {
        #region User Supplied Properties
        public required SetOperation Operation { get; init; }
        public required long ExpectedValue { get; init; }
        public required bool ExpectedSuccess { get; init; }
        #endregion

        #region Calculated Properties
        private long ActualValue { get; set; }
        private bool ActualSuccess { get; set; }
        #endregion

        protected override void Act()
        {
            var clrObject = new ScalarsOnly("Alice", 123, true);
            switch (this.Operation)
            {
                case SetOperation.ObjectDirect:
                    this.ApiProperty!.SetValue(clrObject, 42L);
                    this.ActualSuccess = true;
                    break;
                case SetOperation.ObjectCoercing:
                    this.ApiProperty!.SetValue(clrObject, "42");
                    this.ActualSuccess = true;
                    break;
                case SetOperation.GenericDirect:
                    this.ApiProperty!.SetValue(clrObject, 42L);
                    this.ActualSuccess = true;
                    break;
                case SetOperation.GenericCoercing:
                    this.ApiProperty!.SetValue(clrObject, "42");
                    this.ActualSuccess = true;
                    break;
                case SetOperation.TryObjectCoercing:
                    this.ActualSuccess = this.ApiProperty!.TrySetValue(clrObject, "42");
                    break;
                case SetOperation.TryFailedCoercion:
                    this.ActualSuccess = this.ApiProperty!.TrySetValue(clrObject, "invalid");
                    break;
            }

            this.ActualValue = clrObject.RequiredNumber;
        }

        protected override void Assert()
        {
            this.ActualSuccess.Should().Be(this.ExpectedSuccess);
            this.ActualValue.Should().Be(this.ExpectedValue);
        }
    }

    private sealed class ByRefSetForwarderTest : MemberAccessTestBase
    {
        #region User Supplied Properties
        public required bool UseTryMethod { get; init; }
        public required bool UseCoercion { get; init; }
        #endregion

        #region Calculated Properties
        private Point ActualPoint { get; set; }
        private bool ActualSuccess { get; set; }
        #endregion

        protected override void Act()
        {
            var point = new Point { X = 1, Y = 2 };
            if (this.UseTryMethod)
            {
                this.ActualSuccess = this.UseCoercion
                    ? this.ApiProperty!.TrySetValueByRef(ref point, "10")
                    : this.ApiProperty!.TrySetValueByRef(ref point, 10L);
            }
            else
            {
                if (this.UseCoercion)
                {
                    this.ApiProperty!.SetValueByRef(ref point, "10");
                }
                else
                {
                    this.ApiProperty!.SetValueByRef(ref point, 10L);
                }

                this.ActualSuccess = true;
            }

            this.ActualPoint = point;
        }

        protected override void Assert()
        {
            this.ActualSuccess.Should().BeTrue();
            this.ActualPoint.X.Should().Be(10);
            this.ActualPoint.Y.Should().Be(2);
        }
    }
    #endregion

    #region Error and Coercion Tests
    private sealed class ExceptionTranslationTest : MemberAccessTestBase
    {
        #region Calculated Properties
        private Exception? ActualException { get; set; }
        #endregion

        protected override void Act()
        {
            this.ActualException = Record.Exception(() => this.ApiProperty!.GetValue(new object()));
        }

        protected override void Assert()
        {
            this.ActualException.Should().BeOfType<ApiSchemaException>().Which.InnerException.Should().BeOfType<MemberAccess.MemberAccessException>();
            this.ActualException.Message.Should().Contain(this.ApiPropertyName);
        }
    }

    private sealed class CoerceForwarderTest : MemberAccessTestBase
    {
        #region User Supplied Properties
        public required CoerceOperation Operation { get; init; }
        public required bool ExpectedSuccess { get; init; }
        #endregion

        #region Calculated Properties
        private object? ActualValue { get; set; }
        private bool ActualSuccess { get; set; }
        #endregion

        protected override void Act()
        {
            switch (this.Operation)
            {
                case CoerceOperation.Throwing:
                    this.ActualValue = this.ApiProperty!.CoerceValue("42");
                    this.ActualSuccess = true;
                    break;
                case CoerceOperation.Try:
                    this.ActualSuccess = this.ApiProperty!.TryCoerceValue("42", out var value);
                    this.ActualValue = value;
                    break;
                case CoerceOperation.TryFailure:
                    this.ActualSuccess = this.ApiProperty!.TryCoerceValue("invalid", out var failedValue);
                    this.ActualValue = failedValue;
                    break;
            }
        }

        protected override void Assert()
        {
            this.ActualSuccess.Should().Be(this.ExpectedSuccess);
            this.ActualValue.Should().Be(this.ExpectedSuccess ? 42L : null);
        }
    }

    private sealed class CapabilityTest : XUnitTest
    {
        #region User Supplied Properties
        public required string MemberName { get; init; }
        public required bool ExpectedCanRead { get; init; }
        #endregion

        #region Calculated Properties
        private ApiProperty? ApiProperty { get; set; }
        private Exception? ActualException { get; set; }
        private long ActualValue { get; set; }
        #endregion

        protected override void Arrange()
        {
            this.ApiProperty = CreateCapabilityProperty(this.MemberName);
        }

        protected override void Act()
        {
            var clrObject = new AccessorCapabilities();
            if (this.ExpectedCanRead)
            {
                this.ActualValue = (long)this.ApiProperty!.GetValue(clrObject)!;
                this.ActualException = Record.Exception(() => this.ApiProperty.SetValue(clrObject, 9L));
            }
            else
            {
                this.ApiProperty!.SetValue(clrObject, 9L);
                this.ActualValue = clrObject.WrittenValue;
                this.ActualException = Record.Exception(() => this.ApiProperty.GetValue(clrObject));
            }
        }

        protected override void Assert()
        {
            this.ActualValue.Should().Be(this.ExpectedCanRead ? 7L : 9L);
            this.ActualException.Should().BeOfType<ApiSchemaException>();
        }
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] MemberAccessMetadataTheoryData =>
    [
        new BindingMetadataTest
        {
            Name = $"{nameof(ApiProperty)} uses property binding metadata",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            ExpectedMemberKind = ClrMemberKind.Property,
        },
        new BindingMetadataTest
        {
            Name = $"{nameof(ApiProperty)} uses field binding metadata",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.OptionalNumber),
            ExpectedMemberKind = ClrMemberKind.Field,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] MemberAccessGetTheoryData =>
    [
        new GetForwarderTest
        {
            Name = "Object direct get",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = GetOperation.ObjectDirect,
            ExpectedValue = 123L,
            ExpectedSuccess = true,
        },
        new GetForwarderTest
        {
            Name = "Object coercing get",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = GetOperation.ObjectCoercing,
            ExpectedValue = "123",
            ExpectedSuccess = true,
        },
        new GetForwarderTest
        {
            Name = "Generic direct get",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = GetOperation.GenericDirect,
            ExpectedValue = 123L,
            ExpectedSuccess = true,
        },
        new GetForwarderTest
        {
            Name = "Generic coercing get",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = GetOperation.GenericCoercing,
            ExpectedValue = "123",
            ExpectedSuccess = true,
        },
        new GetForwarderTest
        {
            Name = "Object Try get",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = GetOperation.TryObjectDirect,
            ExpectedValue = 123L,
            ExpectedSuccess = true,
        },
        new GetForwarderTest
        {
            Name = "Generic coercing Try get",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = GetOperation.TryGenericCoercing,
            ExpectedValue = "123",
            ExpectedSuccess = true,
        },
        new GetForwarderTest
        {
            Name = "Try get rejects null target",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = GetOperation.TryNullTarget,
            ExpectedValue = null,
            ExpectedSuccess = false,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] MemberAccessSetTheoryData =>
    [
        new SetForwarderTest
        {
            Name = "Object direct set",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = SetOperation.ObjectDirect,
            ExpectedValue = 42L,
            ExpectedSuccess = true,
        },
        new SetForwarderTest
        {
            Name = "Object coercing set",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = SetOperation.ObjectCoercing,
            ExpectedValue = 42L,
            ExpectedSuccess = true,
        },
        new SetForwarderTest
        {
            Name = "Generic direct set",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = SetOperation.GenericDirect,
            ExpectedValue = 42L,
            ExpectedSuccess = true,
        },
        new SetForwarderTest
        {
            Name = "Generic coercing set",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = SetOperation.GenericCoercing,
            ExpectedValue = 42L,
            ExpectedSuccess = true,
        },
        new SetForwarderTest
        {
            Name = "Object coercing Try set",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = SetOperation.TryObjectCoercing,
            ExpectedValue = 42L,
            ExpectedSuccess = true,
        },
        new SetForwarderTest
        {
            Name = "Try set preserves value after failed coercion",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = SetOperation.TryFailedCoercion,
            ExpectedValue = 123L,
            ExpectedSuccess = false,
        },
        new ByRefSetForwarderTest
        {
            Name = "By-reference direct set",
            ApiObjectTypeName = nameof(Point),
            ApiPropertyName = nameof(Point.X),
            UseTryMethod = false,
            UseCoercion = false,
        },
        new ByRefSetForwarderTest
        {
            Name = "By-reference coercing Try set",
            ApiObjectTypeName = nameof(Point),
            ApiPropertyName = nameof(Point.X),
            UseTryMethod = true,
            UseCoercion = true,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] MemberAccessErrorsAndCoercionTheoryData =>
    [
        new ExceptionTranslationTest
        {
            Name = $"{nameof(ApiSchemaException)} retains the Core exception",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
        },
        new CoerceForwarderTest
        {
            Name = $"{nameof(ApiProperty.CoerceValue)} uses schema coercion",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = CoerceOperation.Throwing,
            ExpectedSuccess = true,
        },
        new CoerceForwarderTest
        {
            Name = $"{nameof(ApiProperty.TryCoerceValue)} uses schema coercion",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = CoerceOperation.Try,
            ExpectedSuccess = true,
        },
        new CoerceForwarderTest
        {
            Name = $"{nameof(ApiProperty.TryCoerceValue)} returns false on failure",
            ApiObjectTypeName = nameof(ScalarsOnly),
            ApiPropertyName = nameof(ScalarsOnly.RequiredNumber),
            Operation = CoerceOperation.TryFailure,
            ExpectedSuccess = false,
        },
        new CapabilityTest
        {
            Name = "Read-only property exposes only its getter",
            MemberName = nameof(AccessorCapabilities.ReadOnly),
            ExpectedCanRead = true,
        },
        new CapabilityTest
        {
            Name = "Write-only property exposes only its setter",
            MemberName = nameof(AccessorCapabilities.WriteOnly),
            ExpectedCanRead = false,
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(MemberAccessMetadataTheoryData))]
    public void MemberAccessMetadata(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(MemberAccessGetTheoryData))]
    public void MemberAccessGet(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(MemberAccessSetTheoryData))]
    public void MemberAccessSet(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(MemberAccessErrorsAndCoercionTheoryData))]
    public void MemberAccessErrorsAndCoercion(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Helper Methods
    private static ApiProperty CreateCapabilityProperty(string memberName)
    {
        var sourceJson = $$"""
        {
            "ApiName": "CapabilitySchema",
            "ApiScalarTypes": [
                {
                    "ApiKind": "Scalar",
                    "ApiName": "Int64",
                    "ClrType": "System.Int64, System.Private.CoreLib"
                }
            ],
            "ApiEnumTypes": [],
            "ApiObjectTypes": [
                {
                    "ApiKind": "Object",
                    "ApiName": "{{nameof(AccessorCapabilities)}}",
                    "ApiProperties": [
                        {
                            "ApiName": "{{memberName}}",
                            "ApiType": {
                                "ApiKind": "Scalar",
                                "ApiName": "Int64"
                            },
                            "ApiTypeModifiers": "Required",
                            "ClrName": "{{memberName}}",
                            "ClrMemberKind": "Property"
                        }
                    ],
                    "ClrType": "{{typeof(AccessorCapabilities).AssemblyQualifiedName}}"
                }
            ]
        }
        """;

        var apiSchema = JsonSerializer.Deserialize<ApiSchema>(sourceJson) ?? throw new InvalidOperationException($"{nameof(ApiSchema)} deserialization failed.");
        var apiObjectType = apiSchema.GetObjectTypeByApiName(nameof(AccessorCapabilities)) ?? throw new InvalidOperationException($"{nameof(ApiObjectType)} lookup failed.");

        return apiObjectType.GetPropertyByApiName(memberName) ?? throw new InvalidOperationException($"{nameof(ApiProperty)} lookup failed.");
    }
    #endregion
}
