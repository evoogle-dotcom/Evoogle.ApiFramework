// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure the principal end of an <see cref="ApiRelationship"/>.
/// </summary>
/// <remarks>
///     The principal end provides the referenced key type. Use <see cref="WithKeyTypeName"/> to select a
///     non-primary key type. Delete behavior is configured on the relationship builder, not on individual ends.
/// </remarks>
/// <param name="clrObjectType">The CLR type of the principal <see cref="ApiObjectType"/>.</param>
public sealed class ApiRelationshipPrincipalEndBuilder(Type clrObjectType) : ExtensionBuilder<ApiRelationshipPrincipalEndBuilder>
{
    #region Fields
    private string? _apiKeyTypeName;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipPrincipalEndBuilder AddRelationshipPrincipalEndExtension(Type type, object value)
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
    public ApiRelationshipPrincipalEndBuilder AddRelationshipPrincipalEndExtension<T>(T value) where T : notnull
        => this.AddRelationshipPrincipalEndExtension(typeof(T), value);
    #endregion

    #region With Methods
    /// <summary>
    ///     Explicitly selects the <see cref="ApiKeyType"/> on the principal object type that serves as the join key.
    ///     When not called the primary key type is used by convention.
    /// </summary>
    /// <param name="apiKeyTypeName">The name of the key type to use as the join key.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipPrincipalEndBuilder WithKeyTypeName(string apiKeyTypeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKeyTypeName, nameof(apiKeyTypeName));

        _apiKeyTypeName = apiKeyTypeName;
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiRelationshipPrincipalEnd"/> configured by this builder.
    /// </summary>
    internal ApiRelationshipPrincipalEnd Build()
    {
        var end = new ApiRelationshipPrincipalEnd
        (
            clrObjectType,
            _apiKeyTypeName
        );

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            end.Extensions = extensions;
        }

        return end;
    }
    #endregion
}
