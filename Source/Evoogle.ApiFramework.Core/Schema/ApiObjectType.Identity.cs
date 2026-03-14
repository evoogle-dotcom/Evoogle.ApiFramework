// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema;

public sealed partial class ApiObjectType
{
    #region Identity Methods
    // <summary>
    //     Checks if two instances have equal identities.
    // </summary>
    // <param name="clrInstance1">The first CLR instance.</param>
    // <param name="clrInstance2">The second CLR instance.</param>
    // <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    // <returns><c>true</c> if both instances have the same identity; otherwise, <c>false</c>.</returns>
    // <remarks>
    //     <para>Returns <c>false</c> if either instance is null or if identity building fails for either instance.</para>
    //     <para><b>Performance:</b> Builds two identities and compares them - roughly 2x the cost of single identity build</para>
    //     <para><b>Use Cases:</b></para>
    //     <list type="bullet">
    //         <item><description>Detecting duplicate entities in collections</description></item>
    //         <item><description>Change detection (comparing old vs new entity state)</description></item>
    //         <item><description>Verifying entity equality without full property comparison</description></item>
    //     </list>
    // </remarks>
    // public bool IdentitiesEqual(object clrInstance1, object clrInstance2, string? apiIdentityName = null)
    // {
    //     if (clrInstance1 is null || clrInstance2 is null)
    //     {
    //         this.Logger.LogTrace("IdentitiesEqual: one or both instances are null");
    //         return false;
    //     }

    //     // Optimization: if they're the same reference, they have the same identity
    //     if (ReferenceEquals(clrInstance1, clrInstance2))
    //     {
    //         this.Logger.LogTrace("IdentitiesEqual: same reference, identities are equal");
    //         return true;
    //     }

    //     if (!this.TryGetIdentity(clrInstance1, out var apiId1, apiIdentityName))
    //     {
    //         this.Logger.LogTrace("IdentitiesEqual: failed to build identity for clrInstance1");
    //         return false;
    //     }

    //     if (!this.TryGetIdentity(clrInstance2, out var apiId2, apiIdentityName))
    //     {
    //         this.Logger.LogTrace("IdentitiesEqual: failed to build identity for clrInstance2");
    //         return false;
    //     }

    //     var equal = apiId1.Equals(apiId2);
    //     this.Logger.LogTrace("IdentitiesEqual result: {Equal} (apiId1: {Id1}, apiId2: {Id2})", equal, apiId1, apiId2);
    //     return equal;
    // }

    // <summary>
    //     Checks if an instance's identity matches a given <see cref="ApiId"/>.
    // </summary>
    // <param name="clrInstance">The CLR instance to check.</param>
    // <param name="apiId">The identity to compare against.</param>
    // <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    // <returns><c>true</c> if the instance's identity matches the given id; otherwise, <c>false</c>.</returns>
    // <remarks>
    //     <para>This is a convenience method that builds the identity and compares it.</para>
    //     <para>Returns <c>false</c> if identity building fails or if the object type has no identity.</para>
    //     <para><b>Performance:</b> Same as <see cref="TryGetIdentity(object, out ApiId, string?)"/> plus equality check (~5-10ns)</para>
    // </remarks>
    // public bool MatchesIdentity(object clrInstance, ApiId id, string? apiIdentityName = null)
    // {
    //     if (clrInstance is null || !id.HasValue)
    //     {
    //         this.Logger.LogTrace("MatchesIdentity: clrInstance or id is null/empty");
    //         return false;
    //     }

    //     if (!this.TryGetIdentity(clrInstance, out var instanceId, apiIdentityName))
    //     {
    //         this.Logger.LogTrace("MatchesIdentity: failed to build identity for instance");
    //         return false;
    //     }

    //     var matches = instanceId.Equals(id);
    //     this.Logger.LogTrace("MatchesIdentity result: {Matches} (instance: {InstanceId}, expected: {ExpectedId})",
    //         matches, instanceId, id);
    //     return matches;
    // }

    // <summary>
    //     Attempts to get an identity from a CLR instance without throwing exceptions.
    // </summary>
    // <param name="clrInstance">The CLR instance to get the identity from.</param>
    // <param name="apiId">When this method returns, contains the identity if successful; otherwise, <see cref="ApiId.Empty" />.</param>
    // <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    // <returns><c>true</c> if the identity was retrieved successfully; otherwise, <c>false</c>.</returns>
    // <remarks>
    //     <para>This method never throws exceptions and returns <c>false</c> on any failure.</para>
    //     <para>Use <see cref="GetIdentity(object, string?)"/> if you need exception details.</para>
    //     <para><b>Performance Characteristics:</b></para>
    //     <list type="bullet">
    //         <item><description><b>Property Access:</b> Uses compiled accessors (reflection-free after initialization) for O(1) property reads</description></item>
    //         <item><description><b>Memory Allocation:</b> Allocates O(n) memory where n = number of identity parts (typically 1-3 parts)</description></item>
    //         <item><description><b>Type Coercion:</b> Cost depends on source/target type compatibility - primitive conversions are fastest (~10-50ns), string parsing is slower (~100-500ns)</description></item>
    //         <item><description><b>Typical Performance:</b> Simple identities: ~50-100ns, Composite identities: ~50-200ns per part</description></item>
    //         <item><description><b>Caching:</b> No built-in caching - results are computed on every call</description></item>
    //     </list>
    //     <para><b>When to Use:</b></para>
    //     <list type="bullet">
    //         <item><description>Use <c>TryGetIdentity</c> when failures are expected and should be handled gracefully</description></item>
    //         <item><description>Use <c>GetIdentity</c> when failures indicate bugs and you need detailed exception information</description></item>
    //         <item><description>Consider caching results if calling repeatedly for the same instance</description></item>
    //     </list>
    // </remarks>
    // public bool TryGetIdentity(object clrInstance, out ApiId apiId, string? apiIdentityName = null)
    // {
    //     apiId = default;

