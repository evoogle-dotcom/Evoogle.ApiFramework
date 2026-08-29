// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Trace;
using Evoogle.ApiFramework.Schema.Configuration.Trace.Internal;
using Evoogle.Logging;
using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Maintains shared state for <see cref="ApiSchemaBuilder"/> while schema components are being configured.
///     The context caches builder instances and exposes a consolidated logger to provide consistent diagnostics.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during schema construction.</param>
public sealed class ApiSchemaBuilderContext(ILogger? logger = null) : IHasLogger
{
    #region Fields
    private readonly Dictionary<Type, ApiScalarTypeBuilder> _apiScalarTypeBuilders = [];
    private readonly Dictionary<Type, ApiEnumTypeBuilder> _apiEnumTypeBuilders = [];
    private readonly Dictionary<Type, ApiObjectTypeBuilder> _apiObjectTypeBuilders = [];
    private readonly Dictionary<string, ApiRelationshipBuilder> _apiRelationshipBuilders = [];

    private readonly Queue<ApiObjectTypeBuilder> _pendingObjectBuilders = new();
    private readonly Queue<ApiScalarTypeBuilder> _pendingScalarBuilders = new();
    private readonly Queue<ApiEnumTypeBuilder> _pendingEnumBuilders = new();
    private readonly List<ApiInitializationIssue> _configurationIssues = [];
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    private ApiSchemaBuildTraceDispatcher? _traceDispatcher;
    #endregion

    #region IHasLogger Properties
    /// <inheritdoc/>
    public ILogger Logger { get; } = new MultiplexingLogger(logger, MultiplexingLoggerMode.None);
    #endregion

    #region Properties
    internal IEnumerable<ApiScalarTypeBuilder> ApiScalarTypeBuilders => _apiScalarTypeBuilders.Values;
    internal IEnumerable<ApiEnumTypeBuilder> ApiEnumTypeBuilders => _apiEnumTypeBuilders.Values;
    internal IEnumerable<ApiObjectTypeBuilder> ApiObjectTypeBuilders => _apiObjectTypeBuilders.Values;
    internal IEnumerable<ApiRelationshipBuilder> ApiRelationshipBuilders => _apiRelationshipBuilders.Values;
    internal IReadOnlyList<ApiInitializationIssue> ConfigurationIssues => _configurationIssues;

    /// <summary>Gets the source associated with the active configuration callback.</summary>
    internal ApiConfigurationSource CurrentConfigurationSource =>
        _configurationSourceScope.CurrentSource;

    /// <summary>Gets the active optional schema-build trace dispatcher.</summary>
    internal ApiSchemaBuildTraceDispatcher? TraceDispatcher => _traceDispatcher;

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
    /// <summary>Clears configuration-discovery issues for a new schema build.</summary>
    internal void ResetConfigurationIssues()
    {
        _configurationIssues.Clear();
    }

    /// <summary>Records a configuration-discovery issue for the current schema build.</summary>
    internal void AddConfigurationIssue(ApiInitializationIssue issue)
    {
        ArgumentNullException.ThrowIfNull(issue);

        _configurationIssues.Add(issue);
    }

    /// <summary>Sets the optional trace dispatcher for the current build.</summary>
    internal void SetTraceDispatcher(ApiSchemaBuildTraceDispatcher? traceDispatcher)
    {
        _traceDispatcher = traceDispatcher;
    }

    /// <summary>Runs a configuration callback at the supplied source precedence.</summary>
    internal void ApplyConfiguration(ApiConfigurationSource source, Action configure)
    {
        _configurationSourceScope.Apply(source, configure);
    }

    /// <summary>Records a configuration attempt when tracing is enabled.</summary>
    internal void TraceConfigurationChange
    (
        ApiSchemaBuildTraceTarget target,
        ApiSchemaBuildConfigurationFacet facet,
        ApiConfigurationSource source,
        string? previousValue,
        string? requestedValue,
        string? effectiveValue,
        bool wasApplied,
        string? rejectionReason = null
    )
    {
        _traceDispatcher?.Record
        (
            new ApiSchemaBuildConfigurationChangeEvent
            {
                Target = target,
                Facet = facet,
                ConfigurationSource = source.ToTraceSource(),
                PreviousValue = previousValue,
                RequestedValue = requestedValue,
                EffectiveValue = effectiveValue,
                WasApplied = wasApplied,
                RejectionReason = rejectionReason,
            }
        );
    }

