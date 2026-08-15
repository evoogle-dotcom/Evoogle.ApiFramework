// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Expressions;
using System.Text.Json.Serialization;

using Evoogle.Extensions;
using Evoogle.XUnit.Json;

namespace Evoogle.ApiFramework.Schema.TestData;

/// <summary>Builds the actual <see cref="ApiSchema"/> by compiling and invoking a caller-supplied build expression.</summary>
public abstract class ApiSchemaExpressionBuildTest : ApiSchemaBuildTestBase
{
    #region User Supplied Properties
    [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiSchema>))]
    public required Expression<Func<ApiSchema>> ApiSchemaActualBuildExpression { get; init; } = null!;
    #endregion

    #region XUnitTest Methods
    protected override void Act()
    {
        var buildLambda = this.ApiSchemaActualBuildExpression.Compile();

        try
        {
            var apiSchemaActual = buildLambda();
            this.ApiSchemaActual = apiSchemaActual;

            this.WriteLine("ApiSchemaActual:");
            this.WriteLine($"{this.ApiSchemaActual.SafeToJson(_defaultToJsonOptions)}");
        }
        catch (Exception exception)
        {
            this.ExceptionActual = exception;
            this.WriteLine($"ExceptionActual: [{exception.GetType().SafeToName()}] " + exception.Message);
        }
    }
    #endregion
}
