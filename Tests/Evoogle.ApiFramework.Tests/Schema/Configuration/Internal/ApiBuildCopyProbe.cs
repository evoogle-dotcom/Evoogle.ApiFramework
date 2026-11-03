// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Evoogle.ApiFramework.Schema.Configuration.Internal;

/// <summary>
///     Verifies that internal <c>CreateBuildCopy</c> methods copy every instance field of the builder and
///     state types that declare them. Each probed instance has every field populated with a non-default
///     sample, so a field that a copy method forgets either surfaces as a dropped value or as a shared
///     reference to mutable authoring state.
/// </summary>
internal static class ApiBuildCopyProbe
{
    #region Constants
    private const string _copyMethodName = "CreateBuildCopy";

    private static readonly BindingFlags _instanceFlags =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    // These roots rebuild their copies through constructors that cannot accept reflection-populated fields.
    private static readonly HashSet<Type> _excludedTypes =
    [
        typeof(ApiSchemaBuilderContext),
        typeof(ApiSchemaBuilderState),
    ];
    #endregion

    #region Discovery Methods
    /// <summary>Gets the concrete production types that declare a supported <c>CreateBuildCopy</c> method.</summary>
    internal static IReadOnlyList<Type> GetCopyableTypes()
        => [.. typeof(ApiSchemaBuilder).Assembly
            .GetTypes()
            .Where(static type => type is { IsAbstract: false, IsGenericTypeDefinition: false })
            .Where(static type => !_excludedTypes.Contains(type))
            .Where(static type => FindCopyMethod(type) is not null)
            .OrderBy(static type => type.FullName, StringComparer.Ordinal)];

    private static MethodInfo? FindCopyMethod(Type type)
        => type
            .GetMethods(_instanceFlags | BindingFlags.DeclaredOnly)
            .SingleOrDefault(static method =>
                method.Name == _copyMethodName && IsSupportedCopySignature(method.GetParameters()));

    private static bool IsSupportedCopySignature(ParameterInfo[] parameters)
        => parameters.Length == 0 ||
            (parameters.Length == 1 && parameters[0].ParameterType == typeof(ApiSchemaBuilderContext));
    #endregion

    #region Probe Methods
    /// <summary>Copies a fully populated instance of the type and returns every copy defect found.</summary>
    /// <param name="copyType">The type that declares <c>CreateBuildCopy</c>.</param>
    /// <param name="onWriteLine">Receives diagnostic output describing the probe.</param>
    internal static IReadOnlyList<string> Probe(Type copyType, Action<string> onWriteLine)
    {
        var copyMethod = FindCopyMethod(copyType)
            ?? throw new InvalidOperationException($"{copyType} does not declare a supported {_copyMethodName}.");

        var originalContext = (ApiSchemaBuilderContext)RuntimeHelpers.GetUninitializedObject(
            typeof(ApiSchemaBuilderContext));
        var copyContext = (ApiSchemaBuilderContext)RuntimeHelpers.GetUninitializedObject(
            typeof(ApiSchemaBuilderContext));

        var populator = new SamplePopulator(originalContext);
        var original = populator.CreatePopulatedInstance(copyType);
        object?[] arguments = copyMethod.GetParameters().Length == 0 ? [] : [copyContext];
        var copy = copyMethod.Invoke(original, arguments)!;

        onWriteLine($"Populated Fields: {populator.PopulatedFieldCount}");
        onWriteLine($"Copy Type:        {copy.GetType()}");

        var defects = new List<string>();
        var comparer = new CopyComparer(originalContext, copyContext, defects);
        comparer.CompareFields(original, copy, copyType.Name);
        return defects;
    }
    #endregion

    #region Reflection Methods
    private static IEnumerable<FieldInfo> GetInstanceFields(Type type)
    {
        for (var current = type; current is not null && current != typeof(object); current = current.BaseType)
        {
            foreach (var field in current.GetFields(_instanceFlags | BindingFlags.DeclaredOnly))
            {
                yield return field;
            }
        }
    }

