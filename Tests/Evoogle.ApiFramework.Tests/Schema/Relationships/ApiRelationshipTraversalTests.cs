// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;
using Evoogle.ApiFramework.Schema.Configuration;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Relationships;

// public sealed class ApiRelationshipTraversalTests(ITestOutputHelper output) : XUnitTests(output)
// {
//     #region Test Types
//     private sealed class Order
//     {
//         public int Id { get; set; }

//         public Person? ContactPerson { get; set; }

//         public List<Person>? People { get; set; }
//     }

//     private sealed class Person
//     {
//         public int Id { get; set; }

//         public Order? Order { get; set; }
//     }

//     private sealed class OrderPersonLink
//     { }

//     private sealed class CompileTest : XUnitTest
//     {
//         #region User Supplied Properties
//         public required bool UseClrBindings { get; init; }
//         #endregion

//         #region Calculated Properties
//         private ApiSchema? _schema;
//         private string? _json;
//         #endregion

//         #region XUnitTest Methods
//         protected override void Arrange()
//         { }

//         protected override void Act()
//         {
//             var relationshipBuilder = new ApiRelationshipOneToManyBuilder("ordersPeople")
//                 .From
//                 (
//                     typeof(Order),
//                     end => end.WithTraversal
//                     (
//                         "people",
//                         this.UseClrBindings ? nameof(Order.People) : null
//                     )
//                 )
//                 .To
//                 (
//                     typeof(Person),
//                     end => end.WithTraversal
//                     (
//                         "order",
//                         this.UseClrBindings ? nameof(Person.Order) : null
//                     )
//                 );

//             var schema = new ApiSchema
//             (
//                 "TraversalSchema",
//                 apiVersion: null,
//                 apiOptions: null,
//                 apiNamedTypes:
//                 [
//                     new ApiScalarType("Int32", typeof(int)),
//                     new ApiObjectType
//                     (
//                         "Order", null,
//                         [
//                             new ApiProperty
//                             (
//                                 "id", new ApiTypeExpression(ApiTypeReference.ClrRef<int>()),
//                                 ApiTypeModifiers.Required, nameof(Order.Id), ClrMemberKind.Property
//                             ),
//                             new ApiProperty
//                             (
//                                 "contactPerson",
//                                 new ApiTypeExpression(ApiTypeReference.ClrRef<Person>()),
//                                 ApiTypeModifiers.None, nameof(Order.ContactPerson),
//                                 ClrMemberKind.Property
//                             )
//                         ],
//                         null, null, typeof(Order)
//                     ),
//                     new ApiObjectType
//                     (
//                         "Person", null,
//                         [
//                             new ApiProperty
//                             (
//                                 "id", new ApiTypeExpression(ApiTypeReference.ClrRef<int>()),
//                                 ApiTypeModifiers.Required, nameof(Person.Id), ClrMemberKind.Property
//                             )
//                         ],
//                         null, null, typeof(Person)
//                     )
//                 ],
//                 apiRelationships: [relationshipBuilder.Build()]
//             );
//             ApiSchemaCompiler.Compile(schema).ThrowIfInvalid();

//             var options = new JsonSerializerOptions
//             {
//                 DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
//             };
//             _json = JsonSerializer.Serialize(schema, options);
//             _schema = JsonSerializer.Deserialize<ApiSchema>(_json, options);
//         }

//         protected override void Assert()
//         {
//             _schema.Should().NotBeNull();
//             var orderType = _schema!.GetObjectTypeByApiName("Order");
//             var personType = _schema.GetObjectTypeByApiName("Person");
//             var people = orderType.ApiRelationshipTraversals.Should().ContainSingle().Subject;
//             var order = personType.ApiRelationshipTraversals.Should().ContainSingle().Subject;

