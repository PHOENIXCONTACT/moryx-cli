using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Template;
using Moryx.Cli.Template.Models;

namespace Moryx.Cli.Commands
{
    public static class AddSteps
    {
        public static CommandResult Exec(TemplateSettings settings, IEnumerable<string> cells)
        {
            return CommandBase.Exec(settings, (filenames) 
                => cells.Select(step => AddStep.Exec(
                    settings,
                    step
                    ))
                    .ToArray()
                    .Flatten());
        }
    }
}