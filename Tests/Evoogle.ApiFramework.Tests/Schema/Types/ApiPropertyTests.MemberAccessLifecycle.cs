// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Types;

public partial class ApiPropertyTests
{
    #region Member Access Lifecycle Test Types
    private struct StructValueSource
    {
        #region Fields and Properties
        public long Number { get; set; }
        public long NumberField;
        #endregion
    }

    private struct StructNavigationSource
    {
        #region Fields and Properties
        public long Id { get; set; }
        public MemberAccessTarget? Related { get; set; }
        public MemberAccessTarget? RelatedField;
        #endregion
    }

    private sealed class CompiledStructWriteTest : XUnitTest
    {
        #region User Supplied Properties
        public required MemberAccessSurface Surface { get; init; }
        public required ClrMemberKind ClrKind { get; init; }
        public required bool UsesTry { get; init; }
        public required object ExpectedMemberValue { get; init; }
        public required bool ExpectedSuccess { get; init; }
        #endregion

        #region Fields
        private StructValueSource _valueSource;
        private StructNavigationSource _navigationSource;
        #endregion

        #region Calculated Properties
        private ApiProperty? Property { get; set; }
        private ApiRelationshipTraversal? Traversal { get; set; }
        private bool ActualSuccess { get; set; }
        private object? ActualRead { get; set; }
        private object? ActualMemberValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            _valueSource = new StructValueSource { Number = 7, NumberField = 7 };
            _navigationSource = new StructNavigationSource { Id = 1, RelatedField = null };
            var scalar = new ApiScalarType(apiName: "Int64", clrScalarType: typeof(long));

            ApiSchema schema;
            if (this.Surface == MemberAccessSurface.Property)
            {
                var clrName = this.ClrKind == ClrMemberKind.Property ? "Number" : "NumberField";
                this.Property = CreateLifecycleProperty("Value", clrName, this.ClrKind);
                var source = new ApiObjectType(apiName: "Source", apiOptions: null, apiProperties: [this.Property], apiKeys: null, apiCanonicalKeyName: null, apiVersion: null, clrObjectType: typeof(StructValueSource));
                schema = new ApiSchema(apiName: "StructAccess", apiVersion: null, apiOptions: null, apiScalarTypes: [scalar], apiEnumTypes: null, apiObjectTypes: [source], apiRelationships: null, apiQuerySources: null);
            }
            else
            {
                var clrName = this.ClrKind == ClrMemberKind.Property ? "Related" : "RelatedField";
                this.Traversal = new ApiRelationshipTraversal(apiName: "Related", clrNavigationMember: new ClrMemberReference(this.ClrKind, clrName));
                var source = new ApiObjectType(apiName: "Source", apiOptions: null, apiProperties: [CreateLifecycleProperty("Id", "Id", ClrMemberKind.Property)], apiKeys: null, apiCanonicalKeyName: null, apiVersion: null, clrObjectType: typeof(StructNavigationSource));
                var target = new ApiObjectType(apiName: "Target", apiOptions: null, apiProperties: [CreateLifecycleProperty("Id", "Id", ClrMemberKind.Property)], apiKeys: null, apiCanonicalKeyName: null, apiVersion: null, clrObjectType: typeof(MemberAccessTarget));
                var relationship = new ApiRelationshipOneToOne
                (
                    apiName: "SourceTarget",
                    apiPrincipalEnd: new ApiRelationshipPrincipalEnd
                    (
                        apiObjectTypeReference: new ApiTypeReference(clrType: typeof(StructNavigationSource)),
                        apiTraversal: this.Traversal
                    ),
                    apiDependentEnd: new ApiRelationshipDependentEnd(apiObjectTypeReference: new ApiTypeReference(clrType: typeof(MemberAccessTarget)))
                );
                schema = new ApiSchema(apiName: "StructAccess", apiVersion: null, apiOptions: null, apiScalarTypes: [scalar], apiEnumTypes: null, apiObjectTypes: [source, target], apiRelationships: [relationship], apiQuerySources: null);
            }

            ApiSchemaCompiler.Compile(apiSchema: schema).ThrowIfInvalid();

            this.WriteLine($"Compiled Struct Surface: {this.Surface}; Kind: {this.ClrKind}; Try: {this.UsesTry}");
            this.WriteLine($"Initial Number: 7; Initial Navigation: null; Expected Member: {this.ExpectedMemberValue.SafeToString()}");
            this.WriteLine($"Expected Success: {this.ExpectedSuccess}; Expected Boundary: none");
        }

