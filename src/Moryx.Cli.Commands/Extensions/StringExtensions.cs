using System.Diagnostics.CodeAnalysis;

namespace Moryx.Cli.Commands.Extensions
{
    public static class StringExtensions
    {
        public static string Replace(this string s, IEnumerable<string> oldStrings, string newString)
        {
            foreach (var oldString in oldStrings)
            {
                s = s.Replace(oldString, newString);
            }
            return s;
        }

        public static string Capitalize([NotNull] this string s)
            => s[0].ToString().ToUpper() + s.Substring(1);
    }
}
