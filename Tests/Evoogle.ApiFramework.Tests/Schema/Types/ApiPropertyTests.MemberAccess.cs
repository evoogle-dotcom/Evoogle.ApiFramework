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
    #region Member Access Test Types
    private enum MemberAccessOverload
    {
        Object,
        Generic
    }

    private enum MemberAccessSurface
    {
        Property,
        Traversal
    }

    private sealed class MemberAccessSource
    {
        #region Fields and Properties
        public long Number { get; set; } = 7;
        public long NumberField = 7;
        public MemberAccessTarget? Related { get; set; }
        public MemberAccessTarget? RelatedField;
        #endregion
    }

    private sealed class MemberAccessTarget
    {
        #region Properties
        public long Id { get; set; }
        #endregion
    }

    private sealed class CompiledMemberAccessTest : XUnitTest
    {
        #region User Supplied Properties
        public required MemberAccessSurface Surface { get; init; }
        public required MemberAccessOverload Overload { get; init; }
        public required ClrMemberKind ClrKind { get; init; }
        public required object InitialMemberValue { get; init; }
        public required object ExpectedInitialRead { get; init; }
        public required object SuppliedValue { get; init; }
        public required object ExpectedMemberValue { get; init; }
        #endregion

        #region Calculated Properties
        private ApiProperty? Property { get; set; }
        private ApiRelationshipTraversal? Traversal { get; set; }
        private MemberAccessSource? ClrObject { get; set; }
        private object? ActualInitialRead { get; set; }
        private object? ActualRead { get; set; }
        private object? ActualMemberValue { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            (this.Property, this.Traversal) = CreateCompiledMemberAccessSurface(this.Surface, this.ClrKind, false);
            this.ClrObject = new MemberAccessSource();
            if (this.Surface == MemberAccessSurface.Property)
            {
                this.ClrObject.Number = (long)this.InitialMemberValue;
                this.ClrObject.NumberField = (long)this.InitialMemberValue;
            }
            else
            {
                this.ClrObject.Related = (MemberAccessTarget)this.InitialMemberValue;
                this.ClrObject.RelatedField = (MemberAccessTarget)this.InitialMemberValue;
            }

            this.WriteLine($"Surface: {this.Surface}; CLR Kind: {this.ClrKind}; Overload: {this.Overload}");
            this.WriteLine($"Initial Member: {this.InitialMemberValue.SafeToString()}; " +
                $"Supplied: {this.SuppliedValue.SafeToString()}");
            this.WriteLine($"Expected Read: {this.ExpectedInitialRead.SafeToString()}; " +
                $"Expected Member: {this.ExpectedMemberValue.SafeToString()}; Expected Boundary: none");
        }

        protected override void Act()
        {
            if (this.Surface == MemberAccessSurface.Property)
            {
                if (this.Overload == MemberAccessOverload.Object)
                {
                    this.ActualInitialRead = this.Property!.GetValue(this.ClrObject!, typeof(string));
                    this.Property.SetValue(this.ClrObject!, this.SuppliedValue);
                    this.ActualRead = this.Property.GetValue(this.ClrObject!);
                }
                else
                {
                    this.ActualInitialRead = this.Property!.GetValue<MemberAccessSource, string>(this.ClrObject!);
                    this.Property.SetValue(this.ClrObject!, (string)this.SuppliedValue);
                    this.ActualRead = this.Property.GetValue<MemberAccessSource, long>(this.ClrObject!);
                }

                this.ActualMemberValue = this.ClrKind == ClrMemberKind.Property
                    ? this.ClrObject!.Number : this.ClrObject!.NumberField;
            }
            else
            {
                if (this.Overload == MemberAccessOverload.Object)
                {
                    this.ActualInitialRead = this.Traversal!.GetValue(this.ClrObject!);
                    this.Traversal.SetValue(this.ClrObject!, this.SuppliedValue);
                    this.ActualRead = this.Traversal.GetValue(this.ClrObject!);
                }
                else
                {
                    this.ActualInitialRead = this.Traversal!
                        .GetValue<MemberAccessSource, MemberAccessTarget>(this.ClrObject!);
                    this.Traversal.SetValue(
                        this.ClrObject!, (MemberAccessTarget)this.SuppliedValue);
                    this.ActualRead = this.Traversal.GetValue<MemberAccessSource, MemberAccessTarget>(this.ClrObject!);
                }

                this.ActualMemberValue = this.ClrKind == ClrMemberKind.Property
                    ? this.ClrObject!.Related : this.ClrObject!.RelatedField;
            }

            this.WriteLine($"Initial Read: {this.ActualInitialRead.SafeToString()}; " +
                $"Final Read: {this.ActualRead.SafeToString()}; CLR State: {this.ActualMemberValue.SafeToString()}");
        }

        protected override void Assert()
        {
            if (this.Surface == MemberAccessSurface.Property)
            {
                this.ActualInitialRead.Should().Be(this.ExpectedInitialRead);
                this.ActualRead.Should().Be(this.ExpectedMemberValue);
                this.ActualMemberValue.Should().Be(this.ExpectedMemberValue);
            }
            else
            {
                this.ActualInitialRead.Should().BeSameAs(this.ExpectedInitialRead);
                this.ActualRead.Should().BeSameAs(this.ExpectedMemberValue);
                this.ActualMemberValue.Should().BeSameAs(this.ExpectedMemberValue);
            }
        }
        #endregion
    }

    private sealed class MemberAccessBoundaryTest : XUnitTest
    {
        #region User Supplied Properties
        public required MemberAccessSurface Surface { get; init; }
        public required bool IsUnbound { get; init; }
        public required bool IsSet { get; init; }
        public required Type ExpectedExceptionType { get; init; }
        public required Type? ExpectedInnerExceptionType { get; init; }
        public required string[] ExpectedMessageFragments { get; init; }
        public required long ExpectedMemberValue { get; init; }
        #endregion

        #region Calculated Properties
        private ApiProperty? Property { get; set; }
        private ApiRelationshipTraversal? Traversal { get; set; }
        private MemberAccessSource? ClrObject { get; set; }
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            (this.Property, this.Traversal) = CreateCompiledMemberAccessSurface(
                this.Surface, ClrMemberKind.Property, this.IsUnbound);
            this.ClrObject = new MemberAccessSource();
            this.WriteLine($"Surface: {this.Surface}; Unbound: {this.IsUnbound}; Set: {this.IsSet}");
            this.WriteLine($"Expected Boundary: {this.ExpectedExceptionType.Name}; " +
                $"Inner: {this.ExpectedInnerExceptionType?.Name}; CLR State: {this.ExpectedMemberValue}");
        }

        protected override void Act()
        {
            var target = this.IsUnbound ? this.ClrObject : null;
            try
            {
                if (this.IsSet)
                {
                    if (this.Surface == MemberAccessSurface.Property)
                    {
                        this.Property!.SetValue((object)target!, 42L);
                    }
                    else
                    {
                        this.Traversal!.SetValue((object)target!, new MemberAccessTarget { Id = 42 });
                    }
                }
                else if (this.Surface == MemberAccessSurface.Property)
                {
                    this.Property!.GetValue((object)target!);
                }
                else
                {
                    this.Traversal!.GetValue((object)target!);
                }
            }
            catch (Exception exception)
            {
                this.ActualException = exception;
            }

            this.WriteLine($"Target: {target.SafeToString()}; CLR State: {this.ClrObject!.Number}");
            this.WriteLine($"Captured Exception: {this.ActualException.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ActualException.Should().BeOfType(this.ExpectedExceptionType);
            foreach (var fragment in this.ExpectedMessageFragments)
            {
                this.ActualException!.Message.Should().Contain(fragment);
            }

            if (this.ExpectedInnerExceptionType is null)
            {
                this.ActualException!.InnerException.Should().BeNull();
            }
            else
            {
                this.ActualException!.InnerException.Should().BeOfType(this.ExpectedInnerExceptionType);
            }

            this.ClrObject!.Number.Should().Be(this.ExpectedMemberValue);
        }
        #endregion
    }
    #endregion

    #region Member Access Theory Data
    public static TheoryDataRow<IXUnitTest>[] CompiledMemberAccessTheoryData =>
    [
        .. from surface in Enum.GetValues<MemberAccessSurface>()
           from clrKind in Enum.GetValues<ClrMemberKind>()
           from overload in Enum.GetValues<MemberAccessOverload>()
           let initialTarget = new MemberAccessTarget { Id = 7 }
           let suppliedTarget = new MemberAccessTarget { Id = 42 }
           select (TheoryDataRow<IXUnitTest>)new CompiledMemberAccessTest
           {
               Name = $"Compiled {surface}; {clrKind}; {overload}",
               Surface = surface, ClrKind = clrKind, Overload = overload,
               InitialMemberValue = surface == MemberAccessSurface.Property ? 7L : initialTarget,
               ExpectedInitialRead = surface == MemberAccessSurface.Property ? "7" : initialTarget,
               SuppliedValue = surface == MemberAccessSurface.Property ? "42" : suppliedTarget,
               ExpectedMemberValue = surface == MemberAccessSurface.Property ? 42L : suppliedTarget
           }
    ];

    public static TheoryDataRow<IXUnitTest>[] MemberAccessRejectsNullTargetTheoryData =>
    [
        .. from surface in Enum.GetValues<MemberAccessSurface>()
           from isSet in new[] { false, true }
           select (TheoryDataRow<IXUnitTest>)new MemberAccessBoundaryTest
           {
               Name = $"{surface}; Set={isSet}; null object target",
               Surface = surface, IsUnbound = false, IsSet = isSet,
               ExpectedExceptionType = typeof(ArgumentNullException), ExpectedInnerExceptionType = null,
               ExpectedMessageFragments = ["clrObject"], ExpectedMemberValue = 7
           }
    ];

    public static TheoryDataRow<IXUnitTest>[] UnboundMemberAccessUsesSurfaceSpecificExceptionBoundaryTheoryData =>
    [
        .. Enum.GetValues<MemberAccessSurface>().Select(surface => (TheoryDataRow<IXUnitTest>)
            new MemberAccessBoundaryTest
            {
                Name = $"Compiled {surface} with no bound accessor",
                Surface = surface, IsUnbound = true, IsSet = false,
                ExpectedExceptionType = typeof(ApiSchemaException),
                ExpectedInnerExceptionType = surface == MemberAccessSurface.Property
                    ? null : typeof(ApiSchemaException),
                ExpectedMessageFragments = surface == MemberAccessSurface.Property
                    ? ["property 'Number'", "no compiled getter available"]
                    : ["traversal 'Related'", "<unbound>", "Failed to get value"],
                ExpectedMemberValue = 7
            })
    ];
    #endregion

    #region Member Access Test Methods
    [Theory]
    [MemberData(nameof(CompiledMemberAccessTheoryData))]
    public void CompileWiresPropertyAndTraversalMemberAccessors(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(MemberAccessRejectsNullTargetTheoryData))]
    public void MemberAccessRejectsNullTarget(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(UnboundMemberAccessUsesSurfaceSpecificExceptionBoundaryTheoryData))]
    public void UnboundMemberAccessUsesSurfaceSpecificExceptionBoundary(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Member Access Factory Methods
    private static (ApiProperty? Property, ApiRelationshipTraversal? Traversal) CreateCompiledMemberAccessSurface(
        MemberAccessSurface surface, ClrMemberKind clrKind, bool isUnbound)
    {
        var scalar = new ApiScalarType("Int64", typeof(long));
        var numberMember = clrKind == ClrMemberKind.Property ? "Number" : "NumberField";
        var property = new ApiProperty("Value", new ApiTypeExpression(new ApiTypeReference(typeof(long))),
            ApiTypeModifiers.Required, new ClrMemberReference(clrKind, numberMember));
        var source = new ApiObjectType("Source", null, [property], null, null, null,
            isUnbound && surface == MemberAccessSurface.Property ? null! : typeof(MemberAccessSource));
        ApiSchema schema;
        ApiRelationshipTraversal? traversal = null;
        if (surface == MemberAccessSurface.Property)
        {
            schema = new ApiSchema("MemberAccess", null, null, [scalar], null, [source], null);
        }
        else
        {
            var navigationMember = clrKind == ClrMemberKind.Property ? "Related" : "RelatedField";
            traversal = new ApiRelationshipTraversal("Related", isUnbound
                ? null : new ClrMemberReference(clrKind, navigationMember));
            var targetProperty = new ApiProperty("Id", new ApiTypeExpression(new ApiTypeReference(typeof(long))),
                ApiTypeModifiers.Required, new ClrMemberReference(ClrMemberKind.Property, "Id"));
            var target = new ApiObjectType("Target", null, [targetProperty], null, null, null, typeof(MemberAccessTarget));
            var relationship = new ApiRelationshipOneToOne("SourceTarget",
                new ApiRelationshipPrincipalEnd(new ApiTypeReference(typeof(MemberAccessSource)), traversal),
                new ApiRelationshipDependentEnd(new ApiTypeReference(typeof(MemberAccessTarget))));
            schema = new ApiSchema("MemberAccess", null, null, [scalar], null, [source, target], [relationship]);
        }

        var result = ApiSchemaCompiler.Compile(schema);
        if (!isUnbound || surface == MemberAccessSurface.Traversal)
        {
            result.ThrowIfInvalid();
        }

        return surface == MemberAccessSurface.Property ? (property, null) : (null, traversal);
    }
    #endregion
}
