// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

public class ApiInitializationContext
{
    #region Fields
    private readonly string? _apiParentPath;
    private readonly ApiObjectType? _apiParentObjectType;
    private readonly List<ApiInitializationIssue> _issues; // shared, non-null
    #endregion

    #region Properties
    public ApiSchema ApiSchema { get; }

    public string? ApiParentPath => _apiParentPath;

    public ApiObjectType ApiParentObjectType => _apiParentObjectType ?? throw new InvalidOperationException($"No parent {nameof(ApiObjectType)} in this context.");

    public IEnumerable<ApiInitializationIssue> Issues => _issues;
    #endregion

    #region Constructor
    private ApiInitializationContext
    (
        ApiSchema apiSchema,
        string? apiParentPath,
        ApiObjectType? apiParentObjectType,
        List<ApiInitializationIssue> issues
    )
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(issues);

        _apiParentPath = apiParentPath;
        _apiParentObjectType = apiParentObjectType;

        this.ApiSchema = apiSchema;
        _issues = issues; // keep shared reference
    }
    #endregion

    #region Methods
    public void AddIssue
    (
        string path,
        ApiInitializationSeverity severity,
        ApiInitializationCode code,
        string description,
        string? remediation
    )
    {
        var issue = new ApiInitializationIssue(path, severity, code, description, remediation);
        _issues.Add(issue);
    }

    public static ApiInitializationContext CreateRootContext(ApiSchema apiSchema)
    {
        return new ApiInitializationContext
        (
            apiSchema: apiSchema,
            apiParentPath: null,
            apiParentObjectType: null,
            issues: [] // always non-null shared list
        );
    }

    public ApiInitializationContext WithParentObjectType(ApiObjectType apiParentObjectType)
    {
        var apiParentPath = apiParentObjectType.ApiPath;

        return new ApiInitializationContext
        (
            this.ApiSchema,
            apiParentPath,
            apiParentObjectType,
            _issues // share the same list
        );
    }

    public ApiInitializationContext WithParentSchemaElement(ApiSchemaElement apiParentSchemaElement)
    {
        var apiParentPath = apiParentSchemaElement.ApiPath;

        return new ApiInitializationContext
        (
            this.ApiSchema,
            apiParentPath,
            _apiParentObjectType,
            _issues // share the same list
        );
    }
    #endregion
}
