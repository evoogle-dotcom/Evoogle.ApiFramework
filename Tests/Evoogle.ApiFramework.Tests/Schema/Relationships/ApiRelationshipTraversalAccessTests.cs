// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Relationships;

public class ApiRelationshipTraversalAccessTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class Target
    {
        public string Name { get; init; } = string.Empty;
    }

    private sealed class Source
    {
        public Target? Related { get; set; }

        public Target? RelatedField;

        public List<Target> RelatedItems { get; set; } = [];

        public List<Target> RelatedItemField = [];
    }

    private struct StructSource
    {
        public Target? Related;
    }

    private sealed class NumericSource
    {
        public long Related { get; set; }
    }

    private sealed class InvalidSource
    {
        public Target? ReadOnly => null;

        public Target? WriteOnly
        {
            set
            { }
        }

        public string Incompatible { get; set; } = string.Empty;

        public Target? this[int index]
        {
            get => null;
            set
            { }
        }
    }

    private enum AccessScenario
    {
        ObjectProperty,
        ObjectField,
        GenericProperty,
        Coercion,
        TryProperty,
        InvalidValue,
        TryInvalidValue,
        ToManyProperty,
        ToManyField,
        StructByReference,
        TryStructByReference,
        MissingBinding,
        BeforeCompilation,
        InvalidSource
    }

    private enum CompilationScenario
    {
        InvalidName,
        Missing,
        ReadOnly,
        WriteOnly,
        Incompatible,
        Indexer
    }

    private sealed class AccessTest : XUnitTest
    {
        public required AccessScenario Scenario { get; init; }

        private object? ActualValue { get; set; }

        private bool ActualSuccess { get; set; }

        private Exception? ActualException { get; set; }

        protected override void Arrange()
        { }

        protected override void Act()
        {
            switch (this.Scenario)
            {
                case AccessScenario.ObjectProperty:
                    this.AccessObjectNavigationMember(ClrMemberKind.Property, nameof(Source.Related));
                    break;
                case AccessScenario.ObjectField:
                    this.AccessObjectNavigationMember(ClrMemberKind.Field, nameof(Source.RelatedField));
                    break;
                case AccessScenario.GenericProperty:
                    this.AccessGenericMember();
                    break;
                case AccessScenario.Coercion:
                    this.AccessCoercibleMember();
                    break;
                case AccessScenario.TryProperty:
                    this.TryAccessMember();
                    break;
                case AccessScenario.InvalidValue:
                    this.AccessInvalidValue();
                    break;
                case AccessScenario.TryInvalidValue:
                    this.TrySetInvalidValue();
                    break;
                case AccessScenario.ToManyProperty:
                    this.AccessCollectionNavigationMember(ClrMemberKind.Property, nameof(Source.RelatedItems));
                    break;
                case AccessScenario.ToManyField:
                    this.AccessCollectionNavigationMember(ClrMemberKind.Field, nameof(Source.RelatedItemField));
                    break;
                case AccessScenario.StructByReference:
                    this.AccessStructByReference(useTrySet: false);
                    break;
                case AccessScenario.TryStructByReference:
                    this.AccessStructByReference(useTrySet: true);
                    break;
                case AccessScenario.MissingBinding:
                    this.AccessMissingBinding();
                    break;
                case AccessScenario.BeforeCompilation:
                    this.AccessBeforeCompilation();
                    break;
                case AccessScenario.InvalidSource:
                    this.AccessInvalidSource();
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        protected override void Assert()
        {
            switch (this.Scenario)
            {
                case AccessScenario.ObjectProperty:
                case AccessScenario.ObjectField:
                case AccessScenario.GenericProperty:
                case AccessScenario.ToManyProperty:
                case AccessScenario.ToManyField:
                    this.ActualValue.Should().Be("updated");
                    break;
                case AccessScenario.Coercion:
                    this.ActualValue.Should().Be("42");
                    break;
                case AccessScenario.TryProperty:
                case AccessScenario.StructByReference:
                case AccessScenario.TryStructByReference:
                    this.ActualSuccess.Should().BeTrue();
                    this.ActualValue.Should().Be("updated");
                    break;
                case AccessScenario.TryInvalidValue:
                    this.ActualSuccess.Should().BeFalse();
                    this.ActualValue.Should().Be("original");
                    break;
                case AccessScenario.MissingBinding:
                case AccessScenario.BeforeCompilation:
                    this.ActualException.Should().BeOfType<ApiSchemaException>();
                    this.ActualException!.Message.Should().Contain("related");
                    this.ActualException.Message.Should().Contain(typeof(Source).Name);
                    if (this.Scenario == AccessScenario.BeforeCompilation)
                    {
                        this.ActualException.Message.Should().Contain(typeof(Target).Name);
                    }
                    this.ActualSuccess.Should().BeFalse();
                    this.ActualValue.Should().BeNull();
                    break;
                case AccessScenario.InvalidSource:
                    this.ActualException.Should().BeOfType<ApiSchemaException>();
                    this.ActualException!.Message.Should().Contain("related");
                    this.ActualException.Message.Should().Contain(nameof(Source.Related));
                    this.ActualException.Message.Should().Contain(typeof(object).Name);
                    break;
                case AccessScenario.InvalidValue:
                    this.ActualException.Should().BeOfType<ApiSchemaException>();
                    this.ActualException!.Message.Should().Contain("related");
                    this.ActualException.Message.Should().Contain(nameof(Source.Related));
                    this.ActualException.Message.Should().Contain(typeof(Source).Name);
                    this.ActualException.Message.Should().Contain(typeof(string).Name);
                    this.ActualValue.Should().Be("original");
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void AccessCollectionNavigationMember(ClrMemberKind clrNavigationMemberKind, string clrNavigationMemberName)
        {
            var traversal = CreateCompiledTraversal<Source>
            (
                clrNavigationMemberKind,
                clrNavigationMemberName,
                isToMany: true
            );
            var source = new Source
            {
                RelatedItems = [new Target { Name = "original" }],
                RelatedItemField = [new Target { Name = "original" }]
            };
            var updated = new List<Target> { new() { Name = "updated" } };

            traversal.SetValue(source, updated);
            this.ActualValue = ((IEnumerable<Target>)traversal.GetValue(source)!).Single().Name;
        }

        private void AccessGenericMember()
        {
            var traversal = CreateCompiledTraversal<Source>
            (
                ClrMemberKind.Property,
                nameof(Source.Related),
                isToMany: false
            );
            var source = new Source { Related = new Target { Name = "original" } };

            traversal.SetValue<Source, Target>(source, new Target { Name = "updated" });
            this.ActualValue = traversal.GetValue<Source, Target>(source)!.Name;
        }

        private void AccessCoercibleMember()
        {
            var traversal = CreateCompiledNumericTraversal();
            var source = new NumericSource { Related = 1 };

            traversal.SetValue<NumericSource, string>(source, "42");
            this.ActualValue = traversal.GetValue<NumericSource, string>(source);
        }

        private void AccessInvalidSource()
        {
            var traversal = CreateCompiledTraversal<Source>
            (
                ClrMemberKind.Property,
                nameof(Source.Related),
                isToMany: false
            );

            this.ActualException = Record.Exception(() => traversal.GetValue(new object()));
        }

        private void AccessInvalidValue()
        {
            var traversal = CreateCompiledTraversal<Source>
            (
                ClrMemberKind.Property,
                nameof(Source.Related),
                isToMany: false
            );
            var source = new Source { Related = new Target { Name = "original" } };

            this.ActualException = Record.Exception(() => traversal.SetValue(source, "invalid"));
            this.ActualValue = source.Related.Name;
        }

        private void AccessMissingBinding()
        {
            var traversal = CreateCompiledTraversal<Source>
            (
                clrNavigationMemberKind: null,
                clrNavigationMemberName: null,
                isToMany: false
            );
            var source = new Source();

            this.ActualException = Record.Exception(() => traversal.GetValue(source));
            this.ActualSuccess = traversal.TryGetValue(source, out var value);
            this.ActualValue = value;
        }

        private void AccessObjectNavigationMember(ClrMemberKind clrNavigationMemberKind, string clrNavigationMemberName)
        {
            var traversal = CreateCompiledTraversal<Source>(clrNavigationMemberKind, clrNavigationMemberName, isToMany: false);
            var source = new Source
            {
                Related = new Target { Name = "original" },
                RelatedField = new Target { Name = "original" }
            };

            traversal.SetValue(source, new Target { Name = "updated" });
            this.ActualValue = ((Target)traversal.GetValue(source, typeof(object))!).Name;
        }

        private void AccessBeforeCompilation()
        {
            var traversal = new ApiRelationshipTraversal
            (
                "related",
                new ClrMemberReference(ClrMemberKind.Property, nameof(Source.Related))
            );
            var source = new Source();

            this.ActualException = Record.Exception(() => traversal.SetValue(source, new Target()));
            this.ActualSuccess = traversal.TrySetValue(source, new Target());
        }

        private void AccessStructByReference(bool useTrySet)
        {
            var traversal = CreateCompiledTraversal<StructSource>
            (
                ClrMemberKind.Field,
                nameof(StructSource.Related),
                isToMany: false
            );
            var source = new StructSource { Related = new Target { Name = "original" } };
            var updated = new Target { Name = "updated" };

            if (useTrySet)
            {
                this.ActualSuccess = traversal.TrySetValueByRef(ref source, updated);
            }
            else
            {
                traversal.SetValueByRef(ref source, updated);
                this.ActualSuccess = true;
            }

            this.ActualValue = source.Related!.Name;
        }

        private void TryAccessMember()
        {
            var traversal = CreateCompiledTraversal<Source>
            (
                ClrMemberKind.Property,
                nameof(Source.Related),
                isToMany: false
            );
            var source = new Source { Related = new Target { Name = "original" } };

            var isSet = traversal.TrySetValue(source, new Target { Name = "updated" });
            var isGet = traversal.TryGetValue<Source, Target>(source, out var target);
            this.ActualSuccess = isSet && isGet;
            this.ActualValue = target?.Name;
        }

        private void TrySetInvalidValue()
        {
            var traversal = CreateCompiledTraversal<Source>
            (
                ClrMemberKind.Property,
                nameof(Source.Related),
                isToMany: false
            );
            var source = new Source { Related = new Target { Name = "original" } };

            this.ActualSuccess = traversal.TrySetValue(source, "invalid");
            this.ActualValue = source.Related.Name;
        }
    }

    private sealed class CompilationTest : XUnitTest
    {
        public required CompilationScenario Scenario { get; init; }

        public required ApiSchemaCompilationCode ExpectedCode { get; init; }

        private ApiSchemaCompilationResult? Result { get; set; }

        private bool ActualCanGet { get; set; }

        private bool ActualCanSet { get; set; }

        protected override void Arrange()
        { }

        protected override void Act()
        {
            var (clrNavigationMemberKind, clrNavigationMemberName) = this.Scenario switch
            {
                CompilationScenario.InvalidName => (ClrMemberKind.Property, " "),
                CompilationScenario.Missing => (ClrMemberKind.Property, "Missing"),
                CompilationScenario.ReadOnly => (ClrMemberKind.Property, nameof(InvalidSource.ReadOnly)),
                CompilationScenario.WriteOnly => (ClrMemberKind.Property, nameof(InvalidSource.WriteOnly)),
                CompilationScenario.Incompatible =>
                    (ClrMemberKind.Property, nameof(InvalidSource.Incompatible)),
                CompilationScenario.Indexer => (ClrMemberKind.Property, "Item"),
                _ => throw new InvalidOperationException()
            };
            var schema = CreateTraversalSchema<InvalidSource>
            (
                clrNavigationMemberKind,
                clrNavigationMemberName,
                isToMany: false,
                out var traversal
            );

            this.Result = ApiSchemaCompiler.Compile(schema);
            var source = new InvalidSource();
            this.ActualCanGet = traversal.TryGetValue(source, out _);
            this.ActualCanSet = traversal.TrySetValue(source, clrValue: null);
        }

        protected override void Assert()
        {
            var actualIssue = this.Result!.Issues
                .Where(issue => issue.Severity == ApiSchemaCompilationSeverity.Error)
                .Should().ContainSingle().Which;
            actualIssue.Code.Should().Be(this.ExpectedCode);
            actualIssue.ApiPath.Should().EndWith
            (
                $".{nameof(ApiRelationshipTraversal)}[\"related\"]"
            );
            this.ActualCanGet.Should().BeFalse();
            this.ActualCanSet.Should().BeFalse();
        }
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] AccessTheoryData =>
    [
        new AccessTest { Name = "Gets and sets a to-one CLR property", Scenario = AccessScenario.ObjectProperty },
        new AccessTest { Name = "Gets and sets a to-one CLR field", Scenario = AccessScenario.ObjectField },
        new AccessTest
        {
            Name = "Gets and sets a to-one member generically",
            Scenario = AccessScenario.GenericProperty
        },
        new AccessTest
        {
            Name = "Gets and sets a to-one member with schema coercion",
            Scenario = AccessScenario.Coercion
        },
        new AccessTest { Name = "Tries to get and set a to-one member", Scenario = AccessScenario.TryProperty },
        new AccessTest
        {
            Name = "Translates a throwing set failure without mutating the member",
            Scenario = AccessScenario.InvalidValue
        },
        new AccessTest
        {
            Name = "Try set does not mutate for an invalid value",
            Scenario = AccessScenario.TryInvalidValue
        },
        new AccessTest { Name = "Gets and sets a to-many CLR property", Scenario = AccessScenario.ToManyProperty },
        new AccessTest { Name = "Gets and sets a to-many CLR field", Scenario = AccessScenario.ToManyField },
        new AccessTest { Name = "Sets a struct CLR navigation member by reference", Scenario = AccessScenario.StructByReference },
        new AccessTest
        {
            Name = "Tries to set a struct CLR navigation member by reference",
            Scenario = AccessScenario.TryStructByReference
        },
        new AccessTest
        {
            Name = "Rejects access without a CLR navigation member reference",
            Scenario = AccessScenario.MissingBinding
        },
        new AccessTest
        {
            Name = "Rejects access before schema compilation",
            Scenario = AccessScenario.BeforeCompilation
        },
        new AccessTest
        {
            Name = "Translates access failure for an invalid source",
            Scenario = AccessScenario.InvalidSource
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] CompilationTheoryData =>
    [
        new CompilationTest
        {
            Name = "Reports an invalid traversal CLR navigation member reference name at the traversal path",
            Scenario = CompilationScenario.InvalidName,
            ExpectedCode = ApiSchemaCompilationCode.ClrMemberReferenceInvalidClrName
        },
        new CompilationTest
        {
            Name = "Reports an unresolved traversal CLR navigation member reference",
            Scenario = CompilationScenario.Missing,
            ExpectedCode = ApiSchemaCompilationCode.ClrMemberReferenceUnresolved
        },
        new CompilationTest
        {
            Name = "Reports a read-only traversal CLR navigation member",
            Scenario = CompilationScenario.ReadOnly,
            ExpectedCode = ApiSchemaCompilationCode.ClrMemberIncompatible
        },
        new CompilationTest
        {
            Name = "Reports a write-only traversal CLR navigation member",
            Scenario = CompilationScenario.WriteOnly,
            ExpectedCode = ApiSchemaCompilationCode.ClrMemberIncompatible
        },
        new CompilationTest
        {
            Name = "Reports a traversal CLR navigation member with an incompatible type",
            Scenario = CompilationScenario.Incompatible,
            ExpectedCode = ApiSchemaCompilationCode.ClrMemberIncompatible
        },
        new CompilationTest
        {
            Name = "Reports a traversal CLR indexer",
            Scenario = CompilationScenario.Indexer,
            ExpectedCode = ApiSchemaCompilationCode.ClrMemberIncompatible
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(AccessTheoryData))]
    public void Access(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(CompilationTheoryData))]
    public void Compilation(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Test Helpers
    private static ApiRelationshipTraversal CreateCompiledTraversal<TSource>
    (
        ClrMemberKind? clrNavigationMemberKind,
        string? clrNavigationMemberName,
        bool isToMany
    )
    {
        var schema = CreateTraversalSchema<TSource>
        (
            clrNavigationMemberKind,
            clrNavigationMemberName,
            isToMany,
            out var traversal
        );

        ApiSchemaCompiler.Compile(schema).ThrowIfInvalid();
        return traversal;
    }

    private static ApiRelationshipTraversal CreateCompiledNumericTraversal()
    {
        var traversal = new ApiRelationshipTraversal
        (
            "related",
            new ClrMemberReference(ClrMemberKind.Property, nameof(NumericSource.Related))
        );
        var sourceType = new ApiObjectType
        (
            nameof(NumericSource),
            null,
            null,
            null,
            null,
            typeof(NumericSource)
        );
        var targetType = new ApiObjectType("Int64Target", null, null, null, null, typeof(long));
        var sourceEnd = new ApiRelationshipPrincipalEnd(new ApiTypeReference(typeof(NumericSource)), traversal);
        var targetEnd = new ApiRelationshipDependentEnd(new ApiTypeReference(typeof(long)));
        var relationship = new ApiRelationshipOneToOne("relationship", sourceEnd, targetEnd);
        var schema = new ApiSchema
        (
            "TraversalCoercion",
            null,
            null,
            null,
            null,
            [sourceType, targetType],
            [relationship]
        );

        ApiSchemaCompiler.Compile(schema).ThrowIfInvalid();
        return traversal;
    }

    private static ApiSchema CreateTraversalSchema<TSource>
    (
        ClrMemberKind? clrNavigationMemberKind,
        string? clrNavigationMemberName,
        bool isToMany,
        out ApiRelationshipTraversal traversal
    )
    {
        var clrNavigationMemberReference = clrNavigationMemberKind is not null && clrNavigationMemberName is not null
            ? new ClrMemberReference(clrNavigationMemberKind.Value, clrNavigationMemberName)
            : null;
        traversal = new ApiRelationshipTraversal("related", clrNavigationMemberReference);
        var sourceType = new ApiObjectType(nameof(TSource), null, null, null, null, typeof(TSource));
        var targetType = new ApiObjectType(nameof(Target), null, null, null, null, typeof(Target));
        var sourceEnd = new ApiRelationshipPrincipalEnd(new ApiTypeReference(typeof(TSource)), traversal);
        var targetEnd = new ApiRelationshipDependentEnd(new ApiTypeReference(typeof(Target)));
        ApiRelationship relationship = isToMany
            ? new ApiRelationshipOneToMany("relationship", sourceEnd, targetEnd)
            : new ApiRelationshipOneToOne("relationship", sourceEnd, targetEnd);
        return new ApiSchema
        (
            "TraversalAccess",
            null,
            null,
            null,
            null,
            [sourceType, targetType],
            [relationship]
        );
    }
    #endregion
}
