// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Represents the result of navigating through an <see cref="ApiIdentitySnapshot"/> path.
/// </summary>
public class ApiIdentityNavigationResult
{
    #region Properties
    /// <summary>
    ///     Gets the status of the navigation attempt.
    /// </summary>
    public ApiIdentityNavigationStatus Status { get; }

    /// <summary>
    ///     Gets the successfully navigated snapshot, or null if navigation failed.
    /// </summary>
    public ApiIdentitySnapshot? Snapshot { get; }

    /// <summary>
    ///     Gets the path that was being navigated.
    /// </summary>
    public string? RequestedPath { get; }

    /// <summary>
    ///     Gets the segment where navigation failed (only populated for certain failure statuses).
    /// </summary>
    public string? FailedSegment { get; }

    /// <summary>
    ///     Gets the path context where the failure occurred.
    /// </summary>
    public string? FailureContext { get; }

    /// <summary>
    ///     Gets whether the navigation was successful (either fully resolved or synthetic).
    /// </summary>
    [MemberNotNullWhen(true, nameof(Snapshot))]
    public bool IsSuccess => this.Status == ApiIdentityNavigationStatus.Success
                          || this.Status == ApiIdentityNavigationStatus.SuccessWithSyntheticSnapshot;

    /// <summary>
    ///     Gets whether the navigation succeeded with a fully resolved snapshot (not synthetic).
    /// </summary>
    public bool IsFullyResolved => this.Status == ApiIdentityNavigationStatus.Success;

    /// <summary>
    ///     Gets whether the snapshot was found but is synthetic (created from structure without values).
    /// </summary>
    public bool IsSynthetic => this.Status == ApiIdentityNavigationStatus.SuccessWithSyntheticSnapshot;
    #endregion

    #region Constructors
    private ApiIdentityNavigationResult
    (
        ApiIdentityNavigationStatus status,
        ApiIdentitySnapshot? snapshot,
        string? requestedPath,
        string? failedSegment,
        string? failureContext
    )
    {
        this.Status = status;
        this.Snapshot = snapshot;
        this.RequestedPath = requestedPath;
        this.FailedSegment = failedSegment;
        this.FailureContext = failureContext;
    }
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates a successful navigation result with a resolved snapshot.
    /// </summary>
    public static ApiIdentityNavigationResult Success(ApiIdentitySnapshot snapshot, string requestedPath)
    {
        return new ApiIdentityNavigationResult(
            ApiIdentityNavigationStatus.Success,
            snapshot,
            requestedPath,
            null,
            null
        );
    }

    /// <summary>
    ///     Creates a successful navigation result with a synthetic snapshot (created from structure).
    /// </summary>
    public static ApiIdentityNavigationResult SuccessWithSynthetic(ApiIdentitySnapshot syntheticSnapshot, string requestedPath)
    {
        return new ApiIdentityNavigationResult(
            ApiIdentityNavigationStatus.SuccessWithSyntheticSnapshot,
            syntheticSnapshot,
            requestedPath,
            null,
            null
        );
    }

    /// <summary>
    ///     Creates a failure result for attempting to navigate into a scalar snapshot.
    /// </summary>
    public static ApiIdentityNavigationResult ScalarNavigationAttempt(string requestedPath, string currentPath)
    {
        return new ApiIdentityNavigationResult(
            ApiIdentityNavigationStatus.CannotNavigateIntoScalar,
            null,
            requestedPath,
            null,
            currentPath
        );
    }

    /// <summary>
    ///     Creates a failure result for a snapshot with no nested parts.
    /// </summary>
    public static ApiIdentityNavigationResult NoNestedParts(string requestedPath, string currentPath)
    {
        return new ApiIdentityNavigationResult(
            ApiIdentityNavigationStatus.NoNestedPartsAvailable,
            null,
            requestedPath,
            null,
            currentPath
        );
    }

