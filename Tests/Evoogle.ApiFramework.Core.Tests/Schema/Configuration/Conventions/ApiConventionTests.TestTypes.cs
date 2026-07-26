// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
using Evoogle.ApiFramework.Schema.Annotations;

namespace Evoogle.ApiFramework.Schema.Configuration.Conventions;

public partial class ApiConventionTests
{
    #region Test Domain Types
    internal class PersonWithId
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
    }

    internal class OrderWithPersonId
    {
        public Guid OrderId { get; set; }
        public Guid PersonId { get; set; }
        public decimal Total { get; set; }
    }

    internal record struct CustomScalar(string Value);

    internal enum CustomEnum
    {
        Active,
    }

    internal enum PipelineStatus
    {
        Active,
        InProgress,
        OnHold,
        Queued,
    }

    [ApiObjectType]
    public class AssemblyScannedObject
    {
        public Guid Id { get; set; }
    }

    [ApiScalarType]
    public readonly record struct AssemblyScannedScalar(string Value);

    [ApiEnumType]
    public enum AssemblyScannedEnum
    {
        Active,
    }

    // Named "OrderItem" so that "OrderItemId" triggers the {ClassName}Id convention.
    internal class OrderItem
    {
        public Guid OrderItemId { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    internal class TypeWithField
    {
        public Guid Id = Guid.Empty;
        public string Name = string.Empty;
        public int? Count = null;
    }

    internal class ConventionLoopRoot
    {
        public int Id { get; set; }
    }

    internal class ConventionLoopType<T>
    {
        public T? Next { get; set; }
    }

    internal class PropertyConventionTarget
    {
        public int Initial { get; set; }
        public int Added { get; set; }
    }

    internal class PropertyConventionTrigger
    {
        public int Trigger { get; set; }
    }

    internal class PropertyConventionRegistered
    {
        public int Id { get; set; }
    }
    #endregion

    #region Test Result Types
    public sealed record ApiConventionBuildTrace
    (
        ApiSchema ApiSchema,
        IReadOnlyList<string> Events
    );

    public sealed record EnumValueNamingContextSnapshot
    (
        bool IsEnumTypeRegistered,
        ApiNamingConventionTarget Target,
        Type ClrType,
        string ClrName,
        bool HasPropertyContext,
        Type ClrEnumType,
        string ClrMemberName,
        Type ApiEnumTypeBuilderClrType,
        bool HasApiSchemaBuilder
    );

    public sealed record ApiConventionSetSnapshot
    (
        bool AddedConventionIsRegistered,
        bool CopiedConventionIsRegistered,
        bool RemovedConventionIsAbsent,
        bool DefaultConventionIsRegistered
    );
    #endregion
}
