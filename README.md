[![](https://img.shields.io/nuget/v/Soenneker.Extensions.Enumerable.String.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Enumerable.String/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.enumerable.string/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.enumerable.string/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Extensions.Enumerable.String.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Enumerable.String/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.enumerable.string/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.enumerable.string/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.Enumerable.String

Joining, filtering, case-insensitive searching, casing, deduplication, and compound-ID projection extensions for string sequences.

## Installation

```bash
dotnet add package Soenneker.Extensions.Enumerable.String
```

## Join values

```csharp
using Soenneker.Extensions.Enumerable.String;

string csv = new[] { "red", "green", "blue" }
    .ToCommaSeparatedString(includeSpace: true);
// red, green, blue

string path = new[] { "api", "customers", "42" }
    .ToSeparatedString('/');
// api/customers/42
```

Both methods return an empty string for a null or empty source. A null item is represented as an empty field, so `new[] { "a", null, "b" }` joined with commas becomes `a,,b`. Joining is immediate and visits the source once.

`ToSeparatedStringFormattable()` is the allocation-conscious overload for value types implementing `ISpanFormattable`; it avoids the boxing that the unconstrained overload can require. Formatting uses the type’s default format with a null format provider.

## Search strings

```csharp
string[] names = ["Alpha", "Beta"];

names.ContainsAPart("PH");         // true; ignores case by default
names.ContainsAPart("PH", false);  // false; ordinal, case-sensitive
names.ContainsIgnoreCase("alpha"); // true
names.StartsWithIgnoreCase("al");  // true
names.EndsWithIgnoreCase("TA");    // true
```

Searches are ordinal, skip null elements, stop at the first match, and do not allocate normalized copies. `ContainsAPart()` returns `false` for a null source or null/empty search text. The other search methods require non-null source and search arguments. An empty prefix or suffix matches when the sequence contains at least one non-null string.

## Filter, normalize, and deduplicate

```csharp
IEnumerable<string?> optionalValues = ["Alpha", null, " ", "alpha"];
IEnumerable<string> nonEmpty = optionalValues.ExceptNullOrEmpty();
IEnumerable<string> nonBlank = optionalValues.ExceptNullOrWhiteSpace();

IEnumerable<string> values = nonBlank;
IEnumerable<string> lower = values.ToLower();
IEnumerable<string> distinct = values.DistinctIgnoreCase();
HashSet<string> set = values.ToHashSetIgnoreCase();
```

These sequence-returning operations are lazy except `ToHashSetIgnoreCase()`, which materializes a new ordinal case-insensitive set. `ExceptNullOrEmpty()` preserves whitespace-only strings; `ExceptNullOrWhiteSpace()` removes them. `ToLower()` and `ToUpper()` use invariant casing and turn null elements into empty strings. `DistinctIgnoreCase()` skips nulls, keeps the first casing encountered, and preserves order.

## Split compound IDs

```csharp
List<(string PartitionKey, string DocumentId)> ids = new[]
{
    "customer-42:order-100",
    "standalone"
}.ToSplitIds();

// ("customer-42", "order-100")
// ("standalone", "standalone")
```

Each ID is split at its last colon, so earlier colons remain part of a compound partition key. The result is immediately materialized in source order. Null or empty IDs are rejected by the underlying split operation.
