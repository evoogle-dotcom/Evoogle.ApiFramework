// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Key;

/// <summary>
///     Fluent builder used to configure a single <see cref="ApiKeyPathSegment"/>.
/// </summary>
public class ApiKeyPathSegmentBuilder : ExtensionBuilder<ApiKeyPathSegmentBuilder>
{
    #region Fields
    private readonly ApiPropertyReference _apiPropertyReference;
    #endregion

    #region Properties
    /// <summary>Gets the property reference for this segment.</summary>
    internal ApiPropertyReference ApiPropertyReference => _apiPropertyReference;
    #endregion

    #region Constructors
    /// <summary>
    ///     Creates an <see cref="ApiKeyPathSegmentBuilder"/> with the specified CLR member name.
    /// </summary>
    /// <param name="clrMemberName">The CLR member name for this navigation step.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrMemberName"/> is not one CLR member name.</exception>
    public ApiKeyPathSegmentBuilder(string clrMemberName)
        : this(ApiPropertyReference.ClrRef(clrMemberName))
    {
        if (clrMemberName.Contains('.'))
        {
            throw new ArgumentException
            (
                "A key path segment must contain exactly one CLR member name and cannot contain a dot.",
                nameof(clrMemberName)
            );
        }
    }

    /// <summary>Creates a segment builder with the specified property reference.</summary>
    /// <param name="apiPropertyReference">The property reference for this navigation step.</param>
    public ApiKeyPathSegmentBuilder(ApiPropertyReference apiPropertyReference)
    {
        ArgumentNullException.ThrowIfNull(apiPropertyReference);
        _apiPropertyReference = apiPropertyReference;
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

    /// <summary>Creates a builder for a segment with the specified property reference.</summary>
    /// <param name="apiPropertyReference">The property reference for this navigation step.</param>
    /// <returns>A new segment builder.</returns>
    public static ApiKeyPathSegmentBuilder For(ApiPropertyReference apiPropertyReference) =>
        new(apiPropertyReference);
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiKeyPathSegmentBuilder AddKeyPathSegmentExtension(Type extensionType, object extension) => this.AddExtension(extensionType, extension);
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiKeyPathSegment"/> configured by this builder.
    /// </summary>
    internal ApiKeyPathSegment Build()
    {
        var segment = new ApiKeyPathSegment(_apiPropertyReference);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            segment.AttachExtensions(extensions);
        }

        return segment;
    }
    #endregion
}
