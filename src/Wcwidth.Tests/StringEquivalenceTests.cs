using System.Globalization;
using Wcwidth.Tests.Reference;

namespace Wcwidth.Tests;

/// <summary>
/// Compares <see cref="UnicodeCalculator.GetWidth(string, Unicode?)"/>
/// with the implementation of Wcwidth 4.0.1.
/// </summary>
public sealed class StringEquivalenceTests
{
    public static TheoryData<Unicode?> Versions => [null, Unicode.Version_8_0_0, Unicode.Version_9_0_0];

    private static readonly string[] _candidates = CreateCandidates();

    [Theory]
    [MemberData(nameof(Versions))]
    public void Every_BMP_Code_Point_Should_Match_Reference(Unicode? version)
    {
        for (var codePoint = 0; codePoint <= char.MaxValue; codePoint++)
        {
            if (char.IsSurrogate((char)codePoint))
            {
                continue;
            }

            AssertMatchesReference(((char)codePoint).ToString(), version);
        }
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void Every_Pair_Of_Candidates_Should_Match_Reference(Unicode? version)
    {
        foreach (var first in _candidates)
        {
            foreach (var second in _candidates)
            {
                AssertMatchesReference(first + second, version);
            }
        }
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void Random_Sequences_Of_Candidates_Should_Match_Reference(Unicode? version)
    {
        var random = new Random(20261118);
        var builder = new System.Text.StringBuilder();
        for (var i = 0; i < 20_000; i++)
        {
            builder.Clear();
            var length = random.Next(1, 8);
            for (var j = 0; j < length; j++)
            {
                builder.Append(_candidates[random.Next(_candidates.Length)]);
            }

            AssertMatchesReference(builder.ToString(), version);
        }
    }

    private static void AssertMatchesReference(string text, Unicode? version)
    {
        var expected = LegacyUnicodeCalculator.GetWidth(text, version);

        var actual = UnicodeCalculator.GetWidth(text, version);
        if (actual != expected)
        {
            throw new ShouldAssertException(Describe(text, version, expected, actual, "string"));
        }

        var actualSpan = UnicodeCalculator.GetWidth(text.AsSpan(), version);
        if (actualSpan != expected)
        {
            throw new ShouldAssertException(Describe(text, version, expected, actualSpan, "span"));
        }
    }

    private static string Describe(string text, Unicode? version, int expected, int actual, string overload)
    {
        var codeUnits = string.Join(" ", text.Select(c => ((int)c).ToString("X4", CultureInfo.InvariantCulture)));
        return $"The {overload} overload measured [{codeUnits}] (version {version?.ToString() ?? "default"}) as {actual}, but Wcwidth 4.0.1 measures {expected}.";
    }

    private static string[] CreateCandidates()
    {
        var candidates = new List<string>
        {
            "a", " ", "é", "中", "Ａ", "❤", "☺", "☹", "©", "#",
            "\u1100", // ᄀ Hangul Choseong Kiyeok
            "\u1160", // ᅠ Hangul Jungseong Filler
            "\u11A8", // ᆨ Hangul Jongseong Kiyeok
            "\uAC00", // 가 Hangul Syllable Ga
            "\u0007", "\n", "\t", "\u001B", "\u0085", // C0 and C1 control characters
            "\uD83D", "\uDE00", // Halves of 😀: a pair when adjacent, lone surrogates otherwise
            "\U0001F600", // 😀 wide emoji
            "\U0001D400", // 𝐀 narrow, outside the BMP
            "\U0001F1E9", "\U0001F1EA", // Regional indicators D and E
            "\U0001F3FB", // Emoji modifier Fitzpatrick type 1-2
            "\U000E0100", // Variation selector 17, zero width outside the BMP
        };

        for (var codePoint = 0; codePoint <= char.MaxValue; codePoint++)
        {
            var c = (char)codePoint;
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category is UnicodeCategory.Format or UnicodeCategory.EnclosingMark
                || (codePoint >= 0xFE00 && codePoint <= 0xFE0F)
                || (category == UnicodeCategory.NonSpacingMark && codePoint < 0x0370))
            {
                candidates.Add(c.ToString());
            }
        }

        return [.. candidates.Distinct()];
    }
}