// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Key;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     Provides common <see cref="ApiKeyPartNameBuildDelegate"/> implementations.
/// </summary>
internal static class ApiKeyPartNameBuilders
{
    #region Methods
    /// <summary>
    ///     Resolves a part name builder from the configured public option.
    /// </summary>
    /// <param name="builder">The configured part name builder option.</param>
    /// <returns>The matching part name builder implementation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="builder"/> is unsupported.</exception>
    internal static ApiKeyPartNameBuildDelegate Resolve(ApiKeyPartNameBuilder builder) => builder switch
    {
        ApiKeyPartNameBuilder.None => None,
        ApiKeyPartNameBuilder.ClrPathOnly => ClrPathOnly,
        ApiKeyPartNameBuilder.ClrRootAndPath => ClrRootAndPath,
        _ => throw new ArgumentOutOfRangeException(nameof(builder), $"Unsupported {nameof(ApiKeyPartNameBuilder)}: {builder}.")
    };

    /// <summary>
    ///     Creates unnamed/positional key parts.
    /// </summary>
    /// <param name="context">The part naming context.</param>
    /// <returns><see langword="null"/>.</returns>
    private static string? None(ApiKeyPartNameContext context) => null;

    /// <summary>
    ///     Creates names from only the dotted CLR property path.
    /// </summary>
    /// <param name="context">The part naming context.</param>
    /// <returns>A name like <c>Id</c> or <c>Customer.Id</c>.</returns>
    private static string? ClrPathOnly(ApiKeyPartNameContext context)
        => string.Join(".", context.ApiKeyPath.ApiSegments.Select(static s => s.ClrPropertyName));

    /// <summary>
    ///     Creates names from the CLR root type and dotted CLR property path.
    /// </summary>
    /// <param name="context">The part naming context.</param>
    /// <returns>A name like <c>Customer.Id</c>.</returns>
    private static string? ClrRootAndPath(ApiKeyPartNameContext context)
    {
        var path = context.ApiKeyPath;
        var pathTypeName = path.ClrRootType.SafeToName();
        var pathSegmentsDelimited = string.Join(".", path.ApiSegments.Select(static s => s.ClrPropertyName));

        return string.IsNullOrEmpty(pathSegmentsDelimited) ? pathTypeName : $"{pathTypeName}.{pathSegmentsDelimited}";
    }
    #endregion
}

