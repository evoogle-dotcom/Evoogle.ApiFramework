// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API enumeration type, including its possible values.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiEnumType"/> class.
/// </remarks>
/// <param name="apiName">The API name of the enumeration type.</param>
/// <param name="apiEnumValues">The collection of enumeration values associated with this type.</param>
/// <param name="clrEnumType">The CLR type that defines the enumeration.</param>
public sealed class ApiEnumType(string apiName, IEnumerable<ApiEnumValue> apiEnumValues, Type clrEnumType) : ApiNamedType(apiName, clrEnumType)
{
    #region ApiEnumType Fields
    private ApiSchemaContext? _apiSchemaContext = null;

    private Dictionary<string, ApiEnumValue>? _apiNameLookup = null;
    private Dictionary<string, ApiEnumValue>? _clrNameLookup = null;
    private Dictionary<int, ApiEnumValue>? _clrOrdinalLookup = null;
    #endregion

    #region ApiType Properties
    /// <inheritdoc/>
    public override ApiTypeKind Kind => ApiTypeKind.Enum;

    /// <inheritdoc/>
    protected override string ApiTypeName => nameof(ApiEnumType);
    #endregion

    #region ApiEnumType Properties
    /// <summary>Gets the collection of enumeration values defined for this API enum type.</summary>
    public ApiEnumValue[] ApiEnumValues { get; } = apiEnumValues.SafeToArray();

    /// <summary>Gets the schema context for this enum type.</summary>
    internal ApiSchemaContext ApiSchemaContext => this.ThrowIfNotInitialized(_apiSchemaContext);

    private Dictionary<string, ApiEnumValue> ApiNameLookup => this.ThrowIfNotInitialized(_apiNameLookup);
    private Dictionary<string, ApiEnumValue> ClrNameLookup => this.ThrowIfNotInitialized(_clrNameLookup);
    private Dictionary<int, ApiEnumValue> ClrOrdinalLookup => this.ThrowIfNotInitialized(_clrOrdinalLookup);
    #endregion

    #region ApiType Methods
    internal override void Initialize(ApiSchema apiSchema, ApiSchemaContext apiSchemaContext, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(apiSchemaContext);

        _apiSchemaContext = apiSchemaContext;

        base.Initialize(apiSchema, apiSchemaContext, ref results);

        this.InitializeLookupDictionaries(ref results);

        this.InitializeClrType(ref results);
        this.InitializeApiEnumValues(apiSchema, apiSchemaContext, ref results);
    }
    #endregion

    #region ApiEnumType Methods
    /// <summary>
    ///     Attempts to retrieve an API enum value by its API name.
    /// </summary>
    /// <param name="apiName">The API name of the enumeration value to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiEnumValue"/> if found; otherwise, null.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool TryGetValueByApiName(string apiName, out ApiEnumValue? value) => this.ApiNameLookup.TryGetValue(apiName, out value);

    /// <summary>
    ///     Attempts to retrieve an API enum value by its CLR name.
    /// </summary>
    /// <param name="clrName">The CLR name of the enumeration value to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiEnumValue"/> if found; otherwise, null.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool TryGetValueByClrName(string clrName, out ApiEnumValue? value) => this.ClrNameLookup.TryGetValue(clrName, out value);

    /// <summary>
    ///     Attempts to retrieve an API enum value by its CLR ordinal value.
    /// </summary>
    /// <param name="clrOrdinal">The CLR ordinal of the enumeration value to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiEnumValue"/> if found; otherwise, null.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool TryGetValueByClrOrdinal(int clrOrdinal, out ApiEnumValue? value) => this.ClrOrdinalLookup.TryGetValue(clrOrdinal, out value);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrType = this.ClrType.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiEnumType)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ExtensionCount)}={extensionCount}}} [{clrType}]";
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiEnumValues(ApiSchema apiSchema, ApiSchemaContext apiSchemaContext, ref List<ValidationResult>? results)
    {
        var apiValidationPath = this.GetValidationPath();

        if (this.ApiEnumValues is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiEnumValues)} cannot be null.", [nameof(this.ApiEnumValues)]));
            return;
        }

        var apiEnumValuesCount = this.ApiEnumValues.Length;
        for (var i = 0; i < apiEnumValuesCount; ++i)
        {
            var apiEnumValue = this.ApiEnumValues[i];
            if (apiEnumValue is null)
            {
                results ??= [];
                results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiEnumValues)}[{i}] cannot be null.", [nameof(this.ApiEnumValues)]));
                continue;
            }

            var apiEnumValueValidationPath = apiEnumValue.GetValidationPath(parentPath: apiValidationPath);
            apiEnumValue.Initialize
            (
                apiSchema,
                apiSchemaContext,
                apiEnumValueValidationPath,
                ref results
            );
        }
    }

    private void InitializeClrType(ref List<ValidationResult>? results)
    {
        if (this.ClrType is null)
        {
            return;
        }

        if (!TypeReflection.IsEnum(this.ClrType))
        {
            results ??= [];
            results.Add(new ValidationResult($"{this.GetValidationPath()}.{nameof(this.ClrType)} must be a CLR enum type.", [nameof(this.ClrType)]));
        }
    }

    private void InitializeLookupDictionaries(ref List<ValidationResult>? results)
    {
        _apiNameLookup = null;
        _clrNameLookup = null;
        _clrOrdinalLookup = null;

        var anyApiNameDuplicates = ApiSchemaHelpers.ValidateUnique(this.ApiEnumValues, x => x.ApiName, this.GetValidationPath(), nameof(ApiEnumValue.ApiName), ref results);
        var anyClrNameDuplicates = ApiSchemaHelpers.ValidateUnique(this.ApiEnumValues, x => x.ClrName, this.GetValidationPath(), nameof(ApiEnumValue.ClrName), ref results);
        var anyClrOrdinalDuplicates = ApiSchemaHelpers.ValidateUnique(this.ApiEnumValues, x => x.ClrOrdinal, this.GetValidationPath(), nameof(ApiEnumValue.ClrOrdinal), ref results);

        if (!anyApiNameDuplicates)
        {
            _apiNameLookup = this.ApiEnumValues.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        }

        if (!anyClrNameDuplicates)
        {
            _clrNameLookup = this.ApiEnumValues.ToDictionary(x => x.ClrName, StringComparer.OrdinalIgnoreCase);
        }

        if (!anyClrOrdinalDuplicates)
        {
            _clrOrdinalLookup = this.ApiEnumValues.ToDictionary(x => x.ClrOrdinal);
        }
    }
    #endregion
}
