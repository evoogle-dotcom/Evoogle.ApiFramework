// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using System.Collections.Frozen;
using System.Reflection;
using System.Text.Json;

using Evoogle.ApiFramework.Exceptions;
using Evoogle.ApiFramework.Schema.Configuration;
using Evoogle.Extension;
using Evoogle.NTree;
using Evoogle.XUnit;

using FluentAssertions;

namespace Evoogle.ApiFramework.Schema;

public class ApiSchemaFrozenLifecycleTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private enum FrozenStatus
    {
        Active = 1
    }

    private sealed class FrozenEntity
    {
        public int Id { get; set; }
    }

    private sealed class EmptyEntity;

    private sealed class SnapshotExtension : IApiSchemaExtension
    {
        public int Value { get; set; }

        public IApiSchemaExtension CreateFrozenSnapshot() => new SnapshotExtension
        {
            Value = this.Value
        };
    }

    private sealed class FailingExtension : IApiSchemaExtension
    {
        public IApiSchemaExtension CreateFrozenSnapshot() =>
            throw new InvalidOperationException("Snapshot failure.");
    }

    private sealed class ReusingExtension : IApiSchemaExtension
    {
        public IApiSchemaExtension CreateFrozenSnapshot() => this;
    }

    private sealed class PublicSurfaceTest : XUnitTest
    {
        protected override void Arrange()
        { }

        protected override void Act()
        { }

        protected override void Assert()
        {
            typeof(ApiSchema).GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Should().BeEmpty();
            typeof(ApiSchema).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Should().NotContain(method => method.Name == "Compile");
            typeof(ApiSchema).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Should().NotContain(method => method.Name == "Create");

            typeof(ApiKeyType).GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Should().BeEmpty();
            typeof(ApiKeyType).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .Should().ContainSingle(constructor => constructor.IsAssembly);
            typeof(ApiSchemaOptions).IsSealed.Should().BeTrue();
            typeof(ApiObjectTypeOptions).IsSealed.Should().BeTrue();
        }
    }

    private sealed class BuildResultTest : XUnitTest
    {
        private ApiSchemaCompilationResult? ErrorResult { get; set; }

        private ApiSchemaCompilationResult? SuccessResult { get; set; }

        private ApiSchemaCompilationResult? WarningResult { get; set; }

        private ApiSchemaCompilationException? Exception { get; set; }

        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.SuccessResult = CreateBuilder("Success").BuildResult();
            this.WarningResult = new ApiSchemaBuilder()
                .WithName("Warning")
                .AddObject<EmptyEntity>()
                .BuildResult();

            var invalidBuilder = new ApiSchemaBuilder();
            this.ErrorResult = invalidBuilder.BuildResult();
            try
            {
                invalidBuilder.Build();
            }
            catch (ApiSchemaCompilationException exception)
            {
                this.Exception = exception;
            }
        }

        protected override void Assert()
        {
            this.SuccessResult!.IsValid.Should().BeTrue();
            this.SuccessResult.Schema.Should().NotBeNull();
            this.SuccessResult.Issues.IsDefault.Should().BeFalse();
            this.SuccessResult.Errors.IsDefault.Should().BeFalse();
            this.SuccessResult.Warnings.IsDefault.Should().BeFalse();

            this.WarningResult!.IsValid.Should().BeTrue();
            this.WarningResult.Schema.Should().NotBeNull();
            this.WarningResult.HasWarnings.Should().BeTrue();

            this.ErrorResult!.IsValid.Should().BeFalse();
            this.ErrorResult.Schema.Should().BeNull();
            this.ErrorResult.HasErrors.Should().BeTrue();
            this.ErrorResult.Errors.Should().Contain
            (
                issue => issue.Code == ApiSchemaCompilationCode.ApiSchemaInvalidName
            );
            this.Exception.Should().NotBeNull();
            this.Exception!.Result.Schema.Should().BeNull();
            this.Exception.Result.Errors.Should().Contain
            (
                issue => issue.Code == ApiSchemaCompilationCode.ApiSchemaInvalidName
            );
        }
    }

    private sealed class RepeatedBuildTest : XUnitTest
    {
        private SnapshotExtension? ExtensionSource { get; set; }

        private ApiSchema? FirstSchema { get; set; }

        private ApiSchema? SecondSchema { get; set; }

        protected override void Arrange()
        {
            this.ExtensionSource = new SnapshotExtension { Value = 42 };
        }

        protected override void Act()
        {
            var builder = CreateBuilder("Repeated")
                .AddSchemaExtension(this.ExtensionSource!);
            this.FirstSchema = builder.Build();
            this.SecondSchema = builder.Build();
            this.ExtensionSource!.Value = 84;
        }

        protected override void Assert()
        {
            this.FirstSchema.Should().NotBeSameAs(this.SecondSchema);
            this.FirstSchema!.ApiSchemaContext.Should().NotBeSameAs(this.SecondSchema!.ApiSchemaContext);

            var firstElements = this.FirstSchema.SelfAndDescendants(TraversalStrategy.DepthFirst).ToArray();
            var secondElements = this.SecondSchema.SelfAndDescendants(TraversalStrategy.DepthFirst).ToArray();
            firstElements.Should().HaveSameCount(secondElements);
            firstElements.Zip(secondElements).Should().OnlyContain(pair => !ReferenceEquals(pair.First, pair.Second));

            foreach (var lookupField in GetLookupFields(typeof(ApiSchema)))
            {
                lookupField.GetValue(this.FirstSchema).Should()
                    .NotBeSameAs(lookupField.GetValue(this.SecondSchema));
            }

            var firstObjectType = this.FirstSchema.ApiObjectTypes.Single();
            var secondObjectType = this.SecondSchema.ApiObjectTypes.Single();
            foreach (var lookupField in GetLookupFields(typeof(ApiObjectType)))
            {
                lookupField.GetValue(firstObjectType).Should()
                    .NotBeSameAs(lookupField.GetValue(secondObjectType));
            }

            var firstEnumType = this.FirstSchema.ApiEnumTypes.Single();
            var secondEnumType = this.SecondSchema.ApiEnumTypes.Single();
            foreach (var lookupField in GetLookupFields(typeof(ApiEnumType)))
            {
                lookupField.GetValue(firstEnumType).Should()
                    .NotBeSameAs(lookupField.GetValue(secondEnumType));
            }

            this.FirstSchema.TryGetExtension<SnapshotExtension>(out var firstExtension).Should().BeTrue();
            this.SecondSchema.TryGetExtension<SnapshotExtension>(out var secondExtension).Should().BeTrue();
            firstExtension.Should().NotBeSameAs(this.ExtensionSource);
            secondExtension.Should().NotBeSameAs(this.ExtensionSource);
            firstExtension.Should().NotBeSameAs(secondExtension);
            firstExtension!.Value.Should().Be(42);
            secondExtension!.Value.Should().Be(42);

            Action attach = () => this.FirstSchema.AttachExtension(typeof(Uri), new Uri("https://example.com"));
            Action detach = () => this.FirstSchema.DetachExtension<SnapshotExtension>();
            Action modify = () => this.FirstSchema.ModifyExtension<SnapshotExtension>(extension => extension.Value++);
            attach.Should().Throw<InvalidOperationException>();
            detach.Should().Throw<InvalidOperationException>();
            modify.Should().Throw<InvalidOperationException>();
        }
    }

    private sealed class InvalidExtensionTest : XUnitTest
    {
        private ApiSchemaCompilationResult[]? Results { get; set; }

        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.Results =
            [
                CreateBuilder("Unsupported")
                    .AddSchemaExtension(typeof(object), new object())
                    .BuildResult(),
                CreateBuilder("Failed")
                    .AddSchemaExtension(typeof(FailingExtension), new FailingExtension())
                    .BuildResult(),
                CreateBuilder("Reused")
                    .AddSchemaExtension(typeof(ReusingExtension), new ReusingExtension())
                    .BuildResult()
            ];
        }

        protected override void Assert()
        {
            this.Results.Should().OnlyContain(result => result.Schema == null);
            this.Results![0].Errors.Should().ContainSingle
            (
                issue => issue.Code == ApiSchemaCompilationCode.ApiSchemaExtensionUnsupported
            );
            this.Results[1].Errors.Should().ContainSingle
            (
                issue => issue.Code == ApiSchemaCompilationCode.ApiSchemaExtensionSnapshotFailed
            );
            this.Results[2].Errors.Should().ContainSingle
            (
                issue => issue.Code == ApiSchemaCompilationCode.ApiSchemaExtensionInvalidSnapshot
            );
        }
    }

    private sealed class FrozenLookupAndConcurrencyTest : XUnitTest
    {
        private ApiSchema? Schema { get; set; }

        private ApiSchema? ReplacementSchema { get; set; }

        private FieldInfo[]? LookupFields { get; set; }

        private bool? ConcurrentReadsSucceeded { get; set; }

        private ApiSchemaElement[]? OriginalElements { get; set; }

        protected override void Arrange()
        {
            this.Schema = CreateBuilder("Concurrent")
                .AddSchemaExtension(new SnapshotExtension { Value = 42 })
                .Build();
            this.LookupFields =
            [
                .. GetLookupFields(typeof(ApiSchema)),
                .. GetLookupFields(typeof(ApiObjectType)),
                .. GetLookupFields(typeof(ApiEnumType))
            ];
        }

        protected override void Act()
        {
            var schema = this.Schema!;
            this.OriginalElements = schema
                .SelfAndDescendants(TraversalStrategy.DepthFirst)
                .ToArray();
            var objectType = schema.ApiObjectTypes.Single();
            var keyType = objectType.ApiKeyTypes.Single();
            var replacementTask = Task.Run(() => CreateBuilder("Replacement").Build());

            this.ConcurrentReadsSucceeded = Task.WhenAll
            (
                Enumerable.Range(0, 32).Select
                (
                    taskIndex => Task.Run
                    (
                        () => Enumerable.Range(0, 100).All
                        (
                            iteration =>
                            {
                                var traversed = schema
                                    .SelfAndDescendants(TraversalStrategy.DepthFirst)
                                    .ToArray();
                                var serialized = JsonSerializer.Serialize(schema);
                                var materializedKey = keyType.MaterializeKey
                                (
                                    new ApiKeyMaterializationContext().With
                                    (
                                        new FrozenEntity { Id = iteration }
                                    )
                                );
                                var typeFound = schema.TryGetTypeByClrType(typeof(int), out var apiType) &&
                                    apiType is not null;
                                var relationshipFound = schema.TryGetRelationshipByApiName
                                (
                                    "REL_FrozenEntity_FrozenEntity",
                                    out var relationship
                                ) && relationship is not null;
                                var extensionFound = schema.TryGetExtension<SnapshotExtension>
                                (
                                    out var extension
                                ) && extension.Value == 42;
                                var keyMatches = materializedKey.IsComposite;
                                var traversalMatches = traversed.SequenceEqual(this.OriginalElements);
                                var serializedSuccessfully = serialized.Length > 0;

                                if (!typeFound || !relationshipFound || !extensionFound || !keyMatches ||
                                    !traversalMatches || !serializedSuccessfully)
                                {
                                    throw new InvalidOperationException
                                    (
                                        $"Concurrent read failed: type={typeFound}, relationship={relationshipFound}, extension={extensionFound}, key={keyMatches}, traversal={traversalMatches}, json={serializedSuccessfully}."
                                    );
                                }

                                return true;
                            }
                        )
                    )
                )
            ).GetAwaiter().GetResult().All(result => result);
            this.ReplacementSchema = replacementTask.GetAwaiter().GetResult();
        }

        protected override void Assert()
        {
            this.LookupFields.Should().HaveCount(15);
            this.LookupFields.Should().OnlyContain
            (
                field => field.FieldType.IsGenericType &&
                    field.FieldType.GetGenericTypeDefinition() == typeof(FrozenDictionary<,>)
            );

            foreach (var field in GetLookupFields(typeof(ApiSchema)))
            {
                field.GetValue(this.Schema).Should().NotBeNull();
            }

            var objectType = this.Schema!.ApiObjectTypes.First(type => type.ApiKeyTypes.Length > 0);
            foreach (var field in GetLookupFields(typeof(ApiObjectType)))
            {
                field.GetValue(objectType).Should().NotBeNull();
            }

            var enumType = this.Schema.ApiEnumTypes.First();
            foreach (var field in GetLookupFields(typeof(ApiEnumType)))
            {
                field.GetValue(enumType).Should().NotBeNull();
            }

            objectType.ApiKeyTypeApiNames.IsDefault.Should().BeFalse();
            objectType.ApiKeyTypeApiNames.Should().Equal(objectType.ApiKeyTypes.Select(type => type.ApiName));
            this.ConcurrentReadsSucceeded.Should().BeTrue();
            this.ReplacementSchema.Should().NotBeSameAs(this.Schema);
            this.Schema.SelfAndDescendants(TraversalStrategy.DepthFirst).Should().Equal(this.OriginalElements!);
        }

        private static FieldInfo[] GetLookupFields(Type type) =>
        [
            .. type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(field => field.Name.EndsWith("Lookup", StringComparison.Ordinal))
        ];
    }
    #endregion

    #region Theory Data
    public static TheoryDataRow<IXUnitTest>[] FrozenLifecycleTheoryData =>
    [
        new PublicSurfaceTest { Name = "Schema Runtime Has A Closed One-Shot Public Surface" },
        new BuildResultTest { Name = "Build Results Publish Only Valid Frozen Schemas" },
        new RepeatedBuildTest { Name = "Repeated Builder Builds Produce Independent Frozen Graphs" },
        new InvalidExtensionTest { Name = "Invalid Extension Snapshots Produce Build Diagnostics" },
        new FrozenLookupAndConcurrencyTest { Name = "Frozen Lookup Tables Support Concurrent Runtime Reads" }
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(FrozenLifecycleTheoryData))]
    public void FrozenLifecycle(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Implementation Methods
    private static ApiSchemaBuilder CreateBuilder(string apiName) => new ApiSchemaBuilder()
        .WithName(apiName)
        .AddScalar<int>()
        .AddEnum<FrozenStatus>(builder => builder.AddAllValues())
        .AddObject<FrozenEntity>
        (
            builder => builder
                .AddProperty(entity => entity.Id)
                .AddKey("PK_FrozenEntity", entity => entity.Id)
        )
        .AddOneToManyRelationship
        (
            "REL_FrozenEntity_FrozenEntity",
            builder => builder
                .From<FrozenEntity>()
                .To<FrozenEntity>(end => end.WithForeignKey(entity => entity.Id))
        );

    private static FieldInfo[] GetLookupFields(Type type) =>
    [
        .. type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(field => field.Name.EndsWith("Lookup", StringComparison.Ordinal))
    ];
    #endregion
}
