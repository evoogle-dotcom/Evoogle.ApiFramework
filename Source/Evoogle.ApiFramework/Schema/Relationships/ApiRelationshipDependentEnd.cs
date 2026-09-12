// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Relationships;

/// <summary>
///     Represents the dependent end of an <see cref="ApiRelationship"/>.
///
///     A dependent end may declare a foreign key binding where <see cref="ApiForeignKey"/>
///     maps scalar leaves of the principal <see cref="ApiKeyDefinition"/> to properties on the dependent object graph.
/// </summary>
/// <remarks>
///     Use <see cref="HasForeignKey"/> before accessing <see cref="ApiForeignKey"/>.
///
///     When no foreign key is declared, the owning relationship is navigational at the schema level.
/// </remarks>
/// <param name="clrObjectType">The CLR type of the dependent <see cref="ApiObjectType"/>.</param>
/// <param name="apiForeignKey">
///     The optional <see cref="ApiKeyDefinition"/> that maps the principal key's scalar leaves to properties
///     on the dependent object graph.
/// </param>
[JsonConverter(typeof(ApiRelationshipDependentEndJsonConverter))]
public sealed class ApiRelationshipDependentEnd(Type clrObjectType, ApiKeyDefinition? apiForeignKey = null) : ApiRelationshipEnd(clrObjectType)
{
    #region ApiRelationshipDependentEnd Fields
    private readonly ApiKeyDefinition? _apiForeignKey = apiForeignKey;
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
    ///     Gets the foreign key role's <see cref="ApiKeyDefinition"/> that maps scalar leaves of the principal key
    ///     to properties on this dependent object graph.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when <see cref="HasForeignKey"/> is <see langword="false"/>.
    /// </exception>
    public ApiKeyDefinition ApiForeignKey => this.HasForeignKey
        ? _apiForeignKey!
        : throw new ApiSchemaException("No foreign key declared for this dependent end of the relationship.");
    #endregion

    #region ApiRelationshipDependentEnd Computed Properties
    /// <summary>
    ///    Gets a value indicating whether this dependent end has an explicit foreign key declared at the schema level.
    /// </summary>
    public bool HasForeignKey => _apiForeignKey is not null;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrObjectType = this.ClrObjectType.SafeToName();
        var apiForeignKey = _apiForeignKey?.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipDependentEnd)} {{{nameof(this.ClrObjectType)}={clrObjectType}, {nameof(this.ApiForeignKey)}={apiForeignKey}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override IEnumerable<ApiSchemaElement> GetOwnedElements()
    {
        if (_apiForeignKey is not null)
        {
            yield return _apiForeignKey;
        }
    }

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        this.ResolveApiForeignKey(context);
    }
    #endregion

    #region Implementation Methods
    private void ResolveApiForeignKey(ApiSchemaCompilationContext context)
    {
        if (!this.HasForeignKey)
        {
            // No foreign key declared; the owning relationship is navigational.
            return;
        }

        var location = ApiSchemaCompilationLocation.ForRole(nameof(this.ApiForeignKey));
        _apiForeignKey!.Compile(context, location);
    }
    #endregion
}
