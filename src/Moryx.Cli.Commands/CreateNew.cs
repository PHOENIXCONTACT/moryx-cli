using Moryx.Cli.Commands.Options;
using Moryx.Cli.Templates.Extensions;
using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates;

namespace Moryx.Cli.Commands
{
    public static class CreateNew
    {
        public static CommandResult Solution(NewOptions options, Action<string> onStatus)
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

            return CommandBase.Exec(settings, () =>
            {
                string errorMessage = "";
                var configuration = TemplateConfigurationFactory.Load(settings.SourceDirectory, error =>
                {
                    errorMessage = error;
                });
                if (configuration == null)
                    return CommandResult.WithError(errorMessage);

                var template = Template.Load(settings, configuration);

                Directory.CreateDirectory(solutionName);
                CreateBareSolution(template);
                config.Save(dir);

                var results = new List<CommandResult>();
                if ((options.Steps ?? "").Any())
                {
                    results.Add(AddSteps.Exec(template, options.Steps!.ToCleanList()));
                }

                if ((options.Products ?? "").Any())
                {
                    results.Add(AddProducts.Exec(template, options.Products!.ToCleanList()));
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

        private static void CreateBareSolution(Template template)
        {
            var patterns = template.ReplaceVariables(template.Configuration.New);
            var dictionary = template.NewProject();

            var files = template.WriteFilesToDisk(dictionary);
            Template.ReplacePlaceHoldersInsideFiles(files, patterns);
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