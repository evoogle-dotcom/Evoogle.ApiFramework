// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Abstract base class for identity parts that derive their value from a named <see cref="ApiProperty"/> on the declaring object type.
/// </summary>
/// <param name="apiPropertyName">The API property name used to locate the backing <see cref="ApiProperty"/> during initialization.</param>
public abstract class ApiPropertyIdentityPart(string apiPropertyName) : ApiIdentityPart
{
    #region ApiPropertyIdentityPart Fields
    /// <summary>The resolved <see cref="ApiProperty"/> backing this identity part. Set during initialization; <see langword="null"/> before or if resolution fails.</summary>
    private ApiProperty? _apiResolvedProperty = null;
    #endregion

    #region ApiPropertyIdentityPart Properties
    /// <summary>Gets the API property name used to locate the backing property on the declaring object type.</summary>
    public string ApiPropertyName { get; } = apiPropertyName;

    /// <summary>Gets the resolved <see cref="ApiProperty"/> that backs this identity part. Available after initialization.</summary>
    public ApiProperty ApiProperty => this.ThrowIfNotInitialized(_apiResolvedProperty);

    /// <summary>Gets the resolved <see cref="ApiProperty"/> that backs this identity part.</summary>
    protected ApiProperty? ApiResolvedProperty => _apiResolvedProperty;
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: this.ApiPropertyName);

    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiPropertyName(context);
        this.InitializeApiProperty(context);
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
            return;
        }

        var apiDeclaringObjectType = context.ApiDeclaringObjectType;
        if (apiDeclaringObjectType.TryGetPropertyByApiName(this.ApiPropertyName, out var apiResolvedProperty))
        {
            _apiResolvedProperty = apiResolvedProperty;
        }

        if (_apiResolvedProperty is null)
        {
            var apiObjectTypeName = apiDeclaringObjectType.ApiName.SafeToString();

            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_API_PROPERTY;
            var description = $"{nameof(this.ApiProperty)} could not be resolved for {nameof(this.ApiPropertyName)}='{this.ApiPropertyName.SafeToString()}'";
            var remediation = $"Verify that {nameof(this.ApiPropertyName)} refers to a valid property on {nameof(ApiObjectType)}[\"{apiObjectTypeName}\"]";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion
}