    private static bool IsProductionType(Type type)
        => type.Assembly == typeof(ApiSchemaBuilder).Assembly;

    private static bool IsCollection(Type type)
        => type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);

    // Builders, state holders, and source scopes are authoring state that a copy must not share.
    private static bool IsMutableAuthoringState(Type type)
        => IsProductionType(type) &&
            !type.IsValueType &&
            (type.Name.EndsWith("Builder", StringComparison.Ordinal) ||
                type.Name.EndsWith("State", StringComparison.Ordinal) ||
                type.Name.EndsWith("Scope", StringComparison.Ordinal));

    private static bool IsValueLike(Type type)
        => type.IsValueType || type == typeof(string) || type == typeof(Type) ||
            typeof(Delegate).IsAssignableFrom(type);
    #endregion

    #region Sample Populator
    private sealed class SamplePopulator(ApiSchemaBuilderContext context)
    {
        private readonly HashSet<Type> _activeTypes = [];

        internal int PopulatedFieldCount { get; private set; }

        internal object CreatePopulatedInstance(Type type)
        {
            var instance = RuntimeHelpers.GetUninitializedObject(type);
            this.PopulateFields(instance, type);
            return instance;
        }

        private void PopulateFields(object instance, Type type)
        {
            _activeTypes.Add(type);
            foreach (var field in GetInstanceFields(type))
            {
                var sample = this.CreateSample(field.FieldType);
                if (sample is not null)
                {
                    field.SetValue(instance, sample);
                    this.PopulatedFieldCount++;
                }
            }

            _activeTypes.Remove(type);
        }

        private object? CreateSample(Type type)
        {
            if (type == typeof(ApiSchemaBuilderContext))
            {
                return context;
            }

            if (type == typeof(string))
            {
                return "sample";
            }

            if (type == typeof(Type))
            {
                return typeof(SamplePopulator);
            }

            if (type == typeof(object))
            {
                return new object();
            }

            if (Nullable.GetUnderlyingType(type) is Type underlyingType)
            {
                return this.CreateSample(underlyingType);
            }

            if (type.IsEnum)
            {
                return Enum.GetValues(type).Cast<object>().Max();
            }

            if (type.IsPrimitive)
            {
                return type == typeof(bool) ? true : type == typeof(char) ? 'x' : Convert.ChangeType(1, type);
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            if (typeof(Delegate).IsAssignableFrom(type))
            {
                return CreateDelegate(type);
            }

            if (IsCollection(type))
            {
                return this.CreateCollection(type);
            }

            if (type.IsAbstract || type.IsInterface || !IsProductionType(type) || _activeTypes.Contains(type))
            {
                return null;
            }

            return this.CreatePopulatedInstance(type);
        }

        private object? CreateCollection(Type type)
        {
            var concreteType = type;
            if (type.IsInterface)
            {
                var elementType = type.GetGenericArguments().FirstOrDefault();
                if (elementType is null || !type.IsAssignableFrom(typeof(List<>).MakeGenericType(elementType)))
                {
                    return null;
                }

                concreteType = typeof(List<>).MakeGenericType(elementType);
            }

            if (concreteType.GetConstructor(Type.EmptyTypes) is null)
            {
                return null;
            }

            var collection = Activator.CreateInstance(concreteType)!;
            var genericArguments = concreteType.GetGenericArguments();
            var addMethod = concreteType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(method =>
                    method.Name is "Add" or "Enqueue" && method.GetParameters().Length == genericArguments.Length);

            if (addMethod is not null && genericArguments.Length is 1 or 2)
            {
                var arguments = genericArguments.Select(this.CreateSample).ToArray();
                if (arguments.All(static argument => argument is not null))
                {
                    addMethod.Invoke(collection, arguments);
                }
            }

            return collection;
        }

        private static Delegate CreateDelegate(Type delegateType)
        {
            var invokeMethod = delegateType.GetMethod("Invoke")!;
            var parameters = invokeMethod.GetParameters()
                .Select(static parameter => Expression.Parameter(parameter.ParameterType))
                .ToArray();
            var body = invokeMethod.ReturnType == typeof(void)
                ? (Expression)Expression.Empty()
                : Expression.Default(invokeMethod.ReturnType);
            return Expression.Lambda(delegateType, body, parameters).Compile();
        }
    }
    #endregion

    #region Copy Comparer
    private sealed class CopyComparer
    (
        ApiSchemaBuilderContext originalContext,
        ApiSchemaBuilderContext copyContext,
        List<string> defects
    )
    {
        private readonly HashSet<(object Original, object Copy)> _comparedPairs =
            new(ReferencePairComparer.Instance);

        internal void CompareFields(object original, object copy, string path)
        {
            if (!_comparedPairs.Add((original, copy)))
            {
                return;
            }

            foreach (var field in GetInstanceFields(original.GetType()))
            {
                var fieldPath = $"{path}.{field.Name}";
                var originalField = field.GetValue(original);
                var copyField = field.GetValue(copy);

                if (originalField is null)
                {
                    continue;
                }

                if (copyField is null)
                {
                    defects.Add($"{fieldPath}: populated in the original but null in the copy.");
                    continue;
                }

                this.CompareField(field.FieldType, originalField, copyField, fieldPath);
            }
        }

        private void CompareField(Type fieldType, object originalField, object copyField, string path)
        {
            if (ReferenceEquals(originalField, originalContext))
            {
                if (!ReferenceEquals(copyField, copyContext))
                {
                    defects.Add($"{path}: must reference the context supplied to the copy method.");
                }

                return;
            }

            if (IsValueLike(fieldType))
            {
                if (!Equals(originalField, copyField))
                {
                    defects.Add($"{path}: expected '{originalField}' but copied '{copyField}'.");
                }

                return;
            }

            if (IsCollection(fieldType))
            {
                this.CompareCollection(originalField, copyField, path);
                return;
            }

            if (IsMutableAuthoringState(originalField.GetType()))
            {
                this.CompareMutableState(originalField, copyField, path);
            }
        }

        private void CompareMutableState(object originalState, object copyState, string path)
        {
            if (ReferenceEquals(originalState, copyState))
            {
                defects.Add($"{path}: shares mutable authoring state with the original.");
                return;
            }

            this.CompareFields(originalState, copyState, path);
        }

        private void CompareCollection(object originalCollection, object copyCollection, string path)
        {
            if (ReferenceEquals(originalCollection, copyCollection))
            {
                defects.Add($"{path}: shares a mutable collection with the original.");
                return;
            }

            var originalElements = ((IEnumerable)originalCollection).Cast<object>().ToList();
            var copyElements = ((IEnumerable)copyCollection).Cast<object>().ToList();
            if (originalElements.Count != copyElements.Count)
            {
                defects.Add($"{path}: expected {originalElements.Count} elements but copied {copyElements.Count}.");
                return;
            }

            for (var index = 0; index < originalElements.Count; index++)
            {
                if (IsMutableAuthoringState(originalElements[index].GetType()))
                {
                    this.CompareMutableState(originalElements[index], copyElements[index], $"{path}[{index}]");
                }
            }
        }
    }

    private sealed class ReferencePairComparer : IEqualityComparer<(object Original, object Copy)>
    {
        internal static ReferencePairComparer Instance { get; } = new();

        public bool Equals((object Original, object Copy) left, (object Original, object Copy) right)
            => ReferenceEquals(left.Original, right.Original) && ReferenceEquals(left.Copy, right.Copy);

        public int GetHashCode((object Original, object Copy) pair)
            => HashCode.Combine(RuntimeHelpers.GetHashCode(pair.Original), RuntimeHelpers.GetHashCode(pair.Copy));
    }
    #endregion
}
