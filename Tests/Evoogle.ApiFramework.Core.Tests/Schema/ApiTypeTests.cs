// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
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
        public JsonDeserializeTest()
        {
            this.ExcludeMembers = ApiSchemaExcludeMembers.Standard;
        }
        #endregion

        #region JsonDeserializeTest<T, TFactoryArg> Methods
        protected override ApiType? CreateExpected(ApiTypeDescriptor? descriptor)
        {
            return BuildTestApiType(descriptor);
        }
        #endregion
    }

    private class JsonRoundtripTest : JsonRoundtripTest<ApiType, ApiTypeDescriptor>
    {
        #region Constructors
        public JsonRoundtripTest()
        {
            this.ExcludeMembers = ApiSchemaExcludeMembers.Standard;
        }

        #endregion

        #region JsonRoundtripTest<T, TFactoryArg> Methods
        protected override ApiType? CreateExpected(ApiTypeDescriptor? descriptor)
        {
            return BuildTestApiType(descriptor);
        }
        #endregion
    }

    private class JsonSerializeTest : JsonSerializeTest<ApiType, ApiTypeDescriptor>
    {
        #region JsonSerializeTest<T, TFactoryArg> Methods
        protected override ApiType? CreateSource(ApiTypeDescriptor? descriptor)
        {
            return BuildTestApiType(descriptor);
        }
        #endregion
    }
    #endregion
}
