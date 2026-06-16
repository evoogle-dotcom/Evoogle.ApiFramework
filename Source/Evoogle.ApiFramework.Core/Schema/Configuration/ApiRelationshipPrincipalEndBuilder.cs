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
///     The principal end provides the primary key for the relationship. For key-bound relationships, initialization
///     infers the best compatible primary key type from the corresponding foreign key when no name is supplied. Call
///     <see cref="WithPrimaryKey"/> to specify the named primary key type explicitly. Delete behavior is configured on the
///     relationship builder, not on individual ends.
/// </remarks>
/// <param name="clrObjectType">The CLR type of the principal <see cref="ApiObjectType"/>.</param>
public sealed class ApiRelationshipPrincipalEndBuilder(Type clrObjectType) : ExtensionBuilder<ApiRelationshipPrincipalEndBuilder>
{
    #region Fields
    private string? _apiPrimaryKeyTypeName;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipPrincipalEndBuilder AddRelationshipPrincipalEndExtension(Type type, object extension)
    {
        base.AddExtension(type, extension);
        return this;
    }
    #endregion

    #region With Methods
    /// <summary>
    ///     Selects a named key type on the principal object type to use as the relationship's primary key,
    ///     overriding automatic compatibility-based key inference.
    /// </summary>
    /// <param name="apiPrimaryKeyTypeName">The name of the primary key type to use for the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public ApiRelationshipPrincipalEndBuilder WithPrimaryKey(string apiPrimaryKeyTypeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiPrimaryKeyTypeName, nameof(apiPrimaryKeyTypeName));

        _apiPrimaryKeyTypeName = apiPrimaryKeyTypeName;
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
            _apiPrimaryKeyTypeName
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
