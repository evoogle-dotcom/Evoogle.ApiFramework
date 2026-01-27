// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Frozen;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity.Json;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Specifies how to handle unresolved parts when flattening an identity snapshot.
/// </summary>
public enum UnresolvedPartBehavior
{
    /// <summary>
    ///     Throw an <see cref="ApiIdentityException"/> when encountering an unresolved part.
    /// </summary>
    Throw,

    /// <summary>
    ///     Use <see cref="ApiId.Empty"/> for unresolved scalar parts.
    ///     For unresolved nested parts, continue flattening using <see cref="ApiId.Empty"/> for all descendant scalar
    ///     leaves according to the schema-provided blueprint.
    /// </summary>
    UseEmpty
}

/// <summary>
///     Represents a captured snapshot of an object's identity structure.
///     Preserves semantic relationships and nested hierarchy for navigation, introspection, and validation.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiIdentitySnapshot"/> is the bridge between identity schema and runtime value (<see cref="ApiId"/>).
///         It maintains the nested object graph during resolution, enabling path-based navigation and diagnostics
///         before flattening to <see cref="ApiId"/> for performance-critical operations.
///     </para>
///     <para>
///         <strong>Architecture:</strong>
///         <see cref="ApiIdentitySnapshot"/> serves as the semantic layer in the identity system:
///     </para>
///     <list type="bullet">
///         <item>
///             <strong>Schema Layer:</strong> <see cref="Schema.ApiIdentity"/> defines what constitutes an identity (build-time).
///         </item>
///         <item>
///             <strong>Semantic Layer:</strong> <see cref="ApiIdentitySnapshot"/> captures the resolved identity structure
///             with full path context, enabling navigation and introspection (extraction/resolution time).
///         </item>
///         <item>
///             <strong>Runtime Layer:</strong> <see cref="ApiId"/> provides a flat, performance-optimized representation
///             for dictionary keys, caching, and equality operations (hot-path runtime).
///         </item>
///     </list>
///     <para>
///         <strong>Key Features:</strong>
///     </para>
///     <list type="bullet">
///         <item><strong>Structured Navigation:</strong> Access nested parts via indexer or dot-notation paths.</item>
///         <item><strong>Type Preservation:</strong> Stores <see cref="ApiId"/> values directly—no boxing or string conversions.</item>
///         <item><strong>Dual Flattening:</strong> Convert to named (semantic) or unnamed (compact) <see cref="ApiId"/> formats.</item>
///         <item><strong>Resolution Tracking:</strong> Detect unresolved (null) nested identities before use.</item>
///         <item><strong>Immutable:</strong> Thread-safe and cacheable.</item>
///     </list>
///     <para>
///         This type is immutable and thread-safe.
///     </para>
/// </remarks>
/// <example>
/// <para><strong>Example 1: Extract and Navigate Identity Snapshot</strong></para>
/// <code>
/// var order = new Order
/// {
///     Customer = new Customer { Country = new Country { Id = 1 }, CustomerId = 42 },
///     OrderNumber = 1001L
/// };
///
/// // Extract structured snapshot
/// ApiIdentitySnapshot snapshot = orderIdentity.Extract(order);
///
/// // Navigate semantically
/// int customerId = snapshot.GetScalarValue&lt;int&gt;("Customer.CustomerId");
/// var customerSnapshot = snapshot["Customer"];
///
/// // Check resolution status
/// if (!snapshot.IsFullyResolved)
/// {
///     var missing = snapshot.GetUnresolvedParts();
///     throw new ApiIdentityException($"Unresolved: {string.Join(", ", missing)}");
/// }
/// </code>
///
/// <para><strong>Example 2: Flatten to ApiId (Client-Controlled Naming)</strong></para>
/// <code>
/// var snapshot = orderIdentity.Extract(order);
///
/// // Named (semantic) - for logging/debugging
/// ApiId namedId = snapshot.ToApiId(useNamedParts: true);
/// logger.LogInfo("Processing order {OrderId}", namedId);
/// // Output: "Customer.Country.Id=1|Customer.CustomerId=42|OrderNumber=1001"
///
/// // Unnamed (compact) - for cache keys
/// ApiId compactId = snapshot.ToApiId(useNamedParts: false);
/// cache[compactId] = order;
/// // Key: "1|42|1001"
/// </code>
///
/// <para><strong>Example 3: Manual Construction</strong></para>
/// <code>
/// // Scalar snapshot
/// var productSnapshot = ApiIdentitySnapshot.Scalar("ProductId", 99);
///
/// // Composite snapshot
/// var orderSnapshot = ApiIdentitySnapshot.Composite(
///     "Order",
///     new ApiIdentityPartEntry[]
///     {
///         new("Customer", ApiIdentityPart.Nested(customerSnapshot), nestedBlueprint: Array.Empty&lt;ApiIdentityPartEntry&gt;()),
///         new("OrderNumber", ApiIdentityPart.Scalar(ApiId.FromInt64(1001L)), nestedBlueprint: Array.Empty&lt;ApiIdentityPartEntry&gt;())
///     }
/// );
/// </code>
/// </example>
[DebuggerDisplay("{ToDebuggerDisplay(),nq}")]
[JsonConverter(typeof(ApiIdentitySnapshotJsonConverter))]
public sealed class ApiIdentitySnapshot
{
    #region Fields
    private readonly FrozenDictionary<string, ApiIdentitySnapshot?> _nestedParts;
    private readonly FrozenDictionary<string, ApiId> _scalarParts;
    private readonly FrozenSet<string> _unresolvedScalarParts;
    private readonly ApiIdentityPartEntry[] _partsBlueprint;

