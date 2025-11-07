namespace Wcwidth.Generator;

public static class Program
{
    public static int Main(string[] args)
    {
        var app = new CommandApp();
        app.Configure(config =>
        {
            config.AddCommand<TableGeneratorCommand>("tables");
        });

        return app.Run(args);
    }
}