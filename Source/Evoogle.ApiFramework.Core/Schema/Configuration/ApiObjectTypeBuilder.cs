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
    ///     Adds an <see cref="ApiIdentity"/> definition to the object type.
    /// </summary>
    /// <remarks>
    ///     The first identity added is the primary identity by convention unless specified otherwise.
    /// </remarks>
    /// <param name="apiName">The API name of the identity.</param>
    /// <param name="configure">Callback to configure the added identity.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddIdentity(string apiName, Action<ApiIdentityBuilder> configure)
    {
        var apiIdentityBuilder = new ApiIdentityBuilder(apiName);

        configure.Invoke(apiIdentityBuilder);

        _apiIdentityBuilders.Add(apiIdentityBuilder);

        return this;
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
