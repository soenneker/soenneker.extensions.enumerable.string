using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
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
    public static bool ContainsAPart(this IEnumerable<string> enumerable, string part, bool ignoreCase = true)
    {
        return enumerable.Any(str => str.Contains(part, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
    }

    /// <summary>
    /// Equivalent to string.Join(",", enumerable).
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

        return string.Join(",", enumerable);
    }

    /// <summary>
    /// Converts all strings in the enumerable to lowercase. Equivalent to <code>enumerable.Select(str => str.ToLowerInvariant()</code>
    /// </summary>
    /// <param name="enumerable">The enumerable of strings</param>
    /// <returns>An enumerable of lowercase strings</returns>
    [Pure]
    public static IEnumerable<string> ToLower(this IEnumerable<string> enumerable)
    {
        return enumerable.Select(str => str.ToLowerInvariant());
    }

    /// <summary>
    /// Converts all strings in the enumerable to uppercase. Equivalent to <code>enumerable.Select(str => str.ToUpperInvariant()</code>
    /// </summary>
    /// <param name="enumerable">The enumerable of strings</param>
    /// <returns>An enumerable of uppercase strings</returns>
    [Pure]
    public static IEnumerable<string> ToUpper(this IEnumerable<string> enumerable)
    {
        return enumerable.Select(str => str.ToUpperInvariant());
    }
}