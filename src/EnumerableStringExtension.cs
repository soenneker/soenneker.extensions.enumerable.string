using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
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
    public static List<Tuple<string, string>> ToSplitIds(this IEnumerable<string> ids)
    {
        var result = new List<Tuple<string, string>>();

        foreach (string id in ids)
        {
            (string partitionKey, string documentId) = id.ToSplitId();
            result.Add(new Tuple<string, string>(partitionKey, documentId));
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
        if (enumerable.IsNullOrEmpty() || part.IsNullOrEmpty())
            return false;

        StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        // Pre-process part for case-insensitive search
        string searchPart = ignoreCase ? part.ToUpperInvariantFast() : part;

        foreach (string str in enumerable)
        {
            // Normalize string for case-insensitive comparison, if necessary
            string currentStr = ignoreCase ? str.ToUpperInvariantFast() : str;

            if (currentStr.Contains(searchPart, comparison))
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
        if (enumerable == null)
            return "";

        if (includeSpace)
            return string.Join(", ", enumerable);

        return string.Join(',', enumerable);
    }

    /// <summary>
    /// Converts all strings in the enumerable to lowercase. Equivalent to <code>enumerable.Select(str => str.ToLowerInvariant()</code>
    /// </summary>
    /// <param name="enumerable">The enumerable of strings</param>
    /// <returns>An enumerable of lowercase strings</returns>
    [Pure]
    public static IEnumerable<string> ToLower(this IEnumerable<string> enumerable)
    {
        foreach (string str in enumerable)
        {
            yield return str.ToLowerInvariantFast();
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
        foreach (string str in enumerable)
        {
            yield return str.ToUpperInvariantFast();
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
        foreach (string item in source)
        {
            if (!item.IsNullOrEmpty())
            {
                yield return item;
            }
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
    public static IEnumerable<string> RemoveNullOrWhitespace(this IEnumerable<string> source)
    {
        foreach (string item in source)
        {
            if (!item.IsNullOrWhiteSpace())
            {
                yield return item;
            }
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
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string item in source)
        {
            if (seen.Add(item))
            {
                yield return item;
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
        foreach (string item in source)
        {
            if (item.StartsWithIgnoreCase(prefix))
                return true;
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
        foreach (string item in source)
        {
            if (item.EndsWithIgnoreCase(suffix))
                return true;
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

        foreach (string item in source)
        {
            if (comparer.Equals(item, value))
                return true;
        }

        return false;
    }
}