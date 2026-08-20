// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Declares a many-to-many relationship at the type level without requiring navigation
///     properties on the POCO. Apply multiple instances for multiple M:N relationships.
///     Registered by <see cref="Configuration.ApiAttributeAnnotationReader"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class ApiManyToManyRelationshipTypeAttribute : ApiNamedElementAttribute
{
    #region Constructors
    /// <summary>
    ///     Initializes a new <see cref="ApiManyToManyRelationshipTypeAttribute"/>.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="principalTypeA">The CLR type of the first principal end.</param>
    /// <param name="principalTypeB">The CLR type of the second principal end.</param>
    /// <param name="associationType">The CLR type of the association (join) entity.</param>
    public ApiManyToManyRelationshipTypeAttribute
    (
        string apiName,
        Type principalTypeA,
        Type principalTypeB,
        Type associationType
    )
        : base(RequireApiName(apiName))
    {
        ArgumentNullException.ThrowIfNull(principalTypeA);
        ArgumentNullException.ThrowIfNull(principalTypeB);
        ArgumentNullException.ThrowIfNull(associationType);

        this.PrincipalTypeA = principalTypeA;
        this.PrincipalTypeB = principalTypeB;
        this.AssociationType = associationType;
    }
    #endregion

    #region Properties
    /// <summary>Gets the schema-unique API name of the relationship.</summary>
    public new string ApiName => base.ApiName!;

    /// <summary>Gets the CLR type of the first principal end.</summary>
    public Type PrincipalTypeA { get; }

    /// <summary>Gets the CLR type of the second principal end.</summary>
    public Type PrincipalTypeB { get; }

    /// <summary>Gets the CLR type of the association (join) entity.</summary>
    public Type AssociationType { get; }

    /// <summary>
    ///     Gets or sets the CLR property name on <see cref="AssociationType"/> that holds
    ///     the foreign key back to <see cref="PrincipalTypeA"/>.
    /// </summary>
    public string? ForeignKeyA { get; set; }

    /// <summary>
    ///     Gets or sets the CLR property name on <see cref="AssociationType"/> that holds
    ///     the foreign key back to <see cref="PrincipalTypeB"/>.
    /// </summary>
    public string? ForeignKeyB { get; set; }
    #endregion
}
