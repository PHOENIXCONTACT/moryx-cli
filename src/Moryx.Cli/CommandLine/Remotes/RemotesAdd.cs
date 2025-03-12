using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Moryx.Cli.CommandLine
{

    [Description("Adds a Git remote to the local config.")]
    internal class RemotesAdd : Command<RemotesAdd.Settings>
    {

        internal class Settings : CommandSettings
        {
            [Description("Name of the remote.")]
            [CommandArgument(0, "<NAME>")]
            public string? Name { get; set; }

            [Description("URL of the remote repository.")]
            [CommandArgument(1, "<URL>")]
            public string? Url { get; set; }

            [Description("The branch to be used with for this remote.")]
            [CommandArgument(2, "<BRANCH>")]
            public string? Branch { get; set; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            var options = new Remotes.AddOptions
            {
                Name = settings.Name,
                Repository = settings.Url,
                Branch = settings.Branch,
            };

            return Remotes.Add.Remote(options)
                .ProcessResult();
        }
    }
}
