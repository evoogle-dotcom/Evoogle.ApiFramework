// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to configure a single <see cref="ApiKeyPath"/>.
/// </summary>
/// <remarks>
///     Use one of the static <see cref="For(Type, string[])"/> or <see cref="For(Type, ApiKeyPathSegmentBuilder[])"/>
///     factory methods to create instances, optionally attach extensions or add segments, then call
///     <see cref="Build"/> internally.
/// </remarks>
public class ApiKeyPathBuilder : ExtensionBuilder<ApiKeyPathBuilder>
{
    #region Fields
    private readonly Type _clrRootType;
    private readonly List<ApiKeyPathSegmentBuilder> _segmentBuilders;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes an <see cref="ApiKeyPathBuilder"/> with the specified root CLR type and CLR property names.
    ///     Each name is wrapped in a plain <see cref="ApiKeyPathSegmentBuilder"/> with no extensions.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="clrPropertyNames">
    ///     Ordered CLR property names from the root type to the terminal scalar property.
    ///     Must contain at least one name.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> or <paramref name="clrPropertyNames"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrPropertyNames"/> contains no elements.</exception>
    public ApiKeyPathBuilder(Type clrRootType, IEnumerable<string> clrPropertyNames)
    {
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(clrPropertyNames);

        _clrRootType = clrRootType;
        var names = clrPropertyNames as string[] ?? [.. clrPropertyNames];

        if (names.Length == 0)
        {
            throw new ArgumentException("At least one CLR property name must be provided.", nameof(clrPropertyNames));
        }

        if (names.Any(static name => string.IsNullOrWhiteSpace(name)))
        {
            throw new ArgumentException("CLR property names must not contain null, empty, or whitespace values.", nameof(clrPropertyNames));
        }

        _segmentBuilders = [.. names.Select(static n => new ApiKeyPathSegmentBuilder(n))];
    }

    /// <summary>
    ///     Initializes an <see cref="ApiKeyPathBuilder"/> with the specified root CLR type and pre-configured
    ///     segment builders. Use this overload when individual segments require extensions.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="segmentBuilders">
    ///     Ordered <see cref="ApiKeyPathSegmentBuilder"/> instances from the root type to the terminal scalar property.
    ///     Must contain at least one builder.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> or <paramref name="segmentBuilders"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="segmentBuilders"/> contains no elements.</exception>
    public ApiKeyPathBuilder(Type clrRootType, IEnumerable<ApiKeyPathSegmentBuilder> segmentBuilders)
    {
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(segmentBuilders);

        _clrRootType = clrRootType;
        _segmentBuilders = [.. segmentBuilders];

        if (_segmentBuilders.Count == 0)
        {
            throw new ArgumentException("At least one segment builder must be provided.", nameof(segmentBuilders));
        }

        if (_segmentBuilders.Any(static builder => builder is null))
        {
            throw new ArgumentException("Segment builders must not contain null values.", nameof(segmentBuilders));
        }
    }
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates a builder for a path that starts from the specified root CLR type, using plain property names.
    ///     Use <see cref="AddSegment"/> or <see cref="For(Type, ApiKeyPathSegmentBuilder[])"/> when individual
    ///     segments require extensions.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="clrPropertyNames">
    ///     Ordered CLR property names from the root type to the terminal scalar property.
    ///     Provide a single name for a direct property; provide multiple names for a navigated path.
    /// </param>
    /// <returns>A new <see cref="ApiKeyPathBuilder"/> for the specified root CLR type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrPropertyNames"/> is empty.</exception>
    public static ApiKeyPathBuilder For(Type clrRootType, params string[] clrPropertyNames)
    {
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(clrPropertyNames);

        return new(clrRootType, clrPropertyNames);
    }

    /// <summary>
    ///     Creates a builder for a path that starts from the specified root CLR type, using pre-configured
    ///     segment builders. Use this overload when individual segments require extensions.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="segmentBuilders">
    ///     Ordered <see cref="ApiKeyPathSegmentBuilder"/> instances from the root type to the terminal scalar property.
    /// </param>
    /// <returns>A new <see cref="ApiKeyPathBuilder"/> for the specified root CLR type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="segmentBuilders"/> is empty.</exception>
    public static ApiKeyPathBuilder For(Type clrRootType, params ApiKeyPathSegmentBuilder[] segmentBuilders)
    {
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(segmentBuilders);

        return new(clrRootType, segmentBuilders);
    }
    #endregion

    #region AddExtension Methods
    /// <summary>
    ///     Adds an extension value associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiKeyPathBuilder AddKeyPathExtension(Type type, object extension)
    {
        return this.AddExtension(type, extension);
    }
    #endregion

    #region AddSegment Methods
    /// <summary>
    ///     Appends a new segment for the specified CLR property name, optionally configuring it with extensions.
    /// </summary>
    /// <param name="clrPropertyName">The CLR property name for this navigation step.</param>
    /// <param name="configure">Optional callback to attach extensions to the segment.</param>
    /// <returns>The current builder instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrPropertyName"/> is <c>null</c>, empty, or whitespace.</exception>
    public ApiKeyPathBuilder AddSegment(string clrPropertyName, Action<ApiKeyPathSegmentBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrPropertyName);

        var segmentBuilder = new ApiKeyPathSegmentBuilder(clrPropertyName);
        configure?.Invoke(segmentBuilder);
        _segmentBuilders.Add(segmentBuilder);
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiKeyPath"/> configured by this builder.
    /// </summary>
    internal ApiKeyPath Build()
    {
        var segments = _segmentBuilders.Select(b => b.Build());
        var path = new ApiKeyPath(_clrRootType, segments);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            path.Extensions = extensions;
        }

        return path;
    }
    #endregion
}
