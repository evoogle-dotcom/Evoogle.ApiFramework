// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Reflection;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Configuration.Conventions;
using Evoogle.ApiFramework.Schema.Configuration.Trace;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Coordinates discovery conventions, annotations, configuration conventions, and
///     relationship configuration for one schema-builder execution.
/// </summary>
internal sealed class ApiSchemaConfigurationPipeline
{
    #region Fields
    private const int _maxIterations = 100;

    private readonly ApiAnnotationReaderSet? _annotationReaderSet;
    private readonly ApiSchemaBuilderContext _context;
    private readonly ApiConventionSchedule _schedule;
    private readonly ApiSchemaBuilder _schemaBuilder;
    #endregion

    #region Constructors
    internal ApiSchemaConfigurationPipeline
    (
        ApiConventionSet? conventionSet,
        ApiAnnotationReaderSet? annotationReaderSet,
        ApiSchemaBuilderContext context,
        ApiSchemaBuilder schemaBuilder
    )
    {
        _schedule = ApiConventionSchedule.Create(conventionSet ?? new ApiConventionSet());
        _annotationReaderSet = annotationReaderSet;
        _context = context;
        _schemaBuilder = schemaBuilder;
    }
    #endregion

    #region Methods
    /// <summary>Runs the complete schema configuration pipeline.</summary>
    internal void Run()
    {
        var objectBuilders = _context.ApiObjectTypeBuilders.ToList();
        var objectBuilderSet = new HashSet<ApiObjectTypeBuilder>
        (
            objectBuilders,
            ReferenceEqualityComparer.Instance
        );

        var enumBuilders = _context.ApiEnumTypeBuilders.ToList();
        var enumBuilderSet = new HashSet<ApiEnumTypeBuilder>
        (
            enumBuilders,
            ReferenceEqualityComparer.Instance
        );

        var annotatedObjectBuilders = new HashSet<ApiObjectTypeBuilder>(ReferenceEqualityComparer.Instance);
        var annotatedScalarBuilders = new HashSet<ApiScalarTypeBuilder>(ReferenceEqualityComparer.Instance);
        var annotatedEnumBuilders = new HashSet<ApiEnumTypeBuilder>(ReferenceEqualityComparer.Instance);
        var annotatedPropertyBuilders = new HashSet<ApiPropertyBuilder>(ReferenceEqualityComparer.Instance);
        var processedPropertyBuilders = new HashSet<ApiPropertyBuilder>(ReferenceEqualityComparer.Instance);
        var processedEnumValueBuilders = new HashSet<ApiEnumValueBuilder>(ReferenceEqualityComparer.Instance);

        this.RecordPhaseStarted(ApiSchemaBuildPhase.Discovery, 0);
        this.ApplySchemaDiscoveryConventions();
        this.RecordPhaseCompleted(ApiSchemaBuildPhase.Discovery, 0);

        var iterations = 0;

        while (true)
        {
            if (iterations++ >= _maxIterations)
            {
                throw new ApiSchemaConfigurationException
                (
                    $"The convention pipeline exceeded {_maxIterations} iterations. " +
                    "This usually indicates that a convention is registering types in a cycle."
                );
            }

            this.RecordPhaseStarted(ApiSchemaBuildPhase.Discovery, iterations - 1);

            foreach (var objectBuilder in _context.DrainPendingObjectBuilders())
            {
                this.ApplyObjectDiscoveryConventions(objectBuilder);
                this.ApplyObjectTypeAnnotations(objectBuilder, annotatedObjectBuilders);
                this.ApplyPropertyAnnotations
                (
                    [objectBuilder],
                    annotatedPropertyBuilders
                );

                foreach (var convention in _schedule.ObjectConfigurationConventions)
                {
                    this.ApplyPropertyAnnotations
                    (
                        [objectBuilder],
                        annotatedPropertyBuilders
                    );
                    this.ApplyConvention
                    (
                        convention,
                        ApiSchemaConfigurationPipeline.GetTarget(objectBuilder),
                        () => convention.Apply(objectBuilder)
                    );
                }

                this.ApplyPropertyAnnotations
                (
                    [objectBuilder],
                    annotatedPropertyBuilders
                );

                if (objectBuilderSet.Add(objectBuilder))
                {
                    objectBuilders.Add(objectBuilder);
                }
            }

            foreach (var scalarBuilder in _context.DrainPendingScalarBuilders())
            {
                this.ApplyScalarTypeAnnotations(scalarBuilder, annotatedScalarBuilders);

                foreach (var convention in _schedule.ScalarConfigurationConventions)
                {
                    this.ApplyConvention
                    (
                        convention,
                        ApiSchemaConfigurationPipeline.GetTarget(scalarBuilder),
                        () => convention.Apply(scalarBuilder)
                    );
                }
            }

            foreach (var enumBuilder in _context.DrainPendingEnumBuilders())
            {
                this.ApplyEnumTypeAnnotations(enumBuilder, annotatedEnumBuilders);

                foreach (var convention in _schedule.EnumTypeConfigurationConventions)
                {
                    this.ApplyConvention
                    (
                        convention,
                        ApiSchemaConfigurationPipeline.GetTarget(enumBuilder),
                        () => convention.Apply(enumBuilder)
                    );
                }

                if (enumBuilderSet.Add(enumBuilder))
                {
                    enumBuilders.Add(enumBuilder);
                }
            }

            if (!_context.HasPendingBuilders)
            {
                this.ApplyRemainingTypeAnnotations
                (
                    annotatedObjectBuilders,
                    annotatedScalarBuilders,
                    annotatedEnumBuilders
                );
            }

            this.RecordPhaseCompleted(ApiSchemaBuildPhase.Discovery, iterations - 1);
            this.RecordPhaseStarted(ApiSchemaBuildPhase.Configuration, iterations - 1);

            this.ApplyEnumValueConventions(enumBuilders, processedEnumValueBuilders);
            this.ApplyPropertyPipeline
            (
                objectBuilders,
                annotatedPropertyBuilders,
                processedPropertyBuilders
            );

            var hasUnprocessedEnumValues = enumBuilders.Any
            (
                enumBuilder => enumBuilder.ApiEnumValueBuilders.Any
                (
                    enumValueBuilder => !processedEnumValueBuilders.Contains(enumValueBuilder)
                )
            );

            if (!_context.HasPendingBuilders && !hasUnprocessedEnumValues)
            {
                this.RecordPhaseCompleted(ApiSchemaBuildPhase.Configuration, iterations - 1);
                break;
            }

            this.RecordPhaseCompleted(ApiSchemaBuildPhase.Configuration, iterations - 1);
        }

        var structuralBuilderCounts = this.GetStructuralBuilderCounts();

        this.RecordPhaseStarted(ApiSchemaBuildPhase.Relationship, 0);
        this.ApplyRelationshipAnnotations(objectBuilders);
        this.ApplyRelationshipConventions();
        this.RecordPhaseCompleted(ApiSchemaBuildPhase.Relationship, 0);

        this.ThrowIfRelationshipStageAddedStructuralBuilders(structuralBuilderCounts);
    }
    #endregion

