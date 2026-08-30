// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.TestData;
using Evoogle.ApiFramework.TestData;
using Evoogle.XUnit;

using FluentAssertions;

using Microsoft.Extensions.Logging;

namespace Evoogle.ApiFramework.Schema.Internal;

public class ApiInitializationContextTests(ITestOutputHelper output) : XUnitTests(output)
{
    #region Test Types
    private sealed class ContextChainTest : XUnitTest
    {
        #region Calculated Properties
        private ApiInitializationContext? ChildContext { get; set; }
        private ApiInitializationContext? FreshRootContext { get; set; }
        private ApiInitializationContext? LeafContext { get; set; }
        private ObservingElement? LeafElement { get; set; }
        private ApiInitializationContext? RootContext { get; set; }
        private ContainerElement? RootElement { get; set; }
        private ApiInitializationContext? SiblingContext { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            var schema = CreateSchema("ContextChain");
            var session = new ApiInitializationSession(schema, new RecordingLogger());

            this.RootElement = new ContainerElement("Root");
            var childElement = new ContainerElement("Child");
            this.LeafElement = new ObservingElement("Leaf");
            var siblingElement = new ObservingElement("Sibling");

            this.RootContext = this.RootElement.Initialize(session);
            this.ChildContext = childElement.Initialize(this.RootContext);
            this.LeafContext = this.LeafElement.Initialize(this.ChildContext);
            this.SiblingContext = siblingElement.Initialize(this.RootContext);

            this.LeafContext.AddIssue
            (
                ApiInitializationSeverity.Warning,
                ApiInitializationCode.ApiObjectTypeNullOrEmptyProperties,
                "Leaf issue",
                remediation: null
            );

            var freshSession = new ApiInitializationSession(schema, new RecordingLogger());
            this.FreshRootContext = this.RootElement.Initialize(freshSession);
        }

        protected override void Assert()
        {
            this.LeafContext.Should().NotBeNull();
            this.LeafContext!.CurrentElement.Should().BeSameAs(this.LeafElement);
            this.LeafContext.Parent.Should().BeSameAs(this.ChildContext);
            this.LeafContext.ParentElement.Should().BeSameAs(this.ChildContext!.CurrentElement);
            this.LeafContext.Ancestors.Should().Equal
            (
                this.ChildContext.CurrentElement,
                this.RootElement!
            );

            this.LeafElement!.NearestContainer.Should().BeSameAs(this.ChildContext.CurrentElement);
            this.LeafElement.ObservedAncestors.Should().Equal
            (
                this.ChildContext.CurrentElement,
                this.RootElement!
            );

            this.SiblingContext!.Parent.Should().BeSameAs(this.RootContext);
            this.SiblingContext.Ancestors.Should().Equal(this.RootElement!);
            this.SiblingContext.Ancestors.Should().NotContain(this.LeafElement);

            this.RootContext!.Issues.Should().ContainSingle();
            this.ChildContext.Issues.Should().ContainSingle();
            this.LeafContext.Issues.Should().ContainSingle();
            this.FreshRootContext!.Issues.Should().BeEmpty();
            this.FreshRootContext.Session.Should().NotBeSameAs(this.RootContext.Session);
        }
        #endregion
    }

    private enum DiagnosticPathTestCase
    {
        KeyPath,
        OneToRelationship,
        ManyToManyRelationship,
        BlankIndexedLabel
    }

    private sealed class DiagnosticPathTest : XUnitTest
    {
        #region User Supplied Properties
        public required DiagnosticPathTestCase TestCase { get; init; }
        public required string[] ExpectedPaths { get; init; }
        #endregion

        #region Calculated Properties
        private string[]? ActualPaths { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        { }

        protected override void Act()
        {
            this.ActualPaths = this.TestCase switch
            {
                DiagnosticPathTestCase.KeyPath => GetKeyPaths(),
                DiagnosticPathTestCase.OneToRelationship => GetOneToRelationshipPaths(),
                DiagnosticPathTestCase.ManyToManyRelationship =>
                    GetManyToManyRelationshipPaths(),
                DiagnosticPathTestCase.BlankIndexedLabel => GetBlankIndexedLabelPath(),
                _ => throw new InvalidOperationException
                (
                    $"Unsupported {nameof(DiagnosticPathTestCase)} value '{this.TestCase}'."
                ),
            };
        }

        protected override void Assert()
        {
            this.ActualPaths.Should().Equal(this.ExpectedPaths);
        }
        #endregion
    }

