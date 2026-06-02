// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Strongly-typed fluent builder for configuring an <see cref="ApiEnumType"/> whose CLR type is <typeparamref name="T"/>.
///     Extends <see cref="ApiEnumTypeBuilder"/> with enum-member overloads so CLR names and ordinals are
///     inferred from the enum constant rather than supplied as raw strings.
/// </summary>
/// <typeparam name="T">The CLR enum type.</typeparam>
/// <param name="context">The shared builder context.</param>
public sealed class ApiEnumTypeBuilder<T>(ApiSchemaBuilderContext context)
    : ApiEnumTypeBuilder(typeof(T), context)
    where T : Enum
{
    #region AddExtension Methods
    /// <inheritdoc cref="ApiEnumTypeBuilder.AddEnumTypeExtension(Type, object)"/>
    public new ApiEnumTypeBuilder<T> AddEnumTypeExtension(Type type, object value)
    {
        base.AddEnumTypeExtension(type, value);
        return this;
    }

    /// <inheritdoc cref="ApiEnumTypeBuilder.AddEnumTypeExtension{TExt}(TExt)"/>
    public new ApiEnumTypeBuilder<T> AddEnumTypeExtension<TExt>(TExt value) where TExt : notnull
        => this.AddEnumTypeExtension(typeof(TExt), value);
    #endregion

    #region AddValue Methods
    /// <summary>
    ///     Adds an <see cref="ApiEnumValue"/> definition derived from the CLR enum member <paramref name="member"/>.
    ///     The CLR name and ordinal are inferred automatically; the API name defaults to the CLR name.
    /// </summary>
    /// <param name="member">The CLR enum member to add.</param>
    /// <param name="apiName">
    ///     Optional API name override. When <see langword="null"/> the CLR member name is used as the API name.
    /// </param>
    /// <returns>The current builder instance.</returns>
    public ApiEnumTypeBuilder<T> AddValue(T member, string? apiName = null)
    {
        var clrName = member.ToString();
        var ordinal = Convert.ToInt32(member);
        base.AddValue(apiName ?? clrName, clrName, ordinal);
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="ApiEnumValue"/> definition for every member declared on <typeparamref name="T"/>.
    ///     Each member uses its CLR name as the API name.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ApiEnumTypeBuilder<T> AddAllValues()
    {
        foreach (T member in Enum.GetValues(typeof(T)))
        {
            this.AddValue(member);
        }

        return this;
    }
    #endregion

    #region With Methods
    /// <inheritdoc cref="ApiNamedTypeBuilder{TBuilder}.WithName"/>
    public new ApiEnumTypeBuilder<T> WithName(string apiName)
    {
        base.WithName(apiName);
        return this;
    }
    #endregion
}
