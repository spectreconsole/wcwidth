using System;

namespace Generator
{
    public sealed class WideTableGenerator : TableGenerator
    {
        public override string ClassName => "WideTable";
        protected override string DataFilename => "EastAsianWidth";

        protected override string GetUrl(string version)
        {
            return $"http://www.unicode.org/Public/{version}/ucd/EastAsianWidth.txt";
        }

        protected override bool Filter(string category)
        {
            return category.Equals("W", StringComparison.OrdinalIgnoreCase) ||
                category.Equals("F", StringComparison.OrdinalIgnoreCase);
        }
    }
}
