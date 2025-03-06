using Moryx.Cli.Commands.Components;
using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;

namespace Moryx.Cli.Commands
{
    public static class AddModule
    {
        public static CommandResult Exec(Template template, string moduleName)
        {
            return CommandBase.Exec(template, () =>
                AddThing.Exec(
                template,
                new AddConfig
                {
                    SolutionName = template.AppName,
                    ThingName = moduleName,
                    Thing = "module",
                    ThingPlaceholders = [Template.ModulePlaceholder],
                },
                template.Module(moduleName),
                s => s.AddProjectsToSolution(template.Settings)
                ));            
        }
    }
}