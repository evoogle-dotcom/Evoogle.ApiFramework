// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extension;
using Evoogle.NTree;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the abstract base class for all elements in an API schema (e.g., types, properties, relationships).
/// </summary>
/// <remarks>
///     This class provides common initialization and path building functionality for all schema elements.
///     Each schema element maintains an API path that uniquely identifies its location within the schema hierarchy.
///     After topology construction, it also participates in a read-only ownership tree whose root is the containing <see cref="ApiSchema"/>.
///     Reference relationships are not ownership links.
/// </remarks>
public abstract class ApiSchemaElement : ExtensibleBase, INode<ApiSchemaElement>
{
    #region Types
    private sealed class ApiSchemaElementTopology
    {
        public required ApiSchemaElement? FirstChild { get; init; }

        public required ApiSchemaElement? LastChild { get; init; }

        public required ApiSchemaElement? NextSibling { get; init; }

        public required ApiSchemaElement? Parent { get; init; }

        public required ApiSchemaElement? PreviousSibling { get; init; }

        public required ApiSchemaElement Root { get; init; }
    }
    #endregion

    #region Fields
    private string? _apiPath = null;

    private ApiSchemaContext? _apiSchemaContext = null;

    private ApiSchemaElementTopology? _topology = null;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the API path that uniquely identifies this element within the schema hierarchy.
    /// </summary>
    /// <remarks>
    ///     This property is available after the element has been initialized. Its value begins
    ///     with the containing <see cref="ApiSchema"/> and includes the element's structural
    ///     ancestry.
    /// </remarks>
    public string ApiPath => this.ThrowIfNotInitialized(_apiPath);

    /// <summary>Gets the concrete built-in kind of this schema element.</summary>
    public abstract ApiSchemaElementKind Kind { get; }

    /// <summary>Gets the root element of the schema ownership tree.</summary>
    /// <remarks>
    ///     This property is available after the schema topology has been initialized.
    /// </remarks>
    public ApiSchemaElement Root => this.Topology.Root;

    /// <summary>Gets the structural parent of this schema element.</summary>
    /// <remarks>
    ///     This property is available after the schema topology has been initialized.
    /// </remarks>
    public ApiSchemaElement? Parent => this.Topology.Parent;

    /// <summary>Gets the first structurally owned child of this schema element.</summary>
    /// <remarks>
    ///     This property is available after the schema topology has been initialized.
    /// </remarks>
    public ApiSchemaElement? FirstChild => this.Topology.FirstChild;

    /// <summary>Gets the last structurally owned child of this schema element.</summary>
    /// <remarks>
    ///     This property is available after the schema topology has been initialized.
    /// </remarks>
    public ApiSchemaElement? LastChild => this.Topology.LastChild;

    /// <summary>Gets the next structurally owned sibling of this schema element.</summary>
    /// <remarks>
    ///     This property is available after the schema topology has been initialized.
    /// </remarks>
    public ApiSchemaElement? NextSibling => this.Topology.NextSibling;

    /// <summary>Gets the previous structurally owned sibling of this schema element.</summary>
    /// <remarks>
    ///     This property is available after the schema topology has been initialized.
    /// </remarks>
    public ApiSchemaElement? PreviousSibling => this.Topology.PreviousSibling;

    /// <summary>Gets runtime API element name of the API schema element.</summary>
    protected abstract string ApiElementName { get; }

    /// <summary>
    ///     Gets the runtime context for the API schema containing this element.
    /// </summary>
    /// <remarks>
    ///     This property is available after the element has been initialized.
    /// </remarks>
    public ApiSchemaContext ApiSchemaContext => this.ThrowIfNotInitialized(_apiSchemaContext);

    /// <summary>
    ///     Gets the logger for this schema element.
    /// </summary>
    /// <remarks>
    ///     Returns the shared logger from the schema context, categorized under <see cref="ApiSchema"/>.
    /// </remarks>
    protected ILogger Logger => this.ApiSchemaContext.Logger;

    internal string ApiElementTypeName => this.ApiElementName;

    private ApiSchemaElementTopology Topology => this.ThrowIfNotInitialized(_topology);
    #endregion

    #region Methods
    /// <summary>
    ///     Builds the API path for this schema element.
    /// </summary>
    /// <param name="apiPreviousPath">
    ///     The optional API path of the previous element, or <c>null</c> if this is a root element.
    /// </param>
    /// <returns>The complete API path for this element.</returns>
    protected abstract string BuildPath(string? apiPreviousPath);

    /// <summary>
    ///     Initializes this schema element in the specified session.
    /// </summary>
    internal ApiInitializationContext Initialize
    (
        ApiInitializationSession session,
        ApiInitializationLocation location = default
    )
    {
        ArgumentNullException.ThrowIfNull(session);

        var context = session.CreateContext(this, location);

        _apiPath = context.ApiPath;
        _apiSchemaContext = session.ApiSchemaContext;

        this.InitializeCore(context);
        return context;
    }

    internal ApiInitializationContext Initialize
    (
        ApiInitializationContext context,
        ApiInitializationLocation location = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        // The supplied context contributes only shared session state. Structural parentage and
        // path construction come from this element's published ownership topology.
        return this.Initialize(context.Session, location);
    }

    internal string BuildDefaultPath(string? apiPreviousPath) => this.BuildPath(apiPreviousPath);

    internal virtual IEnumerable<ApiSchemaElement> GetOwnedElements() => [];

    internal virtual void InitializeCore(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
    }

    internal void ClearTopology()
    {
        _topology = null;
    }

    internal void SetTopology
    (
        ApiSchemaElement root,
        ApiSchemaElement? parent,
        ApiSchemaElement? firstChild,
        ApiSchemaElement? lastChild,
        ApiSchemaElement? previousSibling,
        ApiSchemaElement? nextSibling
    )
    {
        ArgumentNullException.ThrowIfNull(root);

        _topology = new ApiSchemaElementTopology
        {
            Root = root,
            Parent = parent,
            FirstChild = firstChild,
            LastChild = lastChild,
            PreviousSibling = previousSibling,
            NextSibling = nextSibling
        };
    }

    internal bool TryGetTopology(out ApiSchemaElement? firstChild)
    {
        firstChild = _topology?.FirstChild;
        return _topology is not null;
    }
    #endregion
}
