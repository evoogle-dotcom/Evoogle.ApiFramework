// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Key.Internal;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.ApiFramework.Schema.Types.Internal;

namespace Evoogle.ApiFramework.Schema.Relationships;

/// <summary>
///     Base class for relationship participants that reference an <see cref="ApiObjectType"/>.
/// </summary>
public abstract class ApiRelationshipElement : ApiSchemaElement, IApiKeyPathRootProvider
{
    #region Fields
    private readonly ApiTypeReferenceBinding<ApiObjectType> _apiObjectTypeBinding;
    #endregion

    #region Properties
    /// <summary>Gets the reference identifying the participating API object type.</summary>
    public ApiTypeReference ApiObjectTypeReference => _apiObjectTypeBinding.ApiTypeReference!;

    /// <summary>Gets the resolved participating API object type after compilation.</summary>
    public ApiObjectType ApiObjectType => _apiObjectTypeBinding.ApiType;

    /// <summary>Gets the participating CLR object type after compilation.</summary>
    public Type ClrObjectType => this.ApiObjectType.ClrType;

    internal ApiObjectType? ApiResolvedObjectType => _apiObjectTypeBinding.ApiResolvedType;
    #endregion

    #region IApiKeyPathRootProvider Properties
    ApiObjectType? IApiKeyPathRootProvider.ApiOwnerSuppliedKeyPathRoot =>
        this.ApiResolvedObjectType;

    string? IApiKeyPathRootProvider.OwnerSuppliedKeyPathRootLabel =>
        _apiObjectTypeBinding.ApiTypeReference?.ApiReferenceLabel;
    #endregion

    #region Constructors
    internal ApiRelationshipElement(ApiTypeReference apiObjectTypeReference)
    {
        _apiObjectTypeBinding = new(apiObjectTypeReference);
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath) =>
        ApiSchemaPathFormatting.BuildPath(apiPreviousPath, this.ApiElementName, null);

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.CompileCore(context);

        if (!_apiObjectTypeBinding.HasReference)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiRelationshipElementNullObjectTypeReference;
            var description = $"{nameof(this.ApiObjectTypeReference)} must not be null";
            var remediation = $"Specify a valid {nameof(this.ApiObjectTypeReference)} value";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        _apiObjectTypeBinding.Resolve
        (
            context,
            ApiSchemaCompilationCode.ApiRelationshipElementUnresolvedObjectType,
            nameof(this.ApiObjectTypeReference),
            nameof(this.ApiObjectType)
        );
    }
    #endregion
}