    #region Discovery Methods
    private void ApplySchemaDiscoveryConventions()
    {
        foreach (var convention in _schedule.SchemaDiscoveryConventions)
        {
            this.ApplyConvention
            (
                convention,
                new(ApiSchemaBuildTargetKind.Schema),
                () => convention.Apply(_schemaBuilder)
            );
        }
    }

    private void ApplyObjectDiscoveryConventions(ApiObjectTypeBuilder builder)
    {
        foreach (var convention in _schedule.ObjectDiscoveryConventions)
        {
            this.ApplyConvention
            (
                convention,
                ApiSchemaConfigurationPipeline.GetTarget(builder),
                () => convention.Apply(builder)
            );
        }
    }
    #endregion

    #region Annotation Methods
    private void ApplyObjectTypeAnnotations
    (
        ApiObjectTypeBuilder builder,
        HashSet<ApiObjectTypeBuilder> annotatedBuilders
    )
    {
        if (annotatedBuilders.Add(builder))
        {
            _annotationReaderSet?.ApplyObjectTypeAnnotations(builder);
        }
    }

    private void ApplyScalarTypeAnnotations
    (
        ApiScalarTypeBuilder builder,
        HashSet<ApiScalarTypeBuilder> annotatedBuilders
    )
    {
        if (annotatedBuilders.Add(builder))
        {
            _annotationReaderSet?.ApplyScalarTypeAnnotations(builder);
        }
    }

