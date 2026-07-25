// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Fluent builder used to compose an <see cref="ApiConventionSet"/>.
/// </summary>
public sealed class ApiConventionSetBuilder
{
    #region Fields
    private readonly List<IApiSchemaConvention> _discoveryConventions = [];
    private readonly List<IApiObjectTypeConvention> _objectTypeConventions = [];
    private readonly List<IApiScalarTypeConvention> _scalarTypeConventions = [];
    private readonly List<IApiEnumTypeConvention> _enumTypeConventions = [];
    private readonly List<IApiEnumValueConvention> _enumValueConventions = [];
    private readonly List<IApiPropertyConvention> _propertyConventions = [];
    private readonly List<IApiRelationshipConvention> _relationshipConventions = [];
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new empty <see cref="ApiConventionSetBuilder"/>.
    /// </summary>
    public ApiConventionSetBuilder()
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="ApiConventionSetBuilder"/> pre-populated from an existing
    ///     <see cref="ApiConventionSet"/>, allowing additive customization.
    /// </summary>
    /// <param name="existing">The existing convention set to copy conventions from.</param>
    public ApiConventionSetBuilder(ApiConventionSet existing)
    {
        ArgumentNullException.ThrowIfNull(existing);

        _discoveryConventions.AddRange(existing.DiscoveryConventions);
        _objectTypeConventions.AddRange(existing.ObjectTypeConventions);
        _scalarTypeConventions.AddRange(existing.ScalarTypeConventions);
        _enumTypeConventions.AddRange(existing.EnumTypeConventions);
        _enumValueConventions.AddRange(existing.EnumValueConventions);
        _propertyConventions.AddRange(existing.PropertyConventions);
        _relationshipConventions.AddRange(existing.RelationshipConventions);
    }
    #endregion

    #region Methods
    /// <summary>
    ///     Adds a convention to the appropriate stage list(s) based on which convention interfaces
    ///     it implements. A single convention may implement multiple interfaces.
    /// </summary>
    /// <param name="convention">The convention to add.</param>
    /// <returns>The current builder instance.</returns>
    public ApiConventionSetBuilder AddConvention(IApiConvention convention)
    {
        ArgumentNullException.ThrowIfNull(convention);

        AddConventionIfSupported(convention, _discoveryConventions);
        AddConventionIfSupported(convention, _objectTypeConventions);
        AddConventionIfSupported(convention, _scalarTypeConventions);
        AddConventionIfSupported(convention, _enumTypeConventions);
        AddConventionIfSupported(convention, _enumValueConventions);
        AddConventionIfSupported(convention, _propertyConventions);
        AddConventionIfSupported(convention, _relationshipConventions);

        return this;
    }

    /// <summary>
    ///     Removes all conventions of the specified type from all stage lists.
    /// </summary>
    /// <typeparam name="TConvention">The concrete convention type to remove.</typeparam>
    /// <returns>The current builder instance.</returns>
    public ApiConventionSetBuilder RemoveConvention<TConvention>()
        where TConvention : IApiConvention
    {
        _discoveryConventions.RemoveAll(c => c is TConvention);
        _objectTypeConventions.RemoveAll(c => c is TConvention);
        _scalarTypeConventions.RemoveAll(c => c is TConvention);
        _enumTypeConventions.RemoveAll(c => c is TConvention);
        _enumValueConventions.RemoveAll(c => c is TConvention);
        _propertyConventions.RemoveAll(c => c is TConvention);
        _relationshipConventions.RemoveAll(c => c is TConvention);
        return this;
    }

    /// <summary>
    ///     Builds and returns the configured <see cref="ApiConventionSet"/>.
    /// </summary>
    /// <returns>The built convention set.</returns>
    public ApiConventionSet Build()
    {
        var set = new ApiConventionSet();

        foreach (var c in _discoveryConventions)
        {
            set.DiscoveryConventions.Add(c);
        }

        foreach (var c in _objectTypeConventions)
        {
            set.ObjectTypeConventions.Add(c);
        }

        foreach (var c in _scalarTypeConventions)
        {
            set.ScalarTypeConventions.Add(c);
        }

        foreach (var c in _enumTypeConventions)
        {
            set.EnumTypeConventions.Add(c);
        }

        foreach (var c in _enumValueConventions)
        {
            set.EnumValueConventions.Add(c);
        }

        foreach (var c in _propertyConventions)
        {
            set.PropertyConventions.Add(c);
        }

        foreach (var c in _relationshipConventions)
        {
            set.RelationshipConventions.Add(c);
        }

        return set;
    }

    private static void AddConventionIfSupported<TConvention>
    (
        IApiConvention convention,
        ICollection<TConvention> conventions
    )
        where TConvention : class, IApiConvention
    {
        if (convention is TConvention typedConvention)
        {
            conventions.Add(typedConvention);
        }
    }
    #endregion
}
