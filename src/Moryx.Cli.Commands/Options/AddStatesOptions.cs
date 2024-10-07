using System.ComponentModel;

namespace Moryx.Cli.Commands.Options
{
    public class AddStatesOptions
    {
        public string? ResourceName { get; set; }

        public string? States { get; set; }
        public string? Transitions { get; set; }

        public string? Template { get; set; }

        public string? Branch { get; set; }

        public bool Pull { get; set; }
    }
}