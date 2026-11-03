// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Relationships.Internal;
using Evoogle.ApiFramework.Schema.Relationships;
using Evoogle.ApiFramework.Schema.Types;

namespace Evoogle.ApiFramework.Schema.Configuration.Relationships;

/// <summary>Configures the API name of a relationship traversal while preserving its CLR binding.</summary>
public sealed class ApiRelationshipTraversalBuilder
{
    #region Fields
    private ApiRelationshipTraversalState _state;
    private ApiConfigurationSource _apiNameSource;
    private readonly ApiConfigurationSourceScope _configurationSourceScope = new();
    #endregion

    #region Properties
    /// <summary>Gets the configured traversal API name.</summary>
    public string ApiName => _state.ApiName;

    /// <summary>Gets the CLR navigation binding, or null for an unbound traversal.</summary>
    public ClrMemberReference? ClrNavigationMember => _state.ClrNavigationMember;
    #endregion

    #region Constructors
    /// <summary>Creates a traversal with an explicitly configured API name.</summary>
    /// <param name="apiName">The traversal API name exposed on its source object type.</param>
    /// <param name="clrNavigationMember">The optional CLR navigation binding.</param>
    public ApiRelationshipTraversalBuilder(string apiName, ClrMemberReference? clrNavigationMember = null)
        : this(apiName, clrNavigationMember, ApiConfigurationSource.Explicit)
    {
    }

    internal ApiRelationshipTraversalBuilder
    (
        string apiName,
        ClrMemberReference? clrNavigationMember,
        ApiConfigurationSource source
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        _state = new ApiRelationshipTraversalState(apiName, clrNavigationMember);
        _apiNameSource = source;
    }
    #endregion

    #region With Methods
    /// <summary>Sets the traversal API name using the active configuration precedence.</summary>
    /// <param name="apiName">The traversal API name.</param>
    /// <returns>The current builder.</returns>
    public ApiRelationshipTraversalBuilder WithName(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        var source = _configurationSourceScope.CurrentSource;
        if (source >= _apiNameSource)
        {
            _state = new ApiRelationshipTraversalState(apiName, this.ClrNavigationMember);
            _apiNameSource = source;
        }
        return this;
    }
    #endregion

    #region Configuration Methods
    internal void ApplyConfiguration(ApiConfigurationSource source, Action configure)
        => _configurationSourceScope.Apply(source, configure);

    internal void SetApiNameConvention(string apiName)
        => this.ApplyConfiguration(ApiConfigurationSource.Convention, () => this.WithName(apiName));

    internal void BindNavigation(ClrMemberReference navigation)
        => _state = new ApiRelationshipTraversalState(this.ApiName, navigation);
    #endregion

    #region Build Methods
    internal ApiRelationshipTraversal Build()
        => _state.Build();
    #endregion

    #region Build Copy Methods
    internal ApiRelationshipTraversalBuilder CreateBuildCopy()
        => new(this.ApiName, this.ClrNavigationMember, _apiNameSource);
    #endregion
}
