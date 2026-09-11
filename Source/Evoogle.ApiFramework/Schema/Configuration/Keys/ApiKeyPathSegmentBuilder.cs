// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Keys;

namespace Evoogle.ApiFramework.Schema.Configuration.Keys;

/// <summary>
///     Fluent builder used to configure a single <see cref="ApiKeyPathSegment"/>.
/// </summary>
public class ApiKeyPathSegmentBuilder : ExtensionBuilder<ApiKeyPathSegmentBuilder>
{
    #region Fields
    private readonly string _clrMemberName;
    #endregion

    #region Properties
    /// <summary>Gets the CLR member name for this segment.</summary>
    internal string ClrMemberName => _clrMemberName;
    #endregion

    #region Constructors
    /// <summary>
    ///     Creates an <see cref="ApiKeyPathSegmentBuilder"/> with the specified CLR member name.
    /// </summary>
    /// <param name="clrMemberName">The CLR member name for this navigation step.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrMemberName"/> is not one CLR member name.</exception>
    public ApiKeyPathSegmentBuilder(string clrMemberName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrMemberName);

        if (clrMemberName.Contains('.'))
        {
            throw new ArgumentException
            (
                "A key path segment must contain exactly one CLR member name and cannot contain a dot.",
                nameof(clrMemberName)
            );
        }

        _clrMemberName = clrMemberName;
    }
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates a builder for a segment with the specified CLR member name.
    /// </summary>
    /// <param name="clrMemberName">The CLR member name for this navigation step.</param>
    /// <returns>A new <see cref="ApiKeyPathSegmentBuilder"/> for the specified member name.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrMemberName"/> is not one CLR member name.</exception>
    public static ApiKeyPathSegmentBuilder For(string clrMemberName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrMemberName);

        return new(clrMemberName);
    }
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiKeyPathSegmentBuilder AddKeyPathSegmentExtension(Type extensionType, object extension)
    {
        return this.AddExtension(extensionType, extension);
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiKeyPathSegment"/> configured by this builder.
    /// </summary>
    internal ApiKeyPathSegment Build()
    {
        var segment = new ApiKeyPathSegment(_clrMemberName);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            segment.AttachExtensions(extensions);
        }

        return segment;
    }
    #endregion
}
