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
                    ApiKind = policy.ConvertName(nameof(ApiRelationship.ApiKind)),
                    ApiName = policy.ConvertName(nameof(ApiRelationship.ApiName)),
                    ApiDeleteBehavior = policy.ConvertName(nameof(ApiRelationship.ApiDeleteBehavior)),
                },
                ApiRelationshipOneTo = new ApiRelationshipOneToPropertyNames
                {
                    ApiPrincipalEnd = policy.ConvertName(nameof(ApiRelationshipOneTo.ApiPrincipalEnd)),
                    ApiDependentEnd = policy.ConvertName(nameof(ApiRelationshipOneTo.ApiDependentEnd)),
                },
                ApiRelationshipManyToMany = new ApiRelationshipManyToManyPropertyNames
                {
                    ApiPrincipalEndA = policy.ConvertName(nameof(ApiRelationshipManyToMany.ApiPrincipalEndA)),
                    ApiPrincipalEndB = policy.ConvertName(nameof(ApiRelationshipManyToMany.ApiPrincipalEndB)),
                    ApiAssociation = policy.ConvertName(nameof(ApiRelationshipManyToMany.ApiAssociation)),
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
        public ApiRelationshipDeleteBehavior? ApiDeleteBehavior { get; set; }
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
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadState, ReadHandlers>>> PropertyHandlers = new()
        {
            { propertyNames.ApiRelationship.ApiKind, HandleApiKind },
            { propertyNames.ApiRelationship.ApiName, HandleApiName },
            { propertyNames.ApiRelationship.ApiDeleteBehavior, HandleApiDeleteBehavior },
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
            context.ReadData.ApiRelationship.ApiKind = _kindConverter.Read(ref reader, typeof(ApiRelationshipKind), context.Options);
        }

        private static void HandleApiName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            context.ReadData.ApiRelationship.ApiName = reader.GetString();
        }

        private static void HandleApiDeleteBehavior(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadState, ReadHandlers> context)
        {
            context.ReadData.ApiRelationship ??= new ApiRelationshipReadData();
            context.ReadData.ApiRelationship.ApiDeleteBehavior = _deleteBehaviorConverter.Read(ref reader, typeof(ApiRelationshipDeleteBehavior), context.Options);
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

        if (readState.ApiRelationship?.ApiKind is null)
        {
            return null;
        }

        var apiKindValue = readState.ApiRelationship.ApiKind.Value;
        ApiRelationship? relationship = apiKindValue switch
        {
            ApiRelationshipKind.OneToOne => new ApiRelationshipOneToOne
            (
                readState.ApiRelationship.ApiName!,
                readState.ApiRelationshipOneTo?.ApiPrincipalEnd!,
                readState.ApiRelationshipOneTo?.ApiDependentEnd!,
                readState.ApiRelationship?.ApiDeleteBehavior ?? ApiRelationshipOneToOne.DefaultDeleteBehavior
            ),

            ApiRelationshipKind.OneToMany => new ApiRelationshipOneToMany
            (
                readState.ApiRelationship.ApiName!,
                readState.ApiRelationshipOneTo?.ApiPrincipalEnd!,
                readState.ApiRelationshipOneTo?.ApiDependentEnd!,
                readState.ApiRelationship?.ApiDeleteBehavior ?? ApiRelationshipOneToMany.DefaultDeleteBehavior
            ),

            ApiRelationshipKind.ManyToMany => new ApiRelationshipManyToMany
            (
                readState.ApiRelationship?.ApiName!,
                readState.ApiRelationshipManyToMany?.ApiPrincipalEndA!,
                readState.ApiRelationshipManyToMany?.ApiPrincipalEndB!,
                readState.ApiRelationshipManyToMany?.ApiAssociation!,
                readState.ApiRelationship?.ApiDeleteBehavior ?? ApiRelationshipManyToMany.DefaultDeleteBehavior
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
        var readContext = (DefaultReadContext<PropertyNames, ReadState, ReadHandlers>)context;
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
                    WriteApiPrincipalEnd(writer, oneToRelationship, writeContext);
                    WriteApiDependentEnd(writer, oneToRelationship, writeContext);
                    break;

                case ApiRelationshipManyToMany manyToMany:
                    WriteApiPrincipalEndA(writer, manyToMany, writeContext);
                    WriteApiPrincipalEndB(writer, manyToMany, writeContext);
                    WriteApiAssociation(writer, manyToMany, writeContext);
                    break;
            }

            WriteApiDeleteBehavior(writer, value, writeContext);

            WriteExtensibleBaseExtensions(writer, writeContext.PropertyNames.ExtensibleBase.Extensions, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiKind(Utf8JsonWriter writer, ApiRelationship relationship, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationship.ApiKind, relationship.ApiKind, context.Options, _kindConverter);

    private static void WriteApiDeleteBehavior(Utf8JsonWriter writer, ApiRelationship relationship, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyWithConverter(context.PropertyNames.ApiRelationship.ApiDeleteBehavior, relationship.ApiDeleteBehavior, context.Options, _deleteBehaviorConverter);

    private static void WriteApiName(Utf8JsonWriter writer, ApiRelationship relationship, DefaultWriteContext<PropertyNames> context)
        => writer.TryWritePropertyAsString(context.PropertyNames.ApiRelationship.ApiName, relationship.ApiName, context.Options);

    private static void WriteApiPrincipalEnd(Utf8JsonWriter writer, ApiRelationshipOneTo relationship, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationshipOneTo.ApiPrincipalEnd;
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
        var propertyName = context.PropertyNames.ApiRelationshipOneTo.ApiDependentEnd;
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
        var propertyName = context.PropertyNames.ApiRelationshipManyToMany.ApiPrincipalEndA;
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
        var propertyName = context.PropertyNames.ApiRelationshipManyToMany.ApiPrincipalEndB;
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

    private static void WriteApiAssociation(Utf8JsonWriter writer, ApiRelationshipManyToMany relationship, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiRelationshipManyToMany.ApiAssociation;
        var end = relationship.ApiAssociation;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            end,
            options,
            end => writer.TryWriteWithSerializer(end, options)
        );
    }
    #endregion
}
