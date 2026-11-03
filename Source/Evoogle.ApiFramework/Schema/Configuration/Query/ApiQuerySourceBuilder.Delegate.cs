// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using Evoogle.ApiFramework.Schema.Compilation;
using Evoogle.ApiFramework.Schema.Configuration.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Annotations.Internal;
using Evoogle.ApiFramework.Schema.Configuration.Types.Internal;
using Evoogle.ApiFramework.Schema.Types;
using Evoogle.Reflection;

namespace Evoogle.ApiFramework.Schema.Configuration.Query;

public sealed partial class ApiQuerySourceBuilder
{
    #region Fields
    private Type? _clrDelegateType;
    private readonly Dictionary<string, string> _declarationErrors = new(StringComparer.Ordinal);
    private ApiNullability? _outputItemNullability;
    private ApiConfigurationSource _outputItemNullabilitySource = ApiConfigurationSource.Convention;
    #endregion

    #region Properties
    internal Type? ClrDelegateType => _clrDelegateType;
    #endregion

    #region Declaration Methods
    internal void InitializeDelegate(Type clrDelegateType)
    {
        _clrDelegateType = clrDelegateType;
        var invalidReason = ApiQuerySourceAnnotationValidation.GetInvalidDeclarationReason(clrDelegateType);
        if (invalidReason is not null)
        {
            _declarationErrors["delegate"] = invalidReason;
            return;
        }
        var invokeMethod = clrDelegateType.GetMethod(nameof(Action.Invoke))!;
        var outputNullability = ParameterReflection.GetNullabilityInfo(invokeMethod.ReturnParameter);
        this.ApplyConfiguration(ApiConfigurationSource.Convention, () =>
        {
            this.SetOutputType(() => ApiTypeExpressionBuilder.Build(outputNullability));
            this.SetOutputNullability(outputNullability.Nullability == MemberNullability.NonNullable
                ? ApiNullability.NonNullable : ApiNullability.Nullable);
            foreach (var parameter in invokeMethod.GetParameters())
            {
                var nullability = ParameterReflection.GetNullabilityInfo(parameter);
                var parameterBuilder = new ApiQuerySourceParameterBuilder
                (
                    parameter.Name!,
                    parameter.ParameterType,
                    () => ApiTypeExpressionBuilder.Build(nullability)
                )
                {
                    ClrParameterInfo = parameter
                };
                parameterBuilder.ApplyConfiguration(ApiConfigurationSource.Convention, () =>
                {
                    if (nullability.Nullability != MemberNullability.Unknown)
                    {
                        parameterBuilder.WithNullability(nullability.Nullability == MemberNullability.NonNullable
                            ? ApiNullability.NonNullable : ApiNullability.Nullable);
                    }
                });
                _state.InputParameterBuilders.Add(parameterBuilder);
            }
        });
    }

    internal bool IsCompatibleOutput(Type? clrType)
    {
        if (_clrDelegateType is null)
        {
            return true;
        }
        if (_clrDelegateType.GetMethod(nameof(Action.Invoke))?.ReturnType == clrType)
        {
            _declarationErrors.Remove("output");
            return true;
        }
        _declarationErrors["output"] =
            "Configuration cannot replace the CLR output type of a delegate-backed query source";
        return false;
    }

