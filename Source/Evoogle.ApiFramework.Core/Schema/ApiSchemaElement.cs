// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extension;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the abstract base class for all elements in an API schema (e.g., types, properties, relationships).
/// </summary>
/// <remarks>
///     This class provides common initialization and path building functionality for all schema elements.
///     Each schema element maintains an API path that uniquely identifies its location within the schema hierarchy.
/// </remarks>
public abstract class ApiSchemaElement : ExtensibleBase
{
    #region Fields
    private string? _apiPath = null;

    private ApiSchemaContext? _apiSchemaContext = null;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the API path that uniquely identifies this element within the schema hierarchy.
    /// </summary>
    /// <remarks>
    ///     This property is available after the element has been initialized.
    /// </remarks>
    public string ApiPath => this.ThrowIfNotInitialized(_apiPath);

    /// <summary>Gets runtime API element name of the API schema element.</summary>
    protected abstract string ApiElementName { get; }

    /// <summary>
    ///     Gets the runtime context for the API schema containing this element.
    /// </summary>
    /// <remarks>
    ///     This property is available after the element has been initialized.
    /// </remarks>
    protected internal ApiSchemaContext ApiSchemaContext => this.ThrowIfNotInitialized(_apiSchemaContext);

    /// <summary>
    ///     Gets the logger for this schema element.
    /// </summary>
    /// <remarks>
    ///     Returns the shared logger from the schema context, categorized under <see cref="ApiSchema"/>.
    /// </remarks>
    protected ILogger Logger => this.ApiSchemaContext.Logger;
    #endregion

    #region Methods
    /// <summary>
    ///     Builds the API path for this schema element.
    /// </summary>
    /// <param name="apiPreviousPath">The optional API path of the previous element, or <c>null</c> if this is a root element.</param>
    /// <returns>The complete API path for this element.</returns>
    protected abstract string BuildPath(string? apiPreviousPath);

    /// <summary>
    ///     Initializes this schema element with the specified context.
    /// </summary>
    /// <param name="context">The runtime initialization context needed for initializing this schema element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    internal virtual void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _apiPath = this.BuildPath(context.ApiDeclaringPath);
        _apiSchemaContext = context.ApiSchema.ApiSchemaContext;
    }
    #endregion
}
