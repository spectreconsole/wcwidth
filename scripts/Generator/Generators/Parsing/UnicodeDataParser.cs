using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Generator
{
    public static class UnicodeDataParser
    {
        private static readonly Regex _regex;

        static UnicodeDataParser()
        {
            _regex = new Regex("^(?<start>[0-9A-F]{4,6})(..(?<end>[0-9A-F]{4,6})?)?\\s*;(?<property>[A-Za-z ]*)#(?<comment>.*$)");
        }

        public static UnicodeData Parse(string version, Stream stream, Func<string, bool> predicate)
        {
            if (version is null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var result = new List<int>();

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

                var range = ParseLine(line, predicate);
                if (range != null)
                {
                    result.AddRange(range);
                }
            }

            return BuildTable(version, result);
        }

        private static IEnumerable<int> ParseLine(string line, Func<string, bool> predicate)
        {
            var match = _regex.Match(line);
            if (!match.Success)
            {
                throw new InvalidOperationException("Could not parse line");
            }

            var property = match.GetGroupValue("property", string.Empty).Trim();
            if (predicate(property))
            {
                var (start, end) = GetStartAndEnd(match);
                return Enumerable.Range(start, end - start + 1);
            }

            return null;
        }

        private static (int, int) GetStartAndEnd(Match match)
        {
            var start = match.GetGroupValue("start")?.Trim();
            if (start == null)
            {
                throw new InvalidOperationException("Missing start code point");
            }

            var end = match.GetGroupValue("end", start).Trim();

            return (
                int.Parse(start, NumberStyles.HexNumber),
                int.Parse(end, NumberStyles.HexNumber));
        }

        public static UnicodeData BuildTable(
            string version,
            IEnumerable<int> values)
        {
            var ranges = CollapseRanges(values);

            return new UnicodeData
            {
                Version = version,
                Ranges = ranges.OrderBy(x => x.Start).ToList()
            };
        }

        private static Deque<(int Start, int End)> CollapseRanges(IEnumerable<int> values)
        {
            var queue = new Deque<(int, int)>();

            foreach (var (index, value) in values.Enumerate())
            {
                if (index == 0)
                {
                    queue.Append((value, value));
                    continue;
                }

                var (start, end) = queue.Pop();
                if (end == value - 1)
                {
                    queue.Append((start, value));
                }
                else
                {
                    queue.Append((start, end));
                    queue.Append((value, value));
                }
            }

            return queue;
        }
    }
}
