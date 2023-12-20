using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console.Cli;
using Moryx.Cli.Commands.Options;

namespace Moryx.Cli.CommandLine
{

    [Description("Executes a command against a running MORYX instance.")]
    internal class Exec : Command<Exec.Settings>
    {
        internal class Settings : CommandSettings
        {
            [Description("Command to be executed agains a running MORYX instance")]
            [CommandArgument(0, "<COMMAND>")]
            public string? Command { get; set; }

            [Description("Endpoint URL of the running MORYX instance")]
            [CommandOption("--endpoint")]
            [DefaultValue("https://localhost:5000")]
            public string? Endpoint { get; set; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            var options = new ExecOptions
            {

                Command = settings.Command,
                Endpoint = settings.Endpoint,
            };

            return Commands.ExecCommand.Exec(options)
                .ProcessResult();
        }
    }
}
