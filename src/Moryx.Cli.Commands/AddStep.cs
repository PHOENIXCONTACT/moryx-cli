using Moryx.Cli.Template;
using Moryx.Cli.Template.Models;

namespace Moryx.Cli.Commands
{
    public static class AddStep
    {
        public static CommandResult Exec(TemplateSettings settings, string step)
        {
            return CommandBase.Exec(settings, (filenames)
                => AddThing.Exec(
                    settings,
                    new AddConfig
                    {
                        SolutionName = settings.AppName,
                        ThingName = step,
                        Thing = "step",
                        ThingPlaceholders = [Template.Template.StepPlaceholder],
                    },
                    filenames.Step()
                    ));
        }
    }
}