using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates;

namespace Moryx.Cli.Commands
{
    public static class AddSteps
    {
        public static CommandResult Exec(Template template, IEnumerable<string> cells)
        {
            return CommandBase.Exec(template, ()
                => cells.Select(step => AddStep.Exec(
                    template,
                    step
                    ))
                    .ToArray()
                    .Flatten());
        }
    }
}