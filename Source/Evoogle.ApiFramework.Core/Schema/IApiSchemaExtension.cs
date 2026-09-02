// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Defines extension metadata that can create an immutable runtime snapshot for a frozen schema.
/// </summary>
public interface IApiSchemaExtension
{
    /// <summary>
    ///     Creates a distinct immutable snapshot suitable for concurrent access from a frozen schema.
    /// </summary>
    /// <returns>A distinct, non-null immutable extension snapshot assignable to its registered key type.</returns>
    /// <remarks>
    ///     Implementations are responsible for deep immutability.
    ///     Thread-safe, non-authoritative caches are permitted when they cannot change schema meaning.
    /// </remarks>
    IApiSchemaExtension CreateFrozenSnapshot();
}
