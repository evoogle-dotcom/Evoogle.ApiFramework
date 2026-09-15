// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Annotations;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Key;
using Evoogle.ApiFramework.Schema.Configuration.Relationships;
using Evoogle.ApiFramework.Schema.Configuration.Types.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Version;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Types;

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
    ///     Adds an <see cref="ApiNamedKeyDefinition"/> to the object type.
    /// </summary>
    /// <remarks>
    ///     Key-bound relationship principal ends infer the best compatible key from the corresponding foreign key
    ///     when no key name is supplied; call
    ///     <see cref="ApiRelationshipPrincipalEndBuilder.WithPrincipalKey"/> on the principal end builder to
    ///     select a named key explicitly.
    /// </remarks>
    /// <param name="apiName">The API name of the key.</param>
    /// <param name="configure">Optional callback to configure the added key.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddKey(string apiName, Action<ApiKeyDefinitionBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        var apiKeyDefinitionBuilder = this.GetOrAddKeyBuilder(apiName);

        configure?.Invoke(apiKeyDefinitionBuilder);

        return this;
    }
    #endregion

    #region AddProperty Methods
    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition to the object type using an explicitly
    ///     supplied API name.
    /// </summary>
    /// <param name="apiName">The explicit API property name.</param>
    /// <param name="clrName">The CLR member name.</param>
    /// <param name="configure">Optional callback to configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddProperty(string apiName, string clrName, Action<ApiPropertyBuilder>? configure = null)
    {
        this.AddPropertyCore(apiName, clrName, this.Context.CurrentConfigurationSource, configure);
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

    /// <summary>Configures a property-backed version using a CLR member name.</summary>
    /// <param name="clrMemberName">The CLR member exposed as an API property.</param>
    /// <param name="configure">Optional version metadata configuration.</param>
    /// <returns>The current builder.</returns>
    public ApiObjectTypeBuilder WithVersion
    (
        string clrMemberName,
        Action<ApiVersionDefinitionBuilder>? configure = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrMemberName);

        this.ConfigureVersion
        (
            new ApiVersionDefinitionBuilder(clrMemberName),
            this.Context.CurrentConfigurationSource,
            configure
        );
        return this;
    }

    /// <summary>Configures a repository-backed version using an exact CLR type.</summary>
    /// <param name="clrVersionType">The exact CLR type of the version value.</param>
    /// <param name="configure">Optional version metadata configuration.</param>
    /// <returns>The current builder.</returns>
    public ApiObjectTypeBuilder WithRepositoryVersion
    (
        Type clrVersionType,
        Action<ApiVersionDefinitionBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(clrVersionType);

        this.ConfigureVersion
        (
            new ApiVersionDefinitionBuilder(clrVersionType),
            this.Context.CurrentConfigurationSource,
            configure
        );
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiObjectType"/> using the configured properties and named key
    ///     types.
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

        var apiKeys = _state.KeyBuilders.Count > 0
            ? _state.KeyBuilders.Select(b => b.BuildNamed())
            : null;

        var apiVersionDefinition = _state.VersionBuilder?.Build();

        var apiObjectType = new ApiObjectType
        (
            apiName,
            apiOptions,
            apiProperties,
            apiKeys,
            apiVersionDefinition,
            clrObjectType
        );

        // Add any extensions that were configured.
        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiObjectType.AttachExtensions(extensions);
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
    ///     Allows subclasses to add a pre-constructed key definition builder without bypassing internal list management.
    /// </summary>
    protected void AddKeyBuilderCore(ApiKeyDefinitionBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _state.KeyBuilders.Add(builder);
    }

    /// <summary>Gets an existing named key builder or creates its canonical closed-generic instance.</summary>
    protected ApiKeyDefinitionBuilder GetOrAddKeyBuilder(string apiName)
        => this.GetOrAddKeyBuilderCore(apiName, this.Context.CurrentConfigurationSource);

    internal ApiKeyDefinitionBuilder GetOrAddKeyBuilderAtSource
    (
        string apiName,
        ApiConfigurationSource source
    )
        => this.GetOrAddKeyBuilderCore(apiName, source);

    private ApiKeyDefinitionBuilder GetOrAddKeyBuilderCore
    (
        string apiName,
        ApiConfigurationSource registrationSource
    )
    {
        var existing = _state.KeyBuilders.FirstOrDefault(builder => builder.ApiName == apiName);
        if (existing != null)
        {
            return existing;
        }

        var builder = ApiBuilderFactory.CreateClosedGeneric<ApiKeyDefinitionBuilder>
        (
            typeof(ApiKeyDefinitionBuilder<>),
            this.ClrType,
            apiName
        );
        builder.SetRegistrationSource(registrationSource);
        _state.KeyBuilders.Add(builder);
        return builder;
    }

    internal bool HasExplicitKey(string apiKeyName)
    {
        return _state.KeyBuilders.Any
        (
            builder => builder.ApiName == apiKeyName &&
                builder.RegistrationSource == ApiConfigurationSource.Explicit
        );
    }

    internal void ReplaceKeyFromDataAnnotation
    (
        string apiKeyName,
        IEnumerable<(Type ClrRootType, IReadOnlyList<string> ClrMemberNames)> paths
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKeyName, nameof(apiKeyName));
        ArgumentNullException.ThrowIfNull(paths);

        var builder = this.GetOrAddKeyBuilderAtSource
        (
            apiKeyName,
            ApiConfigurationSource.DataAnnotation
        );
        if (builder.RegistrationSource > ApiConfigurationSource.DataAnnotation)
        {
            return;
        }

        builder.SetRegistrationSource(ApiConfigurationSource.DataAnnotation);
        builder.ClearPaths();

        foreach (var (clrRootType, clrMemberNames) in paths)
        {
            if (clrRootType == this.ClrType)
            {
                builder.AddPath(clrMemberNames);
            }
            else
            {
                builder.AddPath(clrRootType, clrMemberNames);
            }
        }
    }

    internal void ReplaceVersionFromDataAnnotation(ApiVersionAnnotationResult version)
    {
        ArgumentNullException.ThrowIfNull(version);

        this.ConfigureVersion
        (
            version.ClrMemberName is not null
                ? new ApiVersionDefinitionBuilder(version.ClrMemberName)
                : new ApiVersionDefinitionBuilder(version.ClrType!),
            ApiConfigurationSource.DataAnnotation,
            configure: null
        );
    }

    private void ConfigureVersion
    (
        ApiVersionDefinitionBuilder builder,
        ApiConfigurationSource configurationSource,
        Action<ApiVersionDefinitionBuilder>? configure
    )
    {
        if (_state.VersionConfigurationSource > configurationSource)
        {
            return;
        }

        configure?.Invoke(builder);

        _state.VersionBuilder = builder;
        _state.VersionConfigurationSource = configurationSource;
    }

    /// <summary>
    ///     Finds an existing key definition builder with the given API name and appends the specified path
    ///     to it, or creates a new key definition builder with that path when no matching key exists.
    ///     Used by annotation readers to accumulate composite key paths from multiple
    ///     <see cref="ApiKeyAttribute"/> declarations.
    /// </summary>
    internal void AddKeyOrAppendPath
    (
        string apiKeyName,
        Type clrRootType,
        IEnumerable<string> clrMemberNames
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKeyName, nameof(apiKeyName));
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(clrMemberNames);

        var names = clrMemberNames as IReadOnlyList<string> ?? [.. clrMemberNames];
        var existing = _state.KeyBuilders.FirstOrDefault(b => b.ApiName == apiKeyName);
        if (existing != null)
        {
            // Guard against convention + annotation both adding the same path.
            var apiRootTypeReference = clrRootType == this.ClrType
                ? null
                : new ApiTypeReference(clrRootType);
            if (!existing.HasPath(apiRootTypeReference, names))
            {
                if (apiRootTypeReference is null)
                {
                    existing.AddPath(names);
                }
                else
                {
                    existing.AddPath(apiRootTypeReference, names);
                }
            }
        }
        else
        {
            var builder = this.GetOrAddKeyBuilder(apiKeyName);
            if (clrRootType == this.ClrType)
            {
                builder.AddPath(names);
            }
            else
            {
                builder.AddPath(clrRootType, names);
            }
        }
    }

    /// <summary>Gets all <see cref="ApiPropertyBuilder"/> instances currently on this object type builder.</summary>
    internal IEnumerable<ApiPropertyBuilder> ApiPropertyBuilders => _state.PropertyBuilders;

    /// <summary>
    ///     Explicitly adds the CLR member while compiling its API name from the CLR name at
    ///     convention precedence.
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
    ///     already targets that CLR name. The new builder is compiled at
    ///     convention precedence; its API name defaults
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
            apiNameSource
        );
        configure?.Invoke(builder);
        _state.PropertyBuilders.Add(builder);
        return builder;
    }

    /// <summary>
    ///     Adds a key definition builder with the given API name only when no existing builder
    ///     with that API name is already present.
    /// </summary>
    /// <param name="apiKeyName">The API name of the key to add.</param>
    /// <param name="configure">Optional callback to configure the new key definition builder.</param>
    /// <returns>
    ///     <c>true</c> if the key was added; <c>false</c> if a key with that name already existed.
    /// </returns>
    internal bool AddKeyIfAbsent(string apiKeyName, Action<ApiKeyDefinitionBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKeyName, nameof(apiKeyName));

        if (_state.KeyBuilders.Any(b => b.ApiName == apiKeyName))
        {
            return false;
        }

        var builder = this.GetOrAddKeyBuilder(apiKeyName);
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
