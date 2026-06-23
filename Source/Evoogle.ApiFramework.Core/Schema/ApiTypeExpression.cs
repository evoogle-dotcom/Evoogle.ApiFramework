// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a type reference in an API schema. This can either be:
///     - An API inline type, such as a collection (<see cref="ApiInlineType"/> set).
///     - An API named reference to an API type (<see cref="ApiKind"/> and <see cref="ApiName"/> set), or
///     - A CLR type reference to an API type (<see cref="ClrType"/> set), or
/// </summary>
[JsonConverter(typeof(ApiTypeExpressionJsonConverter))]
public sealed class ApiTypeExpression
{
    #region Fields
    private ApiType? _apiResolvedType = null;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the kind of the referenced API type (only used for API named references).
    /// </summary>
    public ApiTypeKind? ApiKind { get; }

    /// <summary>
    ///     Gets the API name of the referenced type (only used for API named references).
    /// </summary>
    public string? ApiName { get; }

    /// <summary>
    ///     Gets the CLR type of the referenced type (only used for CLR references).
    /// </summary>
    public Type? ClrType { get; }

    /// <summary>
    ///     Gets the inline type definition, if any.
    ///     This is typically used for inline collection types.
    /// </summary>
    public ApiType? ApiInlineType { get; }

    /// <summary>
    ///     Gets the resolved <see cref="ApiType"/> this expression refers to, either inline or named reference.
    /// </summary>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if the expression has not been resolved yet.
    /// </exception>
    public ApiType ApiType => this.ThrowIfNotInitialized(_apiResolvedType);
    #endregion

    #region Computed Properties
    /// <summary>
    ///     Gets a value indicating whether this is an inline type (i.e., <see cref="ApiInlineType"/> is not null).
    /// </summary>
    public bool IsInline => this.ApiInlineType is not null;

    /// <summary>
    ///     Gets a value indicating whether this is either an API named reference or a CLR type reference (i.e., <see cref="ApiInlineType"/> is null).
    /// </summary>
    public bool IsReference => this.IsApiNamedReference || this.IsClrTypeReference;

    /// <summary>
    ///     Gets a value indicating whether this is an API named reference (i.e., <see cref="ApiInlineType"/> is null).
    /// </summary>
    public bool IsApiNamedReference => this.ApiInlineType is null && this.ApiKind is not null && !string.IsNullOrWhiteSpace(this.ApiName);

    /// <summary>
    ///     Gets a value indicating whether this is a CLR reference (i.e., <see cref="ApiInlineType"/> is null).
    /// </summary>
    public bool IsClrTypeReference => this.ApiInlineType is null && this.ClrType is not null;

    /// <summary>
    ///    Gets a value indicating whether this expression has been resolved to an API type.
    /// </summary>
    public bool IsResolved => _apiResolvedType is not null;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes an inline type expression (e.g., a collection declared in-place).
    /// </summary>
    /// <param name="apiInlineType">The API type instance used directly by this expression.</param>
    public ApiTypeExpression(ApiType apiInlineType) => this.ApiInlineType = apiInlineType;

    /// <summary>
    ///     Initializes an API named reference to a declared API type within a schema.
    /// </summary>
    /// <param name="apiKind">The expected kind of the referenced API type.</param>
    /// <param name="apiName">The name of the API type to be resolved in the schema.</param>
    public ApiTypeExpression(ApiTypeKind apiKind, string apiName)
    {
        this.ApiKind = apiKind;
        this.ApiName = apiName;
    }

    /// <summary>
    ///     Initializes a CLR typed reference to a declared API type within a schema.
    /// </summary>
    /// <param name="clrType">The CLR type to be resolved in the schema.</param>
    public ApiTypeExpression(Type clrType) => this.ClrType = TypeReflection.IsNullableType(clrType) ? Nullable.GetUnderlyingType(clrType) : clrType;

