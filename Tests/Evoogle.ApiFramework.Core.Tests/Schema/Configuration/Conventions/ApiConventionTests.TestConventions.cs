// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

public partial class ApiConventionTests
{
    #region Test Conventions
    internal sealed class NonConvergingPropertyConvention : IApiPropertyConvention
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

    internal sealed class AddSiblingPropertyConvention : IApiPropertyConvention
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

    internal sealed class AddPropertyToVisitedObjectConvention : IApiPropertyConvention
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

    internal sealed class RecordingPropertyConvention : IApiPropertyConvention
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

    internal sealed class PropertyRegisteredObjectOrderingConvention(List<string> events)
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

    internal sealed class NonConvergingPropertyAdditionConvention : IApiPropertyConvention
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

    internal sealed class PropertyAddsEnumValueConvention : IApiPropertyConvention
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

    internal sealed class AppendApiNameConvention(string suffix) : ApiNamingConvention
    {
        #region ApiNamingConvention Methods
        public override string ConvertName(string apiName, ApiNamingConventionContext context)
        {
            return apiName + suffix;
        }
        #endregion
    }

    internal sealed class AssemblyScannedEnumValueConvention : IApiEnumTypeConvention
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

    internal sealed class CaptureEnumValueNamingConvention : ApiNamingConvention
    {
        #region Properties
        public ApiNamingConventionContext? EnumValueContext { get; private set; }
        #endregion

        #region ApiNamingConvention Methods
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

    internal sealed class EnumTypeAddsValueConvention : IApiEnumTypeConvention
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

    internal sealed class EnumValueAddsValueConvention : IApiEnumValueConvention
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

    internal sealed class ExplicitEnumValueNameConvention : IApiEnumValueConvention
    {
        #region IApiEnumValueConvention
        public void Apply(ApiEnumValueBuilder builder, ApiEnumValueConventionContext context)
        {
            builder.WithName("LockedName");
        }
        #endregion
    }

    internal sealed class RecordingEnumValueConvention : IApiEnumValueConvention
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

    internal sealed class NonConvergingEnumValueConvention : IApiEnumValueConvention
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

    internal sealed class RecordingObjectPhaseConvention
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

    internal sealed class InvalidPropertyPhaseConvention : IApiPropertyConvention
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

    internal sealed class RelationshipAddsObjectConvention : IApiRelationshipConvention
    {
        #region IApiRelationshipConvention
        public void Apply(ApiSchemaBuilder builder)
        {
            builder.AddObject<PropertyConventionRegistered>();
        }
        #endregion
    }
    #endregion
}
