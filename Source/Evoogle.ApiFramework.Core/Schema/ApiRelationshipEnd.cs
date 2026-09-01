// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Abstract base class for the ends of an <see cref="ApiRelationship"/>.
///     Each end describes one participating <see cref="ApiObjectType"/>.
/// </summary>
public abstract class ApiRelationshipEnd : ApiRelationshipElement
{
    #region ApiRelationshipEnd Fields
    private const string _ownershipErrorMessage = $"An {nameof(ApiRelationshipEnd)} must be owned by an {nameof(ApiRelationship)}.";
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override sealed ApiSchemaElementKind Kind => this.ApiKind switch
    {
        ApiRelationshipEndKind.Principal => ApiSchemaElementKind.RelationshipPrincipalEnd,
        ApiRelationshipEndKind.Dependent => ApiSchemaElementKind.RelationshipDependentEnd,
        _ => throw new ArgumentOutOfRangeException(nameof(this.ApiKind))
    };
    #endregion

    #region ApiRelationshipEnd Properties
    /// <summary>Gets the kind of this relationship end, either <see cref="ApiRelationshipEndKind.Principal"/> or <see cref="ApiRelationshipEndKind.Dependent"/>.</summary>
    public abstract ApiRelationshipEndKind ApiKind { get; }

    /// <summary>
    ///     Gets the <see cref="ApiRelationship"/> that owns this end.
    ///     Derived from <see cref="ApiSchemaElement.Parent"/> and available after topology construction.
    /// </summary>
    public ApiRelationship ApiRelationship => this.Parent as ApiRelationship
        ?? throw new ApiSchemaException(_ownershipErrorMessage);
    #endregion

    #region Constructors
    internal ApiRelationshipEnd(Type clrObjectType)
        : base(clrObjectType)
    {
    }
    #endregion
}
