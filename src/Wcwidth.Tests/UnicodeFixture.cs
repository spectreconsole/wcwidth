namespace Wcwidth.Tests;

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