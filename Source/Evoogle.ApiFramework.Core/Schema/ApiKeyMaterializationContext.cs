// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Key;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

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
/// <param name="ApiKeyType">The key type being materialized.</param>
/// <param name="ApiKeyPath">The key path for the current part.</param>
/// <param name="PartIndex">The zero-based index of the current part.</param>
/// <param name="ApiKeyTypeName">The contextual name of the key type being materialized, or <see langword="null"/> for anonymous key types.</param>
public readonly record struct ApiKeyPartNameContext(ApiKeyType ApiKeyType, ApiKeyPath ApiKeyPath, int PartIndex, string? ApiKeyTypeName);

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
    public static ApiKeyMaterializationValue FromKey(ApiKey value) => new(ApiKeyMaterializationValueKind.Key, value, text: null);
    public static ApiKeyMaterializationValue FromText(string? value) => new(ApiKeyMaterializationValueKind.Text, ApiKey.Empty, value);
    #endregion
}

/// <summary>
///     Provides context for materializing an <see cref="ApiKey"/> from CLR object instances
///     using <see cref="ApiKeyType.MaterializeKey(ApiKeyMaterializationContext)"/>.
/// </summary>
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
    public ApiKeyPartNameFormatterDelegate? CustomPartNameFormatter { get; init; }

    /// <summary>
    ///     Gets the behavior when any property in a path — whether an intermediate navigation property
    ///     or the terminal scalar property — is <see langword="null"/>.
    ///     Defaults to <see cref="ApiKeyNullHandling.UseDefaultOnNull"/>.
    /// </summary>
    public ApiKeyNullHandling NullHandling { get; init; } = ApiKeyNullHandling.UseDefaultOnNull;

    /// <summary>
    ///     Gets the optional name of the key type being materialized.
    ///     Propagated into <see cref="ApiKeyPartNameContext.ApiKeyTypeName"/> for each part during materialization.
    /// </summary>
    public string? KeyTypeName { get; init; }
    #endregion

    #region Methods
    /// <summary>
    ///     Registers a CLR object instance as the root for any <see cref="ApiKeyPath"/> whose
    ///     <see cref="ApiKeyPath.ClrRootType"/> is <typeparamref name="T"/> or assignable from <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The CLR type of the root object.</typeparam>
    /// <param name="instance">The root object instance.</param>
    /// <returns>The current context for fluent chaining.</returns>
    public ApiKeyMaterializationContext With<T>(T instance) where T : class
    {
        ArgumentNullException.ThrowIfNull(instance);
        _roots[typeof(T)] = instance;
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
    ///     Registers an already-materialized <see cref="ApiKey"/> value for the specified root CLR type and CLR property path.
    /// </summary>
    /// <param name="clrRootType">The root CLR type of the key path.</param>
    /// <param name="clrPath">The full dotted CLR property path from the root type to the scalar property.</param>
    /// <param name="value">The materialized key value.</param>
    /// <returns>The current context for fluent chaining.</returns>
    public ApiKeyMaterializationContext WithKey(Type clrRootType, string clrPath, ApiKey value)
    {
        var valuePath = CreateValuePath(clrRootType, clrPath);
        _values[valuePath] = ApiKeyMaterializationValue.FromKey(value);
        return this;
    }

    /// <summary>
    ///     Registers raw text for the specified root CLR type and CLR property path.
    ///     The text is parsed according to schema metadata during value-based materialization.
    /// </summary>
    /// <param name="clrRootType">The root CLR type of the key path.</param>
    /// <param name="clrPath">The full dotted CLR property path from the root type to the scalar property.</param>
    /// <param name="value">The raw text value.</param>
    /// <returns>The current context for fluent chaining.</returns>
    public ApiKeyMaterializationContext WithText(Type clrRootType, string clrPath, string? value)
    {
        var valuePath = CreateValuePath(clrRootType, clrPath);
        _values[valuePath] = ApiKeyMaterializationValue.FromText(value);
        return this;
    }

    /// <summary>
    ///     Resolves the root object for a given CLR type.
    ///     Tries an exact match first, then falls back to a registered instance whose declared type is assignable to <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The CLR type to resolve a root object for.</param>
    /// <returns>The registered root object.</returns>
    /// <exception cref="ApiKeyException">Thrown when no root object is registered for <paramref name="type"/>.</exception>
    internal object ResolveRoot(Type type)
    {
        if (this.TryResolveRoot(type, out var result))
        {
            return result!;
        }

        var typeName = type.SafeToName();
        throw new ApiKeyException($"No root object registered for type '{typeName}'. Call With<{typeName}>() before materializing.");
    }

    internal bool TryResolveRoot(Type type, out object? result)
    {
        if (_roots.TryGetValue(type, out var exact))
        {
            result = exact;
            return true;
        }

        foreach (var (key, value) in _roots)
        {
            if (type.IsAssignableFrom(key))
            {
                result = value;
                return true;
            }
        }

        result = null;
        return false;
    }

    internal bool TryResolveValue(Type clrRootType, string clrPath, out ApiKeyMaterializationValue value)
    {
        var valuePath = CreateValuePath(clrRootType, clrPath);
        return _values.TryGetValue(valuePath, out value);
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
