// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;
using Evoogle.NTree;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the root schema element and the collection of <see cref="ApiType"/> instances
///     making up the schema.
/// </summary>
/// <remarks>
///     Instances are produced only by a successful builder compilation or root JSON
///     deserialization. A returned instance and all framework-owned descendants are frozen and
///     safe for concurrent lookup, traversal, relationship and key resolution, property access,
///     extension lookup, and serialization. Build a separate schema to replace runtime metadata.
/// </remarks>
[JsonConverter(typeof(ApiSchemaJsonConverter))]
public sealed class ApiSchema : ApiSchemaElement
{
    #region ApiSchema Fields
    private FrozenDictionary<string, ApiNamedType>? _apiNamedTypeApiNameLookup = null;
    private FrozenDictionary<Type, ApiNamedType>? _apiNamedTypeClrTypeLookup = null;

    private FrozenDictionary<string, ApiEnumType>? _apiEnumTypeApiNameLookup = null;
    private FrozenDictionary<Type, ApiEnumType>? _apiEnumTypeClrTypeLookup = null;

    private FrozenDictionary<string, ApiObjectType>? _apiObjectTypeApiNameLookup = null;
    private FrozenDictionary<Type, ApiObjectType>? _apiObjectTypeClrTypeLookup = null;

    private FrozenDictionary<string, ApiScalarType>? _apiScalarTypeApiNameLookup = null;
    private FrozenDictionary<Type, ApiScalarType>? _apiScalarTypeClrTypeLookup = null;

    private FrozenDictionary<string, ApiRelationship>? _apiRelationshipApiNameLookup = null;

    private int _compilationState;
    #endregion

    #region ApiSchema Properties
    /// <summary>Gets the name of the API schema.</summary>
    public string ApiName { get; }

    /// <summary>Gets the version of the API schema.</summary>
    public string ApiVersion { get; }

    /// <summary>Gets the options used to configure this API schema.</summary>
    public ApiSchemaOptions ApiOptions { get; }

    /// <summary>Gets the immutable snapshot of named types contained within this schema.</summary>
    public ImmutableArray<ApiNamedType> ApiNamedTypes { get; }

    /// <summary>Gets the immutable snapshot of enum types contained within this schema.</summary>
    public ImmutableArray<ApiEnumType> ApiEnumTypes { get; }

    /// <summary>Gets the immutable snapshot of object types contained within this schema.</summary>
    public ImmutableArray<ApiObjectType> ApiObjectTypes { get; }

    /// <summary>Gets the immutable snapshot of scalar types contained within this schema.</summary>
    public ImmutableArray<ApiScalarType> ApiScalarTypes { get; }

    /// <summary>Gets the immutable snapshot of relationships declared within this schema.</summary>
    public ImmutableArray<ApiRelationship> ApiRelationships { get; }

    private FrozenDictionary<string, ApiNamedType> ApiNamedTypeApiNameLookup => this.RequireValue(_apiNamedTypeApiNameLookup);
    private FrozenDictionary<Type, ApiNamedType> ApiNamedTypeClrTypeLookup => this.RequireValue(_apiNamedTypeClrTypeLookup);

    private FrozenDictionary<string, ApiEnumType> ApiEnumTypeApiNameLookup => this.RequireValue(_apiEnumTypeApiNameLookup);
    private FrozenDictionary<Type, ApiEnumType> ApiEnumTypeClrTypeLookup => this.RequireValue(_apiEnumTypeClrTypeLookup);

    private FrozenDictionary<string, ApiObjectType> ApiObjectTypeApiNameLookup => this.RequireValue(_apiObjectTypeApiNameLookup);
    private FrozenDictionary<Type, ApiObjectType> ApiObjectTypeClrTypeLookup => this.RequireValue(_apiObjectTypeClrTypeLookup);

    private FrozenDictionary<string, ApiScalarType> ApiScalarTypeApiNameLookup => this.RequireValue(_apiScalarTypeApiNameLookup);
    private FrozenDictionary<Type, ApiScalarType> ApiScalarTypeClrTypeLookup => this.RequireValue(_apiScalarTypeClrTypeLookup);

