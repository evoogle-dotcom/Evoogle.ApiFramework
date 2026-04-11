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
/// <param name="apiObjectTypeName">The API name of the principal <see cref="ApiObjectType"/>.</param>
public sealed class ApiRelationshipPrincipalEndBuilder
(
    string apiObjectTypeName
) : ExtensionBuilder<ApiRelationshipPrincipalEndBuilder>
{
    #region Fields
    private string? _apiIdentityName;
    private ApiRelationshipDeleteBehavior _apiDeleteBehavior = ApiRelationshipDeleteBehavior.None;
    #endregion

    #region Builder Methods
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
            apiObjectTypeName,
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
