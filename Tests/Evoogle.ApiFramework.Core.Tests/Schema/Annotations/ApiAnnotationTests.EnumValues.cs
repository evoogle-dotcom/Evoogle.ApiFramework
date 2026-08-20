// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Schema.Configuration;
using Evoogle.Reflection;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Annotations;

public partial class ApiAnnotationTests
{
    #region Test Methods
    [Fact]
    public void ApiEnumValueAttributeOverridesConventionNameAndAllowsNoOpAnnotation()
    {
        var schema = ApiAnnotationTestsFactory.BuildWithApiEnumValueAttributeOverridesApiName();

        schema.TryGetEnumTypeByClrType(typeof(OrderStatusValueAnnotated), out var enumType)
            .Should().BeTrue();

        enumType!.ApiEnumValues.Select(x => (x.ClrName, x.ApiName)).Should().Equal
        (
            ("Pending", "awaiting_payment"),
            ("Shipped", "Shipped")
        );
    }

    [Fact]
    public void ExplicitEnumValueNameOverridesApiEnumValueAttribute()
    {
        var schema = ApiAnnotationTestsFactory
            .BuildWithExplicitEnumValueNameOverridesApiEnumValueAttribute();

        schema.TryGetEnumTypeByClrType(typeof(OrderStatusValueAnnotated), out var enumType)
            .Should().BeTrue();

        enumType!.ApiEnumValues.Select(x => (x.ClrName, x.ApiName)).Should().Equal
        (
            ("Pending", "explicit_pending"),
            ("Shipped", "Shipped")
        );
    }

    [Fact]
    public void CustomAnnotationReaderReceivesEnumValueAnnotations()
    {
        var reader = new RecordingAnnotationReader();

        new ApiSchemaBuilder()
            .WithName("Test")
            .UseDefaultConventions()
            .UseAnnotations(x => x.AddReader(reader))
            .AddEnum<OrderStatusValueAnnotated>()
            .Build();

        reader.EnumValueNames.Should().Equal("Pending", "Shipped");
        reader.EnumValueFields.Should().OnlyContain(x => x.DeclaringType == typeof(OrderStatusValueAnnotated));
    }
    #endregion

    #region Test Classes
    private sealed class RecordingAnnotationReader : IApiAnnotationReader
    {
        #region Properties
        public List<string> EnumValueNames { get; } = [];

        public List<FieldInfo> EnumValueFields { get; } = [];
        #endregion

        #region IApiAnnotationReader Methods
        public void ApplyObjectTypeAnnotations(Type clrType, ApiObjectTypeBuilder builder)
        {
        }

        public void ApplyScalarTypeAnnotations(Type clrType, ApiScalarTypeBuilder builder)
        {
        }

        public void ApplyEnumTypeAnnotations(Type clrType, ApiEnumTypeBuilder builder)
        {
        }

        public void ApplyEnumValueAnnotations
        (
            FieldInfo clrField,
            ApiEnumTypeBuilder enumTypeBuilder,
            ApiEnumValueBuilder enumValueBuilder
        )
        {
            this.EnumValueFields.Add(clrField);
            this.EnumValueNames.Add(enumValueBuilder.ClrName);
        }

        public void ApplyPropertyAnnotations
        (
            MemberInfo clrMember,
            ClrMemberKind clrMemberKind,
            MemberNullableInfo clrNullabilityInfo,
            ApiObjectTypeBuilder objectTypeBuilder,
            ApiPropertyBuilder propertyBuilder
        )
        {
        }

        public IReadOnlyList<(string ApiName, Action<ApiRelationshipOneToManyBuilder> Configure)>
            ReadOneToManyRelationships(Type clrType) => [];

        public IReadOnlyList<(string ApiName, Action<ApiRelationshipOneToOneBuilder> Configure)>
            ReadOneToOneRelationships(Type clrType) => [];

        public IReadOnlyList<(string ApiName, Action<ApiRelationshipManyToManyBuilder> Configure)>
            ReadManyToManyRelationships(Type clrType) => [];
        #endregion
    }
    #endregion
}
