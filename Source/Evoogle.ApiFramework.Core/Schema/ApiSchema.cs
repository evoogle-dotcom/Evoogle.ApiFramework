// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extension;
using Evoogle.Extensions;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a collection of <see cref="ApiType"/> instances making up a schema.
/// </summary>
[JsonConverter(typeof(ApiSchemaJsonConverter))]
public sealed class ApiSchema : ExtensibleBase
{
    #region ApiSchema Fields
    private string? _apiPath = null;

    private ApiSchemaContext? _apiSchemaContext = null;

    private Dictionary<string, ApiNamedType>? _apiNamedTypeApiNameLookup = null;
    private Dictionary<Type, ApiNamedType>? _apiNamedTypeClrTypeLookup = null;

    private Dictionary<string, ApiEnumType>? _apiEnumTypeApiNameLookup = null;
    private Dictionary<Type, ApiEnumType>? _apiEnumTypeClrTypeLookup = null;

    private Dictionary<string, ApiObjectType>? _apiObjectTypeApiNameLookup = null;
    private Dictionary<Type, ApiObjectType>? _apiObjectTypeClrTypeLookup = null;

    private Dictionary<string, ApiScalarType>? _apiScalarTypeApiNameLookup = null;
    private Dictionary<Type, ApiScalarType>? _apiScalarTypeClrTypeLookup = null;

    private Dictionary<string, ApiRelationship>? _apiRelationshipApiNameLookup = null;
    #endregion

    #region ApiSchema Properties
    /// <summary>Gets the name of the API schema.</summary>
    public string ApiName { get; }

    /// <summary>Gets the version of the API schema.</summary>
    public string ApiVersion { get; }

    /// <summary>Gets the options used to configure this API schema.</summary>
    public ApiSchemaOptions ApiOptions { get; }

    /// <summary>Gets the API path for this schema. Available after initialization.</summary>
    public string ApiPath => this.ThrowIfNotInitialized(_apiPath);

    /// <summary>Gets the runtime context for this API schema. Available after initialization.</summary>
    public ApiSchemaContext ApiSchemaContext => this.ThrowIfNotInitialized(_apiSchemaContext);

    /// <summary>Gets all API named types contained within this API schema.</summary>
    public ApiNamedType[] ApiNamedTypes { get; }

    /// <summary>Gets all API enum types contained within this API schema.</summary>
    public ApiEnumType[] ApiEnumTypes { get; }

    /// <summary>Gets all API object types contained within this API schema.</summary>
    public ApiObjectType[] ApiObjectTypes { get; }

    /// <summary>Gets all API scalar types contained within this API schema.</summary>
    public ApiScalarType[] ApiScalarTypes { get; }

    /// <summary>Gets all API relationships declared within this API schema.</summary>
    public ApiRelationship[] ApiRelationships { get; }

    private Dictionary<string, ApiNamedType> ApiNamedTypeApiNameLookup => this.ThrowIfNotInitialized(_apiNamedTypeApiNameLookup);
    private Dictionary<Type, ApiNamedType> ApiNamedTypeClrTypeLookup => this.ThrowIfNotInitialized(_apiNamedTypeClrTypeLookup);

    private Dictionary<string, ApiEnumType> ApiEnumTypeApiNameLookup => this.ThrowIfNotInitialized(_apiEnumTypeApiNameLookup);
    private Dictionary<Type, ApiEnumType> ApiEnumTypeClrTypeLookup => this.ThrowIfNotInitialized(_apiEnumTypeClrTypeLookup);

    private Dictionary<string, ApiObjectType> ApiObjectTypeApiNameLookup => this.ThrowIfNotInitialized(_apiObjectTypeApiNameLookup);
    private Dictionary<Type, ApiObjectType> ApiObjectTypeClrTypeLookup => this.ThrowIfNotInitialized(_apiObjectTypeClrTypeLookup);

    private Dictionary<string, ApiScalarType> ApiScalarTypeApiNameLookup => this.ThrowIfNotInitialized(_apiScalarTypeApiNameLookup);
    private Dictionary<Type, ApiScalarType> ApiScalarTypeClrTypeLookup => this.ThrowIfNotInitialized(_apiScalarTypeClrTypeLookup);

    private Dictionary<string, ApiRelationship> ApiRelationshipApiNameLookup => this.ThrowIfNotInitialized(_apiRelationshipApiNameLookup);

    private ILogger Logger => this.ApiSchemaContext.Logger;
    #endregion

