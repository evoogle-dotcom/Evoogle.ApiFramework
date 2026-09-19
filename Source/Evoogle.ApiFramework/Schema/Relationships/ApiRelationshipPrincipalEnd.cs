// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Relationships;

/// <summary>
///     Represents the principal end of an <see cref="ApiRelationship"/>.
///
///     The principal end identifies the object type that participates as a principal in a relationship.
///     Key-bound relationships resolve the concrete principal key binding on the owning relationship.
/// </summary>
/// <param name="apiObjectTypeReference">The reference to the principal <see cref="ApiObjectType"/>.</param>
/// <param name="apiTraversal">The optional traversal exposed from this end.</param>
/// <param name="apiPrincipalKeyName">
///     The optional name of the <see cref="ApiKeyDefinition"/> on the principal type that should be used by the owning
///     relationship's key binding. When <see langword="null"/>, key-bound relationship compilation uses the foreign
///     key binding to infer the best compatible key on the principal object type.
/// </param>
[JsonConverter(typeof(ApiRelationshipPrincipalEndJsonConverter))]
public sealed class ApiRelationshipPrincipalEnd
(
    ApiTypeReference apiObjectTypeReference,
    ApiRelationshipTraversal? apiTraversal = null,
    string? apiPrincipalKeyName = null
) : ApiRelationshipEnd(apiObjectTypeReference, apiTraversal)
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
    ///     Gets the optional explicit principal key name used by the owning relationship's key binding.
    ///     When <see langword="null"/>, key-bound relationships infer the best compatible principal key from the
    ///     corresponding foreign key.
    /// </summary>
    public string? ApiPrincipalKeyName { get; } = apiPrincipalKeyName;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiObjectTypeReference = this.ApiObjectTypeReference.SafeToString();
        var apiPrincipalKeyName = this.ApiPrincipalKeyName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipPrincipalEnd)} "
            + $"{{{nameof(this.ApiObjectTypeReference)}={apiObjectTypeReference}, "
            + $"{nameof(this.ApiPrincipalKeyName)}={apiPrincipalKeyName}, "
            + $"{nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);
    }
    #endregion
}
