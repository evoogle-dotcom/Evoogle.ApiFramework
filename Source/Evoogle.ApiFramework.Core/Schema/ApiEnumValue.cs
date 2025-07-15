// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a single enumeration value within an API enumeration type.
/// </summary>
/// <param name="ApiName">The API name of the enumeration value (typically used in API requests/responses).</param>
/// <param name="ClrName">The CLR name of the enumeration value (corresponding to the C# enum name).</param>
/// <param name="ClrOrdinal">The CLR ordinal (integer value) of the enumeration value.</param>
public sealed record ApiEnumValue(string ApiName, string ClrName, int ClrOrdinal)
{
    #region ApiEnumValue Methods
    /// <summary>
    ///     Validates the <see cref="ApiEnumValue"/> by checking that required fields are not null or empty.
    /// </summary>
    /// <param name="apiSchema">The current API schema.</param>
    /// <param name="apiValidationPath">The string path used to report validation context.</param>
    /// <param name="results">Validation results list to append to if validation fails.</param>
    internal void Initialize(ApiSchema apiSchema, string apiValidationPath, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        this.InitializeApiName(apiSchema, apiValidationPath, ref results);
        this.InitializeClrName(apiSchema, apiValidationPath, ref results);
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiName(ApiSchema _, string apiValidationPath, ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiName)} cannot be null or whitespace.", [nameof(this.ApiName)]));
        }
    }

    private void InitializeClrName(ApiSchema _, string apiValidationPath, ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ClrName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} cannot be null or whitespace.", [nameof(this.ClrName)]));
        }
    }
    #endregion
}
