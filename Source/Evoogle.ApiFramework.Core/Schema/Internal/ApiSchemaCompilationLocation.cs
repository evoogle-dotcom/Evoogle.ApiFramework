// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     Describes transient role or index metadata used to format a schema element's diagnostic
///     path.
/// </summary>
internal readonly struct ApiSchemaCompilationLocation
{
    #region Types
    private enum ApiSchemaCompilationLocationKind
    {
        Default,
        Role,
        IndexedLabel
    }
    #endregion

    #region Fields
    private readonly string? _apiLabel;
    private readonly string? _apiRole;
    private readonly int _index;
    private readonly ApiSchemaCompilationLocationKind _kind;
    #endregion

    #region Constructors
    private ApiSchemaCompilationLocation
    (
        ApiSchemaCompilationLocationKind kind,
        string? apiRole,
        int index,
        string? apiLabel
    )
    {
        _kind = kind;
        _apiRole = apiRole;
        _index = index;
        _apiLabel = apiLabel;
    }
    #endregion

    #region Factory Methods
    public static ApiSchemaCompilationLocation ForRole(string apiRole)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiRole);

        return new ApiSchemaCompilationLocation
        (
            ApiSchemaCompilationLocationKind.Role,
            apiRole,
            index: 0,
            apiLabel: null
        );
    }

    public static ApiSchemaCompilationLocation ForIndexedLabel(int index, string? apiLabel)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        return new ApiSchemaCompilationLocation
        (
            ApiSchemaCompilationLocationKind.IndexedLabel,
            apiRole: null,
            index,
            apiLabel
        );
    }
    #endregion

    #region Methods
    public string BuildPath(ApiSchemaElement apiSchemaElement, string? defaultApiBasePath)
    {
        ArgumentNullException.ThrowIfNull(apiSchemaElement);

        return _kind switch
        {
            ApiSchemaCompilationLocationKind.Default => apiSchemaElement.BuildDefaultPath(defaultApiBasePath),

            ApiSchemaCompilationLocationKind.Role => ApiSchemaPathFormatting.BuildPath
                (
                    defaultApiBasePath,
                    _apiRole!,
                    apiPathSegmentName: null
                ),

            ApiSchemaCompilationLocationKind.IndexedLabel => ApiSchemaPathFormatting.BuildIndexedPath
                (
                    defaultApiBasePath,
                    apiSchemaElement.ApiElementTypeName,
                    _index,
                    _apiLabel
                ),

            _ => throw new InvalidOperationException($"Unsupported {nameof(ApiSchemaCompilationLocationKind)} value '{_kind}'."),
        };
    }
    #endregion
}
