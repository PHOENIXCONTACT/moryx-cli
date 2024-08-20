using Moryx.Cli.Template.Extensions;
using Moryx.Cli.Commands.Options;
using Moryx.Cli.Template.Models;
using Moryx.Cli.Commands.Extensions;

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

        private static CommandResult AddThing(AddOptions options, Func<TemplateSettings, CommandResult> func)
        {
            var currentDir = Environment.CurrentDirectory;
            var solutionNameError = string.Empty;
            var solutionName = Template.Template.GetSolutionName(currentDir, error =>
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