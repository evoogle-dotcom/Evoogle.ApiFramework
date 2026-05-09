// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;
using Evoogle.ApiFramework.Schema.Internal;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents an identity part that sources its value directly from a scalar property on the declaring object type.
/// </summary>
/// <param name="clrPropertyName">The CLR property name of the scalar property whose value becomes the identity component.</param>
/// <param name="clrScalarTypeHint">An optional CLR type hint that overrides the default scalar type resolved from the property. When <see langword="null"/>, the type is inferred from the property's <see cref="ApiScalarType"/>.</param>
public sealed class ApiIdentityScalarPart(string clrPropertyName, Type? clrScalarTypeHint = null) : ApiIdentityPropertyPart(clrPropertyName)
{
    #region ApiIdentityScalarPart Fields
    private Type? _clrResolvedScalarType = null;
    #endregion

    #region ApiSchemaElement Properties
    /// <inheritdoc/>
    protected override string ApiElementName => nameof(ApiIdentityScalarPart);
    #endregion

    #region ApiIdentityPart Properties
    /// <inheritdoc/>
    public override ApiIdentityPartKind ApiKind => ApiIdentityPartKind.Scalar;
    #endregion

    #region ApiIdentityScalarPart Properties
    /// <summary>
    ///     Gets the optional CLR type hint provided at construction time that overrides automatic scalar type resolution.
    ///     When <see langword="null"/>, the scalar type is inferred from the backing property's <see cref="ApiScalarType"/>.
    /// </summary>
    public Type? ClrScalarTypeHint { get; } = clrScalarTypeHint;

    /// <summary>Gets the resolved CLR scalar type used when extracting this identity part's value. Available after initialization.</summary>
    public Type ClrScalarType => this.ThrowIfNotInitialized(_clrResolvedScalarType);
    #endregion

    #region Object Methods
    /// <inheritdoc />
    public override string ToString()
    {
        var clrPropertyName = this.ClrPropertyName.SafeToString();
        var clrScalarTypeHint = this.ClrScalarTypeHint.SafeToName();
        var extensionCount = this.ExtensionCount.SafeToString();

        return $"{nameof(ApiIdentityScalarPart)} {{{nameof(this.ClrPropertyName)}={clrPropertyName}, {nameof(this.ClrScalarTypeHint)}={clrScalarTypeHint}, {nameof(this.ExtensionCount)}={extensionCount}}}";
    }
    #endregion

    #region ApiSchemaElement Methods
    /// <inheritdoc />
    internal override void Initialize(ApiInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        base.Initialize(context);

        this.InitializeClrScalarType(context);
    }
    #endregion

    #region Implementation Methods
    private void InitializeClrScalarType(ApiInitializationContext context)
    {
        if (this.ApiResolvedProperty is null)
        {
            _clrResolvedScalarType = null;
            return;
        }

        if (!this.ApiResolvedProperty.IsResolved)
        {
            _clrResolvedScalarType = null;
            return;
        }

        if (this.ApiResolvedProperty.ApiType is not ApiScalarType apiPropertyScalarType)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Error;
            var code = ApiInitializationCode.API_IDENTITY_PART_INVALID_API_PROPERTY_TYPE;
            var description = $"Property '{this.ClrPropertyName}' must be a scalar type for a scalar identity part";
            var remediation = $"Use a scalar-typed property or switch to {nameof(ApiIdentityNestedPart)}";

            context.AddIssue(path, severity, code, description, remediation);
            _clrResolvedScalarType = null;
            return;
        }

        var clrPropertyScalarType = apiPropertyScalarType.ClrType;

        var clrScalarType = this.ClrScalarTypeHint ?? clrPropertyScalarType;

        _clrResolvedScalarType = ApiId.GetCompatibleScalarType(clrScalarType);

        var clrPropertyScalarTypeIsString = clrPropertyScalarType == typeof(string);
        var clrResolvedScalarTypeIsString = _clrResolvedScalarType == typeof(string);
        var possiblePerformanceConcern = clrPropertyScalarTypeIsString != clrResolvedScalarTypeIsString;

        if (possiblePerformanceConcern)
        {
            var path = this.ApiPath;
            var severity = ApiInitializationSeverity.Warning;
            var code = ApiInitializationCode.API_IDENTITY_PART_PERFORMANCE_CONCERN;
            var description = $"Identity part '{this.ClrPropertyName}' has CLR property type {clrPropertyScalarType.SafeToName()} but resolves to scalar type {_clrResolvedScalarType.SafeToName()}, requiring string parse/format on every identity operation";
            var remediation = $"Align the '{this.ClrPropertyName}' property type with the resolved scalar type {_clrResolvedScalarType.SafeToName()} to eliminate string coercion, or accept the overhead if the type mismatch is intentional";

            context.AddIssue(path, severity, code, description, remediation);
        }
    }
    #endregion
}
