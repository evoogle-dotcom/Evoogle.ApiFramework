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
public abstract class ApiNamedType : ApiType
{
    #region ApiNamedType Properties
    /// <summary>Gets the API name of the API type.</summary>
    public string ApiName { get; }
    #endregion

    #region Constructors
    internal ApiNamedType(string apiName, Type clrType)
        : base(clrType)
    {
        this.ApiName = apiName;
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaPathFormatting.BuildPath(apiBasePath: apiPreviousPath, apiPathSegment: this.ApiElementName, apiPathSegmentName: this.ApiName);

    /// <inheritdoc />
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        this.ValidateApiName(context);
    }
    #endregion

    #region Implementation Methods
    private void ValidateApiName(ApiSchemaCompilationContext context)
    {
        var isApiNameInvalid = ApiSchemaNameValidation.IsNameInvalid(this.ApiName);
        if (isApiNameInvalid)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiNamedTypeInvalidApiName;
            var description = $"{nameof(this.ApiName)} must not be null, empty, or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiName)} value";

            context.AddIssue(severity, code, description, remediation);
        }
    }
    #endregion
}
