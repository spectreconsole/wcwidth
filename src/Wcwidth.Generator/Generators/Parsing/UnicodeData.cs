using System.Collections.Generic;

namespace Generator
{
    public sealed class UnicodeData
    {
        public string Version { get; set; }
        public List<(int Start, int End)> Ranges { get; set; }
    }
}
