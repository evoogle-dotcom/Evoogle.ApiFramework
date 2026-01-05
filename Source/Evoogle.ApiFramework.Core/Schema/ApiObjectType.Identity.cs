// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Globalization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Identity;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema;

public sealed partial class ApiObjectType
{
    #region Public Identity Methods
    /// <summary>
    ///     Attempts to build an identity from a CLR instance without throwing exceptions.
    /// </summary>
    /// <param name="clrInstance">The CLR instance to build the identity from.</param>
    /// <param name="id">When this method returns, contains the built identity if successful; otherwise, <see cref="ApiId.Empty"/>.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns><c>true</c> if the identity was built successfully; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>This method never throws exceptions and returns <c>false</c> on any failure.</para>
    ///     <para>Use <see cref="BuildIdentity(object, string?)"/> if you need exception details.</para>
    ///     <para><b>Performance Characteristics:</b></para>
    ///     <list type="bullet">
    ///         <item><description><b>Property Access:</b> Uses compiled accessors (reflection-free after initialization) for O(1) property reads</description></item>
    ///         <item><description><b>Memory Allocation:</b> Allocates O(n) memory where n = number of identity parts (typically 1-3 parts)</description></item>
    ///         <item><description><b>Type Coercion:</b> Cost depends on source/target type compatibility - primitive conversions are fastest (~10-50ns), string parsing is slower (~100-500ns)</description></item>
    ///         <item><description><b>Typical Performance:</b> Simple identities: ~50-100ns, Composite identities: ~50-200ns per part</description></item>
    ///         <item><description><b>Caching:</b> No built-in caching - results are computed on every call</description></item>
    ///     </list>
    ///     <para><b>When to Use:</b></para>
    ///     <list type="bullet">
    ///         <item><description>Use <c>TryBuildIdentity</c> when failures are expected and should be handled gracefully</description></item>
    ///         <item><description>Use <c>BuildIdentity</c> when failures indicate bugs and you need detailed exception information</description></item>
    ///         <item><description>Consider caching results if calling repeatedly for the same instance</description></item>
    ///     </list>
    /// </remarks>
    public bool TryBuildIdentity(object clrInstance, out ApiId id, string? apiIdentityName = null)
    {
        id = default;

        // Validate inputs without throwing
        if (clrInstance is null)
        {
            this.Logger.LogDebug("TryBuildIdentity failed: clrInstance is null for type '{TypeName}'", this.ApiName);
            return false;
        }

        if (!this.HasIdentity)
        {
            this.Logger.LogDebug("TryBuildIdentity failed: type '{TypeName}' has no identity configured", this.ApiName);
            return false;
        }

        var identity = this.ResolveIdentityForBuild(apiIdentityName);
        if (identity is null)
        {
            var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
            this.Logger.LogDebug("TryBuildIdentity failed: type '{TypeName}' does not have {IdentityRef}", this.ApiName, identityRef);
            return false;
        }

        this.Logger.LogTrace("Building identity '{IdentityName}' for type '{TypeName}' with {PartCount} parts",
            identity.ApiName, this.ApiName, identity.ApiIdentityParts.Length);

        // Core implementation - catch any exceptions from deeper layers
        try
        {
            id = this.BuildIdentityFromInstance(identity, clrInstance);
            var success = id.HasValue;

            if (success)
            {
                this.Logger.LogTrace("Successfully built identity '{IdentityName}' for type '{TypeName}': {Identity}",
                    identity.ApiName, this.ApiName, id);
            }
            else
            {
                this.Logger.LogDebug("TryBuildIdentity returned empty identity for type '{TypeName}' identity '{IdentityName}' (likely null handling)",
                    this.ApiName, identity.ApiName);
            }

            return success;
        }
        catch (Exception ex)
        {
            this.Logger.LogDebug(ex, "TryBuildIdentity failed with exception for type '{TypeName}' identity '{IdentityName}'",
                this.ApiName, identity.ApiName);
            return false;
        }
    }

