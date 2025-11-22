// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extension;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a single enumeration value within an API enumeration type.
/// </summary>
/// <param name="apiName">The API name of the enumeration value (typically used in API requests/responses).</param>
/// <param name="clrName">The CLR name of the enumeration value (corresponding to the C# enum name).</param>
/// <param name="clrOrdinal">The CLR ordinal (integer value) of the enumeration value.</param>
[JsonConverter(typeof(ApiEnumValueJsonConverter))]
public sealed class ApiEnumValue(string apiName, string clrName, int clrOrdinal) : ExtensibleBase
{
    #region Properties
    /// <summary>Gets the API name of the API enumeration value.</summary>
    public string ApiName { get; } = apiName;

    /// <summary>Gets the CLR name of the API enumeration value (matching the C# enum name).</summary>
    public string ClrName { get; } = clrName;

    /// <summary>Gets the CLR ordinal of the API enumeration value (matching the C# enum ordinal value).</summary>
    public int ClrOrdinal { get; } = clrOrdinal;
    #endregion

    #region ApiEnumValue Methods
    internal string GetValidationPath(string parentPath) => $"{parentPath.SafeToString()}.{nameof(ApiEnumValue)}[\"{this.ApiName.SafeToString()}\"]";

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

        this.InitializeApiName(apiValidationPath, ref results);
        this.InitializeClrName(apiValidationPath, ref results);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrName = this.ClrName.SafeToString();
        var clrOrdinal = this.ClrOrdinal.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiEnumValue)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ClrName)}={clrName}, {nameof(this.ClrOrdinal)}={clrOrdinal}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiName(string apiValidationPath, ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiName)} cannot be null or whitespace.", [nameof(this.ApiName)]));
        }
    }

    private void InitializeClrName(string apiValidationPath, ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ClrName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} cannot be null or whitespace.", [nameof(this.ClrName)]));
        }
    }
    #endregion
}
