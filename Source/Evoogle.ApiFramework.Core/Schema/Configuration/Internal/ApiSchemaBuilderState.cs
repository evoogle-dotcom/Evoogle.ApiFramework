// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Conventions;
using Evoogle.ApiFramework.Schema.Configuration.Annotations;

namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Stores mutable schema-level configuration collected by the schema builder.
/// </summary>
internal sealed class ApiSchemaBuilderState
{
    #region Properties
    internal string? ApiName { get; set; }

    internal string? ApiVersion { get; set; }

    internal Action<ApiSchemaOptionsBuilder>? OptionsConfiguration { get; set; }

    internal ApiConventionSet? ConventionSet { get; set; }

    internal ApiAnnotationReaderSet? AnnotationReaderSet { get; set; }
    #endregion
}
