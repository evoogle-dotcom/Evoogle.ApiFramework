// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics.CodeAnalysis;

using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.XUnit;

using static Evoogle.ApiFramework.Schema.TestData.ApiSchemaFactory;
using static Evoogle.XUnit.Tests.JsonUnitTests;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private class JsonDeserializeTest : JsonDeserializeTest<ApiType, ApiTypeDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonDeserializeTest()
        {
            this.Name = nameof(JsonDeserializeTest);
            this.ExpectedFactoryExpression = (arg) => BuildTestApiType(arg);
            this.ExcludeMembers = ApiSchemaExcludeMembers.Standard;
        }
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiType, ApiTypeDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonRoundtripTest()
        {
            this.Name = nameof(JsonRoundtripTest);
            this.ExpectedFactoryExpression = (arg) => BuildTestApiType(arg);
            this.ExcludeMembers = ApiSchemaExcludeMembers.Standard;
        }
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiType, ApiTypeDescriptor>
    {
        #region Constructors
        [SetsRequiredMembers]
        public JsonSerializeTest()
        {
            this.Name = nameof(JsonSerializeTest);
            this.SourceFactoryExpression = (arg) => BuildTestApiType(arg);
        }
        #endregion
    }
    #endregion
}
