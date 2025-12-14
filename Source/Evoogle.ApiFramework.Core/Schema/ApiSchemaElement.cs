// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extension;

namespace Evoogle.ApiFramework.Schema;

public abstract class ApiSchemaElement : ExtensibleBase
{
    #region Fields
    private string? _apiPath = null;

    private ApiSchemaContext? _apiSchemaContext = null;
    #endregion

    #region Properties
    public string ApiPath => this.ThrowIfNotInitialized(_apiPath);

    protected ApiSchemaContext ApiSchemaContext => this.ThrowIfNotInitialized(_apiSchemaContext);
    #endregion

    #region Methods
    protected abstract string BuildPath(string? apiParentPath);

    internal virtual void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _apiPath = this.BuildPath(context.ApiParentPath);
        _apiSchemaContext = context.ApiSchema.ApiSchemaContext;
    }
    #endregion
}
