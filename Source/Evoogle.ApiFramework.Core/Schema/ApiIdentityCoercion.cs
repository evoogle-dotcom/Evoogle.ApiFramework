// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Identity;

namespace Evoogle.ApiFramework.Schema;

public sealed class ApiIdentityCoercion(ApiIdentityTargetKind? targetKind = null, Func<object?, ApiId>? converter = null)
{
    public ApiIdentityTargetKind? TargetKind { get; } = targetKind;
    public Func<object?, ApiId>? Converter { get; } = converter;
}
