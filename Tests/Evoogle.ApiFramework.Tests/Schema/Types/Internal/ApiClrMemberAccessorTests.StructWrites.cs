// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;
using Evoogle.MemberAccess;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Types.Internal;

public partial class ApiClrMemberAccessorTests
{
    #region Struct Write Test Types
    private struct StructValueSource
    {
        #region Fields and Properties
        public long Number { get; set; }
        public long NumberField;
        #endregion
    }

    private sealed class NavigationTarget
    {
        #region Properties
        public long Id { get; set; }
        public NavigationTarget? Related { get; set; }
        #endregion
    }

    private struct StructNavigationSource
    {
        #region Fields and Properties
        public long Id { get; set; }
        public NavigationTarget? Related { get; set; }
        public NavigationTarget? RelatedField;
        #endregion
    }

    private enum StructWriteKind
    {
        Direct,
        Coercing,
        Invalid
    }

    private sealed class StructMemberWriteTest : XUnitTest
    {
        #region User Supplied Properties
        public required MemberAccessSurface Surface { get; init; }
        public required ClrMemberKind ClrKind { get; init; }
        public required StructWriteKind WriteKind { get; init; }
        public required bool UsesTry { get; init; }
        public required bool ExpectedSuccess { get; init; }
        public required long ExpectedId { get; init; }
        #endregion

        #region Calculated Fields and Properties
        private ApiClrMemberAccessor? Accessor { get; set; }
        private NavigationTarget? SuppliedTarget { get; set; }
        private NavigationTarget? InitialTarget { get; set; }
        private StructValueSource _clrValueSource;
        private StructNavigationSource _clrNavigationSource;
        private bool? ActualSuccess { get; set; }
        private Exception? ActualException { get; set; }
        private long ActualId { get; set; }
        private NavigationTarget? ActualTarget { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            _clrValueSource = new StructValueSource { Number = 7, NumberField = 7 };
            var initialTarget = new NavigationTarget { Id = 7 };
            this.InitialTarget = initialTarget;
            _clrNavigationSource = new StructNavigationSource
            {
                Id = 1,
                Related = initialTarget,
                RelatedField = initialTarget
            };

            this.SuppliedTarget = new NavigationTarget { Id = 42 };

            var sourceClrType = this.Surface == MemberAccessSurface.Property ? typeof(StructValueSource) : typeof(StructNavigationSource);
            var clrName = this.Surface == MemberAccessSurface.Property
                ? this.ClrKind == ClrMemberKind.Property ? "Number" : "NumberField"
                : this.ClrKind == ClrMemberKind.Property ? "Related" : "RelatedField";
            var memberInfo = this.ClrKind == ClrMemberKind.Property ? (System.Reflection.MemberInfo)sourceClrType.GetProperty(clrName)! : sourceClrType.GetField(clrName)!;
            var schema = new ApiSchema("StructAccess", null, null, null, null, null, null);
            var description = this.Surface == MemberAccessSurface.Property
                ? ApiClrMemberAccessDescription.ForProperty(clrName)
                : ApiClrMemberAccessDescription.ForTraversal("Related", clrName);
            this.Accessor = new ApiClrMemberAccessor(MemberAccessor.Create(memberInfo), new ApiSchemaContext(schema), description);

            this.WriteLine($"Surface:           {this.Surface}; Member: {clrName}; Kind: {this.ClrKind}");
            this.WriteLine($"Write:             {this.WriteKind}; Try: {this.UsesTry}; Initial Id: 7; Supplied Id: 42");
            this.WriteLine($"Expected Success:  {this.ExpectedSuccess}; Expected Id: {this.ExpectedId}");

            var expectedBoundary = this.UsesTry || this.ExpectedSuccess ? "none" : nameof(ApiSchemaException);
            this.WriteLine($"Expected Boundary: {expectedBoundary}");
            this.WriteLine();
        }

