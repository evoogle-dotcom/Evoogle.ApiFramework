// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.TestData;

/// <summary>Builds the expected <see cref="ApiSchema"/> from a built-in <see cref="TestData.ApiSchemaKind"/> fixture.</summary>
public class ApiSchemaExpressionBuildThrowsTest : ApiSchemaExpressionBuildTest
{
    #region User Supplied Properties
    public new required Type ExceptionTypeExpected
    {
        get => base.ExceptionTypeExpected!;
        init => base.ExceptionTypeExpected = value;
    }

    public new required string ExceptionMessagePatternExpected
    {
        get => base.ExceptionMessagePatternExpected!;
        init => base.ExceptionMessagePatternExpected = value;
    }
    #endregion

    #region Constructors
    public ApiSchemaExpressionBuildThrowsTest() => this.Name = nameof(ApiSchemaExpressionBuildThrowsTest);
    #endregion

    #region XUnitTest Methods
    protected override void Arrange()
    {
        this.WriteLine($"ExceptionExpected: [{this.ExceptionTypeExpected.SafeToName()}] " + this.ExceptionMessagePatternExpected);
        this.WriteLine();
    }
    #endregion
}
