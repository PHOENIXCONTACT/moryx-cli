using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Template;
using Moryx.Cli.Template.Models;

namespace Moryx.Cli.Commands
{
    public static class AddResources
    {
        public static CommandResult Exec(TemplateSettings settings, IEnumerable<string> resources)
        {
            return CommandBase.Exec(settings, (filenames)
                => resources.Select(resource => AddThing.Exec(
                    settings,
                    new AddConfig
                    {
                        SolutionName = settings.AppName,
                        ThingName = $"{resource}Resource",
                        Thing = "resource",
                        ThingPlaceholders = 
                        [
                            Template.Template.ResourcePlaceholder, 
                            Template.Template.ResourcePlaceholder2
                        ],
                    },
                    filenames.Resource()
                    ))
                    .ToArray()
                    .Flatten());
        }
    }
}