// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
namespace Evoogle.ApiFramework.Schema.Configuration.Query;

/// <summary>Configures the query-source declaration identified by a CLR delegate.</summary>
/// <typeparam name="TDelegate">The public closed delegate that defines the source signature.</typeparam>
public interface IApiQuerySourceConfiguration<TDelegate> : IApiConfiguration where TDelegate : Delegate
{
    #region Methods
    /// <summary>Configures API-facing facets of the delegate declaration.</summary>
    /// <param name="builder">The query-source builder.</param>
    void Configure(ApiQuerySourceBuilder builder);
    #endregion
}
