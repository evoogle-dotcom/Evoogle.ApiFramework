// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

public sealed class ApiInitializationResult(IEnumerable<ApiInitializationIssue>? issues = null)
{
    #region Fields
    public static readonly ApiInitializationResult Success = new();
    #endregion

    #region Properties
    public bool IsValid => !this.Issues?.Any(x => x.Severity == ApiInitializationSeverity.Error) ?? true;

    public ApiInitializationIssue[]? Errors { get; } = issues is not null
        ? [.. issues.Where(x => x.Severity == ApiInitializationSeverity.Error)]
        : null;

    public ApiInitializationIssue[]? Warnings { get; } = issues is not null
        ? [.. issues.Where(x => x.Severity == ApiInitializationSeverity.Warning)]
        : null;

    public ApiInitializationIssue[]? Issues { get; } = issues is not null
        ? [.. issues]
        : null;
    #endregion

    #region Methods
    public void ThrowIfInvalid()
    {
        if (this.IsValid)
        {
            return;
        }

        throw new ApiSchemaInitializationException(this);
    }

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
}
