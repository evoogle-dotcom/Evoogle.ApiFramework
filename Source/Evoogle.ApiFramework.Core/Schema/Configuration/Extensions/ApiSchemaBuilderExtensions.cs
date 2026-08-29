// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Convenience extension methods for <see cref="ApiSchemaBuilder"/>.
/// </summary>
public static class ApiSchemaBuilderExtensions
{
    #region Add Extensions
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
    ///     Adds an enumeration type to the schema using a strongly-typed implementation of
    ///     <see cref="IApiEnumTypeConfiguration{TEnum}"/>.
    /// </summary>
    /// <typeparam name="TEnum">The CLR enum type.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="configuration">The strongly-typed configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddEnum<TEnum>(this ApiSchemaBuilder builder, IApiEnumTypeConfiguration<TEnum> configuration)
        where TEnum : Enum
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        return builder.AddEnum(configuration);
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

        return builder.AddObject(configuration);
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
    ///     Adds a scalar type to the schema using a strongly-typed implementation of
    ///     <see cref="IApiScalarTypeConfiguration{TScalar}"/>.
    /// </summary>
    /// <typeparam name="TScalar">The CLR scalar type.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="configuration">The strongly-typed configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddScalar<TScalar>(this ApiSchemaBuilder builder, IApiScalarTypeConfiguration<TScalar> configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        return builder.AddScalar(configuration);
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

