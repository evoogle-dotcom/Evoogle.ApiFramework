// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Key.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Types;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Key;

/// <summary>
///     Fluent builder used to configure the structural paths shared by <see cref="ApiKeyDefinition"/> and
///     <see cref="ApiNamedKeyDefinition"/>.
/// </summary>
/// <param name="apiName">
///     The optional API name used when the builder produces an <see cref="ApiNamedKeyDefinition"/>.
/// </param>
/// <remarks>
///    <para>Key definitions are reusable components that define how to extract key values from CLR objects via one or more key paths. They are primarily used to configure API keys, but can also be used for other purposes such as defining unique identifiers for object types.</para>
///    <para>
///        Each key path represents a navigation chain from an explicit or inferred API object root
///        to a terminal scalar member. Paths and segments can carry extensions. Multiple paths form
///        a composite key value.
///    </para>
/// </remarks>
public class ApiKeyDefinitionBuilder(string? apiName = null) : ExtensionBuilder<ApiKeyDefinitionBuilder>
{
    #region Fields
    private readonly ApiKeyDefinitionState _state = new() { ApiName = apiName };
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiKeyDefinitionBuilder AddKeyExtension(Type extensionType, object extension) => this.AddExtension(extensionType, extension);
    #endregion

    #region AddPath Methods
    /// <summary>
    ///     Adds a key path to this key definition using CLR member names or dot-delimited CLR member paths.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="clrMemberNames">
    ///     Ordered CLR member names or dot-delimited CLR member paths from the root type to the terminal scalar member.
    /// </param>
    /// <returns>The current builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrMemberNames"/> is empty.</exception>
    public ApiKeyDefinitionBuilder AddPath(Type clrRootType, params string[] clrMemberNames)
    {
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(clrMemberNames);

        return this.AddPath(new ApiTypeReference(clrRootType), clrMemberNames);
    }

    /// <summary>Adds a key path with an explicit API root object-type reference.</summary>
    /// <param name="apiRootObjectTypeReference">The explicit root object-type reference.</param>
    /// <param name="clrMemberNames">The CLR member paths.</param>
    /// <returns>The current builder.</returns>
    public ApiKeyDefinitionBuilder AddPath
    (
        ApiTypeReference apiRootObjectTypeReference,
        params string[] clrMemberNames
    )
    {
        ArgumentNullException.ThrowIfNull(apiRootObjectTypeReference);
        ArgumentNullException.ThrowIfNull(clrMemberNames);

        _state.KeyPathBuilders.Add
            (ApiKeyPathBuilder.For(apiRootObjectTypeReference, clrMemberNames));
        return this;
    }

    /// <summary>Adds a key path whose root is inferred from its enclosing schema context.</summary>
    /// <param name="clrMemberNames">The CLR member paths.</param>
    /// <returns>The current builder.</returns>
    public ApiKeyDefinitionBuilder AddPath(params string[] clrMemberNames)
    {
        ArgumentNullException.ThrowIfNull(clrMemberNames);
        _state.KeyPathBuilders.Add(ApiKeyPathBuilder.For(clrMemberNames));
        return this;
    }

    /// <summary>
    ///     Adds a key path to this key definition using CLR member names or dot-delimited CLR member paths,
    ///     with an optional configuration callback.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="clrMemberNames">
    ///     Ordered CLR member names or dot-delimited CLR member paths from the root type to the terminal scalar member.
    /// </param>
    /// <param name="configure">Optional callback to attach extensions or additional segments to the path builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrMemberNames"/> is empty.</exception>
    public ApiKeyDefinitionBuilder AddPath(Type clrRootType, IEnumerable<string> clrMemberNames, Action<ApiKeyPathBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(clrMemberNames);

        return this.AddPath(new ApiTypeReference(clrRootType), clrMemberNames, configure);
    }

    /// <summary>
    ///     Adds a configurable key path with an explicit API root object-type reference.
    /// </summary>
    /// <param name="apiRootObjectTypeReference">The explicit root object-type reference.</param>
    /// <param name="clrMemberNames">The CLR member paths.</param>
    /// <param name="configure">An optional path-builder callback.</param>
    /// <returns>The current builder.</returns>
    public ApiKeyDefinitionBuilder AddPath
    (
        ApiTypeReference apiRootObjectTypeReference,
        IEnumerable<string> clrMemberNames,
        Action<ApiKeyPathBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(apiRootObjectTypeReference);
        ArgumentNullException.ThrowIfNull(clrMemberNames);

        var builder = new ApiKeyPathBuilder(apiRootObjectTypeReference, clrMemberNames);
        configure?.Invoke(builder);
        _state.KeyPathBuilders.Add(builder);
        return this;
    }

