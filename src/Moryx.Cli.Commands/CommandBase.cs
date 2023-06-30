using Moryx.Cli.Template;
using Moryx.Cli.Template.Models;

namespace Moryx.Cli.Commands
{
    public class CommandBase
    {
        public static CommandResult Exec(TemplateSettings settings, Func<List<string>, CommandResult> func)
        {
            TemplateRepository.Clone(settings);
            var cleanedResourceNames = Template.Template.GetCleanedResourceNames(settings);
            return func(cleanedResourceNames);
        }
    }
}
