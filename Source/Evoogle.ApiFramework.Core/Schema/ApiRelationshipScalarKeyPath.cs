// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a key path that maps a single principal identity scalar leaf to a scalar property
///     directly on the dependent object type at the current navigation depth.
/// </summary>
/// <param name="clrPropertyName">The CLR property name on the dependent type that holds the FK scalar value.</param>
public sealed class ApiRelationshipScalarKeyPath(string clrPropertyName) : ApiRelationshipKeyPath
{
    #region ApiRelationshipScalarKeyPath Fields
    private ApiProperty? _apiResolvedProperty = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiRelationshipScalarKeyPath);
    #endregion

    #region ApiRelationshipKeyPath Properties
    /// <inheritdoc/>
    public override ApiRelationshipKeyPathKind ApiKind => ApiRelationshipKeyPathKind.Scalar;
    #endregion

    #region ApiRelationshipScalarKeyPath Properties
    /// <summary>Gets the CLR property name on the dependent type that holds the FK scalar value.</summary>
    public string ClrPropertyName { get; } = clrPropertyName;

    /// <summary>Gets the resolved <see cref="ApiProperty"/> backing this key path. Available after initialization.</summary>
    public ApiProperty ApiProperty => this.ThrowIfNotInitialized(_apiResolvedProperty);
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrPropertyName = this.ClrPropertyName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiRelationshipScalarKeyPath)} {{{nameof(this.ClrPropertyName)}={clrPropertyName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: this.ClrPropertyName);

    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeClrPropertyName(context);
        this.InitializeApiProperty(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeClrPropertyName(ApiInitializationContext context)
    {
        if (!ApiSchemaHelpers.IsNameInvalid(this.ClrPropertyName))
        {
            return;
        }

        var path = this.ApiPath;
        var severity = ApiInitializationSeverity.Error;
        var code = ApiInitializationCode.API_RELATIONSHIP_KEY_PATH_INVALID_CLR_PROPERTY_NAME;
        var description = $"{nameof(this.ClrPropertyName)} must not be null, empty, or whitespace";
        var remediation = $"Specify a valid {nameof(this.ClrPropertyName)} value";

        context.AddIssue(path, severity, code, description, remediation);
    }

    private void InitializeApiProperty(ApiInitializationContext context)
    {
        _apiResolvedProperty = null;

        if (ApiSchemaHelpers.IsNameInvalid(this.ClrPropertyName))
        {
            return;
        }

        var apiDeclaringObjectType = context.ApiDeclaringObjectType;
        if (apiDeclaringObjectType.TryGetPropertyByClrName(this.ClrPropertyName, out var apiResolvedProperty))
        {
            _apiResolvedProperty = apiResolvedProperty;
            return;
        }

        var path = this.ApiPath;
        var severity = ApiInitializationSeverity.Error;
        var code = ApiInitializationCode.API_RELATIONSHIP_KEY_PATH_UNRESOLVED_API_PROPERTY;
        var description = $"Property with CLR name '{this.ClrPropertyName}' could not be found on object type '{apiDeclaringObjectType.ApiName}'";
        var remediation = $"Verify the CLR property name or add a property with CLR name '{this.ClrPropertyName}' to '{apiDeclaringObjectType.ApiName}'";

        context.AddIssue(path, severity, code, description, remediation);
    }
    #endregion
}
