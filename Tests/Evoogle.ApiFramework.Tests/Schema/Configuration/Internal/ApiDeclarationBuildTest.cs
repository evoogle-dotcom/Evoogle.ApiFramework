// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Evoogle.ApiFramework.Schema.Query;
using Evoogle.XUnit;
using Evoogle.XUnit.Json;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

internal sealed class ApiDeclarationBuildTest : XUnitTest
{
    #region Theory Inputs
    [JsonConverter(typeof(ExpressionFuncJsonConverter<ApiSchemaCompilationResult>))]
    public required Expression<Func<ApiSchemaCompilationResult>> BuildExpression { get; init; }
    public bool ExpectedSuccess { get; init; } = true;
    public ApiSchemaCompilationCode? ExpectedCode { get; init; }
    public string? ExpectedApiPath { get; init; }
    public string[]? ExpectedSourceNames { get; init; }
    public string[]? ExpectedRelationshipNames { get; init; }
    public string[]? ExpectedTraversals { get; init; }
    public string[]? ExpectedParameterNames { get; init; }
    public Type[]? ExpectedParameterTypes { get; init; }
    public Type? ExpectedOutputClrType { get; init; }
    public ApiNullability? ExpectedOutputNullability { get; init; }
    public ApiNullability? ExpectedItemNullability { get; init; }
    public ApiNullability? ExpectedParameterNullability { get; init; }
    public ApiPresence? ExpectedPresence { get; init; }
    public ApiQueryComposition? ExpectedComposition { get; init; }
    #endregion

    #region Observations
    private ApiDeclarationBuildTest RestoredCase { get; set; } = null!;
    private ApiSchemaCompilationResult ActualResult { get; set; } = null!;
    private string[] ActualTraversals { get; set; } = [];
    #endregion

    #region XUnitTest Methods
    protected override void Arrange()
    {
        this.WriteLine($"Scenario:         {this.Name}");
        this.WriteLine($"Build Expression: {this.BuildExpression}");
        this.WriteLine();
        this.WriteLine($"Expected Success:       {this.ExpectedSuccess}");
        this.WriteLine($"Expected Code:          {this.ExpectedCode}");
        this.WriteLine($"Expected API Path:      {this.ExpectedApiPath}");
        this.WriteLine($"Expected Sources:       {string.Join(",", this.ExpectedSourceNames ?? [])}");
        this.WriteLine($"Expected Relationships: {string.Join(",", this.ExpectedRelationshipNames ?? [])}");
        this.WriteLine($"Expected Traversals:    {string.Join(",", this.ExpectedTraversals ?? [])}");
        this.WriteLine($"Expected Parameters:    {string.Join(",", this.ExpectedParameterNames ?? [])}");
        this.WriteLine($"Expected Output Type:   {this.ExpectedOutputClrType}");
        this.WriteLine($"Expected Nullability:   {this.ExpectedOutputNullability}");
        this.WriteLine($"Expected Item Values:   {this.ExpectedItemNullability}");
        this.WriteLine($"Expected Presence:      {this.ExpectedPresence}");
        this.WriteLine("Expected Exception:     None");
        var serializer = new XUnitTestSerializer();
        this.RestoredCase = (ApiDeclarationBuildTest)serializer.Deserialize(this.GetType(), serializer.Serialize(this));
    }