    // Thread-safe caches (boxed ApiId). Cached only for unresolvedBehavior == Throw.
    private object? _cachedNamedApiIdBox;
    private object? _cachedUnnamedApiIdBox;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of <see cref="ApiIdentitySnapshot"/>.
    /// </summary>
    /// <param name="name">The name of this identity part.</param>
    /// <param name="partsBlueprint">The ordered parts blueprint (interleaved nested/scalar exactly as desired).</param>
    /// <param name="parentPath">Optional parent path for nested values.</param>
    /// <remarks>
    ///     <para>
    ///         <strong>Part Value Handling:</strong>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <strong>Scalar parts:</strong> <see cref="ApiIdentityPart.Scalar"/> values are stored directly
    ///             in the scalar parts dictionary for efficient access.
    ///         </item>
    ///         <item>
    ///             <strong>Nested parts:</strong> <see cref="ApiIdentityPart.Nested"/> values are stored in the
    ///             nested parts dictionary, preserving the hierarchy.
    ///         </item>
    ///         <item>
    ///             <strong>Unresolved nested parts:</strong> <see cref="ApiIdentityPart.UnresolvedNested"/> values
    ///             are stored as null in the nested parts dictionary and tracked separately.
    ///         </item>
    ///     </list>
    /// </remarks>
    public ApiIdentitySnapshot(
        string name,
        IReadOnlyList<ApiIdentityPartEntry> partsBlueprint,
        string? parentPath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(partsBlueprint);

        this.Name = name;
        this.Path = string.IsNullOrWhiteSpace(parentPath)
            ? name
            : $"{parentPath}.{name}";

        _partsBlueprint = partsBlueprint.Count == 0
            ? []
            : [.. partsBlueprint];

        // Separate nested identities from scalar values, preserving blueprint ordering separately.
        var nestedParts = new Dictionary<string, ApiIdentitySnapshot?>(StringComparer.Ordinal);
        var scalarParts = new Dictionary<string, ApiId>(StringComparer.Ordinal);
        var unresolvedScalarParts = new HashSet<string>(StringComparer.Ordinal);
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var entry in _partsBlueprint)
        {
            if (string.IsNullOrWhiteSpace(entry.Name))
            {
                throw new ApiIdentityException(
                    $"Identity snapshot '{this.Path}' contains a part with null/empty name in the blueprint."
                );
            }

            if (!seen.Add(entry.Name))
            {
                throw new ApiIdentityException(
                    $"Identity snapshot '{this.Path}' contains duplicate part name '{entry.Name}'. " +
                    "Part names must be unique within a snapshot node."
                );
            }

            var part = entry.Part;

            switch (part.Kind)
            {
                case ApiIdentityPartKind.Scalar:
                    if (!part.ScalarValue.HasValue)
                    {
                        // Allow unresolved scalar to support deterministic flattening.
                        unresolvedScalarParts.Add(entry.Name);
                        scalarParts[entry.Name] = ApiId.Empty;
                        break;
                    }

                    scalarParts[entry.Name] = part.ScalarValue.Value;
                    break;

                case ApiIdentityPartKind.Nested:
                    if (part.NestedSnapshot == null)
                    {
                        throw new ApiIdentityException(
                            $"Part '{entry.Name}' is marked as Nested but has null NestedSnapshot. " +
                            "Use ApiIdentityPart.UnresolvedNested() for unresolved nested identities."
                        );
                    }
                    if (entry.NestedBlueprint is null)
                    {
                        throw new ApiIdentityException(
                            $"Nested part '{entry.Name}' at '{this.Path}' must define a NestedBlueprint " +
                            "so unresolved nested parts can be flattened deterministically."
                        );
                    }
                    nestedParts[entry.Name] = part.NestedSnapshot;
                    break;

                case ApiIdentityPartKind.UnresolvedNested:
                    if (entry.NestedBlueprint is null)
                    {
                        throw new ApiIdentityException(
                            $"Unresolved nested part '{entry.Name}' at '{this.Path}' must define a NestedBlueprint " +
                            "so it can be flattened deterministically."
                        );
                    }
                    nestedParts[entry.Name] = null;
                    break;

                default:
                    throw new ApiIdentityException(
                        $"Unknown ApiIdentityPart.Kind: {part.Kind}"
                    );
            }
        }

