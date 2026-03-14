// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

public class ApiIdentityPartValue
{
    #region Properties
    public string Name { get; set; } = null!;

    public ApiIdentityPartValueKind Kind { get; set; }

    public ApiId? ScalarValue { get; set; }

    public ApiIdentityValue? ObjectValue { get; set; }
    #endregion
}
