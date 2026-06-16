// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Convenience extension methods for <see cref="ApiObjectTypeBuilder"/>.
/// </summary>
public static class ApiObjectTypeBuilderExtensions
{
    #region Extension Methods
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

    #region AddProperty Methods
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
    #endregion

    #region AddRequiredProperty Methods
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
    #endregion

    #region AddOptionalProperty Methods
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
}
