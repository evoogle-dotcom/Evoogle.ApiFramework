// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using Evoogle.Extensions;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Partial class containing initialization methods for ApiProperty setup and validation.
/// </summary>
public sealed partial class ApiProperty
{
    #region Initialization Methods
    /// <summary>
    ///     Gets the validation path for this property, used for error reporting during validation.
    /// </summary>
    /// <param name="parentPath">The parent validation path.</param>
    /// <returns>A formatted validation path string.</returns>
    internal string GetValidationPath(string parentPath) => $"{parentPath.SafeToString()}.{nameof(ApiProperty)}[\"{this.ApiName.SafeToString()}\"]";

    /// <summary>
    ///     Initializes this property within the context of an API schema.
    /// </summary>
    /// <param name="apiSchema">The parent API schema.</param>
    /// <param name="apiSchemaContext">The schema context containing shared resources.</param>
    /// <param name="apiObjectType">The object type that owns this property.</param>
    /// <param name="apiValidationPath">The validation path for error reporting.</param>
    /// <param name="results">Collection to accumulate validation errors.</param>
    internal void Initialize(ApiSchema apiSchema, ApiSchemaContext apiSchemaContext, ApiObjectType apiObjectType, string apiValidationPath, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(apiSchemaContext);
        ArgumentNullException.ThrowIfNull(apiObjectType);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        _apiSchemaContext = apiSchemaContext;

        this.InitializeApiName(apiValidationPath, ref results);
        this.InitializeApiTypeExpression(apiSchema, apiValidationPath, ref results);
        this.InitializeClrName(apiValidationPath, ref results);
        this.InitializeClrGetterAndSetter(apiObjectType, apiValidationPath, ref results);
    }

    /// <summary>
    ///     Validates that the API name is not null or whitespace.
    /// </summary>
    /// <param name="apiValidationPath">The validation path for error reporting.</param>
    /// <param name="results">Collection to accumulate validation errors.</param>
    private void InitializeApiName(string apiValidationPath, ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiName)} cannot be null or whitespace.", [nameof(this.ApiName)]));
        }
    }

    /// <summary>
    ///     Validates and initializes the API type expression for this property.
    /// </summary>
    /// <param name="apiSchema">The parent API schema.</param>
    /// <param name="apiParentValidationPath">The parent validation path for error reporting.</param>
    /// <param name="results">Collection to accumulate validation errors.</param>
    private void InitializeApiTypeExpression(ApiSchema apiSchema, string apiParentValidationPath, ref List<ValidationResult>? results)
    {
        if (this.ApiTypeExpression is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiParentValidationPath}.{nameof(this.ApiTypeExpression)} cannot be null.", [nameof(this.ApiTypeExpression)]));
            return;
        }

        var apiChildValidationPath = $"{apiParentValidationPath}.{nameof(this.ApiTypeExpression)}";
        this.ApiTypeExpression.Initialize(apiSchema, apiChildValidationPath, ref results);
    }

    /// <summary>
    ///     Initializes and compiles the CLR getter and setter delegates for this property.
    ///     Attempts to resolve the property or field on the CLR type and builds optimized lambda expressions.
    /// </summary>
    /// <param name="apiObjectType">The object type that owns this property.</param>
    /// <param name="apiValidationPath">The validation path for error reporting.</param>
    /// <param name="results">Collection to accumulate validation errors.</param>
    private void InitializeClrGetterAndSetter(ApiObjectType apiObjectType, string apiValidationPath, ref List<ValidationResult>? results)
    {
        var clrObjectType = apiObjectType.ClrType;
        var clrMemberName = this.ClrName;

        try
        {
            // Prefer property, then field
            var clrPropertyInfo = TypeReflection.GetProperty(clrObjectType, clrMemberName, BindingFlags.Public | BindingFlags.Instance);
            if (clrPropertyInfo is not null)
            {
                // Exclude indexers
                if (clrPropertyInfo.GetIndexParameters().Length > 0)
                {
                    results ??= [];
                    results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} refers to an indexer property, which is not supported.", [nameof(this.ClrName)]));
                    return;
                }

                // Build compiled property getter and setter
                try
                {
                    _clrGetter = BuildNonGenericClrPropertyGetter(clrObjectType, clrPropertyInfo);
                }
                catch (Exception ex)
                {
                    results ??= [];
                    results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} failed to compile lambda property getter: {ex.Message}", [nameof(this.ClrName)]));
                }

                try
                {
                    _clrSetter = BuildNonGenericClrPropertySetter(clrObjectType, clrPropertyInfo);
                }
                catch (Exception ex)
                {
                    results ??= [];
                    results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} failed to compile lambda property setter: {ex.Message}", [nameof(this.ClrName)]));
                }

                return; // Found valid property
            }

            var clrFieldInfo = TypeReflection.GetField(clrObjectType, clrMemberName, BindingFlags.Public | BindingFlags.Instance);
            if (clrFieldInfo is not null)
            {
                // Build compiled field getter and setter
                try
                {
                    _clrGetter = BuildNonGenericClrFieldGetter(clrObjectType, clrFieldInfo);
                }
                catch (Exception ex)
                {
                    results ??= [];
                    results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} failed to compile lambda field getter: {ex.Message}", [nameof(this.ClrName)]));
                }

                try
                {
                    _clrSetter = BuildNonGenericClrFieldSetter(clrObjectType, clrFieldInfo);
                }
                catch (Exception ex)
                {
                    results ??= [];
                    results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} failed to compile lambda field setter: {ex.Message}", [nameof(this.ClrName)]));
                }

                return; // Found valid field
            }

            // Member not found
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} '{clrMemberName}' could not be found on {nameof(ApiObjectType.ClrType)} '{clrObjectType.SafeToName()}'.", [nameof(this.ClrName)]));
        }
        catch (Exception ex)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ClrName)} failed to compile lambda getter or setter accessor: {ex.Message}", [nameof(this.ClrName)]));
        }
    }

    /// <summary>
    ///     Validates that the CLR name is not null or whitespace.
    /// </summary>
    /// <param name="apiValidationPath">The validation path for error reporting.</param>
    /// <param name="results">Collection to accumulate validation errors.</param>
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
