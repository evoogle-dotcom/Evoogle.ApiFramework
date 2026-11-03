// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Schema.Configuration.Relationships;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>Configures relationship traversals after relationship inference and navigation binding.</summary>
/// <remarks>
///     Implementations use <see cref="ApiConventionPhase.Configuration"/> and run in registration order
///     against each traversal. They may configure traversal names while preserving relationship structure.
/// </remarks>
public interface IApiRelationshipTraversalConvention : IApiConvention
{
    #region Methods
    /// <summary>Applies configuration to the supplied relationship traversal.</summary>
    /// <param name="builder">The traversal builder.</param>
    /// <param name="context">The source CLR metadata and owning relationship context.</param>
    void Apply(ApiRelationshipTraversalBuilder builder, ApiRelationshipTraversalConventionContext context);
    #endregion
}
