// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Holds ordered lists of conventions for each supported schema target.
/// </summary>
/// <remarks>
///     A convention's <see cref="IApiConvention.Phase"/> determines when it runs. The pipeline
///     snapshots these lists at the beginning of each build and executes phases in this order:
///     <list type="number">
///       <item>
///         <see cref="ApiConventionPhase.Discovery"/> conventions discover types and object
///         properties.
///       </item>
///       <item>
///         Annotation readers configure each discovered type or property at data-annotation
///         precedence.
///       </item>
///       <item>
///         <see cref="ApiConventionPhase.Configuration"/> conventions configure annotated types,
///         enum values, and properties until the structural model settles.
///       </item>
///       <item>
///         Relationship annotations are applied, followed by
///         <see cref="ApiConventionPhase.Relationship"/> conventions.
///       </item>
///     </list>
///     Registration order is preserved within each target and phase. A later convention may
///     override an earlier convention at the same configuration-source precedence.
/// </remarks>
public sealed class ApiConventionSet
{
    #region Properties
    /// <summary>Gets schema-level discovery conventions that may register new types.</summary>
    public IList<IApiSchemaConvention> DiscoveryConventions { get; } = [];

    /// <summary>
    ///     Gets object-type conventions. These may participate in discovery or configuration.
    /// </summary>
    public IList<IApiObjectTypeConvention> ObjectTypeConventions { get; } = [];

    /// <summary>Gets scalar-type configuration conventions.</summary>
    public IList<IApiScalarTypeConvention> ScalarTypeConventions { get; } = [];

    /// <summary>Gets enum-type configuration conventions.</summary>
    public IList<IApiEnumTypeConvention> EnumTypeConventions { get; } = [];

    /// <summary>
    ///     Gets enum-value configuration conventions, run after the declaring enum type has been
    ///     annotated and configured.
    /// </summary>
    public IList<IApiEnumValueConvention> EnumValueConventions { get; } = [];

    /// <summary>
    ///     Gets property configuration conventions, run after each property has been annotated
    ///     and until no unprocessed property builders remain.
    /// </summary>
    public IList<IApiPropertyConvention> PropertyConventions { get; } = [];

    /// <summary>
    ///     Gets relationship conventions, run after all structural builders and relationship
    ///     annotations have settled.
    /// </summary>
    public IList<IApiRelationshipConvention> RelationshipConventions { get; } = [];
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates an <see cref="ApiConventionSet"/> pre-populated with the default conventions.
    /// </summary>
    /// <param name="namingConvention">
    ///     An optional naming convention to apply to all supported schema targets.
    ///     When <see langword="null"/>, no naming convention is added.
    /// </param>
    /// <returns>A new <see cref="ApiConventionSet"/> with default conventions registered.</returns>
    public static ApiConventionSet CreateDefault(ApiNamingConvention? namingConvention = null)
    {
        var set = new ApiConventionSet();

        if (namingConvention is not null)
        {
            set.AddNamingConvention(namingConvention);
        }

        set.EnumTypeConventions.Add(new ApiEnumTypeEnumValueDiscoveryConvention());
        set.ObjectTypeConventions.Add(new ApiObjectTypePropertyDiscoveryConvention());
        set.ObjectTypeConventions.Add(new ApiObjectTypePrimaryKeyInferenceConvention());
        set.PropertyConventions.Add(new ApiPropertyNullabilityModifierConvention());

        return set;
    }

    /// <summary>
    ///     Adds a naming convention to all supported schema targets.
    /// </summary>
    /// <param name="convention">
    ///     The naming convention to add.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="convention"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    ///     This method is a convenience for adding a single naming convention to all supported schema targets.
    /// </remarks>
    public void AddNamingConvention(ApiNamingConvention convention)
    {
        ArgumentNullException.ThrowIfNull(convention);

        this.ScalarTypeConventions.Add(convention);

        this.EnumTypeConventions.Add(convention);
        this.EnumValueConventions.Add(convention);

        this.ObjectTypeConventions.Add(convention);
        this.PropertyConventions.Add(convention);
    }
    #endregion
}
