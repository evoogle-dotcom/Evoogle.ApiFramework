// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Trace;

/// <summary>
///     Identifies the broad stage of schema construction represented by a trace event.
/// </summary>
public enum ApiSchemaBuildPhase
{
    /// <summary>Reports schema and type discovery.</summary>
    Discovery = 0,

    /// <summary>Reports annotation processing.</summary>
    Annotation = 1,

    /// <summary>Reports convention-based configuration.</summary>
    Configuration = 2,

    /// <summary>Reports relationship annotation and convention processing.</summary>
    Relationship = 3,

    /// <summary>Reports conversion of builders into schema model objects.</summary>
    Materialization = 4,

    /// <summary>Reports final schema initialization and validation.</summary>
    Initialization = 5,
}
