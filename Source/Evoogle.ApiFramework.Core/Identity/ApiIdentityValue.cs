// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Represents a structured snapshot of a resolved object identity, capturing all resolved part values
///     at a single point in time.
/// </summary>
/// <remarks>
///     <para>
///         An <see cref="ApiIdentityValue"/> always contains at least one <see cref="ApiIdentityPartValue"/> in <see cref="ApiParts"/>.
///         When the identity is a single scalar property, <see cref="ApiParts"/> contains one <see cref="ApiScalarIdentityPartValue"/>
///         and the convenience property <see cref="IsScalar"/> returns <see langword="true"/>.
///     </para>
///     <para>
///         Use <see cref="ToApiId"/> to flatten the hierarchical identity into a flat <see cref="ApiId"/> for
///         dictionary lookups, equality comparisons, and other performance-critical operations.
///     </para>
///     <para>
///         Use <see cref="TryNavigate"/> to navigate to a nested part using a dotted path
///         (e.g., <c>"Customer.Country.Id"</c>).
///     </para>
/// </remarks>
[JsonConverter(typeof(ApiIdentityValueJsonConverter))]
public sealed class ApiIdentityValue(IEnumerable<ApiIdentityPartValue>? apiParts)
{
    #region Properties
    /// <summary>
    ///     Gets the ordered array of resolved API part values that make up this identity.
    ///     Always non-null and contains at least one element.
    /// </summary>
    public ApiIdentityPartValue[] ApiParts { get; } = [.. apiParts.EmptyIfNull().Where(x => x is not null)];
    #endregion

    #region Computed Properties
    /// <summary>
    ///     Gets the scalar <see cref="ApiId"/> value when <see cref="IsScalar"/> is <see langword="true"/>.
    /// </summary>
    /// <exception cref="ApiIdentityException">
    ///     Thrown when <see cref="IsScalar"/> is <see langword="false"/>.
    /// </exception>
    public ApiId ApiScalarValue
    {
        get
        {
            if (!this.IsScalar)
            {
                throw new ApiIdentityException($"Cannot access {nameof(this.ApiScalarValue)} on a composite {nameof(ApiIdentityValue)}. Check {nameof(this.IsScalar)} first.");
            }

            return ((ApiScalarIdentityPartValue)this.ApiParts[0]).ApiScalarValue;
        }
    }

    /// <summary>
    ///     Gets a value indicating whether this identity is composite (has two or more parts).
    /// </summary>
    public bool IsComposite => this.ApiParts.Length >= 2;

    /// <summary>
    ///     Gets a value indicating whether all parts in this identity are fully resolved.
    ///     Returns <see langword="false"/> if any <see cref="ApiObjectIdentityPartValue"/> has a
    ///     <see langword="null"/> <see cref="ApiObjectIdentityPartValue.ApiObjectValue"/>.
    /// </summary>
    public bool IsFullyResolved => CheckFullyResolved(this.ApiParts);

    /// <summary>
    ///     Gets a value indicating whether this identity is a single scalar value.
    ///     When <see langword="true"/>, <see cref="ApiScalarValue"/> can be used to access the scalar <see cref="ApiId"/> directly.
    /// </summary>
    public bool IsScalar => this.ApiParts.Length == 1 && this.ApiParts[0] is ApiScalarIdentityPartValue;
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates an <see cref="ApiIdentityValue"/> from one or more pre-built parts.
    /// </summary>
    /// <param name="apiParts">The parts composing this identity. Must contain at least one element.</param>
    /// <returns>An <see cref="ApiIdentityValue"/> containing the specified parts.</returns>
    internal static ApiIdentityValue Composite(ApiIdentityPartValue[] apiParts)
        => new(apiParts);

