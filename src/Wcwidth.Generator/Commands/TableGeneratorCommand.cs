namespace Wcwidth.Generator;

[Description("Generates Unicode tables")]
public sealed class TableGeneratorCommand : AsyncCommand<TableGeneratorSettings>
{
    private readonly IFileSystem _fileSystem = new FileSystem();

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

        // Define all generators to run
        var generators = new List<(string Name, Func<TableGeneratorContext, Task<UnicodeRenderingContext>> Run)>
        {
            ("Wide", TableGenerator.GenerateWide),
            ("Zero Width", TableGenerator.GenerateZeroWidth),
            ("Narrow To Wide", TableGenerator.GenerateVs16),
        };

        foreach (var generator in generators)
        {
            var ctx = new TableGeneratorContext
            {
                DataPath = data,
            };

            AnsiConsole.MarkupLine($"⏳ Generating [yellow]{generator.Name}[/]...");
            var renderingCtx = await generator.Run(ctx);
            var result = await TableRenderer.Render(renderingCtx);

            // Write the generated source to disk
            var file = output.CombineWithFilePath($"{renderingCtx.ClassName}.Generated.cs");
            AnsiConsole.MarkupLine($"💾 Saving [yellow]{file.GetFilename().FullPath}[/]...");
            await File.WriteAllTextAsync(file.FullPath, result, cancellationToken);
        }

        return 0;
    }
}