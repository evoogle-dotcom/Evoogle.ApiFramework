// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework;

/// <summary>
///     Declares a many-to-many relationship at the type level without requiring navigation properties on the POCO.
///     Apply multiple instances for multiple M:N relationships.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class ApiManyToManyRelationshipDefinitionAttribute : ApiNamedElementAttribute
{
    #region Fields
    private Type _principalTypeA = null!;
    private Type _principalTypeB = null!;
    private Type _associationType = null!;
    #endregion

    #region Properties
    /// <summary>Gets the the schema-unique API name of the relationship.</summary>
    public new required string ApiName
    {
        get => base.ApiName!;
        init => base.ApiName = RequireApiName(value);
    }

    /// <summary>Gets the the CLR type of the first principal end.</summary>
    public required Type PrincipalTypeA
    {
        get => _principalTypeA;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            _principalTypeA = value;
        }
    }

    /// <summary>Gets the the CLR type of the second principal end.</summary>
    public required Type PrincipalTypeB
    {
        get => _principalTypeB;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            _principalTypeB = value;
        }
    }

    /// <summary>Gets the the CLR type of the association (join) entity.</summary>
    public required Type AssociationType
    {
        get => _associationType;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            _associationType = value;
        }
    }

    /// <summary>
    ///     Gets the the CLR property name on <see cref="AssociationType"/> that holds the foreign key back to <see cref="PrincipalTypeA"/>.
    /// </summary>
    public string? ForeignKeyA { get; init; }

    /// <summary>
    ///     Gets the the CLR property name on <see cref="AssociationType"/> that holds the foreign key back to <see cref="PrincipalTypeB"/>.
    /// </summary>
    public string? ForeignKeyB { get; init; }
    #endregion
}
