// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the dependent end of an <see cref="ApiRelationship"/>.
///
///     A dependent end is either <em>navigational</em> — where no foreign key binding is declared
///     at the schema level — or <em>key-bound</em>, where <see cref="ApiForeignKeyType"/>
///     explicitly maps scalar leaves of the principal <see cref="ApiKeyType"/> to
///     properties on the dependent object graph.
/// </summary>
/// <remarks>
///     Use <see cref="HasForeignKey"/> to determine which state applies before accessing <see cref="ApiForeignKeyType"/>.
///
///     The state is fixed at construction and does not change after initialization.
/// </remarks>
/// <param name="clrObjectType">The CLR type of the dependent <see cref="ApiObjectType"/>.</param>
/// <param name="apiForeignKeyType">
///     The optional <see cref="ApiKeyType"/> that maps the principal key type's scalar leaves to properties
///     on the dependent object graph. When <see langword="null"/>, the dependent end is navigational.
/// </param>
[JsonConverter(typeof(ApiRelationshipDependentEndJsonConverter))]
public sealed class ApiRelationshipDependentEnd(Type clrObjectType, ApiKeyType? apiForeignKeyType = null) : ApiRelationshipEnd(clrObjectType)
{
    #region ApiRelationshipDependentEnd Fields
    private readonly ApiKeyType? _apiForeignKeyType = apiForeignKeyType;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipDependentEnd);
    #endregion

    #region ApiRelationshipEnd Properties
    /// <inheritdoc/>
    public override ApiRelationshipEndKind ApiKind => ApiRelationshipEndKind.Dependent;
    #endregion

    #region ApiRelationshipDependentEnd Properties
    /// <summary>
    ///     Gets the foreign key role's <see cref="ApiKeyType"/> that maps scalar leaves of the principal key type
    ///     to properties on this dependent object graph.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="IsNavigational"/> is <see langword="true"/>.
    ///     Check <see cref="HasForeignKey"/> before accessing this property.
    /// </exception>
    public ApiKeyType ApiForeignKeyType => this.HasForeignKey
        ? _apiForeignKeyType!
        : throw new ApiSchemaException("No foreign key declared for this dependent end of the relationship.");
    #endregion

    #region ApiRelationshipDependentEnd Computed Properties
    /// <summary>
    ///    Gets a value indicating whether this dependent end has an explicit foreign key declared at the schema level.
    /// </summary>
    public bool HasForeignKey => _apiForeignKeyType is not null;

    /// <summary>
    ///   Gets a value indicating whether this dependent end is navigational (i.e. has no explicit foreign key declared at the schema level).
    /// </summary>
    public bool IsNavigational => !this.HasForeignKey;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrObjectType = this.ClrObjectType.SafeToName();
        var apiForeignKeyType = _apiForeignKeyType?.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipDependentEnd)} {{{nameof(this.ClrObjectType)}={clrObjectType}, {nameof(this.ApiForeignKeyType)}={apiForeignKeyType}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiForeignKeyType(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiForeignKeyType(ApiInitializationContext context)
    {
        if (this.IsNavigational)
        {
            // No foreign key declared — purely navigational relationship.
            return;
        }

        var fkContext = context.WithDeclaringSchemaElement(this);
        _apiForeignKeyType!.Initialize(fkContext);
    }
    #endregion
}
