// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Annotations;
using Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

/// <summary>
///     Tests for the convention pipeline: built-in conventions, pipeline ordering,
///     precedence rules, and the AddTypes convenience method.
/// </summary>
public class ConventionPipelineTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Domain Types
    private class PersonWithId
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
    }

    private class OrderWithPersonId
    {
        public Guid OrderId { get; set; }
        public Guid PersonId { get; set; }
        public decimal Total { get; set; }
    }

    private record struct CustomScalar(string Value);

    private enum CustomEnum
    {
        Active,
    }

    private enum PipelineStatus
    {
        Active,
        InProgress,
        OnHold,
        Queued,
    }

    [ApiObjectType]
    public class AssemblyScannedObject
    {
        public Guid Id { get; set; }
    }

    [ApiScalarType]
    public readonly record struct AssemblyScannedScalar(string Value);

    [ApiEnumType]
    public enum AssemblyScannedEnum
    {
        Active,
    }

    // Named "OrderItem" so that "OrderItemId" triggers the {ClassName}Id convention.
    private class OrderItem
    {
        public Guid OrderItemId { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    private class TypeWithField
    {
        public Guid Id = Guid.Empty;
        public string Name = string.Empty;
        public int? Count = null;
    }

    private class ConventionLoopRoot
    {
        public int Id { get; set; }
    }

    private class ConventionLoopType<T>
    {
        public T? Next { get; set; }
    }

    private class PropertyConventionTarget
    {
        public int Initial { get; set; }
        public int Added { get; set; }
    }

    private class PropertyConventionTrigger
    {
        public int Trigger { get; set; }
    }

    private class PropertyConventionRegistered
    {
        public int Id { get; set; }
    }
    #endregion

    #region Test Conventions
    private sealed class NonConvergingPropertyConvention : IApiPropertyConvention
    {
        #region Fields
        private Type _nextType = typeof(ConventionLoopRoot);
        #endregion

        #region IApiPropertyConvention
        public void Apply(ApiPropertyBuilder builder, ApiPropertyConventionContext context)
        {
            _nextType = typeof(ConventionLoopType<>).MakeGenericType(_nextType);
            context.ApiSchemaBuilder.AddObject(_nextType);
        }
        #endregion
    }

    private sealed class AddSiblingPropertyConvention : IApiPropertyConvention
    {
        #region Fields
        private bool _hasAddedProperty;
        #endregion

        #region IApiPropertyConvention
        public void Apply(ApiPropertyBuilder builder, ApiPropertyConventionContext context)
        {
            if
            (
                !_hasAddedProperty &&
                context.ClrDeclaringType == typeof(PropertyConventionTarget) &&
                builder.ClrName == nameof(PropertyConventionTarget.Initial)
            )
            {
                _hasAddedProperty = true;
                context.ApiObjectTypeBuilder.AddProperty
                (
                    nameof(PropertyConventionTarget.Added),
                    nameof(PropertyConventionTarget.Added)
                );
            }
        }
        #endregion
    }

    private sealed class AddPropertyToVisitedObjectConvention : IApiPropertyConvention
    {
        #region Fields
        private bool _hasAddedProperty;
        #endregion

        #region IApiPropertyConvention
        public void Apply(ApiPropertyBuilder builder, ApiPropertyConventionContext context)
        {
            if
            (
                !_hasAddedProperty &&
                context.ClrDeclaringType == typeof(PropertyConventionTrigger) &&
                builder.ClrName == nameof(PropertyConventionTrigger.Trigger)
            )
            {
                _hasAddedProperty = true;
                context.ApiSchemaBuilder.AddObject<PropertyConventionTarget>
                (
                    x => x.AddProperty
                    (
                        nameof(PropertyConventionTarget.Added),
                        nameof(PropertyConventionTarget.Added)
                    )
                );
            }
        }
        #endregion
    }

    private sealed class RecordingPropertyConvention : IApiPropertyConvention
    {
        #region Properties
        public List<(Type ClrDeclaringType, string ClrName)> ProcessedProperties { get; } = [];
        #endregion

        #region IApiPropertyConvention
        public void Apply(ApiPropertyBuilder builder, ApiPropertyConventionContext context)
        {
            this.ProcessedProperties.Add((context.ClrDeclaringType, builder.ClrName));
        }
        #endregion
    }

    private sealed class PropertyRegisteredObjectOrderingConvention(List<string> events)
        : IApiObjectTypeConvention, IApiPropertyConvention
    {
        #region Fields
        private readonly List<string> _events = events;
        private bool _hasRegisteredObject;
        #endregion

        #region IApiObjectTypeConvention
        public void Apply(ApiObjectTypeBuilder builder)
        {
            if (builder.ClrType == typeof(PropertyConventionRegistered))
            {
                _events.Add("ObjectType");
            }
        }
        #endregion

        #region IApiPropertyConvention
        public void Apply(ApiPropertyBuilder builder, ApiPropertyConventionContext context)
        {
            if
            (
                !_hasRegisteredObject &&
                context.ClrDeclaringType == typeof(PropertyConventionTrigger) &&
                builder.ClrName == nameof(PropertyConventionTrigger.Trigger)
            )
            {
                _hasRegisteredObject = true;
                context.ApiSchemaBuilder.AddObject<PropertyConventionRegistered>
                (
                    x => x.AddProperty
                    (
                        nameof(PropertyConventionRegistered.Id),
                        nameof(PropertyConventionRegistered.Id)
                    )
                );
            }

            if
            (
                context.ClrDeclaringType == typeof(PropertyConventionRegistered) &&
                builder.ClrName == nameof(PropertyConventionRegistered.Id)
            )
            {
                _events.Add("Property");
            }
        }
        #endregion
    }

    private sealed class NonConvergingPropertyAdditionConvention : IApiPropertyConvention
    {
        #region Fields
        private int _nextPropertyNumber = 100;
        #endregion

        #region IApiPropertyConvention
        public void Apply(ApiPropertyBuilder builder, ApiPropertyConventionContext context)
        {
            var clrName = $"Added{_nextPropertyNumber}";
            context.ApiObjectTypeBuilder.AddProperty(clrName, clrName);
            _nextPropertyNumber++;
        }
        #endregion
    }

    private sealed class PropertyAddsEnumValueConvention : IApiPropertyConvention
    {
        #region Fields
        private bool _hasAddedValue;
        #endregion

        #region IApiPropertyConvention
        public void Apply(ApiPropertyBuilder builder, ApiPropertyConventionContext context)
        {
            if
            (
                !_hasAddedValue &&
                context.ClrDeclaringType == typeof(PropertyConventionTarget) &&
                builder.ClrName == nameof(PropertyConventionTarget.Initial)
            )
            {
                _hasAddedValue = true;
                context.ApiSchemaBuilder.AddEnum<PipelineStatus>
                (
                    enumBuilder => enumBuilder.AddValue(PipelineStatus.Queued)
                );
            }
        }
        #endregion
    }

    private sealed class AppendApiNameConvention(string suffix) : ApiNamingConvention
    {
        #region IApiNamingConvention
        public override string ConvertName(string apiName, ApiNamingConventionContext context)
        {
            return apiName + suffix;
        }
        #endregion
    }

    private sealed class AssemblyScannedEnumValueConvention : IApiEnumTypeConvention
    {
        #region IApiEnumTypeConvention
        public void Apply(ApiEnumTypeBuilder builder)
        {
            if (builder.ClrType == typeof(AssemblyScannedEnum))
            {
                builder.AddValue
                (
                    nameof(AssemblyScannedEnum.Active),
                    nameof(AssemblyScannedEnum.Active),
                    (int)AssemblyScannedEnum.Active
                );
            }
        }
        #endregion
    }

    private sealed class CaptureEnumValueNamingConvention : ApiNamingConvention
    {
        #region Properties
        public ApiNamingConventionContext? EnumValueContext { get; private set; }
        #endregion

        #region IApiNamingConvention
        public override string ConvertName(string apiName, ApiNamingConventionContext context)
        {
            if (context.Target == ApiNamingConventionTarget.EnumValue)
            {
                this.EnumValueContext = context;
            }

            return apiName;
        }
        #endregion
    }

    private sealed class EnumTypeAddsValueConvention : IApiEnumTypeConvention
    {
        #region IApiEnumTypeConvention
        public void Apply(ApiEnumTypeBuilder builder)
        {
            if (builder.ClrType == typeof(PipelineStatus))
            {
                builder.AddValue
                (
                    nameof(PipelineStatus.OnHold),
                    nameof(PipelineStatus.OnHold),
                    (int)PipelineStatus.OnHold
                );
            }
        }
        #endregion
    }

    private sealed class EnumValueAddsValueConvention : IApiEnumValueConvention
    {
        #region Fields
        private bool _hasAddedValue;
        #endregion

        #region IApiEnumValueConvention
        public void Apply(ApiEnumValueBuilder builder, ApiEnumValueConventionContext context)
        {
            if (!_hasAddedValue && builder.ClrName == nameof(PipelineStatus.Active))
            {
                _hasAddedValue = true;
                context.ApiEnumTypeBuilder.AddValue
                (
                    nameof(PipelineStatus.Queued),
                    nameof(PipelineStatus.Queued),
                    (int)PipelineStatus.Queued
                );
            }
        }
        #endregion
    }

    private sealed class ExplicitEnumValueNameConvention : IApiEnumValueConvention
    {
        #region IApiEnumValueConvention
        public void Apply(ApiEnumValueBuilder builder, ApiEnumValueConventionContext context)
        {
            builder.WithName("LockedName");
        }
        #endregion
    }

    private sealed class RecordingEnumValueConvention : IApiEnumValueConvention
    {
        #region Properties
        public List<string> ProcessedClrNames { get; } = [];
        #endregion

        #region IApiEnumValueConvention
        public void Apply(ApiEnumValueBuilder builder, ApiEnumValueConventionContext context)
        {
            this.ProcessedClrNames.Add(builder.ClrName);
        }
        #endregion
    }

    private sealed class NonConvergingEnumValueConvention : IApiEnumValueConvention
    {
        #region Fields
        private int _nextOrdinal = 100;
        #endregion

        #region IApiEnumValueConvention
        public void Apply(ApiEnumValueBuilder builder, ApiEnumValueConventionContext context)
        {
            var clrName = $"Added{_nextOrdinal}";
            context.ApiEnumTypeBuilder.AddValue(clrName, clrName, _nextOrdinal);
            _nextOrdinal++;
        }
        #endregion
    }

    private sealed class RecordingObjectPhaseConvention
    (
        string eventName,
        ApiConventionPhase phase,
        List<string> events
    ) : IApiObjectTypeConvention
    {
        #region IApiConvention
        public ApiConventionPhase Phase { get; } = phase;
        #endregion

        #region IApiObjectTypeConvention
        public void Apply(ApiObjectTypeBuilder builder)
        {
            if (builder.ClrType == typeof(PersonWithId))
            {
                events.Add(eventName);
            }
        }
        #endregion
    }

    private sealed class InvalidPropertyPhaseConvention : IApiPropertyConvention
    {
        #region IApiConvention
        public ApiConventionPhase Phase => ApiConventionPhase.Discovery;
        #endregion

        #region IApiPropertyConvention
        public void Apply(ApiPropertyBuilder builder, ApiPropertyConventionContext context)
        {
        }
        #endregion
    }

    private sealed class RelationshipAddsObjectConvention : IApiRelationshipConvention
    {
        #region IApiRelationshipConvention
        public void Apply(ApiSchemaBuilder builder)
        {
            builder.AddObject<PropertyConventionRegistered>();
        }
        #endregion
    }
    #endregion

    #region ApiSchemaBuilder Built-In Convention Extension Tests
    [Fact]
    public void UsePropertyDiscoveryDiscoversPublicInstanceProperties()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .AddObject<PersonWithId>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        objectType!.ApiProperties.Select(p => p.ClrName).Should()
            .BeEquivalentTo(["Id", "Name", "Email"]);
    }

    [Fact]
    public void UseCamelCaseNamingCamelCasesObjectAndPropertyApiNames()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .UseCamelCaseNaming()
            .AddObject<PersonWithId>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        objectType!.ApiName.Should().Be("personWithId");
        objectType.ApiProperties.Select(p => p.ApiName).Should()
            .BeEquivalentTo(["id", "name", "email"]);
    }

    [Fact]
    public void UsePropertyNullabilityModifiersAppliesRequiredAndOptionalModifiers()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .UsePropertyNullabilityModifiers()
            .AddObject<PersonWithId>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        objectType!.ApiProperties.Single(p => p.ClrName == "Name").ApiTypeModifiers
            .HasFlag(ApiTypeModifiers.Required).Should().BeTrue();
        objectType.ApiProperties.Single(p => p.ClrName == "Email").ApiTypeModifiers
            .HasFlag(ApiTypeModifiers.Required).Should().BeFalse();
    }

    [Fact]
    public void UsePrimaryKeyInferenceCreatesPrimaryKeyForIdProperty()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .UsePrimaryKeyInference()
            .AddObject<PersonWithId>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        objectType!.ApiKeyTypes.Should().ContainSingle(k => k.ApiName == "PrimaryKey");
    }

    [Fact]
    public void UseAssemblyScanningRegistersAnnotatedTypes()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .UseAssemblyScanning(typeof(AssemblyScannedObject).Assembly, IsAssemblyScannedType)
            .UseConventions(c => c.AddConvention(new AssemblyScannedEnumValueConvention()))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(AssemblyScannedObject), out var objectType)
            .Should().BeTrue();
        objectType!.ApiProperties.Select(p => p.ClrName).Should().Contain("Id");
        schema.TryGetScalarTypeByClrType(typeof(AssemblyScannedScalar), out _).Should().BeTrue();
        schema.TryGetEnumTypeByClrType(typeof(AssemblyScannedEnum), out _).Should().BeTrue();

        static bool IsAssemblyScannedType(Type clrType)
        {
            return clrType == typeof(AssemblyScannedObject)
                || clrType == typeof(AssemblyScannedScalar)
                || clrType == typeof(AssemblyScannedEnum);
        }
    }
    #endregion

    #region UsePropertyDiscovery Tests
    [Fact]
    public void UsePropertyDiscoveryDiscoversAllPublicInstanceProperties()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .AddObject<PersonWithId>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        objectType!.ApiProperties.Select(p => p.ClrName).Should()
            .BeEquivalentTo(["Id", "Name", "Email"]);
    }

    [Fact]
    public void UsePropertyDiscoveryDiscoversPublicFields()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .AddObject<TypeWithField>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(TypeWithField), out var objectType).Should().BeTrue();
        objectType!.ApiProperties.Select(p => p.ClrName).Should()
            .BeEquivalentTo(["Id", "Name", "Count"]);
    }

    [Fact]
    public void UsePropertyDiscoveryDoesNotDuplicateExplicitlyAddedProperties()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .AddObject<PersonWithId>(x => x
                .AddProperty("identifier", "Id")
                .AddProperty("displayName", "Name"))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        // Explicit "Id" and "Name" are present; convention adds "Email".
        objectType!.ApiProperties.Select(p => p.ClrName).Should()
            .BeEquivalentTo(["Id", "Name", "Email"]);
        // Explicit API names survive.
        objectType.ApiProperties.Single(p => p.ClrName == "Id").ApiName.Should().Be("identifier");
        objectType.ApiProperties.Single(p => p.ClrName == "Name").ApiName.Should().Be("displayName");
    }
    #endregion

    #region UseCamelCaseNaming Tests
    [Fact]
    public void UseCamelCaseNamingLowercasesObjectTypeApiName()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseCamelCaseNaming()
            .AddObject<PersonWithId>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        objectType!.ApiName.Should().Be("personWithId");
    }

    [Fact]
    public void UseCamelCaseNamingLowercasesScalarTypeApiName()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseCamelCaseNaming()
            .AddScalar<CustomScalar>()
            .Build();

        schema.TryGetScalarTypeByClrType(typeof(CustomScalar), out var scalarType).Should().BeTrue();
        scalarType!.ApiName.Should().Be("customScalar");
    }

    [Fact]
    public void UseCamelCaseNamingLowercasesEnumTypeApiName()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseCamelCaseNaming()
            .AddEnum<CustomEnum>(x => x.AddValue(CustomEnum.Active))
            .Build();

        schema.TryGetEnumTypeByClrType(typeof(CustomEnum), out var enumType).Should().BeTrue();
        enumType!.ApiName.Should().Be("customEnum");
    }

    [Fact]
    public void UseCamelCaseNamingLowercasesMultipleInferredEnumValueApiNames()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .UseCamelCaseNaming()
            .AddEnum<PipelineStatus>(x => x
                .AddValue(PipelineStatus.Active)
                .AddValue(PipelineStatus.InProgress))
            .Build();

        schema.TryGetEnumTypeByClrType(typeof(PipelineStatus), out var enumType).Should().BeTrue();
        enumType!.ApiEnumValues.Select(v => v.ApiName)
            .Should().Equal("active", "inProgress");
    }

    [Fact]
    public void UseCamelCaseNamingLowercasesAddAllValuesApiNames()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .UseCamelCaseNaming()
            .AddEnum<PipelineStatus>(x => x.AddAllValues())
            .Build();

        schema.TryGetEnumTypeByClrType(typeof(PipelineStatus), out var enumType).Should().BeTrue();
        enumType!.ApiEnumValues.Select(v => v.ApiName)
            .Should().Equal("active", "inProgress", "onHold", "queued");
    }

    [Fact]
    public void UseCamelCaseNamingDoesNotOverrideTypedExplicitEnumValueApiName()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .UseCamelCaseNaming()
            .AddEnum<PipelineStatus>(x => x
                .AddValue(PipelineStatus.Active, "Enabled"))
            .Build();

        schema.TryGetEnumTypeByClrType(typeof(PipelineStatus), out var enumType).Should().BeTrue();
        enumType!.ApiEnumValues.Should().ContainSingle().Which.ApiName.Should().Be("Enabled");
    }

    [Fact]
    public void UseCamelCaseNamingDoesNotOverrideStringExplicitEnumValueApiNames()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .UseCamelCaseNaming()
            .AddEnum<PipelineStatus>(x => x
                .AddValue("Enabled", nameof(PipelineStatus.Active), (int)PipelineStatus.Active)
                .AddValue(nameof(PipelineStatus.InProgress), (int)PipelineStatus.InProgress))
            .Build();

        schema.TryGetEnumTypeByClrType(typeof(PipelineStatus), out var enumType).Should().BeTrue();
        enumType!.ApiEnumValues.Select(v => v.ApiName)
            .Should().Equal("Enabled", nameof(PipelineStatus.InProgress));
    }

    [Fact]
    public void ApiNamingConventionsComposeAgainstCurrentInferredEnumValueApiName()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .UseConventions(c => c
                .AddConvention(new AppendApiNameConvention("Api"))
                .AddConvention(new AppendApiNameConvention("Model")))
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .Build();

        schema.TryGetEnumTypeByClrType(typeof(PipelineStatus), out var enumType).Should().BeTrue();
        enumType!.ApiEnumValues.Should().ContainSingle().Which.ApiName
            .Should().Be("ActiveApiModel");
    }

    [Fact]
    public void EnumValueNamingContextProvidesTargetAndClrMemberMetadata()
    {
        var namingConvention = new CaptureEnumValueNamingConvention();

        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .UseConventions(c => c.AddConvention(namingConvention))
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .Build();

        schema.TryGetEnumTypeByClrType(typeof(PipelineStatus), out _).Should().BeTrue();

        var namingContext = namingConvention.EnumValueContext;
        namingContext.Should().NotBeNull();
        namingContext!.Target.Should().Be(ApiNamingConventionTarget.EnumValue);
        namingContext.ClrType.Should().Be(typeof(PipelineStatus));
        namingContext.ClrName.Should().Be(nameof(PipelineStatus.Active));
        namingContext.ApiPropertyConventionContext.Should().BeNull();

        var enumValueContext = namingContext.ApiEnumValueConventionContext;
        enumValueContext.Should().NotBeNull();
        enumValueContext!.ClrEnumType.Should().Be(typeof(PipelineStatus));
        enumValueContext.ClrMemberInfo.Should().NotBeNull();
        enumValueContext.ClrMemberInfo!.Name.Should().Be(nameof(PipelineStatus.Active));
        enumValueContext.ApiEnumTypeBuilder.ClrType.Should().Be(typeof(PipelineStatus));
        enumValueContext.ApiSchemaBuilder.Should().NotBeNull();
    }

    [Fact]
    public void EnumValuesAddedByEnumTypeConventionReceiveEnumValueConventions()
    {
        var recordingConvention = new RecordingEnumValueConvention();

        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .UseConventions(c => c
                .AddConvention(new EnumTypeAddsValueConvention())
                .AddConvention(recordingConvention))
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .Build();

        schema.TryGetEnumTypeByClrType(typeof(PipelineStatus), out var enumType).Should().BeTrue();
        enumType!.ApiEnumValues.Select(v => v.ClrName)
            .Should().BeEquivalentTo(nameof(PipelineStatus.Active), nameof(PipelineStatus.OnHold));
        recordingConvention.ProcessedClrNames
            .Should().BeEquivalentTo(nameof(PipelineStatus.Active), nameof(PipelineStatus.OnHold));
    }

    [Fact]
    public void EnumValuesAddedByEnumValueConventionReceiveLaterEnumValuePass()
    {
        var recordingConvention = new RecordingEnumValueConvention();

        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .UseConventions(c => c
                .AddConvention(new EnumValueAddsValueConvention())
                .AddConvention(recordingConvention))
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .Build();

        schema.TryGetEnumTypeByClrType(typeof(PipelineStatus), out var enumType).Should().BeTrue();
        enumType!.ApiEnumValues.Select(v => v.ClrName)
            .Should().BeEquivalentTo(nameof(PipelineStatus.Active), nameof(PipelineStatus.Queued));
        recordingConvention.ProcessedClrNames
            .Should().Equal(nameof(PipelineStatus.Active), nameof(PipelineStatus.Queued));
    }

    [Fact]
    public void ExplicitEnumValueNameFromConventionOverridesNamingConventions()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .UseConventions(c => c
                .AddConvention(new ExplicitEnumValueNameConvention())
                .AddConvention(new AppendApiNameConvention("Changed")))
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .Build();

        schema.TryGetEnumTypeByClrType(typeof(PipelineStatus), out var enumType).Should().BeTrue();
        enumType!.ApiEnumValues.Should().ContainSingle().Which.ApiName.Should().Be("LockedName");
    }

    [Fact]
    public void ApiNamingConventionTargetPreservesExistingValuesAndAppendsEnumValue()
    {
        ((int)ApiNamingConventionTarget.ObjectType).Should().Be(0);
        ((int)ApiNamingConventionTarget.ScalarType).Should().Be(1);
        ((int)ApiNamingConventionTarget.EnumType).Should().Be(2);
        ((int)ApiNamingConventionTarget.Property).Should().Be(3);
        ((int)ApiNamingConventionTarget.EnumValue).Should().Be(4);
    }

    [Fact]
    public void ApiConventionSetBuilderManagesEnumValueConventions()
    {
        var namingConvention = new AppendApiNameConvention("Api");
        var conventionSet = new ApiConventionSetBuilder()
            .AddConvention(namingConvention)
            .Build();

        conventionSet.EnumValueConventions.Should()
            .ContainSingle().Which.Should().BeSameAs(namingConvention);

        var copiedSet = new ApiConventionSetBuilder(conventionSet).Build();
        copiedSet.EnumValueConventions.Should()
            .ContainSingle().Which.Should().BeSameAs(namingConvention);

        var removedSet = new ApiConventionSetBuilder(copiedSet)
            .RemoveConvention<AppendApiNameConvention>()
            .Build();
        removedSet.EnumValueConventions.Should().BeEmpty();

        ApiConventionSet.CreateDefault().EnumValueConventions.Should()
            .ContainSingle(c => c is ApiNamingCamelCaseConvention);
    }

    [Fact]
    public void UseCamelCaseNamingLowercasesPropertyApiNames()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .UseCamelCaseNaming()
            .AddObject<PersonWithId>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        objectType!.ApiProperties.Select(p => p.ApiName).Should()
            .BeEquivalentTo(["id", "name", "email"]);
    }

    [Fact]
    public void UseCamelCaseNamingLowercasesExpressionInferredPropertyApiNames()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseCamelCaseNaming()
            .AddObject<PersonWithId>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Email))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType)
            .Should().BeTrue();
        objectType!.ApiProperties.Select(p => p.ApiName).Should()
            .BeEquivalentTo(["id", "name", "email"]);
    }

    [Fact]
    public void UseCamelCaseNamingLowercasesRequiredAndOptionalExpressionPropertyApiNames()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseCamelCaseNaming()
            .AddObject<PersonWithId>(x => x
                .AddRequiredProperty(p => p.Name)
                .AddOptionalProperty(p => p.Email))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType)
            .Should().BeTrue();

        var nameProperty = objectType!.ApiProperties.Single(p => p.ClrName == "Name");
        nameProperty.ApiName.Should().Be("name");
        nameProperty.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required).Should().BeTrue();

        var emailProperty = objectType.ApiProperties.Single(p => p.ClrName == "Email");
        emailProperty.ApiName.Should().Be("email");
        emailProperty.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required).Should().BeFalse();
    }

    [Fact]
    public void UseCamelCaseNamingDoesNotOverrideSelectorExplicitPropertyName()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseCamelCaseNaming()
            .AddObject<PersonWithId>(x => x
                .AddProperty(p => p.Email, "EmailAddress"))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType)
            .Should().BeTrue();
        objectType!.ApiProperties.Single().ApiName.Should().Be("EmailAddress");
    }

    [Fact]
    public void UseCamelCaseNamingDoesNotOverrideCallbackExplicitPropertyName()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseCamelCaseNaming()
            .AddObject<PersonWithId>(x => x
                .AddProperty(p => p.Email, p => p.WithName("EmailAddress")))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType)
            .Should().BeTrue();
        objectType!.ApiProperties.Single().ApiName.Should().Be("EmailAddress");
    }

    [Fact]
    public void ApiNamingConventionComposesAgainstCurrentApiName()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseConventions(c => c
                .AddConvention(new AppendApiNameConvention("Api"))
                .AddConvention(new AppendApiNameConvention("Model")))
            .AddObject<PersonWithId>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        objectType!.ApiName.Should().Be("PersonWithIdApiModel");
    }

    [Fact]
    public void UseCamelCaseNamingDoesNotOverrideExplicitObjectTypeName()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseCamelCaseNaming()
            .AddObject<PersonWithId>(x => x.WithName("Person"))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        objectType!.ApiName.Should().Be("Person");
    }

    [Fact]
    public void UseCamelCaseNamingDoesNotOverrideExplicitPropertyName()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .UseCamelCaseNaming()
            .AddObject<PersonWithId>(x => x
                .AddProperty("EmailAddress", "Email"))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        // Explicit "EmailAddress" survives; convention-discovered Id and Name become camelCase.
        objectType!.ApiProperties.Single(p => p.ClrName == "Email").ApiName.Should().Be("EmailAddress");
        objectType.ApiProperties.Single(p => p.ClrName == "Id").ApiName.Should().Be("id");
    }
    #endregion

    #region Property Convention Mutation Tests
    [Fact]
    public void PropertiesAddedByPropertyConventionReceiveLaterPropertyPass()
    {
        var recordingConvention = new RecordingPropertyConvention();

        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseConventions(c => c
                .AddConvention(new AddSiblingPropertyConvention())
                .AddConvention(recordingConvention))
            .AddObject<PropertyConventionTarget>(x => x.AddProperty
            (
                nameof(PropertyConventionTarget.Initial),
                nameof(PropertyConventionTarget.Initial)
            ))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PropertyConventionTarget), out var objectType)
            .Should().BeTrue();
        objectType!.ApiProperties.Select(p => p.ClrName).Should()
            .Equal
            (
                nameof(PropertyConventionTarget.Initial),
                nameof(PropertyConventionTarget.Added)
            );
        recordingConvention.ProcessedProperties.Should().Equal
        (
            (typeof(PropertyConventionTarget), nameof(PropertyConventionTarget.Initial)),
            (typeof(PropertyConventionTarget), nameof(PropertyConventionTarget.Added))
        );
    }

    [Fact]
    public void PropertiesAddedToPreviouslyProcessedObjectReceiveLaterPropertyPass()
    {
        var recordingConvention = new RecordingPropertyConvention();

        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseConventions(c => c
                .AddConvention(new AddPropertyToVisitedObjectConvention())
                .AddConvention(recordingConvention))
            .AddObject<PropertyConventionTarget>(x => x.AddProperty
            (
                nameof(PropertyConventionTarget.Initial),
                nameof(PropertyConventionTarget.Initial)
            ))
            .AddObject<PropertyConventionTrigger>(x => x.AddProperty
            (
                nameof(PropertyConventionTrigger.Trigger),
                nameof(PropertyConventionTrigger.Trigger)
            ))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PropertyConventionTarget), out var objectType)
            .Should().BeTrue();
        objectType!.ApiProperties.Select(p => p.ClrName).Should()
            .Equal
            (
                nameof(PropertyConventionTarget.Initial),
                nameof(PropertyConventionTarget.Added)
            );
        recordingConvention.ProcessedProperties.Should().Equal
        (
            (typeof(PropertyConventionTarget), nameof(PropertyConventionTarget.Initial)),
            (typeof(PropertyConventionTrigger), nameof(PropertyConventionTrigger.Trigger)),
            (typeof(PropertyConventionTarget), nameof(PropertyConventionTarget.Added))
        );
    }

    [Fact]
    public void ObjectTypeConventionsRunBeforePropertiesOnObjectRegisteredByPropertyConvention()
    {
        var events = new List<string>();
        var orderingConvention = new PropertyRegisteredObjectOrderingConvention(events);

        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseConventions(c => c.AddConvention(orderingConvention))
            .AddObject<PropertyConventionTrigger>(x => x.AddProperty
            (
                nameof(PropertyConventionTrigger.Trigger),
                nameof(PropertyConventionTrigger.Trigger)
            ))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PropertyConventionRegistered), out _)
            .Should().BeTrue();
        events.Should().Equal("ObjectType", "Property");
    }

    [Fact]
    public void EnumValueAddedByPropertyConventionReceivesEnumValueConventions()
    {
        var recordingConvention = new RecordingEnumValueConvention();

        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseConventions(c => c
                .AddConvention(new PropertyAddsEnumValueConvention())
                .AddConvention(recordingConvention))
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .AddObject<PropertyConventionTarget>(x => x.AddProperty
            (
                nameof(PropertyConventionTarget.Initial),
                nameof(PropertyConventionTarget.Initial)
            ))
            .Build();

        schema.TryGetEnumTypeByClrType(typeof(PipelineStatus), out var enumType)
            .Should().BeTrue();
        enumType!.ApiEnumValues.Select(value => value.ClrName).Should().Equal
        (
            nameof(PipelineStatus.Active),
            nameof(PipelineStatus.Queued)
        );
        recordingConvention.ProcessedClrNames.Should().Equal
        (
            nameof(PipelineStatus.Active),
            nameof(PipelineStatus.Queued)
        );
    }
    #endregion

    #region Convention Phase Tests
    [Fact]
    public void ObjectConventionsRunInPhaseOrderAndPreserveRegistrationOrder()
    {
        var events = new List<string>();

        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseConventions(c => c
                .AddConvention(new RecordingObjectPhaseConvention
                (
                    "Configuration1",
                    ApiConventionPhase.Configuration,
                    events
                ))
                .AddConvention(new RecordingObjectPhaseConvention
                (
                    "Discovery1",
                    ApiConventionPhase.Discovery,
                    events
                ))
                .AddConvention(new RecordingObjectPhaseConvention
                (
                    "Configuration2",
                    ApiConventionPhase.Configuration,
                    events
                ))
                .AddConvention(new RecordingObjectPhaseConvention
                (
                    "Discovery2",
                    ApiConventionPhase.Discovery,
                    events
                )))
            .AddObject<PersonWithId>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out _).Should().BeTrue();
        events.Should().Equal
        (
            "Discovery1",
            "Discovery2",
            "Configuration1",
            "Configuration2"
        );
    }

    [Fact]
    public void BuildRejectsConventionPhaseThatIsInvalidForTarget()
    {
        var act = () => new ApiSchemaBuilder()
            .WithName("Test")
            .UseConventions(c => c.AddConvention(new InvalidPropertyPhaseConvention()))
            .Build();

        act.Should()
            .Throw<ApiSchemaConfigurationException>()
            .WithMessage
            (
                "*InvalidPropertyPhaseConvention*Discovery*IApiPropertyConvention*" +
                "Configuration*"
            );
    }

    [Fact]
    public void BuildRejectsStructuralRegistrationFromRelationshipConvention()
    {
        var act = () => new ApiSchemaBuilder()
            .WithName("Test")
            .UseConventions(c => c.AddConvention(new RelationshipAddsObjectConvention()))
            .Build();

        act.Should()
            .Throw<ApiSchemaConfigurationException>()
            .WithMessage
            (
                "Relationship conventions cannot register schema types, properties, or enum " +
                "values.*"
            );
    }
    #endregion

    #region UsePropertyNullabilityModifiers Tests
    [Fact]
    public void UsePropertyNullabilityModifiersSetsRequiredForNonNullableProperties()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .UsePropertyNullabilityModifiers()
            .AddObject<PersonWithId>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        var nameProperty = objectType!.ApiProperties.Single(p => p.ClrName == "Name");
        nameProperty.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required).Should().BeTrue();
    }

    [Fact]
    public void UsePropertyNullabilityModifiersSetsOptionalForNullableProperties()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .UsePropertyNullabilityModifiers()
            .AddObject<PersonWithId>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        var emailProperty = objectType!.ApiProperties.Single(p => p.ClrName == "Email");
        emailProperty.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required).Should().BeFalse();
    }

    [Fact]
    public void UsePropertyNullabilityModifiersDoesNotOverrideExplicitModifier()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .UsePropertyNullabilityModifiers()
            .AddObject<PersonWithId>(x => x
                // Email is nullable CLR but forced Required via explicit config.
                .AddProperty("email", "Email", p => p.WithModifiers(m => m.Required())))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        var emailProperty = objectType!.ApiProperties.Single(p => p.ClrName == "Email");
        emailProperty.ApiTypeModifiers.HasFlag(ApiTypeModifiers.Required).Should().BeTrue();
    }
    #endregion

    #region UsePrimaryKeyInference Tests
    [Fact]
    public void UsePrimaryKeyInferenceCreatesKeyForIdProperty()
    {
        // Property discovery must run first so that the "Id" property is registered;
        // key-path validation requires the property to exist on the object type.
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .UsePrimaryKeyInference()
            .AddObject<PersonWithId>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        objectType!.HasKeyTypes.Should().BeTrue();
        objectType.ApiKeyTypes.Should().ContainSingle(k => k.ApiName == "PrimaryKey");
    }

    [Fact]
    public void UsePrimaryKeyInferenceCreatesKeyForClassNameIdProperty()
    {
        // OrderItem has "OrderItemId" which matches the {ClassName}Id pattern.
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .UsePrimaryKeyInference()
            .AddObject<OrderItem>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(OrderItem), out var objectType).Should().BeTrue();
        objectType!.HasKeyTypes.Should().BeTrue();
        objectType.ApiKeyTypes.Should().ContainSingle(k => k.ApiName == "PrimaryKey");
    }

    [Fact]
    public void UsePrimaryKeyInferenceDoesNotDuplicateExplicitlyAddedPrimaryKey()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .UsePrimaryKeyInference()
            .AddObject<PersonWithId>(x => x
                .AddKey("PrimaryKey", b => b.AddPath(typeof(PersonWithId), "Id")))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        objectType!.ApiKeyTypes.Count(k => k.ApiName == "PrimaryKey").Should().Be(1);
    }
    #endregion

    #region UseDefaultConventions Tests
    [Fact]
    public void UseDefaultConventionsAppliesAllDefaultBehaviors()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseDefaultConventions()
            .AddObject<PersonWithId>()
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();

        // Default camelCase naming on the object type.
        objectType!.ApiName.Should().Be("personWithId");

        // Default property discovery found all three properties.
        objectType.ApiProperties.Should().HaveCount(3);

        // Default camelCase naming on properties.
        objectType.ApiProperties.Select(p => p.ApiName).Should()
            .BeEquivalentTo(["id", "name", "email"]);

        // Default nullability-based property modifiers.
        objectType.ApiProperties.Single(p => p.ClrName == "Name").ApiTypeModifiers
            .HasFlag(ApiTypeModifiers.Required).Should().BeTrue();
        objectType.ApiProperties.Single(p => p.ClrName == "Email").ApiTypeModifiers
            .HasFlag(ApiTypeModifiers.Required).Should().BeFalse();

        // Default primary-key inference.
        objectType.HasKeyTypes.Should().BeTrue();
        objectType.ApiKeyTypes.Should().ContainSingle(k => k.ApiName == "PrimaryKey");
    }
    #endregion

    #region Precedence Tests
    [Fact]
    public void ExplicitConfigurationBeatsConvention()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseDefaultConventions()
            .AddObject<PersonWithId>(x => x
                .WithName("MyPerson")
                .AddProperty("emailAddress", "Email", p => p.WithModifiers(m => m.Required())))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out var objectType).Should().BeTrue();
        // Explicit name beats convention camelCase.
        objectType!.ApiName.Should().Be("MyPerson");
        // Explicit modifier (required) beats convention (optional for nullable Email).
        objectType.ApiProperties.Single(p => p.ClrName == "Email").ApiTypeModifiers
            .HasFlag(ApiTypeModifiers.Required).Should().BeTrue();
        // Explicit API name beats convention camelCase.
        objectType.ApiProperties.Single(p => p.ClrName == "Email").ApiName.Should().Be("emailAddress");
    }
    #endregion

    #region AddTypes Tests
    [Fact]
    public void AddTypesRegistersMultipleTypesForConventionProcessing()
    {
        var schema = new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseDefaultConventions()
            .AddTypes(typeof(PersonWithId), typeof(OrderWithPersonId))
            .Build();

        schema.TryGetObjectTypeByClrType(typeof(PersonWithId), out _).Should().BeTrue();
        schema.TryGetObjectTypeByClrType(typeof(OrderWithPersonId), out _).Should().BeTrue();
    }
    #endregion

    #region Failure Tests
    [Fact]
    public void BuildThrowsConfigurationExceptionWhenConventionPipelineDoesNotConverge()
    {
        var act = () => new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UsePropertyDiscovery()
            .UseConventions(c => c.AddConvention(new NonConvergingPropertyConvention()))
            .AddObject<ConventionLoopRoot>()
            .Build();

        act.Should()
            .Throw<ApiSchemaConfigurationException>()
            .WithMessage("The convention pipeline exceeded * iterations.*");
    }

    [Fact]
    public void BuildThrowsConfigurationExceptionWhenPropertyPipelineDoesNotConverge()
    {
        var act = () => new ApiSchemaBuilder()
            .WithName("Test")
            .WithTestScalars()
            .UseConventions(c => c.AddConvention(new NonConvergingPropertyAdditionConvention()))
            .AddObject<PropertyConventionTarget>(x => x.AddProperty
            (
                nameof(PropertyConventionTarget.Initial),
                nameof(PropertyConventionTarget.Initial)
            ))
            .Build();

        act.Should()
            .Throw<ApiSchemaConfigurationException>()
            .WithMessage("The property convention pipeline exceeded * iterations.*");
    }

    [Fact]
    public void BuildThrowsConfigurationExceptionWhenEnumValuePipelineDoesNotConverge()
    {
        var act = () => new ApiSchemaBuilder()
            .WithName("Test")
            .UseConventions(c => c.AddConvention(new NonConvergingEnumValueConvention()))
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .Build();

        act.Should()
            .Throw<ApiSchemaConfigurationException>()
            .WithMessage("The enum-value convention pipeline exceeded * iterations.*");
    }
    #endregion

    #region Dummy Demo Smoke Tests
    [Fact]
    public void DummyMethodConventionsBuildsValidSchema()
    {
        // Verifies that the convention demo in Dummy.cs produces a valid, initializable schema.
        var act = Dummy.DummyMethodConventions;
        act.Should().NotThrow();
    }

    [Fact]
    public void DummyMethodAnnotationsBuildsValidSchema()
    {
        // Verifies that the annotation demo in Dummy.cs produces a valid, initializable schema.
        var act = Dummy.DummyMethodAnnotations;
        act.Should().NotThrow();
    }
    #endregion
}
