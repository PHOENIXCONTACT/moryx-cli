using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;

namespace Moryx.Cli.Commands
{
    public class CommandBase
    {
        public static CommandResult Exec(TemplateSettings settings, Func<List<string>, CommandResult> func)
        {
            TemplateRepository.Clone(settings);
            var cleanedResourceNames = Template.GetCleanedResourceNames(settings);
            return func(cleanedResourceNames);
        }
    }
}
