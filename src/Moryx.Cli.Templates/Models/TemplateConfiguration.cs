namespace Moryx.Cli.Templates.Models
{
    /// <summary>
    /// A configuration that describes how the CLI should create
    /// representations of digital twins, concepts, etc.
    /// </summary>
    public class TemplateConfiguration
    {
        public string Version { get; set; } = "";

        /// <summary>
        /// Template wide placeholders
        /// </summary>
        public GlobalPlaceholders Placeholders { get; set; } = new();

        /// <summary>
        /// <see cref="NewConfiguration">
        /// </summary>
        public NewConfiguration New { get; set; } = new();

        /// <summary>
        /// List of <see cref="AddConfiguration">
        /// </summary>
        public AddConfigurations Add { get; set; } = new();
    }

    /// <summary>
    /// Definition of template wide placeholders
    /// </summary>
    public class GlobalPlaceholders
    {
        /// <summary>
        /// Placeholder to be replaced by the solution name
        /// </summary>
        public string SolutionName { get; set; } = "";
    }

    /// <summary>
    /// Pattern of files and identifiers to be copied and replaced
    /// </summary>
    public class ConfigurationPattern
    {
        /// <summary>
        /// Dictionary of strings to replace in file names and content.
        /// </summary>
        public Dictionary<string, string> Replacements { get; set; } = [];

        /// <summary>
        /// File patterns to be copied from the template to the 
        /// target project
        /// </summary>
        public List<string> Files { get; set; } = [];
    }

    public class NewConfiguration: ConfigurationPattern
    {
    }

    public class AddConfiguration: ConfigurationPattern
    {
    }

    public class AddConfigurations
    {
        public AddConfiguration Product { get; set; } = new();
        public AddConfiguration Resource { get; set; } = new();
    }
}
