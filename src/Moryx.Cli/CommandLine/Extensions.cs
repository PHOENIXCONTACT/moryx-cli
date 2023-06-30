using Moryx.Cli.Commands;
using Spectre.Console;

namespace Moryx.Cli.CommandLine
{
    internal static class CliExtensions
    {
        internal static int ProcessResult(this CommandResult result)
            => result
                .OnSuccess(msg => AnsiConsole.Markup($"[green]{msg}[/]"))
                .OnError(msg => AnsiConsole.Markup($"[red]Error: [/]{msg}"))
                .ReturnValue();
    }
}
