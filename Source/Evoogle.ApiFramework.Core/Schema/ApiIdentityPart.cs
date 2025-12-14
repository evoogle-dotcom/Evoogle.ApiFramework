// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a single property part within an <see cref="ApiIdentity"/>.
/// </summary>
/// <remarks>
///     Each part references a property of the parent <see cref="ApiObjectType"/> and specifies
///     ordering behavior for identity serialization. Type coercion is handled automatically via
///     <see cref="ApiProperty"/> and the configured <see cref="IApiIdTypeDetectionStrategy"/>.
/// </remarks>
/// <param name="apiPropertyName">The name of the property that is part of the identity.</param>
/// <param name="emitAsOrdered">Indicates whether this part should be emitted as an ordered (positional) component rather than named.</param>
public sealed class ApiIdentityPart(string apiPropertyName, bool emitAsOrdered = false) : ApiSchemaElement
{
    #region Fields
    private ApiProperty? _apiResolvedProperty = null;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the name of the property that is part of this identity.
    /// </summary>
    public string ApiPropertyName { get; } = apiPropertyName;

    /// <summary>
    ///     Gets a value indicating whether this part should be emitted as an ordered (positional) component.
    /// </summary>
    public bool EmitAsOrdered { get; } = emitAsOrdered;

    /// <summary>
    ///     Gets the resolved property referenced by this identity part.
    /// </summary>
    /// <remarks>
    ///     This property is available after initialization.
    /// </remarks>
    public ApiProperty ApiProperty => this.ThrowIfNotInitialized(_apiResolvedProperty);
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
    #endregion
}
