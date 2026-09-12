// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Key.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal static class ApiKeyPartNameFormatters
{
    #region Methods
    /// <summary>
    ///     Resolves a part name formatter from the configured public format.
    /// </summary>
    /// <param name="format">The configured part name format.</param>
    /// <returns>The matching part name formatter implementation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="format"/> is unsupported.</exception>
    internal static ApiKeyPartNameFormatterDelegate Resolve(ApiKeyPartNameFormat format) => format switch
    {
        ApiKeyPartNameFormat.None => None,
        ApiKeyPartNameFormat.ClrPathOnly => ClrPathOnly,
        ApiKeyPartNameFormat.ClrRootAndPath => ClrRootAndPath,
        _ => throw new ArgumentOutOfRangeException(nameof(format), $"Unsupported {nameof(ApiKeyPartNameFormat)}: {format}.")
    };

    /// <summary>
    ///     Creates unnamed/positional key parts.
    /// </summary>
    /// <param name="context">The part naming context.</param>
    /// <returns><see langword="null"/>.</returns>
    private static string? None(ApiKeyPartNameContext context) => null;

    /// <summary>
    ///     Creates names from only the dotted CLR member path.
    /// </summary>
    /// <param name="context">The part naming context.</param>
    /// <returns>A name like <c>Id</c> or <c>Customer.Id</c>.</returns>
    private static string? ClrPathOnly(ApiKeyPartNameContext context)
        => string.Join(".", context.ApiKeyPath.ApiSegments.Select(static s => s.ClrMemberName));

    /// <summary>
    ///     Creates names from the CLR root type and dotted CLR member path.
    /// </summary>
    /// <param name="context">The part naming context.</param>
    /// <returns>A name like <c>Customer.Id</c>.</returns>
    private static string? ClrRootAndPath(ApiKeyPartNameContext context)
    {
        var path = context.ApiKeyPath;
        var pathTypeName = path.ClrRootType.SafeToName();
        var pathSegmentsDelimited = string.Join(".", path.ApiSegments.Select(static s => s.ClrMemberName));

        return string.IsNullOrEmpty(pathSegmentsDelimited) ? pathTypeName : $"{pathTypeName}.{pathSegmentsDelimited}";
    }
    #endregion
}
