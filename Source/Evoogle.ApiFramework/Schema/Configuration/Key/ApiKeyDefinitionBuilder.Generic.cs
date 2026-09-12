// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;

using Evoogle.ApiFramework.Schema.Key;

namespace Evoogle.ApiFramework.Schema.Configuration.Key;

/// <summary>
///     Strongly-typed fluent builder for configuring key-type paths rooted at
///     <typeparamref name="TRoot"/>. Extends <see cref="ApiKeyDefinitionBuilder"/> with expression-based
///     overloads so CLR member names are extracted at compile time rather than supplied as raw
///     strings.
/// </summary>
/// <typeparam name="TRoot">The default root CLR type for key paths added via expression overloads.</typeparam>
/// <param name="apiName">
///     The optional API name used when the builder produces an <see cref="ApiNamedKeyDefinition"/>.
/// </param>
/// <remarks>
///    <para>Key definitions are reusable components that define how to extract key values from CLR objects via one or more key paths. They are primarily used to configure API keys, but can also be used for other purposes such as defining unique identifiers for object types.</para>
///    <para>Each key path represents a navigation chain from a specified CLR root type to a terminal scalar member, and can be configured with extensions at both the path and segment levels. When multiple key paths are defined within a key definition, the resulting key value is a composite of the individual path values.</para>
/// </remarks>
public sealed class ApiKeyDefinitionBuilder<TRoot>(string? apiName = null) : ApiKeyDefinitionBuilder(apiName)
{
    #region AddPath Methods
    /// <summary>
    ///     Adds a key path rooted at <typeparamref name="TRoot"/> using a type-safe lambda expression.
    ///     The expression must consist only of chained member access (e.g. <c>x => x.Address.CityId</c>).
    /// </summary>
    /// <typeparam name="TScalar">The return type of the terminal scalar member.</typeparam>
    /// <param name="expression">A lambda expression selecting the scalar member, optionally through navigation members.</param>
    /// <param name="configure">Optional callback to attach extensions to the key path builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="expression"/> is not a simple member access chain.</exception>
    public ApiKeyDefinitionBuilder<TRoot> AddPath<TScalar>
    (
        Expression<Func<TRoot, TScalar>> expression,
        Action<ApiKeyPathBuilder<TRoot>>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(expression);

        var builder = ApiKeyPathBuilder<TRoot>.For(expression);
        configure?.Invoke(builder);
        this.AddKeyPathBuilderCore(builder);
        return this;
    }

    /// <summary>
    ///     Adds a key path rooted at <typeparamref name="TPathRoot"/> using a type-safe lambda expression.
    ///     This overload allows callers to specify only the root type while the terminal value is boxed to
    ///     <see cref="object"/>.
    /// </summary>
    /// <typeparam name="TPathRoot">The CLR type from which the navigation begins.</typeparam>
    /// <param name="expression">A lambda expression selecting the scalar member on <typeparamref name="TPathRoot"/>, optionally through navigation members.</param>
    /// <param name="configure">Optional callback to attach extensions to the key path builder.</param>
    /// <returns>The current builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="expression"/> is not a simple member access chain.</exception>
    public ApiKeyDefinitionBuilder<TRoot> AddPathFrom<TPathRoot>
    (
        Expression<Func<TPathRoot, object?>> expression,
        Action<ApiKeyPathBuilder<TRoot>>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(expression);

        var builder = ApiKeyPathBuilder<TRoot>.For(expression);
        configure?.Invoke(builder);
        this.AddKeyPathBuilderCore(builder);
        return this;
    }
    #endregion

    #region With Methods
    /// <inheritdoc cref="ApiKeyDefinitionBuilder.WithName(string)"/>
    public new ApiKeyDefinitionBuilder<TRoot> WithName(string apiName)
    {
        base.WithName(apiName);
        return this;
    }
    #endregion
}
