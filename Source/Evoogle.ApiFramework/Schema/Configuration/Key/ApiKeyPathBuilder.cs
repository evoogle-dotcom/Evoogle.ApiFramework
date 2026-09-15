// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Key.Internal;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Key.Internal;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Key;

/// <summary>
///     Fluent builder used to configure a single <see cref="ApiKeyPath"/>.
/// </summary>
/// <remarks>
///     Factory and constructor overloads accept an explicit CLR type, an explicit
///     <see cref="ApiTypeReference"/>, or an inferred root. Extensions and additional segments can
///     be attached before the path is built.
/// </remarks>
public class ApiKeyPathBuilder : ExtensionBuilder<ApiKeyPathBuilder>
{
    #region Fields
    private readonly ApiKeyPathState _state;
    #endregion

    #region Properties
    /// <summary>Gets the explicit root reference, or null when the root is inferred.</summary>
    internal ApiTypeReference? ApiRootTypeReference => _state.ApiRootTypeReference;

    /// <summary>Gets the ordered segment builders that make up this key path.</summary>
    internal IReadOnlyList<ApiKeyPathSegmentBuilder> SegmentBuilders => _state.SegmentBuilders;
    #endregion

    #region Constructors
    /// <summary>
    ///     Creates an <see cref="ApiKeyPathBuilder"/> with the specified root CLR type and CLR member paths.
    ///     Each dot-delimited path is expanded into plain <see cref="ApiKeyPathSegmentBuilder"/> instances with no extensions.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="clrMemberNames">
    ///     Ordered CLR member names or dot-delimited CLR member paths from the root type to the terminal scalar member.
    ///     Must contain at least one path.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> or <paramref name="clrMemberNames"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrMemberNames"/> contains no valid paths.</exception>
    public ApiKeyPathBuilder(Type clrRootType, IEnumerable<string> clrMemberNames)
        : this
        (
            new ApiTypeReference
                (clrRootType ?? throw new ArgumentNullException(nameof(clrRootType))),
            clrMemberNames
        )
    {
    }

    /// <summary>Creates a key-path builder with an explicit API type reference.</summary>
    /// <param name="apiRootTypeReference">The explicit root object-type reference.</param>
    /// <param name="clrMemberNames">The CLR member paths.</param>
    public ApiKeyPathBuilder
    (
        ApiTypeReference apiRootTypeReference,
        IEnumerable<string> clrMemberNames
    ) : this
    (
        CreateState
        (
            apiRootTypeReference ??
                throw new ArgumentNullException(nameof(apiRootTypeReference)),
            clrMemberNames
        )
    )
    {
    }

    /// <summary>Creates a key-path builder whose root is inferred from its owner.</summary>
    /// <param name="clrMemberNames">The CLR member paths.</param>
    public ApiKeyPathBuilder(IEnumerable<string> clrMemberNames)
        : this(CreateState(null, clrMemberNames))
    {
    }

    /// <summary>
    ///     Creates an <see cref="ApiKeyPathBuilder"/> with the specified root CLR type and pre-configured
    ///     segment builders. Use this overload when individual segments require extensions.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="segmentBuilders">
    ///     Ordered <see cref="ApiKeyPathSegmentBuilder"/> instances from the root type to the terminal scalar member.
    ///     Must contain at least one builder.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> or <paramref name="segmentBuilders"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="segmentBuilders"/> contains no elements.</exception>
    public ApiKeyPathBuilder(Type clrRootType, IEnumerable<ApiKeyPathSegmentBuilder> segmentBuilders)
        : this
        (
            new ApiTypeReference
                (clrRootType ?? throw new ArgumentNullException(nameof(clrRootType))),
            segmentBuilders
        )
    {
    }

    /// <summary>Creates a key-path builder with an explicit root reference and segment builders.</summary>
    /// <param name="apiRootTypeReference">The explicit root object-type reference.</param>
    /// <param name="segmentBuilders">The ordered segment builders.</param>
    public ApiKeyPathBuilder
    (
        ApiTypeReference apiRootTypeReference,
        IEnumerable<ApiKeyPathSegmentBuilder> segmentBuilders
    ) : this
    (
        CreateState
        (
            apiRootTypeReference ??
                throw new ArgumentNullException(nameof(apiRootTypeReference)),
            segmentBuilders
        )
    )
    {
    }

    /// <summary>Creates a key-path builder with an inferred root and segment builders.</summary>
    /// <param name="segmentBuilders">The ordered segment builders.</param>
    public ApiKeyPathBuilder(IEnumerable<ApiKeyPathSegmentBuilder> segmentBuilders)
        : this(CreateState(null, segmentBuilders))
    {
    }

