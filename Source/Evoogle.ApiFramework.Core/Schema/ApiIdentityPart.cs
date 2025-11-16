// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extension;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

public sealed class ApiIdentityPart(string apiPropertyName, ApiIdentityCoercion? coercion = null, bool emitAsOrdered = false) : ExtensibleBase
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

    #region ApiIdentityPart Methods
    internal void Initialize(ApiSchema apiSchema, ApiObjectType apiObjectType, string apiValidationPath, ref List<ValidationResult>? results)
    {
        ArgumentNullException.ThrowIfNull(apiSchema);
        ArgumentNullException.ThrowIfNull(apiObjectType);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiValidationPath);

        this.InitializeApiPropertyName(apiValidationPath, ref results);
        this.InitializeApiProperty(apiObjectType, apiValidationPath, ref results);
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
    private void InitializeApiPropertyName(string apiValidationPath, ref List<ValidationResult>? results)
    {
        if (string.IsNullOrWhiteSpace(this.ApiPropertyName))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiPropertyName)} cannot be null or whitespace.", [nameof(this.ApiPropertyName)]));
            return;
        }
    }

    private void InitializeApiProperty(ApiObjectType apiObjectType, string apiValidationPath, ref List<ValidationResult>? results)
    {
        _apiResolvedProperty = null;

        // Resolve the related API property for the parent API object type.
        if (apiObjectType.TryGetPropertyByApiName(this.ApiPropertyName, out var apiResolvedProperty))
        {
            _apiResolvedProperty = apiResolvedProperty;
        }

        if (_apiResolvedProperty is null)
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath}.{nameof(this.ApiProperty)} unable to resolve {nameof(this.ApiProperty)}[\"{this.ApiPropertyName.SafeToString()}\"].", [nameof(this.ApiProperty)]));
            return;
        }

        // Basic coercion compatibility
        if (this.Coercion?.TargetKind is ApiIdentityTargetKind tk && !IsCoercible(_apiResolvedProperty, tk))
        {
            results ??= [];
            results.Add(new ValidationResult($"{apiValidationPath} property '{this.ApiPropertyName}' not coercible to {tk}.", [nameof(this.ApiPropertyName)]));
        }
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
