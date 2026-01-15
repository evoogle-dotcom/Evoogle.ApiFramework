// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema;

public partial class ApiTypeTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Fields
    private static readonly List<string> _excludeMembers =
    [
        $"{nameof(ApiSchemaElement.ApiPath)}"
    ];
    #endregion
}
