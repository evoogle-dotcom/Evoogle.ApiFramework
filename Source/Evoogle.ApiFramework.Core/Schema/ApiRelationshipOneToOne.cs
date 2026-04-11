// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a one-to-one relationship between two <see cref="ApiObjectType"/> instances.
/// </summary>
/// <remarks>
///     <para>
///         The principal value appears exactly once among all principal objects.
///         A corresponding dependent value may or may not exist, but when it does it appears exactly once —
///         i.e. the dependent side is 0..1.
///     </para>
///     <para>
///         Self-referential one-to-one relationships are supported by setting both ends to the same
///         <see cref="ApiRelationshipEnd.ApiObjectTypeName"/>.
///     </para>
/// </remarks>
/// <param name="apiName">The schema-unique API name of the relationship.</param>
/// <param name="apiPrincipalEnd">The principal end, which owns the join key identity.</param>
/// <param name="apiDependentEnd">The dependent end, which holds the FK key paths.</param>
/// <param name="apiDisplayName">The optional human-readable display name for this relationship.</param>
/// <param name="apiDescription">The optional description for this relationship.</param>
public sealed class ApiRelationshipOneToOne
(
    string apiName,
    ApiRelationshipPrincipalEnd apiPrincipalEnd,
    ApiRelationshipDependentEnd apiDependentEnd,
    string? apiDisplayName = null,
    string? apiDescription = null
) : ApiRelationshipOneTo(apiName, apiPrincipalEnd, apiDependentEnd, apiDisplayName, apiDescription)
{
    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipOneToOne);
    #endregion

    #region ApiRelationship Properties
    /// <inheritdoc/>
    public override ApiRelationshipKind ApiKind => ApiRelationshipKind.OneToOne;
    #endregion
}
