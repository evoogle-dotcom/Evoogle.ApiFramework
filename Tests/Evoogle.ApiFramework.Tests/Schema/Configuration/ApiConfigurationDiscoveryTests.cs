// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Conventions;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration;

public class ApiConfigurationDiscoveryTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    public sealed class DiscoveryMarker
    {
    }

    public sealed class DiscoveryObject
    {
    }

    public sealed class DiscoveryScalar
    {
    }

    public enum DiscoveryEnum
    {
        #region Values
        First,
        #endregion
    }

    public sealed class DiscoveryPrincipal
    {
        #region Properties
        public int Id { get; set; }
        #endregion
    }

    public sealed class DiscoveryDependent
    {
        #region Properties
        public int Id { get; set; }

        public int PrincipalId { get; set; }
        #endregion
    }

    public sealed class DiscoveryAssociation
    {
        #region Properties
        public int PrincipalId { get; set; }

        public int DependentId { get; set; }
        #endregion
    }

    public sealed class AssemblyObjectConfiguration : IApiObjectTypeConfiguration<DiscoveryObject>
    {
        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<DiscoveryObject> builder)
            => builder.WithName("DiscoveredObject");
        #endregion
    }

    public sealed class AssemblyScalarConfiguration : IApiScalarTypeConfiguration<DiscoveryScalar>
    {
        #region Configuration Methods
        public void Configure(ApiScalarTypeBuilder<DiscoveryScalar> builder)
            => builder.WithName("DiscoveredScalar");
        #endregion
    }

    public sealed class AssemblyIntScalarConfiguration : IApiScalarTypeConfiguration<int>
    {
        #region Configuration Methods
        public void Configure(ApiScalarTypeBuilder<int> builder)
            => builder.WithName("Int");
        #endregion
    }

    public sealed class AssemblyEnumConfiguration : IApiEnumTypeConfiguration<DiscoveryEnum>
    {
        #region Configuration Methods
        public void Configure(ApiEnumTypeBuilder<DiscoveryEnum> builder)
        {
            builder
                .WithName("DiscoveredEnum")
                .AddValue("First", nameof(DiscoveryEnum.First), (int)DiscoveryEnum.First);
        }
        #endregion
    }

    public sealed class AssemblyPrincipalConfiguration : IApiObjectTypeConfiguration<DiscoveryPrincipal>
    {
        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<DiscoveryPrincipal> builder)
        {
            builder
                .WithName("DiscoveredPrincipal")
                .AddProperty(static p => p.Id)
                .AddKey("PrimaryKey", static p => p.Id);
        }
        #endregion
    }

    public sealed class AssemblyDependentConfiguration : IApiObjectTypeConfiguration<DiscoveryDependent>
    {
        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<DiscoveryDependent> builder)
        {
            builder
                .WithName("DiscoveredDependent")
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.PrincipalId)
                .AddKey("PrimaryKey", static p => p.Id);
        }
        #endregion
    }

    public sealed class AssemblyAssociationConfiguration : IApiObjectTypeConfiguration<DiscoveryAssociation>
    {
        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<DiscoveryAssociation> builder)
        {
            builder
                .WithName("DiscoveredAssociation")
                .AddProperty(static p => p.PrincipalId)
                .AddProperty(static p => p.DependentId)
                .AddKey("PrimaryKey", static p => p.PrincipalId, static p => p.DependentId);
        }
        #endregion
    }

    public sealed class AssemblyOneToOneConfiguration : IApiRelationshipOneToOneConfiguration
    {
        #region Properties
        public string ApiName => "DiscoveredOneToOne";
        #endregion

        #region Configuration Methods
        public void Configure(ApiRelationshipOneToOneBuilder builder)
        {
            builder
                .From<DiscoveryPrincipal>()
                .To<DiscoveryDependent>(static d => d.WithForeignKey(static x => x.PrincipalId));
        }
        #endregion
    }

    public sealed class AssemblyOneToManyConfiguration : IApiRelationshipOneToManyConfiguration
    {
        #region Properties
        public string ApiName => "DiscoveredOneToMany";
        #endregion

        #region Configuration Methods
        public void Configure(ApiRelationshipOneToManyBuilder builder)
        {
            builder
                .From<DiscoveryPrincipal>()
                .To<DiscoveryDependent>(static d => d.WithForeignKey(static x => x.PrincipalId));
        }
        #endregion
    }

    public sealed class AssemblyManyToManyConfiguration : IApiRelationshipManyToManyConfiguration
    {
        #region Properties
        public string ApiName => "DiscoveredManyToMany";
        #endregion

        #region Configuration Methods
        public void Configure(ApiRelationshipManyToManyBuilder builder)
        {
            builder
                .Between<DiscoveryPrincipal>()
                .And<DiscoveryDependent>()
                .WithAssociation<DiscoveryAssociation>
                (
                    static a => a
                        .WithForeignKeyA(static x => x.PrincipalId)
                        .WithForeignKeyB(static x => x.DependentId)
                );
        }
        #endregion
    }

    public sealed class FreshInstanceConfiguration : IApiObjectTypeConfiguration<DiscoveryObject>
    {
        #region Properties
        public static int ConstructorCount { get; set; }
        #endregion

        #region Constructors
        public FreshInstanceConfiguration()
        {
            ConstructorCount++;
        }
        #endregion

        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<DiscoveryObject> builder)
            => builder.WithName("FreshObject");
        #endregion
    }

    public sealed class EligibleConfiguration : IApiObjectTypeConfiguration<EligibilityObject>
    {
        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<EligibilityObject> builder)
            => builder.WithName("EligibleObject");
        #endregion
    }

    public sealed class EligibilityObject
    {
    }

    public abstract class AbstractConfiguration : IApiObjectTypeConfiguration<EligibilityObject>
    {
        #region Configuration Methods
        public abstract void Configure(ApiObjectTypeBuilder<EligibilityObject> builder);
        #endregion
    }

    public struct StructConfiguration : IApiObjectTypeConfiguration<EligibilityObject>
    {
        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<EligibilityObject> builder)
        {
        }
        #endregion
    }

    public sealed class OpenGenericConfiguration<T> : IApiObjectTypeConfiguration<T>
    {
        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<T> builder)
        {
        }
        #endregion
    }

    internal sealed class NonPublicConfiguration : IApiObjectTypeConfiguration<EligibilityObject>
    {
        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<EligibilityObject> builder)
        {
        }
        #endregion
    }

    public sealed class GenericObjectDiscoveryConfiguration : IApiObjectTypeConfiguration<BridgeObject>
    {
        #region Properties
        public static int ConfigureCount { get; set; }

        public static Type? BuilderType { get; set; }
        #endregion

        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<BridgeObject> builder)
        {
            ConfigureCount++;
            BuilderType = builder.GetType();
            builder.WithName("BridgeObject");
        }
        #endregion
    }

    public sealed class GenericScalarDiscoveryConfiguration : IApiScalarTypeConfiguration<BridgeScalar>
    {
        #region Properties
        public static int ConfigureCount { get; set; }

        public static Type? BuilderType { get; set; }
        #endregion

        #region Configuration Methods
        public void Configure(ApiScalarTypeBuilder<BridgeScalar> builder)
        {
            ConfigureCount++;
            BuilderType = builder.GetType();
            builder.WithName("BridgeScalar");
        }
        #endregion
    }

    public sealed class GenericEnumDiscoveryConfiguration : IApiEnumTypeConfiguration<BridgeEnum>
    {
        #region Properties
        public static int ConfigureCount { get; set; }

        public static Type? BuilderType { get; set; }
        #endregion

        #region Configuration Methods
        public void Configure(ApiEnumTypeBuilder<BridgeEnum> builder)
        {
            ConfigureCount++;
            BuilderType = builder.GetType();
            builder
                .WithName("BridgeEnum")
                .AddValue("First", nameof(BridgeEnum.First), (int)BridgeEnum.First);
        }
        #endregion
    }

    public sealed class BridgeObject
    {
    }

    public sealed class BridgeScalar
    {
    }

    public enum BridgeEnum
    {
        #region Values
        First,
        #endregion
    }

    public sealed class MultiRoleDiscoveryConfiguration :
        IApiObjectTypeConfiguration<MultiRoleObject>,
        IApiRelationshipOneToOneConfiguration
    {
        #region Properties
        public static int ConstructorCount { get; set; }

        public static List<string> AppliedRoles { get; } = [];
        #endregion

        #region Constructors
        public MultiRoleDiscoveryConfiguration()
        {
            ConstructorCount++;
        }
        #endregion

        #region Properties
        public string ApiName => "MultiRoleRelationship";
        #endregion

        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<MultiRoleObject> builder)
        {
            AppliedRoles.Add("Object");
            builder.WithName("MultiRoleObject");
        }

        public void Configure(ApiRelationshipOneToOneBuilder builder)
        {
            AppliedRoles.Add("OneToOne");
            builder
                .From<RolePrincipal>()
                .To<RoleDependent>(static d => d.WithForeignKey(static x => x.PrincipalId));
        }
        #endregion
    }

    public sealed class MultiRoleObject
    {
    }

    public sealed class RolePrincipal
    {
        #region Properties
        public int Id { get; set; }
        #endregion
    }

    public sealed class RoleDependent
    {
        #region Properties
        public int Id { get; set; }

        public int PrincipalId { get; set; }
        #endregion
    }

    public sealed class RolePrincipalConfiguration : IApiObjectTypeConfiguration<RolePrincipal>
    {
        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<RolePrincipal> builder)
        {
            builder
                .WithName("RolePrincipal")
                .AddProperty(static p => p.Id)
                .AddKey("PrimaryKey", static p => p.Id);
        }
        #endregion
    }

    public sealed class RoleDependentConfiguration : IApiObjectTypeConfiguration<RoleDependent>
    {
        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<RoleDependent> builder)
        {
            builder
                .WithName("RoleDependent")
                .AddProperty(static p => p.Id)
                .AddProperty(static p => p.PrincipalId)
                .AddKey("PrimaryKey", static p => p.Id);
        }
        #endregion
    }

    public sealed class RoleScalarConfiguration : IApiScalarTypeConfiguration<int>
    {
        #region Configuration Methods
        public void Configure(ApiScalarTypeBuilder<int> builder)
            => builder.WithName("RoleInt");
        #endregion
    }

    public sealed class FailingRoleDiscoveryConfiguration :
        IApiObjectTypeConfiguration<FailingRoleObject>,
        IApiRelationshipOneToOneConfiguration
    {
        #region Properties
        public static List<string> AppliedRoles { get; } = [];

        public string ApiName => "FailingRoleRelationship";
        #endregion

        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<FailingRoleObject> builder)
        {
            AppliedRoles.Add("Object");
            builder.WithName("FailingRoleObject");
            throw new InvalidOperationException("The object role failed.");
        }

        public void Configure(ApiRelationshipOneToOneBuilder builder)
        {
            AppliedRoles.Add("OneToOne");
            builder
                .From<RolePrincipal>()
                .To<RoleDependent>(static d => d.WithForeignKey(static x => x.PrincipalId));
        }
        #endregion
    }

    public sealed class FailingRoleObject
    {
    }

    public sealed class IdentityFailureConfiguration : IApiRelationshipOneToOneConfiguration
    {
        #region Properties
        public string ApiName => throw new InvalidOperationException("The relationship identity failed.");
        #endregion

        #region Configuration Methods
        public void Configure(ApiRelationshipOneToOneBuilder builder)
        {
        }
        #endregion
    }

    public sealed class ActivationFailureConfiguration : IApiObjectTypeConfiguration<ActivationObject>
    {
        #region Constructors
        public ActivationFailureConfiguration(int value)
        {
        }
        #endregion

        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<ActivationObject> builder)
        {
        }
        #endregion
    }

    public sealed class ActivationObject
    {
    }

    public sealed class MarkerOnlyConfiguration : IApiConfiguration
    {
    }

    public sealed class ADuplicateObjectConfiguration : IApiObjectTypeConfiguration<DuplicateObject>
    {
        #region Properties
        public static List<string> AppliedConfigurations { get; } = [];
        #endregion

        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<DuplicateObject> builder)
        {
            AppliedConfigurations.Add("First");
            builder.WithName("DuplicateObject");
        }
        #endregion
    }

    public sealed class BDuplicateObjectConfiguration : IApiObjectTypeConfiguration<DuplicateObject>
    {
        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<DuplicateObject> builder)
        {
            ADuplicateObjectConfiguration.AppliedConfigurations.Add("Second");
            builder.WithName("DuplicateObject");
        }
        #endregion
    }

    public sealed class DuplicateObject
    {
    }

    public sealed class RepeatedRegistrationConfiguration : IApiObjectTypeConfiguration<RepeatedObject>
    {
        #region Properties
        public static int ConfigureCount { get; set; }
        #endregion

        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<RepeatedObject> builder)
        {
            ConfigureCount++;
            builder.WithName("RepeatedObject");
        }
        #endregion
    }

    public sealed class RepeatedObject
    {
    }

    public sealed class PrecedenceConfiguration : IApiObjectTypeConfiguration<PrecedenceObject>
    {
        #region Configuration Methods
        public void Configure(ApiObjectTypeBuilder<PrecedenceObject> builder)
            => builder.WithName("ExplicitName");
        #endregion
    }

    public sealed class PrecedenceObject
    {
    }

    private enum ConfigurationFailureKind
    {
        #region Values
        Configure,
        Identity,
        #endregion
    }

    private enum NullScanKind
    {
        #region Values
        ExplicitBuilder,
        ExplicitAssembly,
        MarkerBuilder,
        #endregion
    }

    private sealed class ConventionNameOverride : IApiObjectTypeConvention
    {
        #region Properties
        public ApiConventionPhase Phase => ApiConventionPhase.Configuration;
        #endregion

        #region Convention Methods
        public void Apply(ApiObjectTypeBuilder builder)
        {
            if (builder.ClrType == typeof(PrecedenceObject))
            {
                builder.SetApiNameConvention("ConventionName");
            }
        }
        #endregion
    }
    #endregion

    #region Test Classes
    private sealed class AssemblyDiscoveryTest : XUnitTest
    {
        #region User Supplied Properties
        public bool UseMarkerType { get; init; }
        #endregion

        #region Calculated Properties
        private ApiSchema? ApiSchemaActual { get; set; }
        private bool ReturnedSameBuilder { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
            => this.WriteLine($"UseMarkerType: {this.UseMarkerType}");

        protected override void Act()
        {
            var schemaBuilder = new ApiSchemaBuilder().WithName("ConfigurationDiscovery");
            var filter = IncludeTypes
            (
                typeof(AssemblyObjectConfiguration),
                typeof(AssemblyScalarConfiguration),
                typeof(AssemblyIntScalarConfiguration),
                typeof(AssemblyEnumConfiguration),
                typeof(AssemblyPrincipalConfiguration),
                typeof(AssemblyDependentConfiguration),
                typeof(AssemblyAssociationConfiguration),
                typeof(AssemblyOneToOneConfiguration),
                typeof(AssemblyOneToManyConfiguration),
                typeof(AssemblyManyToManyConfiguration)
            );

            var registeredBuilder = this.UseMarkerType
                ? schemaBuilder.UseConfigurationsFromAssemblyOf<DiscoveryMarker>(filter)
                : schemaBuilder.UseConfigurationsFromAssembly(typeof(DiscoveryMarker).Assembly, filter);

            this.ReturnedSameBuilder = ReferenceEquals(schemaBuilder, registeredBuilder);
            this.ApiSchemaActual = schemaBuilder.Build();
        }

        protected override void Assert()
        {
            this.ReturnedSameBuilder.Should().BeTrue();
            this.ApiSchemaActual.Should().NotBeNull();

            this.ApiSchemaActual.ApiObjectTypes.Should().HaveCount(4);
            this.ApiSchemaActual.ApiObjectTypes.Should().ContainSingle(static x => x.ApiName == "DiscoveredObject");
            this.ApiSchemaActual.ApiObjectTypes.Should().ContainSingle(static x => x.ApiName == "DiscoveredPrincipal");
            this.ApiSchemaActual.ApiObjectTypes.Should().ContainSingle(static x => x.ApiName == "DiscoveredDependent");
            this.ApiSchemaActual.ApiObjectTypes.Should().ContainSingle(static x => x.ApiName == "DiscoveredAssociation");

            this.ApiSchemaActual.ApiScalarTypes.Should().HaveCount(2);
            this.ApiSchemaActual.ApiScalarTypes.Should().ContainSingle(static x => x.ApiName == "DiscoveredScalar");
            this.ApiSchemaActual.ApiScalarTypes.Should().ContainSingle(static x => x.ApiName == "Int");

            this.ApiSchemaActual.ApiEnumTypes.Should().ContainSingle(static x => x.ApiName == "DiscoveredEnum");
            this.ApiSchemaActual.ApiRelationships.Should().HaveCount(3);
            this.ApiSchemaActual.ApiRelationships.Should().ContainSingle(static x => x.ApiName == "DiscoveredOneToOne");
            this.ApiSchemaActual.ApiRelationships.Should().ContainSingle(static x => x.ApiName == "DiscoveredOneToMany");
            this.ApiSchemaActual.ApiRelationships.Should().ContainSingle(static x => x.ApiName == "DiscoveredManyToMany");
        }
        #endregion
    }

    private sealed class NullScanArgumentsTest : XUnitTest
    {
        #region User Supplied Properties
        public required NullScanKind ScanKind { get; init; }
        #endregion

        #region Calculated Properties
        private ArgumentNullException? ExceptionActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
            => this.WriteLine($"ScanKind: {this.ScanKind}");

        protected override void Act()
        {
            try
            {
                switch (this.ScanKind)
                {
                    case NullScanKind.ExplicitBuilder:
                        ApiSchemaBuilderExtensions.UseConfigurationsFromAssembly
                        (
                            null!,
                            typeof(DiscoveryMarker).Assembly
                        );
                        break;
                    case NullScanKind.ExplicitAssembly:
                        new ApiSchemaBuilder().UseConfigurationsFromAssembly(null!);
                        break;
                    case NullScanKind.MarkerBuilder:
                        ApiSchemaBuilderExtensions.UseConfigurationsFromAssemblyOf<DiscoveryMarker>(null!);
                        break;
                }
            }
            catch (ArgumentNullException exception)
            {
                this.ExceptionActual = exception;
            }
        }

        protected override void Assert()
        {
            this.ExceptionActual.Should().NotBeNull();
            this.ExceptionActual.ParamName.Should().Be
            (
                this.ScanKind == NullScanKind.ExplicitAssembly
                    ? "assembly"
                    : "builder"
            );
        }
        #endregion
    }

    private sealed class TypeEligibilityTest : XUnitTest
    {
        #region Calculated Properties
        private HashSet<Type> ObservedTypes { get; set; } = [];
        private ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
            => this.ObservedTypes = [];

        protected override void Act()
        {
            var schemaBuilder = new ApiSchemaBuilder()
                .WithName("ConfigurationEligibility")
                .UseConfigurationsFromAssembly
                (
                    typeof(DiscoveryMarker).Assembly,
                    type =>
                    {
                        this.ObservedTypes.Add(type);
                        return type == typeof(EligibleConfiguration);
                    }
                );

            this.ApiSchemaActual = schemaBuilder.Build();
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();
            this.ApiSchemaActual.ApiObjectTypes.Should().ContainSingle(static x => x.ApiName == "EligibleObject");
            this.ObservedTypes.Should().Contain(typeof(EligibleConfiguration));
            this.ObservedTypes.Should().NotContain(typeof(AbstractConfiguration));
            this.ObservedTypes.Should().NotContain(typeof(StructConfiguration));
            this.ObservedTypes.Should().NotContain(typeof(OpenGenericConfiguration<>));
            this.ObservedTypes.Should().NotContain(typeof(NonPublicConfiguration));
        }
        #endregion
    }

    private sealed class BuildLifecycleTest : XUnitTest
    {
        #region Calculated Properties
        private string RestoredScenario { get; set; } = null!;
        private int FirstIssueCount { get; set; }
        private int SecondIssueCount { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario: {this.Name}");
            this.WriteLine();
            this.WriteLine("Expected Exception: None; configuration failures appear in the build result");
            var serializer = new XUnitTestSerializer();
            this.RestoredScenario = ((XUnitTest)serializer.Deserialize
                (this.GetType(), serializer.Serialize(this))).Name;
            this.WriteLine($"Restored Scenario:  {this.RestoredScenario}");
            this.WriteLine("Expected Builds:    2");
            this.WriteLine("Issues Per Build:   1");
            FreshInstanceConfiguration.ConstructorCount = 0;
        }

        protected override void Act()
        {
            var schemaBuilder = new ApiSchemaBuilder()
                .WithName("ConfigurationLifecycle")
                .UseConfigurationsFromAssembly
                (
                    typeof(DiscoveryMarker).Assembly,
                    IncludeTypes
                    (
                        typeof(FreshInstanceConfiguration),
                        typeof(ActivationFailureConfiguration)
                    )
                );

            this.FirstIssueCount = schemaBuilder.BuildResult().Issues.Count(static issue =>
                issue.Code == ApiSchemaCompilationCode.ApiConfigurationActivationFailed);
            this.SecondIssueCount = schemaBuilder.BuildResult().Issues.Count(static issue =>
                issue.Code == ApiSchemaCompilationCode.ApiConfigurationActivationFailed);
            this.WriteLine($"First Build Issues:  {this.FirstIssueCount}");
            this.WriteLine($"Second Build Issues: {this.SecondIssueCount}");
        }

        protected override void Assert()
        {
            this.RestoredScenario.Should().Be(this.Name);
            FreshInstanceConfiguration.ConstructorCount.Should().Be(2);
            this.FirstIssueCount.Should().Be(1);
            this.SecondIssueCount.Should().Be(1);
        }
        #endregion
    }

    private sealed class GenericBridgeTest : XUnitTest
    {
        #region Calculated Properties
        private ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            GenericObjectDiscoveryConfiguration.ConfigureCount = 0;
            GenericObjectDiscoveryConfiguration.BuilderType = null;
            GenericScalarDiscoveryConfiguration.ConfigureCount = 0;
            GenericScalarDiscoveryConfiguration.BuilderType = null;
            GenericEnumDiscoveryConfiguration.ConfigureCount = 0;
            GenericEnumDiscoveryConfiguration.BuilderType = null;
        }

        protected override void Act()
        {
            var schemaBuilder = new ApiSchemaBuilder()
                .WithName("GenericConfigurationDiscovery")
                .UseConfigurationsFromAssemblyOf<DiscoveryMarker>
                (
                    IncludeTypes
                    (
                        typeof(GenericObjectDiscoveryConfiguration),
                        typeof(GenericScalarDiscoveryConfiguration),
                        typeof(GenericEnumDiscoveryConfiguration)
                    )
                );

            this.ApiSchemaActual = schemaBuilder.Build();
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();
            GenericObjectDiscoveryConfiguration.ConfigureCount.Should().Be(1);
            GenericObjectDiscoveryConfiguration.BuilderType.Should().Be<ApiObjectTypeBuilder<BridgeObject>>();
            GenericScalarDiscoveryConfiguration.ConfigureCount.Should().Be(1);
            GenericScalarDiscoveryConfiguration.BuilderType.Should().Be<ApiScalarTypeBuilder<BridgeScalar>>();
            GenericEnumDiscoveryConfiguration.ConfigureCount.Should().Be(1);
            GenericEnumDiscoveryConfiguration.BuilderType.Should().Be<ApiEnumTypeBuilder<BridgeEnum>>();
        }
        #endregion
    }

    private sealed class MultipleRolesTest : XUnitTest
    {
        #region Calculated Properties
        private ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            MultiRoleDiscoveryConfiguration.ConstructorCount = 0;
            MultiRoleDiscoveryConfiguration.AppliedRoles.Clear();
        }

        protected override void Act()
        {
            var schemaBuilder = new ApiSchemaBuilder()
                .WithName("MultipleRoles")
                .UseConfigurationsFromAssembly
                (
                    typeof(DiscoveryMarker).Assembly,
                    IncludeTypes
                    (
                        typeof(MultiRoleDiscoveryConfiguration),
                        typeof(MultiRoleObject),
                        typeof(RolePrincipalConfiguration),
                        typeof(RoleDependentConfiguration),
                        typeof(RoleScalarConfiguration)
                    )
                );

            this.ApiSchemaActual = schemaBuilder.Build();
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();
            MultiRoleDiscoveryConfiguration.ConstructorCount.Should().Be(1);
            MultiRoleDiscoveryConfiguration.AppliedRoles.Should().Equal("Object", "OneToOne");
            this.ApiSchemaActual.ApiRelationships.Should().ContainSingle
            (
                static x => x.ApiName == "MultiRoleRelationship"
            );
        }
        #endregion
    }

    private sealed class ConfigurationFailureTest : XUnitTest
    {
        #region User Supplied Properties
        public required ConfigurationFailureKind FailureKind { get; init; }
        #endregion

        #region Calculated Properties
        private ConfigurationFailureTest RestoredCase { get; set; } = null!;

        private ApiSchema? ApiSchemaActual { get; set; }
        private IReadOnlyList<ApiSchemaCompilationIssue> IssuesActual { get; set; } = [];
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario:     {this.Name}");
            this.WriteLine($"Failure Kind: {this.FailureKind}");
            this.WriteLine();
            this.WriteLine("Expected Code:      ApiConfigurationExecutionFailed");
            this.WriteLine("Expected Exception: None; failure is a warning in the build result");
            var serializer = new XUnitTestSerializer();
            this.RestoredCase = (ConfigurationFailureTest)serializer.Deserialize
                (this.GetType(), serializer.Serialize(this));
            FailingRoleDiscoveryConfiguration.AppliedRoles.Clear();
        }

        protected override void Act()
        {
            var configurationType = this.FailureKind == ConfigurationFailureKind.Configure
                ? typeof(FailingRoleDiscoveryConfiguration)
                : typeof(IdentityFailureConfiguration);

            var configurationTypes = this.FailureKind == ConfigurationFailureKind.Configure
                ? IncludeTypes
                (
                    configurationType,
                    typeof(RolePrincipalConfiguration),
                    typeof(RoleDependentConfiguration),
                    typeof(RoleScalarConfiguration)
                )
                : IncludeTypes(configurationType);

            var schemaBuilder = new ApiSchemaBuilder()
                .WithName("ConfigurationFailure")
                .UseConfigurationsFromAssembly(typeof(DiscoveryMarker).Assembly, configurationTypes);

            var result = schemaBuilder.BuildResult();
            this.ApiSchemaActual = result.Schema;
            this.IssuesActual = [.. result.Issues.Where(static issue =>
                issue.Code == ApiSchemaCompilationCode.ApiConfigurationExecutionFailed)];
            this.WriteLine($"Actual Success: {result.IsValid}");
            this.WriteLine($"Actual Issues:  {this.IssuesActual.Count}");
        }

        protected override void Assert()
        {
            this.RestoredCase.FailureKind.Should().Be(this.FailureKind);
            this.ApiSchemaActual.Should().NotBeNull();
            this.IssuesActual.Should().ContainSingle
            (
                issue => issue.Code == ApiSchemaCompilationCode.ApiConfigurationExecutionFailed &&
                    issue.Severity == ApiSchemaCompilationSeverity.Warning &&
                    issue.ApiPath ==
                    (
                        this.FailureKind == ConfigurationFailureKind.Configure
                            ? typeof(FailingRoleDiscoveryConfiguration).FullName
                            : typeof(IdentityFailureConfiguration).FullName
                    )
            );

            if (this.FailureKind == ConfigurationFailureKind.Configure)
            {
                FailingRoleDiscoveryConfiguration.AppliedRoles.Should().Equal("Object", "OneToOne");
            }
        }
        #endregion
    }

    private sealed class ActivationFailureTest : XUnitTest
    {
        #region Calculated Properties
        private string RestoredScenario { get; set; } = null!;
        private IReadOnlyList<ApiSchemaCompilationIssue> IssuesActual { get; set; } = [];
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario: {this.Name}");
            this.WriteLine();
            this.WriteLine("Expected Exception: None; configuration failures appear in the build result");
            var serializer = new XUnitTestSerializer();
            this.RestoredScenario = ((XUnitTest)serializer.Deserialize
                (this.GetType(), serializer.Serialize(this))).Name;
            this.WriteLine($"Restored Scenario:  {this.RestoredScenario}");
            this.WriteLine("Expected Code:      ApiConfigurationActivationFailed");
        }

        protected override void Act()
        {
            var schemaBuilder = new ApiSchemaBuilder()
                .WithName("ActivationFailure")
                .UseConfigurationsFromAssembly
                (
                    typeof(DiscoveryMarker).Assembly,
                    IncludeTypes(typeof(ActivationFailureConfiguration))
                );

            this.IssuesActual = [.. schemaBuilder.BuildResult().Issues.Where(static issue =>
                issue.Code == ApiSchemaCompilationCode.ApiConfigurationActivationFailed)];
            this.WriteLine($"Activation Issues: {this.IssuesActual.Count}");
        }

        protected override void Assert()
        {
            this.RestoredScenario.Should().Be(this.Name);
            this.IssuesActual.Should().ContainSingle();
            var issue = this.IssuesActual.Single();
            issue.Code.Should().Be(ApiSchemaCompilationCode.ApiConfigurationActivationFailed);
            issue.Severity.Should().Be(ApiSchemaCompilationSeverity.Warning);
            issue.ApiPath.Should().Be(typeof(ActivationFailureConfiguration).FullName);
            issue.Exception.Should().NotBeNull();
        }
        #endregion
    }

    private sealed class MarkerOnlyTest : XUnitTest
    {
        #region Calculated Properties
        private ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            var schemaBuilder = new ApiSchemaBuilder()
                .WithName("MarkerOnly")
                .UseConfigurationsFromAssembly
                (
                    typeof(DiscoveryMarker).Assembly,
                    IncludeTypes(typeof(MarkerOnlyConfiguration))
                );

            this.ApiSchemaActual = schemaBuilder.Build();
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();
            this.ApiSchemaActual.ApiObjectTypes.Should().BeEmpty();
            this.ApiSchemaActual.ApiScalarTypes.Should().BeEmpty();
            this.ApiSchemaActual.ApiEnumTypes.Should().BeEmpty();
            this.ApiSchemaActual.ApiRelationships.Should().BeEmpty();
        }
        #endregion
    }

    private sealed class CompositionAndRegistrationTest : XUnitTest
    {
        #region Calculated Properties
        private string RestoredScenario { get; set; } = null!;
        private ApiSchema? ApiSchemaActual { get; set; }
        private int RepeatedConfigureCountActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.WriteLine($"Scenario: {this.Name}");
            this.WriteLine();
            this.WriteLine("Expected Exception: None; configuration failures appear in the build result");
            var serializer = new XUnitTestSerializer();
            this.RestoredScenario = ((XUnitTest)serializer.Deserialize
                (this.GetType(), serializer.Serialize(this))).Name;
            this.WriteLine($"Restored Scenario:  {this.RestoredScenario}");
            this.WriteLine("Scenario:             Compose distinct configurations and deduplicate repeated discovery");
            this.WriteLine("Expected Role Calls:  1");
            ADuplicateObjectConfiguration.AppliedConfigurations.Clear();
            RepeatedRegistrationConfiguration.ConfigureCount = 0;
        }

        protected override void Act()
        {
            var assembly = typeof(DiscoveryMarker).Assembly;
            var duplicateFilter = IncludeTypes
            (
                typeof(ADuplicateObjectConfiguration),
                typeof(BDuplicateObjectConfiguration)
            );

            var schemaBuilder = new ApiSchemaBuilder()
                .WithName("Composition")
                .UseConfigurationsFromAssembly(assembly, duplicateFilter)
                .UseConfigurationsFromAssembly
                (
                    assembly,
                    IncludeTypes(typeof(RepeatedRegistrationConfiguration))
                )
                .UseConfigurationsFromAssembly
                (
                    assembly,
                    IncludeTypes(typeof(RepeatedRegistrationConfiguration))
                );

            this.ApiSchemaActual = schemaBuilder.Build();
            this.RepeatedConfigureCountActual = RepeatedRegistrationConfiguration.ConfigureCount;
            this.WriteLine($"Actual Role Calls: {this.RepeatedConfigureCountActual}");
        }

        protected override void Assert()
        {
            this.RestoredScenario.Should().Be(this.Name);
            this.ApiSchemaActual.Should().NotBeNull();
            this.ApiSchemaActual.ApiObjectTypes.Should().ContainSingle(static x => x.ApiName == "DuplicateObject");
            ADuplicateObjectConfiguration.AppliedConfigurations.Should().Equal("First", "Second");
            this.RepeatedConfigureCountActual.Should().Be(1);
        }
        #endregion
    }

    private sealed class ExplicitPrecedenceTest : XUnitTest
    {
        #region Calculated Properties
        private ApiSchema? ApiSchemaActual { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Act()
        {
            this.ApiSchemaActual = new ApiSchemaBuilder()
                .WithName("ExplicitPrecedence")
                .UseConfigurationsFromAssembly
                (
                    typeof(DiscoveryMarker).Assembly,
                    IncludeTypes(typeof(PrecedenceConfiguration))
                )
                .UseConventions(static c => c.AddConvention(new ConventionNameOverride()))
                .Build();
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();
            this.ApiSchemaActual.ApiObjectTypes.Should().ContainSingle
            (
                static x => x.ApiName == "ExplicitName"
            );
        }
        #endregion
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] AssemblyDiscoveryTheoryData =>
    [
        new AssemblyDiscoveryTest
        {
            Name = "Assembly configuration discovery builds all schema categories",
            UseMarkerType = false,
        },
        new AssemblyDiscoveryTest
        {
            Name = "Marker-type configuration discovery builds all schema categories",
            UseMarkerType = true,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] NullScanArgumentsTheoryData =>
    [
        new NullScanArgumentsTest
        {
            Name = "Assembly configuration scan rejects a null builder",
            ScanKind = NullScanKind.ExplicitBuilder,
        },
        new NullScanArgumentsTest
        {
            Name = "Assembly configuration scan rejects a null assembly",
            ScanKind = NullScanKind.ExplicitAssembly,
        },
        new NullScanArgumentsTest
        {
            Name = "Marker-type configuration scan rejects a null builder",
            ScanKind = NullScanKind.MarkerBuilder,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] ConfigurationFailureTheoryData =>
    [
        new ConfigurationFailureTest
        {
            Name = "Configuration execution failure continues with later roles",
            FailureKind = ConfigurationFailureKind.Configure,
        },
        new ConfigurationFailureTest
        {
            Name = "Configuration identity failure becomes an compilation issue",
            FailureKind = ConfigurationFailureKind.Identity,
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] TypeEligibilityTheoryData =>
    [
        new TypeEligibilityTest
        {
            Name = "Configuration type eligibility",
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] BuildLifecycleTheoryData =>
    [
        new BuildLifecycleTest
        {
            Name = "Configuration discovery uses fresh instances and per-build issues",
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] GenericBridgeTheoryData =>
    [
        new GenericBridgeTest
        {
            Name = "Generic configuration bridges invoke typed builders once",
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] MultipleRolesTheoryData =>
    [
        new MultipleRolesTest
        {
            Name = "Multiple configuration roles share one activation and use fixed order",
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] ActivationFailureTheoryData =>
    [
        new ActivationFailureTest
        {
            Name = "Configuration activation failure becomes an compilation issue",
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] MarkerOnlyTheoryData =>
    [
        new MarkerOnlyTest
        {
            Name = "Marker-only configuration is ignored",
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] CompositionAndRegistrationTheoryData =>
    [
        new CompositionAndRegistrationTest
        {
            Name = "Distinct configurations compose and repeated scans are idempotent",
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] ExplicitPrecedenceTheoryData =>
    [
        new ExplicitPrecedenceTest
        {
            Name = "Discovered configuration uses explicit precedence",
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(AssemblyDiscoveryTheoryData))]
    public void AssemblyDiscovery(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(NullScanArgumentsTheoryData))]
    public void NullScanArguments(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(TypeEligibilityTheoryData))]
    public void TypeEligibility(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(BuildLifecycleTheoryData))]
    public void BuildLifecycle(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(GenericBridgeTheoryData))]
    public void GenericBridge(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(MultipleRolesTheoryData))]
    public void MultipleRoles(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(ConfigurationFailureTheoryData))]
    public void ConfigurationFailure(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(ActivationFailureTheoryData))]
    public void ActivationFailure(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(MarkerOnlyTheoryData))]
    public void MarkerOnly(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(CompositionAndRegistrationTheoryData))]
    public void CompositionAndRegistration(IXUnitTest test)
        => test.Execute(this);

    [Theory]
    [MemberData(nameof(ExplicitPrecedenceTheoryData))]
    public void ExplicitPrecedence(IXUnitTest test)
        => test.Execute(this);
    #endregion

    #region Helper Methods
    private static Func<Type, bool> IncludeTypes(params Type[] types)
    {
        var typeSet = types.ToHashSet();
        return typeSet.Contains;
    }
    #endregion
}
