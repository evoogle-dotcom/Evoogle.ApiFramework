// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

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
