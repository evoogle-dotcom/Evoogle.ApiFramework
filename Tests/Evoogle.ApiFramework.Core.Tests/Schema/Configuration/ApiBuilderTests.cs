// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema.Configuration;

public abstract class ApiBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    public class TestClass
    {
        public string RequiredName { get; set; } = string.Empty;
        public int? OptionalAge { get; set; }
    }

    public enum TestEnum
    {
        First,
        Second
    }

    public class TestExtension
    {
        public bool Flag { get; set; }

        public static TestExtension Instance() => new() { Flag = true };
    }
    #endregion
}

