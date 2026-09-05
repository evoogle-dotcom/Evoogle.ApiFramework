// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiConfigurationTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private enum ConfigurationKind
    {
        Object,
        Scalar,
        Enum,
        OneToOne,
        OneToMany,
        ManyToMany,
    }

    private enum RelationshipRegistrationTarget
    {
        SchemaBuilder,
        GenericObjectBuilder,
        ObjectBuilder,
    }

    private sealed class ConfigurationObject
    {
    }

    private sealed class ConfigurationScalar
    {
    }

    private enum ConfigurationEnum
    {
        First,
    }

    private sealed class RelationshipPrincipal
    {
        public int Id { get; set; }
    }

    private sealed class RelationshipDependent
    {
        public int Id { get; set; }
        public int PrincipalId { get; set; }
    }

    private sealed class RelationshipAssociation
    {
        public int PrincipalId { get; set; }
        public int DependentId { get; set; }
    }

    private sealed class NonGenericObjectConfiguration : IApiObjectTypeConfiguration
    {
        public Type ClrType => typeof(ConfigurationObject);

        public int ConfigureCount { get; private set; }

        public void Configure(ApiObjectTypeBuilder builder)
        {
            this.ConfigureCount++;
            builder.WithName("ConfigurationObject");
        }
    }

    private sealed class NonGenericScalarConfiguration : IApiScalarTypeConfiguration
    {
        public Type ClrType => typeof(ConfigurationScalar);

        public int ConfigureCount { get; private set; }

        public void Configure(ApiScalarTypeBuilder builder)
        {
            this.ConfigureCount++;
            builder.WithName("ConfigurationScalar");
        }
    }

    private sealed class NonGenericEnumConfiguration : IApiEnumTypeConfiguration
    {
        public Type ClrType => typeof(ConfigurationEnum);

        public int ConfigureCount { get; private set; }

        public void Configure(ApiEnumTypeBuilder builder)
        {
            this.ConfigureCount++;
            builder
                .WithName("ConfigurationEnum")
                .AddValue("First", "First", 0);
        }
    }

    private sealed class GenericObjectConfiguration : IApiObjectTypeConfiguration<ConfigurationObject>
    {
        public int ConfigureCount { get; private set; }

        public void Configure(ApiObjectTypeBuilder<ConfigurationObject> builder)
        {
            this.ConfigureCount++;
            builder.WithName("ConfigurationObject");
        }
    }

    private sealed class GenericScalarConfiguration : IApiScalarTypeConfiguration<ConfigurationScalar>
    {
        public int ConfigureCount { get; private set; }

        public void Configure(ApiScalarTypeBuilder<ConfigurationScalar> builder)
        {
            this.ConfigureCount++;
            builder.WithName("ConfigurationScalar");
        }
    }

    private sealed class GenericEnumConfiguration : IApiEnumTypeConfiguration<ConfigurationEnum>
    {
        public int ConfigureCount { get; private set; }

        public void Configure(ApiEnumTypeBuilder<ConfigurationEnum> builder)
        {
            this.ConfigureCount++;
            builder
                .WithName("ConfigurationEnum")
                .AddValue("First", "First", 0);
        }
    }

    private sealed class OneToOneConfiguration : IApiRelationshipOneToOneConfiguration
    {
        public string ApiName => "ConfigurationOneToOne";

        public int ConfigureCount { get; private set; }

        public void Configure(ApiRelationshipOneToOneBuilder builder)
        {
            this.ConfigureCount++;
            builder
                .From<RelationshipPrincipal>()
                .To<RelationshipDependent>(d => d.WithForeignKey(x => x.PrincipalId));
        }
    }

    private sealed class OneToManyConfiguration : IApiRelationshipOneToManyConfiguration
    {
        public string ApiName => "ConfigurationOneToMany";

        public int ConfigureCount { get; private set; }

        public void Configure(ApiRelationshipOneToManyBuilder builder)
        {
            this.ConfigureCount++;
            builder
                .From<RelationshipPrincipal>()
                .To<RelationshipDependent>(d => d.WithForeignKey(x => x.PrincipalId));
        }
    }

    private sealed class ManyToManyConfiguration : IApiRelationshipManyToManyConfiguration
    {
        public string ApiName => "ConfigurationManyToMany";

        public int ConfigureCount { get; private set; }

        public void Configure(ApiRelationshipManyToManyBuilder builder)
        {
            this.ConfigureCount++;
            builder
                .Between<RelationshipPrincipal>()
                .And<RelationshipDependent>()
                .WithAssociation<RelationshipAssociation>
                (
                    a => a
                        .WithForeignKeyA(x => x.PrincipalId)
                        .WithForeignKeyB(x => x.DependentId)
                );
        }
    }

    private sealed class InvalidRelationshipNameConfiguration : IApiRelationshipOneToOneConfiguration
    {
        public string ApiName => " ";

        public void Configure(ApiRelationshipOneToOneBuilder builder)
        {
        }
    }
    #endregion

    #region Test Classes
    private sealed class ConfigurationHierarchyTest : XUnitTest
    {
        public required Type ConfigurationType { get; init; }
        public required Type IdentityType { get; init; }

        private bool IsConfigurationActual { get; set; }
        private bool HasIdentityActual { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"ConfigurationType: {this.ConfigurationType}");
            this.WriteLine($"IdentityType: {this.IdentityType}");
        }

        protected override void Act()
        {
            this.IsConfigurationActual = typeof(IApiConfiguration).IsAssignableFrom(this.ConfigurationType);
            this.HasIdentityActual = this.IdentityType.IsAssignableFrom(this.ConfigurationType);
        }

        protected override void Assert()
        {
            this.IsConfigurationActual.Should().BeTrue();
            this.HasIdentityActual.Should().BeTrue();
        }
    }

    private sealed class NonGenericIdentityTest : XUnitTest
    {
        public required ConfigurationKind ConfigurationKind { get; init; }

        private Type? ClrTypeActual { get; set; }
        private string? ApiNameActual { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"ConfigurationKind: {this.ConfigurationKind}");
        }

        protected override void Act()
        {
            switch (this.ConfigurationKind)
            {
                case ConfigurationKind.Object:
                    this.ClrTypeActual = new NonGenericObjectConfiguration().ClrType;
                    break;
                case ConfigurationKind.Scalar:
                    this.ClrTypeActual = new NonGenericScalarConfiguration().ClrType;
                    break;
                case ConfigurationKind.Enum:
                    this.ClrTypeActual = new NonGenericEnumConfiguration().ClrType;
                    break;
                case ConfigurationKind.OneToOne:
                    this.ApiNameActual = new OneToOneConfiguration().ApiName;
                    break;
                case ConfigurationKind.OneToMany:
                    this.ApiNameActual = new OneToManyConfiguration().ApiName;
                    break;
                case ConfigurationKind.ManyToMany:
                    this.ApiNameActual = new ManyToManyConfiguration().ApiName;
                    break;
            }
        }

        protected override void Assert()
        {
            switch (this.ConfigurationKind)
            {
                case ConfigurationKind.Object:
                    this.ClrTypeActual.Should().Be(typeof(ConfigurationObject));
                    break;
                case ConfigurationKind.Scalar:
                    this.ClrTypeActual.Should().Be(typeof(ConfigurationScalar));
                    break;
                case ConfigurationKind.Enum:
                    this.ClrTypeActual.Should().Be(typeof(ConfigurationEnum));
                    break;
                case ConfigurationKind.OneToOne:
                    this.ApiNameActual.Should().Be("ConfigurationOneToOne");
                    break;
                case ConfigurationKind.OneToMany:
                    this.ApiNameActual.Should().Be("ConfigurationOneToMany");
                    break;
                case ConfigurationKind.ManyToMany:
                    this.ApiNameActual.Should().Be("ConfigurationManyToMany");
                    break;
            }
        }
    }

    private sealed class SchemaBuilderConfigurationTest : XUnitTest
    {
        public required ConfigurationKind ConfigurationKind { get; init; }

        private Type? BuilderClrTypeActual { get; set; }
        private int ConfigureCountActual { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"ConfigurationKind: {this.ConfigurationKind}");
        }

        protected override void Act()
        {
            var schemaBuilder = new ApiSchemaBuilder();
            switch (this.ConfigurationKind)
            {
                case ConfigurationKind.Object:
                    var objectConfiguration = new NonGenericObjectConfiguration();
                    schemaBuilder.AddObject(objectConfiguration);
                    this.BuilderClrTypeActual = schemaBuilder.Context.ApiObjectTypeBuilders.Single().ClrType;
                    this.ConfigureCountActual = objectConfiguration.ConfigureCount;
                    break;
                case ConfigurationKind.Scalar:
                    var scalarConfiguration = new NonGenericScalarConfiguration();
                    schemaBuilder.AddScalar(scalarConfiguration);
                    this.BuilderClrTypeActual = schemaBuilder.Context.ApiScalarTypeBuilders.Single().ClrType;
                    this.ConfigureCountActual = scalarConfiguration.ConfigureCount;
                    break;
                case ConfigurationKind.Enum:
                    var enumConfiguration = new NonGenericEnumConfiguration();
                    schemaBuilder.AddEnum(enumConfiguration);
                    this.BuilderClrTypeActual = schemaBuilder.Context.ApiEnumTypeBuilders.Single().ClrType;
                    this.ConfigureCountActual = enumConfiguration.ConfigureCount;
                    break;
            }
        }

        protected override void Assert()
        {
            var expectedClrType = this.ConfigurationKind switch
            {
                ConfigurationKind.Object => typeof(ConfigurationObject),
                ConfigurationKind.Scalar => typeof(ConfigurationScalar),
                ConfigurationKind.Enum => typeof(ConfigurationEnum),
                _ => throw new InvalidOperationException(),
            };

            this.BuilderClrTypeActual.Should().Be(expectedClrType);
            this.ConfigureCountActual.Should().Be(1);
        }
    }

    private sealed class GenericConfigurationBridgeTest : XUnitTest
    {
        public required ConfigurationKind ConfigurationKind { get; init; }

        private Type? ClrTypeActual { get; set; }
        private string? ApiNameActual { get; set; }
        private int ConfigureCountActual { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"ConfigurationKind: {this.ConfigurationKind}");
        }

        protected override void Act()
        {
            var schemaBuilder = new ApiSchemaBuilder();
            switch (this.ConfigurationKind)
            {
                case ConfigurationKind.Object:
                    var objectConfiguration = new GenericObjectConfiguration();
                    this.ClrTypeActual = ((IApiTypeConfiguration)objectConfiguration).ClrType;
                    ApiSchemaBuilderExtensions.AddObject(schemaBuilder, objectConfiguration);
                    this.ApiNameActual = schemaBuilder.Context.ApiObjectTypeBuilders.Single().ApiName;
                    this.ConfigureCountActual = objectConfiguration.ConfigureCount;
                    break;
                case ConfigurationKind.Scalar:
                    var scalarConfiguration = new GenericScalarConfiguration();
                    this.ClrTypeActual = ((IApiTypeConfiguration)scalarConfiguration).ClrType;
                    ApiSchemaBuilderExtensions.AddScalar(schemaBuilder, scalarConfiguration);
                    this.ApiNameActual = schemaBuilder.Context.ApiScalarTypeBuilders.Single().ApiName;
                    this.ConfigureCountActual = scalarConfiguration.ConfigureCount;
                    break;
                case ConfigurationKind.Enum:
                    var enumConfiguration = new GenericEnumConfiguration();
                    this.ClrTypeActual = ((IApiTypeConfiguration)enumConfiguration).ClrType;
                    ApiSchemaBuilderExtensions.AddEnum(schemaBuilder, enumConfiguration);
                    this.ApiNameActual = schemaBuilder.Context.ApiEnumTypeBuilders.Single().ApiName;
                    this.ConfigureCountActual = enumConfiguration.ConfigureCount;
                    break;
            }
        }

        protected override void Assert()
        {
            var expectedClrType = this.ConfigurationKind switch
            {
                ConfigurationKind.Object => typeof(ConfigurationObject),
                ConfigurationKind.Scalar => typeof(ConfigurationScalar),
                ConfigurationKind.Enum => typeof(ConfigurationEnum),
                _ => throw new InvalidOperationException(),
            };
            var expectedApiName = this.ConfigurationKind switch
            {
                ConfigurationKind.Object => "ConfigurationObject",
                ConfigurationKind.Scalar => "ConfigurationScalar",
                ConfigurationKind.Enum => "ConfigurationEnum",
                _ => throw new InvalidOperationException(),
            };

            this.ClrTypeActual.Should().Be(expectedClrType);
            this.ApiNameActual.Should().Be(expectedApiName);
            this.ConfigureCountActual.Should().Be(1);
        }
    }

    private sealed class RelationshipConfigurationTest : XUnitTest
    {
        public required ConfigurationKind ConfigurationKind { get; init; }
        public required RelationshipRegistrationTarget RegistrationTarget { get; init; }

        private ApiSchema? ApiSchemaActual { get; set; }
        private string? ApiNameExpected { get; set; }
        private int ConfigureCountActual { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"ConfigurationKind: {this.ConfigurationKind}");
            this.WriteLine($"RegistrationTarget: {this.RegistrationTarget}");
        }

        protected override void Act()
        {
            var schemaBuilder = new ApiSchemaBuilder()
                .WithName("ConfigurationRelationshipSchema")
                .AddScalar<int>();

            switch (this.ConfigurationKind)
            {
                case ConfigurationKind.OneToOne:
                    var oneToOneConfiguration = new OneToOneConfiguration();
                    this.ApiNameExpected = oneToOneConfiguration.ApiName;
                    this.AddOneToOneConfiguration(schemaBuilder, oneToOneConfiguration);
                    this.ConfigureCountActual = oneToOneConfiguration.ConfigureCount;
                    break;
                case ConfigurationKind.OneToMany:
                    var oneToManyConfiguration = new OneToManyConfiguration();
                    this.ApiNameExpected = oneToManyConfiguration.ApiName;
                    this.AddOneToManyConfiguration(schemaBuilder, oneToManyConfiguration);
                    this.ConfigureCountActual = oneToManyConfiguration.ConfigureCount;
                    break;
                case ConfigurationKind.ManyToMany:
                    var manyToManyConfiguration = new ManyToManyConfiguration();
                    this.ApiNameExpected = manyToManyConfiguration.ApiName;
                    this.AddManyToManyConfiguration(schemaBuilder, manyToManyConfiguration);
                    this.ConfigureCountActual = manyToManyConfiguration.ConfigureCount;
                    break;
            }

            this.ApiSchemaActual = schemaBuilder.Build();
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();
            this.ApiSchemaActual!.ApiRelationships.Should().ContainSingle();
            this.ApiSchemaActual.ApiRelationships.Single().ApiName.Should().Be(this.ApiNameExpected);
            this.ConfigureCountActual.Should().Be(1);
        }

        private void AddOneToOneConfiguration
        (
            ApiSchemaBuilder schemaBuilder,
            OneToOneConfiguration configuration
        )
        {
            this.AddRelationshipPrincipal(schemaBuilder, configuration);
        }

        private void AddOneToManyConfiguration
        (
            ApiSchemaBuilder schemaBuilder,
            OneToManyConfiguration configuration
        )
        {
            this.AddRelationshipPrincipal(schemaBuilder, configuration);
        }

        private void AddManyToManyConfiguration
        (
            ApiSchemaBuilder schemaBuilder,
            ManyToManyConfiguration configuration
        )
        {
            switch (this.RegistrationTarget)
            {
                case RelationshipRegistrationTarget.SchemaBuilder:
                    schemaBuilder.AddObject<RelationshipPrincipal>(ConfigurePrincipal);
                    schemaBuilder.AddManyToManyRelationship(configuration);
                    break;
                case RelationshipRegistrationTarget.GenericObjectBuilder:
                    schemaBuilder.AddObject<RelationshipPrincipal>(builder =>
                    {
                        ConfigurePrincipal(builder);
                        builder.AddManyToManyRelationship(configuration);
                    });
                    break;
                case RelationshipRegistrationTarget.ObjectBuilder:
                    schemaBuilder.AddObject(typeof(RelationshipPrincipal), builder =>
                    {
                        ConfigurePrincipal(builder);
                        builder.AddManyToManyRelationship(configuration);
                    });
                    break;
            }

            AddRelationshipTypes(schemaBuilder, includeAssociation: true);
        }

        private void AddRelationshipPrincipal<TConfiguration>
        (
            ApiSchemaBuilder schemaBuilder,
            TConfiguration configuration
        )
            where TConfiguration : IApiRelationshipConfiguration
        {
            switch (this.RegistrationTarget)
            {
                case RelationshipRegistrationTarget.SchemaBuilder:
                    schemaBuilder.AddObject<RelationshipPrincipal>(ConfigurePrincipal);
                    switch (configuration)
                    {
                        case OneToOneConfiguration oneToOne:
                            schemaBuilder.AddOneToOneRelationship(oneToOne);
                            break;
                        case OneToManyConfiguration oneToMany:
                            schemaBuilder.AddOneToManyRelationship(oneToMany);
                            break;
                    }
                    break;
                case RelationshipRegistrationTarget.GenericObjectBuilder:
                    schemaBuilder.AddObject<RelationshipPrincipal>(builder =>
                    {
                        ConfigurePrincipal(builder);
                        switch (configuration)
                        {
                            case OneToOneConfiguration oneToOne:
                                builder.AddOneToOneRelationship(oneToOne);
                                break;
                            case OneToManyConfiguration oneToMany:
                                builder.AddOneToManyRelationship(oneToMany);
                                break;
                        }
                    });
                    break;
                case RelationshipRegistrationTarget.ObjectBuilder:
                    schemaBuilder.AddObject(typeof(RelationshipPrincipal), builder =>
                    {
                        ConfigurePrincipal(builder);
                        switch (configuration)
                        {
                            case OneToOneConfiguration oneToOne:
                                builder.AddOneToOneRelationship(oneToOne);
                                break;
                            case OneToManyConfiguration oneToMany:
                                builder.AddOneToManyRelationship(oneToMany);
                                break;
                        }
                    });
                    break;
            }

            AddRelationshipTypes(schemaBuilder, includeAssociation: false);
        }

        private static void ConfigurePrincipal(ApiObjectTypeBuilder<RelationshipPrincipal> builder)
        {
            builder
                .AddProperty(p => p.Id)
                .AddKey("PrimaryKey", p => p.Id);
        }

        private static void ConfigurePrincipal(ApiObjectTypeBuilder builder)
        {
            builder
                .AddProperty("Id", "Id")
                .AddKey("PrimaryKey", p => p.AddPath(typeof(RelationshipPrincipal), "Id"));
        }

        private static void AddRelationshipTypes(ApiSchemaBuilder schemaBuilder, bool includeAssociation)
        {
            schemaBuilder
                .AddObject<RelationshipDependent>(builder => builder
                    .AddProperty(p => p.Id)
                    .AddProperty(p => p.PrincipalId)
                    .AddKey("PrimaryKey", p => p.Id));

            if (includeAssociation)
            {
                schemaBuilder.AddObject<RelationshipAssociation>(builder => builder
                    .AddProperty(p => p.PrincipalId)
                    .AddProperty(p => p.DependentId)
                    .AddKey("PrimaryKey", p => p.PrincipalId, p => p.DependentId));
            }
        }
    }

    private sealed class NullConfigurationTest : XUnitTest
    {
        public required ConfigurationKind ConfigurationKind { get; init; }
        public bool IsObjectBuilderExtension { get; init; }

        private Exception? ExceptionActual { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"ConfigurationKind: {this.ConfigurationKind}");
            this.WriteLine($"IsObjectBuilderExtension: {this.IsObjectBuilderExtension}");
        }

        protected override void Act()
        {
            try
            {
                if (this.IsObjectBuilderExtension)
                {
                    var objectBuilder = new ApiObjectTypeBuilder
                    (
                        typeof(RelationshipPrincipal),
                        new ApiSchemaBuilderContext()
                    );

                    switch (this.ConfigurationKind)
                    {
                        case ConfigurationKind.OneToOne:
                            objectBuilder.AddOneToOneRelationship(null!);
                            break;
                        case ConfigurationKind.OneToMany:
                            objectBuilder.AddOneToManyRelationship(null!);
                            break;
                        case ConfigurationKind.ManyToMany:
                            objectBuilder.AddManyToManyRelationship(null!);
                            break;
                    }
                }
                else
                {
                    var schemaBuilder = new ApiSchemaBuilder();
                    switch (this.ConfigurationKind)
                    {
                        case ConfigurationKind.Object:
                            schemaBuilder.AddObject(null!);
                            break;
                        case ConfigurationKind.Scalar:
                            schemaBuilder.AddScalar(null!);
                            break;
                        case ConfigurationKind.Enum:
                            schemaBuilder.AddEnum(null!);
                            break;
                        case ConfigurationKind.OneToOne:
                            schemaBuilder.AddOneToOneRelationship(null!);
                            break;
                        case ConfigurationKind.OneToMany:
                            schemaBuilder.AddOneToManyRelationship(null!);
                            break;
                        case ConfigurationKind.ManyToMany:
                            schemaBuilder.AddManyToManyRelationship(null!);
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                this.ExceptionActual = exception;
            }
        }

        protected override void Assert()
        {
            this.ExceptionActual.Should().BeOfType<ArgumentNullException>();
            ((ArgumentNullException)this.ExceptionActual!).ParamName.Should().Be("configuration");
        }
    }

    private sealed class InvalidRelationshipNameTest : XUnitTest
    {
        public bool IsObjectBuilderExtension { get; init; }

        private Exception? ExceptionActual { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"IsObjectBuilderExtension: {this.IsObjectBuilderExtension}");
        }

        protected override void Act()
        {
            try
            {
                var configuration = new InvalidRelationshipNameConfiguration();
                if (this.IsObjectBuilderExtension)
                {
                    var objectBuilder = new ApiObjectTypeBuilder
                    (
                        typeof(RelationshipPrincipal),
                        new ApiSchemaBuilderContext()
                    );
                    objectBuilder.AddOneToOneRelationship(configuration);
                }
                else
                {
                    new ApiSchemaBuilder().AddOneToOneRelationship(configuration);
                }
            }
            catch (Exception exception)
            {
                this.ExceptionActual = exception;
            }
        }

        protected override void Assert()
        {
            this.ExceptionActual.Should().BeOfType<ArgumentException>();
            ((ArgumentException)this.ExceptionActual!).ParamName.Should().Be("apiName");
        }
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] ConfigurationHierarchyTheoryData =>
    [
        new ConfigurationHierarchyTest
        {
            Name = "Object configuration inherits the shared configuration abstractions",
            ConfigurationType = typeof(IApiObjectTypeConfiguration),
            IdentityType = typeof(IApiTypeConfiguration),
        },
        new ConfigurationHierarchyTest
        {
            Name = "Scalar configuration inherits the shared configuration abstractions",
            ConfigurationType = typeof(IApiScalarTypeConfiguration),
            IdentityType = typeof(IApiTypeConfiguration),
        },
        new ConfigurationHierarchyTest
        {
            Name = "Enum configuration inherits the shared configuration abstractions",
            ConfigurationType = typeof(IApiEnumTypeConfiguration),
            IdentityType = typeof(IApiTypeConfiguration),
        },
        new ConfigurationHierarchyTest
        {
            Name = "One-to-one configuration inherits the shared configuration abstractions",
            ConfigurationType = typeof(IApiRelationshipOneToOneConfiguration),
            IdentityType = typeof(IApiRelationshipConfiguration),
        },
        new ConfigurationHierarchyTest
        {
            Name = "One-to-many configuration inherits the shared configuration abstractions",
            ConfigurationType = typeof(IApiRelationshipOneToManyConfiguration),
            IdentityType = typeof(IApiRelationshipConfiguration),
        },
        new ConfigurationHierarchyTest
        {
            Name = "Many-to-many configuration inherits the shared configuration abstractions",
            ConfigurationType = typeof(IApiRelationshipManyToManyConfiguration),
            IdentityType = typeof(IApiRelationshipConfiguration),
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] NonGenericIdentityTheoryData =>
    [
        new NonGenericIdentityTest { Name = "Non-generic object configuration exposes its CLR type", ConfigurationKind = ConfigurationKind.Object },
        new NonGenericIdentityTest { Name = "Non-generic scalar configuration exposes its CLR type", ConfigurationKind = ConfigurationKind.Scalar },
        new NonGenericIdentityTest { Name = "Non-generic enum configuration exposes its CLR type", ConfigurationKind = ConfigurationKind.Enum },
        new NonGenericIdentityTest { Name = "One-to-one configuration exposes its API name", ConfigurationKind = ConfigurationKind.OneToOne },
        new NonGenericIdentityTest { Name = "One-to-many configuration exposes its API name", ConfigurationKind = ConfigurationKind.OneToMany },
        new NonGenericIdentityTest { Name = "Many-to-many configuration exposes its API name", ConfigurationKind = ConfigurationKind.ManyToMany },
    ];

    public static TheoryDataRow<IXUnitTest>[] SchemaBuilderConfigurationTheoryData =>
    [
        new SchemaBuilderConfigurationTest { Name = "Schema builder uses object configuration CLR identity", ConfigurationKind = ConfigurationKind.Object },
        new SchemaBuilderConfigurationTest { Name = "Schema builder uses scalar configuration CLR identity", ConfigurationKind = ConfigurationKind.Scalar },
        new SchemaBuilderConfigurationTest { Name = "Schema builder uses enum configuration CLR identity", ConfigurationKind = ConfigurationKind.Enum },
    ];

    public static TheoryDataRow<IXUnitTest>[] GenericConfigurationBridgeTheoryData =>
    [
        new GenericConfigurationBridgeTest { Name = "Generic object configuration uses its default CLR type and typed builder", ConfigurationKind = ConfigurationKind.Object },
        new GenericConfigurationBridgeTest { Name = "Generic scalar configuration uses its default CLR type and typed builder", ConfigurationKind = ConfigurationKind.Scalar },
        new GenericConfigurationBridgeTest { Name = "Generic enum configuration uses its default CLR type and typed builder", ConfigurationKind = ConfigurationKind.Enum },
    ];

    public static TheoryDataRow<IXUnitTest>[] RelationshipConfigurationTheoryData =>
    [
        .. CreateRelationshipTheoryData(ConfigurationKind.OneToOne),
        .. CreateRelationshipTheoryData(ConfigurationKind.OneToMany),
        .. CreateRelationshipTheoryData(ConfigurationKind.ManyToMany),
    ];

    public static TheoryDataRow<IXUnitTest>[] NullConfigurationTheoryData =>
    [
        new NullConfigurationTest { Name = "Schema builder rejects null object configuration", ConfigurationKind = ConfigurationKind.Object },
        new NullConfigurationTest { Name = "Schema builder rejects null scalar configuration", ConfigurationKind = ConfigurationKind.Scalar },
        new NullConfigurationTest { Name = "Schema builder rejects null enum configuration", ConfigurationKind = ConfigurationKind.Enum },
        new NullConfigurationTest { Name = "Schema builder rejects null one-to-one configuration", ConfigurationKind = ConfigurationKind.OneToOne },
        new NullConfigurationTest { Name = "Schema builder rejects null one-to-many configuration", ConfigurationKind = ConfigurationKind.OneToMany },
        new NullConfigurationTest { Name = "Schema builder rejects null many-to-many configuration", ConfigurationKind = ConfigurationKind.ManyToMany },
        new NullConfigurationTest { Name = "Object builder rejects null one-to-one configuration", ConfigurationKind = ConfigurationKind.OneToOne, IsObjectBuilderExtension = true },
        new NullConfigurationTest { Name = "Object builder rejects null one-to-many configuration", ConfigurationKind = ConfigurationKind.OneToMany, IsObjectBuilderExtension = true },
        new NullConfigurationTest { Name = "Object builder rejects null many-to-many configuration", ConfigurationKind = ConfigurationKind.ManyToMany, IsObjectBuilderExtension = true },
    ];

    public static TheoryDataRow<IXUnitTest>[] InvalidRelationshipNameTheoryData =>
    [
        new InvalidRelationshipNameTest
        {
            Name = "Schema builder validates a relationship configuration API name",
        },
        new InvalidRelationshipNameTest
        {
            Name = "Object builder validates a relationship configuration API name",
            IsObjectBuilderExtension = true,
        },
    ];

    private static IEnumerable<TheoryDataRow<IXUnitTest>> CreateRelationshipTheoryData
    (
        ConfigurationKind configurationKind
    )
    {
        yield return new RelationshipConfigurationTest
        {
            Name = $"Schema builder applies {configurationKind} configuration identity",
            ConfigurationKind = configurationKind,
            RegistrationTarget = RelationshipRegistrationTarget.SchemaBuilder,
        };
        yield return new RelationshipConfigurationTest
        {
            Name = $"Generic object builder applies {configurationKind} configuration identity",
            ConfigurationKind = configurationKind,
            RegistrationTarget = RelationshipRegistrationTarget.GenericObjectBuilder,
        };
        yield return new RelationshipConfigurationTest
        {
            Name = $"Object builder applies {configurationKind} configuration identity",
            ConfigurationKind = configurationKind,
            RegistrationTarget = RelationshipRegistrationTarget.ObjectBuilder,
        };
    }
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(ConfigurationHierarchyTheoryData))]
    public void ConfigurationHierarchy(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(NonGenericIdentityTheoryData))]
    public void NonGenericIdentity(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(SchemaBuilderConfigurationTheoryData))]
    public void SchemaBuilderConfiguration(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(GenericConfigurationBridgeTheoryData))]
    public void GenericConfigurationBridge(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(RelationshipConfigurationTheoryData))]
    public void RelationshipConfiguration(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(NullConfigurationTheoryData))]
    public void NullConfiguration(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(InvalidRelationshipNameTheoryData))]
    public void InvalidRelationshipName(IXUnitTest test) => test.Execute(this);
    #endregion
}
