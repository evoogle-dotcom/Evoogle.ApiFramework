// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a one-to-many relationship between two <see cref="ApiObjectType"/> instances.
/// </summary>
/// <remarks>
///     <para>
///         The principal value appears exactly once among all principal objects.
///         A corresponding dependent value may appear zero or more times among all dependent objects.
///     </para>
///     <para>
///         Self-referential one-to-many relationships are supported by setting both ends to the same <see cref="ApiRelationshipElement.ClrObjectType"/>.
///     </para>
/// </remarks>
/// <param name="apiName">The schema-unique API name of the relationship.</param>
/// <param name="apiPrincipalEnd">The principal end, which owns the join key identity.</param>
/// <param name="apiDependentEnd">The dependent end, which holds the FK key paths.</param>
/// <param name="apiDeleteBehavior">The delete behavior applied when either end is affected. Defaults to <see cref="ApiRelationshipDeleteBehavior.None"/>.</param>
public sealed class ApiRelationshipOneToMany
(
    string apiName,
    ApiRelationshipPrincipalEnd apiPrincipalEnd,
    ApiRelationshipDependentEnd apiDependentEnd,
    ApiRelationshipDeleteBehavior apiDeleteBehavior = ApiRelationshipOneToMany.DefaultDeleteBehavior
) : ApiRelationshipOneTo(apiName, apiPrincipalEnd, apiDependentEnd, apiDeleteBehavior)
{
    #region ApiRelationshipOneToMany Fields
    public const ApiRelationshipDeleteBehavior DefaultDeleteBehavior = ApiRelationshipDeleteBehavior.None;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipOneToMany);
    #endregion

    #region ApiRelationship Properties
    /// <inheritdoc/>
    public override ApiRelationshipKind ApiKind => ApiRelationshipKind.OneToMany;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiPrincipalEnd = this.ApiPrincipalEnd.SafeToString();
        var apiDependentEnd = this.ApiDependentEnd.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipOneToMany)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiPrincipalEnd)}={apiPrincipalEnd}, {nameof(this.ApiDependentEnd)}={apiDependentEnd}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion
}
