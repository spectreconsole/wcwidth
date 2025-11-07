namespace Wcwidth.Generator;

public static class Extensions
{
    extension(Match match)
    {
        public string? GetGroupValue(string group, string? defaultValue = null)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            return match.Groups[group].Success
                ? match.Groups[group].Value
                : defaultValue;
        }
    }

    extension<T>(IEnumerator<T> source)
    {
        private IEnumerable<(int Index, T Item)> Enumerate()
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var last = !source.MoveNext();

            for (var index = 0; !last; index++)
            {
                var current = source.Current;
                last = !source.MoveNext();
                yield return (index, current);
            }
        }
    }

    extension<T>(IEnumerable<T> source)
    {
        public IEnumerable<(int Index, T Item)> Enumerate()
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Enumerate(source.GetEnumerator());
        }
    }

    extension(Enumerable)
    {
        public static IEnumerable<int> RangeStartEnd(int start, int end)
        {
            return Enumerable.Range(start, end - start + 1);
        }
    }

    extension<T>(HashSet<T> source)
    {
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                source.Add(item);
            }
        }
    }

    extension(string text)
    {
        public (string Before, string After) Partition(string separator)
        {
            var index = text.IndexOf(separator, StringComparison.Ordinal);
            if (index == -1)
            {
                return (text, string.Empty);
            }

            return (
                text[..index],
                text.Substring(index, text.Length - index));
        }
    }
}