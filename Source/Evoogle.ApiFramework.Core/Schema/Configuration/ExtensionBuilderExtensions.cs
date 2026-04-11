// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Schema.Configuration;

// ExtensionBuilderExtensions is intentionally empty.
// The generic AddExtension<T> convenience overload is now a named, type-specific method
// on each concrete builder (e.g. AddSchemaExtension<T>, AddObjectExtension<T>),
// which prevents the silent mis-attachment bug where a top-level .AddExtension() call
// after a lambda could attach to the outer schema builder instead of the intended type.
public static class ExtensionBuilderExtensions
{
}

