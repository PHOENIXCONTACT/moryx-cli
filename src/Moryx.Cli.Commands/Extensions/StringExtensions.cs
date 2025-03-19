using Moryx.Cli.Commands.Components;
using Moryx.Cli.Templates.Models;
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

        public static string Replace(this string s, Dictionary<string, string> dictionary)
        {
            foreach (var entry in dictionary)
            {
                s = s.Replace(entry.Key, entry.Value);
            }
            return s;
        }

        public static string Capitalize([NotNull] this string s)
            => s[0].ToString().ToUpper() + s.Substring(1);

        public static void AddProjectsToSolution(this IEnumerable<string> fileNames, TemplateSettings settings)
        {
            var projectFiles = fileNames
                .Where(f => f.EndsWith(".csproj"))
                .ToList();

            foreach (var file in projectFiles)
            {
                SolutionFileManipulation.AddProjectToSolution(settings, file);
            }
        }
    }
}
