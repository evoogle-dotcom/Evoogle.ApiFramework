// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.NTree;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Provides ownership-tree traversal operations for concrete <see cref="ApiSchemaElement"/>
///     types.
/// </summary>
public static class ApiSchemaElementExtensions
{
    #region Enumerator Methods
    /// <summary>Creates a breadth-first enumerator starting at the specified element.</summary>
    /// <param name="apiSchemaElement">The element at which traversal starts.</param>
    /// <returns>A breadth-first enumerator that includes the starting element.</returns>
    public static IEnumerator<ApiSchemaElement> CreateBreadthFirstEnumerator
    (
        this ApiSchemaElement apiSchemaElement
    )
    {
        return NodeExtensions.CreateBreadthFirstEnumerator(apiSchemaElement);
    }

    /// <summary>
    ///     Creates a depth-first preorder enumerator starting at the specified element.
    /// </summary>
    /// <param name="apiSchemaElement">The element at which traversal starts.</param>
    /// <returns>A depth-first preorder enumerator that includes the starting element.</returns>
    public static IEnumerator<ApiSchemaElement> CreateDepthFirstEnumerator
    (
        this ApiSchemaElement apiSchemaElement
    )
    {
        return NodeExtensions.CreateDepthFirstEnumerator(apiSchemaElement);
    }
    #endregion

    #region Traversal Methods
    /// <summary>Enumerates the immediate owned children of the specified element.</summary>
    /// <param name="apiSchemaElement">The element whose children are enumerated.</param>
    /// <returns>The direct children in left-to-right order.</returns>
    public static IEnumerable<ApiSchemaElement> Children
    (
        this ApiSchemaElement apiSchemaElement
    )
    {
        return NodeExtensions.Children(apiSchemaElement);
    }

    /// <summary>Enumerates descendants with the supplied enumerator.</summary>
    /// <param name="apiSchemaElement">The element excluded from the returned sequence.</param>
    /// <param name="enumerator">The enumerator that defines traversal order.</param>
    /// <returns>All enumerated descendants other than the starting element.</returns>
    public static IEnumerable<ApiSchemaElement> Descendants
    (
        this ApiSchemaElement apiSchemaElement,
        IEnumerator<ApiSchemaElement> enumerator
    )
    {
        return NodeExtensions.Descendants(apiSchemaElement, enumerator);
    }

    /// <summary>Enumerates descendants using the specified traversal strategy.</summary>
    /// <param name="apiSchemaElement">The element whose descendants are enumerated.</param>
    /// <param name="strategy">The traversal strategy.</param>
    /// <returns>All descendants, excluding the starting element.</returns>
    public static IEnumerable<ApiSchemaElement> Descendants
    (
        this ApiSchemaElement apiSchemaElement,
        TraversalStrategy strategy = TraversalStrategy.BreadthFirst
    )
    {
        return NodeExtensions.Descendants(apiSchemaElement, strategy);
    }

    /// <summary>Enumerates the ownership path from the root to the specified element.</summary>
    /// <param name="apiSchemaElement">The path's terminal element.</param>
    /// <returns>The root-to-element path, including both endpoints.</returns>
    public static IEnumerable<ApiSchemaElement> GetPathFromRoot
    (
        this ApiSchemaElement apiSchemaElement
    )
    {
        return NodeExtensions.GetPathFromRoot(apiSchemaElement);
    }

    /// <summary>Enumerates the ownership path from the specified element to the root.</summary>
    /// <param name="apiSchemaElement">The path's initial element.</param>
    /// <returns>The element-to-root path, including both endpoints.</returns>
    public static IEnumerable<ApiSchemaElement> GetPathToRoot
    (
        this ApiSchemaElement apiSchemaElement
    )
    {
        return NodeExtensions.GetPathToRoot(apiSchemaElement);
    }

