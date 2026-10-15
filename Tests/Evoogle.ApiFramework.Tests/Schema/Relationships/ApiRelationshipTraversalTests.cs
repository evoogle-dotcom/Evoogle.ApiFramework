// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Relationships;

public class ApiRelationshipTraversalTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class Source
    {
        #region Fields and Properties
        public long Id { get; set; }
        public Target? Single { get; set; }
        public Target? SingleField = null;
        public Target? TargetMember { get; set; }
        public Target? Targetmember { get; set; }
        public List<Target> Many { get; set; } = [];
        public IEnumerable<Target> Enumerable { get; set; } = [];
        public Target[] Array { get; set; } = [];
        public List<Source> WrongItems { get; set; } = [];
        #endregion
    }

    private sealed class Target
    {
        #region Properties
        public long Id { get; set; }
        public Source? Single { get; set; }
        public List<Source> Many { get; set; } = [];
        #endregion
    }

    private sealed class Association
    {
        #region Properties
        public long Id { get; set; }
        #endregion
    }

    private enum ConflictKind
    {
        DuplicateTraversalName,
        PropertyApiName,
        PropertyClrMember,
        BothPropertyConflicts,
        DifferentApiNameCase,
        DifferentClrNameCase
    }

    private sealed record ExpectedDiagnostic
    (
        ApiSchemaCompilationCode Code,
        ApiSchemaCompilationSeverity Severity,
        string ApiPath
    );

    private sealed class TopologyTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiRelationshipKind RelationshipKind { get; init; }
        public required bool UsesJson { get; init; }
        public required bool IsFromSource { get; init; }
        public required bool ExpectedIsToMany { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? Schema { get; set; }
        private ApiRelationshipTraversal? ActualTraversal { get; set; }
        private ApiRelationshipEnd? ExpectedSourceEnd { get; set; }
        private ApiRelationshipEnd? ExpectedTargetEnd { get; set; }
        private ApiObjectType? ExpectedSourceType { get; set; }
        private ApiObjectType? ExpectedTargetType { get; set; }
        private bool ActualFound { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var sourceMember = this.RelationshipKind == ApiRelationshipKind.OneToOne ? "Single" : "Many";
            var targetMember = this.RelationshipKind == ApiRelationshipKind.ManyToMany ? "Many" : "Single";
            this.Schema = CreateTraversalSchema(this.RelationshipKind, sourceMember, ClrMemberKind.Property, targetMember);

            this.WriteLine($"Relationship: {this.RelationshipKind}; JSON: {this.UsesJson}; From Source: {this.IsFromSource}");
            this.WriteLine($"Source Navigation: {sourceMember}; Target Navigation: {targetMember}");
            this.WriteLine($"Expected Cardinality Is To Many: {this.ExpectedIsToMany}; Expected Boundary: none");
        }

        protected override void Act()
        {
            ApiSchemaCompiler.Compile(this.Schema!).ThrowIfInvalid();
            if (this.UsesJson)
            {
                var json = JsonSerializer.Serialize(this.Schema);
                this.WriteLine($"Serialized Schema JSON:\n{json}");
                this.WriteLine();

                this.Schema = JsonSerializer.Deserialize<ApiSchema>(json)!;
            }

            var relationship = this.Schema!.ApiRelationships.Single();
            var (sourceEnd, targetEnd) = relationship switch
            {
                ApiRelationshipOneTo oneTo => ((ApiRelationshipEnd)oneTo.ApiPrincipalEnd, (ApiRelationshipEnd)oneTo.ApiDependentEnd),
                ApiRelationshipManyToMany manyToMany => ((ApiRelationshipEnd)manyToMany.ApiPrincipalEndA, (ApiRelationshipEnd)manyToMany.ApiPrincipalEndB),
                _ => throw new InvalidOperationException("Unexpected relationship kind.")
            };
            this.ExpectedSourceEnd = this.IsFromSource ? sourceEnd : targetEnd;
            this.ExpectedTargetEnd = this.IsFromSource ? targetEnd : sourceEnd;
            this.ExpectedSourceType = this.Schema.GetObjectTypeByClrType(this.IsFromSource ? typeof(Source) : typeof(Target));
            this.ExpectedTargetType = this.Schema.GetObjectTypeByClrType(this.IsFromSource ? typeof(Target) : typeof(Source));

            this.ActualFound = this.ExpectedSourceType.TryGetTraversalByApiName(this.IsFromSource ? "ToTarget" : "ToSource", out var traversal);
            this.ActualTraversal = traversal;
            this.WriteLine($"Lookup Found: {this.ActualFound}; Traversal: {traversal.SafeToString()}");
            this.WriteLine($"Actual Source: {traversal?.ApiSourceObjectType.ApiName}");
            this.WriteLine($"Actual Target: {traversal?.ApiTargetObjectType.ApiName}; Is To Many: {traversal?.IsToMany}");
        }

        protected override void Assert()
        {
            this.ActualFound.Should().BeTrue();
            this.ActualTraversal.Should().NotBeNull();

            var traversal = this.ActualTraversal!;
            traversal.Should().BeSameAs(this.ExpectedSourceEnd!.ApiTraversal);
            traversal.ApiSourceEnd.Should().BeSameAs(this.ExpectedSourceEnd);
            traversal.ApiTargetEnd.Should().BeSameAs(this.ExpectedTargetEnd);
            traversal.ApiSourceObjectType.Should().BeSameAs(this.ExpectedSourceType);
            traversal.ApiTargetObjectType.Should().BeSameAs(this.ExpectedTargetType);
            traversal.IsToMany.Should().Be(this.ExpectedIsToMany);
            traversal.HasClrNavigationMember.Should().BeTrue();

            this.ExpectedSourceType!.ApiRelationshipTraversals.Should().ContainSingle().Which.Should().BeSameAs(traversal);
            this.ExpectedSourceType.TryGetTraversalByApiName(traversal.ApiName.ToLowerInvariant(), out var missing).Should().BeFalse();
            missing.Should().BeNull();
        }
        #endregion
    }

    private sealed class CompilationTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiRelationshipKind RelationshipKind { get; init; }
        public required string ClrName { get; init; }
        public required ClrMemberKind ClrKind { get; init; }
        public ConflictKind? Conflict { get; init; }
        public required bool UsesJson { get; init; }
        public required ExpectedDiagnostic[] ExpectedDiagnostics { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? Schema { get; set; }
        private ApiSchemaCompilationResult? ActualResult { get; set; }
        private Exception? ActualException { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Schema = CreateTraversalSchema(this.RelationshipKind, this.ClrName, this.ClrKind);
            if (this.Conflict is { } conflict)
            {
                this.Schema = CreateConflictSchema(conflict);
            }

            this.WriteLine($"Relationship: {this.RelationshipKind}; Navigation: {this.ClrKind}.{this.ClrName}");
            this.WriteLine($"Conflict: {this.Conflict}; JSON: {this.UsesJson}");
            var expectedBoundary = this.ExpectedDiagnostics.Length == 0 ? "none" : "compilation error";
            this.WriteLine($"Expected Boundary: {expectedBoundary}");
            foreach (var diagnostic in this.ExpectedDiagnostics)
            {
                this.WriteLine($"Expected: {diagnostic.Severity}; {diagnostic.Code}; {diagnostic.ApiPath}");
            }
        }

        protected override void Act()
        {
            if (this.UsesJson)
            {
                var json = JsonSerializer.Serialize(this.Schema);
                this.WriteLine($"Serialized Schema JSON:\n{json}");
                this.WriteLine();

                try
                {
                    this.Schema = JsonSerializer.Deserialize<ApiSchema>(json);
                }
                catch (Exception exception)
                {
                    this.ActualException = exception;
                }

                if (this.ActualException is ApiSchemaCompilationException compilationException)
                {
                    this.ActualResult = compilationException.Result;
                }
            }
            else
            {
                this.ActualResult = ApiSchemaCompiler.Compile(this.Schema!);
            }

            this.WriteLine($"Actual Exception: {this.ActualException.SafeToString()}");
            if (this.ActualResult is { } result)
            {
                this.WriteLine($"Actual Result: {result.SafeToString()}");
                foreach (var issue in result.Issues)
                {
                    this.WriteLine($"Actual: {issue.Severity}; {issue.Code}; {issue.ApiPath}");
                }
            }
            else
            {
                this.WriteLine($"Published Schema: {this.Schema.SafeToString()}");
            }
        }

        protected override void Assert()
        {
            if (this.UsesJson && this.ExpectedDiagnostics.Length == 0)
            {
                this.ActualException.Should().BeNull();
                this.Schema.Should().NotBeNull();
                this.Schema.GetObjectTypeByClrType<Source>().ApiRelationshipTraversals.Should().NotBeEmpty();
                return;
            }

            if (this.UsesJson)
            {
                this.ActualException.Should().BeOfType<ApiSchemaCompilationException>();
            }
            else
            {
                this.ActualException.Should().BeNull();
            }

            this.ActualResult.Should().NotBeNull();
            var result = this.ActualResult;

            result.Issues.Select(issue => new ExpectedDiagnostic(issue.Code, issue.Severity, issue.ApiPath)).Should().BeEquivalentTo(this.ExpectedDiagnostics);
            result.Errors.Should().HaveCount(this.ExpectedDiagnostics.Length);
            result.Warnings.Should().BeEmpty();
            result.IsValid.Should().Be(this.ExpectedDiagnostics.Length == 0);
            if (this.ExpectedDiagnostics.Length == 0)
            {
                result.Schema.Should().NotBeNull();
            }
            else
            {
                result.Schema.Should().BeNull();
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] TopologyTheoryData =>
    [
        .. from topology in new[]
           {
               (ApiRelationshipKind.OneToOne, false, false),
               (ApiRelationshipKind.OneToMany, true, false),
               (ApiRelationshipKind.ManyToMany, true, true)
           }
           from isFromSource in new[] { true, false }
           from usesJson in new[] { false, true }
           select (TheoryDataRow<IXUnitTest>)new TopologyTest
           {
               Name = $"{topology.Item1}; From Source={isFromSource}; JSON={usesJson}",
               RelationshipKind = topology.Item1, IsFromSource = isFromSource, UsesJson = usesJson,
               ExpectedIsToMany = isFromSource ? topology.Item2 : topology.Item3
           }
    ];

    public static TheoryDataRow<IXUnitTest>[] NavigationCompatibilityTheoryData =>
    [
        .. from navigation in new[]
           {
               (ApiRelationshipKind.OneToOne, "Single", ClrMemberKind.Property, true),
               (ApiRelationshipKind.OneToOne, "SingleField", ClrMemberKind.Field, true),
               (ApiRelationshipKind.OneToMany, "Many", ClrMemberKind.Property, true),
               (ApiRelationshipKind.OneToMany, "Enumerable", ClrMemberKind.Property, true),
               (ApiRelationshipKind.OneToMany, "Array", ClrMemberKind.Property, true),
               (ApiRelationshipKind.OneToOne, "Many", ClrMemberKind.Property, false),
               (ApiRelationshipKind.OneToMany, "Single", ClrMemberKind.Property, false),
               (ApiRelationshipKind.OneToMany, "WrongItems", ClrMemberKind.Property, false)
           }
           from usesJson in new[] { false, true }
           select (TheoryDataRow<IXUnitTest>)new CompilationTest
           {
               Name = $"{navigation.Item1}; {navigation.Item2}; JSON={usesJson}",
               RelationshipKind = navigation.Item1, ClrName = navigation.Item2, ClrKind = navigation.Item3,
               UsesJson = usesJson,
               ExpectedDiagnostics = navigation.Item4 ? [] :
               [
                   new ExpectedDiagnostic(ApiSchemaCompilationCode.ClrMemberIncompatible,
                       ApiSchemaCompilationSeverity.Error,
                       $"ApiSchema[\"Traversal\"].ApiRelationship{navigation.Item1}[\"R1\"]." +
                       "ApiPrincipalEnd.ApiRelationshipTraversal[\"ToTarget\"]")
               ]
           }
    ];

    public static TheoryDataRow<IXUnitTest>[] ConflictTheoryData =>
    [
        .. from conflict in new[]
           {
               (ConflictKind.DuplicateTraversalName, new[]
               {
                   new ExpectedDiagnostic(ApiSchemaCompilationCode.ApiRelationshipTraversalDuplicateApiName,
                       ApiSchemaCompilationSeverity.Error, "ApiSchema[\"Traversal\"].ApiObjectType[\"Source\"]")
               }),
               (ConflictKind.PropertyApiName, new[] { ApiNameConflict }),
               (ConflictKind.PropertyClrMember, new[] { ClrMemberConflict }),
               (ConflictKind.BothPropertyConflicts, new[] { ApiNameConflict, ClrMemberConflict }),
               (ConflictKind.DifferentApiNameCase, System.Array.Empty<ExpectedDiagnostic>()),
               (ConflictKind.DifferentClrNameCase, System.Array.Empty<ExpectedDiagnostic>())
           }
           from usesJson in new[] { false, true }
           select (TheoryDataRow<IXUnitTest>)new CompilationTest
           {
               Name = $"{conflict.Item1}; JSON={usesJson}",
               RelationshipKind = ApiRelationshipKind.OneToOne,
               ClrName = "Single", ClrKind = ClrMemberKind.Property,
               Conflict = conflict.Item1, UsesJson = usesJson, ExpectedDiagnostics = conflict.Item2
           }
    ];
    #endregion

    #region Test Properties
    private static ExpectedDiagnostic ApiNameConflict => new
    (
        ApiSchemaCompilationCode.ApiRelationshipTraversalDuplicateApiName,
        ApiSchemaCompilationSeverity.Error,
        "ApiSchema[\"Traversal\"].ApiRelationshipOneToOne[\"R1\"].ApiPrincipalEnd.ApiRelationshipTraversal[\"ToTarget\"]"
    );

    private static ExpectedDiagnostic ClrMemberConflict => new
    (
        ApiSchemaCompilationCode.ApiRelationshipTraversalClrNavigationMemberConflict,
        ApiSchemaCompilationSeverity.Error,
        "ApiSchema[\"Traversal\"].ApiRelationshipOneToOne[\"R1\"].ApiPrincipalEnd.ApiRelationshipTraversal[\"ToTarget\"]"
    );
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(TopologyTheoryData))]
    public void CompileBindsTraversalSourceAndTargetEnds(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(NavigationCompatibilityTheoryData))]
    public void CompileValidatesNavigationTargetAndCardinality(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ConflictTheoryData))]
    public void CompileReportsTraversalNameAndNavigationMemberConflictsIndependently(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Factory Methods
    private static ApiSchema CreateTraversalSchema(ApiRelationshipKind kind, string clrName, ClrMemberKind clrKind, string? targetClrName = null)
    {
        var source = CreateObjectType(apiName: "Source", clrType: typeof(Source));
        var target = CreateObjectType(apiName: "Target", clrType: typeof(Target));
        var principal = new ApiRelationshipPrincipalEnd
        (
            apiObjectTypeReference: new ApiTypeReference(clrType: typeof(Source)),
            apiTraversal: new ApiRelationshipTraversal(apiName: "ToTarget", clrNavigationMember: new ClrMemberReference(clrKind: clrKind, clrName: clrName))
        );
        var reverse = targetClrName is null ? null : new ApiRelationshipTraversal
        (
            apiName: "ToSource",
            clrNavigationMember: new ClrMemberReference(clrKind: ClrMemberKind.Property, clrName: targetClrName)
        );
        ApiRelationship relationship = kind switch
        {
            ApiRelationshipKind.OneToOne => new ApiRelationshipOneToOne
            (
                apiName: "R1",
                apiPrincipalEnd: principal,
                apiDependentEnd: new ApiRelationshipDependentEnd(apiObjectTypeReference: new ApiTypeReference(clrType: typeof(Target)), apiTraversal: reverse)
            ),
            ApiRelationshipKind.OneToMany => new ApiRelationshipOneToMany
            (
                apiName: "R1",
                apiPrincipalEnd: principal,
                apiDependentEnd: new ApiRelationshipDependentEnd(apiObjectTypeReference: new ApiTypeReference(clrType: typeof(Target)), apiTraversal: reverse)
            ),
            ApiRelationshipKind.ManyToMany => new ApiRelationshipManyToMany
            (
                apiName: "R1",
                apiPrincipalEndA: principal,
                apiPrincipalEndB: new ApiRelationshipPrincipalEnd(apiObjectTypeReference: new ApiTypeReference(clrType: typeof(Target)), apiTraversal: reverse),
                apiAssociation: new ApiRelationshipAssociation(apiObjectTypeReference: new ApiTypeReference(clrType: typeof(Association)))
            ),
            _ => throw new InvalidOperationException("Unexpected relationship kind.")
        };
        return new ApiSchema
        (
            apiName: "Traversal",
            apiVersion: null,
            apiOptions: null,
            apiScalarTypes: [new ApiScalarType(apiName: "Int64", clrScalarType: typeof(long))],
            apiEnumTypes: null,
            apiObjectTypes: kind == ApiRelationshipKind.ManyToMany ? [source, target, CreateObjectType(apiName: "Association", clrType: typeof(Association))] : [source, target],
            apiRelationships: [relationship]
        );
    }

    private static ApiSchema CreateConflictSchema(ConflictKind conflict)
    {
        var containedName = conflict switch
        {
            ConflictKind.PropertyApiName or ConflictKind.BothPropertyConflicts => "ToTarget",
            ConflictKind.DifferentApiNameCase => "totarget",
            _ => "Contained"
        };
        var containedClrName = conflict switch
        {
            ConflictKind.PropertyClrMember or ConflictKind.BothPropertyConflicts => "Single",
            ConflictKind.DifferentClrNameCase => "Targetmember",
            _ => "SingleField"
        };
        var containedClrKind = containedClrName == "SingleField" ? ClrMemberKind.Field : ClrMemberKind.Property;
        var navigationClrName = conflict == ConflictKind.DifferentClrNameCase ? "TargetMember" : "Single";
        var contained = new ApiProperty
        (
            apiName: containedName,
            apiTypeExpression: new ApiTypeExpression(apiTypeReference: new ApiTypeReference(clrType: typeof(Target))),
            apiTypeModifiers: ApiTypeModifiers.None,
            clrValueMember: new ClrMemberReference(clrKind: containedClrKind, clrName: containedClrName)
        );
        var source = new ApiObjectType
        (
            apiName: "Source",
            apiOptions: null,
            apiProperties: conflict == ConflictKind.DuplicateTraversalName ? [CreateIdProperty()] : [CreateIdProperty(), contained],
            apiKeys: null,
            apiVersion: null,
            clrObjectType: typeof(Source)
        );

        var relationships = new List<ApiRelationship>
        {
            new ApiRelationshipOneToOne
            (
                apiName: "R1",
                apiPrincipalEnd: new ApiRelationshipPrincipalEnd
                (
                    apiObjectTypeReference: new ApiTypeReference(clrType: typeof(Source)),
                    apiTraversal: new ApiRelationshipTraversal(apiName: "ToTarget", clrNavigationMember: new ClrMemberReference(clrKind: ClrMemberKind.Property, clrName: navigationClrName))
                ),
                apiDependentEnd: new ApiRelationshipDependentEnd
                (
                    apiObjectTypeReference: new ApiTypeReference(clrType: typeof(Target))
                )
            )
        };

        if (conflict == ConflictKind.DuplicateTraversalName)
        {
            relationships.Add
            (
                new ApiRelationshipOneToOne
                (
                    apiName: "R2",
                    apiPrincipalEnd: new ApiRelationshipPrincipalEnd
                    (
                        apiObjectTypeReference: new ApiTypeReference(clrType: typeof(Source)),
                        apiTraversal: new ApiRelationshipTraversal(apiName: "ToTarget", clrNavigationMember: new ClrMemberReference(clrKind: ClrMemberKind.Field, clrName: "SingleField"))
                    ),
                    apiDependentEnd: new ApiRelationshipDependentEnd
                    (
                        apiObjectTypeReference: new ApiTypeReference(clrType: typeof(Target))
                    )
                )
            );
        }

        return new ApiSchema
        (
            apiName: "Traversal",
            apiVersion: null,
            apiOptions: null,
            apiScalarTypes: [new ApiScalarType(apiName: "Int64", clrScalarType: typeof(long))],
            apiEnumTypes: null,
            apiObjectTypes: [source, CreateObjectType(apiName: "Target", clrType: typeof(Target))],
            apiRelationships: relationships
        );
    }

    private static ApiObjectType CreateObjectType(string apiName, Type clrType) => new
    (
        apiName: apiName,
        apiOptions: null,
        apiProperties: [CreateIdProperty()],
        apiKeys: null,
        apiVersion: null,
        clrObjectType: clrType
    );

    private static ApiProperty CreateIdProperty() => new
    (
        apiName: "Id",
        apiTypeExpression: new ApiTypeExpression(apiTypeReference: new ApiTypeReference(typeof(long))),
        apiTypeModifiers: ApiTypeModifiers.Required,
        clrValueMember: new ClrMemberReference(clrKind: ClrMemberKind.Property, clrName: "Id")
    );
    #endregion
}
