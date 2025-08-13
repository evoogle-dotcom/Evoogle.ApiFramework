// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the result of initializing an <see cref="ApiSchema"/>.
///     Includes validation status and potential validation errors.
/// </summary>
/// <param name="validationResults">A collection of validation results (can be null).</param>
public sealed class ApiSchemaInitializeResult(IEnumerable<ValidationResult>? validationResults)
{
    #region Properties
    /// <summary>Gets a value indicating whether the schema initialization was successful (i.e., no validation errors).</summary>
    public bool Success => this.ValidationResults is null || this.ValidationResults.Length == 0;

    /// <summary>Gets the array of validation results, or null if none exist.</summary>
    public ValidationResult[]? ValidationResults { get; } = validationResults is not null
        ? [.. validationResults]
        : null;
    #endregion

    #region Methods
    /// <summary>
    ///     Throws an <see cref="ApiSchemaException"/> if the schema is not valid.
    /// </summary>
    /// <exception cref="ApiSchemaException">Thrown if validation errors exist.</exception>
    public void ThrowIfInvalid()
    {
        if (this.Success)
            return;

        throw new ApiSchemaValidationException($"{nameof(ApiSchema)} initialization failed.", this.ValidationResults!);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var success = this.Success.SafeToString();
        var errorsCount = this.ValidationResults?.Length.SafeToString();

        return this.Success
            ? $"{nameof(ApiSchemaInitializeResult)} {{{nameof(this.Success)}={success}}}"
            : $"{nameof(ApiSchemaInitializeResult)} {{{nameof(this.Success)}={success}, ErrorsCount={errorsCount}}}";
    }
    #endregion
}