    /// <summary>Determines whether an element is a descendant of another element.</summary>
    /// <param name="apiSchemaElement">The potential descendant.</param>
    /// <param name="potentialAncestor">The potential ancestor.</param>
    /// <returns><see langword="true"/> when the ancestor appears in the parent chain.</returns>
    public static bool IsDescendantOf
    (
        this ApiSchemaElement apiSchemaElement,
        ApiSchemaElement potentialAncestor
    )
    {
        return NodeExtensions.IsDescendantOf(apiSchemaElement, potentialAncestor);
    }

    /// <summary>Enumerates an element and its descendants with the supplied enumerator.</summary>
    /// <param name="apiSchemaElement">The element included at the start of traversal.</param>
    /// <param name="enumerator">The enumerator that defines traversal order.</param>
    /// <returns>The enumerated elements, including the starting element.</returns>
    public static IEnumerable<ApiSchemaElement> SelfAndDescendants
    (
        this ApiSchemaElement apiSchemaElement,
        IEnumerator<ApiSchemaElement> enumerator
    )
    {
        return NodeExtensions.SelfAndDescendants(apiSchemaElement, enumerator);
    }

    /// <summary>Enumerates an element and its descendants using a traversal strategy.</summary>
    /// <param name="apiSchemaElement">The element included at the start of traversal.</param>
    /// <param name="strategy">The traversal strategy.</param>
    /// <returns>The traversed elements, including the starting element.</returns>
    public static IEnumerable<ApiSchemaElement> SelfAndDescendants
    (
        this ApiSchemaElement apiSchemaElement,
        TraversalStrategy strategy = TraversalStrategy.BreadthFirst
    )
    {
        return NodeExtensions.SelfAndDescendants(apiSchemaElement, strategy);
    }

    /// <summary>Traverses elements with an enumerator and delegate visitor.</summary>
    /// <param name="apiSchemaElement">The traversal's starting element.</param>
    /// <param name="enumerator">The enumerator that defines traversal order.</param>
    /// <param name="visitorFunction">Returns whether traversal should continue.</param>
    public static void Traverse
    (
        this ApiSchemaElement apiSchemaElement,
        IEnumerator<ApiSchemaElement> enumerator,
        Func<ApiSchemaElement, bool> visitorFunction
    )
    {
        NodeExtensions.Traverse(apiSchemaElement, enumerator, visitorFunction);
    }

    /// <summary>Traverses elements with a strategy and delegate visitor.</summary>
    /// <param name="apiSchemaElement">The traversal's starting element.</param>
    /// <param name="strategy">The traversal strategy.</param>
    /// <param name="visitorFunction">Returns whether traversal should continue.</param>
    public static void Traverse
    (
        this ApiSchemaElement apiSchemaElement,
        TraversalStrategy strategy,
        Func<ApiSchemaElement, bool> visitorFunction
    )
    {
        NodeExtensions.Traverse(apiSchemaElement, strategy, visitorFunction);
    }

    /// <summary>Traverses elements with an enumerator and visitor object.</summary>
    /// <param name="apiSchemaElement">The traversal's starting element.</param>
    /// <param name="enumerator">The enumerator that defines traversal order.</param>
    /// <param name="visitor">The visitor that controls continuation.</param>
    public static void Traverse
    (
        this ApiSchemaElement apiSchemaElement,
        IEnumerator<ApiSchemaElement> enumerator,
        INodeVisitor<ApiSchemaElement> visitor
    )
    {
        NodeExtensions.Traverse(apiSchemaElement, enumerator, visitor);
    }

    /// <summary>Traverses elements with a strategy and visitor object.</summary>
    /// <param name="apiSchemaElement">The traversal's starting element.</param>
    /// <param name="strategy">The traversal strategy.</param>
    /// <param name="visitor">The visitor that controls continuation.</param>
    public static void Traverse
    (
        this ApiSchemaElement apiSchemaElement,
        TraversalStrategy strategy,
        INodeVisitor<ApiSchemaElement> visitor
    )
    {
        NodeExtensions.Traverse(apiSchemaElement, strategy, visitor);
    }
    #endregion
}
