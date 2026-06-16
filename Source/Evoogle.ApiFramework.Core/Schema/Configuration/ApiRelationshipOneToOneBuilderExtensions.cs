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
    public static ApiRelationshipOneToOneBuilder From<TPrincipal>(this ApiRelationshipOneToOneBuilder builder, string apiPrimaryKeyTypeName)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.From(typeof(TPrincipal), apiPrimaryKeyTypeName);
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
