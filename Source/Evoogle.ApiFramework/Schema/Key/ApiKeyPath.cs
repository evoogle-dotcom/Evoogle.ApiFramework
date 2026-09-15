// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Immutable;
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.ApiFramework.Schema.Key.Internal;
using Evoogle.ApiFramework.Schema.Relationships;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.ApiFramework.Schema.Types.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Key;

/// <summary>
///     Represents an ordered CLR member path from an explicit or inferred API object root.
/// </summary>
/// <remarks>
///     <para>
///         A key path always compiles to one <see cref="ApiRootObjectType"/>, which is the
///         <see cref="ApiObjectType"/> from which its <see cref="ApiSegments"/> are navigated.
///         The root can be specified explicitly or inferred from the enclosing schema context.
///     </para>
///     <para>
///         When <see cref="ApiRootObjectTypeReference"/> is specified, the path has an explicit
///         root. During schema compilation, the reference is resolved against the containing
///         schema and must resolve to an <see cref="ApiObjectType"/>. An explicit reference is
///         authoritative: it is resolved even when it identifies the same object type that the
///         enclosing context would infer, and it may identify a different declared object type.
///     </para>
///     <para>
///         When <see cref="ApiRootObjectTypeReference"/> is <see langword="null"/>, compilation
///         infers the root from the nearest enclosing element that provides a key-path root. An
///         <see cref="ApiObjectType"/> provides itself for paths in its named keys. An
///         <see cref="ApiRelationshipElement"/> provides the object type resolved from its
///         participating type reference for paths in its foreign key definitions. The inferred
///         object type is bound directly; no additional schema lookup occurs.
///     </para>
///     <para>
///         An inferred relationship root is available only after the enclosing relationship
///         element resolves its object-type reference. If that reference cannot resolve, the
///         relationship element reports the authoritative diagnostic and the path does not add a
///         duplicate root-resolution diagnostic. If neither an explicit reference nor an
///         enclosing root provider is available, schema compilation reports that no root can be
///         inferred.
///     </para>
///     <para>
///         During JSON serialization, an explicit reference is written under the
///         <see cref="ApiRootObjectType"/> property only when its resolved root differs from the
///         root inferred from the enclosing context. Inferable roots and equivalent explicit roots
///         are omitted. Deserialization therefore treats an omitted root as inferred.
///     </para>
///     <para>
///         After successful schema compilation, <see cref="ApiRootObjectType"/> and
///         <see cref="ClrRootType"/> expose the resolved root regardless of its source. The
///         segment chain is then validated from that root: every non-terminal segment must be an
///         object-typed property, and the terminal segment must be scalar-typed.
///     </para>
/// </remarks>
/// <param name="apiRootObjectTypeReference">
///     An explicit root object-type reference, or <see langword="null"/> to infer the root from
///     the enclosing schema context.
/// </param>
/// <param name="apiSegments">The ordered member-navigation segments.</param>
[JsonConverter(typeof(ApiKeyPathJsonConverter))]
public sealed class ApiKeyPath
(
    ApiTypeReference? apiRootObjectTypeReference,
    IEnumerable<ApiKeyPathSegment> apiSegments
)
    : ApiSchemaElement
{
    #region Fields
    private readonly ApiTypeBinding<ApiObjectType> _apiRootObjectTypeBinding =
        new(apiRootObjectTypeReference);
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override ApiSchemaElementKind Kind => ApiSchemaElementKind.KeyPath;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiKeyPath);
    #endregion

    #region Root Properties
    /// <summary>Gets the resolved root API object type after compilation.</summary>
    public ApiObjectType ApiRootObjectType => _apiRootObjectTypeBinding.ApiType;

    /// <summary>Gets the resolved root CLR type after compilation.</summary>
    public Type ClrRootType => this.ApiRootObjectType.ClrType;

    /// <summary>
    ///     Gets the explicit root object-type reference, or null when the root is inferred.
    /// </summary>
    internal ApiTypeReference? ApiRootObjectTypeReference =>
        _apiRootObjectTypeBinding.ApiTypeReference;

    /// <summary>Gets a value indicating whether an explicit root reference is configured.</summary>
    internal bool HasExplicitRootReference => _apiRootObjectTypeBinding.HasReference;

    /// <summary>Gets a value indicating whether the root is inferred rather than explicitly referenced.</summary>
    internal bool IsInferredRoot => !this.HasExplicitRootReference;

    /// <summary>Gets a value indicating whether the root has been resolved after compilation.</summary>
    internal bool IsRootResolved => _apiRootObjectTypeBinding.IsBound;
    #endregion

    #region Segment Properties
    /// <summary>Gets the immutable ordered segment chain.</summary>
    public ImmutableArray<ApiKeyPathSegment> ApiSegments { get; } =
        [.. apiSegments.EmptyIfNull().Where(static segment => segment is not null)];

    /// <summary>Gets the terminal scalar segment.</summary>
    public ApiKeyPathSegment ApiScalarSegment => this.ApiSegments[^1];
    #endregion

    #region Path Properties
    /// <summary>Gets the dot-delimited CLR member path.</summary>
    public string ClrPath => string.Join
        ('.', this.ApiSegments.Select(static segment => segment.ClrMemberName));

    internal string? ApiPathLabel
    {
        get
        {
            var rootLabel = this.ApiRootObjectTypeReference?.ApiReferenceLabel
                ?? this.GetInferredRootLabel();
            return rootLabel is null ? null : $"{rootLabel}.{this.ClrPath}";
        }
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiRootObjectTypeReference = this.ApiRootObjectTypeReference.SafeToString();
        var apiSegments = string.Join(".", this.ApiSegments.Select(static segment => segment.ClrMemberName));
        var extensionCount = this.ExtensionCount.SafeToString();
        return $"{nameof(ApiKeyPath)} "
            + $"{{{nameof(this.ApiRootObjectTypeReference)}={apiRootObjectTypeReference}, "
            + $"{nameof(this.ApiSegments)}=\"{apiSegments}\", "
            + $"{nameof(this.ExtensionCount)}={extensionCount}}}";
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
    protected override string BuildPath(string? apiPreviousPath) =>
        ApiSchemaPathFormatting.BuildPath(apiPreviousPath, this.ApiElementName, this.ApiPathLabel);

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);
        this.ValidateSegmentsNonEmpty(context);
        this.ResolveRootObjectType(context);
    }
    #endregion

    #region Implementation Methods
    internal Type? GetInferredClrRootType()
    {
        var apiObjectType = this.FindRootProvider()?.RootObjectType;
        if (apiObjectType is null)
        {
            return null;
        }

        return apiObjectType.ClrType;
    }

    private void ResolveRootObjectType(ApiSchemaCompilationContext context)
    {
        if (this.ApiSegments.Length == 0)
        {
            return;
        }

        if (this.ApiRootObjectTypeReference is not null)
        {
            var resolveResult = _apiRootObjectTypeBinding.TryResolveReference
                (
                    context,
                    ApiSchemaCompilationCode.ApiKeyPathUnresolvedRootType,
                    nameof(this.ApiRootObjectTypeReference),
                    nameof(this.ApiRootObjectType)
                );

            if (!resolveResult)
            {
                return;
            }

            this.CompileSegmentChain(_apiRootObjectTypeBinding.ApiType, context);
            return;
        }

        var rootProvider = this.FindRootProvider();
        var inferredRootObjectType = rootProvider?.RootObjectType;
        if (inferredRootObjectType is not null)
        {
            _apiRootObjectTypeBinding.Bind(inferredRootObjectType);
            this.CompileSegmentChain(inferredRootObjectType, context);
            return;
        }

        if (rootProvider is not null)
        {
            // The owning element already reported its unresolved reference.
            return;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiKeyPathUninferableRootType;
        var description = $"{nameof(this.ApiRootObjectTypeReference)} was not specified and no "
            + $"enclosing {nameof(ApiObjectType)} or {nameof(ApiRelationshipElement)} provided an "
            + "inferred root";
        var remediation = $"Specify an explicit {nameof(this.ApiRootObjectTypeReference)} when "
            + $"creating this {nameof(ApiKeyPath)}";

        context.AddIssue(severity, code, description, remediation);
    }

    private IApiKeyPathRootProvider? FindRootProvider()
    {
        if (!this.HasTopology)
        {
            return null;
        }

        for (var enclosingElement = this.Parent;
            enclosingElement is not null;
            enclosingElement = enclosingElement.Parent)
        {
            if (enclosingElement is IApiKeyPathRootProvider rootProvider)
            {
                return rootProvider;
            }
        }

        return null;
    }

    private string? GetInferredRootLabel() => this.FindRootProvider()?.RootLabel;

    private void ValidateSegmentsNonEmpty(ApiSchemaCompilationContext context)
    {
        if (this.ApiSegments.Length > 0)
        {
            return;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiKeyPathEmptySegments;
        var description = $"{nameof(this.ApiSegments)} must contain at least one member name";
        var remediation = $"Specify at least one CLR member name when creating an {nameof(ApiKeyPath)}";

        context.AddIssue(severity, code, description, remediation);
    }

    private void CompileSegmentChain
    (
        ApiObjectType rootObjectType,
        ApiSchemaCompilationContext context
    )
    {
        for (var i = 0; i < this.ApiSegments.Length; i++)
        {
            var segment = this.ApiSegments[i];
            var isLast = i == this.ApiSegments.Length - 1;
            var location = ApiSchemaCompilationLocation.ForIndexedLabel(i, segment.ClrMemberName);
            segment.Compile(context, location);

            if (!segment.IsPropertyResolved)
            {
                return;
            }

            var apiProperty = segment.ApiProperty;
            if (!apiProperty.IsResolved)
            {
                return;
            }

            if (isLast)
            {
                if (apiProperty.ApiType is not ApiScalarType)
                {
                    var apiPath = segment.ApiPath;
                    var severity = ApiSchemaCompilationSeverity.Error;
                    var code = ApiSchemaCompilationCode.ApiKeyPathScalarSegmentInvalidType;
                    var description = $"Terminal segment member '{segment.ClrMemberName}' must resolve to a scalar type; found '{apiProperty.ApiType.GetType().Name}'";
                    var remediation = "Change the terminal member to a scalar-typed member or remove extra navigation segments";

                    context.AddIssue(apiPath, severity, code, description, remediation);
                }
            }
            else if (apiProperty.ApiType is not ApiObjectType)
            {
                var apiPath = segment.ApiPath;
                var severity = ApiSchemaCompilationSeverity.Error;
                var code = ApiSchemaCompilationCode.ApiKeyPathNavigationSegmentInvalidType;
                var description = $"Navigation segment member '{segment.ClrMemberName}' must resolve to an object type; found '{apiProperty.ApiType.GetType().Name}'";
                var remediation = "Change the navigation member to an object-typed member or restructure the path segments";

                context.AddIssue(apiPath, severity, code, description, remediation);
                return;
            }
        }
    }
    #endregion
}
