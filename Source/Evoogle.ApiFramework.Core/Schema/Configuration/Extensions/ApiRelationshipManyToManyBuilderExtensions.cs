// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Extension methods for <see cref="ApiRelationshipManyToManyBuilder"/>.
/// </summary>
public static class ApiRelationshipManyToManyBuilderExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Adds a relationship extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The relationship builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiRelationshipManyToManyBuilder AddRelationshipExtension<TExtension>(this ApiRelationshipManyToManyBuilder builder, TExtension extension)
        where TExtension : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddRelationshipExtension(typeof(TExtension), extension);
    }
    #endregion

    #region Between/And Methods
    /// <summary>
    ///     Configures principal end A using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    public static ApiRelationshipManyToManyBuilder Between<TPrincipal>
    (
        this ApiRelationshipManyToManyBuilder builder,
        Action<ApiRelationshipPrincipalEndBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.Between(typeof(TPrincipal), configure);
    }

    /// <summary>
    ///     Configures principal end A using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    public static ApiRelationshipManyToManyBuilder Between<TPrincipal>(this ApiRelationshipManyToManyBuilder builder, string apiPrincipalKeyTypeName)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiPrincipalKeyTypeName, nameof(apiPrincipalKeyTypeName));

        return builder.Between(typeof(TPrincipal), apiPrincipalKeyTypeName);
    }

    /// <summary>
    ///     Configures principal end B using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    public static ApiRelationshipManyToManyBuilder And<TPrincipal>
    (
        this ApiRelationshipManyToManyBuilder builder,
        Action<ApiRelationshipPrincipalEndBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.And(typeof(TPrincipal), configure);
    }

    /// <summary>
    ///     Configures principal end B using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    public static ApiRelationshipManyToManyBuilder And<TPrincipal>(this ApiRelationshipManyToManyBuilder builder, string apiPrincipalKeyTypeName)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiPrincipalKeyTypeName, nameof(apiPrincipalKeyTypeName));

        return builder.And(typeof(TPrincipal), apiPrincipalKeyTypeName);
    }
    #endregion

    #region WithAssociation Methods
    /// <summary>
    ///     Configures the association using the CLR type <typeparamref name="TAssociation"/>.
    /// </summary>
    public static ApiRelationshipManyToManyBuilder WithAssociation<TAssociation>
    (
        this ApiRelationshipManyToManyBuilder builder,
        Action<ApiRelationshipAssociationBuilder<TAssociation>>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        var associationBuilder = new ApiRelationshipAssociationBuilder<TAssociation>();
        configure?.Invoke(associationBuilder);
        return builder.SetAssociationBuilderCore(associationBuilder);
    }
    #endregion
}
