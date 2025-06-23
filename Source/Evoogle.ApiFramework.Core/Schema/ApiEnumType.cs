// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
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
            var message = $"Unable to create an API enum type, the CLR type [name={clrEnumType.Name}] is not a CLR enum type.";
            throw new ApiSchemaException(message);
        }

        var values = apiEnumValues.SafeToArray();

        this.ValidateUnique(values, x => x.ApiName, nameof(ApiEnumValue.ApiName));
        this.ValidateUnique(values, x => x.ClrName, nameof(ApiEnumValue.ClrName));
        this.ValidateUnique(values, x => x.ClrOrdinal, nameof(ApiEnumValue.ClrOrdinal));

        this.ApiEnumValues = values;

        _apiNameLookup = values.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        _clrNameLookup = values.ToDictionary(x => x.ClrName, StringComparer.OrdinalIgnoreCase);
        _clrOrdinalLookup = values.ToDictionary(x => x.ClrOrdinal);
    }
    #endregion

    #region ApiEnumType Methods
    /// <summary>
    ///     Attempts to retrieve an enum value by its API name.
    /// </summary>
    /// <param name="apiName">The API name of the enumeration value to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiEnumValue"/> if found; otherwise, null.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool TryGetByApiName(string apiName, out ApiEnumValue? value)
        => _apiNameLookup.TryGetValue(apiName, out value);

    /// <summary>
    ///     Attempts to retrieve an enum value by its CLR name.
    /// </summary>
    /// <param name="clrName">The CLR name of the enumeration value to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiEnumValue"/> if found; otherwise, null.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool TryGetByClrName(string clrName, out ApiEnumValue? value)
        => _clrNameLookup.TryGetValue(clrName, out value);

    /// <summary>
    ///     Attempts to retrieve an enum value by its CLR ordinal value.
    /// </summary>
    /// <param name="clrOrdinal">The CLR ordinal of the enumeration value to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiEnumValue"/> if found; otherwise, null.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool TryGetByClrOrdinal(int clrOrdinal, out ApiEnumValue? value)
        => _clrOrdinalLookup.TryGetValue(clrOrdinal, out value);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrType = this.ClrType.SafeToString();

        return $"{nameof(ApiEnumType)} {{{nameof(ApiName)}={apiName}}} [{clrType}]";
    }
    #endregion

    #region Validation Methods
    /// <summary>
    ///     Validates that the specified key selector produces unique values across all enum values.
    /// </summary>
    /// <typeparam name="T">The type of the key selected from each <see cref="ApiEnumValue"/>.</typeparam>
    /// <param name="values">The collection of enum values to check.</param>
    /// <param name="keySelector">A function to extract the key to test for uniqueness.</param>
    /// <param name="propertyName">The name of the property being validated (for error messages).</param>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when duplicate key values are found for the specified property.
    /// </exception>
    private void ValidateUnique<T>(IEnumerable<ApiEnumValue> values, Func<ApiEnumValue, T> keySelector, string propertyName)
    {
        var duplicates = values
            .GroupBy(keySelector)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count == 0)
            return;

        var duplicatesString = string.Join(",", duplicates);
        var message = $"Unable to create {this} because duplicate {propertyName} values detected: {duplicatesString}";
        throw new ApiSchemaException(message);
    }
    #endregion
}
