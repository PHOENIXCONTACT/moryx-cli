using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Moryx.Cli.Commands.Options;
using Spectre.Console.Cli;

namespace Moryx.Cli.CommandLine
{
    internal class AddSettings : CommandSettings
    {
        [Description("A Git repository url that will be used for the project template.")]
        [CommandOption("-t|--template-url")]
        public string? Template { get; set; }

        [Description("Branch to use with the template repository.")]
        [CommandOption("-b|--branch")]
        public string? Branch { get; set; }

        [Description("Update the template repository.")]
        [CommandOption("--pull"), DefaultValue(false)]
        public bool Pull { get; set; }

    }


    [Description("Adds a product to your MORYX solution.")]
    internal class AddProduct : Command<AddProduct.AddProductSettings>
    {
        internal class AddProductSettings : AddSettings
        {
            [Description("Name of the product to be added")]
            [CommandArgument(0, "<NAME>")]
            public string? Name { get; set; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] AddProductSettings settings)
        {
            var options = new AddOptions
            {
                Name = settings.Name,
                Template = settings.Template,
                Branch = settings.Branch,
                Pull = settings.Pull,
            };

            return Commands.Add.Products(options)
                .ProcessResult();
        }
    }

    [Description("Adds a step to your MORYX solution.")]
    internal class AddStep : Command<AddStep.AddStepSettings>
    {
        internal class AddStepSettings : AddSettings
        {
            [Description("Name of the step to be added")]
            [CommandArgument(0, "<NAME>")]
            public string? Name { get; set; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] AddStepSettings settings)
        {
            var options = new AddOptions
            {
                Name = settings.Name,
                Template = settings.Template,
                Branch = settings.Branch,
                Pull = settings.Pull,
            };

            return Commands.Add.Step(options)
                .ProcessResult();
        }
    }

    [Description("Adds a module|adapter to your MORYX solution.")]
    internal class AddModule : Command<AddModule.AddModuleSettings>
    {
        internal class AddModuleSettings : AddSettings
        {
            [Description("Name of the module to be added")]
            [CommandArgument(0, "<NAME>")]
            public string? Name { get; set; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] AddModuleSettings settings)
        {
            var options = new AddOptions
            {
                Name = settings.Name,
                Branch = settings.Branch,
                Template = settings.Template,
                Pull = settings.Pull
            };

            return Commands.Add.Module(options)
                .ProcessResult();
        }
    }
}
