using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.IO;

namespace Generator.Utilities
{
    public static class UnicodeDataRetriever
    {
        public static async Task<Stream> GetData(
            FilePath filename, string url)
        {
            if (!File.Exists(filename.FullPath))
            {
                AnsiConsole.MarkupLine($"🌍 Downloading [yellow]{url}[/]...");
                using var client = new HttpClient();
                var content = await client.GetStringAsync(url);
                await File.WriteAllTextAsync(filename.FullPath, content);
            }

            return File.OpenRead(filename.FullPath);
        }
    }
}
