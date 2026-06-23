// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Provides strongly-typed configuration for an <see cref="ApiObjectType"/> whose CLR type is <typeparamref name="TObject"/>.
///     Inherits from <see cref="IApiObjectTypeConfiguration"/> and bridges the non-generic
///     <see cref="ApiObjectTypeBuilder"/> interface via a default implementation that up-casts the builder.
/// </summary>
/// <typeparam name="TObject">The CLR type represented by the API object type.</typeparam>
public interface IApiObjectTypeConfiguration<TObject> : IApiObjectTypeConfiguration
{
    #region Methods
    /// <summary>
    ///     Applies configuration to the supplied strongly-typed <see cref="ApiObjectTypeBuilder{TObject}"/>.
    /// </summary>
    /// <param name="builder">The typed builder to configure.</param>
    void Configure(ApiObjectTypeBuilder<TObject> builder);

    /// <summary>
    ///     Bridges the non-generic <see cref="IApiObjectTypeConfiguration.Configure"/> contract by
    ///     down-casting the builder and delegating to the typed overload.
    /// </summary>
    /// <param name="builder">The non-generic builder. Must be an <see cref="ApiObjectTypeBuilder{TObject}"/> instance.</param>
    /// <exception cref="InvalidCastException">
    ///     Thrown when <paramref name="builder"/> is not an <see cref="ApiObjectTypeBuilder{TObject}"/>.
    ///     This happens when the configuration is applied to the wrong object type.
    /// </exception>
    void IApiObjectTypeConfiguration.Configure(ApiObjectTypeBuilder builder)
        => this.Configure((ApiObjectTypeBuilder<TObject>)builder);
    #endregion
}
