#if WCWIDTH
#pragma warning restore
#else
#pragma warning disable
#endif

namespace Wcwidth;

/// <summary>
/// Keeps the table of each Unicode version, generated on first use
/// since we don't want to load everything into memory.
/// It is safe to use from several threads.
/// </summary>
internal sealed class UnicodeTableCache
{
    private readonly Func<Unicode, int[,]> _generate;
    private readonly int[,]?[] _tables;

    public UnicodeTableCache(Func<Unicode, int[,]> generate)
    {
        _generate = generate;

        // The versions are numbered from 0 without gaps
        _tables = new int[,]?[Enum.GetValues(typeof(Unicode)).Length];
    }

    public int[,] Get(Unicode version)
    {
        var index = (int)version;
        if ((uint)index >= (uint)_tables.Length)
        {
            // Not a known version
            return _generate(version);
        }

        var table = Volatile.Read(ref _tables[index]);
        if (table is null)
        {
            // Threads that race here may each generate the table,
            // but only the first one is kept and returned to all
            table = _generate(version);
            table = Interlocked.CompareExchange(ref _tables[index], table, null) ?? table;
        }

        return table;
    }
}