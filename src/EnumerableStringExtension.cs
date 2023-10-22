using System;
using System.Collections.Generic;
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
    public static List<Tuple<string, string>> ToSplitIds(this IEnumerable<string> ids)
    {
        var result = new List<Tuple<string, string>>();

        foreach (string id in ids)
        {
            (string PartitionKey, string DocumentId) split = id.ToSplitId();
            result.Add(new Tuple<string, string>(split.PartitionKey, split.DocumentId));
        }

        return result;
    }

    /// <summary>
    /// Checks if any item in the enumerable contains a part of a string
    /// </summary>
    public static bool ContainsAPart(this IEnumerable<string> enumerable, string part, bool ignoreCase = true)
    {
        return enumerable.Any(str => str.Contains(part, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
    }
}