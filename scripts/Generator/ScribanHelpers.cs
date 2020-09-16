using System;
using System.Collections.Generic;
using System.Text;
using Scriban.Runtime;

namespace Generator.Templating
{
    public class ScribanHelpers : ScriptObject
    {
        public static string StringVer(string version)
        {
            return version.Replace(".", "_");
        }

        public static string HexPad(int value, int pad)
        {
            return string.Format("0x{0}", value.ToString("X").PadLeft(pad, '0'));
        }
    }
}
