using System;

namespace Generator
{
    public sealed class ZeroTableGenerator : TableGenerator
    {
        public override string ClassName => "ZeroTable";
        protected override string DataFilename => "DerivedGeneralCategory";

        protected override string GetUrl(string version)
        {
            return $"http://www.unicode.org/Public/{version}/ucd/extracted/DerivedGeneralCategory.txt";
        }

        protected override bool Filter(string category)
        {
            return category.Equals("Me", StringComparison.OrdinalIgnoreCase) ||
                category.Equals("Mn", StringComparison.OrdinalIgnoreCase);
        }
    }
}
