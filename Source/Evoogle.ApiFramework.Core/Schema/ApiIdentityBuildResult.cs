// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the result of attempting to build an identity for a CLR instance.
/// </summary>
/// <remarks>
///     <para>This type is returned by batch identity building operations to indicate success or failure for each instance.</para>
///     <para><b>Usage Pattern:</b></para>
///     <code>
///     var results = objectType.TryBuildIdentities(instances);
///     var successes = results.Where(r => r.Success);
///     var failures = results.Where(r => !r.Success);
///     
///     foreach (var result in results)
///     {
///         if (result.Success)
///             Console.WriteLine($"Built identity {result.Id} for instance");
///         else
///             Console.WriteLine($"Failed to build identity for instance");
///     }
///     </code>
/// </remarks>
public sealed class ApiIdentityBuildResult
{
    /// <summary>
    ///     Gets the CLR instance for which the identity was attempted to be built.
    /// </summary>
    public required object Instance { get; init; }

    /// <summary>
    ///     Gets the built identity if successful; otherwise, <see cref="ApiId.Empty"/>.
    /// </summary>
    public required ApiId Id { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the identity was successfully built.
    /// </summary>
    /// <remarks>
    ///     When <c>true</c>, <see cref="Id"/> contains a valid identity.
    ///     When <c>false</c>, <see cref="Id"/> will be <see cref="ApiId.Empty"/>.
    /// </remarks>
    public required bool Success { get; init; }
}
