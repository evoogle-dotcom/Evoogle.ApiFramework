// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Schema.Configuration.Query;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>Convention applied to each configured query source.</summary>
public interface IApiQuerySourceConvention : IApiConvention
{
    #region Methods
    /// <summary>Applies the convention to a query source builder.</summary>
    /// <param name="builder">The query source builder.</param>
    /// <param name="context">The query source convention context.</param>
    void Apply(ApiQuerySourceBuilder builder, ApiQuerySourceConventionContext context);
    #endregion
}
