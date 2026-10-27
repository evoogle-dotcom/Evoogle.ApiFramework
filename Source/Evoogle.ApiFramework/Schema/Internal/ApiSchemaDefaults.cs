// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
/// <remarks>
///     These constants define semantic defaults, which may differ from CLR defaults. Every default
///     represented in schema JSON must be passed as <c>defaultValue</c> to the corresponding
///     <c>TryWriteProperty*</c> call in the schema JSON converters, using the same default when reading
///     an omitted property. This preserves values when serializing with <c>WhenWritingDefault</c>.
///     When adding or changing a default, update all corresponding converter reads and writes together.
///     Defaults used only during key materialization have no corresponding schema JSON property.
/// </remarks>
internal static class ApiSchemaDefaults
{
    #region Key Defaults
    public const ApiKeyNullHandling KeyNullHandling = ApiKeyNullHandling.UseEmptyOnNull;
    public const ApiKeyPartNameFormat KeyPartNameFormat = ApiKeyPartNameFormat.None;
    #endregion

    #region Query Defaults
    public const ApiNullability QuerySourceOutputNullability = ApiNullability.Nullable;
    public const ApiNullability QuerySourceParameterNullability = ApiNullability.NonNullable;
    public const ApiNullability QuerySourceCollectionOutputNullability = ApiNullability.NonNullable;
    public const ApiNullability QuerySourceCollectionOutputItemNullability = ApiNullability.NonNullable;
    public const ApiQueryComposition QuerySourceOutputComposition = ApiQueryComposition.Projection;
    public const ApiPresence QuerySourceParameterPresence = ApiPresence.Required;
    #endregion

    #region Relationship Defaults
    public const ApiRelationshipDeleteBehavior OneToOneRelationshipDeleteBehavior = ApiRelationshipDeleteBehavior.None;
    public const ApiRelationshipDeleteBehavior OneToManyRelationshipDeleteBehavior = ApiRelationshipDeleteBehavior.None;
    public const ApiRelationshipDeleteBehavior ManyToManyRelationshipDeleteBehavior = ApiRelationshipDeleteBehavior.Delete;
    #endregion

    #region Schema Defaults
    public const ApiNullability PropertyNullability = ApiNullability.Nullable;
    public const ApiNullability CollectionItemNullability = ApiNullability.Nullable;
    public const string ApiVersion = "0.1.0";
    #endregion
}
