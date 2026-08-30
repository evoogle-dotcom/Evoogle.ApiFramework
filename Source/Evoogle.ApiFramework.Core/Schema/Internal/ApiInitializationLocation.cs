// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     Describes how a schema element is located within initialization ancestry and diagnostics.
/// </summary>
internal readonly struct ApiInitializationLocation
{
    #region Fields
    private readonly string? _apiLabel;
    private readonly string? _apiPathBase;
    private readonly string? _apiRole;
    private readonly int _index;
    private readonly ApiInitializationLocationKind _kind;
    #endregion

    #region Constructors
    private ApiInitializationLocation
    (
        ApiInitializationLocationKind kind,
        string? apiRole,
        int index,
        string? apiLabel,
        string? apiPathBase
    )
    {
        _kind = kind;
        _apiRole = apiRole;
        _index = index;
        _apiLabel = apiLabel;
        _apiPathBase = apiPathBase;
    }
    #endregion

    #region Factory Methods
    public static ApiInitializationLocation ForRole(string apiRole)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiRole);

        return new ApiInitializationLocation
        (
            ApiInitializationLocationKind.Role,
            apiRole,
            index: 0,
            apiLabel: null,
            apiPathBase: null
        );
    }

    public static ApiInitializationLocation ForIndexedLabel
    (
        int index,
        string? apiLabel,
        string? apiPathBase = null
    )
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        return new ApiInitializationLocation
        (
            ApiInitializationLocationKind.IndexedLabel,
            apiRole: null,
            index,
            apiLabel,
            apiPathBase
        );
    }
    #endregion

    #region Methods
    public string BuildPath(ApiSchemaElement apiSchemaElement, string defaultApiBasePath)
    {
        ArgumentNullException.ThrowIfNull(apiSchemaElement);

        return _kind switch
        {
            ApiInitializationLocationKind.Default => apiSchemaElement.BuildDefaultPath(defaultApiBasePath),
            ApiInitializationLocationKind.Role => ApiSchemaPathFormatting.BuildPath(defaultApiBasePath, _apiRole!, null),
            ApiInitializationLocationKind.IndexedLabel => ApiSchemaPathFormatting.BuildIndexedPath
                (
                    _apiPathBase ?? defaultApiBasePath,
                    apiSchemaElement.ApiElementTypeName,
                    _index,
                    _apiLabel
                ),
            _ => throw new InvalidOperationException
            (
                $"Unsupported {nameof(ApiInitializationLocationKind)} value '{_kind}'."
            ),
        };
    }
    #endregion

    #region Types
    private enum ApiInitializationLocationKind
    {
        Default,
        Role,
        IndexedLabel
    }
    #endregion
}
