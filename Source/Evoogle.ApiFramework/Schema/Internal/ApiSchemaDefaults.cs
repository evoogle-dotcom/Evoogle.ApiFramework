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
internal static class ApiSchemaDefaults
{
    #region Key Defaults
    public const ApiKeyNullHandling KeyNullHandling = ApiKeyNullHandling.UseEmptyOnNull;
    public const ApiKeyPartNameFormat KeyPartNameFormat = ApiKeyPartNameFormat.None;
    #endregion

    #region Query Defaults
    public const ApiTypeModifiers NullableTypeModifiers = ApiTypeModifiers.None;
    public const ApiTypeModifiers NonNullableTypeModifiers = ApiTypeModifiers.Required;
    public const ApiQueryComposition QuerySourceOutputComposition = ApiQueryComposition.Projection;
    public const bool QuerySourceParameterRequiresArgument = true;
    #endregion

    #region Relationship Defaults
    public const ApiRelationshipDeleteBehavior OneToOneRelationshipDeleteBehavior = ApiRelationshipDeleteBehavior.None;
    public const ApiRelationshipDeleteBehavior OneToManyRelationshipDeleteBehavior = ApiRelationshipDeleteBehavior.None;
    public const ApiRelationshipDeleteBehavior ManyToManyRelationshipDeleteBehavior = ApiRelationshipDeleteBehavior.Delete;
    #endregion

    #region Schema Defaults
    public const string ApiVersion = "0.1.0";
    #endregion
}
