namespace Wcwidth.Generator;

public static class TableGenerator
{
    public static async Task<UnicodeRenderingContext> GenerateWide(TableGeneratorContext context)
    {
        var data = new List<UnicodeData>();

        foreach (var version in context.GetUnicodeVersions())
        {
            var table = ParseCategory(await UnicodeDataFile.EastAsianWidth(context, version), wide: 2);

            // Subtract wide characters that were defined above as 'W' category in EastAsianWidth,
            // but also zero-width category 'Mn' or 'Mc' in DerivedGeneralCategory
            table.Values.ExceptWith(
                ParseCategory(
                    await UnicodeDataFile.DerivedGeneralCategory(context, version),
                    wide: 0).Values);

            // Also subtract Hangul Jamo Vowels and Hangul Trailing Consonants
            table.Values.ExceptWith(Enumerable.RangeStartEnd(0x1160, 0x1200));
            table.Values.ExceptWith(Enumerable.RangeStartEnd(0xD7B0, 0xD800));

            // Finally, join with atypical 'wide' characters defined by category 'Sk',
            table.Values.AddRange(
                ParseCategory(
                    await UnicodeDataFile.DerivedGeneralCategory(context, version),
                    wide: 2).Values);

            data.Add(new UnicodeData
            {
                Version = version,
                Ranges = table.BuildRanges(),
            }); ;
        }

        return new UnicodeRenderingContext
        {
            ClassName = "WideTable",
            Data = data,
        };
    }

    public static async Task<UnicodeRenderingContext> GenerateZeroWidth(TableGeneratorContext context)
    {
        var data = new List<UnicodeData>();

        foreach (var version in context.GetUnicodeVersions())
        {
            var table = ParseCategory(
                await UnicodeDataFile.DerivedGeneralCategory(context, version), wide: 0);

            // Add NULL
            table.Values.Add(0);

            // Add Hangul Jungseong Filler … Hangul Jongseong Ssangnieun
            table.Values.AddRange(Enumerable.RangeStartEnd(0x1160, 0x1200));
            // Add Hangul Jungseong O-Yeo  … Undefined Character of Hangul Jamo Extended-B
            table.Values.AddRange(Enumerable.RangeStartEnd(0xD7B0, 0xD800));

            // Remove SOFT HYPHEN
            table.Values.Remove(0x00AD);

            data.Add(new UnicodeData
            {
                Version = version,
                Ranges = table.BuildRanges(),
            }); ;
        }

        return new UnicodeRenderingContext
        {
            ClassName = "ZeroTable",
            Data = data,
        };
    }

    public static async Task<UnicodeRenderingContext> GenerateVs16(TableGeneratorContext context)
    {
        var data = new List<UnicodeData>();
        var wideTables = (await GenerateWide(context)).Data;
        var unicodeLatest = context.GetUnicodeVersions().Last();
        var unicodeVersion = "9.0.0";

        // Parse table formatted by the latest emoji release (developed with 15.1.0)
        // and parse a single file for all individual releases
        var table = ParseVs16Data(
            await UnicodeDataFile.EmojiVariationSequences(
                context, unicodeLatest), unicodeVersion);

        // Parse and join the final emoji release 12.0 of the earlier "type"
        table.Values.AddRange(
            ParseVs16Data(
                await UnicodeDataFile.LegacyEmojiVariationSequences(context),
                unicodeVersion).Values);

        // Perform culling on any values that are already understood as 'wide'
        // without the variation-16 selector
        var wideTable = wideTables.Single(x => x.Version == unicodeVersion).ToBinarySearchableArray();
        table.Values = new HashSet<int>(table.Values.Where(v => !wideTable.Exist(v)));

        return new UnicodeRenderingContext
        {
            ClassName = "Vs16Table",
            Data =
            [
                new UnicodeData
                {
                    Version = unicodeVersion,
                    Ranges = table.BuildRanges(),
                }
            ],
        };
    }

    private static UnicodeTable ParseVs16Data(
        UnicodeDataFile file, string unicodeVersion)
    {
        using var stream = file.OpenStream();
        var entries = Vs16TableParser.Parse(stream);
        var values = entries.Select(e => e.Range!.Value.Start);
        return new UnicodeTable { Version = unicodeVersion, Values = values.ToHashSet(), };
    }

    private static UnicodeTable ParseCategory(UnicodeDataFile file, int wide)
    {
        using var stream = file.OpenStream();
        var entries = UnicodeTableParser.Parse(stream);
        var values = ParseWidthCategoryValues(entries, wide);
        return new UnicodeTable { Version = file.Version, Values = values.ToHashSet(), };
    }

    private static IEnumerable<int> ParseWidthCategoryValues(
        IEnumerable<UnicodeTableEntry> entries, int wide)
    {
        foreach (var entry in entries)
        {
            if (entry.Range != null && entry.FilterByCategoryWidth(wide))
            {
                var start = entry.Range.Value.Start;
                var end = entry.Range.Value.End;

                foreach (var value in Enumerable.Range(start, end - start + 1))
                {
                    yield return value;
                }
            }
        }
    }
}

public sealed class UnicodeRenderingContext
{
    public required string ClassName { get; init; }
    public required List<UnicodeData> Data { get; init; }
}

public sealed class UnicodeTable
{
    public required string Version { get; init; }
    public required HashSet<int> Values { get; set; }

    public List<(int Start, int End)> BuildRanges()
    {
        return CollapseRanges(Values).OrderBy(x => x.Start).ToList();
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