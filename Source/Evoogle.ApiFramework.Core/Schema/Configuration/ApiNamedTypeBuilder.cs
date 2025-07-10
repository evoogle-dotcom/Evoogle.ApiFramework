// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public abstract class ApiNamedTypeBuilder<TBuilder>(Type clrType, ApiSchemaBuilderContext context) : IApiNamedTypeBuilder
    where TBuilder : ApiNamedTypeBuilder<TBuilder>
{
    #region Fields
    private string? _apiName;
    #endregion

    #region IApiNamedTypeBuilder Properties
    public string ApiName => _apiName ?? throw new InvalidOperationException("API name has not been set.");
    public Type ClrType { get; } = clrType ?? throw new ArgumentNullException(nameof(clrType));
    #endregion

    #region Properties
    protected ApiSchemaBuilderContext Context { get; } = context ?? throw new ArgumentNullException(nameof(context));
    #endregion

    #region Builder Methods
    public TBuilder WithApiName(string apiName)
    {
        _apiName = apiName ?? throw new ArgumentNullException(nameof(apiName));
        return (TBuilder)this;
    }
    #endregion
}
