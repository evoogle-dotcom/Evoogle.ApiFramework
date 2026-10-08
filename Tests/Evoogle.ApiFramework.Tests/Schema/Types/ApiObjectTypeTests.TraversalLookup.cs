// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.Extensions;
using Evoogle.NTree;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema.Types;

public partial class ApiObjectTypeTests
{
    #region Traversal Lookup Test Types
    private enum TraversalLookupName
    {
        Matching,
        Missing,
        DifferentCase
    }

    private sealed class Customer
    {
        public int Id { get; set; }

        public List<Order> Orders { get; set; } = [];
    }

    private sealed class Order
    {
        public int Id { get; set; }

        public Customer? Customer { get; set; }
    }

    private sealed class EmptyTraversalLookupTest : XUnitTest
    {
        #region User Supplied Properties
        public required InlineObjectShape Shape { get; init; }
        public required bool UsesJsonDeserialization { get; init; }
        #endregion

        #region Calculated Properties
        private ApiObjectType? RootType { get; set; }
        private ApiObjectType[]? InlineTypes { get; set; }
        private (ApiObjectType ObjectType, bool HasTraversal, ApiRelationshipTraversal? Traversal)[]?
            ActualLookups
        { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var (schema, sourceJson) = CompileTraversalLookupSchema
            (
                CreateInlineObjectSchemaDefinition(this.Shape),
                this.UsesJsonDeserialization
            );
            this.RootType = schema.ApiObjectTypes.Single();
            this.InlineTypes = schema.SelfAndDescendants(TraversalStrategy.DepthFirst)
                .OfType<ApiObjectType>().Where(objectType => !ReferenceEquals(objectType, this.RootType)).ToArray();
            this.WriteLine($"Inline Object Shape:    {this.Shape}");
            this.WriteLine($"Compilation Path:       {(this.UsesJsonDeserialization ? "Root JSON" : "Descriptors")}");
            this.WriteLine("Queried Name:           MissingTraversal");
            this.WriteLine("Expected Lookup Result: false; Expected Returned Traversal: null");
            this.WriteLine();

            if (sourceJson is not null)
            {
                this.WriteLine($"Source JSON: {sourceJson}");
                this.WriteLine();
            }
        }

        protected override void Act()
        {
            this.ActualLookups = [.. new[] { this.RootType! }.Concat(this.InlineTypes!).Select(objectType =>
            {
                var hasTraversal = objectType.TryGetTraversalByApiName("MissingTraversal", out var traversal);
                this.WriteLine($"Lookup Object:        {objectType.ApiName}");
                this.WriteLine($"Actual Lookup Result: {hasTraversal}; Returned Traversal: {traversal.SafeToString()}");
                this.WriteLine();
                return (objectType, hasTraversal, traversal);
            })];
        }

        protected override void Assert()
        {
            var expectedInlineTypes = this.Shape == InlineObjectShape.NestedCollections
                ? new[] { (nameof(InlineContainer), typeof(InlineContainer)), (nameof(InlineLeaf), typeof(InlineLeaf)) }
                : new[] { (nameof(InlineLeaf), typeof(InlineLeaf)) };
            this.WriteLine($"Expected Inline Object Count: {expectedInlineTypes.Length}; Actual: {this.InlineTypes!.Length}");
            this.InlineTypes.Select(objectType => (objectType.ApiName, objectType.ClrType))
                .Should().Equal(expectedInlineTypes);
            this.ActualLookups.Should().HaveCount(expectedInlineTypes.Length + 1);
            foreach (var (objectType, hasTraversal, traversal) in this.ActualLookups!)
            {
                this.WriteLine($"Object: {objectType.ApiName}; Expected Default Traversal Collection: false; Actual: {objectType.ApiRelationshipTraversals.IsDefault}");
                objectType.ApiRelationshipTraversals.IsDefault.Should().BeFalse();
                this.WriteLine($"Expected Traversal Count: 0; Actual: {objectType.ApiRelationshipTraversals.Length}");
                objectType.ApiRelationshipTraversals.Should().BeEmpty();
                hasTraversal.Should().BeFalse();
                traversal.Should().BeNull();
                this.WriteLine();
            }
        }
        #endregion
    }

    private sealed class PopulatedTraversalLookupTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiRelationshipEndKind SourceEndKind { get; init; }
        public required bool UsesJsonDeserialization { get; init; }
        public required TraversalLookupName LookupName { get; init; }
        #endregion

