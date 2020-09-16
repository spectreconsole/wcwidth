using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Generator.Models;
using Generator.Templating;
using Generator.Utilities;
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

        public async Task<string> Build(DirectoryPath data, IEnumerable<string> versions)
        {
            // Compile the template
            var template = ScribanTemplate.Parse(File.ReadAllText(Template));

            // Get data
            var model = new ScriptObject();
            model["data"] = await GetData(data, versions, Filter);
            model["name"] = ClassName;

            // Prepare context
            var context = new TemplateContext();
            context.PushGlobal(model);
            context.PushGlobal(new ScribanHelpers());
            context.LoopLimit = 100000;

            // Render template
            return template.Render(context);
        }

        protected abstract string GetUrl(string version);
        protected abstract bool Filter(string category);

        private async Task<IEnumerable<UnicodeDataTable>> GetData(
            DirectoryPath data,
            IEnumerable<string> versions,
            Func<string, bool> predicate)
        {
            var result = new List<UnicodeDataTable>();
            foreach (var version in versions)
            {
                var url = GetUrl(version);
                var filename = data.CombineWithFilePath($"{DataFilename}_{version}.txt");

                using var stream = await UnicodeDataRetriever.GetData(filename, url);
                result.Add(UnicodeDataParser.Parse(url, version, stream, predicate));
            }

            return result;
        }
    }
}
