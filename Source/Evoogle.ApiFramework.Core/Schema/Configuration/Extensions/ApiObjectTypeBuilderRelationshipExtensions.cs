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
}
