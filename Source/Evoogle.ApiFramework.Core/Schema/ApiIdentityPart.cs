// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

public sealed class ApiIdentityPart(string apiPropertyName, ApiIdentityCoercion? coercion = null, bool emitAsOrdered = false) : ApiSchemaElement
{
    #region Fields
    private ApiProperty? _apiResolvedProperty = null;
    #endregion

    #region Properties
    public string ApiPropertyName { get; } = apiPropertyName;
    public ApiIdentityCoercion? Coercion { get; } = coercion;
    public bool EmitAsOrdered { get; } = emitAsOrdered;

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
            var description = $"{nameof(this.ApiPropertyName)} cannot be null, empty, or whitespace";
            var remediation = $"Provide a valid {nameof(this.ApiPropertyName)}";

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
            var description = $"{nameof(this.ApiProperty)} is unresolved for {nameof(this.ApiPropertyName)}='{this.ApiPropertyName.SafeToString()}'";
            var remediation = $"Ensure that {nameof(this.ApiPropertyName)} refers to a valid property on the parent {nameof(ApiObjectType)}";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        // TODO: What to do here? Have to comment out for now.
        // Basic coercion compatibility
        // if (this.Coercion?.TargetKind is ApiIdentityTargetKind tk && !IsCoercible(_apiResolvedProperty, tk))
        // {
        //     results ??= [];
        //     results.Add(new ValidationResult($"{apiValidationPath} property '{this.ApiPropertyName}' not coercible to {tk}.", [nameof(this.ApiPropertyName)]));
        // }
    }

    private static bool IsCoercible(ApiProperty apiProperty, ApiIdentityTargetKind target)
    {
        var clrPropertyType = apiProperty.ApiType.ClrType;
        return target switch
        {
            ApiIdentityTargetKind.String => clrPropertyType == typeof(string),
            ApiIdentityTargetKind.Int32 => clrPropertyType == typeof(int),
            ApiIdentityTargetKind.Int64 => clrPropertyType == typeof(long) || clrPropertyType == typeof(int),
            ApiIdentityTargetKind.Guid => clrPropertyType == typeof(Guid),
            ApiIdentityTargetKind.Ulid => clrPropertyType == typeof(Ulid),
            ApiIdentityTargetKind.Culture => clrPropertyType == typeof(System.Globalization.CultureInfo) || clrPropertyType == typeof(string),
            _ => false
        };
    }
    #endregion
}