    private FrozenDictionary<string, ApiRelationship> ApiRelationshipApiNameLookup => this.RequireValue(_apiRelationshipApiNameLookup);

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
    internal ApiSchema
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
        // Compile the API name.
        this.ApiName = apiName;

        // Compile the API version.
        // Default to standard semantic versioning for initial development version.
        this.ApiVersion = apiVersion ?? "0.1.0";

        // Compile the API schema options.
        this.ApiOptions = apiOptions ?? ApiSchemaOptions.Default;

        // Compile the collections for API named types, scalar types, enum types, and object types.
        this.ApiScalarTypes = [.. apiScalarTypes.EmptyIfNull().Where(x => x is not null).OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];

        this.ApiEnumTypes = [.. apiEnumTypes.EmptyIfNull().Where(x => x is not null).OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];

        this.ApiObjectTypes = [.. apiObjectTypes.EmptyIfNull().Where(x => x is not null).OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];

        // Compile the collection of all API named types.
        this.ApiNamedTypes = [.. this.ApiScalarTypes.SafeCast<ApiNamedType>().Concat(this.ApiEnumTypes.SafeCast<ApiNamedType>()).Concat(this.ApiObjectTypes.SafeCast<ApiNamedType>()).OrderBy(x => x.ApiName, StringComparer.OrdinalIgnoreCase)];