    /// <summary>
    ///     Attempts to build an identity from a dictionary of property values without throwing exceptions.
    /// </summary>
    /// <param name="values">The dictionary of property names to values.</param>
    /// <param name="id">When this method returns, contains the built identity if successful; otherwise, <see cref="ApiId.Empty"/>.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns><c>true</c> if the identity was built successfully; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>This method never throws exceptions and returns <c>false</c> on any failure.</para>
    ///     <para>Use <see cref="BuildIdentity(IReadOnlyDictionary{string, object?}, string?)"/> if you need exception details.</para>
    ///     <para><b>Performance Characteristics:</b></para>
    ///     <list type="bullet">
    ///         <item><description><b>Dictionary Lookup:</b> O(1) lookups per identity part (typically 1-3 lookups)</description></item>
    ///         <item><description><b>Memory Allocation:</b> Allocates O(n) memory where n = number of identity parts</description></item>
    ///         <item><description><b>Type Coercion:</b> Same as instance-based method - cost depends on type compatibility</description></item>
    ///         <item><description><b>Typical Performance:</b> Similar to instance-based method plus dictionary lookup overhead (~10-20ns per lookup)</description></item>
    ///     </list>
    ///     <para><b>Use Cases:</b></para>
    ///     <list type="bullet">
    ///         <item><description>Building identities from deserialized data (JSON, XML, etc.)</description></item>
    ///         <item><description>Query parameter parsing (HTTP GET requests)</description></item>
    ///         <item><description>Batch operations where property values are pre-extracted</description></item>
    ///     </list>
    /// </remarks>
    public bool TryBuildIdentity(IReadOnlyDictionary<string, object?> values, out ApiId id, string? apiIdentityName = null)
    {
        id = default;

        // Validate inputs without throwing
        if (values is null)
        {
            this.Logger.LogDebug("TryBuildIdentity failed: values dictionary is null for type '{TypeName}'", this.ApiName);
            return false;
        }

        if (!this.HasIdentity)
        {
            this.Logger.LogDebug("TryBuildIdentity failed: type '{TypeName}' has no identity configured", this.ApiName);
            return false;
        }

        var identity = this.ResolveIdentityForBuild(apiIdentityName);
        if (identity is null)
        {
            var identityRef = string.IsNullOrWhiteSpace(apiIdentityName) ? "primary identity" : $"identity '{apiIdentityName}'";
            this.Logger.LogDebug("TryBuildIdentity failed: type '{TypeName}' does not have {IdentityRef}", this.ApiName, identityRef);
            return false;
        }

        this.Logger.LogTrace("Building identity '{IdentityName}' from values dictionary for type '{TypeName}' with {PartCount} parts",
            identity.ApiName, this.ApiName, identity.ApiIdentityParts.Length);

        // Core implementation - catch any exceptions from deeper layers
        try
        {
            id = this.BuildIdentityFromValues(identity, values);
            var success = id.HasValue;

            if (success)
            {
                this.Logger.LogTrace("Successfully built identity '{IdentityName}' from values for type '{TypeName}': {Identity}",
                    identity.ApiName, this.ApiName, id);
            }
            else
            {
                this.Logger.LogDebug("TryBuildIdentity returned empty identity from values for type '{TypeName}' identity '{IdentityName}' (likely null handling)",
                    this.ApiName, identity.ApiName);
            }

            return success;
        }
        catch (Exception ex)
        {
            this.Logger.LogDebug(ex, "TryBuildIdentity failed with exception from values for type '{TypeName}' identity '{IdentityName}'",
                this.ApiName, identity.ApiName);
            return false;
        }
    }

