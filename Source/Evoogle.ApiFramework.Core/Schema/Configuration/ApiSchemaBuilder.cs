// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Microsoft.Extensions.Logging;

using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Conventions;
using Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Provides a fluent API for programmatically constructing an <see cref="ApiSchema"/>.
/// </summary>
public sealed class ApiSchemaBuilder(ILogger<ApiSchemaBuilder>? logger = null) : ExtensionBuilder<ApiSchemaBuilder>
{
    #region Fields
    private string? _apiName;
    private string? _apiVersion;
    private Action<ApiSchemaOptionsBuilder>? _apiOptionsConfiguration = null;
    private ApiConventionSet? _conventionSet;
    private ApiAnnotationReaderSet? _annotationReaderSet;

    private readonly ApiSchemaBuilderContext _context = new(logger);
    #endregion

    #region AddEnum Methods
    /// <summary>
    ///     Adds an enumeration type to the schema using an optional inline configuration action.
    /// </summary>
    /// <param name="clrType">The CLR enum type.</param>
    /// <param name="configure">Optional callback to configure the added enumeration type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddEnum(Type clrType, Action<ApiEnumTypeBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(clrType);

        var builder = _context.GetOrAddEnumTypeBuilder(clrType);

        configure?.Invoke(builder);
        return this;
    }

    /// <summary>
    ///     Adds an enumeration type to the schema using an implementation of <see cref="IApiEnumTypeConfiguration"/>.
    /// </summary>
    /// <param name="clrType">The CLR enum type.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddEnum(Type clrType, IApiEnumTypeConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = _context.GetOrAddEnumTypeBuilder(clrType);

        configuration.Configure(builder);
        return this;
    }

    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddSchemaExtension(Type extensionType, object extension)
    {
        return this.AddExtension(extensionType, extension);
    }
    #endregion

    #region AddObject Methods
    /// <summary>
    ///     Adds an object type to the schema using an optional inline configuration action.
    /// </summary>
    /// <param name="clrType">The CLR object type.</param>
    /// <param name="configure">Optional callback to configure the added object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddObject(Type clrType, Action<ApiObjectTypeBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(clrType);

        var builder = _context.GetOrAddObjectTypeBuilder(clrType);

        configure?.Invoke(builder);
        return this;
    }

    /// <summary>
    ///     Adds an object type to the schema using an implementation of <see cref="IApiObjectTypeConfiguration"/>.
    /// </summary>
    /// <param name="clrType">The CLR object type.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddObject(Type clrType, IApiObjectTypeConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = _context.GetOrAddObjectTypeBuilder(clrType);

        configuration.Configure(builder);
        return this;
    }

    #endregion

    #region AddScalar Methods
    /// <summary>
    ///     Adds a scalar type to the schema using an inline configuration action.
    /// </summary>
    /// <param name="clrType">The CLR scalar type.</param>
    /// <param name="configure">Optional callback to configure the added scalar type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddScalar(Type clrType, Action<ApiScalarTypeBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(clrType);

        var builder = _context.GetOrAddScalarTypeBuilder(clrType);

        configure?.Invoke(builder);
        return this;
    }

    /// <summary>
    ///     Adds a scalar type to the schema using an implementation of <see cref="IApiScalarTypeConfiguration"/>.
    /// </summary>
    /// <param name="clrType">The CLR scalar type.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddScalar(Type clrType, IApiScalarTypeConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = _context.GetOrAddScalarTypeBuilder(clrType);

        configuration.Configure(builder);
        return this;
    }

    #endregion

    #region AddRelationship Methods
    /// <summary>
    ///     Adds a one-to-one relationship to the schema using an inline configuration action.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddOneToOneRelationship(string apiName, Action<ApiRelationshipOneToOneBuilder> configure)
    {
        return this.AddOneToOneRelationshipCore
        (
            apiName,
            configure,
            _context.CurrentConfigurationSource
        );
    }

    /// <summary>
    ///     Adds a one-to-one relationship to the schema using an <see cref="IApiRelationshipOneToOneConfiguration"/>.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddOneToOneRelationship(string apiName, IApiRelationshipOneToOneConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return this.AddOneToOneRelationship
        (
            apiName,
            builder => configuration.Configure(builder)
        );
    }

    /// <summary>
    ///     Adds a one-to-many relationship to the schema using an inline configuration action.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddOneToManyRelationship(string apiName, Action<ApiRelationshipOneToManyBuilder> configure)
    {
        return this.AddOneToManyRelationshipCore
        (
            apiName,
            configure,
            _context.CurrentConfigurationSource
        );
    }

    /// <summary>
    ///     Adds a one-to-many relationship to the schema using an <see cref="IApiRelationshipOneToManyConfiguration"/>.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddOneToManyRelationship(string apiName, IApiRelationshipOneToManyConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return this.AddOneToManyRelationship
        (
            apiName,
            builder => configuration.Configure(builder)
        );
    }

    /// <summary>
    ///     Adds a many-to-many relationship to the schema using an inline configuration action.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddManyToManyRelationship(string apiName, Action<ApiRelationshipManyToManyBuilder> configure)
    {
        return this.AddManyToManyRelationshipCore
        (
            apiName,
            configure,
            _context.CurrentConfigurationSource
        );
    }

    /// <summary>
    ///     Adds a many-to-many relationship to the schema using an <see cref="IApiRelationshipManyToManyConfiguration"/>.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddManyToManyRelationship(string apiName, IApiRelationshipManyToManyConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return this.AddManyToManyRelationship
        (
            apiName,
            builder => configuration.Configure(builder)
        );
    }
    #endregion

    #region Internal Relationship Configuration Methods
    /// <summary>Adds or configures a one-to-one relationship at the supplied precedence.</summary>
    internal ApiSchemaBuilder AddOneToOneRelationshipCore
    (
        string apiName,
        Action<ApiRelationshipOneToOneBuilder> configure,
        ApiConfigurationSource source
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        var builder = _context.GetOrAddOneToOneRelationshipBuilder(apiName, source);
        if (builder != null)
        {
            _context.ApplyConfiguration
            (
                source,
                () => builder.ApplyConfiguration(source, () => configure(builder))
            );
        }

        return this;
    }

    /// <summary>Adds or configures a one-to-many relationship at the supplied precedence.</summary>
    internal ApiSchemaBuilder AddOneToManyRelationshipCore
    (
        string apiName,
        Action<ApiRelationshipOneToManyBuilder> configure,
        ApiConfigurationSource source
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        var builder = _context.GetOrAddOneToManyRelationshipBuilder(apiName, source);
        if (builder != null)
        {
            _context.ApplyConfiguration
            (
                source,
                () => builder.ApplyConfiguration(source, () => configure(builder))
            );
        }

        return this;
    }

    /// <summary>Adds or configures a many-to-many relationship at the supplied precedence.</summary>
    internal ApiSchemaBuilder AddManyToManyRelationshipCore
    (
        string apiName,
        Action<ApiRelationshipManyToManyBuilder> configure,
        ApiConfigurationSource source
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        var builder = _context.GetOrAddManyToManyRelationshipBuilder(apiName, source);
        if (builder != null)
        {
            _context.ApplyConfiguration
            (
                source,
                () => builder.ApplyConfiguration(source, () => configure(builder))
            );
        }

        return this;
    }

    /// <summary>Runs a relationship convention at convention precedence.</summary>
    internal void ApplyRelationshipConvention(IApiRelationshipConvention convention)
    {
        ArgumentNullException.ThrowIfNull(convention);

        _context.ApplyConfiguration
        (
            ApiConfigurationSource.Convention,
            () => convention.Apply(this)
        );
    }
    #endregion

    #region With Methods
    /// <summary>
    ///     Sets the name of the schema being built.
    /// </summary>
    /// <param name="apiName">The schema name.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder WithName(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        _apiName = apiName;
        return this;
    }

    /// <summary>
    ///     Sets the optional version string for the schema.
    /// </summary>
    /// <param name="apiVersion">The version identifier.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder WithVersion(string apiVersion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiVersion, nameof(apiVersion));

        _apiVersion = apiVersion;
        return this;
    }

    /// <summary>
    ///     Configures schema-wide options for the schema being built.
    /// </summary>
    /// <param name="configure">Callback to configure the <see cref="ApiSchemaOptionsBuilder"/>.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder WithOptions(Action<ApiSchemaOptionsBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        _apiOptionsConfiguration = configure;
        return this;
    }

    /// <summary>
    ///     Resets the schema options to their out-of-the-box defaults.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder WithDefaultOptions()
    {
        _apiOptionsConfiguration = null;
        return this;
    }
    #endregion

    #region UseConventions Methods
    /// <summary>
    ///     Configures the convention pipeline using a fluent <see cref="ApiConventionSetBuilder"/>.
    ///     Calling this method multiple times is additive — each call starts from the previously
    ///     configured set so conventions accumulate rather than being replaced.
    /// </summary>
    /// <param name="configure">Callback to configure the convention set builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder UseConventions(Action<ApiConventionSetBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var setBuilder = _conventionSet != null
            ? new ApiConventionSetBuilder(_conventionSet)
            : new ApiConventionSetBuilder();

        configure(setBuilder);
        _conventionSet = setBuilder.Build();
        return this;
    }

    /// <summary>
    ///     Applies <see cref="ApiConventionSet.CreateDefault"/> as the starting convention set.
    ///     May be combined with <see cref="UseConventions"/> to augment or remove individual
    ///     conventions.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder UseDefaultConventions()
    {
        _conventionSet = ApiConventionSet.CreateDefault();
        return this;
    }
    #endregion

    #region UseAnnotations Methods
    /// <summary>
    ///     Configures the annotation reader pipeline using a fluent <see cref="ApiAnnotationReaderSetBuilder"/>.
    /// </summary>
    /// <param name="configure">Callback to configure the annotation reader set builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder UseAnnotations(Action<ApiAnnotationReaderSetBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var setBuilder = new ApiAnnotationReaderSetBuilder();
        configure(setBuilder);
        _annotationReaderSet = setBuilder.Build();
        return this;
    }

    /// <summary>
    ///     Registers <see cref="ApiAttributeAnnotationReader"/> as the annotation reader,
    ///     enabling the framework's built-in attribute set (<see cref="Annotations.ApiObjectTypeAttribute"/>,
    ///     <see cref="Annotations.ApiPropertyAttribute"/>, <see cref="Annotations.ApiKeyAttribute"/>, etc.).
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder UseDefaultAnnotations()
    {
        return this.UseAnnotations(a => a.AddReader(new ApiAttributeAnnotationReader()));
    }
    #endregion

    #region AddTypes Methods
    /// <summary>
    ///     Registers one or more CLR types so that conventions and annotations can configure them
    ///     without any explicit fluent configuration.
    ///     Object types, scalar types, and enum types may all be mixed in the same call; the
    ///     method inspects each type and routes it to the correct <c>AddObject</c>, <c>AddScalar</c>,
    ///     or <c>AddEnum</c> overload.
    /// </summary>
    /// <param name="clrTypes">The CLR types to register.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddTypes(params Type[] clrTypes)
    {
        ArgumentNullException.ThrowIfNull(clrTypes);

        foreach (var clrType in clrTypes)
        {
            if (clrType == null)
            {
                continue;
            }

            if (clrType.IsEnum)
            {
                this.AddEnum(clrType);
            }
            else
            {
                this.AddObject(clrType);
            }
        }

        return this;
    }

    /// <summary>
    ///     Scans the specified assembly using the built-in assembly scanning convention and registers
    ///     all public non-abstract types annotated with
    ///     <see cref="Annotations.ApiObjectTypeAttribute"/>,
    ///     <see cref="Annotations.ApiScalarTypeAttribute"/>, or
    ///     <see cref="Annotations.ApiEnumTypeAttribute"/>.
    ///     Equivalent to the <see cref="ApiSchemaBuilderExtensions.UseAssemblyScanning"/> extension
    ///     method.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <param name="filter">Optional predicate to limit which types are considered.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder ScanAssembly
    (
        System.Reflection.Assembly assembly,
        Func<Type, bool>? filter = null
    )
    {
        return this.UseConventions(c =>
            c.AddConvention(new ApiSchemaAssemblyScanConvention(assembly, filter)));
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Constructs the <see cref="ApiSchema"/> using the configured components.
    /// </summary>
    /// <returns>The built <see cref="ApiSchema"/>.</returns>
    public ApiSchema Build()
    {
        if (_conventionSet is not null || _annotationReaderSet is not null)
        {
            var configurationPipeline = new ApiSchemaConfigurationPipeline(
                _conventionSet,
                _annotationReaderSet,
                _context,
                this);

            configurationPipeline.Run();
        }

        // Build ApiSchema instance from all the configured components.
        var apiName = _apiName!;
        var apiVersion = _apiVersion;
        var apiOptions = this.BuildOptions();

        var apiScalarTypes = _context.ApiScalarTypeBuilders.Select(b => b.Build());
        var apiEnumTypes = _context.ApiEnumTypeBuilders.Select(b => b.Build());
        var apiObjectTypes = _context.ApiObjectTypeBuilders.Select(b => b.Build());
        var apiRelationships = _context.ApiRelationshipBuilders.Select(b => b.Build());

        var apiSchema = new ApiSchema
        (
            apiName,
            apiVersion,
            apiOptions,
            apiScalarTypes,
            apiEnumTypes,
            apiObjectTypes,
            apiRelationships
        );

        // Add any extensions that were configured.
        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiSchema.Extensions = extensions;
        }

        // Initialize the ApiSchema instance.
        var result = apiSchema.Initialize();
        result.ThrowIfInvalid();

        return apiSchema;
    }

    private ApiSchemaOptions? BuildOptions()
    {
        if (_apiOptionsConfiguration == null)
        {
            return null;
        }

        var apiOptionsBuilder = new ApiSchemaOptionsBuilder();
        _apiOptionsConfiguration.Invoke(apiOptionsBuilder);
        return apiOptionsBuilder.Build();
    }
    #endregion

    #region Implementation Methods
    /// <summary>
    ///     Allows extension methods to configure a strongly-typed enum builder.
    /// </summary>
    internal ApiSchemaBuilder AddEnumCore<TEnum>(Action<ApiEnumTypeBuilder<TEnum>>? configure = null)
        where TEnum : Enum
    {
        var builder = _context.GetOrAddEnumTypeBuilder<TEnum>();
        configure?.Invoke(builder);
        return this;
    }

    /// <summary>
    ///     Allows extension methods to configure a strongly-typed object builder.
    /// </summary>
    internal ApiSchemaBuilder AddObjectCore<TObject>(Action<ApiObjectTypeBuilder<TObject>>? configure = null)
    {
        var builder = _context.GetOrAddObjectTypeBuilder<TObject>();
        configure?.Invoke(builder);
        return this;
    }

    /// <summary>
    ///     Allows extension methods to configure a strongly-typed scalar builder.
    /// </summary>
    internal ApiSchemaBuilder AddScalarCore<TScalar>(Action<ApiScalarTypeBuilder<TScalar>>? configure = null)
    {
        var builder = _context.GetOrAddScalarTypeBuilder<TScalar>();
        configure?.Invoke(builder);
        return this;
    }
    #endregion
}
