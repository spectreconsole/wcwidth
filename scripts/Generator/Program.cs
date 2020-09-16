using System;
using Generator.Commands;
using Spectre.Cli;

namespace Generator
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandApp<TableGeneratorCommand>();
            return app.Run(args);
        }
    }
}
