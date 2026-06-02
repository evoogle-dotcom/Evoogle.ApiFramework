// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Key;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Builds the optional part name used when materializing one <see cref="ApiKeyPart"/>.
/// </summary>
/// <param name="context">The part naming context.</param>
/// <returns>
///     The part name to use, or <see langword="null"/> to create an unnamed/positional key part.
/// </returns>
public delegate string? ApiKeyPartNameBuilder(ApiKeyPartNameContext context);

/// <summary>
///     Provides metadata to an <see cref="ApiKeyPartNameBuilder"/> while materializing an <see cref="ApiKey"/>.
/// </summary>
/// <param name="ApiKeyType">The key type being materialized.</param>
/// <param name="ApiKeyPath">The key path for the current part.</param>
/// <param name="PartIndex">The zero-based index of the current part.</param>
public readonly record struct ApiKeyPartNameContext(ApiKeyType ApiKeyType, ApiKeyPath ApiKeyPath, int PartIndex);

/// <summary>
///     Provides common <see cref="ApiKeyPartNameBuilder"/> implementations.
/// </summary>
public static class ApiKeyPartNameBuilders
{
    #region Methods
    /// <summary>
    ///     Creates unnamed/positional key parts.
    /// </summary>
    /// <param name="context">The part naming context.</param>
    /// <returns><see langword="null"/>.</returns>
    public static string? None(ApiKeyPartNameContext context) => null;

    /// <summary>
    ///     Creates names from only the dotted CLR property path.
    /// </summary>
    /// <param name="context">The part naming context.</param>
    /// <returns>A name like <c>Id</c> or <c>Customer.Id</c>.</returns>
    public static string? ClrPathOnly(ApiKeyPartNameContext context)
        => string.Join(".", context.ApiKeyPath.ApiSegments.Select(static s => s.ClrPropertyName));

    /// <summary>
    ///     Creates names from the CLR root type and dotted CLR property path.
    /// </summary>
    /// <param name="context">The part naming context.</param>
    /// <returns>A name like <c>Customer.Id</c>.</returns>
    public static string? ClrRootAndPath(ApiKeyPartNameContext context)
    {
        var path = context.ApiKeyPath;
        var pathTypeName = path.ClrRootType.SafeToName();
        var pathSegmentsDelimited = string.Join(".", path.ApiSegments.Select(static s => s.ClrPropertyName));

        return string.IsNullOrEmpty(pathSegmentsDelimited) ? pathTypeName : $"{pathTypeName}.{pathSegmentsDelimited}";
    }
    #endregion
}

