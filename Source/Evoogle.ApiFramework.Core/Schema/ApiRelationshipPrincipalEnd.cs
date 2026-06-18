// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the principal end of an <see cref="ApiRelationship"/>.
///
///     The principal end identifies the object type that participates as a principal in a relationship.
///     Key-bound relationships resolve the concrete principal key binding on the owning relationship.
/// </summary>
/// <param name="clrObjectType">The CLR type of the principal <see cref="ApiObjectType"/>.</param>
/// <param name="apiKeyTypeName">
///     The optional name of the <see cref="ApiKeyType"/> on the principal type that should be used by the owning
///     relationship's key binding. When <see langword="null"/>, key-bound relationship initialization uses the foreign
///     key binding to infer the best compatible key type on the principal object type.
/// </param>
[JsonConverter(typeof(ApiRelationshipPrincipalEndJsonConverter))]
public sealed class ApiRelationshipPrincipalEnd(Type clrObjectType, string? apiKeyTypeName = null) : ApiRelationshipEnd(clrObjectType)
{
    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipPrincipalEnd);
    #endregion

    #region ApiRelationshipEnd Properties
    /// <inheritdoc/>
    public override ApiRelationshipEndKind ApiKind => ApiRelationshipEndKind.Principal;
    #endregion

    #region ApiRelationshipPrincipalEnd Properties
    /// <summary>
    ///     Gets the optional explicit principal key type name used by the owning relationship's key binding.
    ///     When <see langword="null"/>, key-bound relationships infer the best compatible principal key type from the
    ///     corresponding foreign key.
    /// </summary>
    public string? ApiPrincipalKeyTypeName { get; } = apiKeyTypeName;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrObjectType = this.ClrObjectType.SafeToName();
        var apiKeyTypeName = this.ApiPrincipalKeyTypeName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipPrincipalEnd)} {{{nameof(this.ClrObjectType)}={clrObjectType}, {nameof(this.ApiPrincipalKeyTypeName)}={apiKeyTypeName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);
    }
    #endregion
}
