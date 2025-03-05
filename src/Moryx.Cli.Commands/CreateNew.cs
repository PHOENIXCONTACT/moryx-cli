using Moryx.Cli.Commands.Options;
using Moryx.Cli.Templates.Extensions;
using Moryx.Cli.Templates.Models;
using System.Diagnostics.CodeAnalysis;
using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates;

namespace Moryx.Cli.Commands
{
    public static class CreateNew
    {
        public static CommandResult Solution(NewOptions options, [NotNull] Action<string> onStatus)
        {
            var solutionName = options.Name!;
            var dir = Path.Combine(Directory.GetCurrentDirectory(), solutionName);

            if (Directory.Exists(solutionName))
            {
                if (options.Force)
                {
                    try
                    {
                        Directory.Delete(dir, true);
                    }
                    catch (Exception ex)
                    {
                        return CommandResult.WithError(ex.Message);
                    }
                }
                else
                {
                    return CommandResult.WithError($"A directory {solutionName} already exists.");
                }
            }
            var config = options.ToConfiguration();
            var settings = config.AsTemplateSettings(dir, solutionName);
            settings.Pull = options.Pull;

            return CommandBase.Exec(settings, (filenames) =>
            {
                Directory.CreateDirectory(solutionName);
                CreateBareSolution(settings);
                config.Save(dir);

                var results = new List<CommandResult>();
                if ((options.Steps ?? "").Any())
                {
                    results.Add(AddSteps.Exec(settings, options.Steps!.ToCleanList()));
                }

                if ((options.Products ?? "").Any())
                {
                    results.Add(AddProducts.Exec(settings, options.Products!.ToCleanList()));
                }

                if (!options.NoGitInit)
                {
                    InitializeGitRepo(settings.AppName, onStatus);
                }

                return CommandResult
                    .IsOk($"Initialized new solution {solutionName}")
                    .CouldHaveIssues(results);
            });
        }

        private static void CreateBareSolution(TemplateSettings settings)
        {
            var cleanedResourceNames = Template.GetCleanedResourceNames(settings);
            var projectFilenames = cleanedResourceNames.InitialProjects();
            var filteredResourceNames = FilteredFileNames(settings.SourceDirectory, cleanedResourceNames, new());

            var dictionary = Template.PrepareFileStructure(settings.AppName, filteredResourceNames, projectFilenames);

            var files = Template.WriteFilesToDisk(dictionary, settings, s => s);
            Template.ReplacePlaceHoldersInsideFiles(
                files,
                new Dictionary<string, string>
                {
                    { Template.AppPlaceholder, settings.AppName }
                });
        }

        public static List<string> FilteredFileNames(string root, List<string> resourceNames, TemplateConfiguration templateConfig)
        {
            return resourceNames
                .FilterByPattern(root, templateConfig.New)
                ;
        }

        private static void InitializeGitRepo(string solutionName, Action<string> onStatus)
        {
            var initialDirectory = Environment.CurrentDirectory;
            Directory.SetCurrentDirectory(Path.Combine(Environment.CurrentDirectory, solutionName));
            var result = TemplateRepository.ExecCommanLine("git init", onStatus);
            if (result == 0)
            {
                TemplateRepository.ExecCommanLine("git checkout -b main", onStatus);
                TemplateRepository.ExecCommanLine("git add --all", onStatus);
                TemplateRepository.ExecCommanLine($"git commit -am \"Initial commit for {solutionName}\"", onStatus);
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