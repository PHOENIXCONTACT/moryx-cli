using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;

namespace Moryx.Cli.Commands
{
    public static class AddResources
    {
        public static CommandResult Exec(Template template, IEnumerable<string> resources)
        {
            return CommandBase.Exec(template, ()
                => resources.Select(resource =>
                {
                    var addConfig = new AddConfig
                    {
                        SolutionName = template.AppName,
                        ThingName = $"{resource}Resource",
                        Thing = "resource",
                        ThingPlaceholders =
                        [
                            Template.ResourcePlaceholder,
                            Template.ResourcePlaceholder2
                        ],
                    };
                    return AddThing.Exec(
                        template,
                        addConfig,
                        template.Resource(resource)
                    );
                })
                .ToArray()
                .Flatten());
        }
    }
}