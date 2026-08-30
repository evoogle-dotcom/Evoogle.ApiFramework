// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     Holds state shared by every context frame created during one schema initialization.
/// </summary>
internal sealed class ApiInitializationSession
{
    #region Fields
    private readonly List<ApiInitializationIssue> _issues = [];
    #endregion

    #region Properties
    public ApiSchema ApiSchema { get; }

    public IEnumerable<ApiInitializationIssue> Issues => _issues;

    public ILogger Logger { get; }
    #endregion

    #region Constructors
    public ApiInitializationSession(ApiSchema apiSchema)
        : this(apiSchema, apiSchema.ApiSchemaContext.Logger)
    { }

    internal ApiInitializationSession(ApiSchema apiSchema, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(logger);

        this.ApiSchema = apiSchema;
        this.Logger = logger;
    }
    #endregion

    #region Methods
    public void AddIssue
    (
        string apiPath,
        ApiInitializationSeverity severity,
        ApiInitializationCode code,
        string description,
        string? remediation
    )
    {
        var issue = new ApiInitializationIssue(apiPath, severity, code, description, remediation);
        _issues.Add(issue);

        this.LogIssue(issue);
    }

    public ApiInitializationContext CreateContext
    (
        ApiSchemaElement apiSchemaElement,
        ApiInitializationContext? parentContext,
        ApiInitializationLocation location
    )
    {
        ArgumentNullException.ThrowIfNull(apiSchemaElement);

        if (parentContext is not null && !ReferenceEquals(parentContext.Session, this))
        {
            throw new InvalidOperationException
            (
                "An initialization context cannot be reused across initialization sessions."
            );
        }

        var defaultApiBasePath = parentContext?.ApiPath ?? this.ApiSchema.ApiPath;
        var apiPath = location.BuildPath(apiSchemaElement, defaultApiBasePath);

        return new ApiInitializationContext
        (
            this,
            apiSchemaElement,
            parentContext,
            location,
            apiPath
        );
    }
    #endregion

    #region Implementation Methods
    private void LogIssue(ApiInitializationIssue issue)
    {
        var logLevel = issue.Severity switch
        {
            ApiInitializationSeverity.Info => LogLevel.Information,
            ApiInitializationSeverity.Warning => LogLevel.Warning,
            ApiInitializationSeverity.Error => LogLevel.Error,
            _ => LogLevel.Error,
        };

        var eventId = new EventId((int)issue.Code, issue.Code.ToString());

        if (string.IsNullOrWhiteSpace(issue.Remediation))
        {
            this.Logger.Log
            (
                logLevel,
                eventId,
                "API schema initialization issue {InitializationCode} at {ApiPath}: {Description}",
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
                "API schema initialization issue {InitializationCode} at {ApiPath}: "
                    + "{Description} Remediation: {Remediation}",
                issue.Code,
                issue.ApiPath,
                issue.Description,
                issue.Remediation
            );
        }
    }
    #endregion
}