    /// <summary>
    ///     Creates an <see cref="ApiIdentityValue"/> representing a single scalar identity.
    /// </summary>
    /// <param name="apiPartName">The name of the scalar identity part.</param>
    /// <param name="apiScalarValue">The scalar <see cref="ApiId"/> value.</param>
    /// <returns>An <see cref="ApiIdentityValue"/> containing one <see cref="ApiScalarIdentityPartValue"/>.</returns>
    internal static ApiIdentityValue Scalar(string apiPartName, ApiId apiScalarValue)
        => new([new ApiScalarIdentityPartValue(apiPartName, apiScalarValue)]);
    #endregion

    #region ApiIdentityValue Methods
    /// <summary>
    ///     Flattens this hierarchical identity into a flat <see cref="ApiId"/> suitable for dictionary lookups,
    ///     equality comparisons, and other performance-critical operations.
    /// </summary>
    /// <param name="useNamedParts">
    ///     When <see langword="true"/>, each emitted part receives a dotted name derived from the navigation path
    ///     (e.g., <c>"Customer.Country.Id"</c>). When <see langword="false"/>, parts are positional (unnamed).
    /// </param>
    /// <param name="nullHandling">
    ///     Controls behavior when an <see cref="ApiObjectIdentityPartValue"/> is unresolved
    ///     (<see cref="ApiObjectIdentityPartValue.ApiObjectValue"/> is <see langword="null"/>).
    /// </param>
    /// <returns>
    ///     A flat <see cref="ApiId"/>. When <see cref="IsScalar"/> is <see langword="true"/>, returns the scalar
    ///     value directly. Otherwise returns a composite <see cref="ApiId"/> with one entry per scalar leaf.
    /// </returns>
    /// <exception cref="ApiIdentityException">
    ///     Thrown when <paramref name="nullHandling"/> is <see cref="ApiIdentityNullHandling.ThrowException"/>
    ///     and an unresolved <see cref="ApiObjectIdentityPartValue"/> is encountered.
    /// </exception>
    public ApiId ToApiId(bool useNamedParts = true, ApiIdentityNullHandling nullHandling = ApiIdentityNullHandling.ReturnEmpty)
    {
        if (this.IsScalar)
        {
            return this.ApiScalarValue;
        }

        var builder = new ApiIdCompositeBuilder();
        FlattenParts(this.ApiParts, prefix: null, useNamedParts, nullHandling, builder);
        return builder.Build();
    }

    /// <summary>
    ///     Navigates to a nested <see cref="ApiIdentityPartValue"/> using a dotted path
    ///     (e.g., <c>"Customer.Country.Id"</c>).
    /// </summary>
    /// <param name="dottedPath">
    ///     A dot-separated sequence of part names to follow from this identity value's parts.
    /// </param>
    /// <returns>
    ///     The <see cref="ApiIdentityPartValue"/> at the end of the path, or <see langword="null"/> if any
    ///     segment is not found, the path attempts to navigate into a scalar part, or any intermediate
    ///     <see cref="ApiObjectIdentityPartValue"/> is unresolved.
    /// </returns>
    public ApiIdentityPartValue? TryNavigate(string? dottedPath)
    {
        if (string.IsNullOrWhiteSpace(dottedPath))
        {
            return null;
        }

        var segments = dottedPath.Split('.');
        return NavigateParts(this.ApiParts, segments, segmentIndex: 0);
    }

