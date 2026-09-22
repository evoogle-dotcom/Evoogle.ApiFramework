// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.XUnit;

using FluentAssertions;

using Microsoft.Extensions.Logging.Abstractions;

namespace Evoogle.ApiFramework.Schema.Types.Internal;

public class ApiMemberBindingTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class BindingObject
    {
        public int Id { get; set; }

        public string Code = string.Empty;

        public static int StaticValue { get; set; }
    }

    private enum Scenario
    {
        DirectProperty,
        DirectField,
        ResolveProperty,
        ResolveField,
        EnforceMemberKind,
        RejectDirectMemberWithReference,
        RejectInvalidDirectMember,
        RejectRepeatedMemberBinding,
        ConsumeFailedMemberResolution,
        RejectMemberResolutionWithoutReference,
        DirectApiProperty,
        ResolveApiName,
        ResolveClrName,
        RejectDirectPropertyWithReference,
        RejectRepeatedPropertyBinding,
        ConsumeFailedPropertyResolution,
        RejectPropertyResolutionWithoutReference
    }

    private sealed class BindingTest : XUnitTest
    {
        public required Scenario Scenario { get; init; }

        private ApiClrMemberBinding? ClrMemberBinding { get; set; }

        private ApiPropertyBinding? ApiPropertyBinding { get; set; }

        private ApiProperty? ExpectedApiProperty { get; set; }

        private Exception? ActualException { get; set; }

        protected override void Arrange()
        { }

        protected override void Act()
        {
            try
            {
                var propertyInfo = typeof(BindingObject).GetProperty(nameof(BindingObject.Id))!;
                var fieldInfo = typeof(BindingObject).GetField(nameof(BindingObject.Code))!;

                switch (this.Scenario)
                {
                    case Scenario.DirectProperty:
                        this.ClrMemberBinding = new(null);
                        this.ClrMemberBinding.Bind(propertyInfo);
                        break;
                    case Scenario.DirectField:
                        this.ClrMemberBinding = new(null);
                        this.ClrMemberBinding.Bind(fieldInfo);
                        break;
                    case Scenario.ResolveProperty:
                        this.ClrMemberBinding = new
                        (
                            new ApiClrMemberReference(nameof(BindingObject.Id), ClrMemberKind.Property)
                        );
                        this.ClrMemberBinding.TryResolveReference
                        (
                            typeof(BindingObject),
                            CreateContext(),
                            ApiSchemaCompilationCode.ApiRelationshipTraversalInvalidClrMember,
                            "Traversal CLR member reference"
                        );
                        break;
                    case Scenario.ResolveField:
                        this.ClrMemberBinding = new
                        (
                            new ApiClrMemberReference(nameof(BindingObject.Code), ClrMemberKind.Field)
                        );
                        this.ClrMemberBinding.TryResolveReference
                        (
                            typeof(BindingObject),
                            CreateContext(),
                            ApiSchemaCompilationCode.ApiRelationshipTraversalInvalidClrMember,
                            "Traversal CLR member reference"
                        );
                        break;
                    case Scenario.EnforceMemberKind:
                        this.ClrMemberBinding = new
                        (
                            new ApiClrMemberReference(nameof(BindingObject.Id), ClrMemberKind.Field)
                        );
                        this.ClrMemberBinding.TryResolveReference
                        (
                            typeof(BindingObject),
                            CreateContext(),
                            ApiSchemaCompilationCode.ApiRelationshipTraversalInvalidClrMember,
                            "Traversal CLR member reference"
                        );
                        break;
                    case Scenario.RejectDirectMemberWithReference:
                        this.ClrMemberBinding = new
                        (
                            new ApiClrMemberReference(nameof(BindingObject.Id), ClrMemberKind.Property)
                        );
                        this.ClrMemberBinding.Bind(propertyInfo);
                        break;
                    case Scenario.RejectInvalidDirectMember:
                        this.ClrMemberBinding = new(null);
                        this.ClrMemberBinding.Bind
                        (
                            typeof(BindingObject).GetProperty(nameof(BindingObject.StaticValue))!
                        );
                        break;
                    case Scenario.RejectRepeatedMemberBinding:
                        this.ClrMemberBinding = new(null);
                        this.ClrMemberBinding.Bind(propertyInfo);
                        this.ClrMemberBinding.Bind(fieldInfo);
                        break;
                    case Scenario.ConsumeFailedMemberResolution:
                        this.ClrMemberBinding = new
                        (
                            new ApiClrMemberReference(nameof(BindingObject.Id), ClrMemberKind.Field)
                        );
                        this.ClrMemberBinding.TryResolveReference
                        (
                            typeof(BindingObject),
                            CreateContext(),
                            ApiSchemaCompilationCode.ApiRelationshipTraversalInvalidClrMember,
                            "Traversal CLR member reference"
                        );
                        this.ClrMemberBinding.TryResolveReference
                        (
                            typeof(BindingObject),
                            CreateContext(),
                            ApiSchemaCompilationCode.ApiRelationshipTraversalInvalidClrMember,
                            "Traversal CLR member reference"
                        );
                        break;
                    case Scenario.RejectMemberResolutionWithoutReference:
                        this.ClrMemberBinding = new(null);
                        this.ClrMemberBinding.TryResolveReference
                        (
                            typeof(BindingObject),
                            CreateContext(),
                            ApiSchemaCompilationCode.ApiRelationshipTraversalInvalidClrMember,
                            "Traversal CLR member reference"
                        );
                        break;
                    case Scenario.DirectApiProperty:
                        this.ExpectedApiProperty = CreateCompiledProperty(out _);
                        this.ApiPropertyBinding = new(null);
                        this.ApiPropertyBinding.Bind(this.ExpectedApiProperty);
                        break;
                    case Scenario.ResolveApiName:
                        this.ExpectedApiProperty = CreateCompiledProperty(out var apiObjectType);
                        this.ApiPropertyBinding = new(ApiPropertyReference.ApiRef("identifier"));
                        this.ApiPropertyBinding.TryResolveReference
                        (
                            apiObjectType,
                            CreateContext(),
                            ApiSchemaCompilationCode.ApiKeyPathSegmentUnresolvedApiProperty,
                            "Key path property reference"
                        );
                        break;
                    case Scenario.ResolveClrName:
                        this.ExpectedApiProperty = CreateCompiledProperty(out apiObjectType);
                        this.ApiPropertyBinding = new(ApiPropertyReference.ClrRef(nameof(BindingObject.Id)));
                        this.ApiPropertyBinding.TryResolveReference
                        (
                            apiObjectType,
                            CreateContext(),
                            ApiSchemaCompilationCode.ApiKeyPathSegmentUnresolvedApiProperty,
                            "Key path property reference"
                        );
                        break;
                    case Scenario.RejectDirectPropertyWithReference:
                        this.ExpectedApiProperty = CreateCompiledProperty(out _);
                        this.ApiPropertyBinding = new(ApiPropertyReference.ApiRef("identifier"));
                        this.ApiPropertyBinding.Bind(this.ExpectedApiProperty);
                        break;
                    case Scenario.RejectRepeatedPropertyBinding:
                        this.ExpectedApiProperty = CreateCompiledProperty(out _);
                        this.ApiPropertyBinding = new(null);
                        this.ApiPropertyBinding.Bind(this.ExpectedApiProperty);
                        this.ApiPropertyBinding.Bind(this.ExpectedApiProperty);
                        break;
                    case Scenario.ConsumeFailedPropertyResolution:
                        _ = CreateCompiledProperty(out apiObjectType);
                        this.ApiPropertyBinding = new(ApiPropertyReference.ApiRef("missing"));
                        this.ApiPropertyBinding.TryResolveReference
                        (
                            apiObjectType,
                            CreateContext(),
                            ApiSchemaCompilationCode.ApiKeyPathSegmentUnresolvedApiProperty,
                            "Key path property reference"
                        );
                        this.ApiPropertyBinding.TryResolveReference
                        (
                            apiObjectType,
                            CreateContext(),
                            ApiSchemaCompilationCode.ApiKeyPathSegmentUnresolvedApiProperty,
                            "Key path property reference"
                        );
                        break;
                    case Scenario.RejectPropertyResolutionWithoutReference:
                        _ = CreateCompiledProperty(out apiObjectType);
                        this.ApiPropertyBinding = new(null);
                        this.ApiPropertyBinding.TryResolveReference
                        (
                            apiObjectType,
                            CreateContext(),
                            ApiSchemaCompilationCode.ApiKeyPathSegmentUnresolvedApiProperty,
                            "Key path property reference"
                        );
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            catch (Exception exception)
            {
                this.ActualException = exception;
            }
        }

        protected override void Assert()
        {
            switch (this.Scenario)
            {
                case Scenario.DirectProperty:
                case Scenario.ResolveProperty:
                    this.ClrMemberBinding!.IsBound.Should().BeTrue();
                    this.ClrMemberBinding.ClrMemberInfo.Should().BeAssignableTo<PropertyInfo>();
                    this.ClrMemberBinding.ClrMemberType.Should().Be<int>();
                    break;
                case Scenario.DirectField:
                case Scenario.ResolveField:
                    this.ClrMemberBinding!.IsBound.Should().BeTrue();
                    this.ClrMemberBinding.ClrMemberInfo.Should().BeAssignableTo<FieldInfo>();
                    this.ClrMemberBinding.ClrMemberType.Should().Be<string>();
                    break;
                case Scenario.EnforceMemberKind:
                    this.ClrMemberBinding!.IsBound.Should().BeFalse();
                    break;
                case Scenario.DirectApiProperty:
                case Scenario.ResolveApiName:
                case Scenario.ResolveClrName:
                    this.ApiPropertyBinding!.IsBound.Should().BeTrue();
                    this.ApiPropertyBinding.ApiProperty.Should().BeSameAs(this.ExpectedApiProperty);
                    break;
                case Scenario.RejectDirectMemberWithReference:
                case Scenario.RejectInvalidDirectMember:
                case Scenario.RejectRepeatedMemberBinding:
                case Scenario.ConsumeFailedMemberResolution:
                case Scenario.RejectMemberResolutionWithoutReference:
                case Scenario.RejectDirectPropertyWithReference:
                case Scenario.RejectRepeatedPropertyBinding:
                case Scenario.ConsumeFailedPropertyResolution:
                case Scenario.RejectPropertyResolutionWithoutReference:
                    this.ActualException.Should().BeOfType<ApiSchemaConfigurationException>();
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] BindingTheoryData =>
    [
        new BindingTest { Name = "Directly binds CLR property", Scenario = Scenario.DirectProperty },
        new BindingTest { Name = "Directly binds CLR field", Scenario = Scenario.DirectField },
        new BindingTest { Name = "Resolves CLR property", Scenario = Scenario.ResolveProperty },
        new BindingTest { Name = "Resolves CLR field", Scenario = Scenario.ResolveField },
        new BindingTest { Name = "Enforces CLR member kind", Scenario = Scenario.EnforceMemberKind },
        new BindingTest { Name = "Rejects direct CLR binding with reference", Scenario = Scenario.RejectDirectMemberWithReference },
        new BindingTest { Name = "Rejects static direct CLR member", Scenario = Scenario.RejectInvalidDirectMember },
        new BindingTest { Name = "Rejects repeated CLR binding", Scenario = Scenario.RejectRepeatedMemberBinding },
        new BindingTest { Name = "Failed CLR resolution consumes attempt", Scenario = Scenario.ConsumeFailedMemberResolution },
        new BindingTest { Name = "Rejects CLR resolution without reference", Scenario = Scenario.RejectMemberResolutionWithoutReference },
        new BindingTest { Name = "Directly binds API property", Scenario = Scenario.DirectApiProperty },
        new BindingTest { Name = "Resolves API-name property reference", Scenario = Scenario.ResolveApiName },
        new BindingTest { Name = "Resolves CLR-name property reference", Scenario = Scenario.ResolveClrName },
        new BindingTest { Name = "Rejects direct API property binding with reference", Scenario = Scenario.RejectDirectPropertyWithReference },
        new BindingTest { Name = "Rejects repeated API property binding", Scenario = Scenario.RejectRepeatedPropertyBinding },
        new BindingTest { Name = "Failed property resolution consumes attempt", Scenario = Scenario.ConsumeFailedPropertyResolution },
        new BindingTest { Name = "Rejects property resolution without reference", Scenario = Scenario.RejectPropertyResolutionWithoutReference },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BindingTheoryData))]
    public void Binding(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Test Helpers
    private static ApiSchemaCompilationContext CreateContext()
    {
        var schema = new ApiSchema("Binding", null, null, null, null, null, null);
        var session = new ApiSchemaCompilationSession(schema, NullLogger.Instance);
        return new(session, schema, default, nameof(ApiSchema));
    }

    private static ApiProperty CreateCompiledProperty(out ApiObjectType apiObjectType)
    {
        var apiProperty = new ApiProperty
        (
            "identifier",
            new ApiTypeExpression(new ApiTypeReference(typeof(int))),
            ApiTypeModifiers.Required,
            nameof(BindingObject.Id),
            ClrMemberKind.Property
        );
        apiObjectType = new ApiObjectType
        (
            "BindingObject",
            null,
            [apiProperty],
            null,
            null,
            typeof(BindingObject)
        );
        var schema = new ApiSchema
        (
            "Binding",
            null,
            null,
            [new ApiScalarType("Int32", typeof(int))],
            null,
            [apiObjectType],
            null
        );

        ApiSchemaCompiler.Compile(schema).ThrowIfInvalid();
        return apiProperty;
    }
    #endregion
}
