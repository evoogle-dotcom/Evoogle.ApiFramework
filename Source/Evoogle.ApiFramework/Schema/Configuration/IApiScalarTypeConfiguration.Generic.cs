// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Provides strongly-typed configuration for an <see cref="ApiScalarType"/> whose CLR type is
///     <typeparamref name="TScalar"/>.
/// </summary>
/// <typeparam name="TScalar">The CLR type represented by the API scalar type.</typeparam>
public interface IApiScalarTypeConfiguration<TScalar> : IApiScalarTypeConfiguration
{
    #region Properties
    /// <summary>
    ///     Gets the CLR type represented by the configuration.
    /// </summary>
    Type IApiTypeConfiguration.ClrType => typeof(TScalar);
    #endregion

    #region Methods
    /// <summary>
    ///     Applies configuration to the supplied strongly-typed <see cref="ApiScalarTypeBuilder{TScalar}"/>.
    /// </summary>
    /// <param name="builder">The typed builder to configure.</param>
    void Configure(ApiScalarTypeBuilder<TScalar> builder);

    /// <summary>
    ///     Bridges the non-generic <see cref="IApiScalarTypeConfiguration.Configure"/> contract by
    ///     down-casting the builder and delegating to the typed overload.
    /// </summary>
    /// <param name="builder">The builder, which must be an <see cref="ApiScalarTypeBuilder{TScalar}"/> instance.</param>
    /// <exception cref="InvalidCastException">
    ///     Thrown when <paramref name="builder"/> is not an <see cref="ApiScalarTypeBuilder{TScalar}"/>.
    /// </exception>
    void IApiScalarTypeConfiguration.Configure(ApiScalarTypeBuilder builder)
        => this.Configure((ApiScalarTypeBuilder<TScalar>)builder);
    #endregion
}
