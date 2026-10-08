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

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
    #region Inline Object Compilation Test Types
    private sealed class InlineObjectCompilationTest : XUnitTest
    {
        #region User Supplied Properties
        public required InlineObjectShape Shape { get; init; }
        public required bool UsesJsonDeserialization { get; init; }
        public required (string ApiName, Type ClrType)[] ExpectedInlineTypes { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchemaDef? SchemaDefinition { get; set; }
        private string? SourceJson { get; set; }
        private ApiSchema? ActualSchema { get; set; }
        private JsonSerializerOptions? Options { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.SchemaDefinition = CreateInlineObjectSchemaDefinition(this.Shape);
            this.Options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.Never };
            this.WriteLine($"Inline Object Shape: {this.Shape}");
            this.WriteLine($"Compilation Path:    {(this.UsesJsonDeserialization ? "Root JSON" : "Descriptors")}");
            this.WriteLine($"Schema API Name:     {this.SchemaDefinition.ApiName}");
            this.WriteLine();
            if (this.UsesJsonDeserialization)
            {
                this.SourceJson = JsonSerializer.Serialize(BuildApiSchema(this.SchemaDefinition), this.Options);
                this.WriteLine($"Source JSON:\n{this.SourceJson.SafeToString()}");
                this.WriteLine();
            }
        }

        protected override void Act()
        {
            this.ActualSchema = this.UsesJsonDeserialization
                ? JsonSerializer.Deserialize<ApiSchema>(this.SourceJson!, this.Options)
                : BuildApiSchema(this.SchemaDefinition);
            this.WriteLine($"Actual Schema: {this.ActualSchema.SafeToString()}");
            this.WriteLine();
        }

        protected override void Assert()
        {
            var schema = this.ActualSchema.Should().BeOfType<ApiSchema>().Which;
            var inlineTypes = schema.SelfAndDescendants(TraversalStrategy.DepthFirst)
                .OfType<ApiObjectType>().Where(objectType => !ReferenceEquals(objectType.Parent, schema)).ToArray();

            this.WriteLine($"Expected Inline Objects: {string.Join(", ", this.ExpectedInlineTypes.Select(type => type.ApiName))}");
            this.WriteLine($"Actual Inline Objects:   {string.Join(", ", inlineTypes.Select(type => type.ApiName))}");
            inlineTypes.Select(objectType => (objectType.ApiName, objectType.ClrType))
                .Should().BeEquivalentTo(this.ExpectedInlineTypes);

            foreach (var inlineType in inlineTypes)
            {
                inlineType.Root.Should().BeSameAs(schema);
                var containedType = inlineType.Parent switch
                {
                    ApiProperty property => property.ApiType,
                    ApiCollectionType collection => collection.ApiItemType,
                    _ => null
                };
                containedType.Should().BeSameAs(inlineType);

                schema.ApiObjectTypes.Should().NotContain(inlineType);
                schema.ApiNamedTypes.Should().NotContain(inlineType);

                this.WriteLine();
                this.WriteLine("Expected Schema Registry Lookups: false with null outputs");

                var hasObjectByApiName = schema.TryGetObjectTypeByApiName(inlineType.ApiName, out var byApiName);
                this.WriteLine($"Object Lookup By API Name: {hasObjectByApiName}; Output: {byApiName.SafeToString()}");
                hasObjectByApiName.Should().BeFalse();
                byApiName.Should().BeNull();

                var hasObjectByClrType = schema.TryGetObjectTypeByClrType(inlineType.ClrType, out var byClrType);
                this.WriteLine($"Object Lookup By CLR Type: {hasObjectByClrType}; Output: {byClrType.SafeToString()}");
                hasObjectByClrType.Should().BeFalse();
                byClrType.Should().BeNull();

                var hasTypeByApiName = schema.TryGetTypeByApiName(inlineType.ApiName, out var namedType);
                this.WriteLine($"Type Lookup By API Name: {hasTypeByApiName}; Output: {namedType.SafeToString()}");
                hasTypeByApiName.Should().BeFalse();
                namedType.Should().BeNull();

                var hasTypeByClrType = schema.TryGetTypeByClrType(inlineType.ClrType, out var clrType);
                this.WriteLine($"Type Lookup By CLR Type: {hasTypeByClrType}; Output: {clrType.SafeToString()}");
                hasTypeByClrType.Should().BeFalse();
                clrType.Should().BeNull();
                this.WriteLine();
            }
        }
        #endregion
    }
    #endregion

    #region Inline Object Compilation Theory Data
    public static TheoryDataRow<IXUnitTest>[] InlineObjectCompilationTheoryData =>
    [
        .. from fixture in new (InlineObjectShape Shape, (string ApiName, Type ClrType)[] InlineTypes)[]
           {
               (InlineObjectShape.Direct, [(nameof(InlineLeaf), typeof(InlineLeaf))]),
               (InlineObjectShape.Collection, [(nameof(InlineLeaf), typeof(InlineLeaf))]),
               (InlineObjectShape.NestedCollections, [(nameof(InlineContainer), typeof(InlineContainer)), (nameof(InlineLeaf), typeof(InlineLeaf))])
           }
           from usesJsonDeserialization in new[] { false, true }
           select (TheoryDataRow<IXUnitTest>)new InlineObjectCompilationTest
           {
               Name = $"{fixture.Shape}; JSON deserialization={usesJsonDeserialization}",
               Shape = fixture.Shape,
               UsesJsonDeserialization = usesJsonDeserialization,
               ExpectedInlineTypes = fixture.InlineTypes
           }
    ];
    #endregion

    #region Inline Object Compilation Test Methods
    [Theory]
    [MemberData(nameof(InlineObjectCompilationTheoryData))]
    public void CompileEstablishesInlineObjectOwnership(IXUnitTest test) => test.Execute(this);
    #endregion
}
