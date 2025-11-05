using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using Spectre.Console;
using Spectre.IO;
using Wcwidth;

namespace Generator
{
    [Description("Generates Unicode tables")]
    public sealed class TableGeneratorCommand : AsyncCommand<TableGeneratorSettings>
    {
        private readonly IFileSystem _fileSystem = new FileSystem();
        private readonly List<TableGenerator> _generators = new()
        {
            new ZeroTableGenerator(),
            new WideTableGenerator(),
        };

        public override async Task<int> ExecuteAsync(
            CommandContext context, TableGeneratorSettings settings,
            CancellationToken cancellationToken)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

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
            var versions = GetUnicodeVersions();

            foreach (var generator in _generators)
            {
                // Generate the source
                AnsiConsole.MarkupLine($"⏳ Generating [yellow]{generator.ClassName}[/]...");
                var result = await generator.Build(data, versions);

                // Write the generated source to disk
                var file = output.CombineWithFilePath($"{generator.ClassName}.Generated.cs");
                AnsiConsole.MarkupLine($"💾 Saving [yellow]{file.GetFilename().FullPath}[/]...");
                await File.WriteAllTextAsync(file.FullPath, result, cancellationToken);
            }

            return 0;
        }

        private static List<string> GetUnicodeVersions()
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
