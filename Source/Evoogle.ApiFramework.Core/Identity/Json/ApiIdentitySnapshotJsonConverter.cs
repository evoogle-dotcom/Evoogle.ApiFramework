// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;

using Evoogle.Json;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Identity.Json;

/// <summary>
///     JSON converter for <see cref="ApiIdentitySnapshot"/> that round-trips scalar, composite, empty,
///     and unresolved identity snapshots using System.Text.Json.
/// </summary>
/// <remarks>
///     This converter follows the Evoogle JSON infrastructure patterns used by <see cref="ApiIdJsonConverter"/>:
///     it pre-computes property names using the active <see cref="JsonNamingPolicy"/>, uses read handler tables,
///     and constructs the final value in <see cref="CreateValue"/>.
/// </remarks>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiIdentitySnapshotJsonConverter"/> class.
/// </remarks>
/// <param name="logger">The logger used for diagnostics (optional).</param>
public sealed class ApiIdentitySnapshotJsonConverter(ILogger<ApiIdentitySnapshotJsonConverter>? logger)
    : JsonConverterBase<ApiIdentitySnapshot>(logger)
{
    #region Property Types
    private readonly record struct ApiIdentitySnapshotPropertyNames
    {
        #region Immutable Properties
        public required string Name { get; init; }
        public required string ParentPath { get; init; }
        public required string Kind { get; init; }
        public required string Value { get; init; }
        public required string Parts { get; init; }
        #endregion
    }

    private readonly record struct ApiIdentitySnapshotPartEntryPropertyNames
    {
        #region Immutable Properties
        public required string Name { get; init; }
        public required string Kind { get; init; }
        public required string Value { get; init; }
        public required string NestedBlueprint { get; init; }
        public required string IsUnresolved { get; init; }
        #endregion
    }

    private readonly record struct PropertyNames
    {
        #region Immutable Properties
        public required ApiIdentitySnapshotPropertyNames Snapshot { get; init; }
        public required ApiIdentitySnapshotPartEntryPropertyNames PartEntry { get; init; }
        #endregion

        #region Factory Methods
        public static PropertyNames Create(JsonNamingPolicy policy)
            => new()
            {
                Snapshot = new ApiIdentitySnapshotPropertyNames
                {
                    Name = policy.ConvertName(nameof(ApiIdentitySnapshot.Name)),
                    ParentPath = policy.ConvertName("ParentPath"),
                    Kind = policy.ConvertName("Kind"),
                    Value = policy.ConvertName("Value"),
                    Parts = policy.ConvertName("Parts"),
                },
                PartEntry = new ApiIdentitySnapshotPartEntryPropertyNames
                {
                    Name = policy.ConvertName(nameof(ApiIdentityPartEntry.Name)),
                    Kind = policy.ConvertName(nameof(ApiIdentityPart.Kind)),
                    Value = policy.ConvertName("Value"),
                    NestedBlueprint = policy.ConvertName(nameof(ApiIdentityPartEntry.NestedBlueprint)),
                    IsUnresolved = policy.ConvertName("IsUnresolved"),
                }
            };
        #endregion
    }
    #endregion

    #region Internal Types
    private enum SnapshotKind
    {
        Empty = 0,
        Scalar = 1,
        Composite = 2,
    }
    #endregion

    #region Read Types
    private class ApiIdentitySnapshotReadData
    {
        #region Properties
        public string? Name { get; set; }
        public string? ParentPath { get; set; }
        public SnapshotKind? Kind { get; set; }
        public JsonElement? ValueElement { get; set; }
        public List<ApiIdentityPartEntryReadData?>? Parts { get; set; }
        #endregion
    }

    private class ApiIdentityPartEntryReadData
    {
        #region Properties
        public string? Name { get; set; }
        public ApiIdentityPartKind? Kind { get; set; }
        public bool? IsUnresolved { get; set; }
        public JsonElement? ValueElement { get; set; }
        public List<ApiIdentityPartEntryReadData?>? NestedBlueprint { get; set; }
        #endregion
    }

    private class ReadData
    {
        #region Properties
        public ApiIdentitySnapshotReadData? Snapshot { get; set; }
        public ApiIdentityPartEntryReadData? PartEntry { get; set; }

        public Stack<List<ApiIdentityPartEntryReadData?>> NestedBlueprintStack { get; } = new();
        #endregion
    }

    private class ReadHandlers(PropertyNames propertyNames)
    {
        #region Snapshot Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> SnapshotPropertyHandlers = new()
        {
            { propertyNames.Snapshot.Name, HandleSnapshotName },
            { propertyNames.Snapshot.ParentPath, HandleSnapshotParentPath },
            { propertyNames.Snapshot.Kind, HandleSnapshotKind },
            { propertyNames.Snapshot.Value, HandleSnapshotValue },
            { propertyNames.Snapshot.Parts, HandleSnapshotParts },
        };
        #endregion

        #region PartEntry Fields
        public readonly Dictionary<string, JsonReaderHandler<DefaultReadContext<PropertyNames, ReadData, ReadHandlers>>> PartEntryPropertyHandlers = new()
        {
            { propertyNames.PartEntry.Name, HandlePartEntryName },
            { propertyNames.PartEntry.Kind, HandlePartEntryKind },
            { propertyNames.PartEntry.IsUnresolved, HandlePartEntryIsUnresolved },
            { propertyNames.PartEntry.Value, HandlePartEntryValue },
            { propertyNames.PartEntry.NestedBlueprint, HandlePartEntryNestedBlueprint },
        };
        #endregion

        #region Snapshot Methods
        private static void HandleSnapshotName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.Snapshot ??= new ApiIdentitySnapshotReadData();
            context.ReadData.Snapshot.Name = reader.GetString();
        }

        private static void HandleSnapshotParentPath(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.Snapshot ??= new ApiIdentitySnapshotReadData();
            context.ReadData.Snapshot.ParentPath = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
        }

        private static void HandleSnapshotKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.Snapshot ??= new ApiIdentitySnapshotReadData();

            var options = context.Options;
            context.ReadData.Snapshot.Kind = _snapshotKindJsonConverter.Read(ref reader, typeof(SnapshotKind), options);
        }

        private static void HandleSnapshotValue(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.Snapshot ??= new ApiIdentitySnapshotReadData();

            // Snapshot value is an ApiId (object) or null.
            context.ReadData.Snapshot.ValueElement = ReadValueElement(ref reader);
        }

        private static void HandleSnapshotParts(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.Snapshot ??= new ApiIdentitySnapshotReadData();
            context.ReadData.Snapshot.Parts = [];

            ReadJsonArray(ref reader, context, static _ => HandleSnapshotPartsArrayItem);
        }

        private static void HandleSnapshotPartsArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var partEntry = ReadPartEntryObject(ref reader, context);
            context.ReadData.Snapshot!.Parts!.Add(partEntry);
        }
        #endregion

        #region PartEntry Methods
        private static void HandlePartEntryName(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.PartEntry ??= new ApiIdentityPartEntryReadData();
            context.ReadData.PartEntry.Name = reader.GetString();
        }

        private static void HandlePartEntryKind(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.PartEntry ??= new ApiIdentityPartEntryReadData();

            var options = context.Options;
            context.ReadData.PartEntry.Kind = _partKindJsonConverter.Read(ref reader, typeof(ApiIdentityPartKind), options);
        }

        private static void HandlePartEntryIsUnresolved(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.PartEntry ??= new ApiIdentityPartEntryReadData();

            context.ReadData.PartEntry.IsUnresolved = reader.TokenType == JsonTokenType.Null
                ? null
                : reader.GetBoolean();
        }

        private static void HandlePartEntryValue(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.PartEntry ??= new ApiIdentityPartEntryReadData();

            // Value is ambiguous (ApiId and ApiIdentitySnapshot are both objects), so we buffer it.
            context.ReadData.PartEntry.ValueElement = ReadValueElement(ref reader);
        }

        private static void HandlePartEntryNestedBlueprint(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            context.ReadData.PartEntry ??= new ApiIdentityPartEntryReadData();
            context.ReadData.PartEntry.NestedBlueprint = [];

            var current = context.ReadData.PartEntry.NestedBlueprint;
            context.ReadData.NestedBlueprintStack.Push(current);
            try
            {
                ReadJsonArray(ref reader, context, static _ => HandlePartEntryNestedBlueprintArrayItem);
            }
            finally
            {
                context.ReadData.NestedBlueprintStack.Pop();
            }
        }

        private static void HandlePartEntryNestedBlueprintArrayItem(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var blueprintEntry = ReadPartEntryObject(ref reader, context);

            if (context.ReadData.NestedBlueprintStack.Count == 0)
            {
                throw new JsonException("Internal error: NestedBlueprintStack is empty while reading nested blueprint array.");
            }

            context.ReadData.NestedBlueprintStack.Peek().Add(blueprintEntry);
        }
        #endregion

        #region Helper Methods
        private static ApiIdentityPartEntryReadData? ReadPartEntryObject(ref Utf8JsonReader reader, DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context)
        {
            var previousPartEntry = context.ReadData.PartEntry;
            context.ReadData.PartEntry = new ApiIdentityPartEntryReadData();

            try
            {
                ReadJsonObject(ref reader, context, context.ReadHandlers.PartEntryPropertyHandlers);
                return context.ReadData.PartEntry;
            }
            finally
            {
                context.ReadData.PartEntry = previousPartEntry;
            }
        }

        private static JsonElement ReadValueElement(ref Utf8JsonReader reader)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return doc.RootElement.Clone();
        }
        #endregion
    }
    #endregion

    #region Fields
    private static readonly EnumJsonConverter<SnapshotKind> _snapshotKindJsonConverter = new();
    private static readonly EnumJsonConverter<ApiIdentityPartKind> _partKindJsonConverter = new();
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
        var propertyNames = readContext.PropertyNames;
        var readData = readContext.ReadData.Snapshot;

        var name = readData?.Name;
        if (string.IsNullOrWhiteSpace(name))
        {
            var propertyName = propertyNames.Snapshot.Name;
            throw new JsonException($"Missing required property: {propertyName}.");
        }

        var parentPath = readData?.ParentPath;
        var kind = readData?.Kind;

        // Allow Kind to be omitted by inferring from the payload.
        if (kind is null)
        {
            if (readData?.Parts is { Count: > 0 })
            {
                kind = SnapshotKind.Composite;
            }
            else if (readData?.ValueElement is not null)
            {
                kind = SnapshotKind.Scalar;
            }
            else
            {
                kind = SnapshotKind.Empty;
            }
        }

        switch (kind.Value)
        {
            case SnapshotKind.Scalar:
                {
                    var valueElement = readData?.ValueElement;
                    if (valueElement is null)
                    {
                        var valuePropertyName = propertyNames.Snapshot.Value;
                        throw new JsonException($"Missing required property: {valuePropertyName} for scalar snapshot.");
                    }

                    ApiId apiId;
                    if (valueElement.Value.ValueKind == JsonValueKind.Null)
                    {
                        apiId = ApiId.Empty;
                    }
                    else
                    {
                        apiId = valueElement.Value.Deserialize<ApiId>(readContext.Options);
                    }

                    return ApiIdentitySnapshot.Scalar(name, apiId, parentPath);
                }

            case SnapshotKind.Empty:
                return ApiIdentitySnapshot.Empty(name, parentPath);

            case SnapshotKind.Composite:
                {
                    var partsReadData = readData?.Parts ?? [];
                    var parts = new ApiIdentityPartEntry[partsReadData.Count];

                    for (var i = 0; i < partsReadData.Count; i++)
                    {
                        var partReadData = partsReadData[i];
                        if (partReadData is null)
                        {
                            var partsPropertyName = propertyNames.Snapshot.Parts;
                            throw new JsonException($"Null part entry at index {i} in property: {partsPropertyName}.");
                        }

                        parts[i] = CreatePartEntry(readContext, partReadData, partIndex: i);
                    }

                    return ApiIdentitySnapshot.Composite(name, parts, parentPath);
                }

            default:
                throw new JsonException($"Unknown snapshot kind: {kind}.");
        }
    }

    protected override void ReadCore(ref Utf8JsonReader reader, IReadContext context)
    {
        var readContext = (DefaultReadContext<PropertyNames, ReadData, ReadHandlers>)context;
        var handlers = readContext.ReadHandlers.SnapshotPropertyHandlers;

        ReadJsonObject(ref reader, readContext, handlers);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ApiIdentitySnapshot value, IWriteContext context)
    {
        var writeContext = (DefaultWriteContext<PropertyNames>)context;

        WriteJsonObject(writer, () =>
        {
            WriteSnapshotName(writer, value, writeContext);
            WriteSnapshotParentPath(writer, value, writeContext);
            WriteSnapshotKind(writer, value, writeContext);

            if (value.IsScalar)
            {
                WriteSnapshotValue(writer, value, writeContext);
            }
            else
            {
                WriteSnapshotParts(writer, value, writeContext);
            }
        });
    }
    #endregion

    #region Write Implementation Methods
    private static void WriteSnapshotName(Utf8JsonWriter writer, ApiIdentitySnapshot snapshot, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.Snapshot.Name;
        var value = snapshot.Name;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, value, options);
    }

    private static void WriteSnapshotParentPath(Utf8JsonWriter writer, ApiIdentitySnapshot snapshot, DefaultWriteContext<PropertyNames> context)
    {
        var parentPath = GetParentPath(snapshot.Path);
        if (string.IsNullOrWhiteSpace(parentPath))
        {
            return;
        }

        var propertyName = context.PropertyNames.Snapshot.ParentPath;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, parentPath, options);
    }

    private static void WriteSnapshotKind(Utf8JsonWriter writer, ApiIdentitySnapshot snapshot, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.Snapshot.Kind;
        var options = context.Options;

        SnapshotKind kind;
        if (snapshot.IsScalar)
        {
            kind = SnapshotKind.Scalar;
        }
        else if (snapshot.PartCount == 0)
        {
            kind = SnapshotKind.Empty;
        }
        else
        {
            kind = SnapshotKind.Composite;
        }

        writer.TryWritePropertyWithConverter(propertyName, kind, options, _snapshotKindJsonConverter);
    }

    private static void WriteSnapshotValue(Utf8JsonWriter writer, ApiIdentitySnapshot snapshot, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.Snapshot.Value;
        var options = context.Options;

        // Uses ApiIdJsonConverter via [JsonConverter] on ApiId
        writer.TryWritePropertyWithSerializer(propertyName, snapshot.ScalarValue, options);
    }

    private static void WriteSnapshotParts(Utf8JsonWriter writer, ApiIdentitySnapshot snapshot, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.Snapshot.Parts;
        var options = context.Options;

        var parts = snapshot.GetPartsBlueprintUnsafe();

        writer.TryWritePropertyWithAction
        (
            propertyName,
            parts,
            options,
            collection => WriteJsonArray(writer, collection, entry => WritePartEntryObject(writer, entry, context))
        );
    }

    private static void WritePartEntryObject(Utf8JsonWriter writer, ApiIdentityPartEntry entry, DefaultWriteContext<PropertyNames> context)
    {
        WriteJsonObject(writer, () =>
        {
            WritePartEntryName(writer, entry, context);
            WritePartEntryKind(writer, entry, context);

            if (entry.Part.Kind == ApiIdentityPartKind.Scalar)
            {
                WritePartEntryIsUnresolved(writer, entry, context);
            }

            WritePartEntryNestedBlueprint(writer, entry, context);
            WritePartEntryValue(writer, entry, context);
        });
    }

    private static void WritePartEntryName(Utf8JsonWriter writer, ApiIdentityPartEntry entry, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.PartEntry.Name;
        var options = context.Options;

        writer.TryWritePropertyAsString(propertyName, entry.Name, options);
    }

    private static void WritePartEntryKind(Utf8JsonWriter writer, ApiIdentityPartEntry entry, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.PartEntry.Kind;
        var options = context.Options;

        writer.TryWritePropertyWithConverter(propertyName, entry.Part.Kind, options, _partKindJsonConverter);
    }

    private static void WritePartEntryIsUnresolved(Utf8JsonWriter writer, ApiIdentityPartEntry entry, DefaultWriteContext<PropertyNames> context)
    {
        var isUnresolved = entry.Part.Kind == ApiIdentityPartKind.Scalar && !entry.Part.ScalarValue.HasValue;
        if (!isUnresolved)
        {
            return;
        }

        var propertyName = context.PropertyNames.PartEntry.IsUnresolved;
        var options = context.Options;

        writer.TryWritePropertyAsBoolean(propertyName, true, options);
    }

    private static void WritePartEntryNestedBlueprint(Utf8JsonWriter writer, ApiIdentityPartEntry entry, DefaultWriteContext<PropertyNames> context)
    {
        // Only meaningful for nested/unresolved nested; for scalar keep payload lean.
        if (entry.Part.Kind == ApiIdentityPartKind.Scalar)
        {
            return;
        }

        var propertyName = context.PropertyNames.PartEntry.NestedBlueprint;
        var options = context.Options;
        var nestedBlueprint = entry.NestedBlueprint ?? Array.Empty<ApiIdentityPartEntry>();

        writer.TryWritePropertyWithAction
        (
            propertyName,
            nestedBlueprint,
            options,
            collection => WriteJsonArray(writer, collection, child => WritePartEntryObject(writer, child, context))
        );
    }

    private static void WritePartEntryValue(Utf8JsonWriter writer, ApiIdentityPartEntry entry, DefaultWriteContext<PropertyNames> context)
    {
        var propertyName = context.PropertyNames.PartEntry.Value;
        var options = context.Options;

        switch (entry.Part.Kind)
        {
            case ApiIdentityPartKind.Scalar:
                {
                    if (!entry.Part.ScalarValue.HasValue)
                    {
                        // Unresolved scalar
                        writer.TryWritePropertyWithSerializer(propertyName, (ApiId?)null, options);
                        return;
                    }

                    var apiId = entry.Part.ScalarValue.Value;
                    writer.TryWritePropertyWithSerializer(propertyName, apiId, options);
                    return;
                }

            case ApiIdentityPartKind.Nested:
                {
                    var nested = entry.Part.NestedSnapshot;
                    writer.TryWritePropertyWithSerializer(propertyName, nested, options);
                    return;
                }

            case ApiIdentityPartKind.UnresolvedNested:
                writer.TryWritePropertyWithSerializer(propertyName, (ApiIdentitySnapshot?)null, options);
                return;

            default:
                throw new JsonException($"Unknown ApiIdentityPart.Kind: {entry.Part.Kind}.");
        }
    }
    #endregion

    #region Create Helpers
    private static ApiIdentityPartEntry CreatePartEntry(DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context, ApiIdentityPartEntryReadData readData, int partIndex)
    {
        var propertyNames = context.PropertyNames;
        var options = context.Options;

        var name = readData.Name;
        if (string.IsNullOrWhiteSpace(name))
        {
            var propertyName = propertyNames.PartEntry.Name;
            throw new JsonException($"Missing required property: {propertyName} for part entry at index {partIndex}.");
        }

        var nullableKind = readData.Kind;
        if (nullableKind is null)
        {
            var propertyName = propertyNames.PartEntry.Kind;
            throw new JsonException($"Missing required property: {propertyName} for part entry '{name}'.");
        }

        var kind = nullableKind.Value;
        var isUnresolvedScalar = readData.IsUnresolved == true;

        var nestedBlueprintReadData = readData.NestedBlueprint;
        var nestedBlueprint = nestedBlueprintReadData is null
            ? Array.Empty<ApiIdentityPartEntry>()
            : CreateBlueprintEntries(context, nestedBlueprintReadData, ownerPartName: name);

        var valueElement = readData.ValueElement;

        ApiIdentityPart part;
        switch (kind)
        {
            case ApiIdentityPartKind.Scalar:
                {
                    if (isUnresolvedScalar || valueElement is null || valueElement.Value.ValueKind == JsonValueKind.Null)
                    {
                        part = new ApiIdentityPart { Kind = ApiIdentityPartKind.Scalar, ScalarValue = null, NestedSnapshot = null };
                        return new ApiIdentityPartEntry(name, part, Array.Empty<ApiIdentityPartEntry>());
                    }

                    // Defensive fallback: if the value payload looks like a nested ApiIdentitySnapshot (it has a "Name" property),
                    // treat it as nested rather than trying to parse it as an ApiId.
                    // This avoids cascading failures where a nested snapshot (Kind="Composite") is mis-read as ApiIdKind.Composite.
                    if (valueElement.Value.ValueKind == JsonValueKind.Object
                        && valueElement.Value.TryGetProperty(propertyNames.Snapshot.Name, out _))
                    {
                        var nested = valueElement.Value.Deserialize<ApiIdentitySnapshot>(options);
                        if (nested is null)
                        {
                            var valuePropertyName = propertyNames.PartEntry.Value;
                            throw new JsonException($"Nested part '{name}' could not be deserialized from property: {valuePropertyName}.");
                        }

                        part = ApiIdentityPart.Nested(nested);

                        if (nestedBlueprint.Length == 0)
                        {
                            nestedBlueprint = [.. nested.GetPartsBlueprintUnsafe()];
                        }

                        return new ApiIdentityPartEntry(name, part, nestedBlueprint);
                    }

                    var apiId = valueElement.Value.Deserialize<ApiId>(options);
                    part = ApiIdentityPart.Scalar(apiId);
                    return new ApiIdentityPartEntry(name, part, Array.Empty<ApiIdentityPartEntry>());
                }

            case ApiIdentityPartKind.Nested:
                {
                    if (valueElement is null || valueElement.Value.ValueKind == JsonValueKind.Null)
                    {
                        var valuePropertyName = propertyNames.PartEntry.Value;
                        throw new JsonException($"Nested part '{name}' requires non-null property: {valuePropertyName}.");
                    }

                    var nested = valueElement.Value.Deserialize<ApiIdentitySnapshot>(options);
                    if (nested is null)
                    {
                        var valuePropertyName = propertyNames.PartEntry.Value;
                        throw new JsonException($"Nested part '{name}' could not be deserialized from property: {valuePropertyName}.");
                    }

                    part = ApiIdentityPart.Nested(nested);

                    // NestedBlueprint is required by ApiIdentitySnapshot constructor. If it wasn't supplied, use the nested snapshot's blueprint.
                    if (nestedBlueprint.Length == 0)
                    {
                        nestedBlueprint = [.. nested.GetPartsBlueprintUnsafe()];
                    }

                    return new ApiIdentityPartEntry(name, part, nestedBlueprint);
                }

            case ApiIdentityPartKind.UnresolvedNested:
                {
                    part = ApiIdentityPart.UnresolvedNested();

                    if (nestedBlueprint.Length == 0)
                    {
                        var propertyName = propertyNames.PartEntry.NestedBlueprint;
                        throw new JsonException($"Unresolved nested part '{name}' requires non-empty property: {propertyName}.");
                    }

                    return new ApiIdentityPartEntry(name, part, nestedBlueprint);
                }

            default:
                throw new JsonException($"Unknown part kind: {kind} for entry '{name}'.");
        }
    }

    private static ApiIdentityPartEntry[] CreateBlueprintEntries(
        DefaultReadContext<PropertyNames, ReadData, ReadHandlers> context,
        List<ApiIdentityPartEntryReadData?> readData,
        string ownerPartName)
    {
        var propertyNames = context.PropertyNames;

        var result = new ApiIdentityPartEntry[readData.Count];
        for (var i = 0; i < readData.Count; i++)
        {
            var entryReadData = readData[i];
            if (entryReadData is null)
            {
                var propertyName = propertyNames.PartEntry.NestedBlueprint;
                throw new JsonException($"Null nested blueprint entry at index {i} for part '{ownerPartName}' in property: {propertyName}.");
            }

            result[i] = CreatePartEntry(context, entryReadData, partIndex: i);
        }

        return result;
    }

    private static string? GetParentPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var lastDot = path.LastIndexOf('.');
        return lastDot < 0
            ? null
            : path[..lastDot];
    }
    #endregion
}
