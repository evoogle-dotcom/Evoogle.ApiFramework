// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Exceptions;

/// <summary>
///    Represents errors that occur during API identity processing.
/// </summary>
public class ApiIdentityException : ApiException
{
    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiIdentityException"/> class with a default identity error message.
    /// </summary>    
    public ApiIdentityException()
        : base("An API identity error occurred.")
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiIdentityException"/> class with a specified identity error message.
    /// </summary>
    /// <param name="message">The message that describes the identity error.</param>
    public ApiIdentityException(string message)
        : base(message)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiIdentityException"/> class with a specified identity error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the identity error.</param>
    /// <param name="innerException"> The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.
    /// </param>
    public ApiIdentityException(string message, Exception innerException)
        : base(message, innerException)
    { }
    #endregion
}
