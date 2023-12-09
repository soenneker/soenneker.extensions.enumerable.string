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
    [Pure]
    public static bool ContainsAPart(this IEnumerable<string> enumerable, string part, bool ignoreCase = true)
    {
        return enumerable.Any(str => str.Contains(part, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
    }

    /// <summary>
    /// Equivalent to string.Join(", ", enumerable)
    /// </summary>
    /// <returns>an empty string if enumerable is null</returns>
    [Pure]
    public static string ToCommaSeparatedString<T>(this IEnumerable<T>? enumerable)
    {
        if (enumerable == null)
            return "";

        return string.Join(", ", enumerable);
    }
}