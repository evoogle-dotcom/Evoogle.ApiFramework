// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema;

namespace Evoogle.ApiFramework.Exceptions;

/// <summary>
///     Represents errors that occur during API schema configuration.
/// </summary>
public class ApiSchemaConfigurationException : ApiSchemaException
{
    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaConfigurationException"/> class with a default schema configuration error message.
    /// </summary>
    public ApiSchemaConfigurationException()
        : base($"An {nameof(ApiSchema)} configuration error occurred.")
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaConfigurationException"/> class with a specified schema configuration error message.
    /// </summary>
    /// <param name="message">The message that describes the schema configuration error.</param>
    public ApiSchemaConfigurationException(string message)
        : base(message)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaConfigurationException"/> class with a specified schema configuration error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the schema configuration error.</param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.
    /// </param>
    public ApiSchemaConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    { }
    #endregion
}
