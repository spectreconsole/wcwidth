namespace Wcwidth.Generator;

public sealed class TableGeneratorSettings : CommandSettings
{
    [CommandArgument(0, "<OUTPUT>")]
    [Description("The output path.")]
    public string Output { get; set; } = null!;

    [CommandOption("-i|--input <PATH>")]
    [Description("The input path where Unicode data is downloaded to.")]
    public string? Input { get; set; }
}