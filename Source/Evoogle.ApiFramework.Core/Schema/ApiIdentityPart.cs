// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.Schema.Internal;
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
/// <param name="targetClrType">Optional CLR type to use for <see cref="ApiId"/> conversion. If null, the type is detected automatically.</param>
public sealed class ApiIdentityPart
(
    string apiPropertyName,
    Type? targetClrType = null
) : ApiSchemaElement
{
    #region Fields
    private ApiProperty? _apiResolvedProperty = null;
    private Type? _resolvedTargetType = null;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the name of the property that is part of this identity.
    /// </summary>
    public string ApiPropertyName { get; } = apiPropertyName;

    /// <summary>
    ///     Gets the optional CLR type override for <see cref="Identity.ApiId"/> conversion.
    /// </summary>
    /// <remarks>
    ///     If null, the type is detected automatically based on the property's CLR type.
    /// </remarks>
    public Type? TargetClrType { get; } = targetClrType;

    /// <summary>
    ///     Gets the resolved property referenced by this identity part.
    /// </summary>
    /// <remarks>
    ///     This property is available after initialization.
    /// </remarks>
    public ApiProperty ApiProperty => this.ThrowIfNotInitialized(_apiResolvedProperty);

    /// <summary>
    ///     Gets the resolved target CLR type for <see cref="Identity.ApiId"/> conversion.
    /// </summary>
    /// <remarks>
    ///     This is either the <see cref="TargetClrType"/> override or automatically detected from
    ///     the <see cref="ApiProperty"/>'s CLR type during initialization.
    /// </remarks>
    public Type ResolvedTargetType => this.ThrowIfNotInitialized(_resolvedTargetType);
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
        this.InitializeResolvedTargetType(context);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        var apiPropertyName = this.ApiPropertyName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiIdentityPart)} {{{nameof(this.ApiPropertyName)}={apiPropertyName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiPropertyName(ApiInitializationContext context)
    {
        if (string.IsNullOrWhiteSpace(this.ApiPropertyName))
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiPropertyName)}";
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

        if (!string.IsNullOrWhiteSpace(this.ApiPropertyName))
        {
            // Resolve the related API property for the parent API object type.
            var apiParentObjectType = context.ApiParentObjectType;
            if (apiParentObjectType.TryGetPropertyByApiName(this.ApiPropertyName, out var apiResolvedProperty))
            {
                _apiResolvedProperty = apiResolvedProperty;

                // Check for circular identity references
                // ApiPath is guaranteed to be set during initialization before identity resolution
                if (_apiResolvedProperty.ApiType is ApiObjectType referencedObjectType && referencedObjectType.HasIdentity)
                {
                    var referencedTypePath = referencedObjectType.ApiPath;
                    if (context.IdentityTraversalPath.Contains(referencedTypePath))
                    {
                        var path = $"{this.ApiPath}.{nameof(this.ApiProperty)}";
                        var severity = ApiInitializationSeverity.Error;
                        var code = ApiInitializationCode.API_IDENTITY_PART_CIRCULAR_REFERENCE;
                        var description = $"Circular identity reference detected: property '{this.ApiPropertyName}' references type '{referencedObjectType.ApiName}' which has an identity that depends on the current type";
                        var remediation = "Remove the circular dependency by restructuring the identity definitions or using a non-identity property";

                        context.AddIssue(path, severity, code, description, remediation);
                        _apiResolvedProperty = null; // Clear to prevent further issues
                    }
                }
            }
        }

        if (_apiResolvedProperty is null)
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiProperty)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_PROPERTY;
            var description = $"{nameof(this.ApiProperty)} could not be resolved for {nameof(this.ApiPropertyName)}='{this.ApiPropertyName.SafeToString()}'";
            var remediation = $"Verify that {nameof(this.ApiPropertyName)} refers to a valid property on the parent {nameof(ApiObjectType)}";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeResolvedTargetType(ApiInitializationContext context)
    {
        _resolvedTargetType = null;

        // If the resolved property is null, we can't determine the target type
        if (_apiResolvedProperty is null)
        {
            return;
        }

        // Use the override if provided, otherwise detect from property type
        var targetType = this.TargetClrType ?? ApiSchemaHelpers.DetectApiIdTargetType(_apiResolvedProperty.ApiType.ClrType);

        // Validate that the target type is ApiId-compatible
        if (!ApiId.IsCompatibleScalarType(targetType))
        {
            var path = $"{this.ApiPath}.{nameof(this.ResolvedTargetType)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_PART_INVALID_TARGET_TYPE;
            var description = $"Target type '{targetType.SafeToName()}' is not compatible with {nameof(Identity.ApiId)}";
            var remediation = $"Use one of the supported types: int, long, Guid, Ulid, CultureInfo, or string";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        _resolvedTargetType = targetType;
    }
    #endregion
}
