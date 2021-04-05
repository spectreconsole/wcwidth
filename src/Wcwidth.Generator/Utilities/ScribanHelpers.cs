using System;
using Scriban.Runtime;

namespace Generator
{
    public class ScribanHelpers : ScriptObject
    {
        public static int RangeStart((int Start, int End) range)
        {
            return range.Start;
        }

        public static int RangeEnd((int Start, int End) range)
        {
            return range.End;
        }

        public static string StringVer(string version)
        {
            if (version is null)
            {
                return string.Empty;
            }

            return version.Replace(".", "_", StringComparison.Ordinal);
        }

        public static string HexPad(int value, int pad)
        {
            return string.Format("0x{0}", value.ToString("X").PadLeft(pad, '0'));
        }
    }
}
