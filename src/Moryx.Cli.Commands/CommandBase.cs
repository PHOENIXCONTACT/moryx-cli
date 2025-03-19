using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;

namespace Moryx.Cli.Commands
{
    public class CommandBase
    {
        public static CommandResult Exec(Template template, Func<CommandResult> func)
        {
            return Exec(template.Settings, func);
        }

        public static CommandResult Exec(TemplateSettings settings, Func<CommandResult> func)
        {
            TemplateRepository.Clone(settings);
            return func();
        }
    }
}
