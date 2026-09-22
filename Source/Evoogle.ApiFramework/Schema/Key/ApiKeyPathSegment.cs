// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.ApiFramework.Schema.Types.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Key;

/// <summary>
///     Represents one API-property navigation step within an <see cref="ApiKeyPath"/>.
/// </summary>
/// <remarks>
///     Each segment holds an API-name or CLR-name property reference. Compilation resolves that
///     identity relative to the current object type. The parent path validates whether the
///     resolved property is valid in its navigation or terminal position.
/// </remarks>
/// <param name="apiPropertyReference">The API property reference for this navigation step.</param>
[JsonConverter(typeof(ApiKeyPathSegmentJsonConverter))]
public sealed class ApiKeyPathSegment(ApiPropertyReference apiPropertyReference) : ApiSchemaElement
{
    #region Fields
    private readonly ApiPropertyBinding _apiPropertyBinding = new
    (
        apiPropertyReference ?? throw new ArgumentNullException(nameof(apiPropertyReference))
    );
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override ApiSchemaElementKind Kind => ApiSchemaElementKind.KeyPathSegment;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiKeyPathSegment);
    #endregion

    #region ApiKeyPathSegment Properties
    /// <summary>Gets the configured API property reference.</summary>
    public ApiPropertyReference ApiPropertyReference => _apiPropertyBinding.ApiPropertyReference!;

    /// <summary>Gets the resolved CLR member name after compilation.</summary>
    public string ClrMemberName => this.ApiProperty.ClrName;

    /// <summary>Gets the resolved <see cref="ApiProperty"/> for this segment. Available after compilation.</summary>
    public ApiProperty ApiProperty => _apiPropertyBinding.ApiProperty;

    /// <summary>
    ///     Gets a value indicating whether the CLR member was successfully resolved to an
    ///     <see cref="ApiProperty"/> during compilation.
    /// </summary>
    internal bool IsPropertyResolved => _apiPropertyBinding.IsBound;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiPropertyReference = this.ApiPropertyReference.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiKeyPathSegment)} "
            + $"{{{nameof(this.ApiPropertyReference)}={apiPropertyReference}, "
            + $"{nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaPathFormatting.BuildPath
        (
            apiBasePath: apiPreviousPath,
            apiPathSegment: this.ApiElementName,
            apiPathSegmentName: this.ApiPropertyReference.ReferenceLabel
        );

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        this.ResolveApiProperty(context);
    }
    #endregion

    #region Implementation Methods
    private void ResolveApiProperty(ApiSchemaCompilationContext context)
    {
        var apiObjectType = this.GetApiObjectType();
        _apiPropertyBinding.TryResolveReference
        (
            apiObjectType,
            context,
            ApiSchemaCompilationCode.ApiKeyPathSegmentUnresolvedApiProperty,
            "Key path property reference"
        );
    }

    private ApiObjectType GetApiObjectType()
    {
        if (this.PreviousSibling is ApiKeyPathSegment precedingSegment)
        {
            return precedingSegment.ApiProperty.ApiType as ApiObjectType
                ?? throw new ApiSchemaException("A key path navigation segment must resolve to an API object type before compiling its next segment.");
        }

        if (this.PreviousSibling is not null)
        {
            throw new ApiSchemaException($"A {nameof(ApiKeyPathSegment)} can only have another {nameof(ApiKeyPathSegment)} as its previous sibling.");
        }

        return (this.Parent as ApiKeyPath)?.ApiRootObjectType
            ?? throw new ApiSchemaException($"A {nameof(ApiKeyPathSegment)} must be owned by an {nameof(ApiKeyPath)}.");
    }
    #endregion
}
