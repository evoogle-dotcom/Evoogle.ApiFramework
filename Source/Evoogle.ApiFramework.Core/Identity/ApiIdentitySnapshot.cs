// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Evoogle.ApiFramework.Exceptions;

namespace Evoogle.ApiFramework.Identity;

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
///     new Dictionary&lt;string, object?&gt;
///     {
///         ["Customer"] = customerSnapshot,
///         ["OrderNumber"] = 1001L
///     }
/// );
/// </code>
/// </example>
public sealed class ApiIdentitySnapshot
{
    #region Fields
    private readonly FrozenDictionary<string, ApiIdentitySnapshot?> _nestedParts;
    private readonly FrozenDictionary<string, ApiId> _scalarParts;
    private ApiId? _cachedNamedApiId;
    private ApiId? _cachedUnnamedApiId;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new instance of <see cref="ApiIdentitySnapshot"/>.
    /// </summary>
    /// <param name="name">The name of this identity part.</param>
    /// <param name="parts">The resolved parts (name -> value mappings).</param>
    /// <param name="parentPath">Optional parent path for nested values.</param>
    public ApiIdentitySnapshot(
        string name,
        IReadOnlyDictionary<string, object?> parts,
        string? parentPath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(parts);

        this.Name = name;
        this.Path = string.IsNullOrWhiteSpace(parentPath)
            ? name
            : $"{parentPath}.{name}";

        // Separate nested identities from scalar values
        var nestedParts = new Dictionary<string, ApiIdentitySnapshot?>();
        var scalarParts = new Dictionary<string, ApiId>();

        foreach (var kvp in parts)
        {
            if (kvp.Value is ApiIdentitySnapshot nested)
            {
                nestedParts[kvp.Key] = nested;
            }
            else if (kvp.Value is ApiId apiId)
            {
                // Already an ApiId - store directly (zero allocation)
                scalarParts[kvp.Key] = apiId;
            }
            else
            {
                // Convert object to ApiId (preserves original type)
                var value = kvp.Value;
                scalarParts[kvp.Key] = value != null ? ApiId.FromObject(value) : ApiId.Empty;
            }
        }

        _nestedParts = nestedParts.ToFrozenDictionary();
        _scalarParts = scalarParts.ToFrozenDictionary();
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
        _scalarParts = new Dictionary<string, ApiId> { ["Value"] = scalarValue }
            .ToFrozenDictionary();
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
    ///     Use this property to validate the snapshot before flattening to <see cref="ApiId"/>
    ///     or performing operations that require complete identity data.
    /// </remarks>
    public bool IsFullyResolved
    {
        get
        {
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
    public IEnumerable<string> PartNames => _nestedParts.Keys.Concat(_scalarParts.Keys);

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
    /// <param name="parts">The resolved parts.</param>
    /// <param name="parentPath">Optional parent path.</param>
    /// <returns>A new composite <see cref="ApiIdentitySnapshot"/>.</returns>
    public static ApiIdentitySnapshot Composite(
        string name,
        IReadOnlyDictionary<string, object?> parts,
        string? parentPath = null)
    {
        return new ApiIdentitySnapshot(name, parts, parentPath);
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
            new Dictionary<string, object?>(),
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
    /// <returns>An enumerable of paths to unresolved parts.</returns>
    public IEnumerable<string> GetUnresolvedParts()
    {
        foreach (var kvp in _nestedParts)
        {
            if (kvp.Value is null)
            {
                yield return $"{this.Path}.{kvp.Key}";
            }
            else
            {
                foreach (var unresolved in kvp.Value.GetUnresolvedParts())
                {
                    yield return unresolved;
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
    /// <returns>A flat <see cref="ApiId"/> suitable for runtime operations.</returns>
    /// <exception cref="ApiIdentityException">Thrown if any nested parts are unresolved (null).</exception>
    /// <remarks>
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
    public ApiId ToApiId(bool useNamedParts = true)
    {
        // Check cache based on naming mode
        if (useNamedParts)
        {
            if (_cachedNamedApiId.HasValue)
            {
                return _cachedNamedApiId.Value;
            }
        }
        else
        {
            if (_cachedUnnamedApiId.HasValue)
            {
                return _cachedUnnamedApiId.Value;
            }
        }

        var flatParts = new List<ApiIdPart>();
        FlattenRecursive(this, prefix: null, flatParts, useNamedParts);

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

        // Cache result
        if (useNamedParts)
        {
            _cachedNamedApiId = result;
        }
        else
        {
            _cachedUnnamedApiId = result;
        }

        return result;
    }

    /// <summary>
    ///     Returns a debug-friendly string representation showing the nested structure.
    /// </summary>
    /// <returns>A hierarchical string representation of this snapshot.</returns>
    public string ToDebugString()
    {
        var sb = new StringBuilder();
        this.AppendDebugString(sb, indent: 0);
        return sb.ToString();
    }
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString() => this.ToDebugString();
    #endregion

    #region Implementation Methods
    private static void FlattenRecursive(
        ApiIdentitySnapshot snapshot,
        string? prefix,
        List<ApiIdPart> output,
        bool useNamedParts)
    {
        if (snapshot.IsScalar)
        {
            // Leaf node: add scalar part (ApiId is already the right type!)
            var partName = useNamedParts ? prefix : null;
            var apiId = snapshot.ScalarValue;

            output.Add(ApiIdPart.Create(partName, apiId));
            return;
        }

        // Composite: recurse into parts
        foreach (var kvp in snapshot._nestedParts)
        {
            var partName = kvp.Key;
            var fullName = useNamedParts
                ? (string.IsNullOrWhiteSpace(prefix) ? partName : $"{prefix}.{partName}")
                : null;

            if (kvp.Value is not null)
            {
                FlattenRecursive(kvp.Value, fullName, output, useNamedParts);
            }
            else
            {
                var unresolvedPath = string.IsNullOrWhiteSpace(prefix) ? partName : $"{prefix}.{partName}";
                throw new ApiIdentityException(
                    $"Cannot flatten identity snapshot with unresolved nested part at '{unresolvedPath}'. " +
                    $"Check IsFullyResolved or use TryGetPart() before flattening."
                );
            }
        }

        foreach (var kvp in snapshot._scalarParts)
        {
            var partName = kvp.Key;
            var fullName = useNamedParts
                ? (string.IsNullOrWhiteSpace(prefix) ? partName : $"{prefix}.{partName}")
                : null;

            var apiId = kvp.Value;
            output.Add(ApiIdPart.Create(fullName, apiId));
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

        foreach (var kvp in _nestedParts)
        {
            sb.Append(indentStr);
            sb.Append("  ");
            sb.Append(kvp.Key);
            sb.Append(" = ");

            if (kvp.Value is null)
            {
                sb.AppendLine("null (unresolved)");
            }
            else
            {
                sb.AppendLine();
                kvp.Value.AppendDebugString(sb, indent + 2);
            }
        }

        foreach (var kvp in _scalarParts)
        {
            sb.Append(indentStr);
            sb.Append("  ");
            sb.Append(kvp.Key);
            sb.Append(" = ");
            sb.AppendLine(kvp.Value.ToString());
        }

        sb.Append(indentStr);
        sb.Append('}');
    }
    #endregion
}

