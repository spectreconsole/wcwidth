namespace Wcwidth.Tests;

public sealed class StringTests
{
    [Theory]
    [InlineData("", 0)]
    [InlineData("\U0001F600", 2)] // 😀 wide emoji
    [InlineData("\U0001D400", 1)] // 𝐀 narrow, outside the BMP
    [InlineData("\U0001F1E9\U0001F1EA", 2)] // 🇩🇪 flag
    [InlineData("\u2764\uFE0F", 2)] // ❤️ VS16
    [InlineData("\U0001F468\u200D\U0001F469\u200D\U0001F467", 2)] // 👨‍👩‍👧 ZWJ sequence
    [InlineData("a\u200D\U0001F600b", 2)] // ZWJ skips the whole surrogate pair after it
    [InlineData("\u200D\U0001F600", 0)]
    [InlineData("a\u200D", 1)] // Trailing ZWJ
    [InlineData("a\u200D\n", 1)] // ZWJ skips the control character after it
    [InlineData("\uFE0Fa", 1)] // VS16 at the start is measured as a normal rune
    [InlineData("\u0301\uFE0F", 0)] // VS16 after a width-0 rune only
    [InlineData("\u2764\u0301\uFE0F", 2)] // VS16 applies to the last rune with a width
    [InlineData("\u2764\uFE0F\uFE0F", 2)] // VS16 clears the last measured rune
    [InlineData("a\u0007b", -1)]
    [InlineData("a\nb", -1)]
    [InlineData("a\tb", -1)]
    [InlineData("\U0001F600\u001B[0m", -1)]
    [InlineData("a\u0085b", -1)] // C1 control character
    public void Should_Return_Width(string text, int expected)
    {
        UnicodeCalculator.GetWidth(text).ShouldBe(expected);
        UnicodeCalculator.GetWidth(text.AsSpan()).ShouldBe(expected);
    }

    [Fact]
    public void Should_Measure_Lone_Surrogates_As_Replacement_Character()
    {
        (string Text, int Width)[] cases =
        [
            ("\uD83D", 1), // Lone high surrogate
            ("\uDE00", 1), // Lone low surrogate
            ("a\uD83Db", 3),
            ("\uDE00\uD83D", 2), // Low before high is two lone surrogates
            ("\uD83D\u200D\U0001F600", 1), // Lone surrogate, then ZWJ
            ("\u200D\uDE00a", 1), // ZWJ skips a lone surrogate as one code point
        ];

        foreach (var (text, width) in cases)
        {
            UnicodeCalculator.GetWidth(text).ShouldBe(width, text);
            UnicodeCalculator.GetWidth(text.AsSpan()).ShouldBe(width, text);
        }
    }

    [Theory]
    [InlineData("\u2764\uFE0F", 1)]
    [InlineData("\u2764\uFE0F\uFE0F", 1)]
    [InlineData("\uFE0F\u2764", 1)]
    public void Should_Ignore_VS16_Before_Unicode_9(string text, int expected)
    {
        UnicodeCalculator.GetWidth(text, Unicode.Version_8_0_0).ShouldBe(expected);
        UnicodeCalculator.GetWidth(text.AsSpan(), Unicode.Version_8_0_0).ShouldBe(expected);
    }

    [Fact]
    public void Should_Use_Version_Instead_Of_Cached_Widths()
    {
        // U+1F6DD (playground slide) is wide from Unicode 14.0.0
        UnicodeCalculator.GetWidth("\U0001F6DD").ShouldBe(2);
        UnicodeCalculator.GetWidth("\U0001F6DD", Unicode.Version_13_0_0).ShouldBe(1);

        // U+31E4 (CJK stroke HXG) is wide from Unicode 16.0.0
        UnicodeCalculator.GetWidth("\u31E4").ShouldBe(2);
        UnicodeCalculator.GetWidth("\u31E4", Unicode.Version_15_1_0).ShouldBe(1);
    }

    [Fact]
    public void Should_Measure_Span_Cut_From_Middle_Of_String()
    {
        var text = "xx\U0001F600\u4E2Dyy";

        UnicodeCalculator.GetWidth(text.AsSpan(2, 3)).ShouldBe(4); // 😀中
        UnicodeCalculator.GetWidth(text.AsSpan(3, 2)).ShouldBe(3); // Lone low surrogate, 中
        UnicodeCalculator.GetWidth(text.AsSpan(1, 2)).ShouldBe(2); // x, lone high surrogate
        UnicodeCalculator.GetWidth(text.AsSpan(4, 0)).ShouldBe(0);
    }

    [Fact]
    public void Should_Throw_If_String_Is_Null()
    {
        string text = null!;
        Should.Throw<ArgumentNullException>(() => UnicodeCalculator.GetWidth(text));
    }
}