    //     // Validate inputs without throwing
    //     if (clrInstance is null)
    //     {
    //         this.Logger.LogDebug("TryGetIdentity failed: clrInstance is null for type '{TypeName}'", this.ApiName);
    //         return false;
    //     }

    //     if (!this.HasIdentity)
    //     {
    //         this.Logger.LogDebug("TryGetIdentity failed: type '{TypeName}' has no identity configured", this.ApiName);
    //         return false;
    //     }

    //     var apiIdentity = this.ResolveIdentityForBuild(apiIdentityName);
    //     if (apiIdentity is null)
    //     {
    //         var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
    //         this.Logger.LogDebug("TryGetIdentity failed: type '{TypeName}' does not have {IdentityRef}", this.ApiName, identityRef);
    //         return false;
    //     }

    //     this.Logger.LogTrace("Building identity '{IdentityName}' for type '{TypeName}' with {SourceCount} parts",
    //         apiIdentity.ApiName, this.ApiName, apiIdentity.ApiIdentitySources.Length);

    //     // Core implementation - catch any exceptions from deeper layers
    //     try
    //     {
    //         apiId = this.GetIdentityFromInstance(apiIdentity, clrInstance);
    //         var success = apiId.HasValue;

    //         if (success)
    //         {
    //             this.Logger.LogTrace("Successfully built identity '{IdentityName}' for type '{TypeName}': {Identity}",
    //                 apiIdentity.ApiName, this.ApiName, apiId);
    //         }
    //         else
    //         {
    //             this.Logger.LogDebug("TryGetIdentity returned empty identity for type '{TypeName}' identity '{IdentityName}' (likely null handling)",
    //                 this.ApiName, apiIdentity.ApiName);
    //         }

    //         return success;
    //     }
    //     catch (Exception ex)
    //     {
    //         this.Logger.LogDebug(ex, "TryGetIdentity failed with exception for type '{TypeName}' identity '{IdentityName}'",
    //             this.ApiName, apiIdentity.ApiName);
    //         return false;
    //     }
    // }

    // <summary>
    //     Attempts to get an identity from a dictionary of property values without throwing exceptions.
    // </summary>
    // <param name="values">The dictionary of property names to values.</param>
    // <param name="apiId">When this method returns, contains the identity if successful; otherwise, <see cref="ApiId.Empty" />.</param>
    // <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    // <returns><c>true</c> if the identity was retrieved successfully; otherwise, <c>false</c>.</returns>
    // <remarks>
    //     <para>This method never throws exceptions and returns <c>false</c> on any failure.</para>
    //     <para>Use <see cref="GetIdentity(IReadOnlyDictionary{string, object?}, string?)"/> if you need exception details.</para>
    //     <para><b>Performance Characteristics:</b></para>
    //     <list type="bullet">
    //         <item><description><b>Dictionary Lookup:</b> O(1) lookups per identity part (typically 1-3 lookups)</description></item>
    //         <item><description><b>Memory Allocation:</b> Allocates O(n) memory where n = number of identity parts</description></item>
    //         <item><description><b>Type Coercion:</b> Same as instance-based method - cost depends on type compatibility</description></item>
    //         <item><description><b>Typical Performance:</b> Similar to instance-based method plus dictionary lookup overhead (~10-20ns per lookup)</description></item>
    //     </list>
    //     <para><b>Use Cases:</b></para>
    //     <list type="bullet">
    //         <item><description>Building identities from deserialized data (JSON, XML, etc.)</description></item>
    //         <item><description>Query parameter parsing (HTTP GET requests)</description></item>
    //         <item><description>Batch operations where property values are pre-extracted</description></item>
    //     </list>
    // </remarks>
    // public bool TryGetIdentity(IReadOnlyDictionary<string, object?> values, out ApiId apiId, string? apiIdentityName = null)
    // {
    //     apiId = default;

    //     // Validate inputs without throwing
    //     if (values is null)
    //     {
    //         this.Logger.LogDebug("TryGetIdentity failed: values dictionary is null for type '{TypeName}'", this.ApiName);
    //         return false;
    //     }

    //     if (!this.HasIdentity)
    //     {
    //         this.Logger.LogDebug("TryGetIdentity failed: type '{TypeName}' has no identity configured", this.ApiName);
    //         return false;
    //     }

    //     var apiIdentity = this.ResolveIdentityForBuild(apiIdentityName);
    //     if (apiIdentity is null)
    //     {
    //         var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
    //         this.Logger.LogDebug("TryGetIdentity failed: type '{TypeName}' does not have {IdentityRef}", this.ApiName, identityRef);
    //         return false;
    //     }

