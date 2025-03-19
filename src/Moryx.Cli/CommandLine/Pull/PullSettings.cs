using System.ComponentModel;
using Spectre.Console.Cli;

namespace Moryx.Cli.CommandLine
{
    internal class PullSettings : CommandSettings
    {
        [Description("Name of the remote to be updated.")]
        [CommandArgument(0, "[REMOTE]")]
        public string? Remote { get; set; }
    }
}
