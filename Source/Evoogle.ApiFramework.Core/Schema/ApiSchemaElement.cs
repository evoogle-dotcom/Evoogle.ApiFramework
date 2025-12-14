// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extension;

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

    /// <summary>
    ///     Gets the runtime context for the API schema containing this element.
    /// </summary>
    /// <remarks>
    ///     This property is available after the element has been initialized.
    /// </remarks>
    protected ApiSchemaContext ApiSchemaContext => this.ThrowIfNotInitialized(_apiSchemaContext);
    #endregion

    #region Methods
    /// <summary>
    ///     Builds the API path for this schema element based on the parent path.
    /// </summary>
    /// <param name="apiParentPath">The API path of the parent element, or <c>null</c> if this is a root element.</param>
    /// <returns>The complete API path for this element.</returns>
    protected abstract string BuildPath(string? apiParentPath);

    /// <summary>
    ///     Initializes this schema element with the specified context.
    /// </summary>
    /// <param name="context">The initialization context containing the schema and parent path information.</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    internal virtual void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _apiPath = this.BuildPath(context.ApiParentPath);
        _apiSchemaContext = context.ApiSchema.ApiSchemaContext;
    }
    #endregion
}
