// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Declares a one-to-one or one-to-many relationship at the type level, without requiring
///     a navigation property on the POCO. Apply multiple instances for multiple relationships.
///     Registered by <see cref="Configuration.ApiAttributeAnnotationReader"/>.
/// </summary>
/// <remarks>
///     Use this attribute when the POCO does not expose navigation properties and you want to
///     keep the domain model clean. For POCOs with navigation properties prefer
///     <see cref="ApiRelationshipAttribute"/> on the navigation property.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class ApiRelationshipTypeAttribute : ApiNamedElementAttribute
{
    #region Constructors
    /// <summary>
    ///     Initializes a new <see cref="ApiRelationshipTypeAttribute"/>.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="principalType">The CLR type of the principal end.</param>
    /// <param name="dependentType">The CLR type of the dependent end.</param>
    public ApiRelationshipTypeAttribute(string apiName, Type principalType, Type dependentType)
        : base(RequireApiName(apiName))
    {
        ArgumentNullException.ThrowIfNull(principalType);
        ArgumentNullException.ThrowIfNull(dependentType);

        this.PrincipalType = principalType;
        this.DependentType = dependentType;
    }
    #endregion

    #region Properties
    /// <summary>Gets the schema-unique API name of the relationship.</summary>
    public new string ApiName => base.ApiName!;

    /// <summary>Gets the CLR type of the principal end.</summary>
    public Type PrincipalType { get; }

    /// <summary>Gets the CLR type of the dependent end.</summary>
    public Type DependentType { get; }

    /// <summary>Gets or sets the relationship kind. Defaults to <see cref="ApiRelationshipKind.OneToMany"/>.</summary>
    public ApiRelationshipKind Kind { get; set; } = ApiRelationshipKind.OneToMany;

    /// <summary>
    ///     Gets or sets the name of the foreign-key CLR property on the dependent type.
    ///     When <c>null</c> the framework infers the foreign key via convention.
    /// </summary>
    public string? ForeignKey { get; set; }

    /// <summary>Gets or sets the delete behavior for the relationship.</summary>
    public ApiRelationshipDeleteBehavior DeleteBehavior { get; set; } = ApiRelationshipDeleteBehavior.None;
    #endregion
}