    /// <summary>
    ///     Checks if an instance's identity matches a given <see cref="ApiId"/>.
    /// </summary>
    /// <param name="clrInstance">The CLR instance to check.</param>
    /// <param name="id">The identity to compare against.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns><c>true</c> if the instance's identity matches the given id; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>This is a convenience method that builds the identity and compares it.</para>
    ///     <para>Returns <c>false</c> if identity building fails or if the object type has no identity.</para>
    ///     <para><b>Performance:</b> Same as <see cref="TryBuildIdentity(object, out ApiId, string?)"/> plus equality check (~5-10ns)</para>
    /// </remarks>
    public bool MatchesIdentity(object clrInstance, ApiId id, string? apiIdentityName = null)
    {
        if (clrInstance is null || !id.HasValue)
        {
            this.Logger.LogTrace("MatchesIdentity: clrInstance or id is null/empty");
            return false;
        }

        if (!this.TryBuildIdentity(clrInstance, out var instanceId, apiIdentityName))
        {
            this.Logger.LogTrace("MatchesIdentity: failed to build identity for instance");
            return false;
        }

        var matches = instanceId.Equals(id);
        this.Logger.LogTrace("MatchesIdentity result: {Matches} (instance: {InstanceId}, expected: {ExpectedId})",
            matches, instanceId, id);
        return matches;
    }

    /// <summary>
    ///     Checks if two instances have equal identities.
    /// </summary>
    /// <param name="instance1">The first CLR instance.</param>
    /// <param name="instance2">The second CLR instance.</param>
    /// <param name="apiIdentityName">Optional identity name. If null, uses the primary identity.</param>
    /// <returns><c>true</c> if both instances have the same identity; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     <para>Returns <c>false</c> if either instance is null or if identity building fails for either instance.</para>
    ///     <para><b>Performance:</b> Builds two identities and compares them - roughly 2x the cost of single identity build</para>
    ///     <para><b>Use Cases:</b></para>
    ///     <list type="bullet">
    ///         <item><description>Detecting duplicate entities in collections</description></item>
    ///         <item><description>Change detection (comparing old vs new entity state)</description></item>
    ///         <item><description>Verifying entity equality without full property comparison</description></item>
    ///     </list>
    /// </remarks>
    public bool IdentitiesEqual(object instance1, object instance2, string? apiIdentityName = null)
    {
        if (instance1 is null || instance2 is null)
        {
            this.Logger.LogTrace("IdentitiesEqual: one or both instances are null");
            return false;
        }

        // Optimization: if they're the same reference, they have the same identity
        if (ReferenceEquals(instance1, instance2))
        {
            this.Logger.LogTrace("IdentitiesEqual: same reference, identities are equal");
            return true;
        }

        if (!this.TryBuildIdentity(instance1, out var id1, apiIdentityName))
        {
            this.Logger.LogTrace("IdentitiesEqual: failed to build identity for instance1");
            return false;
        }

        if (!this.TryBuildIdentity(instance2, out var id2, apiIdentityName))
        {
            this.Logger.LogTrace("IdentitiesEqual: failed to build identity for instance2");
            return false;
        }

        var equal = id1.Equals(id2);
        this.Logger.LogTrace("IdentitiesEqual result: {Equal} (id1: {Id1}, id2: {Id2})", equal, id1, id2);
        return equal;
    }
    #endregion

    #region Implementation Methods
    internal ApiIdentity? ResolveIdentityForBuild(string? apiIdentityName)
    {
        if (!string.IsNullOrWhiteSpace(apiIdentityName))
        {
            return this.TryGetIdentityByApiName(apiIdentityName, out var id) ? id : null;
        }

        return this.ApiIdentitySet!.ApiPrimaryIdentity;
    }

