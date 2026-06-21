// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;


/// <summary>
///     Strongly-typed fluent builder for configuring an <see cref="ApiObjectType"/> whose CLR type is <typeparamref name="T"/>.
///     Extends <see cref="ApiObjectTypeBuilder"/> with expression-based overloads so CLR member names are
///     extracted at compile time rather than supplied as raw strings.
/// </summary>
/// <typeparam name="T">The CLR type represented by the API object type.</typeparam>
/// <param name="context">The shared builder context.</param>
public sealed class ApiObjectTypeBuilder<T>(ApiSchemaBuilderContext context)
    : ApiObjectTypeBuilder(typeof(T), context)
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiObjectTypeBuilder.AddObjectTypeExtension(Type, object)"/>
    public new ApiObjectTypeBuilder<T> AddObjectTypeExtension(Type type, object extension)
    {
        base.AddObjectTypeExtension(type, extension);
        return this;
    }
    #endregion

    #region AddKey Methods
    /// <summary>
    ///     Adds an <see cref="ApiKeyType"/> definition using a strongly-typed <see cref="ApiKeyTypeBuilder{T}"/>
    ///     callback that supports expression-based property selection.
    /// </summary>
    /// <remarks>
    ///     Key-bound relationship principal ends infer the best compatible key from the corresponding foreign key
    ///     when no key name is supplied; call
    ///     <see cref="ApiRelationshipPrincipalEndBuilder.WithPrincipalKey"/> on the principal end builder to
    ///     select a named key explicitly.
    ///     To configure with string-based property names instead, use the non-generic
    ///     <see cref="ApiObjectTypeBuilder.AddKey"/> overload on the base class.
    /// </remarks>
    /// <param name="apiName">The API name of the key type.</param>
    /// <param name="configure">Optional callback to configure the key type using a typed builder.</param>
    /// <returns>The current builder instance.</returns>
    public ApiObjectTypeBuilder<T> AddKey(string apiName, Action<ApiKeyTypeBuilder<T>>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        var apiKeyTypeBuilder = new ApiKeyTypeBuilder<T>(apiName);
        configure?.Invoke(apiKeyTypeBuilder);
        base.AddKeyTypeBuilderCore(apiKeyTypeBuilder);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.AddKey(string, Action{ApiKeyTypeBuilder}?)"/>
    public new ApiObjectTypeBuilder<T> AddKey(string apiName, Action<ApiKeyTypeBuilder>? configure = null)
    {
        base.AddKey(apiName, configure);
        return this;
    }
    #endregion

    #region With Methods
    /// <inheritdoc cref="ApiNamedTypeBuilder{TBuilder}.WithName"/>
    public new ApiObjectTypeBuilder<T> WithName(string apiName)
    {
        base.WithName(apiName);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.WithOptions"/>
    public new ApiObjectTypeBuilder<T> WithOptions(Action<ApiObjectTypeOptionsBuilder> configure)
    {
        base.WithOptions(configure);
        return this;
    }

    /// <inheritdoc cref="ApiObjectTypeBuilder.WithDefaultOptions"/>
    public new ApiObjectTypeBuilder<T> WithDefaultOptions()
    {
        base.WithDefaultOptions();
        return this;
    }
    #endregion
}
