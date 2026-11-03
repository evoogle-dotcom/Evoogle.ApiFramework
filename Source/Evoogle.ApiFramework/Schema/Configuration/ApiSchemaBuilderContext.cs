// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Types;
using Evoogle.Logging;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Maintains shared state for <see cref="ApiSchemaBuilder"/> while schema components are being configured.
///     The context caches builder instances and exposes a consolidated logger to provide consistent diagnostics.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during schema construction.</param>
public sealed partial class ApiSchemaBuilderContext(ILogger? logger = null) : IHasLogger
{
    #region Fields
    private readonly Dictionary<Type, ApiScalarTypeBuilder> _apiScalarTypeBuilders = [];
    private readonly Dictionary<Type, ApiEnumTypeBuilder> _apiEnumTypeBuilders = [];
    private readonly Dictionary<Type, ApiObjectTypeBuilder> _apiObjectTypeBuilders = [];

    private readonly Queue<ApiObjectTypeBuilder> _pendingObjectBuilders = new();
    private readonly Queue<ApiScalarTypeBuilder> _pendingScalarBuilders = new();
    private readonly Queue<ApiEnumTypeBuilder> _pendingEnumBuilders = new();
    private readonly List<ApiSchemaCompilationIssue> _configurationIssues = [];
    private readonly HashSet<Type> _discoveredConfigurationTypes = [];
    private readonly HashSet<(Type ClrType, string Role)> _discoveredConfigurationRoles = [];
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    #endregion

    #region IHasLogger Properties
    /// <inheritdoc/>
    public ILogger Logger { get; } = new MultiplexingLogger(logger, MultiplexingLoggerMode.None);
    #endregion

    #region Properties
    internal IEnumerable<ApiScalarTypeBuilder> ApiScalarTypeBuilders => _apiScalarTypeBuilders.Values;
    internal IEnumerable<ApiEnumTypeBuilder> ApiEnumTypeBuilders => _apiEnumTypeBuilders.Values;
    internal IEnumerable<ApiObjectTypeBuilder> ApiObjectTypeBuilders => _apiObjectTypeBuilders.Values;
    internal IReadOnlyList<ApiSchemaCompilationIssue> ConfigurationIssues => _configurationIssues;

    /// <summary>Gets the source associated with the active configuration callback.</summary>
    internal ApiConfigurationSource CurrentConfigurationSource => _configurationSourceScope.CurrentSource;

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

    #region Configuration Methods
    /// <summary>Records a configuration-discovery issue for the current schema build.</summary>
    internal void AddConfigurationIssue(ApiSchemaCompilationIssue issue)
    {
        ArgumentNullException.ThrowIfNull(issue);

        _configurationIssues.Add(issue);
    }

    /// <summary>Runs a configuration callback at the supplied source precedence.</summary>
    internal void ApplyConfiguration(ApiConfigurationSource source, Action configure)
        => _configurationSourceScope.Apply(source, configure);
    #endregion

    #region Type Builder Methods
    /// <summary>
    ///     Gets existing or adds new <see cref="ApiScalarTypeBuilder{TScalar}"/> for the CLR type <typeparamref name="TScalar"/>.
    /// </summary>
    /// <typeparam name="TScalar">The CLR scalar type.</typeparam>
    /// <returns>The corresponding <see cref="ApiScalarTypeBuilder{TScalar}"/>.</returns>
    internal ApiScalarTypeBuilder<TScalar> GetOrAddScalarTypeBuilder<TScalar>()
        => (ApiScalarTypeBuilder<TScalar>)this.GetOrAddScalarTypeBuilder(typeof(TScalar));

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiScalarTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR scalar type.</param>
    /// <returns>The corresponding <see cref="ApiScalarTypeBuilder"/>.</returns>
    internal ApiScalarTypeBuilder GetOrAddScalarTypeBuilder(Type clrType)
        => GetOrAddBuilder
        (
            clrType,
            _apiScalarTypeBuilders,
            static (t, ctx) => ApiBuilderFactory.CreateClosedGeneric<ApiScalarTypeBuilder>(typeof(ApiScalarTypeBuilder<>), t, ctx),
            this,
            _pendingScalarBuilders
        );

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
    internal ApiEnumTypeBuilder GetOrAddEnumTypeBuilder(Type clrType)
        => GetOrAddBuilder
        (
            clrType,
            _apiEnumTypeBuilders,
            static (t, ctx) => ApiBuilderFactory.CreateClosedGeneric<ApiEnumTypeBuilder>(typeof(ApiEnumTypeBuilder<>), t, ctx),
            this,
            _pendingEnumBuilders
        );

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiObjectTypeBuilder{TObject}"/> for the CLR type <typeparamref name="TObject"/>.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type.</typeparam>
    /// <returns>The corresponding <see cref="ApiObjectTypeBuilder{TObject}"/>.</returns>
    internal ApiObjectTypeBuilder<TObject> GetOrAddObjectTypeBuilder<TObject>()
        => (ApiObjectTypeBuilder<TObject>)this.GetOrAddObjectTypeBuilder(typeof(TObject));

