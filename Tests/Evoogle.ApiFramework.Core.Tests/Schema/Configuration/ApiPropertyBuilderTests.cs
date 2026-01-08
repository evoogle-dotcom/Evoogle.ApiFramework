// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiPropertyBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    private class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public Type ClrObjectType { get; init; } = null!;
        public string ApiName { get; init; } = null!;
        public string ClrName { get; init; } = null!;
        public ApiProperty ApiPropertyExpected { get; init; } = null!;
        public Type? ApiExtensionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiProperty? ApiPropertyActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"ClrObjectType: {this.ClrObjectType.SafeToName()}");
            this.WriteLine($"ApiName: {this.ApiName.SafeToString()}");
            this.WriteLine($"ClrName: {this.ClrName.SafeToString()}");
            this.WriteLine($"ApiExtensionType: {this.ApiExtensionType.SafeToName()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.ApiPropertyExpected.SafeToString()}");
        }

        protected override void Act()
        {
            var builder = new ApiPropertyBuilder(this.ApiName, this.ClrName);
            if (this.ApiExtensionType != null)
            {
                var extension = Activator.CreateInstance(this.ApiExtensionType);
                builder.AddExtension(this.ApiExtensionType, extension!);
            }

            this.ApiPropertyActual = builder.Build(this.ClrObjectType);
            this.WriteLine($"Actual:   {this.ApiPropertyActual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ApiPropertyActual.Should().NotBeNull();

            this.ApiPropertyActual.Should().BeEquivalentTo
            (
                this.ApiPropertyExpected,
                opt => opt
                    .Excluding(info => info.Path.Contains(nameof(ApiSchemaElement.ApiPath)))
                    .Excluding(p => p.ApiType)
                    .WithStrictOrdering()
            );
        }
        #endregion
    }
    #endregion

    #region Theory Data
    private static ApiProperty RequiredNameProperty { get; } = new ApiProperty(nameof(ScalarsOnly.RequiredName), ApiTypeExpression.ClrRef<string>(), ApiTypeModifiers.Required, nameof(ScalarsOnly.RequiredName), ClrMemberKind.Property);
    private static ApiProperty RequiredNamePropertyWithExtension { get; } = new ApiProperty(nameof(ScalarsOnly.RequiredName), ApiTypeExpression.ClrRef<string>(), ApiTypeModifiers.Required, nameof(ScalarsOnly.RequiredName), ClrMemberKind.Property)
    {
        Extensions = new OrderedDictionary<Type, object>
        {
            [typeof(TestExtension)] = new TestExtension()
        }
    };

    private static ApiProperty OptionalNumberProperty { get; } = new ApiProperty(nameof(ScalarsOnly.OptionalNumber), ApiTypeExpression.ClrRef<long>(), ApiTypeModifiers.None, nameof(ScalarsOnly.OptionalNumber), ClrMemberKind.Field);
    private static ApiProperty OptionalNumberPropertyWithExtension { get; } = new ApiProperty(nameof(ScalarsOnly.OptionalNumber), ApiTypeExpression.ClrRef<long>(), ApiTypeModifiers.None, nameof(ScalarsOnly.OptionalNumber), ClrMemberKind.Field)
    {
        Extensions = new OrderedDictionary<Type, object>
        {
            [typeof(TestExtension)] = new TestExtension()
        }
    };

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = $"Builds {RequiredNameProperty}",
            ClrObjectType = typeof(ScalarsOnly),
            ApiName = RequiredNameProperty.ApiName,
            ClrName = RequiredNameProperty.ClrName,
            ApiPropertyExpected = RequiredNameProperty,
        },
        new BuildTest
        {
            Name = $"Builds {RequiredNamePropertyWithExtension} with extension",
            ClrObjectType = typeof(ScalarsOnly),
            ApiName = RequiredNamePropertyWithExtension.ApiName,
            ClrName = RequiredNamePropertyWithExtension.ClrName,
            ApiPropertyExpected = RequiredNamePropertyWithExtension,
            ApiExtensionType = typeof(TestExtension),
        },
        new BuildTest
        {
            Name = $"Builds {OptionalNumberProperty}",
            ClrObjectType = typeof(ScalarsOnly),
            ApiName = OptionalNumberProperty.ApiName,
            ClrName = OptionalNumberProperty.ClrName,
            ApiPropertyExpected = OptionalNumberProperty,
        },
        new BuildTest
        {
            Name = $"Builds {OptionalNumberPropertyWithExtension} with extension",
            ClrObjectType = typeof(ScalarsOnly),
            ApiName = OptionalNumberPropertyWithExtension.ApiName,
            ClrName = OptionalNumberPropertyWithExtension.ClrName,
            ApiPropertyExpected = OptionalNumberPropertyWithExtension,
            ApiExtensionType = typeof(TestExtension),
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}

