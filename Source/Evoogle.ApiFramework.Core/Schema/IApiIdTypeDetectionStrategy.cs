// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Defines a strategy for detecting the target type for <see cref="Identity.ApiId"/> scalar conversion
///     based on an <see cref="ApiProperty"/>'s CLR type.
/// </summary>
/// <remarks>
///     Implementations should be thread-safe and designed for subclassing extensibility.
///     The detected type is used with <see cref="Coercion.TypeCoercion"/> to convert
///     property values to appropriate <see cref="Identity.ApiId"/> scalar types.
/// </remarks>
public interface IApiIdTypeDetectionStrategy
{
    /// <summary>
    ///     Detects the target CLR type for converting the specified property's value to an <see cref="Identity.ApiId"/> scalar.
    /// </summary>
    /// <param name="property">The property whose type should be detected.</param>
    /// <returns>
    ///     The target CLR type for type coercion. Common types include <see cref="int"/>, <see cref="long"/>,
    ///     <see cref="System.Guid"/>, <see cref="System.Ulid"/>, <see cref="System.Globalization.CultureInfo"/>,
    ///     or <see cref="string"/> as a fallback.
    /// </returns>
    Type DetectTargetType(ApiProperty property);
}
