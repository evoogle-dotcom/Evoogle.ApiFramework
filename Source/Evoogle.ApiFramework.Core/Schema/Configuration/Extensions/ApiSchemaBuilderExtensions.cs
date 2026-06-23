// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Convenience extension methods for <see cref="ApiSchemaBuilder"/>.
/// </summary>
public static class ApiSchemaBuilderExtensions
{
    /// <summary>
    ///     Adds an enumeration type to the schema using an optional inline configuration action.
    /// </summary>
    /// <typeparam name="TEnum">The CLR enum type.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="configure">Optional callback to configure the added enumeration type.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddEnum<TEnum>(this ApiSchemaBuilder builder, Action<ApiEnumTypeBuilder<TEnum>>? configure = null)
        where TEnum : Enum
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddEnumCore(configure);
    }

    /// <summary>
    ///     Adds an enumeration type to the schema using an implementation of <see cref="IApiEnumTypeConfiguration"/>.
    /// </summary>
    /// <typeparam name="TEnum">The CLR enum type.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddEnum<TEnum>(this ApiSchemaBuilder builder, IApiEnumTypeConfiguration configuration)
        where TEnum : Enum
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        return builder.AddEnumCore<TEnum>(configuration.Configure);
    }

    /// <summary>
    ///     Adds an object type to the schema using a strongly-typed optional inline configuration action.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="configure">Optional callback to configure the added object type.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddObject<TObject>(this ApiSchemaBuilder builder, Action<ApiObjectTypeBuilder<TObject>>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddObjectCore(configure);
    }

    /// <summary>
    ///     Adds an object type to the schema using a strongly-typed <see cref="IApiObjectTypeConfiguration{TObject}"/>.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="configuration">The typed configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddObject<TObject>(this ApiSchemaBuilder builder, IApiObjectTypeConfiguration<TObject> configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        return builder.AddObjectCore<TObject>(configuration.Configure);
    }

    /// <summary>
    ///     Adds a scalar type to the schema using an inline configuration action.
    /// </summary>
    /// <typeparam name="TScalar">The CLR scalar type.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="configure">Optional callback to configure the added scalar type.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddScalar<TScalar>(this ApiSchemaBuilder builder, Action<ApiScalarTypeBuilder<TScalar>>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddScalarCore(configure);
    }

    /// <summary>
    ///     Adds a scalar type to the schema using an implementation of <see cref="IApiScalarTypeConfiguration"/>.
    /// </summary>
    /// <typeparam name="TScalar">The CLR scalar type.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddScalar<TScalar>(this ApiSchemaBuilder builder, IApiScalarTypeConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        return builder.AddScalarCore<TScalar>(configuration.Configure);
    }

    /// <summary>
    ///     Adds an extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddSchemaExtension<TExtension>(this ApiSchemaBuilder builder, TExtension extension)
        where TExtension : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddSchemaExtension(typeof(TExtension), extension);
    }
}
