using System.ComponentModel;

namespace Moryx.Cli.Commands.Options
{
    public class NewOptions
    {
        public string? Name { get; set; }

        public string? Steps { get; set; }

        public string? Products { get; set; }

        [DefaultValue(false)]
        public bool NoGitInit { get; set; }

        public string? Template { get; set; }

        public string? Branch { get; set; }

        [DefaultValue(false)]
        public bool Pull { get; set; }
    }
}