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

public partial class ApiClrMemberAccessorTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Member Access Test Types
    private enum MemberAccessOperation
    {
        GetValueUsesSchemaTypeCoercion,
        SetValueUsesSchemaTypeCoercion,
        GetValueRejectsNullTarget,
        SetValueRejectsNullTarget,
        GetValueWrapsClrMemberFailure,
        SetValueWrapsClrMemberFailure,
        TryGetValueReturnsFalseForNullTarget,
        TryGetValueReturnsFalseForClrMemberFailure,
        TryGetValueReturnsFalseForCoercionFailure,
        TrySetValueReturnsFalseForClrMemberFailure,
        TrySetValueReturnsFalseForCoercionFailure,
        UnboundMemberAccessUsesSurfaceSpecificExceptionBoundary
    }

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
        private long _throwingSet = 7;

        public long Number { get; set; } = 7;

        public long NumberField = 7;

        public string Text { get; set; } = "invalid";

        public long ThrowingGet
        {
            get => throw new InvalidOperationException("Getter failure.");
            set => _throwingSet = value;
        }

        public long ThrowingSet
        {
            get => _throwingSet;
            set => throw new InvalidOperationException("Setter failure.");
        }
        #endregion
    }

    private sealed class MemberAccessTest : XUnitTest
    {
        #region User Supplied Properties
        public required ClrMemberKind ClrKind { get; init; }

        public required string ClrName { get; init; }

        public required MemberAccessOperation Operation { get; init; }

        public required MemberAccessOverload Overload { get; init; }

        public required MemberAccessSurface Surface { get; init; }
        public required object? ExpectedValue { get; init; }
        public required long? ExpectedMemberValue { get; init; }
        public required bool? ExpectedSuccess { get; init; }
        public required Type? ExpectedExceptionType { get; init; }
        public required Type? ExpectedInnerExceptionType { get; init; }
        public required Type? ExpectedRootExceptionType { get; init; }
        public required string[] ExpectedExceptionFragments { get; init; }
        public required string? ExpectedRootMessage { get; init; }
        #endregion

        #region Calculated Properties
        private object? ActualValue { get; set; }

        private Exception? ActualException { get; set; }

        private long? ActualMemberValue { get; set; }

        private bool? ActualSuccess { get; set; }

        private ApiClrMemberAccessor? Accessor { get; set; }

        private MemberAccessSource? ClrObject { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var isUnbound = this.Operation == MemberAccessOperation.UnboundMemberAccessUsesSurfaceSpecificExceptionBoundary;

            var memberInfo = this.ClrKind == ClrMemberKind.Property ? (System.Reflection.MemberInfo)typeof(MemberAccessSource).GetProperty(this.ClrName)! : typeof(MemberAccessSource).GetField(this.ClrName)!;
            var schema = new ApiSchema("MemberAccess", null, null, null, null, null, null);
            var description = this.Surface == MemberAccessSurface.Property
                ? ApiClrMemberAccessDescription.ForProperty(this.ClrName)
                : ApiClrMemberAccessDescription.ForTraversal("Related", this.ClrName);
            this.Accessor = new ApiClrMemberAccessor(isUnbound ? null : MemberAccessor.Create(memberInfo), new ApiSchemaContext(schema), description);
            this.ClrObject = new MemberAccessSource();

            this.WriteLine($"Expected Return:             {this.ExpectedValue.SafeToString()}; Member: {this.ExpectedMemberValue.SafeToString()}; Success: {this.ExpectedSuccess.SafeToString()}");
            this.WriteLine($"Scenario:                    {this.Operation.SafeToString()}");
            this.WriteLine($"Member Surface:              {this.Surface.SafeToString()}");
            this.WriteLine($"CLR Member Kind:             {this.ClrKind.SafeToString()}");
            this.WriteLine($"CLR Member Name:             {this.ClrName.SafeToString()}");
            this.WriteLine($"Overload:                    {this.Overload.SafeToString()}");
            this.WriteLine($"Source CLR Type:             {typeof(MemberAccessSource).SafeToName()}");
            this.WriteLine($"Initial CLR Member Value:    7");
            this.WriteLine($"Supplied Value:              {GetSuppliedValue(this.Operation).SafeToString()}");
            this.WriteLine($"Requested Output Type:       {GetRequestedOutputType(this.Operation, this.Overload).SafeToString()}");

            var expectedExceptionBoundary = GetExpectedExceptionBoundary(this.Operation, this.Surface);
            this.WriteLine($"Expected Exception Boundary: {expectedExceptionBoundary.SafeToString()}");

            this.WriteLine();
        }

        protected override void Act()
        {
            switch (this.Operation)
            {
                case MemberAccessOperation.GetValueUsesSchemaTypeCoercion:
                    this.ActualValue = this.GetValue(this.ClrObject!, typeof(string));
                    break;
                case MemberAccessOperation.SetValueUsesSchemaTypeCoercion:
                    this.SetValue(this.ClrObject!, "42");
                    this.ActualMemberValue = this.GetMemberValue();
                    break;
                case MemberAccessOperation.GetValueRejectsNullTarget:
                    this.ActualException = Record.Exception(() => this.GetValue(clrObject: null, typeof(string)));
                    break;
                case MemberAccessOperation.SetValueRejectsNullTarget:
                    this.ActualException = Record.Exception(() => this.SetValue(clrObject: null, "42"));
                    break;
                case MemberAccessOperation.GetValueWrapsClrMemberFailure:
                    this.ActualException = Record.Exception(() => this.GetValue(this.ClrObject!, typeof(long)));
                    break;
                case MemberAccessOperation.SetValueWrapsClrMemberFailure:
                    this.ActualException = Record.Exception(() => this.SetValue(this.ClrObject!, 42L));
                    this.ActualMemberValue = this.ClrObject!.ThrowingSet;
                    break;
                case MemberAccessOperation.TryGetValueReturnsFalseForNullTarget:
                    this.ActualSuccess = this.TryGetValue(clrObject: null, out var nullTargetValue);
                    this.ActualValue = nullTargetValue;
                    break;
                case MemberAccessOperation.TryGetValueReturnsFalseForClrMemberFailure:
                case MemberAccessOperation.TryGetValueReturnsFalseForCoercionFailure:
                    this.ActualSuccess = this.TryGetValue(this.ClrObject, out var clrValue);
                    this.ActualValue = clrValue;
                    break;
                case MemberAccessOperation.TrySetValueReturnsFalseForClrMemberFailure:
                    this.ActualSuccess = this.TrySetValue(this.ClrObject, 42L);
                    this.ActualMemberValue = this.ClrObject!.ThrowingSet;
                    break;
                case MemberAccessOperation.TrySetValueReturnsFalseForCoercionFailure:
                    this.ActualSuccess = this.TrySetValue(this.ClrObject, "invalid");
                    this.ActualMemberValue = this.GetMemberValue();
                    break;
                case MemberAccessOperation.UnboundMemberAccessUsesSurfaceSpecificExceptionBoundary:
                    this.ActualException = Record.Exception(() => this.GetValue(this.ClrObject!, typeof(long)));
                    break;
                default:
                    throw new InvalidOperationException($"Unknown member access operation: {this.Operation}");
            }

            var actualExceptionType = this.ActualException?.GetType();
            this.WriteLine($"Actual Success:           {this.ActualSuccess.SafeToString()}");
            this.WriteLine($"Actual Return Value:      {this.ActualValue.SafeToString()}");
            this.WriteLine($"Resulting Member Value:   {this.ActualMemberValue.SafeToString()}");
            this.WriteLine($"Actual Exception Type:    {actualExceptionType.SafeToName()}");

            var actualExceptionMessage = this.ActualException?.Message;
            this.WriteLine($"Actual Exception Message: {actualExceptionMessage.SafeToString()}");
            this.WriteLine($"Root Exception:           {GetRootExceptionDescription(this.ActualException).SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            this.ActualValue.Should().Be(this.ExpectedValue);
            this.ActualMemberValue.Should().Be(this.ExpectedMemberValue);
            this.ActualSuccess.Should().Be(this.ExpectedSuccess);
            if (this.ExpectedExceptionType is null)
            {
                this.ActualException.Should().BeNull();
                return;
            }

            this.ActualException.Should().BeOfType(this.ExpectedExceptionType);
            foreach (var fragment in this.ExpectedExceptionFragments)
            {
                this.ActualException.Message.Should().Contain(fragment);
            }

            if (this.ExpectedInnerExceptionType is null)
            {
                this.ActualException.InnerException.Should().BeNull();
            }
            else
            {
                this.ActualException.InnerException.Should().BeOfType(this.ExpectedInnerExceptionType);
            }

            if (this.ExpectedRootMessage is not null)
            {
                this.ActualException.GetBaseException().Should().BeOfType(this.ExpectedRootExceptionType);
                this.ActualException.GetBaseException().Message.Should().Be(this.ExpectedRootMessage);
            }
        }
        #endregion
        #region Member Access Methods
        private object? GetValue(MemberAccessSource? clrObject, Type clrValueType)
            => this.Overload == MemberAccessOverload.Object
                ? this.Accessor!.GetValue(clrObject!, clrValueType)
                : this.Accessor!.GetValue<MemberAccessSource, string>(clrObject!);

        private long GetMemberValue() => this.ClrKind == ClrMemberKind.Property
            ? this.ClrObject!.Number : this.ClrObject!.NumberField;

        private void SetValue<TValue>(MemberAccessSource? clrObject, TValue clrValue)
        {
            if (this.Overload == MemberAccessOverload.Object)
            {
                this.Accessor!.SetValue(clrObject!, clrValue);
            }
            else
            {
                this.Accessor!.SetValue(clrObject!, clrValue);
            }
        }

        private bool TryGetValue(MemberAccessSource? clrObject, out object? clrValue)
        {
            if (this.Overload == MemberAccessOverload.Object)
            {
                return this.Accessor!.TryGetValue(clrObject, out clrValue, typeof(long));
            }

            var isSuccess = this.Accessor!.TryGetValue<MemberAccessSource, long>(clrObject, out var memberValue);
            clrValue = memberValue;
            return isSuccess;
        }

        private bool TrySetValue<TValue>(MemberAccessSource? clrObject, TValue clrValue)
            => this.Overload == MemberAccessOverload.Object
                ? this.Accessor!.TrySetValue(clrObject, clrValue)
                : this.Accessor!.TrySetValue(clrObject, clrValue);
        #endregion
    }
    #endregion
    #region Member Access Theory Data
    public static TheoryDataRow<IXUnitTest>[] GetValueUsesSchemaTypeCoercionTheoryData =>
        CreateMemberAndOverloadTheoryData(MemberAccessOperation.GetValueUsesSchemaTypeCoercion);

    public static TheoryDataRow<IXUnitTest>[] SetValueUsesSchemaTypeCoercionTheoryData =>
        CreateMemberAndOverloadTheoryData(MemberAccessOperation.SetValueUsesSchemaTypeCoercion);

    public static TheoryDataRow<IXUnitTest>[] MemberAccessRejectsNullTargetTheoryData =>
    [
        .. from surface in Enum.GetValues<MemberAccessSurface>()
           from overload in Enum.GetValues<MemberAccessOverload>()
           from operation in new[]
           {
               MemberAccessOperation.GetValueRejectsNullTarget,
               MemberAccessOperation.SetValueRejectsNullTarget
           }
           select CreateMemberAccessTest
           (
               operation,
               surface,
               overload,
               ClrMemberKind.Property,
               nameof(MemberAccessSource.Number)
           )
    ];

    public static TheoryDataRow<IXUnitTest>[] MemberAccessWrapsClrMemberFailureTheoryData =>
    [
        .. from surface in Enum.GetValues<MemberAccessSurface>()
           from operation in new[]
           {
               MemberAccessOperation.GetValueWrapsClrMemberFailure,
               MemberAccessOperation.SetValueWrapsClrMemberFailure
           }
           let clrName = operation == MemberAccessOperation.GetValueWrapsClrMemberFailure
               ? nameof(MemberAccessSource.ThrowingGet)
               : nameof(MemberAccessSource.ThrowingSet)
           select CreateMemberAccessTest
           (
               operation,
               surface,
               MemberAccessOverload.Object,
               ClrMemberKind.Property,
               clrName
           )
    ];

    public static TheoryDataRow<IXUnitTest>[] TryGetValueReturnsFalseWhenAccessFailsTheoryData =>
    [
        .. from surface in Enum.GetValues<MemberAccessSurface>()
           from overload in Enum.GetValues<MemberAccessOverload>()
           from operation in new[]
           {
               MemberAccessOperation.TryGetValueReturnsFalseForNullTarget,
               MemberAccessOperation.TryGetValueReturnsFalseForClrMemberFailure,
               MemberAccessOperation.TryGetValueReturnsFalseForCoercionFailure
           }
           let clrName = operation switch
           {
               MemberAccessOperation.TryGetValueReturnsFalseForClrMemberFailure => nameof(MemberAccessSource.ThrowingGet),
               MemberAccessOperation.TryGetValueReturnsFalseForCoercionFailure => nameof(MemberAccessSource.Text),
               _ => nameof(MemberAccessSource.Number)
           }
           select CreateMemberAccessTest(operation, surface, overload, ClrMemberKind.Property, clrName)
    ];

    public static TheoryDataRow<IXUnitTest>[] TrySetValueReturnsFalseWithoutMutatingWhenAccessFailsTheoryData =>
    [
        .. from surface in Enum.GetValues<MemberAccessSurface>()
           from overload in Enum.GetValues<MemberAccessOverload>()
           from operation in new[]
           {
               MemberAccessOperation.TrySetValueReturnsFalseForClrMemberFailure,
               MemberAccessOperation.TrySetValueReturnsFalseForCoercionFailure
           }
           let clrName = operation == MemberAccessOperation.TrySetValueReturnsFalseForClrMemberFailure
               ? nameof(MemberAccessSource.ThrowingSet)
               : nameof(MemberAccessSource.Number)
           select CreateMemberAccessTest(operation, surface, overload, ClrMemberKind.Property, clrName)
    ];

    public static TheoryDataRow<IXUnitTest>[] UnboundMemberAccessUsesSurfaceSpecificExceptionBoundaryTheoryData =>
    [
        .. from surface in Enum.GetValues<MemberAccessSurface>()
           select CreateMemberAccessTest
           (
               MemberAccessOperation.UnboundMemberAccessUsesSurfaceSpecificExceptionBoundary,
               surface,
               MemberAccessOverload.Object,
               ClrMemberKind.Property,
               nameof(MemberAccessSource.Number)
           )
    ];
    #endregion

    #region Member Access Test Methods
    [Theory]
    [MemberData(nameof(GetValueUsesSchemaTypeCoercionTheoryData))]
    public void GetValueUsesSchemaTypeCoercion(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(SetValueUsesSchemaTypeCoercionTheoryData))]
    public void SetValueUsesSchemaTypeCoercion(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(MemberAccessRejectsNullTargetTheoryData))]
    public void MemberAccessRejectsNullTarget(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(MemberAccessWrapsClrMemberFailureTheoryData))]
    public void MemberAccessWrapsClrMemberFailure(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TryGetValueReturnsFalseWhenAccessFailsTheoryData))]
    public void TryGetValueReturnsFalseWhenAccessFails(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TrySetValueReturnsFalseWithoutMutatingWhenAccessFailsTheoryData))]
    public void TrySetValueReturnsFalseWithoutMutatingWhenAccessFails(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(UnboundMemberAccessUsesSurfaceSpecificExceptionBoundaryTheoryData))]
    public void UnboundMemberAccessUsesSurfaceSpecificExceptionBoundary(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Member Access Factory Methods
    private static TheoryDataRow<IXUnitTest>[] CreateMemberAndOverloadTheoryData(MemberAccessOperation operation) =>
    [
        .. from surface in Enum.GetValues<MemberAccessSurface>()
           from clrMember in new[]
           {
               (ClrMemberKind.Property, nameof(MemberAccessSource.Number)),
               (ClrMemberKind.Field, nameof(MemberAccessSource.NumberField))
           }
           from overload in Enum.GetValues<MemberAccessOverload>()
           select CreateMemberAccessTest(operation, surface, overload, clrMember.Item1, clrMember.Item2)
    ];

    private static MemberAccessTest CreateMemberAccessTest
    (
        MemberAccessOperation operation,
        MemberAccessSurface surface,
        MemberAccessOverload overload,
        ClrMemberKind clrKind,
        string clrName
    ) => new()
    {
        Name = $"{surface}; {clrKind}; {overload}; {operation}",
        Operation = operation,
        Surface = surface,
        Overload = overload,
        ClrKind = clrKind,
        ClrName = clrName,
        ExpectedValue = operation == MemberAccessOperation.GetValueUsesSchemaTypeCoercion ? "7"
            : operation is MemberAccessOperation.TryGetValueReturnsFalseForNullTarget
                or MemberAccessOperation.TryGetValueReturnsFalseForClrMemberFailure
                or MemberAccessOperation.TryGetValueReturnsFalseForCoercionFailure
                ? overload == MemberAccessOverload.Object ? null : 0L : null,
        ExpectedMemberValue = operation == MemberAccessOperation.SetValueUsesSchemaTypeCoercion ? 42
            : operation is MemberAccessOperation.SetValueWrapsClrMemberFailure
                or MemberAccessOperation.TrySetValueReturnsFalseForClrMemberFailure
                or MemberAccessOperation.TrySetValueReturnsFalseForCoercionFailure ? 7 : null,
        ExpectedSuccess = operation is MemberAccessOperation.TryGetValueReturnsFalseForNullTarget
            or MemberAccessOperation.TryGetValueReturnsFalseForClrMemberFailure
            or MemberAccessOperation.TryGetValueReturnsFalseForCoercionFailure
            or MemberAccessOperation.TrySetValueReturnsFalseForClrMemberFailure
            or MemberAccessOperation.TrySetValueReturnsFalseForCoercionFailure ? false : null,
        ExpectedExceptionType = operation is MemberAccessOperation.GetValueRejectsNullTarget
            or MemberAccessOperation.SetValueRejectsNullTarget ? typeof(ArgumentNullException)
            : operation is MemberAccessOperation.GetValueWrapsClrMemberFailure
                or MemberAccessOperation.SetValueWrapsClrMemberFailure
                or MemberAccessOperation.UnboundMemberAccessUsesSurfaceSpecificExceptionBoundary
                ? typeof(ApiSchemaException) : null,
        ExpectedInnerExceptionType = operation is MemberAccessOperation.GetValueWrapsClrMemberFailure
            or MemberAccessOperation.SetValueWrapsClrMemberFailure ? typeof(MemberAccess.MemberAccessException)
            : operation == MemberAccessOperation.UnboundMemberAccessUsesSurfaceSpecificExceptionBoundary
                && surface == MemberAccessSurface.Traversal ? typeof(ApiSchemaException) : null,
        ExpectedRootExceptionType = operation is MemberAccessOperation.GetValueWrapsClrMemberFailure
            or MemberAccessOperation.SetValueWrapsClrMemberFailure ? typeof(InvalidOperationException) : null,
        ExpectedExceptionFragments = operation is MemberAccessOperation.GetValueWrapsClrMemberFailure
            or MemberAccessOperation.SetValueWrapsClrMemberFailure
            ? [clrName, typeof(MemberAccessSource).Name,
                operation == MemberAccessOperation.GetValueWrapsClrMemberFailure ? "Failed to get" : "Failed to set"]
            : operation == MemberAccessOperation.UnboundMemberAccessUsesSurfaceSpecificExceptionBoundary
                ? [clrName, surface == MemberAccessSurface.Property
                    ? "no compiled getter available" : "Failed to get value for traversal"] : [],
        ExpectedRootMessage = operation == MemberAccessOperation.GetValueWrapsClrMemberFailure ? "Getter failure."
            : operation == MemberAccessOperation.SetValueWrapsClrMemberFailure ? "Setter failure." : null
    };

    private static string GetExpectedExceptionBoundary
    (
        MemberAccessOperation operation,
        MemberAccessSurface surface
    ) => operation switch
    {
        MemberAccessOperation.GetValueRejectsNullTarget or
        MemberAccessOperation.SetValueRejectsNullTarget => nameof(ArgumentNullException),
        MemberAccessOperation.GetValueWrapsClrMemberFailure or
        MemberAccessOperation.SetValueWrapsClrMemberFailure => nameof(ApiSchemaException),
        MemberAccessOperation.UnboundMemberAccessUsesSurfaceSpecificExceptionBoundary =>
            surface == MemberAccessSurface.Property
                ? $"{nameof(ApiSchemaException)} (direct)"
                : $"{nameof(ApiSchemaException)} (contextual outer and unavailable-accessor inner)",
        _ => "none"
    };

    private static string GetRequestedOutputType
    (
        MemberAccessOperation operation,
        MemberAccessOverload overload
    ) => operation switch
    {
        MemberAccessOperation.GetValueUsesSchemaTypeCoercion => typeof(string).SafeToName(),
        MemberAccessOperation.TryGetValueReturnsFalseForNullTarget or
        MemberAccessOperation.TryGetValueReturnsFalseForClrMemberFailure or
        MemberAccessOperation.TryGetValueReturnsFalseForCoercionFailure => typeof(long).SafeToName(),
        MemberAccessOperation.GetValueRejectsNullTarget or
        MemberAccessOperation.GetValueWrapsClrMemberFailure or
        MemberAccessOperation.UnboundMemberAccessUsesSurfaceSpecificExceptionBoundary =>
            (overload == MemberAccessOverload.Object ? typeof(long) : typeof(string)).SafeToName(),
        _ => "<none>"
    };

    private static string GetSuppliedValue(MemberAccessOperation operation) => operation switch
    {
        MemberAccessOperation.SetValueUsesSchemaTypeCoercion or
        MemberAccessOperation.SetValueRejectsNullTarget => "42 (String)",
        MemberAccessOperation.SetValueWrapsClrMemberFailure or
        MemberAccessOperation.TrySetValueReturnsFalseForClrMemberFailure => "42 (Int64)",
        MemberAccessOperation.TrySetValueReturnsFalseForCoercionFailure => "invalid (String)",
        _ => "<none>"
    };

    private static string GetRootExceptionDescription(Exception? exception)
    {
        if (exception is null)
        {
            return "<none>";
        }

        var rootException = exception.GetBaseException();
        return $"{rootException.GetType().SafeToName()}: {rootException.Message}";
    }
    #endregion
}