//             people.ApiName.Should().Be("people");
//             orderType.TryGetTraversalByApiName("people", out var matchedTraversal)
//                 .Should().BeTrue();
//             matchedTraversal.Should().BeSameAs(people);
//             orderType.TryGetTraversalByApiName("People", out _).Should().BeFalse();
//             people.SourceObjectType.Should().BeSameAs(orderType);
//             people.TargetObjectType.Should().BeSameAs(personType);
//             people.IsToMany.Should().BeTrue();
//             people.HasClrMember.Should().Be(this.UseClrBindings);
//             orderType.ApiProperties.Should().ContainSingle(property =>
//                 property.ApiName == "contactPerson" &&
//                 ReferenceEquals(property.ApiType, personType));
//             orderType.ApiProperties.Should().NotContain(property => property.ApiName == "people");

//             order.ApiName.Should().Be("order");
//             order.SourceObjectType.Should().BeSameAs(personType);
//             order.TargetObjectType.Should().BeSameAs(orderType);
//             order.IsToMany.Should().BeFalse();
//             order.HasClrMember.Should().Be(this.UseClrBindings);
//             if (!this.UseClrBindings)
//             {
//                 _json.Should().NotContain("\"ClrMemberName\"");
//             }
//         }
//         #endregion
//     }

//     private sealed class DiscoveryTest : XUnitTest
//     {
//         #region User Supplied Properties
//         public string? ExplicitPropertyApiName { get; init; }
//         #endregion

//         #region Calculated Properties
//         private ApiSchemaCompilationResult? _result;
//         #endregion

//         #region XUnitTest Methods
//         protected override void Arrange()
//         { }

//         protected override void Act()
//         {
//             var builder = new ApiSchemaBuilder()
//                 .WithName("DiscoverySchema")
//                 .AddScalar<int>()
//                 .AddObject<Order>(order =>
//                 {
//                     if (this.ExplicitPropertyApiName is not null)
//                     {
//                         order.AddProperty(this.ExplicitPropertyApiName, nameof(Order.People));
//                     }
//                 })
//                 .AddObject<Person>()
//                 .UsePropertyDiscovery()
//                 .AddOneToManyRelationship
//                 (
//                     "ordersPeople",
//                     relationship => relationship
//                         .From
//                         (
//                             typeof(Order),
//                             end => end.WithTraversal("people", nameof(Order.People))
//                         )
//                         .To(typeof(Person), end => end.WithTraversal("order", nameof(Person.Order)))
//                 );

//             _result = builder.BuildResult();
//         }

//         protected override void Assert()
//         {
//             _result.Should().NotBeNull();
//             if (this.ExplicitPropertyApiName is not null)
//             {
//                 _result!.Schema.Should().BeNull();
//                 var expectedCode = this.ExplicitPropertyApiName == "people"
//                     ? ApiSchemaCompilationCode.ApiRelationshipTraversalDuplicateApiName
//                     : ApiSchemaCompilationCode.ApiRelationshipTraversalClrMemberConflict;
//                 _result.Issues.Should().ContainSingle(issue =>
//                     issue.Code == expectedCode);
//                 if (this.ExplicitPropertyApiName == "people")
//                 {
//                     _result.Issues.Should().ContainSingle(issue =>
//                         issue.Code ==
//                             ApiSchemaCompilationCode.ApiRelationshipTraversalClrMemberConflict);
//                 }
//                 return;
//             }

//             _result!.Schema.Should().NotBeNull();
//             var orderType = _result.Schema!.GetObjectTypeByApiName("Order");
//             var personType = _result.Schema.GetObjectTypeByApiName("Person");
//             orderType.ApiProperties.Should().Contain(property =>
//                 property.ClrName == nameof(Order.ContactPerson));
//             orderType.ApiProperties.Should().NotContain(property =>
//                 property.ClrName == nameof(Order.People));
//             personType.ApiProperties.Should().NotContain(property =>
//                 property.ClrName == nameof(Person.Order));
//         }
//         #endregion
//     }

//     private sealed class ManyToManyTest : XUnitTest
//     {
//         #region Calculated Properties
//         private ApiSchema? _schema;
//         #endregion

//         #region XUnitTest Methods
//         protected override void Arrange()
//         { }

