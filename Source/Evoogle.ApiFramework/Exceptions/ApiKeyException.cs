// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Exceptions;

/// <summary>
///    Represents errors that occur during API key processing.
/// </summary>
public class ApiKeyException : ApiException
{
    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiKeyException"/> class with a default API key error message.
    /// </summary>
    public ApiKeyException()
        : base("An API key error occurred.")
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiKeyException"/> class with a specified API key error message.
    /// </summary>
    /// <param name="message">The message that describes the API key error.</param>
    public ApiKeyException(string message)
        : base(message)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiKeyException"/> class with a specified API key error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the API key error.</param>
    /// <param name="innerException"> The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.
    /// </param>
    public ApiKeyException(string message, Exception innerException)
        : base(message, innerException)
    { }
    #endregion
}