    //     this.Logger.LogTrace("Getting identity '{IdentityName}' from values dictionary for type '{TypeName}' with {SourceCount} parts",
    //         apiIdentity.ApiName, this.ApiName, apiIdentity.ApiIdentitySources.Length);

    //     // Core implementation - catch any exceptions from deeper layers
    //     try
    //     {
    //         apiId = this.GetIdentityFromValues(apiIdentity, values);
    //         var success = apiId.HasValue;

    //         if (success)
    //         {
    //             this.Logger.LogTrace("Successfully built identity '{IdentityName}' from values for type '{TypeName}': {Identity}",
    //                 apiIdentity.ApiName, this.ApiName, apiId);
    //         }
    //         else
    //         {
    //             this.Logger.LogDebug("TryGetIdentity returned empty identity from values for type '{TypeName}' identity '{IdentityName}' (likely null handling)",
    //                 this.ApiName, apiIdentity.ApiName);
    //         }

    //         return success;
    //     }
    //     catch (Exception ex)
    //     {
    //         this.Logger.LogDebug(ex, "TryGetIdentity failed with exception from values for type '{TypeName}' identity '{IdentityName}'",
    //             this.ApiName, apiIdentity.ApiName);
    //         return false;
    //     }
    // }

    // <summary>
    //     Attempts to get a dictionary mapping instances to their identities without throwing exceptions.
    // </summary>
    // <param name="clrInstances">The collection of CLR instances to get identities for.</param>
    // <param name="apiIdentityMap">When this method returns, contains the dictionary mapping instances to identities if successful; otherwise, an empty dictionary.</param>
    // <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    // <returns><c>true</c> if all identities were built successfully; otherwise, <c>false</c>.</returns>
    // <remarks>
    //     <para>This method never throws exceptions and returns <c>false</c> on any failure.</para>
    //     <para>The <paramref name="apiIdentityMap" /> will contain partial results up to the first failure.</para>
    //     <para><b>Performance Characteristics:</b></para>
    //     <list type="bullet">
    //         <item><description><b>Dictionary Overhead:</b> Additional O(n) memory for reference mapping</description></item>
    //         <item><description><b>Lookup Performance:</b> O(1) identity lookup by instance reference</description></item>
    //         <item><description><b>Fail-Fast:</b> Stops on first failure for consistency</description></item>
    //     </list>
    // </remarks>
    // public bool TryGetIdentityMap(IEnumerable<object?> clrInstances, out IReadOnlyDictionary<object, ApiId> apiIdentityMap, string? apiIdentityName = null)
    // {
    //     apiIdentityMap = new Dictionary<object, ApiId>();

    //     if (clrInstances is null)
    //     {
    //         this.Logger.LogDebug("TryGetIdentityMap: clrInstances collection is null");
    //         return false;
    //     }

    //     if (!this.HasIdentity)
    //     {
    //         this.Logger.LogDebug("TryGetIdentityMap: type '{TypeName}' has no identity configured", this.ApiName);
    //         return false;
    //     }

    //     var apiIdentity = this.ResolveIdentityForBuild(apiIdentityName);
    //     if (apiIdentity is null)
    //     {
    //         var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
    //         this.Logger.LogDebug("TryGetIdentityMap: type '{TypeName}' does not have {IdentityRef}", this.ApiName, identityRef);
    //         return false;
    //     }

    //     this.Logger.LogTrace("Getting identity map for batch using identity '{IdentityName}' on type '{TypeName}'",
    //         apiIdentity.ApiName, this.ApiName);

    //     var results = new Dictionary<object, ApiId>();

    //     foreach (var clrInstance in clrInstances)
    //     {
    //         if (clrInstance is null)
    //         {
    //             this.Logger.LogDebug("TryGetIdentityMap: encountered null clrInstance, failing fast");
    //             return false;
    //         }

    //         if (!this.TryGetIdentity(clrInstance, out var apiId, apiIdentityName))
    //         {
    //             this.Logger.LogDebug("TryGetIdentityMap: failed to get identity for clrInstance, failing fast");
    //             return false;
    //         }

    //         results[clrInstance] = apiId;
    //     }

    //     apiIdentityMap = results;
    //     this.Logger.LogDebug("TryGetIdentityMap completed successfully with {Count} entries for type '{TypeName}'", results.Count, this.ApiName);
    //     return true;
    // }

    // <summary>
    //     Attempts to get identities for a collection of instances without throwing exceptions.
    // </summary>
    // <param name="clrInstances">The collection of CLR instances to get identities for.</param>
    // <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    // <returns>A read-only list of <see cref="ApiIdentityBuildResult"/> containing the result for each instance.</returns>
    // <remarks>
    //     <para>This method never throws exceptions. Failed identity builds are indicated in the result tuple.</para>
    //     <para>Null instances are skipped and not included in the results.</para>
    //     <para><b>Performance Characteristics:</b></para>
    //     <list type="bullet">
    //         <item><description><b>Identity Resolution:</b> Resolved once for all instances (O(1) vs O(n))</description></item>
    //         <item><description><b>Fault Tolerance:</b> Continues processing all instances even if some fail</description></item>
    //         <item><description><b>Result Pairing:</b> Returns instance-identity pairs with success flag for correlation</description></item>
    //     </list>
    //     <para><b>Use Cases:</b></para>
    //     <list type="bullet">
    //         <item><description>Validation - identify which instances have invalid identities</description></item>
    //         <item><description>Partial processing - continue with valid identities, log failures</description></item>
    //         <item><description>Data import - process as much as possible despite errors</description></item>
    //     </list>
    // </remarks>
    // public IReadOnlyList<ApiIdentityBuildResult> TryGetIdentities(IEnumerable<object?> clrInstances, string? apiIdentityName = null)
    // {
    //     if (clrInstances is null)
    //     {
    //         this.Logger.LogDebug("TryGetIdentities: clrInstances collection is null");
    //         return [];
    //     }

