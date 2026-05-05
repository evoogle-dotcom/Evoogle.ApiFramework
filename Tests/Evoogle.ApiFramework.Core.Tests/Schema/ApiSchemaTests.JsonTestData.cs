// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiSchemaTests
{
    #region Test Data
    private record ApiSchemaJsonTestCase
    {
        public required string Name { get; init; }
        public required ApiSchemaDef? FactoryArgument { get; init; }
        public required string Json { get; init; }
    }
    #endregion
}