        protected override void Act()
        {
            if (this.Surface == MemberAccessSurface.Property)
            {
                var suppliedValue = (long)this.ExpectedMemberValue;
                if (this.UsesTry)
                {
                    this.ActualSuccess = this.Property!.TrySetValueByRef(ref _valueSource, suppliedValue);
                }
                else
                {
                    this.Property!.SetValueByRef(ref _valueSource, suppliedValue);
                    this.ActualSuccess = true;
                }

                this.ActualRead = this.Property.GetValue<StructValueSource, long>(_valueSource);
                this.ActualMemberValue = this.ClrKind == ClrMemberKind.Property ? _valueSource.Number : _valueSource.NumberField;
            }
            else
            {
                var suppliedValue = (MemberAccessTarget)this.ExpectedMemberValue;
                if (this.UsesTry)
                {
                    this.ActualSuccess = this.Traversal!.TrySetValueByRef(ref _navigationSource, suppliedValue);
                }
                else
                {
                    this.Traversal!.SetValueByRef(ref _navigationSource, suppliedValue);
                    this.ActualSuccess = true;
                }

                this.ActualRead = this.Traversal.GetValue<StructNavigationSource, MemberAccessTarget>(_navigationSource);
                this.ActualMemberValue = this.ClrKind == ClrMemberKind.Property ? _navigationSource.Related : _navigationSource.RelatedField;
            }

            this.WriteLine($"Actual Success: {this.ActualSuccess}; Read: {this.ActualRead.SafeToString()}; Original Struct Member: {this.ActualMemberValue.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualSuccess.Should().Be(this.ExpectedSuccess);
            if (this.Surface == MemberAccessSurface.Property)
            {
                this.ActualRead.Should().Be(this.ExpectedMemberValue);
                this.ActualMemberValue.Should().Be(this.ExpectedMemberValue);
            }
            else
            {
                this.ActualRead.Should().BeSameAs(this.ExpectedMemberValue);
                this.ActualMemberValue.Should().BeSameAs(this.ExpectedMemberValue);
            }
        }
        #endregion
    }

    private sealed class UncompiledMemberAccessTest : XUnitTest
    {
        #region User Supplied Properties
        public required MemberAccessSurface Surface { get; init; }
        public required ClrMemberKind ClrKind { get; init; }
        public required MemberAccessOverload Overload { get; init; }
        public required bool UsesByRef { get; init; }
        public required bool UsesTry { get; init; }
        public required object? ExpectedValue { get; init; }
        public required string ExpectedMemberIdentity { get; init; }
        public required Type ExpectedClrType { get; init; }
        #endregion

        #region Calculated Fields and Properties
        private ApiProperty? Property { get; set; }
        private ApiRelationshipTraversal? Traversal { get; set; }
        private MemberAccessSource? ClrObject { get; set; }
        private StructValueSource _clrStruct;
        private object? ActualValue { get; set; }
        private bool? GetSucceeded { get; set; }
        private bool? SetSucceeded { get; set; }
        private Exception? GetException { get; set; }
        private Exception? SetException { get; set; }
        private long ActualMemberValue { get; set; }
        private string ClrName => this.ClrKind == ClrMemberKind.Property ? nameof(MemberAccessSource.Number) : nameof(MemberAccessSource.NumberField);
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var reference = new ClrMemberReference(this.ClrKind, this.ClrName);
            if (this.Surface == MemberAccessSurface.Property)
            {
                this.Property = new ApiProperty
                (
                    apiName: "Value",
                    apiTypeExpression: new ApiTypeExpression(apiTypeReference: new ApiTypeReference(clrType: typeof(long))),
                    apiNullability: ApiNullability.NonNullable,
                    clrValueMember: reference);
            }
            else
            {
                this.Traversal = new ApiRelationshipTraversal(apiName: "Related", clrNavigationMember: reference);
            }

            this.ClrObject = new MemberAccessSource();
            _clrStruct = new StructValueSource { Number = 7, NumberField = 7 };

            this.WriteLine($"Surface:       {this.Surface}; Member: {this.ClrName}; Kind: {this.ClrKind}");
            this.WriteLine($"Overload:      {this.Overload}; By Ref: {this.UsesByRef}; Try: {this.UsesTry}");
            this.WriteLine($"Initial Value: 7; Supplied Value: 42; Expected Value: {this.ExpectedValue.SafeToString()}");
            this.WriteLine();

