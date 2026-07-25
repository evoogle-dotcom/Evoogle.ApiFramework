// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Base class for naming conventions that apply to named schema type builders, enum-value
///     builders, and property builders by transforming the current candidate API name or deriving
///     a replacement from CLR metadata.
/// </summary>
public abstract class ApiNamingConvention :
    IApiNamingConvention,
    IApiObjectTypeConvention,
    IApiScalarTypeConvention,
    IApiEnumTypeConvention,
    IApiEnumValueConvention,
    IApiPropertyConvention
{
    #region IApiObjectTypeConvention Methods
    /// <inheritdoc />
    public void Apply(ApiObjectTypeBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var context = new ApiNamingConventionContext
        (
            ApiNamingConventionTarget.ObjectType,
            builder.ClrType
        );

        builder.SetApiNameConvention(this.ConvertName(builder.ApiName, context));
    }
    #endregion

    #region IApiScalarTypeConvention Methods
    /// <inheritdoc />
    public void Apply(ApiScalarTypeBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var context = new ApiNamingConventionContext
        (
            ApiNamingConventionTarget.ScalarType,
            builder.ClrType
        );

        builder.SetApiNameConvention(this.ConvertName(builder.ApiName, context));
    }
    #endregion

    #region IApiEnumTypeConvention Methods
    /// <inheritdoc />
    public void Apply(ApiEnumTypeBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var context = new ApiNamingConventionContext
        (
            ApiNamingConventionTarget.EnumType,
            builder.ClrType
        );

        builder.SetApiNameConvention(this.ConvertName(builder.ApiName, context));
    }
    #endregion

    #region IApiEnumValueConvention Methods
    /// <inheritdoc />
    public void Apply(ApiEnumValueBuilder builder, ApiEnumValueConventionContext context)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(context);

        var namingContext = new ApiNamingConventionContext
        (
            ApiNamingConventionTarget.EnumValue,
            context.ClrEnumType,
            builder.ClrName,
            null,
            context
        );

        builder.SetApiNameConvention(this.ConvertName(builder.ApiName, namingContext));
    }
    #endregion

    #region IApiPropertyConvention Methods
    /// <inheritdoc />
    public void Apply(ApiPropertyBuilder builder, ApiPropertyConventionContext context)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(context);

        var namingContext = new ApiNamingConventionContext
        (
            ApiNamingConventionTarget.Property,
            context.ClrDeclaringType,
            builder.ClrName,
            context
        );

        builder.SetApiNameConvention(this.ConvertName(builder.ApiName, namingContext));
    }
    #endregion

    #region IApiNamingConvention Methods
    /// <inheritdoc />
    public abstract string ConvertName(string apiName, ApiNamingConventionContext context);
    #endregion
}
