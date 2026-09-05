// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Immutable;
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
///         During schema compilation this is resolved to an <see cref="ApiObjectType"/> via the containing
///         <see cref="ApiSchema"/>'s type registry, and the segment chain is validated against that root object type.
///     </para>
///     <para>
///         When <paramref name="clrRootType"/> is <see langword="null"/>, it is inferred during compilation
///         from the owning <see cref="ApiObjectType"/> (for a named key type) or the owning
///         <see cref="ApiRelationshipElement"/> (for a foreign key type, e.g. an
///         <see cref="ApiRelationshipDependentEnd"/> or <see cref="ApiRelationshipAssociation"/>).
///     </para>
/// </remarks>
/// <param name="clrRootType">
///     The CLR type from which the navigation chain begins, or <see langword="null"/> to infer it from the
///     owning <see cref="ApiObjectType"/> or <see cref="ApiRelationshipElement"/> during compilation.
/// </param>
/// <param name="apiSegments">Ordered <see cref="ApiKeyPathSegment"/> instances from the root type to the terminal scalar property. Must contain at least one segment.</param>
[JsonConverter(typeof(ApiKeyPathJsonConverter))]
public sealed class ApiKeyPath(Type? clrRootType, IEnumerable<ApiKeyPathSegment> apiSegments) : ApiSchemaElement
{
    #region Fields
    private ApiObjectType? _apiRootObjectType = null;
    private Type? _clrRootType = clrRootType;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override ApiSchemaElementKind Kind => ApiSchemaElementKind.KeyPath;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiKeyPath);
    #endregion

    #region ApiKeyPath Properties
    /// <summary>Gets the immutable ordered segment chain from root to terminal scalar step.</summary>
    public ImmutableArray<ApiKeyPathSegment> ApiSegments { get; } =
        [.. apiSegments.EmptyIfNull().Where(x => x is not null)];

    /// <summary>Gets the terminal (scalar) segment — the last element in <see cref="ApiSegments"/>.</summary>
    /// <remarks>This is equivalent to <c>ApiSegments[^1]</c>.</remarks>
    public ApiKeyPathSegment ApiScalarSegment => this.ApiSegments[^1];

    /// <summary>
    ///     Gets the root <see cref="ApiObjectType"/> from which the segment chain begins.
    ///     Available after compilation.
    /// </summary>
    public ApiObjectType ApiRootObjectType => this.RequireValue(_apiRootObjectType);

    /// <summary>
    ///     Gets the CLR type from which the navigation chain of this key path begins.
    ///     Available immediately when supplied explicitly; otherwise available after compilation.
    /// </summary>
    public Type ClrRootType => this.RequireValue(_clrRootType);

    /// <summary>Gets the dot-delimited CLR property path represented by <see cref="ApiSegments"/>.</summary>
    public string ClrPath => string.Join('.', this.ApiSegments.Select(static segment => segment.ClrPropertyName));

    internal string? ApiPathLabel
    {
        get
        {
            // this.Parent is available before CompileCore backfills _clrRootType, so the path
            // label must resolve the same effective type here to stay stable once compiled.
            var effectiveClrRootType = _clrRootType ?? this.GetOwningDefaultClrRootType();
            return effectiveClrRootType is null
                ? null
                : $"{effectiveClrRootType.Name}." + string.Join(".", this.ApiSegments.Select(s => s.ClrPropertyName));
        }
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrRootTypeName = _clrRootType.SafeToName();
        var apiSegments = string.Join(".", this.ApiSegments.Select(s => s.ClrPropertyName));
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiKeyPath)} {{{nameof(this.ClrRootType)}={clrRootTypeName}, {nameof(this.ApiSegments)}=\"{apiSegments}\", {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override IEnumerable<ApiSchemaElement> GetOwnedElements()
    {
        foreach (var apiSegment in this.ApiSegments)
        {
            yield return apiSegment;
        }
    }

    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
    {
        return ApiSchemaPathFormatting.BuildPath
        (
            apiBasePath: apiPreviousPath,
            apiPathSegment: this.ApiElementName,
            apiPathSegmentName: this.ApiPathLabel
        );
    }

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        this.ValidateSegmentsNonEmpty(context);
        this.ResolveRootObjectType(context);
    }

    /// <summary>Backfills an omitted <see cref="ClrRootType"/> immediately after JSON deserialization, without requiring compilation.</summary>
    internal void EnsureClrRootType(Type defaultClrRootType)
    {
        ArgumentNullException.ThrowIfNull(defaultClrRootType);

        _clrRootType ??= defaultClrRootType;
    }
    #endregion

    #region Implementation Methods
    private void ResolveRootObjectType(ApiSchemaCompilationContext context)
    {
        if (this.ApiSegments.Length == 0)
        {
            return; // Error already reported by ValidateSegmentsNonEmpty.
        }

        var effectiveClrRootType = _clrRootType ?? this.GetOwningDefaultClrRootType();
        if (effectiveClrRootType is null)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiKeyPathUninferableRootType;
            var description = $"{nameof(this.ClrRootType)} was not specified and no owning {nameof(ApiObjectType)} or {nameof(ApiRelationshipElement)} could supply a default";
            var remediation = $"Specify an explicit {nameof(this.ClrRootType)} when creating this {nameof(ApiKeyPath)}";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        _clrRootType = effectiveClrRootType;

        var rootObjectType = this.GetOwningObjectType(effectiveClrRootType);
        if (rootObjectType is null &&
            !context.ApiSchema.TryGetObjectTypeByClrType(effectiveClrRootType, out rootObjectType))
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiKeyPathUnresolvedRootType;
            var description = $"Root CLR type '{effectiveClrRootType.Name}' is not registered as an {nameof(ApiObjectType)} in the schema";
            var remediation = $"Add an {nameof(ApiObjectType)} for '{effectiveClrRootType.Name}' to the schema, or correct the root CLR type";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        this.CompileSegmentChain(rootObjectType, context);
    }

    private ApiObjectType? GetOwningObjectType(Type effectiveClrRootType)
    {
        var apiObjectType = (this.Parent as ApiNamedKeyType)?.Parent as ApiObjectType;
        return apiObjectType?.ClrType == effectiveClrRootType ? apiObjectType : null;
    }

    /// <summary>Infers the default root CLR type from the owning ApiObjectType or ApiRelationshipElement.</summary>
    internal Type? GetOwningDefaultClrRootType()
    {
        if (!this.HasTopology)
        {
            return null;
        }

        return this.Parent switch
        {
            ApiNamedKeyType { Parent: ApiObjectType apiObjectType } => apiObjectType.ClrType,
            ApiKeyType { Parent: ApiRelationshipElement apiRelationshipElement } => apiRelationshipElement.ClrObjectType,
            _ => null,
        };
    }

    private void ValidateSegmentsNonEmpty(ApiSchemaCompilationContext context)
    {
        if (this.ApiSegments.Length > 0)
        {
            return;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiKeyPathEmptySegments;
        var description = $"{nameof(this.ApiSegments)} must contain at least one property name";
        var remediation = $"Specify at least one CLR property name when creating an {nameof(ApiKeyPath)}";

        context.AddIssue(severity, code, description, remediation);
    }

    private void CompileSegmentChain(ApiObjectType rootObjectType, ApiSchemaCompilationContext context)
    {
        _apiRootObjectType = rootObjectType;

        for (var i = 0; i < this.ApiSegments.Length; i++)
        {
            var segment = this.ApiSegments[i];
            var isLast = i == this.ApiSegments.Length - 1;

            var location = ApiSchemaCompilationLocation.ForIndexedLabel
            (
                i,
                segment.ClrPropertyName
            );
            segment.Compile(context, location);

            if (!segment.IsPropertyResolved)
            {
                // Error already reported in segment.Compile; bail the chain.
                return;
            }

            var apiProperty = segment.ApiProperty;

            if (!apiProperty.IsResolved)
            {
                // The property's type expression already reported an compilation issue.
                return;
            }

            if (isLast)
            {
                if (apiProperty.ApiType is not ApiScalarType)
                {
                    var path = segment.ApiPath;
                    var severity = ApiSchemaCompilationSeverity.Error;
                    var code = ApiSchemaCompilationCode.ApiKeyPathScalarSegmentInvalidType;
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
                    var severity = ApiSchemaCompilationSeverity.Error;
                    var code = ApiSchemaCompilationCode.ApiKeyPathNavigationSegmentInvalidType;
                    var description = $"Navigation segment property '{segment.ClrPropertyName}' must resolve to an object type; found '{apiProperty.ApiType.GetType().Name}'";
                    var remediation = $"Change the navigation property to an object-typed property or restructure the path segments";

                    context.AddIssue(path, severity, code, description, remediation);
                    return;
                }
            }
        }
    }
    #endregion
}
