using Moryx.Cli.Commands;
using Spectre.Console;

namespace Moryx.Cli.CommandLine
{
    internal static class CliExtensions
    {
        internal static int ProcessResult(this CommandResult result)
            => result
                .OnSuccess((msg, warning) => AnsiConsole.Markup($"[green]{msg}[/]\n[yellow]{warning}[/]"))
                .OnError(msg => AnsiConsole.Markup($"[red]Error: [/]{msg}"))
                .ReturnValue();
    }
}
