using System.Text;
using System.Text.RegularExpressions;

namespace Moryx.Cli.Templates.Extensions
{
    public static class StringExtensions
    {
        public static List<string> ToCleanList(this string str)
        {
            return str
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .ToList();
        }

        public static string ReplaceFileExtension(this string str, string oldStr, string newStr)
        {
            var position = str.LastIndexOf(oldStr);
            if (position == -1)
            {
                return str;
            }
            return str.Remove(position).Insert(position, newStr);
        }

        public static string StateBase(this string str) => str + "StateBase";

        public static string AsFolderName(this string url)
        {
            var pattern = @"[\:\/\?\#\[\]\!\$\&\'\(\)\*\+\,\;\=]";
            url = Regex.Replace(url, pattern, "_");
            var result = new StringBuilder();
            var previous = '\0';
            foreach (char current in url)
            {
                if (current != '_' || (current == '_' && current != previous))
                {
                    result.Append(current);
                    previous = current;
                }
            }
            return result.ToString();
        }
    }
}
