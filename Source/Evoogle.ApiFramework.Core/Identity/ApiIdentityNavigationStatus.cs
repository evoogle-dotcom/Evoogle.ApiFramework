// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Specifies the status of an identity snapshot navigation operation.
/// </summary>
public enum ApiIdentityNavigationStatus
{
    /// <summary>
    ///     Navigation succeeded with a fully resolved snapshot.
    /// </summary>
    Success = 0,

    /// <summary>
    ///     Navigation succeeded but returned a synthetic snapshot created from structure (values unresolved).
    /// </summary>
    SuccessWithSyntheticSnapshot = 1,

    /// <summary>
    ///     Cannot navigate into a scalar (leaf) snapshot.
    /// </summary>
    CannotNavigateIntoScalar = 2,

    /// <summary>
    ///     The snapshot has no nested parts to navigate.
    /// </summary>
    NoNestedPartsAvailable = 3,

    /// <summary>
    ///     A path segment was null, empty, or whitespace.
    /// </summary>
    InvalidPathSegment = 4,

    /// <summary>
    ///     The requested part name was not found in the available nested parts.
    /// </summary>
    PartNotFound = 5,

    /// <summary>
    ///     The part exists but is unresolved (null snapshot) and has no structure information.
    /// </summary>
    UnresolvedPartWithoutStructure = 6
}
