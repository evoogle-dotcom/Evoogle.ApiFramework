// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
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
    ///     Initializes a new instance of the <see cref="ApiException"/> class
    ///     with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ApiException(string message)
        : base(message)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiException"/> class
    ///     with a specified error message and a reference to the inner exception
    ///     that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception, or <c>null</c>
    ///     if no inner exception is specified.
    /// </param>
    public ApiException(string message, Exception innerException)
        : base(message, innerException)
    { }
    #endregion
}
