[![](https://img.shields.io/nuget/v/Soenneker.Extensions.Enumerable.String.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Enumerable.String/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.enumerable.string/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.enumerable.string/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Extensions.Enumerable.String.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Enumerable.String/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.enumerable.string/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.enumerable.string/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.Enumerable.String
A collection of helpful enumerable string extension methods.

## Installation

```bash
dotnet add package Soenneker.Extensions.Enumerable.String
```

## Quick start

```csharp
using Soenneker.Extensions.Enumerable.String;

// Given an existing IEnumerable<string>? named enumerable:
var result = enumerable.ContainsAPart(part);
```

## Common operations

- `ContainsAPart()` - Returns `true` if any element contains `part` using ordinal comparisons.
- `ToCommaSeparatedString()` - Joins the values with commas and optionally spaces; null or empty input produces an empty string.
- `ToSeparatedStringFormattable()` - Fast-path join for `T` that implements `ISpanFormattable`. Avoids boxing that can occur in the unconstrained join overload.
- `ToSeparatedString()` - Joins the elements into a single string using `separator` (optionally followed by a space). Null items match `string.Join(string?, IEnumerablestring?)` semantics (treated as empty).
- `ToLower()` - Lazily projects every string to lowercase; enumeration performs the conversions.
- `ToUpper()` - Lazily projects every string to uppercase; enumeration performs the conversions.
- `ToHashSetIgnoreCase()` - Materializes a case-insensitive `HashSet<string>`, so casing-only duplicates collapse.
- `ExceptNullOrEmpty()` - Removes null or empty strings from the `source`.
- `ExceptNullOrWhiteSpace()` - Removes null or white space strings from the `source`.
- `DistinctIgnoreCase()` - Returns a lazy sequence with casing-only duplicates removed, preserving the first encountered value.
- `StartsWithIgnoreCase()` - Returns `true` when any string starts with the prefix using a case-insensitive comparison.
- `EndsWithIgnoreCase()` - Returns `true` when any string ends with the suffix using a case-insensitive comparison.

The package also includes one additional operation for more specialized cases.
