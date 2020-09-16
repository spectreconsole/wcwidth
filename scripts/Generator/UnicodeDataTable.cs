using System;
using System.Collections.Generic;
using System.Text;

namespace Generator.Models
{
    public sealed class UnicodeDataTable
    {
        public string Version { get; set; }
        public string Source { get; set; }
        public List<UnicodeDataRange> Ranges { get; set; }
    }
}