    /// <summary>
    ///     Gets existing or adds new <see cref="ApiObjectTypeBuilder"/> for the specified CLR type.
    /// </summary>
    /// <param name="clrType">The CLR object type.</param>
    /// <returns>The corresponding <see cref="ApiObjectTypeBuilder"/>.</returns>
    internal ApiObjectTypeBuilder GetOrAddObjectTypeBuilder(Type clrType)
        => GetOrAddBuilder
        (
            clrType,
            _apiObjectTypeBuilders,
            static (t, ctx) => ApiBuilderFactory.CreateClosedGeneric<ApiObjectTypeBuilder>(typeof(ApiObjectTypeBuilder<>), t, ctx),
            this,
            _pendingObjectBuilders
        );

    private static TBuilder GetOrAddBuilder<TBuilder>
    (
        Type clrType,
        Dictionary<Type, TBuilder> builders,
        Func<Type, ApiSchemaBuilderContext, TBuilder> factory,
        ApiSchemaBuilderContext context,
        Queue<TBuilder>? pendingQueue
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
    #endregion

    #region Build Copy Methods
    private void CopyConfigurationIssueStateTo(ApiSchemaBuilderContext copy)
        => copy._configurationIssues.AddRange(_configurationIssues);

    private void CopyEnumTypeStateTo(ApiSchemaBuilderContext copy)
    {
        foreach (var (clrType, builder) in _apiEnumTypeBuilders)
        {
            var copiedBuilder = builder.CreateBuildCopy(copy);
            copy._apiEnumTypeBuilders.Add(clrType, copiedBuilder);
            copy._pendingEnumBuilders.Enqueue(copiedBuilder);
        }
    }

    private void CopyObjectTypeStateTo(ApiSchemaBuilderContext copy)
    {
        foreach (var (clrType, builder) in _apiObjectTypeBuilders)
        {
            var copiedBuilder = builder.CreateBuildCopy(copy);
            copy._apiObjectTypeBuilders.Add(clrType, copiedBuilder);
            copy._pendingObjectBuilders.Enqueue(copiedBuilder);
        }
    }

    private void CopyScalarTypeStateTo(ApiSchemaBuilderContext copy)
    {
        foreach (var (clrType, builder) in _apiScalarTypeBuilders)
        {
            var copiedBuilder = builder.CreateBuildCopy(copy);
            copy._apiScalarTypeBuilders.Add(clrType, copiedBuilder);
            copy._pendingScalarBuilders.Enqueue(copiedBuilder);
        }
    }

    internal ApiSchemaBuilderContext CreateBuildCopy()
    {
        var copy = new ApiSchemaBuilderContext(this.Logger);

        this.CopyScalarTypeStateTo(copy);
        this.CopyEnumTypeStateTo(copy);
        this.CopyObjectTypeStateTo(copy);
        this.CopyQuerySourceStateTo(copy);
        this.CopyRelationshipStateTo(copy);
        this.CopyConfigurationIssueStateTo(copy);

        return copy;
    }
    #endregion

    #region Configuration Discovery Methods
    internal bool TryBeginConfigurationDiscovery(Type clrType)
        => _discoveredConfigurationTypes.Add(clrType);

    internal bool TryBeginConfigurationRole(Type clrType, string role)
        => _discoveredConfigurationRoles.Add((clrType, role));
    #endregion

}
