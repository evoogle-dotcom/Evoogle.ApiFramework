// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

#region Result Types
/// <summary>Describes a diagnostic emitted by an annotation reader.</summary>
/// <param name="Code">The initialization code identifying the diagnostic.</param>
/// <param name="ApiPath">The API path associated with the diagnostic.</param>
/// <param name="Description">The human-readable diagnostic description.</param>
/// <param name="Remediation">Optional guidance for resolving the diagnostic.</param>
/// <param name="Exception">The exception associated with the diagnostic, if any.</param>
/// <param name="Severity">The severity assigned to the diagnostic.</param>
public sealed record ApiAnnotationReaderDiagnostic
(
    ApiInitializationCode Code,
    string ApiPath,
    string Description,
    string? Remediation = null,
    Exception? Exception = null,
    ApiInitializationSeverity Severity = ApiInitializationSeverity.Error
);

/// <summary>Contains annotation-reader contributions and reader-emitted diagnostics.</summary>
/// <typeparam name="TContribution">The type of contribution returned by the reader.</typeparam>
/// <param name="Contributions">The contributions returned by the reader.</param>
/// <param name="Diagnostics">The diagnostics emitted by the reader.</param>
public sealed record ApiAnnotationReaderResult<TContribution>
(
    IReadOnlyList<TContribution> Contributions,
    IReadOnlyList<ApiAnnotationReaderDiagnostic> Diagnostics
);
#endregion
