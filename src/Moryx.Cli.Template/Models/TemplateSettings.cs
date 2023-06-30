namespace Moryx.Cli.Template.Models
{
    public class TemplateSettings
    {
        public string Branch { get; set; }
        public string Repository { get; set; }
        public bool Pull { get; set; }
        public string AppName { get; set; }
        public string ProfileName { get; set; } = "default";
        public string TargetDirectory { get; set; }
        public string SourceDirectory { get => Path.Combine(TemplatesRoot(), ProfileName, Branch); }


        public string TemplatesRoot()
           => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Moryx.Cli");

    }
}
