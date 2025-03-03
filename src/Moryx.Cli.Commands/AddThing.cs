using Castle.Components.DictionaryAdapter;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Moryx.Cli.Commands.Extensions;
using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Models;

namespace Moryx.Cli.Commands
{
    public static class AddThing
    {
        /// <summary>
        /// Retrieves files of a certain category from the template and moves them as defined 
        /// in <paramref name="filenames"/> to the current project.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="config"></param>
        /// <param name="resourceNames"></param>
        /// <param name="onAddedFiles"></param>
        /// <returns></returns>
        public static CommandResult Exec(TemplateSettings settings, AddConfig config, List<string> resourceNames, Action<IEnumerable<string>>? onAddedFiles = null, StringReplacements? replacements = null)
        {
            return CommandBase.Exec(settings, (filenames) =>
            {
                var projectFilenames = filenames.InitialProjects();

                try
                {
                    if(replacements == null)
                    {
                        replacements = new StringReplacements(config);
                    }
                    var dictionary = Template.PrepareFileStructure(config.SolutionName, resourceNames, projectFilenames);

                    var files = Template.WriteFilesToDisk(
                        dictionary,
                        settings,
                        s => s.Replace(replacements.FileNamePatterns)
                    );

                    Template.ReplacePlaceHoldersInsideFiles(
                        files,
                        replacements.FileContentPatterns);

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

    public class StringReplacements
    {
        public Dictionary<string, string> FileNamePatterns { get; }
        public Dictionary<string, string> FileContentPatterns { get; }

        public StringReplacements(AddConfig config)
        {
            FileNamePatterns = CreateDictionary(config);
            FileContentPatterns = CreateDictionary(config);
        }

        public Dictionary<string, string> CreateDictionary(AddConfig config)
        {
            var result = config.ThingPlaceholders
                .ToDictionary(s => s, s => config.ThingName);

            result.TryAdd(Template.AppPlaceholder, config.SolutionName);
            return result;
        }


        public StringReplacements AddFileNamePatterns(Dictionary<string, string> patterns)
        {
            foreach (var pattern in patterns)
            {
                if (!FileNamePatterns.ContainsKey(pattern.Key))
                {
                    FileNamePatterns.Add(pattern.Key, pattern.Value);
                }
            }
            return this;
        }

        public StringReplacements AddContentPatterns(Dictionary<string, string> patterns)
        {
            foreach (var pattern in patterns)
            {
                if (!FileContentPatterns.ContainsKey(pattern.Key))
                {
                    FileContentPatterns.Add(pattern.Key, pattern.Value);
                }
            }
            return this;
        }
    }
}