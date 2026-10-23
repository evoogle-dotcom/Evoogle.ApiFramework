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
    /// <summary>
    ///     Stores the resolved JSON property names for the <see cref="ApiRelationship"/> members under a given naming
    ///     policy.
    /// </summary>
    private readonly record struct ApiRelationshipPropertyNames
    {
        #region Immutable Properties
        public required string ApiKind { get; init; }
        public required string ApiName { get; init; }
        public required string ApiDeleteBehavior { get; init; }
        #endregion
    }

    /// <summary>
    ///     Stores the resolved JSON property names for the <see cref="ApiRelationshipOneTo"/> members under a given
    ///     naming policy.
    /// </summary>
    private readonly record struct ApiRelationshipOneToPropertyNames
    {
        #region Immutable Properties
        public required string ApiPrincipalEnd { get; init; }
        public required string ApiDependentEnd { get; init; }
        #endregion
    }

    /// <summary>
    ///     Stores the resolved JSON property names for the <see cref="ApiRelationshipManyToMany"/> members under a
    ///     given naming policy.
    /// </summary>
    private readonly record struct ApiRelationshipManyToManyPropertyNames
    {
        #region Immutable Properties
        public required string ApiPrincipalEndA { get; init; }
        public required string ApiPrincipalEndB { get; init; }
        public required string ApiAssociation { get; init; }
        #endregion
    }

    /// <summary>
    ///     Bundles the property name metadata used during serialization and deserialization.
    /// </summary>
    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiRelationshipPropertyNames ApiRelationship { get; init; }
        public required ApiRelationshipOneToPropertyNames ApiRelationshipOneTo { get; init; }
        public required ApiRelationshipManyToManyPropertyNames ApiRelationshipManyToMany { get; init; }
        public required ExtensibleBasePropertyNames ExtensibleBase { get; init; }
        #endregion

        #region Factory Methods
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
        #endregion
    }
    #endregion

    #region Read Types
    /// <summary>
    ///     Temporary storage used while reading relationship members from JSON.
    /// </summary>
    private class ApiRelationshipReadData
    {
        #region Properties
        public ApiRelationshipKind? ApiKind { get; set; }
        public string? ApiName { get; set; }
        public JsonEnumReadState<ApiRelationshipDeleteBehavior>? ApiDeleteBehavior { get; set; }
        #endregion
    }

    /// <summary>
    ///     Temporary storage used while reading one-to relationship members from JSON.
    /// </summary>
    private class ApiRelationshipOneToReadData
    {
        #region Properties
        public ApiRelationshipPrincipalEnd? ApiPrincipalEnd { get; set; }
        public ApiRelationshipDependentEnd? ApiDependentEnd { get; set; }
        #endregion
    }

    /// <summary>
    ///     Temporary storage used while reading many-to-many relationship members from JSON.
    /// </summary>
    private class ApiRelationshipManyToManyReadData
    {
        #region Properties
        public ApiRelationshipPrincipalEnd? ApiPrincipalEndA { get; set; }
        public ApiRelationshipPrincipalEnd? ApiPrincipalEndB { get; set; }
        public ApiRelationshipAssociation? ApiAssociation { get; set; }
        #endregion
    }

    /// <summary>
    ///     Collects the data required to instantiate an <see cref="ApiRelationship"/> during deserialization.
    /// </summary>
    private class ReadData : ExtensibleReadData
    {
        #region Properties
        public ApiRelationshipReadData? ApiRelationship { get; set; }
        public ApiRelationshipOneToReadData? ApiRelationshipOneTo { get; set; }
        public ApiRelationshipManyToManyReadData? ApiRelationshipManyToMany { get; set; }
        #endregion
    }

    /// <summary>
    ///     Supplies JSON property handlers for mapping serialized values to a <see cref="ReadData"/> instance.
    /// </summary>
    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiRelationship Fields
        public readonly JsonReaderHandlerTable<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>> PropertyHandlers = new()
        {
            // ApiRelationship Property Handlers
            { propertyNames.ApiRelationship.ApiKind, HandleApiRelationshipApiKind, true },
            { propertyNames.ApiRelationship.ApiName, HandleApiRelationshipApiName },
            { propertyNames.ApiRelationship.ApiDeleteBehavior, HandleApiRelationshipApiDeleteBehavior, true },

            // ApiRelationshipOneTo Property Handlers
            { propertyNames.ApiRelationshipOneTo.ApiPrincipalEnd, HandleApiRelationshipOneToApiPrincipalEnd },
            { propertyNames.ApiRelationshipOneTo.ApiDependentEnd, HandleApiRelationshipOneToApiDependentEnd },

            // ApiRelationshipManyToMany Property Handlers
            { propertyNames.ApiRelationshipManyToMany.ApiPrincipalEndA, HandleApiRelationshipManyToManyApiPrincipalEndA },
            { propertyNames.ApiRelationshipManyToMany.ApiPrincipalEndB, HandleApiRelationshipManyToManyApiPrincipalEndB },
            { propertyNames.ApiRelationshipManyToMany.ApiAssociation, HandleApiRelationshipManyToManyApiAssociation },

            // ExtensibleBase Property Handlers
            { propertyNames.ExtensibleBase.Extensions, CreateExtensionsHandler<PropertyNames, ReadData, ReadHandlers>() },
        };
        #endregion

        #region ApiRelationship Methods
        private static void HandleApiRelationshipApiKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();

            context.ReadData.ApiRelationship.ApiKind = _nullableApiRelationshipKindJsonConverter.Read
            (
                ref reader,
                typeof(ApiRelationshipKind?),
                context.Options
            );
        }

        private static void HandleApiRelationshipApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();

            context.ReadData.ApiRelationship.ApiName = reader.GetString();
        }

        private static void HandleApiRelationshipApiDeleteBehavior(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();

            var apiRelationshipReadData = context.ReadData.ApiRelationship;
            apiRelationshipReadData.ApiDeleteBehavior ??= new JsonEnumReadState<ApiRelationshipDeleteBehavior>();
            apiRelationshipReadData.ApiDeleteBehavior.Read(ref reader, context.Options, _nullableApiRelationshipDeleteBehaviorJsonConverter);
        }
        #endregion

        #region ApiRelationshipOneTo Methods
        private static void HandleApiRelationshipOneToApiPrincipalEnd(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipOneTo ??= new ApiRelationshipOneToReadData();

            context.ReadData.ApiRelationshipOneTo.ApiPrincipalEnd =
                JsonSerializer.Deserialize<ApiRelationshipPrincipalEnd>(ref reader, context.Options);
        }

        private static void HandleApiRelationshipOneToApiDependentEnd(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipOneTo ??= new ApiRelationshipOneToReadData();

            context.ReadData.ApiRelationshipOneTo.ApiDependentEnd =
                JsonSerializer.Deserialize<ApiRelationshipDependentEnd>(ref reader, context.Options);
        }
        #endregion

        #region ApiRelationshipManyToMany Methods
        private static void HandleApiRelationshipManyToManyApiPrincipalEndA(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipManyToMany ??= new ApiRelationshipManyToManyReadData();

            context.ReadData.ApiRelationshipManyToMany.ApiPrincipalEndA =
                JsonSerializer.Deserialize<ApiRelationshipPrincipalEnd>(ref reader, context.Options);
        }

        private static void HandleApiRelationshipManyToManyApiPrincipalEndB(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipManyToMany ??= new ApiRelationshipManyToManyReadData();

            context.ReadData.ApiRelationshipManyToMany.ApiPrincipalEndB =
                JsonSerializer.Deserialize<ApiRelationshipPrincipalEnd>(ref reader, context.Options);
        }

        private static void HandleApiRelationshipManyToManyApiAssociation(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiRelationshipManyToMany ??= new ApiRelationshipManyToManyReadData();

            context.ReadData.ApiRelationshipManyToMany.ApiAssociation =
                JsonSerializer.Deserialize<ApiRelationshipAssociation>(ref reader, context.Options);
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiRelationshipKind> _apiRelationshipKindJsonConverter = new();
    private static readonly EnumJsonConverter<ApiRelationshipDeleteBehavior> _apiRelationshipDeleteBehaviorJsonConverter = new();
    private static readonly NullableEnumJsonConverter<ApiRelationshipKind> _nullableApiRelationshipKindJsonConverter =
        new(EnumJsonInvalidValuePolicy.Throw);
    private static readonly NullableEnumJsonConverter<ApiRelationshipDeleteBehavior> _nullableApiRelationshipDeleteBehaviorJsonConverter =
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
        => CreateDefaultReadContext<PropertyNames, ReadData, ReadHandlers>
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create,
                buildReadHandlers: names => new ReadHandlers(names)
            );

    /// <inheritdoc/>
    protected override IWriteContext CreateWriteContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultWriteContext
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create
            );

    /// <inheritdoc/>
    protected override ApiRelationship? CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData;

        var apiRelationshipReadData = readData.ApiRelationship;
        var apiKind = apiRelationshipReadData?.ApiKind
            ?? throw new JsonException($"Missing {nameof(ApiRelationship.ApiKind)} enumeration value.");
        var apiDeleteBehaviorReadState = apiRelationshipReadData.ApiDeleteBehavior;

        ApiRelationship apiRelationship = apiKind switch
        {
            ApiRelationshipKind.OneToOne => new ApiRelationshipOneToOne
            (
                apiRelationshipReadData.ApiName!,
                readData.ApiRelationshipOneTo?.ApiPrincipalEnd!,
                readData.ApiRelationshipOneTo?.ApiDependentEnd!,
                apiDeleteBehaviorReadState?.Value ?? ApiRelationshipOneToOne.DefaultDeleteBehavior
            ),

            ApiRelationshipKind.OneToMany => new ApiRelationshipOneToMany
            (
                apiRelationshipReadData.ApiName!,
                readData.ApiRelationshipOneTo?.ApiPrincipalEnd!,
                readData.ApiRelationshipOneTo?.ApiDependentEnd!,
                apiDeleteBehaviorReadState?.Value ?? ApiRelationshipOneToMany.DefaultDeleteBehavior
            ),

            ApiRelationshipKind.ManyToMany => new ApiRelationshipManyToMany
            (
                apiRelationshipReadData.ApiName!,
                readData.ApiRelationshipManyToMany?.ApiPrincipalEndA!,
                readData.ApiRelationshipManyToMany?.ApiPrincipalEndB!,
                readData.ApiRelationshipManyToMany?.ApiAssociation!,
                apiDeleteBehaviorReadState?.Value ?? ApiRelationshipManyToMany.DefaultDeleteBehavior
            ),

            _ => throw new JsonException($"Unsupported {nameof(ApiRelationshipKind)} enumeration value: '{apiKind}'.")
        };

        if (apiDeleteBehaviorReadState?.IsInvalid == true || apiDeleteBehaviorReadState?.IsNull == true)
        {
            apiRelationship.MarkInvalidApiDeleteBehavior();
        }

        AttachExtensions(apiRelationship, readContext.ReadData.Extensions);
        return apiRelationship;
    }

    /// <inheritdoc/>
    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
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
            WriteApiRelationshipApiKind(writer, apiRelationship, writeContext);
            WriteApiRelationshipApiName(writer, apiRelationship, writeContext);

            switch (apiRelationship)
            {
                case ApiRelationshipOneTo apiRelationshipOneTo:
                    WriteApiRelationshipOneToApiPrincipalEnd(writer, apiRelationshipOneTo, writeContext);
                    WriteApiRelationshipOneToApiDependentEnd(writer, apiRelationshipOneTo, writeContext);
                    break;

                case ApiRelationshipManyToMany apiRelationshipManyToMany:
                    WriteApiRelationshipManyToManyApiPrincipalEndA(writer, apiRelationshipManyToMany, writeContext);
                    WriteApiRelationshipManyToManyApiPrincipalEndB(writer, apiRelationshipManyToMany, writeContext);
                    WriteApiRelationshipManyToManyApiAssociation(writer, apiRelationshipManyToMany, writeContext);
                    break;
            }

            WriteApiRelationshipApiDeleteBehavior(writer, apiRelationship, writeContext);

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
    private static void WriteApiRelationshipApiKind(Utf8JsonWriter writer, ApiRelationship apiRelationship, DefaultWriteContext<PropertyNames> writeContext)
        => writer.WritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiRelationship.ApiKind, value: apiRelationship.ApiKind, options: writeContext.Options, converter: _apiRelationshipKindJsonConverter);

    private static void WriteApiRelationshipApiName(Utf8JsonWriter writer, ApiRelationship apiRelationship, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyAsString(propertyName: writeContext.PropertyNames.ApiRelationship.ApiName, value: apiRelationship.ApiName, options: writeContext.Options);

    private static void WriteApiRelationshipOneToApiPrincipalEnd(Utf8JsonWriter writer, ApiRelationshipOneTo apiRelationshipOneTo, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipOneTo.ApiPrincipalEnd, obj: apiRelationshipOneTo.ApiPrincipalEnd, options: writeContext.Options);

    private static void WriteApiRelationshipOneToApiDependentEnd(Utf8JsonWriter writer, ApiRelationshipOneTo apiRelationshipOneTo, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipOneTo.ApiDependentEnd, obj: apiRelationshipOneTo.ApiDependentEnd, options: writeContext.Options);

    private static void WriteApiRelationshipManyToManyApiPrincipalEndA(Utf8JsonWriter writer, ApiRelationshipManyToMany apiRelationshipManyToMany, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipManyToMany.ApiPrincipalEndA, obj: apiRelationshipManyToMany.ApiPrincipalEndA, options: writeContext.Options);

    private static void WriteApiRelationshipManyToManyApiPrincipalEndB(Utf8JsonWriter writer, ApiRelationshipManyToMany apiRelationshipManyToMany, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipManyToMany.ApiPrincipalEndB, obj: apiRelationshipManyToMany.ApiPrincipalEndB, options: writeContext.Options);

    private static void WriteApiRelationshipManyToManyApiAssociation(Utf8JsonWriter writer, ApiRelationshipManyToMany apiRelationshipManyToMany, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithSerializer(propertyName: writeContext.PropertyNames.ApiRelationshipManyToMany.ApiAssociation, obj: apiRelationshipManyToMany.ApiAssociation, options: writeContext.Options);

    private static void WriteApiRelationshipApiDeleteBehavior(Utf8JsonWriter writer, ApiRelationship apiRelationship, DefaultWriteContext<PropertyNames> writeContext)
        => writer.TryWritePropertyWithConverter(propertyName: writeContext.PropertyNames.ApiRelationship.ApiDeleteBehavior, value: apiRelationship.ApiDeleteBehavior, options: writeContext.Options, converter: _apiRelationshipDeleteBehaviorJsonConverter);
    #endregion
}
