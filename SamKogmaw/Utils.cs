using System.Diagnostics;
using EloBuddy;

namespace SamKogmaw
{
    internal static class Utils
    {
        private const bool DebugMode = true;
        private const string HexMainColor = "#ff1990"; // System.Drawing.Color.DodgerBlue

        public static void Debug<T>(T s)
        {
            if(!DebugMode) return;
            Chat.Print("<font color='#f03040'>"+Program.AssemblyName+":</font> <font color='#ffffff'>" + s + "</font>");
        }
        public static void Print<T>(T s, System.Drawing.Color c)
        {
            Chat.Print("<font color='" + HexMainColor + "'>" + Program.AssemblyName + ":</font> <font color='" + HexConverter(c) + "'>" + s + "</font>");
        }
        public static string HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static void Print<T>(T s)
        {
            Print(s, System.Drawing.Color.White);
        }
    }
}