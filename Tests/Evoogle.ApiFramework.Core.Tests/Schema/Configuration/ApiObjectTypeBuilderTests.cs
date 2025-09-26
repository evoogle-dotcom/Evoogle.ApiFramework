// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.Extensions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiObjectTypeBuilderTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Classes
    public class BuildTest : XUnitTest
    {
        #region User Supplied Properties
        public string ApiName { get; init; } = null!;
        public Type ClrType { get; init; } = null!;
        public ApiProperty[] ApiProperties { get; init; } = null!;
        public ApiRelationship[] ApiRelationships { get; init; } = null!;
        public ApiType ApiTypeExpected { get; init; } = null!;
        public Type? ApiExtensionType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiType? ApiTypeActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"ApiName: {this.ApiName.SafeToString()}");
            this.WriteLine($"ClrType: {this.ClrType.SafeToName()}");
            this.WriteLine($"ApiExtensionType: {this.ApiExtensionType.SafeToName()}");
            this.WriteLine();
            this.WriteLine($"Expected: {this.ApiTypeExpected.SafeToString()}");
        }

        protected override void Act()
        {
            var context = new ApiSchemaBuilderContext();
            var builder = new ApiObjectTypeBuilder(this.ClrType, context)
                .WithName(this.ApiName);

            foreach (var apiProperty in this.ApiProperties ?? [])
            {
                builder.AddProperty(apiProperty.ApiName, apiProperty.ClrName);
            }

            foreach (var apiRelationship in this.ApiRelationships ?? [])
            {
                builder.AddRelationship(apiRelationship.ApiName, apiRelationship.ApiPropertyName);
            }

            if (this.ApiExtensionType != null)
            {
                var extension = Activator.CreateInstance(this.ApiExtensionType);
                builder.AddExtension(this.ApiExtensionType, extension!);
            }

            this.ApiTypeActual = builder.Build();
            this.WriteLine($"Actual:   {this.ApiTypeActual.SafeToString()}");
        }

        protected override void Assert()
        {
            this.ApiTypeActual.Should().NotBeNull();

            this.ApiTypeActual.Should().BeOfType<ApiObjectType>();
            this.ApiTypeExpected.Should().BeOfType<ApiObjectType>();

            this.ApiTypeActual.As<ApiObjectType>().Should().BeEquivalentTo
            (
                this.ApiTypeExpected.As<ApiObjectType>(),
                opt => opt
                    .WithStrictOrdering()
                    .Excluding(p => p.DeclaringType == typeof(ApiProperty) && p.Name == nameof(ApiProperty.ApiType))
                    .Excluding(p => p.DeclaringType == typeof(ApiRelationship) && p.Name == nameof(ApiRelationship.ApiProperty))
                    .Excluding(p => p.DeclaringType == typeof(ApiRelationship) && p.Name == nameof(ApiRelationship.ApiCardinality))
            );
        }
        #endregion
    }
    #endregion

    #region Theory Data
    private static ApiObjectType EmptyObjectType { get; } = new ApiObjectType(nameof(Empty), [], [], typeof(Empty));
    private static ApiObjectType EmptyObjectTypeWithExtension { get; } = new ApiObjectType(nameof(Empty), [], [], typeof(Empty))
    {
        Extensions = new OrderedDictionary<Type, object>
        {
            [typeof(TestExtension)] = new TestExtension()
        }
    };

    private static ApiObjectType ScalarsOnlyObjectType { get; } = new ApiObjectType
    (
        nameof(ScalarsOnly),
        [
            new ApiProperty(nameof(ScalarsOnly.RequiredName), ApiTypeExpression.ClrRef<string>(), ApiTypeModifiers.Required, nameof(ScalarsOnly.RequiredName)),
            new ApiProperty(nameof(ScalarsOnly.RequiredNumber), ApiTypeExpression.ClrRef<long>(), ApiTypeModifiers.Required, nameof(ScalarsOnly.RequiredNumber)),
            new ApiProperty(nameof(ScalarsOnly.RequiredPredicate), ApiTypeExpression.ClrRef<bool>(), ApiTypeModifiers.Required, nameof(ScalarsOnly.RequiredPredicate)),
            new ApiProperty(nameof(ScalarsOnly.OptionalName), ApiTypeExpression.ClrRef<string>(), ApiTypeModifiers.None, nameof(ScalarsOnly.OptionalName)),
            new ApiProperty(nameof(ScalarsOnly.OptionalNumber), ApiTypeExpression.ClrRef<long>(), ApiTypeModifiers.None, nameof(ScalarsOnly.OptionalNumber)),
            new ApiProperty(nameof(ScalarsOnly.OptionalPredicate), ApiTypeExpression.ClrRef<bool>(), ApiTypeModifiers.None, nameof(ScalarsOnly.OptionalPredicate)),
        ],
        [],
        typeof(ScalarsOnly)
    );

    private static ApiObjectType ScalarsOnlyObjectTypeWithExtension { get; } = new ApiObjectType
    (
        nameof(ScalarsOnly),
        [
            new ApiProperty(nameof(ScalarsOnly.RequiredName), ApiTypeExpression.ClrRef<string>(), ApiTypeModifiers.Required, nameof(ScalarsOnly.RequiredName)),
            new ApiProperty(nameof(ScalarsOnly.RequiredNumber), ApiTypeExpression.ClrRef<long>(), ApiTypeModifiers.Required, nameof(ScalarsOnly.RequiredNumber)),
            new ApiProperty(nameof(ScalarsOnly.RequiredPredicate), ApiTypeExpression.ClrRef<bool>(), ApiTypeModifiers.Required, nameof(ScalarsOnly.RequiredPredicate)),
            new ApiProperty(nameof(ScalarsOnly.OptionalName), ApiTypeExpression.ClrRef<string>(), ApiTypeModifiers.None, nameof(ScalarsOnly.OptionalName)),
            new ApiProperty(nameof(ScalarsOnly.OptionalNumber), ApiTypeExpression.ClrRef<long>(), ApiTypeModifiers.None, nameof(ScalarsOnly.OptionalNumber)),
            new ApiProperty(nameof(ScalarsOnly.OptionalPredicate), ApiTypeExpression.ClrRef<bool>(), ApiTypeModifiers.None, nameof(ScalarsOnly.OptionalPredicate)),
        ],
        [],
        typeof(ScalarsOnly)
    )
    {
        Extensions = new OrderedDictionary<Type, object>
        {
            [typeof(TestExtension)] = new TestExtension()
        }
    };

    private static ApiObjectType CompanyObjectType { get; } = new ApiObjectType
    (
        nameof(Company),
        [
            new ApiProperty(nameof(Company.Name), ApiTypeExpression.ClrRef<string>(), ApiTypeModifiers.Required, nameof(Company.Name)),
            new ApiProperty(nameof(Company.Owner), ApiTypeExpression.ClrRef<Person>(), ApiTypeModifiers.None, nameof(Company.Owner)),
            new ApiProperty(nameof(Company.Employees), ApiTypeExpression.ListOf<Person>(ApiTypeModifiers.Required), ApiTypeModifiers.None, nameof(Company.Employees)),
        ],
        [
            new ApiRelationship(nameof(Company.Owner)),
            new ApiRelationship(nameof(Company.Employees)),
        ],
        typeof(Company)
    );

    private static ApiObjectType CompanyObjectTypeWithExtension { get; } = new ApiObjectType
    (
        nameof(Company),
        [
            new ApiProperty(nameof(Company.Name), ApiTypeExpression.ClrRef<string>(), ApiTypeModifiers.Required, nameof(Company.Name)),
            new ApiProperty(nameof(Company.Owner), ApiTypeExpression.ClrRef<Person>(), ApiTypeModifiers.None, nameof(Company.Owner)),
            new ApiProperty(nameof(Company.Employees), ApiTypeExpression.ListOf<Person>(ApiTypeModifiers.Required), ApiTypeModifiers.None, nameof(Company.Employees)),
        ],
        [
            new ApiRelationship(nameof(Company.Owner)),
            new ApiRelationship(nameof(Company.Employees)),
        ],
        typeof(Company)
    )
    {
        Extensions = new OrderedDictionary<Type, object>
        {
            [typeof(TestExtension)] = new TestExtension()
        }
    };

    public static TheoryDataRow<IXUnitTest>[] BuildTheoryData =>
    [
        new BuildTest
        {
            Name = $"Builds {EmptyObjectType}",
            ApiName = nameof(Empty),
            ClrType = typeof(Empty),
            ApiProperties = [.. EmptyObjectType.ApiProperties],
            ApiRelationships = [.. EmptyObjectType.ApiRelationships],
            ApiTypeExpected = EmptyObjectType,
        },
        new BuildTest
        {
            Name = $"Builds {EmptyObjectTypeWithExtension} with extension",
            ApiName = nameof(Empty),
            ClrType = typeof(Empty),
            ApiProperties = [.. EmptyObjectTypeWithExtension.ApiProperties],
            ApiRelationships = [.. EmptyObjectTypeWithExtension.ApiRelationships],
            ApiTypeExpected = EmptyObjectTypeWithExtension,
            ApiExtensionType = typeof(TestExtension),
        },
        new BuildTest
        {
            Name = $"Builds {ScalarsOnlyObjectType}",
            ApiName = nameof(ScalarsOnly),
            ClrType = typeof(ScalarsOnly),
            ApiProperties = [.. ScalarsOnlyObjectType.ApiProperties],
            ApiRelationships = [.. ScalarsOnlyObjectType.ApiRelationships],
            ApiTypeExpected = ScalarsOnlyObjectType,
        },
        new BuildTest
        {
            Name = $"Builds {ScalarsOnlyObjectTypeWithExtension} with extension",
            ApiName = nameof(ScalarsOnly),
            ClrType = typeof(ScalarsOnly),
            ApiProperties = [.. ScalarsOnlyObjectTypeWithExtension.ApiProperties],
            ApiRelationships = [.. ScalarsOnlyObjectTypeWithExtension.ApiRelationships],
            ApiTypeExpected = ScalarsOnlyObjectTypeWithExtension,
            ApiExtensionType = typeof(TestExtension),
        },
        new BuildTest
        {
            Name = $"Builds {CompanyObjectType}",
            ApiName = nameof(Company),
            ClrType = typeof(Company),
            ApiProperties = [.. CompanyObjectType.ApiProperties],
            ApiRelationships = [.. CompanyObjectType.ApiRelationships],
            ApiTypeExpected = CompanyObjectType,
        },
        new BuildTest
        {
            Name = $"Builds {CompanyObjectTypeWithExtension} with extension",
            ApiName = nameof(Company),
            ClrType = typeof(Company),
            ApiProperties = [.. CompanyObjectTypeWithExtension.ApiProperties],
            ApiRelationships = [.. CompanyObjectTypeWithExtension.ApiRelationships],
            ApiTypeExpected = CompanyObjectTypeWithExtension,
            ApiExtensionType = typeof(TestExtension),
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);
    #endregion
}

