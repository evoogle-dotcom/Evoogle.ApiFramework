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
///     and resolves the corresponding <see cref="ApiObjectType"/> during initialization.
/// </summary>
/// <param name="clrObjectType">
///     The CLR type of the <see cref="ApiObjectType"/> this element participates as.
/// </param>
public abstract class ApiRelationshipElement(Type clrObjectType) : ApiSchemaElement
{
    #region ApiRelationshipElement Fields
    private ApiObjectType? _apiResolvedObjectType = null;
    #endregion

    #region ApiRelationshipElement Properties
    /// <summary>Gets the CLR type that identifies the participating <see cref="ApiObjectType"/>.</summary>
    public Type ClrObjectType { get; } = clrObjectType;

    /// <summary>
    ///     Gets the resolved <see cref="ApiObjectType"/> that corresponds to <see cref="ClrObjectType"/>.
    ///     Available after schema initialization. Throws if accessed before initialization completes.
    /// </summary>
    public ApiObjectType ApiObjectType => this.ThrowIfNotInitialized(_apiResolvedObjectType);

    /// <summary>
    ///     Gets the resolved <see cref="ApiObjectType"/>, or <see langword="null"/> if initialization
    ///     has not yet run or failed to resolve the object type.
    /// </summary>
    internal ApiObjectType? ApiResolvedObjectType => _apiResolvedObjectType;
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaPathFormatting.BuildPath(basePath: apiPreviousPath, segment: this.ApiElementName, segmentName: null);

    /// <inheritdoc/>
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeClrObjectType(context);
        this.InitializeApiObjectType(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeClrObjectType(ApiInitializationContext context)
    {
        if (this.ClrObjectType is not null)
        {
            return;
        }

        var path = this.ApiPath;
        var severity = ApiInitializationSeverity.Error;
        var code = ApiInitializationCode.API_RELATIONSHIP_ELEMENT_NULL_CLR_OBJECT_TYPE;
        var description = $"{nameof(this.ClrObjectType)} must not be null";
        var remediation = $"Specify a valid {nameof(this.ClrObjectType)} value";

        context.AddIssue(path, severity, code, description, remediation);
    }

    private void InitializeApiObjectType(ApiInitializationContext context)
    {
        _apiResolvedObjectType = null;

        if (this.ClrObjectType is null)
        {
            return;
        }

        if (context.ApiSchema.TryGetObjectTypeByClrType(this.ClrObjectType, out var apiObjectType))
        {
            _apiResolvedObjectType = apiObjectType;
            return;
        }

        var path = this.ApiPath;
        var severity = ApiInitializationSeverity.Error;
        var code = ApiInitializationCode.API_RELATIONSHIP_ELEMENT_UNRESOLVED_OBJECT_TYPE;
        var description = $"No {nameof(Schema.ApiObjectType)} is registered for CLR type '{this.ClrObjectType.FullName}'";
        var availableTypes = string.Join(", ", context.ApiSchema.ApiObjectTypes.Select(t => $"'{t.ApiName}' ({t.ClrType.Name})"));
        var remediation = !string.IsNullOrEmpty(availableTypes)
            ? $"Use one of the available object types: {availableTypes}"
            : $"Define an {nameof(Schema.ApiObjectType)} for CLR type '{this.ClrObjectType.FullName}' in the schema";

        context.AddIssue(path, severity, code, description, remediation);
    }
    #endregion
}
