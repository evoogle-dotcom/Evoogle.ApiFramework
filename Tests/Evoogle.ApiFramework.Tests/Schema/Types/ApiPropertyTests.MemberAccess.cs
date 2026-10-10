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
    }

    private sealed class MemberAccessTest : XUnitTest
    {
        #region User Supplied Properties
        public required ClrMemberKind ClrKind { get; init; }

        public required string ClrName { get; init; }

        public required MemberAccessOperation Operation { get; init; }

        public required MemberAccessOverload Overload { get; init; }

        public required MemberAccessSurface Surface { get; init; }
        #endregion

        #region Calculated Properties
        private object? ActualValue { get; set; }

        private Exception? ActualException { get; set; }

        private long? ActualMemberValue { get; set; }

        private bool? ActualSuccess { get; set; }

        private ApiProperty? ApiProperty { get; set; }

        private ApiRelationshipTraversal? ApiTraversal { get; set; }

        private MemberAccessSource? ClrObject { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var isUnbound = this.Operation == MemberAccessOperation.UnboundMemberAccessUsesSurfaceSpecificExceptionBoundary;

            (this.ApiProperty, this.ApiTraversal) = CreateMemberAccessSurface
            (
                this.Surface,
                this.ClrKind,
                this.ClrName,
                isUnbound
            );
            this.ClrObject = new MemberAccessSource();

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

            this.WriteLine($"Actual Success:           {this.ActualSuccess.SafeToString()}");
            this.WriteLine($"Actual Return Value:      {this.ActualValue.SafeToString()}");
            this.WriteLine($"Resulting Member Value:   {this.ActualMemberValue.SafeToString()}");
            this.WriteLine($"Actual Exception Type:    {this.ActualException?.GetType().SafeToName()}");

            var actualExceptionMessage = this.ActualException?.Message;
            this.WriteLine($"Actual Exception Message: {actualExceptionMessage.SafeToString()}");
            this.WriteLine($"Root Exception:           {GetRootExceptionDescription(this.ActualException).SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            switch (this.Operation)
            {
                case MemberAccessOperation.GetValueUsesSchemaTypeCoercion:
                    this.ActualValue.Should().Be("7");
                    break;
                case MemberAccessOperation.SetValueUsesSchemaTypeCoercion:
                    this.ActualMemberValue.Should().Be(42);
                    break;
                case MemberAccessOperation.GetValueRejectsNullTarget:
                case MemberAccessOperation.SetValueRejectsNullTarget:
                    this.ActualException.Should().BeOfType<ArgumentNullException>();
                    break;
                case MemberAccessOperation.GetValueWrapsClrMemberFailure:
                    this.AssertWrappedClrMemberFailure("get", "Getter failure.");
                    break;
                case MemberAccessOperation.SetValueWrapsClrMemberFailure:
                    this.AssertWrappedClrMemberFailure("set", "Setter failure.");
                    this.ActualMemberValue.Should().Be(7);
                    break;
                case MemberAccessOperation.TryGetValueReturnsFalseForNullTarget:
                case MemberAccessOperation.TryGetValueReturnsFalseForClrMemberFailure:
                case MemberAccessOperation.TryGetValueReturnsFalseForCoercionFailure:
                    this.ActualSuccess.Should().BeFalse();
                    if (this.Overload == MemberAccessOverload.Object)
                    {
                        this.ActualValue.Should().BeNull();
                    }
                    else
                    {
                        this.ActualValue.Should().Be(default(long));
                    }

                    break;
                case MemberAccessOperation.TrySetValueReturnsFalseForClrMemberFailure:
                case MemberAccessOperation.TrySetValueReturnsFalseForCoercionFailure:
                    this.ActualSuccess.Should().BeFalse();
                    this.ActualMemberValue.Should().Be(7);
                    break;
                case MemberAccessOperation.UnboundMemberAccessUsesSurfaceSpecificExceptionBoundary:
                    var exception = this.ActualException.Should().BeOfType<ApiSchemaException>().Which;
                    exception.Message.Should().Contain(this.Surface == MemberAccessSurface.Property
                        ? "no compiled getter available"
                        : "Failed to get value for traversal");
                    if (this.Surface == MemberAccessSurface.Property)
                    {
                        exception.InnerException.Should().BeNull();
                    }
                    else
                    {
                        exception.InnerException.Should().BeOfType<ApiSchemaException>();
                    }

                    break;
                default:
                    throw new InvalidOperationException($"Unknown member access operation: {this.Operation}");
            }
        }
        #endregion

        #region Assertion Methods
        private void AssertWrappedClrMemberFailure(string operation, string rootMessage)
        {
            var exception = this.ActualException.Should().BeOfType<ApiSchemaException>().Which;
            exception.Message.Should().Contain($"Failed to {operation} value");
            exception.Message.Should().Contain(this.ClrName);
            exception.Message.Should().Contain(typeof(MemberAccessSource).Name);
            exception.InnerException.Should().NotBeNull();
            exception.GetBaseException().Should().BeOfType<InvalidOperationException>()
                .Which.Message.Should().Be(rootMessage);
        }
        #endregion

        #region Member Access Methods
        private object? GetValue(MemberAccessSource? clrObject, Type clrValueType)
        {
            if (this.Overload == MemberAccessOverload.Object)
            {
                return this.Surface == MemberAccessSurface.Property
                    ? this.ApiProperty!.GetValue(clrObject!, clrValueType)
                    : this.ApiTraversal!.GetValue(clrObject!, clrValueType);
            }

            return this.Surface == MemberAccessSurface.Property
                ? this.ApiProperty!.GetValue<MemberAccessSource, string>(clrObject!)
                : this.ApiTraversal!.GetValue<MemberAccessSource, string>(clrObject!);
        }

        private long GetMemberValue() => this.ClrKind == ClrMemberKind.Property
            ? this.ClrObject!.Number
            : this.ClrObject!.NumberField;

        private void SetValue<TValue>(MemberAccessSource? clrObject, TValue clrValue)
        {
            if (this.Overload == MemberAccessOverload.Object)
            {
                if (this.Surface == MemberAccessSurface.Property)
                {
                    this.ApiProperty!.SetValue(clrObject!, clrValue);
                }
                else
                {
                    this.ApiTraversal!.SetValue(clrObject!, clrValue);
                }

                return;
            }

            if (this.Surface == MemberAccessSurface.Property)
            {
                this.ApiProperty!.SetValue(clrObject!, clrValue);
            }
            else
            {
                this.ApiTraversal!.SetValue(clrObject!, clrValue);
            }
        }

        private bool TryGetValue(MemberAccessSource? clrObject, out object? clrValue)
        {
            if (this.Overload == MemberAccessOverload.Object)
            {
                return this.Surface == MemberAccessSurface.Property
                    ? this.ApiProperty!.TryGetValue(clrObject, out clrValue, typeof(long))
                    : this.ApiTraversal!.TryGetValue(clrObject, out clrValue, typeof(long));
            }

            long propertyValue;
            var isSuccess = this.Surface == MemberAccessSurface.Property
                ? this.ApiProperty!.TryGetValue(clrObject, out propertyValue)
                : this.ApiTraversal!.TryGetValue(clrObject, out propertyValue);
            clrValue = propertyValue;
            return isSuccess;
        }

        private bool TrySetValue<TValue>(MemberAccessSource? clrObject, TValue clrValue)
        {
            if (this.Overload == MemberAccessOverload.Object)
            {
                return this.Surface == MemberAccessSurface.Property
                    ? this.ApiProperty!.TrySetValue(clrObject, (object?)clrValue)
                    : this.ApiTraversal!.TrySetValue(clrObject, (object?)clrValue);
            }

            if (this.Surface == MemberAccessSurface.Property)
            {
                return this.ApiProperty!.TrySetValue(clrObject, clrValue);
            }

            return this.ApiTraversal!.TrySetValue(clrObject, clrValue);
        }
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
               MemberAccessOperation.TryGetValueReturnsFalseForClrMemberFailure =>
                   nameof(MemberAccessSource.ThrowingGet),
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
        ClrName = clrName
    };

    private static (ApiProperty? ApiProperty, ApiRelationshipTraversal? ApiTraversal) CreateMemberAccessSurface
    (
        MemberAccessSurface surface,
        ClrMemberKind clrKind,
        string clrName,
        bool isUnbound
    )
    {
        var clrMemberType = clrName == nameof(MemberAccessSource.Text) ? typeof(string) : typeof(long);
        var scalarType = new ApiScalarType(clrMemberType.Name, clrMemberType);
        if (surface == MemberAccessSurface.Property)
        {
            var property = new ApiProperty
            (
                "Value",
                new ApiTypeExpression(new ApiTypeReference(clrMemberType)),
                ApiTypeModifiers.Required,
                new ClrMemberReference(clrKind, clrName)
            );
            var objectType = new ApiObjectType
            (
                "Source",
                null,
                [property],
                null,
                null,
                isUnbound ? null! : typeof(MemberAccessSource)
            );
            var schema = new ApiSchema("MemberAccess", null, null, [scalarType], null, [objectType], null);
            var result = ApiSchemaCompiler.Compile(schema);
            if (!isUnbound)
            {
                result.ThrowIfInvalid();
            }

            return (property, null);
        }

        var clrNavigationMember = isUnbound ? null : new ClrMemberReference(clrKind, clrName);
        var traversal = new ApiRelationshipTraversal("Related", clrNavigationMember);
        var sourceType = new ApiObjectType("Source", null, null, null, null, typeof(MemberAccessSource));
        var targetType = new ApiObjectType("Target", null, null, null, null, clrMemberType);
        var relationship = new ApiRelationshipOneToOne
        (
            "SourceTarget",
            new ApiRelationshipPrincipalEnd(new ApiTypeReference(typeof(MemberAccessSource)), traversal),
            new ApiRelationshipDependentEnd(new ApiTypeReference(clrMemberType))
        );
        var traversalSchema = new ApiSchema
        (
            "MemberAccess",
            null,
            null,
            null,
            null,
            [sourceType, targetType],
            [relationship]
        );
        ApiSchemaCompiler.Compile(traversalSchema).ThrowIfInvalid();
        return (null, traversal);
    }

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
