// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

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
public sealed class ApiEnumType
(
    string apiName,
    IEnumerable<ApiEnumValue> apiEnumValues,
    Type clrEnumType
) : ApiNamedType(apiName, clrEnumType)
{
    #region ApiEnumType Fields
    private Dictionary<string, ApiEnumValue>? _apiNameLookup = null;
    private Dictionary<string, ApiEnumValue>? _clrNameLookup = null;
    private Dictionary<int, ApiEnumValue>? _clrOrdinalLookup = null;
    #endregion

    #region ApiType Properties
    /// <inheritdoc/>
    public override ApiTypeKind ApiKind => ApiTypeKind.Enum;

    /// <inheritdoc/>
    protected override string ApiTypeName => nameof(ApiEnumType);
    #endregion

    #region ApiEnumType Properties
    /// <summary>Gets the collection of enumeration values defined for this API enum type.</summary>
    public ApiEnumValue[] ApiEnumValues { get; } = [.. apiEnumValues.EmptyIfNull().Where(x => x is not null).OrderBy(x => x.ClrOrdinal)];

    private Dictionary<string, ApiEnumValue> ApiNameLookup => this.ThrowIfNotInitialized(_apiNameLookup);
    private Dictionary<string, ApiEnumValue> ClrNameLookup => this.ThrowIfNotInitialized(_clrNameLookup);
    private Dictionary<int, ApiEnumValue> ClrOrdinalLookup => this.ThrowIfNotInitialized(_clrOrdinalLookup);
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeLookupDictionaries(context);

        this.InitializeClrType(context);
        this.InitializeApiEnumValues(context);
    }
    #endregion

    #region ApiEnumType Methods
    /// <summary>
    ///     Attempts to retrieve an API enum value by its API name.
    /// </summary>
    /// <param name="apiName">The API name of the enumeration value to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiEnumValue"/> if found; otherwise, null.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool TryGetValueByApiName(string apiName, [NotNullWhen(true)] out ApiEnumValue? value) => this.ApiNameLookup.TryGetValue(apiName, out value);

    /// <summary>
    ///     Attempts to retrieve an API enum value by its CLR name.
    /// </summary>
    /// <param name="clrName">The CLR name of the enumeration value to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiEnumValue"/> if found; otherwise, null.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool TryGetValueByClrName(string clrName, [NotNullWhen(true)] out ApiEnumValue? value) => this.ClrNameLookup.TryGetValue(clrName, out value);

    /// <summary>
    ///     Attempts to retrieve an API enum value by its CLR ordinal value.
    /// </summary>
    /// <param name="clrOrdinal">The CLR ordinal of the enumeration value to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiEnumValue"/> if found; otherwise, null.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool TryGetValueByClrOrdinal(int clrOrdinal, [NotNullWhen(true)] out ApiEnumValue? value) => this.ClrOrdinalLookup.TryGetValue(clrOrdinal, out value);
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
    private void InitializeApiEnumValues(ApiInitializationContext context)
    {
        if (this.ApiEnumValues is null || this.ApiEnumValues.Length == 0)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_ENUM_TYPE_NULL_OR_EMPTY_VALUES;
            var description = $"{nameof(this.ApiEnumValues)} must not be null or empty";
            var remediation = $"Define at least one {nameof(ApiEnumValue)}";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        var apiEnumValuesCount = this.ApiEnumValues.Length;
        for (var i = 0; i < apiEnumValuesCount; ++i)
        {
            var apiEnumValue = this.ApiEnumValues[i];

            var childContext = context.WithParentSchemaElement(this);
            apiEnumValue.Initialize(childContext);
        }
    }

    private void InitializeClrType(ApiInitializationContext context)
    {
        // If ClrType is null, the base ApiNamedType.Initialize will have already reported the issue.
        if (this.ClrType is null)
        {
            return;
        }

        if (!TypeReflection.IsEnum(this.ClrType))
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_ENUM_TYPE_INVALID_CLR_TYPE;
            var description = $"{nameof(this.ClrType)} '{this.ClrType.SafeToName()}' must be a CLR Enum";
            var remediation = $"Set {nameof(this.ClrType)} to a CLR Enum type";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeLookupDictionaries(ApiInitializationContext context)
    {
        // Initialize lookup dictionaries for lookup by API name, CLR name, and CLR ordinal.
        _apiNameLookup = null;
        _clrNameLookup = null;
        _clrOrdinalLookup = null;

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiEnumValues,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaHelpers.IsNameValid(x),
            partKeyPropertyName: nameof(ApiEnumValue.ApiName),
            path: this.ApiPath,
            code: ApiInitializationCode.API_ENUM_TYPE_DUPLICATE_VALUE_API_NAME,
            context: context,
            lookupDictionary: out _apiNameLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiEnumValues,
            partKeySelector: x => x.ClrName,
            partKeyFilter: x => ApiSchemaHelpers.IsNameValid(x),
            partKeyPropertyName: nameof(ApiEnumValue.ClrName),
            path: this.ApiPath,
            code: ApiInitializationCode.API_ENUM_TYPE_DUPLICATE_VALUE_CLR_NAME,
            context: context,
            lookupDictionary: out _clrNameLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiEnumValues,
            partKeySelector: x => x.ClrOrdinal,
            partKeyFilter: null,
            partKeyPropertyName: nameof(ApiEnumValue.ClrOrdinal),
            path: this.ApiPath,
            code: ApiInitializationCode.API_ENUM_TYPE_DUPLICATE_VALUE_CLR_ORDINAL,
            context: context,
            lookupDictionary: out _clrOrdinalLookup
        );
    }
    #endregion
}