    /// <summary>
    ///     Registers a single CLR type for convention-driven and annotation-driven schema configuration.
    /// </summary>
    /// <typeparam name="T1">The CLR type to register.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddTypes<T1>(this ApiSchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddTypes(typeof(T1));
    }

    /// <summary>
    ///     Registers two CLR types for convention-driven and annotation-driven schema configuration.
    /// </summary>
    /// <typeparam name="T1">The first CLR type to register.</typeparam>
    /// <typeparam name="T2">The second CLR type to register.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddTypes<T1, T2>(this ApiSchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddTypes(typeof(T1), typeof(T2));
    }

    /// <summary>
    ///     Registers three CLR types for convention-driven and annotation-driven schema configuration.
    /// </summary>
    /// <typeparam name="T1">The first CLR type to register.</typeparam>
    /// <typeparam name="T2">The second CLR type to register.</typeparam>
    /// <typeparam name="T3">The third CLR type to register.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddTypes<T1, T2, T3>(this ApiSchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddTypes(typeof(T1), typeof(T2), typeof(T3));
    }

    /// <summary>
    ///     Registers four CLR types for convention-driven and annotation-driven schema configuration.
    /// </summary>
    /// <typeparam name="T1">The first CLR type to register.</typeparam>
    /// <typeparam name="T2">The second CLR type to register.</typeparam>
    /// <typeparam name="T3">The third CLR type to register.</typeparam>
    /// <typeparam name="T4">The fourth CLR type to register.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddTypes<T1, T2, T3, T4>(this ApiSchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
    }
    #endregion

    #region Use Extensions
    /// <summary>
    ///     Adds the built-in convention that scans an assembly for annotated schema types.
    /// </summary>
    /// <remarks>
    ///     Built-in attributes are discovered when <see cref="ApiSchemaBuilder.UseDefaultAnnotations"/>
    ///     is configured. Custom type-discovery readers can be registered through
    ///     <see cref="ApiSchemaBuilder.UseAnnotations"/>.
    /// </remarks>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="assembly">The assembly to scan for annotated types.</param>
    /// <param name="filter">
    ///     Optional inclusion predicate. When <see langword="null"/>, all eligible types are
    ///     considered; returning <see langword="false"/> skips a type.
    /// </param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder UseAssemblyAnnotationScanning
    (
        this ApiSchemaBuilder builder,
        Assembly assembly,
        Func<Type, bool>? filter = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(assembly);

        return builder.UseConventions(c => c.AddConvention(new ApiSchemaAssemblyAnnotationScanConvention(assembly, filter)));
    }

    /// <summary>
    ///     Adds the built-in convention that scans an assembly for API configuration implementations.
    /// </summary>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="assembly">The assembly to scan for API configurations.</param>
    /// <param name="filter">
    ///     Optional inclusion predicate. When <see langword="null"/>, all eligible configuration types are
    ///     considered; returning <see langword="false"/> skips a type.
    /// </param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder UseConfigurationsFromAssembly
    (
        this ApiSchemaBuilder builder,
        Assembly assembly,
        Func<Type, bool>? filter = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(assembly);

        return builder.UseConventions
        (
            c => c.AddConvention(new ApiSchemaAssemblyConfigurationDiscoveryConvention(assembly, filter))
        );
    }

    /// <summary>
    ///     Adds the built-in convention that scans the assembly containing <typeparamref name="TMarker"/>
    ///     for API configuration implementations.
    /// </summary>
    /// <typeparam name="TMarker">The type whose assembly contains the configurations to scan.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="filter">
    ///     Optional inclusion predicate. When <see langword="null"/>, all eligible configuration types are
    ///     considered; returning <see langword="false"/> skips a type.
    /// </param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder UseConfigurationsFromAssemblyOf<TMarker>
    (
        this ApiSchemaBuilder builder,
        Func<Type, bool>? filter = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.UseConfigurationsFromAssembly(typeof(TMarker).Assembly, filter);
    }

    /// <summary>
    ///     Adds the built-in convention that scans an assembly and infers API type kinds from CLR
    ///     type reflection.
    /// </summary>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="assembly">The assembly to scan for types.</param>
    /// <param name="filter">
    ///     Optional inclusion predicate. When <see langword="null"/>, all eligible types are
    ///     considered; returning <see langword="false"/> skips a type.
    /// </param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder UseAssemblyTypeInference
    (
        this ApiSchemaBuilder builder,
        Assembly assembly,
        Func<Type, bool>? filter = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(assembly);

        return builder.UseConventions(c => c.AddConvention(new ApiSchemaAssemblyTypeInferenceConvention(assembly, filter)));
    }

    /// <summary>
    ///     Adds the built-in conventions that compose assembly type inference, property discovery,
    ///     enum-value discovery, and property nullability inference to create a complete API schema
    ///     from CLR types.
    /// </summary>
    /// <remarks>
    ///     This is equivalent to composing <see cref="UseAssemblyTypeInference"/>,
    ///     <see cref="UseConfigurationsFromAssembly"/>,
    ///     <see cref="UsePropertyDiscovery"/>, <see cref="UseEnumValueDiscovery"/>, and
    ///     <see cref="UsePropertyNullabilityModifiers"/>. The optional filter is applied while
    ///     scanning the assembly for eligible CLR types and configurations.
    /// </remarks>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="assembly">The assembly to scan for types.</param>
    /// <param name="filter">
    ///     Optional inclusion predicate. When <see langword="null"/>, all eligible types are
    ///     considered; returning <see langword="false"/> skips a type.
    /// </param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder UseAssemblySchemaDiscovery
    (
        this ApiSchemaBuilder builder,
        Assembly assembly,
        Func<Type, bool>? filter = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(assembly);

        return builder.UseConventions
        (
            c => c
                .AddConvention(new ApiSchemaAssemblyTypeInferenceConvention(assembly, filter))
                .AddConvention(new ApiSchemaAssemblyConfigurationDiscoveryConvention(assembly, filter))
                .AddConvention(new ApiEnumTypeEnumValueDiscoveryConvention())
                .AddConvention(new ApiObjectTypePropertyDiscoveryConvention())
                .AddConvention(new ApiPropertyNullabilityModifierConvention())
        );
    }

    /// <summary>
    ///     Adds the built-in convention that converts current schema type, enum-value, and
    ///     property API names to camelCase when their names remain convention-configurable.
    /// </summary>
    /// <param name="builder">The schema builder to configure.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder UseCamelCaseNaming(this ApiSchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.UseConventions(c => c.AddConvention(new ApiNamingCamelCaseConvention()));
    }

    /// <summary>
    ///     Adds the built-in convention that discovers missing values from registered CLR enum
    ///     types.
    /// </summary>
    /// <param name="builder">The schema builder to configure.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder UseEnumValueDiscovery(this ApiSchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.UseConventions(c => c.AddConvention(new ApiEnumTypeEnumValueDiscoveryConvention()));
    }

    /// <summary>
    ///     Adds the built-in convention that discovers public CLR properties and fields on object
    ///     types.
    /// </summary>
    /// <param name="builder">The schema builder to configure.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder UsePropertyDiscovery(this ApiSchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.UseConventions(c => c.AddConvention(new ApiObjectTypePropertyDiscoveryConvention()));
    }

    /// <summary>
    ///     Adds the built-in convention that infers property required/optional modifiers from CLR
    ///     nullability.
    /// </summary>
    /// <param name="builder">The schema builder to configure.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder UsePropertyNullabilityModifiers(this ApiSchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.UseConventions(c => c.AddConvention(new ApiPropertyNullabilityModifierConvention()));
    }

    /// <summary>
    ///     Adds the built-in convention that infers an object type primary key from an <c>Id</c>
    ///     or <c>{ClassName}Id</c> CLR member.
    /// </summary>
    /// <param name="builder">The schema builder to configure.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder UsePrimaryKeyInference(this ApiSchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.UseConventions(c => c.AddConvention(new ApiObjectTypePrimaryKeyInferenceConvention()));
    }
    #endregion
}
