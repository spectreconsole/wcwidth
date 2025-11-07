namespace Wcwidth.Tests;

public sealed partial class WcwidthTests
{
    [Fact]
    public void EmptyString()
    {
        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
            string.Empty);

        // Then
        seq.Length.ShouldBe(0);
        width.ShouldBe(0);
    }

    [Fact]
    public void Basic_String()
    {
        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
            "hello\0world");

        // Then
        seq.ShouldBe([1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1]);
        width.ShouldBe(10);
    }

    [Fact]
    public void Japanese_Hello()
    {
        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
            "コンニチハ, セカイ!");

        // Then
        seq.ShouldBe([2, 2, 2, 2, 2, 1, 1, 2, 2, 2, 1]);
        width.ShouldBe(19);
    }

    [Fact]
    public void Null_Width()
    {
        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
            "abc\0def");

        // Then
        seq.ShouldBe([1, 1, 1, 0, 1, 1, 1]);
        width.ShouldBe(6);
    }

    [Fact]
    public void C0_Width_Negative_1()
    {
        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
            "\x1b[0m");

        // Then
        seq.ShouldBe([-1, 1, 1, 1]);
        width.ShouldBe(-1);
    }

    [Fact]
    public void Combining_Width()
    {
        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
            "--\u05bf--");

        // Then
        seq.ShouldBe([1, 1, 0, 1, 1]);
        width.ShouldBe(4);
    }

    [Fact]
    public void Combining_Cafe()
    {
        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
            "cafe\u0301");

        // Then
        seq.ShouldBe([1, 1, 1, 1, 0]);
        width.ShouldBe(4);
    }

    [Fact]
    public void Combining_Enclosing()
    {
        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
            "\u0410\u0488");

        // Then
        seq.ShouldBe([1, 0]);
        width.ShouldBe(1);
    }

    /// <summary>
    /// Balinese kapal (ship) is length 3.
    /// This may be an example that is not yet correctly rendered by any terminal so
    /// far, like devanagari.
    /// </summary>
    [Fact]
    public void Balinese_Script()
    {
        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
        [
            "\u1B13", // Category 'Lo', EAW 'N' -- BALINESE LETTER KA
            "\u1B28", // Category 'Lo', EAW 'N' -- BALINESE LETTER PA KAPAL
            "\u1B2E", // Category 'Lo', EAW 'N' -- BALINESE LETTER LA
            "\u1B44", // Category 'Mc', EAW 'N' -- BALINESE ADEG ADEG
        ]);

        // Then
        seq.ShouldBe([1, 1, 1, 0]);
        width.ShouldBe(3);
    }

    /// <summary>
    /// <para>
    /// Test basic combining of HANGUL CHOSEONG and JUNGSEONG
    /// This is an example where both characters are "wide" when displayed alone.
    /// </para>
    /// <para>But JUNGSEONG (vowel) is designed for combination with a CHOSEONG (consonant).</para>
    /// <para>
    /// This wcwidth library understands their width only when combination,
    /// and not by independent display, like other zero-width characters that may
    /// only combine with an appropriate preceding character.
    /// </para>
    /// <para>
    /// Example from Raymond Chen's blog post,
    /// https://devblogs.microsoft.com/oldnewthing/20201009-00/?p=104351
    /// </para>
    /// </summary>
    [Fact]
    public void Kr_Jamo()
    {
        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
        [
            "\u1100", // Category 'Lo', EAW 'N' -- BALINESE LETTER KA
            "\u1161", // Category 'Lo', EAW 'N' -- BALINESE LETTER PA KAPAL
        ]);

        // Then
        seq.ShouldBe([2, 0]);
        width.ShouldBe(2);
    }

    [Fact]
    public void Kr_Jamo_Filler()
    {
        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
        [
            "\u1100", // HANGUL CHOSEONG KIYEOK (consonant)
            "\u1160", // HANGUL JUNGSEONG FILLER (vowel)
        ]);

        // Then
        seq.ShouldBe([2, 0]);
        width.ShouldBe(2);
    }

    /// <summary>
    /// <para>Attempt to test the measurement width of Devanagari script.</para>
    /// <para>I believe this 'phrase' should be length 3.</para>
    /// <para>
    /// This is a difficult problem, and this library does not yet get it right,
    /// because we interpret the Unicode data files programmatically, but they do
    /// not correctly describe how their terminal width is measured.
    /// </para>
    /// <para>There are very few Terminals that do!</para>
    /// <para>As of 2023:</para>
    /// <para>
    /// - iTerm2: correct length but individual characters are out of order and
    ///             horizaontally misplaced as to be unreadable in its language when
    ///             using 'Noto Sans' font.
    /// - mlterm: mixed results, it offers several options in the configuration
    ///             dialog, "Xft", "Cario", and "Variable Column Width" have some
    ///             effect, but with neither 'Noto Sans' or 'unifont', it is not
    ///             recognizable as the Devanagari script it is meant to display.
    /// </para>
    /// <para>Previous testing with Devanagari documented at address https://benizi.com/vim/devanagari/</para>
    /// <para>See also, https://askubuntu.com/questions/8437/is-there-a-good-mono-spaced-font-for-devanagari-script-in-the-terminal</para>
    /// </summary>
    [Fact]
    public void Devanagari_Script()
    {
        // This test adapted from https://www.unicode.org/L2/L2023/23107-terminal-suppt.pdf
        // please note that document correctly points out that the final width cannot be determined
        // as a sum of each individual width, as this library currently performs with exception of
        // ZWJ, but I think it incorrectly gestures what a stateless call to wcwidth.wcwidth of
        // each codepoint *should* return.

        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
        [
            "\u0915", // Akhand, Category 'Lo', East Asian Width property 'N' -- DEVANAGARI LETTER KA
            "\u094D", // Joiner, Category 'Mn', East Asian Width property 'N' -- DEVANAGARI SIGN VIRAMA
            "\u0937", // Fused, Category 'Lo', East Asian Width property 'N' -- DEVANAGARI LETTER SSA
            "\u093F", // MatraL, Category 'Mc', East Asian Width property 'N' -- DEVANAGARI VOWEL SIGN I
        ]);

        // Then
        seq.ShouldBe([1, 0, 1, 0]); // 23107-terminal-suppt.pdf suggests wcwidth.wcwidth should return (2, 0, 0, 1)
        width.ShouldBe(2); // I believe the final width *should* be 3.
    }

    [Fact]
    public void Tamil_Script()
    {
        // This test adapted from https://www.unicode.org/L2/L2023/23107-terminal-suppt.pdf

        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
        [
            "\u0b95", // Akhand, Category 'Lo', East Asian Width property 'N' -- TAMIL LETTER KA
            "\u0bcd", // Joiner, Category 'Mn', East Asian Width property 'N' -- TAMIL SIGN VIRAMA
            "\u0bb7", // Fused, Category 'Lo', East Asian Width property 'N' -- TAMIL LETTER SSA
            "\u0bcc", // MatraLR, Category 'Mc', East Asian Width property 'N' -- TAMIL VOWEL SIGN AU
        ]);

        // Then
        seq.ShouldBe([1, 0, 1, 0]); // 23107-terminal-suppt.pdf suggests wcwidth.wcwidth should return (3, 0, 0, 4)
        width.ShouldBe(2); // I believe the final width should be about 5 or 6.
    }

    [Fact]
    public void Kannada_Script()
    {
        // This test adapted from https://www.unicode.org/L2/L2023/23107-terminal-suppt.pdf

        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
        [
            "\u0cb0", // Repha, Category 'Lo', East Asian Width property 'N' -- KANNADA LETTER RA
            "\u0ccd", // Joiner, Category 'Mn', East Asian Width property 'N' -- KANNADA SIGN VIRAMA
            "\u0c9d", // Base, Category 'Lo', East Asian Width property 'N' -- KANNADA LETTER JHA
            "\u0cc8", // MatraUR, Category 'Mc', East Asian Width property 'N' -- KANNADA VOWEL SIGN AI
        ]);

        // Then
        seq.ShouldBe([1, 0, 1, 0]); // 23107-terminal-suppt.pdf suggests should be (2, 0, 3, 1)
        width.ShouldBe(2); // I believe the correct final width *should* be 3 or 4.
    }

    [Fact]
    public void Kannada_Script_2()
    {
        // This test adapted from https://www.unicode.org/L2/L2023/23107-terminal-suppt.pdf

        // Given, When
        var (seq, width) = UnicodeFixture.Calculate(
        [
            "\u0cb0", // Base, Category 'Lo', East Asian Width property 'N' -- KANNADA LETTER RA
            "\u0cbc", // Nukta, Category 'Mn', East Asian Width property 'N' -- KANNADA SIGN NUKTA
            "\u0ccd", // Joiner, Category 'Lo', East Asian Width property 'N' -- KANNADA SIGN VIRAMA
            "\u0c9a", // Subjoin, Category 'Mc', East Asian Width property 'N' -- KANNADA LETTER CA
        ]);

        // Then
        seq.ShouldBe([1, 0, 0, 1]); // 23107-terminal-suppt.pdf suggests wcwidth.wcwidth should return (2, 0, 0, 1)
        width.ShouldBe(2); // I believe the final width is correct, but maybe for the wrong reasons!
    }

    /// <summary>
    /// Test characters considered both "wide" and "zero" width.
    /// </summary>
    [Fact]
    public void Zero_Wide_Conflict()
    {
        // -  (0x03000, 0x0303e,),  # Ideographic Space       ..Ideographic Variation In
        // +  (0x03000, 0x03029,),  # Ideographic Space       ..Hangzhou Numeral Nine
        UnicodeCalculator.GetWidth(0x03029, Unicode.Version_4_1_0).ShouldBe(2);
        UnicodeCalculator.GetWidth(0x0302a, Unicode.Version_4_1_0).ShouldBe(0);

        // # - (0x03099, 0x030ff,),  # Combining Katakana-hirag..Katakana Digraph Koto
        // # + (0x0309b, 0x030ff,),  # Katakana-hiragana Voiced..Katakana Digraph Koto
        UnicodeCalculator.GetWidth(0x03099, Unicode.Version_4_1_0).ShouldBe(0);
        UnicodeCalculator.GetWidth(0x0309a, Unicode.Version_4_1_0).ShouldBe(0);
        UnicodeCalculator.GetWidth(0x0309b, Unicode.Version_4_1_0).ShouldBe(2);
    }

    /// <summary>
    /// Test SOFT HYPHEN, category 'Cf' usually are zero-width, but most
    /// implementations agree to draw it was '1' cell, visually
    /// indistinguishable from a space, ' ' in Konsole, for example.
    /// </summary>
    [Fact]
    public void Soft_Hyphen()
    {
        UnicodeCalculator.GetWidth(0x000ad).ShouldBe(1);
    }
}

public static class UnicodeFixture
{
    public static (int[] Seq, int Width) Calculate(string phrase, Unicode? version = null)
    {
        var seq = phrase.Select(c => UnicodeCalculator.GetWidth(c, version)).ToArray();
        var width = UnicodeCalculator.GetWidth(phrase, version);
        return (seq, width);
    }

    public static (int[] Each, int Phrase) Calculate(string[] phrase, Unicode? version = null)
    {
        var foo = phrase.Select(c => UnicodeCalculator.GetWidth(c, version)).ToArray();
        var bar = UnicodeCalculator.GetWidth(string.Concat(phrase), version);
        return (foo, bar);
    }
}