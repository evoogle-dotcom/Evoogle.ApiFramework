// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Internal;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents the base class for API types that are identified by a unique API name.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ApiNamedType"/> class.
/// </remarks>
/// <param name="apiName">The unique API name of the type.</param>
/// <param name="clrType">The underlying CLR type this API type maps to.</param>
public abstract class ApiNamedType(string apiName, Type clrType) : ApiType()
{
    #region ApiType Properties
    /// <inheritdoc />
    public override Type ClrType { get; } = clrType;
    #endregion

    #region ApiNamedType Properties
    /// <summary>Gets the API name of the API type.</summary>
    public string ApiName { get; } = apiName;
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiParentPath)
        => ApiSchemaHelpers.BuildPath(apiParentPath, apiChildPath: this.ApiTypeName, apiApiName: this.ApiName);

    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeApiName(context);
        this.InitializeClrType(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeApiName(ApiInitializationContext context)
    {
        if (string.IsNullOrWhiteSpace(this.ApiName))
        {
            var path = $"{this.ApiPath}.{nameof(this.ApiName)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_NAMED_TYPE_INVALID_API_NAME;
            var description = $"{nameof(this.ApiName)} cannot be null, empty, or whitespace";
            var remediation = $"Provide a valid {nameof(this.ApiName)}";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }

    private void InitializeClrType(ApiInitializationContext context)
    {
        if (this.ClrType is null)
        {
            var path = $"{this.ApiPath}.{nameof(this.ClrType)}";
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_NAMED_TYPE_NULL_CLR_TYPE;
            var description = $"{nameof(this.ClrType)} cannot be null";
            var remediation = $"Provide a valid {nameof(this.ClrType)}";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion
}
