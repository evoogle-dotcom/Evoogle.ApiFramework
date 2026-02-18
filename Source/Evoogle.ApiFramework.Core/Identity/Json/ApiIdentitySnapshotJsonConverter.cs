// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Identity.Json;

public sealed class ApiIdentitySnapshotJsonConverter(ILogger<ApiIdentitySnapshotJsonConverter>? logger)
    : JsonConverterBase<ApiIdentitySnapshot>(logger)
{
    #region Property Types
    private readonly record struct ApiIdentitySnapshotPropertyNames
    {
        #region Immutable Properties
        public required string Kind { get; init; }
        public required string Path { get; init; }
        public required string ScalarValue { get; init; }
        public required string NestedParts { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiIdentitySnapshotPropertyNames ApiIdentitySnapshot { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                ApiIdentitySnapshot = new ApiIdentitySnapshotPropertyNames
                {
                    Kind = policy.ConvertName(nameof(Identity.ApiIdentitySnapshot.Kind)),
                    Path = policy.ConvertName(nameof(Identity.ApiIdentitySnapshot.Path)),
                    ScalarValue = policy.ConvertName(nameof(Identity.ApiIdentitySnapshot.ScalarValue)),
                    NestedParts = policy.ConvertName(nameof(Identity.ApiIdentitySnapshot.NestedParts)),
                }
            };
        #endregion
    }
    #endregion

    #region Read Types
    private class ApiIdentitySnapshotReadData
    {
        #region Properties
        public ApiIdentitySnapshotKind? Kind { get; set; }
        public string? Path { get; set; }
        public ApiId? ScalarValue { get; set; }
        public List<ApiIdentityPart>? NestedParts { get; set; }
        #endregion
    }

    private class ReadData
    {
        #region Properties
        public ApiIdentitySnapshotReadData? ApiIdentitySnapshot { get; set; }
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region ApiIdentitySnapshot Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> ApiIdentitySnapshotPropertyHandlers = new()
        {
            { propertyNames.ApiIdentitySnapshot.Kind, HandleApiSnapshotIdentityKind },
            { propertyNames.ApiIdentitySnapshot.Path, HandleApiSnapshotIdentityPath },
            { propertyNames.ApiIdentitySnapshot.ScalarValue, HandleApiSnapshotIdentityScalarValue },
            { propertyNames.ApiIdentitySnapshot.NestedParts, HandleApiSnapshotIdentityNestedParts },
        };
        #endregion

        #region ApiIdentitySnapshot Methods
        private static void HandleApiSnapshotIdentityKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentitySnapshot ??= new ApiIdentitySnapshotReadData();

            var options = context.Options;
            context.ReadData.ApiIdentitySnapshot.Kind = _kindJsonConverter.Read(ref reader, typeof(ApiIdentitySnapshotKind), options);
        }

        private static void HandleApiSnapshotIdentityPath(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentitySnapshot ??= new ApiIdentitySnapshotReadData();

            context.ReadData.ApiIdentitySnapshot.Path = reader.GetString();
        }

        private static void HandleApiSnapshotIdentityScalarValue(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentitySnapshot ??= new ApiIdentitySnapshotReadData();

            context.ReadData.ApiIdentitySnapshot.ScalarValue = JsonSerializer.Deserialize<ApiId>(ref reader, context.Options);
        }

        private static void HandleApiSnapshotIdentityNestedParts(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.ApiIdentitySnapshot ??= new ApiIdentitySnapshotReadData();

            var options = context.Options;
            var propertyName = context.PropertyNames.ApiIdentitySnapshot.NestedParts;
            var nestedParts = DeserializeListOf<ApiIdentityPart>(ref reader, options, propertyName);

            context.ReadData.ApiIdentitySnapshot.NestedParts = nestedParts;
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<ApiIdentitySnapshotKind> _kindJsonConverter = new();
    #endregion

    #region Constructors
    /// <summary>Parameterless constructor for use via [JsonConverter(typeof(...))] attribute.</summary>
    public ApiIdentitySnapshotJsonConverter()
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

    protected override ApiIdentitySnapshot CreateValue(IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var readData = readContext.ReadData.ApiIdentitySnapshot;

        var kind = readData?.Kind ?? throw new JsonException($"{nameof(ApiIdentitySnapshot)} is missing required {nameof(ApiIdentitySnapshot.Kind)} property.");
        switch (kind)
        {
            case ApiIdentitySnapshotKind.Scalar:
                {
                    var path = readData?.Path;
                    var scalarValue = readData?.ScalarValue;

                    return new ApiIdentitySnapshot(path, scalarValue);
                }

            case ApiIdentitySnapshotKind.Composite:
                {
                    var path = readData?.Path;
                    var nestedParts = readData?.NestedParts;

                    return new ApiIdentitySnapshot(path, nestedParts);
                }

            default:
                throw new JsonException($"Unknown {nameof(ApiIdentitySnapshotKind)}: {kind}.");
        }
    }

    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.ApiIdentitySnapshotPropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiIdentitySnapshot value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteApiIdentitySnapshotKind(writer, value, writeContext);
            WriteApiIdentitySnapshotPath(writer, value, writeContext);
            WriteApiIdentitySnapshotScalarValue(writer, value, writeContext);
            WriteApiIdentitySnapshotNestedParts(writer, value, writeContext);
        });
    }
    #endregion

    #region Write Methods
    private static void WriteApiIdentitySnapshotKind(Utf8JsonWriter writer, ApiIdentitySnapshot snapshot, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentitySnapshot.Kind;
        var kind = snapshot.Kind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, kind, options, _kindJsonConverter);
    }

    private static void WriteApiIdentitySnapshotPath(Utf8JsonWriter writer, ApiIdentitySnapshot snapshot, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentitySnapshot.Path;
        var path = snapshot.Path;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, path, options);
    }

    private static void WriteApiIdentitySnapshotScalarValue(Utf8JsonWriter writer, ApiIdentitySnapshot snapshot, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentitySnapshot.ScalarValue;
        var scalarValue = snapshot.ScalarValue;
        var options = context.Options;

        writer.TryWritePropertyWithSerializer(propertyName, scalarValue, options);
    }

    private static void WriteApiIdentitySnapshotNestedParts(Utf8JsonWriter writer, ApiIdentitySnapshot snapshot, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.ApiIdentitySnapshot.NestedParts;
        var nestedParts = snapshot.NestedParts;
        var options = context.Options;

        writer.TryWritePropertyWithAction
        (
            propertyName,
            nestedParts,
            options,
            collection => WriteJsonArray(writer, collection, item => writer.TryWriteWithSerializer(item, options))
        );
    }
    #endregion

    #region Create Helpers
    #endregion
}