            var expectedBoundary = this.UsesTry ? "false without exception" : nameof(ApiSchemaException);
            this.WriteLine($"Expected Boundary:        {expectedBoundary}");
            this.WriteLine($"Expected Member Identity: {this.ExpectedMemberIdentity}; CLR Type: {this.ExpectedClrType.FullName}");
            this.WriteLine();
        }

        protected override void Act()
        {
            try
            {
                if (this.UsesTry)
                {
                    if (this.UsesByRef)
                    {
                        this.GetSucceeded = this.Surface == MemberAccessSurface.Property
                            ? this.Property!.TryGetValue(_clrStruct, out long clrValue)
                            : this.Traversal!.TryGetValue(_clrStruct, out clrValue);
                        this.ActualValue = clrValue;
                    }
                    else if (this.Overload == MemberAccessOverload.Generic)
                    {
                        this.GetSucceeded = this.Surface == MemberAccessSurface.Property
                            ? this.Property!.TryGetValue(this.ClrObject, out long clrValue)
                            : this.Traversal!.TryGetValue(this.ClrObject, out clrValue);
                        this.ActualValue = clrValue;
                    }
                    else
                    {
                        this.GetSucceeded = this.Surface == MemberAccessSurface.Property
                            ? this.Property!.TryGetValue(this.ClrObject, out var clrValue)
                            : this.Traversal!.TryGetValue(this.ClrObject, out clrValue);
                        this.ActualValue = clrValue;
                    }
                }
                else
                {
                    this.ActualValue = this.UsesByRef
                        ? this.Surface == MemberAccessSurface.Property
                            ? this.Property!.GetValue<StructValueSource, long>(_clrStruct)
                            : this.Traversal!.GetValue<StructValueSource, long>(_clrStruct)
                        : this.Overload == MemberAccessOverload.Generic
                            ? this.Surface == MemberAccessSurface.Property
                                ? this.Property!.GetValue<MemberAccessSource, long>(this.ClrObject!)
                                : this.Traversal!.GetValue<MemberAccessSource, long>(this.ClrObject!)
                            : this.Surface == MemberAccessSurface.Property
                                ? this.Property!.GetValue(this.ClrObject!)
                                : this.Traversal!.GetValue(this.ClrObject!);
                }
            }
            catch (Exception exception)
            {
                this.GetException = exception;
            }

            try
            {
                if (this.UsesByRef)
                {
                    if (this.UsesTry)
                    {
                        this.SetSucceeded = this.Surface == MemberAccessSurface.Property
                            ? this.Property!.TrySetValueByRef(ref _clrStruct, 42L)
                            : this.Traversal!.TrySetValueByRef(ref _clrStruct, 42L);
                    }
                    else if (this.Surface == MemberAccessSurface.Property)
                    {
                        this.Property!.SetValueByRef(ref _clrStruct, 42L);
                    }
                    else
                    {
                        this.Traversal!.SetValueByRef(ref _clrStruct, 42L);
                    }
                }
                else if (this.UsesTry)
                {
                    this.SetSucceeded = this.Overload == MemberAccessOverload.Generic
                        ? this.Surface == MemberAccessSurface.Property
                            ? this.Property!.TrySetValue(this.ClrObject, 42L)
                            : this.Traversal!.TrySetValue(this.ClrObject, 42L)
                        : this.Surface == MemberAccessSurface.Property
                            ? this.Property!.TrySetValue(this.ClrObject, (object)42L)
                            : this.Traversal!.TrySetValue(this.ClrObject, (object)42L);
                }
                else if (this.Overload == MemberAccessOverload.Generic)
                {
                    if (this.Surface == MemberAccessSurface.Property)
                    {
                        this.Property!.SetValue(this.ClrObject!, 42L);
                    }
                    else
                    {
                        this.Traversal!.SetValue(this.ClrObject!, 42L);
                    }
                }
                else if (this.Surface == MemberAccessSurface.Property)
                {
                    this.Property!.SetValue(this.ClrObject!, (object)42L);
                }
                else
                {
                    this.Traversal!.SetValue(this.ClrObject!, (object)42L);
                }
            }
            catch (Exception exception)
            {
                this.SetException = exception;
            }

            this.ActualMemberValue = this.UsesByRef
                ? this.ClrKind == ClrMemberKind.Property ? _clrStruct.Number : _clrStruct.NumberField
                : this.ClrKind == ClrMemberKind.Property ? this.ClrObject!.Number : this.ClrObject!.NumberField;
            this.WriteLine($"Get Succeeded: {this.GetSucceeded}; Set Succeeded: {this.SetSucceeded}");
            this.WriteLine($"Actual Value:  {this.ActualValue.SafeToString()}; Member Value: {this.ActualMemberValue}");

            if (this.GetException != null)
            {
                this.WriteLine($"Get Exception: Name={this.GetException.GetType().SafeToName()}; Message={this.GetException.Message.SafeToString()}");
            }

            if (this.SetException != null)
            {
                this.WriteLine($"Set Exception: Name={this.SetException.GetType().SafeToName()}; Message={this.SetException.Message.SafeToString()}");
            }
        }

