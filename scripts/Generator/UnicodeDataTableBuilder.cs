using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Generator.Models;
using Spectre.Console;

namespace Generator.Utilities
{
    public static class UnicodeDataTableBuilder
    {
        public static UnicodeDataTable Build(
            string source,
            string version,
            List<int> values)
        {
            var ranges = CollapseRanges(values);

            return new UnicodeDataTable
            {
                Source = source,
                Version = version,
                Ranges = ranges.OrderBy(x => x.Item1).Select((value) =>
                {
                    return new UnicodeDataRange
                    {
                        Start = value.Item1,
                        End = value.Item2
                    };
                }).ToList()
            };
        }

        private static Deque<(int, int)> CollapseRanges(List<int> values)
        {
            var queue = new Deque<(int, int)>();

            var start = values[0];
            var end = values[0];

            foreach (var (index, value) in values.Enumerate())
            {
                if (index == 0)
                {
                    queue.Append((value, value));
                    continue;
                }

                (start, end) = queue.Pop();
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