    private sealed class IssueLoggingTest : XUnitTest
    {
        #region User Supplied Properties
        public required ApiInitializationCode Code { get; init; }
        public required LogLevel ExpectedLogLevel { get; init; }
        public required string? Remediation { get; init; }
        public required ApiInitializationSeverity Severity { get; init; }
        #endregion

        #region Calculated Properties
        private RecordingLogger? Logger { get; set; }
        private ApiInitializationSession? Session { get; set; }
        #endregion

        #region XUnitTest Methods
        protected override void Arrange()
        {
            this.Logger = new RecordingLogger();
            var schema = CreateSchema("IssueLogging");
            this.Session = new ApiInitializationSession(schema, this.Logger);
        }

        protected override void Act()
        {
            var element = new ObservingElement("Current");
            var context = element.Initialize(this.Session!);

            context.AddIssue
            (
                this.Severity,
                this.Code,
                "Description",
                this.Remediation
            );
        }

        protected override void Assert()
        {
            this.Session!.Issues.Should().ContainSingle();
            this.Logger!.Entries.Should().ContainSingle();

            var entry = this.Logger.Entries.Single();
            entry.LogLevel.Should().Be(this.ExpectedLogLevel);
            entry.EventId.Id.Should().Be((int)this.Code);
            entry.EventId.Name.Should().Be(this.Code.ToString());
            entry.Properties["InitializationCode"].Should().Be(this.Code);
            entry.Properties["ApiPath"].Should().Be
            (
                $"{nameof(ApiSchema)}[\"IssueLogging\"].TestElement[\"Current\"]"
            );
            entry.Properties["Description"].Should().Be("Description");

            if (this.Remediation is null)
            {
                entry.Properties.Should().NotContainKey("Remediation");
            }
            else
            {
                entry.Properties["Remediation"].Should().Be(this.Remediation);
            }
        }
        #endregion
    }

    private class TestElement(string apiLabel) : ApiSchemaElement
    {
        #region ApiSchemaElement Properties
        protected override string ApiElementName => "TestElement";
        #endregion

        #region ApiSchemaElement Methods
        protected override string BuildPath(string? apiPreviousPath)
            => ApiSchemaPathFormatting.BuildPath
            (
                apiPreviousPath,
                this.ApiElementName,
                apiLabel
            );
        #endregion
    }

    private sealed class ContainerElement(string apiLabel) : TestElement(apiLabel);

    private sealed class ObservingElement(string apiLabel) : TestElement(apiLabel)
    {
        #region Properties
        public ContainerElement? NearestContainer { get; private set; }

        public ApiSchemaElement[]? ObservedAncestors { get; private set; }
        #endregion

        #region ApiSchemaElement Methods
        internal override void InitializeCore(ApiInitializationContext context)
        {
            base.InitializeCore(context);

            context.TryGetNearestAncestor(out ContainerElement? nearestContainer);
            this.NearestContainer = nearestContainer;
            this.ObservedAncestors = [.. context.Ancestors];
        }
        #endregion
    }

    private sealed class RecordingLogger : ILogger
    {
        #region Properties
        public List<LogEntry> Entries { get; } = [];
        #endregion

        #region ILogger Methods
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>
        (
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            var properties = state is IEnumerable<KeyValuePair<string, object?>> values
                ? values.ToDictionary(static value => value.Key, static value => value.Value)
                : [];

            this.Entries.Add
            (
                new LogEntry(logLevel, eventId, properties, formatter(state, exception))
            );
        }
        #endregion
    }

    private sealed record LogEntry
    (
        LogLevel LogLevel,
        EventId EventId,
        IReadOnlyDictionary<string, object?> Properties,
        string Message
    );
    #endregion

