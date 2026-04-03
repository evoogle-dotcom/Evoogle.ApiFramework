// Copyright (c) 2024-2025 Evoogle.com
// SPDX-License-Identifier: MIT
//
// This file is licensed under the MIT License.
// See the LICENSE file in the project root for more information.
namespace Evoogle.ApiFramework.Identity;

/// <summary>
///     Provides a fluent interface for building composite <see cref="ApiId"/> values from multiple parts.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ApiIdCompositeBuilder"/> accumulates parts through <see cref="Add(ApiId)"/> (for unnamed/positional parts)
///         and <see cref="Add(string, ApiId)"/> (for named parts), then produces a validated composite identifier via <see cref="Build"/>.
///     </para>
///     <para>
///         Validation rules enforced by <see cref="ApiId.Composite(ApiIdPart[])"/>:
///         <list type="bullet">
///             <item>All parts must be either named or unnamed (no mixing within a single composite).</item>
///             <item>Part values must be scalar identifiers (nested composites are not allowed).</item>
///             <item>Named parts must have unique names.</item>
///         </list>
///     </para>
/// </remarks>
/// <example>
///     <code>
///     // Build an unnamed (positional) composite
///     var orderedId = new ApiIdCompositeBuilder()
///         .Add(ApiId.FromInt32(42))
///         .Add(ApiId.FromInt32(1001))
///         .Build();
///     // Result: ApiId representing "42|1001"
///
///     // Build a named composite
///     var namedId = new ApiIdCompositeBuilder()
///         .Add("CustomerId", ApiId.FromInt32(42))
///         .Add("OrderNumber", ApiId.FromInt32(1001))
///         .Build();
///     // Result: ApiId representing "CustomerId=42|OrderNumber=1001"
///     </code>
/// </example>
public sealed class ApiIdCompositeBuilder
{
    #region Fields
    /// <summary>
    ///     Internal mutable collection of parts accumulated during the build process.
    /// </summary>
    /// <remarks>
    ///     Initialized lazily on first <see cref="Add(ApiId)"/> or <see cref="Add(string, ApiId)"/> call.
    /// </remarks>
    private List<ApiIdPart>? _parts;
    #endregion

    #region Methods
    /// <summary>
    ///     Adds an unnamed (positional) identifier part.
    /// </summary>
    /// <param name="value">The scalar identifier value to add as a positional part.</param>
    /// <returns>
    ///     The current <see cref="ApiIdCompositeBuilder"/> instance for method chaining.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Unnamed parts create ordered/positional composites where the sequence of parts matters.
    ///         All parts in the composite must be unnamed if this method is used.
    ///     </para>
    ///     <para>
    ///         The <paramref name="value"/> must be a scalar <see cref="ApiId"/> (not a composite).
    ///         Validation occurs during <see cref="Build"/>.
    ///     </para>
    /// </remarks>
    public ApiIdCompositeBuilder Add(ApiId value)
    {
        _parts ??= [];
        _parts.Add(new ApiIdPart(null, value));
        return this;
    }

    /// <summary>
    ///     Adds a named identifier part to the composite sequence.
    /// </summary>
    /// <param name="name">
    ///     The name for this part. Must be non-null and contain at least one non-whitespace character.
    /// </param>
    /// <param name="value">The scalar identifier value associated with <paramref name="name"/>.</param>
    /// <returns>
    ///     The current <see cref="ApiIdCompositeBuilder"/> instance for method chaining.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown if <paramref name="name"/> is <see langword="null"/>, empty, or contains only whitespace.
    /// </exception>
    /// <remarks>
    ///     <para>
    ///         Named parts create semantic composites where each part has explicit meaning (e.g., "CustomerId", "OrderNumber").
    ///         All parts in the composite must be named if this method is used.
    ///     </para>
    ///     <para>
    ///         The <paramref name="value"/> must be a scalar <see cref="ApiId"/> (not a composite).
    ///         Duplicate names and name uniqueness are validated during <see cref="Build"/>.
    ///     </para>
    /// </remarks>
    public ApiIdCompositeBuilder Add(string name, ApiId value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name must be non-empty for a named part.", nameof(name));
        }

        _parts ??= [];
        _parts.Add(new ApiIdPart(name.Trim(), value));
        return this;
    }

    /// <summary>
    ///     Constructs and validates the composite <see cref="ApiId"/> from all accumulated parts.
    /// </summary>
    /// <returns>
    ///     A composite <see cref="ApiId"/> containing all added parts, or <see cref="ApiId.Empty"/> if no parts were added.
    /// </returns>
    /// <exception cref="Evoogle.ApiFramework.Exceptions.ApiIdentityException">
    ///     Thrown if validation fails, such as:
    ///     <list type="bullet">
    ///         <item>Mixing named and unnamed parts in the same composite.</item>
    ///         <item>Including nested composite parts (only scalar parts allowed).</item>
    ///         <item>Duplicate part names in named composites.</item>
    ///     </list>
    /// </exception>
    /// <remarks>
    ///     After calling <see cref="Build"/>, the builder can be reused by adding new parts for a fresh composite.
    /// </remarks>
    public ApiId Build()
    {
        var result = ApiId.Composite(_parts);
        _parts = null;
        return result;
    }
    #endregion
}

