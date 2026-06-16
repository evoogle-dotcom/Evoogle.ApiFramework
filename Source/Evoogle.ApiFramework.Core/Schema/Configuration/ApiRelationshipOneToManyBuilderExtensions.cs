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
    public static ApiRelationshipOneToManyBuilder From<TPrincipal>(this ApiRelationshipOneToManyBuilder builder, string apiPrimaryKeyTypeName)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.From(typeof(TPrincipal), apiPrimaryKeyTypeName);
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
