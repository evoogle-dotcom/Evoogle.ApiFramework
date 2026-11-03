// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
namespace Evoogle.ApiFramework.Schema.Configuration.Annotations.Internal;

internal static class ApiQuerySourceAnnotationValidation
{
    #region Methods
    internal static string? GetInvalidDeclarationReason(Type clrDelegateType)
    {
        if (!typeof(MulticastDelegate).IsAssignableFrom(clrDelegateType.BaseType))
        {
            return "A query-source declaration requires a CLR delegate type";
        }

        if (!clrDelegateType.IsVisible)
        {
            return "A query-source delegate must be public";
        }

        if (clrDelegateType.ContainsGenericParameters)
        {
            return "A query-source delegate cannot contain open generic parameters";
        }

        var invokeMethod = clrDelegateType.GetMethod(nameof(Action.Invoke));
        if (invokeMethod is null || invokeMethod.ReturnType == typeof(void))
        {
            return "A query-source delegate must declare a non-void output";
        }

        if (IsUnsupportedQueryOutput(invokeMethod.ReturnType))
        {
            return "A query-source delegate cannot use Task, ValueTask, or IAsyncEnumerable output wrappers";
        }

        if (invokeMethod.ReturnType.IsByRef || invokeMethod.ReturnType.IsPointer)
        {
            return "A query-source delegate cannot declare a by-reference or pointer output";
        }

        if (invokeMethod.GetParameters().Any(static parameter => string.IsNullOrWhiteSpace(parameter.Name)))
        {
            return "A query-source delegate parameter must have a CLR metadata name";
        }

        if (invokeMethod.GetParameters().Any(static parameter => parameter.ParameterType.IsByRef || parameter.ParameterType.IsPointer))
        {
            return "A query-source delegate cannot declare by-reference or pointer parameters";
        }

        return null;
    }

    private static bool IsUnsupportedQueryOutput(Type clrType)
    {
        if (typeof(Task).IsAssignableFrom(clrType) || clrType == typeof(ValueTask))
        {
            return true;
        }

        if (clrType.IsGenericType)
        {
            var genericTypeDefinition = clrType.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(ValueTask<>) ||
                genericTypeDefinition == typeof(IAsyncEnumerable<>))
            {
                return true;
            }
        }

        return clrType.GetInterfaces().Any
        (
            static interfaceType => interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)
        );
    }
    #endregion
}
