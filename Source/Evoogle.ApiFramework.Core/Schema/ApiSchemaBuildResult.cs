// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Immutable;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the immutable result of compiling an API schema.
/// </summary>
public sealed class ApiSchemaBuildResult
{
    #region Properties
    /// <summary>Gets the frozen schema when compilation succeeds; otherwise, <c>null</c>.</summary>
    public ApiSchema? Schema { get; }

    /// <summary>Gets all compilation issues.</summary>
    public ImmutableArray<ApiInitializationIssue> Issues { get; }

    /// <summary>Gets all error-level compilation issues.</summary>
    public ImmutableArray<ApiInitializationIssue> Errors { get; }

    /// <summary>Gets all warning-level compilation issues.</summary>
    public ImmutableArray<ApiInitializationIssue> Warnings { get; }

    /// <summary>Gets a value indicating whether compilation succeeded without errors.</summary>
    public bool IsValid => !this.HasErrors;

    /// <summary>Gets a value indicating whether compilation produced errors.</summary>
    public bool HasErrors => !this.Errors.IsEmpty;

    /// <summary>Gets a value indicating whether compilation produced warnings.</summary>
    public bool HasWarnings => !this.Warnings.IsEmpty;
    #endregion

    #region Constructors
    internal ApiSchemaBuildResult(ApiSchema? schema, IEnumerable<ApiInitializationIssue>? issues)
    {
        this.Issues = issues is null ? [] : [.. issues];
        this.Errors = [.. this.Issues.Where(issue => issue.Severity == ApiInitializationSeverity.Error)];
        this.Warnings = [.. this.Issues.Where(issue => issue.Severity == ApiInitializationSeverity.Warning)];
        this.Schema = this.Errors.IsEmpty
            ? schema ?? throw new ArgumentNullException(nameof(schema))
            : null;
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var isValid = this.IsValid.SafeToString();
        return $"{nameof(ApiSchemaBuildResult)} {{{nameof(this.IsValid)}={isValid}, IssuesCount={this.Issues.Length}, ErrorsCount={this.Errors.Length}, WarningsCount={this.Warnings.Length}}}";
    }
    #endregion

    #region Methods
    /// <summary>
    ///     Throws an <see cref="ApiSchemaInitializationException"/> when compilation contains errors.
    /// </summary>
    /// <exception cref="ApiSchemaInitializationException">The build result contains one or more errors.</exception>
    public void ThrowIfInvalid()
    {
        if (!this.IsValid)
        {
            throw new ApiSchemaInitializationException(this);
        }
    }
    #endregion
}
