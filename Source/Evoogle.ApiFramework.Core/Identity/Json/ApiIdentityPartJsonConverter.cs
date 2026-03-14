// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity.Json;

#if false
public sealed class ApiIdentityPartJsonConverter(ILogger<ApiIdentityPartJsonConverter>? logger)
    : JsonConverterBase<ApiIdentityPart>(logger)
{
    #region Property Types
    private readonly record struct ApiIdentityPartPropertyNames
    {
        #region Immutable Properties
        public required string Name { get; init; }
        public required string Snapshot { get; init; }
        public required string Structure { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiIdentityPartPropertyNames ApiIdentityPart { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiIdentityPart = new ApiIdentityPartPropertyNames
                {
                    Name = policy.ConvertName(nameof(Identity.ApiIdentityPart.Name)),
                    Snapshot = policy.ConvertName(nameof(Identity.ApiIdentityPart.Snapshot)),
                    Structure = policy.ConvertName(nameof(Identity.ApiIdentityPart.Structure)),
                }
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiIdentityPartReadData
    {
        #region Properties
        public string? Name { get; set; }
        public ApiIdentitySnapshot? Snapshot { get; set; }
        public List<ApiIdentityPart>? Structure { get; set; }
        #endregion
    }

    private class ReadData
    {
        #region Properties
        public ApiIdentityPartReadData? ApiIdentityPart { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> SnapshotPropertyHandlers = new()
        {
            { propertyNames.ApiIdentityPart.Name, HandleApiIdentityPartName },
            { propertyNames.ApiIdentityPart.Snapshot, HandleApiIdentityPartSnapshot },
            { propertyNames.ApiIdentityPart.Structure, HandleApiIdentityPartStructure }
        };
        #endregion

        #region Methods
        private static void HandleApiIdentityPartName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPart ??= new ApiIdentityPartReadData();

            context.ReadData.ApiIdentityPart.Name = reader.GetString();
        }

        private static void HandleApiIdentityPartSnapshot(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPart ??= new ApiIdentityPartReadData();

            var options = context.Options;
            context.ReadData.ApiIdentityPart.Snapshot = JsonSerializer.Deserialize<ApiIdentitySnapshot>(ref reader, options);
        }

        private static void HandleApiIdentityPartStructure(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentityPart ??= new ApiIdentityPartReadData();
            var options = context.Options;
            var propertyName = context.PropertyNames.ApiIdentityPart.Structure;
            var structure = DeserializeListOf<ApiIdentityPart>(ref reader, options, propertyName);

            context.ReadData.ApiIdentityPart.Structure = structure;
        }
        #endregion
    }
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiIdentityPartJsonConverter()
        : this(null)
    {
    }
    #endregion

    #region JsonConverterBase<T> Methods
    protected override IReadContext CreateReadContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultReadContext<PropertyNames, ReadData, ReadHandlers>
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create,
                buildReadHandlers: names => new ReadHandlers(names)
            );

    protected override IWriteContext CreateWriteContext(ILogger logger, JsonSerializerOptions options)
        => CreateDefaultWriteContext
            (
                logger,
                options,
                buildPropertyNames: PropertyNames.Create
            );

    protected override ApiIdentityPart CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiIdentityPart;

        var name = (readData?.Name) ?? throw new JsonException($"{nameof(ApiIdentityPart)} is missing required {nameof(ApiIdentityPart.Name)} property.");
        var snapshot = readData?.Snapshot;
        var structure = readData?.Structure;

        var apiIdentityPart = new ApiIdentityPart(name, snapshot, structure);

        return apiIdentityPart;
    }

    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.SnapshotPropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiIdentityPart value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiIdentityPartName(writer, value, writeContext);
            WriteApiIdentityPartSnapshot(writer, value, writeContext);
            WriteApiIdentityPartStructure(writer, value, writeContext);
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteApiIdentityPartName(Utf8JsonWriter writer, ApiIdentityPart snapshot, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentityPart.Name;
        var name = snapshot.Name;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, name, options);
    }

    private static void WriteApiIdentityPartSnapshot(Utf8JsonWriter writer, ApiIdentityPart part, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentityPart.Snapshot;
        var snapshot = part.Snapshot;
        var options = context.Options;

        writer.TryWritePropertyWithSerializer(propertyName, snapshot, options);
    }

    private static void WriteApiIdentityPartStructure(Utf8JsonWriter writer, ApiIdentityPart part, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentityPart.Structure;
        var structure = part.Structure;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            structure,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }
    #endregion
}
#endif
