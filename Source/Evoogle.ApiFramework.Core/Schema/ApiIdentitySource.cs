// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a single source within an <see cref="ApiIdentity"/> that defines where an identity value originates.
/// </summary>
/// <remarks>
///     <para>
///         Each source references a property on the parent <see cref="ApiObjectType"/>.
///         A source is either <see cref="ApiIdentitySourceKind.Scalar"/> (resolves directly to a value)
///         or <see cref="ApiIdentitySourceKind.Nested"/> (delegates to a referenced object type's identity).
///     </para>
///     <para>
///         The <see cref="ApiKind"/> is inferred during initialization based on the resolved property's type:
///         <list type="bullet">
///             <item><description>If the property's type is a scalar type, the source is <see cref="ApiIdentitySourceKind.Scalar"/>.</description></item>
///             <item><description>If the property's type is an <see cref="ApiObjectType"/> with an identity, the source is <see cref="ApiIdentitySourceKind.Nested"/>.</description></item>
///         </list>
///     </para>
/// </remarks>
/// <param name="apiPropertyName">The name of the property on the parent object type that is part of the identity.</param>
/// <param name="clrScalarType">Optional user-configured CLR type for scalar sources. If not provided, the type is inferred from the resolved property. Ignored for nested sources.</param>
/// <param name="apiNestedName">Optional name of the identity on the nested object type to use. If null, the primary identity is used.</param>
[JsonConverter(typeof(ApiIdentitySourceJsonConverter))]
public sealed class ApiIdentitySource
(
    string apiPropertyName,
    Type? clrScalarType = null,
    string? apiNestedName = null
) : ApiSchemaElement
{
    #region Fields
    private ApiIdentitySourceKind? _apiResolvedKind = null;
    private ApiProperty? _apiResolvedProperty = null;
    private ApiIdentity? _apiResolvedNestedIdentity = null;
    private Type? _clrResolvedScalarType = clrScalarType;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the name of the property on the parent <see cref="ApiObjectType"/> that is part of this identity.
    /// </summary>
    public string ApiPropertyName { get; } = apiPropertyName;

    /// <summary>
    ///     Gets whether this source resolves to a scalar value or delegates to a nested object's identity.
    /// </summary>
    /// <remarks>
    ///     This value is inferred during initialization based on the resolved property's type.
    ///     Available after initialization.
    /// </remarks>
    public ApiIdentitySourceKind ApiKind => this.ThrowIfNotInitialized(_apiResolvedKind);

    /// <summary>
    ///     Gets the resolved <see cref="ApiProperty"/> on the parent object type.
    /// </summary>
    /// <remarks>
    ///     Available after initialization.
    /// </remarks>
    public ApiProperty ApiProperty => this.ThrowIfNotInitialized(_apiResolvedProperty);

    /// <summary>
    ///     Gets the resolved nested <see cref="ApiIdentity"/> on the referenced object type for nested sources.
    /// </summary>
    /// <remarks>
    ///     For <see cref="ApiIdentitySourceKind.Scalar"/> sources, this is null.
    ///     For <see cref="ApiIdentitySourceKind.Nested"/> sources, this is the nested identity that this source delegates to.
    ///     Available after initialization.
    /// </remarks>
    public ApiIdentity? ApiNestedIdentity => this.ApiKind == ApiIdentitySourceKind.Nested ? this.ThrowIfNotInitialized(_apiResolvedNestedIdentity) : null;

    /// <summary>
    ///     Gets the resolved CLR scalar type of the identity source for scalar sources.
    /// </summary>
    /// <remarks>
    ///     For <see cref="ApiIdentitySourceKind.Scalar"/> sources, this is the scalar type used for the <see cref="ApiId"/>.
    ///     For <see cref="ApiIdentitySourceKind.Nested"/> sources, this is null (the nested identity's sources define the types).
    ///     Available after initialization.
    /// </remarks>
    public Type? ClrScalarType => this.ApiKind == ApiIdentitySourceKind.Scalar ? this.ThrowIfNotInitialized(_clrResolvedScalarType) : null;

    /// <summary>
    ///     Gets the name of the identity on the referenced object type to use for nested sources.
    /// </summary>
    /// <remarks>
    ///     If null and the source is <see cref="ApiIdentitySourceKind.Nested"/>, the primary identity of the
    ///     referenced <see cref="ApiObjectType"/> is used.
    /// </remarks>
    public string? ApiNestedName { get; } = apiNestedName;
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        var apiPropertyName = this.ApiPropertyName.SafeToString();
        var apiKind = _apiResolvedKind.SafeToString();
        var clrScalarType = _clrResolvedScalarType.SafeToName();
        var apiNestedName = this.ApiNestedName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiIdentitySource)} {{{nameof(this.ApiPropertyName)}={apiPropertyName}, {nameof(this.ApiKind)}={apiKind}, {nameof(this.ClrScalarType)}={clrScalarType}, {nameof(this.ApiNestedName)}={apiNestedName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiParentPath)
        => ApiSchemaHelpers.BuildPath(apiParentPath, apiChildPath: nameof(ApiIdentitySource), apiApiName: this.ApiPropertyName);

    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiPropertyName(context);
        this.InitializeApiProperty(context);
        this.InitializeApiKind(context);
        this.InitializeApiNestedIdentity(context);
        this.InitializeClrScalarType(context);
    }
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates a scalar identity source that resolves directly to a value.
    /// </summary>
    /// <param name="apiPropertyName">The name of the property on the parent object type that is part of the identity.</param>
    /// <param name="clrScalarType">Optional user-configured CLR scalar type for the identity source. If not provided, the scalar type is inferred from the resolved property.</param>
    /// <returns>A new <see cref="ApiIdentitySource"/> instance representing a scalar source.</returns>
    public static ApiIdentitySource Scalar(string apiPropertyName, Type? clrScalarType = null)
    {
        var scalar = new ApiIdentitySource(apiPropertyName, clrScalarType, null)
        {
            _apiResolvedKind = ApiIdentitySourceKind.Scalar,
        };
        return scalar;
    }

    /// <summary>
    ///     Creates a nested identity source that delegates to a referenced object's identity.
    /// </summary>
    /// <param name="apiPropertyName">The name of the property on the parent object type that is part of the identity.</param>
    /// <param name="apiNestedName">Optional name of the identity on the referenced object type to use. If null, the primary identity is used.</param>
    /// <returns>A new <see cref="ApiIdentitySource"/> instance representing a nested source.</returns>
    public static ApiIdentitySource Nested(string apiPropertyName, string? apiNestedName = null)
    {
        var nested = new ApiIdentitySource(apiPropertyName, null, apiNestedName)
        {
            _apiResolvedKind = ApiIdentitySourceKind.Nested,
        };
        return nested;
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiPropertyName(ApiInitializationContext context)
    {
        var isApiPropertyNameInvalid = ApiSchemaHelpers.IsNameInvalid(this.ApiPropertyName);
        if (isApiPropertyNameInvalid)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_SOURCE_INVALID_API_PROPERTY_NAME;
            var description = $"{nameof(this.ApiPropertyName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiPropertyName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeApiProperty(ApiInitializationContext context)
    {
        _apiResolvedProperty = null;

        var isApiPropertyNameInvalid = ApiSchemaHelpers.IsNameInvalid(this.ApiPropertyName);
        if (isApiPropertyNameInvalid)
        {
            return;
        }

        var apiParentObjectType = context.ApiParentObjectType;
        if (apiParentObjectType.TryGetPropertyByApiName(this.ApiPropertyName, out var apiResolvedProperty))
        {
            _apiResolvedProperty = apiResolvedProperty;

            // Check for circular identity references when property type is an object type with identity
            if (_apiResolvedProperty.ApiType is ApiObjectType referencedObjectType && referencedObjectType.HasIdentity)
            {
                var isDirectSelfReference = ReferenceEquals(referencedObjectType, apiParentObjectType);
                var isIndirectCircularReference = !isDirectSelfReference && IdentityDependsOn(referencedObjectType, apiParentObjectType);

                if (isDirectSelfReference || isIndirectCircularReference)
                {
                    var path = this.ApiPath;
                    var severity = ApiInitializationSeverity.Error;
                    var code = ApiInitializationCode.API_IDENTITY_SOURCE_CIRCULAR_REFERENCE;
                    var description = $"Circular identity reference detected: property '{this.ApiPropertyName}' references type '{referencedObjectType.ApiName}' which has an identity that depends on the current type";
                    var remediation = "Remove the circular dependency by restructuring the identity definitions or using a non-identity property";

                    context.AddIssue(path, severity, code, description, remediation);
                    _apiResolvedProperty = null;
                }
            }
        }

        if (_apiResolvedProperty is null)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_SOURCE_UNRESOLVED_PROPERTY;
            var description = $"{nameof(this.ApiProperty)} could not be resolved for {nameof(this.ApiPropertyName)}='{this.ApiPropertyName.SafeToString()}'";
            var remediation = $"Verify that {nameof(this.ApiPropertyName)} refers to a valid property on the parent {nameof(ApiObjectType)}";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeApiKind(ApiInitializationContext context)
    {
        if (_apiResolvedProperty is null)
        {
            // Cannot determine kind without a resolved property,
            // but avoid cascading errors by not attempting to infer kind when the property is missing.
            _apiResolvedKind = null;
            return;
        }

        // If the kind was explicitly set (e.g. via factory methods), use it. Otherwise, infer based on the resolved property's type.
        if (_apiResolvedKind is not null)
        {
            return;
        }

        // Infer kind based on the resolved property's type
        if (_apiResolvedProperty.ApiType is ApiScalarType)
        {
            _apiResolvedKind = ApiIdentitySourceKind.Scalar;
        }
        else if (_apiResolvedProperty.ApiType is ApiObjectType)
        {
            _apiResolvedKind = ApiIdentitySourceKind.Nested;
        }

        if (_apiResolvedKind is null)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_SOURCE_UNRESOLVED_KIND;
            var description = $"Could not determine {nameof(this.ApiKind)} for property '{this.ApiPropertyName}' with CLR type '{_apiResolvedProperty.ApiType.ClrTypeName}'";
            var remediation = $"Ensure that the property's type is either a scalar type or an object type with an identity, or explicitly specify the kind using factory methods";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeApiNestedIdentity(ApiInitializationContext context)
    {
        _apiResolvedNestedIdentity = null;

        if (_apiResolvedProperty is null)
        {
            return;
        }

        if (_apiResolvedKind != ApiIdentitySourceKind.Nested)
        {
            return;
        }

        var referencedObjectType = (ApiObjectType)_apiResolvedProperty.ApiType;

        if (this.ApiNestedName is not null)
        {
            // Resolve by explicit name
            if (referencedObjectType.TryGetIdentityByApiName(this.ApiNestedName, out var namedIdentity))
            {
                _apiResolvedNestedIdentity = namedIdentity;
            }
            else
            {
                var path = this.ApiPath;
                var severity = ApiInitializationSeverity.Error;
                var code = ApiInitializationCode.API_IDENTITY_SOURCE_UNRESOLVED_REFERENCED_IDENTITY;
                var description = $"Referenced identity '{this.ApiNestedName}' could not be found on object type '{referencedObjectType.ApiName}'";
                var availableIdentities = string.Join(", ", referencedObjectType.ApiIdentities.Select(i => $"'{i.ApiName}'"));
                var remediation = !string.IsNullOrEmpty(availableIdentities)
                    ? $"Use one of the available identities: {availableIdentities}"
                    : $"Define an identity on '{referencedObjectType.ApiName}' or remove {nameof(this.ApiNestedName)}";

                context.AddIssue(path, severity, code, description, remediation);
            }
        }
        else
        {
            // Use primary identity (first by convention)
            _apiResolvedNestedIdentity = referencedObjectType.ApiPrimaryIdentity;

            if (_apiResolvedNestedIdentity is null)
            {
                var path = this.ApiPath;
                var severity = ApiInitializationSeverity.Error;
                var code = ApiInitializationCode.API_IDENTITY_SOURCE_NO_PRIMARY_IDENTITY;
                var description = $"Property '{this.ApiPropertyName}' references object type '{referencedObjectType.ApiName}' which has no primary identity";
                var remediation = $"Define a primary identity on '{referencedObjectType.ApiName}' or specify {nameof(this.ApiNestedName)} explicitly";

                context.AddIssue(path, severity, code, description, remediation);
            }
        }
    }

    private void InitializeClrScalarType(ApiInitializationContext context)
    {
        if (_apiResolvedProperty is null)
        {
            _clrResolvedScalarType = null;
            return;
        }

        if (_apiResolvedKind != ApiIdentitySourceKind.Scalar)
        {
            _clrResolvedScalarType = null;
            return;
        }

        var referencedScalarType = (ApiScalarType)_apiResolvedProperty.ApiType;

        var clrResolvedScalarType = _clrResolvedScalarType ?? referencedScalarType.ClrType;

        _clrResolvedScalarType ??= ApiId.GetCompatibleScalarType(clrResolvedScalarType);

        if (clrResolvedScalarType == typeof(string) && _clrResolvedScalarType != typeof(string))
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Warning;
            var code = ApiInitializationCode.API_IDENTITY_SOURCE_PERFORMANCE_CONCERN;
            var description = $"Identity source '{this.ApiPropertyName}' requires type coercion between {nameof(String)} and {_clrResolvedScalarType.SafeToName()}, which may impact performance";
            var remediation = $"Consider using {_clrResolvedScalarType.SafeToName()} as the property type directly, or be aware of the parsing overhead";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private static bool IdentityDependsOn(ApiObjectType apiObjectType, ApiObjectType targetObjectType)
    {
        var visited = new HashSet<ApiObjectType>(ReferenceEqualityComparer.Instance);
        var stack = new Stack<ApiObjectType>();

        visited.Add(apiObjectType);
        stack.Push(apiObjectType);

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            foreach (var identity in current.ApiIdentities)
            {
                foreach (var source in identity.ApiIdentitySources)
                {
                    ApiProperty? sourceProperty = null;
                    try
                    {
                        sourceProperty = source.ApiProperty;
                    }
                    catch
                    {
                        // Best-effort: referenced types may not be initialized yet during schema initialization.
                    }

                    if (sourceProperty?.ApiType is not ApiObjectType referenced)
                    {
                        continue;
                    }

                    if (ReferenceEquals(referenced, targetObjectType))
                    {
                        return true;
                    }

                    if (!referenced.HasIdentity)
                    {
                        continue;
                    }

                    if (visited.Add(referenced))
                    {
                        stack.Push(referenced);
                    }
                }
            }
        }

        return false;
    }
    #endregion
}
