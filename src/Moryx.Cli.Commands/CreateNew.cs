using Moryx.Cli.Commands.Options;
using Moryx.Cli.Template.Extensions;
using Moryx.Cli.Template;
using Moryx.Cli.Template.Models;
using Moryx.Cli.Commands.Extensions;

namespace Moryx.Cli.Commands
{
    public static class CreateNew
    {
        public static CommandResult Solution(NewOptions options, Action<string> onStatus)
        {
            var solutionName = options.Name!;
            if (Directory.Exists(solutionName))
            {
                return CommandResult.WithError($"A directory {solutionName} already exists.");
            }
            var dir = Path.Combine(Directory.GetCurrentDirectory(), solutionName);
            var config = options.ToConfiguration();
            var settings = config.AsTemplateSettings(dir, solutionName);
            settings.Pull = options.Pull;

            return CommandBase.Exec(settings, _ =>
            {
                Directory.CreateDirectory(solutionName);
                CreateBareSolution(settings);
                config.Save(dir);

                if ((options.Steps ?? "").Any())
                {
                    AddSteps.Exec(settings, options.Steps!.ToCleanList());
                }

                if ((options.Products ?? "").Any())
                {
                    AddProducts.Exec(settings, options.Products!.ToCleanList());
                }

                if (!options.NoGitInit)
                {
                    InitializeGitRepo(settings.AppName, onStatus);
                }

                return CommandResult.IsOk($"Initialized new solution {solutionName}");
            });
        }

        private static void CreateBareSolution(TemplateSettings settings)
        {
            var cleanedResourceNames = Template.Template.GetCleanedResourceNames(settings);
            var projectFilenames = cleanedResourceNames.InitialProjects();
            var filteredResourceNames = cleanedResourceNames
                .WithoutStep()
                .WithoutProduct()
                .WithoutRecipe()
                .WithoutSetupTrigger()
                .WithoutCellSelector()
                .WithoutModule()
                ;

            var dictionary = Template.Template.PrepareFileStructure(settings.AppName, filteredResourceNames, projectFilenames);

            var files = Template.Template.WriteFilesToDisk(dictionary, settings, s => s);
            Template.Template.ReplacePlaceHoldersInsideFiles(
                files,
                new Dictionary<string, string>
                {
                    { Template.Template.AppPlaceholder, settings.AppName }
                });
        }

        private static void InitializeGitRepo(string solutionName, Action<string> onStatus)
        {
            var initialDirectory = Environment.CurrentDirectory;
            Directory.SetCurrentDirectory(Path.Combine(Environment.CurrentDirectory, solutionName));
            var result = TemplateRepository.ExecCommandLine("git init", onStatus);
            if (result == 0)
            {
                TemplateRepository.ExecCommandLine("git checkout -b main", onStatus);
                TemplateRepository.ExecCommandLine("git add --all", onStatus);
                TemplateRepository.ExecCommandLine($"git commit -am \"Initial commit for {solutionName}\"", onStatus);
                onStatus("Initialized Git repository");
            }
            else
            {

                onStatus("Could not initialize Git repository! Please, check your Git installation.");
            }
            Directory.SetCurrentDirectory(initialDirectory);
        }
    }
}