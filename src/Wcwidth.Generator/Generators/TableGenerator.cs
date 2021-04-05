using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Scriban;
using Scriban.Runtime;
using Spectre.Console;
using Spectre.IO;
using ScribanTemplate = Scriban.Template;

namespace Generator
{
    public abstract class TableGenerator
    {
        public abstract string ClassName { get; }

        protected abstract string DataFilename { get; }
        protected virtual string Template { get; } = "Templates/Table.template";

        public async Task<string> Build(DirectoryPath path, IEnumerable<string> versions)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (versions is null)
            {
                throw new ArgumentNullException(nameof(versions));
            }

            // Compile the template
            var template = ScribanTemplate.Parse(File.ReadAllText(Template));

            // Get all data
            var data = await GetAllData(path, versions, Filter);

            // Prepare template context
            var context = new TemplateContext();
            context.LoopLimit = int.MaxValue;
            context.PushGlobal(new ScribanHelpers());
            context.PushGlobal(new ScriptObject
            {
                ["data"] = data,
                ["name"] = ClassName,
            });

            // Render template
            return template.Render(context);
        }

        protected abstract Uri GetUrl(string version);
        protected abstract bool Filter(string category);

        private async Task<IEnumerable<UnicodeData>> GetAllData(
            DirectoryPath data,
            IEnumerable<string> versions,
            Func<string, bool> predicate)
        {
            var result = new List<UnicodeData>();
            foreach (var version in versions)
            {
                using (var stream = await GetDataStream(data, version))
                {
                    result.Add(UnicodeDataParser.Parse(version, stream, predicate));
                }
            }

            return result;
        }

        private async Task<Stream> GetDataStream(DirectoryPath data, string version)
        {
            var url = GetUrl(version);
            var filename = data.CombineWithFilePath($"{DataFilename}_{version}.txt");

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
