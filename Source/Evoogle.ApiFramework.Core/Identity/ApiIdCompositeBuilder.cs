// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Identity;

public sealed class ApiIdCompositeBuilder
{
    private readonly List<ApiIdPart> _parts = [];
    private bool _requireUniqueNames;
    private IComparer<string?> _nameComparer = StringComparer.Ordinal;

    public ApiIdCompositeBuilder EnforceUniqueNames(bool require = true, IComparer<string?>? comparer = null)
    {
        _requireUniqueNames = require;
        if (comparer is not null)
        {
            _nameComparer = comparer;
        }
        return this;
    }

    public ApiIdCompositeBuilder Add(ApiId value)
    {
        _parts.Add(new ApiIdPart(null, value));
        return this;
    }

    public ApiIdCompositeBuilder Add(string name, ApiId value)
    {
        _parts.Add(new ApiIdPart(name, value));
        return this;
    }

    public ApiId Build()
    {
        if (_parts.Count == 0)
        {
            return ApiId.Empty;
        }

        // No mixing + no nested composites
        var anyNamed = false;
        var anyUnnamed = false;
        foreach (var p in _parts)
        {
            if (p.Value.Kind == ApiIdKind.Composite)
            {
                throw new ApiIdentityException("Nested composite parts are not allowed in ApiId.");
            }

            if (p.Name is null)
            {
                anyUnnamed = true;
            }
            else
            {
                anyNamed = true;
            }

            if (anyNamed && anyUnnamed)
            {
                throw new ApiIdentityException("Cannot mix named and unnamed parts in the same composite ApiId.");
            }
        }

        if (_requireUniqueNames)
        {
            var set = new SortedSet<string?>(_nameComparer);
            foreach (var p in _parts)
            {
                if (p.Name is null)
                {
                    continue;
                }

                if (!set.Add(p.Name))
                {
                    throw new ApiIdentityException($"Duplicate composite part name '{p.Name}'.");
                }
            }
        }
        return ApiId.Composite(_parts.ToArray());
    }
}
