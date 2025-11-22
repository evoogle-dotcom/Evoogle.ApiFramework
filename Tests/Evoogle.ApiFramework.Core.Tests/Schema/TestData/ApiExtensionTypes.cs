// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.TestData;

#region Extension Types
public class TestExtension
{
    public bool Flag { get; set; } = true;
}

public class TestExtension1
{
    public string Description { get; set; } = nameof(TestExtension1);
}

public class TestExtension2
{
    public string Id { get; set; } = "2";
    public string Name { get; set; } = nameof(TestExtension2);
}
#endregion
