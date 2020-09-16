namespace Wcwidth
{
    internal readonly struct Interval
    {
        public int Start { get; }
        public int End { get; }

        public Interval(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}
