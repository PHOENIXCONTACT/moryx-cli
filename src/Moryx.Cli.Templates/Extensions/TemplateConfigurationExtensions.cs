using Moryx.Cli.Templates.Models;
using System.IO;
using System.Text.Json;

namespace Moryx.Cli.Templates.Extensions
{
    public static class TemplateConfigurationExtensions
    {
        public static void Save(this TemplateConfiguration @this, string filename)
        {
            var content = JsonSerializer.Serialize(@this, new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true });
            File.WriteAllText(filename, content);
        }

        public static Tuple<string, string> SolutionPlaceholder(this TemplateConfiguration @this, string solutionName)
        {
            return new Tuple<string, string>(@this.Placeholders.SolutionName, solutionName);
        }
    }
}
