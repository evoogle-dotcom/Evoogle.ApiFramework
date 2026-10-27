// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Types;

/// <summary>
///     Represents metadata of an API collection type, which describes a collection of items of a specified API type.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiCollectionType"/> class.
/// </remarks>
/// <param name="apiItemTypeExpression">The API type expression of the items in the collection.</param>
/// <param name="apiItemNullability">Nullability of each collection item.</param>
/// <param name="clrCollectionType">The CLR type representing the collection type (e.g., List&lt;T&gt;).</param>
public sealed class ApiCollectionType
(
    ApiTypeExpression apiItemTypeExpression,
    ApiNullability apiItemNullability,
    Type clrCollectionType
) : ApiType(clrCollectionType)
{
    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiCollectionType);
    #endregion

    #region ApiType Properties
    /// <inheritdoc />
    public override ApiTypeKind ApiKind => ApiTypeKind.Collection;
    #endregion

    #region ApiCollectionType Properties
    /// <summary>
    ///     Gets the resolved API item type (named or inline) associated with this collection.
    /// </summary>
    public ApiType ApiItemType => this.ApiItemTypeExpression.ApiType;

    /// <summary>Gets the nullability of each collection item.</summary>
    public ApiNullability ApiItemNullability { get; } = apiItemNullability;

    internal ApiTypeExpression ApiItemTypeExpression { get; } = apiItemTypeExpression;
    #endregion

    #region ApiCollectionType Computed Properties
    /// <summary>Gets a value indicating whether collection items permit null values.</summary>
    public bool IsItemNullable => this.ApiItemNullability == ApiNullability.Nullable;

    /// <summary>Gets a value indicating whether collection items prohibit null values.</summary>
    public bool IsItemNonNullable => this.ApiItemNullability == ApiNullability.NonNullable;

    internal bool IsItemTypeResolved => this.ApiItemTypeExpression.IsResolved;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiItemTypeExpression = this.ApiItemTypeExpression.SafeToString();
        var apiItemNullability = this.ApiItemNullability.SafeToString();
        var clrType = this.ClrType.SafeToName();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiCollectionType)} {{{nameof(this.ApiItemTypeExpression)}={apiItemTypeExpression}, {nameof(this.ApiItemNullability)}={apiItemNullability}, {nameof(this.ExtensionCount)}={extensionCount}}} [{clrType}]";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    internal override IEnumerable<ApiSchemaElement> GetOwnedElements()
    {
        if (this.ApiItemTypeExpression?.ApiInlineType is ApiType apiInlineType)
        {
            yield return apiInlineType;
        }
    }

    /// <inheritdoc />
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaPathFormatting.BuildPath(apiBasePath: apiPreviousPath, apiPathSegment: this.ApiElementName, apiPathSegmentName: null);

    /// <inheritdoc />
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        this.ValidateApiItemNullability(context);
        this.ValidateApiItemTypeExpression(context);
    }
    #endregion

    #region Implementation Methods
    private void ValidateApiItemTypeExpression(ApiSchemaCompilationContext context)
    {
        if (this.ApiItemTypeExpression is null)
        {
            var apiPath = this.ApiPath;
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiCollectionTypeItemTypeUnspecified;
            var description = $"{nameof(this.ApiItemType)} was not specified";
            var remediation = $"Specify a valid {nameof(this.ApiItemType)}";

            context.AddIssue(apiPath, severity, code, description, remediation);
            return;
        }

        this.ApiItemTypeExpression.ResolveForCollection(context);
    }

    private void ValidateApiItemNullability(ApiSchemaCompilationContext context)
    {
        var jsonEnumFinding = this.GetJsonEnumFinding(nameof(this.ApiItemNullability));
        if (jsonEnumFinding is null && Enum.IsDefined(this.ApiItemNullability))
        {
            return;
        }

        var apiPath = this.ApiPath;
        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiCollectionTypeInvalidApiItemNullability;
        var description = jsonEnumFinding?.DescribeProblem() ?? $"{nameof(this.ApiItemNullability)} is invalid";
        var remediation = $"Specify {ApiNullability.Nullable} or {ApiNullability.NonNullable} for {nameof(this.ApiItemNullability)}";

        context.AddIssue(apiPath, severity, code, description, remediation);
    }
    #endregion
}
