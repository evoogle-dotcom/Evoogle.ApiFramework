// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Text.Json;
using System.Text.Json.Serialization;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.TestData;

/// <summary>Shared base for tests that build an <see cref="ApiSchema"/> and assert it against an expected schema.</summary>
public abstract class ApiSchemaBuildTestBase : XUnitTest
{
    #region Fields
    protected static readonly JsonSerializerOptions _defaultToJsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
    };
    #endregion

    #region Calculated Properties
    protected ApiSchema? ApiSchemaExpected { get; set; }
    protected ApiSchema? ApiSchemaActual { get; set; }

    protected Type? ExceptionTypeExpected { get; set; }
    protected string? ExceptionMessagePatternExpected { get; set; }
    protected Exception? ExceptionActual { get; set; }
    #endregion

    #region Constructors
    protected ApiSchemaBuildTestBase() => this.ExcludeMembers = ApiSchemaExcludeMembers.SchemaInitialized;
    #endregion

    #region XUnitTest Methods
    protected override void Assert()
    {
        if (this.ApiSchemaExpected is not null)
        {
            this.ApiSchemaActual.Should().NotBeNull();
            this.AssertBeEquivalentTo(this.ApiSchemaActual, this.ApiSchemaExpected);
        }
        else if (this.ExceptionTypeExpected is not null)
        {
            this.ExceptionActual.Should().NotBeNull();
            this.ExceptionActual.Should().BeOfType(this.ExceptionTypeExpected);

            if (this.ExceptionMessagePatternExpected is not null)
            {
                this.ExceptionActual.Message.Should().MatchEquivalentOf(this.ExceptionMessagePatternExpected);
            }
        }
        else
        {
            throw new InvalidOperationException("Either ApiSchemaExpected or ExceptionTypeExpected must be set.");
        }
    }
    #endregion
}
