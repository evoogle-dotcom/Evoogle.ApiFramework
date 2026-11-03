// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration.Conventions.Internal;

/// <summary>
///     Provides convenience helpers for convention and annotation tests.
/// </summary>
internal static class ApiConventionTestHelpers
{
    #region Test Helpers
    /// <summary>
    ///     Registers the common .NET primitive scalar types used by convention and annotation
    ///     test domain models so that schema compilation succeeds.
    /// </summary>
    public static ApiSchemaBuilder WithTestScalars(this ApiSchemaBuilder builder)
    {
        return builder
            .AddScalar<bool>(static x => x.WithName("Boolean"))
            .AddScalar<byte>(static x => x.WithName("Byte"))
            .AddScalar<decimal>(static x => x.WithName("Decimal"))
            .AddScalar<double>(static x => x.WithName("Double"))
            .AddScalar<float>(static x => x.WithName("Single"))
            .AddScalar<Guid>(static x => x.WithName("Guid"))
            .AddScalar<int>(static x => x.WithName("Int32"))
            .AddScalar<long>(static x => x.WithName("Int64"))
            .AddScalar<short>(static x => x.WithName("Int16"))
            .AddScalar<string>(static x => x.WithName("String"));
    }
    #endregion
}
