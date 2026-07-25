// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring an <see cref="ApiEnumType"/> whose CLR type is <typeparamref name="TEnum"/>.
///     Extends <see cref="ApiEnumTypeBuilder"/> with enum-member overloads so CLR names and ordinals are
///     inferred from the enum constant rather than supplied as raw strings.
/// </summary>
/// <typeparam name="TEnum">The CLR enum type.</typeparam>
/// <param name="context">The shared builder context.</param>
public sealed class ApiEnumTypeBuilder<TEnum>(ApiSchemaBuilderContext context)
    : ApiEnumTypeBuilder(typeof(TEnum), context)
    where TEnum : Enum
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiEnumTypeBuilder.AddEnumTypeExtension(Type, object)"/>
    public new ApiEnumTypeBuilder<TEnum> AddEnumTypeExtension(Type extensionType, object extension)
    {
        base.AddEnumTypeExtension(extensionType, extension);
        return this;
    }
    #endregion

    #region AddValue Methods
    /// <summary>
    ///     Adds an <see cref="ApiEnumValue"/> definition derived from the CLR enum member <paramref name="member"/>.
    ///     The CLR name and ordinal are inferred automatically. When no API name is supplied, its
    ///     initial value is inferred from the CLR name and remains configurable by conventions.
    /// </summary>
    /// <param name="member">The CLR enum member to add.</param>
    /// <param name="apiName">
    ///     Optional explicit API name. When <see langword="null"/>, the API name is inferred from
    ///     the CLR member name.
    /// </param>
    /// <returns>The current builder instance.</returns>
    public ApiEnumTypeBuilder<TEnum> AddValue(TEnum member, string? apiName = null)
    {
        var clrName = member.ToString();
        var ordinal = Convert.ToInt32(member);

        if (apiName == null)
        {
            base.AddValueWithInferredName(clrName, ordinal);
        }
        else
        {
            base.AddValue(apiName, clrName, ordinal);
        }

        return this;
    }

    /// <summary>
    ///     Adds an <see cref="ApiEnumValue"/> definition for every member declared on <typeparamref name="TEnum"/>.
    ///     Each API name is inferred from its CLR member name and remains configurable by
    ///     conventions.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiEnumTypeBuilder<TEnum> AddAllValues()
    {
        foreach (TEnum member in Enum.GetValues(typeof(TEnum)))
        {
            this.AddValue(member);
        }

        return this;
    }
    #endregion

    #region With Methods
    /// <inheritdoc cref="ApiNamedTypeBuilder{TBuilder}.WithName"/>
    public new ApiEnumTypeBuilder<TEnum> WithName(string apiName)
    {
        base.WithName(apiName);
        return this;
    }
    #endregion
}
