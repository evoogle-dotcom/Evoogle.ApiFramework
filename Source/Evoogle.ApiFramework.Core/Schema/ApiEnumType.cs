// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API enumeration type, including its possible values.
/// </summary>
public sealed class ApiEnumType : ApiNamedType
{
    #region ApiType Properties
    /// <inheritdoc/>
    public override ApiTypeKind Kind => ApiTypeKind.Enum;
    #endregion

    #region ApiEnumType Fields
    private readonly Dictionary<string, ApiEnumValue> _apiNameLookup;
    private readonly Dictionary<string, ApiEnumValue> _clrNameLookup;
    private readonly Dictionary<int, ApiEnumValue> _clrOrdinalLookup;
    #endregion

    #region ApiEnumType Properties
    /// <summary>Gets the collection of enumeration values defined for this API enum type.</summary>
    public IReadOnlyCollection<ApiEnumValue> ApiEnumValues { get; }
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiEnumType"/> class.
    /// </summary>
    /// <param name="apiName">The API name of the enumeration type.</param>
    /// <param name="apiEnumValues">The collection of enumeration values associated with this type.</param>
    /// <param name="clrEnumType">The CLR type that defines the enumeration.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="clrEnumType"/> is not an enum.</exception>
    /// <exception cref="ApiSchemaException">Thrown if duplicate enum values are detected by API name, CLR name, or CLR ordinal.</exception>
    public ApiEnumType(string apiName, IEnumerable<ApiEnumValue> apiEnumValues, Type clrEnumType)
        : base(apiName, clrEnumType)
    {
        if (!TypeReflection.IsEnum(clrEnumType))
        {
            var message = $"Unable to create {nameof(ApiEnumType)} because the CLR type [name={clrEnumType.Name}] is not a CLR enum type.";
            throw new ApiSchemaException(message);
        }

        this.ApiEnumValues = apiEnumValues.SafeToArray();

        ApiSchemaHelpers.ValidateUnique<ApiEnumType, ApiEnumValue, string>(this.ApiEnumValues, x => x.ApiName, nameof(ApiEnumValue.ApiName));
        ApiSchemaHelpers.ValidateUnique<ApiEnumType, ApiEnumValue, string>(this.ApiEnumValues, x => x.ClrName, nameof(ApiEnumValue.ClrName));
        ApiSchemaHelpers.ValidateUnique<ApiEnumType, ApiEnumValue, int>(this.ApiEnumValues, x => x.ClrOrdinal, nameof(ApiEnumValue.ClrOrdinal));

        _apiNameLookup = this.ApiEnumValues.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        _clrNameLookup = this.ApiEnumValues.ToDictionary(x => x.ClrName, StringComparer.OrdinalIgnoreCase);
        _clrOrdinalLookup = this.ApiEnumValues.ToDictionary(x => x.ClrOrdinal);
    }
    #endregion

    #region ApiEnumType Methods
    /// <summary>
    ///     Attempts to retrieve an API enum value by its API name.
    /// </summary>
    /// <param name="apiName">The API name of the enumeration value to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiEnumValue"/> if found; otherwise, null.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool TryGetValueByApiName(string apiName, out ApiEnumValue? value) => _apiNameLookup.TryGetValue(apiName, out value);

    /// <summary>
    ///     Attempts to retrieve an API enum value by its CLR name.
    /// </summary>
    /// <param name="clrName">The CLR name of the enumeration value to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiEnumValue"/> if found; otherwise, null.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool TryGetValueByClrName(string clrName, out ApiEnumValue? value) => _clrNameLookup.TryGetValue(clrName, out value);

    /// <summary>
    ///     Attempts to retrieve an API enum value by its CLR ordinal value.
    /// </summary>
    /// <param name="clrOrdinal">The CLR ordinal of the enumeration value to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiEnumValue"/> if found; otherwise, null.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool TryGetValueByClrOrdinal(int clrOrdinal, out ApiEnumValue? value) => _clrOrdinalLookup.TryGetValue(clrOrdinal, out value);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrType = this.ClrType.SafeToString();

        return $"{nameof(ApiEnumType)} {{{nameof(this.ApiName)}={apiName}}} [{clrType}]";
    }
    #endregion
}
