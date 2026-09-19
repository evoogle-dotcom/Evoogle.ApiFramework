// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.ApiFramework.Schema.Json.Internal;
using Evoogle.ApiFramework.Schema.Relationships;
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
        public required string ApiDeleteBehavior { get; init; }
    }

    private readonly record struct ApiRelationshipOneToPropertyNames
    {
        public required string ApiPrincipalEnd { get; init; }
        public required string ApiDependentEnd { get; init; }
    }

    private readonly record struct ApiRelationshipManyToManyPropertyNames
    {
        public required string ApiPrincipalEndA { get; init; }
        public required string ApiPrincipalEndB { get; init; }
        public required string ApiAssociation { get; init; }
    }

    private readonly record struct PropertyNames
    {
        public required ApiRelationshipPropertyNames ApiRelationship { get; init; }
        public required ApiRelationshipOneToPropertyNames ApiRelationshipOneTo { get; init; }
        public required ApiRelationshipManyToManyPropertyNames ApiRelationshipManyToMany { get; init; }

        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }

        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiRelationship = new ApiRelationshipPropertyNames
                {
                    ApiKind = policy.ConvertName(nameof(Relationships.ApiRelationship.ApiKind)),
                    ApiName = policy.ConvertName(nameof(Relationships.ApiRelationship.ApiName)),
                    ApiDeleteBehavior = policy.ConvertName(nameof(Relationships.ApiRelationship.ApiDeleteBehavior)),
                },
                ApiRelationshipOneTo = new ApiRelationshipOneToPropertyNames
                {
                    ApiPrincipalEnd = policy.ConvertName(nameof(Relationships.ApiRelationshipOneTo.ApiPrincipalEnd)),
                    ApiDependentEnd = policy.ConvertName(nameof(Relationships.ApiRelationshipOneTo.ApiDependentEnd)),
                },
                ApiRelationshipManyToMany = new ApiRelationshipManyToManyPropertyNames
                {
                    ApiPrincipalEndA = policy.ConvertName(nameof(Relationships.ApiRelationshipManyToMany.ApiPrincipalEndA)),
                    ApiPrincipalEndB = policy.ConvertName(nameof(Relationships.ApiRelationshipManyToMany.ApiPrincipalEndB)),
                    ApiAssociation = policy.ConvertName(nameof(Relationships.ApiRelationshipManyToMany.ApiAssociation)),
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
        public JsonEnumReadState<ApiRelationshipDeleteBehavior>? ApiDeleteBehavior { get; set; }
    }

    private class ApiRelationshipOneToReadData
    {
        public ApiRelationshipPrincipalEnd? ApiPrincipalEnd { get; set; }
        public ApiRelationshipDependentEnd? ApiDependentEnd { get; set; }
    }

    private class ApiRelationshipManyToManyReadData
    {
        public ApiRelationshipPrincipalEnd? ApiPrincipalEndA { get; set; }
        public ApiRelationshipPrincipalEnd? ApiPrincipalEndB { get; set; }
        public ApiRelationshipAssociation? ApiAssociation { get; set; }
    }

    private class ReadState : ExtensibleReadData
    {
        public ApiRelationshipReadData? ApiRelationship { get; set; }
        public ApiRelationshipOneToReadData? ApiRelationshipOneTo { get; set; }
        public ApiRelationshipManyToManyReadData? ApiRelationshipManyToMany { get; set; }
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>> PropertyHandlers = new()
        {
            { propertyNames.ApiRelationship.ApiKind, HandleApiKind, true },
            { propertyNames.ApiRelationship.ApiName, HandleApiName },
            { propertyNames.ApiRelationship.ApiDeleteBehavior, HandleApiDeleteBehavior, true },
            { propertyNames.ApiRelationshipOneTo.ApiPrincipalEnd, HandleApiPrincipalEnd },
            { propertyNames.ApiRelationshipOneTo.ApiDependentEnd, HandleApiDependentEnd },
            { propertyNames.ApiRelationshipManyToMany.ApiPrincipalEndA, HandleApiPrincipalEndA },
            { propertyNames.ApiRelationshipManyToMany.ApiPrincipalEndB, HandleApiPrincipalEndB },
            { propertyNames.ApiRelationshipManyToMany.ApiAssociation, HandleApiAssociation },
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadState, ReadHandlers>() },
        };

        private static void HandleApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            context.ReadData.ApiRelationship.ApiKind = _nullableKindJsonConverter.Read
            (
                ref reader,
                typeof(ApiRelationshipKind?),
                context.Options
            );
        }

        private static void HandleApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            context.ReadData.ApiRelationship.ApiName = reader.GetString();
        }

        private static void HandleApiDeleteBehavior(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            var readData = context.ReadData.ApiRelationship;
            readData.ApiDeleteBehavior ??= new JsonEnumReadState<ApiRelationshipDeleteBehavior>();
            readData.ApiDeleteBehavior.Read(ref reader, context.Options, _nullableDeleteBehaviorJsonConverter);
        }

        private static void HandleApiPrincipalEnd(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipOneTo ??= new ApiRelationshipOneToReadData();
            var end = JsonSerializer.Deserialize<ApiRelationshipPrincipalEnd>(ref reader, context.Options);
            context.ReadData.ApiRelationshipOneTo.ApiPrincipalEnd = end;
        }

        private static void HandleApiDependentEnd(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipOneTo ??= new ApiRelationshipOneToReadData();
            var end = JsonSerializer.Deserialize<ApiRelationshipDependentEnd>(ref reader, context.Options);
            context.ReadData.ApiRelationshipOneTo.ApiDependentEnd = end;
        }

        private static void HandleApiPrincipalEndA(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipManyToMany ??= new ApiRelationshipManyToManyReadData();
            var end = JsonSerializer.Deserialize<ApiRelationshipPrincipalEnd>(ref reader, context.Options);
            context.ReadData.ApiRelationshipManyToMany.ApiPrincipalEndA = end;
        }

        private static void HandleApiPrincipalEndB(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipManyToMany ??= new ApiRelationshipManyToManyReadData();
            var end = JsonSerializer.Deserialize<ApiRelationshipPrincipalEnd>(ref reader, context.Options);
            context.ReadData.ApiRelationshipManyToMany.ApiPrincipalEndB = end;
        }

        private static void HandleApiAssociation(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipManyToMany ??= new ApiRelationshipManyToManyReadData();
            var end = JsonSerializer.Deserialize<ApiRelationshipAssociation>(ref reader, context.Options);
            context.ReadData.ApiRelationshipManyToMany.ApiAssociation = end;
        }
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiRelationshipKind> _kindConverter = new();
    private static readonly EnumJsonConverter<ApiRelationshipDeleteBehavior> _deleteBehaviorConverter = new();
    private static readonly NullableEnumJsonConverter<ApiRelationshipKind> _nullableKindJsonConverter =
        new(EnumJsonInvalidValuePolicy.Throw);
    private static readonly NullableEnumJsonConverter<ApiRelationshipDeleteBehavior> _nullableDeleteBehaviorJsonConverter =
        new(EnumJsonInvalidValuePolicy.ReturnNull);
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
        => CreateDefaultReadContext<PropertyNames, ReadState, ReadHandlers>
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
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        var readState = readContext.ReadData;

        var apiRelationshipReadData = readState.ApiRelationship;
        var apiKind = apiRelationshipReadData?.ApiKind
            ?? throw new JsonException($"Missing {nameof(ApiRelationship.ApiKind)} enumeration value.");
        var apiDeleteBehaviorReadState = apiRelationshipReadData.ApiDeleteBehavior;
        ApiRelationship relationship = apiKind switch
        {
            ApiRelationshipKind.OneToOne => new ApiRelationshipOneToOne
            (
                apiRelationshipReadData.ApiName!,
                readState.ApiRelationshipOneTo?.ApiPrincipalEnd!,
                readState.ApiRelationshipOneTo?.ApiDependentEnd!,
                apiDeleteBehaviorReadState?.Value ?? ApiRelationshipOneToOne.DefaultDeleteBehavior
            ),

            ApiRelationshipKind.OneToMany => new ApiRelationshipOneToMany
            (
                apiRelationshipReadData.ApiName!,
                readState.ApiRelationshipOneTo?.ApiPrincipalEnd!,
                readState.ApiRelationshipOneTo?.ApiDependentEnd!,
                apiDeleteBehaviorReadState?.Value ?? ApiRelationshipOneToMany.DefaultDeleteBehavior
            ),

            ApiRelationshipKind.ManyToMany => new ApiRelationshipManyToMany
            (
                apiRelationshipReadData.ApiName!,
                readState.ApiRelationshipManyToMany?.ApiPrincipalEndA!,
                readState.ApiRelationshipManyToMany?.ApiPrincipalEndB!,
                readState.ApiRelationshipManyToMany?.ApiAssociation!,
                apiDeleteBehaviorReadState?.Value ?? ApiRelationshipManyToMany.DefaultDeleteBehavior
            ),

            _ => throw new JsonException($"Unsupported {nameof(ApiRelationshipKind)} enumeration value: '{apiKind}'.")
        };

        if (apiDeleteBehaviorReadState?.IsInvalid == true || apiDeleteBehaviorReadState?.IsNull == true)
        {
            relationship.MarkInvalidApiDeleteBehavior();
        }

        AttachExtensions(relationship, readContext.ReadData.Extensions);
        return relationship;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
        ReadJsonObject
        (
            ref reader,
            readContext,
            readContext.ReadHandlers.PropertyHandlers
        );
    }

    /// <inheritdoc/>
    protected override void WriteCore(Utf8JsonWriter writer, ApiRelationship apiRelationship, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        writer.WriteJsonObject(state: (apiRelationship, writeContext), writeObject: static (writer, state) =>
        {
            var (apiRelationship, writeContext) = state;
            WriteApiKind(writer, apiRelationship, writeContext);
            WriteApiName(writer, apiRelationship, writeContext);

            switch (apiRelationship)
            {
                case ApiRelationshipOneTo oneToRelationship:
                    WriteApiPrincipalEnd(writer, oneToRelationship, writeContext);
                    WriteApiDependentEnd(writer, oneToRelationship, writeContext);
                    break;

                case ApiRelationshipManyToMany manyToMany:
                    WriteApiPrincipalEndA(writer, manyToMany, writeContext);
                    WriteApiPrincipalEndB(writer, manyToMany, writeContext);
                    WriteApiAssociation(writer, manyToMany, writeContext);
                    break;
            }

            WriteApiDeleteBehavior(writer, apiRelationship, writeContext);

            WriteExtensibleBaseExtensions
            (
                writer,
                propertyName: writeContext.PropertyNames.ExtensibleBase.Extensions,
                extensibleBase: apiRelationship,
                context: writeContext
            );
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKind(Utf8JsonWriter writer, ApiRelationship relationship, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiRelationship.ApiKind, value: relationship.ApiKind, options: writeContext.Options, converter: _kindConverter);

    private static void WriteApiDeleteBehavior(Utf8JsonWriter writer, ApiRelationship relationship, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiRelationship.ApiDeleteBehavior, value: relationship.ApiDeleteBehavior, options: writeContext.Options, converter: _deleteBehaviorConverter);

    private static void WriteApiName(Utf8JsonWriter writer, ApiRelationship relationship, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiRelationship.ApiName, value: relationship.ApiName, options: writeContext.Options);

    private static void WriteApiPrincipalEnd(Utf8JsonWriter writer, ApiRelationshipOneTo relationship, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipOneTo.ApiPrincipalEnd, obj: relationship.ApiPrincipalEnd, options: writeContext.Options);

    private static void WriteApiDependentEnd(Utf8JsonWriter writer, ApiRelationshipOneTo relationship, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipOneTo.ApiDependentEnd, obj: relationship.ApiDependentEnd, options: writeContext.Options);

    private static void WriteApiPrincipalEndA(Utf8JsonWriter writer, ApiRelationshipManyToMany relationship, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipManyToMany.ApiPrincipalEndA, obj: relationship.ApiPrincipalEndA, options: writeContext.Options);

    private static void WriteApiPrincipalEndB(Utf8JsonWriter writer, ApiRelationshipManyToMany relationship, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipManyToMany.ApiPrincipalEndB, obj: relationship.ApiPrincipalEndB, options: writeContext.Options);

    private static void WriteApiAssociation(Utf8JsonWriter writer, ApiRelationshipManyToMany relationship, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipManyToMany.ApiAssociation, obj: relationship.ApiAssociation, options: writeContext.Options);
    #endregion
}
