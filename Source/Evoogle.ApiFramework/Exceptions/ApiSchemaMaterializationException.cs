// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema;

namespace Evoogle.ApiFramework.Exceptions;

/// <summary>
///     Represents errors that occur while materializing schema-defined artifacts from an <see cref="ApiSchema"/>.
/// </summary>
public class ApiSchemaMaterializationException : ApiSchemaException
{
    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaMaterializationException"/> class
    ///     with a default API schema materialization error message.
    /// </summary>
    public ApiSchemaMaterializationException()
        : base($"An {nameof(ApiSchema)} materialization error occurred.")
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaMaterializationException"/> class
    ///     with a specified API schema materialization error message.
    /// </summary>
    /// <param name="message">
    ///     The message that describes the API schema materialization error.
    /// </param>
    public ApiSchemaMaterializationException(string message)
        : base(message)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaMaterializationException"/> class
    ///     with a specified API schema materialization error message and a reference to the inner
    ///     exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">
    ///     The message that describes the API schema materialization error.
    /// </param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception, or <c>null</c> if no inner
    ///     exception is specified.
    /// </param>
    public ApiSchemaMaterializationException(string message, Exception innerException)
        : base(message, innerException)
    { }
    #endregion
}
