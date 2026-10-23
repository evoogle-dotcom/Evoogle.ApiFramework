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
    public static ApiSchema BuildRequiredArgumentWithNonNullableValue()
        => BuildSchema(parameter => parameter.WithRequiredArgument().AsNonNullable());

    public static ApiSchema BuildRequiredArgumentWithNullableValue()
        => BuildSchema(parameter => parameter.WithRequiredArgument().AsNullable());

    public static ApiSchema BuildOptionalArgumentWithNonNullableValue()
        => BuildSchema(parameter => parameter.WithOptionalArgument().AsNonNullable());

    public static ApiSchema BuildOptionalArgumentWithNullableValue()
        => BuildSchema(parameter => parameter.AsNullable().WithOptionalArgument());

    public static ApiSchema BuildWithLaterRequiredNonNullableConfiguration()
        => BuildSchema(parameter => parameter.WithOptionalArgument().AsNullable()
            .WithRequiredArgument().AsNonNullable());

    public static ApiSchema BuildWithLaterNullableConfiguration()
        => BuildSchema(parameter => parameter.WithModifiers(ApiTypeModifiers.Required).AsNullable());

    public static ApiSchema BuildWithExplicitOptionalNullableConfiguration()
        => BuildSchema(parameter =>
        {
            parameter.WithOptionalArgument().AsNullable();
            parameter.ApplyConfiguration(ApiConfigurationSource.Convention, () =>
                parameter.WithRequiredArgument().AsNonNullable());
        });

    public static ApiSchema BuildWithExplicitRequiredNonNullableConfiguration()
        => BuildSchema(parameter =>
        {
            parameter.WithRequiredArgument().AsNonNullable();
            parameter.ApplyConfiguration(ApiConfigurationSource.Convention, () =>
                parameter.WithOptionalArgument().AsNullable());
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