    //     if (!this.HasIdentity)
    //     {
    //         this.Logger.LogDebug("TryGetIdentities: type '{TypeName}' has no identity configured", this.ApiName);
    //         return [];
    //     }

    //     var apiIdentity = this.ResolveIdentityForBuild(apiIdentityName);
    //     if (apiIdentity is null)
    //     {
    //         var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
    //         this.Logger.LogDebug("TryGetIdentities: type '{TypeName}' does not have {IdentityRef}", this.ApiName, identityRef);
    //         return [];
    //     }

    //     this.Logger.LogTrace("Getting identities for batch (fault-tolerant) using identity '{IdentityName}' on type '{TypeName}'",
    //         apiIdentity.ApiName, this.ApiName);

    //     var results = new List<ApiIdentityBuildResult>();
    //     var successCount = 0;
    //     var failureCount = 0;

    //     foreach (var clrInstance in clrInstances)
    //     {
    //         if (clrInstance is null)
    //         {
    //             this.Logger.LogTrace("TryGetIdentities: skipping null clrInstance");
    //             continue;
    //         }

    //         if (this.TryGetIdentity(clrInstance, out var apiId, apiIdentityName))
    //         {
    //             results.Add(new ApiIdentityBuildResult { Instance = clrInstance, Id = apiId, Success = true });
    //             successCount++;
    //         }
    //         else
    //         {
    //             results.Add(new ApiIdentityBuildResult { Instance = clrInstance, Id = ApiId.Empty, Success = false });
    //             failureCount++;
    //         }
    //     }

    //     this.Logger.LogDebug("TryGetIdentities completed: {SuccessCount} succeeded, {FailureCount} failed for type '{TypeName}'",
    //         successCount, failureCount, this.ApiName);

    //     return results;
    // }

    // <summary>
    //     Attempts to build an <see cref="ApiIdentitySnapshot"/> from a CLR instance without throwing exceptions.
    // </summary>
    // <param name="clrInstance">The CLR instance to build the identity snapshot from.</param>
    // <param name="snapshot">When this method returns, contains the identity snapshot if successful; otherwise, <c>default</c>.</param>
    // <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    // <returns><c>true</c> if the identity snapshot was built successfully; otherwise, <c>false</c>.</returns>
    // <remarks>
    //     <para>This method never throws exceptions and returns <c>false</c> on any failure.</para>
    //     <para>
    //         <see cref="ApiIdentitySnapshot"/> preserves the full nested identity structure, enabling path-based navigation
    //         and diagnostics before flattening to <see cref="ApiId"/>.
    //         Use <see cref="TryGetIdentity(object, out ApiId, string?)"/> when the full structure is not needed.
    //     </para>
    //     <para><b>Nested Identity:</b></para>
    //     <para>
    //         When a source references a nested <see cref="ApiObjectType"/>, this method recursively resolves
    //         the nested object's identity, producing a composite snapshot with named parts.
    //         Null nested objects produce an unresolved part (structure metadata preserved) according to the
    //         configured <see cref="ApiIdentityNullHandling"/> policy.
    //     </para>
    // </remarks>
    // public bool TryGetIdentitySnapshot(object clrInstance, out ApiIdentitySnapshot? snapshot, string? apiIdentityName = null)
    // {
    //     snapshot = default;

    //     if (clrInstance is null)
    //     {
    //         this.Logger.LogDebug("TryGetIdentitySnapshot failed: clrInstance is null for type '{TypeName}'", this.ApiName);
    //         return false;
    //     }

    //     if (!this.HasIdentity)
    //     {
    //         this.Logger.LogDebug("TryGetIdentitySnapshot failed: type '{TypeName}' has no identity configured", this.ApiName);
    //         return false;
    //     }

    //     var apiIdentity = this.ResolveIdentityForBuild(apiIdentityName);
    //     if (apiIdentity is null)
    //     {
    //         var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
    //         this.Logger.LogDebug("TryGetIdentitySnapshot failed: type '{TypeName}' does not have {IdentityRef}", this.ApiName, identityRef);
    //         return false;
    //     }

    //     this.Logger.LogTrace("Building identity snapshot '{IdentityName}' for type '{TypeName}' with {SourceCount} sources", apiIdentity.ApiName, this.ApiName, apiIdentity.ApiIdentitySources.Length);

    //     try
    //     {
    //         snapshot = this.GetIdentitySnapshotFromInstance(apiIdentity, clrInstance, parentPath: null);
    //         this.Logger.LogTrace("Successfully built identity snapshot '{IdentityName}' for type '{TypeName}': {Snapshot}", apiIdentity.ApiName, this.ApiName, snapshot);
    //         return true;
    //     }
    //     catch (Exception ex)
    //     {
    //         this.Logger.LogDebug(ex, "TryGetIdentitySnapshot failed with exception for type '{TypeName}' identity '{IdentityName}'", this.ApiName, apiIdentity.ApiName);
    //         return false;
    //     }
    // }

