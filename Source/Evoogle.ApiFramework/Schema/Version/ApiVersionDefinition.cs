// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Compilation.Internal;
using Evoogle.ApiFramework.Schema.Json;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.ApiFramework.Schema.Types.Internal;
using Evoogle.ApiFramework.Version;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Version;

/// <summary>
///     Describes the single scalar version token associated with an <see cref="ApiObjectType"/>.
/// </summary>
/// <remarks>
///     A property-backed version is configured with an API property reference and can be materialized
///     from an object instance. A repository-backed version is configured with a CLR type and
///     must be materialized from a value supplied by the repository.
/// </remarks>
[JsonConverter(typeof(ApiVersionDefinitionJsonConverter))]
public sealed class ApiVersionDefinition : ApiSchemaElement
{
    #region Types
    private enum VersionSourceKind
    {
        Property,
        Repository
    }
    #endregion

    #region Fields
    private readonly VersionSourceKind _sourceKind;
    private readonly Type? _clrRepositoryType;
    private readonly ApiPropertyBinding _apiPropertyBinding;
    private ApiScalarType? _apiResolvedScalarType = null;
    #endregion

    #region Constructors
    /// <summary>Initializes a property-backed version definition.</summary>
    /// <param name="apiPropertyReference">The property that supplies the version.</param>
    public ApiVersionDefinition(ApiPropertyReference apiPropertyReference)
    {
        ArgumentNullException.ThrowIfNull(apiPropertyReference);
        _sourceKind = VersionSourceKind.Property;
        _apiPropertyBinding = new(apiPropertyReference);
    }

