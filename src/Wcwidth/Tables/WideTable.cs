#if WCWIDTH
#pragma warning restore
#else
#pragma warning disable
#endif

namespace Wcwidth;

internal static partial class WideTable
{
    private static readonly UnicodeTableCache _cache = new(GenerateTable);

    public static int[,] GetTable(Unicode version)
    {
        return _cache.Get(version);
    }
}