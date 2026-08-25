// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Declares a many-to-many relationship on a navigation property.
/// </summary>
/// <remarks>
///     Place on the navigation property of either principal end.
///     To declare without a navigation property use <see cref="ApiManyToManyRelationshipDefinitionAttribute"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ApiManyToManyRelationshipAttribute : ApiNamedElementAttribute
{
    #region Fields
    private Type _associationType = null!;
    private Type _otherPrincipalType = null!;
    #endregion

    #region Properties
    /// <summary>Gets or initializes the schema-unique API name of the relationship.</summary>
    public new required string ApiName
    {
        get => base.ApiName!;
        init => base.ApiName = RequireApiName(value);
    }

    /// <summary>Gets or initializes the CLR type of the association (join) entity.</summary>
    public required Type AssociationType
    {
        get => this._associationType;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._associationType = value;
        }
    }

    /// <summary>
    ///     Gets or initializes the CLR type of the other principal end (not the type carrying this attribute).
    /// </summary>
    public required Type OtherPrincipalType
    {
        get => this._otherPrincipalType;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._otherPrincipalType = value;
        }
    }

    /// <summary>
    ///     Gets or initializes the CLR property name on <see cref="AssociationType"/> that holds the foreign key back to the type carrying this attribute.
    /// </summary>
    public string? ForeignKeyA { get; init; }

    /// <summary>
    ///     Gets or initializes the CLR property name on <see cref="AssociationType"/> that holds the foreign key back to <see cref="OtherPrincipalType"/>.
    /// </summary>
    public string? ForeignKeyB { get; init; }
    #endregion
}
