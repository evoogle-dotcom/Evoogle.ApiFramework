// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API object that has API named properties that can be read/written from/to an API service.
/// </summary>
public sealed class ApiObjectType : ApiNamedType
{
    #region ApiType Properties
    /// <inheritdoc/>
    public override ApiTypeKind Kind => ApiTypeKind.Object;
    #endregion

    #region ApiObject Fields
    private readonly Dictionary<string, ApiProperty> _apiNameLookup;
    private readonly Dictionary<string, ApiProperty> _clrNameLookup;
    #endregion

    #region ApiObject Properties
    /// <summary>
    ///     Gets the collection of API properties defined on this object type.
    /// </summary>
    public IReadOnlyCollection<ApiProperty> ApiProperties { get; }
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiObjectType"/> class.
    /// </summary>
    /// <param name="apiName">The API name of the object type.</param>
    /// <param name="apiProperties">The collection of API properties defined on this object type.</param>
    /// <param name="clrObjectType">The CLR type representing this API object.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="apiProperties"/> is null.</exception>
    /// <exception cref="ApiSchemaException">Thrown if duplicate API or CLR property names are detected.</exception>
    public ApiObjectType(string apiName, IEnumerable<ApiProperty> apiProperties, Type clrObjectType)
        : base(apiName, clrObjectType)
    {
        var values = apiProperties.SafeToArray();

        this.ValidateUnique(values, x => x.ApiName, nameof(ApiProperty.ApiName));
        this.ValidateUnique(values, x => x.ClrName, nameof(ApiProperty.ClrName));

        this.ApiProperties = values;

        _apiNameLookup = values.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        _clrNameLookup = values.ToDictionary(x => x.ClrName, StringComparer.OrdinalIgnoreCase);
    }
    #endregion

    #region ApiObjectType Methods
    /// <summary>
    ///     Attempts to retrieve an API property by its API name.
    /// </summary>
    /// <param name="apiName">The API name of the property to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiProperty"/> if found; otherwise, null.</param>
    /// <returns>True if the property was found; otherwise, false.</returns>
    public bool TryGetByApiName(string apiName, out ApiProperty? value) => _apiNameLookup.TryGetValue(apiName, out value);

    /// <summary>
    ///     Attempts to retrieve an API property by its CLR property name.
    /// </summary>
    /// <param name="clrName">The CLR name of the property to retrieve.</param>
    /// <param name="value">When this method returns, contains the <see cref="ApiProperty"/> if found; otherwise, null.</param>
    /// <returns>True if the property was found; otherwise, false.</returns>
    public bool TryGetByClrName(string clrName, out ApiProperty? value) => _clrNameLookup.TryGetValue(clrName, out value);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrType = this.ClrType.SafeToString();

        return $"{nameof(ApiObjectType)} {{{nameof(this.ApiName)}={apiName}}} [{clrType}]";
    }
    #endregion

    #region Validation Methods
    /// <summary>
    ///     Validates that the specified key selector produces unique values across all API properties.
    /// </summary>
    /// <typeparam name="T">The type of the key selected from each <see cref="ApiProperty"/>.</typeparam>
    /// <param name="values">The collection of properties to check.</param>
    /// <param name="keySelector">A function to extract the key to test for uniqueness.</param>
    /// <param name="propertyName">The name of the property being validated (for error messages).</param>
    /// <exception cref="ApiSchemaException">
    ///     Thrown when duplicate key values are found for the specified property.
    /// </exception>
    private void ValidateUnique<T>(IEnumerable<ApiProperty> values, Func<ApiProperty, T> keySelector, string propertyName)
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
