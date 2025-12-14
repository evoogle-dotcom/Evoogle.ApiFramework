// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Concurrent;
using System.Globalization;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Default implementation of <see cref="IApiIdTypeDetectionStrategy"/> that maps common CLR types
///     to their corresponding <see cref="Identity.ApiId"/> scalar types with thread-safe caching.
/// </summary>
/// <remarks>
///     <para>
///         This strategy uses a singleton pattern with a concurrent dictionary cache for performance.
///         Detection results are memoized per <see cref="ApiProperty"/> instance.
///     </para>
///     <para>
///         Type mapping rules:
///         <list type="bullet">
///             <item><description><see cref="int"/> → <see cref="int"/></description></item>
///             <item><description><see cref="long"/> → <see cref="long"/></description></item>
///             <item><description><see cref="Guid"/> → <see cref="Guid"/></description></item>
///             <item><description><see cref="Ulid"/> → <see cref="Ulid"/></description></item>
///             <item><description><see cref="CultureInfo"/> → <see cref="CultureInfo"/></description></item>
///             <item><description>All other types → <see cref="string"/> (fallback)</description></item>
///         </list>
///     </para>
/// </remarks>
public sealed class DefaultApiIdTypeDetectionStrategy : IApiIdTypeDetectionStrategy
{
    #region Fields
    private static readonly Lazy<DefaultApiIdTypeDetectionStrategy> _instance = new(() => new DefaultApiIdTypeDetectionStrategy());
    private readonly ConcurrentDictionary<ApiProperty, Type> _cache = new();
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the singleton instance of the default type detection strategy.
    /// </summary>
    public static DefaultApiIdTypeDetectionStrategy Instance => _instance.Value;
    #endregion

    #region Constructors
    /// <summary>
    ///     Private constructor to enforce singleton pattern.
    /// </summary>
    private DefaultApiIdTypeDetectionStrategy()
    {
    }
    #endregion

    #region IApiIdTypeDetectionStrategy Methods
    /// <inheritdoc />
    public Type DetectTargetType(ApiProperty property)
    {
        ArgumentNullException.ThrowIfNull(property);

        return _cache.GetOrAdd(property, static p =>
        {
            var clrType = p.ApiType.ClrType;

            // Map common CLR types to ApiId-compatible scalar types
            if (clrType == typeof(int))
            {
                return typeof(int);
            }

            if (clrType == typeof(long))
            {
                return typeof(long);
            }

            if (clrType == typeof(Guid))
            {
                return typeof(Guid);
            }

            if (clrType == typeof(Ulid))
            {
                return typeof(Ulid);
            }

            if (clrType == typeof(CultureInfo))
            {
                return typeof(CultureInfo);
            }

            // Default fallback to string for all other types
            return typeof(string);
        });
    }
    #endregion
}
