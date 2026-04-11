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
/// <param name="clrPropertyName">The CLR property name used to locate the backing <see cref="ApiProperty"/> during initialization.</param>
public abstract class ApiIdentityPropertyPart(string clrPropertyName) : ApiIdentityPart
{
    #region ApiIdentityPropertyPart Fields
    private ApiProperty? _apiResolvedProperty = null;
    #endregion

    #region ApiIdentityPropertyPart Properties
    /// <summary>Gets the CLR property name used to locate the backing property on the declaring object type.</summary>
    public string ClrPropertyName { get; } = clrPropertyName;

    /// <summary>Gets the resolved <see cref="ApiProperty"/> that backs this identity part. Available after initialization.</summary>
    public ApiProperty ApiProperty => this.ThrowIfNotInitialized(_apiResolvedProperty);

    /// <summary>Gets the resolved <see cref="ApiProperty"/> that backs this identity part.</summary>
    protected ApiProperty? ApiResolvedProperty => _apiResolvedProperty;
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: this.ClrPropertyName);

    /// <inheritdoc />
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
        var isClrPropertyNameInvalid = ApiSchemaHelpers.IsNameInvalid(this.ClrPropertyName);
        if (isClrPropertyNameInvalid)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_PART_INVALID_CLR_PROPERTY_NAME;
            var description = $"{nameof(this.ClrPropertyName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ClrPropertyName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeApiProperty(ApiInitializationContext context)
    {
        _apiResolvedProperty = null;

        var isClrPropertyNameInvalid = ApiSchemaHelpers.IsNameInvalid(this.ClrPropertyName);
        if (isClrPropertyNameInvalid)
        {
            return;
        }

        var apiDeclaringObjectType = context.ApiDeclaringObjectType;
        if (apiDeclaringObjectType.TryGetPropertyByClrName(this.ClrPropertyName, out var apiResolvedProperty))
        {
            _apiResolvedProperty = apiResolvedProperty;
        }

        if (_apiResolvedProperty is null)
        {
            var apiObjectTypeName = apiDeclaringObjectType.ApiName.SafeToString();

            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_PART_UNRESOLVED_API_PROPERTY;
            var description = $"{nameof(this.ApiProperty)} could not be resolved for {nameof(this.ClrPropertyName)}='{this.ClrPropertyName.SafeToString()}'";
            var remediation = $"Verify that {nameof(this.ClrPropertyName)} refers to a valid property on {nameof(ApiObjectType)}[\"{apiObjectTypeName}\"]";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion
}