    protected override void Act()
    {
        this.ActualResult = this.RestoredCase.BuildExpression.Compile()();
        this.WriteLine();
        this.WriteLine($"Actual Success: {this.ActualResult.IsValid}");
        foreach (var issue in this.ActualResult.Issues)
        {
            this.WriteLine($"Actual Issue:   {issue.Code} at {issue.ApiPath}: {issue.Description}");
        }
        if (this.ActualResult.Schema is { } schema)
        {
            this.ActualTraversals = [.. schema.ApiObjectTypes.SelectMany(static owner => owner.ApiRelationshipTraversals
                .Select(traversal => $"{owner.ApiName}.{traversal.ApiName}:{traversal.ClrNavigationMember?.ClrName}"))
                .Order(StringComparer.Ordinal)];
            this.WriteLine(
                $"Actual Sources:       {string.Join(",", schema.ApiQuerySources.Select(static source => source.ApiName))}");
            this.WriteLine(
                $"Actual Relationships: " +
                    $"{string.Join(",", schema.ApiRelationships.Select(static relationship => relationship.ApiName))}");
            this.WriteLine($"Actual Traversals:    {string.Join(",", this.ActualTraversals)}");
            foreach (var source in schema.ApiQuerySources)
            {
                this.WriteLine($"Actual Output:        {source.ApiOutputType.ClrType}, {source.ApiOutputNullability}");
                this.WriteLine($"Actual Composition:   {source.ApiOutputComposition}");
                foreach (var parameter in source.ApiInputParameters)
                {
                    this.WriteLine(
                        $"Actual Parameter:     {parameter.ApiName}, {parameter.ApiType.ClrType}" +
                            $", {parameter.ApiNullability}, {parameter.ApiPresence}");
                }
            }
        }
    }

    protected override void Assert()
    {
        this.ActualResult.IsValid.Should().Be(this.ExpectedSuccess);
        if (this.ExpectedCode is { } code)
        {
            this.ActualResult.Issues.Should().Contain(issue => issue.Code == code &&
                issue.Severity == ApiSchemaCompilationSeverity.Error &&
                (this.ExpectedApiPath == null || issue.ApiPath == this.ExpectedApiPath));
        }
        if (!this.ExpectedSuccess)
        {
            this.ActualResult.Schema.Should().BeNull();
            return;
        }
        var schema = this.ActualResult.Schema!;
        if (this.ExpectedSourceNames is not null)
        {
            schema.ApiQuerySources.Select(static source => source.ApiName).Should().Equal(this.ExpectedSourceNames);
        }
        if (this.ExpectedRelationshipNames is not null)
        {
            schema.ApiRelationships.Select(static relationship => relationship.ApiName).Should().Equal(this
                .ExpectedRelationshipNames);
        }
        if (this.ExpectedTraversals is not null)
        {
            this.ActualTraversals.Should().Equal(this.ExpectedTraversals);
        }
        if (this.ExpectedParameterNames is null && this.ExpectedParameterTypes is null &&
            this.ExpectedOutputClrType is null && this.ExpectedOutputNullability is null &&
            this.ExpectedItemNullability is null && this.ExpectedParameterNullability is null &&
            this.ExpectedPresence is null && this.ExpectedComposition is null)
        {
            return;
        }
        var source = schema.ApiQuerySources.Should().ContainSingle().Subject;
        if (this.ExpectedParameterNames is not null)
        {
            source.ApiInputParameters.Select(static parameter => parameter.ApiName).Should().Equal(this
                .ExpectedParameterNames);
        }
        if (this.ExpectedParameterTypes is not null)
        {
            source.ApiInputParameters.Select(static parameter => parameter.ApiType.ClrType).Should().Equal(this
                .ExpectedParameterTypes);
        }
        if (this.ExpectedOutputClrType is not null)
        {
            source.ApiOutputType.ClrType.Should().Be(this.ExpectedOutputClrType);
        }
        if (this.ExpectedOutputNullability is { } nullability)
        {
            source.ApiOutputNullability.Should().Be(nullability);
        }
        if (this.ExpectedItemNullability is { } itemNullability)
        {
            source.ApiOutputType.Should().BeOfType<ApiCollectionType>().Subject.ApiItemNullability.Should()
                .Be(itemNullability);
        }
        if (this.ExpectedParameterNullability is { } parameterNullability)
        {
            source.ApiInputParameters[0].ApiNullability.Should().Be(parameterNullability);
        }
        if (this.ExpectedPresence is { } presence)
        {
            source.ApiInputParameters[0].ApiPresence.Should().Be(presence);
        }
        if (this.ExpectedComposition is { } composition)
        {
            source.ApiOutputComposition.Should().Be(composition);
        }
    }
    #endregion
}
