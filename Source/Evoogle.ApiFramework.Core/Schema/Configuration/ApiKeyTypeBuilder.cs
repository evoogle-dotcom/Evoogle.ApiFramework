// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure the structural paths shared by <see cref="ApiKeyType"/> and
///     <see cref="ApiNamedKeyType"/>.
/// </summary>
/// <param name="apiName">
///     The optional API name used when the builder produces an <see cref="ApiNamedKeyType"/>.
/// </param>
/// <remarks>
///    <para>Key types are reusable components that define how to extract key values from CLR objects via one or more key paths. They are primarily used to configure API keys, but can also be used for other purposes such as defining unique identifiers for object types.</para>
///    <para>Each key path represents a navigation chain from a specified CLR root type to a terminal scalar property, and can be configured with extensions at both the path and segment levels. When multiple key paths are defined within a key type, the resulting key value is a composite of the individual path values.</para>
/// </remarks>
public class ApiKeyTypeBuilder(string? apiName = null) : ExtensionBuilder<ApiKeyTypeBuilder>
{
    #region Fields
    private readonly ApiKeyTypeState _state = new() { ApiName = apiName };
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiKeyTypeBuilder AddKeyTypeExtension(Type extensionType, object extension)
    {
        return this.AddExtension(extensionType, extension);
    }
    #endregion

    #region AddPath Methods
    /// <summary>
    ///     Adds a key path to this key type using CLR property names or dot-delimited CLR property paths.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="clrPropertyNames">
    ///     Ordered CLR property names or dot-delimited CLR property paths from the root type to the terminal scalar property.
    /// </param>
    /// <returns>The current builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrPropertyNames"/> is empty.</exception>
    public ApiKeyTypeBuilder AddPath(Type clrRootType, params string[] clrPropertyNames)
    {
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(clrPropertyNames);

        _state.KeyPathBuilders.Add(ApiKeyPathBuilder.For(clrRootType, clrPropertyNames));
        return this;
    }

    /// <summary>
    ///     Adds a key path to this key type using CLR property names or dot-delimited CLR property paths,
    ///     with an optional configuration callback.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="clrPropertyNames">
    ///     Ordered CLR property names or dot-delimited CLR property paths from the root type to the terminal scalar property.
    /// </param>
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
        _state.KeyPathBuilders.Add(builder);
        return this;
    }
    #endregion

    #region With Methods
    /// <summary>Gets the API name currently configured on this key type builder.</summary>
    internal string? ApiName => _state.ApiName;

    internal ApiConfigurationSource RegistrationSource => _state.RegistrationSource;

    internal void SetRegistrationSource(ApiConfigurationSource source)
    {
        if (source > _state.RegistrationSource)
        {
            _state.RegistrationSource = source;
        }
    }

    internal void ClearPaths() => _state.KeyPathBuilders.Clear();

    /// <summary>
    ///     Returns <c>true</c> when this key type already contains the specified CLR root type
    ///     and ordered CLR property path.
    ///
    ///     Used by <see cref="ApiObjectTypeBuilder.AddKeyOrAppendPath"/> to prevent
    ///     convention and annotation passes from adding the same path twice.
    /// </summary>
    internal bool HasPath(Type clrRootType, IEnumerable<string> clrPropertyNames)
    {
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(clrPropertyNames);

        var names = clrPropertyNames as IReadOnlyList<string> ?? [.. clrPropertyNames];

        return _state.KeyPathBuilders.Any(p =>
            p.ClrRootType == clrRootType &&
            p.SegmentBuilders.Select(s => s.ClrPropertyName).SequenceEqual(names));
    }

    /// <summary>
    ///    Sets the API name used when this builder produces an <see cref="ApiNamedKeyType"/>.
    /// </summary>
    /// <param name="apiName">The API name to use.</param>
    /// <returns>The current builder instance.</returns>
    public ApiKeyTypeBuilder WithName(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        _state.ApiName = apiName;
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiKeyType"/> configured by this builder.
    /// </summary>
    internal ApiKeyType Build()
    {
        var keyPaths = _state.KeyPathBuilders.Select(b => b.Build());
        var keyType = new ApiKeyType(keyPaths);

        this.AttachExtensions(keyType);

        return keyType;
    }

    /// <summary>
    ///     Builds the <see cref="ApiNamedKeyType"/> configured by this builder.
    /// </summary>
    internal ApiNamedKeyType BuildNamed()
    {
        var apiName = _state.ApiName!;
        var keyPaths = _state.KeyPathBuilders.Select(b => b.Build());
        var keyType = new ApiNamedKeyType(apiName, keyPaths);

        this.AttachExtensions(keyType);

        return keyType;
    }
    #endregion

    #region Implementation Methods
    private void AttachExtensions(ApiKeyType keyType)
    {
        ArgumentNullException.ThrowIfNull(keyType);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            keyType.AttachExtensions(extensions);
        }
    }

    /// <summary>
    ///     Allows subclasses to add a pre-constructed key path builder without bypassing internal list management.
    /// </summary>
    protected void AddKeyPathBuilderCore(ApiKeyPathBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _state.KeyPathBuilders.Add(builder);
    }
    #endregion
}
