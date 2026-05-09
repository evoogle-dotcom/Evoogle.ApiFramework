// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Logging;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Maintains shared state for <see cref="ApiSchemaBuilder"/> while schema components are being configured.
///     The context caches builder instances and exposes a consolidated logger to provide consistent diagnostics.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during schema construction.</param>
public sealed class ApiSchemaBuilderContext(ILogger? logger = null)
{
    #region Fields
    private readonly Dictionary<Type, ApiScalarTypeBuilder> _apiScalarTypeBuilders = [];
    private readonly Dictionary<Type, ApiEnumTypeBuilder> _apiEnumTypeBuilders = [];
    private readonly Dictionary<Type, ApiObjectTypeBuilder> _apiObjectTypeBuilders = [];
    private readonly Dictionary<string, ApiRelationshipBuilder> _apiRelationshipBuilders = [];
    #endregion

    #region Properties
    internal IEnumerable<ApiScalarTypeBuilder> ApiScalarTypeBuilders => _apiScalarTypeBuilders.Values;
    internal IEnumerable<ApiEnumTypeBuilder> ApiEnumTypeBuilders => _apiEnumTypeBuilders.Values;
    internal IEnumerable<ApiObjectTypeBuilder> ApiObjectTypeBuilders => _apiObjectTypeBuilders.Values;
    internal IEnumerable<ApiRelationshipBuilder> ApiRelationshipBuilders => _apiRelationshipBuilders.Values;
    internal ILogger Logger { get; } = new MultiplexingLogger(logger, MultiplexingLoggerMode.None);
    #endregion

