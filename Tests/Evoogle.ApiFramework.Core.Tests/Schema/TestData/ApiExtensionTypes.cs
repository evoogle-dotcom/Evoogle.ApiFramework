// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.TestData;

#region Extension Types
public class GraphQlExtension
{
    public int Count { get; set; } = 42;
}

public class JsonApiExtension
{
    public string Website { get; set; } = "http://jsonapi.org";
}

public class ProtobufExtension
{
    public int Edition { get; set; } = 2024;
}

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
