// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

using Xunit.Sdk;

[assembly: RegisterXunitSerializer(typeof(XUnitTestSerializer), typeof(XUnitTest), typeof(XUnitTestAsync))]
