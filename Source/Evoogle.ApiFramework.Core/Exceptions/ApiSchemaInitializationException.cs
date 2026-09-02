// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Immutable;

using Evoogle.ApiFramework.Schema;

namespace Evoogle.ApiFramework.Exceptions;

/// <summary>
///     Represents expected validation errors that occur during API schema compilation.
/// </summary>
/// <remarks>
///     This exception is thrown when an <see cref="ApiSchema"/> fails to compile successfully.
///     It contains an <see cref="ApiSchemaBuildResult"/> with detailed information about the compilation issues.
/// </remarks>
public sealed class ApiSchemaInitializationException : ApiSchemaException
{
    #region Properties
    /// <summary>
    ///     Gets the build result containing all issues, errors, and warnings.
    /// </summary>
    public ApiSchemaBuildResult Result { get; }

    /// <summary>
    ///     Gets a value indicating whether compilation was valid (no errors).
    /// </summary>
    public bool IsValid => this.Result.IsValid;

    /// <summary>
    ///     Gets all compilation issues (errors and warnings).
    /// </summary>
    public ImmutableArray<ApiInitializationIssue> Issues => this.Result.Issues;

    /// <summary>
    ///     Gets all compilation errors.
    /// </summary>
    public ImmutableArray<ApiInitializationIssue> Errors => this.Result.Errors;

    /// <summary>
    ///     Gets all compilation warnings.
    /// </summary>
    public ImmutableArray<ApiInitializationIssue> Warnings => this.Result.Warnings;

    /// <summary>
    ///     Gets the error message that describes the compilation failure.
    /// </summary>
    public override string Message => _lazyMessage.Value;

    private readonly Lazy<string> _lazyMessage;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaInitializationException"/> class with the specified build result.
    /// </summary>
    /// <param name="result">The build result containing the issues encountered during schema compilation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="result"/> is <c>null</c>.</exception>
    public ApiSchemaInitializationException(ApiSchemaBuildResult result)
        : base(default!)
    {
        ArgumentNullException.ThrowIfNull(result);
        this.Result = result;
        _lazyMessage = new Lazy<string>(this.BuildMessage);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaInitializationException"/> class with a specified error message and build result.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="result">The build result containing the issues encountered during schema compilation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="result"/> is <c>null</c>.</exception>
    public ApiSchemaInitializationException(string message, ApiSchemaBuildResult result)
        : base(message)
    {
        ArgumentNullException.ThrowIfNull(result);
        this.Result = result;
        _lazyMessage = new Lazy<string>(this.BuildMessage);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaInitializationException"/> class with a specified error message, build result, and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="result">The build result containing the issues encountered during schema compilation.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <exception cref="ArgumentNullException"><paramref name="result"/> is <c>null</c>.</exception>
    public ApiSchemaInitializationException(string message, ApiSchemaBuildResult result, Exception innerException)
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
        var issueMessages = this.Issues.Select(static issue => issue.ToMessage());

        return $"{header} {counts}{Environment.NewLine}{string.Join(Environment.NewLine, issueMessages)}";
    }
    #endregion
}
