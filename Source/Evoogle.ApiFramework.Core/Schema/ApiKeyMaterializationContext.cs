// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Key;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Builds the optional part name used when materializing one <see cref="ApiKeyPart"/>.
/// </summary>
/// <param name="context">The part naming context.</param>
/// <returns>
///     The part name to use, or <see langword="null"/> to create an unnamed/positional key part.
/// </returns>
public delegate string? ApiKeyPartNameBuildDelegate(ApiKeyPartNameContext context);

/// <summary>
///     Provides metadata to an <see cref="ApiKeyPartNameBuildDelegate"/> while materializing an <see cref="ApiKey"/>.
/// </summary>
/// <param name="ApiKeyType">The key type being materialized.</param>
/// <param name="ApiKeyPath">The key path for the current part.</param>
/// <param name="PartIndex">The zero-based index of the current part.</param>
public readonly record struct ApiKeyPartNameContext(ApiKeyType ApiKeyType, ApiKeyPath ApiKeyPath, int PartIndex);

/// <summary>
///     Provides context for materializing an <see cref="ApiKey"/> from CLR object instances
///     using <see cref="ApiKeyType.MaterializeKey(ApiKeyMaterializationContext)"/>.
/// </summary>
public sealed class ApiKeyMaterializationContext
{
    #region Fields
    private readonly Dictionary<Type, object> _roots = [];
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the builder used to create optional <see cref="ApiKeyPart.ApiName"/> values during materialization.
    ///     Defaults to <see cref="ApiKeyPartNameBuilder.None"/>.
    /// </summary>
    /// <remarks>
    ///     Use <see cref="ApiKeyPartNameBuilder.None"/> to materialize unnamed/positional key parts.
    ///     A single composite key must still be consistently named or unnamed.
    /// </remarks>
    public ApiKeyPartNameBuilder PartNameBuilder { get; init; } = ApiKeyPartNameBuilder.None;

    /// <summary>
    ///     Gets an optional custom builder used to create <see cref="ApiKeyPart.ApiName"/> values during materialization.
    /// </summary>
    /// <remarks>
    ///     When provided, this delegate takes precedence over <see cref="PartNameBuilder"/>.
    ///     Return <see langword="null"/> to materialize unnamed/positional key parts.
    /// </remarks>
    public ApiKeyPartNameBuildDelegate? CustomPartNameBuilder { get; init; }

    /// <summary>
    ///     Gets the behavior when any property in a path — whether an intermediate navigation property
    ///     or the terminal scalar property — is <see langword="null"/>.
    ///     Defaults to <see cref="ApiKeyNullHandling.UseDefaultOnNull"/>.
    /// </summary>
    public ApiKeyNullHandling NullHandling { get; init; } = ApiKeyNullHandling.UseDefaultOnNull;
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
    ///     Resolves the root object for a given CLR type.
    ///     Tries an exact match first, then falls back to a registered instance whose declared type is assignable to <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The CLR type to resolve a root object for.</param>
    /// <returns>The registered root object.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no root object is registered for <paramref name="type"/>.</exception>
    internal object ResolveRoot(Type type)
    {
        if (this.TryResolveRoot(type, out var result))
        {
            return result!;
        }

        var typeName = type.SafeToName();
        throw new InvalidOperationException($"No root object registered for type '{typeName}'. Call With<{typeName}>() before materializing.");
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
    #endregion
}
