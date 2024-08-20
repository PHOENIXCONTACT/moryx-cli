using Moryx.Cli.Commands.Options;
using Moryx.Cli.Template.Models;

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
    }
}
