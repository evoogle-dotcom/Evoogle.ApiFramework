// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure an individual <see cref="ApiIdentityPart"/> within an <see cref="ApiIdentityBuilder"/>.
/// </summary>
/// <param name="apiKind">The kind of identity part to build.</param>
/// <param name="clrPropertyName">The CLR property name for property-based parts (<see cref="ApiIdentityPartKind.Scalar"/> and <see cref="ApiIdentityPartKind.Nested"/>). <see langword="null"/> for <see cref="ApiIdentityPartKind.Owner"/> parts.</param>
/// <param name="apiIdentityName">An optional explicit identity name for <see cref="ApiIdentityPartKind.Nested"/> and <see cref="ApiIdentityPartKind.Owner"/> parts.</param>
/// <param name="clrScalarTypeHint">An optional CLR type hint used to override scalar type resolution for <see cref="ApiIdentityPartKind.Scalar"/> parts.</param>
public class ApiIdentityPartBuilder
(
    ApiIdentityPartKind apiKind,
    string? clrPropertyName,
    string? apiIdentityName,
    Type? clrScalarTypeHint
)
: ExtensionBuilder<ApiIdentityPartBuilder>
{
    #region Fields
    private readonly ApiIdentityPartKind _apiKind = apiKind;
    private readonly string? _clrPropertyName = clrPropertyName;
    private readonly string? _apiIdentityName = apiIdentityName;
    private readonly Type? _clrScalarTypeHint = clrScalarTypeHint;
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="value">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiIdentityPartBuilder AddIdentityPartExtension(Type type, object value)
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
    public ApiIdentityPartBuilder AddIdentityPartExtension<T>(T value) where T : notnull
        => this.AddIdentityPartExtension(typeof(T), value);
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiIdentityPart"/> configured by this builder.
    /// </summary>
    /// <returns>A fully constructed <see cref="ApiIdentityPart"/> instance.</returns>
    internal ApiIdentityPart Build()
    {
        ApiIdentityPart apiIdentityPart = _apiKind switch
        {
            ApiIdentityPartKind.Scalar => new ApiIdentityScalarPart(_clrPropertyName!, _clrScalarTypeHint),
            ApiIdentityPartKind.Nested => new ApiIdentityNestedPart(_clrPropertyName!, _apiIdentityName),
            ApiIdentityPartKind.Owner => new ApiIdentityOwnerPart(_apiIdentityName),
            _ => throw new ApiSchemaException($"Unsupported API identity part kind: {_apiKind}"),
        };

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            apiIdentityPart.Extensions = extensions;
        }

        return apiIdentityPart;
    }
    #endregion
}
