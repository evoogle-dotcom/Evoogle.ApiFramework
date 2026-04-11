// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Microsoft.Extensions.Logging;

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

    private readonly ApiSchemaBuilderContext _context = new(logger);
    #endregion

    #region Builder Methods
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

    /// <summary>
    ///     Adds an enumeration type to the schema using an inline configuration action.
    /// </summary>
    /// <param name="clrType">The CLR enum type.</param>
    /// <param name="configure">Callback to configure the added enumeration type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddEnum(Type clrType, Action<ApiEnumTypeBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(configure);

        var builder = _context.GetOrAddEnumTypeBuilder(clrType);

        configure(builder);
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

    /// <summary>
    ///     Adds an object type to the schema using an inline configuration action.
    /// </summary>
    /// <param name="clrType">The CLR object type.</param>
    /// <param name="configure">Callback to configure the added object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddObject(Type clrType, Action<ApiObjectTypeBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(configure);

        var builder = _context.GetOrAddObjectTypeBuilder(clrType);

        configure(builder);
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

    /// <summary>
    ///     Adds an object type to the schema using a strongly-typed inline configuration action.
    /// </summary>
    /// <typeparam name="T">The CLR object type.</typeparam>
    /// <param name="configure">Callback to configure the added object type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddObject<T>(Action<ApiObjectTypeBuilder<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var builder = _context.GetOrAddObjectTypeBuilder<T>();

        configure(builder);
        return this;
    }

    /// <summary>
    ///     Adds an object type to the schema using a strongly-typed <see cref="IApiObjectTypeConfiguration{T}"/>.
    /// </summary>
    /// <typeparam name="T">The CLR object type.</typeparam>
    /// <param name="configuration">The typed configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddObject<T>(IApiObjectTypeConfiguration<T> configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = _context.GetOrAddObjectTypeBuilder<T>();

        configuration.Configure(builder);
        return this;
    }

    /// <summary>
    ///     Adds a scalar type to the schema using an inline configuration action.
    /// </summary>
    /// <param name="clrType">The CLR scalar type.</param>
    /// <param name="configure">Callback to configure the added scalar type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddScalar(Type clrType, Action<ApiScalarTypeBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(configure);

        var builder = _context.GetOrAddScalarTypeBuilder(clrType);

        configure(builder);
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

    /// <summary>
    ///     Adds a one-to-one relationship to the schema using an inline configuration action.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddOneToOneRelationship(string apiName, Action<ApiRelationshipOneToOneBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        var builder = _context.GetOrAddOneToOneRelationshipBuilder(apiName);

        configure(builder);
        return this;
    }

    /// <summary>
    ///     Adds a one-to-one relationship to the schema using an <see cref="IApiRelationshipOneToOneConfiguration"/>.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddOneToOneRelationship(string apiName, IApiRelationshipOneToOneConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = _context.GetOrAddOneToOneRelationshipBuilder(apiName);

        configuration.Configure(builder);
        return this;
    }

    /// <summary>
    ///     Adds a one-to-many relationship to the schema using an inline configuration action.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddOneToManyRelationship(string apiName, Action<ApiRelationshipOneToManyBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        var builder = _context.GetOrAddOneToManyRelationshipBuilder(apiName);

        configure(builder);
        return this;
    }

    /// <summary>
    ///     Adds a one-to-many relationship to the schema using an <see cref="IApiRelationshipOneToManyConfiguration"/>.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddOneToManyRelationship(string apiName, IApiRelationshipOneToManyConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = _context.GetOrAddOneToManyRelationshipBuilder(apiName);

        configuration.Configure(builder);
        return this;
    }

    /// <summary>
    ///     Adds a many-to-many relationship to the schema using an inline configuration action.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddManyToManyRelationship(string apiName, Action<ApiRelationshipManyToManyBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        var builder = _context.GetOrAddManyToManyRelationshipBuilder(apiName);

        configure(builder);
        return this;
    }

    /// <summary>
    ///     Adds a many-to-many relationship to the schema using an <see cref="IApiRelationshipManyToManyConfiguration"/>.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiSchemaBuilder AddManyToManyRelationship(string apiName, IApiRelationshipManyToManyConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = _context.GetOrAddManyToManyRelationshipBuilder(apiName);

        configuration.Configure(builder);
        return this;
    }

    /// <summary>
    ///     Constructs the <see cref="ApiSchema"/> using the configured components.
    /// </summary>
    /// <returns>The built <see cref="ApiSchema"/>.</returns>
    public ApiSchema Build()
    {
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
    #endregion

    #region Implementation Methods
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
}
