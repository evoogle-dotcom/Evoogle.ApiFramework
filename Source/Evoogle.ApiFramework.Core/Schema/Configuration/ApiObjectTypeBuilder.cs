// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiObjectType"/>.
/// </summary>
/// <param name="clrType">The CLR type represented by the API object type.</param>
/// <param name="context">The shared builder context.</param>
public class ApiObjectTypeBuilder(Type clrType, ApiSchemaBuilderContext context)
    : ApiNamedTypeBuilder<ApiObjectTypeBuilder>(clrType, context)
{
    #region Fields
    private readonly List<ApiIdentityBuilder> _apiIdentityBuilders = [];
    private readonly List<ApiPropertyBuilder> _apiPropertyBuilders = [];
    private Action<ApiObjectTypeOptionsBuilder>? _apiOptionsConfiguration = null;
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddObjectExtension(Type type, object value)
    {
        base.AddExtension(type, value);
        return this;
    }

    /// <summary>
    ///     Adds an extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="T">The extension value type.</typeparam>
    /// <param name="value">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddObjectExtension<T>(T value) where T : notnull
        => this.AddObjectExtension(typeof(T), value);

    /// <summary>
    ///     Adds an <see cref="ApiIdentity"/> definition to the object type.
    /// </summary>
    /// <remarks>
    ///     The first identity added is the primary identity by convention unless specified otherwise.
    /// </remarks>
    /// <param name="apiName">The API name of the identity.</param>
    /// <param name="configure">Optional callback to configure the added identity.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddIdentity(string apiName, Action<ApiIdentityBuilder>? configure = null)
    {
        var apiIdentityBuilder = new ApiIdentityBuilder(apiName);

        configure?.Invoke(apiIdentityBuilder);

        _apiIdentityBuilders.Add(apiIdentityBuilder);

        return this;
    }

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition to the object type, using <paramref name="name"/> as both
    ///     the API name and the CLR property name.
    /// </summary>
    /// <param name="name">The API and CLR property name.</param>
    /// <param name="configure">Optional callback to configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddProperty(string name, Action<ApiPropertyBuilder>? configure = null)
    {
        return this.AddProperty(name, name, configure);
    }

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition to the object type.
    /// </summary>
    /// <param name="apiName">The API property name.</param>
    /// <param name="clrName">The CLR property name.</param>
    /// <param name="configure">Optional callback to configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddProperty(string apiName, string clrName, Action<ApiPropertyBuilder>? configure = null)
    {
        var apiPropertyBuilder = new ApiPropertyBuilder(apiName, clrName);

        configure?.Invoke(apiPropertyBuilder);

        _apiPropertyBuilders.Add(apiPropertyBuilder);

        return this;
    }

    /// <summary>
    ///     Resets the object type options to their schema-wide defaults.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder WithDefaultOptions()
    {
        _apiOptionsConfiguration = null;
        return this;
    }

    /// <summary>
    ///     Configures type-specific options for this object type.
    /// </summary>
    /// <param name="configure">Callback to configure the <see cref="ApiObjectTypeOptionsBuilder"/>.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder WithOptions(Action<ApiObjectTypeOptionsBuilder> configure)
    {
        _apiOptionsConfiguration = configure;
        return this;
    }

    #endregion

    #region AddRelationship Methods
    /// <summary>
    ///     Registers a one-to-one relationship at the schema level while authoring this object type.
    /// </summary>
    /// <remarks>
    ///     This is a convenience entry point equivalent to calling
    ///     <see cref="ApiSchemaBuilder.AddOneToOneRelationship(string, Action{ApiRelationshipOneToOneBuilder})"/>
    ///     directly on the schema builder.  The resulting <see cref="ApiRelationship"/> is owned by the
    ///     schema, not by this object type.
    /// </remarks>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddOneToOneRelationship(string apiName, Action<ApiRelationshipOneToOneBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        var builder = this.Context.GetOrAddOneToOneRelationshipBuilder(apiName);
        configure(builder);
        return this;
    }

    /// <summary>
    ///     Registers a one-to-one relationship at the schema level while authoring this object type,
    ///     using an <see cref="IApiRelationshipOneToOneConfiguration"/> implementation.
    /// </summary>
    /// <remarks>
    ///     This is a convenience entry point equivalent to calling
    ///     <see cref="ApiSchemaBuilder.AddOneToOneRelationship(string, IApiRelationshipOneToOneConfiguration)"/>
    ///     directly on the schema builder.  The resulting <see cref="ApiRelationship"/> is owned by the
    ///     schema, not by this object type.
    /// </remarks>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddOneToOneRelationship(string apiName, IApiRelationshipOneToOneConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = this.Context.GetOrAddOneToOneRelationshipBuilder(apiName);
        configuration.Configure(builder);
        return this;
    }

    /// <summary>
    ///     Registers a one-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <remarks>
    ///     This is a convenience entry point equivalent to calling
    ///     <see cref="ApiSchemaBuilder.AddOneToManyRelationship(string, Action{ApiRelationshipOneToManyBuilder})"/>
    ///     directly on the schema builder.  The resulting <see cref="ApiRelationship"/> is owned by the
    ///     schema, not by this object type.
    /// </remarks>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddOneToManyRelationship(string apiName, Action<ApiRelationshipOneToManyBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        var builder = this.Context.GetOrAddOneToManyRelationshipBuilder(apiName);
        configure(builder);
        return this;
    }

    /// <summary>
    ///     Registers a one-to-many relationship at the schema level while authoring this object type,
    ///     using an <see cref="IApiRelationshipOneToManyConfiguration"/> implementation.
    /// </summary>
    /// <remarks>
    ///     This is a convenience entry point equivalent to calling
    ///     <see cref="ApiSchemaBuilder.AddOneToManyRelationship(string, IApiRelationshipOneToManyConfiguration)"/>
    ///     directly on the schema builder.  The resulting <see cref="ApiRelationship"/> is owned by the
    ///     schema, not by this object type.
    /// </remarks>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddOneToManyRelationship(string apiName, IApiRelationshipOneToManyConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = this.Context.GetOrAddOneToManyRelationshipBuilder(apiName);
        configuration.Configure(builder);
        return this;
    }

    /// <summary>
    ///     Registers a many-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <remarks>
    ///     This is a convenience entry point equivalent to calling
    ///     <see cref="ApiSchemaBuilder.AddManyToManyRelationship(string, Action{ApiRelationshipManyToManyBuilder})"/>
    ///     directly on the schema builder.  The resulting <see cref="ApiRelationship"/> is owned by the
    ///     schema, not by this object type.
    /// </remarks>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddManyToManyRelationship(string apiName, Action<ApiRelationshipManyToManyBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        var builder = this.Context.GetOrAddManyToManyRelationshipBuilder(apiName);
        configure(builder);
        return this;
    }

    /// <summary>
    ///     Registers a many-to-many relationship at the schema level while authoring this object type,
    ///     using an <see cref="IApiRelationshipManyToManyConfiguration"/> implementation.
    /// </summary>
    /// <remarks>
    ///     This is a convenience entry point equivalent to calling
    ///     <see cref="ApiSchemaBuilder.AddManyToManyRelationship(string, IApiRelationshipManyToManyConfiguration)"/>
    ///     directly on the schema builder.  The resulting <see cref="ApiRelationship"/> is owned by the
    ///     schema, not by this object type.
    /// </remarks>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddManyToManyRelationship(string apiName, IApiRelationshipManyToManyConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = this.Context.GetOrAddManyToManyRelationshipBuilder(apiName);
        configuration.Configure(builder);
        return this;
    }

    /// <summary>
    ///     Registers a many-to-many relationship at the schema level while authoring this object type,
    ///     with the association CLR type <typeparamref name="TAssociation"/> fixed for the builder.
    /// </summary>
    /// <remarks>
    ///     This is a convenience entry point equivalent to calling
    ///     <see cref="ApiSchemaBuilder.AddManyToManyRelationship{TAssociation}(string, Action{ApiRelationshipManyToManyBuilder{TAssociation}})"/>
    ///     directly on the schema builder.  The resulting <see cref="ApiRelationship"/> is owned by the
    ///     schema, not by this object type.
    /// </remarks>
    /// <typeparam name="TAssociation">The CLR type of the association object type that mediates the relationship.</typeparam>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddManyToManyRelationship<TAssociation>(string apiName, Action<ApiRelationshipManyToManyBuilder<TAssociation>> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        var builder = this.Context.GetOrAddManyToManyRelationshipBuilder<TAssociation>(apiName);
        configure(builder);
        return this;
    }

    /// <summary>
    ///     Registers a many-to-many relationship at the schema level while authoring this object type,
    ///     using a strongly-typed <see cref="IApiRelationshipManyToManyConfiguration{TAssociation}"/>.
    /// </summary>
    /// <remarks>
    ///     This is a convenience entry point equivalent to calling
    ///     <see cref="ApiSchemaBuilder.AddManyToManyRelationship{TAssociation}(string, IApiRelationshipManyToManyConfiguration{TAssociation})"/>
    ///     directly on the schema builder.  The resulting <see cref="ApiRelationship"/> is owned by the
    ///     schema, not by this object type.
    /// </remarks>
    /// <typeparam name="TAssociation">The CLR type of the association object type that mediates the relationship.</typeparam>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The typed configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddManyToManyRelationship<TAssociation>(string apiName, IApiRelationshipManyToManyConfiguration<TAssociation> configuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = this.Context.GetOrAddManyToManyRelationshipBuilder<TAssociation>(apiName);
        configuration.Configure(builder);
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="ApiObjectType"/> using the configured properties and relationships.
    /// </summary>
    /// <returns>The constructed <see cref="ApiObjectType"/>.</returns>
    internal ApiObjectType Build()
    {
        // Build ApiObjectType instance from all the configured components.
        var apiName = this.ApiName;
        var clrObjectType = this.ClrType;

        var apiOptions = this.BuildOptions();

        var apiIdentities = _apiIdentityBuilders
            .Select(b => b.Build());

        var apiProperties = _apiPropertyBuilders
            .Select(b => b.Build(clrObjectType));

        var apiObjectType = new ApiObjectType
        (
            apiName,
            apiOptions,
            apiIdentities,
            apiProperties,
            clrObjectType
        );

        // Add any extensions that were configured.
        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiObjectType.Extensions = extensions;
        }

        return apiObjectType;
    }
    #endregion

    #region Implementation Methods
    /// <summary>
    ///     Adds a pre-constructed identity builder to this object type, allowing subclasses to inject
    ///     typed builders without bypassing internal list management.
    /// </summary>
    protected void AddIdentityBuilderCore(ApiIdentityBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _apiIdentityBuilders.Add(builder);
    }

    private ApiObjectTypeOptions? BuildOptions()
    {
        if (_apiOptionsConfiguration == null)
        {
            return null;
        }

        var apiOptionsBuilder = new ApiObjectTypeOptionsBuilder();
        _apiOptionsConfiguration.Invoke(apiOptionsBuilder);
        return apiOptionsBuilder.Build();
    }
    #endregion
}
