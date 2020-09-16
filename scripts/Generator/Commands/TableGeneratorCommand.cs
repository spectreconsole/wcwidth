using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Spectre.Cli;
using Spectre.Console;
using Spectre.IO;
using Wcwidth;

namespace Generator
{
    [Description("Generates Unicode tables")]
    public sealed class TableGeneratorCommand : AsyncCommand<TableGeneratorSettings>
    {
        private readonly IFileSystem _fileSystem;
        private readonly List<TableGenerator> _generators;

        public TableGeneratorCommand()
        {
            _fileSystem = new FileSystem();
            _generators = new List<TableGenerator>
            {
                new ZeroTableGenerator(),
                new WideTableGenerator(),
            };
        }

        public override async Task<int> ExecuteAsync(CommandContext context, TableGeneratorSettings settings)
        {
            // Get the output path
            var output = new DirectoryPath(settings.Output);
            if (!_fileSystem.Directory.Exists(settings.Output))
            {
                _fileSystem.Directory.Create(settings.Output);
            }

            // Get the data path
            var data = settings.Input != null
                ? new DirectoryPath(settings.Input)
                : output;

            // Get all versions
            var versions = GetVersions();

            foreach (var generator in _generators)
            {
                // Generate the source
                AnsiConsole.MarkupLine("⏳ Generating [yellow]{0}[/]...", generator.ClassName);
                var result = await generator.Build(data, versions);

                // Write the generated source to disk
                var file = output.CombineWithFilePath($"{generator.ClassName}.Generated.cs");
                AnsiConsole.MarkupLine("💾 Saving [yellow]{0}[/]...", file.GetFilename().FullPath);
                File.WriteAllText(file.FullPath, result);
            }

            return 0;
        }

        private List<string> GetVersions()
        {
            var result = new List<string>();
            foreach (var field in typeof(Unicode).GetFields().Where(x => x.IsStatic))
            {
                var attr = field.GetCustomAttribute<DescriptionAttribute>();
                if (attr == null || string.IsNullOrWhiteSpace(attr.Description))
                {
                    throw new InvalidOperationException("Unicode version enum is missing version attribute.");
                }

                result.Add(attr.Description);
            }

            return result;
        }
    }
}
