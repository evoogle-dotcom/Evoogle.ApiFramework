// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Key;
using Evoogle.ApiFramework.Schema.Configuration.Relationships;
using Evoogle.ApiFramework.Schema.Configuration.Version;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Types;


/// <summary>
///     Strongly-typed fluent builder for configuring an <see cref="ApiObjectType"/> whose CLR type is <typeparamref name="TObject"/>.
///     Extends <see cref="ApiObjectTypeBuilder"/> with expression-based overloads so CLR member names are
///     extracted at compile time rather than supplied as raw strings.
/// </summary>
/// <typeparam name="TObject">The CLR type represented by the API object type.</typeparam>
/// <param name="context">The shared builder context.</param>
public sealed class ApiObjectTypeBuilder<TObject>(ApiSchemaBuilderContext context)
    : ApiObjectTypeBuilder(typeof(TObject), context)
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiObjectTypeBuilder.AddObjectTypeExtension(Type, object)"/>
    public new ApiObjectTypeBuilder<TObject> AddObjectTypeExtension(Type extensionType, object extension)
    {
        base.AddObjectTypeExtension(extensionType, extension);
        return this;
    }
    #endregion

    #region AddKey Methods
    /// <summary>
    ///     Adds an <see cref="ApiKeyDefinition"/> using a strongly-typed
    ///     <see cref="ApiKeyDefinitionBuilder{TObject}"/>
    ///     callback that supports expression-based property selection.
    /// </summary>
    /// <remarks>
    ///     Key-bound relationship principal ends infer the best compatible key from the corresponding foreign key
    ///     when no key name is supplied; call
    ///     <see cref="ApiRelationshipPrincipalEndBuilder.WithPrincipalKey"/> on the principal end builder to
    ///     select a named key explicitly.
    ///     To configure with string-based property names instead, use the base
    ///     <see cref="ApiObjectTypeBuilder.AddKey"/> overload. Both overloads configure
    ///     the same internally managed key builder.
    /// </remarks>
    /// <param name="apiName">The API name of the key.</param>
    /// <param name="configure">Optional callback to configure the key using a typed builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<TObject> AddKey(string apiName, Action<ApiKeyDefinitionBuilder<TObject>>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        var apiKeyDefinitionBuilder = (ApiKeyDefinitionBuilder<TObject>)this.GetOrAddKeyBuilder(apiName);
        configure?.Invoke(apiKeyDefinitionBuilder);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.AddKey(string, Action{ApiKeyDefinitionBuilder}?)"/>
    public new ApiObjectTypeBuilder<TObject> AddKey(string apiName, Action<ApiKeyDefinitionBuilder>? configure = null)
    {
        base.AddKey(apiName, configure);
        return this;
    }
    #endregion

    #region With Methods
    /// <inheritdoc cref="ApiNamedTypeBuilder{TBuilder}.WithName"/>
    public new ApiObjectTypeBuilder<TObject> WithName(string apiName)
    {
        base.WithName(apiName);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.WithOptions"/>
    public new ApiObjectTypeBuilder<TObject> WithOptions(Action<ApiObjectTypeOptionsBuilder> configure)
    {
        base.WithOptions(configure);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.WithDefaultOptions"/>
    public new ApiObjectTypeBuilder<TObject> WithDefaultOptions()
    {
        base.WithDefaultOptions();
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.WithVersion(string, Action{ApiVersionDefinitionBuilder}?)"/>
    public new ApiObjectTypeBuilder<TObject> WithVersion
    (
        string clrMemberName,
        Action<ApiVersionDefinitionBuilder>? configure = null
    )
    {
        base.WithVersion(clrMemberName, configure);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.WithVersion(ApiPropertyReference, Action{ApiVersionDefinitionBuilder}?)"/>
    public new ApiObjectTypeBuilder<TObject> WithVersion
    (
        ApiPropertyReference apiPropertyReference,
        Action<ApiVersionDefinitionBuilder>? configure = null
    )
    {
        base.WithVersion(apiPropertyReference, configure);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.WithRepositoryVersion"/>
    public new ApiObjectTypeBuilder<TObject> WithRepositoryVersion
    (
        Type clrVersionType,
        Action<ApiVersionDefinitionBuilder>? configure = null
    )
    {
        base.WithRepositoryVersion(clrVersionType, configure);
        return this;
    }

    /// <summary>Configures a repository-backed version using a type-safe CLR version type.</summary>
    /// <typeparam name="TVersion">The exact CLR version type.</typeparam>
    /// <param name="configure">Optional version metadata configuration.</param>
    /// <returns>The current builder.</returns>
    public ApiObjectTypeBuilder<TObject> WithRepositoryVersion<TVersion>
    (
        Action<ApiVersionDefinitionBuilder>? configure = null
    )
    {
        base.WithRepositoryVersion(typeof(TVersion), configure);
        return this;
    }
    #endregion
}
