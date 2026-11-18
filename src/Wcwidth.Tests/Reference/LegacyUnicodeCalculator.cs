using System.Text;

namespace Wcwidth.Tests.Reference;

/// <summary>
/// A copy of <c>UnicodeCalculator.GetWidth(string)</c> from Wcwidth 4.0.1,
/// used as the reference that the current implementation must match.
/// </summary>
public static class LegacyUnicodeCalculator
{
    public static int GetWidth(string value, Unicode? version = null)
    {
        version ??= Unicode.Version_16_0_0;

        var runes = value.EnumerateRunes().ToArray();
        var end = runes.Length;

        var width = 0;
        var index = 0;
        var lastMeasureRune = default(Rune?);
        while (index < end)
        {
            var rune = runes[index];

            if (rune.Value == '\u200D')
            {
                // Zero Width Joiner, do not measure this or next character
                index += 2;
                continue;
            }

            if (rune.Value == '\uFE0F' && lastMeasureRune != null)
            {
                // on variation selector 16 (VS16) following another character,
                // conditionally add '1' to the measured width if that character is
                // known to be converted from narrow to wide by the VS16 character.
                if (version >= Unicode.Version_9_0_0)
                {
                    width += Vs16Table.GetTable(Unicode.Version_9_0_0).Find(lastMeasureRune.Value.Value);
                    lastMeasureRune = null;
                }

                index++;
                continue;
            }

            // Measure rune at current index
            var wcw = UnicodeCalculator.GetWidth(rune.Value, version);
            if (wcw < 0)
            {
                // Early return -1 on C0 and C1 control characters
                return wcw;
            }

            if (wcw > 0)
            {
                // Track last character measured to contain a cell, so that
                // subsequent VS-16 modifiers may be understood.
                lastMeasureRune = rune;
            }

            width += wcw;
            index++;
        }

        return width;
    }
}