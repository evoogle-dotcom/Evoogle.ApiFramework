// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

public sealed class ApiInitializationIssue
(
    string path,
    ApiInitializationSeverity severity,
    ApiInitializationCode code,
    string description,
    string? remediation
)
{
    #region Properties
    public string Path { get; } = path;

    public ApiInitializationSeverity Severity { get; } = severity;

    public ApiInitializationCode Code { get; } = code;

    public string Description { get; } = description;

    public string? Remediation { get; } = remediation;
    #endregion

    #region Methods
    public string ToMessage()
    {
        var core = $"{this.Path}: {this.Severity} | {this.Code} - {this.Description}.";
        return string.IsNullOrWhiteSpace(this.Remediation)
            ? core
            : $"{core} {this.Remediation}.";
    }
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
}