    private ApiId BuildIdentityFromInstance(ApiIdentity identity, object clrInstance)
    {
        var parts = new List<ApiIdPart>(identity.ApiIdentityParts.Length);

        foreach (var part in identity.ApiIdentityParts)
        {
            // Get the property value using compiled accessors
            if (!part.ApiProperty.TryGetValue(clrInstance, out var rawValue))
            {
                throw new ApiIdentityException(
                    $"Failed to read property '{part.ApiProperty.ApiName}' from type '{clrInstance.GetType().Name}' for identity '{identity.ApiName}'.");
            }

            // Materialize the ApiId using TypeCoercion and pre-resolved target type
            var partId = this.MaterializeApiIdFromProperty(part, rawValue, identity, clrInstance, identity.ApiSchemaContext);

            // Always create named parts (serialization layer decides ordered vs named formatting)
            parts.Add(ApiIdPart.Create(part.ApiProperty.ApiName, partId));
        }

        return FinalizeComposite(parts);
    }

    private ApiId BuildIdentityFromValues(ApiIdentity identity, IReadOnlyDictionary<string, object?> values)
    {
        var parts = new List<ApiIdPart>(identity.ApiIdentityParts.Length);

        foreach (var part in identity.ApiIdentityParts)
        {
            if (!values.TryGetValue(part.ApiPropertyName, out var rawValue))
            {
                throw new ApiIdentityException(
                    $"Property '{part.ApiPropertyName}' not found in values dictionary for identity '{identity.ApiName}'.");
            }

            // Materialize the ApiId using TypeCoercion and pre-resolved target type
            // Note: We don't have the actual CLR instance type, so error messages will reference the dictionary
            var partId = this.MaterializeApiIdFromPropertyValue(part, rawValue, identity, identity.ApiSchemaContext);

            // Always create named parts (serialization layer decides ordered vs named formatting)
            parts.Add(ApiIdPart.Create(part.ApiPropertyName, partId));
        }

        return FinalizeComposite(parts);
    }

    private ApiId MaterializeApiIdFromProperty(ApiIdentityPart part, object? rawValue, ApiIdentity identity, object clrInstance, ApiSchemaContext schemaContext)
    {
        // Use the pre-resolved target type from the identity part
        var targetType = part.ResolvedTargetType;

        this.Logger.LogTrace("Coercing property '{PropertyName}' from type '{SourceType}' to '{TargetType}' for identity '{IdentityName}'",
            part.ApiProperty.ApiName, rawValue?.GetType().Name ?? "null", targetType.Name, identity.ApiName);

        // Use the schema's TypeCoercion to convert the raw value to the target type
        object? coercedValue;
        try
        {
            coercedValue = schemaContext.TypeCoercion.Coerce(rawValue, targetType, schemaContext.TypeCoercionContext);
        }
        catch (Exception ex)
        {
            this.Logger.LogWarning(ex, "Type coercion failed for property '{PropertyName}' from '{SourceType}' to '{TargetType}'",
                part.ApiProperty.ApiName, rawValue?.GetType().Name ?? "null", targetType.Name);
            throw new ApiIdentityException(
                $"Failed to coerce property '{part.ApiProperty.ApiName}' value to type '{targetType.Name}' for identity '{identity.ApiName}' on type '{clrInstance.GetType().Name}'.",
                ex);
        }

        // Handle null values according to the configured null handling
        if (coercedValue is null)
        {
            var nullHandling = this.GetIdentityNullHandling();
            this.Logger.LogDebug("Property '{PropertyName}' has null value for identity '{IdentityName}', null handling: {NullHandling}",
                part.ApiProperty.ApiName, identity.ApiName, nullHandling);

            if (nullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Property '{part.ApiProperty.ApiName}' has a null value for identity '{identity.ApiName}' on type '{clrInstance.GetType().Name}'. Null values are not allowed with {nameof(ApiIdentityNullHandling.ThrowException)} configured.");
            }

            return ApiId.Empty;
        }

        // Convert the typed value to ApiId
        return this.ConvertToApiId(coercedValue, targetType, part.ApiProperty.ApiName, identity.ApiName, clrInstance.GetType().Name);
    }