        #region Calculated Properties
        private ApiObjectType? SourceType { get; set; }
        private ApiRelationshipTraversal? ExpectedTraversal { get; set; }
        private string? QueriedName { get; set; }
        private bool ExpectedResult => this.LookupName == TraversalLookupName.Matching;
        private bool? ActualResult { get; set; }
        private ApiRelationshipTraversal? ActualTraversal { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            var (schema, sourceJson) = CompileTraversalLookupSchema(CreatePopulatedTraversalSchemaDefinition(), this.UsesJsonDeserialization);
            var relationship = (ApiRelationshipOneToMany)schema.ApiRelationships.Single();
            ApiRelationshipEnd sourceEnd = this.SourceEndKind == ApiRelationshipEndKind.Principal
                ? relationship.ApiPrincipalEnd
                : relationship.ApiDependentEnd;
            this.SourceType = sourceEnd.ApiObjectType;
            this.ExpectedTraversal = sourceEnd.ApiTraversal
                ?? throw new InvalidOperationException("The relationship end must expose a traversal.");
            this.QueriedName = this.LookupName switch
            {
                TraversalLookupName.Matching => this.ExpectedTraversal.ApiName,
                TraversalLookupName.Missing => "MissingTraversal",
                TraversalLookupName.DifferentCase => this.ExpectedTraversal.ApiName.ToLowerInvariant(),
                _ => throw new InvalidOperationException("Unknown traversal lookup name.")
            };
            this.WriteLine($"Relationship Side:           {this.SourceEndKind}; Source Object: {this.SourceType.ApiName}");
            this.WriteLine($"Compilation Path:            {(this.UsesJsonDeserialization ? "Root JSON" : "Descriptors")}");
            this.WriteLine($"Configured Traversal:        {this.ExpectedTraversal.SafeToString()}");
            this.WriteLine($"Queried Name:                {this.QueriedName}");
            this.WriteLine($"Expected Lookup Result:      {this.ExpectedResult}");
            this.WriteLine($"Expected Returned Traversal: {(this.ExpectedResult ? this.ExpectedTraversal : null).SafeToString()}");
            this.WriteLine();

            if (sourceJson is not null)
            {
                this.WriteLine($"Source JSON: {sourceJson}");
                this.WriteLine();
            }
        }

        protected override void Act()
        {
            this.ActualResult = this.SourceType!.TryGetTraversalByApiName(this.QueriedName!, out var traversal);
            this.ActualTraversal = traversal;
            this.WriteLine($"Actual Lookup Result: {this.ActualResult}");
            this.WriteLine($"Returned Traversal:   {this.ActualTraversal.SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            var expectedApiName = this.SourceEndKind == ApiRelationshipEndKind.Principal ? "Orders" : "Customer";
            this.ExpectedTraversal!.ApiName.Should().Be(expectedApiName);
            this.ExpectedTraversal.HasClrNavigationMember.Should().BeTrue();
            this.ExpectedTraversal.ClrKind.Should().Be(ClrMemberKind.Property);
            this.ExpectedTraversal.ClrName.Should().Be(expectedApiName);
            this.SourceType!.ClrType.Should().Be(this.SourceEndKind == ApiRelationshipEndKind.Principal
                ? typeof(Customer)
                : typeof(Order));
            this.SourceType.ApiRelationshipTraversals.IsDefault.Should().BeFalse();
            this.SourceType.ApiRelationshipTraversals.Should().ContainSingle()
                .Which.Should().BeSameAs(this.ExpectedTraversal);
            this.ActualResult.Should().Be(this.ExpectedResult);
            if (this.ExpectedResult)
            {
                this.ActualTraversal.Should().BeSameAs(this.ExpectedTraversal);
            }
            else
            {
                this.ActualTraversal.Should().BeNull();
            }

            this.AssertClrNavigationAccessor();
        }

        #endregion

        #region Assertion Methods
        private void AssertClrNavigationAccessor()
        {
            if (this.SourceEndKind == ApiRelationshipEndKind.Principal)
            {
                var customer = new Customer();
                var ordersNavigation = new List<Order> { new() };

                this.ExpectedTraversal!.SetValue(customer, ordersNavigation);

                this.ExpectedTraversal.GetValue<Customer, List<Order>>(customer).Should().BeSameAs(ordersNavigation);
                customer.Orders.Should().BeSameAs(ordersNavigation);
                return;
            }

            var order = new Order();
            var customerNavigation = new Customer();

            this.ExpectedTraversal!.SetValue(order, customerNavigation);

            this.ExpectedTraversal.GetValue<Order, Customer>(order).Should().BeSameAs(customerNavigation);
            order.Customer.Should().BeSameAs(customerNavigation);
        }
        #endregion
    }
    #endregion

    #region Traversal Lookup Theory Data
    public static TheoryDataRow<IXUnitTest>[] EmptyTraversalLookupTheoryData =>
    [
        .. from shape in Enum.GetValues<InlineObjectShape>()
           from usesJsonDeserialization in new[] { false, true }
           select (TheoryDataRow<IXUnitTest>)new EmptyTraversalLookupTest
           {
               Name = $"{shape}; JSON deserialization={usesJsonDeserialization}",
               Shape = shape,
               UsesJsonDeserialization = usesJsonDeserialization
           }
    ];

