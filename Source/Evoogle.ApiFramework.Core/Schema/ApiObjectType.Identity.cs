// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;

namespace Evoogle.ApiFramework.Schema;

public sealed partial class ApiObjectType
{
    public bool TryBuildIdentity(object clrInstance, out ApiId id, string? apiIdentityName = null)
    {
        id = default;
        if (clrInstance is null || !this.HasIdentity)
        {
            return false;
        }

        var identity = this.ResolveIdentityForBuild(apiIdentityName);
        if (identity is null)
        {
            return false;
        }

        return BuildIdentityFromInstance(identity, clrInstance, out id);
    }

    public bool TryBuildIdentity(IReadOnlyDictionary<string, object?> values, out ApiId id, string? apiIdentityName = null)
    {
        id = default;
        if (values is null || !this.HasIdentity)
        {
            return false;
        }

        var identity = this.ResolveIdentityForBuild(apiIdentityName);
        if (identity is null)
        {
            return false;
        }

        return BuildIdentityFromValues(identity, values, out id);
    }

    private ApiIdentity? ResolveIdentityForBuild(string? apiIdentityName)
    {
        if (!string.IsNullOrWhiteSpace(apiIdentityName))
        {
            return this.TryGetIdentityByApiName(apiIdentityName, out var id) ? id : null;
        }

        return this.ApiIdentitySet!.ApiPrimaryIdentity;
    }

    private static ApiId FinalizeComposite(List<ApiIdPart> parts)
        => parts.Count switch
        {
            0 => ApiId.Empty,
            1 => parts[0].Value,
            _ => ApiId.Composite(parts)
        };

    private static ApiId CoerceRawToApiId(ApiIdentityPart part, object? raw)
    {
        if (part.Coercion?.Converter is not null)
        {
            return part.Coercion.Converter(raw);
        }

        if (raw is null)
        {
            return ApiId.Empty;
        }

        if (raw is ApiId pre)
        {
            return pre;
        }

        var target = part.Coercion?.TargetKind;
        if (raw is string s && target.HasValue)
        {
            return target.Value switch
            {
                ApiIdentityTargetKind.Int32 => ApiId.TryParse(ApiIdKind.Int32, s, out var i32) ? i32 : ApiId.FromString(s),
                ApiIdentityTargetKind.Int64 => ApiId.TryParse(ApiIdKind.Int64, s, out var i64) ? i64 : ApiId.FromString(s),
                ApiIdentityTargetKind.Guid => ApiId.TryParse(ApiIdKind.Guid, s, out var g) ? g : ApiId.FromString(s),
                ApiIdentityTargetKind.Ulid => ApiId.TryParse(ApiIdKind.Ulid, s, out var u) ? u : ApiId.FromString(s),
                ApiIdentityTargetKind.Culture => ApiId.TryParse(ApiIdKind.Culture, s, out var c) ? c : ApiId.FromString(s),
                _ => ApiId.Parse(s)
            };
        }

        return raw switch
        {
            int i32 => ApiId.FromInt32(i32),
            long i64 => ApiId.FromInt64(i64),
            Guid g => ApiId.FromGuid(g),
            Ulid u => ApiId.FromUlid(u),
            System.Globalization.CultureInfo c => ApiId.FromCulture(c),
            string str => ApiId.Parse(str),
            _ => ApiId.FromString(raw?.ToString() ?? string.Empty)
        };
    }

    private static bool BuildIdentityFromInstance(ApiIdentity identity, object clrInstance, out ApiId id)
    {
        id = default;
        // Disallow mixed named/ordered parts (already validated); just in case:
        var anyOrdered = identity.ApiIdentityParts.Any(p => p.EmitAsOrdered);
        var anyNamed = identity.ApiIdentityParts.Any(p => !p.EmitAsOrdered);
        if (anyOrdered && anyNamed)
        {
            return false;
        }

        var parts = new List<ApiIdPart>(identity.ApiIdentityParts.Length);
        foreach (var part in identity.ApiIdentityParts)
        {
            // Assumes ApiProperty can fetch CLR value; if not available, adapt to your accessor layer.
            if (!part.ApiProperty.TryGetValue(clrInstance, out var raw))
            {
                return false;
            }

            var pid = CoerceRawToApiId(part, raw);
            if (!pid.HasValue)
            {
                return false;
            }

            parts.Add(part.EmitAsOrdered
                ? ApiIdPart.Create(pid)
                : ApiIdPart.Create(part.ApiProperty.ApiName, pid));
        }

        id = FinalizeComposite(parts);
        return id.HasValue;
    }

    private static bool BuildIdentityFromValues(ApiIdentity identity, IReadOnlyDictionary<string, object?> values, out ApiId id)
    {
        id = default;
        var anyOrdered = identity.ApiIdentityParts.Any(p => p.EmitAsOrdered);
        var anyNamed = identity.ApiIdentityParts.Any(p => !p.EmitAsOrdered);
        if (anyOrdered && anyNamed)
        {
            return false;
        }

        var parts = new List<ApiIdPart>(identity.ApiIdentityParts.Length);
        foreach (var part in identity.ApiIdentityParts)
        {
            if (!values.TryGetValue(part.ApiPropertyName, out var raw))
            {
                return false;
            }

            var pid = CoerceRawToApiId(part, raw);
            if (!pid.HasValue)
            {
                return false;
            }

            parts.Add(part.EmitAsOrdered
                ? ApiIdPart.Create(pid)
                : ApiIdPart.Create(part.ApiPropertyName, pid));
        }

        id = FinalizeComposite(parts);
        return id.HasValue;
    }
}