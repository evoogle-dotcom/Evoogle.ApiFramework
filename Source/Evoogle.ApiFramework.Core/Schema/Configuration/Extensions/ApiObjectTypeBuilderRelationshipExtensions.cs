// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Relationship convenience extension methods for <see cref="ApiObjectTypeBuilder"/>.
/// </summary>
public static class ApiObjectTypeBuilderRelationshipExtensions
{
    #region AddOneToOneRelationship Methods
    /// <summary>
    ///     Registers a one-to-one relationship at the schema level while authoring this object type.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddOneToOneRelationship(this ApiObjectTypeBuilder builder, string apiName, Action<ApiRelationshipOneToOneBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        builder.AddOneToOneRelationshipCore(apiName, configure);
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
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddOneToOneRelationship(this ApiObjectTypeBuilder builder, IApiRelationshipOneToOneConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        builder.AddOneToOneRelationshipCore(configuration.ApiName, configuration.Configure);
        return builder;
    }

    /// <summary>
    ///     Registers a one-to-one relationship at the schema level while authoring this object type.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddOneToOneRelationship<TObject>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        IApiRelationshipOneToOneConfiguration configuration
    )
    {
        AddOneToOneRelationship((ApiObjectTypeBuilder)builder, configuration);
        return builder;
    }
    #endregion

    #region AddOneToManyRelationship Methods
    /// <summary>
    ///     Registers a one-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddOneToManyRelationship(this ApiObjectTypeBuilder builder, string apiName, Action<ApiRelationshipOneToManyBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        builder.AddOneToManyRelationshipCore(apiName, configure);
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
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddOneToManyRelationship(this ApiObjectTypeBuilder builder, IApiRelationshipOneToManyConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        builder.AddOneToManyRelationshipCore(configuration.ApiName, configuration.Configure);
        return builder;
    }

    /// <summary>
    ///     Registers a one-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddOneToManyRelationship<TObject>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        IApiRelationshipOneToManyConfiguration configuration
    )
    {
        AddOneToManyRelationship((ApiObjectTypeBuilder)builder, configuration);
        return builder;
    }
    #endregion

    #region AddManyToManyRelationship Methods
    /// <summary>
    ///     Registers a many-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="configure">Callback to configure the relationship.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddManyToManyRelationship(this ApiObjectTypeBuilder builder, string apiName, Action<ApiRelationshipManyToManyBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentNullException.ThrowIfNull(configure);

        builder.AddManyToManyRelationshipCore(apiName, configure);
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
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder AddManyToManyRelationship(this ApiObjectTypeBuilder builder, IApiRelationshipManyToManyConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        builder.AddManyToManyRelationshipCore(configuration.ApiName, configuration.Configure);
        return builder;
    }

    /// <summary>
    ///     Registers a many-to-many relationship at the schema level while authoring this object type.
    /// </summary>
    /// <typeparam name="TObject">The CLR object type represented by the builder.</typeparam>
    /// <param name="builder">The object type builder to configure.</param>
    /// <param name="configuration">The configuration implementation.</param>
    /// <returns>The current builder instance.</returns>
    public static ApiObjectTypeBuilder<TObject> AddManyToManyRelationship<TObject>
    (
        this ApiObjectTypeBuilder<TObject> builder,
        IApiRelationshipManyToManyConfiguration configuration
    )
    {
        AddManyToManyRelationship((ApiObjectTypeBuilder)builder, configuration);
        return builder;
    }
    #endregion
}
