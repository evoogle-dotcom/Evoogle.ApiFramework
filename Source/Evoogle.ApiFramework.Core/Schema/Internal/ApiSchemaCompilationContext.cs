// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal sealed class ApiSchemaCompilationContext
{
    #region Properties
    public ApiSchema ApiSchema => this.Session.ApiSchema;

    public string ApiPath { get; }

    public ApiSchemaElement CurrentElement { get; }

    public IEnumerable<ApiSchemaCompilationIssue> Issues => this.Session.Issues;

    public ApiSchemaCompilationLocation Location { get; }

    public ApiSchemaCompilationSession Session { get; }
    #endregion

    #region Constructors
    internal ApiSchemaCompilationContext
    (
        ApiSchemaCompilationSession session,
        ApiSchemaElement currentElement,
        ApiSchemaCompilationLocation location,
        string apiPath
    )
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(currentElement);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiPath);

        this.Session = session;
        this.CurrentElement = currentElement;
        this.Location = location;
        this.ApiPath = apiPath;
    }
    #endregion

    #region Methods
    public void AddIssue
    (
        ApiSchemaCompilationSeverity severity,
        ApiSchemaCompilationCode code,
        string description,
        string? remediation
    )
    => this.Session.AddIssue(this.ApiPath, severity, code, description, remediation);

    public void AddIssue
    (
        string apiPath,
        ApiSchemaCompilationSeverity severity,
        ApiSchemaCompilationCode code,
        string description,
        string? remediation
    )
    => this.Session.AddIssue(apiPath, severity, code, description, remediation);
    #endregion
}
