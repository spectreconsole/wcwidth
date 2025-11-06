using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Generator;

public static class Vs16TableParser
{
    private static readonly Regex _regex;

    static Vs16TableParser()
    {
        _regex = new Regex("^(?<start>[0-9A-F]{4,6})(..(?<end>[0-9A-F]{4,6})?)?\\s*;(?<property>[A-Za-z ]*)#(?<comment>.*$)");
    }

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
        if (codePoints.Length == 2 && codePoints[1] == "FE0F")
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