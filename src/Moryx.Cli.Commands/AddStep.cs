using Moryx.Cli.Commands.Components;
using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;

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
                };
                var namespacePlaceholder = template.ReplaceVariables(template.Configuration.Add.Step, step);

                return AddThing.Exec(
                    template,
                    addConfig,
                    template.Step(step),
                    (createdFiles) =>
                    {
                        createdFiles.AddProjectsToSolution(template.Settings);
                        AddProjectsTests(createdFiles);
                        AddProjectsStartupApplication(createdFiles, template.Settings);
                    },
                    namespacePlaceholder
                );
            });
        }

        private static void AddProjectsStartupApplication(IEnumerable<string> createdFiles, TemplateSettings settings)
        {
            var projectFiles = createdFiles
               .Where(f => f.EndsWith(".csproj"))
               .ToList();

            var applicationProjectFile = Path.Combine(settings.TargetDirectory, "src", settings.AppName + ".App", settings.AppName + ".App.csproj");

            foreach (var project in projectFiles)
            {
                ProjectFileManipulation.AddProjectReference(applicationProjectFile, project);
            }
        }

        private static void AddProjectsTests(IEnumerable<string> createdFiles)
        {
            var projectFiles = createdFiles
               .Where(f => f.EndsWith(".csproj"))
               .ToList();
            var testFiles = createdFiles
                .Where(f => f.ToLower().EndsWith("tests.cs") || f.ToLower().EndsWith("test.cs"))
                .Select(Path.GetFullPath)
                .Distinct()
                .Select(p => Directory.GetFiles(Path.Combine(Path.GetDirectoryName(p) ?? "")))
                .SelectMany(p => p)
                .Where(f => f.EndsWith(".csproj"))
                .ToList()
                ?? [];

            foreach (var testFile in testFiles)
            {
                foreach (var project in projectFiles)
                {
                    ProjectFileManipulation.AddProjectReference(testFile, project);
                }
            }
        }
    }
}