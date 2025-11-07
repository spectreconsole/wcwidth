namespace Wcwidth.Generator;

public sealed class UnicodeData
{
    public required string Version { get; init; }
    public required List<(int Start, int End)> Ranges { get; init; }

    public uint[,] ToBinarySearchableArray()
    {
        var l = new uint[Ranges.Count, 2];
        for (var i = 0; i < Ranges.Count; i++)
        {
            l[i, 0] = (uint)Ranges[i].Start;
            l[i, 1] = (uint)Ranges[i].End;
        }

        return l;
    }
}