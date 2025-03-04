using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Moryx.Cli.CommandLine
{

    [Description("Updates the source repository defined by the provided remote.")]
    internal class Pull : Command<PullSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] PullSettings settings)
        {
            var options = new Remotes.PullOptions
            {
                Name = settings.Remote,
            };

            return Remotes.Pull.Remote(options, AnsiConsole.WriteLine)
                .ProcessResult();
        }
    }
}
