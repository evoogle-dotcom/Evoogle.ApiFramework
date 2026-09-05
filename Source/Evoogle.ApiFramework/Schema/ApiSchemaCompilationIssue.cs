// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents an issue encountered during API schema compilation.
/// </summary>
/// <param name="apiPath">The API path to the schema element where the issue occurred.</param>
/// <param name="severity">The severity level of the issue.</param>
/// <param name="code">The specific error or warning code identifying the issue.</param>
/// <param name="description">A human-readable description of the issue.</param>
/// <param name="remediation">Optional guidance on how to resolve the issue.</param>
/// <param name="readerType">
///     The annotation reader type that produced the issue, if applicable.
/// </param>
/// <param name="exception">
///     The exception associated with the issue, if applicable.
/// </param>
public sealed class ApiSchemaCompilationIssue
(
    string apiPath,
    ApiSchemaCompilationSeverity severity,
    ApiSchemaCompilationCode code,
    string description,
    string? remediation,
    Type? readerType = null,
    Exception? exception = null
)
{
    #region ApiSchemaCompilationIssue Properties
    /// <summary>
    ///     Gets the API path to the schema element where the issue occurred.
    /// </summary>
    public string ApiPath { get; } = apiPath;

    /// <summary>
    ///     Gets the severity level of the issue.
    /// </summary>
    public ApiSchemaCompilationSeverity Severity { get; } = severity;

    /// <summary>
    ///     Gets the specific error or warning code identifying the issue.
    /// </summary>
    public ApiSchemaCompilationCode Code { get; } = code;

    /// <summary>
    ///     Gets a human-readable description of the issue.
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    ///     Gets optional guidance on how to resolve the issue.
    /// </summary>
    public string? Remediation { get; } = remediation;

    /// <summary>
    ///     Gets the annotation reader type that produced the issue, if applicable.
    /// </summary>
    public Type? ReaderType { get; } = readerType;

    /// <summary>
    ///     Gets the exception associated with the issue, if applicable.
    /// </summary>
    public Exception? Exception { get; } = exception;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiPath = this.ApiPath.SafeToString();
        var severity = this.Severity.SafeToString();
        var code = this.Code.SafeToString();
        var description = this.Description.SafeToString();
        var remediation = this.Remediation.SafeToString();

        return $"{nameof(ApiSchemaCompilationIssue)} {{{nameof(this.ApiPath)}={apiPath}, {nameof(this.Severity)}={severity}, {nameof(this.Code)}={code}, {nameof(this.Description)}={description}, {nameof(this.Remediation)}={remediation}}}";
    }
    #endregion

    #region ApiSchemaCompilationIssue Methods
    /// <summary>
    ///     Converts the issue to a formatted message string.
    /// </summary>
    /// <returns>A formatted string containing the issue's API path, severity, code, description, and optional remediation.</returns>
    public string ToMessage()
    {
        var core = $"{this.ApiPath}: {this.Severity} | {this.Code} - {this.Description}.";
        return string.IsNullOrWhiteSpace(this.Remediation)
            ? core
            : $"{core} {this.Remediation}.";
    }
    #endregion
}
