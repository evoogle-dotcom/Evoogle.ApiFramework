// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Provides a base class for builders that collect extension values keyed by type.
///     Extensions allow attaching arbitrary metadata to schema elements during configuration.
/// </summary>
/// <typeparam name="TBuilder">The concrete builder type.</typeparam>
public abstract class ExtensionBuilder<TBuilder>
    where TBuilder : ExtensionBuilder<TBuilder>
{
    #region Fields
    private readonly OrderedDictionary<Type, object> _extensions = [];
    #endregion

    #region Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    ///     Called by the concrete builder's named extension method (e.g. <c>AddSchemaExtension</c>).
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current <typeparamref name="TBuilder"/> instance.</returns>
    protected TBuilder AddExtension(Type type, object extension)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(extension);

        if (type.IsValueType)
        {
            throw new ArgumentException("Extension metadata keys must be reference types.", nameof(type));
        }

        if (extension.GetType().IsValueType)
        {
            throw new ArgumentException("Extension metadata values must be reference types.", nameof(extension));
        }

        _extensions[type] = extension;
        return (TBuilder)this;
    }

    /// <summary>
    ///     Builds a new ordered dictionary containing the collected extensions, or <c>null</c> if none exist.
    /// </summary>
    internal OrderedDictionary<Type, object>? BuildExtensions() =>
        _extensions.Count > 0 ? new OrderedDictionary<Type, object>(_extensions) : null;
    #endregion
}