    // <summary>
    //     Attempts to build an <see cref="ApiIdentitySnapshot"/> from a dictionary of property values without throwing exceptions.
    // </summary>
    // <param name="values">
    //     The dictionary of property names to values.
    //     For nested identity sources, the value must be an <see cref="IReadOnlyDictionary{String, Object}"/>
    //     containing the nested object's property values, or <c>null</c> to represent an unresolved nested object.
    // </param>
    // <param name="snapshot">When this method returns, contains the identity snapshot if successful; otherwise, <c>default</c>.</param>
    // <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    // <returns><c>true</c> if the identity snapshot was built successfully; otherwise, <c>false</c>.</returns>
    // <remarks>
    //     <para>This method never throws exceptions and returns <c>false</c> on any failure.</para>
    //     <para>
    //         <see cref="ApiIdentitySnapshot"/> preserves the full nested identity structure, enabling path-based navigation
    //         and diagnostics before flattening to <see cref="ApiId"/>.
    //         Use <see cref="TryGetIdentity(IReadOnlyDictionary{string, object?}, out ApiId, string?)"/> when the full structure is not needed.
    //     </para>
    //     <para><b>Values Dictionary Format:</b></para>
    //     <para>
    //         Scalar sources expect a primitive-compatible value keyed by the property's API name.
    //         Nested sources expect a nested <see cref="IReadOnlyDictionary{String, Object}"/> (or <c>null</c> for unresolved)
    //         keyed by the property's API name.
    //     </para>
    //     <para><b>Use Cases:</b></para>
    //     <list type="bullet">
    //         <item><description>Building identities from deserialized data (JSON, XML, query parameters)</description></item>
    //         <item><description>Batch operations where property values are pre-extracted</description></item>
    //         <item><description>Integration with external systems that provide structured identity data</description></item>
    //     </list>
    // </remarks>
    // public bool TryGetIdentitySnapshot(IReadOnlyDictionary<string, object?> values, out ApiIdentitySnapshot? snapshot, string? apiIdentityName = null)
    // {
    //     snapshot = default;

    //     if (values is null)
    //     {
    //         this.Logger.LogDebug("TryGetIdentitySnapshot failed: values dictionary is null for type '{TypeName}'", this.ApiName);
    //         return false;
    //     }

    //     if (!this.HasIdentity)
    //     {
    //         this.Logger.LogDebug("TryGetIdentitySnapshot failed: type '{TypeName}' has no identity configured", this.ApiName);
    //         return false;
    //     }

    //     var apiIdentity = this.ResolveIdentityForBuild(apiIdentityName);
    //     if (apiIdentity is null)
    //     {
    //         var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
    //         this.Logger.LogDebug("TryGetIdentitySnapshot failed: type '{TypeName}' does not have {IdentityRef}", this.ApiName, identityRef);
    //         return false;
    //     }

    //     this.Logger.LogTrace("Building identity snapshot '{IdentityName}' from values dictionary for type '{TypeName}' with {SourceCount} sources", apiIdentity.ApiName, this.ApiName, apiIdentity.ApiIdentitySources.Length);

    //     try
    //     {
    //         snapshot = this.GetIdentitySnapshotFromValues(apiIdentity, values, parentPath: null);
    //         this.Logger.LogTrace("Successfully built identity snapshot '{IdentityName}' from values for type '{TypeName}': {Snapshot}", apiIdentity.ApiName, this.ApiName, snapshot);
    //         return true;
    //     }
    //     catch (Exception ex)
    //     {
    //         this.Logger.LogDebug(ex, "TryGetIdentitySnapshot failed with exception from values for type '{TypeName}' identity '{IdentityName}'", this.ApiName, apiIdentity.ApiName);
    //         return false;
    //     }
    // }
    #endregion

    #region Implementation Methods
    // private ApiId GetIdentityFromInstance(ApiIdentity apiIdentity, object clrInstance)
    // {
    //     var apiIdentitySources = apiIdentity.ApiIdentitySources;

    //     var apiPartsCount = apiIdentitySources.Length;
    //     var apiParts = new List<ApiIdPart>(apiPartsCount);

    //     foreach (var apiIdentitySource in apiIdentitySources)
    //     {
    //         // ApiProperty handles property access AND coercion in one optimized call
    //         var apiPartId = this.MaterializeApiIdFromProperty(apiIdentitySource, apiIdentity, clrInstance);

    //         // Always create named parts (serialization layer decides ordered vs named formatting)
    //         var apiPropertyName = apiIdentitySource.ApiProperty.ApiName;
    //         apiParts.Add(ApiIdPart.Create(apiPropertyName, apiPartId));
    //     }

    //     return FinalizeComposite(apiParts);
    // }

    // private ApiId GetIdentityFromValues(ApiIdentity apiIdentity, IReadOnlyDictionary<string, object?> values)
    // {
    //     var apiIdentitySources = apiIdentity.ApiIdentitySources;

    //     var apiPartsCount = apiIdentitySources.Length;
    //     var apiParts = new List<ApiIdPart>(apiPartsCount);

