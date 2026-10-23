// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Schema.Query;

/// <summary>Provides throwing lookups for query source metadata.</summary>
public static class ApiQuerySourceExtensions
{
    #region Extension Methods
    /// <summary>Gets a parameter by its API name.</summary>
    /// <param name="source">The compiled source.</param>
    /// <param name="apiName">The parameter API name.</param>
    /// <returns>The resolved parameter declaration.</returns>
    /// <exception cref="ApiSchemaException">The parameter was not declared by the source.</exception>
    public static ApiQuerySourceParameter GetParameterByApiName(this ApiQuerySource source, string apiName)
    {
        if (source.TryGetParameterByApiName(apiName, out var parameter))
        {
            return parameter;
        }

        throw new ApiSchemaException
        (
            $"Parameter '{apiName}' was not found in query source '{source.ApiName}'. "
            + $"Available parameters: {string.Join(", ", source.ApiInputParameters.Select(parameter => parameter.ApiName))}."
        );
    }
    #endregion
}