    /// <summary>
    ///     Returns the <see cref="ApiIdentityPartValue"/> reached by following the dotted path,
    ///     or <see langword="null"/> if any segment is not found, the path navigates into a scalar part,
    ///     or any intermediate <see cref="ApiObjectIdentityPartValue"/> is unresolved.
    /// </summary>
    /// <param name="dottedPath">
    ///     A dot-separated sequence of part names to follow from this identity value's parts
    ///     (e.g., <c>"Customer.Country.Id"</c>).
    /// </param>
    /// <remarks>
    ///     This indexer is powered by <see cref="TryNavigate"/>. See that method for full navigation semantics.
    /// </remarks>
    public ApiIdentityPartValue? this[string dottedPath] => this.TryNavigate(dottedPath);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiId = this.ToApiId(useNamedParts: true, nullHandling: ApiIdentityNullHandling.ReturnEmpty);
        return apiId.ToString()!;
    }
    #endregion

    #region Implementation Methods
    private static bool CheckFullyResolved(ApiIdentityPartValue[] parts)
    {
        foreach (var part in parts)
        {
            if (part is ApiObjectIdentityPartValue objectPart)
            {
                if (objectPart.ApiObjectValue is null)
                {
                    return false;
                }

                if (!CheckFullyResolved(objectPart.ApiObjectValue.ApiParts))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static void FlattenParts
    (
        ApiIdentityPartValue[] parts,
        string? prefix,
        bool useNamedParts,
        ApiIdentityNullHandling nullHandling,
        ApiIdCompositeBuilder builder
    )
    {
        foreach (var part in parts)
        {
            var qualifiedName = prefix is null ? part.ApiName : $"{prefix}.{part.ApiName}";

            switch (part)
            {
                case ApiScalarIdentityPartValue scalarPart:
                    if (useNamedParts)
                    {
                        builder.Add(qualifiedName, scalarPart.ApiScalarValue);
                    }
                    else
                    {
                        builder.Add(scalarPart.ApiScalarValue);
                    }

                    break;

                case ApiObjectIdentityPartValue objectPart:
                    if (objectPart.ApiObjectValue is not null)
                    {
                        FlattenParts(objectPart.ApiObjectValue.ApiParts, qualifiedName, useNamedParts, nullHandling, builder);
                    }
                    else if (nullHandling == ApiIdentityNullHandling.ThrowException)
                    {
                        throw new ApiIdentityException(
                            $"Unresolved identity part '{qualifiedName}' encountered during flattening. " +
                            $"Set {nameof(ApiIdentityNullHandling)} to {nameof(ApiIdentityNullHandling.ReturnEmpty)} to allow unresolved parts.");
                    }
                    else
                    {
                        FlattenUnresolvedParts(objectPart, qualifiedName, useNamedParts, builder);
                    }
                    break;
            }
        }
    }

    private static void FlattenUnresolvedParts
    (
        ApiObjectIdentityPartValue objectPart,
        string qualifiedName,
        bool useNamedParts,
        ApiIdCompositeBuilder builder
    )
    {
        if (objectPart.ApiStructure is { Length: > 0 } structure)
        {
            foreach (var structurePart in structure)
            {
                var childName = $"{qualifiedName}.{structurePart.ApiName}";

                switch (structurePart)
                {
                    case ApiScalarIdentityPartValue:
                        if (useNamedParts)
                        {
                            builder.Add(childName, ApiId.Empty);
                        }
                        else
                        {
                            builder.Add(ApiId.Empty);
                        }

                        break;

                    case ApiObjectIdentityPartValue nestedObject:
                        FlattenUnresolvedParts(nestedObject, childName, useNamedParts, builder);
                        break;
                }
            }
        }
        else
        {
            if (useNamedParts)
            {
                builder.Add(qualifiedName, ApiId.Empty);
            }
            else
            {
                builder.Add(ApiId.Empty);
            }
        }
    }

    private static ApiIdentityPartValue? NavigateParts(ApiIdentityPartValue[] parts, string[] segments, int segmentIndex)
    {
        if (segmentIndex >= segments.Length)
        {
            return null;
        }

        var targetName = segments[segmentIndex];

        foreach (var part in parts)
        {
            if (!string.Equals(part.ApiName, targetName, StringComparison.Ordinal))
            {
                continue;
            }

            if (segmentIndex == segments.Length - 1)
            {
                return part;
            }

            if (part is ApiObjectIdentityPartValue objectPart)
            {
                var childParts = objectPart.ApiObjectValue?.ApiParts ?? objectPart.ApiStructure;
                if (childParts is not null)
                {
                    return NavigateParts(childParts, segments, segmentIndex + 1);
                }
            }

            return null;
        }

        return null;
    }
    #endregion
}
