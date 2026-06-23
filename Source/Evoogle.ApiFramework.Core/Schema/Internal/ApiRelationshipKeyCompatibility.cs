// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Internal;

/// <summary>
///     This API supports the Evoogle.ApiFramework infrastructure and is not intended to be used directly from your code.
///     This API may change or be removed in future releases.
/// </summary>
internal static class ApiRelationshipKeyCompatibility
{
    #region Utility Methods
    public static int? CountKeyLeaves(ApiKeyType keyType)
    {
        // Each ApiKeyPath in a key type corresponds to exactly one scalar leaf.
        return keyType.ApiKeyPaths.Length;
    }

    public static bool AreKeyTypesCompatible(ApiKeyType principalKeyType, ApiKeyType foreignKeyType)
        => TryAreKeyTypesCompatible(principalKeyType, foreignKeyType, out var isCompatible) && isCompatible;

    public static bool TryAreKeyTypesCompatible(ApiKeyType principalKeyType, ApiKeyType foreignKeyType, out bool isCompatible)
    {
        isCompatible = false;

        if (!TryGetKeyLeafTypes(principalKeyType, out var principalLeafTypes) ||
            !TryGetKeyLeafTypes(foreignKeyType, out var foreignLeafTypes))
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

    public static string DescribeKeyLeafTypes(ApiKeyType keyType)
    {
        if (!TryGetKeyLeafTypes(keyType, out var leafTypes))
        {
            return "(unresolved)";
        }

        return string.Join(", ", leafTypes.Select(static clrType => clrType.SafeToName()));
    }

    private static bool TryGetKeyLeafTypes(ApiKeyType keyType, out Type[] leafTypes)
    {
        var paths = keyType.ApiKeyPaths;
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
