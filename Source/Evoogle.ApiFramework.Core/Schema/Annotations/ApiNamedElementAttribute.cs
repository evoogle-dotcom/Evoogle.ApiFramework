// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Annotations;

/// <summary>
///     Provides the common API name for annotations that identify named API schema elements.
/// </summary>
public abstract class ApiNamedElementAttribute : Attribute
{
    #region Properties
    /// <summary>
    ///     Gets the the API name for the annotated schema element.
    ///     When <c>null</c>, the applicable naming convention supplies the name.
    /// </summary>
    public string? ApiName { get; protected init; }
    #endregion

    #region Validation Methods
    /// <summary>
    ///     Validates and returns an API name required by an annotation constructor.
    /// </summary>
    /// <param name="apiName">The required API name.</param>
    /// <returns>The validated API name.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="apiName"/> is <see langword="null"/>, empty, or whitespace.
    /// </exception>
    protected static string RequireApiName(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));
        return apiName;
    }
    #endregion
}
