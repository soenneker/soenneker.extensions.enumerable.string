using System;
using System.Collections.Generic;
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
}