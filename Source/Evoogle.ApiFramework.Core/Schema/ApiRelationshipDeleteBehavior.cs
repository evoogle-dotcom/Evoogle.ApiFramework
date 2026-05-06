// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Defines the behavior applied to related objects when either end of an <see cref="ApiRelationship"/>
///     is affected by a delete or orphan operation.
/// </summary>
/// <remarks>
///     <para>
///         This behavior is declared at the <see cref="ApiRelationship"/> level and answers two related questions
///         using a single consistent concept:
///     </para>
///     <list type="bullet">
///         <item>
///             <description>
///                 <strong>Principal deleted:</strong> what happens to the dependent objects when the principal object is deleted?
///             </description>
///         </item>
///         <item>
///             <description>
///                 <strong>Dependent orphaned:</strong> what happens to a dependent object when it is explicitly removed
///                 from the relationship while the principal continues to exist — for example, when a child is removed
///                 from a collection via a PATCH or PUT operation?
///             </description>
///         </item>
///     </list>
///     <para>
///         The same value governs both scenarios because the outcome — what happens to the related object —
///         is conceptually identical regardless of which end triggers the operation.
///     </para>
/// </remarks>
public enum ApiRelationshipDeleteBehavior
{
    #region Values
    /// <summary>
    ///     No delete rule is declared. The downstream layer (persistence, API handler) determines
    ///     the behavior, or the behavior is intentionally left unspecified.
    ///     This is the default value.
    ///     <list type="bullet">
    ///         <item><description><strong>Principal deleted:</strong> no action is enforced on dependent objects.</description></item>
    ///         <item><description><strong>Dependent orphaned:</strong> no action is enforced on the orphaned dependent.</description></item>
    ///     </list>
    /// </summary>
    None,

    /// <summary>
    ///     The operation is blocked if related objects exist on the opposite end.
    ///     The caller must explicitly remove or reassign those objects before the operation is permitted.
    ///     <list type="bullet">
    ///         <item><description><strong>Principal deleted:</strong> deletion is blocked while any dependent objects exist.</description></item>
    ///         <item><description><strong>Dependent orphaned:</strong> the orphaning operation is blocked; the dependent must be explicitly deleted first.</description></item>
    ///     </list>
    /// </summary>
    Restrict,

    /// <summary>
    ///     Related objects on the opposite end are automatically deleted when the operation occurs.
    ///     <list type="bullet">
    ///         <item><description><strong>Principal deleted:</strong> all dependent objects are deleted in the same operation.</description></item>
    ///         <item><description><strong>Dependent orphaned:</strong> the orphaned dependent is automatically deleted.</description></item>
    ///     </list>
    /// </summary>
    Delete,

    /// <summary>
    ///     The foreign-key properties on related objects are set to <see langword="null"/> when the operation occurs,
    ///     leaving those objects intact as unassociated records.
    ///     Only valid when the dependent foreign-key is nullable.
    ///     <list type="bullet">
    ///         <item><description><strong>Principal deleted:</strong> the FK properties on all dependent objects are set to <see langword="null"/>.</description></item>
    ///         <item><description><strong>Dependent orphaned:</strong> the FK properties on the orphaned dependent are set to <see langword="null"/>.</description></item>
    ///     </list>
    /// </summary>
    SetNull
    #endregion
}
