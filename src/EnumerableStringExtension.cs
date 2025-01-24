using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Soenneker.Extensions.String;

namespace Soenneker.Extensions.Enumerable.String;

/// <summary>
/// A collection of helpful enumerable string extension methods
/// </summary>
public static class EnumerableStringExtension
{
    /// <summary>
    /// Partition Key, Document Id
    /// </summary>
    /// <param name="ids">The enumerable of string ids</param>
    /// <returns>A list of tuples containing the partition key and document id</returns>
    [Pure]
    public static List<(string PartitionKey, string DocumentId)> ToSplitIds(this IEnumerable<string> ids)
    {
        // Attempt to cast to a collection to pre-size the list if possible.
        var idsCollection = ids as ICollection<string>;
        var result = new List<(string PartitionKey, string DocumentId)>(idsCollection?.Count ?? 0);

        using IEnumerator<string> enumerator = ids.GetEnumerator();

        while (enumerator.MoveNext())
        {
            (string PartitionKey, string DocumentId) split = enumerator.Current.ToSplitId();
            result.Add(split);
        }

        return result;
    }

    /// <summary>
    /// Checks if any item in the enumerable contains a part of a string
    /// </summary>
    /// <param name="enumerable">The enumerable of strings</param>
    /// <param name="part">The part to search for</param>
    /// <param name="ignoreCase">True to ignore case, false otherwise</param>
    /// <returns>True if any item contains the part, false otherwise</returns>
    [Pure]
    public static bool ContainsAPart(this IEnumerable<string>? enumerable, string part, bool ignoreCase = true)
    {
        // Early exit for null or empty inputs
        if (enumerable is null || part.IsNullOrEmpty())
            return false;

        if (enumerable is ICollection<string> {Count: 0})
            return false;

        // Precompute the normalized part if case-insensitive
        string searchPart = ignoreCase ? part.ToUpperInvariantFast() : part;

        // Use enumerator for optimal performance over IEnumerable
        using IEnumerator<string> enumerator = enumerable.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string current = enumerator.Current;

            // Normalize the current string only if case-insensitive comparison is required
            string normalizedCurrent = ignoreCase ? current.ToUpperInvariantFast() : current;

            if (normalizedCurrent.Contains(searchPart, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Equivalent to string.Join(",", enumerable). Optimized to avoid creating intermediate strings.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable</typeparam>
    /// <param name="enumerable">The enumerable to join</param>
    /// <param name="includeSpace">True to include a space after each comma, false otherwise</param>
    /// <returns>A comma-separated string representation of the enumerable</returns>
    [Pure]
    public static string ToCommaSeparatedString<T>(this IEnumerable<T>? enumerable, bool includeSpace = false)
    {
        // Handle null input quickly
        if (enumerable is null)
            return "";

        // Attempt to cast to ICollection for better performance with predictable sizing
        if (enumerable is ICollection<T> {Count: 0})
            return "";

        // Use a separator depending on the flag
        string separator = includeSpace ? ", " : ",";

        // Create the StringBuilder with a reasonable initial capacity
        const int defaultCapacity = 256;
        var sb = new StringBuilder(defaultCapacity);

        // Iterate through the enumerable and build the string
        using IEnumerator<T> enumerator = enumerable.GetEnumerator();

        if (enumerator.MoveNext())
        {
            // Append the first item without a separator
            sb.Append(enumerator.Current);

            // Append the remaining items with separators
            while (enumerator.MoveNext())
            {
                sb.Append(separator).Append(enumerator.Current);
            }
        }

        return sb.ToString();
    }


    /// <summary>
    /// Converts all strings in the enumerable to lowercase. Equivalent to <code>enumerable.Select(str => str.ToLowerInvariant()</code>
    /// </summary>
    /// <param name="enumerable">The enumerable of strings</param>
    /// <returns>An enumerable of lowercase strings</returns>
    [Pure]
    public static IEnumerable<string> ToLower(this IEnumerable<string> enumerable)
    {
        using IEnumerator<string> enumerator = enumerable.GetEnumerator();

        while (enumerator.MoveNext())
        {
            yield return enumerator.Current.ToLowerInvariantFast();
        }
    }

    /// <summary>
    /// Converts all strings in the enumerable to uppercase. Equivalent to <code>enumerable.Select(str => str.ToUpperInvariant()</code>
    /// </summary>
    /// <param name="enumerable">The enumerable of strings</param>
    /// <returns>An enumerable of uppercase strings</returns>
    [Pure]
    public static IEnumerable<string> ToUpper(this IEnumerable<string> enumerable)
    {
        using IEnumerator<string> enumerator = enumerable.GetEnumerator();

        while (enumerator.MoveNext())
        {
            yield return enumerator.Current.ToUpperInvariantFast();
        }
    }

    /// <summary>
    /// Converts an IEnumerable of strings to a HashSet of strings, 
    /// using case-insensitive comparison based on OrdinalIgnoreCase.
    /// </summary>
    /// <param name="source">The source collection of strings.</param>
    /// <returns>
    /// A HashSet containing the unique elements from the source collection, 
    /// with case-insensitive comparison.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the source collection is null.
    /// </exception>
    [Pure]
    public static HashSet<string> ToHashSetIgnoreCase(this IEnumerable<string> source)
    {
        return new HashSet<string>(source, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns an IEnumerable of strings excluding null or empty entries.
    /// </summary>
    /// <param name="source">The source collection of strings.</param>
    /// <returns>An IEnumerable of strings with all null or empty entries removed.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the source collection is null.
    /// </exception>
    [Pure]
    public static IEnumerable<string> RemoveNullOrEmpty(this IEnumerable<string> source)
    {
        using IEnumerator<string> enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string current = enumerator.Current;

            // Check for null or empty using direct conditions for better performance
            if (!current.IsNullOrEmpty())
                yield return current;
        }
    }

    /// <summary>
    /// Returns an IEnumerable of strings excluding null, empty, or whitespace-only entries.
    /// </summary>
    /// <param name="source">The source collection of strings.</param>
    /// <returns>An IEnumerable of strings with all null, empty, or whitespace-only entries removed.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the source collection is null.
    /// </exception>
    [Pure]
    public static IEnumerable<string> RemoveNullOrWhiteSpace(this IEnumerable<string> source)
    {
        using IEnumerator<string> enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string current = enumerator.Current;

            if (!current.IsNullOrWhiteSpace())
                yield return current;
        }
    }

    /// <summary>
    /// Returns a distinct IEnumerable of strings, using case-insensitive comparison.
    /// </summary>
    /// <param name="source">The source collection of strings.</param>
    /// <returns>An IEnumerable of strings with distinct values, ignoring case differences.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the source collection is null.
    /// </exception>
    [Pure]
    public static IEnumerable<string> DistinctIgnoreCase(this IEnumerable<string> source)
    {
        // Attempt to cast to ICollection for better performance with predictable sizing
        var collection = source as ICollection<string>;
        int initialCapacity = collection?.Count ?? 0;

        // Initialize the HashSet with the calculated capacity and a case-insensitive comparer
        var seen = new HashSet<string>(initialCapacity, StringComparer.OrdinalIgnoreCase);

        // Use an enumerator for precise control and efficient iteration
        using IEnumerator<string> enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string? current = enumerator.Current;

            // Skip null values and add unique items to the HashSet
            if (current is not null && seen.Add(current))
            {
                yield return current;
            }
        }
    }

    /// <summary>
    /// Determines whether any string in the <paramref name="source"/> collection starts with the specified <paramref name="prefix"/>, ignoring case.
    /// </summary>
    /// <param name="source">The collection of strings to search.</param>
    /// <param name="prefix">The prefix to compare against the start of each string in the collection, ignoring case.</param>
    /// <returns>
    /// <c>true</c> if any string in the collection starts with the specified <paramref name="prefix"/>, ignoring case; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> or <paramref name="prefix"/> is <c>null</c>.
    /// </exception>
    [Pure]
    public static bool StartsWithIgnoreCase(this IEnumerable<string> source, string prefix)
    {
        using IEnumerator<string> enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string current = enumerator.Current;

            if (current is not null && current.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether any string in the <paramref name="source"/> collection ends with the specified <paramref name="suffix"/>, ignoring case.
    /// </summary>
    /// <param name="source">The collection of strings to search.</param>
    /// <param name="suffix">The suffix to compare against the end of each string in the collection, ignoring case.</param>
    /// <returns>
    /// <c>true</c> if any string in the collection ends with the specified <paramref name="suffix"/>, ignoring case; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> or <paramref name="suffix"/> is <c>null</c>.
    /// </exception>
    [Pure]
    public static bool EndsWithIgnoreCase(this IEnumerable<string> source, string suffix)
    {
        using IEnumerator<string> enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string current = enumerator.Current;

            if (current is not null && current.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether any string in the <paramref name="source"/> collection is equal to the specified <paramref name="value"/>, ignoring case.
    /// </summary>
    /// <param name="source">The collection of strings to search.</param>
    /// <param name="value">The string value to search for in the collection, ignoring case.</param>
    /// <returns>
    /// <c>true</c> if any string in the collection equals the specified <paramref name="value"/>, ignoring case; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> or <paramref name="value"/> is <c>null</c>.
    /// </exception>
    [Pure]
    public static bool ContainsIgnoreCase(this IEnumerable<string> source, string value)
    {
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;

        using IEnumerator<string> enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            string current = enumerator.Current;

            if (current is not null && comparer.Equals(current, value))
            {
                return true;
            }
        }

        return false;
    }
}