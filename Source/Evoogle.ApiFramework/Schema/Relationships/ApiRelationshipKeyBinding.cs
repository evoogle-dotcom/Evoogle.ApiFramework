// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Key;

namespace Evoogle.ApiFramework.Schema.Relationships;

/// <summary>
///     Represents the resolved key binding between a principal end's named key and a
///     declared foreign key shape.
/// </summary>
public sealed class ApiRelationshipKeyBinding
{
    #region Constructors
    internal ApiRelationshipKeyBinding
    (
        ApiRelationshipPrincipalEnd apiPrincipalEnd,
        ApiNamedKeyDefinition apiPrincipalKey,
        ApiKeyDefinition apiForeignKey,
        ApiRelationshipPrincipalKeyResolutionSource apiPrincipalKeyResolutionSource
    )
    {
        this.ApiPrincipalEnd = apiPrincipalEnd ?? throw new ArgumentNullException(nameof(apiPrincipalEnd));
        this.ApiPrincipalKey = apiPrincipalKey ?? throw new ArgumentNullException(nameof(apiPrincipalKey));
        this.ApiForeignKey = apiForeignKey ?? throw new ArgumentNullException(nameof(apiForeignKey));
        this.ApiPrincipalKeyResolutionSource = apiPrincipalKeyResolutionSource;
    }
    #endregion

    #region ApiRelationshipKeyBinding Properties
    /// <summary>Gets the relationship principal end referenced by this binding.</summary>
    public ApiRelationshipPrincipalEnd ApiPrincipalEnd { get; }

    /// <summary>Gets the resolved named key declared by the principal object type.</summary>
    public ApiNamedKeyDefinition ApiPrincipalKey { get; }

    /// <summary>Gets the API name of the resolved principal key.</summary>
    public string ApiPrincipalKeyName => this.ApiPrincipalKey.ApiName;

    /// <summary>Gets the declared foreign key that maps to the principal key.</summary>
    public ApiKeyDefinition ApiForeignKey { get; }

    /// <summary>Gets how the principal key was selected.</summary>
    public ApiRelationshipPrincipalKeyResolutionSource ApiPrincipalKeyResolutionSource { get; }
    #endregion
}