    #region Methods
    /// <summary>
    ///     Gets existing or adds new <see cref="ApiScalarTypeBuilder{T}"/> for the CLR type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The CLR scalar type.</typeparam>
    /// <returns>The corresponding <see cref="ApiScalarTypeBuilder{T}"/>.</returns>
    internal ApiScalarTypeBuilder<T> GetOrAddScalarTypeBuilder<T>()
    {
        var clrType = typeof(T);
        if (_apiScalarTypeBuilders.TryGetValue(clrType, out var existing))
        {
            if (existing is not ApiScalarTypeBuilder<T> typed)
            {
                throw new ApiSchemaException(
                    $"Scalar type '{clrType.Name}' was already registered as {existing.GetType().Name} and cannot be reconfigured as {typeof(ApiScalarTypeBuilder<T>).Name}.");
            }

            return typed;
        }

        var builder = new ApiScalarTypeBuilder<T>(this);
        _apiScalarTypeBuilders[clrType] = builder;
        return builder;
    }

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiEnumTypeBuilder{T}"/> for the CLR type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The CLR enum type.</typeparam>
    /// <returns>The corresponding <see cref="ApiEnumTypeBuilder{T}"/>.</returns>
    internal ApiEnumTypeBuilder<T> GetOrAddEnumTypeBuilder<T>() where T : Enum
    {
        var clrType = typeof(T);
        if (_apiEnumTypeBuilders.TryGetValue(clrType, out var existing))
        {
            if (existing is not ApiEnumTypeBuilder<T> typed)
            {
                throw new ApiSchemaException(
                    $"Enum type '{clrType.Name}' was already registered as {existing.GetType().Name} and cannot be reconfigured as {typeof(ApiEnumTypeBuilder<T>).Name}.");
            }

            return typed;
        }

        var builder = new ApiEnumTypeBuilder<T>(this);
        _apiEnumTypeBuilders[clrType] = builder;
        return builder;
    }

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiEnumTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR enum type.</param>
    /// <returns>The corresponding <see cref="ApiEnumTypeBuilder"/>.</returns>
    internal ApiEnumTypeBuilder GetOrAddEnumTypeBuilder(Type clrType) =>
        GetOrAddBuilder(clrType, _apiEnumTypeBuilders, static (t, ctx) => new ApiEnumTypeBuilder(t, ctx), this);

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiObjectTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR object type.</param>
    /// <returns>The corresponding <see cref="ApiObjectTypeBuilder"/>.</returns>
    internal ApiObjectTypeBuilder GetOrAddObjectTypeBuilder(Type clrType) =>
        GetOrAddBuilder(clrType, _apiObjectTypeBuilders, static (t, ctx) => new ApiObjectTypeBuilder(t, ctx), this);

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiObjectTypeBuilder{T}"/> for the CLR type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The CLR object type.</typeparam>
    /// <returns>The corresponding <see cref="ApiObjectTypeBuilder{T}"/>.</returns>
    internal ApiObjectTypeBuilder<T> GetOrAddObjectTypeBuilder<T>()
    {
        var clrType = typeof(T);
        if (_apiObjectTypeBuilders.TryGetValue(clrType, out var existing))
        {
            if (existing is not ApiObjectTypeBuilder<T> typed)
            {
                throw new ApiSchemaException(
                    $"Object type '{clrType.Name}' was already registered as {existing.GetType().Name} and cannot be reconfigured as {typeof(ApiObjectTypeBuilder<T>).Name}.");
            }

            return typed;
        }

        var builder = new ApiObjectTypeBuilder<T>(this);
        _apiObjectTypeBuilders[clrType] = builder;
        return builder;
    }

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiScalarTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR scalar type.</param>
    /// <returns>The corresponding <see cref="ApiScalarTypeBuilder"/>.</returns>
    internal ApiScalarTypeBuilder GetOrAddScalarTypeBuilder(Type clrType) =>
        GetOrAddBuilder(clrType, _apiScalarTypeBuilders, static (t, ctx) => new ApiScalarTypeBuilder(t, ctx), this);

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiRelationshipOneToOneBuilder"/> for the specified API name.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <returns>The corresponding <see cref="ApiRelationshipOneToOneBuilder"/>.</returns>
    internal ApiRelationshipOneToOneBuilder GetOrAddOneToOneRelationshipBuilder(string apiName)
        => this.GetOrAddTypedRelationshipBuilder(apiName, static n => new ApiRelationshipOneToOneBuilder(n));

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiRelationshipOneToManyBuilder"/> for the specified API name.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <returns>The corresponding <see cref="ApiRelationshipOneToManyBuilder"/>.</returns>
    internal ApiRelationshipOneToManyBuilder GetOrAddOneToManyRelationshipBuilder(string apiName)
        => this.GetOrAddTypedRelationshipBuilder(apiName, static n => new ApiRelationshipOneToManyBuilder(n));

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiRelationshipManyToManyBuilder"/> for the specified API name.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <returns>The corresponding <see cref="ApiRelationshipManyToManyBuilder"/>.</returns>
    internal ApiRelationshipManyToManyBuilder GetOrAddManyToManyRelationshipBuilder(string apiName)
        => this.GetOrAddTypedRelationshipBuilder(apiName, static n => new ApiRelationshipManyToManyBuilder(n));

    private TBuilder GetOrAddTypedRelationshipBuilder<TBuilder>(string apiName, Func<string, TBuilder> factory)
        where TBuilder : ApiRelationshipBuilder
    {
        if (_apiRelationshipBuilders.TryGetValue(apiName, out var existing))
        {
            if (existing is not TBuilder typed)
            {
                throw new ApiSchemaException($"Relationship '{apiName}' was already registered as {existing.GetType().Name} and cannot be reconfigured as {typeof(TBuilder).Name}.");
            }

            return typed;
        }

        var builder = factory(apiName);
        _apiRelationshipBuilders[apiName] = builder;
        return builder;
    }

    private static TBuilder GetOrAddBuilder<TBuilder>
    (
        Type clrType,
        Dictionary<Type, TBuilder> builders,
        Func<Type, ApiSchemaBuilderContext, TBuilder> factory,
        ApiSchemaBuilderContext context
    )
    {
        ArgumentNullException.ThrowIfNull(clrType);

        if (!builders.TryGetValue(clrType, out var builder))
        {
            builder = factory(clrType, context);
            builders[clrType] = builder;
        }

        return builder;
    }
    #endregion
}
