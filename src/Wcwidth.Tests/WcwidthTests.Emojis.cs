namespace Wcwidth.Tests;

/// <summary>
/// Test suite from https://github.com/jquast/wcwidth/blob/master/tests/test_emojis.py
/// </summary>
public sealed partial class WcwidthTests
{
    public sealed class Emojis
    {
        /// <summary>
        /// Emoji zwj sequence of four codepoints is just 2 cells.
        /// </summary>
        [Fact]
        public void Emoji_ZWJ_Sequence()
        {
            // Given, When
            var (seq, width) = UnicodeFixture.Calculate(
            [
                "\U0001f469", // Base, Category So, East Asian Width property 'W' -- WOMAN
                "\U0001f3fb", // Modifier, Category Sk, East Asian Width property 'W' -- EMOJI MODIFIER FITZPATRICK TYPE-1-2
                "\u200d", // Joiner, Category Cf, East Asian Width property 'N'  -- ZERO WIDTH JOINER
                "\U0001f4bb", // Fused, Category So, East Asian Width peroperty 'W' -- PERSONAL COMPUTER
            ]);

            // Then
            seq.ShouldBe([2, 0, 0, 2]);
            width.ShouldBe(2);
        }

        /// <summary>
        /// Ensure index-out-of-bounds does not occur for zero-width joiner without any following character.
        /// </summary>
        [Fact]
        public void Unfinished_ZWJ_Sequence()
        {
            // Given, When
            var (seq, width) = UnicodeFixture.Calculate(
            [
                "\U0001f469", // Base, Category So, East Asian Width property 'W' -- WOMAN
                "\U0001f3fb", // Modifier, Category Sk, East Asian Width property 'W' -- EMOJI MODIFIER FITZPATRICK TYPE-1-2
                "\u200d", // Joiner, Category Cf, East Asian Width property 'N'  -- ZERO WIDTH JOINER
            ]);

            // Then
            seq.ShouldBe([2, 0, 0]);
            width.ShouldBe(2);
        }

        /// <summary>
        /// Verify ZWJ is measured as though successful with characters that cannot be joined, wcwidth does not verify.
        /// </summary>
        [Fact]
        public void Non_Recommended_ZWJ_Sequence()
        {
            // Given, When
            var (seq, width) = UnicodeFixture.Calculate(
            [
                "\U0001f469", // Base, Category So, East Asian Width property 'W' -- WOMAN
                "\U0001f3fb", // Modifier, Category Sk, East Asian Width property 'W' -- EMOJI MODIFIER FITZPATRICK TYPE-1-2
                "\u200d", // Joiner, Category Cf, East Asian Width property 'N'  -- ZERO WIDTH JOINER
            ]);

            // Then
            seq.ShouldBe([2, 0, 0]);
            width.ShouldBe(2);
        }

        [Fact]
        public void Another_ZWJ_Sequence()
        {
            // Given, When
            var (seq, width) = UnicodeFixture.Calculate(
            [
                "\u26F9",     // PERSON WITH BALL
                "\U0001F3FB", // EMOJI MODIFIER FITZPATRICK TYPE-1-2
                "\u200D",     // ZERO WIDTH JOINER
                "\u2640",     // FEMALE SIGN
                "\uFE0F",     // VARIATION SELECTOR-16
            ]);

            // Then
            seq.ShouldBe([1, 0, 0, 1, 0]);
            width.ShouldBe(2);
        }

        /// <summary>
        /// <para>A much longer emoji ZWJ sequence of 10 total codepoints is just 2 cells.</para>
        /// <para>
        /// Also test the same sequence in duplicate, verifying multiple VS-16 sequences
        /// in a single function call.
        /// </para>
        /// </summary>
        [Fact]
        public void Long_ZWJ_Sequence()
        {
            // Given, When
            var (seq, width) = UnicodeFixture.Calculate(
            [
                "\U0001F9D1", // 'So', 'W' -- ADULT
                "\U0001F3FB", // 'Sk', 'W' -- EMOJI MODIFIER FITZPATRICK TYPE-1-2
                "\u200d",     // 'Cf', 'N' -- ZERO WIDTH JOINER
                "\u2764",     // 'So', 'N' -- HEAVY BLACK HEART
                "\uFE0F",     // 'Mn', 'A' -- VARIATION SELECTOR-16
                "\u200d",     // 'Cf', 'N' -- ZERO WIDTH JOINER
                "\U0001F48B", // 'So', 'W' -- KISS MARK
                "\u200d",     // 'Cf', 'N' -- ZERO WIDTH JOINER
                "\U0001F9D1", // 'So', 'W' -- ADULT
                "\U0001F3FD", // 'Sk', 'W' -- EMOJI MODIFIER FITZPATRICK TYPE-4
                "\U0001F9D1", // 'So', 'W' -- ADULT
                "\U0001F3FB", // 'Sk', 'W' -- EMOJI MODIFIER FITZPATRICK TYPE-1-2
                "\u200d",     // 'Cf', 'N' -- ZERO WIDTH JOINER
                "\u2764",     // 'So', 'N' -- HEAVY BLACK HEART
                "\uFE0F",     // 'Mn', 'A' -- VARIATION SELECTOR-16
                "\u200d",     // 'Cf', 'N' -- ZERO WIDTH JOINER
                "\U0001F48B", // 'So', 'W' -- KISS MARK
                "\u200d",     // 'Cf', 'N' -- ZERO WIDTH JOINER
                "\U0001F9D1", // 'So', 'W' -- ADULT
                "\U0001F3FD", // 'Sk', 'W' -- EMOJI MODIFIER FITZPATRICK TYPE-4
            ]);

            // Then
            seq.ShouldBe([2, 0, 0, 1, 0, 0, 2, 0, 2, 0, 2, 0, 0, 1, 0, 0, 2, 0, 2, 0]);
            width.ShouldBe(4);
        }

        /// <summary>
        /// Verify effect of VS-16 on unicode_version 9.0 and later.
        /// </summary>
        [Fact]
        public void Unicode_9_VS16()
        {
            // Given, When
            var (seq, width) = UnicodeFixture.Calculate(
            [
                "\u2640", // FEMALE SIGN
                "\uFE0F", // VARIATION SELECTOR-16
            ], Unicode.Version_9_0_0);

            // Then
            seq.ShouldBe([1, 0]);
            width.ShouldBe(2);
        }

        /// <summary>
        /// Verify that VS-16 has no effect on unicode_version 8.0 and earler.
        /// </summary>
        [Fact]
        public void Unicode_8_VS16()
        {
            // Given, When
            var (seq, width) = UnicodeFixture.Calculate(
            [
                "\u2640", // FEMALE SIGN
                "\uFE0F", // VARIATION SELECTOR-16
            ], Unicode.Version_8_0_0);

            // Then
            seq.ShouldBe([1, 0]);
            width.ShouldBe(1);
        }
    }
}