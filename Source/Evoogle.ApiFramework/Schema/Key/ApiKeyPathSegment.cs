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
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Key;

/// <summary>
///     Represents a single CLR member navigation step within an <see cref="ApiKeyPath"/>.
/// </summary>
/// <remarks>
///     Each segment holds a CLR member name and, after compilation, a reference to the resolved <see cref="ApiProperty"/>.
///     Segments do not validate whether the resolved property is scalar or object-typed — that responsibility belongs to the parent <see cref="ApiKeyPath"/>, which has positional context (navigation vs. terminal).
/// </remarks>
/// <param name="clrMemberName">The CLR member name for this navigation step.</param>
[JsonConverter(typeof(ApiKeyPathSegmentJsonConverter))]
public sealed class ApiKeyPathSegment(string clrMemberName) : ApiSchemaElement
{
    #region Fields
    private ApiProperty? _apiResolvedProperty = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override ApiSchemaElementKind Kind => ApiSchemaElementKind.KeyPathSegment;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiKeyPathSegment);
    #endregion

    #region ApiKeyPathSegment Properties
    /// <summary>Gets the CLR member name for this navigation step.</summary>
    public string ClrMemberName { get; } = clrMemberName;

    /// <summary>Gets the resolved <see cref="ApiProperty"/> for this segment. Available after compilation.</summary>
    public ApiProperty ApiProperty => this.RequireValue(_apiResolvedProperty);

    /// <summary>
    ///     Gets a value indicating whether the CLR member was successfully resolved to an
    ///     <see cref="ApiProperty"/> during compilation.
    /// </summary>
    internal bool IsPropertyResolved => _apiResolvedProperty is not null;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrMemberName = this.ClrMemberName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiKeyPathSegment)} {{{nameof(this.ClrMemberName)}={clrMemberName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaPathFormatting.BuildPath(apiBasePath: apiPreviousPath, apiPathSegment: this.ApiElementName, apiPathSegmentName: this.ClrMemberName);

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        this.ValidateClrMemberName(context);
        this.ResolveApiProperty(context);
    }
    #endregion

    #region Implementation Methods
    private void ValidateClrMemberName(ApiSchemaCompilationContext context)
    {
        if (!ApiSchemaNameValidation.IsNameInvalid(this.ClrMemberName))
        {
            return;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiKeyPathSegmentInvalidClrMemberName;
        var description = $"{nameof(this.ClrMemberName)} must not be null, empty, or whitespace";
        var remediation = $"Specify a valid {nameof(this.ClrMemberName)} value";

        context.AddIssue(severity, code, description, remediation);
    }

    private void ResolveApiProperty(ApiSchemaCompilationContext context)
    {
        if (ApiSchemaNameValidation.IsNameInvalid(this.ClrMemberName))
        {
            return;
        }

        var apiObjectType = this.GetApiObjectType();
        if (apiObjectType.TryGetPropertyByClrName
        (
            this.ClrMemberName,
            out var apiResolvedProperty
        ))
        {
            _apiResolvedProperty = apiResolvedProperty;
            return;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiKeyPathSegmentUnresolvedApiProperty;
        var description = $"An API property for CLR member '{this.ClrMemberName}' could not be found on object type '{apiObjectType.ApiName}'";
        var remediation = $"Verify the CLR member name or add an API property for CLR member '{this.ClrMemberName}' to '{apiObjectType.ApiName}'";

        context.AddIssue(severity, code, description, remediation);
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
