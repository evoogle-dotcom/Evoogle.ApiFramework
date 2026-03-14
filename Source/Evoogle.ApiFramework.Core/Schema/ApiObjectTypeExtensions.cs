// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Extension methods for <see cref="ApiObjectType"/> class.
/// </summary>
public static class ApiObjectTypeExtensions
{
    #region Extension Methods
    /// <summary>
    ///     Gets an identity from a CLR instance.
    /// </summary>
    /// <param name="apiObjectType">The API object type to get the identity for.</param>
    /// <param name="clrInstance">The CLR instance to get the identity from.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns>The identity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrInstance"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the object type has no identity configured or the specified identity name is not found.</exception>
    /// <exception cref="ApiIdentityException">Thrown when type coercion fails or null handling requires throwing.</exception>
    /// <remarks>
    ///     <para><b>Performance Characteristics:</b></para>
    ///     <list type="bullet">
    ///         <item><description><b>Property Access:</b> Uses compiled accessors (reflection-free) for optimal performance</description></item>
    ///         <item><description><b>Type Coercion:</b> Automatic type conversion using the schema's TypeCoercion service</description></item>
    ///         <item><description><b>Memory:</b> Allocates memory proportional to identity complexity (O(n) where n = parts)</description></item>
    ///     </list>
    ///     <para><b>Method Selection Guide:</b></para>
    ///     <list type="bullet">
    ///         <item><description>Use <c>GetIdentity</c> when failures indicate programming errors and you need detailed exceptions</description></item>
    ///         <item><description>Use <see cref="ApiObjectType.TryGetIdentity(object, out ApiId, string?)"/> for expected failures or validation scenarios</description></item>
    ///         <item><description>Use <see cref="ApiObjectType.MatchesIdentity"/> to compare without allocating an ApiId</description></item>
    ///     </list>
    /// </remarks>
    // public static ApiId GetIdentity(this ApiObjectType apiObjectType, object clrInstance, string? apiIdentityName = null)
    // {
    //     ArgumentNullException.ThrowIfNull(clrInstance);

    //     if (!apiObjectType.TryGetIdentity(clrInstance, out var id, apiIdentityName))
    //     {
    //         // Provide detailed diagnostics for the throwing version
    //         if (!apiObjectType.HasIdentity)
    //         {
    //             throw new InvalidOperationException($"ApiObjectType '{apiObjectType.ApiName}' has no identity configured.");
    //         }

    //         var identity = apiObjectType.ResolveIdentityForBuild(apiIdentityName);
    //         if (identity is null)
    //         {
    //             var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
    //             throw new InvalidOperationException($"ApiObjectType '{apiObjectType.ApiName}' does not have {identityRef}.");
    //         }

    //         // If we got here, something else failed during building
    //         throw new ApiIdentityException($"Failed to build identity for '{apiObjectType.ApiName}'.");
    //     }

    //     return id;
    // }

    /// <summary>
    ///     Gets an identity from a dictionary of property values.
    /// </summary>
    /// <param name="apiObjectType">The API object type to get the identity for.</param>
    /// <param name="values">The dictionary of property names to values.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns>The identity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the object type has no identity configured or the specified identity name is not found.</exception>
    /// <exception cref="ApiIdentityException">Thrown when type coercion fails, required properties are missing, or null handling requires throwing.</exception>
    /// <remarks>
    ///     <para><b>Typical Use Cases:</b></para>
    ///     <list type="bullet">
    ///         <item><description>Deserializing identities from JSON/XML where you have property-value pairs</description></item>
    ///         <item><description>Parsing query string parameters or form data into identities</description></item>
    ///         <item><description>Building identities from database row dictionaries</description></item>
    ///         <item><description>Batch processing where property values are extracted once and reused</description></item>
    ///     </list>
    ///     <para><b>Performance Notes:</b></para>
    ///     <list type="bullet">
    ///         <item><description>Dictionary lookups add ~10-20ns overhead per identity part compared to instance-based method</description></item>
    ///         <item><description>Type coercion cost is identical to instance-based method</description></item>
    ///         <item><description>Consider this method for batch operations to avoid repeated reflection on source objects</description></item>
    ///     </list>
    /// </remarks>
    // public static ApiId GetIdentity(this ApiObjectType apiObjectType, IReadOnlyDictionary<string, object?> values, string? apiIdentityName = null)
    // {
    //     ArgumentNullException.ThrowIfNull(values);

