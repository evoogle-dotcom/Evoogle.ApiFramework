// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

/// <summary>Reads query-source declarations during assembly annotation scanning.</summary>
public interface IApiQuerySourceAnnotationReader : IApiAnnotationReader
{
    #region Methods
    /// <summary>Reads query-source annotation contributions from an assembly.</summary>
    /// <param name="assembly">The assembly being scanned.</param>
    /// <param name="filter">The optional caller-supplied delegate-type filter.</param>
    /// <returns>The query-source contributions and reader diagnostics.</returns>
    ApiAnnotationReaderResult<ApiQuerySourceAnnotationResult> ReadQuerySourceAnnotations
    (
        Assembly assembly,
        Func<Type, bool>? filter
    );
    #endregion
}
