using System;
using System.Collections.Generic;
using System.Text;
using Spectre.Cli;

namespace Generator.Commands
{
    public sealed class TableGeneratorSettings : CommandSettings
    {
        [CommandArgument(0, "<OUTPUT>")]
        public string Output { get; set; }

        [CommandOption("-d|--data <PATH>")]
        public string Data { get; set; }
    }
}
