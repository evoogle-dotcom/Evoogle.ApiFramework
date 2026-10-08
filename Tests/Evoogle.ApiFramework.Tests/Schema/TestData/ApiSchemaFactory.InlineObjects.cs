// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace Evoogle.ApiFramework.Schema.TestData;

public static partial class ApiSchemaFactory
{
    #region Inline Object Test Types
    internal enum InlineObjectShape
    {
        Direct,
        Collection,
        NestedCollections
    }

    internal sealed class InlineLeaf
    {
        public int Id { get; set; }
    }

    internal sealed class DirectInlineRoot
    {
        public InlineLeaf? Child { get; set; }
    }

    internal sealed class CollectionInlineRoot
    {
        public List<InlineLeaf>? Children { get; set; }
    }

    internal sealed class NestedInlineRoot
    {
        public InlineContainer? Child { get; set; }
    }

    internal sealed class InlineContainer
    {
        public List<List<InlineLeaf>>? Children { get; set; }
    }

    #endregion

    #region Inline Object Factory Methods
    internal static ApiSchemaDef CreateInlineObjectSchemaDefinition(InlineObjectShape shape)
    {
        var leaf = new ApiObjectTypeDef
        (
            ApiName: nameof(InlineLeaf),
            ClrType: typeof(InlineLeaf),
            ApiProperties:
            [
                new ApiPropertyDef
                (
                    ApiName: nameof(InlineLeaf.Id),
                    ApiTypeExpression: new ApiTypeExpressionDef
                    (
                        ApiTypeDef: null,
                        ApiTypeReferenceDef: new ApiTypeReferenceDef(ApiTypeKind.Scalar, "Int32", null)
                    ),
                    ApiTypeModifiers: ApiTypeModifiers.Required,
                    ClrValueMember: new ClrMemberReferenceDef(ClrMemberKind.Property, nameof(InlineLeaf.Id))
                )
            ]
        );
        var leafExpression = new ApiTypeExpressionDef(leaf, null);
        var collection = new ApiCollectionTypeDef
        (
            typeof(List<InlineLeaf>),
            leafExpression,
            ApiTypeModifiers.Required
        );
        var container = new ApiObjectTypeDef
        (
            ApiName: nameof(InlineContainer),
            ClrType: typeof(InlineContainer),
            ApiProperties:
            [
                CreateInlineObjectProperty
                (
                    nameof(InlineContainer.Children),
                    new ApiTypeExpressionDef
                    (
                        new ApiCollectionTypeDef
                        (
                            typeof(List<List<InlineLeaf>>),
                            new ApiTypeExpressionDef(collection, null),
                            ApiTypeModifiers.Required
                        ),
                        null
                    )
                )
            ]
        );
        var root = shape switch
        {
            InlineObjectShape.Direct => new ApiObjectTypeDef
            (
                nameof(DirectInlineRoot),
                typeof(DirectInlineRoot),
                ApiProperties: [CreateInlineObjectProperty(nameof(DirectInlineRoot.Child), leafExpression)]
            ),
            InlineObjectShape.Collection => new ApiObjectTypeDef
            (
                nameof(CollectionInlineRoot),
                typeof(CollectionInlineRoot),
                ApiProperties:
                [
                    CreateInlineObjectProperty
                    (
                        nameof(CollectionInlineRoot.Children),
                        new ApiTypeExpressionDef(collection, null)
                    )
                ]
            ),
            InlineObjectShape.NestedCollections => new ApiObjectTypeDef
            (
                nameof(NestedInlineRoot),
                typeof(NestedInlineRoot),
                ApiProperties:
                [
                    CreateInlineObjectProperty
                    (
                        nameof(NestedInlineRoot.Child),
                        new ApiTypeExpressionDef(container, null)
                    )
                ]
            ),

            _ => throw new InvalidOperationException("Unknown inline object shape.")
        };

        return new ApiSchemaDef
        (
            ApiName: $"Inline{shape}",
            ApiNamedTypes: [new ApiScalarTypeDef("Int32", typeof(int)), root]
        );
    }

    private static ApiPropertyDef CreateInlineObjectProperty(string clrName, ApiTypeExpressionDef typeExpression) => new
    (
        ApiName: clrName,
        ApiTypeExpression: typeExpression,
        ApiTypeModifiers: ApiTypeModifiers.None,
        ClrValueMember: new ClrMemberReferenceDef(ClrMemberKind.Property, clrName)
    );
    #endregion
}
