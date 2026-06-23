// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Key;
using Evoogle.ApiFramework.Schema.Internal;

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
    ///     Part names are formatted by <see cref="ApiKeyMaterializationContext.CustomPartNameFormatter"/>,
    ///     when provided; otherwise they are formatted according to <see cref="ApiKeyMaterializationContext.PartNameFormat"/>.
    /// </returns>
    /// <exception cref="ApiKeyException">
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

        return this.MaterializeKeyCore(context, WalkPath);
    }

    /// <summary>
    ///     Materializes an <see cref="ApiKey"/> from caller-supplied values registered in <paramref name="context"/>.
    /// </summary>
    /// <param name="context">The materialization context containing value-path inputs and configuration.</param>
    /// <returns>
    ///     A composite <see cref="ApiKey"/> whose part values are read from the configured key paths.
    ///     Text values are parsed according to the terminal scalar CLR type for each path.
    /// </returns>
    /// <exception cref="ApiKeyException">
    ///     Thrown when a required value is missing, a text value cannot be parsed, a supplied
    ///     <see cref="ApiKey"/> has the wrong kind, or the terminal scalar CLR type is unsupported.
    /// </exception>
    public ApiKey MaterializeKeyFromValues(ApiKeyMaterializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return this.MaterializeKeyCore(context, MaterializeValuePath);
    }
    #endregion

    #region MaterializeKey Implementation Methods
    private ApiKey MaterializeKeyCore(ApiKeyMaterializationContext context, Func<ApiKeyPath, ApiKeyMaterializationContext, ApiKey> valueFactory)
    {
        // All keys produce a named-composite ApiKey regardless of path count,
        // so callers can always inspect part names uniformly.
        var partNameFormatter = context.CustomPartNameFormatter ?? ApiKeyPartNameFormatters.Resolve(context.PartNameFormat);
        var parts = new ApiKeyPart[this.ApiKeyPaths.Length];
        for (var i = 0; i < this.ApiKeyPaths.Length; i++)
        {
            var path = this.ApiKeyPaths[i];
            var partName = partNameFormatter(new ApiKeyPartNameContext(this, path, i, context.KeyTypeName));
            var partValue = valueFactory(path, context);
            parts[i] = new ApiKeyPart(partName, partValue);
        }

        return ApiKey.Composite(parts);
    }

    private static ApiKey WalkPath(ApiKeyPath path, ApiKeyMaterializationContext context)
    {
        var pathName = GetPathName(path);

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

    private static ApiKey MaterializeValuePath(ApiKeyPath path, ApiKeyMaterializationContext context)
    {
        var pathName = GetPathName(path);
        var expectedClrType = path.ApiScalarSegment.ApiProperty.ApiType.ClrType;

        if (context.TryResolveValue(path.ClrRootType, pathName, out var materializationValue) is false)
        {
            return HandleMissingValue(context, path, pathName);
        }

        return materializationValue.Kind switch
        {
            ApiKeyMaterializationValueKind.Key => MaterializeKeyValue(materializationValue.ApiKey, expectedClrType, context, path, pathName),
            ApiKeyMaterializationValueKind.Text => MaterializeTextValue(materializationValue.Text, expectedClrType, context, path, pathName),
            _ => throw new ApiKeyException($"Key path '{pathName}' has an unsupported materialization value kind '{materializationValue.Kind}'."),
        };

    }

    private static ApiKey MaterializeKeyValue(ApiKey apiKey, Type expectedClrType, ApiKeyMaterializationContext context, ApiKeyPath path, string pathName)
    {
        if (apiKey == ApiKey.Empty)
        {
            return HandleMissingValue(context, path, pathName);
        }

        var expectedKind = GetExpectedScalarKind(expectedClrType, pathName);
        if (apiKey.ApiKind != expectedKind)
        {
            throw new ApiKeyException($"Key path '{pathName}' expected {expectedKind} but received {apiKey.ApiKind}.");
        }

        return apiKey;
    }

    private static ApiKey MaterializeTextValue(string? text, Type expectedClrType, ApiKeyMaterializationContext context, ApiKeyPath path, string pathName)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return HandleMissingValue(context, path, pathName);
        }

        if (ApiKey.TryParse(expectedClrType, text, out var apiKey))
        {
            return apiKey;
        }

        var expectedKind = GetExpectedScalarKind(expectedClrType, pathName);
        throw new ApiKeyException($"Text '{text}' is not a valid {expectedKind} value for key path '{pathName}'.");
    }

    private static ApiKey HandleMissingValue(ApiKeyMaterializationContext context, ApiKeyPath path, string pathName)
    {
        if (context.NullHandling == ApiKeyNullHandling.ThrowOnNull)
        {
            throw new ApiKeyException($"Cannot materialize key path '{pathName}': no value registered for type '{path.ClrRootType.Name}'.");
        }

        return ApiKey.Empty;
    }

    private static string GetPathName(ApiKeyPath path) => string.Join(".", path.ApiSegments.Select(static s => s.ClrPropertyName));

    private static ApiKeyKind GetExpectedScalarKind(Type clrType, string pathName)
    {
        if (ApiKey.TryGetCompatibleScalarKind(clrType, out var kind))
        {
            return kind;
        }

        throw new ApiKeyException($"Key path '{pathName}' has unsupported terminal scalar CLR type '{clrType.Name}'.");
    }
    #endregion
}
