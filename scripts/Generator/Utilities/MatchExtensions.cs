using System.Text.RegularExpressions;

namespace Generator
{
    public static class MatchExtensions
    {
        public static string GetGroupValue(this Match match, string group, string defaultValue = null)
        {
            return match.Groups[group].Success
                ? match.Groups[group].Value
                : defaultValue;
        }
    }
}