    private void ApplyEnumTypeAnnotations
    (
        ApiEnumTypeBuilder builder,
        HashSet<ApiEnumTypeBuilder> annotatedBuilders
    )
    {
        if (annotatedBuilders.Add(builder))
        {
            _annotationReaderSet?.ApplyEnumTypeAnnotations(builder);
        }
    }

    private void ApplyPropertyAnnotations
    (
        IReadOnlyCollection<ApiObjectTypeBuilder> objectBuilders,
        HashSet<ApiPropertyBuilder> annotatedBuilders
    )
    {
        var iterations = 0;

        while (true)
        {
            var hasUnannotatedProperties = false;

            foreach (var objectBuilder in objectBuilders)
            {
                foreach (var propertyBuilder in objectBuilder.ApiPropertyBuilders.ToList())
                {
                    if (!annotatedBuilders.Add(propertyBuilder))
                    {
                        continue;
                    }

                    hasUnannotatedProperties = true;
                    _annotationReaderSet?.ApplyPropertyAnnotations
                    (
                        propertyBuilder,
                        objectBuilder
                    );
                }
            }

            if (!hasUnannotatedProperties)
            {
                return;
            }

            if (iterations++ >= _maxIterations)
            {
                throw new ApiSchemaConfigurationException
                (
                    $"The property annotation pipeline exceeded {_maxIterations} iterations. " +
                    "This usually indicates that an annotation reader is adding properties in " +
                    "a cycle. Check IApiAnnotationReader implementations for unconditional " +
                    "property registrations."
                );
            }
        }
    }

    private void ApplyRemainingTypeAnnotations
    (
        HashSet<ApiObjectTypeBuilder> annotatedObjectBuilders,
        HashSet<ApiScalarTypeBuilder> annotatedScalarBuilders,
        HashSet<ApiEnumTypeBuilder> annotatedEnumBuilders
    )
    {
        foreach (var objectBuilder in _context.ApiObjectTypeBuilders.ToList())
        {
            this.ApplyObjectTypeAnnotations(objectBuilder, annotatedObjectBuilders);
        }

        foreach (var scalarBuilder in _context.ApiScalarTypeBuilders.ToList())
        {
            this.ApplyScalarTypeAnnotations(scalarBuilder, annotatedScalarBuilders);
        }

        foreach (var enumBuilder in _context.ApiEnumTypeBuilders.ToList())
        {
            this.ApplyEnumTypeAnnotations(enumBuilder, annotatedEnumBuilders);
        }
    }

    private void ApplyRelationshipAnnotations
    (
        IReadOnlyCollection<ApiObjectTypeBuilder> objectBuilders
    )
    {
        if (_annotationReaderSet == null)
        {
            return;
        }

        foreach (var objectBuilder in objectBuilders)
        {
            _annotationReaderSet.ApplyRelationshipAnnotations
            (
                _schemaBuilder,
                objectBuilder.ClrType
            );
        }
    }
    #endregion

    #region Configuration Convention Methods
    private void ApplyPropertyPipeline
    (
        IReadOnlyCollection<ApiObjectTypeBuilder> objectBuilders,
        HashSet<ApiPropertyBuilder> annotatedBuilders,
        HashSet<ApiPropertyBuilder> processedBuilders
    )
    {
        var iterations = 0;

        while (true)
        {
            this.ApplyPropertyAnnotations(objectBuilders, annotatedBuilders);

            var hasUnprocessedProperties = false;

            foreach (var objectBuilder in objectBuilders)
            {
                var clrType = objectBuilder.ClrType;

                foreach (var propertyBuilder in objectBuilder.ApiPropertyBuilders.ToList())
                {
                    if (!processedBuilders.Add(propertyBuilder))
                    {
                        continue;
                    }

                    hasUnprocessedProperties = true;
                    var context = this.BuildPropertyContext
                    (
                        propertyBuilder,
                        clrType,
                        objectBuilder
                    );

                    foreach (var convention in _schedule.PropertyConfigurationConventions)
                    {
                        this.ApplyConvention
                        (
                            convention,
                            ApiSchemaConfigurationPipeline.GetTarget(propertyBuilder, objectBuilder.ClrType),
                            () => convention.Apply(propertyBuilder, context)
                        );
                    }
                }
            }

            if (!hasUnprocessedProperties)
            {
                return;
            }

            if (iterations++ >= _maxIterations)
            {
                throw new ApiSchemaConfigurationException
                (
                    $"The property convention pipeline exceeded {_maxIterations} iterations. " +
                    "This usually indicates that a property convention is adding properties in " +
                    "a cycle. Check IApiPropertyConvention implementations for unconditional " +
                    "property registrations."
                );
            }
        }
    }

