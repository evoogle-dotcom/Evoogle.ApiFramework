// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Diagnostics;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Represents a captured snapshot of an object's identity structure.
///     Can be either a scalar leaf (ApiId) or a composite branch (nested parts).
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiIdentitySnapshot"/> is the bridge between identity schema and runtime value (<see cref="ApiId"/>).
///         It maintains the nested object graph during resolution, enabling path-based navigation and diagnostics
///         before flattening to <see cref="ApiId"/> for performance-critical operations.
///     </para>
///     <para>
///         <strong>Architecture:</strong>
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
///         <item><strong>Deterministic Structure:</strong> Preserve ApiId nestedPart count even when nested objects are null.</item>
///         <item><strong>Immutable:</strong> Thread-safe and cacheable.</item>
///     </list>
/// </remarks>
[DebuggerDisplay("{ToDebuggerDisplay(),nq}")]
[JsonConverter(typeof(ApiIdentitySnapshotJsonConverter))]
public sealed record ApiIdentitySnapshot
{
    #region Constants
    /// <summary>
    ///     The root path segment used for error messages when the snapshot's Path is null.
    ///     This is not used as an actual path value in the snapshot, which uses null to represent the root.
    /// </summary>
    public const string RootPath = "<root>";
    #endregion

    #region Fields
    // Thread-safe caches (no boxing). Cached only for unresolvedBehavior == Throw.
    // State: 0 = empty, 1 = writing, 2 = ready
    private int _cachedNamedApiIdState;
    private int _cachedUnnamedApiIdState;

    private ApiId _cachedNamedApiId;
    private ApiId _cachedUnnamedApiId;
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the kind of this identity snapshot (Scalar or Composite).
    /// </summary>
    public ApiIdentitySnapshotKind Kind { get; }

    /// <summary>
    ///     Gets the full dot-separated path to this identity snapshot from the root.
    ///     Null for root snapshots.
    ///     Example: "Customer.Country.Id"
    /// </summary>
    public string? Path { get; }

    /// <summary>
    ///     Gets the scalar ApiId value if Kind is Scalar; otherwise null.
    /// </summary>
    public ApiId? ScalarValue { get; }

    /// <summary>
    ///     Gets the nested parts if Kind is Composite; otherwise null.
    ///     This array is truly immutable after construction - no mutation methods exist.
    /// </summary>
    public ApiIdentityPart[]? NestedParts { get; }
    #endregion

    #region Computed Properties
    /// <summary>
    ///     Gets whether this is a composite snapshot.
    /// </summary>
    public bool IsComposite => this.Kind == ApiIdentitySnapshotKind.Composite;

