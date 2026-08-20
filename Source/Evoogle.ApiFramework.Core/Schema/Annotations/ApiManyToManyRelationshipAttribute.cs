// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Declares a many-to-many relationship on a navigation property.
///     Registered by <see cref="Configuration.ApiAttributeAnnotationReader"/>.
/// </summary>
/// <remarks>
///     Place on the navigation property of either principal end.
///     To declare without a navigation property use <see cref="ApiManyToManyRelationshipTypeAttribute"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ApiManyToManyRelationshipAttribute : ApiNamedElementAttribute
{
    #region Constructors
    /// <summary>
    ///     Initializes a new <see cref="ApiManyToManyRelationshipAttribute"/>.
    /// </summary>
    /// <param name="apiName">The schema-unique API name of the relationship.</param>
    /// <param name="associationType">The CLR type of the association/join entity.</param>
    /// <param name="otherPrincipalType">The CLR type of the other principal end.</param>
    public ApiManyToManyRelationshipAttribute(string apiName, Type associationType, Type otherPrincipalType)
        : base(RequireApiName(apiName))
    {
        ArgumentNullException.ThrowIfNull(associationType);
        ArgumentNullException.ThrowIfNull(otherPrincipalType);

        this.AssociationType = associationType;
        this.OtherPrincipalType = otherPrincipalType;
    }
    #endregion

    #region Properties
    /// <summary>Gets the schema-unique API name of the relationship.</summary>
    public new string ApiName => base.ApiName!;

    /// <summary>Gets the CLR type of the association (join) entity.</summary>
    public Type AssociationType { get; }

    /// <summary>Gets the CLR type of the other principal end (not the type carrying this attribute).</summary>
    public Type OtherPrincipalType { get; }

    /// <summary>
    ///     Gets or sets the CLR property name on <see cref="AssociationType"/> that holds the
    ///     foreign key back to the type carrying this attribute.
    /// </summary>
    public string? ForeignKeyA { get; set; }

    /// <summary>
    ///     Gets or sets the CLR property name on <see cref="AssociationType"/> that holds the
    ///     foreign key back to <see cref="OtherPrincipalType"/>.
    /// </summary>
    public string? ForeignKeyB { get; set; }
    #endregion
}