    private void ApplyEnumValueConventions
    (
        IReadOnlyCollection<ApiEnumTypeBuilder> enumBuilders,
        HashSet<ApiEnumValueBuilder> processedBuilders
    )
    {
        var iterations = 0;

        while (true)
        {
            var hasUnprocessedEnumValues = false;

            foreach (var enumBuilder in enumBuilders)
            {
                foreach (var enumValueBuilder in enumBuilder.ApiEnumValueBuilders.ToList())
                {
                    if (!processedBuilders.Add(enumValueBuilder))
                    {
                        continue;
                    }

                    hasUnprocessedEnumValues = true;
                    var context = this.BuildEnumValueContext(enumValueBuilder, enumBuilder);

                    foreach (var convention in _schedule.EnumValueConfigurationConventions)
                    {
                        this.ApplyConvention
                        (
                            convention,
                            ApiSchemaConfigurationPipeline.GetTarget(enumValueBuilder, enumBuilder.ClrType),
                            () => convention.Apply(enumValueBuilder, context)
                        );
                    }
                }
            }

            if (!hasUnprocessedEnumValues)
            {
                return;
            }

            if (iterations++ >= _maxIterations)
            {
                throw new ApiSchemaConfigurationException
                (
                    $"The enum-value convention pipeline exceeded {_maxIterations} iterations. " +
                    "This usually indicates that an enum-value convention is adding values in " +
                    "a cycle. Check IApiEnumValueConvention implementations for unconditional " +
                    "value registrations."
                );
            }
        }
    }

    private void ApplyRelationshipConventions()
    {
        foreach (var convention in _schedule.RelationshipConventions)
        {
            this.ApplyConvention
            (
                convention,
                new(ApiSchemaBuildTargetKind.Schema),
                () => _schemaBuilder.ApplyRelationshipConvention(convention)
            );
        }
    }

    private void ApplyConvention
    (
        IApiConvention convention,
        ApiSchemaBuildTraceTarget target,
        Action apply
    )
    {
        var traceDispatcher = _context.TraceDispatcher;
        traceDispatcher?.Record
        (
            new ApiSchemaBuildConventionStartedEvent
            {
                ConventionType = convention.GetType(),
                ConventionPhase = convention.Phase,
                Target = target,
            }
        );

        try
        {
            _context.ApplyConfiguration(ApiConfigurationSource.Convention, apply);
            traceDispatcher?.Record
            (
                new ApiSchemaBuildConventionCompletedEvent
                {
                    ConventionType = convention.GetType(),
                    ConventionPhase = convention.Phase,
                    Target = target,
                }
            );
        }
        catch (Exception exception)
        {
            traceDispatcher?.Record
            (
                new ApiSchemaBuildConventionFailedEvent
                {
                    ConventionType = convention.GetType(),
                    ConventionPhase = convention.Phase,
                    Target = target,
                    ExceptionType = exception.GetType().FullName ?? exception.GetType().Name,
                    ExceptionMessage = exception.Message,
                }
            );
            throw;
        }
    }

    private void RecordPhaseStarted(ApiSchemaBuildPhase phase, int iteration)
    {
        _context.TraceDispatcher?.Record
        (
            new ApiSchemaBuildPhaseStartedEvent
            {
                Phase = phase,
                Iteration = iteration,
            }
        );
    }

    private void RecordPhaseCompleted(ApiSchemaBuildPhase phase, int iteration)
    {
        _context.TraceDispatcher?.Record
        (
            new ApiSchemaBuildPhaseCompletedEvent
            {
                Phase = phase,
                Iteration = iteration,
            }
        );
    }

    private static ApiSchemaBuildTraceTarget GetTarget(ApiObjectTypeBuilder builder)
    {
        return new(ApiSchemaBuildTargetKind.ObjectType, builder.ClrType, ApiName: builder.ApiName);
    }

