// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Schema.Configuration.Query;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>Convention applied to each configured query-source parameter.</summary>
public interface IApiQuerySourceParameterConvention : IApiConvention
{
    #region Methods
    /// <summary>Applies the convention to a query-source parameter builder.</summary>
    /// <param name="builder">The query-source parameter builder.</param>
    /// <param name="context">The query-source parameter convention context.</param>
    void Apply(ApiQuerySourceParameterBuilder builder, ApiQuerySourceParameterConventionContext context);
    #endregion
}