        // Compile the collection of API relationships.
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
    internal ApiSchema
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

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override ApiSchemaElementKind Kind => ApiSchemaElementKind.Schema;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiSchema);
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

    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
    {
        return ApiSchemaPathFormatting.BuildPath
        (
            apiBasePath: null,
            apiPathSegment: this.ApiElementName,
            apiPathSegmentName: this.ApiName
        );
    }

    /// <inheritdoc/>
    internal override IEnumerable<ApiSchemaElement> GetOwnedElements()
    {
        foreach (var apiScalarType in this.ApiScalarTypes)
        {
            yield return apiScalarType;
        }

        foreach (var apiEnumType in this.ApiEnumTypes)
        {
            yield return apiEnumType;
        }

        foreach (var apiObjectType in this.ApiObjectTypes)
        {
            yield return apiObjectType;
        }

        foreach (var apiRelationship in this.ApiRelationships)
        {
            yield return apiRelationship;
        }
    }

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        // Phase 1: Validate schema name and build all lookup dictionaries.
        this.ValidateApiName(context);
        this.ValidateApiOptions(context);
        this.BuildLookupDictionaries(context);

        // Phase 2: Compile all type definitions.
        this.CompileApiScalarTypes(context);
        this.CompileApiEnumTypes(context);
        this.CompileApiObjectTypes(context);

        // Phase 3: Compile key types after all type definitions are available.
        this.CompileApiKeyTypes(context);

        // Phase 4: Compile relationships.
        this.CompileApiRelationships(context);
    }
    #endregion

    #region Implementation Methods
    internal void BeginCompilation()
    {
        if (Interlocked.CompareExchange(ref _compilationState, 1, 0) != 0)
        {
            throw new InvalidOperationException("An API schema graph can only be compiled once.");
        }
    }

    internal void CompleteCompilation(bool isSuccessful)
    {
        Interlocked.Exchange(ref _compilationState, isSuccessful ? 2 : 3);
    }

    private void ValidateApiName(ApiSchemaCompilationContext context)
    {
        var isApiNameInvalid = ApiSchemaNameValidation.IsNameInvalid(this.ApiName);
        if (isApiNameInvalid)
        {
            var path = this.ApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiSchemaInvalidName;
            var description = $"{nameof(this.ApiName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void ValidateApiOptions(ApiSchemaCompilationContext context)
    {
        if (!this.ApiOptions.HasInvalidApiKeyNullHandling)
        {
            return;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiSchemaInvalidApiKeyNullHandling;
        var description = $"{nameof(this.ApiOptions)}.{nameof(ApiSchemaOptions.ApiKeyNullHandling)} must be a valid {nameof(ApiKeyNullHandling)} value";
        var remediation = $"Specify a valid {nameof(ApiSchemaOptions.ApiKeyNullHandling)} value";

        context.AddIssue(severity, code, description, remediation);
    }

    private void CompileApiEnumTypes(ApiSchemaCompilationContext context)
    {
        foreach (var apiEnumType in this.ApiEnumTypes)
        {
            apiEnumType.Compile(context);
        }
    }

    private void CompileApiObjectTypes(ApiSchemaCompilationContext context)
    {
        foreach (var apiObjectType in this.ApiObjectTypes)
        {
            apiObjectType.Compile(context);
        }
    }

    private void CompileApiKeyTypes(ApiSchemaCompilationContext context)
    {
        foreach (var apiObjectType in this
            .SelfAndDescendants(TraversalStrategy.DepthFirst)
            .OfType<ApiObjectType>())
        {
            apiObjectType.CompileKeyTypes(context);
        }
    }

    private void CompileApiRelationships(ApiSchemaCompilationContext context)
    {
        foreach (var apiRelationship in this.ApiRelationships)
        {
            apiRelationship.Compile(context);
        }

        this.PopulateRelationshipCrossReferences();
    }

    private void CompileApiScalarTypes(ApiSchemaCompilationContext context)
    {
        foreach (var apiScalarType in this.ApiScalarTypes)
        {
            apiScalarType.Compile(context);
        }
    }

    private void BuildLookupDictionaries(ApiSchemaCompilationContext context)
    {
        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: this.ApiNamedTypes,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaNameValidation.IsNameValid(x),
            partKeyPropertyName: nameof(ApiNamedType.ApiName),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiSchemaDuplicateNamedTypeApiName,
            session: context.Session,
            lookupDictionary: out _apiNamedTypeApiNameLookup
        );

        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: this.ApiNamedTypes,
            partKeySelector: x => x.ClrType,
            partKeyFilter: x => x is not null,
            partKeyPropertyName: nameof(ApiNamedType.ClrType),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiSchemaDuplicateNamedTypeClrType,
            session: context.Session,
            lookupDictionary: out _apiNamedTypeClrTypeLookup
        );

        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: this.ApiEnumTypes,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaNameValidation.IsNameValid(x),
            partKeyPropertyName: nameof(ApiEnumType.ApiName),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiSchemaDuplicateEnumTypeApiName,
            session: context.Session,
            lookupDictionary: out _apiEnumTypeApiNameLookup
        );

        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: this.ApiEnumTypes,
            partKeySelector: x => x.ClrType,
            partKeyFilter: x => x is not null,
            partKeyPropertyName: nameof(ApiEnumType.ClrType),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiSchemaDuplicateEnumTypeClrType,
            session: context.Session,
            lookupDictionary: out _apiEnumTypeClrTypeLookup
        );

        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: this.ApiObjectTypes,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaNameValidation.IsNameValid(x),
            partKeyPropertyName: nameof(ApiObjectType.ApiName),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiSchemaDuplicateObjectTypeApiName,
            session: context.Session,
            lookupDictionary: out _apiObjectTypeApiNameLookup
        );

        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: this.ApiObjectTypes,
            partKeySelector: x => x.ClrType,
            partKeyFilter: x => x is not null,
            partKeyPropertyName: nameof(ApiObjectType.ClrType),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiSchemaDuplicateObjectTypeClrType,
            session: context.Session,
            lookupDictionary: out _apiObjectTypeClrTypeLookup
        );

        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: this.ApiScalarTypes,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaNameValidation.IsNameValid(x),
            partKeyPropertyName: nameof(ApiScalarType.ApiName),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiSchemaDuplicateScalarTypeApiName,
            session: context.Session,
            lookupDictionary: out _apiScalarTypeApiNameLookup
        );

        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: this.ApiScalarTypes,
            partKeySelector: x => x.ClrType,
            partKeyFilter: x => x is not null,
            partKeyPropertyName: nameof(ApiScalarType.ClrType),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiSchemaDuplicateScalarTypeClrType,
            session: context.Session,
            lookupDictionary: out _apiScalarTypeClrTypeLookup
        );

        ApiSchemaCompilationLookup.BuildLookupDictionary
        (
            parts: this.ApiRelationships,
            partKeySelector: x => x.ApiName,
            partKeyFilter: x => ApiSchemaNameValidation.IsNameValid(x),
            partKeyPropertyName: nameof(ApiRelationship.ApiName),
            apiPath: this.ApiPath,
            duplicatePartCode: ApiSchemaCompilationCode.ApiSchemaDuplicateRelationshipApiName,
            session: context.Session,
            lookupDictionary: out _apiRelationshipApiNameLookup
        );
    }

    private void PopulateRelationshipCrossReferences()
    {
        // Collect phase: bucket ends and associations by target object type using lists (no reallocation per insertion).
        var endMap = new Dictionary<ApiObjectType, (List<ApiRelationshipEnd> All, List<ApiRelationshipPrincipalEnd> Principal, List<ApiRelationshipDependentEnd> Dependent)>();
        var associationMap = new Dictionary<ApiObjectType, List<ApiRelationshipAssociation>>();

        foreach (var relationship in this.ApiRelationships)
        {
            switch (relationship)
            {
                case ApiRelationshipOneTo oneTo:
                    Collect(oneTo.ApiPrincipalEnd);
                    Collect(oneTo.ApiDependentEnd);
                    break;

                case ApiRelationshipManyToMany manyToMany:
                    Collect(manyToMany.ApiPrincipalEndA);
                    Collect(manyToMany.ApiPrincipalEndB);
                    CollectAssociation(manyToMany.ApiAssociation);
                    break;
            }
        }

        // Apply phase: deliver complete immutable values to each object type.
        // Types not present in a map receive empty immutable values.
        foreach (var apiObjectType in this.ApiObjectTypes)
        {
            if (endMap.TryGetValue(apiObjectType, out var lists))
            {
                apiObjectType.SetRelationshipEnds([.. lists.All], [.. lists.Principal], [.. lists.Dependent]);
            }
            else
            {
                apiObjectType.ClearRelationshipEnds();
            }

            if (associationMap.TryGetValue(apiObjectType, out var associations))
            {
                apiObjectType.SetRelationshipAssociations([.. associations]);
            }
            else
            {
                apiObjectType.SetRelationshipAssociations([]);
            }
        }

        void Collect(ApiRelationshipEnd? end)
        {
            if (end is null)
            {
                // Already reported as ApiRelationshipNullPrincipalEnd or ApiRelationshipNullDependentEnd (Error).
                return;
            }

            if (end.ClrObjectType is null || !this.TryGetObjectTypeByClrType(end.ClrObjectType, out var apiObjectType))
            {
                // Already reported as ApiRelationshipElementNullClrObjectType or ApiRelationshipElementUnresolvedObjectType (Error).
                return;
            }

            if (!endMap.TryGetValue(apiObjectType, out var lists))
            {
                lists = ([], [], []);
                endMap[apiObjectType] = lists;
            }

            lists.All.Add(end);
            if (end is ApiRelationshipPrincipalEnd principalEnd)
            {
                lists.Principal.Add(principalEnd);
            }
            else if (end is ApiRelationshipDependentEnd dependentEnd)
            {
                lists.Dependent.Add(dependentEnd);
            }
        }

        void CollectAssociation(ApiRelationshipAssociation? association)
        {
            if (association is null)
            {
                return;
            }

            var clrObjectType = association.ClrObjectType;
            if (clrObjectType is null)
            {
                // Already reported as ApiRelationshipElementNullClrObjectType (Error).
                return;
            }

            if (!this.TryGetObjectTypeByClrType(clrObjectType, out var apiObjectType))
            {
                // Already reported as ApiRelationshipElementUnresolvedObjectType (Error).
                return;
            }

            if (!associationMap.TryGetValue(apiObjectType, out var list))
            {
                list = [];
                associationMap[apiObjectType] = list;
            }

            list.Add(association);
        }
    }
    #endregion
}