        protected override void Act()
        {
            try
            {
                if (this.Surface == MemberAccessSurface.Property)
                {
                    if (this.WriteKind == StructWriteKind.Direct)
                    {
                        this.WriteValue(42L);
                    }
                    else
                    {
                        this.WriteValue(this.WriteKind == StructWriteKind.Coercing ? "42" : "invalid");
                    }
                }
                else if (this.WriteKind == StructWriteKind.Direct)
                {
                    this.WriteNavigation(this.SuppliedTarget!);
                }
                else if (this.WriteKind == StructWriteKind.Coercing)
                {
                    this.WriteNavigation((object)this.SuppliedTarget!);
                }
                else
                {
                    this.WriteNavigation("invalid");
                }
            }
            catch (Exception exception)
            {
                this.ActualException = exception;
            }

            this.ActualTarget = this.ClrKind == ClrMemberKind.Property ? _clrNavigationSource.Related : _clrNavigationSource.RelatedField;
            this.ActualId = this.Surface == MemberAccessSurface.Property
                ? this.ClrKind == ClrMemberKind.Property ? _clrValueSource.Number : _clrValueSource.NumberField
                : this.ActualTarget!.Id;

            this.WriteLine($"Actual Success:     {this.ActualSuccess.SafeToString()}; Actual Id: {this.ActualId}");

            if (this.ActualException is not null)
            {
                this.WriteLine();

                var actualExceptionType = this.ActualException.GetType();
                var actualExceptionMessage = this.ActualException.Message;
                this.WriteLine($"Actual Exception Type:    {actualExceptionType.SafeToName()}");
                this.WriteLine($"Actual Exception Message: {actualExceptionMessage.SafeToString()}");
            }
        }

        protected override void Assert()
        {
            this.ActualId.Should().Be(this.ExpectedId);
            if (this.UsesTry)
            {
                this.ActualSuccess.Should().Be(this.ExpectedSuccess);
                this.ActualException.Should().BeNull();
            }
            else if (this.ExpectedSuccess)
            {
                this.ActualException.Should().BeNull();
            }
            else
            {
                this.ActualException.Should().BeOfType<ApiSchemaException>().Which.InnerException.Should().NotBeNull();
            }

            if (this.Surface == MemberAccessSurface.Traversal)
            {
                this.ActualTarget.Should().BeSameAs(this.ExpectedSuccess ? this.SuppliedTarget : this.InitialTarget);
            }
        }
        #endregion

        #region Member Access Methods
        private void WriteValue<TValue>(TValue clrValue)
        {
            if (this.UsesTry)
            {
                this.ActualSuccess = this.Accessor!.TrySetValueByRef(ref _clrValueSource, clrValue);
            }
            else
            {
                this.Accessor!.SetValueByRef(ref _clrValueSource, clrValue);
            }
        }

        private void WriteNavigation<TValue>(TValue clrValue)
        {
            if (this.UsesTry)
            {
                this.ActualSuccess = this.Accessor!.TrySetValueByRef(ref _clrNavigationSource, clrValue);
            }
            else
            {
                this.Accessor!.SetValueByRef(ref _clrNavigationSource, clrValue);
            }
        }
        #endregion
    }
    #endregion

    #region Struct Write Theory Data
    public static TheoryDataRow<IXUnitTest>[] StructMemberWriteTheoryData =>
    [
        .. from surface in Enum.GetValues<MemberAccessSurface>()
           from clrKind in Enum.GetValues<ClrMemberKind>()
           from writeKind in Enum.GetValues<StructWriteKind>()
           from usesTry in new[] { false, true }
           select (TheoryDataRow<IXUnitTest>)new StructMemberWriteTest
           {
               Name = $"{surface}; {clrKind}; {writeKind}; Try={usesTry}",
               Surface = surface, ClrKind = clrKind, WriteKind = writeKind, UsesTry = usesTry,
               ExpectedSuccess = writeKind != StructWriteKind.Invalid,
               ExpectedId = writeKind == StructWriteKind.Invalid ? 7 : 42
           }
    ];
    #endregion

    #region Struct Write Test Methods
    [Theory]
    [MemberData(nameof(StructMemberWriteTheoryData))]
    public void SetValueByRefUpdatesOriginalStructMember(IXUnitTest test) => test.Execute(this);
    #endregion
}
