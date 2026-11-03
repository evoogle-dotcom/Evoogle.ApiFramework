// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Schema.Compilation;

namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

internal sealed class ApiSchemaBuildSession(ApiSchemaBuilder authoringBuilder)
{
    #region Build Methods
    internal ApiSchemaCompilationResult BuildResult()
        => authoringBuilder.CreateBuildSessionBuilder().BuildSessionResult();
    #endregion
}

internal sealed class ApiSchemaBuildGuard
{
    #region Fields
    internal int _isBuilding;
    #endregion
}
