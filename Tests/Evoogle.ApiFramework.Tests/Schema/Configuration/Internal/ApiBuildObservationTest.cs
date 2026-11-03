// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Linq.Expressions;
using System.Text.Json.Serialization;

using Evoogle.XUnit;
using Evoogle.XUnit.Json;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

internal sealed class ApiBuildObservationTest : XUnitTest
{
    #region Theory Inputs
    [JsonConverter(typeof(ExpressionFuncJsonConverter<string>))]
    public required Expression<Func<string>> ObserveExpression { get; init; }
    public string? ExpectedObservation { get; init; }
    public Type? ExpectedExceptionType { get; init; }
    #endregion

    #region Observations
    private ApiBuildObservationTest RestoredCase { get; set; } = null!;
    private string? ActualObservation { get; set; }
    private Exception? ActualException { get; set; }
    #endregion

    #region XUnitTest Methods
    protected override void Arrange()
    {
        this.WriteLine($"Scenario:           {this.Name}");
        this.WriteLine($"Observe Expression: {this.ObserveExpression}");
        this.WriteLine();
        this.WriteLine($"Expected Observation: {this.ExpectedObservation}");
        this.WriteLine($"Expected Exception:   {this.ExpectedExceptionType}");
        var serializer = new XUnitTestSerializer();
        this.RestoredCase = (ApiBuildObservationTest)serializer.Deserialize(this.GetType(), serializer.Serialize(this));
    }

    protected override void Act()
    {
        try
        {
            this.ActualObservation = this.RestoredCase.ObserveExpression.Compile()();
        }
        catch (Exception exception)
        {
            this.ActualException = exception;
        }
        this.WriteLine();
        this.WriteLine($"Actual Observation: {this.ActualObservation}");
        this.WriteLine($"Actual Exception:   {this.ActualException}");
    }

    protected override void Assert()
    {
        if (this.ExpectedExceptionType is not null)
        {
            this.ActualException.Should().BeOfType(this.ExpectedExceptionType);
        }
        else
        {
            this.ActualException.Should().BeNull();
            this.ActualObservation.Should().Be(this.ExpectedObservation);
        }
    }
    #endregion
}
