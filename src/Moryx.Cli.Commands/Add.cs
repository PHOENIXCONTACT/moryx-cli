using Moryx.Cli.Templates.Extensions;
using Moryx.Cli.Commands.Options;
using Moryx.Cli.Templates.Models;
using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates;

namespace Moryx.Cli.Commands
{
    public static class Add
    {
        public static CommandResult Step(AddOptions options)
        {
            return AddThing(options, (template) => AddSteps.Exec(template, options.Name!.ToCleanList()));
        }

        public static CommandResult Products(AddOptions options)
        {
            return AddThing(options, (template) => AddProducts.Exec(template, options.Name!.ToCleanList()));
        }

        public static CommandResult Resources(AddOptions options)
        {
            return AddThing(options, (template) => AddResources.Exec(template, options.Name!.ToCleanList()));
        }

        public static CommandResult Module(AddOptions options)
        {
            return AddThing(options, (template) => AddModule.Exec(template, options.Name!));
        }

        public static CommandResult StateMachine(AddStatesOptions options)
        {
            var addOptions = new AddOptions
            {
                Branch = options.Branch,
                Name = options.ResourceName,
                Pull = options.Pull,
                Template = options.Template,
            };
            var states = options.States?
                .Split(',')
                .Select(x => x.Trim())
                .ToList() 
                ?? new List<string>();
            var transitions = options.States?
                .Split(',')
                .Select(x => x.Trim())
                .ToList()
                ?? new List<string>();
            return AddThing(addOptions, (template) => AddStates.Exec(template, options.ResourceName!, states, transitions));
        }

        private static CommandResult AddThing(AddOptions options, Func<Template, CommandResult> func)
        {
            var currentDir = Environment.CurrentDirectory;
            var errorMessage = string.Empty;
            var solutionName = Solution.GetSolutionName(currentDir, error =>
            {
                errorMessage = error;
            });

            if (string.IsNullOrEmpty(solutionName))
                return CommandResult.WithError(errorMessage);

            var dir = Path.GetFullPath(currentDir);

            var settings = ConfigExtensions.LoadSettings(dir, solutionName);
            settings.Repository = options.Template ?? settings.Repository;
            settings.Branch = options.Branch ?? settings.Branch;
            settings.Pull = options.Pull;

            var configuration = TemplateConfigurationFactory.Load(settings.SourceDirectory, error =>
            {
                errorMessage = error;
            });
            if (configuration == null)
                return CommandResult.WithError(errorMessage);

            var template = Template.Load(settings, configuration);

            return func(template);
        }
    }
}