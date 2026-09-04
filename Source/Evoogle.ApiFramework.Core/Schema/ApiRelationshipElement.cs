// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Abstract base class for all typed participants in an <see cref="ApiRelationship"/>:
///     <see cref="ApiRelationshipEnd"/> subclasses (principal and dependent ends)
///     and <see cref="ApiRelationshipAssociation"/>.
///     Holds the <see cref="ClrObjectType"/> that identifies the participating CLR type
///     and resolves the corresponding <see cref="ApiObjectType"/> during schema compilation.
/// </summary>
public abstract class ApiRelationshipElement : ApiSchemaElement
{
    #region ApiRelationshipElement Fields
    private ApiObjectType? _apiResolvedObjectType = null;
    #endregion

    #region ApiRelationshipElement Properties
    /// <summary>Gets the CLR type that identifies the participating <see cref="ApiObjectType"/>.</summary>
    public Type ClrObjectType { get; }

    /// <summary>
    ///     Gets the resolved <see cref="ApiObjectType"/> that corresponds to <see cref="ClrObjectType"/>.
    ///     Available after schema compilation. Throws if accessed before compilation completes.
    /// </summary>
    public ApiObjectType ApiObjectType => this.RequireValue(_apiResolvedObjectType);

    /// <summary>
    ///     Gets the resolved <see cref="ApiObjectType"/>, or <see langword="null"/> if compilation
    ///     has not yet run or failed to resolve the object type.
    /// </summary>
    internal ApiObjectType? ApiResolvedObjectType => _apiResolvedObjectType;
    #endregion

    #region Constructors
    internal ApiRelationshipElement(Type clrObjectType)
    {
        this.ClrObjectType = clrObjectType;
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaPathFormatting.BuildPath(apiBasePath: apiPreviousPath, apiPathSegment: this.ApiElementName, apiPathSegmentName: null);

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        this.ValidateClrObjectType(context);
        this.ResolveApiObjectType(context);
    }
    #endregion

    #region Implementation Methods
    private void ValidateClrObjectType(ApiSchemaCompilationContext context)
    {
        if (this.ClrObjectType is not null)
        {
            return;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiRelationshipElementNullClrObjectType;
        var description = $"{nameof(this.ClrObjectType)} must not be null";
        var remediation = $"Specify a valid {nameof(this.ClrObjectType)} value";

        context.AddIssue(severity, code, description, remediation);
    }

    private void ResolveApiObjectType(ApiSchemaCompilationContext context)
    {
        if (this.ClrObjectType is null)
        {
            return;
        }

        if (context.ApiSchema.TryGetObjectTypeByClrType(this.ClrObjectType, out var apiObjectType))
        {
            _apiResolvedObjectType = apiObjectType;
            return;
        }

        var severity = ApiSchemaCompilationSeverity.Error;
        var code = ApiSchemaCompilationCode.ApiRelationshipElementUnresolvedObjectType;
        var description = $"No {nameof(Schema.ApiObjectType)} is registered for CLR type '{this.ClrObjectType.FullName}'";
        var availableTypes = string.Join(", ", context.ApiSchema.ApiObjectTypes.Select(t => $"'{t.ApiName}' ({t.ClrType.Name})"));
        var remediation = !string.IsNullOrEmpty(availableTypes)
            ? $"Use one of the available object types: {availableTypes}"
            : $"Define an {nameof(Schema.ApiObjectType)} for CLR type '{this.ClrObjectType.FullName}' in the schema";

        context.AddIssue(severity, code, description, remediation);
    }
    #endregion
}
