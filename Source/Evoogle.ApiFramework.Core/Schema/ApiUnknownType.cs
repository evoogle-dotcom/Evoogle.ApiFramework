// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.ComponentModel.DataAnnotations;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.Extensions;

namespace Evoogle.ApiFramework.Schema;

/// <summary>
///     Represents an unknown API type in the API schema.
///     This type is used when the API type cannot be determined or is not recognized.
/// </summary>
/// <remarks>
///     I am thinking of renaming this class to "ApiNullType" in the future.
/// </remarks>
[Obsolete("Consider using nullable ApiType references instead of ApiUnknownType.")]
public sealed class ApiUnknownType() : ApiType()
{
    #region ApiType Properties
    /// <inheritdoc/>
    public override ApiTypeKind Kind => ApiTypeKind.Unknown;

    /// <summary>
    ///     Gets the CLR type associated with the API type.
    /// </summary>
    /// <remarks>
    ///     This property always throws an exception because unknown API types do not have a CLR type associated with them.
    /// </remarks>
    /// <exception cref="ApiSchemaException">Always thrown when trying to access the CLR type of an unknown API type.</exception>
    public override Type ClrType => throw new ApiSchemaException($"{this} does not have a CLR type associated with it.");

    /// <inheritdoc/>
    protected override string ApiTypeName => nameof(ApiUnknownType);
    #endregion

    #region ApiType Methods
    /// <inheritdoc />
    protected override string GetValidationPath() => $"{this.ApiTypeName.SafeToString()}";
    #endregion

    #region Object Methods
    /// <inheritdoc/>
    public override string ToString() => $"{nameof(ApiUnknownType)}";

    internal override void Initialize(ApiSchema apiSchema, ref List<ValidationResult>? results)
    { 
        // No initialization needed for unknown type.
    }
    #endregion
}
