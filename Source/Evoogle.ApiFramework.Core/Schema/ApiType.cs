// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
using System.Text.Json.Serialization;

using Evoogle.Extension;

namespace Evoogle.ApiFramework.Schema;

[JsonConverter(typeof(ApiTypeJsonConverter))]
public abstract class ApiType(Type clrType) : ExtensibleBase
{
    #region ApiType Properties
    public abstract ApiTypeKind Kind { get; }

    public Type ClrType { get; } = clrType;
    #endregion
}
