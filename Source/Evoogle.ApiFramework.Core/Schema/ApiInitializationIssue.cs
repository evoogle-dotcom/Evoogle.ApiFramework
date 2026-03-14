// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents an issue encountered during API schema initialization.
/// </summary>
/// <param name="path">The path to the schema element where the issue occurred.</param>
/// <param name="severity">The severity level of the issue.</param>
/// <param name="code">The specific error or warning code identifying the issue.</param>
/// <param name="description">A human-readable description of the issue.</param>
/// <param name="remediation">Optional guidance on how to resolve the issue.</param>
public sealed class ApiInitializationIssue
(
    string path,
    ApiInitializationSeverity severity,
    ApiInitializationCode code,
    string description,
    string? remediation
)
{
    #region ApiInitializationIssue Properties
    /// <summary>
    ///     Gets the path to the schema element where the issue occurred.
    /// </summary>
    public string Path { get; } = path;

    /// <summary>
    ///     Gets the severity level of the issue.
    /// </summary>
    public ApiInitializationSeverity Severity { get; } = severity;

    /// <summary>
    ///     Gets the specific error or warning code identifying the issue.
    /// </summary>
    public ApiInitializationCode Code { get; } = code;

    /// <summary>
    ///     Gets a human-readable description of the issue.
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    ///     Gets optional guidance on how to resolve the issue.
    /// </summary>
    public string? Remediation { get; } = remediation;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var path = this.Path.SafeToString();
        var severity = this.Severity.SafeToString();
        var code = this.Code.SafeToString();
        var description = this.Description.SafeToString();
        var remediation = this.Remediation.SafeToString();

        return $"{nameof(ApiInitializationIssue)} {{{nameof(this.Path)}={path}, {nameof(this.Severity)}={severity}, {nameof(this.Code)}={code}, {nameof(this.Description)}={description}, {nameof(this.Remediation)}={remediation}}}";
    }
    #endregion

    #region ApiInitializationIssue Methods
    /// <summary>
    ///     Converts the issue to a formatted message string.
    /// </summary>
    /// <returns>A formatted string containing the issue's path, severity, code, description, and optional remediation.</returns>
    public string ToMessage()
    {
        var core = $"{this.Path}: {this.Severity} | {this.Code} - {this.Description}.";
        return string.IsNullOrWhiteSpace(this.Remediation)
            ? core
            : $"{core} {this.Remediation}.";
    }
    #endregion
}
