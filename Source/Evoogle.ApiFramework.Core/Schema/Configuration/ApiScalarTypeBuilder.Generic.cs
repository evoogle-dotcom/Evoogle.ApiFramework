// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring an <see cref="ApiScalarType"/> whose CLR type is <typeparamref name="T"/>.
///     Extends <see cref="ApiScalarTypeBuilder"/> ensuring that the typed callback passed to
///     <see cref="ApiSchemaBuilder.AddScalar{T}(System.Action{ApiScalarTypeBuilder{T}}?)"/> receives
///     a <typeparamref name="T"/>-parameterized builder, consistent with <see cref="ApiObjectTypeBuilder{T}"/>.
/// </summary>
/// <typeparam name="T">The CLR scalar type.</typeparam>
/// <param name="context">The shared builder context.</param>
public sealed class ApiScalarTypeBuilder<T>(ApiSchemaBuilderContext context)
    : ApiScalarTypeBuilder(typeof(T), context)
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiScalarTypeBuilder.AddScalarExtension(Type, object)"/>
    public new ApiScalarTypeBuilder<T> AddScalarExtension(Type type, object value)
    {
        base.AddScalarExtension(type, value);
        return this;
    }

    /// <inheritdoc cref="ApiScalarTypeBuilder.AddScalarExtension{TExt}(TExt)"/>
    public new ApiScalarTypeBuilder<T> AddScalarExtension<TExt>(TExt value) where TExt : notnull
        => this.AddScalarExtension(typeof(TExt), value);
    #endregion

    #region With Methods
    /// <inheritdoc cref="ApiNamedTypeBuilder{TBuilder}.WithName"/>
    public new ApiScalarTypeBuilder<T> WithName(string apiName)
    {
        base.WithName(apiName);
        return this;
    }
    #endregion
}
