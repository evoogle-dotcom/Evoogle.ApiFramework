// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Marker interface for all API schema conventions.
/// </summary>
/// <remarks>
///     Concrete conventions implement one or more of the specific targeted convention interfaces
///     such as <see cref="IApiObjectTypeConvention"/> or <see cref="IApiPropertyConvention"/>.
///     The target interface identifies what the convention configures, while
///     <see cref="Phase"/> identifies when it runs.
/// </remarks>
public interface IApiConvention
{
    /// <summary>
    ///     Gets the phase in which this convention participates.
    /// </summary>
    ApiConventionPhase Phase { get; }
}
