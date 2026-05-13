// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Abstract base class for the ends of an <see cref="ApiRelationship"/>.
///     Each end describes one participating <see cref="ApiObjectType"/>.
/// </summary>
/// <param name="clrObjectType">The CLR type of the participating <see cref="ApiObjectType"/> on this end of the relationship.</param>
public abstract class ApiRelationshipEnd(Type clrObjectType) : ApiRelationshipElement(clrObjectType)
{
    #region ApiRelationshipEnd Fields
    private ApiRelationship? _apiResolvedRelationship = null;
    #endregion

    #region ApiRelationshipEnd Properties
    /// <summary>Gets the kind of this relationship end, either <see cref="ApiRelationshipEndKind.Principal"/> or <see cref="ApiRelationshipEndKind.Dependent"/>.</summary>
    public abstract ApiRelationshipEndKind ApiKind { get; }

    /// <summary>
    ///     Gets the <see cref="ApiRelationship"/> that owns this end.
    ///     Available after schema initialization.
    /// </summary>
    public ApiRelationship ApiRelationship => this.ThrowIfNotInitialized(_apiResolvedRelationship);
    #endregion

    #region ApiRelationshipEnd Methods
    internal void SetRelationship(ApiRelationship relationship)
    {
        ArgumentNullException.ThrowIfNull(relationship);
        _apiResolvedRelationship = relationship;
    }
    #endregion
}
