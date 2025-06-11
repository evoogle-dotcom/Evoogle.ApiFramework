// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extension;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a collection of <see cref="ApiType"/> instances making up a schema.
/// </summary>
public sealed class ApiSchema : ExtensibleBase
{
    #region ApiSchema Fields
    private readonly Dictionary<string, ApiType> _apiNameLookup;
    private readonly Dictionary<string, ApiType> _clrNameLookup;
    private readonly Dictionary<Type, ApiType> _clrTypeLookup;
    #endregion

    #region ApiSchema Properties
    /// <summary>Gets all API types contained within this schema.</summary>
    public IReadOnlyCollection<ApiType> ApiTypes { get; }
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchema"/> class.
    /// </summary>
    /// <param name="apiTypes">The collection of API types to include in the schema.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="apiTypes"/> is null.</exception>
    /// <exception cref="ApiSchemaException">Thrown if duplicate API or CLR identifiers are detected.</exception>
    public ApiSchema(IEnumerable<ApiType> apiTypes)
    {
        var values = apiTypes.SafeToArray();

        ValidateUnique(values.OfType<ApiNamedType>(), x => x.ApiName, nameof(ApiNamedType.ApiName));
        ValidateUnique(values, x => x.ClrType.Name, "ClrName");
        ValidateUnique(values, x => x.ClrType, nameof(ApiType.ClrType));

        this.ApiTypes = values;

        this._apiNameLookup = values.OfType<ApiNamedType>().Cast<ApiType>().ToDictionary(x => ((ApiNamedType)x).ApiName, StringComparer.OrdinalIgnoreCase);
        this._clrNameLookup = values.ToDictionary(x => x.ClrType.Name, StringComparer.OrdinalIgnoreCase);
        this._clrTypeLookup = values.ToDictionary(x => x.ClrType);
    }
    #endregion

    #region ApiSchema Methods
    /// <summary>
    ///     Attempts to retrieve an <see cref="ApiType"/> by its API name.
    /// </summary>
    /// <param name="apiName">The API name of the type to retrieve.</param>
    /// <param name="apiType">When this method returns, contains the <see cref="ApiType"/> if found.</param>
    /// <returns>True if the type is found; otherwise, false.</returns>
    public bool TryGetByApiName(string apiName, out ApiType? apiType)
        => this._apiNameLookup.TryGetValue(apiName, out apiType);

    /// <summary>
    ///     Attempts to retrieve an <see cref="ApiType"/> by its CLR name.
    /// </summary>
    /// <param name="clrName">The CLR type name to look up.</param>
    /// <param name="apiType">When this method returns, contains the <see cref="ApiType"/> if found.</param>
    /// <returns>True if the type is found; otherwise, false.</returns>
    public bool TryGetByClrName(string clrName, out ApiType? apiType)
        => this._clrNameLookup.TryGetValue(clrName, out apiType);

    /// <summary>
    ///     Attempts to retrieve an <see cref="ApiType"/> by its CLR type.
    /// </summary>
    /// <param name="clrType">The CLR <see cref="Type"/> to look up.</param>
    /// <param name="apiType">When this method returns, contains the <see cref="ApiType"/> if found.</param>
    /// <returns>True if the type is found; otherwise, false.</returns>
    public bool TryGetByClrType(Type clrType, out ApiType? apiType)
        => this._clrTypeLookup.TryGetValue(clrType, out apiType);

    private static void ValidateUnique<TApiType, TKey>(IEnumerable<TApiType> values, Func<TApiType, TKey> keySelector, string propertyName)
        where TApiType : ApiType
        where TKey : notnull
    {
        var duplicates = values
            .GroupBy(keySelector)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count == 0)
            return;

        var duplicatesString = string.Join(",", duplicates);
        var message = $"Unable to create {nameof(ApiSchema)} because duplicate {propertyName} values detected: {duplicatesString}";
        throw new ApiSchemaException(message);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var count = this.ApiTypes.Count;
        return $"{nameof(ApiSchema)} {{Count={count}}}";
    }
    #endregion
}
