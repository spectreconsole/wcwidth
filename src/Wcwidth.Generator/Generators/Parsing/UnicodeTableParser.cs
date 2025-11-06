using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Generator;

public static class UnicodeTableParser
{
    private static readonly Regex _regex;

    static UnicodeTableParser()
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

        var match = _regex.Match(line);
        if (!match.Success)
        {
            throw new InvalidOperationException("Could not parse line");
        }

        var start = match.GetGroupValue("start", null)?.Trim();
        var end = match.GetGroupValue("end", null)?.Trim();
        var property = match.GetGroupValue("property", null)?.Trim();
        var comment = match.GetGroupValue("comment", null)?.Trim();

        return new UnicodeTableEntry
        {
            Range = GetStartAndEnd(start, end),
            Property = property ?? string.Empty,
            Comment = comment ?? string.Empty,
        };
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