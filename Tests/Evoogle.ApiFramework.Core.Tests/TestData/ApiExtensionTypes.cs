// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.TestData;

#region Extension Types
public class GraphQlExtension
{
    public int Count { get; set; } = 42;

    public override string ToString()
    {
        var count = this.Count.SafeToString();

        return $"{nameof(GraphQlExtension)} {{{nameof(this.Count)}={count}}}";
    }
}

public class JsonApiExtension
{
    public string Website { get; set; } = "http://jsonapi.org";

    public override string ToString()
    {
        var website = this.Website.SafeToString();

        return $"{nameof(JsonApiExtension)} {{{nameof(this.Website)}={website}}}";
    }
}

public class ProtobufExtension
{
    public int Edition { get; set; } = 2024;

    public override string ToString()
    {
        var edition = this.Edition.SafeToString();

        return $"{nameof(ProtobufExtension)} {{{nameof(this.Edition)}={edition}}}";
    }
}

public class TestExtension
{
    public bool Flag { get; set; } = true;

    public override string ToString()
    {
        var flag = this.Flag.SafeToString();

        return $"{nameof(TestExtension)} {{{nameof(this.Flag)}={flag}}}";
    }
}

public class TestExtension1
{
    public string Description { get; set; } = nameof(TestExtension1);

    public override string ToString()
    {
        var description = this.Description.SafeToString();

        return $"{nameof(TestExtension1)} {{{nameof(this.Description)}={description}}}";
    }
}

public class TestExtension2
{
    public string Id { get; set; } = "2";
    public string Name { get; set; } = nameof(TestExtension2);

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var name = this.Name.SafeToString();

        return $"{nameof(TestExtension2)} {{{nameof(this.Id)}={id}, {nameof(this.Name)}={name}}}";
    }
}
#endregion
