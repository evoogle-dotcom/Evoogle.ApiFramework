// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Json;

/// <summary>
///     Handles JSON serialization for <see cref="ApiRelationship"/> instances, with polymorphic
///     dispatch across <see cref="ApiRelationshipOneToOne"/>, <see cref="ApiRelationshipOneToMany"/>,
///     and <see cref="ApiRelationshipManyToMany"/> using <see cref="ApiRelationshipKind"/> as discriminator.
/// </summary>
/// <param name="logger">The optional logger used to emit diagnostics during JSON operations.</param>
public class ApiRelationshipJsonConverter(ILogger<ApiRelationshipJsonConverter>? logger)
    : JsonConverterBase<ApiRelationship>(logger)
{
    #region Property Types
    private readonly record struct ApiRelationshipPropertyNames
    {
        public required string ApiKind { get; init; }
        public required string ApiName { get; init; }
        // One-to-one / one-to-many
        public required string ApiDeleteBehavior { get; init; }
        public required string ApiPrincipalEnd { get; init; }
        public required string ApiDependentEnd { get; init; }
        // Many-to-many
        public required string ApiPrincipalEndA { get; init; }
        public required string ApiPrincipalEndB { get; init; }
        public required string ApiDependentEndA { get; init; }
        public required string ApiDependentEndB { get; init; }
        public required string ClrAssociationObjectType { get; init; }
    }

    private readonly record struct PropertyNames
    {
        public required ApiRelationshipPropertyNames ApiRelationship { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }

        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiRelationship = new ApiRelationshipPropertyNames
                {
                    ApiKind = policy.ConvertName(nameof(ApiRelationship.ApiKind)),
                    ApiName = policy.ConvertName(nameof(ApiRelationship.ApiName)),
                    ApiDeleteBehavior = policy.ConvertName(nameof(ApiRelationshipOneTo.ApiDeleteBehavior)),
                    ApiPrincipalEnd = policy.ConvertName(nameof(ApiRelationshipOneTo.ApiPrincipalEnd)),
                    ApiDependentEnd = policy.ConvertName(nameof(ApiRelationshipOneTo.ApiDependentEnd)),
                    ApiPrincipalEndA = policy.ConvertName(nameof(ApiRelationshipManyToMany.ApiPrincipalEndA)),
                    ApiPrincipalEndB = policy.ConvertName(nameof(ApiRelationshipManyToMany.ApiPrincipalEndB)),
                    ApiDependentEndA = policy.ConvertName(nameof(ApiRelationshipManyToMany.ApiDependentEndA)),
                    ApiDependentEndB = policy.ConvertName(nameof(ApiRelationshipManyToMany.ApiDependentEndB)),
                    ClrAssociationObjectType = policy.ConvertName(nameof(ApiRelationshipManyToMany.ClrAssociationObjectType)),
                },
                ExtensibleBase = GetExtensiblePropertyNames(policy),
            };
    }
    #endregion

    #region Read Types
    private class ApiRelationshipReadData
    {
        public ApiRelationshipKind? ApiKind { get; set; }
        public string? ApiName { get; set; }
        // One-to-one / one-to-many
        public ApiRelationshipDeleteBehavior? ApiDeleteBehavior { get; set; }
        public ApiRelationshipPrincipalEnd? ApiPrincipalEnd { get; set; }
        public ApiRelationshipDependentEnd? ApiDependentEnd { get; set; }
        // Many-to-many
        public ApiRelationshipPrincipalEnd? ApiPrincipalEndA { get; set; }
        public ApiRelationshipPrincipalEnd? ApiPrincipalEndB { get; set; }
        public ApiRelationshipDependentEnd? ApiDependentEndA { get; set; }
        public ApiRelationshipDependentEnd? ApiDependentEndB { get; set; }
        public Type? ClrAssociationObjectType { get; set; }
    }

    private class ReadData : ExtensibleReadData
    {
        public ApiRelationshipReadData? ApiRelationship { get; set; }
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiRelationship.ApiKind, HandleApiKind },
            { propertyNames.ApiRelationship.ApiName, HandleApiName },
            { propertyNames.ApiRelationship.ApiDeleteBehavior, HandleApiDeleteBehavior },
            { propertyNames.ApiRelationship.ApiPrincipalEnd, HandleApiPrincipalEnd },
            { propertyNames.ApiRelationship.ApiDependentEnd, HandleApiDependentEnd },
            { propertyNames.ApiRelationship.ApiPrincipalEndA, HandleApiPrincipalEndA },
            { propertyNames.ApiRelationship.ApiPrincipalEndB, HandleApiPrincipalEndB },
            { propertyNames.ApiRelationship.ApiDependentEndA, HandleApiDependentEndA },
            { propertyNames.ApiRelationship.ApiDependentEndB, HandleApiDependentEndB },
            { propertyNames.ApiRelationship.ClrAssociationObjectType, HandleClrAssociationObjectType },
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };

        private static void HandleApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            context.ReadData.ApiRelationship.ApiKind = _kindConverter.Read(ref reader, typeof(ApiRelationshipKind), context.Options);
        }

        private static void HandleApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            context.ReadData.ApiRelationship.ApiName = reader.GetString();
        }

        private static void HandleApiDeleteBehavior(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            context.ReadData.ApiRelationship.ApiDeleteBehavior = _deleteBehaviorConverter.Read(ref reader, typeof(ApiRelationshipDeleteBehavior), context.Options);
        }

        private static void HandleApiPrincipalEnd(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            var end = JsonSerializer.Deserialize<ApiRelationshipPrincipalEnd>(ref reader, context.Options);
            context.ReadData.ApiRelationship.ApiPrincipalEnd = end;
        }

        private static void HandleApiDependentEnd(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            var end = JsonSerializer.Deserialize<ApiRelationshipDependentEnd>(ref reader, context.Options);
            context.ReadData.ApiRelationship.ApiDependentEnd = end;
        }

        private static void HandleApiPrincipalEndA(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            var end = JsonSerializer.Deserialize<ApiRelationshipPrincipalEnd>(ref reader, context.Options);
            context.ReadData.ApiRelationship.ApiPrincipalEndA = end;
        }

        private static void HandleApiPrincipalEndB(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            var end = JsonSerializer.Deserialize<ApiRelationshipPrincipalEnd>(ref reader, context.Options);
            context.ReadData.ApiRelationship.ApiPrincipalEndB = end;
        }

        private static void HandleApiDependentEndA(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            var end = JsonSerializer.Deserialize<ApiRelationshipDependentEnd>(ref reader, context.Options);
            context.ReadData.ApiRelationship.ApiDependentEndA = end;
        }

        private static void HandleApiDependentEndB(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            var end = JsonSerializer.Deserialize<ApiRelationshipDependentEnd>(ref reader, context.Options);
            context.ReadData.ApiRelationship.ApiDependentEndB = end;
        }

        private static void HandleClrAssociationObjectType(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            context.ReadData.ApiRelationship.ClrAssociationObjectType = _typeJsonConverter.Read(ref reader, typeof(Type), context.Options);
        }
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiRelationshipKind> _kindConverter = new();
    private static readonly EnumJsonConverter<ApiRelationshipDeleteBehavior> _deleteBehaviorConverter = new();
    private static readonly TypeJsonConverter _typeJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiRelationshipJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverterBase<T> Methods
    /// <inheritdoc/>
    protected override IReadContext CreateReadContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultReadContext<PropertyNames, ReadData, ReadHandlers>
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create,
                buildReadHandlers: names => new ReadHandlers(names)
            );

    /// <inheritdoc/>
    protected override IWriteContext CreateWriteContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultWriteContext(logger, options, buildPropertyNames: PropertyNames.Create);

    /// <inheritdoc/>
    protected override ApiRelationship? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiRelationship;

        if (readData?.ApiKind is null)
        {
            return null;
        }

        var apiKindValue = readData.ApiKind.Value;
        ApiRelationship? relationship = apiKindValue switch
        {
            ApiRelationshipKind.OneToOne => new ApiRelationshipOneToOne
            (
                readData.ApiName!,
                readData.ApiPrincipalEnd!,
                readData.ApiDependentEnd!,
                readData.ApiDeleteBehavior ?? ApiRelationshipDeleteBehavior.None
            ),

            ApiRelationshipKind.OneToMany => new ApiRelationshipOneToMany
            (
                readData.ApiName!,
                readData.ApiPrincipalEnd!,
                readData.ApiDependentEnd!,
                readData.ApiDeleteBehavior ?? ApiRelationshipDeleteBehavior.None
            ),

            ApiRelationshipKind.ManyToMany => new ApiRelationshipManyToMany
            (
                readData.ApiName!,
                readData.ApiPrincipalEndA!,
                readData.ApiPrincipalEndB!,
                readData.ApiDependentEndA!,
                readData.ApiDependentEndB!,
                readData.ClrAssociationObjectType!
            ),

            _ => null
        };

        if (relationship is null)
        {
            readContext.Logger.LogError("Unsupported {ApiKind} enumeration value: '{ApiKindValue}'", nameof(ApiRelationshipKind), apiKindValue);
            return null;
        }

        AttachExtensions(relationship, readContext.ReadData.Extensions);
        return relationship;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        ReadJsonObject(ref reader, readContext, readContext.ReadHandlers.PropertyHandlers);
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationship value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiKind(writer, value, writeContext);
            WriteApiName(writer, value, writeContext);

            switch (value)
            {
                case ApiRelationshipOneTo oneToRelationship:
                    WriteApiDeleteBehavior(writer, oneToRelationship, writeContext);
                    WriteApiPrincipalEnd(writer, oneToRelationship, writeContext);
                    WriteApiDependentEnd(writer, oneToRelationship, writeContext);
                    break;

                case ApiRelationshipManyToMany manyToMany:
                    WriteApiPrincipalEndA(writer, manyToMany, writeContext);
                    WriteApiPrincipalEndB(writer, manyToMany, writeContext);
                    WriteApiDependentEndA(writer, manyToMany, writeContext);
                    WriteApiDependentEndB(writer, manyToMany, writeContext);
                    WriteClrAssociationObjectType(writer, manyToMany, writeContext);
                    break;
            }

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKind(Utf8JsonWriter writer, ApiRelationship relationship, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationship.ApiKind, relationship.ApiKind, context.Options, _kindConverter);

    private static void WriteApiName(Utf8JsonWriter writer, ApiRelationship relationship, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyAsString(context.PropertyNames.ApiRelationship.ApiName, relationship.ApiName, context.Options);

    private static void WriteApiDeleteBehavior(Utf8JsonWriter writer, ApiRelationshipOneTo relationship, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationship.ApiDeleteBehavior, relationship.ApiDeleteBehavior, context.Options, _deleteBehaviorConverter);

    private static void WriteApiPrincipalEnd(Utf8JsonWriter writer, ApiRelationshipOneTo relationship, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationship.ApiPrincipalEnd;
        var end = relationship.ApiPrincipalEnd;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            end,
            options,
            end => writer.TryWriteWithSerializer(end, options)
        );
    }

    private static void WriteApiDependentEnd(Utf8JsonWriter writer, ApiRelationshipOneTo relationship, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationship.ApiDependentEnd;
        var end = relationship.ApiDependentEnd;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            end,
            options,
            end => writer.TryWriteWithSerializer(end, options)
        );
    }

    private static void WriteApiPrincipalEndA(Utf8JsonWriter writer, ApiRelationshipManyToMany relationship, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationship.ApiPrincipalEndA;
        var end = relationship.ApiPrincipalEndA;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            end,
            options,
            end => writer.TryWriteWithSerializer(end, options)
        );
    }

    private static void WriteApiPrincipalEndB(Utf8JsonWriter writer, ApiRelationshipManyToMany relationship, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationship.ApiPrincipalEndB;
        var end = relationship.ApiPrincipalEndB;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            end,
            options,
            end => writer.TryWriteWithSerializer(end, options)
        );
    }

    private static void WriteApiDependentEndA(Utf8JsonWriter writer, ApiRelationshipManyToMany relationship, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationship.ApiDependentEndA;
        var end = relationship.ApiDependentEndA;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            end,
            options,
            end => writer.TryWriteWithSerializer(end, options)
        );
    }

    private static void WriteApiDependentEndB(Utf8JsonWriter writer, ApiRelationshipManyToMany relationship, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationship.ApiDependentEndB;
        var end = relationship.ApiDependentEndB;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            end,
            options,
            end => writer.TryWriteWithSerializer(end, options)
        );
    }

    private static void WriteClrAssociationObjectType(Utf8JsonWriter writer, ApiRelationshipManyToMany relationship, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationship.ClrAssociationObjectType, relationship.ClrAssociationObjectType, context.Options, _typeJsonConverter);
    #endregion
}
