using System;

namespace Generator
{
    public sealed class WideTableGenerator : TableGenerator
    {
        public override string ClassName => "WideTable";
        protected override string DataFilename => "EastAsianWidth";

        protected override Uri GetUrl(string version)
        {
            if (version == Constants.Unicode_14_0_0.Version)
            {
                var filename = Constants.Unicode_14_0_0.Filenames.EastAsianWidth;
                return new Uri($"http://www.unicode.org/Public/{version}/ucd/{filename}");
            }

            return new Uri($"http://www.unicode.org/Public/{version}/ucd/EastAsianWidth.txt");
        }

        protected override bool Filter(string category)
        {
            return category.Equals("W", StringComparison.OrdinalIgnoreCase) ||
                category.Equals("F", StringComparison.OrdinalIgnoreCase);
        }
    }
}
