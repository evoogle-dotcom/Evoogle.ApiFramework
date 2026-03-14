// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents an identity part that sources its value from the identity of a parent object in a parent-child relationship.
/// </summary>
/// <param name="apiIdentityName">The optional explicit name of the identity to use on the parent object type. When <see langword="null"/>, the primary identity of the parent type is used.</param>
public class ApiParentIdentityPart(string? apiIdentityName = null) : ApiIdentityPart
{
    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiParentIdentityPart);
    #endregion

    #region ApiIdentityPart Properties
    /// <inheritdoc/>
    public override ApiIdentityPartKind ApiKind => ApiIdentityPartKind.Parent;
    #endregion

    #region ApiParentIdentityPart Properties
    /// <summary>
    ///     Gets the optional explicit identity name used to select a specific identity on the parent object type.
    ///     When <see langword="null"/>, the primary identity of the parent type is used.
    /// </summary>
    public string? ApiIdentityName { get; } = apiIdentityName;
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        var apiIdentityName = this.ApiIdentityName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiParentIdentityPart)} {{{nameof(this.ApiIdentityName)}={apiIdentityName}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: nameof(ApiParentIdentityPart), null);
    #endregion
}
