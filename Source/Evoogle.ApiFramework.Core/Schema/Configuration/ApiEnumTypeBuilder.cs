// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to define an <see cref="ApiEnumType"/> within an <see cref="ApiSchemaBuilder"/>.
/// </summary>
/// <param name="clrType">The CLR enum type being described.</param>
/// <param name="context">The shared builder context.</param>
public sealed class ApiEnumTypeBuilder(Type clrType, ApiSchemaBuilderContext context)
    : ApiNamedTypeBuilder<ApiEnumTypeBuilder>(clrType, context)
{
    #region Fields
    private readonly List<ApiEnumValue> _values = [];
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiEnumTypeBuilder AddEnumExtension(Type type, object value)
    {
        base.AddExtension(type, value);
        return this;
    }

    /// <summary>
    ///     Adds an extension value keyed by its own type.
    /// </summary>
    /// <typeparam name="T">The extension value type.</typeparam>
    /// <param name="value">The extension value.</param>
    /// <returns>The current builder instance.</returns>
    public ApiEnumTypeBuilder AddEnumExtension<T>(T value) where T : notnull
        => this.AddEnumExtension(typeof(T), value);

    /// <summary>
    ///     Adds an <see cref="ApiEnumValue"/> definition to the enumeration.
    /// </summary>
    /// <param name="apiName">The API name of the enumeration value.</param>
    /// <param name="clrName">The CLR name of the enumeration value.</param>
    /// <param name="clrOrdinal">The CLR ordinal of the enumeration value.</param>
    /// <returns>The current builder instance.</returns>
    public ApiEnumTypeBuilder AddValue(string apiName, string clrName, int clrOrdinal)
    {
        _values.Add(new ApiEnumValue(apiName, clrName, clrOrdinal));
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="ApiEnumType"/> using the configured values.
    /// </summary>
    /// <returns>The constructed <see cref="ApiEnumType"/>.</returns>
    internal ApiEnumType Build()
    {
        var apiName = this.ApiName;
        var apiEnumValues = _values;
        var clrEnumType = this.ClrType;

        var apiEnumType = new ApiEnumType
        (
            apiName: apiName,
            apiEnumValues: apiEnumValues,
            clrEnumType: clrEnumType
        );

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiEnumType.Extensions = extensions;
        }

        return apiEnumType;
    }
    #endregion
}
