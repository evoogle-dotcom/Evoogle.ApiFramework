// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;

using Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

using static Evoogle.ApiFramework.Schema.Configuration.Conventions.ApiConventionTests;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

[DynamicLinqType]
public static class ApiConventionTestsFactory
{
    #region Camel Case Naming Factory Methods
    public static ApiSchema BuildWithCamelCaseNamingExpressionInferredPropertyNames()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Email))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithCamelCaseNamingRequiredAndOptionalExpressionPropertyNames()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddRequiredProperty(p => p.Id)
                .AddRequiredProperty(p => p.Name)
                .AddOptionalProperty(p => p.Email))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithCamelCaseNamingPreservesSelectorExplicitApiName()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Email, "EmailAddress"))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithCamelCaseNamingPreservesCallbackExplicitApiName()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Email, p => p.WithName("EmailAddress")))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithCamelCaseNamingPreservesStringBasedExplicitApiNames()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .WithName("Person")
                .AddProperty("Id")
                .AddProperty("Name")
                .AddProperty("EmailAddress", "Email"))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithCamelCaseNamingForEnumTypeAndValues()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<CustomEnum>(x => x
                .AddValue(CustomEnum.Active))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }
    #endregion

    #region Property Discovery Factory Methods
    public static ApiSchema BuildWithPropertyDiscoveryDiscoversPublicInstanceProperties()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>()
            .UsePropertyDiscovery()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithPropertyDiscoveryDiscoversPublicFields()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<int>()
            .AddScalar<string>()
            .AddObject<TypeWithField>()
            .UsePropertyDiscovery()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithPropertyDiscoveryDoesNotDuplicateExplicitlyAddedProperties()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddProperty("identifier", "Id")
                .AddProperty("displayName", "Name"))
            .UsePropertyDiscovery()
            .Build();

        return apiSchema;
    }
    #endregion

    #region Built-In Convention Factory Methods
    public static ApiSchema BuildWithPropertyDiscoveryAndCamelCaseNaming()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>()
            .UsePropertyDiscovery()
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithPropertyNullabilityModifiers()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>()
            .UsePropertyDiscovery()
            .UsePropertyNullabilityModifiers()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithPrimaryKeyInference()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>()
            .UsePropertyDiscovery()
            .UsePrimaryKeyInference()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithAssemblyScanning()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .UsePropertyDiscovery()
            .UseAssemblyScanning(typeof(AssemblyScannedObject).Assembly, IsAssemblyScannedType)
            .UseConventions(c => c.AddConvention(new AssemblyScannedEnumValueConvention()))
            .Build();

        return apiSchema;

        static bool IsAssemblyScannedType(Type clrType)
        {
            return clrType == typeof(AssemblyScannedObject)
                || clrType == typeof(AssemblyScannedScalar)
                || clrType == typeof(AssemblyScannedEnum);
        }
    }

    public static ApiSchema BuildWithCamelCaseNamingForScalarType()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<CustomScalar>()
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithCamelCaseNamingForAllEnumValues()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x.AddAllValues())
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithCamelCaseNamingPreservesTypedExplicitEnumValueApiName()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x
                .AddValue(PipelineStatus.Active, "Enabled"))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithCamelCaseNamingPreservesStringExplicitEnumValueApiNames()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x
                .AddValue("Enabled", nameof(PipelineStatus.Active), (int)PipelineStatus.Active)
                .AddValue(nameof(PipelineStatus.InProgress), (int)PipelineStatus.InProgress))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithComposedNamingConventions()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .AddObject<PersonWithId>()
            .UseConventions(c => c
                .AddConvention(new AppendApiNameConvention("Api"))
                .AddConvention(new AppendApiNameConvention("Model")))
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithExplicitEnumValueConventionName()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .UseConventions(c => c
                .AddConvention(new ExplicitEnumValueNameConvention())
                .AddConvention(new AppendApiNameConvention("Changed")))
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithExplicitPropertyModifier()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddProperty("email", "Email", p => p.WithModifiers(m => m.Required())))
            .UsePropertyDiscovery()
            .UsePropertyNullabilityModifiers()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithClassNamePrimaryKeyInference()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<OrderItem>()
            .UsePropertyDiscovery()
            .UsePrimaryKeyInference()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithExistingPrimaryKey()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddKey("PrimaryKey", b => b.AddPath(typeof(PersonWithId), "Id")))
            .UsePropertyDiscovery()
            .UsePrimaryKeyInference()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithDefaultConventions()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>()
            .UseDefaultConventions()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithExplicitConfiguration()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .WithName("MyPerson")
                .AddProperty("emailAddress", "Email", p => p.WithModifiers(m => m.Required())))
            .UseDefaultConventions()
            .Build();

        return apiSchema;
    }

    public static ApiSchema BuildWithAddTypes()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<decimal>()
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddTypes(typeof(PersonWithId), typeof(OrderWithPersonId))
            .UseDefaultConventions()
            .Build();

        return apiSchema;
    }
    #endregion

    #region Convention Trace Factory Methods
    public static ApiConventionBuildTrace BuildWithEnumTypeAddedValueTrace()
    {
        var recordingConvention = new RecordingEnumValueConvention();
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .UseConventions(c => c
                .AddConvention(new EnumTypeAddsValueConvention())
                .AddConvention(recordingConvention))
            .Build();

        return new(apiSchema, recordingConvention.ProcessedClrNames);
    }

    public static ApiConventionBuildTrace BuildWithEnumValueAddedValueTrace()
    {
        var recordingConvention = new RecordingEnumValueConvention();
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .UseConventions(c => c
                .AddConvention(new EnumValueAddsValueConvention())
                .AddConvention(recordingConvention))
            .Build();

        return new(apiSchema, recordingConvention.ProcessedClrNames);
    }

    public static ApiConventionBuildTrace BuildWithSiblingPropertyTrace()
    {
        var recordingConvention = new RecordingPropertyConvention();
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<PropertyConventionTarget>(x => x.AddProperty
            (
                nameof(PropertyConventionTarget.Initial),
                nameof(PropertyConventionTarget.Initial)
            ))
            .UseConventions(c => c
                .AddConvention(new AddSiblingPropertyConvention())
                .AddConvention(recordingConvention))
            .Build();
        var events = recordingConvention.ProcessedProperties
            .Select(p => $"{p.ClrDeclaringType.Name}.{p.ClrName}")
            .ToArray();

        return new(apiSchema, events);
    }

    public static ApiConventionBuildTrace BuildWithVisitedObjectPropertyTrace()
    {
        var recordingConvention = new RecordingPropertyConvention();
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
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
            .UseConventions(c => c
                .AddConvention(new AddPropertyToVisitedObjectConvention())
                .AddConvention(recordingConvention))
            .Build();
        var events = recordingConvention.ProcessedProperties
            .Select(p => $"{p.ClrDeclaringType.Name}.{p.ClrName}")
            .ToArray();

        return new(apiSchema, events);
    }

    public static ApiConventionBuildTrace BuildWithPropertyRegisteredObjectTrace()
    {
        var events = new List<string>();
        var orderingConvention = new PropertyRegisteredObjectOrderingConvention(events);
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<PropertyConventionTrigger>(x => x.AddProperty
            (
                nameof(PropertyConventionTrigger.Trigger),
                nameof(PropertyConventionTrigger.Trigger)
            ))
            .UseConventions(c => c.AddConvention(orderingConvention))
            .Build();

        return new(apiSchema, events);
    }

    public static ApiConventionBuildTrace BuildWithPropertyAddedEnumValueTrace()
    {
        var recordingConvention = new RecordingEnumValueConvention();
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .AddObject<PropertyConventionTarget>(x => x.AddProperty
            (
                nameof(PropertyConventionTarget.Initial),
                nameof(PropertyConventionTarget.Initial)
            ))
            .UseConventions(c => c
                .AddConvention(new PropertyAddsEnumValueConvention())
                .AddConvention(recordingConvention))
            .Build();

        return new(apiSchema, recordingConvention.ProcessedClrNames);
    }

    public static ApiConventionBuildTrace BuildWithObjectPhaseTrace()
    {
        var events = new List<string>();
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddObject<PersonWithId>()
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
            .Build();

        return new(apiSchema, events);
    }
    #endregion

    #region Convention Contract Factory Methods
    public static EnumValueNamingContextSnapshot BuildEnumValueNamingContextSnapshot()
    {
        var namingConvention = new CaptureEnumValueNamingConvention();
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .UseConventions(c => c.AddConvention(namingConvention))
            .Build();
        var isEnumTypeRegistered = apiSchema.TryGetEnumTypeByClrType(typeof(PipelineStatus), out _);
        var namingContext = namingConvention.EnumValueContext
            ?? throw new InvalidOperationException
            (
                "The enum-value naming context was not captured."
            );
        var enumValueContext = namingContext.ApiEnumValueConventionContext
            ?? throw new InvalidOperationException
            (
                "The enum-value convention context was not captured."
            );
        var clrMemberInfo = enumValueContext.ClrMemberInfo
            ?? throw new InvalidOperationException("The enum-value CLR member was not captured.");
        var clrName = namingContext.ClrName
            ?? throw new InvalidOperationException("The enum-value CLR name was not captured.");

        return new
        (
            isEnumTypeRegistered,
            namingContext.Target,
            namingContext.ClrType,
            clrName,
            namingContext.ApiPropertyConventionContext is not null,
            enumValueContext.ClrEnumType,
            clrMemberInfo.Name,
            enumValueContext.ApiEnumTypeBuilder.ClrType,
            enumValueContext.ApiSchemaBuilder is not null
        );
    }

    public static ApiConventionSetSnapshot BuildConventionSetSnapshot()
    {
        var namingConvention = new AppendApiNameConvention("Api");
        var conventionSet = new ApiConventionSetBuilder()
            .AddConvention(namingConvention)
            .Build();
        var copiedSet = new ApiConventionSetBuilder(conventionSet).Build();
        var removedSet = new ApiConventionSetBuilder(copiedSet)
            .RemoveConvention<AppendApiNameConvention>()
            .Build();
        var defaultSet = ApiConventionSet.CreateDefault();

        return new
        (
            conventionSet.EnumValueConventions.SingleOrDefault() == namingConvention,
            copiedSet.EnumValueConventions.SingleOrDefault() == namingConvention,
            removedSet.EnumValueConventions.Count == 0,
            defaultSet.EnumValueConventions.Count == 1 &&
            defaultSet.EnumValueConventions[0] is ApiNamingCamelCaseConvention
        );
    }
    #endregion

    #region Failure Factory Methods
    public static ApiSchema BuildWithInvalidPropertyConventionPhase()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .UseConventions(c => c.AddConvention(new InvalidPropertyPhaseConvention()))
            .Build();
    }

    public static ApiSchema BuildWithRelationshipStructuralRegistration()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .UseConventions(c => c.AddConvention(new RelationshipAddsObjectConvention()))
            .Build();
    }

    public static ApiSchema BuildWithNonConvergingConventionPipeline()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<ConventionLoopRoot>()
            .UsePropertyDiscovery()
            .UseConventions(c => c.AddConvention(new NonConvergingPropertyConvention()))
            .Build();
    }

    public static ApiSchema BuildWithNonConvergingPropertyPipeline()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<PropertyConventionTarget>(x => x.AddProperty
            (
                nameof(PropertyConventionTarget.Initial),
                nameof(PropertyConventionTarget.Initial)
            ))
            .UseConventions(c => c.AddConvention(new NonConvergingPropertyAdditionConvention()))
            .Build();
    }

    public static ApiSchema BuildWithNonConvergingEnumValuePipeline()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .UseConventions(c => c.AddConvention(new NonConvergingEnumValueConvention()))
            .Build();
    }
    #endregion
}
