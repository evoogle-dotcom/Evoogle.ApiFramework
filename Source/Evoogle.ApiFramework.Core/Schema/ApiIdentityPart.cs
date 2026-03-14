// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;

using Evoogle.ApiFramework.Schema.Json;

namespace Evoogle.ApiFramework.Schema;

[JsonConverter(typeof(ApiIdentityPartJsonConverter))]
public abstract class ApiIdentityPart : ApiSchemaElement
{
    #region ApiIdentityPart Properties
    public abstract ApiIdentityPartKind ApiKind { get; }
    #endregion

    #region Object Methods
    #endregion

    #region ApiSchemaElement Methods
    #endregion

    #region Implementation Methods
    #endregion
}
