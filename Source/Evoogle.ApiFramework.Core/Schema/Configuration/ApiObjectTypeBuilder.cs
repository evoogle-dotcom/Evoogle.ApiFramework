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
    private readonly List<ApiKeyTypeBuilder> _apiKeyTypeBuilders = [];
    private readonly List<ApiPropertyBuilder> _apiPropertyBuilders = [];
    private Action<ApiObjectTypeOptionsBuilder>? _apiOptionsConfiguration = null;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddObjectTypeExtension(Type type, object value)
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
    public ApiObjectTypeBuilder AddObjectTypeExtension<T>(T value) where T : notnull
        => this.AddObjectTypeExtension(typeof(T), value);
    #endregion

    #region AddKeyType Methods
    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition to the object type.
    /// </summary>
    /// <remarks>
    ///     The first key type added is the primary key type by convention.
    /// </remarks>
    /// <param name="apiName">The API name of the key type.</param>
    /// <param name="configure">Optional callback to configure the added key type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddKeyType(string apiName, Action<ApiKeyTypeBuilder>? configure = null)
    {
        var apiKeyTypeBuilder = new ApiKeyTypeBuilder(apiName);

        configure?.Invoke(apiKeyTypeBuilder);

        _apiKeyTypeBuilders.Add(apiKeyTypeBuilder);

        return this;
    }
    #endregion

    #region AddProperty Methods
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

    #endregion

    #region AddRequiredProperty Methods
    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition to the object type marked as required,
    ///     using <paramref name="name"/> as both the API name and the CLR property name.
    /// </summary>
    /// <param name="name">The API and CLR property name.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddRequiredProperty(string name, Action<ApiPropertyBuilder>? configure = null)
    {
        return this.AddProperty(name, name, b => { b.AsRequired(); configure?.Invoke(b); });
    }

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition to the object type marked as required.
    /// </summary>
    /// <param name="apiName">The API property name.</param>
    /// <param name="clrName">The CLR property name.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddRequiredProperty(string apiName, string clrName, Action<ApiPropertyBuilder>? configure = null)
    {
        return this.AddProperty(apiName, clrName, b => { b.AsRequired(); configure?.Invoke(b); });
    }
    #endregion

    #region AddOptionalProperty Methods
    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition to the object type marked as optional,
    ///     using <paramref name="name"/> as both the API name and the CLR property name.
    /// </summary>
    /// <param name="name">The API and CLR property name.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddOptionalProperty(string name, Action<ApiPropertyBuilder>? configure = null)
    {
        return this.AddProperty(name, name, b => { b.AsOptional(); configure?.Invoke(b); });
    }

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition to the object type marked as optional.
    /// </summary>
    /// <param name="apiName">The API property name.</param>
    /// <param name="clrName">The CLR property name.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddOptionalProperty(string apiName, string clrName, Action<ApiPropertyBuilder>? configure = null)
    {
        return this.AddProperty(apiName, clrName, b => { b.AsOptional(); configure?.Invoke(b); });
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
    #endregion

    #region With Methods
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

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiObjectType"/> using the configured properties and identity.
    /// </summary>
    /// <returns>The constructed <see cref="ApiObjectType"/>.</returns>
    internal ApiObjectType Build()
    {
        // Build ApiObjectType instance from all the configured components.
        var apiName = this.ApiName;
        var clrObjectType = this.ClrType;

        var apiOptions = this.BuildOptions();

        var apiProperties = _apiPropertyBuilders
            .Select(b => b.Build(clrObjectType));

        var apiKeyTypes = _apiKeyTypeBuilders.Count > 0
            ? _apiKeyTypeBuilders.Select(b => b.Build())
            : null;

        var apiObjectType = new ApiObjectType
        (
            apiName,
            apiOptions,
            apiProperties,
            apiKeyTypes,
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

    #region Implementation Methods
    /// <summary>
    ///     Allows subclasses to add a pre-constructed key type builder without bypassing internal list management.
    /// </summary>
    protected void AddKeyTypeBuilderCore(ApiKeyTypeBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _apiKeyTypeBuilders.Add(builder);
    }
    #endregion
}
