using Microsoft.Build.Construction;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Moryx.Cli.Commands.Components;
using Moryx.Cli.Template;
using Moryx.Cli.Template.Models;
using Moryx.Configuration;
using System.Diagnostics;

namespace Moryx.Cli.Commands
{
    public static class AddStep
    {
        public static CommandResult Exec(TemplateSettings settings, string step)
        {
            return CommandBase.Exec(settings, (filenames) =>
            {
                var addConfig = new AddConfig
                {
                    SolutionName = settings.AppName,
                    ThingName = step,
                    Thing = "step",
                    ThingPlaceholders = [Template.Template.StepPlaceholder],
                };
                var namespacePlaceholder = new Dictionary<string, string> {

                    { $"{Template.Template.AppPlaceholder}.Resources", $"{settings.AppName}.Resources.{step}" },
                    { $"{settings.AppName}.Resources", $"{settings.AppName}.Resources.{step}" }
                };
                var replacements = new StringReplacements(addConfig)
                    .AddFileNamePatterns(namespacePlaceholder)
                    .AddContentPatterns(namespacePlaceholder)
                    ;
                
                return AddThing.Exec(
                    settings,
                    addConfig,
                    filenames.Step(),
                    (createdFiles) =>
                    {
                        var projectFileName = createdFiles.FirstOrDefault(f => f.EndsWith(".csproj"));
                        SolutionFileManipulation.AddProjectToSolution(settings, projectFileName);
                    },
                    replacements
                );
            });
        }
    }
}