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
    #region Properties
    /// <summary>Gets or initializes the schema-unique API name of the relationship.</summary>
    public new required string ApiName
    {
        get => base.ApiName!;
        init => base.ApiName = RequireApiName(value);
    }

    /// <summary>Gets or initializes the CLR type of the principal end.</summary>
    public required Type PrincipalType
    {
        get => this._principalType;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._principalType = value;
        }
    }

    /// <summary>Gets or initializes the CLR type of the dependent end.</summary>
    public required Type DependentType
    {
        get => this._dependentType;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._dependentType = value;
        }
    }

    /// <summary>
    ///     Gets or initializes the relationship kind. Defaults to
    ///     <see cref="ApiRelationshipKind.OneToMany"/>.
    /// </summary>
    public ApiRelationshipKind Kind { get; init; } = ApiRelationshipKind.OneToMany;

    /// <summary>
    ///     Gets or initializes the name of the foreign-key CLR property on the dependent type.
    ///     When <c>null</c> the framework infers the foreign key via convention.
    /// </summary>
    public string? ForeignKey { get; init; }

    /// <summary>Gets or initializes the delete behavior for the relationship.</summary>
    public ApiRelationshipDeleteBehavior DeleteBehavior { get; init; } =
        ApiRelationshipDeleteBehavior.None;
    #endregion

    #region Fields
    private Type _principalType = null!;
    private Type _dependentType = null!;
    #endregion
}
