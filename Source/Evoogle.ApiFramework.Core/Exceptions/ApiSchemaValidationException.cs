// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

namespace Evoogle.ApiFramework.Exceptions;

/// <summary>
///     Represents errors that occur due to API schema validation failures.
/// </summary>
public class ApiSchemaValidationException : ApiSchemaException
{
    #region Properties
    public ValidationResult[] ValidationResults { get; }
    #endregion

    #region Constructors
    public ApiSchemaValidationException(ValidationResult[] validationResults)
        : base()
    {
        ArgumentNullException.ThrowIfNull(validationResults, nameof(validationResults));

        this.ValidationResults = validationResults;
    }

    public ApiSchemaValidationException(string message, ValidationResult[] validationResults)
        : base(message)
    {
        ArgumentNullException.ThrowIfNull(validationResults, nameof(validationResults));

        this.ValidationResults = validationResults;
    }

    public ApiSchemaValidationException(string message, ValidationResult[] validationResults, Exception innerException)
        : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(validationResults, nameof(validationResults));

        this.ValidationResults = validationResults;
    }
    #endregion
}