    #region Test Data
    public static TheoryDataRow<IXUnitTest>[] ContextChainTheoryData =>
    [
        new ContextChainTest
        {
            Name = "Context Frames Preserve Ancestry And Isolate Branches"
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] DiagnosticPathTheoryData =>
    [
        new DiagnosticPathTest
        {
            Name = "Key Paths Are Fully Qualified And Indexed",
            TestCase = DiagnosticPathTestCase.KeyPath,
            ExpectedPaths =
            [
                "ApiSchema[\"Key\"].ApiObjectType[\"KeyNestedComposite\"]." +
                    "ApiNamedKeyType[\"PK_KeyNestedComposite\"]." +
                    "ApiKeyPath[0][\"KeyNestedComposite.NestedPart.Id\"]",
                "ApiSchema[\"Key\"].ApiObjectType[\"KeyNestedComposite\"]." +
                    "ApiNamedKeyType[\"PK_KeyNestedComposite\"]." +
                    "ApiKeyPath[0][\"KeyNestedComposite.NestedPart.Id\"]." +
                    "ApiKeyPathSegment[0][\"NestedPart\"]",
                "ApiSchema[\"Key\"].ApiObjectType[\"KeyNestedComposite\"]." +
                    "ApiNamedKeyType[\"PK_KeyNestedComposite\"]." +
                    "ApiKeyPath[0][\"KeyNestedComposite.NestedPart.Id\"]." +
                    "ApiKeyPathSegment[1][\"Id\"]",
                "ApiSchema[\"Key\"].ApiObjectType[\"KeyNestedComposite\"]." +
                    "ApiNamedKeyType[\"PK_KeyNestedComposite\"]." +
                    "ApiKeyPath[1][\"KeyNestedComposite.Name\"]",
            ]
        },
        new DiagnosticPathTest
        {
            Name = "One-To Relationship Paths Use Semantic Roles",
            TestCase = DiagnosticPathTestCase.OneToRelationship,
            ExpectedPaths =
            [
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipOneToMany[\"REL_User_Post_1toN_ViaScalar\"]." +
                    "ApiPrincipalEnd",
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipOneToMany[\"REL_User_Post_1toN_ViaScalar\"]." +
                    "ApiDependentEnd",
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipOneToMany[\"REL_User_Post_1toN_ViaScalar\"]." +
                    "ApiDependentEnd.ApiForeignKeyType",
            ]
        },
        new DiagnosticPathTest
        {
            Name = "Many-To-Many Relationship Paths Use Semantic Roles",
            TestCase = DiagnosticPathTestCase.ManyToManyRelationship,
            ExpectedPaths =
            [
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipManyToMany[\"REL_Post_Tag_NtoN_ViaPostTag\"]." +
                    "ApiPrincipalEndA",
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipManyToMany[\"REL_Post_Tag_NtoN_ViaPostTag\"]." +
                    "ApiPrincipalEndB",
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipManyToMany[\"REL_Post_Tag_NtoN_ViaPostTag\"]." +
                    "ApiAssociation",
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipManyToMany[\"REL_Post_Tag_NtoN_ViaPostTag\"]." +
                    "ApiAssociation.ApiForeignKeyTypeA",
                "ApiSchema[\"Relationship\"]." +
                    "ApiRelationshipManyToMany[\"REL_Post_Tag_NtoN_ViaPostTag\"]." +
                    "ApiAssociation.ApiForeignKeyTypeB",
            ]
        },
        new DiagnosticPathTest
        {
            Name = "Indexed Paths Omit Blank Labels",
            TestCase = DiagnosticPathTestCase.BlankIndexedLabel,
            ExpectedPaths =
            [
                "ApiSchema[\"Location\"].TestElement[\"Root\"].TestElement[2]",
            ]
        },
    ];

    public static TheoryDataRow<IXUnitTest>[] IssueLoggingTheoryData =>
    [
        new IssueLoggingTest
        {
            Name = "Information Issues Are Logged Without Remediation",
            Severity = ApiInitializationSeverity.Info,
            Code = ApiInitializationCode.ApiSchemaInvalidName,
            ExpectedLogLevel = LogLevel.Information,
            Remediation = null
        },
        new IssueLoggingTest
        {
            Name = "Warning Issues Are Logged With Remediation",
            Severity = ApiInitializationSeverity.Warning,
            Code = ApiInitializationCode.ApiObjectTypeNullOrEmptyProperties,
            ExpectedLogLevel = LogLevel.Warning,
            Remediation = "Remediation"
        },
        new IssueLoggingTest
        {
            Name = "Error Issues Are Logged With Remediation",
            Severity = ApiInitializationSeverity.Error,
            Code = ApiInitializationCode.ApiKeyTypeNullOrEmptyPaths,
            ExpectedLogLevel = LogLevel.Error,
            Remediation = "Remediation"
        },
    ];
    #endregion

    #region Test Methods
    [Theory]
    [MemberData(nameof(ContextChainTheoryData))]
    public void ContextChain(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(DiagnosticPathTheoryData))]
    public void DiagnosticPath(IXUnitTest test) => test.Execute(this);

    [Theory]
    [MemberData(nameof(IssueLoggingTheoryData))]
    public void IssueLogging(IXUnitTest test) => test.Execute(this);
    #endregion

    #region Implementation Methods
    private static ApiSchema CreateSchema(string apiName)
    {
        var schema = new ApiSchema
        (
            apiName,
            apiVersion: null,
            apiOptions: null,
            apiNamedTypes: [],
            apiRelationships: []
        );

        schema.Initialize().ThrowIfInvalid();
        return schema;
    }

    private static string[] GetBlankIndexedLabelPath()
    {
        var schema = CreateSchema("Location");
        var session = new ApiInitializationSession(schema, new RecordingLogger());
        var root = new ContainerElement("Root");
        var child = new ObservingElement("Child");

        var rootContext = root.Initialize(session);
        var location = ApiInitializationLocation.ForIndexedLabel(2, "   ");
        var childContext = child.Initialize(rootContext, location);

        return [childContext.ApiPath];
    }

    private static string[] GetKeyPaths()
    {
        var apiObjectType = ApiSchemaFactory.KeyApiSchema.GetObjectTypeByApiName
        (
            nameof(KeyNestedComposite)
        );
        apiObjectType.TryGetKeyTypeByApiName
        (
            "PK_KeyNestedComposite",
            out var apiKeyType
        ).Should().BeTrue();

        var firstPath = apiKeyType!.ApiKeyPaths[0];
        var secondPath = apiKeyType.ApiKeyPaths[1];
        return
        [
            firstPath.ApiPath,
            firstPath.ApiSegments[0].ApiPath,
            firstPath.ApiSegments[1].ApiPath,
            secondPath.ApiPath,
        ];
    }

    private static string[] GetManyToManyRelationshipPaths()
    {
        ApiSchemaFactory.RelationshipApiSchema.TryGetRelationshipByApiName
        (
            "REL_Post_Tag_NtoN_ViaPostTag",
            out var apiRelationship
        ).Should().BeTrue();

        var manyToMany = apiRelationship.Should().BeOfType<ApiRelationshipManyToMany>().Subject;
        return
        [
            manyToMany.ApiPrincipalEndA.ApiPath,
            manyToMany.ApiPrincipalEndB.ApiPath,
            manyToMany.ApiAssociation.ApiPath,
            manyToMany.ApiAssociation.ApiForeignKeyTypeA.ApiPath,
            manyToMany.ApiAssociation.ApiForeignKeyTypeB.ApiPath,
        ];
    }

    private static string[] GetOneToRelationshipPaths()
    {
        ApiSchemaFactory.RelationshipApiSchema.TryGetRelationshipByApiName
        (
            "REL_User_Post_1toN_ViaScalar",
            out var apiRelationship
        ).Should().BeTrue();

        var oneTo = apiRelationship.Should().BeOfType<ApiRelationshipOneToMany>().Subject;
        return
        [
            oneTo.ApiPrincipalEnd.ApiPath,
            oneTo.ApiDependentEnd.ApiPath,
            oneTo.ApiDependentEnd.ApiForeignKeyType.ApiPath,
        ];
    }
    #endregion
}
