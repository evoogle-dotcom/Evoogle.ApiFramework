// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.XUnit;

namespace Evoogle.ApiFramework.Schema.Configuration.Annotations;

public partial class ApiAnnotationTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Methods
    [Theory]
    [MemberData(nameof(BuildTheoryData))]
    public void Build(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(BuildThrowsTheoryData))]
    public void BuildThrows(IXUnitTest test) => test.Execute(this);
    #endregion
}
