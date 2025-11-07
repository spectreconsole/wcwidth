namespace Wcwidth.Generator;

public sealed class UnicodeData
{
    public required string Version { get; init; }
    public required List<(int Start, int End)> Ranges { get; init; }

    public int[,] ToBinarySearchableArray()
    {
        var array = new int[Ranges.Count, 2];
        for (var i = 0; i < Ranges.Count; i++)
        {
            array[i, 0] = Ranges[i].Start;
            array[i, 1] = Ranges[i].End;
        }

        return array;
    }
}