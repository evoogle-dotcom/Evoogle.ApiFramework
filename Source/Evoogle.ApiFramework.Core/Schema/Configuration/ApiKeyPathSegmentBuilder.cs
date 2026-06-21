// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure a single <see cref="ApiKeyPathSegment"/>.
/// </summary>
public class ApiKeyPathSegmentBuilder : ExtensionBuilder<ApiKeyPathSegmentBuilder>
{
    #region Fields
    private readonly string _clrPropertyName;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes an <see cref="ApiKeyPathSegmentBuilder"/> with the specified CLR property name.
    /// </summary>
    /// <param name="clrPropertyName">The CLR property name for this navigation step.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrPropertyName"/> is <c>null</c>, empty, or whitespace.</exception>
    public ApiKeyPathSegmentBuilder(string clrPropertyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrPropertyName);
        _clrPropertyName = clrPropertyName;
    }
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates a builder for a segment with the specified CLR property name.
    /// </summary>
    /// <param name="clrPropertyName">The CLR property name for this navigation step.</param>
    /// <returns>A new <see cref="ApiKeyPathSegmentBuilder"/> for the specified property name.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrPropertyName"/> is <c>null</c>, empty, or whitespace.</exception>
    public static ApiKeyPathSegmentBuilder For(string clrPropertyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrPropertyName);

        return new(clrPropertyName);
    }
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiKeyPathSegmentBuilder AddKeyPathSegmentExtension(Type type, object extension)
    {
        return this.AddExtension(type, extension);
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiKeyPathSegment"/> configured by this builder.
    /// </summary>
    internal ApiKeyPathSegment Build()
    {
        var segment = new ApiKeyPathSegment(_clrPropertyName);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            segment.Extensions = extensions;
        }

        return segment;
    }
    #endregion
}
