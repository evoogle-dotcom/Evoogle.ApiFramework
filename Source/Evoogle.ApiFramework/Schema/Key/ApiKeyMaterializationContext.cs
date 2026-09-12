// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Key;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Key;

/// <summary>
///     Formats the optional part name used when materializing one <see cref="ApiKeyPart"/>.
/// </summary>
/// <param name="context">The part naming context.</param>
/// <returns>
///     The part name to use, or <see langword="null"/> to create an unnamed/positional key part.
/// </returns>
public delegate string? ApiKeyPartNameFormatterDelegate(ApiKeyPartNameContext context);

/// <summary>
///     Provides metadata to an <see cref="ApiKeyPartNameFormatterDelegate"/> while materializing an <see cref="ApiKey"/>.
/// </summary>
/// <param name="ApiKeyDefinition">The key definition being materialized.</param>
/// <param name="ApiKeyPath">The key path for the current part.</param>
/// <param name="ApiKeyPartIndex">The zero-based index of the current API key part.</param>
/// <param name="ApiKeyName">
///     The effective contextual name of the key definition being materialized, or
///     <see langword="null"/> for an anonymous key definition. An explicit
///     <see cref="ApiKeyMaterializationContext.ContextualKeyName"/> takes precedence over
///     <see cref="ApiNamedKeyDefinition.ApiName"/>.
/// </param>
public readonly record struct ApiKeyPartNameContext
(
    ApiKeyDefinition ApiKeyDefinition,
    ApiKeyPath ApiKeyPath,
    int ApiKeyPartIndex,
    string? ApiKeyName
);

internal enum ApiKeyMaterializationValueKind
{
    Key,
    Text
}

internal readonly record struct ApiKeyMaterializationValue
{
    #region Constructors
    private ApiKeyMaterializationValue(ApiKeyMaterializationValueKind kind, ApiKey apiKey, string? text)
    {
        this.Kind = kind;
        this.ApiKey = apiKey;
        this.Text = text;
    }
    #endregion

    #region Properties
    public ApiKeyMaterializationValueKind Kind { get; }
    public ApiKey ApiKey { get; }
    public string? Text { get; }
    #endregion

    #region Factory Methods
    public static ApiKeyMaterializationValue FromKey(ApiKey apiKey) => new(ApiKeyMaterializationValueKind.Key, apiKey, text: null);
    public static ApiKeyMaterializationValue FromText(string? text) => new(ApiKeyMaterializationValueKind.Text, ApiKey.Empty, text);
    #endregion
}

/// <summary>
///     Provides context for materializing an <see cref="ApiKey"/> from CLR object instances
///     using <see cref="ApiKeyDefinition.MaterializeKey(ApiKeyMaterializationContext)"/>.
/// </summary>
/// <remarks>
///     This mutable context is request-scoped and is not safe for concurrent sharing. Each
///     concurrent materialization operation must use its own context. Synchronizing the CLR object
///     instances supplied to the context remains the caller's responsibility.
/// </remarks>
public sealed class ApiKeyMaterializationContext
{
    #region Fields
    private readonly Dictionary<Type, object> _roots = [];
    private readonly Dictionary<ApiKeyMaterializationValuePath, ApiKeyMaterializationValue> _values = [];
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the predefined format used to create optional <see cref="ApiKeyPart.ApiName"/> values during materialization.
    ///     Defaults to <see cref="ApiKeyPartNameFormat.None"/>.
    /// </summary>
    /// <remarks>
    ///     Use <see cref="ApiKeyPartNameFormat.None"/> to materialize unnamed/positional key parts.
    ///     A single composite key must still be consistently named or unnamed.
    /// </remarks>
    public ApiKeyPartNameFormat PartNameFormat { get; init; } = ApiKeyPartNameFormat.None;

    /// <summary>
    ///     Gets an optional custom formatter used to create <see cref="ApiKeyPart.ApiName"/> values during materialization.
    /// </summary>
    /// <remarks>
    ///     When provided, this delegate takes precedence over <see cref="PartNameFormat"/>.
    ///     Return <see langword="null"/> to materialize unnamed/positional key parts.
    /// </remarks>
    public ApiKeyPartNameFormatterDelegate? PartNameFormatter { get; init; }

    /// <summary>
    ///     Gets the behavior when any property in a path — whether an intermediate navigation member
    ///     or the terminal scalar member — is <see langword="null"/>.
    ///     Defaults to <see cref="ApiKeyNullHandling.UseDefaultOnNull"/>.
    /// </summary>
    public ApiKeyNullHandling NullHandling { get; init; } = ApiKeyNullHandling.UseDefaultOnNull;