    public static TheoryDataRow<IXUnitTest>[] MatchingTraversalLookupTheoryData =>
        CreatePopulatedTraversalLookupTheoryData(TraversalLookupName.Matching);

    public static TheoryDataRow<IXUnitTest>[] MissingTraversalLookupTheoryData =>
        CreatePopulatedTraversalLookupTheoryData(TraversalLookupName.Missing);

    public static TheoryDataRow<IXUnitTest>[] CaseSensitiveTraversalLookupTheoryData =>
        CreatePopulatedTraversalLookupTheoryData(TraversalLookupName.DifferentCase);
    #endregion

    #region Traversal Lookup Test Methods
    [Theory]
    [MemberData(nameof(EmptyTraversalLookupTheoryData))]
    public void TryGetTraversalByApiNameReturnsFalseForObjectsWithoutTraversals(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(MatchingTraversalLookupTheoryData))]
    public void TryGetTraversalByApiNameReturnsMatchingTraversal(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(MissingTraversalLookupTheoryData))]
    public void TryGetTraversalByApiNameReturnsFalseWhenNameIsMissing(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(CaseSensitiveTraversalLookupTheoryData))]
    public void TryGetTraversalByApiNameReturnsFalseWhenNameCaseDiffers(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Traversal Lookup Implementation Methods
    private static TheoryDataRow<IXUnitTest>[] CreatePopulatedTraversalLookupTheoryData
    (
        TraversalLookupName lookupName
    ) =>
    [
        .. from sourceEndKind in new[] { ApiRelationshipEndKind.Principal, ApiRelationshipEndKind.Dependent }
           from usesJsonDeserialization in new[] { false, true }
           select (TheoryDataRow<IXUnitTest>)new PopulatedTraversalLookupTest
           {
               Name = $"{sourceEndKind}; {lookupName}; JSON deserialization={usesJsonDeserialization}",
               SourceEndKind = sourceEndKind,
               UsesJsonDeserialization = usesJsonDeserialization,
               LookupName = lookupName
           }
    ];

    private static (ApiSchema apiSchema, string? sourceJson) CompileTraversalLookupSchema(ApiSchemaDef schemaDefinition, bool usesJsonDeserialization)
    {
        var schema = BuildApiSchema(schemaDefinition)
            ?? throw new InvalidOperationException("Schema descriptor compilation failed.");
        if (!usesJsonDeserialization)
        {
            return (schema, null);
        }

        var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.Never };
        var sourceJson = JsonSerializer.Serialize(schema, options);
        return (JsonSerializer.Deserialize<ApiSchema>(sourceJson, options)
            ?? throw new InvalidOperationException("Root schema JSON deserialization failed."), sourceJson);
    }

    private static ApiSchemaDef CreatePopulatedTraversalSchemaDefinition() => new
    (
        ApiName: "TraversalLookup",
        ApiNamedTypes:
        [
            new ApiScalarTypeDef("Int32", typeof(int)),
            CreateTraversalObjectTypeDefinition(nameof(Customer), typeof(Customer)),
            CreateTraversalObjectTypeDefinition(nameof(Order), typeof(Order))
        ],
        ApiRelationships:
        [
            new ApiRelationshipOneToManyDef
            (
                ApiName: "CustomerOrders",
                ApiPrincipalEnd: new ApiRelationshipPrincipalEndDef
                (
                    new ApiTypeReferenceDef(ApiTypeKind.Object, nameof(Customer), null),
                    new ApiRelationshipTraversalDef
                    (
                        "Orders",
                        new ClrMemberReferenceDef(ClrMemberKind.Property, nameof(Customer.Orders))
                    )
                ),
                ApiDependentEnd: new ApiRelationshipDependentEndDef
                (
                    new ApiTypeReferenceDef(ApiTypeKind.Object, nameof(Order), null),
                    new ApiRelationshipTraversalDef
                    (
                        "Customer",
                        new ClrMemberReferenceDef(ClrMemberKind.Property, nameof(Order.Customer))
                    )
                )
            )
        ]
    );

    private static ApiObjectTypeDef CreateTraversalObjectTypeDefinition(string apiName, Type clrType) => new
    (
        ApiName: apiName,
        ClrType: clrType,
        ApiProperties:
        [
            new ApiPropertyDef
            (
                ApiName: nameof(Customer.Id),
                ApiTypeExpression: new ApiTypeExpressionDef
                (
                    null,
                    new ApiTypeReferenceDef(ApiTypeKind.Scalar, "Int32", null)
                ),
                ApiTypeModifiers: ApiTypeModifiers.Required,
                ClrValueMember: new ClrMemberReferenceDef(ClrMemberKind.Property, nameof(Customer.Id))
            )
        ]
    );
    #endregion
}
