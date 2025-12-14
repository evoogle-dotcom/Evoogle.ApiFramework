// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema;

namespace Evoogle.ApiFramework.Exceptions;

/// <summary>
///     Represents errors that occur during API schema initialization.
/// </summary>
/// <remarks>
///     This exception is thrown when an <see cref="ApiSchema"/> fails to initialize properly.
///     It contains an <see cref="ApiInitializationResult"/> with detailed information about the initialization issues.
/// </remarks>
public sealed class ApiSchemaInitializationException : ApiSchemaException
{
    #region Properties
    /// <summary>
    ///     Gets the initialization result containing all issues, errors, and warnings.
    /// </summary>
    public ApiInitializationResult Result { get; }

    /// <summary>
    ///     Gets a value indicating whether the initialization was valid (no errors).
    /// </summary>
    public bool IsValid => this.Result.IsValid;

    /// <summary>
    ///     Gets all initialization issues (errors and warnings).
    /// </summary>
    public ApiInitializationIssue[] Issues => this.Result.Issues ?? [];

    /// <summary>
    ///     Gets all initialization errors.
    /// </summary>
    public ApiInitializationIssue[] Errors => this.Result.Errors ?? [];

    /// <summary>
    ///     Gets all initialization warnings.
    /// </summary>
    public ApiInitializationIssue[] Warnings => this.Result.Warnings ?? [];

    /// <summary>
    ///     Gets the error message that describes the initialization failure.
    /// </summary>
    public override string Message => _lazyMessage.Value;

    private readonly Lazy<string> _lazyMessage;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaInitializationException"/> class with the specified initialization result.
    /// </summary>
    /// <param name="result">The initialization result containing the issues encountered during schema initialization.</param>
    /// <exception cref="ArgumentNullException"><paramref name="result"/> is <c>null</c>.</exception>
    public ApiSchemaInitializationException(ApiInitializationResult result)
        : base(default!)
    {
        ArgumentNullException.ThrowIfNull(result);
        this.Result = result;
        _lazyMessage = new Lazy<string>(this.BuildMessage);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaInitializationException"/> class with a specified error message and initialization result.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="result">The initialization result containing the issues encountered during schema initialization.</param>
    /// <exception cref="ArgumentNullException"><paramref name="result"/> is <c>null</c>.</exception>
    public ApiSchemaInitializationException(string message, ApiInitializationResult result)
        : base(message)
    {
        ArgumentNullException.ThrowIfNull(result);
        this.Result = result;
        _lazyMessage = new Lazy<string>(this.BuildMessage);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaInitializationException"/> class with a specified error message, initialization result, and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="result">The initialization result containing the issues encountered during schema initialization.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <exception cref="ArgumentNullException"><paramref name="result"/> is <c>null</c>.</exception>
    public ApiSchemaInitializationException(string message, ApiInitializationResult result, Exception innerException)
        : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(result);
        this.Result = result;
        _lazyMessage = new Lazy<string>(this.BuildMessage);
    }
    #endregion

    #region Methods
    private string BuildMessage()
    {
        if (this.IsValid)
        {
            return $"{nameof(ApiSchema)} initialization succeeded.";
        }

        var errors = this.Errors.Length;
        var warnings = this.Warnings.Length;
        var total = this.Issues.Length;
        var header = $"{nameof(ApiSchema)} initialization failed.";
        var counts = $"Issues={total}, Errors={errors}, Warnings={warnings}.";
        return $"{header} {counts}";
    }
    #endregion
}
