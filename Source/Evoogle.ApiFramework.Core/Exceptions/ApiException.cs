// Copyright (c) 2024 Evoogle.com
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Evoogle.ApiFramework.Exceptions;

public class ApiException : Exception
{
    #region Constructors
    public ApiException()
        : base("An error occurred during an API operation.")
    { }

    public ApiException(string message)
        : base(message)
    { }

    public ApiException(string message, Exception innerException)
        : base(message, innerException)
    { }
    #endregion
}
