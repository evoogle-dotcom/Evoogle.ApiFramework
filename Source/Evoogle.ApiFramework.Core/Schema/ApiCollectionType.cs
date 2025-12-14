// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents metadata of an API collection type, which describes a collection of items of a specified API type.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiCollectionType"/> class.
/// </remarks>
/// <param name="apiItemTypeExpression">The API type expression of the items in the collection.</param>
/// <param name="apiItemTypeModifiers">Modifiers applied to the item type (e.g., Required).</param>
/// <param name="clrCollectionType">The CLR type representing the collection type (e.g., List&lt;T&gt;).</param>
public sealed class ApiCollectionType(ApiTypeExpression apiItemTypeExpression, ApiTypeModifiers apiItemTypeModifiers, Type clrCollectionType) : ApiType(clrCollectionType)
{
    #region ApiType Properties
    /// <inheritdoc />
    public override ApiTypeKind Kind => ApiTypeKind.Collection;

    /// <inheritdoc/>
    protected override string ApiTypeName => nameof(ApiCollectionType);
    #endregion

    #region ApiCollectionType Properties
    /// <summary>
    ///     Gets the resolved API item type (named or inline) associated with this collection.
    /// </summary>
    public ApiType ApiItemType => this.ApiItemTypeExpression.ApiType;

    /// <summary>Gets the modifiers applied to the item type within the collection (e.g., Required).</summary>
    public ApiTypeModifiers ApiItemTypeModifiers { get; } = apiItemTypeModifiers;

    internal ApiTypeExpression ApiItemTypeExpression { get; } = apiItemTypeExpression;
    #endregion

    #region ApiCollectionType Computed Properties
    /// <summary>Gets a value indicating whether this item type is optional (not required).</summary>
    public bool IsItemOptional => !this.ApiItemTypeModifiers.HasFlag(ApiTypeModifiers.Required);

    /// <summary>Gets a value indicating whether this item type is required.</summary>
    public bool IsItemRequired => this.ApiItemTypeModifiers.HasFlag(ApiTypeModifiers.Required);
    #endregion

    #region ApiType Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiParentPath)
        => ApiSchemaHelpers.BuildPath(apiParentPath, apiChildPath: this.ApiTypeName, apiApiName: null);

    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiItemTypeExpression(context);
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiItemTypeExpression = this.ApiItemTypeExpression.SafeToString();
        var apiItemTypeModifiers = this.ApiItemTypeModifiers.SafeToString();
        var clrType = this.ClrType.SafeToName();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiCollectionType)} {{{nameof(this.ApiItemTypeExpression)}={apiItemTypeExpression}, {nameof(this.ApiItemTypeModifiers)}={apiItemTypeModifiers}, {nameof(this.ExtensionCount)}={extensionCount}}} [{clrType}]";
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiItemTypeExpression(ApiInitializationContext context)
    {
        if (this.ApiItemTypeExpression is null)
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiItemType)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_COLLECTION_TYPE_NULL_ITEM_TYPE;
            var description = $"{nameof(this.ApiItemType)} must not be null";
            var remediation = $"Specify a valid {nameof(this.ApiItemType)}";

            context.AddIssue(path, severity, code, description, remediation);
            return;
        }

        var childContext = context.WithParentSchemaElement(this);
        this.ApiItemTypeExpression.InitializeForCollection(childContext);
    }
    #endregion
}