    #region Constructors
    /// <summary>
    ///     Instantiates a new instance of the <see cref="ApiSchema"/> class using separate collections for scalar, enum, and object types.
    /// </summary>
    /// <param name="apiName">The name of the API schema.</param>
    /// <param name="apiVersion">The optional version of the API schema. Will default to "1.0" if not provided.</param>
    /// <param name="apiOptions">The options used to configure this API schema. If null, the default options are used.</param>
    /// <param name="apiScalarTypes">The collection of scalar types to include in the API schema.</param>
    /// <param name="apiEnumTypes">The collection of enum types to include in the API schema.</param>
    /// <param name="apiObjectTypes">The collection of object types to include in the API schema.</param>
    /// <param name="apiRelationships">The collection of relationships to include in the API schema.</param>
    public ApiSchema
    (
        string apiName,
        string? apiVersion,
        ApiSchemaOptions? apiOptions,
        IEnumerable<ApiScalarType>? apiScalarTypes,
        IEnumerable<ApiEnumType>? apiEnumTypes,
        IEnumerable<ApiObjectType>? apiObjectTypes,
        IEnumerable<ApiRelationship>? apiRelationships
    )
    {
        // Initialize the API name.
        this.ApiName = apiName;

        // Initialize the API version.
        // Default to standard semantic versioning for initial development version.
        this.ApiVersion = apiVersion ?? "0.1.0";

        // Initialize the API schema options.
        this.ApiOptions = apiOptions ?? ApiSchemaOptions.Default;

        // Initialize the collections for API named types, scalar types, enum types, and object types.
        this.ApiScalarTypes = [.. apiScalarTypes.EmptyIfNull().Where(x => x is not null).OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];

        this.ApiEnumTypes = [.. apiEnumTypes.EmptyIfNull().Where(x => x is not null).OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];

        this.ApiObjectTypes = [.. apiObjectTypes.EmptyIfNull().Where(x => x is not null).OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];

        // Initialize the collection of all API named types.
        this.ApiNamedTypes = [.. this.ApiScalarTypes.SafeCast<ApiNamedType>().Concat(this.ApiEnumTypes.SafeCast<ApiNamedType>()).Concat(this.ApiObjectTypes.SafeCast<ApiNamedType>()).OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];

        // Initialize the collection of API relationships.
        this.ApiRelationships = [.. apiRelationships.EmptyIfNull().Where(x => x is not null).OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>
    ///     Instantiates a new instance of the <see cref="ApiSchema"/> class from a collection of API named types.
    /// </summary>
    /// <param name="apiName">The name of the API schema.</param>
    /// <param name="apiVersion">The optional version of the API schema. Will default to "1.0" if not provided.</param>
    /// <param name="apiOptions">The options used to configure this API schema. If null, the default options are used.</param>
    /// <param name="apiNamedTypes">The collection of API named types to include in the API schema.</param>
    /// <param name="apiRelationships">The optional collection of relationships to include in the API schema.</param>
    public ApiSchema
    (
        string apiName,
        string? apiVersion,
        ApiSchemaOptions? apiOptions,
        IEnumerable<ApiNamedType>? apiNamedTypes,
        IEnumerable<ApiRelationship>? apiRelationships
    )
        : this(apiName, apiVersion, apiOptions, apiNamedTypes?.OfType<ApiScalarType>(), apiNamedTypes?.OfType<ApiEnumType>(), apiNamedTypes?.OfType<ApiObjectType>(), apiRelationships)
    { }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var apiVersion = this.ApiVersion.SafeToString();
        var apiOptions = this.ApiOptions.SafeToString();
        var apiScalarTypesCount = this.ApiScalarTypes.Length.SafeToString();
        var apiEnumTypesCount = this.ApiEnumTypes.Length.SafeToString();
        var apiObjectTypesCount = this.ApiObjectTypes.Length.SafeToString();
        var apiRelationshipsCount = this.ApiRelationships.Length.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiSchema)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ApiVersion)}={apiVersion}, {nameof(this.ApiOptions)}={apiOptions}, {nameof(this.ApiScalarTypes)}Count={apiScalarTypesCount}, {nameof(this.ApiEnumTypes)}Count={apiEnumTypesCount}, {nameof(this.ApiObjectTypes)}Count={apiObjectTypesCount}, {nameof(this.ApiRelationships)}Count={apiRelationshipsCount}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchema Methods
    /// <summary>
    ///     Initializes all types and elements within this schema, resolving cross-references and validating
    ///     the entire object graph.
    /// </summary>
    /// <returns>
    ///     An <see cref="ApiInitializationResult"/> that contains any warnings or errors discovered during initialization.
    ///     Call <see cref="ApiInitializationResult.ThrowIfInvalid"/> to convert errors into an exception.
    /// </returns>
    public ApiInitializationResult Initialize()
    {
        // Set runtime/shared context
        _apiSchemaContext = new ApiSchemaContext
        {
            ApiSchema = this
        };

        _apiSchemaContext.InitializeLogger();

        // Set path
        _apiPath = this.BuildPath();

        // Phase 1: Validate schema name and build all lookup dictionaries.
        var context = ApiInitializationContext.CreateRootContext(this);
        this.InitializeApiName(context);
        this.InitializeLookupDictionaries(context);

        // Phase 2: Initialize all type definitions.
        this.InitializeApiScalarTypes(context);
        this.InitializeApiEnumTypes(context);
        this.InitializeApiObjectTypes(context);

        // The remaining initialization phases require fully initialized types,
        // so they come after all types have been initialized.

        // Phase 3: Deferred owner identity resolution
        this.ResolveOwnerIdentityParts(context);

        // Phase 4: Initialize relationships
        this.InitializeApiRelationships(context);

        // Phase 5: Resolve owner key paths declared on relationship dependent ends.
        // This must run after Phase 4 (principal end object types are resolved)
        // but before Phase 6 (which only populates computed collections and does not depend on this).
        this.ResolveOwnerRelationshipKeyPaths(context);

        // Phase 6: Populate computed relationship end collections on each object type.
        this.PopulateRelationshipEndsOnObjectTypes(context);

        var issues = context.Issues;
        return new ApiInitializationResult(issues);
    }

    /// <summary>Attempts to retrieve an API named type by its API name.</summary>
    /// <param name="apiName">The API name to look up.</param>
    /// <param name="apiNamedType">The matching named type, or <see langword="null"/> if not found.</param>
    /// <returns><see langword="true"/> if a named type with the given API name was found; otherwise <see langword="false"/>.</returns>
    public bool TryGetTypeByApiName(string apiName, [NotNullWhen(true)] out ApiNamedType? apiNamedType) => this.ApiNamedTypeApiNameLookup.TryGetValue(apiName, out apiNamedType);

    /// <summary>Attempts to retrieve an API named type by its CLR type.</summary>
    /// <param name="clrType">The CLR type to look up.</param>
    /// <param name="apiNamedType">The matching named type, or <see langword="null"/> if not found.</param>
    /// <returns><see langword="true"/> if a named type matching the CLR type was found; otherwise <see langword="false"/>.</returns>
    public bool TryGetTypeByClrType(Type clrType, [NotNullWhen(true)] out ApiNamedType? apiNamedType) => this.ApiNamedTypeClrTypeLookup.TryGetValue(clrType, out apiNamedType);

    /// <summary>Attempts to retrieve an API enumeration type by its API name.</summary>
    /// <param name="apiName">The API name to look up.</param>
    /// <param name="apiEnumType">The matching enum type, or <see langword="null"/> if not found.</param>
    /// <returns><see langword="true"/> if an enum type with the given API name was found; otherwise <see langword="false"/>.</returns>
    public bool TryGetEnumTypeByApiName(string apiName, [NotNullWhen(true)] out ApiEnumType? apiEnumType) => this.ApiEnumTypeApiNameLookup.TryGetValue(apiName, out apiEnumType);

    /// <summary>Attempts to retrieve an API enumeration type by its CLR type.</summary>
    /// <param name="clrType">The CLR type to look up.</param>
    /// <param name="apiEnumType">The matching enum type, or <see langword="null"/> if not found.</param>
    /// <returns><see langword="true"/> if an enum type matching the CLR type was found; otherwise <see langword="false"/>.</returns>
    public bool TryGetEnumTypeByClrType(Type clrType, [NotNullWhen(true)] out ApiEnumType? apiEnumType) => this.ApiEnumTypeClrTypeLookup.TryGetValue(clrType, out apiEnumType);

    /// <summary>Attempts to retrieve an API object type by its API name.</summary>
    /// <param name="apiName">The API name to look up.</param>
    /// <param name="apiObjectType">The matching object type, or <see langword="null"/> if not found.</param>
    /// <returns><see langword="true"/> if an object type with the given API name was found; otherwise <see langword="false"/>.</returns>
    public bool TryGetObjectTypeByApiName(string apiName, [NotNullWhen(true)] out ApiObjectType? apiObjectType) => this.ApiObjectTypeApiNameLookup.TryGetValue(apiName, out apiObjectType);

    /// <summary>Attempts to retrieve an API object type by its CLR type.</summary>
    /// <param name="clrType">The CLR type to look up.</param>
    /// <param name="apiObjectType">The matching object type, or <see langword="null"/> if not found.</param>
    /// <returns><see langword="true"/> if an object type matching the CLR type was found; otherwise <see langword="false"/>.</returns>
    public bool TryGetObjectTypeByClrType(Type clrType, [NotNullWhen(true)] out ApiObjectType? apiObjectType) => this.ApiObjectTypeClrTypeLookup.TryGetValue(clrType, out apiObjectType);

    /// <summary>Attempts to retrieve an API scalar type by its API name.</summary>
    /// <param name="apiName">The API name to look up.</param>
    /// <param name="apiScalarType">The matching scalar type, or <see langword="null"/> if not found.</param>
    /// <returns><see langword="true"/> if a scalar type with the given API name was found; otherwise <see langword="false"/>.</returns>
    public bool TryGetScalarTypeByApiName(string apiName, [NotNullWhen(true)] out ApiScalarType? apiScalarType) => this.ApiScalarTypeApiNameLookup.TryGetValue(apiName, out apiScalarType);

    /// <summary>Attempts to retrieve an API scalar type by its CLR type.</summary>
    /// <param name="clrType">The CLR type to look up.</param>
    /// <param name="apiScalarType">The matching scalar type, or <see langword="null"/> if not found.</param>
    /// <returns><see langword="true"/> if a scalar type matching the CLR type was found; otherwise <see langword="false"/>.</returns>
    public bool TryGetScalarTypeByClrType(Type clrType, [NotNullWhen(true)] out ApiScalarType? apiScalarType) => this.ApiScalarTypeClrTypeLookup.TryGetValue(clrType, out apiScalarType);

    /// <summary>Attempts to retrieve an API relationship by its API name.</summary>
    /// <param name="apiName">The API name to look up.</param>
    /// <param name="apiRelationship">The matching relationship, or <see langword="null"/> if not found.</param>
    /// <returns><see langword="true"/> if a relationship with the given API name was found; otherwise <see langword="false"/>.</returns>
    public bool TryGetRelationshipByApiName(string apiName, [NotNullWhen(true)] out ApiRelationship? apiRelationship) => this.ApiRelationshipApiNameLookup.TryGetValue(apiName, out apiRelationship);

    private string BuildPath()
        => ApiSchemaHelpers.BuildPath(null, segment: nameof(ApiSchema), segmentName: this.ApiName);
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates a new <see cref="ApiSchema"/> from a flat collection of named types, initializes it, and throws on validation errors.
    /// </summary>
    /// <param name="apiName">The name of the API schema.</param>
    /// <param name="apiNamedTypes">The collection of named types (scalar, enum, and object) to include in the schema.</param>
    /// <param name="apiVersion">The optional version string. Defaults to <c>"0.1.0"</c> if not provided.</param>
    /// <param name="apiOptions">The optional schema options. Defaults to <see cref="ApiSchemaOptions.Default"/> if not provided.</param>
    /// <param name="apiRelationships">The optional collection of relationships to include in the schema.</param>
    /// <param name="extensionTypeAndInstances">Optional extension instances to attach to the schema after construction.</param>
    /// <returns>A fully initialized <see cref="ApiSchema"/> instance.</returns>
    /// <exception cref="ApiSchemaInitializationException">Thrown if initialization produces one or more errors.</exception>
    public static ApiSchema Create
    (
        string apiName,
        IEnumerable<ApiNamedType>? apiNamedTypes,
        string? apiVersion = null,
        ApiSchemaOptions? apiOptions = null,
        IEnumerable<ApiRelationship>? apiRelationships = null,
        IEnumerable<(Type ExtensionType, object ExtensionInstance)>? extensionTypeAndInstances = null
    )
    {
        var apiSchema = new ApiSchema(apiName, apiVersion, apiOptions, apiNamedTypes, apiRelationships);

        if (extensionTypeAndInstances is not null)
        {
            foreach (var (extensionType, extensionInstance) in extensionTypeAndInstances)
            {
                apiSchema.AttachExtension(extensionType, extensionInstance);
            }
        }

        var result = apiSchema.Initialize();
        result.ThrowIfInvalid();

        return apiSchema;
    }

    /// <summary>
    ///     Creates a new <see cref="ApiSchema"/> from separate scalar, enum, and object type collections, initializes it, and throws on validation errors.
    /// </summary>
    /// <param name="apiName">The name of the API schema.</param>
    /// <param name="apiScalarTypes">The collection of scalar types to include in the schema.</param>
    /// <param name="apiEnumTypes">The collection of enum types to include in the schema.</param>
    /// <param name="apiObjectTypes">The collection of object types to include in the schema.</param>
    /// <param name="apiVersion">The optional version string. Defaults to <c>"0.1.0"</c> if not provided.</param>
    /// <param name="apiOptions">The optional schema options. Defaults to <see cref="ApiSchemaOptions.Default"/> if not provided.</param>
    /// <param name="apiRelationships">The collection of relationships to include in the schema.</param>
    /// <param name="extensionTypeAndInstances">Optional extension instances to attach to the schema after construction.</param>
    /// <returns>A fully initialized <see cref="ApiSchema"/> instance.</returns>
    /// <exception cref="ApiSchemaInitializationException">Thrown if initialization produces one or more errors.</exception>
    public static ApiSchema Create
    (
        string apiName,
        IEnumerable<ApiScalarType>? apiScalarTypes,
        IEnumerable<ApiEnumType>? apiEnumTypes,
        IEnumerable<ApiObjectType>? apiObjectTypes,
        string? apiVersion = null,
        ApiSchemaOptions? apiOptions = null,
        IEnumerable<ApiRelationship>? apiRelationships = null,
        IEnumerable<(Type ExtensionType, object ExtensionInstance)>? extensionTypeAndInstances = null
    )
    {
        var apiSchema = new ApiSchema(apiName, apiVersion, apiOptions, apiScalarTypes, apiEnumTypes, apiObjectTypes, apiRelationships);

        if (extensionTypeAndInstances is not null)
        {
            foreach (var (extensionType, extensionInstance) in extensionTypeAndInstances)
            {
                apiSchema.AttachExtension(extensionType, extensionInstance);
            }
        }

        var result = apiSchema.Initialize();
        result.ThrowIfInvalid();

        return apiSchema;
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiName(ApiInitializationContext context)
    {
        var isApiNameInvalid = ApiSchemaHelpers.IsNameInvalid(this.ApiName);
        if (isApiNameInvalid)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_SCHEMA_INVALID_NAME;
            var description = $"{nameof(this.ApiName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeApiEnumTypes(ApiInitializationContext context)
    {
        foreach (var apiEnumType in this.ApiEnumTypes)
        {
            apiEnumType.Initialize(context);
        }
    }

    private void InitializeApiObjectTypes(ApiInitializationContext context)
    {
        foreach (var apiObjectType in this.ApiObjectTypes)
        {
            apiObjectType.Initialize(context);
        }
    }

    private void InitializeApiRelationships(ApiInitializationContext context)
    {
        foreach (var apiRelationship in this.ApiRelationships)
        {
            apiRelationship.Initialize(context);
        }
    }

    private void ResolveOwnerRelationshipKeyPaths(ApiInitializationContext context)
    {
        foreach (var relationship in this.ApiRelationships)
        {
            switch (relationship)
            {
                case ApiRelationshipOneTo oneTo:
                    this.ResolveOwnerKeyPathsForPrincipal(oneTo.ApiPrincipalEnd, oneTo.ApiDependentEnd?.ApiKeyPaths, context);
                    break;

                case ApiRelationshipManyToMany manyToMany:
                    this.ResolveOwnerKeyPathsForPrincipal(manyToMany.ApiPrincipalEndA, manyToMany.ApiAssociation?.ApiKeyPathsA, context);
                    this.ResolveOwnerKeyPathsForPrincipal(manyToMany.ApiPrincipalEndB, manyToMany.ApiAssociation?.ApiKeyPathsB, context);
                    break;
            }
        }
    }

    private void ResolveOwnerKeyPathsForPrincipal
    (
        ApiRelationshipPrincipalEnd? principal,
        ApiRelationshipKeyPath[]? keyPaths,
        ApiInitializationContext context
    )
    {
        if (principal is null)
        {
            return;
        }

        if (principal.ClrObjectType is null)
        {
            return;
        }

        // If the principal's object type didn't resolve (error already recorded in Phase 4), skip.
        if (!this.TryGetObjectTypeByClrType(principal.ClrObjectType, out var principalObjectType))
        {
            return;
        }

        if (keyPaths is null || keyPaths.Length == 0)
        {
            return;
        }

        WalkKeyPathsForOwnerPaths(keyPaths, principalObjectType, context);
    }

    private static void WalkKeyPathsForOwnerPaths
    (
        IEnumerable<ApiRelationshipKeyPath> keyPaths,
        ApiObjectType principalObjectType,
        ApiInitializationContext context
    )
    {
        foreach (var keyPath in keyPaths)
        {
            switch (keyPath)
            {
                case ApiRelationshipOwnerKeyPath ownerKeyPath:
                    ownerKeyPath.ResolveOwnerType(principalObjectType, context);
                    break;

                case ApiRelationshipNestedKeyPath nestedKeyPath when nestedKeyPath.ApiKeyPaths.Length > 0:
                    // The owner is always the principal end regardless of nesting depth — recurse with the same principalObjectType rather than the nested object's type.
                    WalkKeyPathsForOwnerPaths(nestedKeyPath.ApiKeyPaths, principalObjectType, context);
                    break;
            }
        }
    }

    private void InitializeApiScalarTypes(ApiInitializationContext context)
    {
        foreach (var apiScalarType in this.ApiScalarTypes)
        {
            apiScalarType.Initialize(context);
        }
    }

    private void InitializeLookupDictionaries(ApiInitializationContext context)
    {
        // Initialize lookup dictionaries for lookup by API name and CLR type.
        _apiNamedTypeApiNameLookup = null;
        _apiNamedTypeClrTypeLookup = null;

        _apiEnumTypeApiNameLookup = null;
        _apiEnumTypeClrTypeLookup = null;

        _apiObjectTypeApiNameLookup = null;
        _apiObjectTypeClrTypeLookup = null;

        _apiScalarTypeApiNameLookup = null;
        _apiScalarTypeClrTypeLookup = null;

        _apiRelationshipApiNameLookup = null;

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiNamedTypes,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaHelpers.IsNameValid(x),
            partKeyPropertyName: nameof(ApiNamedType.ApiName),
            path: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.API_SCHEMA_DUPLICATE_NAMED_TYPE_API_NAME,
            context: context,
            lookupDictionary: out _apiNamedTypeApiNameLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiNamedTypes,
            partKeySelector: x => x.ClrType,
            partKeyFilter: x => x is not null,
            partKeyPropertyName: nameof(ApiNamedType.ClrType),
            path: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.API_SCHEMA_DUPLICATE_NAMED_TYPE_CLR_TYPE,
            context: context,
            lookupDictionary: out _apiNamedTypeClrTypeLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiEnumTypes,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaHelpers.IsNameValid(x),
            partKeyPropertyName: nameof(ApiEnumType.ApiName),
            path: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.API_SCHEMA_DUPLICATE_ENUM_TYPE_API_NAME,
            context: context,
            lookupDictionary: out _apiEnumTypeApiNameLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiEnumTypes,
            partKeySelector: x => x.ClrType,
            partKeyFilter: x => x is not null,
            partKeyPropertyName: nameof(ApiEnumType.ClrType),
            path: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.API_SCHEMA_DUPLICATE_ENUM_TYPE_CLR_TYPE,
            context: context,
            lookupDictionary: out _apiEnumTypeClrTypeLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiObjectTypes,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaHelpers.IsNameValid(x),
            partKeyPropertyName: nameof(ApiObjectType.ApiName),
            path: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.API_SCHEMA_DUPLICATE_OBJECT_TYPE_API_NAME,
            context: context,
            lookupDictionary: out _apiObjectTypeApiNameLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiObjectTypes,
            partKeySelector: x => x.ClrType,
            partKeyFilter: x => x is not null,
            partKeyPropertyName: nameof(ApiObjectType.ClrType),
            path: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.API_SCHEMA_DUPLICATE_OBJECT_TYPE_CLR_TYPE,
            context: context,
            lookupDictionary: out _apiObjectTypeClrTypeLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiScalarTypes,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaHelpers.IsNameValid(x),
            partKeyPropertyName: nameof(ApiScalarType.ApiName),
            path: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.API_SCHEMA_DUPLICATE_SCALAR_TYPE_API_NAME,
            context: context,
            lookupDictionary: out _apiScalarTypeApiNameLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiScalarTypes,
            partKeySelector: x => x.ClrType,
            partKeyFilter: x => x is not null,
            partKeyPropertyName: nameof(ApiScalarType.ClrType),
            path: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.API_SCHEMA_DUPLICATE_SCALAR_TYPE_CLR_TYPE,
            context: context,
            lookupDictionary: out _apiScalarTypeClrTypeLookup
        );

        ApiSchemaHelpers.InitializeLookupDictionary
        (
            parts: this.ApiRelationships,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaHelpers.IsNameValid(x),
            partKeyPropertyName: nameof(ApiRelationship.ApiName),
            path: this.ApiPath,
            duplicatePartCode: ApiInitializationCode.API_SCHEMA_DUPLICATE_RELATIONSHIP_API_NAME,
            context: context,
            lookupDictionary: out _apiRelationshipApiNameLookup
        );
    }

    private void PopulateRelationshipEndsOnObjectTypes(ApiInitializationContext context)
    {
        // Clear any previously-registered ends to support idempotent re-initialization.
        foreach (var apiObjectType in this.ApiObjectTypes)
        {
            apiObjectType.ClearRelationshipEnds();
        }

        foreach (var relationship in this.ApiRelationships)
        {
            switch (relationship)
            {
                case ApiRelationshipOneTo oneTo:
                    TryRegisterEnd(oneTo.ApiPrincipalEnd);
                    TryRegisterEnd(oneTo.ApiDependentEnd);
                    break;

                case ApiRelationshipManyToMany manyToMany:
                    TryRegisterEnd(manyToMany.ApiPrincipalEndA);
                    TryRegisterEnd(manyToMany.ApiPrincipalEndB);
                    break;
            }
        }

        void TryRegisterEnd(ApiRelationshipEnd end)
        {
            var clrObjectType = end.ClrObjectType;
            if (!this.TryGetObjectTypeByClrType(clrObjectType, out var apiObjectType))
            {
                // Already reported as API_RELATIONSHIP_END_UNRESOLVED_OBJECT_TYPE (Error) in Phase 4.
                return;
            }

            apiObjectType.AddRelationshipEnd(end);
        }
    }

    private void ResolveOwnerIdentityParts(ApiInitializationContext context)
    {
        // Pre-compute the set of object types that have at least one ApiIdentityOwnerPart.
        // A type with no identities at all cannot contain an owner part, so HasIdentity is a cheap pre-filter.
        var typesWithOwnerPart = this.ApiObjectTypes
            .Where(t => t.HasIdentity && t.ApiIdentities.Any(i => i.ApiIdentityParts.Any(p => p is ApiIdentityOwnerPart)))
            .ToHashSet();

        // Fast path: no type in the schema uses owner-based identity — skip all work.
        if (typesWithOwnerPart.Count == 0)
        {
            return;
        }

        // Build a reverse lookup:
        // For each owned type C, collect every object type P that owns C either via:
        // 1. A collection property (1-to-many)
        // 2. A direct object property (1-to-1)
        //
        // Only insert entries for owned types that actually need resolution.
        var ownerLookup = new Dictionary<ApiObjectType, List<ApiObjectType>>();
        foreach (var ownerType in this.ApiObjectTypes)
        {
            foreach (var property in ownerType.ApiProperties)
            {
                // Skip properties whose type expression failed to resolve during phase 1 initialization.
                if (!property.IsResolved)
                {
                    continue;
                }

                // 1-to-many: Owner has a collection property whose item type is the owned object type.
                var collectionType = property.ApiType as ApiCollectionType;
                if (collectionType is not null)
                {
                    var collectionOwnedType = collectionType.ApiItemType as ApiObjectType;
                    if (collectionOwnedType is not null && typesWithOwnerPart.Contains(collectionOwnedType))
                    {
                        AddOwnerCandidate(ownerLookup, collectionOwnedType, ownerType);
                    }
                }
                // 1-to-1: Owner has a direct object property typed as the owned object type.
                var directOwnedType = property.ApiType as ApiObjectType;
                if (directOwnedType is not null && typesWithOwnerPart.Contains(directOwnedType))
                {
                    AddOwnerCandidate(ownerLookup, directOwnedType, ownerType);
                }
            }
        }

        static void AddOwnerCandidate(Dictionary<ApiObjectType, List<ApiObjectType>> lookup, ApiObjectType ownedType, ApiObjectType ownerType)
        {
            if (!lookup.TryGetValue(ownedType, out var owners))
            {
                owners = [];
                lookup[ownedType] = owners;
            }

            owners.Add(ownerType);
        }

        // Detect ownership cycles: Find object types whose single-candidate ownership chain forms a loop.
        //
        // A cycle means two or more types transitively own each other, making identity composition impossible.
        var cycleMembers = new HashSet<ApiObjectType>();
        var checkedTypes = new HashSet<ApiObjectType>();

        foreach (var startType in typesWithOwnerPart)
        {
            if (checkedTypes.Contains(startType))
            {
                continue;
            }

            var chain = new List<ApiObjectType>();
            var chainSet = new HashSet<ApiObjectType>();
            var current = startType;

            while (current != null && typesWithOwnerPart.Contains(current) && !checkedTypes.Contains(current))
            {
                if (chainSet.Contains(current))
                {
                    // Cycle found: mark all types in the cycle (from the re-entry point onward) as members.
                    var cycleStart = current;
                    var inCycle = false;
                    foreach (var t in chain)
                    {
                        if (t == cycleStart)
                        {
                            inCycle = true;
                        }

                        if (inCycle)
                        {
                            cycleMembers.Add(t);
                        }
                    }
                    break;
                }

                chainSet.Add(current);
                chain.Add(current);

                ownerLookup.TryGetValue(current, out var singleOwner);
                current = singleOwner?.Count == 1 ? singleOwner[0] : null;
            }

            foreach (var t in chain)
            {
                checkedTypes.Add(t);
            }
        }

        // Report errors for all cycle members.
        foreach (var cycleType in cycleMembers)
        {
            foreach (var identity in cycleType.ApiIdentities)
            {
                foreach (var part in identity.ApiIdentityParts.OfType<ApiIdentityOwnerPart>())
                {
                    var path = part.ApiPath;
                    var severity = ApiInitializationSeverity.Error;
                    var code = ApiInitializationCode.API_IDENTITY_PART_CYCLIC_OWNER;
                    var description = $"A cyclic owner identity reference was detected involving '{cycleType.ApiName}'";
                    var remediation = $"Remove the cyclic {nameof(ApiIdentityOwnerPart)} reference";

                    context.AddIssue(path, severity, code, description, remediation);
                }
            }
        }

        // Propagate tainted status to types whose single resolved owner is itself a cycle member (or transitively tainted).
        //
        // Such types cannot be safely resolved because their owner identity is partially initialized,
        // which would cause a runtime throw without a prior diagnostic.
        var taintedTypes = new HashSet<ApiObjectType>(cycleMembers);
        bool anyNewTainted;
        do
        {
            anyNewTainted = false;
            foreach (var candidate in typesWithOwnerPart)
            {
                if (taintedTypes.Contains(candidate))
                {
                    continue;
                }

                ownerLookup.TryGetValue(candidate, out var candidateOwners);
                if (candidateOwners?.Count == 1 && taintedTypes.Contains(candidateOwners[0]))
                {
                    taintedTypes.Add(candidate);
                    anyNewTainted = true;
                }
            }
        } while (anyNewTainted);

        // Report errors for transitively tainted types (non-cycle members whose single owner is in or leads to a cycle).
        foreach (var taintedType in taintedTypes)
        {
            if (cycleMembers.Contains(taintedType))
            {
                continue;
            }

            ownerLookup.TryGetValue(taintedType, out var taintedOwners);
            var ownerName = taintedOwners?.Count == 1 ? taintedOwners[0].ApiName : "an unknown type";

            foreach (var identity in taintedType.ApiIdentities)
            {
                foreach (var part in identity.ApiIdentityParts.OfType<ApiIdentityOwnerPart>())
                {
                    var path = part.ApiPath;
                    var severity = ApiInitializationSeverity.Error;
                    var code = ApiInitializationCode.API_IDENTITY_PART_CYCLIC_OWNER;
                    var description = $"Owner type '{ownerName}' of '{taintedType.ApiName}' is involved in a cyclic owner identity reference";
                    var remediation = $"Remove the cyclic {nameof(ApiIdentityOwnerPart)} reference";

                    context.AddIssue(path, severity, code, description, remediation);
                }
            }
        }

        // Walk only owned types that need resolution,
        // skipping any that are tainted (direct cycle members or types that transitively depend on a cycle member).
        foreach (var ownedType in typesWithOwnerPart)
        {
            if (taintedTypes.Contains(ownedType))
            {
                continue;
            }

            foreach (var identity in ownedType.ApiIdentities)
            {
                foreach (var ownerPart in identity.ApiIdentityParts.OfType<ApiIdentityOwnerPart>())
                {
                    ownerLookup.TryGetValue(ownedType, out var candidateOwners);

                    var partContext = context
                        .WithDeclaringObjectType(ownedType)
                        .WithDeclaringSchemaElement(identity);

                    ownerPart.ResolveOwnerIdentity(ownedType, candidateOwners ?? [], partContext);
                }
            }
        }
    }
    #endregion
}
