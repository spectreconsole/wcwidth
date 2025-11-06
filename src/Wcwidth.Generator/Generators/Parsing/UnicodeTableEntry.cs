namespace Generator;

public sealed class UnicodeTableEntry
{
    public required (int Start, int End)? Range { get; init; }
    public required string Property { get; init; }
    public required string Comment { get; init; }

    public bool FilterByCategoryWidth(int width)
    {
        if (Range == null)
        {
            return false;
        }

        return Property switch
        {
            "Sk" when Comment.Contains("EMOJI MODIFIER") => width == 0,
            "Sk" when Comment.Contains("FULLWIDTH") => width == 2,
            "Sk" => width == 1,
            "Me" or "Mn" or "Mc" or "Cf" or "Zl" or "Zp" => width == 0,
            "W" or "F" => width == 2,
            _ => width == 1
        };
    }
}