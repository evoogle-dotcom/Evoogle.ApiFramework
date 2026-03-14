// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents a single enumeration value within an API enumeration type.
/// </summary>
/// <param name="apiName">The API name of the enumeration value (typically used in API requests/responses).</param>
/// <param name="clrName">The CLR name of the enumeration value (corresponding to the C# enum name).</param>
/// <param name="clrOrdinal">The CLR ordinal (integer value) of the enumeration value.</param>
[JsonConverter(typeof(ApiEnumValueJsonConverter))]
public sealed class ApiEnumValue
(
    string apiName,
    string clrName,
    int clrOrdinal
) : ApiSchemaElement
{
    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiEnumValue);
    #endregion

    #region ApiEnumValue Properties
    /// <summary>Gets the API name of the API enumeration value.</summary>
    public string ApiName { get; } = apiName;

    /// <summary>Gets the CLR name of the API enumeration value (matching the C# enum name).</summary>
    public string ClrName { get; } = clrName;

    /// <summary>Gets the CLR ordinal of the API enumeration value (matching the C# enum ordinal value).</summary>
    public int ClrOrdinal { get; } = clrOrdinal;
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var apiName = this.ApiName.SafeToString();
        var clrName = this.ClrName.SafeToString();
        var clrOrdinal = this.ClrOrdinal.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiEnumValue)} {{{nameof(this.ApiName)}={apiName}, {nameof(this.ClrName)}={clrName}, {nameof(this.ClrOrdinal)}={clrOrdinal}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaHelpers.BuildPath(basePath: apiPreviousPath, segment: nameof(ApiEnumValue), segmentName: this.ApiName);

    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiName(context);
        this.InitializeClrName(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiName(ApiInitializationContext context)
    {
        var isApiNameInvalid = ApiSchemaHelpers.IsNameInvalid(this.ApiName);
        if (isApiNameInvalid)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_ENUM_VALUE_INVALID_API_NAME;
            var description = $"{nameof(this.ApiName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeClrName(ApiInitializationContext context)
    {
        var isClrNameInvalid = ApiSchemaHelpers.IsNameInvalid(this.ClrName);
        if (isClrNameInvalid)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_ENUM_VALUE_INVALID_CLR_NAME;
            var description = $"{nameof(this.ClrName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ClrName)} value";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion
}
