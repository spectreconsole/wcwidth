using System.Linq;
#if NET6_0_OR_GREATER
using System.Text;
#endif
using Shouldly;
using Xunit;

namespace Wcwidth.Tests
{
    public sealed class WcwidthTests
    {
        [Fact]
        public void Test_Favorite_Emoji()
        {
            // Given
            var length = "💩".Select(c => UnicodeCalculator.GetWidth(c)).Sum();

            // Then
            length.ShouldBe(2);
        }

        [Fact]
        public void Test_Hello_Japanese()
        {
            // Given
            const string phrase = "コンニチハ, セカイ!";
            var expected = new[] { 2, 2, 2, 2, 2, 1, 1, 2, 2, 2, 1 };

            // When
            var length = phrase.Select(c => UnicodeCalculator.GetWidth(c));

            // Then
            length.ShouldBe(expected);
        }

        [Fact]
        public void Test_Null_Width()
        {
            // Given
            const string phrase = "abc\x0000def";
            var expected = new[] { 1, 1, 1, 0, 1, 1, 1 };

            // When
            var length = phrase.Select(c => UnicodeCalculator.GetWidth(c));

            // Then
            length.ShouldBe(expected);
        }

        [Fact]
        public void Test_Control_C0_Width()
        {
            // Given
            const string phrase = "\x1b[0m";
            var expected = new[] { -1, 1, 1, 1 };

            // When
            var length = phrase.Select(c => UnicodeCalculator.GetWidth(c));

            // Then
            length.ShouldBe(expected);
        }

        [Fact]
        public void Test_Combining_Width()
        {
            // Given
            const string phrase = "--\u05bf--";
            var expected = new[] { 1, 1, 0, 1, 1 };

            // When
            var length = phrase.Select(c => UnicodeCalculator.GetWidth(c));

            // Then
            length.ShouldBe(expected);
        }

        [Fact]
        public void Test_Combining_Cafe_Width()
        {
            // Given
            const string phrase = "cafe\u0301";
            var expected = new[] { 1, 1, 1, 1, 0 };

            // When
            var length = phrase.Select(c => UnicodeCalculator.GetWidth(c));

            // Then
            length.ShouldBe(expected);
        }

        [Fact]
        public void Test_Combining_Enclosing()
        {
            // Given
            const string phrase = "\u0410\u0488";
            var expected = new[] { 1, 0 };

            // When
            var length = phrase.Select(c => UnicodeCalculator.GetWidth(c));

            // Then
            length.ShouldBe(expected);
        }

        [Fact]
        public void Test_Combining_Spacing()
        {
            // Given
            const string phrase = "\u1B13\u1B28\u1B2E\u1B44";
            var expected = new[] { 1, 1, 1, 1 };

            // When
            var length = phrase.Select(c => UnicodeCalculator.GetWidth(c));

            // Then
            length.ShouldBe(expected);
        }

        [Fact]
        public void Test_U4DC0()
        {
            // Given, When
            var length = UnicodeCalculator.GetWidth('\u4DC0');

            // Then
            length.ShouldBe(2);
        }

        [Fact]
        public void Test_Int32_Overload()
        {
            // Given
            const int codePoint = 0x1F680; // 🚀
            var expected = 2;

            // When
            var length = UnicodeCalculator.GetWidth(codePoint);

            // Then
            length.ShouldBe(expected);
        }

#if NET6_0_OR_GREATER
        [Fact]
        public void Test_Rune_Overload()
        {
            // Given
            var scalar = new Rune(0x1F32D); // 🌭
            var expected = 2;

            // When
            var length = UnicodeCalculator.GetWidth(scalar);

            // Then
            length.ShouldBe(expected);
        }
#endif
    }
}
