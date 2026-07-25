// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Fluent builder used to compose an <see cref="ApiAnnotationReaderSet"/>.
/// </summary>
public sealed class ApiAnnotationReaderSetBuilder
{
    #region Fields
    private readonly List<IApiAnnotationReader> _readers = [];
    #endregion

    #region Methods
    /// <summary>
    ///     Appends a reader to the end of the ordered reader list.
    ///     Readers run in registration order; later readers may override earlier ones
    ///     at the same <see cref="Internal.ApiConfigurationSource.DataAnnotation"/> precedence level.
    /// </summary>
    /// <param name="reader">The annotation reader to add.</param>
    /// <returns>The current builder instance.</returns>
    public ApiAnnotationReaderSetBuilder AddReader(IApiAnnotationReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        _readers.Add(reader);
        return this;
    }

    /// <summary>
    ///     Builds the configured <see cref="ApiAnnotationReaderSet"/>.
    /// </summary>
    /// <returns>The built reader set.</returns>
    public ApiAnnotationReaderSet Build() => new([.. _readers]);
    #endregion
}
