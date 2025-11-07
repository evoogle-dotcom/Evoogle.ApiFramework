// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Fluent builder for constructing composite <see cref="ApiId"/> values from ordered or named parts.
/// </summary>
/// <remarks>
///     The builder accumulates parts via <see cref="Add(ApiId)"/> and <see cref="Add(string, ApiId)"/> and produces a validated composite identifier using <see cref="Build"/>.
///     Mixed named and unnamed parts or nested composites are rejected by validation in <see cref="ApiId"/>.
/// </remarks>
public sealed class ApiIdCompositeBuilder
{
    #region Fields
    /// <summary>Internal mutable collection of parts accumulated during building.</summary>
    private List<ApiIdPart>? _parts;
    #endregion

    #region Builder Methods
    /// <summary>
    ///     Adds an unnamed (positional) identifier part to the composite sequence.
    /// </summary>
    /// <param name="value">The identifier value to add.</param>
    /// <returns>The current <see cref="ApiIdCompositeBuilder"/> for fluent chaining.</returns>
    public ApiIdCompositeBuilder Add(ApiId value)
    {
        _parts ??= [];
        _parts.Add(new ApiIdPart(null, value));
        return this;
    }

    /// <summary>
    ///     Adds a named identifier part to the composite sequence.
    /// </summary>
    /// <param name="name">The name associated with the part.</param>
    /// <param name="value">The identifier value to add.</param>
    /// <returns>The current <see cref="ApiIdCompositeBuilder"/> for fluent chaining.</returns>
    public ApiIdCompositeBuilder Add(string name, ApiId value)
    {
        _parts ??= [];
        _parts.Add(new ApiIdPart(name, value));
        return this;
    }

    /// <summary>
    ///     Builds the composite <see cref="ApiId"/> from the accumulated parts.
    ///     Returns <see cref="ApiId.Empty"/> if no parts were added.
    /// </summary>
    /// <returns>The constructed composite identifier or <see cref="ApiId.Empty"/>.</returns>
    public ApiId Build()
    {
        return ApiId.Composite(_parts?.ToArray());
    }
    #endregion
}
