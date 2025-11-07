namespace Wcwidth.Generator;

public sealed class UnicodeDataFile
{
    public string Version { get; }
    private readonly FilePath _filename;

    private UnicodeDataFile(FilePath filename, string version)
    {
        Version = version ?? throw new ArgumentNullException(nameof(version));
        _filename = filename ?? throw new ArgumentNullException(nameof(filename));
    }

    public Stream OpenStream()
    {
        return File.OpenRead(_filename.FullPath);
    }

    public static async Task<UnicodeDataFile> EastAsianWidth(TableGeneratorContext context, string version)
    {
        var url = $"https://www.unicode.org/Public/{version}/ucd/EastAsianWidth.txt";
        var filename = $"EastAsianWidth_{version}.txt";
        return await GetDataFile(context, new Uri(url), filename, version);
    }

    public static async Task<UnicodeDataFile> DerivedGeneralCategory(TableGeneratorContext context, string version)
    {
        var url = $"https://www.unicode.org/Public/{version}/ucd/extracted/DerivedGeneralCategory.txt";
        var filename = $"DerivedGeneralCategory_{version}.txt";
        return await GetDataFile(context, new Uri(url), filename, version);
    }

    public static async Task<UnicodeDataFile> LegacyEmojiVariationSequences(TableGeneratorContext context)
    {
        var version = "12.0";
        var url = $"https://unicode.org/Public/emoji/{version}/emoji-variation-sequences.txt";
        var filename = $"LegacyEmojiVariationSequences_{version}.txt";
        return await GetDataFile(context, new Uri(url), filename, version);
    }

    public static async Task<UnicodeDataFile> EmojiVariationSequences(TableGeneratorContext context, string version)
    {
        var url = $"https://unicode.org/Public/{version}/ucd/emoji/emoji-variation-sequences.txt";
        var filename = $"EmojiVariationSequences_{version}.txt";
        return await GetDataFile(context, new Uri(url), filename, version);
    }

    private static async Task<UnicodeDataFile> GetDataFile(TableGeneratorContext context, Uri url, FilePath filename, string version)
    {
        filename = context.DataPath.CombineWithFilePath(filename);

        if (!File.Exists(filename.FullPath))
        {
            AnsiConsole.MarkupLine($"🌍 Downloading [yellow]{url}[/]...");
            using var client = new HttpClient();
            var content = await client.GetStringAsync(url);
            await File.WriteAllTextAsync(filename.FullPath, content);
        }

        return new UnicodeDataFile(filename, version);
    }
}