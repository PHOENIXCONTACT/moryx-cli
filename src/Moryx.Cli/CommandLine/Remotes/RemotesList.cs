using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Moryx.Cli.Remotes;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Moryx.Cli.CommandLine
{

    [Description("Lists all configured remotes.")]
    internal class RemotesList : Command<RemotesList.Settings>
    {
        internal class Settings : CommandSettings
        {
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            return List.Get(message => AnsiConsole.WriteLine("{0,-18}{1}", message.Split('\t')))
                .ProcessResult();
        }
    }
}
