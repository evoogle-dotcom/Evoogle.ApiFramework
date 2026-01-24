// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Represents a single part within an identity snapshot, which can be a scalar value,
///     a nested snapshot, or an unresolved nested identity.
/// </summary>
public readonly record struct ApiIdentityPart
{
    private ApiIdentityPart(ApiIdentityPartKind kind, ApiId? scalarValue, ApiIdentitySnapshot? nestedSnapshot)
    {
        this.Kind = kind;
        this.ScalarValue = scalarValue;
        this.NestedSnapshot = nestedSnapshot;
    }

    /// <summary>
    ///     Gets the kind of this identity part.
    /// </summary>
    public ApiIdentityPartKind Kind { get; init; }

    /// <summary>
    ///     Gets the scalar value if this is a scalar part; otherwise null.
    /// </summary>
    public ApiId? ScalarValue { get; init; }

    /// <summary>
    ///     Gets the nested snapshot if this is a nested part; otherwise null.
    /// </summary>
    public ApiIdentitySnapshot? NestedSnapshot { get; init; }

    /// <summary>
    ///     Creates a scalar identity part from an ApiId.
    /// </summary>
    /// <param name="scalarValue">The scalar ApiId value.</param>
    /// <returns>A scalar identity part.</returns>
    public static ApiIdentityPart Scalar(ApiId scalarValue) =>
        new(ApiIdentityPartKind.Scalar, scalarValue, null);

    /// <summary>
    ///     Creates a nested identity part from a snapshot.
    /// </summary>
    /// <param name="nestedSnapshot">The nested identity snapshot.</param>
    /// <returns>A nested identity part.</returns>
    public static ApiIdentityPart Nested(ApiIdentitySnapshot nestedSnapshot)
    {
        ArgumentNullException.ThrowIfNull(nestedSnapshot);
        return new(ApiIdentityPartKind.Nested, null, nestedSnapshot);
    }

    /// <summary>
    ///     Creates an unresolved nested identity part.
    /// </summary>
    /// <returns>An unresolved nested identity part.</returns>
    public static ApiIdentityPart UnresolvedNested() =>
        new(ApiIdentityPartKind.UnresolvedNested, null, null);

    /// <summary>
    ///     Creates an empty scalar identity part.
    /// </summary>
    /// <returns>An empty scalar identity part.</returns>
    public static ApiIdentityPart EmptyScalar() =>
        new(ApiIdentityPartKind.Scalar, ApiId.Empty, null);
}
