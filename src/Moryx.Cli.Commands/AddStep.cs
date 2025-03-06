using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates;

namespace Moryx.Cli.Commands
{
    public static class AddStep
    {
        public static CommandResult Exec(Template template, string step)
        {
            return CommandBase.Exec(template, () =>
            {
                var addConfig = new AddConfig
                {
                    SolutionName = template.AppName,
                    ThingName = step,
                    Thing = "step",
                    ThingPlaceholders = [Template.StepPlaceholder],
                };
                var namespacePlaceholder = new Dictionary<string, string> {

                    { $"{Template.AppPlaceholder}.Resources", $"{template.AppName}.Resources.{step}" },
                    { $"{template.AppName}.Resources", $"{template.AppName}.Resources.{step}" }
                };  
                var replacements = new StringReplacements(addConfig)
                    .AddFileNamePatterns(namespacePlaceholder)
                    .AddContentPatterns(namespacePlaceholder)
                    ;
                
                return AddThing.Exec(
                    template,
                    addConfig,
                    template.Step(step),
                    (createdFiles) =>
                    {
                        createdFiles.AddProjectsToSolution(template.Settings);
                    },
                    replacements
                );
            });
        }
    }
}