// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Exceptions;

/// <summary>
///     Represents errors that occur during API version processing.
/// </summary>
public class ApiVersionException : ApiException
{
    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiVersionException"/> class.
    /// </summary>
    public ApiVersionException()
        : base("An API version error occurred.")
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiVersionException"/> class with the
    ///     specified message.
    /// </summary>
    /// <param name="message">The message that describes the API version error.</param>
    public ApiVersionException(string message)
        : base(message)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiVersionException"/> class with the
    ///     specified message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the API version error.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public ApiVersionException(string message, Exception innerException)
        : base(message, innerException)
    { }
    #endregion
}
