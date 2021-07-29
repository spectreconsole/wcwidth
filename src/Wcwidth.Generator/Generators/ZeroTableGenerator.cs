using System;

namespace Generator
{
    public sealed class ZeroTableGenerator : TableGenerator
    {
        public override string ClassName => "ZeroTable";
        protected override string DataFilename => "DerivedGeneralCategory";

        protected override Uri GetUrl(string version)
        {
            if (version == Constants.Unicode_14_0_0.Version)
            {
                var filename = Constants.Unicode_14_0_0.Filenames.DerivedGeneralCategory;
                return new Uri($"http://www.unicode.org/Public/{version}/ucd/extracted/{filename}");
            }

            return new Uri($"http://www.unicode.org/Public/{version}/ucd/extracted/DerivedGeneralCategory.txt");
        }

        protected override bool Filter(string category)
        {
            return category.Equals("Me", StringComparison.OrdinalIgnoreCase) ||
                category.Equals("Mn", StringComparison.OrdinalIgnoreCase);
        }
    }
}
