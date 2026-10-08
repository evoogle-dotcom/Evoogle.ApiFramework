// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.NTree;
using Evoogle.XUnit;

using FluentAssertions;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema.Json;

public class RequiredEnumJsonSerializationTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private enum RequiredEnumIdentity
    {
        Collection,
        OneToOne,
        Property
    }

    private sealed class RequiredEnumTest : XUnitTest
    {
        #region User Supplied Properties
        public required RequiredEnumIdentity Identity { get; init; }
        public required JsonIgnoreCondition IgnoreCondition { get; init; }
        public required bool UsesCamelCase { get; init; }
        public required bool ShouldRoundtrip { get; init; }
        #endregion

        #region Calculated Properties
        private object? SourceMetadata { get; set; }
        private JsonSerializerOptions? Options { get; set; }
        private string? ActualJson { get; set; }
        private object? ActualMetadata { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Options = CreateOptions(this.UsesCamelCase, this.IgnoreCondition);
            this.SourceMetadata = this.Identity switch
            {
                RequiredEnumIdentity.Collection => new ApiCollectionType
                (
                    new ApiTypeExpression(new ApiScalarType("Text", typeof(string))),
                    ApiTypeModifiers.None,
                    typeof(List<string>)
                ),
                RequiredEnumIdentity.OneToOne => new ApiRelationshipOneToOne
                (
                    "Profile",
                    new ApiRelationshipPrincipalEnd(new ApiTypeReference(ApiTypeKind.Object, "Principal")),
                    new ApiRelationshipDependentEnd(new ApiTypeReference(ApiTypeKind.Object, "Dependent")),
                    ApiRelationshipDeleteBehavior.None
                ),
                RequiredEnumIdentity.Property => new ClrMemberReference(ClrMemberKind.Property, "DisplayName"),
                _ => throw new InvalidOperationException("Unknown required enum identity.")
            };

            this.WriteLine($"Source Metadata:   {this.SourceMetadata.SafeToString()}");
            this.WriteLine($"Expected Identity: {this.Identity}");
            this.WriteLine($"Ignore Condition:  {this.IgnoreCondition}");
            this.WriteLine($"Uses Camel Case:   {this.UsesCamelCase}");
            this.WriteLine($"Should Roundtrip:  {this.ShouldRoundtrip}");
            this.WriteLine();
        }

        protected override void Act()
        {
            this.ActualJson = this.Identity switch
            {
                RequiredEnumIdentity.Collection => JsonSerializer.Serialize((ApiType)this.SourceMetadata!, this.Options),
                RequiredEnumIdentity.OneToOne => JsonSerializer.Serialize((ApiRelationship)this.SourceMetadata!, this.Options),
                RequiredEnumIdentity.Property => JsonSerializer.Serialize((ClrMemberReference)this.SourceMetadata!, this.Options),

                _ => throw new InvalidOperationException("Unknown required enum identity.")
            };

            this.WriteLine($"Actual JSON:\n{this.ActualJson.SafeToString()}");
            this.WriteLine();

            if (this.ShouldRoundtrip)
            {
                this.ActualMetadata = this.Identity switch
                {
                    RequiredEnumIdentity.Collection => JsonSerializer.Deserialize<ApiType>(this.ActualJson, this.Options),
                    RequiredEnumIdentity.OneToOne => JsonSerializer.Deserialize<ApiRelationship>(this.ActualJson, this.Options),
                    RequiredEnumIdentity.Property => JsonSerializer.Deserialize<ClrMemberReference>(this.ActualJson, this.Options),

                    _ => throw new InvalidOperationException("Unknown required enum identity.")
                };
                this.WriteLine($"Actual Metadata: {this.ActualMetadata.SafeToString()}");
                this.WriteLine();
            }
        }

        protected override void Assert()
        {
            using var document = JsonDocument.Parse(this.ActualJson!);
            var identityName = this.Identity == RequiredEnumIdentity.Property
                ? nameof(ClrMemberReference.ClrKind)
                : nameof(ApiType.ApiKind);
            var identityProperties = document.RootElement.EnumerateObject()
                .Where(property => string.Equals(property.Name, identityName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            this.WriteLine($"Expected Required Property: {GetJsonName(identityName, this.Options!)}={this.Identity}");
            this.WriteLine($"Expected Required Property Count: 1; Actual: {identityProperties.Length}");
            foreach (var identityProperty in identityProperties)
            {
                this.WriteLine($"Actual Required Property: {identityProperty.Name}={identityProperty.Value.GetRawText()}");
            }
            this.WriteLine();

            identityProperties.Should().ContainSingle();
            identityProperties[0].Name.Should().Be(GetJsonName(identityName, this.Options!));
            identityProperties[0].Value.GetString().Should().Be(this.Identity.ToString());

            var optionalName = this.Identity switch
            {
                RequiredEnumIdentity.Collection => nameof(ApiCollectionType.ApiItemTypeModifiers),
                RequiredEnumIdentity.OneToOne => nameof(ApiRelationship.ApiDeleteBehavior),
                _ => null
            };
            if (optionalName is not null)
            {
                var optionalProperties = document.RootElement.EnumerateObject()
                    .Where(property => property.Name == GetJsonName(optionalName, this.Options!))
                    .ToArray();
                var expectedOptionalCount = this.IgnoreCondition == JsonIgnoreCondition.WhenWritingDefault ? 0 : 1;
                this.WriteLine($"Optional Default Property: {GetJsonName(optionalName, this.Options!)}=None");
                this.WriteLine($"Expected Optional Property Count: {expectedOptionalCount}; Actual: {optionalProperties.Length}");
                foreach (var optionalProperty in optionalProperties)
                {
                    this.WriteLine($"Actual Optional Property: {optionalProperty.Value.GetRawText()}");
                }
                this.WriteLine();

                if (this.IgnoreCondition == JsonIgnoreCondition.WhenWritingDefault)
                {
                    optionalProperties.Should().BeEmpty();
                }
                else
                {
                    optionalProperties.Should().ContainSingle();
                    optionalProperties[0].Value.GetString().Should().Be("None");
                }
            }

            if (this.ShouldRoundtrip)
            {
                this.AssertRoundtrip();
            }
        }
        #endregion

        #region Assertion Methods
        private void AssertRoundtrip()
        {
            this.ActualMetadata.Should().NotBeSameAs(this.SourceMetadata);
            switch (this.Identity)
            {
                case RequiredEnumIdentity.Collection:
                    var collection = this.ActualMetadata.Should().BeOfType<ApiCollectionType>().Which;
                    collection.ApiKind.Should().Be(ApiTypeKind.Collection);
                    collection.ClrType.Should().Be<List<string>>();
                    collection.ApiItemTypeModifiers.Should().Be(ApiTypeModifiers.None);
                    collection.ApiItemTypeExpression.IsInline.Should().BeTrue();
                    collection.ApiItemTypeExpression.ApiTypeReference.Should().BeNull();

                    var scalar = collection.ApiItemTypeExpression.ApiInlineType.Should().BeOfType<ApiScalarType>().Which;
                    scalar.ApiKind.Should().Be(ApiTypeKind.Scalar);
                    scalar.ApiName.Should().Be("Text");
                    scalar.ClrType.Should().Be<string>();
                    break;

                case RequiredEnumIdentity.OneToOne:
                    var relationship = this.ActualMetadata.Should().BeOfType<ApiRelationshipOneToOne>().Which;
                    var sourceRelationship = (ApiRelationshipOneToOne)this.SourceMetadata!;
                    relationship.ApiKind.Should().Be(ApiRelationshipKind.OneToOne);
                    relationship.ApiName.Should().Be("Profile");
                    relationship.ApiDeleteBehavior.Should().Be(ApiRelationshipDeleteBehavior.None);
                    relationship.ApiPrincipalEnd.ApiObjectTypeReference.Should().Be(sourceRelationship.ApiPrincipalEnd.ApiObjectTypeReference);
                    relationship.ApiDependentEnd.ApiObjectTypeReference.Should().Be(sourceRelationship.ApiDependentEnd.ApiObjectTypeReference);
                    relationship.ApiPrincipalEnd.ApiPrincipalKeyName.Should().BeNull();
                    relationship.ApiDependentEnd.HasForeignKey.Should().BeFalse();
                    break;

                case RequiredEnumIdentity.Property:
                    var reference = this.ActualMetadata.Should().BeOfType<ClrMemberReference>().Which;
                    reference.ClrKind.Should().Be(ClrMemberKind.Property);
                    reference.ClrName.Should().Be("DisplayName");
                    break;
            }
        }
        #endregion
    }

    private sealed class SchemaRoundtripTest : XUnitTest
    {
        #region User Supplied Properties
        public required bool UsesCamelCase { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? SourceSchema { get; set; }
        private ApiSchema? ActualSchema { get; set; }
        private JsonSerializerOptions? Options { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.SourceSchema = BuildTestApiSchema(ApiSchemaKind.Relationship);
            this.Options = CreateOptions(this.UsesCamelCase, JsonIgnoreCondition.WhenWritingDefault);

            this.WriteLine($"Source Schema:    {this.SourceSchema.SafeToString()}");
            this.WriteLine($"Ignore Condition: {this.Options.DefaultIgnoreCondition}");
            this.WriteLine($"Uses Camel Case:  {this.UsesCamelCase}");
            this.WriteLine();
        }

        protected override void Act()
        {
            var json = JsonSerializer.Serialize(this.SourceSchema, this.Options);
            this.WriteLine($"Serialized Schema JSON:\n{json}");
            this.WriteLine();
            this.ActualSchema = JsonSerializer.Deserialize<ApiSchema>(json, this.Options);
            this.WriteLine($"Actual Schema: {this.ActualSchema.SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            var sourceSchema = this.SourceSchema!;
            var schema = this.ActualSchema.Should().BeOfType<ApiSchema>().Which;
            schema.Should().NotBeSameAs(sourceSchema);
            schema.ApiName.Should().Be(sourceSchema.ApiName);
            schema.ApiObjectTypes.Should().HaveSameCount(sourceSchema.ApiObjectTypes);
            schema.ApiRelationships.Should().HaveSameCount(sourceSchema.ApiRelationships);

            var sourceCollections = sourceSchema.SelfAndDescendants(TraversalStrategy.DepthFirst).OfType<ApiCollectionType>().ToArray();
            sourceCollections.Should().NotBeEmpty();
            var collections = schema.SelfAndDescendants(TraversalStrategy.DepthFirst).OfType<ApiCollectionType>().ToArray();
            this.WriteLine($"Expected Collection Count: {sourceCollections.Length}; Actual: {collections.Length}");
            collections.Should().HaveSameCount(sourceCollections);
            foreach (var (sourceCollection, collection) in sourceCollections.Zip(collections))
            {
                this.WriteLine($"Expected Collection: {sourceCollection.SafeToString()}");
                this.WriteLine($"Actual Collection:   {collection.SafeToString()}");
                this.WriteLine($"Expected Item Type:  {sourceCollection.ApiItemType.SafeToString()}");
                this.WriteLine($"Actual Item Type:    {collection.ApiItemType.SafeToString()}");
                this.WriteLine();
                collection.ApiKind.Should().Be(ApiTypeKind.Collection);
                collection.ClrType.Should().Be(sourceCollection.ClrType);
                collection.ApiItemTypeModifiers.Should().Be(sourceCollection.ApiItemTypeModifiers);
                collection.ApiItemType.ApiKind.Should().Be(sourceCollection.ApiItemType.ApiKind);
                collection.ApiItemType.ClrType.Should().Be(sourceCollection.ApiItemType.ClrType);
            }

            var sourceProperties = sourceSchema.SelfAndDescendants(TraversalStrategy.DepthFirst).OfType<ApiProperty>().ToArray();
            sourceProperties.Should().Contain(property => property.ClrKind == ClrMemberKind.Property);
            foreach (var sourceObjectType in sourceSchema.ApiObjectTypes)
            {
                this.WriteLine($"Object Lookup API Name: {sourceObjectType.ApiName}; CLR Type: {sourceObjectType.ClrType.SafeToString()}");
                schema.TryGetObjectTypeByApiName(sourceObjectType.ApiName, out var objectType).Should().BeTrue();
                schema.TryGetObjectTypeByClrType(sourceObjectType.ClrType, out var clrObjectType).Should().BeTrue();
                this.WriteLine($"Actual Object By API Name: {objectType.SafeToString()}");
                this.WriteLine($"Actual Object By CLR Type: {clrObjectType.SafeToString()}");
                objectType.Should().BeSameAs(clrObjectType);
                objectType!.ApiProperties.Should().HaveSameCount(sourceObjectType.ApiProperties);
                foreach (var sourceProperty in sourceObjectType.ApiProperties)
                {
                    this.WriteLine($"Property Lookup API Name: {sourceProperty.ApiName}");
                    this.WriteLine($"Expected CLR Member: {sourceProperty.ClrValueMember.SafeToString()}");
                    objectType.TryGetPropertyByApiName(sourceProperty.ApiName, out var property).Should().BeTrue();
                    this.WriteLine($"Actual CLR Member:   {property!.ClrValueMember.SafeToString()}");
                    property!.ClrValueMember.Should().Be(sourceProperty.ClrValueMember);
                }
                this.WriteLine();
            }

            var sourceRelationships = sourceSchema.ApiRelationships.OfType<ApiRelationshipOneToOne>().ToArray();
            sourceRelationships.Should().NotBeEmpty();
            schema.ApiRelationships.OfType<ApiRelationshipOneToOne>().Should().HaveSameCount(sourceRelationships);
            foreach (var sourceRelationship in sourceRelationships)
            {
                this.WriteLine($"Relationship Lookup API Name: {sourceRelationship.ApiName}");
                this.WriteLine($"Expected Relationship: {sourceRelationship.SafeToString()}");
                schema.TryGetRelationshipByApiName(sourceRelationship.ApiName, out var relationship).Should().BeTrue();
                this.WriteLine($"Actual Relationship:   {relationship.SafeToString()}");
                this.WriteLine();

                var oneToOne = relationship.Should().BeOfType<ApiRelationshipOneToOne>().Which;
                oneToOne.ApiKind.Should().Be(ApiRelationshipKind.OneToOne);
                oneToOne.ApiDeleteBehavior.Should().Be(sourceRelationship.ApiDeleteBehavior);
                oneToOne.ApiPrincipalEnd.ApiObjectTypeReference.Should().Be(sourceRelationship.ApiPrincipalEnd.ApiObjectTypeReference);
                oneToOne.ApiDependentEnd.ApiObjectTypeReference.Should().Be(sourceRelationship.ApiDependentEnd.ApiObjectTypeReference);
                schema.TryGetObjectTypeByApiName(oneToOne.ApiPrincipalEnd.ApiObjectType.ApiName, out var principalType).Should().BeTrue();
                schema.TryGetObjectTypeByApiName(oneToOne.ApiDependentEnd.ApiObjectType.ApiName, out var dependentType).Should().BeTrue();
                oneToOne.ApiPrincipalEnd.ApiObjectType.Should().BeSameAs(principalType);
                oneToOne.ApiDependentEnd.ApiObjectType.Should().BeSameAs(dependentType);
            }
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] SerializeTheoryData => CreateRequiredEnumTheoryData(false);

    public static TheoryDataRow<IXUnitTest>[] JsonRoundtripTheoryData => CreateRequiredEnumTheoryData(true);

    public static TheoryDataRow<IXUnitTest>[] SchemaJsonRoundtripTheoryData =>
    [
        new SchemaRoundtripTest { Name = "Schema with Pascal-case JSON ignoring defaults", UsesCamelCase = false },
        new SchemaRoundtripTest { Name = "Schema with camel-case JSON ignoring defaults", UsesCamelCase = true }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(SerializeTheoryData))]
    public void SerializePreservesRequiredEnumIdentitiesWhenIgnoringDefaults(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(JsonRoundtripTheoryData))]
    public void JsonRoundtripPreservesRequiredEnumIdentitiesWhenIgnoringDefaults(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(SchemaJsonRoundtripTheoryData))]
    public void SchemaJsonRoundtripPreservesRequiredEnumIdentitiesWhenIgnoringDefaults(IXUnitTest test)
        => test.Execute(this);
    #endregion

    #region Implementation Methods
    private static TheoryDataRow<IXUnitTest>[] CreateRequiredEnumTheoryData(bool shouldRoundtrip) =>
    [
        .. from identity in Enum.GetValues<RequiredEnumIdentity>()
           from ignoreCondition in new[]
           {
               JsonIgnoreCondition.Never,
               JsonIgnoreCondition.WhenWritingNull,
               JsonIgnoreCondition.WhenWritingDefault
           }
           from usesCamelCase in new[] { false, true }
           select (TheoryDataRow<IXUnitTest>)new RequiredEnumTest
           {
               Name = $"{identity}; {ignoreCondition}; camel-case={usesCamelCase}; roundtrip={shouldRoundtrip}",
               Identity = identity,
               IgnoreCondition = ignoreCondition,
               UsesCamelCase = usesCamelCase,
               ShouldRoundtrip = shouldRoundtrip
           }
    ];

    private static JsonSerializerOptions CreateOptions(bool usesCamelCase, JsonIgnoreCondition ignoreCondition) => new()
    {
        PropertyNamingPolicy = usesCamelCase ? JsonNamingPolicy.CamelCase : null,
        DefaultIgnoreCondition = ignoreCondition
    };

    private static string GetJsonName(string propertyName, JsonSerializerOptions options)
        => options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName;
    #endregion
}