    //     foreach (var apiIdentitySource in apiIdentitySources)
    //     {
    //         if (!values.TryGetValue(apiIdentitySource.ApiProperty.ApiName, out var rawValue))
    //         {
    //             throw new ApiIdentityException(
    //                 $"Property '{apiIdentitySource.ApiProperty.ApiName}' not found in values dictionary for identity '{apiIdentity.ApiName}'.");
    //         }

    //         // Materialize the ApiId using TypeCoercion and pre-resolved target type
    //         // Note: We don't have the actual CLR instance type, so error messages will reference the dictionary
    //         var apiPartId = this.MaterializeApiIdFromPropertyValue(apiIdentitySource, rawValue, apiIdentity);

    //         // Always create named parts (serialization layer decides ordered vs named formatting)
    //         var apiPropertyName = apiIdentitySource.ApiProperty.ApiName;
    //         apiParts.Add(ApiIdPart.Create(apiPropertyName, apiPartId));
    //     }

    //     return FinalizeComposite(apiParts);
    // }

    // private ApiId ConvertToApiId(object value, Type valueType, string propertyName, string identityName, string contextDescription)
    // {
    //     try
    //     {
    //         return ApiId.FromObject(value, valueType);
    //     }
    //     catch (Exception ex)
    //     {
    //         this.Logger.LogWarning(ex, "Identity coercion failed for {Property}", propertyName);
    //         throw new ApiIdentityException($"Failed to convert property '{propertyName}' value of type '{value.GetType().Name}' to ApiId for identity '{identityName}' on {contextDescription}.", ex);
    //     }
    // }

    // private static ApiId FinalizeComposite(List<ApiIdPart> apiParts)
    //     => apiParts.Count switch
    //     {
    //         0 => ApiId.Empty,
    //         1 => apiParts[0].Value,
    //         _ => ApiId.Composite(apiParts)
    //     };

    // private ApiIdentityNullHandling GetIdentityNullHandling()
    //     => this.ApiOptions?.ApiIdentityNullHandling ?? this.ApiSchemaContext.ApiSchemaOptions.ApiIdentityNullHandling;

    // private ApiId MaterializeApiIdFromProperty(ApiIdentitySource apiIdentitySource, ApiIdentity apiIdentity, object clrInstance)
    // {
    //     // Only valid for scalar sources — ClrScalarType is null for nested sources
    //     var apiPropertyName = apiIdentitySource.ApiProperty.ApiName;
    //     var apiIdentityName = apiIdentity.ApiName;
    //     var clrScalarType = apiIdentitySource.ClrScalarType
    //         ?? throw new ApiIdentityException($"Identity source '{apiPropertyName}' on identity '{apiIdentityName}' does not have a resolved scalar type. Ensure the source is a scalar kind before calling this method.");
    //     var clrScalarTypeName = clrScalarType.Name;

    //     this.Logger.LogTrace("Reading and coercing property '{PropertyName}' to '{ScalarType}' for identity '{IdentityName}'", apiPropertyName, clrScalarTypeName, apiIdentityName);

    //     // Let ApiProperty handle property access AND type coercion in one optimized call
    //     // This leverages ApiProperty's compiled accessors and coercion caching
    //     if (!apiIdentitySource.ApiProperty.TryGetValue(clrInstance, out var coercedValue, clrScalarType))
    //     {
    //         throw new ApiIdentityException($"Failed to read and coerce property '{apiPropertyName}' to type '{clrScalarTypeName}' for identity '{apiIdentityName}' on type '{this.ClrTypeName}'.");
    //     }

    //     // Handle null values according to the configured null handling
    //     if (coercedValue is null)
    //     {
    //         var nullHandling = this.GetIdentityNullHandling();
    //         this.Logger.LogDebug("Property '{PropertyName}' has null value for identity '{IdentityName}', null handling: {NullHandling}", apiPropertyName, apiIdentityName, nullHandling);

    //         if (nullHandling == ApiIdentityNullHandling.ThrowException)
    //         {
    //             throw new ApiIdentityException($"Property '{apiPropertyName}' has a null value for identity '{apiIdentityName}' on type '{this.ClrTypeName}'. Null values are not allowed with {nameof(ApiIdentityNullHandling.ThrowException)} configured.");
    //         }

    //         return ApiId.Empty;
    //     }

    //     // Convert the already-coerced value to ApiId
    //     return this.ConvertToApiId(coercedValue, clrScalarType, apiPropertyName, apiIdentityName, this.ClrTypeName);
    // }

    // private ApiId MaterializeApiIdFromPropertyValue(ApiIdentitySource apiIdentitySource, object? rawValue, ApiIdentity apiIdentity)
    // {
    //     // Only valid for scalar sources — ClrScalarType is null for nested sources
    //     var apiPropertyName = apiIdentitySource.ApiProperty.ApiName;
    //     var apiIdentityName = apiIdentity.ApiName;
    //     var clrScalarType = apiIdentitySource.ClrScalarType
    //         ?? throw new ApiIdentityException($"Identity source '{apiPropertyName}' on identity '{apiIdentityName}' does not have a resolved scalar type. Ensure the source is a scalar kind before calling this method.");
    //     var clrScalarTypeName = clrScalarType.Name;

    //     this.Logger.LogTrace("Coercing property '{PropertyName}' from values dictionary ('{SourceType}' to '{TargetType}') for identity '{IdentityName}'",
    //         apiPropertyName, rawValue?.GetType().SafeToName(), clrScalarTypeName, apiIdentityName);

