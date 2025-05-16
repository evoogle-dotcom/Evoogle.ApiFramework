// Copyright (c) 2024-2025 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
using Evoogle.XUnit;

using Xunit.Sdk;

[assembly: RegisterXunitSerializer(typeof(XUnitTestSerializer), typeof(XUnitTest), typeof(XUnitTestAsync))]