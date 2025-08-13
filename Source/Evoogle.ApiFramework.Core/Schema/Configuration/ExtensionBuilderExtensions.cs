// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

public static class ExtensionBuilderExtensions
{
    #region Methods
    public static TBuilder AddExtension<TBuilder, T>(this ExtensionBuilder<TBuilder> builder, T value)
        where TBuilder : ExtensionBuilder<TBuilder>
        where T : notnull => builder.AddExtension(typeof(T), value);
    #endregion
}