    /// <summary>Initializes a repository-backed version definition.</summary>
    /// <param name="clrType">The exact CLR type of the repository-supplied version value.</param>
    public ApiVersionDefinition(Type clrType)
    {
        _sourceKind = VersionSourceKind.Repository;
        _clrRepositoryType = clrType;
        _apiPropertyBinding = new(apiPropertyReference: null);
    }
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override ApiSchemaElementKind Kind => ApiSchemaElementKind.VersionDefinition;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiVersionDefinition);
    #endregion

    #region Properties
    /// <summary>
    ///     Gets the property that supplies the version for a property-backed definition.
    /// </summary>
    /// <exception cref="ApiSchemaException">The schema has not compiled successfully.</exception>
    public ApiProperty? ApiProperty
    {
        get
        {
            _ = this.ApiScalarType;
            return _apiPropertyBinding.BoundApiProperty;
        }
    }

    /// <summary>Gets the scalar schema type that represents the version value.</summary>
    /// <exception cref="ApiSchemaException">The schema has not compiled successfully.</exception>
    public ApiScalarType ApiScalarType => this.RequireValue(_apiResolvedScalarType);

    /// <summary>Gets the property reference, or null for a repository-backed version.</summary>
    public ApiPropertyReference? ApiPropertyReference => _apiPropertyBinding.ApiPropertyReference;

    /// <summary>Gets the exact CLR type used to represent the version.</summary>
    /// <exception cref="ApiSchemaException">The schema has not compiled successfully.</exception>
    public Type ClrType => this.ApiScalarType.ClrType;

    internal Type? ClrRepositoryType => _clrRepositoryType;
    #endregion

    #region Computed Properties
    /// <summary>Gets whether the version is materialized from an API property.</summary>
    public bool IsPropertyBacked => _sourceKind == VersionSourceKind.Property;

    /// <summary>Gets whether the version is supplied directly by a repository.</summary>
    public bool IsRepositoryBacked => _sourceKind == VersionSourceKind.Repository;
    #endregion

    #region Materialization Methods
    /// <summary>Materializes a property-backed version from a CLR object instance.</summary>
    /// <param name="clrObject">The object instance containing the version property.</param>
    /// <returns>The materialized version.</returns>
    /// <exception cref="ApiSchemaMaterializationException">
    ///     The definition is repository-backed, the object is incompatible, or the property
    ///     supplies a null or invalid value.
    /// </exception>
    public ApiVersion MaterializeVersion(object clrObject)
    {
        ArgumentNullException.ThrowIfNull(clrObject);

        var apiProperty = this.ApiProperty ?? throw new ApiSchemaMaterializationException
            (
                $"{nameof(ApiVersionDefinition)} is repository-backed and cannot materialize a version from an object instance."
            );
        var apiObjectType = this.GetApiObjectType();
        if (!apiObjectType.ClrType.IsInstanceOfType(clrObject))
        {
            throw new ApiSchemaMaterializationException
            (
                $"CLR object type '{clrObject.GetType().SafeToName()}' is not compatible with '{apiObjectType.ClrType.SafeToName()}'."
            );
        }

        try
        {
            var value = apiProperty.GetValue(clrObject);
            return this.MaterializeVersionFromValue(value);
        }
        catch (ApiSchemaMaterializationException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new ApiSchemaMaterializationException
            (
                $"Failed to read version property '{apiProperty.ClrName}'.",
                exception
            );
        }
    }

    /// <summary>
    ///     Materializes a version from a scalar value supplied by a repository or property.
    /// </summary>
    /// <param name="value">The non-null version value.</param>
    /// <returns>The materialized version.</returns>
    /// <exception cref="ApiSchemaMaterializationException">
    ///     The value is null or does not have the configured exact CLR type.
    /// </exception>
    public ApiVersion MaterializeVersionFromValue(object? value)
    {
        var clrType = this.ApiScalarType.ClrType;

        if (value is null)
        {
            throw new ApiSchemaMaterializationException("An API object version cannot be null.");
        }

        if (value is ApiVersion version)
        {
            if (!version.HasValue || version.ClrType != clrType)
            {
                throw new ApiSchemaMaterializationException
                (
                    $"The supplied {nameof(ApiVersion)} does not contain the configured exact " +
                    $"CLR type '{clrType.SafeToName()}'."
                );
            }

            return version;
        }

        if (value.GetType() != clrType)
        {
            throw new ApiSchemaMaterializationException
            (
                $"Version value CLR type '{value.GetType().SafeToName()}' does not match " +
                $"configured type '{clrType.SafeToName()}'."
            );
        }

        try
        {
            return ApiVersion.FromValue(value, clrType);
        }
        catch (ApiVersionException exception)
        {
            throw new ApiSchemaMaterializationException
            (
                "The version value could not be materialized.",
                exception
            );
        }
    }
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString()
    {
        var clrType = (_apiResolvedScalarType?.ClrType ?? _clrRepositoryType).SafeToString();
        var apiPropertyReference = this.ApiPropertyReference.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiVersionDefinition)} "
            + $"{{{nameof(this.ApiPropertyReference)}={apiPropertyReference}, "
            + $"{nameof(this.ExtensionCount)}={extensionCount}}} [{clrType}]";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc/>
    protected override string BuildPath(string? apiPreviousPath)
        => ApiSchemaPathFormatting.BuildPath
        (
            apiBasePath: apiPreviousPath,
            apiPathSegment: this.ApiElementName,
            apiPathSegmentName: null
        );

    /// <inheritdoc/>
    internal override void CompileCore(ApiSchemaCompilationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        base.CompileCore(context);

        if (this.IsRepositoryBacked)
        {
            if (!this.ValidateClrType(context, _clrRepositoryType))
            {
                return;
            }

            this.ResolveRepositoryApiScalarType(context);
            return;
        }

        this.ResolveApiProperty(context);
    }
    #endregion

    #region Implementation Methods
    private ApiObjectType GetApiObjectType() => this.Parent as ApiObjectType ?? throw new ApiSchemaException($"An {nameof(ApiVersionDefinition)} must be owned by an {nameof(ApiObjectType)}.");

    private void ResolveApiProperty(ApiSchemaCompilationContext context)
    {
        var apiObjectType = this.GetApiObjectType();
        if (!_apiPropertyBinding.TryResolveReference
        (
            apiObjectType,
            context,
            ApiSchemaCompilationCode.ApiVersionDefinitionUnresolvedProperty,
            "Version property reference"
        ))
        {
            return;
        }

        var apiProperty = _apiPropertyBinding.ApiProperty;

        if (!apiProperty.IsResolved)
        {
            return;
        }

        if (apiProperty.ApiType is not ApiScalarType apiScalarType)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiVersionDefinitionNonScalarProperty;
            var description = $"Version property '{apiProperty.ApiName}' is not scalar";
            var remediation = "Configure the version property with a scalar API type";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        if (!this.ValidateClrType(context, apiScalarType.ClrType))
        {
            return;
        }

        if (apiProperty.ClrMemberType is null)
        {
            return;
        }

        if (apiProperty.ClrMemberType != apiScalarType.ClrType)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiVersionDefinitionClrTypeMismatch;
            var description =
                $"Version member CLR type '{apiProperty.ClrMemberType.SafeToName()}' does not " +
                $"match scalar type '{apiScalarType.ClrType.SafeToName()}'";
            var remediation =
                "Use the same exact CLR type for the version member and scalar API type";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        if (apiProperty.IsOptional)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiVersionDefinitionOptionalProperty;
            var description = $"Version property '{apiProperty.ApiName}' is optional";
            var remediation = "Configure the version property as required";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        _apiResolvedScalarType = apiScalarType;
    }

    private void ResolveRepositoryApiScalarType(ApiSchemaCompilationContext context)
    {
        var schema = (ApiSchema)this.Root;
        if (!schema.TryGetScalarTypeByClrType(_clrRepositoryType!, out var apiScalarType))
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiVersionDefinitionUnresolvedScalarType;
            var description = $"No {nameof(this.ApiScalarType)} is registered for CLR type " +
                $"'{_clrRepositoryType.SafeToName()}'";
            var remediation = $"Add an {nameof(this.ApiScalarType)} for the configured version CLR type";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        _apiResolvedScalarType = apiScalarType;
    }

    private bool ValidateClrType(ApiSchemaCompilationContext context, Type? clrType)
    {
        if (clrType is null)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiVersionDefinitionNullClrType;
            var description = $"{nameof(this.ClrType)} must not be null";
            var remediation = $"Specify a valid scalar {nameof(this.ClrType)}";

            context.AddIssue(severity, code, description, remediation);
            return false;
        }

        if (Nullable.GetUnderlyingType(clrType) is not null)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiVersionDefinitionNullableClrType;
            var description = $"{nameof(this.ClrType)} must not be nullable";
            var remediation = "Use the non-nullable underlying CLR type for the version";

            context.AddIssue(severity, code, description, remediation);
            return false;
        }

        return true;
    }
    #endregion
}