//         protected override void Act()
//         {
//             var relationship = new ApiRelationshipManyToMany
//             (
//                 "orderPeople",
//                 new ApiRelationshipPrincipalEnd
//                 (
//                     new ApiTypeReference(typeof(Order)),
//                     new ApiRelationshipTraversal("people")
//                 ),
//                 new ApiRelationshipPrincipalEnd
//                 (
//                     new ApiTypeReference(typeof(Person)),
//                     new ApiRelationshipTraversal("orders")
//                 ),
//                 new ApiRelationshipAssociation
//                 (
//                     new ApiTypeReference(typeof(OrderPersonLink))
//                 )
//             );
//             var schema = new ApiSchema
//             (
//                 "ManyToManyTraversals", null, null,
//                 apiNamedTypes:
//                 [
//                     new ApiObjectType("Order", null, null, null, null, typeof(Order)),
//                     new ApiObjectType("Person", null, null, null, null, typeof(Person)),
//                     new ApiObjectType
//                     (
//                         "OrderPersonLink", null, null, null, null,
//                         typeof(OrderPersonLink)
//                     )
//                 ],
//                 apiRelationships: [relationship]
//             );
//             ApiSchemaCompiler.Compile(schema).ThrowIfInvalid();
//             _schema = schema;
//         }

//         protected override void Assert()
//         {
//             _schema.Should().NotBeNull();
//             var order = _schema!.GetObjectTypeByApiName("Order");
//             var person = _schema.GetObjectTypeByApiName("Person");
//             order.ApiRelationshipTraversals.Should().ContainSingle().Subject
//                 .IsToMany.Should().BeTrue();
//             person.ApiRelationshipTraversals.Should().ContainSingle().Subject
//                 .IsToMany.Should().BeTrue();
//         }
//         #endregion
//     }

//     private sealed class SingularTraversalTest : XUnitTest
//     {
//         #region User Supplied Properties
//         public required bool IsPrincipalEnd { get; init; }
//         #endregion

//         #region Calculated Properties
//         private ApiRelationshipEnd? _end;
//         private ApiRelationshipEnd? _endWithoutTraversal;
//         private string? _json;
//         #endregion

//         #region XUnitTest Methods
//         protected override void Arrange()
//         { }

//         protected override void Act()
//         {
//             if (this.IsPrincipalEnd)
//             {
//                 _end = new ApiRelationshipPrincipalEndBuilder(typeof(Order))
//                     .WithTraversal("first")
//                     .WithTraversal("second")
//                     .Build();
//                 _endWithoutTraversal = new ApiRelationshipPrincipalEnd
//                 (
//                     new ApiTypeReference(typeof(Order))
//                 );
//             }
//             else
//             {
//                 _end = new ApiRelationshipDependentEndBuilder(typeof(Order))
//                     .WithTraversal("first")
//                     .WithTraversal("second")
//                     .Build();
//                 _endWithoutTraversal = new ApiRelationshipDependentEnd
//                 (
//                     new ApiTypeReference(typeof(Order))
//                 );
//             }

//             _json = JsonSerializer.Serialize(_end, _end.GetType());
//         }

//         protected override void Assert()
//         {
//             _end.Should().NotBeNull();
//             _end!.ApiTraversal.Should().NotBeNull();
//             _end.ApiTraversal!.ApiName.Should().Be("second");
//             _endWithoutTraversal!.ApiTraversal.Should().BeNull();
//             _json.Should().Contain("\"ApiTraversal\":{");
//             _json.Should().NotContain("ApiTraversals");
//             var withoutTraversalJson = JsonSerializer.Serialize
//             (
//                 _endWithoutTraversal, _endWithoutTraversal.GetType()
//             );
//             withoutTraversalJson.Should().NotContain("ApiTraversal");

//             var roundTrip = (ApiRelationshipEnd?)JsonSerializer.Deserialize
//             (
//                 _json!, _end.GetType()
//             );
//             roundTrip!.ApiTraversal!.ApiName.Should().Be("second");
//         }
//         #endregion
//     }

//     private sealed class DuplicateTraversalApiNameTest : XUnitTest
//     {
//         #region Calculated Properties
//         private ApiSchemaCompilationResult? _result;
//         #endregion

//         #region XUnitTest Methods
//         protected override void Arrange()
//         { }

