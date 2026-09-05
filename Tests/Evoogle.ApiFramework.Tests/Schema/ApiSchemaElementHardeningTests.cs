// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.NTree;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public class ApiSchemaElementHardeningTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private enum ForeignOwnershipCase
    {
        RootChild,
        Subtree
    }

    private enum InlineEnum
    {
        One = 1
    }

    private enum RelationshipOwnerCase
    {
        CompiledEnd,
        CompiledAssociation,
        UninitializedEnd,
        UninitializedAssociation,
        InvalidEndParent,
        InvalidAssociationParent
    }

    private enum TraversalExceptionCase
    {
        NullReceiver,
        NullEnumerator,
        NullVisitor,
        NullAncestor,
        InvalidStrategy
    }

    private sealed class InlineChild
    {
        public int Id { get; set; }
    }

    private sealed class InlineHost
    {
        public List<int> Collection { get; set; } = [];

        public InlineEnum Enum { get; set; }

        public InlineChild Object { get; set; } = new();

        public InlineKeyedChild ObjectWithKey { get; set; } = new();

        public int Scalar { get; set; }

        public List<List<int>> NestedCollection { get; set; } = [];
    }

    private sealed class InlineKeyedChild
    {
        public int Id { get; set; }
    }

    private sealed class SharedTreeObject
    {
        public int Id { get; set; }
    }

    private sealed class CustomKeyType(IEnumerable<ApiKeyPath> apiKeyPaths)
        : ApiKeyType(apiKeyPaths);

    private sealed class StoppingVisitor(int maximumCount) : INodeVisitor<ApiSchemaElement>
    {
        public List<ApiSchemaElement> VisitedElements { get; } = [];

        public VisitResult Visit(ApiSchemaElement node)
        {
            this.VisitedElements.Add(node);
            return this.VisitedElements.Count >= maximumCount
                ? VisitResult.Done
                : VisitResult.Continue;
        }
    }

    private sealed record InlineSchemaFixture
    (
        ApiSchema Schema,
        ApiScalarType ScalarType,
        ApiEnumType EnumType,
        ApiObjectType ObjectType,
        ApiCollectionType CollectionType,
        ApiCollectionType NestedCollectionType,
        ApiCollectionType NestedItemCollectionType,
        ApiObjectType ObjectTypeWithKey
    );

    private sealed class ForeignOwnershipTest : XUnitTest
    {
        #region User Supplied Properties
        public required ForeignOwnershipCase OwnershipCase { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaElement[]? FirstElementsAfter { get; set; }

        private ApiSchemaElement[]? FirstElementsBefore { get; set; }

        private ApiSchemaContext? FirstSchemaContext { get; set; }

        private ApiSchema? FirstSchema { get; set; }

        private string? FirstSharedPath { get; set; }

        private ApiSchemaElement? SharedDescendant { get; set; }

        private ApiSchemaElement? SharedElement { get; set; }

        private ApiSchemaElement? SharedParent { get; set; }

        private ApiSchemaCompilationResult? SecondResult { get; set; }

        private ApiSchema? SecondSchema { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            if (this.OwnershipCase == ForeignOwnershipCase.RootChild)
            {
                var sharedScalarType = new ApiScalarType("SharedInt32", typeof(int));
                this.SharedElement = sharedScalarType;
                this.FirstSchema = CreateSchema("First", [sharedScalarType]);
                this.SecondSchema = CreateSchema("Second", [sharedScalarType]);
            }
            else
            {
                var inlineScalarType = new ApiScalarType("InlineInt32", typeof(int));
                var sharedProperty = new ApiProperty
                (
                    nameof(SharedTreeObject.Id),
                    new ApiTypeExpression(inlineScalarType),
                    ApiTypeModifiers.Required,
                    nameof(SharedTreeObject.Id),
                    ClrMemberKind.Property
                );
                var sharedObjectType = new ApiObjectType
                (
                    "SharedTreeObject",
                    apiOptions: null,
                    apiProperties: [sharedProperty],
                    apiKeyTypes: [],
                    typeof(SharedTreeObject)
                );

                this.SharedElement = sharedObjectType;
                this.SharedDescendant = inlineScalarType;
                this.FirstSchema = CreateSchema("First", [sharedObjectType]);
                this.SecondSchema = CreateSchema("Second", [sharedObjectType]);
            }

            ApiSchemaCompiler.Compile(this.FirstSchema).ThrowIfInvalid();
            this.FirstElementsBefore =
                [.. this.FirstSchema.SelfAndDescendants(TraversalStrategy.DepthFirst)];
            this.SharedParent = this.SharedElement!.Parent;
            this.FirstSharedPath = this.SharedElement.ApiPath;
            this.FirstSchemaContext = this.SharedElement.ApiSchemaContext;
        }

        protected override void Act()
        {
            this.SecondResult = ApiSchemaCompiler.Compile(this.SecondSchema!);
            this.FirstElementsAfter =
                [.. this.FirstSchema!.SelfAndDescendants(TraversalStrategy.DepthFirst)];
        }

        protected override void Assert()
        {
            this.SecondResult!.Errors.Should().ContainSingle
            (
                issue => issue.Code == ApiSchemaCompilationCode.ApiSchemaElementDuplicateOwnership
            );

            Action getSecondRoot = () => _ = this.SecondSchema!.Root;
            getSecondRoot.Should().Throw<ApiSchemaException>();

            this.FirstElementsAfter.Should().Equal(this.FirstElementsBefore!);
            this.SharedElement!.Root.Should().BeSameAs(this.FirstSchema);
            this.SharedElement.Parent.Should().BeSameAs(this.SharedParent);
            this.SharedElement.ApiPath.Should().Be(this.FirstSharedPath);
            this.SharedElement.ApiSchemaContext.Should().BeSameAs(this.FirstSchemaContext);

            if (this.SharedDescendant is not null)
            {
                this.SharedDescendant.Root.Should().BeSameAs(this.FirstSchema);
                this.SharedDescendant.IsDescendantOf(this.SharedElement).Should().BeTrue();
                this.FirstElementsAfter.Should().Contain(this.SharedDescendant);
            }
        }
        #endregion
    }

    private sealed class InlineTypesInitializeTest : XUnitTest
    {
        #region Calculated Properties
        private InlineSchemaFixture? Fixture { get; set; }

        private ApiSchemaCompilationResult? Result { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Fixture = CreateInlineSchemaFixture();
        }

        protected override void Act()
        {
            this.Result = ApiSchemaCompiler.Compile(this.Fixture!.Schema);
        }

        protected override void Assert()
        {
            this.Result!.IsValid.Should().BeTrue();

            var fixture = this.Fixture!;
            var expectedInlineTypes = new ApiType[]
            {
                fixture.ScalarType,
                fixture.EnumType,
                fixture.ObjectType,
                fixture.CollectionType,
                fixture.NestedCollectionType,
                fixture.NestedItemCollectionType,
                fixture.ObjectTypeWithKey
            };
            var traversedElements = fixture.Schema
                .SelfAndDescendants(TraversalStrategy.DepthFirst)
                .ToArray();

            foreach (var inlineType in expectedInlineTypes)
            {
                traversedElements.Should().Contain(inlineType);
                inlineType.Root.Should().BeSameAs(fixture.Schema);
                inlineType.ApiSchemaContext.Should().BeSameAs(fixture.Schema.ApiSchemaContext);
                inlineType.ApiPath.Should().NotBeNullOrWhiteSpace();
            }

            fixture.EnumType.ApiEnumValues.Single().Parent.Should().BeSameAs(fixture.EnumType);
            fixture.CollectionType.ApiItemType.Root.Should().BeSameAs(fixture.Schema);
            fixture.NestedCollectionType.ApiItemType.Should()
                .BeSameAs(fixture.NestedItemCollectionType);
            fixture.NestedItemCollectionType.ApiItemType.Root.Should().BeSameAs(fixture.Schema);

            var keyType = fixture.ObjectTypeWithKey.ApiKeyTypes.Single();
            keyType.Root.Should().BeSameAs(fixture.Schema);
            keyType.ApiKeyPaths.Single().ApiRootObjectType.Should()
                .BeSameAs(fixture.ObjectTypeWithKey);
            keyType.ApiKeyPaths.Single().ApiScalarSegment.ApiProperty.Parent.Should()
                .BeSameAs(fixture.ObjectTypeWithKey);

            fixture.Schema.TryGetScalarTypeByApiName(fixture.ScalarType.ApiName, out _)
                .Should().BeFalse();
            fixture.Schema.TryGetEnumTypeByApiName(fixture.EnumType.ApiName, out _)
                .Should().BeFalse();
            fixture.Schema.TryGetObjectTypeByApiName(fixture.ObjectType.ApiName, out _)
                .Should().BeFalse();
            fixture.Schema.TryGetObjectTypeByApiName
            (
                fixture.ObjectTypeWithKey.ApiName,
                out _
            ).Should().BeFalse();
        }
        #endregion
    }

    private sealed class RelationshipOwnerTest : XUnitTest
    {
        #region User Supplied Properties
        public required RelationshipOwnerCase OwnerCase { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaElement? Element { get; set; }

        private Exception? Exception { get; set; }

        private ApiRelationship? ExpectedRelationship { get; set; }

        private string? Json { get; set; }

        private ApiRelationship? ResolvedRelationship { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            switch (this.OwnerCase)
            {
                case RelationshipOwnerCase.CompiledEnd:
                    this.ExpectedRelationship = ApiSchemaFactory.RelationshipApiSchema
                        .ApiRelationships.OfType<ApiRelationshipOneTo>().First();
                    this.Element =
                        ((ApiRelationshipOneTo)this.ExpectedRelationship).ApiPrincipalEnd;
                    break;

                case RelationshipOwnerCase.CompiledAssociation:
                    this.ExpectedRelationship = ApiSchemaFactory.RelationshipApiSchema
                        .ApiRelationships.OfType<ApiRelationshipManyToMany>().First();
                    this.Element =
                        ((ApiRelationshipManyToMany)this.ExpectedRelationship).ApiAssociation;
                    break;

                case RelationshipOwnerCase.UninitializedEnd:
                    this.Element = new ApiRelationshipPrincipalEnd(typeof(InlineHost));
                    break;

                case RelationshipOwnerCase.UninitializedAssociation:
                    this.Element = new ApiRelationshipAssociation(typeof(InlineHost));
                    break;

                case RelationshipOwnerCase.InvalidEndParent:
                    this.Element = new ApiRelationshipPrincipalEnd(typeof(InlineHost));
                    SetInvalidRelationshipParent(this.Element);
                    break;

                case RelationshipOwnerCase.InvalidAssociationParent:
                    this.Element = new ApiRelationshipAssociation(typeof(InlineHost));
                    SetInvalidRelationshipParent(this.Element);
                    break;

                default:
                    throw new InvalidOperationException
                    (
                        $"Unsupported {nameof(RelationshipOwnerCase)} value '{this.OwnerCase}'."
                    );
            }
        }

        protected override void Act()
        {
            try
            {
                this.ResolvedRelationship = this.Element switch
                {
                    ApiRelationshipEnd apiRelationshipEnd =>
                        apiRelationshipEnd.ApiRelationship,
                    ApiRelationshipAssociation apiAssociation =>
                        apiAssociation.ApiRelationshipManyToMany,
                    _ => throw new InvalidOperationException()
                };
                this.Json = JsonSerializer.Serialize
                (
                    this.Element,
                    this.Element.GetType()
                );
            }
            catch (Exception exception)
            {
                this.Exception = exception;
            }
        }

        protected override void Assert()
        {
            if (this.ExpectedRelationship is not null)
            {
                this.Exception.Should().BeNull();
                this.ResolvedRelationship.Should().BeSameAs(this.ExpectedRelationship);
                this.ResolvedRelationship.Should().BeSameAs(this.Element!.Parent);
                this.Json.Should().NotContain
                (
                    nameof(ApiRelationshipEnd.ApiRelationship)
                );
                this.Json.Should().NotContain
                (
                    nameof(ApiRelationshipAssociation.ApiRelationshipManyToMany)
                );
                return;
            }

            this.Exception.Should().BeOfType<ApiSchemaException>();
        }
        #endregion
    }

    private sealed class ImmutableCollectionsTest : XUnitTest
    {
        #region Calculated Properties
        private ApiSchema? Schema { get; set; }

        private ApiObjectType? ObjectType { get; set; }

        private ApiEnumType? EnumType { get; set; }

        private ApiKeyPath? KeyPath { get; set; }

        private ApiNamedKeyType? KeyType { get; set; }

        private ImmutableArray<ApiRelationshipAssociation> RelationshipAssociationsBefore
        {
            get;
            set;
        }

        private ImmutableArray<ApiRelationshipEnd> RelationshipEndsBefore { get; set; }

        private ApiObjectType? RelationshipObjectType { get; set; }

        private ApiSchema? RelationshipSchema { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var segmentSource = new List<ApiKeyPathSegment>
            {
                new(nameof(InlineKeyedChild.Id))
            };
            this.KeyPath = new ApiKeyPath(typeof(InlineKeyedChild), segmentSource);
            segmentSource.Clear();

            var keyPathSource = new List<ApiKeyPath> { this.KeyPath };
            this.KeyType = new ApiNamedKeyType("PK_InlineKeyedChild", keyPathSource);
            keyPathSource.Clear();

            var propertySource = new List<ApiProperty>
            {
                CreateInlineProperty
                (
                    nameof(InlineKeyedChild.Id),
                    new ApiScalarType("InlineInt32", typeof(int))
                )
            };
            var keyTypeSource = new List<ApiNamedKeyType> { this.KeyType };
            this.ObjectType = new ApiObjectType
            (
                "InlineKeyedChild",
                apiOptions: null,
                propertySource,
                keyTypeSource,
                typeof(InlineKeyedChild)
            );
            propertySource.Clear();
            keyTypeSource.Clear();

            var enumValueSource = new List<ApiEnumValue>
            {
                new("One", nameof(InlineEnum.One), (int)InlineEnum.One)
            };
            this.EnumType = new ApiEnumType("InlineEnum", enumValueSource, typeof(InlineEnum));
            enumValueSource.Clear();

            var namedTypeSource = new List<ApiNamedType> { this.EnumType, this.ObjectType };
            this.Schema = CreateSchema("Immutable", namedTypeSource);
            namedTypeSource.Clear();
            ApiSchemaCompiler.Compile(this.Schema).ThrowIfInvalid();

            this.RelationshipSchema = CreateRelationshipSchema();
            ApiSchemaCompiler.Compile(this.RelationshipSchema).ThrowIfInvalid();
            this.RelationshipObjectType = this.RelationshipSchema.ApiObjectTypes
                .First(apiObjectType => apiObjectType.ApiRelationshipAssociations.Length > 0);
            this.RelationshipEndsBefore = this.RelationshipSchema.ApiObjectTypes
                .First(apiObjectType => apiObjectType.ApiRelationshipEnds.Length > 0)
                .ApiRelationshipEnds;
            this.RelationshipAssociationsBefore =
                this.RelationshipObjectType.ApiRelationshipAssociations;
        }

        protected override void Act()
        { }

        protected override void Assert()
        {
            this.Schema!.ApiNamedTypes.IsDefault.Should().BeFalse();
            this.Schema.ApiScalarTypes.IsDefault.Should().BeFalse();
            this.Schema.ApiEnumTypes.IsDefault.Should().BeFalse();
            this.Schema.ApiObjectTypes.IsDefault.Should().BeFalse();
            this.Schema.ApiRelationships.IsDefault.Should().BeFalse();

            this.EnumType!.ApiEnumValues.IsDefault.Should().BeFalse();
            this.EnumType.ApiEnumValues.Should().ContainSingle();
            this.ObjectType!.ApiProperties.IsDefault.Should().BeFalse();
            this.ObjectType.ApiProperties.Should().ContainSingle();
            this.ObjectType.ApiKeyTypes.IsDefault.Should().BeFalse();
            this.ObjectType.ApiKeyTypes.Should().ContainSingle();
            this.KeyType!.ApiKeyPaths.IsDefault.Should().BeFalse();
            this.KeyType.ApiKeyPaths.Should().ContainSingle();
            this.KeyPath!.ApiSegments.IsDefault.Should().BeFalse();
            this.KeyPath.ApiSegments.Should().ContainSingle();

            foreach (var apiObjectType in this.RelationshipSchema!.ApiObjectTypes)
            {
                apiObjectType.ApiRelationshipEnds.IsDefault.Should().BeFalse();
                apiObjectType.ApiRelationshipPrincipalEnds.IsDefault.Should().BeFalse();
                apiObjectType.ApiRelationshipDependentEnds.IsDefault.Should().BeFalse();
                apiObjectType.ApiRelationshipAssociations.IsDefault.Should().BeFalse();
            }

            var relationshipObjectType = this.RelationshipSchema.ApiObjectTypes
                .First(apiObjectType => apiObjectType.ApiRelationshipEnds.Length > 0);
            relationshipObjectType.ApiRelationshipEnds.Should()
                .Equal(this.RelationshipEndsBefore);
            relationshipObjectType.ApiRelationshipEnds.Equals(this.RelationshipEndsBefore)
                .Should().BeTrue();
            this.RelationshipObjectType!.ApiRelationshipAssociations.Should()
                .Equal(this.RelationshipAssociationsBefore);
            this.RelationshipObjectType.ApiRelationshipAssociations
                .Equals(this.RelationshipAssociationsBefore).Should().BeTrue();

            var changedSegments = this.KeyPath.ApiSegments.SetItem
            (
                0,
                new ApiKeyPathSegment("Other")
            );
            changedSegments.Should().NotEqual(this.KeyPath.ApiSegments);
            this.KeyPath.ApiSegments.Single().ClrPropertyName.Should()
                .Be(nameof(InlineKeyedChild.Id));
        }
        #endregion
    }

    private sealed class HierarchyAccessibilityTest : XUnitTest
    {
        #region Calculated Properties
        private Type[]? AbstractBranchTypes { get; set; }

        private CustomKeyType? CustomKeyType { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.AbstractBranchTypes =
            [
                typeof(ApiSchemaElement),
                typeof(ApiType),
                typeof(ApiNamedType),
                typeof(ApiRelationship),
                typeof(ApiRelationshipOneTo),
                typeof(ApiRelationshipElement),
                typeof(ApiRelationshipEnd)
            ];
            this.CustomKeyType = new CustomKeyType([]);
        }

        protected override void Act()
        { }

        protected override void Assert()
        {
            foreach (var branchType in this.AbstractBranchTypes!)
            {
                branchType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                    .Should().BeEmpty();
                branchType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Should().Contain(constructor => constructor.IsAssembly);
            }

            this.CustomKeyType!.Kind.Should().Be(ApiSchemaElementKind.KeyType);
            this.CustomKeyType.Should().BeAssignableTo<ApiKeyType>();
        }
        #endregion
    }

    private sealed class TraversalExtensionsTest : XUnitTest
    {
        #region Calculated Properties
        private ApiSchemaElement[]? BreadthFirst { get; set; }

        private ApiSchemaElement[]? Children { get; set; }

        private ApiSchemaElement[]? DelegateEnumeratorVisited { get; set; }

        private ApiSchemaElement[]? DelegateStrategyVisited { get; set; }

        private ApiSchemaElement[]? DepthFirst { get; set; }

        private ApiSchemaElement[]? Descendants { get; set; }

        private ApiSchemaElement[]? StrategyDescendants { get; set; }

        private ApiSchemaElement[]? StrategySelfAndDescendants { get; set; }

        private bool? IsDescendant { get; set; }

        private ApiSchemaElement[]? PathFromRoot { get; set; }

        private ApiSchemaElement[]? PathToRoot { get; set; }

        private ApiSchemaElement[]? VisitorEnumeratorVisited { get; set; }

        private ApiSchemaElement[]? VisitorStrategyVisited { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            var schema = ApiSchemaFactory.KeyApiSchema;
            var objectType = schema.GetObjectTypeByApiName("KeyNestedComposite");
            var keyPath = objectType.GetKeyTypeByApiName("PK_KeyNestedComposite")
                .ApiKeyPaths.Single(path => path.ApiSegments.Length > 1);
            var terminalSegment = keyPath.ApiSegments[^1];

            this.Children = [.. schema.Children()];
            this.BreadthFirst =
                [.. schema.SelfAndDescendants(schema.CreateBreadthFirstEnumerator())];
            this.DepthFirst =
                [.. objectType.SelfAndDescendants(objectType.CreateDepthFirstEnumerator())];
            this.Descendants =
                [.. keyPath.Descendants(keyPath.CreateDepthFirstEnumerator())];
            this.StrategyDescendants =
                [.. keyPath.Descendants(TraversalStrategy.DepthFirst)];
            this.StrategySelfAndDescendants =
                [.. keyPath.SelfAndDescendants(TraversalStrategy.BreadthFirst)];
            this.PathFromRoot = [.. terminalSegment.GetPathFromRoot()];
            this.PathToRoot = [.. terminalSegment.GetPathToRoot()];
            this.IsDescendant = terminalSegment.IsDescendantOf(keyPath);

            var delegateEnumeratorVisited = new List<ApiSchemaElement>();
            keyPath.Traverse
            (
                keyPath.CreateBreadthFirstEnumerator(),
                element =>
                {
                    delegateEnumeratorVisited.Add(element);
                    return delegateEnumeratorVisited.Count < 2;
                }
            );
            this.DelegateEnumeratorVisited = [.. delegateEnumeratorVisited];

            var delegateStrategyVisited = new List<ApiSchemaElement>();
            keyPath.Traverse
            (
                TraversalStrategy.DepthFirst,
                element =>
                {
                    delegateStrategyVisited.Add(element);
                    return delegateStrategyVisited.Count < 2;
                }
            );
            this.DelegateStrategyVisited = [.. delegateStrategyVisited];

            var visitorEnumerator = new StoppingVisitor(maximumCount: 2);
            keyPath.Traverse(keyPath.CreateDepthFirstEnumerator(), visitorEnumerator);
            this.VisitorEnumeratorVisited = [.. visitorEnumerator.VisitedElements];

            var visitorStrategy = new StoppingVisitor(maximumCount: 2);
            keyPath.Traverse(TraversalStrategy.BreadthFirst, visitorStrategy);
            this.VisitorStrategyVisited = [.. visitorStrategy.VisitedElements];
        }

        protected override void Assert()
        {
            var schema = ApiSchemaFactory.KeyApiSchema;
            var objectType = schema.GetObjectTypeByApiName("KeyNestedComposite");
            var keyPath = objectType.GetKeyTypeByApiName("PK_KeyNestedComposite")
                .ApiKeyPaths.Single(path => path.ApiSegments.Length > 1);
            var terminalSegment = keyPath.ApiSegments[^1];

            this.Children.Should().Equal
            (
                schema.ApiScalarTypes.Cast<ApiSchemaElement>()
                    .Concat(schema.ApiEnumTypes)
                    .Concat(schema.ApiObjectTypes)
                    .Concat(schema.ApiRelationships)
            );
            this.BreadthFirst.Should().Equal
            (
                NodeExtensions.SelfAndDescendants<ApiSchemaElement>
                (
                    schema,
                    TraversalStrategy.BreadthFirst
                )
            );
            this.DepthFirst.Should().Equal
            (
                NodeExtensions.SelfAndDescendants<ApiSchemaElement>
                (
                    objectType,
                    TraversalStrategy.DepthFirst
                )
            );
            this.Descendants.Should().Equal(keyPath.ApiSegments);
            this.StrategyDescendants.Should().Equal(keyPath.ApiSegments);
            this.StrategySelfAndDescendants.Should().Equal
            (
                new ApiSchemaElement[] { keyPath }.Concat(keyPath.ApiSegments)
            );
            this.PathFromRoot.Should().StartWith(schema).And.EndWith(terminalSegment);
            this.PathToRoot.Should().StartWith(terminalSegment).And.EndWith(schema);
            this.IsDescendant.Should().BeTrue();
            this.DelegateEnumeratorVisited.Should().HaveCount(2);
            this.DelegateStrategyVisited.Should().HaveCount(2);
            this.VisitorEnumeratorVisited.Should().HaveCount(2);
            this.VisitorStrategyVisited.Should().HaveCount(2);
        }
        #endregion
    }

    private sealed class TraversalExceptionTest : XUnitTest
    {
        #region User Supplied Properties
        public required TraversalExceptionCase ExceptionCase { get; init; }
        #endregion

        #region Calculated Properties
        private Exception? Exception { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            var schema = ApiSchemaFactory.SimpleApiSchema;

            try
            {
                switch (this.ExceptionCase)
                {
                    case TraversalExceptionCase.NullReceiver:
                        _ = ApiSchemaElementExtensions.Children(null!).ToArray();
                        break;

                    case TraversalExceptionCase.NullEnumerator:
                        _ = schema.Descendants
                        (
                            null!
                        ).ToArray();
                        break;

                    case TraversalExceptionCase.NullVisitor:
                        schema.Traverse
                        (
                            TraversalStrategy.BreadthFirst,
                            (INodeVisitor<ApiSchemaElement>)null!
                        );
                        break;

                    case TraversalExceptionCase.NullAncestor:
                        _ = schema.IsDescendantOf(null!);
                        break;

                    case TraversalExceptionCase.InvalidStrategy:
                        _ = schema.Descendants((TraversalStrategy)int.MaxValue).ToArray();
                        break;

                    default:
                        throw new InvalidOperationException
                        (
                            $"Unsupported {nameof(TraversalExceptionCase)} value "
                                + $"'{this.ExceptionCase}'."
                        );
                }
            }
            catch (Exception exception)
            {
                this.Exception = exception;
            }
        }

        protected override void Assert()
        {
            if (this.ExceptionCase == TraversalExceptionCase.InvalidStrategy)
            {
                this.Exception.Should().BeOfType<ArgumentOutOfRangeException>();
                return;
            }

            this.Exception.Should().BeOfType<ArgumentNullException>();
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] ForeignOwnershipTheoryData =>
    [
        .. Enum.GetValues<ForeignOwnershipCase>().Select
        (
            ownershipCase => new TheoryDataRow<IXUnitTest>
            (
                new ForeignOwnershipTest
                {
                    Name = $"Foreign {ownershipCase} Ownership Is Rejected",
                    OwnershipCase = ownershipCase
                }
            )
        )
    ];

    public static TheoryDataRow<IXUnitTest>[] InlineTypesTheoryData =>
    [
        new InlineTypesInitializeTest
        {
            Name = "Every Inline Type Is Compiled As An Owned Element"
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] RelationshipOwnerTheoryData =>
    [
        .. Enum.GetValues<RelationshipOwnerCase>().Select
        (
            ownerCase => new TheoryDataRow<IXUnitTest>
            (
                new RelationshipOwnerTest
                {
                    Name = $"Relationship Owner {ownerCase}",
                    OwnerCase = ownerCase
                }
            )
        )
    ];

    public static TheoryDataRow<IXUnitTest>[] PublicApiTheoryData =>
    [
        new ImmutableCollectionsTest
        {
            Name = "Schema Collections Are Immutable Non-Default Snapshots"
        },
        new HierarchyAccessibilityTest
        {
            Name = "Unsafe Abstract Derivation Is Closed And Key Kind Is Cast Safe"
        },
        new TraversalExtensionsTest
        {
            Name = "Concrete Elements Expose Delegated NTree Traversal"
        },
        .. Enum.GetValues<TraversalExceptionCase>().Select
        (
            exceptionCase => new TheoryDataRow<IXUnitTest>
            (
                new TraversalExceptionTest
                {
                    Name = $"Traversal Delegates {exceptionCase} Validation",
                    ExceptionCase = exceptionCase
                }
            )
        )
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(ForeignOwnershipTheoryData))]
    public void ForeignOwnership(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(InlineTypesTheoryData))]
    public void InlineTypes(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(RelationshipOwnerTheoryData))]
    public void RelationshipOwner(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(PublicApiTheoryData))]
    public void PublicApi(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Factory Methods
    private static ApiSchema CreateSchema
    (
        string apiName,
        IEnumerable<ApiNamedType> apiNamedTypes
    )
    {
        return new ApiSchema
        (
            apiName,
            apiVersion: null,
            apiOptions: null,
            apiNamedTypes,
            apiRelationships: []
        );
    }

    private static ApiProperty CreateInlineProperty(string clrName, ApiType apiInlineType)
    {
        return new ApiProperty
        (
            clrName,
            new ApiTypeExpression(apiInlineType),
            ApiTypeModifiers.Required,
            clrName,
            ClrMemberKind.Property
        );
    }

    private static InlineSchemaFixture CreateInlineSchemaFixture()
    {
        var scalarType = new ApiScalarType("InlineScalar", typeof(int));
        var enumType = new ApiEnumType
        (
            "InlineEnum",
            [new ApiEnumValue("One", nameof(InlineEnum.One), (int)InlineEnum.One)],
            typeof(InlineEnum)
        );

        var objectType = new ApiObjectType
        (
            "InlineObject",
            apiOptions: null,
            apiProperties:
            [
                CreateInlineProperty
                (
                    nameof(InlineChild.Id),
                    new ApiScalarType("InlineObjectInt32", typeof(int))
                )
            ],
            apiKeyTypes: [],
            typeof(InlineChild)
        );

        var collectionType = new ApiCollectionType
        (
            new ApiTypeExpression(new ApiScalarType("InlineItemInt32", typeof(int))),
            ApiTypeModifiers.Required,
            typeof(List<int>)
        );
        var nestedItemCollectionType = new ApiCollectionType
        (
            new ApiTypeExpression(new ApiScalarType("NestedItemInt32", typeof(int))),
            ApiTypeModifiers.Required,
            typeof(List<int>)
        );
        var nestedCollectionType = new ApiCollectionType
        (
            new ApiTypeExpression(nestedItemCollectionType),
            ApiTypeModifiers.Required,
            typeof(List<List<int>>)
        );

        var keyedProperty = CreateInlineProperty
        (
            nameof(InlineKeyedChild.Id),
            new ApiScalarType("InlineKeyInt32", typeof(int))
        );
        var keyedPath = new ApiKeyPath
        (
            typeof(InlineKeyedChild),
            [new ApiKeyPathSegment(nameof(InlineKeyedChild.Id))]
        );
        var objectTypeWithKey = new ApiObjectType
        (
            "InlineObjectWithKey",
            apiOptions: null,
            apiProperties: [keyedProperty],
            apiKeyTypes: [new ApiNamedKeyType("PK_InlineObjectWithKey", [keyedPath])],
            typeof(InlineKeyedChild)
        );

        var hostType = new ApiObjectType
        (
            "InlineHost",
            apiOptions: null,
            apiProperties:
            [
                CreateInlineProperty(nameof(InlineHost.Scalar), scalarType),
                CreateInlineProperty(nameof(InlineHost.Enum), enumType),
                CreateInlineProperty(nameof(InlineHost.Object), objectType),
                CreateInlineProperty(nameof(InlineHost.Collection), collectionType),
                CreateInlineProperty
                (
                    nameof(InlineHost.NestedCollection),
                    nestedCollectionType
                ),
                CreateInlineProperty(nameof(InlineHost.ObjectWithKey), objectTypeWithKey)
            ],
            apiKeyTypes: [],
            typeof(InlineHost)
        );
        var schema = CreateSchema("InlineTypes", [hostType]);

        return new InlineSchemaFixture
        (
            schema,
            scalarType,
            enumType,
            objectType,
            collectionType,
            nestedCollectionType,
            nestedItemCollectionType,
            objectTypeWithKey
        );
    }

    private static ApiSchema CreateRelationshipSchema()
    {
        var principalObjectTypeA = new ApiObjectType
        (
            "PrincipalA",
            apiOptions: null,
            apiProperties: [],
            apiKeyTypes: [],
            typeof(SharedTreeObject)
        );
        var principalObjectTypeB = new ApiObjectType
        (
            "PrincipalB",
            apiOptions: null,
            apiProperties: [],
            apiKeyTypes: [],
            typeof(InlineHost)
        );
        var associationObjectType = new ApiObjectType
        (
            "Association",
            apiOptions: null,
            apiProperties: [],
            apiKeyTypes: [],
            typeof(InlineKeyedChild)
        );
        var oneToMany = new ApiRelationshipOneToMany
        (
            "OneToMany",
            new ApiRelationshipPrincipalEnd(typeof(SharedTreeObject)),
            new ApiRelationshipDependentEnd(typeof(InlineHost))
        );
        var manyToMany = new ApiRelationshipManyToMany
        (
            "ManyToMany",
            new ApiRelationshipPrincipalEnd(typeof(SharedTreeObject)),
            new ApiRelationshipPrincipalEnd(typeof(InlineHost)),
            new ApiRelationshipAssociation(typeof(InlineKeyedChild))
        );

        return new ApiSchema
        (
            "Relationships",
            apiVersion: null,
            apiOptions: null,
            apiNamedTypes:
            [
                principalObjectTypeA,
                principalObjectTypeB,
                associationObjectType
            ],
            apiRelationships: [oneToMany, manyToMany]
        );
    }

    private static void SetInvalidRelationshipParent(ApiSchemaElement element)
    {
        var schema = CreateSchema("InvalidRelationshipParent", []);
        element.SetTopology
        (
            schema,
            schema,
            firstChild: null,
            lastChild: null,
            previousSibling: null,
            nextSibling: null
        );
    }
    #endregion
}
