using Soenneker.Extensions.String;
using Soenneker.Utils.PooledStringBuilders;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Soenneker.Extensions.Enumerable.String;

/// <summary>
/// A collection of helpful enumerable string extension methods
/// </summary>
public static class EnumerableStringExtension
{
    private static readonly StringComparer _ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// Partition Key, Document Id
    /// </summary>
    [Pure]
    public static List<(string PartitionKey, string DocumentId)> ToSplitIds(this IEnumerable<string> ids)
    {
        if (ids is null)
            throw new ArgumentNullException(nameof(ids));

        int capacity = ids.GetNonEnumeratedCount();

        List<(string PartitionKey, string DocumentId)> result = capacity > 0 ? new List<(string PartitionKey, string DocumentId)>(capacity) : [];

        foreach (string id in ids)
            result.Add(id.ToSplitId());

        return result;
    }

    /// <summary>
    /// Checks if any item in the enumerable contains a part of a string
    /// </summary>
    [Pure]
    public static bool ContainsAPart(this IEnumerable<string>? enumerable, string part, bool ignoreCase = true)
    {
        if (enumerable is null || part.IsNullOrEmpty())
            return false;

        if (enumerable is ICollection<string> { Count: 0 })
            return false;

        StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        foreach (string? current in enumerable)
        {
            if (current is null)
                continue;

            if (current.IndexOf(part, comparison) >= 0)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Equivalent to ToSeparatedString(',', includeSpace)
    /// </summary>
    [Pure]
    public static string ToCommaSeparatedString<T>(this IEnumerable<T>? enumerable, bool includeSpace = false) =>
        enumerable.ToSeparatedString(',', includeSpace);

    /// <summary>
    /// Fast-path join for value-types (and other types) that implement ISpanFormattable.
    /// Avoids boxing that would occur via "item is ISpanFormattable" checks in the general method.
    /// </summary>
    [Pure]
    public static string ToSeparatedStringFormattable<T>(this IEnumerable<T>? enumerable, char separator, bool includeSpace = false) where T : ISpanFormattable
    {
        if (enumerable is null)
            return string.Empty;

        if (enumerable is ICollection<T> { Count: 0 })
            return string.Empty;

        int count = enumerable.GetNonEnumeratedCount();
        int initialCapacity = count > 0 ? Math.Min(Math.Max(128, count * 4), 4096) : 128;

        using var psb = new PooledStringBuilder(initialCapacity);
        var wroteAny = false;

        foreach (T item in enumerable)
        {
            if (wroteAny)
            {
                if (includeSpace)
                    psb.Append(separator, ' ');
                else
                    psb.Append(separator);
            }
            else
            {
                wroteAny = true;
            }

            // This hits your allocation-free generic Append<T>(T value) where T : ISpanFormattable
            psb.Append(item);
        }

        return wroteAny ? psb.ToString() : string.Empty;
    }

    /// <summary>
    /// Joins the elements of an enumerable into a single string, using the specified separator character
    /// (and optionally a space after each separator). Returns an empty string if the input is null or empty.
    /// </summary>
    [Pure]
    public static string ToSeparatedString<T>(this IEnumerable<T>? enumerable, char separator, bool includeSpace = false)
    {
        if (enumerable is null)
            return string.Empty;

        if (enumerable is ICollection<T> { Count: 0 })
            return string.Empty;

        int count = enumerable.GetNonEnumeratedCount();
        int initialCapacity = count > 0 ? Math.Min(Math.Max(128, count * 4), 4096) : 128;

        var psb = new PooledStringBuilder(initialCapacity);
        var wroteAny = false;

        try
        {
            foreach (T item in enumerable)
            {
                if (wroteAny)
                {
                    if (includeSpace)
                        psb.Append(separator, ' ');
                    else
                        psb.Append(separator);
                }
                else
                {
                    wroteAny = true;
                }

                // Match string.Join behavior: null => empty.
                if (item is null)
                    continue;

                // Fast paths
                if (item is string s)
                {
                    psb.Append(s);
                    continue;
                }

                // NOTE: For value-types T, this interface check boxes.
                // Use ToSeparatedStringFormattable<T>() to avoid that.
                if (item is ISpanFormattable sf)
                {
                    AppendFormattable(ref psb, sf);
                    continue;
                }

                // Fallback (may allocate depending on type)
                psb.Append(item.ToString());
            }

            return wroteAny ? psb.ToStringAndDispose() : string.Empty;
        }
        catch
        {
            psb.Dispose();
            throw;
        }
    }

    [Pure]
    public static IEnumerable<string> ToLower(this IEnumerable<string> enumerable)
    {
        if (enumerable is null)
            throw new ArgumentNullException(nameof(enumerable));

        foreach (string str in enumerable)
            yield return str.ToLowerInvariantFast() ?? "";
    }

    [Pure]
    public static IEnumerable<string> ToUpper(this IEnumerable<string> enumerable)
    {
        if (enumerable is null)
            throw new ArgumentNullException(nameof(enumerable));

        foreach (string str in enumerable)
            yield return str.ToUpperInvariantFast() ?? "";
    }

    [Pure]
    public static HashSet<string> ToHashSetIgnoreCase(this IEnumerable<string> source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        int capacity = source.GetNonEnumeratedCount();

        HashSet<string> hashSet = capacity > 0 ? new HashSet<string>(capacity, _ordinalIgnoreCase) : new HashSet<string>(_ordinalIgnoreCase);

        foreach (string item in source)
            hashSet.Add(item);

        return hashSet;
    }

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

    [Pure]
    public static IEnumerable<string> DistinctIgnoreCase(this IEnumerable<string> source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        int capacity = source.GetNonEnumeratedCount();

        HashSet<string> seen = capacity > 0 ? new HashSet<string>(capacity, _ordinalIgnoreCase) : new HashSet<string>(_ordinalIgnoreCase);

        foreach (string? str in source)
        {
            if (str is null)
                continue;

            if (seen.Add(str))
                yield return str;
        }
    }

    [Pure]
    public static bool StartsWithIgnoreCase(this IEnumerable<string> source, string prefix)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));
        if (prefix is null)
            throw new ArgumentNullException(nameof(prefix));

        if (prefix.Length == 0)
        {
            foreach (string? s in source)
                if (s is not null)
                    return true;
            return false;
        }

        foreach (string? str in source)
        {
            if (str is not null && str.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    [Pure]
    public static bool EndsWithIgnoreCase(this IEnumerable<string> source, string suffix)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (suffix is null)
            throw new ArgumentNullException(nameof(suffix));

        if (suffix.Length == 0)
        {
            foreach (string? s in source)
                if (s is not null)
                    return true;
            return false;
        }

        foreach (string? str in source)
        {
            if (str is not null && str.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    [Pure]
    public static bool ContainsIgnoreCase(this IEnumerable<string> source, string value)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (value is null)
            throw new ArgumentNullException(nameof(value));

        foreach (string? str in source)
        {
            if (str is not null && _ordinalIgnoreCase.Equals(str, value))
                return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AppendFormattable(ref PooledStringBuilder psb, ISpanFormattable value, ReadOnlySpan<char> format = default,
        IFormatProvider? provider = null)
    {
        var hint = 32;

        while (true)
        {
            Span<char> span = psb.AppendSpan(hint);

            if (value.TryFormat(span, out int written, format, provider))
            {
                psb.Shrink(hint - written);
                return;
            }

            psb.Shrink(hint);
            hint <<= 1;
        }
    }
}