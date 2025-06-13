// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extension;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a collection of <see cref="ApiNamedType"/> instances making up a schema.
/// </summary>
[JsonConverter(typeof(ApiSchemaJsonConverter))]
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
    /// <summary>Gets the name of the schema.</summary>
    public string Name { get; }

    /// <summary>Gets all API named types contained within this schema.</summary>
    public IReadOnlyCollection<ApiNamedType> ApiTypes { get; }

    /// <summary>Gets all API enum types contained within this schema.</summary>
    public IReadOnlyCollection<ApiEnumType> ApiEnumTypes { get; }

    /// <summary>Gets all API object types contained within this schema.</summary>
    public IReadOnlyCollection<ApiObjectType> ApiObjectTypes { get; }

    /// <summary>Gets all API scalar types contained within this schema.</summary>
    public IReadOnlyCollection<ApiScalarType> ApiScalarTypes { get; }
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchema"/> class.
    /// </summary>
    /// <param name="name">The schema name.</param>
    /// <param name="apiTypes">The collection of API types to include in the schema.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="apiTypes"/> is null.</exception>
    /// <exception cref="ApiSchemaException">Thrown if duplicate API or CLR identifiers are detected.</exception>
    public ApiSchema(string name, IEnumerable<ApiType> apiTypes)
    {
        this.Name = name ?? throw new ArgumentNullException(nameof(name));

        var values = apiTypes.SafeToArray();
        var namedValues = values
            .OfType<ApiNamedType>()
            .OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        ValidateUnique(namedValues, x => x.ApiName, nameof(ApiNamedType.ApiName));
        ValidateUnique(namedValues, x => x.ClrType, nameof(ApiType.ClrType));

        this.ApiTypes = namedValues;
        this.ApiEnumTypes = namedValues.OfType<ApiEnumType>().ToArray();
        this.ApiObjectTypes = namedValues.OfType<ApiObjectType>().ToArray();
        this.ApiScalarTypes = namedValues.OfType<ApiScalarType>().ToArray();

        _apiNameLookup = namedValues.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        _clrTypeLookup = namedValues.ToDictionary(x => x.ClrType);

        _enumApiNameLookup = this.ApiEnumTypes.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        _enumClrTypeLookup = this.ApiEnumTypes.ToDictionary(x => x.ClrType);

        _objectApiNameLookup = this.ApiObjectTypes.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        _objectClrTypeLookup = this.ApiObjectTypes.ToDictionary(x => x.ClrType);

        _scalarApiNameLookup = this.ApiScalarTypes.ToDictionary(x => x.ApiName, StringComparer.OrdinalIgnoreCase);
        _scalarClrTypeLookup = this.ApiScalarTypes.ToDictionary(x => x.ClrType);
    }
    #endregion

    #region ApiSchema Methods
    /// <summary>Attempts to retrieve an API named type by its API name.</summary>
    public bool TryGetApiType(string apiName, out ApiNamedType? apiNamedType)
        => _apiNameLookup.TryGetValue(apiName, out apiNamedType);

    /// <summary>Attempts to retrieve an API named type by its CLR type.</summary>
    public bool TryGetApiType(Type clrType, out ApiNamedType? apiNamedType)
        => _clrTypeLookup.TryGetValue(clrType, out apiNamedType);

    /// <summary>Attempts to retrieve an API enumeration type by its API name.</summary>
    public bool TryGetApiEnumType(string apiName, out ApiEnumType? apiEnumType)
        => _enumApiNameLookup.TryGetValue(apiName, out apiEnumType);

    /// <summary>Attempts to retrieve an API enumeration type by its CLR type.</summary>
    public bool TryGetApiEnumType(Type clrType, out ApiEnumType? apiEnumType)
        => _enumClrTypeLookup.TryGetValue(clrType, out apiEnumType);

    /// <summary>Attempts to retrieve an API object type by its API name.</summary>
    public bool TryGetApiObjectType(string apiName, out ApiObjectType? apiObjectType)
        => _objectApiNameLookup.TryGetValue(apiName, out apiObjectType);

    /// <summary>Attempts to retrieve an API object type by its CLR type.</summary>
    public bool TryGetApiObjectType(Type clrType, out ApiObjectType? apiObjectType)
        => _objectClrTypeLookup.TryGetValue(clrType, out apiObjectType);

    /// <summary>Attempts to retrieve an API scalar type by its API name.</summary>
    public bool TryGetApiScalarType(string apiName, out ApiScalarType? apiScalarType)
        => _scalarApiNameLookup.TryGetValue(apiName, out apiScalarType);

    /// <summary>Attempts to retrieve an API scalar type by its CLR type.</summary>
    public bool TryGetApiScalarType(Type clrType, out ApiScalarType? apiScalarType)
        => _scalarClrTypeLookup.TryGetValue(clrType, out apiScalarType);

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
        var name = this.Name.SafeToString();
        var count = this.ApiTypes.Count.SafeToString();
        var scalarCount = this.ApiScalarTypes.Count.SafeToString();
        var enumCount = this.ApiEnumTypes.Count.SafeToString();
        var objectCount = this.ApiObjectTypes.Count.SafeToString();
        return $"{nameof(ApiSchema)} {{Name={name}, Count={count}, ScalarCount={scalarCount}, EnumCount={enumCount}, ObjectCount={objectCount}}}";
    }
    #endregion
}
