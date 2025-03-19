using Moryx.Cli.Templates.Models;
using System.Text.Json;

namespace Moryx.Cli.Templates.Extensions
{
    public static class TemplateConfigurationExtensions
    {
        public static void Save(this TemplateConfiguration @this, string fileName)
        {
            var content = JsonSerializer.Serialize(@this, new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true });
            File.WriteAllText(fileName, content);
        }
    }
}