    private ApiId MaterializeApiIdFromPropertyValue(ApiIdentityPart part, object? rawValue, ApiIdentity identity, ApiSchemaContext schemaContext)
    {
        // Use the pre-resolved target type from the identity part
        var targetType = part.ResolvedTargetType;

        this.Logger.LogTrace("Coercing property '{PropertyName}' from values dictionary (type '{SourceType}') to '{TargetType}' for identity '{IdentityName}'",
            part.ApiPropertyName, rawValue?.GetType().Name ?? "null", targetType.Name, identity.ApiName);

        // Use the schema's TypeCoercion to convert the raw value to the target type
        object? coercedValue;
        try
        {
            coercedValue = schemaContext.TypeCoercion.Coerce(rawValue, targetType, schemaContext.TypeCoercionContext);
        }
        catch (Exception ex)
        {
            this.Logger.LogWarning(ex, "Type coercion failed for property '{PropertyName}' from values dictionary ('{SourceType}' to '{TargetType}')",
                part.ApiPropertyName, rawValue?.GetType().Name ?? "null", targetType.Name);
            throw new ApiIdentityException(
                $"Failed to coerce property '{part.ApiPropertyName}' value to type '{targetType.Name}' for identity '{identity.ApiName}' from values dictionary.",
                ex);
        }

        // Handle null values according to the configured null handling
        if (coercedValue is null)
        {
            var nullHandling = this.GetIdentityNullHandling();
            this.Logger.LogDebug("Property '{PropertyName}' from values has null value for identity '{IdentityName}', null handling: {NullHandling}",
                part.ApiPropertyName, identity.ApiName, nullHandling);

            if (nullHandling == ApiIdentityNullHandling.ThrowException)
            {
                throw new ApiIdentityException(
                    $"Property '{part.ApiPropertyName}' has a null value for identity '{identity.ApiName}' from values dictionary. Null values are not allowed with {nameof(ApiIdentityNullHandling.ThrowException)} configured.");
            }

            return ApiId.Empty;
        }

        // Convert the typed value to ApiId
        return this.ConvertToApiId(coercedValue, targetType, part.ApiPropertyName, identity.ApiName, "values dictionary");
    }

    private ApiId ConvertToApiId(object value, Type targetType, string propertyName, string identityName, string contextDescription)
    {
        // Handle ApiId passthrough
        if (value is ApiId apiId)
        {
            return apiId;
        }

        // Convert based on the target type
        try
        {
            if (targetType == typeof(int))
            {
                return ApiId.FromInt32((int)value);
            }

            if (targetType == typeof(long))
            {
                return ApiId.FromInt64((long)value);
            }

            if (targetType == typeof(Guid))
            {
                return ApiId.FromGuid((Guid)value);
            }

            if (targetType == typeof(Ulid))
            {
                return ApiId.FromUlid((Ulid)value);
            }

            if (targetType == typeof(CultureInfo))
            {
                return ApiId.FromCulture((CultureInfo)value);
            }

            if (targetType == typeof(string))
            {
                return ApiId.FromString((string)value);
            }

            // Fallback: convert to string
            return ApiId.FromString(value.ToString() ?? string.Empty);
        }
        catch (Exception ex)
        {
            this.Logger.LogWarning(ex, "Identity coercion failed for {Property}", propertyName);
            throw new ApiIdentityException(
                $"Failed to convert property '{propertyName}' value of type '{value.GetType().Name}' to ApiId for identity '{identityName}' on {contextDescription}.",
                ex);
        }
    }

    private static ApiId FinalizeComposite(List<ApiIdPart> parts)
        => parts.Count switch
        {
            0 => ApiId.Empty,
            1 => parts[0].Value,
            _ => ApiId.Composite(parts)
        };

    private ApiIdentityNullHandling GetIdentityNullHandling()
        => this.ApiOptions?.ApiIdentityNullHandling ?? this.ApiSchemaContext.ApiSchemaOptions.ApiIdentityNullHandling;
    #endregion
}