    /// <summary>
    ///     Creates a failure result for an invalid path segment.
    /// </summary>
    public static ApiIdentityNavigationResult InvalidSegment(string requestedPath, string invalidSegment)
    {
        return new ApiIdentityNavigationResult(
            ApiIdentityNavigationStatus.InvalidPathSegment,
            null,
            requestedPath,
            invalidSegment,
            null
        );
    }

    /// <summary>
    ///     Creates a failure result for a part name not found.
    /// </summary>
    public static ApiIdentityNavigationResult PartNotFound(string requestedPath, string segment, string currentPath)
    {
        return new ApiIdentityNavigationResult(
            ApiIdentityNavigationStatus.PartNotFound,
            null,
            requestedPath,
            segment,
            currentPath
        );
    }

    /// <summary>
    ///     Creates a failure result for an unresolved part with no structure.
    /// </summary>
    public static ApiIdentityNavigationResult UnresolvedWithoutStructure(string requestedPath, string segment, string currentPath)
    {
        return new ApiIdentityNavigationResult(
            ApiIdentityNavigationStatus.UnresolvedPartWithoutStructure,
            null,
            requestedPath,
            segment,
            currentPath
        );
    }
    #endregion

    #region Methods
    /// <summary>
    ///     Gets a human-readable error message describing the navigation failure.
    /// </summary>
    public string GetErrorMessage()
    {
        return this.Status switch
        {
            ApiIdentityNavigationStatus.Success => "Navigation succeeded.",
            ApiIdentityNavigationStatus.SuccessWithSyntheticSnapshot => "Navigation succeeded with synthetic snapshot from structure.",
            ApiIdentityNavigationStatus.CannotNavigateIntoScalar => $"Cannot navigate into scalar snapshot at path '{this.FailureContext}'.",
            ApiIdentityNavigationStatus.NoNestedPartsAvailable => $"No nested parts available at path '{this.FailureContext}'.",
            ApiIdentityNavigationStatus.InvalidPathSegment => $"Invalid path segment '{this.FailedSegment}' in path '{this.RequestedPath}'.",
            ApiIdentityNavigationStatus.PartNotFound => $"Part with name '{this.FailedSegment}' not found at path '{this.FailureContext}'.",
            ApiIdentityNavigationStatus.UnresolvedPartWithoutStructure => $"Part with name '{this.FailedSegment}' at path '{this.FailureContext}' is unresolved and has no structure information.",
            _ => "Unknown navigation failure."
        };
    }

    /// <summary>
    ///     Throws an exception if the navigation was not successful.
    /// </summary>
    /// <exception cref="ApiIdentityException">If navigation failed.</exception>
    /// <exception cref="ArgumentException">If the path segment was invalid.</exception>
    /// <exception cref="KeyNotFoundException">If the part was not found.</exception>
    public void ThrowIfFailed()
    {
        if (this.IsSuccess || this.IsSynthetic)
        {
            return;
        }

        throw this.Status switch
        {
            ApiIdentityNavigationStatus.InvalidPathSegment => new ArgumentException(this.GetErrorMessage(), nameof(this.RequestedPath)),
            ApiIdentityNavigationStatus.PartNotFound => new KeyNotFoundException(this.GetErrorMessage()),
            _ => new ApiIdentityException(this.GetErrorMessage()),
        };
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var status = this.Status.SafeToString();
        var snapshot = this.Snapshot.SafeToString();
        var requestedPath = this.RequestedPath.SafeToString();
        var failedSegment = this.FailedSegment.SafeToString();
        var failureContext = this.FailureContext.SafeToString();

        return $"{nameof(ApiIdentityNavigationResult)} {{{nameof(this.Status)}={status}, {nameof(this.Snapshot)}={snapshot}, {nameof(this.RequestedPath)}={requestedPath}, {nameof(this.FailedSegment)}={failedSegment}, {nameof(this.FailureContext)}={failureContext}}}";
    }
    #endregion

    #region Operators
    /// <summary>
    ///     Implicitly converts the navigation result to a boolean indicating success.
    /// </summary>
    public static implicit operator bool(ApiIdentityNavigationResult result) => result.IsSuccess;
    #endregion
}
