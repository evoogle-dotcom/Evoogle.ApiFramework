// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;

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
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddObjectTypeExtension(Type extensionType, object extension)
    {
        return this.AddExtension(extensionType, extension);
    }
    #endregion

    #region AddKey Methods
    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition to the object type.
    /// </summary>
    /// <remarks>
    ///     Key-bound relationship principal ends infer the best compatible key from the corresponding foreign key
    ///     when no key name is supplied; call
    ///     <see cref="ApiRelationshipPrincipalEndBuilder.WithPrincipalKey"/> on the principal end builder to
    ///     select a named key explicitly.
    /// </remarks>
    /// <param name="apiName">The API name of the key type.</param>
    /// <param name="configure">Optional callback to configure the added key type.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddKey(string apiName, Action<ApiKeyTypeBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        var apiKeyTypeBuilder = new ApiKeyTypeBuilder(apiName);

        configure?.Invoke(apiKeyTypeBuilder);

        _apiKeyTypeBuilders.Add(apiKeyTypeBuilder);

        return this;
    }
    #endregion

    #region AddProperty Methods
    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition to the object type using an explicitly
    ///     supplied API name.
    /// </summary>
    /// <param name="apiName">The explicit API property name.</param>
    /// <param name="clrName">The CLR property name.</param>
    /// <param name="configure">Optional callback to configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddProperty(string apiName, string clrName, Action<ApiPropertyBuilder>? configure = null)
    {
        this.AddPropertyCore(apiName, clrName, ApiConfigurationSource.Explicit, configure);
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
        ArgumentNullException.ThrowIfNull(configure);

        _apiOptionsConfiguration = configure;
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiObjectType"/> using the configured properties and key types.
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

    /// <summary>
    ///     Finds an existing key type builder with the given API name and appends the specified path
    ///     to it, or creates a new key type builder with that path when no matching key exists.
    ///     Used by annotation readers to accumulate composite key paths from multiple
    ///     <see cref="Annotations.ApiKeyAttribute"/> declarations.
    /// </summary>
    internal void AddKeyOrAppendPath(string apiKeyName, Type clrRootType, string clrPropertyName)
    {
        var existing = _apiKeyTypeBuilders.FirstOrDefault(b => b.ApiName == apiKeyName);
        if (existing != null)
        {
            // Guard against convention + annotation both adding the same single-segment path.
            if (!existing.HasSimplePath(clrRootType, clrPropertyName))
            {
                existing.AddPath(clrRootType, clrPropertyName);
            }
        }
        else
        {
            var builder = new ApiKeyTypeBuilder(apiKeyName);
            builder.AddPath(clrRootType, clrPropertyName);
            _apiKeyTypeBuilders.Add(builder);
        }
    }

    /// <summary>Gets all <see cref="ApiPropertyBuilder"/> instances currently on this object type builder.</summary>
    internal IEnumerable<ApiPropertyBuilder> ApiPropertyBuilders => _apiPropertyBuilders;

    /// <summary>
    ///     Explicitly adds the CLR member while initializing its API name from the CLR name at
    ///     <see cref="ApiConfigurationSource.Convention"/> precedence.
    /// </summary>
    /// <param name="clrName">
    ///     The CLR property or field name to add and use as the candidate API name.
    /// </param>
    /// <param name="configure">Optional callback to configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    internal ApiObjectTypeBuilder AddPropertyWithInferredName
    (
        string clrName,
        Action<ApiPropertyBuilder>? configure = null
    )
    {
        this.AddPropertyCore
        (
            clrName,
            clrName,
            ApiConfigurationSource.Convention,
            configure
        );

        return this;
    }

    /// <summary>
    ///     Adds a property builder for the given CLR member name only when no existing builder
    ///     already targets that CLR name. The new builder is initialized at
    ///     <see cref="ApiConfigurationSource.Convention"/> precedence; its API name defaults
    ///     to the CLR name and can be overridden by a later naming convention.
    /// </summary>
    /// <param name="clrName">The CLR property or field name to add.</param>
    /// <returns>
    ///     The newly created <see cref="ApiPropertyBuilder"/>, or <c>null</c> if a builder
    ///     for that CLR name was already present.
    /// </returns>
    internal ApiPropertyBuilder? AddPropertyIfAbsent(string clrName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrName, nameof(clrName));

        if (_apiPropertyBuilders.Any(b => b.ClrName == clrName))
        {
            return null;
        }

        return this.AddPropertyCore
        (
            clrName,
            clrName,
            ApiConfigurationSource.Convention,
            configure: null
        );
    }

    private ApiPropertyBuilder AddPropertyCore
    (
        string apiName,
        string clrName,
        ApiConfigurationSource apiNameSource,
        Action<ApiPropertyBuilder>? configure
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentException.ThrowIfNullOrWhiteSpace(clrName, nameof(clrName));

        var builder = new ApiPropertyBuilder(apiName, clrName, apiNameSource);
        configure?.Invoke(builder);
        _apiPropertyBuilders.Add(builder);
        return builder;
    }

    /// <summary>
    ///     Adds a key type builder with the given API name only when no existing builder
    ///     with that API name is already present.
    /// </summary>
    /// <param name="apiKeyName">The API name of the key type to add.</param>
    /// <param name="configure">Optional callback to configure the new key type builder.</param>
    /// <returns>
    ///     <c>true</c> if the key type was added; <c>false</c> if a key type with that name already existed.
    /// </returns>
    internal bool AddKeyIfAbsent(string apiKeyName, Action<ApiKeyTypeBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKeyName, nameof(apiKeyName));

        if (_apiKeyTypeBuilders.Any(b => b.ApiName == apiKeyName))
        {
            return false;
        }

        var builder = new ApiKeyTypeBuilder(apiKeyName);
        configure?.Invoke(builder);
        _apiKeyTypeBuilders.Add(builder);
        return true;
    }

    /// <summary>Configures a schema-level one-to-one relationship at the active source.</summary>
    internal void AddOneToOneRelationshipCore
    (
        string apiName,
        Action<ApiRelationshipOneToOneBuilder> configure
    )
    {
        var source = this.Context.CurrentConfigurationSource;
        var builder = this.Context.GetOrAddOneToOneRelationshipBuilder(apiName, source);

        if (builder != null)
        {
            builder.ApplyConfiguration(source, () => configure(builder));
        }
    }

    /// <summary>Configures a schema-level one-to-many relationship at the active source.</summary>
    internal void AddOneToManyRelationshipCore
    (
        string apiName,
        Action<ApiRelationshipOneToManyBuilder> configure
    )
    {
        var source = this.Context.CurrentConfigurationSource;
        var builder = this.Context.GetOrAddOneToManyRelationshipBuilder(apiName, source);

        if (builder != null)
        {
            builder.ApplyConfiguration(source, () => configure(builder));
        }
    }

    /// <summary>Configures a schema-level many-to-many relationship at the active source.</summary>
    internal void AddManyToManyRelationshipCore
    (
        string apiName,
        Action<ApiRelationshipManyToManyBuilder> configure
    )
    {
        var source = this.Context.CurrentConfigurationSource;
        var builder = this.Context.GetOrAddManyToManyRelationshipBuilder(apiName, source);

        if (builder != null)
        {
            builder.ApplyConfiguration(source, () => configure(builder));
        }
    }
    #endregion
}
