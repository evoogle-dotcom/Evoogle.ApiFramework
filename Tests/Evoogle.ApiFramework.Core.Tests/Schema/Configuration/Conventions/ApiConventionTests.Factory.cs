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
    #region EnumType - EnumValue Discovery Convention Factory Methods
    internal static ApiSchema BuildWithEnumTypeEnumValueDiscoveryThatDiscoversAllEnumValues()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>()
            .UseEnumValueDiscovery()
            .Build();

        return apiSchema;
    }

    internal static ApiSchema BuildWithEnumTypeEnumValueDiscoveryWithExplicitEnumValueOverride()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active, "Enabled"))
            .UseEnumValueDiscovery()
            .Build();

        return apiSchema;
    }
    #endregion

    #region Naming Convention Factory Methods
    internal static ApiSchema BuildWithNamingConventionForScalarType()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<CustomScalar>()
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    internal static ApiSchema BuildWithNamingConventionForEnumTypeAndAllEnumValues()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x.AddAllValues())
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    internal static ApiSchema BuildWithNamingConventionForEnumTypeAndPreservesTypedExplicitEnumValueApiName()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x
                .AddValue(PipelineStatus.Active, "Enabled"))
            .UseCamelCaseNaming()
            .Build();

        return apiSchema;
    }

    internal static ApiSchema BuildWithNamingConventionForEnumTypeAndPreservesStringExplicitEnumValueApiNames()
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

    internal static ApiSchema BuildWithNamingConventionForObjectTypeAndExpressionInferredPropertyNames()
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

    internal static ApiSchema BuildWithNamingConventionForObjectTypeAndPreservesSelectorExplicitApiName()
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

    internal static ApiSchema BuildWithNamingConventionForObjectTypeAndPreservesCallbackExplicitApiName()
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

    internal static ApiSchema BuildWithNamingConventionForObjectTypeAndPreservesStringBasedExplicitApiNames()
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

    internal static ApiSchema BuildWithCustomNamingConventionsThatAppendsApiAndModelToApiName()
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

    internal static ApiSchema BuildWithCustomNamingConventionsThatHardCodesApiEnumValueAndAppendsChangedToApiName()
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
    #endregion

    #region ObjectType - PrimaryKey Inference Convention Factory Methods
    internal static ApiSchema BuildWithObjectTypePrimaryKeyInferenceThatDiscoversId()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Email))
            .UsePrimaryKeyInference()
            .Build();

        return apiSchema;
    }

    internal static ApiSchema BuildWithObjectTypePrimaryKeyInferenceThatDiscoversIdButDoesNotOverwriteAnExplicitPrimaryKey()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Email)
                .AddKey("PrimaryKey", b => b.AddPath(x => x.Name)))
            .UsePrimaryKeyInference()
            .Build();

        return apiSchema;
    }

    internal static ApiSchema BuildWithObjectTypePrimaryKeyInferenceThatDiscoversClassNameId()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<OrderItem>(x => x
                .AddProperty(p => p.OrderItemId)
                .AddProperty(p => p.Description))
            .UsePrimaryKeyInference()
            .Build();

        return apiSchema;
    }
    #endregion

    #region ObjectType - Property Configuration Convention Factory Methods
    internal static ApiSchema BuildWithObjectTypePropertyNullabilityModifiers()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty(p => p.Email))
            .UsePropertyNullabilityModifiers()
            .Build();

        return apiSchema;
    }

    internal static ApiSchema BuildWithObjectTypePropertyNullabilityModifiersAndExplicitPropertyModifier()
    {
        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .AddScalar<string>()
            .AddObject<PersonWithId>(x => x
                .AddProperty(p => p.Id)
                .AddProperty(p => p.Name)
                .AddProperty("email", "Email", p => p.WithModifiers(m => m.Required())))
            .UsePropertyNullabilityModifiers()
            .Build();

        return apiSchema;
    }
    #endregion

    #region ObjectType - Property Discovery Convention Factory Methods
    internal static ApiSchema BuildWithObjectTypePropertyDiscoveryThatDiscoversPublicInstanceProperties()
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

    internal static ApiSchema BuildWithObjectTypePropertyDiscoveryThatDiscoversPublicInstanceFields()
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

    internal static ApiSchema BuildWithObjectTypePropertyDiscoveryThatDoesNotDuplicateExplicitlyAddedProperties()
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

    #region Schema - Type Discovery Convention Factory Methods
    internal static ApiSchema BuildWithSchemaAssemblyTypeInference()
    {
        var assemblyScannedAssembly = typeof(AssemblyScannedObject).Assembly;

        static bool IsAssemblyScannedType(Type clrType)
        {
            return clrType == typeof(AssemblyScannedObject)
                || clrType == typeof(AssemblyScannedScalar)
                || clrType == typeof(AssemblyScannedEnum);
        }

        var apiSchema = new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<Guid>()
            .UseAssemblyTypeInference(assemblyScannedAssembly, IsAssemblyScannedType)
            .UsePropertyDiscovery()
            .UseEnumValueDiscovery()
            .Build();

        return apiSchema;
    }
    #endregion

    #region Convention Trace Factory Methods
    internal static ApiSchemaBuilder BuildWithEnumTypeAddedValueTrace()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .UseConventions(c => c
                .AddConvention(new EnumTypeAddsValueConvention())
                .AddConvention(new RecordingEnumValueConvention()));
    }

    internal static ApiSchemaBuilder BuildWithEnumValueAddedValueTrace()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .UseConventions(c => c
                .AddConvention(new EnumValueAddsValueConvention())
                .AddConvention(new RecordingEnumValueConvention()));
    }

    internal static ApiSchemaBuilder BuildWithSiblingPropertyTrace()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<PropertyConventionTarget>(x => x.AddProperty
            (
                nameof(PropertyConventionTarget.Initial),
                nameof(PropertyConventionTarget.Initial)
            ))
            .UseConventions(c => c
                .AddConvention(new AddSiblingPropertyConvention())
                .AddConvention(new RecordingPropertyConvention()));
    }

    internal static ApiSchemaBuilder BuildWithVisitedObjectPropertyTrace()
    {
        return new ApiSchemaBuilder()
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
                .AddConvention(new RecordingPropertyConvention()));
    }

    internal static ApiSchemaBuilder BuildWithPropertyRegisteredObjectTrace()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<PropertyConventionTrigger>(x => x.AddProperty
            (
                nameof(PropertyConventionTrigger.Trigger),
                nameof(PropertyConventionTrigger.Trigger)
            ))
            .UseConventions(c => c.AddConvention(new PropertyRegisteredObjectOrderingConvention()));
    }

    internal static ApiSchemaBuilder BuildWithPropertyAddedEnumValueTrace()
    {
        return new ApiSchemaBuilder()
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
                .AddConvention(new RecordingEnumValueConvention()));
    }

    internal static ApiSchemaBuilder BuildWithObjectPhaseTrace()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddObject<PersonWithId>()
            .UseConventions(c => c
                .AddConvention(new RecordingObjectPhaseConvention
                (
                    ApiConventionPhase.Configuration
                ))
                .AddConvention(new RecordingObjectPhaseConvention
                (
                    ApiConventionPhase.Discovery
                ))
                .AddConvention(new RecordingObjectPhaseConvention
                (
                    ApiConventionPhase.Configuration
                ))
                .AddConvention(new RecordingObjectPhaseConvention
                (
                    ApiConventionPhase.Discovery
                )));
    }
    #endregion

    #region Convention Contract Factory Methods
    internal static EnumValueNamingContextSnapshot BuildEnumValueNamingContextSnapshot()
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

    internal static ApiConventionSetSnapshot BuildConventionSetSnapshot()
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
    internal static ApiSchema BuildWithInvalidPropertyConventionPhase()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .UseConventions(c => c.AddConvention(new InvalidPropertyPhaseConvention()))
            .Build();
    }

    internal static ApiSchema BuildWithRelationshipStructuralRegistration()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .UseConventions(c => c.AddConvention(new RelationshipAddsObjectConvention()))
            .Build();
    }

    internal static ApiSchema BuildWithNonConvergingConventionPipeline()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddScalar<int>()
            .AddObject<ConventionLoopRoot>()
            .UsePropertyDiscovery()
            .UseConventions(c => c.AddConvention(new NonConvergingPropertyConvention()))
            .Build();
    }

    internal static ApiSchema BuildWithNonConvergingPropertyPipeline()
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

    internal static ApiSchema BuildWithNonConvergingEnumValuePipeline()
    {
        return new ApiSchemaBuilder()
            .WithName("Test")
            .AddEnum<PipelineStatus>(x => x.AddValue(PipelineStatus.Active))
            .UseConventions(c => c.AddConvention(new NonConvergingEnumValueConvention()))
            .Build();
    }
    #endregion
}
