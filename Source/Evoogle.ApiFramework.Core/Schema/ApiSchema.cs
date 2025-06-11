// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extension;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a collection of <see cref="ApiNamedType"/> instances making up a schema.
/// </summary>
public sealed class ApiSchema : ExtensibleBase
{
    #region ApiSchema Fields
    private readonly Dictionary<string, ApiNamedType> _apiNameLookup;
    private readonly Dictionary<Type, ApiNamedType> _clrTypeLookup;

    private readonly Dictionary<string, ApiEnumType> _enumApiNameLookup;
    private readonly Dictionary<Type, ApiEnumType> _enumClrTypeLookup;

    private readonly Dictionary<string, ApiObjectType> _objectApiNameLookup;
    private readonly Dictionary<Type, ApiObjectType> _objectClrTypeLookup;

    private readonly Dictionary<string, ApiScalarType> _scalarApiNameLookup;
    private readonly Dictionary<Type, ApiScalarType> _scalarClrTypeLookup;
    #endregion

    #region ApiSchema Properties
    /// <summary>Gets all API named types contained within this schema.</summary>
    public IReadOnlyCollection<ApiNamedType> ApiTypes { get; }

    /// <summary>Gets all API enumeration types contained within this schema.</summary>
    public IReadOnlyCollection<ApiEnumType> ApiEnumerationTypes { get; }

    /// <summary>Gets all API object types contained within this schema.</summary>
    public IReadOnlyCollection<ApiObjectType> ApiObjectTypes { get; }

    /// <summary>Gets all API scalar types contained within this schema.</summary>
    public IReadOnlyCollection<ApiScalarType> ApiScalarTypes { get; }
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
        var namedValues = values.OfType<ApiNamedType>().ToArray();

        ValidateUnique(namedValues, x => x.ApiName, nameof(ApiNamedType.ApiName));
        ValidateUnique(namedValues, x => x.ClrType.Name, "ClrName");
        ValidateUnique(namedValues, x => x.ClrType, nameof(ApiType.ClrType));

        this.ApiTypes = namedValues;
        this.ApiEnumerationTypes = namedValues.OfType<ApiEnumType>().ToArray();
        this.ApiObjectTypes = namedValues.OfType<ApiObjectType>().ToArray();
        this.ApiScalarTypes = namedValues.OfType<ApiScalarType>().ToArray();

        this._apiNameLookup = namedValues.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        this._clrTypeLookup = namedValues.ToDictionary(x => x.ClrType);

        this._enumApiNameLookup = this.ApiEnumerationTypes.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        this._enumClrTypeLookup = this.ApiEnumerationTypes.ToDictionary(x => x.ClrType);

        this._objectApiNameLookup = this.ApiObjectTypes.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        this._objectClrTypeLookup = this.ApiObjectTypes.ToDictionary(x => x.ClrType);

        this._scalarApiNameLookup = this.ApiScalarTypes.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        this._scalarClrTypeLookup = this.ApiScalarTypes.ToDictionary(x => x.ClrType);
    }
    #endregion

    #region ApiSchema Methods
    /// <summary>Attempts to retrieve an API named type by its API name.</summary>
    public bool TryGetApiType(string apiName, out ApiNamedType? apiNamedType)
        => this._apiNameLookup.TryGetValue(apiName, out apiNamedType);

    /// <summary>Attempts to retrieve an API named type by its CLR type.</summary>
    public bool TryGetApiType(Type clrType, out ApiNamedType? apiNamedType)
        => this._clrTypeLookup.TryGetValue(clrType, out apiNamedType);

    /// <summary>Attempts to retrieve an API enumeration type by its API name.</summary>
    public bool TryGetApiEnumerationType(string apiName, out ApiEnumType? apiEnumerationType)
        => this._enumApiNameLookup.TryGetValue(apiName, out apiEnumerationType);

    /// <summary>Attempts to retrieve an API enumeration type by its CLR type.</summary>
    public bool TryGetApiEnumerationType(Type clrType, out ApiEnumType? apiEnumerationType)
        => this._enumClrTypeLookup.TryGetValue(clrType, out apiEnumerationType);

    /// <summary>Attempts to retrieve an API object type by its API name.</summary>
    public bool TryGetApiObjectType(string apiName, out ApiObjectType? apiObjectType)
        => this._objectApiNameLookup.TryGetValue(apiName, out apiObjectType);

    /// <summary>Attempts to retrieve an API object type by its CLR type.</summary>
    public bool TryGetApiObjectType(Type clrType, out ApiObjectType? apiObjectType)
        => this._objectClrTypeLookup.TryGetValue(clrType, out apiObjectType);

    /// <summary>Attempts to retrieve an API scalar type by its API name.</summary>
    public bool TryGetApiScalarType(string apiName, out ApiScalarType? apiScalarType)
        => this._scalarApiNameLookup.TryGetValue(apiName, out apiScalarType);

    /// <summary>Attempts to retrieve an API scalar type by its CLR type.</summary>
    public bool TryGetApiScalarType(Type clrType, out ApiScalarType? apiScalarType)
        => this._scalarClrTypeLookup.TryGetValue(clrType, out apiScalarType);

    private static void ValidateUnique<TApiType, TKey>(IEnumerable<TApiType> values, Func<TApiType, TKey> keySelector, string propertyName)
        where TApiType : ApiNamedType
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
