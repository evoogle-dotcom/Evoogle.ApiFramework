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
public sealed class ApiObjectTypeBuilder(Type clrType, ApiSchemaBuilderContext context)
    : ApiNamedTypeBuilder<ApiObjectTypeBuilder>(clrType, context)
{
    #region Fields
    private readonly List<ApiPropertyBuilder> _apiPropertyBuilders = [];
    private readonly List<ApiRelationshipBuilder> _apiRelationshipBuilders = [];
    private Action<ApiObjectTypeOptionsBuilder>? _apiOptionsConfiguration = null;
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition to the object type.
    /// </summary>
    /// <param name="apiName">The API property name.</param>
    /// <param name="clrName">The CLR property name.</param>
    /// <param name="modifiers">Optional callback to configure type modifiers.</param>
    /// <param name="extensions">Optional callback to configure property extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddProperty(string apiName, string clrName, Action<ApiTypeModifiersBuilder>? modifiers = null, Action<ApiPropertyBuilder>? extensions = null)
    {
        var apiPropertyBuilder = new ApiPropertyBuilder(apiName, clrName)
        {
            Modifiers = modifiers
        };

        extensions?.Invoke(apiPropertyBuilder);

        _apiPropertyBuilders.Add(apiPropertyBuilder);

        return this;
    }

    /// <summary>
    ///     Adds a relationship definition to the object type.
    /// </summary>
    /// <param name="apiName">The API name of the relationship.</param>
    /// <param name="apiPropertyName">Optional API property name backing the relationship.</param>
    /// <param name="extensions">Optional callback to configure relationship extensions.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder AddRelationship(string apiName, string? apiPropertyName = null, Action<ApiRelationshipBuilder>? extensions = null)
    {
        var apiRelationshipBuilder = new ApiRelationshipBuilder(apiName, apiPropertyName);

        extensions?.Invoke(apiRelationshipBuilder);

        _apiRelationshipBuilders.Add(apiRelationshipBuilder);

        return this;
    }

    public ApiObjectTypeBuilder WithDefaultOptions()
    {
        _apiOptionsConfiguration = null;
        return this;
    }

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

        var apiProperties = _apiPropertyBuilders
            .Select(b => b.Build(clrObjectType));

        var apiRelationships = _apiRelationshipBuilders
            .Select(b => b.Build());

        var apiObjectType = new ApiObjectType
        (
            apiName,
            apiIdentitySet: null,
            apiOptions,
            apiProperties,
            apiRelationships,
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
