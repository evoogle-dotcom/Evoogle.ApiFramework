// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Convenience extension methods for schema configuration builders.
/// </summary>
public static class ApiBuilderConvenienceExtensions
{
    #region Schema Extensions
    /// <summary>
    ///     Adds an extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The schema builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiSchemaBuilder AddSchemaExtension<TExtension>(this ApiSchemaBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddSchemaExtension(typeof(TExtension), extension);
    #endregion

    #region Named Type Extensions
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

    /// <summary>
    ///     Adds a scalar type extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The scalar type builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiScalarTypeBuilder AddScalarTypeExtension<TExtension>(this ApiScalarTypeBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddScalarTypeExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds a scalar type extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TScalar">The CLR scalar type represented by the builder.</typeparam>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The scalar type builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiScalarTypeBuilder<TScalar> AddScalarTypeExtension<TScalar, TExtension>(this ApiScalarTypeBuilder<TScalar> builder, TExtension extension)
        where TExtension : class
        => builder.AddScalarTypeExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds an object type extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddObjectTypeExtension<TExtension>(this ApiObjectTypeBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddObjectTypeExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds an object type extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddObjectTypeExtension<TObject, TExtension>(this ApiObjectTypeBuilder<TObject> builder, TExtension extension)
        where TExtension : class
        => builder.AddObjectTypeExtension(typeof(TExtension), extension);
    #endregion

    #region Key Builder Extensions
    /// <summary>
    ///     Adds a key type extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The key type builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiKeyTypeBuilder AddKeyTypeExtension<TExtension>(this ApiKeyTypeBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddKeyTypeExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds a key path extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The key path builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiKeyPathBuilder AddKeyPathExtension<TExtension>(this ApiKeyPathBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddKeyPathExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds a key path segment extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The key path segment builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiKeyPathSegmentBuilder AddKeyPathSegmentExtension<TExtension>(this ApiKeyPathSegmentBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddKeyPathSegmentExtension(typeof(TExtension), extension);
    #endregion

    #region Property Extensions
    /// <summary>
    ///     Adds a property extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The property builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiPropertyBuilder AddPropertyExtension<TExtension>(this ApiPropertyBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddPropertyExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Marks this property as required, overriding any nullability-inferred modifier.
    /// </summary>
    /// <param name="builder">The property builder to configure.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiPropertyBuilder AsRequired(this ApiPropertyBuilder builder)
        => builder.WithModifiers(m => m.Required());

    /// <summary>
    ///     Marks this property as optional, overriding any nullability-inferred modifier.
    /// </summary>
    /// <param name="builder">The property builder to configure.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiPropertyBuilder AsOptional(this ApiPropertyBuilder builder)
        => builder.WithModifiers(m => m.Optional());
    #endregion

    #region Object Property Extensions
    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition using <paramref name="name"/> as both the API name and CLR property name.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="name">The API and CLR property name.</param>
    /// <param name="configure">Optional callback to configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddProperty(this ApiObjectTypeBuilder builder, string name, Action<ApiPropertyBuilder>? configure = null)
        => builder.AddProperty(name, name, configure);

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition using <paramref name="name"/> as both the API name and CLR property name.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="name">The API and CLR property name.</param>
    /// <param name="configure">Optional callback to configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddProperty<TObject>(this ApiObjectTypeBuilder<TObject> builder, string name, Action<ApiPropertyBuilder>? configure = null)
    {
        builder.AddProperty(name, name, configure);
        return builder;
    }

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition marked as required.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="name">The API and CLR property name.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddRequiredProperty(this ApiObjectTypeBuilder builder, string name, Action<ApiPropertyBuilder>? configure = null)
        => builder.AddRequiredProperty(name, name, configure);

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition marked as required.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The API property name.</param>
    /// <param name="clrName">The CLR property name.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddRequiredProperty(this ApiObjectTypeBuilder builder, string apiName, string clrName, Action<ApiPropertyBuilder>? configure = null)
        => builder.AddProperty(apiName, clrName, b => { b.AsRequired(); configure?.Invoke(b); });

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition marked as required.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="name">The API and CLR property name.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddRequiredProperty<TObject>(this ApiObjectTypeBuilder<TObject> builder, string name, Action<ApiPropertyBuilder>? configure = null)
        => builder.AddRequiredProperty(name, name, configure);

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition marked as required.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The API property name.</param>
    /// <param name="clrName">The CLR property name.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddRequiredProperty<TObject>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        string apiName,
        string clrName,
        Action<ApiPropertyBuilder>? configure = null
    )
    {
        builder.AddProperty(apiName, clrName, b => { b.AsRequired(); configure?.Invoke(b); });
        return builder;
    }

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition marked as optional.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="name">The API and CLR property name.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddOptionalProperty(this ApiObjectTypeBuilder builder, string name, Action<ApiPropertyBuilder>? configure = null)
        => builder.AddOptionalProperty(name, name, configure);

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition marked as optional.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The API property name.</param>
    /// <param name="clrName">The CLR property name.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddOptionalProperty(this ApiObjectTypeBuilder builder, string apiName, string clrName, Action<ApiPropertyBuilder>? configure = null)
        => builder.AddProperty(apiName, clrName, b => { b.AsOptional(); configure?.Invoke(b); });

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition marked as optional.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="name">The API and CLR property name.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddOptionalProperty<TObject>(this ApiObjectTypeBuilder<TObject> builder, string name, Action<ApiPropertyBuilder>? configure = null)
        => builder.AddOptionalProperty(name, name, configure);

