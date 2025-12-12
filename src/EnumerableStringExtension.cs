using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        if (ids is null)
            throw new ArgumentNullException(nameof(ids));

        if (ids is ICollection<string> col)
        {
            var result = new List<(string PartitionKey, string DocumentId)>(col.Count);

            foreach (string id in ids)
            {
                result.Add(id.ToSplitId());
            }

            return result;
        }
        else
        {
            var result = new List<(string PartitionKey, string DocumentId)>();

            foreach (string id in ids)
            {
                result.Add(id.ToSplitId());
            }

            return result;
        }
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
        if (enumerable == null || part.IsNullOrEmpty())
            return false;

        // If it's an empty collection, no need to iterate
        if (enumerable is ICollection<string> {Count: 0})
            return false;

        foreach (var current in enumerable)
        {
            if (current == null)
                continue;

            if (ignoreCase)
            {
                // Avoid allocating a new uppercase copy of 'current' on each iteration
                if (current.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            else
            {
                if (current.Contains(part, StringComparison.Ordinal))
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
        return enumerable.ToSeparatedString(',', includeSpace);
    }

    /// <summary>
    /// Joins the elements of an enumerable into a single string, using the specified separator character
    /// (and optionally a space after each separator). Returns an empty string if the input is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="enumerable">
    /// The collection of items to join. If null or has no elements, this method returns an empty string.
    /// </param>
    /// <param name="separator">
    /// The character to place between each element in the resulting string.
    /// </param>
    /// <param name="includeSpace">
    /// If <c>true</c>, inserts a space after each separator; otherwise, no space is added.
    /// </param>
    /// <returns>
    /// A single string containing all elements of <paramref name="enumerable"/> separated by 
    /// <paramref name="separator"/> (plus a space if <paramref name="includeSpace"/> is <c>true</c>),
    /// or an empty string if <paramref name="enumerable"/> is null or empty.
    /// </returns>
    [Pure]
    public static string ToSeparatedString<T>(this IEnumerable<T>? enumerable, char separator, bool includeSpace = false)
    {
        if (enumerable is null)
            return string.Empty;

        if (enumerable is ICollection<T> {Count: 0})
            return string.Empty;

        if (!includeSpace)
        {
            var sb = new StringBuilder(32);
            sb.AppendJoin(separator, enumerable);
            return sb.ToString();
        }

        string sepWithSpace = string.Concat(separator, " ");
        var sb2 = new StringBuilder(32);
        sb2.AppendJoin(sepWithSpace, enumerable);
        return sb2.ToString();
    }

    /// <summary>
    /// Converts all strings in the enumerable to lowercase. Equivalent to <code>enumerable.Select(str => str.ToLowerInvariant()</code>
    /// </summary>
    /// <param name="enumerable">The enumerable of strings</param>
    /// <returns>An enumerable of lowercase strings</returns>
    [Pure]
    public static IEnumerable<string> ToLower(this IEnumerable<string> enumerable)
    {
        if (enumerable is null)
            throw new ArgumentNullException(nameof(enumerable));

        foreach (string str in enumerable)
        {
            yield return str.ToLowerInvariantFast() ?? "";
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
        if (enumerable is null)
            throw new ArgumentNullException(nameof(enumerable));

        foreach (string str in enumerable)
        {
            yield return str.ToUpperInvariantFast() ?? "";
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
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        // Pre-allocate capacity if available
        int capacity = source is ICollection<string> collection ? collection.Count : 0;
        var hashSet = capacity > 0 
            ? new HashSet<string>(capacity, StringComparer.OrdinalIgnoreCase) 
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        foreach (string item in source)
        {
            hashSet.Add(item);
        }
        
        return hashSet;
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
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        foreach (string str in source)
        {
            if (str.HasContent())
                yield return str;
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
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        foreach (string str in source)
        {
            if (!string.IsNullOrWhiteSpace(str))
                yield return str;
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
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        int initialCapacity = (source is ICollection<string> col) ? col.Count : 0;

        HashSet<string> seen = initialCapacity > 0
            ? new HashSet<string>(initialCapacity, StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var str in source)
        {
            if (str is null)
                continue;

            if (seen.Add(str))
                yield return str;
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
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (prefix is null)
            throw new ArgumentNullException(nameof(prefix));

        foreach (var str in source)
        {
            if (str is not null && str.StartsWithIgnoreCase(prefix))
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
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (suffix is null)
            throw new ArgumentNullException(nameof(suffix));

        foreach (var str in source)
        {
            if (str is not null && str.EndsWithIgnoreCase(suffix))
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
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (value is null)
            throw new ArgumentNullException(nameof(value));

        StringComparer comparer = StringComparer.OrdinalIgnoreCase;

        foreach (var str in source)
        {
            if (str is not null && comparer.Equals(str, value))
                return true;
        }

        return false;
    }
}