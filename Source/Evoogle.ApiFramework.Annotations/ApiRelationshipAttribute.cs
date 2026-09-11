// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework;

/// <summary>
///     Declares a one-to-one or one-to-many relationship on a navigation member.
/// </summary>
/// <remarks>
///     Place this attribute on the navigation member of the principal end.
///     To declare a relationship without a navigation member use
///     <see cref="ApiRelationshipDefinitionAttribute"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ApiRelationshipAttribute : ApiNamedElementAttribute
{
    #region Properties
    /// <summary>Gets the the schema-unique API name of the relationship.</summary>
    public new required string ApiName
    {
        get => base.ApiName!;
        init => base.ApiName = RequireApiName(value);
    }

    /// <summary>
    ///     Gets the the relationship kind.
    /// </summary>
    public required ApiRelationshipKind Kind { get; init; }

    /// <summary>
    ///     Gets the foreign-key CLR member name on the dependent end.
    ///     When <c>null</c> the framework infers the foreign key via convention.
    /// </summary>
    public string? ForeignKey { get; init; }

    /// <summary>Gets the the delete behavior for the relationship.</summary>
    public ApiRelationshipDeleteBehavior DeleteBehavior { get; init; } = ApiRelationshipDeleteBehavior.None;
    #endregion
}
