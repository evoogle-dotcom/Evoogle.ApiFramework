// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an <see cref="ApiScalarType"/>.
/// </summary>
/// <param name="clrType">The CLR scalar type.</param>
/// <param name="context">The shared builder context.</param>
public class ApiScalarTypeBuilder(Type clrType, ApiSchemaBuilderContext context)
    : ApiNamedTypeBuilder<ApiScalarTypeBuilder>(clrType, context)
{
    #region Builder Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiScalarTypeBuilder AddScalarExtension(Type type, object value)
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
    public ApiScalarTypeBuilder AddScalarExtension<T>(T value) where T : notnull
        => this.AddScalarExtension(typeof(T), value);

    /// <summary>
    ///     Builds the <see cref="ApiScalarType"/> using the configured settings.
    /// </summary>
    /// <returns>The constructed <see cref="ApiScalarType"/>.</returns>
    internal ApiScalarType Build()
    {
        var apiName = this.ApiName;
        var clrScalarType = this.ClrType;

        var apiScalarType = new ApiScalarType
        (
            apiName: apiName,
            clrScalarType: clrScalarType
        );

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiScalarType.Extensions = extensions;
        }

        return apiScalarType;
    }
    #endregion
}