    private ApiKeyPathBuilder(ApiKeyPathState state)
    {
        _state = state;
    }
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates a builder for a path that starts from the specified root CLR type, using CLR member paths.
    ///     Use <see cref="AddSegment"/> or <see cref="For(Type, ApiKeyPathSegmentBuilder[])"/> when individual
    ///     segments require extensions.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="clrMemberNames">
    ///     Ordered CLR member names or dot-delimited CLR member paths from the root type to the terminal scalar member.
    ///     Provide a single name for a direct member, a dot-delimited path for navigation, or
    ///     multiple path fragments.
    /// </param>
    /// <returns>A new <see cref="ApiKeyPathBuilder"/> for the specified root CLR type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrRootType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrMemberNames"/> is empty or contains an invalid path.</exception>
    public static ApiKeyPathBuilder For(Type clrRootType, params string[] clrMemberNames)
    {
        ArgumentNullException.ThrowIfNull(clrRootType);
        ArgumentNullException.ThrowIfNull(clrMemberNames);

        return new(clrRootType, clrMemberNames);
    }

    /// <summary>Creates a builder with an explicit API root reference.</summary>
    public static ApiKeyPathBuilder For
    (
        ApiTypeReference apiRootTypeReference,
        params string[] clrMemberNames
    ) => new(apiRootTypeReference, clrMemberNames);

    /// <summary>Creates a builder whose root is inferred from its owner.</summary>
    public static ApiKeyPathBuilder For(params string[] clrMemberNames) => new(clrMemberNames);

    /// <summary>
    ///     Creates a builder for a path that starts from the specified root CLR type, using pre-configured
    ///     segment builders. Use this overload when individual segments require extensions.
    /// </summary>
    /// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
    /// <param name="segmentBuilders">
    ///     Ordered <see cref="ApiKeyPathSegmentBuilder"/> instances from the root type to the terminal scalar member.
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

    /// <summary>Creates a builder with an explicit API root reference and segment builders.</summary>
    public static ApiKeyPathBuilder For
    (
        ApiTypeReference apiRootTypeReference,
        params ApiKeyPathSegmentBuilder[] segmentBuilders
    ) => new(apiRootTypeReference, segmentBuilders);

    /// <summary>Creates a builder with an inferred root and segment builders.</summary>
    public static ApiKeyPathBuilder For(params ApiKeyPathSegmentBuilder[] segmentBuilders) =>
        new(segmentBuilders);
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
    ///     Appends a new segment for the specified CLR member name, optionally configuring it with extensions.
    /// </summary>
    /// <param name="clrMemberName">The CLR member name for this navigation step.</param>
    /// <param name="configure">Optional callback to attach extensions to the segment.</param>
    /// <returns>The current builder instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="clrMemberName"/> is not one CLR member name.</exception>
    public ApiKeyPathBuilder AddSegment(string clrMemberName, Action<ApiKeyPathSegmentBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrMemberName);

        var segmentBuilder = new ApiKeyPathSegmentBuilder(clrMemberName);
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
        var path = new ApiKeyPath(_state.ApiRootTypeReference, segments);

        var extensions = this.BuildExtensions();
        if (extensions != null)
        {
            path.AttachExtensions(extensions);
        }

        return path;
    }
    #endregion

    #region Implementation Methods
    private static ApiKeyPathState CreateState
    (
        ApiTypeReference? apiRootTypeReference,
        IEnumerable<string> clrMemberNames
    )
    {
        ArgumentNullException.ThrowIfNull(clrMemberNames);

        var names = clrMemberNames as string[] ?? [.. clrMemberNames];
        if (names.Length == 0)
        {
            throw new ArgumentException
                ("At least one CLR member name must be provided.", nameof(clrMemberNames));
        }

        var parsedClrMemberNames = new List<string>();
        foreach (var name in names)
        {
            var parseResult = ApiKeyPathClrPathParser.Parse(name);
            parseResult.ThrowIfInvalid(nameof(clrMemberNames));
            parsedClrMemberNames.AddRange(parseResult.ClrMemberNames);
        }

        return new ApiKeyPathState
        (
            apiRootTypeReference,
            parsedClrMemberNames.Select(static name => new ApiKeyPathSegmentBuilder(name))
        );
    }

    private static ApiKeyPathState CreateState
    (
        ApiTypeReference? apiRootTypeReference,
        IEnumerable<ApiKeyPathSegmentBuilder> segmentBuilders
    )
    {
        ArgumentNullException.ThrowIfNull(segmentBuilders);

        var state = new ApiKeyPathState(apiRootTypeReference, segmentBuilders);
        if (state.SegmentBuilders.Count == 0)
        {
            throw new ArgumentException
                ("At least one segment builder must be provided.", nameof(segmentBuilders));
        }

        if (state.SegmentBuilders.Any(static builder => builder is null))
        {
            throw new ArgumentException
                ("Segment builders must not contain null values.", nameof(segmentBuilders));
        }

        return state;
    }
    #endregion
}
