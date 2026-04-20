// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the result of API schema initialization, including any issues encountered.
/// </summary>
/// <param name="issues">Optional collection of issues encountered during initialization.</param>
public sealed class ApiInitializationResult(IEnumerable<ApiInitializationIssue>? issues = null)
{
    #region ApiInitializationResult Fields
    /// <summary>
    ///     Represents a successful initialization result with no issues.
    /// </summary>
    public static readonly ApiInitializationResult Success = new();
    #endregion

    #region ApiInitializationResult Properties
    /// <summary>
    ///     Gets a value indicating whether the initialization was successful (no errors).
    /// </summary>
    public bool IsValid => this.Errors is null || this.Errors.Length == 0;

    /// <summary>
    ///     Gets the collection of error-level issues, or <c>null</c> if no issues were encountered.
    /// </summary>
    public ApiInitializationIssue[]? Errors { get; } = issues is not null
        ? [.. issues.Where(x => x.Severity == ApiInitializationSeverity.Error)]
        : null;

    /// <summary>
    ///     Gets the collection of warning-level issues, or <c>null</c> if no issues were encountered.
    /// </summary>
    public ApiInitializationIssue[]? Warnings { get; } = issues is not null
        ? [.. issues.Where(x => x.Severity == ApiInitializationSeverity.Warning)]
        : null;

    /// <summary>
    ///     Gets all issues (errors and warnings), or <c>null</c> if no issues were encountered.
    /// </summary>
    public ApiInitializationIssue[]? Issues { get; } = issues is not null
        ? [.. issues]
        : null;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var isValid = this.IsValid.SafeToString();
        var issuesCount = this.Issues?.Length ?? 0;
        var errorsCount = this.Errors?.Length ?? 0;
        var warningsCount = this.Warnings?.Length ?? 0;

        return $"{nameof(ApiInitializationResult)} {{{nameof(this.IsValid)}={isValid}, IssuesCount={issuesCount}, ErrorsCount={errorsCount}, WarningsCount={warningsCount}}}";
    }
    #endregion

    #region ApiInitializationResult Methods
    /// <summary>
    ///     Throws an <see cref="ApiSchemaInitializationException"/> if the initialization result contains errors.
    /// </summary>
    /// <exception cref="ApiSchemaInitializationException">The initialization result contains one or more errors.</exception>
    public void ThrowIfInvalid()
    {
        if (this.IsValid)
        {
            return;
        }

        throw new ApiSchemaInitializationException(this);
    }

    #endregion
}
