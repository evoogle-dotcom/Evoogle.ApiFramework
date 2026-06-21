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
    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiScalarTypeBuilder AddScalarTypeExtension(Type type, object extension)
    {
        return this.AddExtension(type, extension);
    }
    #endregion

    #region Build Methods
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
