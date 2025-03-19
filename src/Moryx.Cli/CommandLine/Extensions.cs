using Moryx.Cli.Commands;
using Spectre.Console;

namespace Moryx.Cli.CommandLine
{
    internal static class CliExtensions
    {
        internal static int ProcessResult(this CommandResult result)
            => result
                .OnSuccess((msg, warning) => AnsiConsole.Markup($"[green]{msg}[/]{LineBreakIfNotEmpty(msg)}[yellow]{warning}[/]"))
                .OnError(msg => AnsiConsole.Markup($"[red]Error: [/]{msg}"))
                .ReturnValue();

        private static string LineBreakIfNotEmpty(string s)
            => s == string.Empty
                ? ""
                : "\n";
    }
}
