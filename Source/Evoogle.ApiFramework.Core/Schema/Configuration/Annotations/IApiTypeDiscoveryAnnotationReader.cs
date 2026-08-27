// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

/// <summary>Reads CLR types to discover during assembly annotation scanning.</summary>
public interface IApiTypeDiscoveryAnnotationReader : IApiAnnotationReader
{
    #region Methods
    /// <summary>Reads type-discovery contributions from an assembly.</summary>
    /// <param name="assembly">The assembly currently being scanned.</param>
    /// <param name="filter">The optional caller-supplied type filter.</param>
    /// <returns>The type-discovery contributions and any reader-emitted diagnostics.</returns>
    ApiAnnotationReaderResult<ApiTypeDiscoveryAnnotationResult> ReadTypeDiscoveryAnnotations
    (
        Assembly assembly,
        Func<Type, bool>? filter
    );
    #endregion
}
