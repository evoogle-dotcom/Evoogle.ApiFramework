// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Trace;

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
    private readonly ApiObjectTypeState _state = new();
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

        var apiKeyTypeBuilder = this.GetOrAddKeyTypeBuilder(apiName);

        configure?.Invoke(apiKeyTypeBuilder);
        this.Context.TraceStructuralRegistration
        (
            new(ApiSchemaBuildTargetKind.KeyType, this.ClrType, ApiName: apiName),
            ApiSchemaBuildRegistrationKind.KeyType,
            this.Context.CurrentConfigurationSource,
            wasRegistered: true
        );

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
        _state.OptionsConfiguration = null;
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

        _state.OptionsConfiguration = configure;
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

        var apiProperties = _state.PropertyBuilders
            .Select(b => b.Build(clrObjectType));

        var apiKeyTypes = _state.KeyTypeBuilders.Count > 0
            ? _state.KeyTypeBuilders.Select(b => b.Build())
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
        if (_state.OptionsConfiguration == null)
        {
            return null;
        }

        var apiOptionsBuilder = new ApiObjectTypeOptionsBuilder();
        _state.OptionsConfiguration.Invoke(apiOptionsBuilder);
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
        _state.KeyTypeBuilders.Add(builder);
    }

    /// <summary>Gets an existing named key builder or creates its canonical closed-generic instance.</summary>
    protected ApiKeyTypeBuilder GetOrAddKeyTypeBuilder(string apiName)
    {
        var existing = _state.KeyTypeBuilders.FirstOrDefault(builder => builder.ApiName == apiName);
        if (existing != null)
        {
            return existing;
        }

        var builder = ApiBuilderFactory.CreateClosedGeneric<ApiKeyTypeBuilder>
        (
            typeof(ApiKeyTypeBuilder<>),
            this.ClrType,
            apiName
        );
        _state.KeyTypeBuilders.Add(builder);
        return builder;
    }

    /// <summary>
    ///     Finds an existing key type builder with the given API name and appends the specified path
    ///     to it, or creates a new key type builder with that path when no matching key exists.
    ///     Used by annotation readers to accumulate composite key paths from multiple
    ///     <see cref="Annotations.ApiKeyAttribute"/> declarations.
    /// </summary>
    internal void AddKeyOrAppendPath
    (
        string apiKeyName,
        Type clrRootType,
        IEnumerable<string> clrPropertyNames
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKeyName, nameof(apiKeyName));
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(clrPropertyNames);

        var names = clrPropertyNames as IReadOnlyList<string> ?? [.. clrPropertyNames];
        var existing = _state.KeyTypeBuilders.FirstOrDefault(b => b.ApiName == apiKeyName);
        if (existing != null)
        {
            // Guard against convention + annotation both adding the same path.
            if (!existing.HasPath(clrRootType, names))
            {
                existing.AddPath(clrRootType, names);
            }
        }
        else
        {
            var builder = this.GetOrAddKeyTypeBuilder(apiKeyName);
            builder.AddPath(clrRootType, names);
        }
    }

    /// <summary>Gets all <see cref="ApiPropertyBuilder"/> instances currently on this object type builder.</summary>
    internal IEnumerable<ApiPropertyBuilder> ApiPropertyBuilders => _state.PropertyBuilders;

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

        if (_state.PropertyBuilders.Any(b => b.ClrName == clrName))
        {
            this.Context.TraceStructuralRegistration
            (
                new(ApiSchemaBuildTargetKind.Property, this.ClrType, clrName),
                ApiSchemaBuildRegistrationKind.Property,
                this.Context.CurrentConfigurationSource,
                wasRegistered: false,
                rejectionReason: "A property with the CLR name was already registered."
            );
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

        var builder = new ApiPropertyBuilder
        (
            apiName,
            clrName,
            apiNameSource,
            this.Context,
            this.ClrType
        );
        configure?.Invoke(builder);
        _state.PropertyBuilders.Add(builder);
        this.Context.TraceStructuralRegistration
        (
            new(ApiSchemaBuildTargetKind.Property, this.ClrType, clrName, apiName),
            ApiSchemaBuildRegistrationKind.Property,
            apiNameSource,
            wasRegistered: true
        );
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

        if (_state.KeyTypeBuilders.Any(b => b.ApiName == apiKeyName))
        {
            return false;
        }

        var builder = this.GetOrAddKeyTypeBuilder(apiKeyName);
        configure?.Invoke(builder);
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
