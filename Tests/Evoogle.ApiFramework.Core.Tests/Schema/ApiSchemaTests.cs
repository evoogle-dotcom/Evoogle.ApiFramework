using FluentAssertions;
using Evoogle.ApiFramework.Schema;

namespace Evoogle.ApiFramework.Schema.Tests;

public class ApiSchemaTests
{
    private class Person
    {
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void Constructor_IgnoresNonNamedTypes()
    {
        var types = new ApiType[]
        {
            new ApiScalarType("Boolean", typeof(bool)),
            new ApiCollectionType(new ApiScalarType("Boolean", typeof(bool)), ApiTypeModifiers.None, typeof(List<bool>))
        };

        var schema = new ApiSchema(types);
        schema.ApiTypes.Should().ContainSingle(t => t is ApiScalarType);
    }

    [Fact]
    public void TryGetApiObjectType_ByApiName()
    {
        var personType = new ApiObjectType(
            nameof(Person),
            [new ApiProperty("Name", new ApiScalarType("String", typeof(string)), ApiTypeModifiers.Required, nameof(Person.Name))],
            typeof(Person));

        var schema = new ApiSchema([personType]);

        var found = schema.TryGetApiObjectType(nameof(Person), out var result);

        found.Should().BeTrue();
        result.Should().BeSameAs(personType);
    }
}

