// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Runtime.CompilerServices;

// Allow framework assemblies to be friend assemblies of this assembly

// Allow unit test assemblies to be friend assemblies of this assembly
[assembly: InternalsVisibleTo("Evoogle.ApiFramework.Core.Tests")]
