// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace Evoogle.ApiFramework.Schema.Configuration.Query;

[DynamicLinqType]
public static class ApiQuerySourceBuilderTestsFactory
{
    #region Input Parameter Factory Methods
    public static ApiSchema BuildWithLaterOptionalPresence()
        => BuildSchema(parameter => parameter.WithPresence(ApiPresence.Required).WithPresence(ApiPresence.Optional));

    public static ApiSchema BuildWithAnnotationPresence()
        => BuildSchema(parameter =>
        {
            parameter.ApplyConfiguration(ApiConfigurationSource.DataAnnotation, () => parameter.AsOptional());
            parameter.ApplyConfiguration(ApiConfigurationSource.Convention, () => parameter.AsRequired());
        });

    public static ApiSchema BuildWithExplicitPresenceOverAnnotation()
        => BuildSchema(parameter =>
        {
            parameter.WithPresence(ApiPresence.Optional);
            parameter.ApplyConfiguration(ApiConfigurationSource.DataAnnotation, () => parameter.AsRequired());
        });

    public static ApiSchema BuildRequiredArgumentWithNonNullableValue()
        => BuildSchema(parameter => parameter.AsRequired().AsNonNullable());

    public static ApiSchema BuildWithDefaultArgumentSettings()
        => BuildSchema(static _ => { });

    public static ApiSchema BuildRequiredArgumentWithNullableValue()
        => BuildSchema(parameter => parameter.AsRequired().AsNullable());

    public static ApiSchema BuildOptionalArgumentWithNonNullableValue()
        => BuildSchema(parameter => parameter.AsOptional().AsNonNullable());

    public static ApiSchema BuildOptionalArgumentWithNullableValue()
        => BuildSchema(parameter => parameter.AsNullable().AsOptional());

    public static ApiSchema BuildWithLaterRequiredNonNullableConfiguration()
        => BuildSchema(parameter => parameter.AsOptional().AsNullable()
            .AsRequired().AsNonNullable());

    public static ApiSchema BuildWithLaterNullableConfiguration()
        => BuildSchema(parameter => parameter.WithNullability(ApiNullability.NonNullable).AsNullable());

    public static ApiSchema BuildWithExplicitOptionalNullableConfiguration()
        => BuildSchema(parameter =>
        {
            parameter.AsOptional().AsNullable();
            parameter.ApplyConfiguration(ApiConfigurationSource.Convention, () =>
                parameter.AsRequired().AsNonNullable());
        });

    public static ApiSchema BuildWithExplicitRequiredNonNullableConfiguration()
        => BuildSchema(parameter =>
        {
            parameter.AsRequired().AsNonNullable();
            parameter.ApplyConfiguration(ApiConfigurationSource.Convention, () =>
                parameter.AsOptional().AsNullable());
        });
    #endregion

    #region Implementation Methods
    private static ApiSchema BuildSchema(Action<ApiQuerySourceParameterBuilder> configure)
        => new ApiSchemaBuilder()
            .WithName("InputParameterTests")
            .AddScalar<string>()
            .AddQuerySource("Echo", source => source
                .AddInputParameter<string>("text", configure)
                .WithOutput<string>())
            .Build();
    #endregion
}
