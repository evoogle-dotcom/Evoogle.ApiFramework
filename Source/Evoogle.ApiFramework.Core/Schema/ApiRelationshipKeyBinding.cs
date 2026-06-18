// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the resolved key binding between a principal end's key type and a declared foreign key shape.
/// </summary>
/// <param name="apiPrincipalEnd">The relationship principal end referenced by this binding.</param>
/// <param name="apiPrincipalKeyType">The resolved principal key type.</param>
/// <param name="apiForeignKeyType">The declared foreign key type that maps to the principal key type.</param>
/// <param name="apiPrincipalKeyResolutionSource">How the principal key type was selected.</param>
public sealed class ApiRelationshipKeyBinding
(
    ApiRelationshipPrincipalEnd apiPrincipalEnd,
    ApiKeyType apiPrincipalKeyType,
    ApiKeyType apiForeignKeyType,
    ApiRelationshipPrincipalKeyResolutionSource apiPrincipalKeyResolutionSource
)
{
    #region ApiRelationshipKeyBinding Properties
    /// <summary>Gets the relationship principal end referenced by this binding.</summary>
    public ApiRelationshipPrincipalEnd ApiPrincipalEnd { get; } = apiPrincipalEnd ?? throw new ArgumentNullException(nameof(apiPrincipalEnd));

    /// <summary>Gets the resolved principal key type.</summary>
    public ApiKeyType ApiPrincipalKeyType { get; } = apiPrincipalKeyType ?? throw new ArgumentNullException(nameof(apiPrincipalKeyType));

    /// <summary>Gets the API name of the resolved principal key type.</summary>
    public string? ApiPrincipalKeyTypeName => this.ApiPrincipalKeyType.ApiName;

    /// <summary>Gets the declared foreign key type that maps to the principal key type.</summary>
    public ApiKeyType ApiForeignKeyType { get; } = apiForeignKeyType ?? throw new ArgumentNullException(nameof(apiForeignKeyType));

    /// <summary>Gets how the principal key type was selected.</summary>
    public ApiRelationshipPrincipalKeyResolutionSource ApiPrincipalKeyResolutionSource { get; } = apiPrincipalKeyResolutionSource;
    #endregion
}
