// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Key;

namespace Evoogle.ApiFramework.Schema;

public sealed partial class ApiKeyType
{
    #region MaterializeKey Methods
    /// <summary>
    ///     Materializes an <see cref="ApiKey"/> by walking each <see cref="ApiKeyPath"/> against the CLR object instances
    ///     supplied in <paramref name="context"/>.
    /// </summary>
    /// <param name="context">The materialization context containing the CLR instances and configuration.</param>
    /// <returns>
    ///     A composite <see cref="ApiKey"/> whose part values are read from the configured key paths.
    ///     Part names are created by <see cref="ApiKeyMaterializationContext.PartNameBuilder"/>;
    ///     a builder may return <see langword="null"/> to create unnamed/positional parts.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when no root object is registered in <paramref name="context"/> for a path's
    ///     <see cref="ApiKeyPath.ClrRootType"/>.
    /// </exception>
    /// <exception cref="ApiKeyException">
    ///     Thrown when <see cref="ApiKeyMaterializationContext.NullHandling"/> is
    ///     <see cref="ApiKeyNullHandling.ThrowOnNull"/> and any property in a path — whether an
    ///     intermediate navigation property or the terminal scalar property — is <see langword="null"/>.
    /// </exception>
    public ApiKey MaterializeKey(ApiKeyMaterializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // All keys produce a named-composite ApiKey regardless of path count,
        // so callers can always inspect part names uniformly.
        var parts = new ApiKeyPart[this.ApiKeyPaths.Length];
        for (var i = 0; i < this.ApiKeyPaths.Length; i++)
        {
            var path = this.ApiKeyPaths[i];
            var partName = context.PartNameBuilder(new ApiKeyPartNameContext(this, path, i));
            var partValue = WalkPath(path, context);
            parts[i] = new ApiKeyPart(partName, partValue);
        }

        return ApiKey.Composite(parts);
    }
    #endregion

    #region MaterializeKey Implementation Methods
    private static ApiKey WalkPath(ApiKeyPath path, ApiKeyMaterializationContext context)
    {
        var pathName = string.Join(".", path.ApiSegments.Select(static s => s.ClrPropertyName));

        var rootObject = context.TryResolveRoot(path.ClrRootType, out var root)
            ? root
            : null;

        if (rootObject is null)
        {
            if (context.NullHandling == ApiKeyNullHandling.ThrowOnNull)
            {
                throw new ApiKeyException($"Cannot walk key path '{pathName}': no root object registered for type '{path.ClrRootType.Name}'.");
            }

            return ApiKey.Empty;
        }

        var current = (object?)rootObject;

        foreach (var segment in path.ApiSegments)
        {
            if (current is null)
            {
                if (context.NullHandling == ApiKeyNullHandling.ThrowOnNull)
                {
                    throw new ApiKeyException($"Cannot walk key path '{pathName}': navigation property '{segment.ClrPropertyName}' resolved to null.");
                }

                return ApiKey.Empty;
            }

            current = segment.ApiProperty.GetValue(current);
        }

        if (current is null)
        {
            if (context.NullHandling == ApiKeyNullHandling.ThrowOnNull)
            {
                throw new ApiKeyException($"Key path '{pathName}' resolved to a null scalar value.");
            }

            return ApiKey.Empty;
        }

        return ApiKey.FromObject(current, path.ApiScalarSegment.ApiProperty.ApiType.ClrType);
    }
    #endregion
}