    private static ApiSchemaBuildTraceTarget GetTarget(ApiScalarTypeBuilder builder)
    {
        return new(ApiSchemaBuildTargetKind.ScalarType, builder.ClrType, ApiName: builder.ApiName);
    }

    private static ApiSchemaBuildTraceTarget GetTarget(ApiEnumTypeBuilder builder)
    {
        return new(ApiSchemaBuildTargetKind.EnumType, builder.ClrType, ApiName: builder.ApiName);
    }

    private static ApiSchemaBuildTraceTarget GetTarget
    (
        ApiPropertyBuilder builder,
        Type clrObjectType
    )
    {
        return new
        (
            ApiSchemaBuildTargetKind.Property,
            clrObjectType,
            builder.ClrName,
            builder.ApiName
        );
    }

    private static ApiSchemaBuildTraceTarget GetTarget
    (
        ApiEnumValueBuilder builder,
        Type clrEnumType
    )
    {
        return new
        (
            ApiSchemaBuildTargetKind.EnumValue,
            clrEnumType,
            builder.ClrName,
            builder.ApiName
        );
    }
    #endregion

    #region Validation Methods
    private (int ObjectTypes, int ScalarTypes, int EnumTypes, int Properties, int EnumValues)
        GetStructuralBuilderCounts()
    {
        return
        (
            _context.ApiObjectTypeBuilders.Count(),
            _context.ApiScalarTypeBuilders.Count(),
            _context.ApiEnumTypeBuilders.Count(),
            _context.ApiObjectTypeBuilders.Sum
            (
                builder => builder.ApiPropertyBuilders.Count()
            ),
            _context.ApiEnumTypeBuilders.Sum
            (
                builder => builder.ApiEnumValueBuilders.Count()
            )
        );
    }

    private void ThrowIfRelationshipStageAddedStructuralBuilders
    (
        (
            int ObjectTypes,
            int ScalarTypes,
            int EnumTypes,
            int Properties,
            int EnumValues
        ) previousCounts
    )
    {
        var currentCounts = this.GetStructuralBuilderCounts();
        if (!_context.HasPendingBuilders && currentCounts == previousCounts)
        {
            return;
        }

        throw new ApiSchemaConfigurationException
        (
            "Relationship conventions cannot register schema types, properties, or enum values. " +
            "Register structural builders during discovery or configuration conventions instead."
        );
    }
    #endregion

    #region Context Methods
    private ApiEnumValueConventionContext BuildEnumValueContext
    (
        ApiEnumValueBuilder enumValueBuilder,
        ApiEnumTypeBuilder enumBuilder
    )
    {
        var clrEnumType = enumBuilder.ClrType;
        var clrMemberInfo = clrEnumType.GetField
        (
            enumValueBuilder.ClrName,
            BindingFlags.Public | BindingFlags.Static
        );

        return new ApiEnumValueConventionContext
        (
            clrMemberInfo,
            clrEnumType,
            enumBuilder,
            _schemaBuilder
        );
    }

    private ApiPropertyConventionContext BuildPropertyContext
    (
        ApiPropertyBuilder propertyBuilder,
        Type clrType,
        ApiObjectTypeBuilder objectBuilder
    )
    {
        var clrName = propertyBuilder.ClrName;

        var propertyInfo = TypeReflection.GetProperty(clrType, clrName);
        if (propertyInfo != null)
        {
            var nullabilityInfo = PropertyReflection.GetNullabilityInfo(propertyInfo);
            return new ApiPropertyConventionContext
            (
                ClrMemberKind.Property,
                propertyInfo,
                nullabilityInfo,
                clrType,
                objectBuilder,
                _schemaBuilder
            );
        }

        var fieldInfo = TypeReflection.GetField(clrType, clrName);
        if (fieldInfo != null)
        {
            var nullabilityInfo = FieldReflection.GetNullabilityInfo(fieldInfo);
            return new ApiPropertyConventionContext
            (
                ClrMemberKind.Field,
                fieldInfo,
                nullabilityInfo,
                clrType,
                objectBuilder,
                _schemaBuilder
            );
        }

        return new ApiPropertyConventionContext
        (
            ClrMemberKind.Unknown,
            null,
            null,
            clrType,
            objectBuilder,
            _schemaBuilder
        );
    }
    #endregion
}
