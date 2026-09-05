// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Key;

/// <summary>
///     Parse convenience extension methods for <see cref="ApiKey"/>.
/// </summary>
public static class ApiKeyParseExtensions
{
    #region Parse Methods
    /// <summary>
    ///     Parses <paramref name="text"/> into the specified <paramref name="kind"/> or throws.
    /// </summary>
    /// <param name="kind">The expected kind.</param>
    /// <param name="text">The textual representation.</param>
    /// <returns>The parsed key.</returns>
    /// <exception cref="ApiKeyException">Thrown if parsing fails.</exception>
    public static ApiKey Parse(this ApiKeyKind kind, string text)
        => ApiKey.TryParse(kind, text, out var apiKey)
            ? apiKey
            : throw new ApiKeyException($"Text '{text}' is not a valid {kind} id.");

    /// <summary>
    ///     Parses <paramref name="text"/> inferring key kind or throws.
    /// </summary>
    /// <param name="text">The textual representation.</param>
    /// <returns>The parsed key.</returns>
    /// <exception cref="ApiKeyException">Thrown if text is null/empty or invalid.</exception>
    public static ApiKey Parse(this string text)
        => ApiKey.TryParse(text, out var apiKey) ? apiKey : throw new ApiKeyException("Text is null/empty.");

    /// <summary>
    ///     Parses <paramref name="text"/> using the specified <paramref name="provider"/>.
    /// </summary>
    /// <param name="text">The textual representation.</param>
    /// <param name="provider">The format provider.</param>
    /// <returns>The parsed key.</returns>
    /// <exception cref="ApiKeyException">Thrown if text is null/empty or invalid.</exception>
    public static ApiKey Parse(this string text, IFormatProvider? provider)
    {
        if (ApiKey.TryParse(text, provider, out var apiKey))
        {
            return apiKey;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ApiKeyException("Text is null/empty.");
        }

        throw new ApiKeyException($"Text '{text}' is not a valid {nameof(ApiKey)}.");
    }

    /// <summary>
    ///     Parses <paramref name="text"/> using the specified <paramref name="provider"/>.
    /// </summary>
    /// <param name="text">The textual representation.</param>
    /// <param name="provider">The format provider.</param>
    /// <returns>The parsed key.</returns>
    /// <exception cref="ApiKeyException">Thrown if text is null/empty or invalid.</exception>
    public static ApiKey Parse(this ReadOnlySpan<char> text, IFormatProvider? provider)
    {
        if (ApiKey.TryParse(text, provider, out var apiKey))
        {
            return apiKey;
        }

        if (text.IsEmpty)
        {
            throw new ApiKeyException("Text is null/empty.");
        }

        throw new ApiKeyException($"Text '{text}' is not a valid {nameof(ApiKey)}.");
    }
    #endregion
}
