// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Extension methods for <see cref="ApiRelationshipOneToManyBuilder"/>.
/// </summary>
public static class ApiRelationshipOneToManyBuilderExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Adds a relationship extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The relationship builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiRelationshipOneToManyBuilder AddRelationshipExtension<TExtension>(this ApiRelationshipOneToManyBuilder builder, TExtension extension)
        where TExtension : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddRelationshipExtension(typeof(TExtension), extension);
    }
    #endregion

    #region From/To Methods
    /// <summary>
    ///     Configures the principal end of the 1:M relationship using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    public static ApiRelationshipOneToManyBuilder From<TPrincipal>
    (
        this ApiRelationshipOneToManyBuilder builder,
        Action<ApiRelationshipPrincipalEndBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.From(typeof(TPrincipal), configure);
    }

    /// <summary>
    ///     Configures the principal end of the 1:M relationship using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    public static ApiRelationshipOneToManyBuilder From<TPrincipal>(this ApiRelationshipOneToManyBuilder builder, string apiPrincipalKeyTypeName)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiPrincipalKeyTypeName, nameof(apiPrincipalKeyTypeName));

        return builder.From(typeof(TPrincipal), apiPrincipalKeyTypeName);
    }

    /// <summary>
    ///     Configures the dependent end of the 1:M relationship using the CLR type <typeparamref name="TDependent"/>.
    /// </summary>
    public static ApiRelationshipOneToManyBuilder To<TDependent>
    (
        this ApiRelationshipOneToManyBuilder builder,
        Action<ApiRelationshipDependentEndBuilder<TDependent>>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        var dependentEndBuilder = new ApiRelationshipDependentEndBuilder<TDependent>();
        configure?.Invoke(dependentEndBuilder);
        return builder.SetDependentEndBuilderCore(dependentEndBuilder);
    }
    #endregion
}
