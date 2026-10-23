// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Types.Internal;

public class ClrMemberBindingTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class BindingSource
    {
        #region Fields and Properties
        public long ValueField = 7;
        public long Value { get; set; } = 7;
        public long ReadOnly { get; } = 7;
        public long WriteOnly { set => this.ValueField = value; }
        public long this[int index]
        {
            get => this.Value + index;
            set => this.Value = value;
        }
        #endregion
    }

    private ref struct RefLikeSource
    {
        #region Constructors
        public RefLikeSource()
        {
            this.SpanField = default;
        }
        #endregion

        #region Fields and Properties
        public Span<byte> SpanField;
        public Span<byte> SpanProperty { get; set; }
        #endregion
    }

    private sealed unsafe class PointerSource
    {
        #region Fields and Properties
        public byte* PointerField = null;
        public byte* PointerProperty { get; set; }
        #endregion
    }

    private sealed record ExpectedDiagnostic(ApiSchemaCompilationCode Code, ApiSchemaCompilationSeverity Severity, string ApiPath);

    private sealed record ResolutionCase
    (
        Type ClrType, ClrMemberKind ClrKind, string ClrName, Type? ExpectedMemberType,
        bool RequiresRead, bool RequiresWrite, bool ExpectedIsBound, bool ExpectedHasAccessor,
        bool ExpectedCanRead, bool ExpectedCanWrite, ApiSchemaCompilationCode[] ExpectedCodes,
        string[] ExpectedDescriptionFragments
    );

    private sealed class ResolutionTest : XUnitTest
    {
        #region User Supplied Properties
        public required ResolutionCase Case { get; init; }
        public required string? ApiPathOverride { get; init; }
        public required ExpectedDiagnostic[] ExpectedIssues { get; init; }
        #endregion

        #region Calculated Properties
        private ClrMemberBinding? Binding { get; set; }
        private ApiSchemaCompilationContext? Context { get; set; }
        private bool ActualSuccess { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Binding = new ClrMemberBinding(new ClrMemberReference(this.Case.ClrKind, this.Case.ClrName));
            this.Context = CreateContext();

            this.WriteLine($"CLR Type: {this.Case.ClrType.SafeToName()}; Member: {this.Case.ClrKind}.{this.Case.ClrName}");
            this.WriteLine($"Requires Read: {this.Case.RequiresRead}; Write: {this.Case.RequiresWrite}; Path: {this.ApiPathOverride ?? this.Context.ApiPath}");
            this.WriteLine($"Expected Bound: {this.Case.ExpectedIsBound}; Accessor: {this.Case.ExpectedHasAccessor}; Expected Boundary: none");
            foreach (var issue in this.ExpectedIssues)
            {
                this.WriteLine($"Expected: {issue.Severity}; {issue.Code}; {issue.ApiPath}");
            }
        }

        protected override void Act()
        {
            this.ActualSuccess = this.Binding!.TryResolveReference
            (
                clrObjectType: this.Case.ClrType,
                context: this.Context!,
                referenceName: "Test CLR member reference",
                requiresRead: this.Case.RequiresRead,
                requiresWrite: this.Case.RequiresWrite,
                apiPath: this.ApiPathOverride
            );

            this.WriteLine($"Resolution Success: {this.ActualSuccess}; Bound: {this.Binding.IsBound}; Accessor Available: {this.Binding.BoundClrMemberAccessor is not null}");
            foreach (var issue in this.Context!.Issues)
            {
                this.WriteLine($"Actual: {issue.Severity}; {issue.Code}; {issue.ApiPath}; {issue.Description}");
            }
        }

        protected override void Assert()
        {
            this.ActualSuccess.Should().Be(this.Case.ExpectedIsBound);
            this.Binding!.IsBound.Should().Be(this.Case.ExpectedIsBound);
            this.Binding.HasReference.Should().BeTrue();
            this.Context!.Issues.Select(issue => new ExpectedDiagnostic(issue.Code, issue.Severity, issue.ApiPath)).Should().BeEquivalentTo(this.ExpectedIssues);
            foreach (var fragment in this.Case.ExpectedDescriptionFragments)
            {
                this.Context.Issues.Should().Contain(issue => issue.Description.Contains(fragment));
            }

            if (this.Case.ExpectedIsBound)
            {
                this.Binding.ClrMemberInfo.Name.Should().Be(this.Case.ClrName);
                this.Binding.ClrMemberInfo.DeclaringType.Should().Be(this.Case.ClrType);
                this.Binding.ClrMemberType.Should().Be(this.Case.ExpectedMemberType);
                this.Binding.ClrMemberInfo.Should().BeSameAs(this.Binding.BoundClrMemberInfo);
            }
            else
            {
                this.Binding.BoundClrMemberInfo.Should().BeNull();
            }

            if (this.Case.ExpectedHasAccessor)
            {
                var accessor = this.Binding.ClrMemberAccessor;
                accessor.Should().BeSameAs(this.Binding.BoundClrMemberAccessor);
                accessor.MemberInfo.Should().BeSameAs(this.Binding.ClrMemberInfo);
                accessor.MemberType.Should().Be(this.Case.ExpectedMemberType);
                accessor.CanRead.Should().Be(this.Case.ExpectedCanRead);
                accessor.CanWrite.Should().Be(this.Case.ExpectedCanWrite);
            }
            else
            {
                this.Binding.BoundClrMemberAccessor.Should().BeNull();
            }
        }
        #endregion
    }

    private sealed class BindingStateTest : XUnitTest
    {
        #region User Supplied Properties
        public required ClrMemberKind ClrKind { get; init; }
        public required bool UsesReference { get; init; }
        public required bool IsFirstResolutionMissing { get; init; }
        public required bool ShouldInvalidate { get; init; }
        public required bool ShouldRepeat { get; init; }
        public required bool ExpectedIsBound { get; init; }
        public required bool ExpectedHasAccessor { get; init; }
        public required Type? ExpectedExceptionType { get; init; }
        public required ExpectedDiagnostic[] ExpectedIssues { get; init; }
        #endregion

        #region Calculated Properties
        private ClrMemberBinding? Binding { get; set; }
        private ApiSchemaCompilationContext? Context { get; set; }
        private MemberInfo? ExpectedMember { get; set; }
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var clrName = this.IsFirstResolutionMissing ? "Missing" : this.ClrKind == ClrMemberKind.Property ? "Value" : "ValueField";
            this.ExpectedMember = this.ClrKind == ClrMemberKind.Property ? typeof(BindingSource).GetProperty(clrName) : typeof(BindingSource).GetField(clrName);
            this.Binding = new ClrMemberBinding(this.UsesReference ? new ClrMemberReference(this.ClrKind, clrName) : null);
            this.Context = CreateContext();

            this.WriteLine($"Member: {this.ClrKind}.{clrName}; Uses Reference: {this.UsesReference}; Invalidate: {this.ShouldInvalidate}; Repeat: {this.ShouldRepeat}");
            this.WriteLine($"Expected Bound: {this.ExpectedIsBound}; Accessor: {this.ExpectedHasAccessor}; Boundary: {this.ExpectedExceptionType?.Name ?? "none"}");
            foreach (var issue in this.ExpectedIssues)
            {
                this.WriteLine($"Expected: {issue.Severity}; {issue.Code}; {issue.ApiPath}");
            }
        }

        protected override void Act()
        {
            this.BindMember();
            if (this.ShouldInvalidate)
            {
                this.Binding!.InvalidateAccessor();
            }

            if (this.ShouldRepeat)
            {
                try
                {
                    this.BindMember();
                }
                catch (Exception exception)
                {
                    this.ActualException = exception;
                }
            }

            this.WriteLine($"Bound: {this.Binding!.IsBound}; Accessor Available: {this.Binding.BoundClrMemberAccessor is not null}");
            this.WriteLine($"Captured Exception: {this.ActualException.SafeToString()}");
            foreach (var issue in this.Context!.Issues)
            {
                this.WriteLine($"Actual: {issue.Severity}; {issue.Code}; {issue.ApiPath}");
            }
        }

        protected override void Assert()
        {
            this.Binding!.HasReference.Should().Be(this.UsesReference);
            this.Binding.IsBound.Should().Be(this.ExpectedIsBound);
            this.Binding.BoundClrMemberInfo.Should().BeSameAs(this.ExpectedMember);
            (this.Binding.BoundClrMemberAccessor is not null).Should().Be(this.ExpectedHasAccessor);
            this.Context!.Issues.Select(issue => new ExpectedDiagnostic(issue.Code, issue.Severity, issue.ApiPath)).Should().BeEquivalentTo(this.ExpectedIssues);
            if (this.ExpectedIsBound)
            {
                this.Binding.ClrMemberInfo.Should().BeSameAs(this.ExpectedMember);
                this.Binding.ClrMemberType.Should().Be<long>();
            }

            if (this.ExpectedHasAccessor)
            {
                this.Binding.ClrMemberAccessor.MemberInfo.Should().BeSameAs(this.ExpectedMember);
                this.Binding.ClrMemberAccessor.MemberType.Should().Be<long>();
            }

            if (this.ExpectedExceptionType is null)
            {
                this.ActualException.Should().BeNull();
            }
            else
            {
                this.ActualException.Should().BeOfType(this.ExpectedExceptionType);
                this.ActualException!.Message.Should().Contain("can only be bound once");
            }
        }
        #endregion

        #region Binding Methods
        private void BindMember()
        {
            if (this.UsesReference)
            {
                this.Binding!.TryResolveReference
                (
                    clrObjectType: typeof(BindingSource),
                    context: this.Context!,
                    referenceName: "Test CLR member reference",
                    requiresRead: false,
                    requiresWrite: false
                );
            }
            else
            {
                this.Binding!.Bind(clrMemberInfo: this.ExpectedMember!);
            }
        }
        #endregion
    }
    #endregion

    #region Test Properties
    private const string _contextApiPath = "ApiSchema[\"Binding\"].ApiObjectType[\"Source\"]";
    private const string _memberApiPath = _contextApiPath + ".ApiProperty[\"Value\"]";

    private static ResolutionCase[] ValidCases =>
    [
        new(typeof(BindingSource), ClrMemberKind.Property, "Value", typeof(long), false, false, true, true, true, true, [], []),
        new(typeof(BindingSource), ClrMemberKind.Field, "ValueField", typeof(long), false, false, true, true, true, true, [], [])
    ];

    private static ResolutionCase[] RequiredAccessCases =>
    [
        new(typeof(BindingSource), ClrMemberKind.Property, "ReadOnly", typeof(long), true, false, true, true, true, false, [], []),
        new(typeof(BindingSource), ClrMemberKind.Property, "WriteOnly", typeof(long), false, true, true, true, false, true, [], []),
        new(typeof(BindingSource), ClrMemberKind.Property, "ReadOnly", typeof(long), false, false, true, true, true, false, [], []),
        new(typeof(BindingSource), ClrMemberKind.Property, "ReadOnly", typeof(long), true, true, true, false, false, false, [ApiSchemaCompilationCode.ClrMemberIncompatible], ["must be readable and writable"]),
        new(typeof(BindingSource), ClrMemberKind.Property, "WriteOnly", typeof(long), true, true, true, false, false, false, [ApiSchemaCompilationCode.ClrMemberIncompatible], ["must be readable and writable"])
    ];

    private static ResolutionCase[] UnsupportedShapeCases =>
    [
        new(typeof(RefLikeSource), ClrMemberKind.Field, "SpanField", typeof(Span<byte>),false, false, true, false, false, false, [ApiSchemaCompilationCode.ClrMemberIncompatible], ["unsupported by-ref-like"]),
        new(typeof(RefLikeSource), ClrMemberKind.Property, "SpanProperty", typeof(Span<byte>),false, false, true, false, false, false, [ApiSchemaCompilationCode.ClrMemberIncompatible], ["unsupported by-ref-like"]),
        new(typeof(BindingSource), ClrMemberKind.Property, "Item", typeof(long), false, false, true, false, false, false, [ApiSchemaCompilationCode.ClrMemberIncompatible], ["is an indexer"])
    ];

    private static ResolutionCase[] InvalidAccessorCases =>
    [
        .. from clrKind in Enum.GetValues<ClrMemberKind>()
           let clrName = clrKind == ClrMemberKind.Property ? "PointerProperty" : "PointerField"
           select new ResolutionCase(typeof(PointerSource), clrKind, clrName, typeof(byte).MakePointerType(),
               false, false, true, false, false, false,
               [ApiSchemaCompilationCode.ClrMemberInvalidGetter, ApiSchemaCompilationCode.ClrMemberInvalidSetter],
               ["Failed to compile getter", "Failed to compile setter"])
    ];
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] ResolutionTheoryData => CreateResolutionTheoryData(ValidCases);
    public static TheoryDataRow<IXUnitTest>[] RequiredAccessTheoryData => CreateResolutionTheoryData(RequiredAccessCases);
    public static TheoryDataRow<IXUnitTest>[] UnsupportedShapeTheoryData => CreateResolutionTheoryData(UnsupportedShapeCases);
    public static TheoryDataRow<IXUnitTest>[] InvalidAccessorTheoryData => CreateResolutionTheoryData(InvalidAccessorCases);

    public static TheoryDataRow<IXUnitTest>[] DirectBindingTheoryData => CreateBindingStateTheoryData(usesReference: false, shouldInvalidate: false, shouldRepeat: false);

    public static TheoryDataRow<IXUnitTest>[] InvalidationTheoryData =>
    [
        .. CreateBindingStateTheoryData(usesReference: false, shouldInvalidate: true, shouldRepeat: false),
        .. CreateBindingStateTheoryData(usesReference: true, shouldInvalidate: true, shouldRepeat: false)
    ];

    public static TheoryDataRow<IXUnitTest>[] RepeatedBindingTheoryData =>
    [
        .. CreateBindingStateTheoryData(usesReference: false, shouldInvalidate: false, shouldRepeat: true),
        .. CreateBindingStateTheoryData(usesReference: true, shouldInvalidate: false, shouldRepeat: true),
        .. CreateBindingStateTheoryData(usesReference: true, shouldInvalidate: false, shouldRepeat: true, isFirstResolutionMissing: true)
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(DirectBindingTheoryData))]
    public void BindPublishesClrMemberMetadataAndAccessor(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ResolutionTheoryData))]
    public void TryResolveReferencePublishesClrMemberMetadataAndAccessor(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(RequiredAccessTheoryData))]
    public void TryResolveReferenceHonorsRequiredReadAndWriteAccess(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(UnsupportedShapeTheoryData))]
    public void TryResolveReferenceRejectsUnsupportedMemberShapes(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(InvalidAccessorTheoryData))]
    public void TryResolveReferenceReportsGetterAndSetterCompilationFailures(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(RepeatedBindingTheoryData))]
    public void BindingCanOnlyBeAttemptedOnce(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(InvalidationTheoryData))]
    public void InvalidateAccessorPreservesResolvedMemberMetadata(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Factory Methods
    private static ApiSchemaCompilationContext CreateContext()
    {
        var schema = new ApiSchema("Binding", null, null, null, null, null, null, null);
        var session = new ApiSchemaCompilationSession(schema, new ApiSchemaContext(schema));
        return new ApiSchemaCompilationContext(session, schema, default, _contextApiPath);
    }

    private static TheoryDataRow<IXUnitTest>[] CreateResolutionTheoryData(ResolutionCase[] cases) =>
    [
        .. from resolutionCase in cases
           from apiPathOverride in new[] { null, _memberApiPath }
           select (TheoryDataRow<IXUnitTest>)new ResolutionTest
           {
               Name = $"{resolutionCase.ClrKind}.{resolutionCase.ClrName}; Read={resolutionCase.RequiresRead}; Write={resolutionCase.RequiresWrite}; Override={apiPathOverride is not null}",
               Case = resolutionCase, ApiPathOverride = apiPathOverride,
               ExpectedIssues = [.. resolutionCase.ExpectedCodes.Select(code => new ExpectedDiagnostic(code, ApiSchemaCompilationSeverity.Error, apiPathOverride ?? _contextApiPath))]
           }
    ];

    private static TheoryDataRow<IXUnitTest>[] CreateBindingStateTheoryData(bool usesReference,
        bool shouldInvalidate, bool shouldRepeat, bool isFirstResolutionMissing = false) =>
    [
        .. Enum.GetValues<ClrMemberKind>().Select(clrKind => (TheoryDataRow<IXUnitTest>)new BindingStateTest
        {
            Name = $"{clrKind}; Reference={usesReference}; Missing={isFirstResolutionMissing}; Invalidate={shouldInvalidate}; Repeat={shouldRepeat}",
            ClrKind = clrKind, UsesReference = usesReference, IsFirstResolutionMissing = isFirstResolutionMissing,
            ShouldInvalidate = shouldInvalidate, ShouldRepeat = shouldRepeat,
            ExpectedIsBound = !isFirstResolutionMissing,
            ExpectedHasAccessor = !isFirstResolutionMissing && !shouldInvalidate,
            ExpectedExceptionType = shouldRepeat ? typeof(ApiSchemaConfigurationException) : null,
            ExpectedIssues = isFirstResolutionMissing ? [new ExpectedDiagnostic(ApiSchemaCompilationCode.ClrMemberReferenceUnresolved, ApiSchemaCompilationSeverity.Error, _contextApiPath)] : []
        })
    ];
    #endregion
}
