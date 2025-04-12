// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Evoogle.ApiFramework.Exceptions;

public class ApiSchemaException : ApiException
{
    #region Constructors
    public ApiSchemaException()
        : base("An API schema error occurred.")
    { }

    public ApiSchemaException(string message)
        : base(message)
    { }

    public ApiSchemaException(string message, Exception innerException)
        : base(message, innerException)
    { }
    #endregion
}
