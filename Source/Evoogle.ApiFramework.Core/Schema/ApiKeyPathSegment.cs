// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a single property navigation step within an <see cref="ApiKeyPath"/>.
/// </summary>
/// <remarks>
///     Each segment holds a CLR property name and, after initialization, a reference to the resolved <see cref="ApiProperty"/>.
///     Segments do not validate whether the resolved property is scalar or object-typed — that responsibility belongs to the parent <see cref="ApiKeyPath"/>, which has positional context (navigation vs. terminal).
/// </remarks>
/// <param name="clrPropertyName">The CLR property name for this navigation step.</param>
[JsonConverter(typeof(ApiKeyPathSegmentJsonConverter))]
public sealed class ApiKeyPathSegment(string clrPropertyName) : ApiSchemaElement
{
    #region Fields
    private ApiProperty? _apiResolvedProperty = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiKeyPathSegment);
    #endregion

    #region ApiKeyPathSegment Properties
    /// <summary>Gets the CLR property name for this navigation step.</summary>
    public string ClrPropertyName { get; } = clrPropertyName;

    /// <summary>Gets the resolved <see cref="ApiProperty"/> for this segment. Available after initialization.</summary>
    public ApiProperty ApiProperty => this.ThrowIfNotInitialized(_apiResolvedProperty);

    /// <summary>Gets a value indicating whether the CLR property was successfully resolved during initialization.</summary>
    internal bool IsPropertyResolved => _apiResolvedProperty is not null;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrPropertyName = this.ClrPropertyName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiKeyPathSegment)} {{{nameof(this.ClrPropertyName)}={clrPropertyName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
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
        var code = ApiInitializationCode.API_KEY_PATH_SEGMENT_INVALID_CLR_PROPERTY_NAME;
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
        var code = ApiInitializationCode.API_KEY_PATH_SEGMENT_UNRESOLVED_API_PROPERTY;
        var description = $"Property with CLR name '{this.ClrPropertyName}' could not be found on object type '{apiDeclaringObjectType.ApiName}'";
        var remediation = $"Verify the CLR property name or add a property with CLR name '{this.ClrPropertyName}' to '{apiDeclaringObjectType.ApiName}'";

        context.AddIssue(path, severity, code, description, remediation);
    }
    #endregion
}
