using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Template;
using Moryx.Cli.Template.Models;

namespace Moryx.Cli.Commands
{
    public static class AddThing
    {
        public static CommandResult Exec(TemplateSettings settings, AddConfig config, List<string> resourceNames, Action<IEnumerable<string>>? onAddedFiles = null)
        {
            return CommandBase.Exec(settings, (filenames) =>
            {
                var projectFilenames = filenames.InitialProjects();

                try
                {
                    var dictionary = Template.Template.PrepareFileStructure(config.SolutionName, resourceNames, projectFilenames);

                    var files = Template.Template.WriteFilesToDisk(
                        dictionary,
                        settings,
                        s => s.Replace(config.ThingPlaceholders, config.ThingName).Replace(Template.Template.AppPlaceholder, config.SolutionName));

                    var replaceHolders = config.ThingPlaceholders
                        .ToDictionary(s => s, s => config.ThingName);
                    replaceHolders.Add(Template.Template.AppPlaceholder, config.SolutionName);

                    Template.Template.ReplacePlaceHoldersInsideFiles(
                        files,
                        replaceHolders);

                    onAddedFiles?.Invoke(files);
                }
                catch (Exception ex)
                {
                    return CommandResult.WithError($"Failed to add {config.Thing} `{config.ThingName}`!\n" + ex.Message);
                }

                return CommandResult.IsOk($"Successfully added {config.Thing} `{config.ThingName}`");
            });
        }
    }

    public class AddConfig
    {
        /// <summary>
        /// Name of the solution file (<SolutionName>.sln)
        /// </summary>
        public required string SolutionName { get; set; }

        /// <summary>
        /// Type of this thing, like `module`. Used for displaying user outputs
        /// </summary>
        public required string Thing { get; set; }

        /// <summary>
        /// Actual name identifiere of the *thing* to be added
        /// </summary>
        public required string ThingName { get; set; }

        /// <summary>
        /// *Thing*s placeholder
        /// </summary>
        public required IEnumerable<string> ThingPlaceholders { get; set; }
    }
}