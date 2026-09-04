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
///     After topology construction, it also participates in an exclusive, read-only ownership tree
///     whose root is the containing <see cref="ApiSchema"/>. An element instance cannot be owned by
///     multiple schemas. Reference relationships are not ownership links. Use
///     <see cref="ApiSchemaElementExtensions"/> to traverse concrete schema-element instances.
///     Elements in a successfully returned schema are frozen and safe for concurrent reads.
///     Standalone draft elements are construction-time objects and carry no runtime safety
///     guarantee until their owning schema compiles successfully.
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

    private bool _isFrozen;
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

    /// <summary>Gets the cast-safe runtime kind of this schema element.</summary>
    /// <remarks>
    ///     Specialized type, relationship, and relationship-end kind properties remain the
    ///     authoritative domain discriminators. Custom <see cref="ApiKeyType"/> subclasses report
    ///     <see cref="ApiSchemaElementKind.KeyType"/>.
    /// </remarks>
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

    internal bool IsFrozen => _isFrozen;

    /// <summary>Gets a value indicating whether this element's ownership topology has been established.</summary>
    internal bool HasTopology => _topology is not null;

    private ApiSchemaElementTopology Topology => this.ThrowIfNotInitialized(_topology);
    #endregion

    #region Constructors
    internal ApiSchemaElement()
    { }
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
        this.ThrowIfFrozen();

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

    internal void AttachExtensions(IEnumerable<KeyValuePair<Type, object>> extensions)
    {
        ArgumentNullException.ThrowIfNull(extensions);
        this.ThrowIfFrozen();

        foreach (var (extensionType, extension) in extensions)
        {
            this.AttachExtension(extensionType, extension);
        }
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
        this.ThrowIfFrozen();

        if (_topology is not null)
        {
            throw new InvalidOperationException("Schema element topology can only be established once.");
        }

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

    internal bool TryGetTopology
    (
        out ApiSchemaElement? root,
        out ApiSchemaElement? firstChild
    )
    {
        root = _topology?.Root;
        firstChild = _topology?.FirstChild;
        return _topology is not null;
    }

    internal bool TryCreateFrozenExtensionSnapshot
    (
        ApiInitializationSession session,
        out IReadOnlyList<KeyValuePair<Type, object>> frozenExtensions
    )
    {
        ArgumentNullException.ThrowIfNull(session);
        this.ThrowIfFrozen();

        var snapshots = new List<KeyValuePair<Type, object>>(this.Extensions.Count);
        var isValid = true;
        var issuePath = _apiPath ?? session.ApiSchema.ApiPath;

        foreach (var (extensionType, extension) in this.Extensions)
        {
            if (!extensionType.IsInstanceOfType(extension))
            {
                isValid = false;
                session.AddIssue
                (
                    issuePath,
                    ApiInitializationSeverity.Error,
                    ApiInitializationCode.ApiSchemaExtensionInvalidSnapshot,
                    $"Extension value '{extension.GetType().FullName}' is not assignable to its registered key type '{extensionType.FullName}'.",
                    "Register the extension under a type assignable from the extension value and its frozen snapshot."
                );
                continue;
            }

            if (extension is not IApiSchemaExtension schemaExtension)
            {
                isValid = false;
                session.AddIssue
                (
                    issuePath,
                    ApiInitializationSeverity.Error,
                    ApiInitializationCode.ApiSchemaExtensionUnsupported,
                    $"Extension '{extensionType.FullName}' does not implement {nameof(IApiSchemaExtension)}.",
                    $"Implement {nameof(IApiSchemaExtension)} and return an immutable runtime snapshot."
                );
                continue;
            }

            IApiSchemaExtension snapshot;
            try
            {
                snapshot = schemaExtension.CreateFrozenSnapshot();
            }
            catch (Exception exception)
            {
                isValid = false;
                session.AddIssue
                (
                    issuePath,
                    ApiInitializationSeverity.Error,
                    ApiInitializationCode.ApiSchemaExtensionSnapshotFailed,
                    $"Extension '{extensionType.FullName}' failed to create a frozen snapshot: {exception.Message}",
                    "Correct the extension snapshot implementation so it completes successfully."
                );
                continue;
            }

            if (snapshot is null ||
                ReferenceEquals(snapshot, extension) ||
                !extensionType.IsInstanceOfType(snapshot))
            {
                isValid = false;
                session.AddIssue
                (
                    issuePath,
                    ApiInitializationSeverity.Error,
                    ApiInitializationCode.ApiSchemaExtensionInvalidSnapshot,
                    $"Extension '{extensionType.FullName}' returned a null, reused, or incompatible frozen snapshot.",
                    "Return a distinct immutable snapshot assignable to the registered extension key type."
                );
                continue;
            }

            snapshots.Add(new KeyValuePair<Type, object>(extensionType, snapshot));
        }

        frozenExtensions = snapshots;
        return isValid;
    }

    internal void Freeze(IReadOnlyList<KeyValuePair<Type, object>> frozenExtensions)
    {
        ArgumentNullException.ThrowIfNull(frozenExtensions);
        this.ThrowIfFrozen();
        this.FreezeExtensions(frozenExtensions);
        _isFrozen = true;
    }

    internal void ThrowIfFrozen()
    {
        if (_isFrozen)
        {
            throw new InvalidOperationException("A frozen API schema element cannot be modified.");
        }
    }
    #endregion
}
