namespace Wcwidth.Generator;

public static class Vs16TableParser
{
    public static IEnumerable<UnicodeTableEntry> Parse(Stream stream)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using var reader = new StreamReader(stream);
        while (true)
        {
            var line = reader.ReadLine();
            if (line == null)
            {
                break;
            }

            if (line.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var entry = ParseLine(line);
            if (entry != null)
            {
                yield return entry;
            }
        }
    }

    private static UnicodeTableEntry? ParseLine(string line)
    {
        if (line.StartsWith("#"))
        {
            return null;
        }

        var (data, comment) = line.Partition("#");

        var fields = data.Split(';', StringSplitOptions.RemoveEmptyEntries);
        var codePointsString = fields[0];
        var properties = fields.Skip(1).ToArray();

        var codePoints = codePointsString.Trim().Split(' ');
        if (codePoints is [_, "FE0F"])
        {
            return new UnicodeTableEntry
            {
                Range = GetStartAndEnd(codePoints[0], codePoints[1]),
                Property = string.Join(";", properties),
                Comment = comment,
            };
        }

        return null;
    }

    private static (int Start, int End)? GetStartAndEnd(string? start, string? end)
    {
        if (start == null)
        {
            return null;
        }

        var startInteger = int.Parse(start, NumberStyles.HexNumber);
        var endInteger = end != null ? int.Parse(end, NumberStyles.HexNumber) : startInteger;

        return (startInteger, endInteger);
    }
}