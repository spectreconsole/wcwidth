namespace Smoke
{
    /// <summary>
    /// Uses the API of the package, so that this project
    /// also fails when an overload is missing for a setup.
    /// </summary>
    internal static class Probe
    {
        public static int Measure()
        {
            var width = global::Wcwidth.UnicodeCalculator.GetWidth('a');
            width += global::Wcwidth.UnicodeCalculator.GetWidth(0x1F600, global::Wcwidth.Unicode.Version_9_0_0);
            width += global::Wcwidth.UnicodeCalculator.GetWidth("text");
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER || WCWIDTH_SYSTEM_MEMORY
            width += global::Wcwidth.UnicodeCalculator.GetWidth(System.MemoryExtensions.AsSpan("text"));
#endif
#if NET6_0_OR_GREATER
            width += global::Wcwidth.UnicodeCalculator.GetWidth(new System.Text.Rune(0x1F600));
#endif
            return width;
        }
    }
}
