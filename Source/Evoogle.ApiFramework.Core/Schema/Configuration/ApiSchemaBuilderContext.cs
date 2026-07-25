// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
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

    private readonly Queue<ApiObjectTypeBuilder> _pendingObjectBuilders = new();
    private readonly Queue<ApiScalarTypeBuilder> _pendingScalarBuilders = new();
    private readonly Queue<ApiEnumTypeBuilder> _pendingEnumBuilders = new();
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    #endregion

    #region Properties
    internal IEnumerable<ApiScalarTypeBuilder> ApiScalarTypeBuilders => _apiScalarTypeBuilders.Values;
    internal IEnumerable<ApiEnumTypeBuilder> ApiEnumTypeBuilders => _apiEnumTypeBuilders.Values;
    internal IEnumerable<ApiObjectTypeBuilder> ApiObjectTypeBuilders => _apiObjectTypeBuilders.Values;
    internal IEnumerable<ApiRelationshipBuilder> ApiRelationshipBuilders => _apiRelationshipBuilders.Values;
    internal ILogger Logger { get; } = new MultiplexingLogger(logger, MultiplexingLoggerMode.None);

    /// <summary>Gets the source associated with the active configuration callback.</summary>
    internal ApiConfigurationSource CurrentConfigurationSource =>
        _configurationSourceScope.CurrentSource;

    /// <summary>Returns <c>true</c> when at least one newly registered type builder is awaiting convention processing.</summary>
    internal bool HasPendingBuilders =>
        _pendingObjectBuilders.Count > 0 ||
        _pendingScalarBuilders.Count > 0 ||
        _pendingEnumBuilders.Count > 0;
    #endregion

    #region Drain Methods
    /// <summary>Dequeues and returns all pending object type builders, clearing the queue.</summary>
    internal IReadOnlyList<ApiObjectTypeBuilder> DrainPendingObjectBuilders()
    {
        var result = _pendingObjectBuilders.ToList();
        _pendingObjectBuilders.Clear();
        return result;
    }

    /// <summary>Dequeues and returns all pending scalar type builders, clearing the queue.</summary>
    internal IReadOnlyList<ApiScalarTypeBuilder> DrainPendingScalarBuilders()
    {
        var result = _pendingScalarBuilders.ToList();
        _pendingScalarBuilders.Clear();
        return result;
    }

    /// <summary>Dequeues and returns all pending enum type builders, clearing the queue.</summary>
    internal IReadOnlyList<ApiEnumTypeBuilder> DrainPendingEnumBuilders()
    {
        var result = _pendingEnumBuilders.ToList();
        _pendingEnumBuilders.Clear();
        return result;
    }
    #endregion

    #region Methods
    /// <summary>Runs a configuration callback at the supplied source precedence.</summary>
    internal void ApplyConfiguration(ApiConfigurationSource source, Action configure)
    {
        _configurationSourceScope.Apply(source, configure);
    }

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiScalarTypeBuilder{TScalar}"/> for the CLR type <typeparamref name="TScalar"/>.
    /// </summary>
    /// <typeparam name="TScalar">The CLR scalar type.</typeparam>
    /// <returns>The corresponding <see cref="ApiScalarTypeBuilder{TScalar}"/>.</returns>
    internal ApiScalarTypeBuilder<TScalar> GetOrAddScalarTypeBuilder<TScalar>()
    {
        var clrType = typeof(TScalar);
        if (_apiScalarTypeBuilders.TryGetValue(clrType, out var existing))
        {
            if (existing is not ApiScalarTypeBuilder<TScalar> typed)
            {
                throw new ApiSchemaConfigurationException($"Scalar type '{clrType.Name}' was already registered as {existing.GetType().Name} and cannot be reconfigured as {typeof(ApiScalarTypeBuilder<TScalar>).Name}.");
            }

            return typed;
        }

        var builder = new ApiScalarTypeBuilder<TScalar>(this);
        _apiScalarTypeBuilders[clrType] = builder;
        _pendingScalarBuilders.Enqueue(builder);
        return builder;
    }

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiEnumTypeBuilder{TEnum}"/> for the CLR type <typeparamref name="TEnum"/>.
    /// </summary>
    /// <typeparam name="TEnum">The CLR enum type.</typeparam>
    /// <returns>The corresponding <see cref="ApiEnumTypeBuilder{TEnum}"/>.</returns>
    internal ApiEnumTypeBuilder<TEnum> GetOrAddEnumTypeBuilder<TEnum>() where TEnum : Enum
    {
        var clrType = typeof(TEnum);
        if (_apiEnumTypeBuilders.TryGetValue(clrType, out var existing))
        {
            if (existing is not ApiEnumTypeBuilder<TEnum> typed)
            {
                throw new ApiSchemaConfigurationException($"Enum type '{clrType.Name}' was already registered as {existing.GetType().Name} and cannot be reconfigured as {typeof(ApiEnumTypeBuilder<TEnum>).Name}.");
            }

            return typed;
        }

        var builder = new ApiEnumTypeBuilder<TEnum>(this);
        _apiEnumTypeBuilders[clrType] = builder;
        _pendingEnumBuilders.Enqueue(builder);
        return builder;
    }

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiEnumTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR enum type.</param>
    /// <returns>The corresponding <see cref="ApiEnumTypeBuilder"/>.</returns>
    internal ApiEnumTypeBuilder GetOrAddEnumTypeBuilder(Type clrType) =>
        GetOrAddBuilder(clrType, _apiEnumTypeBuilders, static (t, ctx) => new ApiEnumTypeBuilder(t, ctx), this, _pendingEnumBuilders);

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiObjectTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR object type.</param>
    /// <returns>The corresponding <see cref="ApiObjectTypeBuilder"/>.</returns>
    internal ApiObjectTypeBuilder GetOrAddObjectTypeBuilder(Type clrType) =>
        GetOrAddBuilder(clrType, _apiObjectTypeBuilders, static (t, ctx) => new ApiObjectTypeBuilder(t, ctx), this, _pendingObjectBuilders);

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiObjectTypeBuilder{TObject}"/> for the CLR type <typeparamref name="TObject"/>.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type.</typeparam>
    /// <returns>The corresponding <see cref="ApiObjectTypeBuilder{TObject}"/>.</returns>
    internal ApiObjectTypeBuilder<TObject> GetOrAddObjectTypeBuilder<TObject>()
    {
        var clrType = typeof(TObject);
        if (_apiObjectTypeBuilders.TryGetValue(clrType, out var existing))
        {
            if (existing is not ApiObjectTypeBuilder<TObject> typed)
            {
                throw new ApiSchemaConfigurationException($"Object type '{clrType.Name}' was already registered as {existing.GetType().Name} and cannot be reconfigured as {typeof(ApiObjectTypeBuilder<TObject>).Name}.");
            }

            return typed;
        }

        var builder = new ApiObjectTypeBuilder<TObject>(this);
        _apiObjectTypeBuilders[clrType] = builder;
        _pendingObjectBuilders.Enqueue(builder);
        return builder;
    }

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiScalarTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR scalar type.</param>
    /// <returns>The corresponding <see cref="ApiScalarTypeBuilder"/>.</returns>
    internal ApiScalarTypeBuilder GetOrAddScalarTypeBuilder(Type clrType) =>
        GetOrAddBuilder(clrType, _apiScalarTypeBuilders, static (t, ctx) => new ApiScalarTypeBuilder(t, ctx), this, _pendingScalarBuilders);

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiRelationshipOneToOneBuilder"/> for the specified API name.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <returns>The corresponding <see cref="ApiRelationshipOneToOneBuilder"/>.</returns>
    internal ApiRelationshipOneToOneBuilder GetOrAddOneToOneRelationshipBuilder(string apiName)
        => this.GetOrAddOneToOneRelationshipBuilder(apiName, this.CurrentConfigurationSource)!;

    /// <summary>
    ///     Gets or adds a one-to-one relationship builder at the supplied configuration source.
    /// </summary>
    internal ApiRelationshipOneToOneBuilder? GetOrAddOneToOneRelationshipBuilder
    (
        string apiName,
        ApiConfigurationSource source
    )
        => this.GetOrAddTypedRelationshipBuilder
        (
            apiName,
            source,
            static n => new ApiRelationshipOneToOneBuilder(n)
        );

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiRelationshipOneToManyBuilder"/> for the specified API name.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <returns>The corresponding <see cref="ApiRelationshipOneToManyBuilder"/>.</returns>
    internal ApiRelationshipOneToManyBuilder GetOrAddOneToManyRelationshipBuilder(string apiName)
        => this.GetOrAddOneToManyRelationshipBuilder(apiName, this.CurrentConfigurationSource)!;

    /// <summary>
    ///     Gets or adds a one-to-many relationship builder at the supplied configuration source.
    /// </summary>
    internal ApiRelationshipOneToManyBuilder? GetOrAddOneToManyRelationshipBuilder
    (
        string apiName,
        ApiConfigurationSource source
    )
        => this.GetOrAddTypedRelationshipBuilder
        (
            apiName,
            source,
            static n => new ApiRelationshipOneToManyBuilder(n)
        );

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiRelationshipManyToManyBuilder"/> for the specified API name.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <returns>The corresponding <see cref="ApiRelationshipManyToManyBuilder"/>.</returns>
    internal ApiRelationshipManyToManyBuilder GetOrAddManyToManyRelationshipBuilder(string apiName)
        => this.GetOrAddManyToManyRelationshipBuilder(apiName, this.CurrentConfigurationSource)!;

    /// <summary>
    ///     Gets or adds a many-to-many relationship builder at the supplied configuration source.
    /// </summary>
    internal ApiRelationshipManyToManyBuilder? GetOrAddManyToManyRelationshipBuilder
    (
        string apiName,
        ApiConfigurationSource source
    )
        => this.GetOrAddTypedRelationshipBuilder
        (
            apiName,
            source,
            static n => new ApiRelationshipManyToManyBuilder(n)
        );

    private static TBuilder GetOrAddBuilder<TBuilder>
    (
        Type clrType,
        Dictionary<Type, TBuilder> builders,
        Func<Type, ApiSchemaBuilderContext, TBuilder> factory,
        ApiSchemaBuilderContext context,
        Queue<TBuilder>? pendingQueue = null
    )
    {
        ArgumentNullException.ThrowIfNull(clrType);

        if (!builders.TryGetValue(clrType, out var builder))
        {
            builder = factory(clrType, context);
            builders[clrType] = builder;
            pendingQueue?.Enqueue(builder);
        }

        return builder;
    }

    private TBuilder? GetOrAddTypedRelationshipBuilder<TBuilder>
    (
        string apiName,
        ApiConfigurationSource source,
        Func<string, TBuilder> factory
    )
        where TBuilder : ApiRelationshipBuilder
    {
        if (_apiRelationshipBuilders.TryGetValue(apiName, out var existing))
        {
            if (existing is not TBuilder typed)
            {
                if (source < existing.RegistrationSource)
                {
                    return null;
                }

                throw new ApiSchemaConfigurationException
                (
                    $"Relationship '{apiName}' was already registered as " +
                    $"{existing.GetType().Name} and cannot be reconfigured as " +
                    $"{typeof(TBuilder).Name}."
                );
            }

            typed.SetRegistrationSource(source);
            return typed;
        }

        var builder = factory(apiName);
        builder.SetRegistrationSource(source);
        _apiRelationshipBuilders[apiName] = builder;
        return builder;
    }
    #endregion
}