        _nestedParts = nestedParts.ToFrozenDictionary(StringComparer.Ordinal);
        _scalarParts = scalarParts.ToFrozenDictionary(StringComparer.Ordinal);
        _unresolvedScalarParts = unresolvedScalarParts.ToFrozenSet(StringComparer.Ordinal);
    }

    /// <summary>
    ///     Internal constructor for wrapping a single scalar ApiId.
    /// </summary>
    private ApiIdentitySnapshot(string name, ApiId scalarValue, string? parentPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        this.Name = name;
        this.Path = string.IsNullOrWhiteSpace(parentPath)
            ? name
            : $"{parentPath}.{name}";

        _nestedParts = FrozenDictionary<string, ApiIdentitySnapshot?>.Empty;
        _scalarParts = new Dictionary<string, ApiId>(StringComparer.Ordinal) { ["Value"] = scalarValue }
            .ToFrozenDictionary(StringComparer.Ordinal);
        _unresolvedScalarParts = FrozenSet<string>.Empty;
        _partsBlueprint = [];
    }
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the name of this identity part.
    /// </summary>
    /// <value>
    ///     The name of this snapshot node (e.g., "Customer", "Order", "Id").
    /// </value>
    public string Name { get; }

    /// <summary>
    ///     Gets the full path to this identity snapshot in the object graph.
    /// </summary>
    /// <value>
    ///     The dot-separated path from the root identity to this node.
    /// </value>
    /// <example>"Order.Customer.Id"</example>
    public string Path { get; }

    /// <summary>
    ///     Gets whether this identity snapshot is a leaf (scalar) node.
    /// </summary>
    /// <value>
    ///     <see langword="true"/> if this snapshot contains a single scalar value;
    ///     otherwise, <see langword="false"/> (composite or empty).
    /// </value>
    public bool IsScalar => _nestedParts.Count == 0 && _scalarParts.Count == 1 && _scalarParts.ContainsKey("Value");

    /// <summary>
    ///     Gets whether this identity is a composite (has multiple parts).
    /// </summary>
    /// <value>
    ///     <see langword="true"/> if this snapshot has more than one part (nested or scalar);
    ///     otherwise, <see langword="false"/>.
    /// </value>
    public bool IsComposite => _nestedParts.Count + _scalarParts.Count > 1;

    /// <summary>
    ///     Gets the scalar ApiId value if this is a leaf node.
    /// </summary>
    /// <value>
    ///     The <see cref="ApiId"/> stored in this scalar snapshot.
    /// </value>
    /// <exception cref="ApiIdentityException">Thrown if this is not a scalar snapshot.</exception>
    public ApiId ScalarValue
    {
        get
        {
            if (!this.IsScalar)
            {
                throw new ApiIdentityException(
                    $"Cannot access ScalarValue on composite identity snapshot at path '{this.Path}'. " +
                    $"This snapshot has {this.PartCount} parts: [{string.Join(", ", this.PartNames)}]."
                );
            }
            return _scalarParts["Value"];
        }
    }

    /// <summary>
    ///     Gets whether all identity parts were successfully resolved (no null nested snapshots).
    /// </summary>
    /// <value>
    ///     <see langword="true"/> if all nested parts are non-null and fully resolved;
    ///     otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    ///     <para>
    ///         <strong>What is "Resolution"?</strong>
    ///     </para>
    ///     <para>
    ///         Resolution refers to whether all nested object references in the identity have been successfully
    ///         navigated and extracted. During identity extraction (<c>ApiIdentity.Extract(obj)</c>), the system
    ///         walks through the object graph following navigation properties. If any navigation property returns
    ///         <see langword="null"/>, that part becomes "unresolved" because its nested values cannot be accessed.
    ///     </para>
    ///     <para>
    ///         <strong>Fully Resolved:</strong> All navigation properties exist and their values were extracted.
    ///         The snapshot contains complete identity data and can be safely flattened to <see cref="ApiId"/>.
    ///     </para>
    ///     <para>
    ///         <strong>Partially Resolved:</strong> One or more navigation properties were <see langword="null"/>,
    ///         leaving gaps in the identity data. Attempting to flatten will throw <see cref="ApiIdentityException"/>.
    ///     </para>
    ///     <para>
    ///         Use this property to validate the snapshot before operations that require complete identity data,
    ///         such as flattening to <see cref="ApiId"/> or storing in a database. For partially resolved snapshots,
    ///         use <see cref="GetUnresolvedParts"/> to identify which parts are missing.
    ///     </para>
    /// </remarks>
    public bool IsFullyResolved
    {
        get
        {
            if (_unresolvedScalarParts.Count > 0)
            {
                return false;
            }

            // Check all nested parts are not null
            if (_nestedParts.Any(kvp => kvp.Value is null))
            {
                return false;
            }

            // Recursively check nested parts are fully resolved
            foreach (var nested in _nestedParts.Values)
            {
                if (nested is not null && !nested.IsFullyResolved)
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    ///     Gets the names of all parts in this identity snapshot.
    /// </summary>
    /// <value>
    ///     An enumerable of part names (both nested and scalar).
    /// </value>
    public IEnumerable<string> PartNames =>
        this.IsScalar
            ? _scalarParts.Keys
            : _partsBlueprint.Select(p => p.Name);

    /// <summary>
    ///     Gets the count of parts (nested + scalar).
    /// </summary>
    /// <value>
    ///     The total number of parts (nested snapshots + scalar values) in this snapshot.
    /// </value>
    public int PartCount => _nestedParts.Count + _scalarParts.Count;
    #endregion

    #region Indexers
    /// <summary>
    ///     Gets a part by name. Supports nested path navigation using dot notation.
    /// </summary>
    /// <param name="pathOrName">Part name or dot-separated path (e.g., "Customer" or "Customer.Id").</param>
    /// <returns>The identity snapshot at the specified path.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the path is not found.</exception>
    /// <exception cref="ApiIdentityException">Thrown if accessing an unresolved (null) part.</exception>
    public ApiIdentitySnapshot this[string pathOrName]
    {
        get
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(pathOrName);

            // Check if it's a path (contains dots)
            var segments = pathOrName.Split('.');

            if (segments.Length == 1)
            {
                // Direct part access
                if (_nestedParts.TryGetValue(pathOrName, out var nested))
                {
                    if (nested is null)
                    {
                        throw new ApiIdentityException(
                            $"Identity part '{pathOrName}' at path '{this.Path}' is null (unresolved). " +
                            $"Use TryGetPart() or check IsFullyResolved before accessing."
                        );
                    }
                    return nested;
                }

                if (_scalarParts.TryGetValue(pathOrName, out var scalarApiId))
                {
                    if (_unresolvedScalarParts.Contains(pathOrName))
                    {
                        throw new ApiIdentityException(
                            $"Identity scalar part '{pathOrName}' at path '{this.Path}' is unresolved. " +
                            $"Use TryGetPart() or check IsFullyResolved before accessing."
                        );
                    }
                    // Wrap ApiId in ApiIdentitySnapshot for consistent API
                    return new ApiIdentitySnapshot(pathOrName, scalarApiId, this.Path);
                }

                throw new KeyNotFoundException(
                    $"Identity part '{pathOrName}' not found at path '{this.Path}'. " +
                    $"Available parts: [{string.Join(", ", this.PartNames)}]"
                );
            }

            // Navigate nested path
            var current = this;
            foreach (var segment in segments)
            {
                current = current[segment];  // Recursive navigation
            }
            return current;
        }
    }
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates a scalar identity snapshot from an ApiId.
    /// </summary>
    /// <param name="name">The name of this identity part.</param>
    /// <param name="value">The scalar ApiId value.</param>
    /// <param name="parentPath">Optional parent path.</param>
    /// <returns>A new scalar <see cref="ApiIdentitySnapshot"/>.</returns>
    public static ApiIdentitySnapshot Scalar(string name, ApiId value, string? parentPath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new ApiIdentitySnapshot(name, value, parentPath);
    }

    /// <summary>
    ///     Creates a scalar identity snapshot from a CLR object (converted to ApiId).
    /// </summary>
    /// <param name="name">The name of this identity part.</param>
    /// <param name="value">The scalar value (int, long, Guid, string, etc.).</param>
    /// <param name="parentPath">Optional parent path.</param>
    /// <returns>A new scalar <see cref="ApiIdentitySnapshot"/>.</returns>
    public static ApiIdentitySnapshot Scalar(string name, object value, string? parentPath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var apiId = ApiId.FromObject(value);
        return new ApiIdentitySnapshot(name, apiId, parentPath);
    }

    /// <summary>
    ///     Creates a composite identity snapshot from parts.
    /// </summary>
    /// <param name="name">The name of this identity.</param>
    /// <param name="parts">The resolved parts as ApiIdentityPart mappings.</param>
    /// <param name="parentPath">Optional parent path.</param>
    /// <returns>A new composite <see cref="ApiIdentitySnapshot"/>.</returns>
    public static ApiIdentitySnapshot Composite(
        string name,
        IReadOnlyList<ApiIdentityPartEntry> partsBlueprint,
        string? parentPath = null)
    {
        return new ApiIdentitySnapshot(name, partsBlueprint, parentPath);
    }

    /// <summary>
    ///     Creates an empty/unresolved identity snapshot.
    /// </summary>
    /// <param name="name">The name of this identity.</param>
    /// <param name="parentPath">Optional parent path.</param>
    /// <returns>An empty <see cref="ApiIdentitySnapshot"/>.</returns>
    public static ApiIdentitySnapshot Empty(string name, string? parentPath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new ApiIdentitySnapshot(
            name,
            Array.Empty<ApiIdentityPartEntry>(),
            parentPath
        );
    }
    #endregion

    #region Methods
    /// <summary>
    ///     Attempts to get a part by name.
    /// </summary>
    /// <param name="name">The part name.</param>
    /// <param name="snapshot">The identity snapshot if found and resolved.</param>
    /// <returns>True if the part exists and is resolved; otherwise false.</returns>
    public bool TryGetPart(string name, [NotNullWhen(true)] out ApiIdentitySnapshot? snapshot)
    {
        if (_nestedParts.TryGetValue(name, out snapshot))
        {
            return snapshot is not null;
        }

        if (_scalarParts.TryGetValue(name, out var scalarApiId))
        {
            if (_unresolvedScalarParts.Contains(name))
            {
                snapshot = null;
                return false;
            }
            snapshot = new ApiIdentitySnapshot(name, scalarApiId, this.Path);
            return true;
        }

        snapshot = null;
        return false;
    }

    /// <summary>
    ///     Gets the raw ApiId for a scalar part directly by name or path.
    /// </summary>
    /// <param name="pathOrName">Part name or path.</param>
    /// <returns>The ApiId value.</returns>
    /// <exception cref="ApiIdentityException">If the part is not scalar.</exception>
    public ApiId GetScalarApiId(string pathOrName)
    {
        var part = this[pathOrName];

        if (!part.IsScalar)
        {
            throw new ApiIdentityException(
                $"Identity part at '{pathOrName}' (full path: '{part.Path}') is not a scalar snapshot. " +
                $"It has {part.PartCount} parts: [{string.Join(", ", part.PartNames)}]."
            );
        }

        return part.ScalarValue;
    }

    /// <summary>
    ///     Gets a strongly-typed scalar value by name or path.
    /// </summary>
    /// <typeparam name="T">The expected scalar type.</typeparam>
    /// <param name="pathOrName">Part name or path.</param>
    /// <returns>The typed scalar value.</returns>
    /// <exception cref="ApiIdentityException">If the part is not scalar or type doesn't match.</exception>
    public T GetScalarValue<T>(string pathOrName)
    {
        var apiId = this.GetScalarApiId(pathOrName);

        // Use ApiId's type-safe extraction (throws ApiIdentityException on mismatch)
        if (typeof(T) == typeof(string))
        {
            return (T)(object)apiId.AsStringOrThrow();
        }
        if (typeof(T) == typeof(int))
        {
            return (T)(object)apiId.AsInt32OrThrow();
        }
        if (typeof(T) == typeof(long))
        {
            return (T)(object)apiId.AsInt64OrThrow();
        }
        if (typeof(T) == typeof(Guid))
        {
            return (T)(object)apiId.AsGuidOrThrow();
        }
        if (typeof(T) == typeof(Ulid))
        {
            return (T)(object)apiId.AsUlidOrThrow();
        }
        if (typeof(T) == typeof(System.Globalization.CultureInfo))
        {
            return (T)(object)apiId.AsCultureOrThrow();
        }

        throw new ApiIdentityException(
            $"Cannot convert ApiId of kind '{apiId.Kind}' at path '{pathOrName}' to type '{typeof(T).Name}'. " +
            $"Supported types: string, int, long, Guid, Ulid, CultureInfo."
        );
    }

    /// <summary>
    ///     Gets the paths of all unresolved (null) nested identity parts.
    /// </summary>
    /// <returns>
    ///     An enumerable of dot-separated paths identifying which nested parts are <see langword="null"/>.
    ///     Empty if the snapshot is fully resolved.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         An "unresolved part" occurs when a navigation property in the object graph was <see langword="null"/>
    ///         during identity extraction, preventing the extraction of that object's identity values.
    ///     </para>
    ///     <para>
    ///         <strong>Common Scenarios:</strong>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <strong>Lazy Loading:</strong> Related entities not yet loaded from database.
    ///             Example: <c>order.Customer == null</c> when Customer wasn't eager-loaded.
    ///         </item>
    ///         <item>
    ///             <strong>Optional Data:</strong> Client didn't provide optional nested objects in API request.
    ///             Example: Order submitted without shipment address.
    ///         </item>
    ///         <item>
    ///             <strong>Partial Construction:</strong> Object being built incrementally and not all parts set yet.
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Use this method to generate helpful error messages or to determine which related data needs to be loaded
    ///         before the identity can be used.
    ///     </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var snapshot = orderIdentity.Extract(order);
    /// if (!snapshot.IsFullyResolved)
    /// {
    ///     var missing = snapshot.GetUnresolvedParts().ToArray();
    ///     // Returns: ["Order.Customer", "Order.Shipment.Address"]
    ///     throw new InvalidOperationException(
    ///         $"Cannot process order: missing {string.Join(", ", missing)}"
    ///     );
    /// }
    /// </code>
    /// </example>
    public IEnumerable<string> GetUnresolvedParts()
    {
        if (this.IsScalar)
        {
            yield break;
        }

        foreach (var entry in _partsBlueprint)
        {
            if (entry.Part.Kind == ApiIdentityPartKind.Scalar)
            {
                if (_unresolvedScalarParts.Contains(entry.Name))
                {
                    yield return $"{this.Path}.{entry.Name}";
                }

                continue;
            }

            if (entry.Part.Kind is ApiIdentityPartKind.Nested or ApiIdentityPartKind.UnresolvedNested)
            {
                if (_nestedParts.TryGetValue(entry.Name, out var nested))
                {
                    if (nested is null)
                    {
                        yield return $"{this.Path}.{entry.Name}";
                    }
                    else
                    {
                        foreach (var unresolved in nested.GetUnresolvedParts())
                        {
                            yield return unresolved;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Flattens this structured identity snapshot to an <see cref="ApiId"/> for runtime operations.
    ///     The result is cached per naming mode.
    /// </summary>
    /// <param name="useNamedParts">
    ///     If true, flattens to named <see cref="ApiIdPart"/>s with path-based names (e.g., "Customer.Id=42").
    ///     If false, flattens to unnamed <see cref="ApiIdPart"/>s (e.g., "42|1001").
    /// </param>
    /// <param name="unresolvedBehavior">
    ///     Specifies how to handle unresolved parts. Default is <see cref="UnresolvedPartBehavior.Throw"/>.
    /// </param>
    /// <returns>A flat <see cref="ApiId"/> suitable for runtime operations.</returns>
    /// <exception cref="ApiIdentityException">
    ///     Thrown if any parts are unresolved and <paramref name="unresolvedBehavior"/>
    ///     is <see cref="UnresolvedPartBehavior.Throw"/>. Check <see cref="IsFullyResolved"/>
    ///     before calling this method, or use <see cref="GetUnresolvedParts"/> to identify missing parts.
    /// </exception>
    /// <remarks>
    ///     <para>
    ///         <strong>Resolution Requirement:</strong>
    ///         By default, this method requires a fully resolved snapshot. If any nested navigation properties were
    ///         <see langword="null"/> during extraction, flattening will fail because the complete identity cannot be constructed.
    ///         Use <see cref="IsFullyResolved"/> to validate before calling, or specify an alternative
    ///         <paramref name="unresolvedBehavior"/> to handle unresolved parts.
    ///     </para>
    ///     <para>
    ///         <strong>Named Parts (useNamedParts: true):</strong>
    ///         Produces a self-documenting <see cref="ApiId"/> with part names included.
    ///         Best for logging, debugging, and scenarios where semantic meaning is important.
    ///     </para>
    ///     <para>
    ///         <strong>Unnamed Parts (useNamedParts: false):</strong>
    ///         Produces a compact positional <see cref="ApiId"/> without part names.
    ///         Best for cache keys, database storage, and performance-critical scenarios.
    ///     </para>
    ///     <para>
    ///         Both modes are cached independently to avoid redundant computation.
    ///     </para>
    /// </remarks>
    public ApiId ToApiId(bool useNamedParts = true, UnresolvedPartBehavior unresolvedBehavior = UnresolvedPartBehavior.Throw)
    {
        // Thread-safe cache lookup (only for default behavior)
        if (unresolvedBehavior == UnresolvedPartBehavior.Throw)
        {
            var cachedBox = useNamedParts
                ? Volatile.Read(ref _cachedNamedApiIdBox)
                : Volatile.Read(ref _cachedUnnamedApiIdBox);

            if (cachedBox is ApiId cached)
            {
                return cached;
            }
        }

        var flatParts = new List<ApiIdPart>();

        // Root scalar naming: in named mode, the root scalar should use snapshot.Name.
        var initialPrefix = (useNamedParts && this.IsScalar) ? this.Name : null;
        FlattenRecursive(this, prefix: initialPrefix, flatParts, useNamedParts, unresolvedBehavior);

        ApiId result;
        if (flatParts.Count == 0)
        {
            result = ApiId.Empty;
        }
        else if (flatParts.Count == 1 && flatParts[0].Name is null)
        {
            // Single unnamed part - return as scalar
            result = flatParts[0].Value;
        }
        else
        {
            result = ApiId.Composite([.. flatParts]);
        }

        // Thread-safe cache store (only for default behavior)
        if (unresolvedBehavior == UnresolvedPartBehavior.Throw)
        {
            if (useNamedParts)
            {
                Interlocked.CompareExchange(ref _cachedNamedApiIdBox, result, null);
            }
            else
            {
                Interlocked.CompareExchange(ref _cachedUnnamedApiIdBox, result, null);
            }
        }

        return result;
    }

    /// <summary>Returns a debugger-friendly display string.</summary>
    /// <returns>A hierarchical string representation of this snapshot.</returns>
    internal string ToDebuggerDisplay()
    {
        var sb = new StringBuilder();
        this.AppendDebugString(sb, indent: 0);
        return sb.ToString();
    }
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString() => this.ToDebuggerDisplay();
    #endregion

    #region Implementation Methods
    private static void FlattenRecursive(
        ApiIdentitySnapshot snapshot,
        string? prefix,
        List<ApiIdPart> output,
        bool useNamedParts,
        UnresolvedPartBehavior unresolvedBehavior)
    {
        if (snapshot.IsScalar)
        {
            // Leaf node: add scalar part (ApiId is already the right type!)
            var partName = useNamedParts ? (prefix ?? snapshot.Name) : null;
            var apiId = snapshot.ScalarValue;

            output.Add(ApiIdPart.Create(partName, apiId));
            return;
        }

        // Composite: recurse into parts in schema-provided blueprint order.
        foreach (var entry in snapshot._partsBlueprint)
        {
            var fullName = useNamedParts
                ? (string.IsNullOrWhiteSpace(prefix) ? entry.Name : $"{prefix}.{entry.Name}")
                : null;

            switch (entry.Part.Kind)
            {
                case ApiIdentityPartKind.Scalar:
                    if (snapshot._unresolvedScalarParts.Contains(entry.Name))
                    {
                        if (unresolvedBehavior == UnresolvedPartBehavior.Throw)
                        {
                            throw new ApiIdentityException(
                                $"Cannot flatten identity snapshot with unresolved scalar part at '{snapshot.Path}.{entry.Name}'. " +
                                $"Check IsFullyResolved or use GetUnresolvedParts() before flattening."
                            );
                        }

                        output.Add(ApiIdPart.Create(fullName, ApiId.Empty));
                        break;
                    }

                    if (!snapshot._scalarParts.TryGetValue(entry.Name, out var apiId))
                    {
                        throw new ApiIdentityException(
                            $"Blueprint/reference mismatch: scalar part '{entry.Name}' not found in snapshot '{snapshot.Path}'."
                        );
                    }

                    output.Add(ApiIdPart.Create(fullName, apiId));
                    break;

                case ApiIdentityPartKind.Nested:
                case ApiIdentityPartKind.UnresolvedNested:
                    if (!snapshot._nestedParts.TryGetValue(entry.Name, out var nested))
                    {
                        throw new ApiIdentityException(
                            $"Blueprint/reference mismatch: nested part '{entry.Name}' not found in snapshot '{snapshot.Path}'."
                        );
                    }

                    if (nested is not null)
                    {
                        FlattenRecursive(nested, fullName, output, useNamedParts, unresolvedBehavior);
                        break;
                    }

                    if (unresolvedBehavior == UnresolvedPartBehavior.Throw)
                    {
                        throw new ApiIdentityException(
                            $"Cannot flatten identity snapshot with unresolved nested part at '{snapshot.Path}.{entry.Name}'. " +
                            $"Check IsFullyResolved or use GetUnresolvedParts() before flattening."
                        );
                    }

                    // UseEmpty: emit ApiId.Empty for all descendant scalar leaves according to the blueprint.
                    FlattenBlueprintAsEmpty(entry.NestedBlueprint, fullName, output, useNamedParts);
                    break;

                default:
                    throw new ApiIdentityException(
                        $"Unknown ApiIdentityPart.Kind '{entry.Part.Kind}' while flattening '{snapshot.Path}.{entry.Name}'."
                    );
            }
        }
    }

    private static void FlattenBlueprintAsEmpty(
        IReadOnlyList<ApiIdentityPartEntry> blueprint,
        string? prefix,
        List<ApiIdPart> output,
        bool useNamedParts)
    {
        foreach (var entry in blueprint)
        {
            var fullName = useNamedParts
                ? (string.IsNullOrWhiteSpace(prefix) ? entry.Name : $"{prefix}.{entry.Name}")
                : null;

            switch (entry.Part.Kind)
            {
                case ApiIdentityPartKind.Scalar:
                    output.Add(ApiIdPart.Create(fullName, ApiId.Empty));
                    break;

                case ApiIdentityPartKind.Nested:
                case ApiIdentityPartKind.UnresolvedNested:
                    FlattenBlueprintAsEmpty(entry.NestedBlueprint, fullName, output, useNamedParts);
                    break;

                default:
                    throw new ApiIdentityException(
                        $"Unknown ApiIdentityPart.Kind '{entry.Part.Kind}' while flattening empty subtree."
                    );
            }
        }
    }

    private void AppendDebugString(StringBuilder sb, int indent)
    {
        var indentStr = new string(' ', indent * 2);

        sb.Append(indentStr);
        sb.Append(this.Name);

        if (this.IsScalar)
        {
            sb.Append(" = ");
            sb.Append(this.ScalarValue.ToString());
            return;
        }

        sb.AppendLine(" {");

        foreach (var entry in _partsBlueprint)
        {
            sb.Append(indentStr);
            sb.Append("  ");
            sb.Append(entry.Name);
            sb.Append(" = ");

            switch (entry.Part.Kind)
            {
                case ApiIdentityPartKind.Scalar:
                    if (_unresolvedScalarParts.Contains(entry.Name))
                    {
                        sb.AppendLine("null (unresolved scalar)");
                    }
                    else
                    {
                        sb.AppendLine(_scalarParts[entry.Name].ToString());
                    }
                    break;

                case ApiIdentityPartKind.Nested:
                case ApiIdentityPartKind.UnresolvedNested:
                    if (_nestedParts.TryGetValue(entry.Name, out var nested) && nested is not null)
                    {
                        sb.AppendLine();
                        nested.AppendDebugString(sb, indent + 2);
                    }
                    else
                    {
                        sb.AppendLine("null (unresolved)");
                    }
                    break;

                default:
                    sb.AppendLine($"<unknown kind: {entry.Part.Kind}>");
                    break;
            }
        }

        sb.Append(indentStr);
        sb.Append('}');
    }

    internal IReadOnlyList<ApiIdentityPartEntry> GetPartsBlueprintUnsafe() => _partsBlueprint;
    #endregion
}
