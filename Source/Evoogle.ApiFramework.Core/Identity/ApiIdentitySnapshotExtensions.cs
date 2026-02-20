// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///   Extension methods for <see cref="ApiIdentitySnapshot"/> class.
/// </summary>
public static class ApiIdentitySnapshotExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Attempts to get the scalar <see cref="ApiId"/> value at the specified dot-separated path.
    /// </summary>
    /// <param name="path">A dot-separated path (e.g., "Customer.Country.Id").</param>
    /// <param name="value">
    ///     The scalar <see cref="ApiId"/> value if the snapshot at <paramref name="path"/> is scalar;
    ///     otherwise <see cref="ApiId.Empty"/>.
    /// </param>
    /// <returns>The scalar ApiId value.</returns>
    /// <exception cref="ArgumentException">If the path is invalid or contains empty segments.</exception>
    /// <exception cref="ApiIdentityException">If attempting to navigate into a scalar snapshot or the part is unresolved without structure.</exception>
    /// <exception cref="ApiIdentityException">If navigation succeeded but the target snapshot is not scalar.</exception>
    /// <exception cref="KeyNotFoundException">If the specified part is not found.</exception>
    public static ApiId GetScalarValue(this ApiIdentitySnapshot snapshot, string path)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var (success, navigationResult) = snapshot.TryGetScalarValue(path, out var scalarValue);
        if (success)
        {
            return scalarValue;
        }

        navigationResult.ThrowIfFailed(); // Will throw the original exception (e.g., part not found, unresolved without structure, etc.)

        // Navigation succeeded but the target snapshot is not scalar.
        throw new ApiIdentityException($"The snapshot at path '{path}' is not scalar.");
    }

    /// <summary>
    ///     Navigates to a nested part by dot-separated path.
    /// </summary>
    /// <param name="snapshot">The snapshot to navigate.</param>
    /// <param name="path">
    ///     A dot-separated path (e.g., "Customer.Country.Id").
    ///     Single-segment paths like "Customer" navigate one level deep.
    /// </param>
    /// <returns>The nested identity snapshot.</returns>
    /// <exception cref="ArgumentException">If the path is invalid or contains empty segments.</exception>
    /// <exception cref="ApiIdentityException">If attempting to navigate into a scalar snapshot or the part is unresolved without structure.</exception>
    /// <exception cref="KeyNotFoundException">If the specified part is not found.</exception>
    public static ApiIdentitySnapshot Navigate(this ApiIdentitySnapshot snapshot, string path)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var result = snapshot.TryNavigate(path);
        result.ThrowIfFailed();

        return result.Snapshot!;
    }
    #endregion
}
