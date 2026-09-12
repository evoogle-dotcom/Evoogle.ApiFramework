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
using Evoogle.ApiFramework.Version;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema.Version;

/// <summary>
///     Describes the single scalar version token associated with an <see cref="ApiObjectType"/>.
/// </summary>
/// <remarks>
///     A property-backed version is configured with a CLR member name and can be materialized
///     from an object instance. A repository-backed version is configured with a CLR type and
///     must be materialized from a value supplied by the repository.
/// </remarks>
[JsonConverter(typeof(ApiVersionTypeJsonConverter))]
public sealed class ApiVersionType : ApiSchemaElement
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
    private ApiProperty? _apiResolvedProperty = null;
    private ApiScalarType? _apiResolvedScalarType = null;
    #endregion

    #region Constructors
    /// <summary>Initializes a property-backed version definition.</summary>
    /// <param name="clrMemberName">The CLR member exposed as an API property.</param>
    public ApiVersionType(string clrMemberName)
    {
        _sourceKind = VersionSourceKind.Property;
        this.ClrMemberName = clrMemberName;
    }

    /// <summary>Initializes a repository-backed version definition.</summary>
    /// <param name="clrType">The exact CLR type of the repository-supplied version value.</param>
    public ApiVersionType(Type clrType)
    {
        _sourceKind = VersionSourceKind.Repository;
        _clrRepositoryType = clrType;
    }
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    public override ApiSchemaElementKind Kind => ApiSchemaElementKind.VersionType;

    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiVersionType);
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
            return _apiResolvedProperty;
        }
    }

    /// <summary>Gets the scalar schema type that represents the version value.</summary>
    /// <exception cref="ApiSchemaException">The schema has not compiled successfully.</exception>
    public ApiScalarType ApiScalarType => this.RequireValue(_apiResolvedScalarType);

    /// <summary>
    ///     Gets the CLR member name for a property-backed version, or null for a
    ///     repository-backed version.
    /// </summary>
    public string? ClrMemberName { get; }

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
                $"{nameof(ApiVersionType)} is repository-backed and cannot materialize a version from an object instance."
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
        var clrMemberName = this.ClrMemberName.SafeToString();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiVersionType)} {{{nameof(this.ClrMemberName)}={clrMemberName}, {nameof(this.ExtensionCount)}={extensionCount}}} [{clrType}]";
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
    private ApiObjectType GetApiObjectType()
    {
        return this.Parent as ApiObjectType ?? throw new ApiSchemaException($"An {nameof(ApiVersionType)} must be owned by an {nameof(ApiObjectType)}.");
    }

    private void ResolveApiProperty(ApiSchemaCompilationContext context)
    {
        if (ApiSchemaNameValidation.IsNameInvalid(this.ClrMemberName))
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiVersionTypeInvalidClrMemberName;
            var description = $"{nameof(this.ClrMemberName)} must not be empty or whitespace";
            var remediation = $"Specify a valid {nameof(this.ApiProperty.ClrName)} or omit {nameof(this.ClrMemberName)} for a repository-backed version";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        var apiObjectType = this.GetApiObjectType();
        if (!apiObjectType.TryGetPropertyByClrName(this.ClrMemberName!, out var apiProperty))
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiVersionTypeUnresolvedProperty;
            var description = $"No {nameof(this.ApiProperty)} has CLR name '{this.ClrMemberName}'";
            var remediation = $"Add the version member as an {nameof(this.ApiProperty)} on {nameof(ApiObjectType)}['{apiObjectType.ApiName}']";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        if (!apiProperty.IsResolved)
        {
            return;
        }

        if (apiProperty.ApiType is not ApiScalarType apiScalarType)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiVersionTypeNonScalarProperty;
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
            var code = ApiSchemaCompilationCode.ApiVersionTypeClrTypeMismatch;
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
            var code = ApiSchemaCompilationCode.ApiVersionTypeOptionalProperty;
            var description = $"Version property '{apiProperty.ApiName}' is optional";
            var remediation = "Configure the version property as required";

            context.AddIssue(severity, code, description, remediation);
            return;
        }

        _apiResolvedProperty = apiProperty;
        _apiResolvedScalarType = apiScalarType;
    }

    private void ResolveRepositoryApiScalarType(ApiSchemaCompilationContext context)
    {
        var schema = (ApiSchema)this.Root;
        if (!schema.TryGetScalarTypeByClrType(_clrRepositoryType!, out var apiScalarType))
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiVersionTypeUnresolvedScalarType;
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
            var code = ApiSchemaCompilationCode.ApiVersionTypeNullClrType;
            var description = $"{nameof(this.ClrType)} must not be null";
            var remediation = $"Specify a valid scalar {nameof(this.ClrType)}";

            context.AddIssue(severity, code, description, remediation);
            return false;
        }

        if (Nullable.GetUnderlyingType(clrType) is not null)
        {
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = ApiSchemaCompilationCode.ApiVersionTypeNullableClrType;
            var description = $"{nameof(this.ClrType)} must not be nullable";
            var remediation = "Use the non-nullable underlying CLR type for the version";

            context.AddIssue(severity, code, description, remediation);
            return false;
        }

        return true;
    }
    #endregion
}
