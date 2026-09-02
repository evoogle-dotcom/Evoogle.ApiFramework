// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Frozen;
using System.Collections.Immutable;
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
    private FrozenDictionary<string, ApiEnumValue>? _apiNameLookup = null;
    private FrozenDictionary<string, ApiEnumValue>? _clrNameLookup = null;
    private FrozenDictionary<int, ApiEnumValue>? _clrOrdinalLookup = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiEnumType);
    #endregion

    #region ApiType Properties
    /// <inheritdoc/>
    public override ApiTypeKind ApiKind => ApiTypeKind.Enum;
    #endregion

    #region ApiEnumType Properties
    /// <summary>Gets the immutable snapshot of values defined for this API enum type.</summary>
    public ImmutableArray<ApiEnumValue> ApiEnumValues { get; } =
        [.. apiEnumValues.EmptyIfNull().Where(x => x is not null).OrderBy(x => x.ClrOrdinal)];

    private FrozenDictionary<string, ApiEnumValue> ApiNameLookup => this.ThrowIfNotInitialized(_apiNameLookup);
    private FrozenDictionary<string, ApiEnumValue> ClrNameLookup => this.ThrowIfNotInitialized(_clrNameLookup);
    private FrozenDictionary<int, ApiEnumValue> ClrOrdinalLookup => this.ThrowIfNotInitialized(_clrOrdinalLookup);
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

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override IEnumerable<ApiSchemaElement> GetOwnedElements()
    {
        foreach (var apiEnumValue in this.ApiEnumValues)
        {
            yield return apiEnumValue;
        }
    }

    /// <inheritdoc />
    internal override void InitializeCore(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.InitializeCore(context);

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

    #region Implementation Methods
    private void InitializeApiEnumValues(ApiInitializationContext context)
    {
        if (this.ApiEnumValues.Length == 0)
        {
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.ApiEnumTypeNullOrEmptyValues;
            var description = $"{nameof(this.ApiEnumValues)} must not be null or empty";
            var remediation = $"Define at least one {nameof(ApiEnumValue)}";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        var apiEnumValuesCount = this.ApiEnumValues.Length;
        for (var i = 0; i < apiEnumValuesCount; ++i)
        {
            var apiEnumValue = this.ApiEnumValues[i];

            apiEnumValue.Initialize(context);
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
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.ApiEnumTypeInvalidClrType;
            var description = $"{nameof(this.ClrType)} '{this.ClrType.SafeToName()}' must be a CLR Enum";
            var remediation = $"Set {nameof(this.ClrType)} to a CLR Enum type";

            context.AddIssue(severity, code, description, remediation);
        }
    }

    private void InitializeLookupDictionaries(ApiInitializationContext context)
    {
        // Initialize lookup dictionaries for lookup by API name, CLR name, and CLR ordinal.
        ApiSchemaInitializationLookup.InitializeLookupDictionary
        (
            parts: this.ApiEnumValues,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaNameValidation.IsNameValid(x),
            partKeyPropertyName: nameof(ApiEnumValue.ApiName),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.ApiEnumTypeDuplicateValueApiName,
            session: context.Session,
            lookupDictionary: out _apiNameLookup
        );

        ApiSchemaInitializationLookup.InitializeLookupDictionary
        (
            parts: this.ApiEnumValues,
            partKeySelector: x => x.ClrName,
            partKeyFilter: x => ApiSchemaNameValidation.IsNameValid(x),
            partKeyPropertyName: nameof(ApiEnumValue.ClrName),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.ApiEnumTypeDuplicateValueClrName,
            session: context.Session,
            lookupDictionary: out _clrNameLookup
        );

        ApiSchemaInitializationLookup.InitializeLookupDictionary
        (
            parts: this.ApiEnumValues,
            partKeySelector: x => x.ClrOrdinal,
            partKeyFilter: null,
            partKeyPropertyName: nameof(ApiEnumValue.ClrOrdinal),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.ApiEnumTypeDuplicateValueClrOrdinal,
            session: context.Session,
            lookupDictionary: out _clrOrdinalLookup
        );
    }
    #endregion
}
