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
        First,
    }

    public sealed class DiscoveryPrincipal
    {
        public int Id { get; set; }
    }

    public sealed class DiscoveryDependent
    {
        public int Id { get; set; }

        public int PrincipalId { get; set; }
    }

    public sealed class DiscoveryAssociation
    {
        public int PrincipalId { get; set; }

        public int DependentId { get; set; }
    }

    public sealed class AssemblyObjectConfiguration : IApiObjectTypeConfiguration<DiscoveryObject>
    {
        public void Configure(ApiObjectTypeBuilder<DiscoveryObject> builder)
        {
            builder.WithName("DiscoveredObject");
        }
    }

    public sealed class AssemblyScalarConfiguration : IApiScalarTypeConfiguration<DiscoveryScalar>
    {
        public void Configure(ApiScalarTypeBuilder<DiscoveryScalar> builder)
        {
            builder.WithName("DiscoveredScalar");
        }
    }

    public sealed class AssemblyIntScalarConfiguration : IApiScalarTypeConfiguration<int>
    {
        public void Configure(ApiScalarTypeBuilder<int> builder)
        {
            builder.WithName("Int");
        }
    }

    public sealed class AssemblyEnumConfiguration : IApiEnumTypeConfiguration<DiscoveryEnum>
    {
        public void Configure(ApiEnumTypeBuilder<DiscoveryEnum> builder)
        {
            builder
                .WithName("DiscoveredEnum")
                .AddValue("First", nameof(DiscoveryEnum.First), (int)DiscoveryEnum.First);
        }
    }

    public sealed class AssemblyPrincipalConfiguration : IApiObjectTypeConfiguration<DiscoveryPrincipal>
    {
        public void Configure(ApiObjectTypeBuilder<DiscoveryPrincipal> builder)
        {
            builder
                .WithName("DiscoveredPrincipal")
                .AddProperty(p => p.Id)
                .AddKey("PrimaryKey", p => p.Id);
        }
    }

    public sealed class AssemblyDependentConfiguration : IApiObjectTypeConfiguration<DiscoveryDependent>
    {
        public void Configure(ApiObjectTypeBuilder<DiscoveryDependent> builder)
        {
            builder
                .WithName("DiscoveredDependent")
                .AddProperty(p => p.Id)
                .AddProperty(p => p.PrincipalId)
                .AddKey("PrimaryKey", p => p.Id);
        }
    }

    public sealed class AssemblyAssociationConfiguration : IApiObjectTypeConfiguration<DiscoveryAssociation>
    {
        public void Configure(ApiObjectTypeBuilder<DiscoveryAssociation> builder)
        {
            builder
                .WithName("DiscoveredAssociation")
                .AddProperty(p => p.PrincipalId)
                .AddProperty(p => p.DependentId)
                .AddKey("PrimaryKey", p => p.PrincipalId, p => p.DependentId);
        }
    }

    public sealed class AssemblyOneToOneConfiguration : IApiRelationshipOneToOneConfiguration
    {
        public string ApiName => "DiscoveredOneToOne";

        public void Configure(ApiRelationshipOneToOneBuilder builder)
        {
            builder
                .From<DiscoveryPrincipal>()
                .To<DiscoveryDependent>(d => d.WithForeignKey(x => x.PrincipalId));
        }
    }

    public sealed class AssemblyOneToManyConfiguration : IApiRelationshipOneToManyConfiguration
    {
        public string ApiName => "DiscoveredOneToMany";

        public void Configure(ApiRelationshipOneToManyBuilder builder)
        {
            builder
                .From<DiscoveryPrincipal>()
                .To<DiscoveryDependent>(d => d.WithForeignKey(x => x.PrincipalId));
        }
    }

    public sealed class AssemblyManyToManyConfiguration : IApiRelationshipManyToManyConfiguration
    {
        public string ApiName => "DiscoveredManyToMany";

        public void Configure(ApiRelationshipManyToManyBuilder builder)
        {
            builder
                .Between<DiscoveryPrincipal>()
                .And<DiscoveryDependent>()
                .WithAssociation<DiscoveryAssociation>
                (
                    a => a
                        .WithForeignKeyA(x => x.PrincipalId)
                        .WithForeignKeyB(x => x.DependentId)
                );
        }
    }

    public sealed class FreshInstanceConfiguration : IApiObjectTypeConfiguration<DiscoveryObject>
    {
        public static int ConstructorCount { get; set; }

        public FreshInstanceConfiguration()
        {
            ConstructorCount++;
        }

        public void Configure(ApiObjectTypeBuilder<DiscoveryObject> builder)
        {
            builder.WithName("FreshObject");
        }
    }

    public sealed class EligibleConfiguration : IApiObjectTypeConfiguration<EligibilityObject>
    {
        public void Configure(ApiObjectTypeBuilder<EligibilityObject> builder)
        {
            builder.WithName("EligibleObject");
        }
    }

    public sealed class EligibilityObject
    {
    }

    public abstract class AbstractConfiguration : IApiObjectTypeConfiguration<EligibilityObject>
    {
        public abstract void Configure(ApiObjectTypeBuilder<EligibilityObject> builder);
    }

    public struct StructConfiguration : IApiObjectTypeConfiguration<EligibilityObject>
    {
        public void Configure(ApiObjectTypeBuilder<EligibilityObject> builder)
        {
        }
    }

    public sealed class OpenGenericConfiguration<T> : IApiObjectTypeConfiguration<T>
    {
        public void Configure(ApiObjectTypeBuilder<T> builder)
        {
        }
    }

    internal sealed class NonPublicConfiguration : IApiObjectTypeConfiguration<EligibilityObject>
    {
        public void Configure(ApiObjectTypeBuilder<EligibilityObject> builder)
        {
        }
    }

    public sealed class GenericObjectDiscoveryConfiguration : IApiObjectTypeConfiguration<BridgeObject>
    {
        public static int ConfigureCount { get; set; }

        public static Type? BuilderType { get; set; }

        public void Configure(ApiObjectTypeBuilder<BridgeObject> builder)
        {
            ConfigureCount++;
            BuilderType = builder.GetType();
            builder.WithName("BridgeObject");
        }
    }

    public sealed class GenericScalarDiscoveryConfiguration : IApiScalarTypeConfiguration<BridgeScalar>
    {
        public static int ConfigureCount { get; set; }

        public static Type? BuilderType { get; set; }

        public void Configure(ApiScalarTypeBuilder<BridgeScalar> builder)
        {
            ConfigureCount++;
            BuilderType = builder.GetType();
            builder.WithName("BridgeScalar");
        }
    }

    public sealed class GenericEnumDiscoveryConfiguration : IApiEnumTypeConfiguration<BridgeEnum>
    {
        public static int ConfigureCount { get; set; }

        public static Type? BuilderType { get; set; }

        public void Configure(ApiEnumTypeBuilder<BridgeEnum> builder)
        {
            ConfigureCount++;
            BuilderType = builder.GetType();
            builder
                .WithName("BridgeEnum")
                .AddValue("First", nameof(BridgeEnum.First), (int)BridgeEnum.First);
        }
    }

    public sealed class BridgeObject
    {
    }

    public sealed class BridgeScalar
    {
    }

    public enum BridgeEnum
    {
        First,
    }

    public sealed class MultiRoleDiscoveryConfiguration :
        IApiObjectTypeConfiguration<MultiRoleObject>,
        IApiRelationshipOneToOneConfiguration
    {
        public static int ConstructorCount { get; set; }

        public static List<string> AppliedRoles { get; } = [];

        public MultiRoleDiscoveryConfiguration()
        {
            ConstructorCount++;
        }

        public string ApiName => "MultiRoleRelationship";

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
                .To<RoleDependent>(d => d.WithForeignKey(x => x.PrincipalId));
        }
    }

    public sealed class MultiRoleObject
    {
    }

    public sealed class RolePrincipal
    {
        public int Id { get; set; }
    }

    public sealed class RoleDependent
    {
        public int Id { get; set; }

        public int PrincipalId { get; set; }
    }

    public sealed class RolePrincipalConfiguration : IApiObjectTypeConfiguration<RolePrincipal>
    {
        public void Configure(ApiObjectTypeBuilder<RolePrincipal> builder)
        {
            builder
                .WithName("RolePrincipal")
                .AddProperty(p => p.Id)
                .AddKey("PrimaryKey", p => p.Id);
        }
    }

    public sealed class RoleDependentConfiguration : IApiObjectTypeConfiguration<RoleDependent>
    {
        public void Configure(ApiObjectTypeBuilder<RoleDependent> builder)
        {
            builder
                .WithName("RoleDependent")
                .AddProperty(p => p.Id)
                .AddProperty(p => p.PrincipalId)
                .AddKey("PrimaryKey", p => p.Id);
        }
    }

    public sealed class RoleScalarConfiguration : IApiScalarTypeConfiguration<int>
    {
        public void Configure(ApiScalarTypeBuilder<int> builder)
        {
            builder.WithName("RoleInt");
        }
    }

    public sealed class FailingRoleDiscoveryConfiguration :
        IApiObjectTypeConfiguration<FailingRoleObject>,
        IApiRelationshipOneToOneConfiguration
    {
        public static List<string> AppliedRoles { get; } = [];

        public string ApiName => "FailingRoleRelationship";

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
                .To<RoleDependent>(d => d.WithForeignKey(x => x.PrincipalId));
        }
    }

    public sealed class FailingRoleObject
    {
    }

    public sealed class IdentityFailureConfiguration : IApiRelationshipOneToOneConfiguration
    {
        public string ApiName => throw new InvalidOperationException("The relationship identity failed.");

        public void Configure(ApiRelationshipOneToOneBuilder builder)
        {
        }
    }

    public sealed class ActivationFailureConfiguration : IApiObjectTypeConfiguration<ActivationObject>
    {
        public ActivationFailureConfiguration(int value)
        {
        }

        public void Configure(ApiObjectTypeBuilder<ActivationObject> builder)
        {
        }
    }

    public sealed class ActivationObject
    {
    }

    public sealed class MarkerOnlyConfiguration : IApiConfiguration
    {
    }

    public sealed class ADuplicateObjectConfiguration : IApiObjectTypeConfiguration<DuplicateObject>
    {
        public static List<string> AppliedConfigurations { get; } = [];

        public void Configure(ApiObjectTypeBuilder<DuplicateObject> builder)
        {
            AppliedConfigurations.Add("First");
            builder.WithName("DuplicateObject");
        }
    }

    public sealed class BDuplicateObjectConfiguration : IApiObjectTypeConfiguration<DuplicateObject>
    {
        public void Configure(ApiObjectTypeBuilder<DuplicateObject> builder)
        {
            ADuplicateObjectConfiguration.AppliedConfigurations.Add("Second");
            builder.WithName("DuplicateObject");
        }
    }

    public sealed class DuplicateObject
    {
    }

    public sealed class RepeatedRegistrationConfiguration : IApiObjectTypeConfiguration<RepeatedObject>
    {
        public static int ConfigureCount { get; set; }

        public void Configure(ApiObjectTypeBuilder<RepeatedObject> builder)
        {
            ConfigureCount++;
            builder.WithName("RepeatedObject");
        }
    }

    public sealed class RepeatedObject
    {
    }

    public sealed class PrecedenceConfiguration : IApiObjectTypeConfiguration<PrecedenceObject>
    {
        public void Configure(ApiObjectTypeBuilder<PrecedenceObject> builder)
        {
            builder.WithName("ExplicitName");
        }
    }

    public sealed class PrecedenceObject
    {
    }

    private enum ConfigurationFailureKind
    {
        Configure,
        Identity,
    }

    private enum NullScanKind
    {
        ExplicitBuilder,
        ExplicitAssembly,
        MarkerBuilder,
    }

    private sealed class ConventionNameOverride : IApiObjectTypeConvention
    {
        public ApiConventionPhase Phase => ApiConventionPhase.Configuration;

        public void Apply(ApiObjectTypeBuilder builder)
        {
            if (builder.ClrType == typeof(PrecedenceObject))
            {
                builder.SetApiNameConvention("ConventionName");
            }
        }
    }
    #endregion

    #region Test Classes
    private sealed class AssemblyDiscoveryTest : XUnitTest
    {
        public bool UseMarkerType { get; init; }

        private ApiSchema? ApiSchemaActual { get; set; }
        private bool ReturnedSameBuilder { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"UseMarkerType: {this.UseMarkerType}");
        }

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

            this.ApiSchemaActual!.ApiObjectTypes.Should().HaveCount(4);
            this.ApiSchemaActual.ApiObjectTypes.Should().ContainSingle(x => x.ApiName == "DiscoveredObject");
            this.ApiSchemaActual.ApiObjectTypes.Should().ContainSingle(x => x.ApiName == "DiscoveredPrincipal");
            this.ApiSchemaActual.ApiObjectTypes.Should().ContainSingle(x => x.ApiName == "DiscoveredDependent");
            this.ApiSchemaActual.ApiObjectTypes.Should().ContainSingle(x => x.ApiName == "DiscoveredAssociation");

            this.ApiSchemaActual.ApiScalarTypes.Should().HaveCount(2);
            this.ApiSchemaActual.ApiScalarTypes.Should().ContainSingle(x => x.ApiName == "DiscoveredScalar");
            this.ApiSchemaActual.ApiScalarTypes.Should().ContainSingle(x => x.ApiName == "Int");

            this.ApiSchemaActual.ApiEnumTypes.Should().ContainSingle(x => x.ApiName == "DiscoveredEnum");
            this.ApiSchemaActual.ApiRelationships.Should().HaveCount(3);
            this.ApiSchemaActual.ApiRelationships.Should().ContainSingle(x => x.ApiName == "DiscoveredOneToOne");
            this.ApiSchemaActual.ApiRelationships.Should().ContainSingle(x => x.ApiName == "DiscoveredOneToMany");
            this.ApiSchemaActual.ApiRelationships.Should().ContainSingle(x => x.ApiName == "DiscoveredManyToMany");
        }
    }

    private sealed class NullScanArgumentsTest : XUnitTest
    {
        public required NullScanKind ScanKind { get; init; }

        private ArgumentNullException? ExceptionActual { get; set; }

        protected override void Arrange()
        {
            this.WriteLine($"ScanKind: {this.ScanKind}");
        }

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
            this.ExceptionActual!.ParamName.Should().Be
            (
                this.ScanKind == NullScanKind.ExplicitAssembly
                    ? "assembly"
                    : "builder"
            );
        }
    }

    private sealed class TypeEligibilityTest : XUnitTest
    {
        private HashSet<Type> ObservedTypes { get; set; } = [];
        private ApiSchema? ApiSchemaActual { get; set; }

        protected override void Arrange()
        {
            this.ObservedTypes = [];
        }

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
            this.ApiSchemaActual!.ApiObjectTypes.Should().ContainSingle(x => x.ApiName == "EligibleObject");
            this.ObservedTypes.Should().Contain(typeof(EligibleConfiguration));
            this.ObservedTypes.Should().NotContain(typeof(AbstractConfiguration));
            this.ObservedTypes.Should().NotContain(typeof(StructConfiguration));
            this.ObservedTypes.Should().NotContain(typeof(OpenGenericConfiguration<>));
            this.ObservedTypes.Should().NotContain(typeof(NonPublicConfiguration));
        }
    }

    private sealed class BuildLifecycleTest : XUnitTest
    {
        private int FirstIssueCount { get; set; }
        private int SecondIssueCount { get; set; }

        protected override void Arrange()
        {
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

            schemaBuilder.Build();
            this.FirstIssueCount = schemaBuilder.Context.ConfigurationIssues.Count;

            schemaBuilder.Build();
            this.SecondIssueCount = schemaBuilder.Context.ConfigurationIssues.Count;
        }

        protected override void Assert()
        {
            FreshInstanceConfiguration.ConstructorCount.Should().Be(2);
            this.FirstIssueCount.Should().Be(1);
            this.SecondIssueCount.Should().Be(1);
        }
    }

    private sealed class GenericBridgeTest : XUnitTest
    {
        private ApiSchema? ApiSchemaActual { get; set; }

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
            GenericObjectDiscoveryConfiguration.BuilderType.Should().Be(typeof(ApiObjectTypeBuilder<BridgeObject>));
            GenericScalarDiscoveryConfiguration.ConfigureCount.Should().Be(1);
            GenericScalarDiscoveryConfiguration.BuilderType.Should().Be(typeof(ApiScalarTypeBuilder<BridgeScalar>));
            GenericEnumDiscoveryConfiguration.ConfigureCount.Should().Be(1);
            GenericEnumDiscoveryConfiguration.BuilderType.Should().Be(typeof(ApiEnumTypeBuilder<BridgeEnum>));
        }
    }

    private sealed class MultipleRolesTest : XUnitTest
    {
        private ApiSchema? ApiSchemaActual { get; set; }

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
            this.ApiSchemaActual!.ApiRelationships.Should().ContainSingle
            (
                x => x.ApiName == "MultiRoleRelationship"
            );
        }
    }

    private sealed class ConfigurationFailureTest : XUnitTest
    {
        public required ConfigurationFailureKind FailureKind { get; init; }

        private ApiSchema? ApiSchemaActual { get; set; }
        private IReadOnlyList<ApiSchemaCompilationIssue> IssuesActual { get; set; } = [];

        protected override void Arrange()
        {
            this.WriteLine($"FailureKind: {this.FailureKind}");
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

            this.ApiSchemaActual = schemaBuilder.Build();
            this.IssuesActual = schemaBuilder.Context.ConfigurationIssues;
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();
            this.IssuesActual.Should().ContainSingle
            (
                issue => issue.Code == ApiSchemaCompilationCode.ApiConfigurationExecutionFailed &&
                    issue.Severity == ApiSchemaCompilationSeverity.Warning &&
                    issue.ApiPath ==
                    (
                        this.FailureKind == ConfigurationFailureKind.Configure
                            ? typeof(FailingRoleDiscoveryConfiguration).FullName!
                            : typeof(IdentityFailureConfiguration).FullName!
                    )
            );

            if (this.FailureKind == ConfigurationFailureKind.Configure)
            {
                FailingRoleDiscoveryConfiguration.AppliedRoles.Should().Equal("Object", "OneToOne");
            }
        }
    }

    private sealed class ActivationFailureTest : XUnitTest
    {
        private IReadOnlyList<ApiSchemaCompilationIssue> IssuesActual { get; set; } = [];

        protected override void Act()
        {
            var schemaBuilder = new ApiSchemaBuilder()
                .WithName("ActivationFailure")
                .UseConfigurationsFromAssembly
                (
                    typeof(DiscoveryMarker).Assembly,
                    IncludeTypes(typeof(ActivationFailureConfiguration))
                );

            schemaBuilder.Build();
            this.IssuesActual = schemaBuilder.Context.ConfigurationIssues;
        }

        protected override void Assert()
        {
            this.IssuesActual.Should().ContainSingle();
            var issue = this.IssuesActual.Single();
            issue.Code.Should().Be(ApiSchemaCompilationCode.ApiConfigurationActivationFailed);
            issue.Severity.Should().Be(ApiSchemaCompilationSeverity.Warning);
            issue.ApiPath.Should().Be(typeof(ActivationFailureConfiguration).FullName);
            issue.Exception.Should().NotBeNull();
        }
    }

    private sealed class MarkerOnlyTest : XUnitTest
    {
        private ApiSchema? ApiSchemaActual { get; set; }

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
            this.ApiSchemaActual!.ApiObjectTypes.Should().BeEmpty();
            this.ApiSchemaActual.ApiScalarTypes.Should().BeEmpty();
            this.ApiSchemaActual.ApiEnumTypes.Should().BeEmpty();
            this.ApiSchemaActual.ApiRelationships.Should().BeEmpty();
        }
    }

    private sealed class CompositionAndRegistrationTest : XUnitTest
    {
        private ApiSchema? ApiSchemaActual { get; set; }
        private int RepeatedConfigureCountActual { get; set; }

        protected override void Arrange()
        {
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
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();
            this.ApiSchemaActual!.ApiObjectTypes.Should().ContainSingle(x => x.ApiName == "DuplicateObject");
            ADuplicateObjectConfiguration.AppliedConfigurations.Should().Equal("First", "Second");
            this.RepeatedConfigureCountActual.Should().Be(2);
        }
    }

    private sealed class ExplicitPrecedenceTest : XUnitTest
    {
        private ApiSchema? ApiSchemaActual { get; set; }

        protected override void Act()
        {
            this.ApiSchemaActual = new ApiSchemaBuilder()
                .WithName("ExplicitPrecedence")
                .UseConfigurationsFromAssembly
                (
                    typeof(DiscoveryMarker).Assembly,
                    IncludeTypes(typeof(PrecedenceConfiguration))
                )
                .UseConventions(c => c.AddConvention(new ConventionNameOverride()))
                .Build();
        }

        protected override void Assert()
        {
            this.ApiSchemaActual.Should().NotBeNull();
            this.ApiSchemaActual!.ApiObjectTypes.Should().ContainSingle
            (
                x => x.ApiName == "ExplicitName"
            );
        }
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
            Name = "Duplicate identities compose and repeated scans execute",
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
    public void AssemblyDiscovery(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(NullScanArgumentsTheoryData))]
    public void NullScanArguments(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(TypeEligibilityTheoryData))]
    public void TypeEligibility(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(BuildLifecycleTheoryData))]
    public void BuildLifecycle(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(GenericBridgeTheoryData))]
    public void GenericBridge(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(MultipleRolesTheoryData))]
    public void MultipleRoles(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ConfigurationFailureTheoryData))]
    public void ConfigurationFailure(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ActivationFailureTheoryData))]
    public void ActivationFailure(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(MarkerOnlyTheoryData))]
    public void MarkerOnly(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(CompositionAndRegistrationTheoryData))]
    public void CompositionAndRegistration(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(ExplicitPrecedenceTheoryData))]
    public void ExplicitPrecedence(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Helper Methods
    private static Func<Type, bool> IncludeTypes(params Type[] types)
    {
        var typeSet = types.ToHashSet();
        return typeSet.Contains;
    }
    #endregion
}
