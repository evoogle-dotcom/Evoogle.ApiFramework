// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring an <see cref="ApiScalarType"/> whose CLR type is <typeparamref name="TScalar"/>.
///     Extends <see cref="ApiScalarTypeBuilder"/> ensuring that the typed callback passed to
///     <see cref="ApiSchemaBuilderExtensions.AddScalar{TScalar}(ApiSchemaBuilder, Action{ApiScalarTypeBuilder{TScalar}}?)"/> receives
///     a <typeparamref name="TScalar"/>-parameterized builder, consistent with <see cref="ApiObjectTypeBuilder{TScalar}"/>.
/// </summary>
/// <typeparam name="TScalar">The CLR scalar type.</typeparam>
/// <param name="context">The shared builder context.</param>
public sealed class ApiScalarTypeBuilder<TScalar>(ApiSchemaBuilderContext context)
    : ApiScalarTypeBuilder(typeof(TScalar), context)
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiScalarTypeBuilder.AddScalarTypeExtension(Type, object)"/>
    public new ApiScalarTypeBuilder<TScalar> AddScalarTypeExtension(Type extensionType, object extension)
    {
        base.AddScalarTypeExtension(extensionType, extension);
        return this;
    }
    #endregion

    #region With Methods
    /// <inheritdoc cref="ApiNamedTypeBuilder{TBuilder}.WithName"/>
    public new ApiScalarTypeBuilder<TScalar> WithName(string apiName)
    {
        base.WithName(apiName);
        return this;
    }
    #endregion
}
