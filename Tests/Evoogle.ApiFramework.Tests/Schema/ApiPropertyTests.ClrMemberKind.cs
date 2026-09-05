// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiPropertyTests
{
    #region Test Types
    public class ClrMemberKindBase
    {
        public int Value { get; set; }
    }

    public class ClrMemberKindDerived : ClrMemberKindBase
    {
        public new int Value;
    }
    #endregion

    #region Test Classes
    private sealed class NonGenericFieldBindingTest : XUnitTest
    {
        #region Calculated Properties
        private ApiProperty? ApiProperty { get; set; }

        private ClrMemberKindDerived? ClrObject { get; set; }

        private object? ActualClrValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.ApiProperty = CreateApiProperty
            (
                nameof(NonGenericFieldBindingTest),
                ClrMemberKind.Field
            );
            this.ClrObject = new ClrMemberKindDerived { Value = 10 };
            ((ClrMemberKindBase)this.ClrObject).Value = 20;
        }

        protected override void Act()
        {
            this.ActualClrValue = this.ApiProperty!.GetValue(this.ClrObject!);
            this.ApiProperty.SetValue(this.ClrObject!, 30);
        }

        protected override void Assert()
        {
            this.ActualClrValue.Should().Be(10);
            this.ClrObject!.Value.Should().Be(30);
            ((ClrMemberKindBase)this.ClrObject).Value.Should().Be(20);
        }
        #endregion
    }

    private sealed class GenericCacheBindingTest : XUnitTest
    {
        #region Calculated Properties
        private ApiProperty? PropertyApiProperty { get; set; }

        private ApiProperty? FieldApiProperty { get; set; }

        private ClrMemberKindDerived? ClrObject { get; set; }

        private int ActualPropertyValue { get; set; }

        private int ActualFieldValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.PropertyApiProperty = CreateApiProperty
            (
                nameof(GenericCacheBindingTest) + nameof(ClrMemberKind.Property),
                ClrMemberKind.Property
            );
            this.FieldApiProperty = CreateApiProperty
            (
                nameof(GenericCacheBindingTest) + nameof(ClrMemberKind.Field),
                ClrMemberKind.Field
            );
            this.ClrObject = new ClrMemberKindDerived { Value = 10 };
            ((ClrMemberKindBase)this.ClrObject).Value = 20;
        }

        protected override void Act()
        {
            this.ActualPropertyValue = this.PropertyApiProperty!
                .GetValue<ClrMemberKindDerived, int>(this.ClrObject!);
            this.ActualFieldValue = this.FieldApiProperty!
                .GetValue<ClrMemberKindDerived, int>(this.ClrObject!);

            this.PropertyApiProperty.SetValue<ClrMemberKindDerived, int>(this.ClrObject!, 30);
            this.FieldApiProperty.SetValue<ClrMemberKindDerived, int>(this.ClrObject!, 40);
        }

        protected override void Assert()
        {
            this.ActualPropertyValue.Should().Be(20);
            this.ActualFieldValue.Should().Be(10);
            ((ClrMemberKindBase)this.ClrObject!).Value.Should().Be(30);
            this.ClrObject.Value.Should().Be(40);
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] ClrMemberKindBindingTheoryData =>
    [
        new NonGenericFieldBindingTest
        {
            Name = $"{nameof(ApiProperty)} {nameof(ClrMemberKind.Field)} binding accesses a hidden field",
        },
        new GenericCacheBindingTest
        {
            Name = $"{nameof(ApiProperty)} generic accessors cache {nameof(ClrMemberKind.Property)} and " +
                $"{nameof(ClrMemberKind.Field)} separately",
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(ClrMemberKindBindingTheoryData))]
    public void ClrMemberKindBinding(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Helper Methods
    private static ApiProperty CreateApiProperty(string apiSchemaName, ClrMemberKind clrMemberKind)
    {
        var sourceJson = $$"""
        {
            "ApiName": "{{apiSchemaName}}",
            "ApiScalarTypes": [
                {
                    "ApiKind": "Scalar",
                    "ApiName": "Int32",
                    "ClrType": "System.Int32, System.Private.CoreLib"
                }
            ],
            "ApiEnumTypes": [],
            "ApiObjectTypes": [
                {
                    "ApiKind": "Object",
                    "ApiName": "{{nameof(ClrMemberKindDerived)}}",
                    "ApiProperties": [
                        {
                            "ApiName": "Value",
                            "ApiType": {
                                "ApiKind": "Scalar",
                                "ApiName": "Int32"
                            },
                            "ApiTypeModifiers": "Required",
                            "ClrName": "Value",
                            "ClrMemberKind": "{{clrMemberKind}}"
                        }
                    ],
                    "ClrType": "{{typeof(ClrMemberKindDerived).AssemblyQualifiedName}}"
                }
            ]
        }
        """;

        var apiSchema = JsonSerializer.Deserialize<ApiSchema>(sourceJson)
            ?? throw new InvalidOperationException($"{nameof(ApiSchema)} deserialization failed.");
        var apiObjectType = apiSchema.GetObjectTypeByApiName(nameof(ClrMemberKindDerived))
            ?? throw new InvalidOperationException($"{nameof(ApiObjectType)} lookup failed.");

        return apiObjectType.GetPropertyByApiName("Value")
            ?? throw new InvalidOperationException($"{nameof(ApiProperty)} lookup failed.");
    }
    #endregion
}
