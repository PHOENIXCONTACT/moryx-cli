using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Moryx.Cli.Remotes;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Moryx.Cli.CommandLine
{

    [Description("Removes Git remotes.")]
    internal class RemotesRemove : Command<RemotesRemove.Settings>
    {

        internal class Settings : CommandSettings
        {
            [Description("Name of the remote to be removed.")]
            [CommandArgument(0, "<NAME>")]
            public string? Name { get; set; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            var options = new RemoveOptions
            {
                Name = settings.Name,
            };

            return Remove.Remote(options, AnsiConsole.WriteLine)
                .ProcessResult();
        }
    }
}
