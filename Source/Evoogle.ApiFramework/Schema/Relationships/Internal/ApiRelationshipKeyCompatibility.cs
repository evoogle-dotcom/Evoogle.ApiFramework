// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Key;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Relationships.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
internal static class ApiRelationshipKeyCompatibility
{
    #region Utility Methods
    public static int? CountKeyLeaves(ApiKeyDefinition keyDefinition) =>
        // Each ApiKeyPath in a key corresponds to exactly one scalar leaf.
        keyDefinition.ApiKeyPaths.Length;

    public static bool AreKeysCompatible(ApiKeyDefinition principalKey, ApiKeyDefinition foreignKey)
        => TryAreKeysCompatible(principalKey, foreignKey, out var isCompatible) && isCompatible;

    public static bool TryAreKeysCompatible(ApiKeyDefinition principalKey, ApiKeyDefinition foreignKey, out bool isCompatible)
    {
        isCompatible = false;

        if (!TryGetKeyLeafTypes(principalKey, out var principalLeafTypes) ||
            !TryGetKeyLeafTypes(foreignKey, out var foreignLeafTypes))
        {
            return false;
        }

        if (principalLeafTypes.Length != foreignLeafTypes.Length)
        {
            return true;
        }

        for (var i = 0; i < principalLeafTypes.Length; i++)
        {
            if (principalLeafTypes[i] != foreignLeafTypes[i])
            {
                return true;
            }
        }

        isCompatible = true;
        return true;
    }

    public static string DescribeKeyLeafTypes(ApiKeyDefinition keyDefinition)
    {
        if (!TryGetKeyLeafTypes(keyDefinition, out var leafTypes))
        {
            return "(unresolved)";
        }

        return string.Join(", ", leafTypes.Select(static clrType => clrType.SafeToName()));
    }

    private static bool TryGetKeyLeafTypes(ApiKeyDefinition keyDefinition, out Type[] leafTypes)
    {
        var paths = keyDefinition.ApiKeyPaths;
        leafTypes = new Type[paths.Length];

        for (var i = 0; i < paths.Length; i++)
        {
            var scalarSegment = paths[i].ApiSegments.Length > 0 ? paths[i].ApiScalarSegment : null;
            if (scalarSegment?.IsPropertyResolved != true || !scalarSegment.ApiProperty.IsResolved)
            {
                leafTypes = [];
                return false;
            }

            leafTypes[i] = scalarSegment.ApiProperty.ApiType.ClrType;
        }

        return true;
    }
    #endregion
}
