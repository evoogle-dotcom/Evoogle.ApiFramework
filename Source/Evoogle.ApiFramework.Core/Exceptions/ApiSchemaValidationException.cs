// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

namespace Evoogle.ApiFramework.Exceptions;

/// <summary>
///     Represents an error that occurs when an <see cref="Evoogle.ApiFramework.Schema.ApiSchema"/> fails validation.
/// </summary>
/// <remarks>
///     The exception exposes the set of <see cref="ValidationResults"/> that describe the validation failures.
/// </remarks>
public class ApiSchemaValidationException : ApiSchemaException
{
    #region Properties
    /// <summary>
    ///     Gets the collection of validation results that caused the validation to fail.
    /// </summary>
    public ValidationResult[] ValidationResults { get; }
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaValidationException"/> class with the specified validation results.
    /// </summary>
    /// <param name="validationResults">The set of validation results that describe the validation failures.</param>
    /// <exception cref="ArgumentNullException"><paramref name="validationResults"/> is <c>null</c>.</exception>
    public ApiSchemaValidationException(ValidationResult[] validationResults)
        : base()
    {
        ArgumentNullException.ThrowIfNull(validationResults, nameof(validationResults));

        this.ValidationResults = validationResults;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaValidationException"/> class with a specified error message and validation results.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="validationResults">The set of validation results that describe the validation failures.</param>
    /// <exception cref="ArgumentNullException"><paramref name="validationResults"/> is <c>null</c>.</exception>
    public ApiSchemaValidationException(string message, ValidationResult[] validationResults)
        : base(message)
    {
        ArgumentNullException.ThrowIfNull(validationResults, nameof(validationResults));

        this.ValidationResults = validationResults;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchemaValidationException"/> class with a specified error message, validation results, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="validationResults">The set of validation results that describe the validation failures.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <exception cref="ArgumentNullException"><paramref name="validationResults"/> is <c>null</c>.</exception>
    public ApiSchemaValidationException(string message, ValidationResult[] validationResults, Exception innerException)
        : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(validationResults, nameof(validationResults));

        this.ValidationResults = validationResults;
    }
    #endregion
}