        protected override void Assert()
        {
            this.ActualMemberValue.Should().Be(7);
            if (this.UsesTry)
            {
                this.GetException.Should().BeNull();
                this.SetException.Should().BeNull();
                this.GetSucceeded.Should().BeFalse();
                this.SetSucceeded.Should().BeFalse();
                this.ActualValue.Should().Be(this.ExpectedValue);
                return;
            }

            foreach (var exception in new[] { this.GetException, this.SetException })
            {
                var schemaException = exception.Should().BeOfType<ApiSchemaException>().Which;
                schemaException.Message.Should().Contain(this.ExpectedMemberIdentity).And.Contain(this.ExpectedClrType.FullName).And.Contain("schema compilation");
                schemaException.InnerException.Should().BeNull();
            }
        }
        #endregion
    }
    #endregion

    #region Member Access Lifecycle Theory Data
    public static TheoryDataRow<IXUnitTest>[] UncompiledTryMemberAccessTheoryData => CreateUncompiledTheoryData(true);
    public static TheoryDataRow<IXUnitTest>[] UncompiledMemberAccessTheoryData => CreateUncompiledTheoryData(false);

    public static TheoryDataRow<IXUnitTest>[] CompiledStructWriteTheoryData =>
    [
        .. from surface in Enum.GetValues<MemberAccessSurface>()
           from clrKind in Enum.GetValues<ClrMemberKind>()
           select (TheoryDataRow<IXUnitTest>)new CompiledStructWriteTest
           {
               Name = $"Compiled struct {surface}; {clrKind}",
               Surface = surface, ClrKind = clrKind, UsesTry = clrKind == ClrMemberKind.Field,
               ExpectedSuccess = true,
               ExpectedMemberValue = surface == MemberAccessSurface.Property ? 42L : new MemberAccessTarget { Id = 42 }
           }
    ];
    #endregion

    #region Member Access Lifecycle Test Methods
    [Theory]
    [MemberData(nameof(UncompiledTryMemberAccessTheoryData))]
    public void TryMemberAccessReturnsFalseBeforeSchemaCompilation(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(UncompiledMemberAccessTheoryData))]
    public void MemberAccessReportsUncompiledSchema(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(CompiledStructWriteTheoryData))]
    public void CompileWiresByRefMemberWrites(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Member Access Lifecycle Factory Methods
    private static TheoryDataRow<IXUnitTest>[] CreateUncompiledTheoryData(bool usesTry) =>
    [
        .. from surface in Enum.GetValues<MemberAccessSurface>()
           from clrKind in Enum.GetValues<ClrMemberKind>()
           from overload in new[] { (MemberAccessOverload.Object, false), (MemberAccessOverload.Generic, false),
               (MemberAccessOverload.Generic, true) }
           select (TheoryDataRow<IXUnitTest>)new UncompiledMemberAccessTest
           {
               Name = $"Uncompiled {surface}; {clrKind}; {overload}; Try={usesTry}",
               Surface = surface, ClrKind = clrKind, Overload = overload.Item1, UsesByRef = overload.Item2,
               UsesTry = usesTry, ExpectedValue = overload.Item1 == MemberAccessOverload.Object ? null : 0L,
               ExpectedClrType = overload.Item2 ? typeof(StructValueSource) : typeof(MemberAccessSource),
               ExpectedMemberIdentity = surface == MemberAccessSurface.Property
                   ? $"property '{(clrKind == ClrMemberKind.Property ? "Number" : "NumberField")}'"
                   : $"traversal 'Related' CLR navigation member '{(clrKind == ClrMemberKind.Property ? "Number" : "NumberField")}'"
           }
    ];

    private static ApiProperty CreateLifecycleProperty(string apiName, string clrName, ClrMemberKind clrKind) => new
        (
            apiName: apiName,
            apiTypeExpression: new ApiTypeExpression(apiTypeReference: new ApiTypeReference(clrType: typeof(long))),
            apiNullability: ApiNullability.NonNullable,
            clrValueMember: new ClrMemberReference(clrKind, clrName)
        );
    #endregion
}
