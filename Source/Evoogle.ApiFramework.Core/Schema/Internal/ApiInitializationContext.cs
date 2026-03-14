// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal class ApiInitializationContext
{
    #region Fields
    private readonly string? _apiDeclaringPath;
    private readonly ApiObjectType? _apiDeclaringObjectType;
    private readonly List<ApiInitializationIssue> _issues; // shared, non-null
    private readonly HashSet<string> _identityTraversalPath; // shared, tracks identity resolution path for cycle detection
    #endregion

    #region Properties
    public ApiSchema ApiSchema { get; }

    public string? ApiDeclaringPath => _apiDeclaringPath;

    public ApiObjectType ApiDeclaringObjectType => _apiDeclaringObjectType ?? throw new ApiSchemaException($"No parent {nameof(ApiObjectType)} in this context.");

    public IEnumerable<ApiInitializationIssue> Issues => _issues;

    internal HashSet<string> IdentityTraversalPath => _identityTraversalPath;
    #endregion

    #region Constructor
    private ApiInitializationContext
    (
        ApiSchema apiSchema,
        string? apiDeclaringPath,
        ApiObjectType? apiDeclaringObjectType,
        List<ApiInitializationIssue> issues,
        HashSet<string> identityTraversalPath
    )
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(issues);
        ArgumentNullException.ThrowIfNull(identityTraversalPath);

        _apiDeclaringPath = apiDeclaringPath;
        _apiDeclaringObjectType = apiDeclaringObjectType;

        this.ApiSchema = apiSchema;
        _issues = issues; // keep shared reference
        _identityTraversalPath = identityTraversalPath; // keep shared reference
    }
    #endregion

    #region Methods
    // public void AddIssue(ApiInitializationIssue issue)
    // {
    //     _issues.Add(issue);
    // }

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
            apiDeclaringPath: null,
            apiDeclaringObjectType: null,
            issues: [], // always non-null shared list
            identityTraversalPath: [] // always non-null shared set
        );
    }

    public ApiInitializationContext WithDeclaringObjectType(ApiObjectType apiDeclaringObjectType)
    {
        var apiDeclaringPath = apiDeclaringObjectType.ApiPath;

        return new ApiInitializationContext
        (
            this.ApiSchema,
            apiDeclaringPath,
            apiDeclaringObjectType,
            _issues, // share the same list
            _identityTraversalPath // share the same set
        );
    }

    public ApiInitializationContext WithDeclaringSchemaElement(ApiSchemaElement apiDeclaringSchemaElement)
    {
        var apiDeclaringPath = apiDeclaringSchemaElement.ApiPath;

        return new ApiInitializationContext
        (
            this.ApiSchema,
            apiDeclaringPath,
            _apiDeclaringObjectType,
            _issues, // share the same list
            _identityTraversalPath // share the same set
        );
    }
    #endregion
}
