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
///     The principal end owns the join key identity.  Use <see cref="WithIdentityName"/> to select a
///     non-primary identity, and <see cref="WithDeleteBehavior"/> to control what happens to dependent
///     objects when an object on this end is deleted.
/// </remarks>
/// <param name="clrObjectType">The CLR type of the principal <see cref="ApiObjectType"/>.</param>
public sealed class ApiRelationshipPrincipalEndBuilder
(
    Type clrObjectType
) : ExtensionBuilder<ApiRelationshipPrincipalEndBuilder>
{
    #region Fields
    private string? _apiIdentityName;
    private ApiRelationshipDeleteBehavior _apiDeleteBehavior = ApiRelationshipDeleteBehavior.None;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipPrincipalEndBuilder AddPrincipalEndExtension(Type type, object value)
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
    public ApiRelationshipPrincipalEndBuilder AddPrincipalEndExtension<T>(T value) where T : notnull
        => this.AddPrincipalEndExtension(typeof(T), value);
    #endregion

    #region With Methods
    /// <summary>
    ///     Explicitly selects the <see cref="ApiIdentity"/> on the principal object type that serves as the join key.
    ///     When not called the primary identity is used by convention.
    /// </summary>
    /// <param name="apiIdentityName">The name of the identity to use as the join key.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipPrincipalEndBuilder WithIdentityName(string apiIdentityName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiIdentityName, nameof(apiIdentityName));

        _apiIdentityName = apiIdentityName;
        return this;
    }

    /// <summary>
    ///     Sets the delete behavior that governs what happens to the dependent objects when a principal object is deleted.
    /// </summary>
    /// <param name="apiDeleteBehavior">The desired delete behavior.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipPrincipalEndBuilder WithDeleteBehavior(ApiRelationshipDeleteBehavior apiDeleteBehavior)
    {
        _apiDeleteBehavior = apiDeleteBehavior;
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
            _apiIdentityName,
            _apiDeleteBehavior
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