//         protected override void Act()
//         {
//             var relationships = new[]
//             {
//                 CreateRelationship("firstRelationship"),
//                 CreateRelationship("secondRelationship")
//             };
//             var schema = new ApiSchema
//             (
//                 "DuplicateTraversalSchema", null, null,
//                 apiScalarTypes: null,
//                 apiEnumTypes: null,
//                 apiObjectTypes:
//                 [
//                     new ApiObjectType("Order", null, null, null, null, typeof(Order)),
//                     new ApiObjectType("Person", null, null, null, null, typeof(Person))
//                 ],
//                 apiRelationships: relationships
//             );

//             _result = ApiSchemaCompiler.Compile(schema);
//         }

//         protected override void Assert()
//         {
//             _result.Should().NotBeNull();
//             _result!.Schema.Should().BeNull();
//             _result.Issues.Should().ContainSingle(issue =>
//                 issue.Code == ApiSchemaCompilationCode.ApiRelationshipTraversalDuplicateApiName);
//         }
//         #endregion

//         #region Factory Methods
//         private static ApiRelationshipOneToOne CreateRelationship(string apiName) => new
//         (
//             apiName,
//             new ApiRelationshipPrincipalEnd
//             (
//                 new ApiTypeReference(typeof(Order)),
//                 new ApiRelationshipTraversal("person")
//             ),
//             new ApiRelationshipDependentEnd(new ApiTypeReference(typeof(Person)))
//         );
//         #endregion
//     }
//     #endregion

//     #region Theory Data
//     public static TheoryDataRow<IXUnitTest>[] CompileTheoryData =>
//     [
//         new CompileTest { Name = "Traversal without CLR members", UseClrBindings = false },
//         new CompileTest { Name = "Traversal with CLR members", UseClrBindings = true }
//     ];

//     public static TheoryDataRow<IXUnitTest>[] DiscoveryTheoryData =>
//     [
//         new DiscoveryTest
//         {
//             Name = "Discovery leaves contained values and removes bound navigation members",
//             ExplicitPropertyApiName = null
//         },
//         new DiscoveryTest
//         {
//             Name = "Explicit property conflicts with navigation member binding",
//             ExplicitPropertyApiName = "people"
//         },
//         new DiscoveryTest
//         {
//             Name = "Distinct API names still conflict when they bind the same CLR member",
//             ExplicitPropertyApiName = "containedPeople"
//         }
//     ];

//     public static TheoryDataRow<IXUnitTest>[] ManyToManyTheoryData =>
//     [
//         new ManyToManyTest { Name = "Both many-to-many directions are to-many" }
//     ];

//     public static TheoryDataRow<IXUnitTest>[] SingularTraversalTheoryData =>
//     [
//         new SingularTraversalTest { Name = "Principal end has one traversal", IsPrincipalEnd = true },
//         new SingularTraversalTest { Name = "Dependent end has one traversal", IsPrincipalEnd = false }
//     ];

//     public static TheoryDataRow<IXUnitTest>[] DuplicateTraversalApiNameTheoryData =>
//     [
//         new DuplicateTraversalApiNameTest
//         {
//             Name = "Distinct relationships report one duplicate traversal name issue"
//         }
//     ];
//     #endregion

//     #region Test Methods
//     [Theory]
//     [MemberData(nameof(CompileTheoryData))]
//     public void Compile(IXUnitTest test) => test.Execute(this);

//     [Theory]
//     [MemberData(nameof(DiscoveryTheoryData))]
//     public void Discovery(IXUnitTest test) => test.Execute(this);

//     [Theory]
//     [MemberData(nameof(ManyToManyTheoryData))]
//     public void ManyToMany(IXUnitTest test) => test.Execute(this);

//     [Theory]
//     [MemberData(nameof(SingularTraversalTheoryData))]
//     public void SingularTraversal(IXUnitTest test) => test.Execute(this);

//     [Theory]
//     [MemberData(nameof(DuplicateTraversalApiNameTheoryData))]
//     public void DuplicateTraversalApiName(IXUnitTest test) => test.Execute(this);
//     #endregion
// }
