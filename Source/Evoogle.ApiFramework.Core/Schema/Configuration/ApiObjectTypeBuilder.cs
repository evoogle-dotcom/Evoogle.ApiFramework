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
    ///     Adds an <see cref="ApiProperty"/> definition to the object type.
    /// </summary>
    /// <param name="apiName">The API property name.</param>
    /// <param name="clrName">The CLR property name.</param>
    /// <param name="configure">Optional callback to configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddProperty(string apiName, string clrName, Action<ApiPropertyBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentException.ThrowIfNullOrWhiteSpace(clrName, nameof(clrName));

        var apiPropertyBuilder = new ApiPropertyBuilder(apiName, clrName);

        configure?.Invoke(apiPropertyBuilder);

        _apiPropertyBuilders.Add(apiPropertyBuilder);

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

    /// <summary>Gets or creates a schema-level one-to-one relationship builder.</summary>
    internal ApiRelationshipOneToOneBuilder GetOrAddOneToOneRelationshipBuilderCore(string apiName)
        => this.Context.GetOrAddOneToOneRelationshipBuilder(apiName);

    /// <summary>Gets or creates a schema-level one-to-many relationship builder.</summary>
    internal ApiRelationshipOneToManyBuilder GetOrAddOneToManyRelationshipBuilderCore(string apiName)
        => this.Context.GetOrAddOneToManyRelationshipBuilder(apiName);

    /// <summary>Gets or creates a schema-level many-to-many relationship builder.</summary>
    internal ApiRelationshipManyToManyBuilder GetOrAddManyToManyRelationshipBuilderCore(string apiName)
        => this.Context.GetOrAddManyToManyRelationshipBuilder(apiName);
    #endregion
}
