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
public class ApiEnumTypeBuilder(Type clrType, ApiSchemaBuilderContext context)
    : ApiNamedTypeBuilder<ApiEnumTypeBuilder>(clrType, context)
{
    #region Fields
    private readonly List<ApiEnumValue> _values = [];
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiEnumTypeBuilder AddEnumTypeExtension(Type extensionType, object extension)
    {
        return this.AddExtension(extensionType, extension);
    }
    #endregion

    #region AddValue Methods
    /// <summary>
    ///     Adds an <see cref="ApiEnumValue"/> definition to the enumeration.
    /// </summary>
    /// <param name="apiName">The API name of the enumeration value.</param>
    /// <param name="clrName">The CLR name of the enumeration value.</param>
    /// <param name="clrOrdinal">The CLR ordinal of the enumeration value.</param>
    /// <returns>The current builder instance.</returns>
    public ApiEnumTypeBuilder AddValue(string apiName, string clrName, int clrOrdinal)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        ArgumentException.ThrowIfNullOrWhiteSpace(clrName, nameof(clrName));

        _values.Add(new ApiEnumValue(apiName, clrName, clrOrdinal));
        return this;
    }
    #endregion

    #region Build Methods
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