    /// <summary>
    ///     Initializes either an API named reference or a CLR typed reference to a declared API type within a schema.
    /// </summary>
    /// <param name="apiKind">The expected kind of the referenced API type.</param>
    /// <param name="apiName">The name of the API type to be resolved in the schema.</param>
    /// <param name="clrType">The CLR type to be resolved in the schema.</param>
    public ApiTypeExpression(ApiTypeKind? apiKind, string? apiName, Type? clrType)
    {
        this.ApiKind = apiKind;
        this.ApiName = apiName;
        this.ClrType = clrType;
    }
    #endregion

    #region ApiTypeExpression Methods
    internal void InitializeForCollection(ApiInitializationContext context)
    {
        this.Initialize(context, ApiInitializationCode.API_COLLECTION_TYPE_UNRESOLVED_ITEM_TYPE, nameof(ApiCollectionType.ApiItemType));
    }

    internal void InitializeForProperty(ApiInitializationContext context)
    {
        this.Initialize(context, ApiInitializationCode.API_PROPERTY_UNRESOLVED_TYPE, nameof(ApiProperty.ApiType));
    }

    private void Initialize
    (
        ApiInitializationContext context,
        ApiInitializationCode parentUnresolvedCode,
        string parentUnresolvedName
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        _apiResolvedType = null;

        // Try and resolve API type:
        // - Inline API type
        // - API named reference
        // - CLR type reference
        if (this.IsInline)
        {
            this.InitializeApiTypeByInline(context);
            return;
        }
        else if (this.IsApiNamedReference)
        {
            this.InitializeApiTypeByApiNamedReference(context, parentUnresolvedCode, parentUnresolvedName);
            return;
        }
        else if (this.IsClrTypeReference)
        {
            this.InitializeApiTypeByClrTypeReference(context, parentUnresolvedCode, parentUnresolvedName);
            return;
        }
        else
        {
            var path = context.ApiDeclaringPath!;
            var severity = ApiInitializationSeverity.Error;
            var code = parentUnresolvedCode;
            var description = $"{parentUnresolvedName} could not be resolved because none of the following are set: {nameof(this.ApiInlineType)}, a valid combination of {nameof(this.ApiKind)} and {nameof(this.ApiName)}, or {nameof(this.ClrType)}";
            var remediation = $"Specify either {nameof(this.ApiInlineType)}, a valid combination of {nameof(this.ApiKind)} and {nameof(this.ApiName)}, or {nameof(this.ClrType)}";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion

    #region Factory Methods
    /// <summary>Creates an <see cref="ApiTypeExpression"/> that references the CLR type <typeparamref name="TClr"/>.</summary>
    /// <typeparam name="TClr">The CLR type to reference.</typeparam>
    /// <returns>A new <see cref="ApiTypeExpression"/> backed by <typeparamref name="TClr"/>.</returns>
    public static ApiTypeExpression ClrRef<TClr>() => new(typeof(TClr));

    /// <summary>Creates an <see cref="ApiTypeExpression"/> whose items are of CLR type <typeparamref name="TClr"/> collected into a <see cref="HashSet{TClr}"/>.</summary>
    /// <typeparam name="TClr">The CLR item type.</typeparam>
    /// <param name="apiItemTypeModifiers">Modifiers applied to each item type expression.</param>
    /// <returns>A new collection <see cref="ApiTypeExpression"/> backed by <see cref="HashSet{TClr}"/>.</returns>
    public static ApiTypeExpression HashSetOf<TClr>(ApiTypeModifiers apiItemTypeModifiers)
        => new(new ApiCollectionType(ClrRef<TClr>(), apiItemTypeModifiers, typeof(HashSet<TClr>)));

    /// <summary>Creates an <see cref="ApiTypeExpression"/> whose items are of CLR type <typeparamref name="TClr"/> collected into a <see cref="List{TClr}"/>.</summary>
    /// <typeparam name="TClr">The CLR item type.</typeparam>
    /// <param name="apiItemTypeModifiers">Modifiers applied to each item type expression.</param>
    /// <returns>A new collection <see cref="ApiTypeExpression"/> backed by <see cref="List{TClr}"/>.</returns>
    public static ApiTypeExpression ListOf<TClr>(ApiTypeModifiers apiItemTypeModifiers)
        => new(new ApiCollectionType(ClrRef<TClr>(), apiItemTypeModifiers, typeof(List<TClr>)));
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        if (this.IsInline)
        {
            var apiInlineType = this.ApiInlineType.SafeToString();
            return $"{nameof(ApiTypeExpression)} {{ApiInlineType={apiInlineType}}}";
        }

        var kind = this.ApiKind.SafeToString();
        var apiName = this.ApiName.SafeToString();
        var clrType = this.ClrType.SafeToString();
        return $"{nameof(ApiTypeExpression)} {{{nameof(this.ApiKind)}={kind}, {nameof(this.ApiName)}={apiName}, {nameof(this.ClrType)}={clrType}}}";
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiTypeByApiNamedReference
    (
        ApiInitializationContext context,
        ApiInitializationCode parentUnresolvedCode,
        string parentUnresolvedName
    )
    {
        var kind = this.ApiKind!;
        var apiName = this.ApiName!;

        switch (kind)
        {
            case ApiTypeKind.Scalar:
                _apiResolvedType = context.ApiSchema.TryGetScalarTypeByApiName(apiName, out var apiScalarType) ? apiScalarType : null;
                break;

            case ApiTypeKind.Enum:
                _apiResolvedType = context.ApiSchema.TryGetEnumTypeByApiName(apiName, out var apiEnumType) ? apiEnumType : null;
                break;

            case ApiTypeKind.Object:
                _apiResolvedType = context.ApiSchema.TryGetObjectTypeByApiName(apiName, out var apiObjectType) ? apiObjectType : null;
                break;

            case ApiTypeKind.Collection:
                {
                    var path = context.ApiDeclaringPath!;
                    var severity = ApiInitializationSeverity.Error;
                    var code = parentUnresolvedCode;
                    var description = $"{parentUnresolvedName} could not be resolved for {nameof(this.ApiKind)}={this.ApiKind.SafeToString()} and {nameof(this.ApiName)}={this.ApiName.SafeToString()} because {nameof(ApiTypeKind.Collection)} types must be defined inline";
                    var remediation = $"Define the {nameof(ApiTypeKind.Collection)} type inline using {nameof(this.ApiInlineType)} instead of specifying {nameof(this.ApiKind)} and {nameof(this.ApiName)}";

                    context.AddIssue(path, severity, code, description, remediation);
                    return;
                }

            default:
                break;
        }

        if (_apiResolvedType is null)
        {
            var path = context.ApiDeclaringPath!;
            var severity = ApiInitializationSeverity.Error;
            var code = parentUnresolvedCode;
            var description = $"{parentUnresolvedName} could not be resolved for {nameof(this.ApiKind)}='{this.ApiKind.SafeToString()}' and {nameof(this.ApiName)}='{this.ApiName.SafeToString()}'";
            var remediation = $"Verify that a type is declared in the schema for {nameof(this.ApiKind)}='{this.ApiKind.SafeToString()}' and {nameof(this.ApiName)}='{this.ApiName.SafeToString()}'";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeApiTypeByClrTypeReference
    (
        ApiInitializationContext context,
        ApiInitializationCode parentUnresolvedCode,
        string parentUnresolvedName
    )
    {
        var clrType = this.ClrType!;

        _apiResolvedType = context.ApiSchema.TryGetTypeByClrType(clrType, out var apiType) ? apiType : null;

        if (_apiResolvedType is null)
        {
            var path = context.ApiDeclaringPath!;
            var severity = ApiInitializationSeverity.Error;
            var code = parentUnresolvedCode;
            var description = $"{parentUnresolvedName} could not be resolved for {nameof(this.ClrType)}='{this.ClrType.SafeToName()}'";
            var remediation = $"Verify that a type is declared in the schema for {nameof(this.ClrType)}='{this.ClrType.SafeToName()}'";
            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeApiTypeByInline(ApiInitializationContext context)
    {
        _apiResolvedType = this.ApiInlineType;

        if (this.ApiInlineType is ApiCollectionType collection)
        {
            collection.Initialize(context);
        }
    }
    #endregion
}