    /// <summary>Records a structural registration attempt when tracing is enabled.</summary>
    internal void TraceStructuralRegistration
    (
        ApiSchemaBuildTraceTarget target,
        ApiSchemaBuildRegistrationKind registrationKind,
        ApiConfigurationSource source,
        bool wasRegistered,
        int? clrOrdinal = null,
        string? rejectionReason = null
    )
    {
        _traceDispatcher?.Record
        (
            new ApiSchemaBuildStructuralRegistrationEvent
            {
                Target = target,
                RegistrationKind = registrationKind,
                ConfigurationSource = source.ToTraceSource(),
                WasRegistered = wasRegistered,
                ClrOrdinal = clrOrdinal,
                RejectionReason = rejectionReason,
            }
        );
    }

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiScalarTypeBuilder{TScalar}"/> for the CLR type <typeparamref name="TScalar"/>.
    /// </summary>
    /// <typeparam name="TScalar">The CLR scalar type.</typeparam>
    /// <returns>The corresponding <see cref="ApiScalarTypeBuilder{TScalar}"/>.</returns>
    internal ApiScalarTypeBuilder<TScalar> GetOrAddScalarTypeBuilder<TScalar>()
        => (ApiScalarTypeBuilder<TScalar>)this.GetOrAddScalarTypeBuilder(typeof(TScalar));

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiEnumTypeBuilder{TEnum}"/> for the CLR type <typeparamref name="TEnum"/>.
    /// </summary>
    /// <typeparam name="TEnum">The CLR enum type.</typeparam>
    /// <returns>The corresponding <see cref="ApiEnumTypeBuilder{TEnum}"/>.</returns>
    internal ApiEnumTypeBuilder<TEnum> GetOrAddEnumTypeBuilder<TEnum>() where TEnum : Enum
        => (ApiEnumTypeBuilder<TEnum>)this.GetOrAddEnumTypeBuilder(typeof(TEnum));

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiEnumTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR enum type.</param>
    /// <returns>The corresponding <see cref="ApiEnumTypeBuilder"/>.</returns>
    internal ApiEnumTypeBuilder GetOrAddEnumTypeBuilder(Type clrType) =>
        GetOrAddBuilder
        (
            clrType,
            _apiEnumTypeBuilders,
            static (t, ctx) => ApiBuilderFactory.CreateClosedGeneric<ApiEnumTypeBuilder>(typeof(ApiEnumTypeBuilder<>), t, ctx),
            this,
            _pendingEnumBuilders,
            ApiSchemaBuildTargetKind.EnumType
        );

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiObjectTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR object type.</param>
    /// <returns>The corresponding <see cref="ApiObjectTypeBuilder"/>.</returns>
    internal ApiObjectTypeBuilder GetOrAddObjectTypeBuilder(Type clrType) =>
        GetOrAddBuilder
        (
            clrType,
            _apiObjectTypeBuilders,
            static (t, ctx) => ApiBuilderFactory.CreateClosedGeneric<ApiObjectTypeBuilder>(typeof(ApiObjectTypeBuilder<>), t, ctx),
            this,
            _pendingObjectBuilders,
            ApiSchemaBuildTargetKind.ObjectType
        );

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiObjectTypeBuilder{TObject}"/> for the CLR type <typeparamref name="TObject"/>.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type.</typeparam>
    /// <returns>The corresponding <see cref="ApiObjectTypeBuilder{TObject}"/>.</returns>
    internal ApiObjectTypeBuilder<TObject> GetOrAddObjectTypeBuilder<TObject>()
        => (ApiObjectTypeBuilder<TObject>)this.GetOrAddObjectTypeBuilder(typeof(TObject));

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiScalarTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR scalar type.</param>
    /// <returns>The corresponding <see cref="ApiScalarTypeBuilder"/>.</returns>
    internal ApiScalarTypeBuilder GetOrAddScalarTypeBuilder(Type clrType) =>
        GetOrAddBuilder
        (
            clrType,
            _apiScalarTypeBuilders,
            static (t, ctx) => ApiBuilderFactory.CreateClosedGeneric<ApiScalarTypeBuilder>(typeof(ApiScalarTypeBuilder<>), t, ctx),
            this,
            _pendingScalarBuilders,
            ApiSchemaBuildTargetKind.ScalarType
        );

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
        Queue<TBuilder>? pendingQueue,
        ApiSchemaBuildTargetKind targetKind
    )
    {
        ArgumentNullException.ThrowIfNull(clrType);

        if (!builders.TryGetValue(clrType, out var builder))
        {
            builder = factory(clrType, context);
            builders[clrType] = builder;
            pendingQueue?.Enqueue(builder);
            context.TraceStructuralRegistration
            (
                new(targetKind, clrType),
                ApiSchemaBuildRegistrationKind.Type,
                context.CurrentConfigurationSource,
                wasRegistered: true
            );
        }
        else
        {
            context.TraceStructuralRegistration
            (
                new(targetKind, clrType),
                ApiSchemaBuildRegistrationKind.Type,
                context.CurrentConfigurationSource,
                wasRegistered: false,
                rejectionReason: "The type was already registered."
            );
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
            this.TraceStructuralRegistration
            (
                new(ApiSchemaBuildTargetKind.Relationship, ApiName: apiName),
                ApiSchemaBuildRegistrationKind.Relationship,
                source,
                wasRegistered: false,
                rejectionReason: "The relationship name was already registered."
            );

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
        this.TraceStructuralRegistration
        (
            new(ApiSchemaBuildTargetKind.Relationship, ApiName: apiName),
            ApiSchemaBuildRegistrationKind.Relationship,
            source,
            wasRegistered: true
        );
        return builder;
    }
    #endregion
}
