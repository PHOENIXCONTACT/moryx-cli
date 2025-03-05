using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;

namespace Moryx.Cli.Commands
{
    public static class AddResources
    {
        public static CommandResult Exec(TemplateSettings settings, IEnumerable<string> resources)
        {
            return CommandBase.Exec(settings, (filenames)
                => resources.Select(resource =>
                {
                    var addConfig = new AddConfig
                    {
                        SolutionName = settings.AppName,
                        ThingName = $"{resource}Resource",
                        Thing = "resource",
                        ThingPlaceholders =
                        [
                            Template.ResourcePlaceholder,
                            Template.ResourcePlaceholder2
                        ],
                    };
                    return AddThing.Exec(
                        settings,
                        addConfig,
                        filenames.Resource()
                    );
                })
                .ToArray()
                .Flatten());
        }
    }
}