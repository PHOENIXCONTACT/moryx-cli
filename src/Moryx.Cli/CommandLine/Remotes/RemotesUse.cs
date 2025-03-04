using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Moryx.Cli.CommandLine
{

    [Description("Uses the given remote by default.")]
    internal class RemotesUse : Command<RemotesUse.Settings>
    {

        internal class Settings : CommandSettings
        {
            [Description("Name of the remote.")]
            [CommandArgument(0, "<NAME>")]
            public string? Name { get; set; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            var options = new Remotes.UseOptions
            {
                Name = settings.Name,
            };

            return Remotes.Use.Remote(options)
                .ProcessResult();
        }
    }
}