    /// <summary>
    ///     Adds a configurable key path whose root is inferred from its enclosing schema context.
    /// </summary>
    /// <param name="clrMemberNames">The CLR member paths.</param>
    /// <param name="configure">An optional path-builder callback.</param>
    /// <returns>The current builder.</returns>
    public ApiKeyDefinitionBuilder AddPath
    (
        IEnumerable<string> clrMemberNames,
        Action<ApiKeyPathBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(clrMemberNames);

        var builder = new ApiKeyPathBuilder(clrMemberNames);
        configure?.Invoke(builder);
        _state.KeyPathBuilders.Add(builder);
        return this;
    }

    /// <summary>Adds a key path with an explicit root and property references.</summary>
    /// <param name="apiRootObjectTypeReference">The explicit root object-type reference.</param>
    /// <param name="apiPropertyReferences">The ordered property references.</param>
    /// <param name="configure">An optional path-builder callback.</param>
    /// <returns>The current builder.</returns>
    public ApiKeyDefinitionBuilder AddPath
    (
        ApiTypeReference apiRootObjectTypeReference,
        IEnumerable<ApiPropertyReference> apiPropertyReferences,
        Action<ApiKeyPathBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(apiRootObjectTypeReference);
        ArgumentNullException.ThrowIfNull(apiPropertyReferences);

        var segmentBuilders = apiPropertyReferences.Select(static reference =>
            new ApiKeyPathSegmentBuilder(reference));
        var builder = new ApiKeyPathBuilder(apiRootObjectTypeReference, segmentBuilders);
        configure?.Invoke(builder);
        _state.KeyPathBuilders.Add(builder);
        return this;
    }

    /// <summary>Adds an inferred-root key path with property references.</summary>
    /// <param name="apiPropertyReferences">The ordered property references.</param>
    /// <param name="configure">An optional path-builder callback.</param>
    /// <returns>The current builder.</returns>
    public ApiKeyDefinitionBuilder AddPath
    (
        IEnumerable<ApiPropertyReference> apiPropertyReferences,
        Action<ApiKeyPathBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(apiPropertyReferences);

        var segmentBuilders = apiPropertyReferences.Select(static reference =>
            new ApiKeyPathSegmentBuilder(reference));
        var builder = new ApiKeyPathBuilder(segmentBuilders);
        configure?.Invoke(builder);
        _state.KeyPathBuilders.Add(builder);
        return this;
    }
    #endregion

    #region With Methods
    /// <summary>Gets the API name currently configured on this key definition builder.</summary>
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
    ///     Returns <c>true</c> when this key definition already contains the specified CLR root type
    ///     and ordered CLR member path.
    ///
    ///     Used by <see cref="ApiObjectTypeBuilder.AddKeyOrAppendPath"/> to prevent
    ///     convention and annotation passes from adding the same path twice.
    /// </summary>
    internal bool HasPath(Type clrRootType, IEnumerable<string> clrMemberNames)
    {
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(clrMemberNames);

        return this.HasPath(new ApiTypeReference(clrRootType), clrMemberNames);
    }

    internal bool HasPath
    (
        ApiTypeReference? apiRootObjectTypeReference,
        IEnumerable<string> clrMemberNames
    )
    {
        ArgumentNullException.ThrowIfNull(clrMemberNames);

        var names = clrMemberNames as IReadOnlyList<string> ?? [.. clrMemberNames];

        return _state.KeyPathBuilders.Any(p =>
            p.ApiRootObjectTypeReference == apiRootObjectTypeReference &&
            p.SegmentBuilders.All(s => s.ApiPropertyReference.IsClrNamedReference) &&
            p.SegmentBuilders.Select(s => s.ApiPropertyReference.ClrName!)
                .SequenceEqual(names, ClrNameComparer.Instance));
    }

    /// <summary>
    ///    Sets the API name used when this builder produces an <see cref="ApiNamedKeyDefinition"/>.
    /// </summary>
    /// <param name="apiName">The API name to use.</param>
    /// <returns>The current builder instance.</returns>
    public ApiKeyDefinitionBuilder WithName(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        _state.ApiName = apiName;
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiKeyDefinition"/> configured by this builder.
    /// </summary>
    internal ApiKeyDefinition Build()
    {
        var keyPaths = _state.KeyPathBuilders.Select(b => b.Build());
        var keyDefinition = new ApiKeyDefinition(keyPaths);

        this.AttachExtensions(keyDefinition);

        return keyDefinition;
    }

    /// <summary>
    ///     Builds the <see cref="ApiNamedKeyDefinition"/> configured by this builder.
    /// </summary>
    internal ApiNamedKeyDefinition BuildNamed()
    {
        var apiName = _state.ApiName!;
        var keyPaths = _state.KeyPathBuilders.Select(b => b.Build());
        var keyDefinition = new ApiNamedKeyDefinition(apiName, keyPaths);

        this.AttachExtensions(keyDefinition);

        return keyDefinition;
    }
    #endregion

    #region Implementation Methods
    private void AttachExtensions(ApiKeyDefinition keyDefinition)
    {
        ArgumentNullException.ThrowIfNull(keyDefinition);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            keyDefinition.AttachExtensions(extensions);
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
