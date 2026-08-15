// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.TestData;

/// <summary>Builds the expected <see cref="ApiSchema"/> by deserializing a caller-supplied JSON literal.</summary>
public class ApiSchemaJsonExpressionBuildTest : ApiSchemaExpressionBuildTest
{
    #region User Supplied Properties
    public required string ApiSchemaExpectedJson { get; init; }
    #endregion

    #region Constructors
    public ApiSchemaJsonExpressionBuildTest() => this.Name = nameof(ApiSchemaJsonExpressionBuildTest);
    #endregion

    #region XUnitTest Methods
    protected override void Arrange()
    {
        this.ApiSchemaExpected = JsonSerializer.Deserialize<ApiSchema>(this.ApiSchemaExpectedJson);

        this.WriteLine("ApiSchemaExpected:");
        this.WriteLine($"{this.ApiSchemaExpected.SafeToJson(_defaultToJsonOptions)}");
        this.WriteLine();
    }
    #endregion
}
