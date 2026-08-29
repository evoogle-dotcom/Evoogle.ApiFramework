// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Provides strongly-typed configuration for an <see cref="ApiEnumType"/> whose CLR type is
///     <typeparamref name="TEnum"/>.
/// </summary>
/// <typeparam name="TEnum">The CLR enum type represented by the API enum type.</typeparam>
public interface IApiEnumTypeConfiguration<TEnum> : IApiEnumTypeConfiguration
    where TEnum : Enum
{
    #region Properties
    /// <summary>
    ///     Gets the CLR type represented by the configuration.
    /// </summary>
    Type IApiTypeConfiguration.ClrType => typeof(TEnum);
    #endregion

    #region Methods
    /// <summary>
    ///     Applies configuration to the supplied strongly-typed <see cref="ApiEnumTypeBuilder{TEnum}"/>.
    /// </summary>
    /// <param name="builder">The typed builder to configure.</param>
    void Configure(ApiEnumTypeBuilder<TEnum> builder);

    /// <summary>
    ///     Bridges the non-generic <see cref="IApiEnumTypeConfiguration.Configure"/> contract by
    ///     down-casting the builder and delegating to the typed overload.
    /// </summary>
    /// <param name="builder">The builder, which must be an <see cref="ApiEnumTypeBuilder{TEnum}"/> instance.</param>
    /// <exception cref="InvalidCastException">
    ///     Thrown when <paramref name="builder"/> is not an <see cref="ApiEnumTypeBuilder{TEnum}"/>.
    /// </exception>
    void IApiEnumTypeConfiguration.Configure(ApiEnumTypeBuilder builder)
        => this.Configure((ApiEnumTypeBuilder<TEnum>)builder);
    #endregion
}