    //     // Use ApiProperty's coercion for consistency and potential performance benefits
    //     if (!apiIdentitySource.ApiProperty.TryCoerceValue(rawValue, out var coercedValue, clrScalarType))
    //     {
    //         throw new ApiIdentityException($"Failed to coerce property '{apiPropertyName}' value to type '{clrScalarTypeName}' for identity '{apiIdentityName}' from values dictionary.");
    //     }

    //     // Handle null values according to the configured null handling
    //     if (coercedValue is null)
    //     {
    //         var nullHandling = this.GetIdentityNullHandling();
    //         this.Logger.LogDebug("Property '{PropertyName}' from values has null value for identity '{IdentityName}', null handling: {NullHandling}", apiPropertyName, apiIdentityName, nullHandling);

    //         if (nullHandling == ApiIdentityNullHandling.ThrowException)
    //         {
    //             throw new ApiIdentityException($"Property '{apiPropertyName}' has a null value for identity '{apiIdentityName}' from values dictionary. Null values are not allowed with {nameof(ApiIdentityNullHandling.ThrowException)} configured.");
    //         }

    //         return ApiId.Empty;
    //     }

    //     // Convert the typed value to ApiId
    //     return this.ConvertToApiId(coercedValue, clrScalarType, apiPropertyName, apiIdentityName, "values dictionary");
    // }

    // internal ApiIdentity? ResolveIdentityForBuild(string? apiIdentityName)
    // {
    //     if (!string.IsNullOrWhiteSpace(apiIdentityName))
    //     {
    //         return this.TryGetIdentityByApiName(apiIdentityName, out var apiIdentity) ? apiIdentity : null;
    //     }

    //     return this.ApiPrimaryIdentity;
    // }

    // private ApiIdentitySnapshot GetIdentitySnapshotFromInstance(ApiIdentity apiIdentity, object clrInstance, string? parentPath)
    // {
    //     var sources = apiIdentity.ApiIdentitySources;

    //     // Fast path: single scalar source produces a scalar snapshot directly,
    //     // mirroring FinalizeComposite which unwraps single-part ApiId to a scalar value.
    //     if (sources.Length == 1 && sources[0].ApiKind == ApiIdentitySourceKind.Scalar)
    //     {
    //         var apiId = this.MaterializeApiIdFromProperty(sources[0], apiIdentity, clrInstance);
    //         return ApiIdentitySnapshot.Scalar(parentPath, apiId);
    //     }

    //     var parts = new List<ApiIdentityPart>(sources.Length);

    //     foreach (var source in sources)
    //     {
    //         var partName = source.ApiProperty.ApiName;
    //         var childPath = BuildChildPath(parentPath, partName);

    //         if (source.ApiKind == ApiIdentitySourceKind.Scalar)
    //         {
    //             var apiId = this.MaterializeApiIdFromProperty(source, apiIdentity, clrInstance);
    //             parts.Add(new ApiIdentityPart(partName, ApiIdentitySnapshot.Scalar(childPath, apiId)));
    //         }
    //         else
    //         {
    //             // Nested source: read the nested CLR object and recursively resolve its identity
    //             var nestedObjectType = (ApiObjectType)source.ApiProperty.ApiType;
    //             var nestedIdentity = source.ApiNestedIdentity!;

    //             source.ApiProperty.TryGetValue(clrInstance, out var nestedObject);

    //             if (nestedObject is null)
    //             {
    //                 var nullHandling = this.GetIdentityNullHandling();

    //                 this.Logger.LogDebug("Nested property '{PartName}' is null for identity '{IdentityName}' on type '{TypeName}', null handling: {NullHandling}", partName, apiIdentity.ApiName, this.ApiName, nullHandling);

    //                 if (nullHandling == ApiIdentityNullHandling.ThrowException)
    //                 {
    //                     throw new ApiIdentityException
    //                     (
    //                         $"Nested property '{partName}' has a null value for identity '{apiIdentity.ApiName}' on type '{this.ClrTypeName}'. " +
    //                         $"Null values are not allowed with {nameof(ApiIdentityNullHandling.ThrowException)} configured."
    //                     );
    //                 }

    //                 // Unresolved part
    //                 // Build the structure metadata for the nested identity so that the snapshot can represent the expected shape of the unresolved part.
    //                 var structure = BuildIdentityStructure(nestedIdentity);
    //                 parts.Add(new ApiIdentityPart(partName, null, structure));
    //             }
    //             else
    //             {
    //                 var nestedSnapshot = nestedObjectType.GetIdentitySnapshotFromInstance(nestedIdentity, nestedObject, childPath);
    //                 parts.Add(new ApiIdentityPart(partName, nestedSnapshot));
    //             }
    //         }
    //     }

    //     return ApiIdentitySnapshot.Composite(parentPath, parts);
    // }

    // private ApiIdentitySnapshot GetIdentitySnapshotFromValues(ApiIdentity apiIdentity, IReadOnlyDictionary<string, object?> values, string? parentPath)
    // {
    //     var sources = apiIdentity.ApiIdentitySources;

    //     // Fast path: single scalar source produces a scalar snapshot directly,
    //     // mirroring FinalizeComposite which unwraps single-part ApiId to a scalar value.
    //     if (sources.Length == 1 && sources[0].ApiKind == ApiIdentitySourceKind.Scalar)
    //     {
    //         var source = sources[0];
    //         var partName = source.ApiProperty.ApiName;

