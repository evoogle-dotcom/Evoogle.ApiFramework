// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Convenience extension methods for <see cref="ApiEnumTypeBuilder"/>.
/// </summary>
public static class ApiEnumTypeBuilderExtensions
{
    /// <summary>
    ///     Adds an enum type extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The enum type builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiEnumTypeBuilder AddEnumTypeExtension<TExtension>(this ApiEnumTypeBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddEnumTypeExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds an enum type extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TEnum">The CLR enum type represented by the builder.</typeparam>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The enum type builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiEnumTypeBuilder<TEnum> AddEnumTypeExtension<TEnum, TExtension>(this ApiEnumTypeBuilder<TEnum> builder, TExtension extension)
        where TEnum : Enum
        where TExtension : class
        => builder.AddEnumTypeExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds an <see cref="ApiEnumValue"/> definition using <paramref name="name"/> as both the API name and CLR name.
    /// </summary>
    /// <param name="builder">The enum type builder to configure.</param>
    /// <param name="name">The API and CLR name of the enumeration value.</param>
    /// <param name="clrOrdinal">The CLR ordinal of the enumeration value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiEnumTypeBuilder AddValue(this ApiEnumTypeBuilder builder, string name, int clrOrdinal)
        => builder.AddValue(name, name, clrOrdinal);
}
