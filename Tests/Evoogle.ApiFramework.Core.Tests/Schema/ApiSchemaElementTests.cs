// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.NTree;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public class ApiSchemaElementTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private enum InvalidTopologyTestCase
    {
        DuplicateOwnership,
        OwnershipCycle
    }

    private enum PreInitializationLink
    {
        Root,
        Parent,
        FirstChild,
        LastChild,
        NextSibling,
        PreviousSibling
    }

    private enum TreeEnum
    {
        One
    }

    private sealed class TreeObject
    {
        public int Id { get; set; }
    }

    private sealed class KindMappingTest : XUnitTest
    {
        #region Calculated Properties
        private ApiSchemaElement[]? Elements { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.Elements =
            [
                .. GetSchemas().SelectMany
                (
                    schema => schema.Root.SelfAndDescendants()
                )
            ];
        }

        protected override void Assert()
        {
            this.Elements.Should().NotBeNull();

            foreach (var element in this.Elements!)
            {
                element.Kind.Should().Be(GetExpectedKind(element));
            }

            this.Elements.Select(element => element.Kind).Distinct().Should().BeEquivalentTo
            (
                Enum.GetValues<ApiSchemaElementKind>()
            );
        }
        #endregion
    }

    private sealed class TopologyTest : XUnitTest
    {
        #region Calculated Properties
        private ApiSchema[]? Schemas { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Schemas = GetSchemas();
        }

        protected override void Act()
        { }

        protected override void Assert()
        {
            foreach (var schema in this.Schemas!)
            {
                AssertTopology(schema);
            }
        }
        #endregion
    }

    private sealed class TraversalOrderTest : XUnitTest
    {
        #region Calculated Properties
        private ApiSchema? Schema { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Schema = CreateTraversalSchema();
        }

        protected override void Act()
        { }

        protected override void Assert()
        {
            var breadthFirstKinds = this.Schema!.Root
                .SelfAndDescendants(TraversalStrategy.BreadthFirst)
                .Select(element => element.Kind);
            breadthFirstKinds.Should().Equal
            (
                ApiSchemaElementKind.Schema,
                ApiSchemaElementKind.ScalarType,
                ApiSchemaElementKind.EnumType,
                ApiSchemaElementKind.ObjectType,
                ApiSchemaElementKind.EnumValue,
                ApiSchemaElementKind.Property
            );

            var depthFirstKinds = this.Schema.Root
                .SelfAndDescendants(TraversalStrategy.DepthFirst)
                .Select(element => element.Kind);
            depthFirstKinds.Should().Equal
            (
                ApiSchemaElementKind.Schema,
                ApiSchemaElementKind.ScalarType,
                ApiSchemaElementKind.EnumType,
                ApiSchemaElementKind.EnumValue,
                ApiSchemaElementKind.ObjectType,
                ApiSchemaElementKind.Property
            );
        }
        #endregion
    }

    private sealed class KeyPathSiblingTest : XUnitTest
    {
        #region Calculated Properties
        private ApiKeyPath? KeyPath { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var apiObjectType = ApiSchemaFactory.KeyApiSchema.GetObjectTypeByApiName
            (
                "KeyNestedComposite"
            );
            var apiKeyType = apiObjectType.GetKeyTypeByApiName
            (
                "PK_KeyNestedComposite"
            );

            this.KeyPath = apiKeyType.ApiKeyPaths.Single(path => path.ApiSegments.Length > 1);
        }

        protected override void Act()
        { }

        protected override void Assert()
        {
            var segments = this.KeyPath!.ApiSegments;
            segments.Should().HaveCount(2);
            this.KeyPath.Children().Should().Equal(segments);

            segments[0].Parent.Should().BeSameAs(this.KeyPath);
            segments[1].Parent.Should().BeSameAs(this.KeyPath);
            segments[0].PreviousSibling.Should().BeNull();
            segments[0].NextSibling.Should().BeSameAs(segments[1]);
            segments[1].PreviousSibling.Should().BeSameAs(segments[0]);
            segments[1].NextSibling.Should().BeNull();

            segments[0].IsDescendantOf(segments[1]).Should().BeFalse();
            segments[1].IsDescendantOf(segments[0]).Should().BeFalse();
        }
        #endregion
    }

    private sealed class ReinitializeTest : XUnitTest
    {
        #region Calculated Properties
        private ApiSchemaElement[]? After { get; set; }

        private ApiSchemaElement[]? Before { get; set; }

        private ApiInitializationResult? Result { get; set; }

        private ApiSchema? Schema { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Schema = CreateTraversalSchema();
            this.Before = [.. this.Schema.Root.SelfAndDescendants()];
        }

        protected override void Act()
        {
            this.Result = this.Schema!.Initialize();
            this.After = [.. this.Schema.Root.SelfAndDescendants()];
        }

        protected override void Assert()
        {
            this.Result!.IsValid.Should().BeTrue();
            this.After.Should().Equal(this.Before!);
            AssertTopology(this.Schema!);
        }
        #endregion
    }

    private sealed class PreInitializationTest : XUnitTest
    {
        #region User Supplied Properties
        public required PreInitializationLink Link { get; init; }
        #endregion

        #region Calculated Properties
        private Exception? Exception { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            var element = new ApiSchema
            (
                "Uninitialized",
                apiVersion: null,
                apiOptions: null,
                apiNamedTypes: [],
                apiRelationships: []
            );

            try
            {
                _ = this.Link switch
                {
                    PreInitializationLink.Root => element.Root,
                    PreInitializationLink.Parent => element.Parent,
                    PreInitializationLink.FirstChild => element.FirstChild,
                    PreInitializationLink.LastChild => element.LastChild,
                    PreInitializationLink.NextSibling => element.NextSibling,
                    PreInitializationLink.PreviousSibling => element.PreviousSibling,
                    _ => throw new InvalidOperationException
                    (
                        $"Unsupported {nameof(PreInitializationLink)} value '{this.Link}'."
                    )
                };
            }
            catch (Exception exception)
            {
                this.Exception = exception;
            }
        }

        protected override void Assert()
        {
            this.Exception.Should().BeOfType<ApiSchemaException>();
        }
        #endregion
    }

    private sealed class InvalidTopologyTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiInitializationCode ExpectedCode { get; init; }

        public required InvalidTopologyTestCase TestCase { get; init; }
        #endregion

        #region Calculated Properties
        private string? ExpectedApiPath { get; set; }

        private ApiSchemaElement? RootElement { get; set; }

        private ApiInitializationResult? Result { get; set; }

        private bool? WasTopologyBuilt { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            if (this.TestCase == InvalidTopologyTestCase.DuplicateOwnership)
            {
                var sharedValue = new ApiEnumValue("One", "One", 1);
                var schema = new ApiSchema
                (
                    "DuplicateOwnership",
                    apiVersion: null,
                    apiOptions: null,
                    apiNamedTypes:
                    [
                        new ApiEnumType("First", [sharedValue], typeof(TreeEnum)),
                        new ApiEnumType("Second", [sharedValue], typeof(DayOfWeek))
                    ],
                    apiRelationships: []
                );

                this.ExpectedApiPath = schema.BuildDefaultPath(apiPreviousPath: null);
                this.RootElement = schema;
                this.Result = schema.Initialize();
                return;
            }

            var sessionSchema = CreateTraversalSchema();
            var session = new ApiInitializationSession
            (
                sessionSchema,
                sessionSchema.ApiSchemaContext
            );
            var cyclicElement = new TopologyElement("Cycle");
            cyclicElement.SetChildren(cyclicElement);

            this.ExpectedApiPath = sessionSchema.ApiPath;
            this.RootElement = cyclicElement;
            this.WasTopologyBuilt = ApiSchemaTreeBuilder.TryBuild(cyclicElement, session);
            this.Result = new ApiInitializationResult(session.Issues);
        }

        protected override void Assert()
        {
            var error = this.Result!.Errors.Should().ContainSingle
            (
                issue => issue.Code == this.ExpectedCode
            ).Subject;
            error.ApiPath.Should().Be(this.ExpectedApiPath);
            if (this.TestCase == InvalidTopologyTestCase.OwnershipCycle)
            {
                this.WasTopologyBuilt.Should().BeFalse();
            }

            var action = () => this.RootElement!.Root;
            action.Should().Throw<ApiSchemaException>();
        }
        #endregion
    }

    private sealed class JsonConverterOutputTest : XUnitTest
    {
        #region Calculated Properties
        private string[]? PropertyNames { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            var schema = CreateTraversalSchema();
            var jsonValues = new[]
            {
                JsonSerializer.Serialize(schema),
                JsonSerializer.Serialize(schema.ApiEnumTypes.Single().ApiEnumValues.Single())
            };
            var propertyNames = new List<string>();

            foreach (var json in jsonValues)
            {
                using var document = JsonDocument.Parse(json);
                propertyNames.AddRange(EnumeratePropertyNames(document.RootElement));
            }

            this.PropertyNames = [.. propertyNames];
        }

        protected override void Assert()
        {
            this.PropertyNames.Should().NotContain
            (
                nameof(ApiSchemaElement.Kind),
                nameof(ApiSchemaElement.ApiSchemaContext),
                nameof(ApiSchemaElement.Root),
                nameof(ApiSchemaElement.Parent),
                nameof(ApiSchemaElement.FirstChild),
                nameof(ApiSchemaElement.LastChild),
                nameof(ApiSchemaElement.NextSibling),
                nameof(ApiSchemaElement.PreviousSibling)
            );
        }
        #endregion
    }

    private sealed class TopologyElement(string label) : ApiSchemaElement
    {
        #region Fields
        private ApiSchemaElement[] _children = [];
        #endregion

        #region ApiSchemaElement Properties
        public override ApiSchemaElementKind Kind => ApiSchemaElementKind.Property;

        protected override string ApiElementName => nameof(TopologyElement);
        #endregion

        #region Methods
        public void SetChildren(params ApiSchemaElement[] children)
        {
            _children = children;
        }
        #endregion

        #region ApiSchemaElement Methods
        protected override string BuildPath(string? apiPreviousPath)
        {
            return ApiSchemaPathFormatting.BuildPath
            (
                apiPreviousPath,
                this.ApiElementName,
                label
            );
        }

        internal override IEnumerable<ApiSchemaElement> GetOwnedElements() => _children;
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] KindMappingTheoryData =>
    [
        new KindMappingTest
        {
            Name = "Built-In Elements Expose Their Concrete Kinds"
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] TopologyTheoryData =>
    [
        new TopologyTest
        {
            Name = "Initialized Schemas Expose Canonical Ownership Trees"
        },
        new TraversalOrderTest
        {
            Name = "Traversal Uses Canonical Breadth-First And Depth-First Preorder"
        },
        new KeyPathSiblingTest
        {
            Name = "Key Path Segments Are Ordered Siblings"
        },
        new ReinitializeTest
        {
            Name = "Repeated Initialization Rebuilds The Same Topology"
        },
        new JsonConverterOutputTest
        {
            Name = "Schema Element Converters Exclude Runtime Members"
        }
    ];

    public static TheoryDataRow<IXUnitTest>[] PreInitializationTheoryData =>
    [
        .. Enum.GetValues<PreInitializationLink>().Select
        (
            link => new TheoryDataRow<IXUnitTest>
            (
                new PreInitializationTest
                {
                    Name = $"{link} Requires Initialized Topology",
                    Link = link
                }
            )
        )
    ];

    public static TheoryDataRow<IXUnitTest>[] InvalidTopologyTheoryData =>
    [
        new InvalidTopologyTest
        {
            Name = "Duplicate Ownership Prevents Topology Publication",
            TestCase = InvalidTopologyTestCase.DuplicateOwnership,
            ExpectedCode = ApiInitializationCode.ApiSchemaElementDuplicateOwnership
        },
        new InvalidTopologyTest
        {
            Name = "Ownership Cycles Prevent Topology Publication",
            TestCase = InvalidTopologyTestCase.OwnershipCycle,
            ExpectedCode = ApiInitializationCode.ApiSchemaElementOwnershipCycle
        }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(KindMappingTheoryData))]
    public void KindMapping(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TopologyTheoryData))]
    public void Topology(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(PreInitializationTheoryData))]
    public void PreInitialization(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(InvalidTopologyTheoryData))]
    public void InvalidTopology(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Factory Methods
    private static ApiSchema CreateTraversalSchema()
    {
        var apiScalarType = new ApiScalarType("Int32", typeof(int));
        var apiEnumType = new ApiEnumType
        (
            "TreeEnum",
            [new ApiEnumValue("One", nameof(TreeEnum.One), (int)TreeEnum.One)],
            typeof(TreeEnum)
        );
        var apiProperty = new ApiProperty
        (
            nameof(TreeObject.Id),
            new ApiTypeExpression(typeof(int)),
            ApiTypeModifiers.Required,
            nameof(TreeObject.Id),
            ClrMemberKind.Property
        );
        var apiObjectType = new ApiObjectType
        (
            "TreeObject",
            apiOptions: null,
            apiProperties: [apiProperty],
            apiKeyTypes: [],
            typeof(TreeObject)
        );
        var schema = new ApiSchema
        (
            "Tree",
            apiVersion: null,
            apiOptions: null,
            apiNamedTypes: [apiScalarType, apiEnumType, apiObjectType],
            apiRelationships: []
        );

        schema.Initialize().ThrowIfInvalid();
        return schema;
    }

    private static ApiSchema[] GetSchemas()
    {
        return
        [
            CreateTraversalSchema(),
            ApiSchemaFactory.SimpleApiSchema,
            ApiSchemaFactory.CommerceApiSchema,
            ApiSchemaFactory.KeyApiSchema,
            ApiSchemaFactory.RelationshipApiSchema
        ];
    }
    #endregion

    #region Assertion Methods
    private static void AssertTopology(ApiSchema schema)
    {
        schema.Root.Should().BeSameAs(schema);
        schema.Parent.Should().BeNull();

        var elements = schema.Root.SelfAndDescendants(TraversalStrategy.DepthFirst).ToArray();
        elements.Distinct(ReferenceEqualityComparer.Instance).Should().HaveCount(elements.Length);

        foreach (var element in elements)
        {
            element.Root.Should().BeSameAs(schema);
            element.ApiSchemaContext.Should().BeSameAs(schema.ApiSchemaContext);

            var expectedChildren = GetExpectedChildren(element);
            var actualChildren = element.Children().ToArray();
            actualChildren.Should().Equal(expectedChildren);
            element.FirstChild.Should().BeSameAs(expectedChildren.FirstOrDefault());
            element.LastChild.Should().BeSameAs(expectedChildren.LastOrDefault());

            for (var index = 0; index < actualChildren.Length; ++index)
            {
                var child = actualChildren[index];
                child.Parent.Should().BeSameAs(element);
                child.PreviousSibling.Should().BeSameAs
                (
                    index > 0 ? actualChildren[index - 1] : null
                );
                child.NextSibling.Should().BeSameAs
                (
                    index < actualChildren.Length - 1 ? actualChildren[index + 1] : null
                );
            }

            if (element is ApiProperty apiProperty &&
                apiProperty.ApiTypeExpression is { IsInline: false, IsResolved: true })
            {
                actualChildren.Should().NotContain(apiProperty.ApiType);
            }

            if (element is ApiRelationshipElement apiRelationshipElement)
            {
                actualChildren.Should().NotContain(apiRelationshipElement.ApiObjectType);
            }
        }
    }

    private static ApiSchemaElementKind GetExpectedKind(ApiSchemaElement element)
    {
        return element switch
        {
            ApiSchema => ApiSchemaElementKind.Schema,
            ApiCollectionType => ApiSchemaElementKind.CollectionType,
            ApiEnumType => ApiSchemaElementKind.EnumType,
            ApiObjectType => ApiSchemaElementKind.ObjectType,
            ApiScalarType => ApiSchemaElementKind.ScalarType,
            ApiEnumValue => ApiSchemaElementKind.EnumValue,
            ApiProperty => ApiSchemaElementKind.Property,
            ApiNamedKeyType => ApiSchemaElementKind.NamedKeyType,
            ApiKeyType => ApiSchemaElementKind.KeyType,
            ApiKeyPath => ApiSchemaElementKind.KeyPath,
            ApiKeyPathSegment => ApiSchemaElementKind.KeyPathSegment,
            ApiRelationshipOneToOne => ApiSchemaElementKind.RelationshipOneToOne,
            ApiRelationshipOneToMany => ApiSchemaElementKind.RelationshipOneToMany,
            ApiRelationshipManyToMany => ApiSchemaElementKind.RelationshipManyToMany,
            ApiRelationshipPrincipalEnd => ApiSchemaElementKind.RelationshipPrincipalEnd,
            ApiRelationshipDependentEnd => ApiSchemaElementKind.RelationshipDependentEnd,
            ApiRelationshipAssociation => ApiSchemaElementKind.RelationshipAssociation,
            _ => throw new InvalidOperationException
            (
                $"Unsupported schema element type '{element.GetType().Name}'."
            )
        };
    }

    private static ApiSchemaElement[] GetExpectedChildren(ApiSchemaElement element)
    {
        return element switch
        {
            ApiSchema schema =>
            [
                .. schema.ApiScalarTypes,
                .. schema.ApiEnumTypes,
                .. schema.ApiObjectTypes,
                .. schema.ApiRelationships
            ],
            ApiEnumType apiEnumType => [.. apiEnumType.ApiEnumValues],
            ApiObjectType apiObjectType =>
            [
                .. apiObjectType.ApiProperties,
                .. apiObjectType.ApiKeyTypes
            ],
            ApiProperty apiProperty when apiProperty.ApiTypeExpression?.ApiInlineType is not null =>
                [apiProperty.ApiTypeExpression.ApiInlineType],
            ApiCollectionType apiCollectionType
                when apiCollectionType.ApiItemTypeExpression?.ApiInlineType is not null =>
                [apiCollectionType.ApiItemTypeExpression.ApiInlineType],
            ApiKeyType apiKeyType => [.. apiKeyType.ApiKeyPaths],
            ApiKeyPath apiKeyPath => [.. apiKeyPath.ApiSegments],
            ApiRelationshipOneTo apiRelationship =>
            [
                .. new ApiSchemaElement?[]
                {
                    apiRelationship.ApiPrincipalEnd,
                    apiRelationship.ApiDependentEnd
                }.OfType<ApiSchemaElement>()
            ],
            ApiRelationshipManyToMany apiRelationship =>
            [
                .. new ApiSchemaElement?[]
                {
                    apiRelationship.ApiPrincipalEndA,
                    apiRelationship.ApiPrincipalEndB,
                    apiRelationship.ApiAssociation
                }.OfType<ApiSchemaElement>()
            ],
            ApiRelationshipDependentEnd apiDependentEnd when apiDependentEnd.HasForeignKey =>
                [apiDependentEnd.ApiForeignKeyType],
            ApiRelationshipAssociation apiAssociation when apiAssociation.HasForeignKeys =>
                [apiAssociation.ApiForeignKeyTypeA, apiAssociation.ApiForeignKeyTypeB],
            _ => []
        };
    }
    #endregion

    #region Enumeration Methods
    private static IEnumerable<string> EnumeratePropertyNames(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                yield return property.Name;
                foreach (var nestedName in EnumeratePropertyNames(property.Value))
                {
                    yield return nestedName;
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                foreach (var nestedName in EnumeratePropertyNames(item))
                {
                    yield return nestedName;
                }
            }
        }
    }
    #endregion
}
