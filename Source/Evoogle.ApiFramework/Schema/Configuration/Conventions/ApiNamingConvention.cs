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
public abstract class ApiNamingConvention(ApiNamingConventionTargets targets = ApiNamingConventionTargets.All) :
    IApiNamingConvention,
    IApiObjectTypeConvention,
    IApiScalarTypeConvention,
    IApiEnumTypeConvention,
    IApiEnumValueConvention,
    IApiPropertyConvention
{

    #region Properties
    /// <summary>Gets the schema element kinds to which the convention applies.</summary>
    protected ApiNamingConventionTargets Targets { get; } = ValidateTargets(targets);
    #endregion

    #region IApiConvention Properties
    /// <inheritdoc />
    public abstract ApiConventionPhase Phase { get; }
    #endregion

    #region IApiObjectTypeConvention Methods
    /// <inheritdoc />
    public void Apply(ApiObjectTypeBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (!this.IsTargeted(ApiNamingConventionTargets.ObjectType))
        {
            return;
        }

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

        if (!this.IsTargeted(ApiNamingConventionTargets.ScalarType))
        {
            return;
        }

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

        if (!this.IsTargeted(ApiNamingConventionTargets.EnumType))
        {
            return;
        }

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

        if (!this.IsTargeted(ApiNamingConventionTargets.EnumValue))
        {
            return;
        }

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

        if (!this.IsTargeted(ApiNamingConventionTargets.Property))
        {
            return;
        }

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

    #region Private Helper Methods
    /// <summary>
    ///     Determines whether the convention applies to the specified target.
    /// </summary>
    /// <param name="target">The target to check.</param>
    /// <returns>True if the target is included in the convention's targets; otherwise, false.</returns>
    private bool IsTargeted(ApiNamingConventionTargets target)
    {
        return (this.Targets & target) != ApiNamingConventionTargets.None;
    }

    /// <summary>
    ///     Validates that the specified targets contain only known convention target values.
    /// </summary>
    /// <param name="targets">The targets to validate.</param>
    /// <returns>The validated targets.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when targets contains unknown values.</exception>
    private static ApiNamingConventionTargets ValidateTargets(ApiNamingConventionTargets targets)
    {
        if ((targets & ~ApiNamingConventionTargets.All) != ApiNamingConventionTargets.None)
        {
            throw new ArgumentOutOfRangeException(nameof(targets), targets, "Unknown naming convention target.");
        }

        return targets;
    }
    #endregion
}