    //     if (!apiObjectType.TryGetIdentity(values, out var id, apiIdentityName))
    //     {
    //         if (!apiObjectType.HasIdentity)
    //         {
    //             throw new InvalidOperationException($"ApiObjectType '{apiObjectType.ApiName}' has no identity configured.");
    //         }

    //         var identity = apiObjectType.ResolveIdentityForBuild(apiIdentityName);
    //         if (identity is null)
    //         {
    //             var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
    //             throw new InvalidOperationException($"ApiObjectType '{apiObjectType.ApiName}' does not have {identityRef}.");
    //         }

    //         throw new ApiIdentityException($"Failed to build identity for '{apiObjectType.ApiName}' from values dictionary.");
    //     }

    //     return id;
    // }

    /// <summary>
    ///     Gets an <see cref="ApiIdentity"/> by its API name.
    /// </summary>
    /// <param name="apiObjectType">The API object type to search.</param>
    /// <param name="apiName">The API name of the identity to retrieve.</param>
    /// <returns>The <see cref="ApiIdentity"/> with the specified API name.</returns
    /// <exception cref="ApiSchemaException">
    ///     Thrown if no identity with the specified API name exists in the object type.
    ///     The exception message includes a list of all available identity API names.
    /// </exception>
    /// <remarks>
    ///     This method performs a case-sensitive search for the identity by its API name.
    ///     Use <see cref="ApiObjectType.TryGetIdentityByApiName"/> if you prefer non-throwing behavior.
    /// </remarks>
    public static ApiIdentity GetIdentityByApiName(this ApiObjectType apiObjectType, string apiName)
    {
        if (apiObjectType.TryGetIdentityByApiName(apiName, out var apiIdentity))
        {
            return apiIdentity!;
        }

        var availableIdentitiesByApiName = string.Join(',', apiObjectType.ApiIdentities.OrderBy(i => i.ApiName).Select(i => i.ApiName));
        var errorMessage =
            $"{nameof(ApiIdentity)} with {nameof(ApiIdentity.ApiName)} '{apiName.SafeToString()}' not found in {apiObjectType.SafeToString()}. " +
            $"Available {nameof(ApiIdentity)} by {nameof(ApiIdentity.ApiName)} are: {availableIdentitiesByApiName}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets a dictionary mapping instances to their identities.
    /// </summary>
    /// <param name="apiObjectType">The API object type to get identities for.</param>
    /// <param name="instances">The collection of CLR instances to build identities for.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns>A read-only dictionary mapping each instance to its built identity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="apiObjectType"/> or <paramref name="instances"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the object type has no identity configured or the specified identity name is not found.</exception>
    /// <exception cref="ApiIdentityException">Thrown when identity building fails for any instance.</exception>
    /// <remarks>
    ///     <para><b>Common Use Cases:</b></para>
    ///     <list type="bullet">
    ///         <item><description><b>Change Tracking:</b> Map entities to original identities for comparison</description></item>
    ///         <item><description><b>Duplicate Detection:</b> Find multiple instances with the same identity</description></item>
    ///         <item><description><b>Batch Updates:</b> Correlate instances with database records by identity</description></item>
    ///     </list>
    ///     <para>Example: <c>var map = objectType.GetIdentityMap(entities); var duplicates = map.GroupBy(x => x.Value).Where(g => g.Count() > 1);</c></para>
    /// </remarks>
    // public static IReadOnlyDictionary<object, ApiId> GetIdentityMap(this ApiObjectType apiObjectType, IEnumerable<object?> instances, string? apiIdentityName = null)
    // {
    //     ArgumentNullException.ThrowIfNull(apiObjectType);
    //     ArgumentNullException.ThrowIfNull(instances);

    //     if (!apiObjectType.TryGetIdentityMap(instances, out var identityMap, apiIdentityName))
    //     {
    //         var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
    //         throw new ApiIdentityException($"Failed to build identity map using {identityRef} for type '{apiObjectType.ApiName}'.");
    //     }

    //     return identityMap;
    // }

    /// <summary>
    ///     Gets identities for a collection of instances.
    /// </summary>
    /// <param name="apiObjectType">The API object type to get identities for.</param>
    /// <param name="instances">The collection of CLR instances to get identities for.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns>A read-only list of identities in the same order as the input instances.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="apiObjectType"/> or <paramref name="instances"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the object type has no identity configured or the specified identity name is not found.</exception>
    /// <exception cref="ApiIdentityException">Thrown when identity building fails for any instance.</exception>
    /// <remarks>
    ///     <para><b>Batch Processing Benefits:</b></para>
    ///     <list type="bullet">
    ///         <item><description>Identity resolution happens once for all instances (reduced overhead)</description></item>
    ///         <item><description>Better error context with batch logging</description></item>
    ///         <item><description>Pre-allocated result collection when count is known</description></item>
    ///     </list>
    ///     <para>This method throws on the first failure. For fault-tolerant processing, use <see cref="TryGetIdentities"/>.</para>
    ///     <para><b>Use Cases:</b></para>
    ///     <list type="bullet">
    ///         <item><description>Bulk entity loading from database with identity extraction</description></item>
    ///         <item><description>Batch API responses requiring identity lists</description></item>
    ///         <item><description>Cache warming with multiple entities</description></item>
    ///     </list>
    /// </remarks>
    // public static IReadOnlyList<ApiId> GetIdentities(this ApiObjectType apiObjectType, IEnumerable<object?> instances, string? apiIdentityName = null)
    // {
    //     ArgumentNullException.ThrowIfNull(apiObjectType);
    //     ArgumentNullException.ThrowIfNull(instances);

    //     var results = apiObjectType.TryGetIdentities(instances, apiIdentityName);
    //     var failures = results.Where(r => !r.Success).ToList();

    //     if (failures.Count > 0)
    //     {
    //         throw new ApiIdentityException($"Failed to build identities for {failures.Count} of {results.Count} instances.");
    //     }

    //     return results.Select(r => r.Id).ToList();
    // }

    /// <summary>
    ///     Gets the primary identity from a CLR instance.
    /// </summary>
    /// <param name="apiObjectType">The API object type to get the identity for.</param>
    /// <param name="clrInstance">The CLR instance to get the identity from.</param>
    /// <returns>The primary identity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clrInstance"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the object type has no primary identity configured.</exception>
    /// <exception cref="ApiIdentityException">Thrown when type coercion fails or null handling requires throwing.</exception>
    /// <remarks>
    ///     This is a convenience method equivalent to calling <see cref="GetIdentity(ApiObjectType, object, string?)"/>
    ///     with <c>apiIdentityName</c> set to <c>null</c>.
    /// </remarks>
    // public static ApiId GetPrimaryIdentity(this ApiObjectType apiObjectType, object clrInstance)
    //     => apiObjectType.GetIdentity(clrInstance, apiIdentityName: null);

    /// <summary>
    ///     Gets an <see cref="ApiProperty"/> by its API name.
    /// </summary>
    /// <param name="apiObjectType">The API object type to search.</param>
    /// <param name="apiName">The API name of the property to retrieve.</param>
    /// <returns>The <see cref="ApiProperty"/> with the specified API name.</returns>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if no property with the specified API name exists in the object type.
    ///     The exception message includes a list of all available property API names.
    /// </exception>
    /// <remarks>
    ///     This method performs a case-sensitive search for the property by its API name.
    ///     Use <see cref="ApiObjectType.TryGetPropertyByApiName"/> if you prefer non-throwing behavior.
    /// </remarks>
    public static ApiProperty GetPropertyByApiName(this ApiObjectType apiObjectType, string apiName)
    {
        if (apiObjectType.TryGetPropertyByApiName(apiName, out var apiProperty))
        {
            return apiProperty!;
        }

        var availablePropertiesByApiName = string.Join(',', apiObjectType.ApiProperties.OrderBy(p => p.ApiName).Select(p => p.ApiName));
        var errorMessage =
            $"{nameof(ApiProperty)} with {nameof(ApiProperty.ApiName)} '{apiName.SafeToString()}' not found in {apiObjectType.SafeToString()}. " +
            $"Available {nameof(ApiProperty)} by {nameof(ApiProperty.ApiName)} are: {availablePropertiesByApiName}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiProperty"/> by its CLR name.
    /// </summary>
    /// <param name="apiObjectType">The API object type to search.</param>
    /// <param name="clrName">The CLR name of the property to retrieve.</param>
    /// <returns>The <see cref="ApiProperty"/> with the specified CLR name.</returns>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if no property with the specified CLR name exists in the object type.
    ///     The exception message includes a list of all available property CLR names.
    /// </exception>
    /// <remarks>
    ///     This method performs a case-sensitive search for the property by its CLR name,
    ///     which corresponds to the actual field or property name in the CLR type.
    ///     Use <see cref="ApiObjectType.TryGetPropertyByClrName"/> if you prefer non-throwing behavior.
    /// </remarks>
    public static ApiProperty GetPropertyByClrName(this ApiObjectType apiObjectType, string clrName)
    {
        if (apiObjectType.TryGetPropertyByClrName(clrName, out var apiProperty))
        {
            return apiProperty!;
        }

        var availablePropertiesByClrName = string.Join(',', apiObjectType.ApiProperties.OrderBy(p => p.ClrName).Select(p => p.ClrName));
        var errorMessage = $"{nameof(ApiProperty)} with {nameof(ApiProperty.ClrName)} '{clrName.SafeToString()}' not found in {apiObjectType.SafeToString()}. " +
            $"Available {nameof(ApiProperty)} by {nameof(ApiProperty.ClrName)} are: {availablePropertiesByClrName}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Gets an <see cref="ApiRelationship"/> by its API name.
    /// </summary>
    /// <param name="apiObjectType">The API object type to search.</param>
    /// <param name="apiName">The API name of the relationship to retrieve.</param>
    /// <returns>The <see cref="ApiRelationship"/> with the specified API name.</returns>
    /// <exception cref="ApiSchemaException">
    ///     Thrown if no relationship with the specified API name exists in the object type.
    ///     The exception message includes a list of all available relationship API names.
    /// </exception>
    /// <remarks>
    ///     This method performs a case-sensitive search for the relationship by its API name.
    ///     Use <see cref="ApiObjectType.TryGetRelationshipByApiName"/> if you prefer non-throwing behavior.
    /// </remarks>
    public static ApiRelationship GetRelationshipByApiName(this ApiObjectType apiObjectType, string apiName)
    {
        if (apiObjectType.TryGetRelationshipByApiName(apiName, out var apiRelationship))
        {
            return apiRelationship!;
        }

        var availableRelationshipsByApiName = string.Join(',', apiObjectType.ApiRelationships.OrderBy(r => r.ApiName).Select(r => r.ApiName));
        var errorMessage = $"{nameof(ApiRelationship)} with {nameof(ApiRelationship.ApiName)} '{apiName.SafeToString()}' not found in {apiObjectType.SafeToString()}. " +
            $"Available {nameof(ApiRelationship)} by {nameof(ApiRelationship.ApiName)} are: {availableRelationshipsByApiName}.";
        throw new ApiSchemaException(errorMessage);
    }

    /// <summary>
    ///     Attempts to get identities for a collection of instances without throwing exceptions.
    /// </summary>
    /// <param name="apiObjectType">The API object type to get identities for.</param>
    /// <param name="instances">The collection of CLR instances to get identities for.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns>A read-only list of <see cref="ApiIdentityBuildResult"/> containing the result for each instance.</returns>
    /// <remarks>
    ///     <para>This method provides fault-tolerant batch processing:</para>
    ///     <list type="bullet">
    ///         <item><description>Never throws exceptions - returns empty list or partial results</description></item>
    ///         <item><description>Null instances are skipped</description></item>
    ///         <item><description>Failed builds are indicated with <c>Success = false</c> and <c>Id = ApiId.None</c></description></item>
    ///         <item><description>Returns instance-identity-success tuples for easy correlation</description></item>
    ///     </list>
    ///     <para><b>Validation Example:</b></para>
    ///     <para><c>var results = objectType.TryBuildIdentities(entities);</c></para>
    ///     <para><c>var failures = results.Where(r => !r.Success).Select(r => r.Instance);</c></para>
    ///     <para><c>var successes = results.Where(r => r.Success);</c></para>
    /// </remarks>
    // public static IReadOnlyList<ApiIdentityBuildResult> TryGetIdentities(this ApiObjectType apiObjectType, IEnumerable<object?> instances, string? apiIdentityName = null)
    // {
    //     ArgumentNullException.ThrowIfNull(apiObjectType);
    //     return apiObjectType.TryGetIdentities(instances, apiIdentityName);
    // }

    /// <summary>
    ///     Attempts to get the primary identity from a CLR instance without throwing exceptions.
    /// </summary>
    /// <param name="apiObjectType">The API object type to get the identity for.</param>
    /// <param name="clrInstance">The CLR instance to get the identity from.</param>
    /// <param name="id">When this method returns, contains the identity if successful; otherwise, <see cref="ApiId.Empty"/>.</param>
    /// <returns><c>true</c> if the identity was retrieved successfully; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     This is a convenience method equivalent to calling <see cref="ApiObjectType.TryGetIdentity(object, out ApiId, string?)"/>
    ///     with <c>apiIdentityName</c> set to <c>null</c>.
    /// </remarks>
    // public static bool TryGetPrimaryIdentity(this ApiObjectType apiObjectType, object clrInstance, out ApiId id)
    //     => apiObjectType.TryGetIdentity(clrInstance, out id, apiIdentityName: null);
    #endregion
}
