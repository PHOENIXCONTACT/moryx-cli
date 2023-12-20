using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Moryx.Cli.Commands.Options;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Moryx.Cli.CommandLine
{

    [Description("Creates a new MORYX application project <NAME>.")]
    internal class New : Command<New.Settings>
    {
        internal class Settings : CommandSettings
        {
            [Description("Name of the project. Recommended to be PascalCase.")]
            [CommandArgument(0, "<NAME>")]
            public string? Name { get; set; }

            [Description("A comma separated list of products to be created with the application project.")]
            [CommandOption("-p|--products")]
            public string? Products { get; set; }

            [Description("A comma separated list of steps to be created with the application project.")]
            [CommandOption("-s|--steps")]
            public string? Steps { get; set; }

            [Description("Prevents the command from initializing a Git repository")]
            [CommandOption("--no-git-init")]
            public bool NoGitInit { get; set; }

            [Description("A Git repository url that will be used for the project template.")]
            [CommandOption("-t|--template-url"), DefaultValue("https://github.com/PHOENIXCONTACT/MORYX-Template.git")]
            public string? Template { get; set; }

            [Description("Branch to use with the template repository.")]
            [CommandOption("-b|--branch"), DefaultValue("machine")]
            public string? Branch { get; set; }

            [Description("Update the template repository.")]
            [CommandOption("--pull"), DefaultValue(false)]
            public bool Pull { get; set; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            var options = new NewOptions
            {
                Name = settings.Name,
                Products = settings.Products,
                Steps = settings.Steps,
                NoGitInit = settings.NoGitInit,
                Template = settings.Template,
                Branch = settings.Branch,
                Pull = settings.Pull,
            };



            return Commands.CreateNew.Solution(options, msg => AnsiConsole.WriteLine(msg))
                .ProcessResult();
        }
    }
}
