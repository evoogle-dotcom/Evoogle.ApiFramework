// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Extension methods for <see cref="ApiRelationshipOneToOneBuilder"/>.
/// </summary>
public static class ApiRelationshipOneToOneBuilderExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Adds a relationship extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The relationship builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiRelationshipOneToOneBuilder AddRelationshipExtension<TExtension>(this ApiRelationshipOneToOneBuilder builder, TExtension extension)
        where TExtension : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddRelationshipExtension(typeof(TExtension), extension);
    }
    #endregion

    #region From/To Methods
    /// <summary>
    ///     Configures the principal end of the 1:1 relationship using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    public static ApiRelationshipOneToOneBuilder From<TPrincipal>
    (
        this ApiRelationshipOneToOneBuilder builder,
        Action<ApiRelationshipPrincipalEndBuilder>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.From(typeof(TPrincipal), configure);
    }

    /// <summary>
    ///     Configures the principal end of the 1:1 relationship using the CLR type <typeparamref name="TPrincipal"/>.
    /// </summary>
    public static ApiRelationshipOneToOneBuilder From<TPrincipal>(this ApiRelationshipOneToOneBuilder builder, string apiKeyTypeName)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKeyTypeName, nameof(apiKeyTypeName));

        return builder.From(typeof(TPrincipal), apiKeyTypeName);
    }

    /// <summary>
    ///     Configures the dependent end of the 1:1 relationship using the CLR type <typeparamref name="TDependent"/>.
    /// </summary>
    public static ApiRelationshipOneToOneBuilder To<TDependent>
    (
        this ApiRelationshipOneToOneBuilder builder,
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
