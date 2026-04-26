using AwesomeAssertions;
using Soenneker.Tests.Unit;
using System.Collections.Generic;

namespace Soenneker.Extensions.Enumerable.String.Tests;

public class EnumerableStringExtensionTests : UnitTest
{
    [Test]
    public void ToSplitIds_ShouldSplitIdsCorrectly()
    {
        var input = new List<string> { "PartitionKey:DocumentId", "Key:Id" };

        List<(string PartitionKey, string DocumentId)> result = input.ToSplitIds();

        result.Should().BeEquivalentTo(new List<(string PartitionKey, string DocumentId)>
            {
                ("PartitionKey", "DocumentId"),
                ("Key", "Id")
            });
    }

    [Test]
    public void ContainsAPart_ShouldReturnTrueWhenPartIsFound()
    {
        var input = new List<string> { "hello world", "test string", "sample" };

        bool result = input.ContainsAPart("test");

        result.Should().BeTrue();
    }

    [Test]
    public void ContainsAPart_ShouldReturnFalseWhenPartIsNotFound()
    {
        var input = new List<string> { "hello world", "sample" };

        bool result = input.ContainsAPart("missing");

        result.Should().BeFalse();
    }

    [Test]
    public void ToCommaSeparatedString_ShouldHandleSpacesCorrectly()
    {
        var input = new List<string> { "one", "two", "three" };

        string result = input.ToCommaSeparatedString(includeSpace: true);

        result.Should().Be("one, two, three");
    }

    [Test]
    public void ToCommaSeparatedString_ShouldHandleNoSpacesCorrectly()
    {
        var input = new List<string> { "one", "two", "three" };

        string result = input.ToCommaSeparatedString(includeSpace: false);

        result.Should().Be("one,two,three");
    }

    [Test]
    public void ToLower_ShouldConvertAllStringsToLowercase()
    {
        var input = new List<string> { "ONE", "Two", "three" };

        IEnumerable<string> result = input.ToLower();

        result.Should().BeEquivalentTo(new List<string> { "one", "two", "three" });
    }

    [Test]
    public void ToUpper_ShouldConvertAllStringsToUppercase()
    {
        var input = new List<string> { "one", "Two", "three" };

        IEnumerable<string> result = input.ToUpper();

        result.Should().BeEquivalentTo(new List<string> { "ONE", "TWO", "THREE" });
    }

    [Test]
    public void ToHashSetIgnoreCase_ShouldCreateCaseInsensitiveHashSet()
    {
        var input = new List<string> { "Hello", "hello", "WORLD", "world" };

        HashSet<string> result = input.ToHashSetIgnoreCase();

        result.Should().BeEquivalentTo(new HashSet<string> { "Hello", "WORLD" });
    }

    [Test]
    public void RemoveNullOrEmpty_ShouldRemoveEmptyStrings()
    {
        var input = new List<string> { "one", "", null, "two" };

        IEnumerable<string> result = input.RemoveNullOrEmpty();

        result.Should().BeEquivalentTo(new List<string> { "one", "two" });
    }

    [Test]
    public void ExceptNullOrEmpty_ShouldRemoveEmptyStrings()
    {
        var input = new List<string?> { "one", "", null, "two" };

        IEnumerable<string> result = input.ExceptNullOrEmpty();

        result.Should().BeEquivalentTo(new List<string> { "one", "two" });
    }

    [Test]
    public void RemoveNullOrWhiteSpace_ShouldRemoveWhiteSpaceStrings()
    {
        var input = new List<string> { "one", "  ", null, "two" };

        IEnumerable<string> result = input.RemoveNullOrWhiteSpace();

        result.Should().BeEquivalentTo(new List<string> { "one", "two" });
    }

    [Test]
    public void ExceptNullOrWhiteSpace_ShouldRemoveWhiteSpaceStrings()
    {
        var input = new List<string?> { "one", "  ", null, "two" };

        IEnumerable<string> result = input.ExceptNullOrWhiteSpace();

        result.Should().BeEquivalentTo(new List<string> { "one", "two" });
    }

    [Test]
    public void DistinctIgnoreCase_ShouldReturnDistinctStringsIgnoringCase()
    {
        var input = new List<string> { "one", "One", "TWO", "two", "three" };

        IEnumerable<string> result = input.DistinctIgnoreCase();

        result.Should().BeEquivalentTo(new List<string> { "one", "TWO", "three" });
    }

    [Test]
    public void StartsWithIgnoreCase_ShouldReturnTrueWhenPrefixMatches()
    {
        var input = new List<string> { "apple", "banana", "cherry" };

        bool result = input.StartsWithIgnoreCase("ban");

        result.Should().BeTrue();
    }

    [Test]
    public void StartsWithIgnoreCase_ShouldReturnFalseWhenPrefixDoesNotMatch()
    {
        var input = new List<string> { "apple", "banana", "cherry" };

        bool result = input.StartsWithIgnoreCase("gra");

        result.Should().BeFalse();
    }

    [Test]
    public void EndsWithIgnoreCase_ShouldReturnTrueWhenSuffixMatches()
    {
        var input = new List<string> { "apple", "banana", "cherry" };

        bool result = input.EndsWithIgnoreCase("rry");

        result.Should().BeTrue();
    }

    [Test]
    public void EndsWithIgnoreCase_ShouldReturnFalseWhenSuffixDoesNotMatch()
    {
        var input = new List<string> { "apple", "banana", "cherry" };

        bool result = input.EndsWithIgnoreCase("gra");

        result.Should().BeFalse();
    }

    [Test]
    public void ContainsIgnoreCase_ShouldReturnTrueWhenValueMatches()
    {
        var input = new List<string> { "apple", "banana", "cherry" };

        bool result = input.ContainsIgnoreCase("BANANA");

        result.Should().BeTrue();
    }

    [Test]
    public void ContainsIgnoreCase_ShouldReturnFalseWhenValueDoesNotMatch()
    {
        var input = new List<string> { "apple", "banana", "cherry" };

        bool result = input.ContainsIgnoreCase("grape");

        result.Should().BeFalse();
    }
}
