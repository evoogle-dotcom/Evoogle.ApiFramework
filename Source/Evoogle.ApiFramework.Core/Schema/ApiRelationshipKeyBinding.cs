// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the resolved key binding between a principal end's named key type and a
///     declared foreign key shape.
/// </summary>
public sealed class ApiRelationshipKeyBinding
{
    #region Constructors
    internal ApiRelationshipKeyBinding
    (
        ApiRelationshipPrincipalEnd apiPrincipalEnd,
        ApiNamedKeyType apiPrincipalKeyType,
        ApiKeyType apiForeignKeyType,
        ApiRelationshipPrincipalKeyResolutionSource apiPrincipalKeyResolutionSource
    )
    {
        this.ApiPrincipalEnd = apiPrincipalEnd ?? throw new ArgumentNullException(nameof(apiPrincipalEnd));
        this.ApiPrincipalKeyType = apiPrincipalKeyType ?? throw new ArgumentNullException(nameof(apiPrincipalKeyType));
        this.ApiForeignKeyType = apiForeignKeyType ?? throw new ArgumentNullException(nameof(apiForeignKeyType));
        this.ApiPrincipalKeyResolutionSource = apiPrincipalKeyResolutionSource;
    }
    #endregion

    #region ApiRelationshipKeyBinding Properties
    /// <summary>Gets the relationship principal end referenced by this binding.</summary>
    public ApiRelationshipPrincipalEnd ApiPrincipalEnd { get; }

    /// <summary>Gets the resolved named key type declared by the principal object type.</summary>
    public ApiNamedKeyType ApiPrincipalKeyType { get; }

    /// <summary>Gets the API name of the resolved principal key type.</summary>
    public string ApiPrincipalKeyTypeName => this.ApiPrincipalKeyType.ApiName;

    /// <summary>Gets the declared foreign key type that maps to the principal key type.</summary>
    public ApiKeyType ApiForeignKeyType { get; }

    /// <summary>Gets how the principal key type was selected.</summary>
    public ApiRelationshipPrincipalKeyResolutionSource ApiPrincipalKeyResolutionSource { get; }
    #endregion
}
