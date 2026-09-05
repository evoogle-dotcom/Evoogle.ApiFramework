// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.Extensions;
using Evoogle.ApiFramework.Schema;

namespace Evoogle.ApiFramework.TestData;

#region Extension Types
public class GraphQlExtension : IApiSchemaExtension
{
    public int Count { get; set; } = 42;

    public IApiSchemaExtension CreateFrozenSnapshot() => new GraphQlExtension { Count = this.Count };

    public override string ToString()
    {
        var count = this.Count.SafeToString();

        return $"{nameof(GraphQlExtension)} {{{nameof(this.Count)}={count}}}";
    }
}

public class JsonApiExtension : IApiSchemaExtension
{
    public string Website { get; set; } = "http://jsonapi.org";

    public IApiSchemaExtension CreateFrozenSnapshot() => new JsonApiExtension { Website = this.Website };

    public override string ToString()
    {
        var website = this.Website.SafeToString();

        return $"{nameof(JsonApiExtension)} {{{nameof(this.Website)}={website}}}";
    }
}

public class ProtobufExtension : IApiSchemaExtension
{
    public int Edition { get; set; } = 2024;

    public IApiSchemaExtension CreateFrozenSnapshot() => new ProtobufExtension { Edition = this.Edition };

    public override string ToString()
    {
        var edition = this.Edition.SafeToString();

        return $"{nameof(ProtobufExtension)} {{{nameof(this.Edition)}={edition}}}";
    }
}

public class TestExtension : IApiSchemaExtension
{
    public bool Flag { get; set; } = true;

    public IApiSchemaExtension CreateFrozenSnapshot() => new TestExtension { Flag = this.Flag };

    public override string ToString()
    {
        var flag = this.Flag.SafeToString();

        return $"{nameof(TestExtension)} {{{nameof(this.Flag)}={flag}}}";
    }
}

public class TestExtension1 : IApiSchemaExtension
{
    public string Description { get; set; } = nameof(TestExtension1);

    public IApiSchemaExtension CreateFrozenSnapshot() => new TestExtension1 { Description = this.Description };

    public override string ToString()
    {
        var description = this.Description.SafeToString();

        return $"{nameof(TestExtension1)} {{{nameof(this.Description)}={description}}}";
    }
}

public class TestExtension2 : IApiSchemaExtension
{
    public string Id { get; set; } = "2";
    public string Name { get; set; } = nameof(TestExtension2);

    public IApiSchemaExtension CreateFrozenSnapshot() => new TestExtension2
    {
        Id = this.Id,
        Name = this.Name
    };

    public override string ToString()
    {
        var id = this.Id.SafeToString();
        var name = this.Name.SafeToString();

        return $"{nameof(TestExtension2)} {{{nameof(this.Id)}={id}, {nameof(this.Name)}={name}}}";
    }
}
#endregion
