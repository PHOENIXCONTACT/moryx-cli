using System.ComponentModel;

namespace Moryx.Cli.Commands.Options
{
    public class AddOptions
    {
        public string? Name { get; set; }

        public string? Template { get; set; }

        public string? Branch { get; set; }

        public bool Pull { get; set; }
    }
}