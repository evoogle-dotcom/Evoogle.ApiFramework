// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Trace;

namespace Evoogle.ApiFramework.Schema.Configuration;

/// <summary>
///     Builds an <see cref="ApiEnumValue"/> while preserving the precedence of its configured API
///     name.
/// </summary>
public sealed class ApiEnumValueBuilder
{
    #region Fields
    private readonly ApiEnumValueState _state;
    private readonly ApiSchemaBuilderContext? _context;
    private readonly Type? _clrEnumType;
    #endregion

    #region Constructors
    /// <summary>
    ///     Initializes a new enum-value builder with an explicitly supplied API name.
    /// </summary>
    /// <param name="apiName">The explicit API name of the enumeration value.</param>
    /// <param name="clrName">The CLR name of the enumeration value.</param>
    /// <param name="clrOrdinal">The CLR ordinal of the enumeration value.</param>
    public ApiEnumValueBuilder(string apiName, string clrName, int clrOrdinal)
        : this(apiName, clrName, clrOrdinal, ApiConfigurationSource.Explicit)
    {
    }

    internal ApiEnumValueBuilder
    (
        string apiName,
        string clrName,
        int clrOrdinal,
        ApiConfigurationSource apiNameSource
    ) : this(apiName, clrName, clrOrdinal, apiNameSource, null, null)
    {
    }

    internal ApiEnumValueBuilder
    (
        string apiName,
        string clrName,
        int clrOrdinal,
        ApiConfigurationSource apiNameSource,
        ApiSchemaBuilderContext? context,
        Type? clrEnumType
    )
    {
        _state = new ApiEnumValueState(ValidateName(apiName, nameof(apiName)), apiNameSource);
        this.ClrName = ValidateName(clrName, nameof(clrName));
        this.ClrOrdinal = clrOrdinal;
        _context = context;
        _clrEnumType = clrEnumType;
    }
    #endregion

    #region Properties
    /// <summary>Gets the current API name of the enumeration value.</summary>
    public string ApiName => _state.ApiName;

    /// <summary>Gets the CLR name of the enumeration value.</summary>
    public string ClrName { get; }

    /// <summary>Gets the CLR ordinal of the enumeration value.</summary>
    public int ClrOrdinal { get; }
    #endregion

    #region With Methods
    /// <summary>
    ///     Sets an explicit API name for the enumeration value.
    /// </summary>
    /// <param name="apiName">The explicit API name to use.</param>
    /// <returns>The current builder instance.</returns>
    public ApiEnumValueBuilder WithName(string apiName)
    {
        return this.SetApiName(apiName, ApiConfigurationSource.Explicit);
    }
    #endregion

    #region Internal Convention Methods
    /// <summary>Gets the configuration source that established the current API name.</summary>
    internal ApiConfigurationSource ApiNameSource => _state.ApiNameSource;

    /// <summary>
    ///     Sets the API name at data-annotation precedence.
    ///     Has no effect if an explicit value has already been applied.
    /// </summary>
    internal ApiEnumValueBuilder SetApiNameDataAnnotation(string apiName)
        => this.SetApiName(apiName, ApiConfigurationSource.DataAnnotation);

    /// <summary>
    ///     Sets the API name at convention precedence.
    ///     Has no effect if a higher-precedence value has already been applied.
    /// </summary>
    internal ApiEnumValueBuilder SetApiNameConvention(string apiName)
        => this.SetApiName(apiName, ApiConfigurationSource.Convention);
    #endregion

    #region Build Methods
    /// <summary>Builds the configured <see cref="ApiEnumValue"/>.</summary>
    internal ApiEnumValue Build()
        => new(_state.ApiName, this.ClrName, this.ClrOrdinal);
    #endregion

    #region Implementation Methods
    private ApiEnumValueBuilder SetApiName(string apiName, ApiConfigurationSource source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName, nameof(apiName));

        var previousValue = _state.ApiName;
        var wasApplied = source >= _state.ApiNameSource;

        if (source >= _state.ApiNameSource)
        {
            _state.ApiName = apiName;
            _state.ApiNameSource = source;
        }

        _context?.TraceConfigurationChange
        (
            this.GetTraceTarget(),
            ApiSchemaBuildConfigurationFacet.ApiName,
            source,
            previousValue,
            apiName,
            _state.ApiName,
            wasApplied,
            wasApplied ? null : "A higher-precedence API name is already configured."
        );

        return this;
    }

    private ApiSchemaBuildTraceTarget GetTraceTarget()
    {
        return new
        (
            ApiSchemaBuildTargetKind.EnumValue,
            _clrEnumType,
            this.ClrName,
            _state.ApiName
        );
    }

    private static string ValidateName(string name, string paramName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, paramName);
        return name;
    }
    #endregion
}