    internal void CollectConfigurationIssues(ApiSchemaBuilderContext context, string? schemaName)
    {
        var schemaPath = Compilation.Internal.ApiSchemaPathFormatting.BuildPath(null, nameof(ApiSchema), schemaName);
        if (_outputItemNullability is not null &&
            _state.OutputTypeFactory?.Invoke()?.ApiInlineType is not ApiCollectionType)
        {
            _declarationErrors["configuration:outputItemNullability"] =
                "Collection-item nullability requires a collection output";
        }
        else
        {
            _declarationErrors.Remove("configuration:outputItemNullability");
        }
        foreach (var parameter in _state.InputParameterBuilders.Where(static parameter => parameter.HasInvalidItemNullability))
        {
            _declarationErrors["configuration:parameter:" + parameter.ApiName] =
                $"Collection-item nullability requires a collection parameter; '" +
                    $"{parameter.ApiName}' is not a collection";
        }
        foreach (var (facet, error) in _declarationErrors)
        {
            var apiPath = Compilation.Internal.ApiSchemaPathFormatting.BuildPath
                (schemaPath, nameof(Schema.Query.ApiQuerySource), this.ApiName);
            var severity = ApiSchemaCompilationSeverity.Error;
            var code = facet == "delegate" ? ApiSchemaCompilationCode.ApiQuerySourceInvalidDelegate
                : facet.StartsWith("configuration:", StringComparison.Ordinal)
                    ? ApiSchemaCompilationCode.ApiQuerySourceConfigurationConflict
                    : ApiSchemaCompilationCode.ApiQuerySourceDeclarationConflict;
            var description = error;
            var remediation = "Configure the API-facing facets without changing the public closed delegate signature";
            context.AddConfigurationIssue(new(apiPath, severity, code, description, remediation));
        }
    }
    #endregion

    #region With Methods
    /// <summary>Sets the source API name without changing declaration identity.</summary>
    /// <param name="apiName">The source API name.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder WithName(string apiName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        if (this.CurrentConfigurationSource >= _apiNameSource)
        {
            _apiName = apiName;
            _apiNameSource = this.CurrentConfigurationSource;
        }
        return this;
    }
    #endregion

    #region ConfigureInputParameter Methods
    /// <summary>Configures one delegate parameter by its original CLR metadata name.</summary>
    /// <param name="clrParameterName">The CLR parameter name.</param>
    /// <param name="configure">The parameter configuration.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder ConfigureInputParameter
    (
        string clrParameterName,
        Action<ApiQuerySourceParameterBuilder> configure
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clrParameterName);
        ArgumentNullException.ThrowIfNull(configure);
        var parameter = _state.InputParameterBuilders.FirstOrDefault(builder =>
            Evoogle.ApiFramework.Internal.ClrNameComparer.Instance.Equals(builder.ClrParameterInfo?.Name,
                clrParameterName));
        if (parameter is null)
        {
            _declarationErrors["parameter:" + clrParameterName] =
                $"Delegate '{_clrDelegateType}' has no parameter named '{clrParameterName}'";
            return this;
        }
        parameter.ApplyConfiguration(this.CurrentConfigurationSource, () => configure(parameter));
        return this;
    }
    #endregion

    #region With Methods
    /// <summary>Sets output nullability independently of its declared CLR type.</summary>
    /// <param name="apiOutputNullability">The output nullability.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder WithOutputNullability(ApiNullability apiOutputNullability)
    {
        this.SetOutputNullability(apiOutputNullability);
        return this;
    }

    /// <summary>Sets nullability for items of a collection output.</summary>
    /// <param name="apiItemNullability">The collection-item nullability.</param>
    /// <returns>The current builder.</returns>
    public ApiQuerySourceBuilder WithOutputItemNullability(ApiNullability apiItemNullability)
    {
        if (this.CurrentConfigurationSource >= _outputItemNullabilitySource)
        {
            _outputItemNullability = apiItemNullability;
            _outputItemNullabilitySource = this.CurrentConfigurationSource;
        }
        return this;
    }
    #endregion

    #region Type Expression Methods
    internal static ApiTypeExpression ApplyItemNullability(ApiTypeExpression expression, ApiNullability? nullability)
    {
        if (nullability is null || expression?.ApiInlineType is not ApiCollectionType collection)
        {
            return expression!;
        }
        return new ApiTypeExpression(new ApiCollectionType
            (collection.ApiItemTypeExpression, nullability.Value, collection.ClrType));
    }
    #endregion
}