    /// <summary>
    ///     Adds an <see cref="ApiProperty"/> definition marked as optional.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The API property name.</param>
    /// <param name="clrName">The CLR property name.</param>
    /// <param name="configure">Optional callback to further configure the added property.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddOptionalProperty<TObject>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        string apiName,
        string clrName,
        Action<ApiPropertyBuilder>? configure = null
    )
    {
        builder.AddProperty(apiName, clrName, b => { b.AsOptional(); configure?.Invoke(b); });
        return builder;
    }
    #endregion

    #region Object Relationship Extensions
    /// <summary>
    ///     Registers a one-to-one relationship at the schema level while authoring this object type.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddOneToOneRelationship(this ApiObjectTypeBuilder builder, string apiName, Action<ApiRelationshipOneToOneBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        configure(builder.GetOrAddOneToOneRelationshipBuilderCore(apiName));
        return builder;
    }

    /// <summary>
    ///     Registers a one-to-one relationship at the schema level while authoring this object type.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddOneToOneRelationship<TObject>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        string apiName,
        Action<ApiRelationshipOneToOneBuilder> configure
    )
    {
        AddOneToOneRelationship((ApiObjectTypeBuilder)builder, apiName, configure);
        return builder;
    }

    /// <summary>
    ///     Registers a one-to-one relationship at the schema level while authoring this object type.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddOneToOneRelationship(this ApiObjectTypeBuilder builder, string apiName, IApiRelationshipOneToOneConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configuration);

        configuration.Configure(builder.GetOrAddOneToOneRelationshipBuilderCore(apiName));
        return builder;
    }

    /// <summary>
    ///     Registers a one-to-one relationship at the schema level while authoring this object type.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddOneToOneRelationship<TObject>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        string apiName,
        IApiRelationshipOneToOneConfiguration configuration
    )
    {
        AddOneToOneRelationship((ApiObjectTypeBuilder)builder, apiName, configuration);
        return builder;
    }

    /// <summary>
    ///     Registers a one-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddOneToManyRelationship(this ApiObjectTypeBuilder builder, string apiName, Action<ApiRelationshipOneToManyBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        configure(builder.GetOrAddOneToManyRelationshipBuilderCore(apiName));
        return builder;
    }

    /// <summary>
    ///     Registers a one-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddOneToManyRelationship<TObject>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        string apiName,
        Action<ApiRelationshipOneToManyBuilder> configure
    )
    {
        AddOneToManyRelationship((ApiObjectTypeBuilder)builder, apiName, configure);
        return builder;
    }

    /// <summary>
    ///     Registers a one-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddOneToManyRelationship(this ApiObjectTypeBuilder builder, string apiName, IApiRelationshipOneToManyConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configuration);

        configuration.Configure(builder.GetOrAddOneToManyRelationshipBuilderCore(apiName));
        return builder;
    }

    /// <summary>
    ///     Registers a one-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddOneToManyRelationship<TObject>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        string apiName,
        IApiRelationshipOneToManyConfiguration configuration
    )
    {
        AddOneToManyRelationship((ApiObjectTypeBuilder)builder, apiName, configuration);
        return builder;
    }

    /// <summary>
    ///     Registers a many-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddManyToManyRelationship(this ApiObjectTypeBuilder builder, string apiName, Action<ApiRelationshipManyToManyBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        configure(builder.GetOrAddManyToManyRelationshipBuilderCore(apiName));
        return builder;
    }

    /// <summary>
    ///     Registers a many-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddManyToManyRelationship<TObject>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        string apiName,
        Action<ApiRelationshipManyToManyBuilder> configure
    )
    {
        AddManyToManyRelationship((ApiObjectTypeBuilder)builder, apiName, configure);
        return builder;
    }

    /// <summary>
    ///     Registers a many-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddManyToManyRelationship(this ApiObjectTypeBuilder builder, string apiName, IApiRelationshipManyToManyConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configuration);

        configuration.Configure(builder.GetOrAddManyToManyRelationshipBuilderCore(apiName));
        return builder;
    }

    /// <summary>
    ///     Registers a many-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddManyToManyRelationship<TObject>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        string apiName,
        IApiRelationshipManyToManyConfiguration configuration
    )
    {
        AddManyToManyRelationship((ApiObjectTypeBuilder)builder, apiName, configuration);
        return builder;
    }
    #endregion

    #region Relationship Extensions
    /// <summary>
    ///     Adds a relationship extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The relationship builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiRelationshipOneToOneBuilder AddRelationshipExtension<TExtension>(this ApiRelationshipOneToOneBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddRelationshipExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds a relationship extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The relationship builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiRelationshipOneToManyBuilder AddRelationshipExtension<TExtension>(this ApiRelationshipOneToManyBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddRelationshipExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds a relationship extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The relationship builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiRelationshipManyToManyBuilder AddRelationshipExtension<TExtension>(this ApiRelationshipManyToManyBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddRelationshipExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds a principal end extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The principal end builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiRelationshipPrincipalEndBuilder AddRelationshipPrincipalEndExtension<TExtension>(this ApiRelationshipPrincipalEndBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddRelationshipPrincipalEndExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds a dependent end extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The dependent end builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiRelationshipDependentEndBuilder AddRelationshipDependentEndExtension<TExtension>(this ApiRelationshipDependentEndBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddRelationshipDependentEndExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds a dependent end extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TDependent">The CLR dependent type represented by the builder.</typeparam>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The dependent end builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiRelationshipDependentEndBuilder<TDependent> AddRelationshipDependentEndExtension<TDependent, TExtension>
    (
        this ApiRelationshipDependentEndBuilder<TDependent> builder,
        TExtension extension
    )
        where TExtension : class
        => builder.AddRelationshipDependentEndExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds an association extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The association builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiRelationshipAssociationBuilder AddRelationshipAssociationExtension<TExtension>(this ApiRelationshipAssociationBuilder builder, TExtension extension)
        where TExtension : class
        => builder.AddRelationshipAssociationExtension(typeof(TExtension), extension);

    /// <summary>
    ///     Adds an association extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="TAssociation">The CLR association type represented by the builder.</typeparam>
    /// <typeparam name="TExtension">The extension value type.</typeparam>
    /// <param name="builder">The association builder to configure.</param>
    /// <param name="extension">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiRelationshipAssociationBuilder<TAssociation> AddRelationshipAssociationExtension<TAssociation, TExtension>
    (
        this ApiRelationshipAssociationBuilder<TAssociation> builder,
        TExtension extension
    )
        where TExtension : class
        => builder.AddRelationshipAssociationExtension(typeof(TExtension), extension);
    #endregion
}
