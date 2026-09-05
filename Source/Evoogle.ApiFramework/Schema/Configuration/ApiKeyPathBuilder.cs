// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Internal;

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
    private readonly ApiKeyPathState _state;
    #endregion

    #region Properties
    /// <summary>Gets the CLR root type from which this key path's navigation chain begins.</summary>
    internal Type ClrRootType => _state.ClrRootType;

    /// <summary>Gets the ordered segment builders that make up this key path.</summary>
    internal IReadOnlyList<ApiKeyPathSegmentBuilder> SegmentBuilders => _state.SegmentBuilders;
    #endregion

    #region Constructors
    /// <summary>
    ///     Creates an <see cref="ApiKeyPathBuilder"/> with the specified root CLR type and CLR property paths.
    ///     Each dot-delimited path is expanded into plain <see cref="ApiKeyPathSegmentBuilder"/> instances with no extensions.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="clrPropertyNames">
    ///     Ordered CLR property names or dot-delimited CLR property paths from the root type to the terminal scalar property.
    ///     Must contain at least one path.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> or <paramref name="clrPropertyNames"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrPropertyNames"/> contains no valid paths.</exception>
    public ApiKeyPathBuilder(Type clrRootType, IEnumerable<string> clrPropertyNames)
    {
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(clrPropertyNames);

        var names = clrPropertyNames as string[] ?? [.. clrPropertyNames];

        if (names.Length == 0)
        {
            throw new ArgumentException("At least one CLR property name must be provided.", nameof(clrPropertyNames));
        }

        var parsedClrPropertyNames = new List<string>();
        foreach (var name in names)
        {
            var parseResult = ApiKeyPathClrPathParser.Parse(name);
            parseResult.ThrowIfInvalid(nameof(clrPropertyNames));
            parsedClrPropertyNames.AddRange(parseResult.ClrPropertyNames);
        }

        _state = new ApiKeyPathState
        (
            clrRootType,
            parsedClrPropertyNames.Select(static name => new ApiKeyPathSegmentBuilder(name))
        );
    }

    /// <summary>
    ///     Creates an <see cref="ApiKeyPathBuilder"/> with the specified root CLR type and pre-configured
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

        _state = new ApiKeyPathState(clrRootType, segmentBuilders);

        if (_state.SegmentBuilders.Count == 0)
        {
            throw new ArgumentException("At least one segment builder must be provided.", nameof(segmentBuilders));
        }

        if (_state.SegmentBuilders.Any(static builder => builder is null))
        {
            throw new ArgumentException("Segment builders must not contain null values.", nameof(segmentBuilders));
        }
    }
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates a builder for a path that starts from the specified root CLR type, using CLR property paths.
    ///     Use <see cref="AddSegment"/> or <see cref="For(Type, ApiKeyPathSegmentBuilder[])"/> when individual
    ///     segments require extensions.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="clrPropertyNames">
    ///     Ordered CLR property names or dot-delimited CLR property paths from the root type to the terminal scalar property.
    ///     Provide a single name for a direct property, a dot-delimited path for navigation, or multiple path fragments.
    /// </param>
    /// <returns>A new <see cref="ApiKeyPathBuilder"/> for the specified root CLR type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrPropertyNames"/> is empty or contains an invalid path.</exception>
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
    ///     Adds an extension value associated with the specified <paramref name="extensionType"/>.
    /// </summary>
    /// <param name="extensionType">The type used as the extension key.</param>
    /// <param name="extension">The extension value to store.</param>
    /// <returns>The current builder instance.</returns>
    public ApiKeyPathBuilder AddKeyPathExtension(Type extensionType, object extension)
    {
        return this.AddExtension(extensionType, extension);
    }
    #endregion

    #region AddSegment Methods
    /// <summary>
    ///     Appends a new segment for the specified CLR property name, optionally configuring it with extensions.
    /// </summary>
    /// <param name="clrPropertyName">The CLR property name for this navigation step.</param>
    /// <param name="configure">Optional callback to attach extensions to the segment.</param>
    /// <returns>The current builder instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrPropertyName"/> is not one CLR property name.</exception>
    public ApiKeyPathBuilder AddSegment(string clrPropertyName, Action<ApiKeyPathSegmentBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrPropertyName);

        var segmentBuilder = new ApiKeyPathSegmentBuilder(clrPropertyName);
        configure?.Invoke(segmentBuilder);
        _state.SegmentBuilders.Add(segmentBuilder);
        return this;
    }
    #endregion

    #region Build Methods
    /// <summary>
    ///     Builds the <see cref="ApiKeyPath"/> configured by this builder.
    /// </summary>
    internal ApiKeyPath Build()
    {
        var segments = _state.SegmentBuilders.Select(b => b.Build());
        var path = new ApiKeyPath(_state.ClrRootType, segments);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            path.AttachExtensions(extensions);
        }

        return path;
    }
    #endregion
}