    /// <summary>
    ///     Gets the optional contextual name override for the key definition being materialized.
    /// </summary>
    /// <remarks>
    ///     When non-null, this value is propagated into
    ///     <see cref="ApiKeyPartNameContext.ApiKeyName"/> for each part. It takes precedence over
    ///     <see cref="ApiNamedKeyDefinition.ApiName"/>. When null, the
    ///     <see cref="ApiNamedKeyDefinition.ApiName"/> of a named key definition is propagated;
    ///     anonymous key definitions propagate null. This value is metadata for
    ///     <see cref="PartNameFormatter"/> and is not used by the predefined
    ///     <see cref="PartNameFormat"/> values.
    /// </remarks>
    public string? ContextualKeyName { get; init; }
    #endregion

    #region Methods
    /// <summary>
    ///     Registers a CLR object instance as the root for any <see cref="ApiKeyPath"/> whose
    ///     <see cref="ApiKeyPath.ClrRootType"/> is <typeparamref name="TRoot"/> or assignable from <typeparamref name="TRoot"/>.
    /// </summary>
    /// <typeparam name="TRoot">The CLR type of the root object.</typeparam>
    /// <param name="instance">The root object instance.</param>
    /// <returns>The current context for fluent chaining.</returns>
    public ApiKeyMaterializationContext With<TRoot>(TRoot instance) where TRoot : class
    {
        ArgumentNullException.ThrowIfNull(instance);
        _roots[typeof(TRoot)] = instance;
        return this;
    }

    /// <summary>Registers a CLR object instance under its runtime type.</summary>
    /// <param name="instance">The root object instance; must not be <see langword="null"/>.</param>
    /// <returns>The current context for fluent chaining.</returns>
    internal ApiKeyMaterializationContext WithObject(object instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        _roots[instance.GetType()] = instance;
        return this;
    }

    /// <summary>
    ///     Registers an already-materialized <see cref="ApiKey"/> value for the specified root CLR type and CLR member path.
    /// </summary>
    /// <param name="clrRootType">The root CLR type of the key path.</param>
    /// <param name="clrPath">The full dotted CLR member path from the root type to the scalar member.</param>
    /// <param name="apiKey">The materialized key value.</param>
    /// <returns>The current context for fluent chaining.</returns>
    public ApiKeyMaterializationContext WithKey(Type clrRootType, string clrPath, ApiKey apiKey)
    {
        var valuePath = CreateValuePath(clrRootType, clrPath);
        _values[valuePath] = ApiKeyMaterializationValue.FromKey(apiKey);
        return this;
    }

    /// <summary>
    ///     Registers raw text for the specified root CLR type and CLR member path.
    ///     The text is parsed according to schema metadata during value-based materialization.
    /// </summary>
    /// <param name="clrRootType">The root CLR type of the key path.</param>
    /// <param name="clrPath">The full dotted CLR member path from the root type to the scalar member.</param>
    /// <param name="text">The raw text value.</param>
    /// <returns>The current context for fluent chaining.</returns>
    public ApiKeyMaterializationContext WithText(Type clrRootType, string clrPath, string? text)
    {
        var valuePath = CreateValuePath(clrRootType, clrPath);
        _values[valuePath] = ApiKeyMaterializationValue.FromText(text);
        return this;
    }

    /// <summary>
    ///     Resolves the root object for a given CLR type.
    ///     Tries an exact match first, then falls back to a registered instance whose declared type is assignable to <paramref name="clrRootType"/>.
    /// </summary>
    /// <param name="clrRootType">The CLR type to resolve a root object for.</param>
    /// <returns>The registered root object.</returns>
    /// <exception cref="ApiKeyException">Thrown when no root object is registered for <paramref name="clrRootType"/>.</exception>
    internal object ResolveRoot(Type clrRootType)
    {
        if (this.TryResolveRoot(clrRootType, out var result))
        {
            return result!;
        }

        var typeName = clrRootType.SafeToName();
        throw new ApiSchemaMaterializationException($"No root object registered for type '{typeName}'. Call With<{typeName}>() before materializing.");
    }

    internal bool TryResolveRoot(Type clrRootType, out object? result)
    {
        if (_roots.TryGetValue(clrRootType, out var exact))
        {
            result = exact;
            return true;
        }

        foreach (var (key, value) in _roots)
        {
            if (clrRootType.IsAssignableFrom(key))
            {
                result = value;
                return true;
            }
        }

        result = null;
        return false;
    }

    internal bool TryResolveValue(Type clrRootType, string clrPath, out ApiKeyMaterializationValue materializationValue)
    {
        var valuePath = CreateValuePath(clrRootType, clrPath);
        return _values.TryGetValue(valuePath, out materializationValue);
    }

    private static ApiKeyMaterializationValuePath CreateValuePath(Type clrRootType, string clrPath)
    {
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentException.ThrowIfNullOrWhiteSpace(clrPath);

        return new ApiKeyMaterializationValuePath(clrRootType, clrPath);
    }
    #endregion

    #region Nested Types
    private readonly record struct ApiKeyMaterializationValuePath(Type ClrRootType, string ClrPath);
    #endregion
}
