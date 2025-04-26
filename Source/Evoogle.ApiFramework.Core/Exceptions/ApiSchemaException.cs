// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Evoogle.ApiFramework.Exceptions;

/// <summary>
///     Represents errors that occur due to API schema validation failures.
/// </summary>
public class ApiSchemaException : ApiException
{
    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaException"/> class
    ///     with a default schema error message.
    /// </summary>    
    public ApiSchemaException()
        : base("An API schema error occurred.")
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaException"/> class
    ///     with a specified schema error message.
    /// </summary>
    /// <param name="message">The message that describes the schema error.</param>
    public ApiSchemaException(string message)
        : base(message)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaException"/> class
    ///     with a specified schema error message and a reference to the inner exception
    ///     that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the schema error.</param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception, or <c>null</c>
    ///     if no inner exception is specified.
    /// </param>
    public ApiSchemaException(string message, Exception innerException)
        : base(message, innerException)
    { }
    #endregion
}