    //         if (!values.TryGetValue(partName, out var rawScalarValue))
    //         {
    //             throw new ApiIdentityException($"Property '{partName}' not found in values dictionary for identity '{apiIdentity.ApiName}' on type '{this.ApiName}'.");
    //         }

    //         var apiId = this.MaterializeApiIdFromPropertyValue(source, rawScalarValue, apiIdentity);
    //         return ApiIdentitySnapshot.Scalar(parentPath, apiId);
    //     }

    //     var parts = new List<ApiIdentityPart>(sources.Length);

    //     foreach (var source in sources)
    //     {
    //         var partName = source.ApiProperty.ApiName;
    //         var childPath = BuildChildPath(parentPath, partName);

    //         if (!values.TryGetValue(partName, out var rawValue))
    //         {
    //             throw new ApiIdentityException($"Property '{partName}' not found in values dictionary for identity '{apiIdentity.ApiName}' on type '{this.ApiName}'.");
    //         }

    //         if (source.ApiKind == ApiIdentitySourceKind.Scalar)
    //         {
    //             var apiId = this.MaterializeApiIdFromPropertyValue(source, rawValue, apiIdentity);
    //             parts.Add(new ApiIdentityPart(partName, ApiIdentitySnapshot.Scalar(childPath, apiId)));
    //         }
    //         else
    //         {
    //             // Nested source: expect a nested dictionary or null
    //             var nestedObjectType = (ApiObjectType)source.ApiProperty.ApiType;
    //             var nestedIdentity = source.ApiNestedIdentity!;

    //             if (rawValue is null)
    //             {
    //                 var nullHandling = this.GetIdentityNullHandling();

    //                 this.Logger.LogDebug("Nested property '{PartName}' is null in values dictionary for identity '{IdentityName}' on type '{TypeName}', null handling: {NullHandling}", partName, apiIdentity.ApiName, this.ApiName, nullHandling);

    //                 if (nullHandling == ApiIdentityNullHandling.ThrowException)
    //                 {
    //                     throw new ApiIdentityException
    //                     (
    //                         $"Nested property '{partName}' has a null value for identity '{apiIdentity.ApiName}' from values dictionary. " +
    //                         $"Null values are not allowed with {nameof(ApiIdentityNullHandling.ThrowException)} configured."
    //                     );
    //                 }

    //                 // Unresolved part
    //                 // Build the structure metadata for the nested identity so that the snapshot can represent the expected shape of the unresolved part.
    //                 var structure = BuildIdentityStructure(nestedIdentity);
    //                 parts.Add(new ApiIdentityPart(partName, null, structure));
    //             }
    //             else if (rawValue is IReadOnlyDictionary<string, object?> nestedValues)
    //             {
    //                 var nestedSnapshot = nestedObjectType.GetIdentitySnapshotFromValues(nestedIdentity, nestedValues, childPath);
    //                 parts.Add(new ApiIdentityPart(partName, nestedSnapshot));
    //             }
    //             else
    //             {
    //                 throw new ApiIdentityException
    //                 (
    //                     $"Nested property '{partName}' for identity '{apiIdentity.ApiName}' expected an {nameof(IReadOnlyDictionary<string, object?>)} " +
    //                     $"but received '{rawValue.GetType().Name}'. Nested identity sources require a dictionary of property values."
    //                 );
    //             }
    //         }
    //     }

    //     return ApiIdentitySnapshot.Composite(parentPath, parts);
    // }

    // <summary>
    //     Builds a structural skeleton of <see cref="ApiIdentityPart"/> entries for the given identity.
    //     This structure captures the expected shape of a resolved snapshot and is attached to unresolved
    //     nested parts so that <see cref="ApiIdentitySnapshot.ToApiId"/> can emit the correct number of
    //     empty slots under <see cref="ApiUnresolvedIdentityPartBehavior.UseEmpty"/>.
    // </summary>
    // private static ApiIdentityPart[]? BuildIdentityStructure(ApiIdentity apiIdentity)
    // {
    //     var sources = apiIdentity.ApiIdentitySources;

    //     if (sources.Length == 0)
    //     {
    //         return null;
    //     }

    //     // A single scalar source maps to a scalar snapshot (no composite wrapper), so no structure needed
    //     if (sources.Length == 1 && sources[0].ApiKind == ApiIdentitySourceKind.Scalar)
    //     {
    //         return null;
    //     }

    //     var structure = new ApiIdentityPart[sources.Length];

    //     for (var i = 0; i < sources.Length; i++)
    //     {
    //         var source = sources[i];
    //         var partName = source.ApiProperty.ApiName;

    //         if (source.ApiKind == ApiIdentitySourceKind.Scalar)
    //         {
    //             structure[i] = new ApiIdentityPart(partName, null);
    //         }
    //         else
    //         {
    //             // Recursively build the nested structure skeleton
    //             var nestedStructure = BuildIdentityStructure(source.ApiNestedIdentity!);
    //             structure[i] = new ApiIdentityPart(partName, null, nestedStructure);
    //         }
    //     }

    //     return structure;
    // }

    // private static string BuildChildPath(string? parentPath, string segment) => string.IsNullOrEmpty(parentPath) ? segment : $"{parentPath}.{segment}";
    #endregion
}
