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
    #endregion

    #region Properties
    public ApiSchema ApiSchema { get; }

    public string? ApiDeclaringPath => _apiDeclaringPath;

    public ApiObjectType ApiDeclaringObjectType => _apiDeclaringObjectType ?? throw new ApiSchemaException($"No parent {nameof(ApiObjectType)} in this context.");

    public IEnumerable<ApiInitializationIssue> Issues => _issues;
    #endregion

    #region Constructor
    private ApiInitializationContext
    (
        ApiSchema apiSchema,
        string? apiDeclaringPath,
        ApiObjectType? apiDeclaringObjectType,
        List<ApiInitializationIssue> issues
    )
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(issues);

        _apiDeclaringPath = apiDeclaringPath;
        _apiDeclaringObjectType = apiDeclaringObjectType;

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
            apiDeclaringPath: null,
            apiDeclaringObjectType: null,
            issues: [] // always non-null shared list
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
            _issues // share the same list
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
            _issues // share the same list
        );
    }

    /// <summary>
    ///     Creates a child context with a new declaring object type but the same declaring path.
    ///     Use this when you need <see cref="ApiDeclaringObjectType"/> for property lookup but must
    ///     preserve the current path chain (e.g., inside relationship key path initialization).
    /// </summary>
    public ApiInitializationContext WithDeclaringObjectTypeOnly(ApiObjectType apiDeclaringObjectType)
    {
        return new ApiInitializationContext
        (
            this.ApiSchema,
            _apiDeclaringPath, // preserve current path unchanged
            apiDeclaringObjectType,
            _issues // share the same list
        );
    }
    #endregion
}
