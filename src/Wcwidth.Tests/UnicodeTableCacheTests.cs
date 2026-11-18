namespace Wcwidth.Tests;

public sealed class UnicodeTableCacheTests
{
    [Fact]
    public void Should_Generate_Each_Table_Once()
    {
        // Given
        var calls = new int[Enum.GetValues(typeof(Unicode)).Length];
        var cache = new UnicodeTableCache(version =>
        {
            Interlocked.Increment(ref calls[(int)version]);
            return new int[,] { { 0, 0 } };
        });

        // When
        var first = cache.Get(Unicode.Version_9_0_0);
        var second = cache.Get(Unicode.Version_9_0_0);

        // Then
        second.ShouldBeSameAs(first);
        calls[(int)Unicode.Version_9_0_0].ShouldBe(1);
        calls.Sum().ShouldBe(1);
    }

    [Fact]
    public async Task Should_Return_The_Same_Table_To_Racing_Threads()
    {
        // Given
        var versions = Enum.GetValues(typeof(Unicode)).Cast<Unicode>().ToArray();
        var cache = new UnicodeTableCache(_ =>
        {
            Thread.SpinWait(1000);
            return new int[,] { { 0, 0 } };
        });
        using var start = new Barrier(8);

        // When
        var results = await Task.WhenAll(Enumerable.Range(0, 8)
            .Select(_ => Task.Run(() =>
            {
                start.SignalAndWait();
                return versions.Select(cache.Get).ToArray();
            })));

        // Then
        for (var index = 0; index < versions.Length; index++)
        {
            foreach (var result in results)
            {
                result[index].ShouldBeSameAs(results[0][index]);
            }
        }
    }

    [Fact]
    public void Should_Let_The_Generator_Report_Unknown_Versions()
    {
        Should.Throw<InvalidOperationException>(() => ZeroTable.GetTable((Unicode)999));
        Should.Throw<InvalidOperationException>(() => WideTable.GetTable((Unicode)(-1)));
    }

    [Fact]
    public void Tables_Should_Cache_Every_Version()
    {
        foreach (var version in Enum.GetValues(typeof(Unicode)).Cast<Unicode>())
        {
            ZeroTable.GetTable(version).ShouldBeSameAs(ZeroTable.GetTable(version));
            WideTable.GetTable(version).ShouldBeSameAs(WideTable.GetTable(version));
        }

        Vs16Table.GetTable(Unicode.Version_9_0_0).ShouldBeSameAs(Vs16Table.GetTable(Unicode.Version_9_0_0));
    }
}