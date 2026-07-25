// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Declares a one-to-one or one-to-many relationship on a navigation property.
///     The relationship is named, keyed by <see cref="Name"/>, and registered by
///     <see cref="Configuration.ApiAttributeAnnotationReader"/>.
/// </summary>
/// <remarks>
///     Place this attribute on the navigation property of the principal end.
///     To declare a relationship without a navigation property use <see cref="ApiRelationshipTypeAttribute"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ApiRelationshipAttribute : Attribute
{
    #region Constructors
    /// <summary>
    ///     Initializes a new <see cref="ApiRelationshipAttribute"/> with the given relationship name.
    /// </summary>
    /// <param name="name">The schema-unique API name of the relationship.</param>
    public ApiRelationshipAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        this.Name = name;
    }
    #endregion

    #region Properties
    /// <summary>Gets the schema-unique API name of the relationship.</summary>
    public string Name { get; }

    /// <summary>Gets or sets the relationship kind. Defaults to <see cref="ApiRelationshipKind.OneToMany"/>.</summary>
    public ApiRelationshipKind Kind { get; set; } = ApiRelationshipKind.OneToMany;

    /// <summary>
    ///     Gets or sets the name of the foreign-key CLR property on the dependent end.
    ///     When <c>null</c> the framework infers the foreign key via convention.
    /// </summary>
    public string? ForeignKey { get; set; }

    /// <summary>Gets or sets the delete behavior for the relationship.</summary>
    public ApiRelationshipDeleteBehavior DeleteBehavior { get; set; } = ApiRelationshipDeleteBehavior.None;
    #endregion
}
