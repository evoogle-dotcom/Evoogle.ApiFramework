// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the base class for API types that are identified by a unique API name.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiNamedType"/> class.
/// </remarks>
/// <param name="apiName">The unique API name of the type.</param>
/// <param name="clrType">The underlying CLR type this API type maps to.</param>
public abstract class ApiNamedType(string apiName, Type clrType) : ApiType()
{
    #region ApiType Properties
    /// <inheritdoc />
    public override Type ClrType { get; } = clrType;

    /// <inheritdoc />
    protected override string ValidationPath => $"{this.ApiTypeName}[\"{this.ClrType.SafeToString()}\"][\"{this.ApiName.SafeToString()}\"]";
    #endregion

    #region ApiNamedType Properties
    /// <summary>Gets the API name of the API type.</summary>
    public string ApiName { get; } = apiName;
    #endregion

    #region ApiType Methods
    /// <summary>
    ///     Performs validation and initialization of the <see cref="ApiNamedType"/> within the specified <see cref="ApiSchema"/>.
    /// </summary>
    /// <param name="apiSchema">The API schema the type is part of.</param>
    /// <param name="results">Optional validation results to populate with any issues.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="apiSchema"/> is null.</exception>
    internal override void Initialize(ApiSchema apiSchema, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);

        this.InitializeApiName(apiSchema, ref results);
        this.InitializeClrType(apiSchema, ref results);
    }
    #endregion

    #region Implementation Methods
    /// <summary>
    /// Validates that the <see cref="ApiName"/> is not null or whitespace.
    /// </summary>
    /// <param name="_">The current <see cref="ApiSchema"/> instance (unused).</param>
    /// <param name="results">Validation results collection to populate.</param>
    private void InitializeApiName(ApiSchema _, ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{this.ValidationPath}.{nameof(this.ApiName)} cannot be null or whitespace.", [nameof(this.ApiName)]));
        }
    }

    /// <summary>
    /// Validates that the <see cref="ClrType"/> is not null.
    /// </summary>
    /// <param name="_">The current <see cref="ApiSchema"/> instance (unused).</param>
    /// <param name="results">Validation results collection to populate.</param>
    private void InitializeClrType(ApiSchema _, ref List<ValidationResult>? results)
    {
        if (this.ClrType is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{this.ValidationPath}.{nameof(this.ClrType)} cannot be null.", [nameof(this.ClrType)]));
        }
    }
    #endregion
}
