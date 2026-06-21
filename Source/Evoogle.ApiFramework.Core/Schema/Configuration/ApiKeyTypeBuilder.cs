// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure a single <see cref="ApiKeyType"/>.
/// </summary>
/// <param name="apiName">The optional API name of the key type.</param>
/// <remarks>
///    <para>Key types are reusable components that define how to extract key values from CLR objects via one or more key paths. They are primarily used to configure API keys, but can also be used for other purposes such as defining unique identifiers for object types.</para>
///    <para>Each key path represents a navigation chain from a specified CLR root type to a terminal scalar property, and can be configured with extensions at both the path and segment levels. When multiple key paths are defined within a key type, the resulting key value is a composite of the individual path values.</para>
/// </remarks>
public class ApiKeyTypeBuilder(string? apiName = null) : ExtensionBuilder<ApiKeyTypeBuilder>
{
    #region Fields
    private string? _apiName = apiName;
    private readonly List<ApiKeyPathBuilder> _keyPathBuilders = [];
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiKeyTypeBuilder AddKeyTypeExtension(Type type, object extension)
    {
        return this.AddExtension(type, extension);
    }
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
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(clrPropertyNames);

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
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(clrPropertyNames);

        var builder = new ApiKeyPathBuilder(clrRootType, clrPropertyNames);
        configure?.Invoke(builder);
        _keyPathBuilders.Add(builder);
        return this;
    }
    #endregion

    #region With Methods
    /// <summary>
    ///    Sets the API name for the key type being built.
    /// </summary>
    /// <param name="apiName">The API name to use.</param>
    /// <returns>The current builder instance.</returns>
    public ApiKeyTypeBuilder WithName(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        _apiName = apiName;
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiKeyType"/> configured by this builder.
    /// </summary>
    internal ApiKeyType Build()
    {
        var apiName = _apiName;
        var keyPaths = _keyPathBuilders.Select(b => b.Build());
        var keyType = new ApiKeyType(apiName, keyPaths);

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
