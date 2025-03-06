using Moryx.Cli.Templates.Extensions;

namespace Moryx.Cli.Templates.Models
{
    public class TemplateSettings
    {
        public required string Branch { get; set; }
        public required string Repository { get; set; }
        public bool Pull { get; set; }
        public required string AppName { get; set; }
        public string ProfileName { get; set; } = "default";
        public required string TargetDirectory { get; set; }
        public virtual string SourceDirectory { get => Path.Combine(TemplatesRoot(), Repository.AsFolderName(), Branch); }

        public TemplateSettings()
        {
        }

        public string TemplatesRoot()
           => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Moryx.Cli");

    }
}
