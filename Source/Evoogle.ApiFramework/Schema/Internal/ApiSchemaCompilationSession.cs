// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     Holds state shared by every context created during one schema compilation.
/// </summary>
internal sealed class ApiSchemaCompilationSession
{
    #region Fields
    private readonly List<ApiSchemaCompilationIssue> _issues = [];
    #endregion

    #region Properties
    public ApiSchema ApiSchema { get; }

    public ApiSchemaContext ApiSchemaContext { get; }

    public IEnumerable<ApiSchemaCompilationIssue> Issues => _issues;

    public ILogger Logger { get; }
    #endregion

    #region Constructors
    public ApiSchemaCompilationSession
    (
        ApiSchema apiSchema,
        ApiSchemaContext apiSchemaContext
    )
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(apiSchemaContext);

        this.ApiSchema = apiSchema;
        this.ApiSchemaContext = apiSchemaContext;
        this.Logger = apiSchemaContext.Logger;
    }

    internal ApiSchemaCompilationSession(ApiSchema apiSchema, ILogger logger)
        : this(apiSchema, new ApiSchemaContext(apiSchema, logger))
    {
    }

    #endregion

    #region Methods
    public void AddIssue
    (
        string apiPath,
        ApiSchemaCompilationSeverity severity,
        ApiSchemaCompilationCode code,
        string description,
        string? remediation
    )
    {
        var issue = new ApiSchemaCompilationIssue(apiPath, severity, code, description, remediation);
        _issues.Add(issue);

        this.LogIssue(issue);
    }

    public void AddIssue(ApiSchemaCompilationIssue issue)
    {
        ArgumentNullException.ThrowIfNull(issue);
        _issues.Add(issue);
        this.LogIssue(issue);
    }

    public ApiSchemaCompilationContext CreateContext
    (
        ApiSchemaElement apiSchemaElement,
        ApiSchemaCompilationLocation location
    )
    {
        ArgumentNullException.ThrowIfNull(apiSchemaElement);

        if (!ReferenceEquals(apiSchemaElement.Root, this.ApiSchema))
        {
            throw new InvalidOperationException("A schema element can only be compiled by the session for its ownership tree.");
        }

        var defaultApiBasePath = ReferenceEquals(apiSchemaElement, this.ApiSchema)
            ? null
            : apiSchemaElement.Parent?.ApiPath ?? throw new InvalidOperationException("A non-root schema element must have a compiled structural parent.");
        var apiPath = location.BuildPath(apiSchemaElement, defaultApiBasePath);

        return new ApiSchemaCompilationContext
        (
            this,
            apiSchemaElement,
            location,
            apiPath
        );
    }
    #endregion

    #region Implementation Methods
    private void LogIssue(ApiSchemaCompilationIssue issue)
    {
        var logLevel = issue.Severity switch
        {
            ApiSchemaCompilationSeverity.Info => LogLevel.Information,
            ApiSchemaCompilationSeverity.Warning => LogLevel.Warning,
            ApiSchemaCompilationSeverity.Error => LogLevel.Error,
            _ => LogLevel.Error,
        };

        if (!this.Logger.IsEnabled(logLevel))
        {
            return;
        }

        var eventId = new EventId((int)issue.Code, issue.Code.ToString());

        if (string.IsNullOrWhiteSpace(issue.Remediation))
        {
            this.Logger.Log
            (
                logLevel,
                eventId,
                "API schema compilation issue {CompilationCode} at {ApiPath}: {Description}",
                issue.Code,
                issue.ApiPath,
                issue.Description
            );
        }
        else
        {
            this.Logger.Log
            (
                logLevel,
                eventId,
                "API schema compilation issue {CompilationCode} at {ApiPath}: {Description} Remediation: {Remediation}",
                issue.Code,
                issue.ApiPath,
                issue.Description,
                issue.Remediation
            );
        }
    }
    #endregion
}
