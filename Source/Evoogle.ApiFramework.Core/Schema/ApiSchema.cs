// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
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
    private Dictionary<string, ApiNamedType>? _apiNamedTypeApiNameLookup = null;
    private Dictionary<Type, ApiNamedType>? _apiNamedTypeClrTypeLookup = null;

    private Dictionary<string, ApiEnumType>? _apiEnumTypeApiNameLookup = null;
    private Dictionary<Type, ApiEnumType>? _apiEnumTypeClrTypeLookup = null;

    private Dictionary<string, ApiObjectType>? _apiObjectTypeApiNameLookup = null;
    private Dictionary<Type, ApiObjectType>? _apiObjectTypeClrTypeLookup = null;

    private Dictionary<string, ApiScalarType>? _apiScalarTypeApiNameLookup = null;
    private Dictionary<Type, ApiScalarType>? _apiScalarTypeClrTypeLookup = null;
    #endregion

    #region ApiSchema Properties
    /// <summary>Gets the name of the API schema.</summary>
    public string ApiName { get; }

    /// <summary>Gets the optional version of the API schema.</summary>
    public string? ApiVersion { get; init; }

    /// <summary>Gets all API named types contained within this API schema.</summary>
    public ApiNamedType[] ApiNamedTypes { get; }

    /// <summary>Gets all API enum types contained within this API schema.</summary>
    public ApiEnumType[] ApiEnumTypes { get; }

    /// <summary>Gets all API object types contained within this API schema.</summary>
    public ApiObjectType[] ApiObjectTypes { get; }

    /// <summary>Gets all API scalar types contained within this API schema.</summary>
    public ApiScalarType[] ApiScalarTypes { get; }

    private Dictionary<string, ApiNamedType> ApiNamedTypeApiNameLookup => this.ThrowIfNotInitialized(_apiNamedTypeApiNameLookup);
    private Dictionary<Type, ApiNamedType> ApiNamedTypeClrTypeLookup => this.ThrowIfNotInitialized(_apiNamedTypeClrTypeLookup);

    private Dictionary<string, ApiEnumType> ApiEnumTypeApiNameLookup => this.ThrowIfNotInitialized(_apiEnumTypeApiNameLookup);
    private Dictionary<Type, ApiEnumType> ApiEnumTypeClrTypeLookup => this.ThrowIfNotInitialized(_apiEnumTypeClrTypeLookup);

    private Dictionary<string, ApiObjectType> ApiObjectTypeApiNameLookup => this.ThrowIfNotInitialized(_apiObjectTypeApiNameLookup);
    private Dictionary<Type, ApiObjectType> ApiObjectTypeClrTypeLookup => this.ThrowIfNotInitialized(_apiObjectTypeClrTypeLookup);

    private Dictionary<string, ApiScalarType> ApiScalarTypeApiNameLookup => this.ThrowIfNotInitialized(_apiScalarTypeApiNameLookup);
    private Dictionary<Type, ApiScalarType> ApiScalarTypeClrTypeLookup => this.ThrowIfNotInitialized(_apiScalarTypeClrTypeLookup);

    private string ValidationPath => $"{nameof(ApiSchema)}[\"{this.ApiName.SafeToString()}\"]";
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchema"/> class using separate collections for scalar, enum, and object types.
    /// </summary>
    /// <param name="apiName">The name of the API schema.</param>
    /// <param name="apiVersion">The optional version of the API schema.</param>
    /// <param name="apiScalarTypes">The collection of scalar types to include in the API schema.</param>
    /// <param name="apiEnumTypes">The collection of enum types to include in the API schema.</param>
    /// <param name="apiObjectTypes">The collection of object types to include in the API schema.</param>
    public ApiSchema
    (
        string apiName,
        IEnumerable<ApiScalarType>? apiScalarTypes,
        IEnumerable<ApiEnumType>? apiEnumTypes,
        IEnumerable<ApiObjectType>? apiObjectTypes
    )
    {
        // Initialize the API name.
        this.ApiName = apiName;

        // Initialize the collections for API types, scalar types, enum types, and object types.
        this.ApiScalarTypes = [.. apiScalarTypes.EmptyIfNull().OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];

        this.ApiEnumTypes = [.. apiEnumTypes.EmptyIfNull().OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];

        this.ApiObjectTypes = [.. apiObjectTypes.EmptyIfNull().OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];

        // Initialize the collection of all API named types.
        this.ApiNamedTypes = [.. this.ApiScalarTypes.SafeCast<ApiNamedType>().Concat(this.ApiEnumTypes.SafeCast<ApiNamedType>()).Concat(this.ApiObjectTypes.SafeCast<ApiNamedType>()).OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiSchema"/> class from a collection of API named types.
    /// </summary>
    /// <param name="apiName">The name of the API schema.</param>
    /// <param name="apiVersion">The optional version of the API schema.</param>
    /// <param name="apiNamedTypes">The collection of API named types to include in the API schema.</param>
    public ApiSchema(string apiName, IEnumerable<ApiNamedType>? apiNamedTypes)
        : this(apiName, apiNamedTypes?.OfType<ApiScalarType>(), apiNamedTypes?.OfType<ApiEnumType>(), apiNamedTypes?.OfType<ApiObjectType>())
    { }
    #endregion

    #region ApiSchema Methods
    public ApiSchemaInitializeResult Initialize()
    {
        List<ValidationResult>? results = null;
        this.Initialize(ref results);

        return new ApiSchemaInitializeResult(results);
    }

    public void Initialize(ref List<ValidationResult>? results)
    {
        this.InitializeApiName(ref results);

        this.InitializeLookupDictionaries(ref results);

        this.InitializeApiScalarTypes(ref results);
        this.InitializeApiEnumTypes(ref results);
        this.InitializeApiObjectTypes(ref results);
    }

    /// <summary>Attempts to retrieve an API named type by its API name.</summary>
    public bool TryGetApiType(string apiName, out ApiNamedType? apiNamedType) => this.ApiNamedTypeApiNameLookup.TryGetValue(apiName, out apiNamedType);

    /// <summary>Attempts to retrieve an API named type by its CLR type.</summary>
    public bool TryGetApiType(Type clrType, out ApiNamedType? apiNamedType) => this.ApiNamedTypeClrTypeLookup.TryGetValue(clrType, out apiNamedType);

    /// <summary>Attempts to retrieve an API enumeration type by its API name.</summary>
    public bool TryGetApiEnumType(string apiName, out ApiEnumType? apiEnumType) => this.ApiEnumTypeApiNameLookup.TryGetValue(apiName, out apiEnumType);

    /// <summary>Attempts to retrieve an API enumeration type by its CLR type.</summary>
    public bool TryGetApiEnumType(Type clrType, out ApiEnumType? apiEnumType) => this.ApiEnumTypeClrTypeLookup.TryGetValue(clrType, out apiEnumType);

    /// <summary>Attempts to retrieve an API object type by its API name.</summary>
    public bool TryGetApiObjectType(string apiName, out ApiObjectType? apiObjectType) => this.ApiObjectTypeApiNameLookup.TryGetValue(apiName, out apiObjectType);

    /// <summary>Attempts to retrieve an API object type by its CLR type.</summary>
    public bool TryGetApiObjectType(Type clrType, out ApiObjectType? apiObjectType) => this.ApiObjectTypeClrTypeLookup.TryGetValue(clrType, out apiObjectType);

    /// <summary>Attempts to retrieve an API scalar type by its API name.</summary>
    public bool TryGetApiScalarType(string apiName, out ApiScalarType? apiScalarType) => this.ApiScalarTypeApiNameLookup.TryGetValue(apiName, out apiScalarType);

    /// <summary>Attempts to retrieve an API scalar type by its CLR type.</summary>
    public bool TryGetApiScalarType(Type clrType, out ApiScalarType? apiScalarType) => this.ApiScalarTypeClrTypeLookup.TryGetValue(clrType, out apiScalarType);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiVersion = this.ApiVersion.SafeToString();
        var apiNamedTypeCount = this.ApiNamedTypes.Length.SafeToString();
        var apiScalarTypeCount = this.ApiScalarTypes.Length.SafeToString();
        var apiEnumTypeCount = this.ApiEnumTypes.Length.SafeToString();
        var apiObjectTypeCount = this.ApiObjectTypes.Length.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiSchema)} {{ApiName={apiName}, ApiVersion={apiVersion}, ApiNamedTypeCount={apiNamedTypeCount}, ApiScalarTypeCount={apiScalarTypeCount}, ApiEnumTypeCount={apiEnumTypeCount}, ApiObjectTypeCount={apiObjectTypeCount}, {nameof(ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiEnumTypes(ref List<ValidationResult>? results)
    {
        foreach (var apiEnumType in this.ApiEnumTypes)
        {
            apiEnumType.Initialize(this, ref results);
        }
    }

    private void InitializeApiName(ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{nameof(ApiSchema)}.{nameof(this.ApiName)} cannot be null or whitespace.", [nameof(this.ApiName)]));
        }
    }

    private void InitializeApiObjectTypes(ref List<ValidationResult>? results)
    {
        foreach (var apiObjectType in this.ApiObjectTypes)
        {
            apiObjectType.Initialize(this, ref results);
        }
    }

    private void InitializeApiScalarTypes(ref List<ValidationResult>? results)
    {
        foreach (var apiScalarType in this.ApiScalarTypes)
        {
            apiScalarType.Initialize(this, ref results);
        }
    }

    private void InitializeLookupDictionaries(ref List<ValidationResult>? results)
    {
        // Initialize the lookup dictionaries for lookup by API name and CLR type.
        _apiNamedTypeApiNameLookup = null;
        _apiNamedTypeClrTypeLookup = null;

        _apiEnumTypeApiNameLookup = null;
        _apiEnumTypeClrTypeLookup = null;

        _apiObjectTypeApiNameLookup = null;
        _apiObjectTypeClrTypeLookup = null;

        _apiScalarTypeApiNameLookup = null;
        _apiScalarTypeClrTypeLookup = null;

        // Validate uniqueness of API names and CLR types across all API named types.
        var anyApiEnumTypeApiNameDuplicates = ApiSchemaHelpers.ValidateUnique(this.ApiEnumTypes, x => x.ApiName, this.ValidationPath, nameof(ApiEnumType.ApiName), ref results);
        var anyApiEnumTypeClrTypeDuplicates = ApiSchemaHelpers.ValidateUnique(this.ApiEnumTypes, x => x.ClrType, this.ValidationPath, nameof(ApiEnumType.ClrType), ref results);

        var anyApiObjectTypeApiNameDuplicates = ApiSchemaHelpers.ValidateUnique(this.ApiObjectTypes, x => x.ApiName, this.ValidationPath, nameof(ApiObjectType.ApiName), ref results);
        var anyApiObjectTypeClrTypeDuplicates = ApiSchemaHelpers.ValidateUnique(this.ApiObjectTypes, x => x.ClrType, this.ValidationPath, nameof(ApiObjectType.ClrType), ref results);

        var anyApiScalarTypeApiNameDuplicates = ApiSchemaHelpers.ValidateUnique(this.ApiScalarTypes, x => x.ApiName, this.ValidationPath, nameof(ApiScalarType.ApiName), ref results);
        var anyApiScalarTypeClrTypeDuplicates = ApiSchemaHelpers.ValidateUnique(this.ApiScalarTypes, x => x.ClrType, this.ValidationPath, nameof(ApiScalarType.ClrType), ref results);

        // Initialize the lookup dictionaries for fast access to API types by API name and CLR type.
        _apiNamedTypeApiNameLookup = new(StringComparer.OrdinalIgnoreCase);
        _apiNamedTypeClrTypeLookup = [];
        foreach (var apiNamedType in this.ApiNamedTypes)
        {
            _apiNamedTypeApiNameLookup[apiNamedType.ApiName] = apiNamedType;
            _apiNamedTypeClrTypeLookup[apiNamedType.ClrType] = apiNamedType;
        }

        // Initialize the lookup dictionaries for API scalar types by API name and CLR type.
        _apiScalarTypeApiNameLookup = new(StringComparer.OrdinalIgnoreCase);
        _apiScalarTypeClrTypeLookup = new();
        foreach (var apiScalarType in this.ApiScalarTypes)
        {
            _apiScalarTypeApiNameLookup[apiScalarType.ApiName] = apiScalarType;
            _apiScalarTypeClrTypeLookup[apiScalarType.ClrType] = apiScalarType;
        }

        // Initialize the lookup dictionaries for API enum types by API name and CLR type.
        _apiEnumTypeApiNameLookup = new(StringComparer.OrdinalIgnoreCase);
        _apiEnumTypeClrTypeLookup = new();
        foreach (var apiEnumType in this.ApiEnumTypes)
        {
            _apiEnumTypeApiNameLookup[apiEnumType.ApiName] = apiEnumType;
            _apiEnumTypeClrTypeLookup[apiEnumType.ClrType] = apiEnumType;
        }

        // Initialize the lookup dictionaries for API object types by API name and CLR type.
        _apiObjectTypeApiNameLookup = new(StringComparer.OrdinalIgnoreCase);
        _apiObjectTypeClrTypeLookup = new();
        foreach (var apiObjectType in this.ApiObjectTypes)
        {
            _apiObjectTypeApiNameLookup[apiObjectType.ApiName] = apiObjectType;
            _apiObjectTypeClrTypeLookup[apiObjectType.ClrType] = apiObjectType;
        }
    }
    #endregion
}
