// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>
///     Describes the schema target associated with a build trace event.
/// </summary>
public sealed record ApiSchemaBuildTraceTarget
(
    ApiSchemaBuildTargetKind Kind,
    Type? ClrType = null,
    string? ClrName = null,
    string? ApiName = null
);
