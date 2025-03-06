using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;

namespace Moryx.Cli.Commands
{
    public class CommandBase
    {
        public static CommandResult Exec(Template template, Func<CommandResult> func)
        {
            TemplateRepository.Clone(template.Settings);
            return func();
        }
    }
}
