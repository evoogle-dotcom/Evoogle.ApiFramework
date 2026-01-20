// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Net.Sockets;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a single property part within an <see cref="ApiIdentity"/>.
/// </summary>
/// <remarks>
///     Each part references a property of the parent <see cref="ApiObjectType"/>.
///     Type coercion is handled automatically via <see cref="ApiProperty"/> and resolved at initialization time.
/// </remarks>
/// <param name="apiPropertyName">The name of the property that is part of the identity.</param>
/// <param name="clrConfiguredIdType">Optional user configured CLR type for the identity part. If not provided, the type is inferred from the resolved property.</param>
[JsonConverter(typeof(ApiIdentityPartJsonConverter))]
public sealed class ApiIdentityPart
(
    string apiPropertyName,
    Type? clrConfiguredIdType = null
) : ApiSchemaElement
{
    #region Fields
    private ApiProperty? _apiResolvedProperty = null;
    private Type? _clrResolvedIdType = null;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the name of the property that is part of this identity.
    /// </summary>
    public string ApiPropertyName { get; } = apiPropertyName;

    /// <summary>
    ///     Gets the resolved property referenced by this identity part.
    /// </summary>
    /// <remarks>
    ///     This property is available after initialization.
    /// </remarks>
    public ApiProperty ApiProperty => this.ThrowIfNotInitialized(_apiResolvedProperty);

    /// <summary>
    ///     Gets the user configured CLR type for the identity part.
    /// </summary>
    /// <remarks>
    ///     This property is optional. If not provided, the type is inferred from the resolved property.
    /// </remarks>
    public Type? ClrConfiguredIdType { get; } = clrConfiguredIdType;

    /// <summary>
    ///     Gets the resolved CLR type of the identity part.
    /// </summary>
    /// <remarks>
    ///    This property is available after initialization.
    /// </remarks>
    public Type ClrIdType => this.ThrowIfNotInitialized(_clrResolvedIdType);
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiParentPath)
        => ApiSchemaHelpers.BuildPath(apiParentPath, apiChildPath: nameof(ApiIdentityPart), apiApiName: this.ApiPropertyName);

    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiPropertyName(context);
        this.InitializeApiProperty(context);
        this.InitializeClrIdType(context);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        var apiPropertyName = this.ApiPropertyName.SafeToString();
        var clrIdTypeHint = this.ClrConfiguredIdType.SafeToName();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiIdentityPart)} {{{nameof(this.ApiPropertyName)}={apiPropertyName}, {nameof(this.ClrConfiguredIdType)}={clrIdTypeHint}, {nameof(this.ExtensionCount)}={extensionCount}}}";
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
            var code = ApiInitializationCode.API_IDENTITY_PART_INVALID_API_PROPERTY_NAME;
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
            // If the ApiPropertyName is invalid, skip further processing
            return;
        }

        // Resolve the related API property for the parent API object type.
        var apiParentObjectType = context.ApiParentObjectType;
        if (apiParentObjectType.TryGetPropertyByApiName(this.ApiPropertyName, out var apiResolvedProperty))
        {
            _apiResolvedProperty = apiResolvedProperty;

            // Check for circular identity references
            if (_apiResolvedProperty.ApiType is ApiObjectType referencedObjectType && referencedObjectType.HasIdentity)
            {
                // Direct self reference, e.g. Node identity includes Node.Self
                var isDirectSelfReference = ReferenceEquals(referencedObjectType, apiParentObjectType);

                // Indirect reference, e.g. Beta identity references Alpha and Alpha identity references Beta
                var isIndirectCircularReference = !isDirectSelfReference
                    && IdentityDependsOn(referencedObjectType, apiParentObjectType);

                if (isDirectSelfReference || isIndirectCircularReference)
                {
                    var path = this.ApiPath;
                    var severity = ApiInitializationSeverity.Error;
                    var code = ApiInitializationCode.API_IDENTITY_PART_CIRCULAR_REFERENCE;
                    var description = $"Circular identity reference detected: property '{this.ApiPropertyName}' references type '{referencedObjectType.ApiName}' which has an identity that depends on the current type";
                    var remediation = "Remove the circular dependency by restructuring the identity definitions or using a non-identity property";

                    context.AddIssue(path, severity, code, description, remediation);
                    _apiResolvedProperty = null; // Clear to prevent further issues
                }
            }
        }

        if (_apiResolvedProperty is null)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_PROPERTY;
            var description = $"{nameof(this.ApiProperty)} could not be resolved for {nameof(this.ApiPropertyName)}='{this.ApiPropertyName.SafeToString()}'";
            var remediation = $"Verify that {nameof(this.ApiPropertyName)} refers to a valid property on the parent {nameof(ApiObjectType)}";

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
                foreach (var part in identity.ApiIdentityParts)
                {
                    ApiProperty? partProperty = null;
                    try
                    {
                        partProperty = part.ApiProperty;
                    }
                    catch
                    {
                        // Best-effort: referenced types may not be initialized yet during schema initialization.
                    }

                    if (partProperty?.ApiType is not ApiObjectType referenced)
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

    private void InitializeClrIdType(ApiInitializationContext context)
    {
        _clrResolvedIdType = null;

        // Use the configured type if provided
        if (this.ClrConfiguredIdType is not null)
        {
            // Validate that the configured type is ApiId compatible
            if (!ApiId.IsCompatibleScalarType(this.ClrConfiguredIdType))
            {
                var path = this.ApiPath;
                var severity = ApiInitializationSeverity.Error;
                var code = ApiInitializationCode.API_IDENTITY_PART_INVALID_SCALAR_TYPE;
                var description = $"Scalar type '{this.ClrConfiguredIdType.SafeToName()}' is not compatible with {nameof(ApiId)}";
                var scalarTypeNames = string.Join(",", ApiId.GetCompatibleScalarTypes().Select(t => t.SafeToName()).OrderBy(n => n, StringComparer.Ordinal));
                var remediation = $"Use one of the supported scalar types: {scalarTypeNames}";

                context.AddIssue(path, severity, code, description, remediation);
            }

            _clrResolvedIdType = this.ClrConfiguredIdType;
        }

        if (_apiResolvedProperty is not null)
        {
            var clrResolvedPropertyType = _apiResolvedProperty.ApiType.ClrType;

            _clrResolvedIdType ??= ApiId.GetCompatibleScalarType(clrResolvedPropertyType);

            if (clrResolvedPropertyType == typeof(string) && _clrResolvedIdType != typeof(string))
            {
                var path = this.ApiPath;
                var severity = ApiInitializationSeverity.Warning;
                var code = ApiInitializationCode.API_IDENTITY_PART_PERFORMANCE_CONCERN;
                var description = $"Identity part '{this.ApiPropertyName}' requires type coercion between {nameof(String)} and {_clrResolvedIdType.SafeToName()}, which may impact performance";
                var remediation = $"Consider using {_clrResolvedIdType.SafeToName()} as the property type directly, or be aware of the parsing overhead";

                context.AddIssue(path, severity, code, description, remediation);
            }
        }
    }
    #endregion
}
