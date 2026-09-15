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
///     Represents an ordered CLR member path from an explicit or owner-supplied API object root.
/// </summary>
/// <remarks>
///     <para>
///         A key path always compiles to one <see cref="ApiRootObjectType"/>, which is the
///         <see cref="ApiObjectType"/> from which its <see cref="ApiSegments"/> are navigated.
///         The root can originate from one of two mutually exclusive sources.
///     </para>
///     <para>
///         When <see cref="ApiRootTypeReference"/> is specified, the path has an explicit root.
///         During schema compilation, the reference is resolved against the containing schema and
///         must resolve to an <see cref="ApiObjectType"/>. An explicit reference is authoritative:
///         it is resolved even when it identifies the same object type that structurally owns the
///         path, and it may identify a different declared object type.
///     </para>
///     <para>
///         When <see cref="ApiRootTypeReference"/> is <see langword="null"/>, the path has an
///         owner-supplied root. During compilation, the path walks its ownership topology to the
///         nearest element that supplies a key-path root. An <see cref="ApiObjectType"/> supplies
///         itself for paths in its named keys. An <see cref="ApiRelationshipElement"/> supplies
///         the object type resolved from its participating type reference for paths in its foreign
///         key definitions. The supplied object type is bound directly; no additional schema
///         lookup occurs.
///     </para>
///     <para>
///         An owner-supplied relationship root is available only after its owning relationship
///         element resolves its object-type reference. If that reference cannot resolve, the
///         relationship element reports the authoritative diagnostic and the path does not add a
///         duplicate root-resolution diagnostic. If neither an explicit reference nor a
///         root-supplying owner is available, schema compilation reports that no root is
///         available.
///     </para>
///     <para>
///         After successful schema compilation, <see cref="ApiRootObjectType"/> and
///         <see cref="ClrRootType"/> expose the resolved root regardless of its source. The
///         segment chain is then validated from that root: every non-terminal segment must be an
///         object-typed property, and the terminal segment must be scalar-typed.
///     </para>
/// </remarks>
/// <param name="apiRootTypeReference">
///     An explicit root type reference, or <see langword="null"/> to obtain the root from the owner.
/// </param>
/// <param name="apiSegments">The ordered member-navigation segments.</param>
[JsonConverter(typeof(ApiKeyPathJsonConverter))]
public sealed class ApiKeyPath(ApiTypeReference? apiRootTypeReference, IEnumerable<ApiKeyPathSegment> apiSegments)
    : ApiSchemaElement
{
    #region Fields
    private readonly ApiTypeReferenceBinding<ApiObjectType> _apiRootTypeBinding = new(apiRootTypeReference);
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override ApiSchemaElementKind Kind => ApiSchemaElementKind.KeyPath;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiKeyPath);
    #endregion

    #region Root Properties
    /// <summary>Gets the resolved root API object type after compilation.</summary>
    public ApiObjectType ApiRootObjectType => _apiRootTypeBinding.ApiType;

    /// <summary>Gets the resolved root CLR type after compilation.</summary>
    public Type ClrRootType => this.ApiRootObjectType.ClrType;

    /// <summary>Gets the explicit root reference, or null when the root is owner-supplied.</summary>
    internal ApiTypeReference? ApiRootTypeReference => _apiRootTypeBinding.ApiTypeReference;

    /// <summary>Gets a value indicating whether the root is supplied by the owner rather than explicitly.</summary>
    internal bool IsOwnerSuppliedRoot => !_apiRootTypeBinding.HasReference;

    /// <summary>Gets a value indicating whether the root has been resolved after compilation.</summary>
    internal bool IsResolvedRoot => _apiRootTypeBinding.IsResolved;
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
            var rootLabel = this.ApiRootTypeReference?.ApiReferenceLabel ?? this.GetOwnerSuppliedRootLabel();
            return rootLabel is null ? null : $"{rootLabel}.{this.ClrPath}";
        }
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiRootTypeReference = this.ApiRootTypeReference.SafeToString();
        var apiSegments = string.Join(".", this.ApiSegments.Select(static segment => segment.ClrMemberName));
        var extensionCount = this.ExtensionCount.SafeToString();
        return $"{nameof(ApiKeyPath)} "
            + $"{{{nameof(this.ApiRootTypeReference)}={apiRootTypeReference}, "
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
    internal Type? GetOwnerSuppliedClrRootType()
    {
        var apiObjectType = this.GetOwnerSuppliedRootProvider()?.ApiOwnerSuppliedKeyPathRoot;
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

        if (this.ApiRootTypeReference is not null)
        {
            var resolveResult = _apiRootTypeBinding.Resolve
                (
                    context,
                    ApiSchemaCompilationCode.ApiKeyPathUnresolvedRootType,
                    nameof(this.ApiRootTypeReference),
                    nameof(this.ApiRootObjectType)
                );

            if (!resolveResult)
            {
                return;
            }

            this.CompileSegmentChain(_apiRootTypeBinding.ApiType, context);
            return;
        }

        var ownerSuppliedRootProvider = this.GetOwnerSuppliedRootProvider();
        var ownerSuppliedRootApiObjectType = ownerSuppliedRootProvider?.ApiOwnerSuppliedKeyPathRoot;
        if (ownerSuppliedRootApiObjectType is not null)
        {
            _apiRootTypeBinding.Bind(ownerSuppliedRootApiObjectType);
            this.CompileSegmentChain(ownerSuppliedRootApiObjectType, context);
            return;
        }

        if (ownerSuppliedRootProvider is not null)
        {
            // The owning element already reported its unresolved reference.
            return;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiKeyPathUninferableRootType;
        var description = $"{nameof(this.ApiRootTypeReference)} was not specified and no owning "
            + $"{nameof(ApiObjectType)} or {nameof(ApiRelationshipElement)} supplied a root";
        var remediation = $"Specify an explicit {nameof(this.ApiRootTypeReference)} when creating this "
            + $"{nameof(ApiKeyPath)}";

        context.AddIssue(severity, code, description, remediation);
    }

    private IApiKeyPathRootProvider? GetOwnerSuppliedRootProvider()
    {
        if (!this.HasTopology)
        {
            return null;
        }

        for (var owner = this.Parent; owner is not null; owner = owner.Parent)
        {
            if (owner is IApiKeyPathRootProvider ownerSuppliedRootProvider)
            {
                return ownerSuppliedRootProvider;
            }
        }

        return null;
    }

    private string? GetOwnerSuppliedRootLabel() =>
        this.GetOwnerSuppliedRootProvider()?.OwnerSuppliedKeyPathRootLabel;

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
                    var description = $"Terminal segment member '{segment.ClrMemberName}' must resolve to a "
                        + $"scalar type; found '{apiProperty.ApiType.GetType().Name}'";
                    var remediation = "Change the terminal member to a scalar-typed member or remove extra "
                        + "navigation segments";

                    context.AddIssue(apiPath, severity, code, description, remediation);
                }
            }
            else if (apiProperty.ApiType is not ApiObjectType)
            {
                var apiPath = segment.ApiPath;
                var severity = ApiSchemaCompilationSeverity.Error;
                var code = ApiSchemaCompilationCode.ApiKeyPathNavigationSegmentInvalidType;
                var description = $"Navigation segment member '{segment.ClrMemberName}' must resolve to an "
                    + $"object type; found '{apiProperty.ApiType.GetType().Name}'";
                var remediation = "Change the navigation member to an object-typed member or restructure the "
                    + "path segments";

                context.AddIssue(apiPath, severity, code, description, remediation);
                return;
            }
        }
    }
    #endregion
}
