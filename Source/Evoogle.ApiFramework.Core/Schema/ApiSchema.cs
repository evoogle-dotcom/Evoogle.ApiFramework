// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extension;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a collection of <see cref="ApiType"/> instances making up a schema.
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

    /// <summary>Gets the optional version of the schema.</summary>
    public string? Version { get; init; }

    /// <summary>Gets all API named types contained within this schema.</summary>
    public IReadOnlyCollection<ApiNamedType> ApiNamedTypes { get; }

    /// <summary>Gets all API enum types contained within this schema.</summary>
    public IReadOnlyCollection<ApiEnumType> ApiEnumTypes { get; }

    /// <summary>Gets all API object types contained within this schema.</summary>
    public IReadOnlyCollection<ApiObjectType> ApiObjectTypes { get; }

    /// <summary>Gets all API scalar types contained within this schema.</summary>
    public IReadOnlyCollection<ApiScalarType> ApiScalarTypes { get; }
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchema"/> class using separate collections for scalar, enum, and object types.
    /// </summary>
    /// <param name="name">The name of the schema.</param>
    /// <param name="version">The optional version of the schema.</param>
    /// <param name="apiScalarTypes">The collection of scalar types to include in the schema.</param>
    /// <param name="apiEnumTypes">The collection of enum types to include in the schema.</param>
    /// <param name="apiObjectTypes">The collection of object types to include in the schema.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="name"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if duplicate API or CLR identifiers are detected across the provided types.
    /// </exception>
    public ApiSchema
    (
        string name,
        IEnumerable<ApiScalarType>? apiScalarTypes,
        IEnumerable<ApiEnumType>? apiEnumTypes,
        IEnumerable<ApiObjectType>? apiObjectTypes
    )
    {
        // Initialize the schema with the provided name and version.
        this.Name = name ?? throw new ArgumentNullException(nameof(name), "Schema name cannot be null.");

        // Construct the API types from the provided collections.
        var apiNamedTypes = apiScalarTypes.SafeCast<ApiNamedType>()
            .Concat(apiEnumTypes.SafeCast<ApiNamedType>())
            .Concat(apiObjectTypes.SafeCast<ApiNamedType>())
            .OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        // Validate that there are no duplicate API names or CLR types across all API named types.
        ValidateUnique(apiNamedTypes, x => x.ApiName, nameof(ApiNamedType.ApiName));
        ValidateUnique(apiNamedTypes, x => x.ClrType, nameof(ApiType.ClrType));

        // Initialize the collections for API types, scalar types, enum types, and object types.
        this.ApiNamedTypes = apiNamedTypes;
        this.ApiScalarTypes = apiScalarTypes.SafeCast<ApiScalarType>().OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase).ToArray();
        this.ApiEnumTypes = apiEnumTypes.SafeCast<ApiEnumType>().OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase).ToArray();
        this.ApiObjectTypes = apiObjectTypes.SafeCast<ApiObjectType>().OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase).ToArray();

        // Initialize the lookup dictionaries for fast access to API types by API name and CLR type.
        _apiNameLookup = new(StringComparer.OrdinalIgnoreCase);
        _clrTypeLookup = new();
        foreach (var apiType in this.ApiNamedTypes)
        {
            _apiNameLookup[apiType.ApiName] = apiType;
            _clrTypeLookup[apiType.ClrType] = apiType;
        }

        // Initialize the lookup dictionaries for API scalar types by API name and CLR type.
        _scalarApiNameLookup = new(StringComparer.OrdinalIgnoreCase);
        _scalarClrTypeLookup = new();
        foreach (var apiScalarType in this.ApiScalarTypes)
        {
            _scalarApiNameLookup[apiScalarType.ApiName] = apiScalarType;
            _scalarClrTypeLookup[apiScalarType.ClrType] = apiScalarType;
        }

        // Initialize the lookup dictionaries for API enum types by API name and CLR type.
        _enumApiNameLookup = new(StringComparer.OrdinalIgnoreCase);
        _enumClrTypeLookup = new();
        foreach (var apiEnumType in this.ApiEnumTypes)
        {
            _enumApiNameLookup[apiEnumType.ApiName] = apiEnumType;
            _enumClrTypeLookup[apiEnumType.ClrType] = apiEnumType;
        }

        // Initialize the lookup dictionaries for API object types by API name and CLR type.
        _objectApiNameLookup = new(StringComparer.OrdinalIgnoreCase);
        _objectClrTypeLookup = new();
        foreach (var apiObjectType in this.ApiObjectTypes)
        {
            _objectApiNameLookup[apiObjectType.ApiName] = apiObjectType;
            _objectClrTypeLookup[apiObjectType.ClrType] = apiObjectType;
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchema"/> class from a collection of API named types.
    /// </summary>
    /// <param name="name">The schema name.</param>
    /// <param name="version">The optional version of the schema.</param>
    /// <param name="apiNamedTypes">The collection of API named types to include in the schema.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
    /// <exception cref="ApiSchemaException">Thrown if duplicate API or CLR identifiers are detected.</exception>
    public ApiSchema(string name, IEnumerable<ApiNamedType>? apiNamedTypes)
        : this
        (
            name,
            apiNamedTypes?.OfType<ApiScalarType>(),
            apiNamedTypes?.OfType<ApiEnumType>(),
            apiNamedTypes?.OfType<ApiObjectType>()
        )
    {
    }
    #endregion

    #region ApiSchema Methods
    /// <summary>
    ///     Resolves all <see cref="ApiTypeExpression"/> instances within object and collection types in the schema.
    ///     This ensures that every property and nested structure has its referenced or inline type linked.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if any named reference could not be resolved in the schema.
    /// </exception>
    public void ResolveAllReferences(ref List<ValidationResult>? results)
    {
        foreach (var apiObjectType in this.ApiObjectTypes)
        {
            foreach (var apiProperty in apiObjectType.ApiProperties)
            {
                apiProperty.Resolve(this, ref results);
            }
        }
    }

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
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var name = this.Name.SafeToString();
        var count = this.ApiNamedTypes.Count.SafeToString();
        var scalarCount = this.ApiScalarTypes.Count.SafeToString();
        var enumCount = this.ApiEnumTypes.Count.SafeToString();
        var objectCount = this.ApiObjectTypes.Count.SafeToString();
        return $"{nameof(ApiSchema)} {{Name={name}, Count={count}, ScalarCount={scalarCount}, EnumCount={enumCount}, ObjectCount={objectCount}}}";
    }
    #endregion

    #region Validation Methods
    /// <summary>
    ///     Validates that a collection of named types contains unique values for a specified key.
    /// </summary>
    /// <typeparam name="TApiType">The type of API named type being validated.</typeparam>
    /// <typeparam name="TKey">The type of the key to validate uniqueness for.</typeparam>
    /// <param name="values">The collection of values to check.</param>
    /// <param name="keySelector">Function to select the key from each value.</param>
    /// <param name="propertyName">The name of the property being validated (used in exception message).</param>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if any duplicate keys are found in the collection.
    /// </exception>
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
}
