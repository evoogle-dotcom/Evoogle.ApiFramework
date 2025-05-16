// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Exceptions;

/// <summary>
///     Represents errors that occur during API operations.
/// </summary>
public class ApiException : Exception
{
    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiException"/> class with a default error message.
    /// </summary>    
    public ApiException()
        : base("An error occurred during an API operation.")
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ApiException(string message)
        : base(message)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.
    /// </param>
    public ApiException(string message, Exception innerException)
        : base(message, innerException)
    { }
    #endregion
}
