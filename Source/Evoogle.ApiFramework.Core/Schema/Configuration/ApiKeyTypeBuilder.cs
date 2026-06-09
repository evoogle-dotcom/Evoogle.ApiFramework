// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure a single <see cref="ApiKeyType"/>.
/// </summary>
public class ApiKeyTypeBuilder : ExtensionBuilder<ApiKeyTypeBuilder>
{
    #region Fields
    private readonly List<ApiKeyPathBuilder> _keyPathBuilders = [];
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes an <see cref="ApiKeyTypeBuilder"/>.
    /// </summary>
    public ApiKeyTypeBuilder()
    {
    }
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiKeyTypeBuilder AddKeyTypeExtension(Type type, object value)
    {
        base.AddExtension(type, value);
        return this;
    }

    /// <summary>
    ///     Adds an extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="T">The extension value type.</typeparam>
    /// <param name="value">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public ApiKeyTypeBuilder AddKeyTypeExtension<T>(T value) where T : notnull
        => this.AddKeyTypeExtension(typeof(T), value);
    #endregion

    #region AddPath Methods
    /// <summary>
    ///     Adds a key path to this key type using plain CLR property names.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="clrPropertyNames">Ordered CLR property names from the root type to the terminal scalar property.</param>
    /// <returns>The current builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrPropertyNames"/> is empty.</exception>
    public ApiKeyTypeBuilder AddPath(Type clrRootType, params string[] clrPropertyNames)
    {
        _keyPathBuilders.Add(ApiKeyPathBuilder.For(clrRootType, clrPropertyNames));
        return this;
    }

    /// <summary>
    ///     Adds a key path to this key type using plain CLR property names, with an optional configuration callback.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="clrPropertyNames">Ordered CLR property names from the root type to the terminal scalar property.</param>
    /// <param name="configure">Optional callback to attach extensions or additional segments to the path builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrPropertyNames"/> is empty.</exception>
    public ApiKeyTypeBuilder AddPath(Type clrRootType, IEnumerable<string> clrPropertyNames, Action<ApiKeyPathBuilder>? configure = null)
    {
        var builder = new ApiKeyPathBuilder(clrRootType, clrPropertyNames);
        configure?.Invoke(builder);
        _keyPathBuilders.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a key path to this key type using a pre-configured <see cref="ApiKeyPathBuilder"/>.
    ///     Use this overload when the path requires segment-level extensions.
    /// </summary>
    /// <param name="keyPathBuilder">The pre-configured key path builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="keyPathBuilder"/> is <c>null</c>.</exception>
    public ApiKeyTypeBuilder AddPath(ApiKeyPathBuilder keyPathBuilder)
    {
        ArgumentNullException.ThrowIfNull(keyPathBuilder);
        _keyPathBuilders.Add(keyPathBuilder);
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiKeyType"/> configured by this builder.
    /// </summary>
    internal ApiKeyType Build()
    {
        var keyPaths = _keyPathBuilders.Select(b => b.Build());
        var keyType = new ApiKeyType(keyPaths);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            keyType.Extensions = extensions;
        }

        return keyType;
    }
    #endregion

    #region Implementation Methods
    /// <summary>
    ///     Allows subclasses to add a pre-constructed key path builder without bypassing internal list management.
    /// </summary>
    protected void AddKeyPathBuilderCore(ApiKeyPathBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _keyPathBuilders.Add(builder);
    }
    #endregion
}
