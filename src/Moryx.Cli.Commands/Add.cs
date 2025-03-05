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
            return AddThing(options, (settings) => AddSteps.Exec(settings, options.Name!.ToCleanList()));
        }

        public static CommandResult Products(AddOptions options)
        {
            return AddThing(options, (settings) => AddProducts.Exec(settings, options.Name!.ToCleanList()));
        }

        public static CommandResult Resources(AddOptions options)
        {
            return AddThing(options, (settings) => AddResources.Exec(settings, options.Name!.ToCleanList()));
        }

        public static CommandResult Module(AddOptions options)
        {
            return AddThing(options, (settings) => AddModule.Exec(settings, options.Name!));
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
            return AddThing(addOptions, (settings) => AddStates.Exec(settings, options.ResourceName!, states, transitions));
        }

        private static CommandResult AddThing(AddOptions options, Func<TemplateSettings, CommandResult> func)
        {
            var currentDir = Environment.CurrentDirectory;
            var solutionNameError = string.Empty;
            var solutionName = Template.GetSolutionName(currentDir, error =>
            {
                Console.WriteLine(error);
                solutionNameError = error;
            });

            if (string.IsNullOrEmpty(solutionName))
                return CommandResult.WithError(solutionNameError);

            var dir = Path.GetFullPath(currentDir);

            var settings = ConfigExtensions.LoadSettings(dir, solutionName);
            settings.Repository = options.Template ?? settings.Repository;
            settings.Branch = options.Branch ?? settings.Branch;
            settings.Pull = options.Pull;

            return func(settings);
        }
    }
}