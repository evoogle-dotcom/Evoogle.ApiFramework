// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework;

public sealed class ApiVersionAnnotationContractTests(ITestOutputHelper output)
    : XUnitTests(output)
{
    #region Test Classes
    private sealed class AttributeUsageTest : XUnitTest
    {
        #region Calculated Properties
        private AttributeUsageAttribute? AttributeUsage { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.AttributeUsage = typeof(ApiVersionAttribute)
                .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
                .Cast<AttributeUsageAttribute>()
                .Single();
        }

        protected override void Assert()
        {
            this.AttributeUsage.Should().NotBeNull();
            this.AttributeUsage!.ValidOn.Should().Be
            (
                AttributeTargets.Class |
                AttributeTargets.Struct |
                AttributeTargets.Property |
                AttributeTargets.Field
            );
            this.AttributeUsage.AllowMultiple.Should().BeFalse();
            this.AttributeUsage.Inherited.Should().BeTrue();
        }
        #endregion
    }

    private sealed class PocoUsageTest : XUnitTest
    {
        #region Calculated Properties
        private ApiVersionAttribute? MemberAttribute { get; set; }
        private ApiVersionAttribute? TypeAttribute { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.MemberAttribute = typeof(AnnotationsOnlyPoco)
                .GetProperty(nameof(AnnotationsOnlyPoco.Version))!
                .GetCustomAttributes(typeof(ApiVersionAttribute), inherit: true)
                .Cast<ApiVersionAttribute>()
                .Single();
            this.TypeAttribute = typeof(AnnotationsOnlyRepositoryVersionedPoco)
                .GetCustomAttributes(typeof(ApiVersionAttribute), inherit: true)
                .Cast<ApiVersionAttribute>()
                .Single();
        }

        protected override void Assert()
        {
            this.MemberAttribute.Should().NotBeNull();
            this.MemberAttribute.ClrType.Should().BeNull();
            this.TypeAttribute.Should().NotBeNull();
            this.TypeAttribute.ClrType.Should().Be<string>();
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] AttributeUsageTheoryData =>
    [
        new AttributeUsageTest
        {
            Name = $"{nameof(ApiVersionAttribute)} supports object and member targets"
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] PocoUsageTheoryData =>
    [
        new PocoUsageTest
        {
            Name = $"{nameof(ApiVersionAttribute)} supports member and repository declarations"
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(AttributeUsageTheoryData))]
    public void AttributeUsage(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(PocoUsageTheoryData))]
    public void PocoUsage(IXUnitTest test) => test.Execute(this);
    #endregion
}