    /// <summary>
    ///     Gets whether this snapshot is fully resolved (no null nested snapshots in the tree).
    /// </summary>
    public bool IsFullyResolved
    {
        get
        {
            if (this.Kind == ApiIdentitySnapshotKind.Scalar)
            {
                return true;
            }

            if (this.NestedParts is null || this.NestedParts.Length == 0)
            {
                return true;
            }

            foreach (var nestedPart in this.NestedParts)
            {
                if (nestedPart.Snapshot is null)
                {
                    return false;
                }

                if (!nestedPart.Snapshot.IsFullyResolved)
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    ///     Gets whether this is a root snapshot (Path is null).
    /// </summary>
    public bool IsRoot => this.Path is null;

    /// <summary>
    ///     Gets whether this is a scalar snapshot.
    /// </summary>
    public bool IsScalar => this.Kind == ApiIdentitySnapshotKind.Scalar;
    #endregion

    #region Constructors
    /// <summary>
    ///     Creates a scalar identity snapshot (leaf node).
    /// </summary>
    internal ApiIdentitySnapshot(string? path, ApiId? scalarValue)
    {
        this.Kind = ApiIdentitySnapshotKind.Scalar;
        this.Path = path;
        this.ScalarValue = scalarValue ?? ApiId.Empty;
        this.NestedParts = null;
    }

    /// <summary>
    ///     Creates a composite identity snapshot (branch node).
    /// </summary>
    internal ApiIdentitySnapshot(string? path, IEnumerable<ApiIdentityPart>? nestedParts = null)
    {
        this.Kind = ApiIdentitySnapshotKind.Composite;
        this.Path = path;
        this.ScalarValue = null;
        this.NestedParts = nestedParts.SafeToArray();
    }
    #endregion

    #region Factory Methods
    /// <summary>
    ///     Creates a composite identity snapshot without a path.
    ///     The path will be assigned when the snapshot is placed within a parent via Composite.
    /// </summary>
    /// <param name="nestedParts">The nested identity parts.</param>
    /// <returns>A composite identity snapshot.</returns>
    public static ApiIdentitySnapshot Composite(IEnumerable<ApiIdentityPart>? nestedParts)
    {
        return Composite(null, nestedParts);
    }

    /// <summary>
    ///    Creates a composite identity snapshot with a specified parent path.
    ///    The parent path is used to construct full paths for nested parts. It can be null for root snapshots.
    /// </summary>
    /// <param name="parentPath">The parent path for the composite snapshot.</param>
    /// <param name="nestedParts">The nested identity parts.</param>
    /// <returns>A composite identity snapshot.</returns>
    public static ApiIdentitySnapshot Composite(string? parentPath, IEnumerable<ApiIdentityPart>? nestedParts)
    {
        if (nestedParts is null)
        {
            return new(parentPath);
        }

        var partsArray = nestedParts.SafeToArray();

        // Build child snapshots with proper paths (parent path is null for root)
        var nestedPartsWithPaths = new ApiIdentityPart[partsArray.Length];
        for (var i = 0; i < partsArray.Length; i++)
        {
            var nestedPart = partsArray[i];

            nestedPartsWithPaths[i] = nestedPart.Snapshot is not null
                ? new ApiIdentityPart(nestedPart.Name, WithPath(nestedPart.Snapshot, parentPath, nestedPart.Name), nestedPart.Structure)
                : nestedPart;
        }

        return new(parentPath, nestedPartsWithPaths);
    }

    /// <summary>
    ///     Creates a composite identity snapshot from an array without a path.
    ///     The path will be assigned when the snapshot is placed within a parent via Composite.
    /// </summary>
    /// <param name="nestedParts">The nested identity parts.</param>
    /// <returns>A composite identity snapshot.</returns>
    public static ApiIdentitySnapshot Composite(params ApiIdentityPart[] nestedParts) =>
        Composite(nestedParts.AsEnumerable());

    /// <summary>
    ///    Creates a composite identity snapshot from an array with a specified parent path.
    /// </summary>
    /// <param name="parentPath">The parent path for the composite snapshot.</param>
    /// <param name="nestedParts">The nested identity parts.</param>
    /// <returns>A composite identity snapshot.</returns>
    /// <remarks>
    ///     This is primarily for testing and internal use. In typical usage, paths are assigned automatically when building composite snapshots.
    /// </remarks>
    public static ApiIdentitySnapshot Composite(string? parentPath, params ApiIdentityPart[] nestedParts) =>
        Composite(parentPath, nestedParts.AsEnumerable());

    /// <summary>
    ///     Creates a scalar identity snapshot without a path.
    ///     The path will be assigned when the snapshot is placed within a parent via Composite.
    /// </summary>
    /// <param name="scalarValue">The scalar ApiId value.</param>
    /// <returns>A scalar identity snapshot.</returns>
    public static ApiIdentitySnapshot Scalar(ApiId scalarValue)
    {
        return new(default, scalarValue);
    }

    /// <summary>
    ///    Creates a scalar identity snapshot with a specified path.
    /// </summary>
    /// <param name="path">The full dot-separated path to this snapshot (e.g., "Customer.Id").</param>
    /// <param name="scalarValue">The scalar ApiId value.</param>
    /// <returns>A scalar identity snapshot.</returns>
    /// <remarks>
    ///     This is primarily for testing and internal use. In typical usage, paths are assigned automatically when building composite snapshots.
    /// </remarks>
    public static ApiIdentitySnapshot Scalar(string? path, ApiId scalarValue)
    {
        return new(path, scalarValue);
    }

    private static ApiIdentitySnapshot WithPath(ApiIdentitySnapshot snapshot, string? parentPath, string segment)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentException.ThrowIfNullOrWhiteSpace(segment);

        var newPath = string.IsNullOrEmpty(parentPath) ? segment : $"{parentPath}.{segment}";

        if (snapshot.Kind == ApiIdentitySnapshotKind.Scalar)
        {
            return new ApiIdentitySnapshot(newPath, snapshot.ScalarValue);
        }

        // Recursively update paths for composite children
        var nestedParts = snapshot.NestedParts;
        if (nestedParts is null)
        {
            return new ApiIdentitySnapshot(newPath, []);
        }

        var updatedNestedParts = new ApiIdentityPart[nestedParts.Length];
        for (var i = 0; i < nestedParts.Length; i++)
        {
            var nestedPart = nestedParts[i];

            updatedNestedParts[i] = nestedPart.Snapshot is not null
                ? new ApiIdentityPart(nestedPart.Name, WithPath(nestedPart.Snapshot, newPath, nestedPart.Name), nestedPart.Structure)
                : nestedPart;
        }

        return new ApiIdentitySnapshot(newPath, updatedNestedParts);
    }
    #endregion

    #region Methods
    /// <summary>
    ///     Gets the scalar ApiId value. Throws if not scalar.
    /// </summary>
    /// <returns>The scalar ApiId value.</returns>
    /// <exception cref="ApiIdentityException">If this is not a scalar snapshot.</exception>
    public ApiId GetScalarValue()
    {
        if (this.Kind != ApiIdentitySnapshotKind.Scalar)
        {
            throw new ApiIdentityException($"Cannot get scalar value from composite snapshot at path '{this.Path}'.");
        }

        return this.ScalarValue!.Value;
    }

    /// <summary>
    ///     Gets the scalar value converted to type T.
    /// </summary>
    /// <typeparam name="T">The target type (must be a value type supported by ApiId).</typeparam>
    /// <returns>The scalar value as type T.</returns>
    /// <exception cref="ApiIdentityException">If this is not a scalar snapshot.</exception>
    public T GetScalarValue<T>() where T : struct
    {
        var apiId = this.GetScalarValue();

        // Use ApiId's built-in conversion logic
        if (typeof(T) == typeof(int) && apiId.TryGet(out int intVal))
        {
            return (T)(object)intVal;
        }
        if (typeof(T) == typeof(long) && apiId.TryGet(out long longVal))
        {
            return (T)(object)longVal;
        }
        if (typeof(T) == typeof(Guid) && apiId.TryGet(out Guid guidVal))
        {
            return (T)(object)guidVal;
        }
        if (typeof(T) == typeof(Ulid) && apiId.TryGet(out Ulid ulidVal))
        {
            return (T)(object)ulidVal;
        }

        throw new ApiIdentityException($"Cannot convert ApiId of kind {apiId.Kind} to type {typeof(T).Name}.");
    }

    /// <summary>
    ///     Gets all unresolved nestedPart paths in this snapshot tree.
    /// </summary>
    /// <returns>A read-only list of dot-separated paths to unresolved parts.</returns>
    public IReadOnlyList<string> GetUnresolvedParts()
    {
        var unresolved = new List<string>();
        CollectUnresolvedParts(this, unresolved);
        return unresolved;
    }

    /// <summary>
    ///     Navigates to a nested part by dot-separated path.
    /// </summary>
    /// <param name="path">
    ///     A dot-separated path (e.g., "Customer.Country.Id").
    ///     Single-segment paths like "Customer" navigate one level deep.
    /// </param>
    /// <returns>The nested identity snapshot.</returns>
    /// <exception cref="ArgumentException">If the path is invalid or contains empty segments.</exception>
    /// <exception cref="ApiIdentityException">If attempting to navigate into a scalar snapshot or the part is unresolved without structure.</exception>
    /// <exception cref="KeyNotFoundException">If the specified part is not found.</exception>
    public ApiIdentitySnapshot Navigate(string path)
    {
        var result = this.TryNavigate(path);
        if (!result)
        {
            result.ThrowIfFailed();
        }

        return result.Snapshot!;
    }

    /// <summary>
    ///     Flattens this snapshot into an ApiId.
    /// </summary>
    /// <param name="useNamedParts">
    ///     If true, includes nested part paths in the flattened ApiId (for debugging/logging).
    ///     If false, creates an unnamed compact ApiId (for cache keys).
    /// </param>
    /// <param name="unresolvedBehavior">
    ///     Specifies how to handle unresolved parts during flattening.
    /// </param>
    /// <returns>A flattened ApiId representation.</returns>
    /// <exception cref="ApiIdentityException">
    ///     If unresolvedBehavior is Throw and unresolved parts are encountered.
    /// </exception>
    public ApiId ToApiId
    (
        bool useNamedParts = false,
        ApiUnresolvedIdentityPartBehavior unresolvedBehavior = ApiUnresolvedIdentityPartBehavior.Throw
    )
    {
        // Thread-safe cache lookup (only for default behavior)
        if (unresolvedBehavior == ApiUnresolvedIdentityPartBehavior.Throw)
        {
            if (useNamedParts)
            {
                if (Volatile.Read(ref _cachedNamedApiIdState) == 2)
                {
                    return _cachedNamedApiId;
                }
            }
            else
            {
                if (Volatile.Read(ref _cachedUnnamedApiIdState) == 2)
                {
                    return _cachedUnnamedApiId;
                }
            }
        }

        var apiId = this.ToApiIdImpl(useNamedParts, unresolvedBehavior);

        // Thread-safe cache store (only for default behavior)
        if (unresolvedBehavior == ApiUnresolvedIdentityPartBehavior.Throw)
        {
            if (useNamedParts)
            {
                TryPublishCache(apiId, ref _cachedNamedApiId, ref _cachedNamedApiIdState);
            }
            else
            {
                TryPublishCache(apiId, ref _cachedUnnamedApiId, ref _cachedUnnamedApiIdState);
            }
        }

        return apiId;
    }

    private static void TryPublishCache(ApiId value, ref ApiId cache, ref int state)
    {
        // Fast path: already published.
        if (Volatile.Read(ref state) == 2)
        {
            return;
        }

        // Become the single publisher.
        if (Interlocked.CompareExchange(ref state, 1, 0) != 0)
        {
            // Someone else is publishing or it's already published.
            return;
        }

        cache = value;

        // Publish: make the value visible before state becomes 'ready'.
        Volatile.Write(ref state, 2);
    }

    /// <summary>
    ///     Returns a string representation of this snapshot.
    /// </summary>
    public override string ToString()
    {
        try
        {
            var apiId = this.ToApiId(useNamedParts: true, unresolvedBehavior: ApiUnresolvedIdentityPartBehavior.AllowUnresolved);
            return apiId.ToString() ?? this.FallbackToString();
        }
        catch
        {
            // Fallback if ToApiId fails for any reason
            return this.FallbackToString();
        }
    }

    /// <summary>
    ///     Attempts to navigate to a nested part by dot-separated path without throwing exceptions.
    /// </summary>
    /// <param name="path">
    ///     A dot-separated path (e.g., "Customer.Country.Id").
    ///     Single-segment paths like "Customer" navigate one level deep.
    /// </param>
    /// <returns>An <see cref="ApiIdentityNavigationResult"/> describing the outcome.</returns>
    public ApiIdentityNavigationResult TryNavigate(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return ApiIdentityNavigationResult.InvalidSegment(path ?? string.Empty, path ?? string.Empty);
        }

        if (this.Kind == ApiIdentitySnapshotKind.Scalar)
        {
            return ApiIdentityNavigationResult.ScalarNavigationAttempt(path, this.Path ?? RootPath);
        }

        var nestedPartsLength = this.NestedParts?.Length ?? 0;
        if (nestedPartsLength == 0)
        {
            return ApiIdentityNavigationResult.NoNestedParts(path, this.Path ?? RootPath);
        }

        var segments = path.Split('.');
        var current = this;
        var hasSynthetic = false;

        foreach (var segment in segments)
        {
            if (string.IsNullOrWhiteSpace(segment))
            {
                return ApiIdentityNavigationResult.InvalidSegment(path, segment);
            }

            ApiIdentityPart? foundPart = null;
            foreach (var nestedPart in current.NestedParts!)
            {
                if (nestedPart.Name == segment)
                {
                    foundPart = nestedPart;
                    break;
                }
            }

            if (foundPart is null)
            {
                return ApiIdentityNavigationResult.PartNotFound(path, segment, current.Path ?? RootPath);
            }

            // If snapshot exists, use it
            if (foundPart.Value.Snapshot is not null)
            {
                current = foundPart.Value.Snapshot;
            }
            // If snapshot is null but structure exists, create synthetic snapshot from structure
            else if (foundPart.Value.Structure is not null && foundPart.Value.Structure.Count > 0)
            {
                var partPath = string.IsNullOrEmpty(current.Path)
                    ? segment
                    : $"{current.Path}.{segment}";

                current = new ApiIdentitySnapshot(partPath, foundPart.Value.Structure);
                hasSynthetic = true;
            }
            else
            {
                return ApiIdentityNavigationResult.UnresolvedWithoutStructure(path, segment, current.Path ?? RootPath);
            }
        }

        return hasSynthetic
            ? ApiIdentityNavigationResult.SuccessWithSynthetic(current, path)
            : ApiIdentityNavigationResult.Success(current, path);
    }

    internal string ToDebuggerDisplay()
    {
        try
        {
            var apiId = this.ToApiId(useNamedParts: true, unresolvedBehavior: ApiUnresolvedIdentityPartBehavior.AllowUnresolved);
            return apiId.ToDebuggerDisplay();
        }
        catch
        {
            return this.FallbackToDebuggerDisplay();
        }
    }

    private static void CollectUnresolvedParts(ApiIdentitySnapshot snapshot, List<string> output)
    {
        if (snapshot.Kind == ApiIdentitySnapshotKind.Scalar || snapshot.NestedParts is null)
        {
            return;
        }

        foreach (var nestedPart in snapshot.NestedParts)
        {
            var partPath = string.IsNullOrEmpty(snapshot.Path)
                ? nestedPart.Name
                : $"{snapshot.Path}.{nestedPart.Name}";

            if (nestedPart.Snapshot is null)
            {
                output.Add(partPath);
            }
            else
            {
                CollectUnresolvedParts(nestedPart.Snapshot, output);
            }
        }
    }

    private string FallbackToDebuggerDisplay()
    {
        var displayPath = this.Path ?? RootPath;
        return this.Kind == ApiIdentitySnapshotKind.Scalar
            ? $"{displayPath} = {this.ScalarValue}"
            : $"{displayPath} [error]";
    }

    private string FallbackToString()
    {
        return this.Path ?? RootPath;
    }

    private static void FlattenRecursive
    (
        ApiIdentitySnapshot snapshot,
        List<ApiIdPart> output,
        bool useNamedParts,
        ApiUnresolvedIdentityPartBehavior unresolvedBehavior
    )
    {
        if (snapshot.Kind == ApiIdentitySnapshotKind.Scalar)
        {
            var scalarValue = snapshot.ScalarValue!.Value;
            var name = useNamedParts ? snapshot.Path : null;

            // A scalar ApiId might itself be composite, so we need to flatten those parts too
            if (scalarValue.IsComposite)
            {
                foreach (var nestedPart in scalarValue.PartsAsSpan)
                {
                    output.Add(useNamedParts
                        ? ApiIdPart.Create(name ?? nestedPart.Name, nestedPart.Value)
                        : ApiIdPart.Create(nestedPart.Value));
                }
            }
            else
            {
                output.Add(useNamedParts
                    ? ApiIdPart.Create(name, scalarValue)
                    : ApiIdPart.Create(scalarValue));
            }
            return;
        }

        if (snapshot.NestedParts is null)
        {
            return;
        }

        // Composite - recursively flatten each nestedPart
        foreach (var nestedPart in snapshot.NestedParts)
        {
            if (nestedPart.Snapshot is null)
            {
                // Handle unresolved nestedPart
                if (unresolvedBehavior == ApiUnresolvedIdentityPartBehavior.Throw)
                {
                    var unresolvedPath = string.IsNullOrEmpty(snapshot.Path)
                        ? nestedPart.Name
                        : $"{snapshot.Path}.{nestedPart.Name}";
                    throw new ApiIdentityException($"Cannot flatten identity snapshot: nestedPart at path '{unresolvedPath}' is unresolved.");
                }

                // AllowUnresolved: emit structure with ApiId.Empty values if structure is available
                if (nestedPart.Structure is not null && nestedPart.Structure.Count > 0)
                {
                    var partPath = string.IsNullOrEmpty(snapshot.Path)
                        ? nestedPart.Name
                        : $"{snapshot.Path}.{nestedPart.Name}";

                    FlattenStructureWithEmptyValuesRecursive(nestedPart.Structure, partPath, output, useNamedParts);
                }
                else
                {
                    // No structure info - emit single ApiId.Empty value
                    var name = useNamedParts
                        ? (string.IsNullOrEmpty(snapshot.Path) ? nestedPart.Name : $"{snapshot.Path}.{nestedPart.Name}")
                        : null;

                    output.Add(useNamedParts
                        ? ApiIdPart.Create(name, ApiId.Empty)
                        : ApiIdPart.Create(ApiId.Empty));
                }
            }
            else
            {
                FlattenRecursive(nestedPart.Snapshot, output, useNamedParts, unresolvedBehavior);
            }
        }
    }

    private static void FlattenStructureWithEmptyValuesRecursive
    (
        IReadOnlyList<ApiIdentityPart> structure,
        string currentPath,
        List<ApiIdPart> output,
        bool useNamedParts
    )
    {
        foreach (var structurePart in structure)
        {
            var partPath = $"{currentPath}.{structurePart.Name}";

            if (structurePart.Structure is null || structurePart.Structure.Count == 0)
            {
                // Leaf scalar - emit ApiId.Empty
                output.Add(useNamedParts
                    ? ApiIdPart.Create(partPath, ApiId.Empty)
                    : ApiIdPart.Create(ApiId.Empty));
            }
            else
            {
                // Nested composite - recurse into structure
                FlattenStructureWithEmptyValuesRecursive(structurePart.Structure, partPath, output, useNamedParts);
            }
        }
    }

    private ApiId ToApiIdImpl
    (
        bool useNamedParts,
        ApiUnresolvedIdentityPartBehavior unresolvedBehavior
    )
    {
        if (this.Kind == ApiIdentitySnapshotKind.Scalar)
        {
            return this.ScalarValue!.Value;
        }

        var flatParts = new List<ApiIdPart>();
        FlattenRecursive(this, flatParts, useNamedParts, unresolvedBehavior);

        return ApiId.Composite(flatParts);
    }
    #endregion

    #region Index Operators
    /// <summary>
    ///     Navigates to a nested part by dot-separated path.
    /// </summary>
    /// <param name="path">
    ///     A dot-separated path (e.g., "Customer.Country.Id").
    ///     Single-segment paths like "Customer" navigate one level deep.
    /// </param>
    /// <returns>The nested identity snapshot.</returns>
    /// <exception cref="ArgumentException">If the path is invalid or contains empty segments.</exception>
    /// <exception cref="ApiIdentityException">If attempting to navigate into a scalar snapshot or the part is unresolved without structure.</exception>
    /// <exception cref="KeyNotFoundException">If the specified part is not found.</exception>
    public ApiIdentitySnapshot this[string path]
    {
        get
        {
            var result = this.TryNavigate(path);
            if (!result)
            {
                result.ThrowIfFailed();
            }

            return result.Snapshot!;
        }
    }
    #endregion
}
