// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema;

namespace Evoogle.ApiFramework.Exceptions;

public sealed class ApiSchemaInitializationException : ApiSchemaException
{
    #region Properties
    public ApiInitializationResult Result { get; }
    public bool IsValid => this.Result.IsValid;
    public ApiInitializationIssue[] Issues => this.Result.Issues ?? [];
    public ApiInitializationIssue[] Errors => this.Result.Errors ?? [];
    public ApiInitializationIssue[] Warnings => this.Result.Warnings ?? [];

    public override string Message => _lazyMessage.Value;

    private readonly Lazy<string> _lazyMessage;
    #endregion

    #region Constructors
    public ApiSchemaInitializationException(ApiInitializationResult result)
        : base(default!)
    {
        ArgumentNullException.ThrowIfNull(result);
        this.Result = result;
        _lazyMessage = new Lazy<string>(this.BuildMessage);
    }

    public ApiSchemaInitializationException(string message, ApiInitializationResult result)
        : base(message)
    {
        ArgumentNullException.ThrowIfNull(result);
        this.Result = result;
        _lazyMessage = new Lazy<string>(this.BuildMessage);
    }

    public ApiSchemaInitializationException(string message, ApiInitializationResult result, Exception innerException)
        : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(result);
        this.Result = result;
        _lazyMessage = new Lazy<string>(this.BuildMessage);
    }
    #endregion

    #region Methods
    private string BuildMessage()
    {
        if (this.IsValid)
        {
            return $"{nameof(ApiSchema)} initialization succeeded.";
        }

        var errors = this.Errors.Length;
        var warnings = this.Warnings.Length;
        var total = this.Issues.Length;
        var header = $"{nameof(ApiSchema)} initialization failed.";
        var counts = $"Issues={total}, Errors={errors}, Warnings={warnings}.";
        return $"{header} {counts}";
    }
    #endregion
}