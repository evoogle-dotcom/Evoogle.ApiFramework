// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a flat, ordered chain of property navigation steps from a root CLR type to a scalar value.
/// </summary>
/// <remarks>
///     <para>
///         An <see cref="ApiKeyPath"/> unifies the concepts of primary-key field declarations and foreign-key
///         field bindings. Both are expressed as an ordered sequence of <see cref="ApiKeyPathSegment"/> instances
///         that navigate from a root object type to a terminal <see cref="ApiScalarType"/> property.
///     </para>
///     <para>
///         The <see cref="ClrRootType"/> property specifies the CLR type from which the navigation chain begins.
///         During schema initialization this is resolved to an <see cref="ApiObjectType"/> via the containing
///         <see cref="ApiSchema"/>'s type registry, and the segment chain is validated against that root object type.
///     </para>
/// </remarks>
/// <param name="clrRootType">The CLR type from which the navigation chain begins.</param>
/// <param name="apiSegments">Ordered <see cref="ApiKeyPathSegment"/> instances from the root type to the terminal scalar property. Must contain at least one segment.</param>
[JsonConverter(typeof(ApiKeyPathJsonConverter))]
public sealed class ApiKeyPath(Type clrRootType, IEnumerable<ApiKeyPathSegment> apiSegments) : ApiSchemaElement
{
    #region Fields
    private ApiObjectType? _apiRootObjectType = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiKeyPath);
    #endregion

    #region ApiKeyPath Properties
    /// <summary>Gets the ordered segment chain, from root navigation step to the terminal scalar step.</summary>
    public ApiKeyPathSegment[] ApiSegments { get; } = [.. apiSegments.EmptyIfNull().Where(x => x is not null)];

    /// <summary>Gets the terminal (scalar) segment — the last element in <see cref="ApiSegments"/>.</summary>
    /// <remarks>This is equivalent to <c>ApiSegments[^1]</c>.</remarks>
    public ApiKeyPathSegment ApiScalarSegment => this.ApiSegments[^1];

    /// <summary>
    ///     Gets the root <see cref="ApiObjectType"/> from which the segment chain begins.
    ///     Available after initialization.
    /// </summary>
    public ApiObjectType ApiRootObjectType => this.ThrowIfNotInitialized(_apiRootObjectType);

    /// <summary>Gets the CLR type from which the navigation chain of this key path begins.</summary>
    public Type ClrRootType { get; } = clrRootType;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var rootClrTypeName = this.ClrRootType.Name.SafeToString();
        var segments = string.Join(".", this.ApiSegments.Select(s => s.ClrPropertyName));
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiKeyPath)} {{{nameof(this.ClrRootType)}={rootClrTypeName}, Segments=\"{segments}\", {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
    {
        var segmentName = $"{this.ClrRootType.Name}.{string.Join(".", this.ApiSegments.Select(s => s.ClrPropertyName))}";
        return ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: segmentName);
    }

    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.ValidateSegmentsNonEmpty(context);
        this.ResolveRootObjectType(context);
    }
    #endregion

    #region Implementation Methods
    private void ResolveRootObjectType(ApiInitializationContext context)
    {
        if (this.ApiSegments.Length == 0)
        {
            return; // Error already reported by ValidateSegmentsNonEmpty.
        }

        if (!context.ApiSchema.TryGetObjectTypeByClrType(this.ClrRootType, out var rootObjectType))
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_KEY_PATH_UNRESOLVED_ROOT_TYPE;
            var description = $"Root CLR type '{this.ClrRootType.Name}' is not registered as an {nameof(ApiObjectType)} in the schema";
            var remediation = $"Add an {nameof(ApiObjectType)} for '{this.ClrRootType.Name}' to the schema, or correct the root CLR type";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        this.InitializeSegmentChain(rootObjectType, context);
    }

    private void ValidateSegmentsNonEmpty(ApiInitializationContext context)
    {
        if (this.ApiSegments.Length > 0)
        {
            return;
        }

        var path = this.ApiPath;
        var severity = ApiInitializationSeverity.Error;
        var code = ApiInitializationCode.API_KEY_PATH_EMPTY_SEGMENTS;
        var description = $"{nameof(this.ApiSegments)} must contain at least one property name";
        var remediation = $"Specify at least one CLR property name when creating an {nameof(ApiKeyPath)}";

        context.AddIssue(path, severity, code, description, remediation);
    }

    private void InitializeSegmentChain(ApiObjectType rootObjectType, ApiInitializationContext context)
    {
        _apiRootObjectType = rootObjectType;

        var currentContext = context
            .WithDeclaringSchemaElement(this)
            .WithDeclaringObjectTypeOnly(rootObjectType);

        for (var i = 0; i < this.ApiSegments.Length; i++)
        {
            var segment = this.ApiSegments[i];
            var isLast = i == this.ApiSegments.Length - 1;

            segment.Initialize(currentContext);

            if (!segment.IsPropertyResolved)
            {
                // Error already reported in segment.Initialize; bail the chain.
                return;
            }

            var apiProperty = segment.ApiProperty;

            if (isLast)
            {
                if (apiProperty.ApiType is not ApiScalarType)
                {
                    var path = segment.ApiPath;
                    var severity = ApiInitializationSeverity.Error;
                    var code = ApiInitializationCode.API_KEY_PATH_SCALAR_SEGMENT_INVALID_TYPE;
                    var description = $"Terminal segment property '{segment.ClrPropertyName}' must resolve to a scalar type; found '{apiProperty.ApiType.GetType().Name}'";
                    var remediation = $"Change the terminal property to a scalar-typed property or remove extra navigation segments";

                    context.AddIssue(path, severity, code, description, remediation);
                }
            }
            else
            {
                if (apiProperty.ApiType is not ApiObjectType nestedObjectType)
                {
                    var path = segment.ApiPath;
                    var severity = ApiInitializationSeverity.Error;
                    var code = ApiInitializationCode.API_KEY_PATH_NAVIGATION_SEGMENT_INVALID_TYPE;
                    var description = $"Navigation segment property '{segment.ClrPropertyName}' must resolve to an object type; found '{apiProperty.ApiType.GetType().Name}'";
                    var remediation = $"Change the navigation property to an object-typed property or restructure the path segments";

                    context.AddIssue(path, severity, code, description, remediation);
                    return;
                }

                currentContext = currentContext
                    .WithDeclaringSchemaElement(segment)
                    .WithDeclaringObjectTypeOnly(nestedObjectType);
            }
        }
    }
    #endregion
}
